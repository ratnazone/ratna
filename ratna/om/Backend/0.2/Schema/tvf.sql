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

IF OBJECT_ID(N'TVF_Ratna_App_Site', N'TF') IS NOT NULL
    DROP FUNCTION TVF_Ratna_App_Site;
GO

CREATE FUNCTION TVF_Ratna_App_Site(@SiteId INT)
RETURNS @Apps TABLE 
(
    SiteId          INT                 NOT NULL,
    Id              BIGINT              NOT NULL    PRIMARY KEY,
    Name            NVARCHAR(256)       NOT NULL,
    UniqueId        UNIQUEIDENTIFIER    NOT NULL,
    Publisher       NVARCHAR(256)       NOT NULL,
    Description     NVARCHAR(2048)      NOT NULL,
    Url             NVARCHAR(1024)      NOT NULL,
    Scope           INT                 NOT NULL,
    Version         NVARCHAR(64)        NOT NULL,
    Location        NVARCHAR(2048)      NOT NULL,
    [File]          NVARCHAR(64)        ,
    [FileEntry]     NVARCHAR(256)       ,
    [References]    NVARCHAR(2048)      ,
    Enabled         BIT                 NOT NULL,
    IconUrl         NVARCHAR(2048)      NOT NULL,
    RawData         NTEXT
)
AS 
BEGIN

    INSERT 
        INTO  @Apps
            SELECT 
                    SiteId,
                    Id,
                    Name,
                    UniqueId,
                    Publisher,
                    Description,
                    Url,
                    Scope,
                    Version,
                    Location,
                    [File],
                    [FileEntry],
                    [References],
                    Enabled,
                    IconUrl,
                    RawData
                FROM 
                    Tbl_Ratna_App
                WHERE
                    SiteId = @SiteId

    RETURN
END
GO

