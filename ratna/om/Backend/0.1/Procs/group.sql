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
-- Proc_Ratna_Group_Create
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Group_Create') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Group_Create
GO

CREATE PROCEDURE Proc_Ratna_Group_Create
    @SiteId             INT,
    @Name               NVARCHAR(24),
    @Description        NVARCHAR(512),
    @ErrorCode          BIGINT OUTPUT
AS
    DECLARE    @PrincipalId        BIGINT
BEGIN

    SET NOCOUNT ON

    DECLARE @cId BIGINT
    SET @ErrorCode = 0

    IF NOT EXISTS(SELECT 1 FROM Tbl_Ratna_Group WHERE SiteId = @SiteId AND Name = @Name)
    BEGIN

        -- generate principal id for the group
        INSERT INTO Tbl_Ratna_Principal ( SiteId, Name )
            VALUES ( @SiteId, @Name )

        SET @PrincipalId = @@IDENTITY

        INSERT INTO Tbl_Ratna_Group ( SiteId, PrincipalId, Name, Description )
            VALUES (@SiteId, @PrincipalId, @Name, @Description )

        SET @cId = @@IDENTITY
    END
    ELSE
    BEGIN
            SET @ErrorCode = 1000    
    END

    IF (@ErrorCode = 0)
    BEGIN
        SELECT *
            FROM 
                Tbl_Ratna_Group
            WHERE
                SiteId = @SiteId AND 
                Id = @cId
    END


END
GO

----------------------------------------------------------
-- Proc_Ratna_Group_Update
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Group_Update') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Group_Update
GO

CREATE PROCEDURE Proc_Ratna_Group_Update
    @SiteId             INT,
    @Id                 BIGINT,
    @Description        NVARCHAR(512),
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    DECLARE @cId BIGINT
    SET @ErrorCode = 0

    UPDATE Tbl_Ratna_Group
        SET 
            Description = @Description
        WHERE 
            SiteId = @SiteId AND
            Id = @Id

END
GO

----------------------------------------------------------
-- Proc_Ratna_Group_Read
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Group_Read') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Group_Read
GO

CREATE PROCEDURE Proc_Ratna_Group_Read
    @SiteId           INT,
    @Name             NVARCHAR(24),
    @ErrorCode        BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    
    SELECT *
        FROM 
            Tbl_Ratna_Group
        WHERE
            SiteId = @SiteId AND 
            Name = @Name


END
GO

----------------------------------------------------------
-- Proc_Ratna_Group_IsMember
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Group_IsMember') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Group_IsMember
GO

CREATE PROCEDURE Proc_Ratna_Group_IsMember
    @SiteId             INT,
    @GroupId            BIGINT,
    @PrincipalId        BIGINT,
    @IsMember           BIT OUTPUT,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0
    SET @IsMember = 0

    IF EXISTS (SELECT 1 FROM Tbl_Ratna_GroupMembers WHERE SiteId = @SiteId AND GroupId = @GroupId AND PrincipalId = @PrincipalId)
    BEGIN

        SET @IsMember = 1

    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_Group_GetMembership
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Group_GetMembership') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Group_GetMembership
GO

CREATE PROCEDURE Proc_Ratna_Group_GetMembership
    @SiteId                 INT,
    @PrincipalId             BIGINT,
    @ErrorCode              BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0

    SELECT *
        FROM
            Tbl_Ratna_GroupMembers
        WHERE
            SiteId = @SiteId AND
            PrincipalId = @PrincipalId
            

END

GO

----------------------------------------------------------
-- Proc_Ratna_Group_AddMember
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Group_AddMember') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Group_AddMember
GO

CREATE PROCEDURE Proc_Ratna_Group_AddMember
    @SiteId             INT,
    @GroupId            BIGINT,
    @PrincipalId        BIGINT,
    @ErrorCode          BIGINT OUTPUT
AS
    DECLARE     @GroupPrincipalId   BIGINT
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0

    IF NOT EXISTS (SELECT 1 FROM Tbl_Ratna_GroupMembers WHERE SiteId = @SiteId AND GroupId = @GroupId AND PrincipalId = @PrincipalId)
    BEGIN

        -- make sure the groupdId exists and principalId for group is not the same
        -- as the supplied.
        SELECT 
                @GroupPrincipalId = PrincipalId
            FROM
                Tbl_Ratna_Group
            WHERE
                SiteId = @SiteId AND
                Id = @GroupId

        IF (@@ROWCOUNT =1)
        BEGIN
            IF ( @GroupPrincipalId <> @PrincipalId)
            BEGIN
                INSERT INTO Tbl_Ratna_GroupMembers ( SiteId, GroupId, PrincipalId)
                    VALUES ( @SiteId, @GroupId, @PrincipalId )
            END
            ELSE
            BEGIN
                SET @ErrorCode = 1002
            END
        END
        ELSE
        BEGIN
            SET @ErrorCode = 1001
        END

    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_Group_RemoveMember
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Group_RemoveMember') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Group_RemoveMember
GO

CREATE PROCEDURE Proc_Ratna_Group_RemoveMember
    @SiteId                 INT,
    @GroupId            BIGINT,
    @PrincipalId        BIGINT,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0


    DELETE 
        FROM 
            Tbl_Ratna_GroupMembers 
        WHERE 
            SiteId = @SiteId AND
            GroupId = @GroupId AND 
            PrincipalId = @PrincipalId

END

GO
