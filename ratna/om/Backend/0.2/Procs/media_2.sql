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
-- Proc_Ratna_Media_GetByUrls
--
-- Returns media information for the given urls
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Media_GetByUrls') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Media_GetByUrls
GO

CREATE PROCEDURE Proc_Ratna_Media_GetByUrls
    @SiteId           INT,
    @UrlsXml          NTEXT,
    @ErrorCode        BIGINT OUTPUT
AS
    DECLARE @XmlHandle  INT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    /*
        // Sample XML
        // <Urls><Url Value="/2011/11/8/9/myphoto.png" /></Urls>
        //
    */

    EXEC sp_xml_preparedocument @XmlHandle output, @UrlsXml

    -- grab media for url match. for the urls that don't match, they will not
    -- be included.
    SELECT Media.*
        FROM
            OPENXML (@XmlHandle, '/Urls/Url',1) 
            WITH (
                    Value NVARCHAR(80)
                )
            SelectedUrls
        INNER JOIN TVF_Ratna_Media(@SiteId, 0) Media 
            ON Media.Url = SelectedUrls.Value

    -- select the tags for the media
    SELECT 
            Media.ResourceId,
            Tag as Name,
            Weight
        FROM
            OPENXML (@XmlHandle, '/Urls/Url',1) 
            WITH (
                    Value NVARCHAR(80)
                )
            SelectedUrls
        INNER JOIN TVF_Ratna_Media(@SiteId, 0) Media 
            ON Media.Url = SelectedUrls.Value
        INNER JOIN Tbl_Ratna_Tags
            ON Tbl_Ratna_Tags.ResourceId = Media.ResourceId
        WHERE 
            Tbl_Ratna_Tags.SiteId = @SiteId


END
GO

