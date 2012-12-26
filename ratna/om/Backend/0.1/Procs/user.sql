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
-- Proc_Ratna_GetUser
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetUser') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetUser
GO

CREATE PROCEDURE Proc_Ratna_GetUser
    @SiteId                 INT,
    @Alias                  NVARCHAR(12)
AS
BEGIN

    SET NOCOUNT ON

    SELECT *
        FROM 
            TVF_Ratna_User_Site(@SiteId)
        WHERE
            Alias = @Alias

END
GO

----------------------------------------------------------
-- Proc_Ratna_GetUserById
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetUserById') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetUserById
GO

CREATE PROCEDURE Proc_Ratna_GetUserById
    @SiteId        INT,
    @UserId        BIGINT
AS
BEGIN

    SET NOCOUNT ON

    SELECT *
        FROM 
            TVF_Ratna_User_Site(@SiteId)
        WHERE
            Id = @UserId

END
GO

----------------------------------------------------------
-- Proc_Ratna_CreateUser
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_CreateUser') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_CreateUser
GO

CREATE PROCEDURE Proc_Ratna_CreateUser
    @SiteId             INT,
    @Alias              NVARCHAR(12),
    @Email              NVARCHAR(64),
    @Password           NVARCHAR(128),
    @DisplayName        NVARCHAR(64),
    @FirstName          NVARCHAR(32),
    @LastName           NVARCHAR(32),
    @Description        NVARCHAR(1024),
    @ErrorCode          BIGINT OUTPUT
AS
    DECLARE    @PrincipalId        BIGINT
BEGIN

    SET NOCOUNT ON

    -- default error code is success
    SET @ErrorCode = 0    

    BEGIN TRAN


    -- only one user per alias
    IF EXISTS(SELECT 1 FROM Tbl_Ratna_User WHERE Alias = @Alias AND SiteId = @SiteId)
    BEGIN
            SET @ErrorCode = 1000
            ROLLBACK TRAN
            RETURN
    END

    -- if the user does not exist make sure there is no other user with the same email address
    IF EXISTS(SELECT 1 FROM Tbl_Ratna_User WHERE Email = @Email AND SiteId = @SiteId)
    BEGIN
        -- error, cannot have same email address associated with the two different users
        SET @ErrorCode = 1001
        ROLLBACK TRAN
        RETURN
    END

    -- generate principal id for the user
    INSERT INTO Tbl_Ratna_Principal ( Name, SiteId )
        VALUES ( @Alias, @SiteId )

    SET @PrincipalId = @@IDENTITY

    -- insert data into user table
    INSERT INTO Tbl_Ratna_User
        ( SiteId, PrincipalId, Alias, Email, Password, DisplayName, FirstName, LastName, Description, ActivationCode, IsActivated, IsDeleted )
        VALUES
        ( @SiteId, @PrincipalId, @Alias, @Email, @Password, @DisplayName, @FirstName, @LastName, @Description, NEWID(), 0, 0 )

    -- make sure the insert was successful
    IF (@@ROWCOUNT = 1)
    BEGIN
        COMMIT TRAN
    END
    ELSE
    BEGIN
        ROLLBACK TRAN
        RETURN
    END

    -- select the user information
    SELECT 
            *
        FROM 
            Tbl_Ratna_User
        WHERE 
            Alias = @Alias AND
            SiteId = @SiteId

END
GO

----------------------------------------------------------
-- Proc_Ratna_UpdateUser
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_UpdateUser') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_UpdateUser
GO

CREATE PROCEDURE Proc_Ratna_UpdateUser
    @SiteId             INT,
    @Alias              NVARCHAR(12),
    @DisplayName        NVARCHAR(64),
    @FirstName          NVARCHAR(32),
    @LastName           NVARCHAR(32),
    @Description        NVARCHAR(1024),
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    DECLARE @UserId        BIGINT

    -- default error code is success
    SET @ErrorCode = 0        

    SELECT @UserId = Id 
        FROM 
            Tbl_Ratna_User 
        WHERE 
            Alias = @Alias AND
            SiteId = @SiteId

    -- make sure the user is found
    IF ( @@ROWCOUNT = 0 )
    BEGIN

        SET @ErrorCode = 1005
        RETURN

    END

    UPDATE Tbl_Ratna_User
        SET 
            DisplayName = @DisplayName,
            FirstName = @FirstName,
            LastName = @LastName,
            Description = @Description
        WHERE 
            Id = @UserId AND
            SiteId = @SiteId

END
GO

----------------------------------------------------------
-- Proc_Ratna_UpdateUserPhoto
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_UpdateUserPhoto') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_UpdateUserPhoto
GO

CREATE PROCEDURE Proc_Ratna_UpdateUserPhoto
    @SiteId           INT,
    @Alias            NVARCHAR(12),
    @Photo            NVARCHAR(256),
    @ErrorCode        BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    DECLARE @UserId        BIGINT

    -- default error code is success
    SET @ErrorCode = 0        

    SELECT @UserId = Id 
        FROM 
            Tbl_Ratna_User 
        WHERE 
            Alias = @Alias AND
            SiteId = @SiteId

    -- make sure the user is found
    IF ( @@ROWCOUNT = 0 )
    BEGIN

        SET @ErrorCode = 1005
        RETURN

    END

    UPDATE Tbl_Ratna_User
        SET 
            Photo = @Photo
        WHERE 
            Id = @UserId AND
            Siteid = @SiteId

END
GO


----------------------------------------------------------
-- Proc_Ratna_IsValidUserPassword
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_IsValidUserPassword') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_IsValidUserPassword
GO

CREATE PROCEDURE Proc_Ratna_IsValidUserPassword
    @SiteId          INT,
    @Alias           NVARCHAR(12),
    @Password        NVARCHAR(128),
    @Valid           BIT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    DECLARE @UserId        BIGINT

    -- default is fail
    SET @Valid = 0

    SELECT @UserId = Id 
        FROM 
            Tbl_Ratna_User 
        WHERE 
            Alias = @Alias AND 
            [Password] = @Password AND
            SiteId = @SiteId

    IF (@@ROWCOUNT = 1)
    BEGIN
        SET @Valid = 1

        -- get the user cookie
        exec Proc_Ratna_GetUserCookie @SiteId, @UserId

    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_GetUserCookie
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetUserCookie') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetUserCookie
GO

CREATE PROCEDURE Proc_Ratna_GetUserCookie
    @SiteId            INT,
    @UserId            BIGINT
AS
BEGIN

    SET NOCOUNT ON
    
    DECLARE @CookieExpiry    DATETIME,
            @Cookie            UNIQUEIDENTIFIER

    SELECT 
            @CookieExpiry = CookieExpiry
        FROM
            Tbl_Ratna_UserLogin
        WHERE
            UserId = @UserId AND
            SiteId = @SiteId

    IF (@@ROWCOUNT = 0)
    BEGIN
            SET @CookieExpiry = DATEADD( day, 7, GETDATE() )
            SET @Cookie = NEWID()

            -- no information existed before just insert one
            INSERT INTO Tbl_Ratna_UserLogin ( SiteId, UserId, Cookie, CookieExpiry, LastLoginTime)
                VALUES ( @SiteId, @UserId, @Cookie, @CookieExpiry, GETDATE() )
    END

    -- now the row is there.
    -- if the cookie has expired generate a new one
    IF ( @CookieExpiry < GETDATE() )
    BEGIN
        
        SET @Cookie = NEWID()
        SET @CookieExpiry = DATEADD( day, 7, GETDATE() )

        UPDATE Tbl_Ratna_UserLogin
            SET Cookie = @Cookie,
                CookieExpiry = @CookieExpiry,
                LastLoginTime = GETDATE()
            WHERE
                UserId = @UserId AND
                SiteId = @SiteId
    END

    -- select the cookie information
    SELECT *
        FROM 
            Tbl_Ratna_UserLogin
        WHERE
            UserId = @UserId AND
            SiteId = @SiteId

END
GO

----------------------------------------------------------
-- Proc_Ratna_IsUserCookieValid
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_IsUserCookieValid') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_IsUserCookieValid
GO

CREATE PROCEDURE Proc_Ratna_IsUserCookieValid
    @SiteId             INT,
    @Alias              NVARCHAR(12),
    @Cookie             NVARCHAR(40),
    @IsValid            BIT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    SET @IsValid = 0

    DECLARE @ExpiryTime DATETIME,
            @UserId        BIGINT

    -- select the userid
    SELECT 
            @UserId = Id 
        FROM 
            Tbl_Ratna_User 
        WHERE 
            Alias = @Alias AND
            SiteId = @SiteId

    IF (@@ROWCOUNT = 1)
    BEGIN
        SELECT @ExpiryTime = CookieExpiry
            FROM 
                Tbl_Ratna_UserLogin
            WHERE
                UserId = @UserId AND
                Cookie = @Cookie AND
                SiteId = @SiteId

        IF (@@ROWCOUNT = 1)
        BEGIN

            -- user has the cookie, check if the cookie valid
            IF (@ExpiryTime > GETDATE())
            BEGIN

                SET @IsValid = 1

            END

        END
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_ExpireUserCookie
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_ExpireUserCookie') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_ExpireUserCookie
GO

CREATE PROCEDURE Proc_Ratna_ExpireUserCookie
    @SiteId                 INT,
    @Alias            NVARCHAR(12)
AS
BEGIN

    SET NOCOUNT ON


    DECLARE @UserId        BIGINT

    -- select the userid
    SELECT 
            @UserId = Id 
        FROM 
            Tbl_Ratna_User 
        WHERE 
            Alias = @Alias AND
            SiteId = @SiteId

    IF (@@ROWCOUNT = 1)
    BEGIN
        DELETE
            FROM 
                Tbl_Ratna_UserLogin
            WHERE
                UserId = @UserId AND
                SiteId = @SiteId
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_ChangeUserPassword
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_ChangeUserPassword') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_ChangeUserPassword
GO

CREATE PROCEDURE Proc_Ratna_ChangeUserPassword
    @SiteId                 INT,
    @Alias                  NVARCHAR(12),
    @OldPassword            NVARCHAR(128),
    @NewPassword            NVARCHAR(128),
    @ErrorCode              BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    -- default is fail
    SET @ErrorCode = 1002
        
    UPDATE Tbl_Ratna_User
        SET [Password] = @NewPassword
        WHERE 
            Alias = @Alias AND 
            [Password] = @OldPassword AND
            SiteId = @SiteId
        
    IF (@@ROWCOUNT = 1)
    BEGIN
    
        SET @ErrorCode = 0
    
    END    

END

GO

----------------------------------------------------------
-- Proc_Ratna_ActivateUser
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_ActivateUser') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_ActivateUser
GO

CREATE PROCEDURE Proc_Ratna_ActivateUser
    @SiteId                 INT,
    @Alias                  NVARCHAR(12),
    @ActivationCode         UNIQUEIDENTIFIER,
    @ErrorCode              BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    -- default is fail
    SET @ErrorCode = 1003

    UPDATE Tbl_Ratna_User
        SET [IsActivated] = 1
        WHERE 
            Alias = @Alias AND 
            [ActivationCode] = @ActivationCode AND
            SiteId = @SiteId
        
    IF (@@ROWCOUNT = 1)
    BEGIN
    
        SET @ErrorCode = 0
    
    END    

END

GO

----------------------------------------------------------
-- Proc_Ratna_ActivateUserWithoutKey
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_ActivateUserWithoutKey') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_ActivateUserWithoutKey
GO

CREATE PROCEDURE Proc_Ratna_ActivateUserWithoutKey
    @SiteId                 INT,
    @Alias            NVARCHAR(12),
    @ActorId          BIGINT,
    @ErrorCode        BIGINT OUTPUT
AS
    DECLARE @IsActorAdmin    BIT
BEGIN

    SET NOCOUNT ON

    SET @IsActorAdmin = 0

    -- default is fail for access denied
    SET @ErrorCode = 1006

    -- make sure the actor has administrator
    -- privileges to activate the user

    SELECT 1 
        FROM 
            Tbl_Ratna_User
        INNER JOIN Tbl_Ratna_GroupMembers
            ON Tbl_Ratna_GroupMembers.PrincipalId = Tbl_Ratna_User.PrincipalId
        INNER JOIN Tbl_Ratna_Group
            ON Tbl_Ratna_GroupMembers.GroupId = Tbl_Ratna_Group.Id
        WHERE
            Tbl_Ratna_User.PrincipalId = @ActorId AND
            Tbl_Ratna_Group.Name = 'Administrator' AND
            Tbl_Ratna_User.SiteId = @SiteId

    IF (@@ROWCOUNT = 1)
    BEGIN
        SET @IsActorAdmin = 1
    END

    IF (@IsActorAdmin = 1)
    BEGIN
        UPDATE Tbl_Ratna_User
            SET [IsActivated] = 1, 
                [IsDeleted] = 0
            WHERE 
                Alias = @Alias AND
                SiteId = @SiteId
        
        IF (@@ROWCOUNT = 1)
        BEGIN
    
            SET @ErrorCode = 0
    
        END
    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_GetActivationCode
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetActivationCode') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetActivationCode
GO

CREATE PROCEDURE Proc_Ratna_GetActivationCode
    @SiteId                 INT,
    @Alias            NVARCHAR(12)
AS
BEGIN

    SET NOCOUNT ON

    SELECT 
            ActivationCode 
        FROM 
            Tbl_Ratna_User 
        WHERE 
            Alias = @Alias AND
            SiteId = @SiteId

END

GO

----------------------------------------------------------
-- Proc_Ratna_DeleteUser
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_DeleteUser') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_DeleteUser
GO

CREATE PROCEDURE Proc_Ratna_DeleteUser
    @SiteId                 INT,
    @Alias            NVARCHAR(12),
    @ActorId          BIGINT,
    @ErrorCode        BIGINT OUTPUT
AS
     DECLARE @IsActorAdmin    BIT
BEGIN

    SET NOCOUNT ON

    -- default is fail
    SET @ErrorCode = 1004
    SET @IsActorAdmin = 0

    -- allow delete for self
    UPDATE Tbl_Ratna_User
        SET [IsDeleted] = 1,
            [IsActivated] = 0
        WHERE 
            Alias = @Alias AND
            PrincipalId = @ActorId AND
            SiteId = @SiteId
        
    IF (@@ROWCOUNT = 1)
    BEGIN
    
        SET @ErrorCode = 0
    
    END
    ELSE
    BEGIN

        -- allow to delete for admins.
        -- make sure the actor has administrator

        SELECT 
                1 
            FROM 
                Tbl_Ratna_User
            INNER JOIN Tbl_Ratna_GroupMembers
                ON Tbl_Ratna_GroupMembers.PrincipalId = Tbl_Ratna_User.PrincipalId
            INNER JOIN Tbl_Ratna_Group
                ON Tbl_Ratna_GroupMembers.GroupId = Tbl_Ratna_Group.Id
            WHERE
                Tbl_Ratna_User.PrincipalId = @ActorId AND
                Tbl_Ratna_Group.Name = 'Administrator' AND
                Tbl_Ratna_User.SiteId = @SiteId

        IF (@@ROWCOUNT = 1)
        BEGIN
            SET @IsActorAdmin = 1
        END

        IF (@IsActorAdmin = 1)
        BEGIN
            UPDATE Tbl_Ratna_User
                SET [IsActivated] = 0, 
                    [IsDeleted] = 1
                WHERE 
                    Alias = @Alias AND
                    SiteId = @SiteId
        
            IF (@@ROWCOUNT = 1)
            BEGIN
    
                SET @ErrorCode = 0
    
            END
        END

    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_GetUsers
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetUsers') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetUsers
GO

CREATE PROCEDURE Proc_Ratna_GetUsers
    @SiteId           INT,
    @Query            NVARCHAR(64),
    @IsActive         BIT,
    @IsDeleted        BIT,
    @Start            INT,
    @Count            INT,
    @Records          INT OUTPUT,
    @ErrorCode        BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @Records = 0

     -- sort users based on alias
    ;WITH SortedUsers(RowNumber, Id, PrincipalId, Alias, Email, DisplayName, FirstName, LastName, Photo, Description, 
                   IsActivated, IsDeleted, CreatedTime, UpdatedTime, LastLoginTime) AS
    (    
        SELECT ROW_NUMBER() OVER(ORDER BY Alias DESC) as RowNumber, 
                    Id, PrincipalId, Alias, Email, DisplayName, FirstName, LastName, Photo, Description, 
                    IsActivated, IsDeleted, CreatedTime, UpdatedTime, LastLoginTime
                FROM 
                    TVF_Ratna_Users( @SiteId, @Query, @IsActive, @IsDeleted )
    )

    SELECT *
            FROM 
                SortedUsers
            WHERE
                RowNumber >= @Start AND
                RowNumber < @Start + @Count

   -- get the total count
   SELECT @Records = COUNT(*) FROM TVF_Ratna_Users( @SiteId, @Query, @IsActive, @IsDeleted )

END
GO

----------------------------------------------------------
-- Proc_Ratna_GetUserGroups
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetUserGroups') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetUserGroups
GO

CREATE PROCEDURE Proc_Ratna_GetUserGroups
    @SiteId                 INT,
    @Alias          NVARCHAR(12),
    @Start          INT,
    @Count          INT,
    @Total          INT OUTPUT,
    @ErrorCode      INT OUTPUT
AS
    DECLARE     @PrincipalId    BIGINT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @Total = 0

    SELECT 
            @PrincipalId = PrincipalId
        FROM 
            Tbl_Ratna_User
        WHERE 
            Alias = @Alias AND
            SiteId = @SiteId

    IF (@@ROWCOUNT = 0)
    BEGIN
        SET @ErrorCode = 1005
    END
    ELSE
    BEGIN
        
        SELECT
                Tbl_Ratna_Group.*
            FROM 
                Tbl_Ratna_Group
            INNER JOIN Tbl_Ratna_GroupMembers 
                ON Tbl_Ratna_Group.Id = Tbl_Ratna_GroupMembers.GroupId
            WHERE
                Tbl_Ratna_GroupMembers.PrincipalId = @PrincipalId AND
                Tbl_Ratna_Group.SiteId = @SiteId AND
                Tbl_Ratna_GroupMembers.SiteId = @SiteId
    END
END
GO
