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
-- version for Ratna

IF NOT EXISTS(SELECT 1 FROM Tbl_Ratna_Version WHERE Major = 0 AND Minor = 1)
BEGIN
    INSERT INTO Tbl_Ratna_Version (Major, Minor) VALUES ( 0, 1 )
END

IF NOT EXISTS(SELECT 1 FROM Tbl_Ratna_Version WHERE Major = 0 AND Minor = 2)
BEGIN
    INSERT INTO Tbl_Ratna_Version (Major, Minor) VALUES ( 0, 2 )
END
