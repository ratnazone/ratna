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
-- this file contains the TVFs used in Ratna

IF OBJECT_ID(N'TVF_Ratna_User_Site', N'TF') IS NOT NULL
    DROP FUNCTION TVF_Ratna_User_Site;
GO
CREATE FUNCTION TVF_Ratna_User_Site(@SiteId INT)
RETURNS @Users TABLE 
(
    Id                 BIGINT          NOT NULL    PRIMARY KEY,
    PrincipalId        BIGINT          NOT NULL,
    Alias              NVARCHAR(12)    NOT NULL    UNIQUE,
    Email              NVARCHAR(64)    NOT NULL    UNIQUE,
    DisplayName        NVARCHAR(64),
    FirstName          NVARCHAR(32),
    LastName           NVARCHAR(32),
    Photo              NVARCHAR(256),
    Description        NVARCHAR(1024),
    IsActivated        BIT,
    IsDeleted          BIT,
    CreatedTime        DATETIME    NOT NULL,
    UpdatedTime        DATETIME    NOT NULL,
    LastLoginTime      DATETIME    NOT NULL
)
AS 
BEGIN

    INSERT 
        INTO  @Users
            SELECT 
                   Id, 
                   PrincipalId, 
                   Alias, 
                   Email, 
                   DisplayName, 
                   FirstName, 
                   LastName,
                   Photo,
                   Description, 
                   IsActivated,
                   IsDeleted,
                   CreatedTime, 
                   UpdatedTime,
                   CASE 
                        WHEN ISNULL(LastLoginTime, 0) = 0 THEN  CAST('1753-01-01 00:00:00.000' as datetime)
                        ELSE LastLoginTime
                   END
                FROM 
                    Tbl_Ratna_User
                LEFT JOIN
                    Tbl_Ratna_UserLogin ON Tbl_Ratna_UserLogin.UserId = Tbl_Ratna_User.Id
                WHERE
                    Tbl_Ratna_User.SiteId = @SiteId

    RETURN
END
GO

IF OBJECT_ID(N'TVF_Ratna_Users', N'TF') IS NOT NULL
    DROP FUNCTION TVF_Ratna_Users;
GO
CREATE FUNCTION TVF_Ratna_Users(@SiteId INT, @Query NVARCHAR(64), @IsActive BIT, @IsDeleted BIT)
RETURNS @Users TABLE 
(
    Id                 BIGINT          NOT NULL    PRIMARY KEY,
    PrincipalId        BIGINT          NOT NULL,
    Alias              NVARCHAR(12)    NOT NULL    UNIQUE,
    Email              NVARCHAR(64)    NOT NULL    UNIQUE,
    DisplayName        NVARCHAR(64),
    FirstName          NVARCHAR(32),
    LastName           NVARCHAR(32),
    Photo              NVARCHAR(256),
    Description        NVARCHAR(1024),
    IsActivated        BIT,
    IsDeleted          BIT,
    CreatedTime        DATETIME    NOT NULL,
    UpdatedTime        DATETIME    NOT NULL,
    LastLoginTime      DATETIME    NOT NULL
)
AS 
BEGIN

    DECLARE @LikeQuery NVARCHAR(66)
    SET @LikeQuery = '%' + @Query + '%'

    INSERT 
        INTO  @Users
            SELECT 
                   Id, 
                   PrincipalId, 
                   Alias, 
                   Email, 
                   DisplayName, 
                   FirstName, 
                   LastName,
                   Photo,
                   Description, 
                   IsActivated,
                   IsDeleted,
                   CreatedTime, 
                   UpdatedTime,
                   CASE 
                        WHEN ISNULL(LastLoginTime, 0) = 0 THEN  CAST('1753-01-01 00:00:00.000' as datetime)
                        ELSE LastLoginTime
                   END
                FROM 
                    Tbl_Ratna_User
                LEFT JOIN
                    Tbl_Ratna_UserLogin ON Tbl_Ratna_UserLogin.UserId = Tbl_Ratna_User.Id
                WHERE
                    Tbl_Ratna_User.SiteId = @SiteId AND
                    Tbl_Ratna_UserLogin.SiteId = @SiteId AND
                    IsDeleted = @IsDeleted AND
                    IsActivated = @IsActive AND
                    ( 
                        Alias LIKE @LikeQuery OR
                        Email LIKE @LikeQuery OR
                        Firstname LIKE @LikeQuery OR
                        LastName LIKE @LikeQuery
                    )

   

    RETURN
END
GO

IF OBJECT_ID(N'TVF_Ratna_Media', N'TF') IS NOT NULL
    DROP FUNCTION TVF_Ratna_Media;
GO
CREATE FUNCTION TVF_Ratna_Media(@SiteId INT, @MediaType INT)
RETURNS @Media TABLE 
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
AS 
BEGIN

   -- MediaType = 0 will mean select all

   IF ( @MediaType = 0 )
   BEGIN

       INSERT 
            INTO  @Media
                SELECT 
                       Id, 
                       ResourceId, 
                       MediaType, 
                       Name,
                       Url, 
                       OwnerId, 
                       RawData, 
                       CreatedDate, 
                       LastModifiedDate
                    FROM 
                        Tbl_Ratna_Media
                    WHERE
                        SiteId = @SiteId

   END
   ELSE
   BEGIN
        INSERT 
            INTO  @Media
                SELECT 
                       Id, 
                       ResourceId, 
                       MediaType, 
                       Name,
                       Url, 
                       OwnerId, 
                       RawData, 
                       CreatedDate, 
                       LastModifiedDate
                    FROM 
                        Tbl_Ratna_Media
                    WHERE
                        SiteId = @SiteId AND
                        MediaType = @MediaType

    END

    RETURN
END
GO


IF OBJECT_ID(N'TVF_Ratna_PluginData', N'TF') IS NOT NULL
    DROP FUNCTION TVF_Ratna_PluginData;
GO
CREATE FUNCTION TVF_Ratna_PluginData(@SiteId    INT, @PluginId           UNIQUEIDENTIFIER, @PropertyTag     NVARCHAR(170))
RETURNS @PluginData TABLE 
(
    PluginId            UNIQUEIDENTIFIER     NOT NULL,
    [Key]               NVARCHAR(128)        NOT NULL,
    Id                  NVARCHAR(128)        NOT NULL,
    UId                 UNIQUEIDENTIFIER     NOT NULL,
    RawData             NTEXT,
    CreatedTime         DATETIME    NOT NULL,
    UpdatedTime         DATETIME    NOT NULL
)
AS 
BEGIN

    IF ( @PropertyTag = '' )
   
    BEGIN

        INSERT 
            INTO  @PluginData
                SELECT 
                       PluginId, 
                       [Key], 
                       Id,
                       UId,
                       RawData,
                       CreatedTime,
                       UpdatedTime
                    FROM 
                        Tbl_Ratna_PluginData
                    WHERE
                        SiteId = @SiteId AND
                        PluginId = @PluginId

    END
    ELSE
    BEGIN


        INSERT 
            INTO  @PluginData
                SELECT 
                       PluginId, 
                       [Key], 
                       Id,
                       UId,
                       RawData,
                       CreatedTime,
                       UpdatedTime
                    FROM 
                        Tbl_Ratna_PluginData
                    WHERE
                        SiteId = @SiteId AND
                        PluginId = @PluginId AND
                        RawData LIKE @PropertyTag

    END


    RETURN
END
GO
