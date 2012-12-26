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
$ErrorActionPreference = 'Stop'

# --------------------------------------------------------------------- 
# includes
# ---------------------------------------------------------------------
$lib = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) library.ps1
. $lib

#---------------------------------------------------------
# functions
#---------------------------------------------------------



function UpdateVersionNumber()
{    
    Invoke-SqlCmdForConfiguration -InputFile $versionFile
}

function CreateRatnaDb()
{
  [CmdletBinding()]
    param
    (
        [parameter(Mandatory=$true)]
        [string] $Server,
        
        [parameter(Mandatory=$true)]
        [string] $DbName,
        
        [parameter(Mandatory=$true)]
        [string] $DbFileLocation,
        
        [parameter(Mandatory=$true)]
        [string] $DbTemplateFile
    )
    
      $contents = Get-Content $DbTemplateFile
      $generatedContents = ""

      foreach( $line in $contents)
      {

        $line = $line.Replace("%dbname%", $DbName)
        $line = $line.Replace("%dbfilelocatiom%", $DbFileLocation)

        $generatedContents += $line + [System.Environment]::NewLine
      
      }

      $genDBFile = Join-Path (Split-Path $DBTemplateFile) "db-generated.sql"
      
      $generatedContents > $genDBFile
    
    Invoke-SqlCmd -Server $Server -Database master -InputFile $genDBFile
}

function DoesVersionTableExist()
{
    
    $exists = $false
    
    $sql = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE='BASE TABLE' 
                AND TABLE_NAME='Tbl_Ratna_Version'"
                
    try
    {
        $output = Invoke-SqlCmdForConfiguration -Query $sql          
        if ($output -ne $null)
        {
            $exists = $true
        }  
    }
    catch
    {
    }
    
    return $exists
}


function UpgradeDb()
{
    
    $sql = "SELECT major, minor FROM Tbl_Ratna_Version WHERE Installed = 0"
    $output = Invoke-SqlCmdForConfiguration -Query $sql
    
    if ($output -ne $null)
    {
        foreach($data in $output)
        {
            $major = $data["major"]
            $minor = $data["minor"]
            
            $version = "$major.$minor"
            $vpath = Join-Path $sqlBuildLocation $version
            if (Test-Path $vpath)
            {
                $vSchemaFile = Join-Path $vPath "schema.sql"
                if (Test-Path $vSchemaFile)
                {
                    Log -Level Info -Message "Installing $vSchemaFile"
                    Invoke-SqlCmdForConfiguration -InputFile $vSchemaFile
                    Log -Level Info -Message "Installed."
                }
                
                $vProcFile = Join-Path $vPath "proc.sql"
                if (Test-Path $vSchemaFile)
                {
                    Log -Level Info -Message "Installing $vProcFile"
                    Invoke-SqlCmdForConfiguration -InputFile $vProcFile
                    Log -Level Info -Message "Installed."
                }
                
                $sql = "UPDATE Tbl_Ratna_Version SET Installed = 1 WHERE major = $major AND minor =$minor"
                Invoke-SqlCmdForConfiguration -Query $sql
            }
        }
    }
    
    
}

#-------------------------------------------------------------------
# main execution
#-------------------------------------------------------------------

$sqlBuildLocation = "$Config_package\db"

$dbFile = "$sqlBuildLocation\db.sql"
$firstSchemaFile = "$sqlBuildLocation\schema.sql"
$versionFile = "$sqlBuildLocation\versions.sql"


if ($Config_DbCreateNew)
{
  Log -Level Info -Message "Checking if database [$Config_dbName] exists"
  $exists = CheckDatabaseExists -Server $Config_sqlServer -Database $Config_dbName
  if (-not $exists )
  {
     Log -Level Info -Message "Database [$Config_dbName] does not exist. Creating"
     CreateRatnaDb -Server $Config_sqlServer -DbName $Config_dbName -DbFileLocation $Config_dbFileLocation -DbTemplateFile $dbFile
  }
}

Log -Level Info -Message "Checking if version schema exists"
$exists = DoesVersionTableExist

if ($exists -eq $false)
{
    Log -Level Info -Message "No versions found. Installing version schema"

    # run the original schema
    Invoke-SqlCmdForConfiguration -InputFile $firstSchemaFile
}

Log -Level Info -Message "Inserting versions"

UpdateVersionNumber

Log -Level Info -Message "Upgrading database"

#upgrade db
UpgradeDb

Log -Level Info -Message "Upgrade done."
