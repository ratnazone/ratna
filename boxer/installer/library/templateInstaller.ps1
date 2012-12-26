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
# install templates from the extensions

# --------------------------------------------------------------------- 
# includes
# ---------------------------------------------------------------------
$lib = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) library.ps1
. $lib

# --------------------------------------------------------------------- 
# functions
# ---------------------------------------------------------------------

function InstallTemplates()
{
  param(
    [string] $InstallDirectory,
    [string] $PublishedDirectory
  )
  
  if (-not (Test-Path $InstallDirectory))
  {
     Write-Host -Foreground Red "Unable to find the install directory - $InstallDirectory"
  }
  else
  {
  
     $TemplatesInstallDirectory = Join-Path $InstallDirectory "templates\$config_SiteId"
     $TemplatesPublishedFiles = $PublishedDirectory
  
     Log -Level Info -Message "Installing template files from : [$TemplatesPublishedFiles] to [$TemplatesInstallDirectory]"
     Copy-Item -Recurse -Force -Path $TemplatesPublishedFiles -Destination $TemplatesInstallDirectory
  }
  
}

function main()
{

  param(
    [string] $RatnaRootDirectory,
    [string] $TemplatesPublishedDirectory
  )

  if (-not (IsAdminMode))
  {
      Write-Host -Foreground Red "Templates can only be installed in (Run As Administrator) mode"
      return
  }
  else
  {
    InstallTemplates -InstallDirectory $RatnaRootDirectory -PublishedDirectory $TemplatesPublishedDirectory
  }

}

#-------------------------------------------------------------------
# main execution
#-------------------------------------------------------------------

main -RatnaRootDirectory $Config_installat -TemplatesPublishedDirectory "C:\depot\build\Ratna\extensions\templates"
