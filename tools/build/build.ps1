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

$ziptools = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) ziptool.ps1
. $ziptools


function Help()
{
    [CmdletBinding()]
    param($Message)
    
    Write-Host $Message
}

function Log-Build()
{
  [CmdletBinding()]
  param($Level, $Message)
  
  if ($Level -eq "Info")
  {
    Write-Host "[$Level] [build] $Message"
  }
}


Function Flatten()
{
    [CmdletBinding()]
    param($StringArray)

    return [string]::join(" ", $StringArray)
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

Function Check-BuildFileExists()
{
    [CmdletBinding()]
    param($BuildFile)
    
    return Test-Path $BuildFile
    
}

###############################################################
#
# sanities file location. This file must be in build directory
# if not qualified.
#
# qaulified locations are
# .\
# ..\
# C:\
# \\
Function SanitizeFileLocation()
{
  [CmdletBinding()]
  param($FileLocation, $BuildFolder)
  
  $file = $FileLocation
  
  if ($FileLocation -ne $null)
  {
    if (-not ($FileLocation.StartsWith(".")))
    {
        if (-not ($FileLocation.StartsWith("..")))
        {
            $file = $BuildFolder + $FileLocation
        }
    }
  }
  
  return $file
}

##################################################
# replaces build tokens
#
Function ReplaceBuildTokens()
{
  [CmdletBinding()]
  param($Tokenized, $BuildFolder)    
  
  $replaced = $Tokenized
  
  if ($replaced.Contains("%build_location%"))
  {
    $replaced = $replaced.Replace("%build_location%", $BuildFolder);
  }
  
  return $replaced;
}

##################################################
# Imports the file to specified location
#
Function ImportFile()
{
  [CmdletBinding()]
  param($ImportsNode, $TargetBase, $BuildBase, $Flavor)
  
  if (-not($TargetBase.EndsWith("\")))
  {
    $TargetBase = $TargetBase + "\"
  }
    
  if (-not($BuildBase.EndsWith("\")))
  {
    $BuildBase = $BuildBase + "\"
  }
  
  if ($ImportsNode -ne $null)
  {    
    foreach($importNode in $ImportsNode.ChildNodes)
    {
      $target = $importNode.target
      $build = $importNode.build
      $source = $importNode.source
      $destination = $importNode.destination
      
            
      if ($target -ne $null)
      {
        if (-not ($target.EndsWith("\")))
        {
          $target = $target + "\"
        }
      
        $source = $TargetBase + $target + $Flavor + "\" + $source
      }
      else
      {
        if ($build -ne $null)
        {
          if (-not ($build.EndsWith("\")))
          {
            $build = $build + "\"
          }
        
          $source = $BuildBase + $build + $source
        }
      }
      
      Log-Build -Level "Info" -Message "Importing from:[$source] to:[$destination]"
      Copy-Item -Path $source -Destination $destination | Out-Null
    }
  }
  
}


##################################################
Function ExecuteCode()
{
  [CmdletBinding()]
  param($ExecuteNode, $BuildFolder)
  
  Log-Build -Level "Verbose" -Message "Execute Code called"
  
  foreach($scriptNode in $ExecuteNode.ChildNodes)
  {
  
    if ($scriptNode -eq $null)
    {
      continue;
    }
    
    #cscript execute
    if ($scriptNode.Name -eq "cscript")
    {      
        $script = $scriptNode.script
        $arguments = $scriptNode.arguments
        
        #replace tokens in arguments
        $arguments = ReplaceBuildTokens -Tokenized $arguments -BuildFolder $BuildFolder
        
        if($script -eq $null)
        {
            continue;
        }
        
        Log-Build -Level "Verbose" -Message "Executing cscript: $script, args: $arguments"
          
        Invoke-Expression "cscript /nologo $script $arguments"
    }
    else
    {
        if ($scriptNode.Name -eq "powershell")
        {
            $psFile = $scriptNode.file
            $arguments = $scriptNode.arguments
        
            #replace tokens in arguments
            $arguments = ReplaceBuildTokens -Tokenized $arguments -BuildFolder $BuildFolder
            
            if($psFile -eq $null)
            {
                continue;
            }
            
            Log-Build -Level "Verbose" -Message "Executing powershell: $psFile $arguments"
            Invoke-Expression -command "$psFile $arguments"
        }
    }
  
  }
  
  Log-Build -Level "Verbose" -Message "Execute Code done"
}


Function ReplaceTokens()
{
  [CmdletBinding()]
  param($ReplaceTokenNode, $BuildFolder)
  
  Log-Build -Level "Verbose" -Message "Execute ReplaceTokens called"
  
  $tokensFile = $ReplaceTokenNode.tokens
  $inputFile = $ReplaceTokenNode.inputFile
  $outputFile = $ReplaceTokenNode.outputFile
  
  #sanitize the locations
  $tokensFile = SanitizeFileLocation -FileLocation $tokensFile  -BuildFolder $BuildFolder
  $inputFile = SanitizeFileLocation -FileLocation $inputFile  -BuildFolder $BuildFolder
  $outputFile = SanitizeFileLocation -FileLocation $outputFile  -BuildFolder $BuildFolder

  Invoke-Expression "cscript $cobra_tool_replaceToken $tokensFile $inputFile $outputFile"
  
  Log-Build -Level "Verbose" -Message "Execute ReplaceTokens done"
}

Function ZipFiles()
{
    [CmdletBinding()]
    param($ZipNode, $BuildFolder, $TargetFolder)

    Log-Build -Level "Verbose" -Message "ZipFiles called"
    
    if ($ZipNode -ne $null)
    {
        $zipFile =  [String]::Format("{0}\{1}", $TargetFolder, $ZipNode.name)

		Log-Build -Level "Verbose" -Message "Ensuring path to [$zipFile]"
		#ensure the path
		if (-not(Test-Path $TargetFolder))
		{
			New-Item -path $TargetFolder -type directory | out-null
		}

        $files = @()

        Log-Build -Level "Info" -Message "Creating zip file : [$zipFile]"

        foreach($file in $ZipNode.ChildNodes)
        {
            if ($file -eq $null)
            { 
                continue;
            }

            $source = $file.source.Replace("`$build\", $BuildFolder)
            $source = $source.Replace("`$target", $TargetFolder)

            $files += $source
        }

        ZipAllFiles -files $files -zipFile $zipFile 
    }

}

Function ExportFiles()
{
  [CmdletBinding()]
  param($ExportNodes, $BuildFolder)
  
  Log-Build -Level "Verbose" -Message "ExportFiles called"
 
  if ($ExportNodes -ne $null)
  {
    foreach($export in $ExportNodes.ChildNodes)
    {
      if ($export -eq $null)
      { 
          continue;
      }
      
      $file = $export.file
      
      if ($file -eq $null)
      {
        continue;
      }
      
      $source = $file
      $tokens = $file.split('\');
      
      #check if the export needs to be done in the parent folder
      $mainPathStructure = $true
      
      if ( $export.folder -ne $null )
      {
        $destination = $BuildFolder + $tokens[$tokens.Length-1]
      }
      else
      { 
      
        $destination = $BuildFolder + $file  
        
        #ensure the directory structure is available for copy                
        Ensure-Path -Path $destination          
        
      }  
      
      $destinationFolder = Split-Path $destination
      
      Log-Build -Level "Info" -Message "Exporting from:[$source] to: [$destinationFolder]"
      Copy-Item -Path $source -Destination $destinationFolder -force
      
    }
  }
}

Function CodeGenerate()
{
  [CmdletBinding()]
  param($CodeGenerateNode, $Type, $BuildFolder)
  
  $output =  $BuildFolder + $CodeGenerateNode.outputFile
  
  Log-Build -Level "Verbose" -Message "CodeGenerate :: Input: $CodeGenerateNode.inputFile Output: $CodeGenerateNode.outputFile Type: $Type"
  
  CodeGenerator $CodeGenerateNode.inputFile $Type $output
}

Function SqlCodeGenerate()
{
  [CmdletBinding()]
  param($SqlNode, $BuildFolder)
  
  Log-Build -Level "Verbose" -Message "SqlCodeGenerate called"
  
  
  if ($SqlNode -ne $null)
  {
    $mergeFiles = $true
    if ( ($SqlNode.merge -ne $null) -and ( $SqlNode.merge -eq "false" ) )
    {
        $mergeFiles = $false
    }
    
    $outputFile = $null
    
    if ($mergeFiles)
    {
        $outputFile = $BuildFolder + $SqlNode.outputFile
    }
    
    $count = 1;
    $tempFilePrefix = "____temp_sql_";
    $tempFileSuffix = ".sql_";
  
    foreach($sqlGen in $SqlNode.ChildNodes)
    {
      $output = $BuildFolder + $tempFilePrefix + $count + $tempFileSuffix;
      $count = $count + 1
      
      $rules = $sqlGen.rulesFile
      $data = $sqlGen.dataFile
      
      Log-Build -Level "Verbose" -Message "Calling data: $data, rules: $rules, output: $output"
      
      #generate the sql code
      Invoke-Expression "SqlDataGen $rules $data $output"
    }
    
    if ($mergeFiles)
    {
        $allfiles = @()
        
        for($i=1; $i -lt $count; $i++)
        {
            $allfiles += $BuildFolder + $tempFilePrefix + $i + $tempFileSuffix;
        }
        
        $allfiles += $outputFile;
        
        $flattened = Flatten -StringArray $allfiles
        Log-Build -Level "Verbose" -Message "merging files : $flattened"
        Invoke-Expression  "MergeFiles $flattened"
        
        #delete temporary files
        for($i=1; $i -lt $count; $i++)
        {
            $file = $BuildFolder + $tempFilePrefix + $i + $tempFileSuffix
            Log-Build -Level "Verbose" -Message "Deleting file: $file" 
            Remove-Item $file;
        }
    }
    
  }
  else
  {
    Log-Build -Level "Verbose" -Message "SqlNode is null"
  }
  
  
  Log-Build -Level "Verbose" -Message "SqlCodeGenerate done"    
}


Function MergeFilesCall()
{
  [CmdletBinding()]
  param($MergeNode, $BuildFolder)
  
  Log-Build -Level "Verbose" -Message "MergeFiles:: called"
  
  $outputFile = SanitizeFileLocation -FileLocation $MergeNode.outputFile -BuildFolder $BuildFolder  
  $inputFiles = @()
    
  foreach($file in $MergeNode.ChildNodes)
  {
    if ($file -eq $null)
    { 
        continue;
    }
    
    if ($file.NodeType -ne "Element")
    { 
        continue;
    }
        
    $t_file = SanitizeFileLocation -FileLocation $file.name  -BuildFolder $BuildFolder
  
    Log-Build -Level "Verbose" -Message "file $t_file"
    $inputFiles += $t_file
  }
  
   Log-Build -Level "Verbose" -Message "Merge output file:  $outputFile"
  
  $inputFiles += $outputFile
    
  if ($inputFiles.Length -ne 0 )
  {
    $flattened = Flatten -StringArray $inputFiles
    Log-Build -Level "Verbose" -Message "merging files : $flattened"
    Invoke-Expression  "MergeFiles $flattened"
  }
  else
  {
    Log-Build -Level "Info" -Message "no files to merge"
  }  
}

Function RunBatch()
{
  [CmdletBinding()]
  param($Batch, $BuildFolder, $TargetFolder)
  
  if ($Batch -ne $null)
  {
    if ($Batch.zip -ne $null)
    {
        ZipFiles -ZipNode $Batch.zip -BuildFolder $BuildFolder -TargetFolder $TargetFolder
    }
  }

}

Function RunPass()
{
  [CmdletBinding()]
  param($Pass, $BuildFolder)
  
  if ($Pass -ne $null)
  {
    if ($Pass.codeGenerate -ne $null)
    {
      CodeGenerate -CodeGenerateNode $Pass.codeGenerate -Type "-class" -BuildFolder $BuildFolder
    }
    
    if ($Pass.tokenGenerate -ne $null)
    {
      CodeGenerate -CodeGenerateNode $Pass.tokenGenerate -Type "-token" -BuildFolder $BuildFolder 
    }
    
    if ($Pass.merge -ne $null)
    {
      MergeFilesCall $Pass.merge -BuildFolder $BuildFolder
    }
    
    if ($Pass.sql -ne $null)
    {
        SqlCodeGenerate $Pass.sql -BuildFolder $BuildFolder
    }
    
    if ($Pass.execute -ne $null)
    {
        ExecuteCode $Pass.execute -BuildFolder $BuildFolder
    }
    
    if ($Pass.replaceTokens -ne $null)
    {
        ReplaceTokens $Pass.replaceTokens -BuildFolder $BuildFolder
    }
    
    if ($Pass.exports -ne $null)
    {
        ExportFiles $Pass.exports -BuildFolder $BuildFolder
    }

  }
}

Function Build-Project()
{
   [CmdletBinding()]
   param($BuildFile, $Flavor)
 
   [xml]$buildXml = get-content $BuildFile
        
   $project_file = $buildXml.build.project
   $target_folder_base = $cobra_target_base + "\" + $buildXml.build.target
   $output_folder = $target_folder_base + "\" + $Flavor
   $build_folder = $cobra_build_base + "\" + $buildXml.build.target + "\"
   
   # make sure folders exist
   if (-not (Test-Path $build_folder))
   {
       New-Item $build_folder -type directory | Out-Null
   }
   
   #imports
   if ($buildXml.build.imports -ne $null)
   {
       ImportFile -ImportsNode $buildXml.build.imports -TargetBase $cobra_target_base -BuildBase $cobra_build_base -Flavor $Flavor
   }
   
   $passes = $buildXml.build.passes
   if ($passes -ne $null)
   {
      foreach($pass in $passes.ChildNodes)
      {
         RunPass -Pass $pass -BuildFolder $build_folder
      }
   }
   
   Log-Build -Level "Verbose" -Message "Project file:  $project_file, Target: $target_folder, Output folder: $output_folder"
   
   $buildConfiguration = "Debug"
   if ( $Flavor -eq "ship")
   {
      $buildConfiguration = "Release"
   }
   
   
    #ignore if there is no project file defined or does not
    #exists on the folder.
    if (($project_file -ne $null) -and (Test-Path $project_file))
    {        
        # generate properties if required
        if ($buildXml.build.properties -ne $null)
        {
           $filePath = $buildXml.build.properties.file
           if ($filePath -ne $null)
           {
              Create-AssemblyFile -FilePath $filePath -Properties $buildXml.build.properties
           }
        }
        
        msbuild $project_file /property:Configuration=$buildConfiguration /property:OutputPath=$output_folder   
    }

    # post build events
   $postbuild = $buildXml.build.postbuild
   if ($postbuild -ne $null)
   {
      foreach($batch in $postbuild.ChildNodes)
      {
         RunBatch -Batch $batch -BuildFolder $build_folder -TargetFolder $output_folder
      }
   }
}


Function Build-Dirs()
{
  [CmdletBinding()]
  param($Dirs, $BuildFileName, $DirsFile, $Flavor)
  
  foreach($dir in $Dirs)
  {
    cd $dir
    
    $internaldirs = Get-Dirs -DirsFile $DirsFile
    if ($internaldirs.Length -gt 0 )
    {
      Build-Dirs -Dirs $internaldirs -DirsFile $DirsFile  -BuildFileName $BuildFileName -Flavor $Flavor
      cd ..
    }
    else
    {
      Log-Build -Level "Info" -Message "Building $dir"
      
      Build-Project -BuildFile $BuildFileName -Flavor $Flavor
      
      cd ..
    }
  }
}

Function Create-AssemblyFile()
{
  [cmdletBinding()]
  param($FilePath, $Properties)
  
  $defaultProperties = @{ "AssemblyCompany" = "Jardalu LLC"; "AssemblyCopyright" = "Copyright © Jardalu LLC 2012"}
  
  $output = "using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;

[assembly: ComVisible(false)]
";

  foreach ($property in $defaultProperties.Keys)
  {
     $formatted = [string]::Format( "[assembly: {0}(""{1}"")]", $property, $defaultProperties[$property])
     $output += $formatted + [environment]::newline
  }
  
  
  if ( $Properties -ne $null )
  {
      foreach($property in $Properties.ChildNodes)
      {
         $name = $property.name
         $val  = $property.value
         
         $noquote = $property.noquote
         
         if ($noquote -eq $null)
         {
            $noquote = $false
         }
         else
         {
            if ($noquote -eq "true")
            {
               $noquote = $true
            }
            else
            {
               $noquote = $false
            }
         }
         
         if ($noquote)
         {
            $formatted = [string]::Format( "[assembly: {0}({1})]", $name, $val)
         }
         else
         {
            $formatted = [string]::Format( "[assembly: {0}(""{1}"")]", $name, $val)
         }
         
         $output += $formatted + [environment]::newline
      }
  }
  
  if (Test-Path $FilePath)
  {
      Log-Build -Level "Verbose" -Message "Deleting file - $FilePath"
      Remove-Item $FilePath
  }
  
  $output | Out-File $FilePath
  
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
  Build-Dirs -Dirs $dirs -BuildFileName $buildFileName -DirsFile $dirsFileName -Flavor $flavor
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
        Log-Build -Level "Verbose" -Message "Calling build"
        Build-Project -BuildFile $buildFileName -Flavor $flavor
    }
}

