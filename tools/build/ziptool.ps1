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

function New-Zip
{
    param(
       [string]$zipfilename
    )
    
    set-content $zipfilename ("PK" + [char]5 + [char]6 + ("$([char]0)" * 18))
    
    (dir $zipfilename).IsReadOnly = $false
}

function Add-Zip
{
    param(
       [string]$zipfilename,
       $file
    )

    if(-not (test-path($zipfilename)))
    {
        throw "Zip File $zipfilename does not exist";
    }
    
    $shellApplication = new-object -com shell.application
    $zipPackage = $shellApplication.NameSpace($zipfilename)
    
    $file | foreach {
      $f = $_
      $zipPackage.CopyHere($f.FullName)
      sleep -m 500
    }
    
    
}


function ZipAllFiles
{
    [CmdletBinding()]
    param(
        [ValidateNotNull()]
        [array] $files,
    
        [ValidateNotNull()]
        [string] $zipFile
    )

    New-Zip -zipfilename $zipFile
    #Add-Zip -zipfilename $zipFile -files $files

    foreach($file in $files)
    {
        $item = Get-Item $file
        Add-Zip $zipFile $item
    }


}
