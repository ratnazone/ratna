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
-- Proc_Ratna_Plugin_GetPlugin
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_GetPlugin') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_GetPlugin
GO

CREATE PROCEDURE Proc_Ratna_Plugin_GetPlugin
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    SELECT *
        FROM 
            Tbl_Ratna_Plugins
        WHERE
            SiteId = @SiteId AND
            Id = @PluginId

    IF (@@ROWCOUNT = 0)
    BEGIN
        SET @ErrorCode = 2002
    END

END
GO
----------------------------------------------------------
-- Proc_Ratna_Plugin_Register
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_Register') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_Register
GO

CREATE PROCEDURE Proc_Ratna_Plugin_Register
    @SiteId          INT,
    @Name            NVARCHAR(64),
    @Id              UNIQUEIDENTIFIER,
    @Type            NVARCHAR(256),
    @RawData         NTEXT,
    @ErrorCode       BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    DECLARE @Success BIT
    SET @Success = 1
    SET @ErrorCode = 0

    BEGIN TRAN

    IF NOT EXISTS (SELECT 1 FROM Tbl_Ratna_Plugins WHERE SiteId = @SiteId AND Name = @Name AND Id = @Id)
    BEGIN
        
        INSERT INTO Tbl_Ratna_Plugins ( SiteId, Name, Id, Type, RawData, Active )
            VALUES ( @SiteId, @Name, @Id, @Type, @RawData, 0 )

        IF (@@ROWCOUNT <> 1)
        BEGIN
            SET @Success = 0
        END
    
    END
    ELSE    
    BEGIN

        -- if already exists its ok
        IF EXISTS(SELECT 1 FROM Tbl_Ratna_Plugins WHERE SiteId = @SiteId AND Name = @Name AND Id = @Id)
        BEGIN
            SET @Success = 1

            UPDATE Tbl_Ratna_Plugins
                SET 
                    RawData = @RawData
                WHERE 
                    SiteId = @SiteId AND
                    Name = @Name AND 
                    Id = @Id
        END
        ELSE
        BEGIN
            -- name must be unique
            IF EXISTS (SELECT 1 FROM Tbl_Ratna_Plugins WHERE SiteId = @SiteId AND Name = @Name)
            BEGIN
                SET @ErrorCode = 2000
                SET @Success = 0
            END

            -- id must be unique
            IF EXISTS (SELECT 1 FROM Tbl_Ratna_Plugins WHERE SiteId = @SiteId AND Id = @Id)
            BEGIN
                SET @ErrorCode = 2001
                SET @Success = 0
            END

        END

    END

    IF ( @Success = 1)
    BEGIN
        COMMIT TRAN
    END
    ELSE
    BEGIN
        ROLLBACK TRAN
        SET @ErrorCode = 1
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_ChangeActiveState
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_ChangeActiveState') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_ChangeActiveState
GO

CREATE PROCEDURE Proc_Ratna_Plugin_ChangeActiveState
    @SiteId           INT,
    @Id               UNIQUEIDENTIFIER,
    @State            BIT,
    @ErrorCode        BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    UPDATE Tbl_Ratna_Plugins 
        SET 
            Active = @State
        WHERE 
            SiteId = @SiteId AND
            Id = @Id

    IF (@@ROWCOUNT <> 1)
    BEGIN
        SET @ErrorCode = 1
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_GetPluginData
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_GetPluginData') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_GetPluginData
GO

CREATE PROCEDURE Proc_Ratna_Plugin_GetPluginData
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @Key                NVARCHAR(128),
    @Id                 NVARCHAR(128),
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    SELECT * 
        FROM 
            Tbl_Ratna_PluginData
        WHERE 
            SiteId = @SiteId AND
            PluginId = @PluginId AND
            [Key] = @Key AND
            Id = @Id

    IF (@@ROWCOUNT <> 1)
    BEGIN
        SET @ErrorCode = 2003
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_GetPluginDataWithUid
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_GetPluginDataWithUid') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_GetPluginDataWithUid
GO

CREATE PROCEDURE Proc_Ratna_Plugin_GetPluginDataWithUid
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @UId                UNIQUEIDENTIFIER,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    SELECT * 
        FROM 
            Tbl_Ratna_PluginData
        WHERE 
            SiteId = @SiteId AND
            PluginId = @PluginId AND
            UId = @UId

    IF (@@ROWCOUNT <> 1)
    BEGIN
        SET @ErrorCode = 1
    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_GetPluginDataForKey
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_GetPluginDataForKey') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_GetPluginDataForKey
GO

CREATE PROCEDURE Proc_Ratna_Plugin_GetPluginDataForKey
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @Key                NVARCHAR(128),
    @Start              INT,
    @Count              INT,
    @Records            INT OUTPUT,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @Records = 0

    IF ( @Count <= 0 )
    BEGIN

        SELECT * 
            FROM 
                Tbl_Ratna_PluginData
            WHERE 
                SiteId = @SiteId AND
                PluginId = @PluginId AND
                [Key] = @Key

        SET @Records = @@ROWCOUNT

    END
    ELSE
    BEGIN

        -- selected plugindata based on start/size criteria
        DECLARE @SelectedPluginData TABLE
            (
                RowNumber           INT, 
                PluginId            UNIQUEIDENTIFIER     NOT NULL,
                [Key]               NVARCHAR(128)        NOT NULL,
                Id                  NVARCHAR(128)        NOT NULL,
                UId                 UNIQUEIDENTIFIER     NOT NULL,
                RawData             NTEXT,
                CreatedTime         DATETIME    NOT NULL,
                UpdatedTime         DATETIME    NOT NULL
            )

        -- sort plugindata based on updated date
        ;WITH SortedPluginData(RowNumber, PluginId, [Key], Id, UId, RawData, CreatedTime, UpdatedTime) AS
        (    
            SELECT ROW_NUMBER() OVER(ORDER BY UpdatedTime DESC) as RowNumber, 
                        PluginId, [Key], Id, UId, RawData, CreatedTime, UpdatedTime
                    FROM 
                        (SELECT * 
                            FROM 
                                Tbl_Ratna_PluginData
                            WHERE 
                                SiteId = @SiteId AND
                                PluginId = @PluginId AND
                                [Key] = @Key) Tbl
        )

        -- select plugindata
        INSERT INTO @SelectedPluginData
            SELECT *
                FROM 
                    SortedPluginData
                WHERE
                    RowNumber >= @Start AND
                    RowNumber < @Start + @Count

        SELECT *
            FROM @SelectedPluginData

        SELECT @Records = COUNT(UId)
            FROM
                (SELECT * 
                    FROM 
                        Tbl_Ratna_PluginData
                    WHERE 
                        SiteId = @SiteId AND
                        PluginId = @PluginId AND
                        [Key] = @Key) Tbl

    END

END
GO


----------------------------------------------------------
-- Proc_Ratna_Plugin_GetPluginDataForPlugin
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_GetPluginDataForPlugin') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_GetPluginDataForPlugin
GO

CREATE PROCEDURE Proc_Ratna_Plugin_GetPluginDataForPlugin
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @LikeQuery          NVARCHAR(600),
    @Start              INT,
    @Count              INT,
    @Records            INT OUTPUT,
    @ErrorCode          BIGINT OUTPUT
AS
BEGIN


    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @Records = 0

     -- selected plugindata based on start/size criteria
    DECLARE @SelectedPluginData TABLE
        (
            RowNumber           INT, 
            PluginId            UNIQUEIDENTIFIER     NOT NULL,
            [Key]               NVARCHAR(128)        NOT NULL,
            Id                  NVARCHAR(128)        NOT NULL,
            UId                 UNIQUEIDENTIFIER     NOT NULL,
            RawData             NTEXT,
            CreatedTime         DATETIME    NOT NULL,
            UpdatedTime         DATETIME    NOT NULL
        )

    -- sort plugindata based on updated date
    ;WITH SortedPluginData(RowNumber, PluginId, [Key], Id, UId, RawData, CreatedTime, UpdatedTime) AS
    (    
        SELECT ROW_NUMBER() OVER(ORDER BY UpdatedTime DESC) as RowNumber, 
                    PluginId, [Key], Id, UId, RawData, CreatedTime, UpdatedTime
                FROM 
                    TVF_Ratna_PluginData ( @SiteId, @PluginId, @LikeQuery )
    )

    -- select plugindata
    INSERT INTO @SelectedPluginData
        SELECT *
            FROM 
                SortedPluginData
            WHERE
                RowNumber >= @Start AND
                RowNumber < @Start + @Count

    SELECT *
        FROM @SelectedPluginData

    SELECT @Records = COUNT(UId)
        FROM TVF_Ratna_PluginData ( @SiteId, @PluginId, @LikeQuery )

END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_SavePluginData
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_SavePluginData') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_SavePluginData
GO

CREATE PROCEDURE Proc_Ratna_Plugin_SavePluginData
    @SiteId                 INT,
    @PluginId       UNIQUEIDENTIFIER,
    @Key            NVARCHAR(128),
    @Id             NVARCHAR(128),
    @UId            UNIQUEIDENTIFIER,
    @RawData        NTEXT,
    @ErrorCode      BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    DECLARE @Success BIT
    SET @Success = 0
    SET @ErrorCode = 0

    BEGIN TRAN

    IF EXISTS (SELECT 1 FROM Tbl_Ratna_PluginData WHERE SiteId = @SiteId AND PluginId = @PluginId AND [Key] = @Key AND Id = @Id AND UId = @UId)
    BEGIN
        
        UPDATE Tbl_Ratna_PluginData
            SET 
                RawData = @RawData,
                UpdatedTime = GETDATE()
            WHERE
                SiteId = @SiteId AND
                PluginId = @PluginId AND 
                [Key] = @Key AND 
                Id = @Id AND
                UId = @UId

        IF (@@ROWCOUNT = 1)
        BEGIN
            SET @Success = 1 
        END
    
    END
    ELSE
    BEGIN

        -- check if dupe is not being inserted
        IF EXISTS (SELECT 1 FROM Tbl_Ratna_PluginData WHERE SiteId = @SiteId AND PluginId = @PluginId AND [Key] = @Key AND Id = @Id)
        BEGIN
            SET @Success = 0
            SET @ErrorCode = 2001
        END
        ELSE
        BEGIN
            INSERT INTO Tbl_Ratna_PluginData ( SiteId, PluginId, [Key], Id, RawData )
                VALUES ( @SiteId, @PluginId, @Key, @Id, @RawData )

            IF (@@ROWCOUNT = 1)
            BEGIN
                SET @Success = 1 
            END
        END

    END

    IF ( @Success = 1)
    BEGIN
        COMMIT TRAN
    END
    ELSE
    BEGIN
        IF (@ErrorCode = 0)
        BEGIN
            SET @ErrorCode = 1
        END
        ROLLBACK TRAN
    END

END
GO
----------------------------------------------------------
-- Proc_Ratna_Plugin_DeletePluginData
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_DeletePluginData') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_DeletePluginData
GO

CREATE PROCEDURE Proc_Ratna_Plugin_DeletePluginData
    @SiteId                 INT,
    @PluginId       UNIQUEIDENTIFIER,
    @UId            UNIQUEIDENTIFIER,
    @ErrorCode      BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0

    DELETE 
        FROM 
            Tbl_Ratna_PluginData
        WHERE 
            SiteId = @SiteId AND
            PluginId = @PluginId AND
            UId = @UId
END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_DeletePluginDataWithKeyId
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_DeletePluginDataWithKeyId') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_DeletePluginDataWithKeyId
GO

CREATE PROCEDURE Proc_Ratna_Plugin_DeletePluginDataWithKeyId
    @SiteId                 INT,
    @PluginId       UNIQUEIDENTIFIER,
    @Key            NVARCHAR(128),
    @Id             NVARCHAR(128),
    @ErrorCode      BIGINT OUTPUT
AS
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0

    DELETE 
        FROM 
            Tbl_Ratna_PluginData
        WHERE 
            SiteId = @SiteId AND
            PluginId = @PluginId AND
            [Key] = @Key AND
            Id = @Id
END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_DeletePluginDataWithUids
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_DeletePluginDataWithUids') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_DeletePluginDataWithUids
GO

CREATE PROCEDURE Proc_Ratna_Plugin_DeletePluginDataWithUids
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @UIdsXml            NTEXT,
    @ErrorCode          BIGINT OUTPUT
AS
    DECLARE @XmlHandle  INT
BEGIN

    SET NOCOUNT ON

    SET @ErrorCode = 0

    /*
        // Sample XML
        // <UIds><UId Value="--GUID--" /></UIds>
        //
    */
    EXEC sp_xml_preparedocument @XmlHandle output, @UIdsXml

    DELETE 
        FROM 
            Tbl_Ratna_PluginData
        WHERE 
            SiteId = @SiteId AND
            PluginId = @PluginId AND
            UId IN (
                SELECT  
                        Value AS UId
                   FROM 
                        OPENXML (@XmlHandle, '/UIds/UId',1) 
                   WITH (
                            Value UNIQUEIDENTIFIER
                        )
            )
END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_GetPluginDataCountForKey
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_GetPluginDataCountForKey') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_GetPluginDataCountForKey
GO

CREATE PROCEDURE Proc_Ratna_Plugin_GetPluginDataCountForKey
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @KeysXml            NTEXT,
    @ErrorCode          BIGINT OUTPUT
AS
    DECLARE @XmlHandle  INT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0   
    
    
     /*
        // Sample XML
        // <Keys><Key Value="--string--" /></Keys>
        //
    */
    EXEC sp_xml_preparedocument @XmlHandle output, @KeysXml

    SELECT 
            [Key], Count(*) as Count
        FROM 
            Tbl_Ratna_PluginData
        WHERE
            PluginId = @PluginId AND
            SiteId   = @SiteId  AND
            [Key] IN (
                       SELECT  
                            Value AS [Key]
                       FROM 
                            OPENXML (@XmlHandle, '/Keys/Key',1) 
                       WITH (
                                Value NVARCHAR(128)
                            )
                     )
        GROUP BY [Key]

END
GO
