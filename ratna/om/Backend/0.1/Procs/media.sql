/*
    Copyright (c) 2012, Jardalu LLC. (http://jardalu.com)
        
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
  
    For complete licensing, see license.txt or visit http://ratnazone.com/v0.2/license.txt

*/
----------------------------------------------------------
-- Proc_Ratna_Media_GetByUrl
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Media_GetByUrl') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Media_GetByUrl
GO

CREATE PROCEDURE Proc_Ratna_Media_GetByUrl
    @SiteId           INT,
    @Url              NVARCHAR(512),
    @ErrorCode        BIGINT OUTPUT
AS
    DECLARE @ResourceId     BIGINT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    SELECT 
            @ResourceId = ResourceId
        FROM 
            Tbl_Ratna_Media
        WHERE
            SiteId = @SiteId AND
            Url = @Url

    SELECT * 
        FROM 
            Tbl_Ratna_Media
        WHERE 
            SiteId = @SiteId AND
            Url = @Url

    -- check if anything found
    IF (@@ROWCOUNT = 0)
    BEGIN
        SET @ErrorCode = 1003
    END
    ELSE
    BEGIN
        
        -- select the tags for the media
        SELECT 
                ResourceId,
                Tag as Name,
                Weight
            FROM 
                Tbl_Ratna_Tags
            WHERE 
                SiteId = @SiteId AND
                ResourceId = @ResourceId

    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Media_Save
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Media_Save') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Media_Save
GO

CREATE PROCEDURE Proc_Ratna_Media_Save
    @SiteId                 INT,
    @MediaType        INT,
    @Name             NVARCHAR(256),
    @Url              NVARCHAR(512),
    @OwnerId          BIGINT,
    @RawData          NTEXT,
    @ErrorCode        BIGINT    OUTPUT
AS
    DECLARE @Success            BIT,
            @Id                 BIGINT,
            @ResourceId         BIGINT,
            @PrincipalId        BIGINT,
            @ExistingMediaType  INT
BEGIN

    SET NOCOUNT ON
    
    -- default initialization
    SET @ErrorCode = 0
    SET @Success = 1
    SET @Id = 0

    BEGIN TRAN

    SELECT 
            @ExistingMediaType = MediaType,
            @Id = Id
        FROM 
            Tbl_Ratna_Media 
        WHERE 
            SiteId = @SiteId AND
            [Url] = @Url

    -- does not exist
    IF ( @@ROWCOUNT = 0 )
    BEGIN

        -- generate a new resource id. resource-id mapping is done with principal-id.
        SELECT @PrincipalId = PrincipalId FROM Tbl_Ratna_User WHERE SiteId = @SiteId AND Id = @OwnerId

        INSERT INTO Tbl_Ratna_Resource ( SiteId, PrincipalId ) VALUES ( @SiteId, @PrincipalId )
        SET @ResourceId = @@Identity

        -- simple most case, media never existed.
        INSERT INTO Tbl_Ratna_Media ( SiteId, ResourceId, MediaType, Name, Url, OwnerId, RawData ) 
            VALUES ( @SiteId, @ResourceId, @MediaType, @Name, @Url, @OwnerId, @RawData )

        -- check if there was actually an insert
        IF (@@ROWCOUNT <> 1)
        BEGIN

            SET @Success = 0
            SET @ErrorCode = 1001

        END
        ELSE
        BEGIN

            -- get the id for the created article
            SET @Id = @@Identity

        END

    END
    ELSE
    BEGIN

        -- for a media, mediatype/url cannot be updated once it has been set.
        IF ( @ExistingMediaType <> @MediaType )
        BEGIN
             SET @Success = 0
             SET @ErrorCode = 1002
        END
        ELSE
        BEGIN
            UPDATE 
                    Tbl_Ratna_Media
                SET 
                    Name = @Name,
                    RawData = @RawData,
                    LastModifiedDate = GETDATE()
                WHERE 
                    SiteId = @SiteId AND
                    [Url] = @Url
        END

    END

    IF (@Success = 0)
    BEGIN
        ROLLBACK TRAN
    END
    ELSE
    BEGIN
        COMMIT TRAN
    END

    -- select the media data
    IF ( @Id <> 0)
    BEGIN
        SELECT * 
            FROM 
                Tbl_Ratna_Media
            WHERE
                SiteId = @SiteId AND
                Id = @Id
    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_Media_GetList
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Media_GetList') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Media_GetList
GO

CREATE PROCEDURE Proc_Ratna_Media_GetList
    @SiteId             INT,
    @Query              NVARCHAR(80),
    @MediaType          INT,
    @OwnerId            BIGINT,
    @Start              INT,
    @Count              INT,
    @TagKey             UNIQUEIDENTIFIER,
    @Records            INT OUTPUT,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    -- default outs
    SET @Records = 0
    SET @ErrorCode = 0

    DECLARE @LikeQuery NVARCHAR(82),
            @Success   BIT

    -- table that keeps all the matched articles. This table is used to find the size
    -- of the query.
    DECLARE @MatchedMedia TABLE
        (
            ResourceId    BIGINT
        )

    -- selected articles based on start/size criteria
    DECLARE @SelectedMedia TABLE
        (
            Id                    BIGINT                NOT NULL,
            ResourceId            BIGINT                NOT NULL,
            MediaType             INT                   NOT NULL,
            Name                  NVARCHAR(256)         NOT NULL,
            Url                   NVARCHAR(512)         NOT NULL    UNIQUE,
            OwnerId               BIGINT                NOT NULL,
            RawData               NTEXT,
            CreatedDate           DATETIME              NOT NULL,
            LastModifiedDate      DATETIME              NOT NULL
        )
            
    SET @Records = 0
    SET @Success = 1
    SET @LikeQuery = '%' + @Query + '%'

    IF (@query = '')
    BEGIN

        -- no tag search
        INSERT INTO @MatchedMedia
            SELECT 
                    Id
                FROM 
                    TVF_Ratna_Media(@SiteId, @MediaType) as Media
                WHERE 
                    (@OwnerId = -1 OR Media.OwnerId = @OwnerId)
    END
    ELSE
    BEGIN

        -- find all the articles that has the corresponding tags
        INSERT INTO @MatchedMedia
            SELECT 
                    DISTINCT Media.Id 
                FROM 
                    Tbl_Ratna_Tags
                INNER JOIN
                    Tbl_Ratna_TagKeys
                ON
                    Tbl_Ratna_TagKeys.Id = Tbl_Ratna_Tags.KeyId
                INNER JOIN
                    TVF_Ratna_Media(@SiteId, @MediaType) as Media
                ON
                    Media.ResourceId = Tbl_Ratna_Tags.ResourceId
                WHERE 
                    Tbl_Ratna_Tags.SiteId = @SiteId AND
                    Tbl_Ratna_TagKeys.SiteId = @SiteId AND
                    Tag Like @LikeQuery AND
                    (@OwnerId = -1 OR Media.OwnerId = @OwnerId)

        -- select media with the name
        INSERT INTO @MatchedMedia
            SELECT Media.Id
                FROM 
                    TVF_Ratna_Media(@SiteId, @MediaType) as Media
                WHERE
                    Name Like @LikeQuery AND
                    Media.Id NOT IN ( SELECT ResourceId FROM @MatchedMedia )

    END

    -- sort media based on last-modified date
    ;WITH SortedMedia(RowNumber, Id, MediaType, Name, Url, OwnerId, RawData, CreatedDate, LastModifiedDate) AS
    (    
        SELECT ROW_NUMBER() OVER(ORDER BY LastModifiedDate DESC) as RowNumber, 
                   Id, MediaType, Name, Url, OwnerId, RawData, CreatedDate, LastModifiedDate
                FROM 
                    Tbl_Ratna_Media
                INNER JOIN @MatchedMedia MatchedMedia
                    ON MatchedMedia.ResourceId = Tbl_Ratna_Media.Id
    )

    -- select media with the matched tags
    INSERT INTO @SelectedMedia
        SELECT *
            FROM 
                SortedMedia
            WHERE
                RowNumber >= @Start AND
                RowNumber < @Start + @Count

    SELECT *
        FROM @SelectedMedia

    -- select tags of the media
    SELECT 
            ResourceId,
            Tag as Name,
            Weight
        FROM 
            Tbl_Ratna_Tags
        INNER JOIN
            Tbl_Ratna_TagKeys
        ON
            Tbl_Ratna_TagKeys.Id = Tbl_Ratna_Tags.KeyId
        WHERE 
            Tbl_Ratna_Tags.SiteId = @SiteId AND
            Tbl_Ratna_TagKeys.SiteId = @SiteId AND
            [Key] = @TagKey AND
            ResourceId IN (SELECT Id FROM @SelectedMedia)
            
    SELECT @Records = COUNT(ResourceId)
        FROM @MatchedMedia
        
    IF (ISNULL(@Records,0) = 0)
    BEGIN
        SET @Records = 0
        SET @Success = 0
    END    
        
    --update the search records
    IF ((@Query <> '') OR (ISNULL(@Query,0) <>0))
    BEGIN
        DECLARE @ResolvedMediaType INT
        SET @ResolvedMediaType = 2 + @MediaType
        EXEC Proc_Ratna_AddSearchQuery @SiteId, @Query, @ResolvedMediaType , @Success
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Media_Delete
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Media_Delete') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Media_Delete
GO

CREATE PROCEDURE Proc_Ratna_Media_Delete
    @SiteId           INT,
    @Url              NVARCHAR(512),
    @ErrorCode        BIGINT OUTPUT
AS
    DECLARE  @ResourceId        BIGINT
BEGIN

    SET NOCOUNT ON
    
    SET @ErrorCode = 0
    SET @ResourceId = 0

    SELECT 
            @ResourceId = ResourceId
        FROM 
            Tbl_Ratna_Media
        WHERE 
            SiteId = @SiteId AND
            Url = @Url

    IF (@@ROWCOUNT = 1)
    BEGIN TRAN

        DELETE 
            FROM 
                Tbl_Ratna_Media
            WHERE 
                SiteId = @SiteId AND
                ResourceId = @ResourceId

        DELETE 
            FROM 
                Tbl_Ratna_Acls
            WHERE 
                SiteId = @SiteId AND
                ResourceId = @ResourceId

        DELETE 
            FROM 
                Tbl_Ratna_Resource
            WHERE 
                SiteId = @SiteId AND 
                Id = @ResourceId

    COMMIT TRAN

END
GO
