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
# ------------------------------------------------------------------------------------------------- 
# configuration
# -------------------------------------------------------------------------------------------------

$sysdrive = (Get-ChildItem env:SystemDrive).Value
$iisroot = "$sysdrive\inetpub\wwwroot"

# -------------------------------------------------------------------------------------------------
# Database parameters
#    
# $Config_DbCreateNew : Set this parameter to $true, if you want the installer to create a new db.
#
# $Config_dbFileLocation : When the $config_DbCreateNew parameter is set to $true, this parameter 
#                       must be defined. This parameter tells the installer where to store the
#                       database files.
#
# $Config_UseUserName : Set this parameter to $false, when the SQL server can be accessed through 
#                       Integrated security. 
#                       
# -------------------------------------------------------------------------------------------------

$Config_DbCreateNew = $false
$Config_dbFileLocation = "C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA"

$Config_UseUserName = $false
$Config_dbName="ratna"
$Config_sqlServer="-- enter SQL Server --"
$Config_UserName=$null
$Config_Password=$null


# -------------------------------------------------------------------------------------------------
# IIS parameters
#
# $Config_AppPooluser : The account that will be used to run the application pool. This account
#                       must have SQL rights for the database specified with $Config_dbName
#   
# -------------------------------------------------------------------------------------------------
$Config_sitename = "ratna"
$Config_apppool = "ratna-pool"
$Config_installat = "$iisroot\ratna"

$Config_createNewPool = $true
$Config_setAppPoolIdentity = $true
$Config_AppPoolUser="-- enter App Pool User --"
$Config_AppPoolUserPassword="-- enter App Pool User Password --"

# -------------------------------------------------------------------------------------------------
# Site Provision parameters
#
# $Config_AdminAlias    : Administrator Alias
# $Config_AdminPassword : Password the for administrator
# $Config_AdminEmail    : Email associated with the administrator
# -------------------------------------------------------------------------------------------------

$Config_AdminAlias = "admin"
$Config_AdminPassword = "password"
$Config_AdminEmail = "myemail@ratnazone.com"

$Config_Site = "default"            # can set the value "<yourdomain>.com"
$Config_SiteTitle = "default site"

# -------------------------------------------------------------------------------------------------
$Config_package = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) ".."
