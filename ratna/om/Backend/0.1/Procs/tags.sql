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
-- Proc_Ratna_Tags_RegisterKey
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Tags_RegisterKey') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Tags_RegisterKey
GO

CREATE PROCEDURE Proc_Ratna_Tags_RegisterKey
    @SiteId                 INT,
    @Key                UNIQUEIDENTIFIER,
    @ErrorCode          BIGINT    OUTPUT
AS
    DECLARE @Success BIT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    IF NOT EXISTS ( SELECT 1 FROM Tbl_Ratna_TagKeys WHERE SiteId = @SiteId AND [Key] = @Key )
    BEGIN
        INSERT INTO Tbl_Ratna_TagKeys
            ( SiteId, [Key] ) VALUES ( @SiteId, @Key )
    END

END

GO

----------------------------------------------------------
-- Proc_Ratna_Tags_AddAll
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Tags_AddAll') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Tags_AddAll
GO

CREATE PROCEDURE Proc_Ratna_Tags_AddAll
    @SiteId             INT,
    @Key                UNIQUEIDENTIFIER,
    @ResourceId         BIGINT,
    @TagsXml            NTEXT,
    @ErrorCode          BIGINT    OUTPUT
AS
    DECLARE @XmlHandle  INT,
            @KeyId      BIGINT,
            @Success    BIT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @Success = 0

    /*
            // Sample XML
            // <Tags><Tag Name="Ratna" Weight="100" /></Tags>
            //
    */
    EXEC sp_xml_preparedocument @XmlHandle output, @TagsXml

    SELECT @KeyId = Id FROM Tbl_Ratna_TagKeys WHERE SiteId = @SiteId AND [Key] = @Key

    IF (@@ROWCOUNT = 1)
    BEGIN

        BEGIN TRAN

            -- delete existing tags
            DELETE 
                FROM 
                    Tbl_Ratna_Tags 
                WHERE
                    SiteId = @SiteId AND
                    [KeyId] = @KeyId AND 
                    ResourceId = @ResourceId

            INSERT INTO Tbl_Ratna_Tags
                    ( SiteId, KeyId, ResourceId, Tag, Weight )
                SELECT @SiteId, @KeyId, @ResourceId, Name, Weight
                      FROM 
                        OPENXML (@XmlHandle, '/Tags/Tag',1) 
                      WITH (
                            Name NVARCHAR(80),
                            Weight    INT
                           )

          IF (@@ROWCOUNT > 0)
          BEGIN
            SET @Success = 1
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

END

GO

----------------------------------------------------------
-- Proc_Ratna_Tags_Add
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Tags_Add') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Tags_Add
GO

CREATE PROCEDURE Proc_Ratna_Tags_Add
    @SiteId                 INT,
    @Key                UNIQUEIDENTIFIER,
    @ResourceId         BIGINT,
    @Tag                NVARCHAR(80),
    @Weight             INT,
    @ErrorCode          BIGINT    OUTPUT
AS
    DECLARE @KeyId    BIGINT
BEGIN

    SET NOCOUNT ON        
    SET @ErrorCode = 0

    SELECT 
            @KeyId = Id 
        FROM 
            Tbl_Ratna_TagKeys 
        WHERE
            SiteId = @SiteId AND 
            [Key] = @Key

    IF (@@ROWCOUNT = 1)
    BEGIN
        
        IF NOT EXISTS(SELECT 1 FROM Tbl_Ratna_Tags WHERE SiteId = @SiteId AND [KeyId] = @KeyId AND ResourceId = @ResourceId AND Tag = @Tag)
        BEGIN
            
            INSERT INTO Tbl_Ratna_Tags ( SiteId, KeyId, ResourceId, Tag, Weight)
                VALUES ( @SiteId, @KeyId, @ResourceId, @Tag, @Weight )

        END
        ELSE
        BEGIN

            UPDATE Tbl_Ratna_Tags
                SET 
                    Weight = @Weight
                WHERE 
                    SiteId = @SiteId AND
                    [KeyId] = @KeyId AND 
                    ResourceId = @ResourceId AND 
                    Tag = @Tag

        END

    END
    

END
GO

----------------------------------------------------------
-- Proc_Ratna_Tags_Delete
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Tags_Delete') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Tags_Delete
GO

CREATE PROCEDURE Proc_Ratna_Tags_Delete
    @SiteId                 INT,
    @Key                UNIQUEIDENTIFIER,
    @ResourceId         BIGINT,
    @Tag                NVARCHAR(80),
    @ErrorCode          BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    
    DECLARE @KeyId    BIGINT
    SET @ErrorCode = 0

    SELECT @KeyId = Id FROM Tbl_Ratna_TagKeys WHERE SiteId = @SiteId AND  [Key] = @Key

    IF (@@ROWCOUNT = 1)
    BEGIN

        DELETE FROM Tbl_Ratna_Tags
            WHERE
                SiteId = @SiteId AND
                [KeyId] = @KeyId AND 
                ResourceId = @ResourceId AND 
                Tag = @Tag
    END    

END

GO

----------------------------------------------------------
-- Proc_Ratna_Tags_Get
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Tags_Get') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Tags_Get
GO

CREATE PROCEDURE Proc_Ratna_Tags_Get
    @SiteId                 INT,
    @Key                UNIQUEIDENTIFIER,
    @ResourceId         BIGINT,
    @ErrorCode          BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    
    DECLARE @KeyId    BIGINT
    SET @ErrorCode = 0

    SELECT @KeyId = Id FROM Tbl_Ratna_TagKeys WHERE SiteId = @SiteId AND [Key] = @Key

    IF (@@ROWCOUNT = 1)
    BEGIN

        SELECT * 
            FROM 
                Tbl_Ratna_Tags
            WHERE
                SiteId = @SiteId AND
                [KeyId] = @KeyId AND 
                ResourceId = @ResourceId
    END    

END

GO
