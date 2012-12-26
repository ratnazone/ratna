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
-------------------------------------------------------------------------------
--
-- Tbl_Ratna_App : App Information
--
-------------------------------------------------------------------------------
CREATE TABLE Tbl_Ratna_App
(
    SiteId          INT                 NOT NULL,
    Id              BIGINT              NOT NULL    IDENTITY ( 1, 1 ) PRIMARY KEY,
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
    Enabled         BIT                 NOT NULL    DEFAULT(0),
    IconUrl         NVARCHAR(2048)      NOT NULL,
    RawData         NTEXT
)

GO


ALTER TABLE Tbl_Ratna_App
    ADD CONSTRAINT UK_App_SiteId UNIQUE (SiteId, UniqueId)

GO

-------------------------------------------------------------------------------
--
-- Tbl_Ratna_Redirect : Redirect Url information
--
-------------------------------------------------------------------------------
CREATE TABLE Tbl_Ratna_Redirect
(
    SiteId          INT                  NOT NULL,
    Url             NVARCHAR(1024)       NOT NULL,
    RedirectUrl     NVARCHAR(1024)       NOT NULL
)

GO

