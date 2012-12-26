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

function Copy-AllFiles()
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true)]
        [string] $installfolder,

        [parameter(Mandatory=$true)]
        [string] $filesLocation
    )
    
    Log -Level Info -Message "Installing ratna web files from : [$filesLocation] to [$installfolder]"
    
    # Copy-Item -LiteralPath $filesLocation -Destination $installfolder -Recurse -Force:$true
    xcopy $filesLocation $installfolder /y /c /i /s /q
  
    Log -Level Info -Message "Ratna web files copied to [$installfolder]"
}

function Create-LogFolder()
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true)]
        [string] $installfolder
    )

    $logFolder = Join-Path $installfolder "logs"
    if (-not (Test-Path $logFolder))
    {
        New-Item $logFolder -Type Container | out-null
        Log -Level Info -Message "Created log folder $logFolder"
    }
}

function Update-WebConfig()
{
    [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true)]
        [string] $installfolder,

        [parameter(Mandatory=$true)]
        [string] $DbServer,

        [parameter(Mandatory=$true)]
        [string] $DbName
    )

    Log -Level Info -Message "Updating web config for Ratna"

    $webConfigFile = Join-Path $installfolder "web.config"

    $xml = new-object xml
    $xml.Load( $webConfigFile )

    Log -Level Debug -Message "Changing Database information"

    $dbConnectionString = "Server=$DbServer;Database=$DbName;Trusted_Connection=True;Integrated Security=SSPI;"

    $node = $xml.configuration.appSettings.add | where { $_.key -eq "RatnaDbConnectionString" }
    $node.value = $dbConnectionString

    $logFile = Join-Path $installfolder "logs\ratnalog.txt"

    # set the log4net
    $node = $xml.configuration.log4net.appender.file;
    $node.value = "" + $logFile

    #save the config file and rename
    $updatedwebConfigFile = Join-Path $installfolder "web.config.updated"
    $xml.Save($updatedwebConfigFile)

    Write-Host "Renaming $updatedwebConfigFile to $webConfigFile"

    #remove original one and copy the new one
    Move-Item -LiteralPath $updatedwebConfigFile -Destination $webConfigFile -Force

    Log -Level Info -Message "Updated web config for Ratna"
}

function Create-RatnaSite()
{
   param(
      $siteName,

      $installFolder,

      $poolName
   )
   
   Log -Level Info -Message "Attempting to create site : $siteName, physical path : $installFolder"
   
   $sites = Get-ChildItem "IIS:\Sites\"
   $siteId = 1

   if ( $sites -ne $null )
   {
        $siteId = $sites.Length + 1
   }

   New-Item "IIS:\Sites\$siteName" -bindings @{protocol="http";bindingInformation=":80:"} -physicalPath $installFolder -ID $siteId -Force:$true| out-null
   Set-ItemProperty "IIS:\Sites\$siteName" -name applicationPool -value $poolName

}

function Create-RatnaAppPool()
{
    param(
       [parameter(Mandatory=$true)]
       [string] $poolName,

       [parameter(Mandatory=$true)]
       [switch] $configureIdentity,

       [string] $poolUserName,
       [string] $poolUserPassword
    )

    Log -Level Info -Message "Attempting to create application pool : $poolName"

    $apppool = Get-ChildItem "IIS:\AppPools\" | where { $_.Name -eq $poolName }
    
    if ( $apppool -eq $null )
    {
        $pool = New-Item "IIS:\AppPools\$poolName"

        if ($configureIdentity)
        {
            Log -Level Info -Message "Configuring identity for app pool : $poolName"
            $pool.processModel.username = $poolUserName
            $pool.processModel.password = $poolUserPassword
            $pool.processModel.identityType = "SpecificUser"
            $pool | set-item 
        }

        $pool = Get-Item "IIS:\AppPools\$poolName"
        $pool | set-itemproperty -Name "managedRuntimeVersion" -Value "v4.0"
    }
    else
    {
        Log -Level Info -Message "Application pool [$poolName] already exists"
    }
}


#-------------------------------------------------------------------
# main execution
#-------------------------------------------------------------------

$webFilesLocation = "$Config_package\web"

Copy-AllFiles -installfolder $Config_installat -filesLocation $webFilesLocation
Create-LogFolder -installfolder $Config_installat
Update-WebConfig -installfolder $Config_installat -DbServer $Config_sqlServer -DbName $Config_dbName

if ($Config_createNewPool)
{
    if ($Config_setAppPoolIdentity)
    {
        Create-RatnaAppPool -poolName $Config_apppool -configureIdentity:$true -poolUserName $Config_AppPoolUser -poolUserPassword $Config_AppPoolUserPassword
    }
    else
    {
        Create-RatnaAppPool -poolName $Config_apppool -configureIdentity:$false
    }
}

Create-RatnaSite -siteName $Config_sitename -installFolder $Config_installat -poolName $Config_apppool
