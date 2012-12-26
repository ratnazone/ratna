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
-- Proc_Ratna_Plugin_GetPluginDataInRange
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Plugin_GetPluginDataInRange') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Plugin_GetPluginDataInRange
GO

CREATE PROCEDURE Proc_Ratna_Plugin_GetPluginDataInRange
    @SiteId             INT,
    @PluginId           UNIQUEIDENTIFIER,
    @UpdateAfter        DATETIME,
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
            UpdatedTime > @UpdateAfter

END
GO

----------------------------------------------------------
-- Proc_Ratna_Plugin_GetPluginDataForKey
-- Bug fix for version 0.1
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
            ORDER BY
                UpdatedTime DESC

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

