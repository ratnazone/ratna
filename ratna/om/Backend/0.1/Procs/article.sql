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
-- Proc_Ratna_Article_GetByUrl
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_GetByUrl') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_GetByUrl
GO

CREATE PROCEDURE Proc_Ratna_Article_GetByUrl
    @SiteId           INT,
    @UrlKey           NVARCHAR(256),
    @Stage            INT,
    @ErrorCode        BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    
    DECLARE @MaxVersion INT,
            @ResourceId BIGINT

    SET @ErrorCode = 0
    SET @MaxVersion = 0

    -- choose the highest number for draft.
    IF (@Stage = 0)
    BEGIN

        SELECT @MaxVersion = MAX(Version)
            FROM 
                Tbl_Ratna_Article
            WHERE
                SiteId = @SiteId AND
                UrlKey = @UrlKey
            GROUP 
                BY UrlKey

        SELECT @ResourceId = ResourceId
            FROM 
                Tbl_Ratna_Article
            WHERE
                SiteId = @SiteId AND
                UrlKey = @UrlKey AND
                Version = @MaxVersion

        SELECT * 
            FROM 
                Tbl_Ratna_Article
            WHERE
                SiteId = @SiteId AND 
                UrlKey = @UrlKey AND
                Version = @MaxVersion

    END
    ELSE
    BEGIN

        SELECT 
                @ResourceId = ResourceId
            FROM 
                Tbl_Ratna_Article
            WHERE
                SiteId = @SiteId AND
                UrlKey = @UrlKey AND
                Stage = @Stage

        SELECT * 
            FROM 
                Tbl_Ratna_Article
            WHERE
                SiteId = @SiteId AND
                UrlKey = @UrlKey AND
                Stage = @Stage

    END

    IF (@@ROWCOUNT <> 1)
    BEGIN
        SET @ErrorCode = 1002
    END
    BEGIN

        -- select the tags for the article
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
-- Proc_Ratna_Article_GetByUrlVersion
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_GetByUrlVersion') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_GetByUrlVersion
GO

CREATE PROCEDURE Proc_Ratna_Article_GetByUrlVersion
    @SiteId           INT,
    @UrlKey           NVARCHAR(256),
    @Version          INT,
    @ErrorCode        BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    
    DECLARE @ResourceId BIGINT

    SET @ErrorCode = 0

    SELECT 
            @ResourceId = ResourceId
        FROM 
            Tbl_Ratna_ArticleArchive
        WHERE
            SiteId = @SiteId AND
            UrlKey = @UrlKey AND
            Version = @Version

    SELECT * 
        FROM 
            Tbl_Ratna_ArticleArchive
        WHERE
            SiteId = @SiteId AND
            UrlKey = @UrlKey AND
            Version = @Version


    IF (@@ROWCOUNT <> 1)
    BEGIN
        SET @ErrorCode = 1002
    END
    BEGIN

        -- select the tags for the article
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
-- Proc_Ratna_Article_Save
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_Save') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_Save
GO

CREATE PROCEDURE Proc_Ratna_Article_Save
    @SiteId                 INT,
    @HandlerId              UNIQUEIDENTIFIER,
    @Title                  NVARCHAR(256),
    @UrlKey                 NVARCHAR(256),
    @OwnerId                BIGINT,
    @RawData                NTEXT,
    @ReadVersion            INT,
    @UpdateVersion          BIT,
    @ErrorCode              BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    DECLARE @Success        BIT,
            @Id             BIGINT,
            @PrincipalId    BIGINT,
            @Version        INT,
            @ResourceId     BIGINT,
            @Stage          INT

    -- default initialization
    SET @ErrorCode = 0
    SET @Success = 1
    SET @Id = 0

    BEGIN TRAN

    IF NOT EXISTS (SELECT 1 FROM Tbl_Ratna_Article WHERE SiteId = @SiteId AND UrlKey = @UrlKey)
    BEGIN
    
        -- generate a resource id
        -- resource has the principal id
        SELECT @PrincipalId = PrincipalId FROM Tbl_Ratna_User WHERE SiteId = @SiteId AND Id = @OwnerId

        INSERT INTO Tbl_Ratna_Resource ( SiteId, PrincipalId ) VALUES ( @SiteId, @PrincipalId )
        SET @ResourceId = @@Identity

        -- simple most case, the article never existed
        INSERT INTO Tbl_Ratna_Article ( SiteId, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData )
            VALUES ( @SiteId, @ResourceId, @HandlerId, @Title, @UrlKey, @OwnerId, @RawData )

        IF (@@ROWCOUNT <> 1)
        BEGIN

            SET @Success = 0
            SET @ErrorCode = 1004

        END
        ELSE
        BEGIN

            -- get the id for the created article
            SET @Id = @@Identity

        END

    END
    ELSE
    BEGIN

        -- if the original version is zero, must ensure not to override existing article.
        IF ( @ReadVersion = 0 )
        BEGIN

            -- error case. already exists an article with the same url.
            SET @Success = 0
            SET @ErrorCode = 1000

        END
        ELSE
        BEGIN
        
            -- get the max version of the article
            SELECT 
                    @Version = MAX(Version) 
                FROM
                    Tbl_Ratna_Article
                WHERE
                    SiteId = @SiteId AND 
                    UrlKey = @UrlKey
                GROUP 
                    BY UrlKey

           -- make sure that the version that was read has not been updated
           IF ( @ReadVersion < @Version )
           BEGIN
            
                -- error case, can't overwrite
                SET @Success = 0
                SET @ErrorCode = 1001

           END
           ELSE
           BEGIN

                SELECT
                        @Id = Id,
                        @ResourceId = ResourceId,
                        @Stage = Stage
                    FROM
                        Tbl_Ratna_Article
                    WHERE
                        SiteId = @SiteId AND
                        UrlKey = @UrlKey AND
                        Version = @Version
                
                -- if the article is already published, bump up the version
                -- if asked for, bump up the version
                IF (( @Stage = 1 ) OR (@UpdateVersion = 1 ))
                BEGIN

                    -- archive the pervious version
                    INSERT INTO Tbl_Ratna_ArticleArchive 
                            ( SiteId, Id, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData, Version, CreatedDate, LastModifiedDate, ArchivedDate)
                        SELECT  @SiteId, Id, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData, Version, CreatedDate, LastModifiedDate, GETDATE()
                            FROM 
                                Tbl_Ratna_Article
                            WHERE
                                SiteId = @SiteId AND
                                UrlKey = @UrlKey AND
                                ResourceId = @ResourceId AND
                                Version = @Version

                    -- update the version
                    INSERT INTO Tbl_Ratna_Article (SiteId, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData , Version )
                        VALUES (@SiteId, @ResourceId, @HandlerId, @Title, @UrlKey, @OwnerId, @RawData, (@Version +1) )

                   -- reset the Id
                   SET @Id = @@Identity

                    -- delete the previous version in case the article is not published.
                    DELETE
                        FROM 
                            Tbl_Ratna_Article
                        WHERE
                            SiteId = @SiteId AND
                            UrlKey = @UrlKey AND
                            ResourceId = @ResourceId AND
                            Version = @Version AND
                            Stage = 0

                END
                ELSE
                BEGIN

                    -- overwrite the unpublished version
                    UPDATE Tbl_Ratna_Article
                        SET 
                            Title = @Title,
                            RawData = @RawData,
                            LastModifiedDate = GETDATE()
                        WHERE
                            SiteId = @SiteId AND
                            Id = @Id

                END
            END
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

    -- select the article data
    IF ( @Id <> 0)
    BEGIN
        SELECT * 
            FROM 
                Tbl_Ratna_Article
            WHERE
                SiteId = @SiteId AND
                Id = @Id
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Article_Publish
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_Publish') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_Publish
GO

CREATE PROCEDURE Proc_Ratna_Article_Publish
    @SiteId               INT,
    @UrlKey               NVARCHAR(256),
    @ErrorCode            BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    DECLARE @Success BIT
    DECLARE @MaxVersion    INT

    SET @ErrorCode = 0
    SET @Success = 1
    
    BEGIN TRAN

    SELECT 
            @MaxVersion = Max(Version)
        FROM
            Tbl_Ratna_Article
        WHERE
            SiteId = @SiteId AND
            UrlKey = @UrlKey

    IF (@@ROWCOUNT <> 1)
    BEGIN

        SET @ErrorCode = 1
        SET @Success = 0

    END
    ELSE
    BEGIN

        -- delete the last published ( it would have been already archived )
        DELETE 
            FROM 
                Tbl_Ratna_Article
            WHERE
                SiteId = @SiteId AND
                UrlKey = @UrlKey AND
                Version <> @MaxVersion

        
        -- publish the latest
        UPDATE Tbl_Ratna_Article
            SET
                Stage = 1
            WHERE
                SiteId = @SiteId AND
                UrlKey = @UrlKey AND
                Version = @MaxVersion

    END

    IF (@Success = 1)
    BEGIN
        COMMIT TRAN
    END
    ELSE
    BEGIN
        ROLLBACK TRAN
    END

END

GO
----------------------------------------------------------
-- Proc_Ratna_Article_GetList
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_GetList') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_GetList
GO

CREATE PROCEDURE Proc_Ratna_Article_GetList
    @SiteId                 INT,
    @Query              NVARCHAR(80),
    @OwnerId            BIGINT,
    @Stage              INT,
    @Start              INT,
    @Count              INT,
    @TagKey             UNIQUEIDENTIFIER,
    @HandlerId          UNIQUEIDENTIFIER,
    @Records            INT OUTPUT,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    -- default outs
    SET @Records = 0
    SET @ErrorCode = 0

    DECLARE @LikeQuery NVARCHAR(82),
            @BodyLikeQuery  NVARCHAR(97),
            @Success   BIT

    -- table that keeps all the matched articles. This table is used to find the size
    -- of the query.
    DECLARE @MatchedArticles TABLE
        (
            ResourceId    BIGINT
        )

    -- selected articles based on start/size criteria
    DECLARE @SelectedArticles TABLE
        (
            RowNumber           INT, 
            Id                  BIGINT, 
            HandlerId           UNIQUEIDENTIFIER, 
            Title               NVARCHAR(256),
            UrlKey              NVARCHAR(256),
            Version             INT, 
            Stage               INT, 
            OwnerId             BIGINT, 
            RawData             NTEXT, 
            CreatedDate         DATETIME, 
            LastModifiedDate    DATETIME, 
            PublishedDate       DATETIME
        )
            
    SET @Records = 0
    SET @Success = 1
    SET @LikeQuery = '%' + @Query + '%'
    SET @BodyLikeQuery = '%<body>%' + @Query + '%</body>%'

    IF (@query = '')
    BEGIN

        -- no tag search
        INSERT INTO @MatchedArticles   
            SELECT 
                    Id
                FROM 
                    Tbl_Ratna_Article
                WHERE
                    SiteId = @SiteId AND
                    (@HandlerId = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER) OR HandlerId = @HandlerId) AND
                    (@OwnerId = -1 OR Tbl_Ratna_Article.OwnerId = @OwnerId) AND
                    Stage = @Stage
    END
    ELSE
    BEGIN

        -- find all the articles that has the corresponding tags
        INSERT INTO @MatchedArticles
            SELECT 
                    DISTINCT Tbl_Ratna_Article.Id 
                FROM 
                    Tbl_Ratna_Tags
                INNER JOIN
                    Tbl_Ratna_TagKeys
                ON
                    Tbl_Ratna_TagKeys.Id = Tbl_Ratna_Tags.KeyId
                INNER JOIN
                    Tbl_Ratna_Article
                ON
                    Tbl_Ratna_Article.ResourceId = Tbl_Ratna_Tags.ResourceId
                WHERE
                    Tbl_Ratna_Tags.SiteId = @SiteId AND
                    Tag Like @LikeQuery AND
                    [Key] = @TagKey AND
                    (@HandlerId = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER) OR HandlerId = @HandlerId) AND
                    (@OwnerId = -1 OR Tbl_Ratna_Article.OwnerId = @OwnerId) AND
                    Stage = @Stage

        -- find all the articles that have match on title or contents
        INSERT INTO @MatchedArticles
            SELECT
                    DISTINCT Id
                 FROM
                    Tbl_Ratna_Article
                 WHERE
                    Title LIKE @LikeQuery OR
                    RawData LIKE @BodyLikeQuery AND
                    Id NOT IN ( SELECT ResourceId FROM @MatchedArticles)

    END
    

    -- sort articles based on published date
    ;WITH SortedArticles(RowNumber, Id, HandlerId, Title, UrlKey, Version, Stage, OwnerId, RawData, CreatedDate, LastModifiedDate, PublishedDate) AS
    (    
        SELECT ROW_NUMBER() OVER(ORDER BY PublishedDate DESC) as RowNumber, 
                    Id, HandlerId, Title, UrlKey, Version, Stage, OwnerId, RawData, CreatedDate, LastModifiedDate, PublishedDate
                FROM 
                    Tbl_Ratna_Article
                INNER JOIN @MatchedArticles MatchedArticles
                    ON MatchedArticles.ResourceId = Tbl_Ratna_Article.Id
                WHERE
                    SiteId = @SiteId
    )

    -- select article with the matched tags
    INSERT INTO @SelectedArticles
        SELECT *
            FROM 
                SortedArticles
            WHERE
                RowNumber >= @Start AND
                RowNumber < @Start + @Count

    SELECT *
        FROM @SelectedArticles

    -- select tags of the articles
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
            ResourceId IN (SELECT Id FROM @SelectedArticles)
            
    SELECT @Records = COUNT(ResourceId)
        FROM @MatchedArticles    
        
    IF (ISNULL(@Records,0) = 0)
    BEGIN
        SET @Records = 0
        SET @Success = 0
    END    
        
    --update the search records
    IF ((@Query <> '') OR (ISNULL(@Query,0) <>0))
    BEGIN
        EXEC Proc_Ratna_AddSearchQuery @SiteId, @Query, 1, @Success
    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_Article_GetVersions
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_GetVersions') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_GetVersions
GO

CREATE PROCEDURE Proc_Ratna_Article_GetVersions
    @SiteId           INT,
    @UrlKey           NVARCHAR(256),
    @ErrorCode        BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0

    SELECT
            Version,
            LastModifiedDate,
            ArchivedDate
        FROM 
            Tbl_Ratna_ArticleArchive
        WHERE
            SiteId = @SiteId AND
            UrlKey = @UrlKey
        ORDER 
            BY Version DESC

END
GO

----------------------------------------------------------
-- Proc_Ratna_Article_Delete
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_Delete') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_Delete
GO

CREATE PROCEDURE Proc_Ratna_Article_Delete
    @SiteId                 INT,
    @UrlKey            NVARCHAR(256),
    @ErrorCode         BIGINT OUTPUT
AS
    DECLARE  @ResourceId        BIGINT
BEGIN

    SET NOCOUNT ON
    
    SET @ErrorCode = 0
    SET @ResourceId = 0

    SELECT 
            @ResourceId = ResourceId
        FROM 
            Tbl_Ratna_Article
        WHERE 
            SiteId = @SiteId AND
            UrlKey = @UrlKey

    IF (@@ROWCOUNT = 1)
    BEGIN TRAN

        DELETE FROM Tbl_Ratna_Article
            WHERE SiteId = @SiteId AND ResourceId = @ResourceId

        DELETE FROM Tbl_Ratna_ArticleArchive
            WHERE SiteId = @SiteId AND ResourceId = @ResourceId

        DELETE FROM Tbl_Ratna_Acls
            WHERE SiteId = @SiteId AND ResourceId = @ResourceId

        DELETE FROM Tbl_Ratna_Resource
            WHERE SiteId = @SiteId AND Id = @ResourceId

    COMMIT TRAN

END
GO

----------------------------------------------------------
-- Proc_Ratna_Article_DeleteVersion
--
-- Deletes a particular version from the archive
-- current version cannot be deleted without deleting
-- the article itself.
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_DeleteVersion') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_DeleteVersion
GO

CREATE PROCEDURE Proc_Ratna_Article_DeleteVersion
    @SiteId            INT,
    @UrlKey            NVARCHAR(256),
    @Version           INT,
    @ErrorCode         BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    
    SET @ErrorCode = 0

    BEGIN TRAN

        DELETE 
            FROM 
                Tbl_Ratna_ArticleArchive
            WHERE
                SiteId = @SiteId AND
                UrlKey = @UrlKey AND
                Version = @Version

    COMMIT TRAN

END
GO

----------------------------------------------------------
-- Proc_Ratna_Article_SetTimes
--
-- sets the time for the article. this is mostly used for
-- migration purpose
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_SetTimes') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_SetTimes
GO

CREATE PROCEDURE Proc_Ratna_Article_SetTimes
    @SiteId             INT,
    @UrlKey             NVARCHAR(256),
    @CreatedDate        DATETIME,
    @LastModifiedDate   DATETIME,
    @PublishedDate      DATETIME,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    
    SET @ErrorCode = 0

    BEGIN TRAN

    -- update the current
    UPDATE 
            Tbl_Ratna_Article
        SET 
            CreatedDate = @CreatedDate, 
            LastModifiedDate = @LastModifiedDate,
            PublishedDate = @PublishedDate
        WHERE
            SiteId = @SiteId AND
            UrlKey = @UrlKey

    COMMIT TRAN

END
GO

----------------------------------------------------------
-- Proc_Ratna_Article_Revert
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_Revert') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_Revert
GO

CREATE PROCEDURE Proc_Ratna_Article_Revert
    @SiteId           INT,
    @UrlKey           NVARCHAR(256),
    @Version          INT,
    @ErrorCode        BIGINT OUTPUT
AS
    DECLARE 
            @PublishedVersion   INT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    BEGIN TRAN
        IF EXISTS (SELECT 1 FROM Tbl_Ratna_ArticleArchive WHERE SiteId = @SiteId AND
                                UrlKey = @UrlKey AND Version = @Version)
        BEGIN

            -- update the draft copy from archive, stage = 0 is draft stage.
            IF EXISTS (SELECT 1 FROM Tbl_Ratna_Article WHERE SiteId = @SiteId AND UrlKey = @UrlKey AND Stage = 0)
            BEGIN
               
                -- draft already exists, just copy the contents.
                UPDATE Tbl_Ratna_Article
                    SET 
                        Tbl_Ratna_Article.Title = Tbl_Ratna_ArticleArchive.Title,
                        Tbl_Ratna_Article.RawData = Tbl_Ratna_ArticleArchive.RawData,
                        Tbl_Ratna_Article.LastModifiedDate = GETDATE()
                    FROM
                        Tbl_Ratna_Article
                    INNER JOIN Tbl_Ratna_ArticleArchive ON
                        Tbl_Ratna_Article.UrlKey = Tbl_Ratna_ArticleArchive.UrlKey
                    WHERE
                        Tbl_Ratna_Article.SiteId = @SiteId AND
                        Tbl_Ratna_ArticleArchive.Version = @Version
            END
            ELSE
            BEGIN

                -- if the article is not in published stage, the article cannot be
                -- reverted.
                IF EXISTS (SELECT 1 FROM Tbl_Ratna_Article WHERE SiteId = @SiteId AND UrlKey = @UrlKey AND Stage <> 1  )
                BEGIN
                    SET @ErrorCode = 1007
                END
                ELSE
                BEGIN

                    -- get the version of the published article ( published stage - 1 )
                    SELECT @PublishedVersion = Version 
                        FROM 
                            Tbl_Ratna_Article
                        WHERE
                            SiteId = @SiteId AND
                            UrlKey = @UrlKey AND 
                            Stage = 1

                    -- draft does not exists, copy all the columns
                    INSERT INTO Tbl_Ratna_Article (SiteId, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData , Version )
                            SELECT SiteId, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData, (@PublishedVersion +1)
                                FROM 
                                    Tbl_Ratna_ArticleArchive
                                WHERE 
                                    SiteId = @SiteId AND
                                    UrlKey = @UrlKey AND
                                    Version = @Version

                    -- create archive of the published article.
                    INSERT INTO Tbl_Ratna_ArticleArchive 
                            ( SiteId, Id, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData, Version, CreatedDate, LastModifiedDate, ArchivedDate)
                        SELECT  @SiteId, Id, ResourceId, HandlerId, Title, UrlKey, OwnerId, RawData, Version, CreatedDate, LastModifiedDate, GETDATE()
                            FROM 
                                Tbl_Ratna_Article
                            WHERE
                                SiteId = @SiteId AND
                                UrlKey = @UrlKey AND
                                Version = @PublishedVersion

                END
            END

        END
        ELSE
        BEGIN
            -- error scenario. version does not exist
            SET @ErrorCode = 1006
        END

   IF (@ErrorCode = 0)
   BEGIN
        COMMIT TRAN
   END
   ELSE
   BEGIN
        ROLLBACK TRAN
   END

END
GO
