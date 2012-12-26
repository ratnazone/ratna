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
-- Proc_Ratna_Acls_Set
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Acls_Set') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Acls_Set
GO

CREATE PROCEDURE Proc_Ratna_Acls_Set
    @SiteId             INT,
    @ResourceId         BIGINT,
    @ActorId            BIGINT,
    @PrincipalId        BIGINT,
    @Acls               INT,
    @ErrorCode          BIGINT    OUTPUT
AS
    DECLARE @OwnerId            BIGINT,
            @ExistingAcls       INT,
            @AllowChange        BIT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @AllowChange = 0

    SELECT @OwnerId = PrincipalId FROM Tbl_Ratna_Resource WHERE SiteId = @SiteId AND Id = @ResourceId

    IF (@@ROWCOUNT = 1)
    BEGIN

       -- make sure the actor has permission to grant permissions
       IF ( @OwnerId <> @ActorId )
       BEGIN
            SELECT @ExistingAcls = Acls FROM Tbl_Ratna_Acls WHERE SiteId = @SiteId AND ResourceId = @ResourceId AND PrincipalId = @ActorId

            IF (@@ROWCOUNT = 1)
            BEGIN
                -- make sure the actor has grant permissions.
                IF ( (@ExistingAcls & 16) = 16 )
                BEGIN
                    SET @AllowChange = 1
                END
            END
       END
       ELSE
       BEGIN
            SET @AllowChange = 1
       END

       IF ( @AllowChange = 0 )
       BEGIN
            SET @ErrorCode = 1002
       END
       ELSE

        BEGIN
            -- insert or update
            BEGIN TRAN

                IF NOT EXISTS (SELECT 1 FROM Tbl_Ratna_Acls WHERE SiteId = @SiteId AND ResourceId = @ResourceId AND PrincipalId = @PrincipalId)
                BEGIN

                    INSERT INTO Tbl_Ratna_Acls ( SiteId, ResourceId, PrincipalId, Acls)
                        VALUES ( @SiteId, @ResourceId, @PrincipalId, @Acls )

                END
                ELSE
                BEGIN

                    UPDATE Tbl_Ratna_Acls
                        SET
                            Acls = @Acls
                        WHERE
                             SiteId = @SiteId AND
                             ResourceId = @ResourceId AND 
                             PrincipalId = @PrincipalId
                END

            COMMIT TRAN
        END

    END
    ELSE
    BEGIN
        SET @ErrorCode = 1001
    END

END

GO


----------------------------------------------------------
-- Proc_Ratna_Acls_Delete
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Acls_Delete') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Acls_Delete
GO

CREATE PROCEDURE Proc_Ratna_Acls_Delete
    @SiteId             INT,
    @ResourceId         BIGINT,
    @ActorId            BIGINT,
    @PrincipalId        BIGINT,
    @ErrorCode          BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    DELETE FROM 
            Tbl_Ratna_Acls
        WHERE 
            SiteId = @SiteId AND
            ResourceId = @ResourceId AND 
            PrincipalId = @PrincipalId

END
GO

----------------------------------------------------------
-- Proc_Ratna_Acls_GetAll
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Acls_GetAll') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Acls_GetAll
GO

CREATE PROCEDURE Proc_Ratna_Acls_GetAll
    @SiteId             INT,
    @ResourceId         BIGINT,
    @ActorId            BIGINT,
    @ErrorCode          BIGINT    OUTPUT
AS
    DECLARE @Specific BIT,
            @OwnerId    BIGINT
BEGIN

    SET @ErrorCode = 0

    -- make sure the actor has priviledges to view the acls

    SELECT 
            Acls, Name, PrincipalId
        FROM
            Tbl_Ratna_Acls
        INNER JOIN Tbl_Ratna_Principal ON Tbl_Ratna_Principal.Id = Tbl_Ratna_Acls.PrincipalId
        WHERE 
            Tbl_Ratna_Acls.SiteId = @SiteId AND
            Tbl_Ratna_Principal.SiteId = @SiteId AND
            ResourceId = @ResourceId

END
GO

----------------------------------------------------------
-- Proc_Ratna_Acls_Get
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Acls_Get') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Acls_Get
GO

CREATE PROCEDURE Proc_Ratna_Acls_Get
    @SiteId             INT,
    @ResourceId         BIGINT,
    @PrincipalId        BIGINT,
    @ErrorCode          BIGINT    OUTPUT
AS
    DECLARE @Specific   BIT,
            @IsGroup    BIT,
            @OwnerId    BIGINT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @Specific = 0
    SET @IsGroup = 0

    IF EXISTS(SELECT 1 FROM Tbl_Ratna_Group WHERE SiteId = @SiteId AND PrincipalId = @PrincipalId)
    BEGIN
        SET @IsGroup = 1 
    END

    SELECT 
            @OwnerId = PrincipalId 
        FROM 
            Tbl_Ratna_Resource
        WHERE 
            SiteId = @SiteId AND
            Id = @ResourceId

    IF ( @OwnerId = @PrincipalId )
    BEGIN

        -- all permissions
        SELECT ( 1 | 2 | 4 | 8 | 16 ) as Acls

    END
    ELSE
    BEGIN
        -- if there is no policy, nothing is allowed unless owner.
        SELECT @Specific = 1 
            FROM
                Tbl_Ratna_Acls
            WHERE
                SiteId = @SiteId AND 
                ResourceId = @ResourceId AND
                PrincipalId = @PrincipalId

        IF ( @Specific = 1 )
        BEGIN
            SELECT Acls 
                FROM 
                    Tbl_Ratna_Acls
                WHERE
                    SiteId = @SiteId AND 
                    ResourceId = @ResourceId AND
                    PrincipalId = @PrincipalId
        END
        ELSE
        BEGIN
            
            -- check if there is a guest policy ( every user is also a guest )
            IF EXISTS (SELECT 1 FROM Tbl_Ratna_Acls WHERE SiteId = @SiteId AND ResourceId = @ResourceId AND PrincipalId = 0 )
            BEGIN
                SELECT Acls FROM Tbl_Ratna_Acls WHERE SiteId = @SiteId AND ResourceId = @ResourceId AND PrincipalId = 0 
            END
            ELSE
            BEGIN
                -- if there is no specific acl or guest acl, check if there is a group acl
                -- and the user belongs to the group.
                IF ( @IsGroup = 0 )
                BEGIN
                    
                    SELECT Acls 
                        FROM 
                            Tbl_Ratna_Acls
                        INNER JOIN Tbl_Ratna_Group ON Tbl_Ratna_Group.PrincipalId = Tbl_Ratna_Acls.PrincipalId
                        INNER JOIN Tbl_Ratna_GroupMembers ON Tbl_Ratna_GroupMembers.GroupId = Tbl_Ratna_Group.id
                        INNER JOIN Tbl_Ratna_User ON Tbl_Ratna_User.PrincipalId = Tbl_Ratna_GroupMembers.PrincipalId
                        WHERE 
                            Tbl_Ratna_Acls.SiteId = @SiteId AND
                            Tbl_Ratna_Group.SiteId = @SiteId AND
                            Tbl_Ratna_GroupMembers.SiteId = @SiteId AND
                            Tbl_Ratna_User.SiteId = @SiteId AND
                            ResourceId = @ResourceId AND 
                            Tbl_Ratna_User.PrincipalId = @PrincipalId
                END
                ELSE
                BEGIN
                    SELECT 0 as Acls
                END
            END

        END
    END
END
GO
