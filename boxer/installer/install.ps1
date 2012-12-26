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
    Write-Host "Sql Server  - $Config_SqlServer"
    Write-Host "Database    - $Config_dbName"
    Write-Host ""
    Write-Host "Folder      - $Config_installat"
    Write-Host "Admin alias - $Config_AdminAlias"
    Write-Host "******************************************************************"
    
    $title = "Continue Ratna Installation"
    $message = "Do you want to install Ratna with the specified parameters?"

    $yes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", `
       "Installs Ratna with the specified parameters."

    $no = New-Object System.Management.Automation.Host.ChoiceDescription "&No", `
        "Quits without installing Ratna."

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
    Log -Level Warning -Message "Error 100: Please execute in Administrator mode"
    return
}

$continue = ConfirmConfigParameters
if (-not $continue)
{
    Log -Level Warning -Message "User cancelled operation. Quiting without installation"
    return
}

$setone = $false
$install_db = $false
$install_web = $false

foreach($arg in $args)
{
    if ( "-db".Equals( $arg, [System.StringComparison]::OrdinalIgnoreCase))
    {
       $install_db = $true      
       $setone = $true 
    }
    if ( "-web".Equals( $arg, [System.StringComparison]::OrdinalIgnoreCase))
    {
       $install_web = $true      
       $setone = $true 
    }
}

if (-not $setone)
{
    $install_db = $true
    $install_web = $true
} 

Load-ModulesAndSnapins

if ($install_db)
{
    Log -Level Info -Message "Installing Ratna db"
    $installdbCode = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "library\dbInstaller.ps1"
    . $installdbCode

    Log -Level Info -Message "Provisioning Site"
    $provisionSiteCode = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "library\siteProvisioner.ps1"
    . $provisionSiteCode
}

if ($install_web)
{
    Log -Level Info -Message "Installing Ratna web"
    $installwebCode = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "library\webInstaller.ps1"
    . $installwebCode
    
    # run the upgrader    
    $upgradeCode = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "upgrader\upgrader.ps1"
    Log -Level Info -Message "Executing upgrader"
    . $upgradeCode

     Log -Level Info -Message "Upgrader done."
}
