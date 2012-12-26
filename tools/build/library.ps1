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
# library functions

function Log()
{
  [CmdletBinding()]
  param($Level, $Message)
  
  Write-Host "[$Level] $Message"
  
}


function IsAdminMode()
{
  $wid=[System.Security.Principal.WindowsIdentity]::GetCurrent()
  $prp=new-object System.Security.Principal.WindowsPrincipal($wid)
  $adm=[System.Security.Principal.WindowsBuiltInRole]::Administrator
  $IsAdmin=$prp.IsInRole($adm)
  return $IsAdmin
}

function Ensure-Path()
{
    param(
       [string] $Path,
       [bool] $Create=$false
    )
    
    if ( $Path -eq $null )
    {
        return
    }
    
    $parent = Split-Path $Path
    
    if (Test-Path $parent)
    {       
       if ($Create)
       {
           New-Item $path -ItemType Directory | Out-Null
       }
       
       return
    }
    else
    {
       Ensure-Path -path $parent -create:$true
    }
}
