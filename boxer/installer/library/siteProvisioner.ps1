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
<#
   Provisions the site.
#>

$ErrorActionPreference = 'Stop'

# --------------------------------------------------------------------- 
# includes
# ---------------------------------------------------------------------
$lib = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) library.ps1
. $lib

#----------------------------------------------------------------------
# functions
#----------------------------------------------------------------------

function ProvisionSite
{

    param
    (
        [string] $SiteName,
        [string] $SiteTitle,
        [string] $email,
        [string] $alias,
        [string] $password
    )

    $Query = "DECLARE @ErrorCode BIGINT,
                           @siteId INT
                   
                   SELECT @siteId = Id FROM Tbl_Ratna_Site WHERE Host = '$SiteName';

                   IF (@@ROWCOUNT = 0)
                   BEGIN
                        EXEC Proc_Ratna_CreateSite '$SiteName', '$SiteTitle' , @ErrorCode OUTPUT;
                   END

                   -- read the site id again.
                   SELECT @siteId = Id FROM Tbl_Ratna_Site WHERE Host = '$SiteName';

                   EXEC Proc_Ratna_ProvisionSite @siteId, '$email', '$alias', '$password', @ErrorCode OUTPUT
    ";

    if ($Config_UseUserName)
    {
        Invoke-SqlCmd -Server $Config_sqlServer -Database $Config_dbName -UserName $Config_UserName -Password $Config_Password -Query $Query | Out-Null
    }
    else
    {
        Invoke-SqlCmd -Server $Config_sqlServer -Database $Config_dbName -Query $Query | Out-Null
    }

}

function GetPasswordHash()
{
    param
    (
        [string] $Password
    )

    $encoding =  [System.Text.UTF8Encoding]::UTF8
    $bytes = $encoding.GetBytes($Password)

    $shaM = new-object -TypeName System.Security.Cryptography.SHA256Managed
    $result = $shaM.ComputeHash($bytes);

    $hash = [System.Convert]::ToBase64String($result);

    return $hash
}

#----------------------------------------------------------------------
# main execution
#----------------------------------------------------------------------

# get the hash of the password.

$flag = $true

if ( [System.String]::IsNullOrEmpty($Config_Site) )
{
    $flag = $false
    Log -Level Error -Message "Config_Site is null or empty"
}

if ( [System.String]::IsNullOrEmpty($Config_AdminEmail) )
{
    $flag = $false
    Log -Level Error -Message "Config_AdminEmail is null or empty"
}

if ( [System.String]::IsNullOrEmpty($Config_AdminAlias) )
{
    $flag = $false
    Log -Level Error -Message "Config_AdminAlias is null or empty"
}

if ( [System.String]::IsNullOrEmpty($Config_AdminPassword) )
{
    $flag = $false
    Log -Level Error -Message "Config_AdminPassword is null or empty"
}

if ($flag)
{
    $password = GetPasswordHash -Password $Config_AdminPassword

    ProvisionSite -SiteName $Config_Site -SiteTitle $Config_SiteTitle -email $Config_AdminEmail -alias $Config_AdminAlias -password $password
}
