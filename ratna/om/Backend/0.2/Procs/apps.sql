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
-- Proc_Ratna_App_AddUpdate
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_AddUpdate') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_AddUpdate
GO

CREATE PROCEDURE Proc_Ratna_App_AddUpdate
    @SiteId              INT,
    @Name                NVARCHAR(256),
    @UniqueId            UNIQUEIDENTIFIER,
    @Publisher           NVARCHAR(256),
    @Description         NVARCHAR(2048),
    @Url                 NVARCHAR(1024),
    @Scope               INT,
    @Version             NVARCHAR(64),
    @Location            NVARCHAR(2048),
    @File                NVARCHAR(64),
    @FileEntry           NVARCHAR(256),
    @References          NVARCHAR(2048),
    @IconUrl             NVARCHAR(1024),
    @Id                  BIGINT    OUTPUT,
    @ErrorCode           BIGINT    OUTPUT
AS
DECLARE
    @ExistingAppId      BIGINT,
    @ExistingName       NVARCHAR(256),
    @ExistingPublisher  NVARCHAR(256),
    @AppId              BIGINT,
    @Count              INT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @Id = 0

    SELECT @ExistingAppId = Id, @ExistingName = Name, @ExistingPublisher = Publisher
        FROM 
            TVF_Ratna_App_Site(@SiteId)
        WHERE
            UniqueId = @UniqueId

    SET @Count = @@ROWCOUNT

    IF (@Count = 1)
    BEGIN
        SET @AppId = @ExistingAppId

        -- make sure that the Publisher is the same.
        IF (@ExistingPublisher <> @Publisher)
        BEGIN
            SET @ErrorCode = 1001
        END
        ELSE IF (@ExistingName <> @Name)
        BEGIN
            SET @ErrorCode = 1002
        END
        ELSE
        BEGIN
            
            -- update the information on the app
            UPDATE
                    Tbl_Ratna_App
                SET
                    Description = @Description,
                    Url = @Url,
                    Version = @Version,
                    Location = @Location,
                    [File] = @File,
                    [FileEntry] = @FileEntry,
                    [References] = @References
                WHERE
                    SiteId = @SiteId AND
                    Id = @AppId

        END

    END
    ELSE
    BEGIN

        -- insert app in the table
        INSERT 
            INTO 
                Tbl_Ratna_App 
                    (SiteId, Name, UniqueId, Publisher, Description, Url, Scope, Version, Location, [File], [FileEntry], [References], IconUrl)
            VALUES 
                (@SiteId, @Name, @UniqueId, @Publisher, @Description, @Url, @Scope, @Version, @Location, @File,  @FileEntry, @References, @IconUrl)

        SET @AppId = @@IDENTITY
    END

    IF ( @ErrorCode = 0 )
    BEGIN
        SET @Id = @AppId
    END

END

GO


----------------------------------------------------------
-- Proc_Ratna_App_GetList - returns all app of the site.
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_GetList') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_GetList
GO

CREATE PROCEDURE Proc_Ratna_App_GetList
    @SiteId              INT,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    -- select the apps
    SELECT 
        *
    FROM 
        TVF_Ratna_App_Site(@SiteId)



END
GO

----------------------------------------------------------
-- Proc_Ratna_App_GetEnabledList - returns apps that are enabled/disabled.
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_GetEnabledList') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_GetEnabledList
GO

CREATE PROCEDURE Proc_Ratna_App_GetEnabledList
    @SiteId              INT,
    @Enabled             BIT,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    -- select the apps
    SELECT 
        *
    FROM 
        TVF_Ratna_App_Site(@SiteId)
    WHERE
        Enabled = @Enabled



END
GO

----------------------------------------------------------
-- Proc_Ratna_App_GetListWithScope - returns all app of the site
-- with the given app scope.
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_GetListWithScope') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_GetListWithScope
GO

CREATE PROCEDURE Proc_Ratna_App_GetListWithScope
    @SiteId              INT,
    @Scope               INT,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    -- select the apps
    SELECT 
        *
    FROM 
        TVF_Ratna_App_Site(@SiteId)
    WHERE
        Scope = @Scope AND
        Enabled = 1


END
GO

----------------------------------------------------------
-- Proc_Ratna_App_Get - returns a single app
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_Get') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_Get
GO

CREATE PROCEDURE Proc_Ratna_App_Get
    @SiteId              INT,
    @Id                  BIGINT,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    -- select the apps
    SELECT 
        *
    FROM 
        TVF_Ratna_App_Site(@SiteId)
    WHERE
        Id = @Id

END
GO

----------------------------------------------------------
-- Proc_Ratna_App_GetWithUniqueId - returns a single app
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_GetWithUniqueId') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_GetWithUniqueId
GO

CREATE PROCEDURE Proc_Ratna_App_GetWithUniqueId
    @SiteId              INT,
    @UniqueId            UNIQUEIDENTIFIER,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    -- select the apps
    SELECT 
        *
    FROM 
        TVF_Ratna_App_Site(@SiteId)
    WHERE
        UniqueId = @UniqueId

END
GO

----------------------------------------------------------
-- Proc_Ratna_App_SetRawData - sets raw data for an app
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_SetRawData') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_SetRawData
GO

CREATE PROCEDURE Proc_Ratna_App_SetRawData
    @SiteId              INT,
    @Id                  BIGINT,
    @RawData             NTEXT,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    UPDATE 
            Tbl_Ratna_App
        SET
            RawData = @RawData
        WHERE
            SiteId = @SiteId AND
            Id = @Id

END
GO

----------------------------------------------------------
-- Proc_Ratna_App_Activate - Activates or deactivates an app
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_Activate') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_Activate
GO

CREATE PROCEDURE Proc_Ratna_App_Activate
    @SiteId              INT,
    @Id                  BIGINT,
    @Enable              BIT,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    UPDATE 
            Tbl_Ratna_App
        SET
            [Enabled] = @Enable
        WHERE
            SiteId = @SiteId AND
            Id = @Id

END
GO

----------------------------------------------------------
-- Proc_Ratna_App_Delete - Deletes an existing app
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_App_Delete') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_App_Delete
GO

CREATE PROCEDURE Proc_Ratna_App_Delete
    @SiteId              INT,
    @Id                  BIGINT,
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    DELETE
        FROM 
            Tbl_Ratna_App
        WHERE
            SiteId = @SiteId AND
            Id = @Id

END
GO
