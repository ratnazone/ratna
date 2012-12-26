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
# uses YUI compressor to compress javascript files
#
####################################################################

$resolver = @'
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
namespace Utils
{
    public static class AssemblyResolver
    {
        private static Dictionary<string, string> _assemblies;

        static AssemblyResolver()
        {
            var comparer = StringComparer.CurrentCultureIgnoreCase;
            _assemblies = new Dictionary<string,string>(comparer);
            AppDomain.CurrentDomain.AssemblyResolve += ResolveHandler;
        }

        public static void AddAssemblyLocation(string path)
        {
            // This should be made threadsafe for production use
            string name = Path.GetFileNameWithoutExtension(path);
            _assemblies[name]= path;
        }

        private static Assembly ResolveHandler(object sender, 
                                               ResolveEventArgs args) 
        {
            
            var assemblyName = new AssemblyName(args.Name);
            if (_assemblies.ContainsKey(assemblyName.Name))
            {
                return Assembly.LoadFrom(_assemblies[assemblyName.Name]);
            }
            return null;
        }
    }
}
'@


function Load-CompressorModules
{

   param(
      $location
   )
   
   if (-not (Test-Path $location))
   {
      throw "Path $location not found"
   }
  

   Add-Type -TypeDefinition $resolver -Language CSharpVersion3
   
   [Utils.AssemblyResolver]::AddAssemblyLocation("$location\Iesi.Collections.dll")
   [Utils.AssemblyResolver]::AddAssemblyLocation("$location\EcmaScript.NET.dll")
   
   $assemblypath = "$location\Yahoo.Yui.Compressor.dll"

   [Reflection.Assembly]::Loadfrom($assemblypath) | out-null   
   
}


function Internal-Compress-Javascript
{
     param(
         $filePath
     )
     
     if (-not (Test-Path $filePath))
     {
         throw "File $filePath not found"
     }
     
     $compressor = new-object -type "Yahoo.Yui.Compressor.JavaScriptCompressor"
     $compressed =  $null

      if ( $compressor -ne $null )
      {
        $contents = [System.IO.File]::ReadAllText($filePath)
        if (-not ([System.String]::IsNullOrEmpty($contents)))
        {
          $compressed =  $compressor.Compress($contents)
        }
        else
        {
           Write-Host -Foreground Yellow "File contents empty : $filePath"
           $compressed = $contents
        }
      }
      
      return $compressed
}

function Compress-JavaScript
{
    [cmdletbinding()]
    param(
        [string] $JSFile
    )
         
    $compressed = Internal-Compress-JavaScript $JSfile
    $fileName = Split-Path -Leaf $JSFile

    $renamedFile = "$fileName.orig"
    $renamedFilePath = Join-Path (Split-Path $JSFile) $renamedFile
    if (Test-Path $renamedFilePath)
    {
        Remove-Item $renamedFilePath | Out-Null
    }
    
    Rename-Item -Force -Path $file -NewName $renamedFile
    
    #save the compressed file
    $compressed > $file
    
    if (Test-Path $renamedFilePath)
    {
        Remove-Item $renamedFilePath | Out-Null
    }
}
