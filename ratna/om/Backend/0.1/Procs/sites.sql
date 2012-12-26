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
-- Proc_Ratna_GetSites
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetSites') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetSites
GO

CREATE PROCEDURE Proc_Ratna_GetSites
    @ErrorCode             BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    SELECT 
            Id, Host, Title, IsActive, IsProvisioned
        FROM
            Tbl_Ratna_Site
    

END
GO

----------------------------------------------------------
-- Proc_Ratna_CreateSite
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_CreateSite') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_CreateSite
GO

CREATE PROCEDURE Proc_Ratna_CreateSite
    @Host                  NVARCHAR(1024),
    @Title                 NVARCHAR(256),
    @ErrorCode             BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    DECLARE @SiteId INT

    IF EXISTS(SELECT 1 FROM Tbl_Ratna_Site WHERE Host = @Host)
    BEGIN
        
        SET @ErrorCode = 1000 -- site already exists with name

    END
    ELSE
    BEGIN
        
        INSERT INTO Tbl_Ratna_Site ( Host, Title )
            VALUES ( @Host, @Title )

    END

    IF ( @ErrorCode = 0 )
    BEGIN

        SELECT 
                Id, Host, Title, IsActive, IsProvisioned
            FROM
                Tbl_Ratna_Site
            WHERE
                Host = @Host

    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_ProvisionSite
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_ProvisionSite') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_ProvisionSite
GO

CREATE PROCEDURE Proc_Ratna_ProvisionSite
    @SiteId                INT,
    @AdminEmail            NVARCHAR(64),
    @AdminAlias            NVARCHAR(12),
    @AdminPassword         NVARCHAR(128),
    @ErrorCode             BIGINT    OUTPUT
AS
DECLARE
    @IsProvisioned         BIT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @IsProvisioned = 0

    SELECT 
            @IsProvisioned = IsProvisioned
        FROM
            Tbl_Ratna_Site
        WHERE
            Id = @SiteId

   IF (@@ROWCOUNT = 0)
   BEGIN
        SET @IsProvisioned = 0
   END

   IF (@IsProvisioned = 0)
   BEGIN

        -- system account
        EXEC Proc_Ratna_CreateUser @SiteId, 
                                   'system', -- alias
                                   'nonexist1@ratnazone.com',  -- email
                                   '**********', -- unknown password
                                   'System', -- display name
                                   'System', -- first name
                                   'System', -- last name
                                   '', -- decription
                                   @ErrorCode OUTPUT

        -- guest account
        EXEC Proc_Ratna_CreateUser @SiteId, 
                                   'guest', -- alias
                                   'nonexist2@ratnazone.com',  -- email
                                   '**********', -- unknown password
                                   'Guest', -- display name
                                   'Guest', -- first name
                                   'Guest', -- last name
                                   '', -- decription
                                   @ErrorCode OUTPUT

        -- admin account
        EXEC Proc_Ratna_CreateUser @SiteId, 
                                   @AdminAlias, -- alias
                                   @AdminEmail,  -- email
                                   @AdminPassword, -- password
                                   'Administrator', -- display name
                                   'Admin', -- first name
                                   'Admin', -- last name
                                   '', -- decription
                                   @ErrorCode OUTPUT

        -- enable admin account
        UPDATE 
                Tbl_Ratna_User
            SET 
                IsActivated = 1
            WHERE
                SiteId = @SiteId AND
                Alias = @AdminAlias


        -- administrator group
        EXEC Proc_Ratna_Group_Create @SiteId, 
                                     'Administrator',
                                     'Administrator group',
                                     @ErrorCode OUTPUT

        -- visitor group
        EXEC Proc_Ratna_Group_Create @SiteId, 
                                    'Visitor', -- alias of group
                                    'Visitor group', -- description of group
                                    @ErrorCode OUTPUT

        -- add admin to administrator group
        DECLARE @GroupId        BIGINT,
                @PrincipalId    BIGINT

        SELECT 
                @GroupId = Id
              FROM
                Tbl_Ratna_Group
              WHERE
                SiteId = @SiteId AND
                Name = 'Administrator'

        SELECT
                   @PrincipalId = PrincipalId
              FROM
                   Tbl_Ratna_User
              WHERE
                   SiteId = @SiteId AND
                   Alias = @AdminAlias

        EXEC Proc_Ratna_Group_AddMember @SiteId,
                                        @GroupId,
                                        @PrincipalId,
                                        @ErrorCode OUTPUT

        SELECT 
                @GroupId = Id
              FROM
                Tbl_Ratna_Group
              WHERE
                SiteId = @SiteId AND
                Name = 'Visitor'

        SELECT
                   @PrincipalId = PrincipalId
              FROM
                   Tbl_Ratna_User
              WHERE
                   SiteId = @SiteId AND
                   Alias = 'guest'

        EXEC Proc_Ratna_Group_AddMember @SiteId,
                                        @GroupId,
                                        @PrincipalId,
                                        @ErrorCode OUTPUT

        UPDATE
            Tbl_Ratna_Site
        SET
            IsProvisioned = 1
        WHERE
            Id = @SiteId

    END -- end of site provision

END
GO
