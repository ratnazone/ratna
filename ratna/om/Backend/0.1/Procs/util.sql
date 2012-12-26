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
-- Proc_Ratna_GetNavigationBuilderData
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetNavigationBuilderData') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetNavigationBuilderData
GO

CREATE PROCEDURE Proc_Ratna_GetNavigationBuilderData
    @SiteId                INT,
    @UrlKeysXml            NTEXT,
    @ErrorCode             BIGINT    OUTPUT
AS
    DECLARE     @XmlHandle  INT
    DECLARE     @UrlKeysTable     TABLE   ( UrlKey NVARCHAR(512) )
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    /*
        // Sample XML
        // <UrlKeys><UrlKey Value="/default" /></UrlKeys>
        //
    */
    EXEC sp_xml_preparedocument @XmlHandle output, @UrlKeysXml

    INSERT INTO @UrlKeysTable ( UrlKey )
        SELECT  
              Value
        FROM 
            OPENXML (@XmlHandle, '/UrlKeys/UrlKey',1) 
            WITH (
                Value NVARCHAR(512)
                )

    -- select the title and key
    SELECT 
            Title, 
            UrlKey
        FROM
            Tbl_Ratna_Article
        WHERE
            SiteId = @SiteId AND
            UrlKey IN ( SELECT UrlKey FROM @UrlKeysTable )
        ORDER BY
            UrlKey

END
GO

----------------------------------------------------------
-- Proc_Ratna_GetChildNavigationBuilderData
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_GetChildNavigationBuilderData') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_GetChildNavigationBuilderData
GO

CREATE PROCEDURE Proc_Ratna_GetChildNavigationBuilderData
    @SiteId                INT,
    @UrlKey                NVARCHAR(512),
    @ErrorCode             BIGINT    OUTPUT
AS
    DECLARE @LikeQuery      NVARCHAR(514)
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    SET @LikeQuery = @UrlKey + '%'

    -- select the title and key
    SELECT 
            Title, 
            UrlKey
        FROM
            Tbl_Ratna_Article
        WHERE
            SiteId = @SiteId AND
            UrlKey LIKE @LikeQuery
        ORDER BY
            UrlKey
END
GO

