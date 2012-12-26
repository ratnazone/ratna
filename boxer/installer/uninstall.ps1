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
$lib = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "library\library.ps1"
. $lib

# --------------------------------------------------------------------- 
# functions
# ---------------------------------------------------------------------

function ConfirmConfigParameters()
{
    Write-Host -Foreground Yellow "Please confirm the following settings to continue"
    Write-Host "******************************************************************"
    Write-Host "Sql Server - $Config_SqlServer"
    Write-Host "Database   - $Config_dbName"
    Write-Host ""
    Write-Host "Folder     - $Config_installat"
    Write-Host "******************************************************************"
    
    $title = "Continue Ratna Uninstallation"
    $message = "Do you want to uninstall Ratna ?"

    $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", `
       "Uninstalls Ratna."

    $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", `
        "Quits without uninstalling Ratna."

    $options = [System.Management.Automation.Host.ChoiceDescription[]]($yes, $no)
    $result = $host.ui.PromptForChoice($title, $message, $options, 0) 


    $flag = $false
    if ( $result -eq 0)
    {
      $flag = $true
    }
    
    return $flag
}

# --------------------------------------------------------------------- 
# execution
# ---------------------------------------------------------------------

if (-not (IsAdminMode))
{
    Log -Level Warning -Message "Please execute in Administrator mode"
    return
}

if (-not (Load-ModulesAndSnapins))
{
    Log -Level Warning -Message "Prerequisites not met. Quiting without un-installating."
    return
}

$continue = ConfirmConfigParameters
if (-not $continue)
{
    Log -Level Warning -Message "User cancelled operation. Quiting without un-installating."
    return
}

$setone = $false
$uninstall_db = $false       
$uninstall_web = $false       

foreach($arg in $args)
{
    if ( "-db".Equals( $arg, [System.StringComparison]::OrdinalIgnoreCase))
    {
       $uninstall_db = $true      
       $setone = $true 
    }
    if ( "-web".Equals( $arg, [System.StringComparison]::OrdinalIgnoreCase))
    {
       $uninstall_web = $true      
       $setone = $true 
    }
}

if (-not $setone)
{
    $uninstall_db = $true
    $uninstall_web = $true
}    

if ($uninstall_db)
{
    $uninstalldbCode = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "library\dbUninstaller.ps1"
    . $uninstalldbCode
}

if ($uninstall_web)
{
    $uninstallwebCode = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "library\webUninstaller.ps1"
    . $uninstallwebCode
}
