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
####################################################################
#
# Powershell code to count the lines of code.
#
# Usage -
# c:\>linecounter.ps1 -Location <path>
#
# Please modify the variable $CodeFiles to add/remove
# file extensions. 
#
# Author : Jardalu (http://jardalu.com)
# Modified from Lee Holmes's code (http://www.leeholmes.com/blog/2006/10/18/counting-lines-of-source-code-in-powershell/)
#
####################################################################

param(
    [string] $Location
)

function CountLines
{ 
    [CmdletBinding()]
    param(
        [string] $directory,
        [string] $pattern
    )

    $directories = [System.IO.Directory]::GetDirectories($directory) 
    $files = [System.IO.Directory]::GetFiles($directory, $pattern) 

    $lineCount = 0 

    foreach($file in $files) 
    { 
        $lineCount += [System.IO.File]::ReadAllText($file).Split("`n").Count 
    } 

    foreach($subdirectory in $directories) 
    { 
        $lineCount += CountLines -directory $subdirectory -pattern $pattern
    } 

    return $lineCount 
} 

if ( $Location -eq $null )
{
    $Location = Get-Location
}

$CodeFiles = @( "*.cs", "*.asax", "*.aspx", "*.asmx", "*.ascx", "*.js", "*.css", "*.sql", "*.ps1" )

Write-Output "Scanning code lines for [$Location]"
Write-Output ""

$total = 0

foreach ($codeFile in $CodeFiles)
{
    $lc = CountLines -directory $Location -pattern $codeFile
    $total += $lc
    
    $displayCodeFile = $codeFile.Substring(2)
    
    Write-Output "`t[$displayCodeFile]`t: $lc lines"
}

$million = 1000000
$milLines = ($total / $million)

Write-Output ""
Write-Output "Total code : $total lines ( $milLines millions)"
