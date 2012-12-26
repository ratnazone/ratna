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

function Remove-AllFiles()
{
  [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true)]
        [string] $installfolder
    )
    
    Log -Level Info "Uninstalling ratna web files from : $installfolder"
    
    if (Test-Path $installfolder)
    {
       Remove-Item -Path $installfolder -Recurse    | out-null   
       Log -Level Info "Ratna web files are removed."
    }
    else
    {
    Log -Level Info "Ratna web files not found."
    }
  
}

function Remove-RatnaSite()
{
   param(
      $siteName
   )
   
   Log -Level Info -Message "Attempting to remove site : $siteName"
   
   $site = Get-ChildItem "IIS:\Sites\" | where { $_.Name -eq $siteName }
   
   if ( $site -eq $null )
   {
      Log -Level Info -Message "Website [$siteName] not found"
   }
   else
   {
      Remove-Item "IIS:\Sites\$siteName" -Recurse | out-null
      Log -Level Info -Message "Website [$siteName] removed"
   }
}

function Remove-RatnaAppPool()
{
    param(
       $poolName
    )

    Log -Level Info -Message "Attempting to remove application pool : $poolName"
    $apppool = Get-ChildItem "IIS:\AppPools\" | where { $_.Name -eq $poolName }
    
    if ( $apppool -eq $null )
    {
        Log -Level Info -Message "Application pool [$poolName] not found"
    }
    else
    {
        Remove-Item "IIS:\AppPools\$poolName" -Recurse | out-null
        Log -Level Info -Message "Application pool [$poolName] removed"
    }
}        

# --------------------------------------------------------------------- 
# execution
# ---------------------------------------------------------------------

Remove-RatnaSite -siteName $Config_sitename
Remove-RatnaAppPool -poolName $Config_apppool
Remove-AllFiles -installfolder $Config_installat

