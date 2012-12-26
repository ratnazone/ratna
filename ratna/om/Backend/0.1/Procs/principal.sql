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
-- Proc_Ratna_Principal_Find
----------------------------------------------------------

IF ( OBJECT_ID('Proc_Ratna_Principal_Find') IS NOT NULL ) 
   DROP PROCEDURE Proc_Ratna_Principal_Find
GO

CREATE PROCEDURE Proc_Ratna_Principal_Find
    @SiteId                 INT,
    @Query            NVARCHAR(80),
    @ErrorCode        BIGINT    OUTPUT
AS
    DECLARE @PrincipalId BIGINT,
            @Name         NVARCHAR(80),
            @IsGroup     BIT
BEGIN

    SET NOCOUNT ON
    SET @ErrorCode = 0
    SET @IsGroup = 0

    SELECT 
            @Name = Name, 
            @PrincipalId = Id
        FROM 
            Tbl_Ratna_Principal
        WHERE
            SiteId = @SiteId AND 
            Name = @Query

    IF (@@ROWCOUNT = 1)
    BEGIN

        IF EXISTS (SELECT 1 FROM Tbl_Ratna_Group WHERE SiteId = @SiteId AND PrincipalId = @PrincipalId)
        BEGIN
            SET @IsGroup = 1
        END

        SELECT @Name as Name, @PrincipalId as PrincipalId, @IsGroup as IsGroup
    END


END
GO
