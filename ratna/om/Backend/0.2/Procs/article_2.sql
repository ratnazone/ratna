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
-- Proc_Ratna_Article_GetListInPath
--
-- Returns articles that are in the given url path
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Article_GetListInPath') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Article_GetListInPath
GO

CREATE PROCEDURE Proc_Ratna_Article_GetListInPath
    @SiteId             INT,
    @UrlPath            NVARCHAR(1024),
    @OwnerId            BIGINT,
    @Stage              INT,
    @Start              INT,
    @Count              INT,
    @TagKey             UNIQUEIDENTIFIER,
    @HandlerId          UNIQUEIDENTIFIER,
    @Records            INT OUTPUT,
    @ErrorCode          BIGINT OUTPUT
AS
    DECLARE   @GuestId  BIGINT
BEGIN

    SET NOCOUNT ON

    -- default outs
    SET @Records = 0
    SET @ErrorCode = 0

    DECLARE @LikeQuery NVARCHAR(1026),
            @Success   BIT

    -- set the GuestId
    SELECT 
            @GuestId = Id
        FROM 
            Tbl_Ratna_User
        WHERE 
            Alias = 'guest' AND
            SiteId = @SiteId


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
    
    if (@UrlPath = '')
    BEGIN
        SET @UrlPath = '/' ;
    END

    SET @Records = 0
    SET @Success = 1
    SET @LikeQuery = @UrlPath + '%'

    INSERT INTO @MatchedArticles
            SELECT 
                    Id
                FROM 
                    Tbl_Ratna_Article
                WHERE
                    SiteId = @SiteId AND
                    (@HandlerId = CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER) OR HandlerId = @HandlerId) AND
                    (@OwnerId = @GuestId OR Tbl_Ratna_Article.OwnerId = @OwnerId) AND
                    UrlKey LIKE @LikeQuery AND
                    Stage = @Stage

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

END

GO

