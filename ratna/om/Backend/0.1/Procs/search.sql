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
-- Proc_Ratna_AddSearchQuery
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_AddSearchQuery') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_AddSearchQuery
GO

CREATE PROCEDURE Proc_Ratna_AddSearchQuery
    @SiteId                 INT,
    @Query            NVARCHAR(80),
    @SearchType        INT,
    @Success        BIT
AS
BEGIN

    -- search type defined
    -- 1 : article
    -- 2 : pages
    -- 3 : 

    SET NOCOUNT ON


END
GO

