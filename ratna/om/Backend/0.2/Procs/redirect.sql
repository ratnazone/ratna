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
---------
-------------------------------------------------
-- Proc_Ratna_Redirect_GetUrl
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Redirect_GetUrl') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Redirect_GetUrl
GO

CREATE PROCEDURE Proc_Ratna_Redirect_GetUrl
    @SiteId              INT,
    @Url                 NVARCHAR(1024),
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    SELECT 
            RedirectUrl
        FROM
            Tbl_Ratna_Redirect
        WHERE
            SiteId = @SiteId AND
            Url = @Url


END
GO

----------------------------------------------------------
-- Proc_Ratna_Redirect_AddUrl
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Redirect_AddUrl') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Redirect_AddUrl
GO

CREATE PROCEDURE Proc_Ratna_Redirect_AddUrl
    @SiteId              INT,
    @Url                 NVARCHAR(1024),
    @RedirectUrl         NVARCHAR(1024),
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    IF (NOT EXISTS(SELECT 1 FROM Tbl_Ratna_Redirect WHERE SiteId = @SiteId AND Url = @Url))
    BEGIN

    INSERT INTO
            Tbl_Ratna_Redirect
        ( SiteId, Url, RedirectUrl)
        VALUES ( @SiteId, @Url, @RedirectUrl )

    END
    ELSE
    BEGIN
        
        UPDATE
            Tbl_Ratna_Redirect
        SET
            RedirectUrl = @RedirectUrl
        WHERE 
            SiteId = @SiteId AND
            Url = @Url

    END

END
GO

----------------------------------------------------------
-- Proc_Ratna_Redirect_Delete
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Redirect_Delete') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Redirect_Delete
GO

CREATE PROCEDURE Proc_Ratna_Redirect_Delete
    @SiteId              INT,
    @Url                 NVARCHAR(1024),
    @ErrorCode           BIGINT    OUTPUT
AS
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0

    
        
    DELETE FROM
        Tbl_Ratna_Redirect
    WHERE 
        SiteId = @SiteId AND
        Url = @Url


END
GO

