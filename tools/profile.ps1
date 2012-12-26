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
$cobra_base = "c:\depot\";
$nunit_path = $cobra_base + "tools\external\NUnit\NUnit-2.5.10.11092\bin\net-2.0";


############################################################
#
# DON'T CHANGE AFTER THIS
#
############################################################

$build_system_name = "cobra"
$host.ui.RawUI.WindowTitle = "$build_system_name [$cobra_base]"

function prompt
{
   $path = get-location
   return "$build_system_name $path>"
}

$cobra_tool_ftpUpload = $cobra_base + "tools\FtpUpload\FtpUpload.ps1"
$cobra_tool_build = $cobra_base + "tools\Build\Build.ps1"
$cobra_tool_clean = $cobra_base + "tools\Build\clean.ps1"
$cobra_tool_package = $cobra_base + "tools\Build\Package.ps1"
$cobra_tool_code = $cobra_base + "tools\Build\CodeGenerator.ps1"
$cobra_tool_mergefiles = $cobra_base + "tools\Build\mergeFiles.ps1"
$cobra_tool_sqlgen = $cobra_base + "tools\Build\SqlDataGen.ps1";
$cobra_tool_replaceToken = $cobra_base + "tools\Build\replace.js";
$cobra_util_title = $cobra_base + "tools\util\title.ps1";

$cobra_target_base = $cobra_base + "target"
$cobra_build_base = $cobra_base + "build"

$msbuild_path = $env:windir + "\Microsoft.NET\Framework\v4.0.30319"

$path = $env:Path
if (-not $path.Contains($msbuild_path))
{
   $env:Path = $env:Path + ";" + $msbuild_path;
}

#set path to nunit
$env:Path = $env:Path + ";" + $nunit_path;

Set-Alias FtpUpload $cobra_tool_ftpUpload
Set-Alias build $cobra_tool_build
Set-Alias clean $cobra_tool_clean
Set-Alias CodeGenerator $cobra_tool_code
Set-Alias MergeFiles $cobra_tool_mergefiles
Set-Alias SqlDataGen $cobra_tool_sqlgen
Set-Alias package $cobra_tool_package
Set-Alias title $cobra_util_title
