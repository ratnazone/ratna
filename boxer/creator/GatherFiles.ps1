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

function GatherForCD()
{
    param(
        [string] $outputFolder,
        [string] $flavor
    )

    $nuke = $true

    $installerFolder = "..\installer"
    $readMeFile = "..\readme.txt"
    $eulaFile = "..\eula.txt"

    $souceWebFolder = "..\..\target\ratna\web\$flavor\_PublishedWebsites\web"

    $sourceSqlFolder = "..\..\build\ratna\om\*"

    if ( Test-Path $outputFolder )
    {
        if ( $nuke )
        {
            Remove-Item $outputFolder -Recurse
        }
    }

    if ( -not (Test-Path $outputFolder) )
    {
         New-Item -ItemType Container -Path $outputFolder | Out-Null
    }

    $sqlOutputFolder = Join-Path $outputFolder db
    if ( -not (Test-Path $sqlOutputFolder) )
    {
         New-Item -ItemType Container -Path $sqlOutputFolder | Out-Null
    }

    Write-Host "Copying readme file to [$outputFolder]"
    #copy the readme file and eula file
    Copy-Item -Path $readMeFile -Destination $outputFolder | Out-Null
    Copy-Item -Path $eulaFile   -Destination $outputFolder | Out-Null

    Write-Host "Copying installer bits to [$outputFolder]"
    #copy the installer bits
    Copy-Item -Path $installerFolder -Destination $outputFolder -Recurse | Out-Null
 
     Write-Host "Copying website files to [$outputFolder]"
     #copy the web bits
     Copy-Item -Path $souceWebFolder -Destination $outputFolder -Recurse | Out-Null 
 
     Write-Host "Copying db files to [$outputFolder]"
     #copy the sql bits
     Copy-Item -Path $sourceSqlFolder -Destination $sqlOutputFolder -Recurse | Out-Null 
 
     Write-Host "Done"

 }

 #---------------------------------------------------
 # main execution
 #---------------------------------------------------
 

 if ($args.Length -lt 2)
 {
     Write-Host -Foreground Red "Missing output folder input"
 }
 else
 {
    GatherForCD -outputFolder $args[0] -flavor $args[1]
 }
