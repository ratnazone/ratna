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
#mergeFiles syntax

#######################################################
function Log( $message )
{
    $programName="mergeFiles.ps1"
    Write-Host "[$programName]" $message
}

function help()
{
    Write-Host Usage : "mergeFiles.ps1 <file1> <file2> .... <filen> outputfilename"
    exit -1
}

function ReadFile($fileName)
{
    return [string]::join([environment]::newline, (get-content -path $fileName))
}

function Ensure-Path()
{
  [CmdletBinding()]
  param($OutputFileName)
  
  $removeEmpty = [System.StringSplitOptions]::RemoveEmptyEntries;
  $tokens = $OutputFileName.Split("\", $removeEmpty);
  $path = ""
  Log("leng : $($tokens.Length)")
  for($i=0; $i -lt ($tokens.Length - 1); $i++)
  {
     $token = $tokens[$i]
     if ( $i -ne 0 )
     {
        $path += "\"
     }
     $path += $token
     Log("Checking path [$path]")
     
    $directoryExist = Test-Path $path -Type Container
    
    if ($directoryExist -eq $false)
    {
      Log("Creating $path")
      New-Item -Path $path -Type Container | Out-Null
    }
  }
}

#######################################################

$log = ""
Log("mergeFiles.ps1 called args : $args")

if ($args -eq $null)
{
    help
}

Log("argument length : $args.Length")

foreach($p in $args)
{
    $log += $p + " "
}
Log($log)


if ($args.Length -lt 3 )
{
    help
}
else
{
    $outputFileName = $args[$args.Length-1]
    
    $output = ""
    
    for($i=0; $i -lt ($args.Length-1); $i++)
    {
        $fileName = $args[$i]
        if ( (test-path $fileName) -eq $false )
        {
            Log("File $fileName does not exist")
            exit -1
        }
        
        $content=ReadFile $fileName
        
        $output += $content + [environment]::newline
    }
   
    Log ("Ensuring path to $outputFileName")
    
    # make sure the output folder exists
    Ensure-Path -OutputFileName $outputFileName
   
    Log("Saving output to file " + $outputFileName)
    
    $output | Out-file -encoding ascii $outputFileName
    
}
