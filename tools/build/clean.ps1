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

function Log-Build()
{
  [CmdletBinding()]
  param($Level, $Message)
  
  if ($Level -eq "Info")
  {
    Write-Host "[$Level] [clean] $Message"
  }
}

Function Get-Dirs()
{
    [CmdletBinding()]
    param($DirsFile)
    
    if ($DirsFile -eq $null)
    {
      return
    }
    
   $exists = Test-Path $DirsFile
   
   $dirs = @()
   
   if ($exists)
   {
      $lines = Get-Content $DirsFile
      
      foreach($line in $lines)
      {
        $dirs += $line
      }
   }
   
   return $dirs
}

function Clean-Project()
{
   [CmdletBinding()]
   param($BuildFile, $Flavor)
 
   [xml]$buildXml = get-content $BuildFile
        
   $project_file = $buildXml.build.project
   $target_folder_base = $cobra_target_base + "\" + $buildXml.build.target
   $output_folder = $target_folder_base + "\" + $Flavor
   $build_folder = $cobra_build_base + "\" + $buildXml.build.target + "\"
   
   # if the build folder exists, clean it
   if ((Test-Path $build_folder))
   {
       Log-Build -Level Info -Message "Deleting folder $build_folder"
       Remove-Item $build_folder -Recurse | Out-Null 
            
   }
   
   # if the target folder exists, clean it
   if ((Test-Path $output_folder))
   {
       $parentPath = (Get-Item $output_folder).Parent
   
       Log-Build -Level Info -Message "Deleting folder $output_folder"
       Remove-Item $output_folder -Recurse | Out-Null
       
       $targetPath = (Get-Item $target_folder_base)
       
       while($true)
       {
            #keep on cleaning parent path, till hit the target folder.
            $childItems = Get-ChildItem $parentPath.FullName
            if ($childItems -ne $null)
            {
                break;
            }
            
            $oldParentPath = $parentPath
            $parentPath = $parentPath.Parent
            Log-Build -Level Info -Message "Removing $($oldparentPath.FullName)"
            Remove-Item $oldparentPath.FullName
            if ($parentPath -eq $targetPath)
            {
               break
            }
       }
       
   }
        
    #ignore if there is no project file defined or does not
    #exists on the folder.
    if (($project_file -ne $null) -and (Test-Path $project_file))
    {               
        msbuild /t:Clean   
    }
}

function Clean-Dirs()
{
  [CmdletBinding()]
  param($Dirs, $BuildFileName, $DirsFile, $Flavor)
  
  foreach($dir in $Dirs)
  {
    cd $dir
    
    $internaldirs = Get-Dirs -DirsFile $DirsFile
    if ($internaldirs.Length -gt 0 )
    {
      Clean-Dirs -Dirs $internaldirs -DirsFile $DirsFile  -BuildFileName $BuildFileName -Flavor $Flavor
      cd ..
    }
    else
    {
      Log-Build -Level "Info" -Message "Cleaning from $dir"
      
      Clean-Project -BuildFile $BuildFileName -Flavor $Flavor
      
      cd ..
    }
  }
}

Function Check-BuildFileExists()
{
    [CmdletBinding()]
    param($BuildFile)
    
    return Test-Path $BuildFile
    
}

#############################################
# main execution
#############################################
$flavor= "debug"

if ($args.Length -eq 1 )
{
    if ($args[0] -eq "-ship")
    {
        $flavor = "ship"
    }
}

$buildFileName = "build.cobra"
$dirsFileName = "dirs"

$dirs = Get-Dirs -DirsFile $dirsFileName
if ($dirs.Length -gt 0 )
{
  Log-Build -Level "Verbose" -Message "$dirsFileName found"  
  Clean-Dirs -Dirs $dirs -BuildFileName $buildFileName -DirsFile $dirsFileName -Flavor $flavor
}
else
{
    Log-Build -Level "Verbose" -Message "Checking build file exists: $buildFileName"
    $buildFileExists = Check-BuildFileExists -BuildFile $buildFileName

    if (-not $buildFileExists)
    {
      Help -Message "Unable to locate build file: $buildFileName"
      exit -1
    }
    else
    {
        Log-Build -Level "Verbose" -Message "Calling clean"
        Clean-Project -BuildFile $buildFileName -Flavor $flavor
    }
}
