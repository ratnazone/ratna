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
# this controls the main upgrader action.

# --------------------------------------------------------------------- 
# includes
# ---------------------------------------------------------------------
$presentDir = (Split-Path -parent $MyInvocation.MyCommand.Definition)
$lib = Join-Path $presentDir "..\library\library.ps1"
. $lib

$presentVersion = "0.2"

function SetUpgradeFile()
{
    param(
      $version
    )
    
    # check for upgrade file.
    $upgradeFile = Join-Path $Config_installat "upgrade.log"

    #create and set the contents with version
    Set-Content -LiteralPath $upgradeFile -value $version

}

function Upgrade()
{
    $upgradeFile = EnsureUpgradeFile
    $version = Get-Content $upgradeFile
    
    if ($version -ne $null)
    {
       $tokens = $version.Split('.')
       $tmajor = $tokens[0]
       $tminor = $tokens[1]
    
       $sql = "SELECT major, minor FROM Tbl_Ratna_Version WHERE major >= $tmajor"
       $output = Invoke-SqlCmdForConfiguration -Query $sql
       
       foreach($data in $output)
       {
            $major = $data["major"]
            $minor = $data["minor"]
            
            $xversion = "$major.$minor"
            
            if ((($major -eq $tmajor) -and ($minor -gt $tminor)) -or
                 ($major -gt $tmajor))
            {
                           
                # run the upgrade
                $upgradeDir = Join-Path $presentDir $xversion
                                
                Log -Level Info -Message "`t upgrader version - [$xversion]"
                
                $upgradeFile = Join-Path $upgradeDir "upgrade.ps1"
                if (Test-Path $upgradeFile)
                {
                    # execute the upgrader
                    . $upgradeFile
                }
                
            }
            
       }
       
    }
    
    # overwrite the version 
    SetUpgradeFile -version $presentVersion
}

function EnsureUpgradeFile()
{
  # check for upgrade file.
  $upgradeFile = Join-Path $Config_installat "upgrade.log"

  if (-not (Test-Path $upgradeFile))
  {
      #create and set the contents with version
      SetUpgradeFile -version "0.1"
  }
  
  return $upgradeFile
}

Upgrade

