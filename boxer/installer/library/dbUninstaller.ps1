<#
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

#>
# --------------------------------------------------------------------- 
# includes
# ---------------------------------------------------------------------
$lib = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) library.ps1
. $lib

# --------------------------------------------------------------------- 
# functions
# ---------------------------------------------------------------------

function DropDatabase()
{
  [CmdletBinding()]
	param
	(
		[parameter(Mandatory=$true)]
		[string] $Server,

		[parameter(Mandatory=$true)]
		[string] $Database
	)
	
  $sql = "use [master]
        ALTER DATABASE [$Database] set single_user with rollback immediate
        DROP DATABASE [$Database]"
     
  Invoke-SqlCmd -Server $Server -Database master -Query $sql
}        

# --------------------------------------------------------------------- 
# execution
# ---------------------------------------------------------------------

Log -Level Info "Uninstalling ratna database from : Server[$Config_sqlServer] Database[$Config_dbName]"
$exists = CheckDatabaseExists -Server $Config_sqlServer -Database $Config_dbName

if ($exists)
{
    DropDatabase -Server $Config_sqlServer -Database $Config_dbName
}

Log -Level Info "Ratna database is removed."
