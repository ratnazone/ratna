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
# FTP upload utility with powershell. Parameters controlled through
# manifest file.
#
# Usage -
# c:\>ftpupload.ps1 <manifest>
#
#
# Author : Jardalu (http://jardalu.com)
#
####################################################################

$debug = $false

#########################################
#
# tool to upload via ftp
#
#########################################

Function Help()
{
  [CmdletBinding()]
  param()
  
  Log -Message "./FtpUpload.ps1 <manifest>"
}

Function Log()
{
  [CmdletBinding()]
  param($Message)
  
  if ($debug)
  {
    Write-Host $Message
  }
}

Function Info()
{
  [CmdletBinding()]
  param($Message)
  
  Write-Host $Message
}

#################################################################3
#
# returns the file list from the given path
#
Function Get-FileList()
{
  [CmdletBinding()]
  param($from, $recursive)
  
  $uploadFiles = @()
  
  $directory = $from.directory
  $files = $from.files
  
  if ($files.filter -eq $null)
  {
    foreach($file in $files.ChildNodes)
    {
      $literalPath = $directory + "\" + $file.InnerText
      $f = get-childitem -LiteralPath $literalPath
      if ( $f -ne $null)
      {
        $uploadFiles += $f
      }
    }
  }
  else
  {
    $uploadFiles = Get-FileListAll -directory $directory -filter $filter -recursive $recursive
  }
  
  return $uploadFiles
}


#################################################################3
#
# returns the file list in recursive mode as well
#
Function Get-FileListAll()
{
  [CmdletBinding()]
  param($directory, $filter, $recursive)
  
  $files = @()
  
  # add all the files
  $dir_files = get-childitem -Path $directory -Filter $filter | where{!($_.PSISContainer)}
  foreach($file in $dir_files)
  {
    $files += $file
  }
  
  if ($recursive)
  {
    $folders = get-childitem -Path $directory -Filter $filter | where{($_.PSISContainer)}
    foreach($folder in $folders)
    {      
      if ($folder -eq $null)
      {
        continue;
      }
      
      $qfolder = $directory + "\" + $folder.Name
      Log -Message "will recurse on $qfolder"
      $rfiles = Get-FileListAll -directory $qfolder -filter $filter -recursive $recursive
      
      foreach($rfile in $rfiles)
      {
        $files += $rfile
      }
    }
  }
    
  return $files
}

#################################################################3
#
# returns if the directory exists on the server
#
Function Does-FTPDirectoryExist()
{
  [CmdletBinding()]
  param($Server, $UserName, $Password, $Path)
  
  try
  {
    $directoryPath = $Server + $Path
    $ftprequest = [System.Net.FtpWebRequest]::Create($directoryPath);
    $ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::ListDirectory;
    $ftprequest.Credentials = New-Object System.Net.NetworkCredential($Username,$Password);
    
    $ftpresponse = $ftprequest.GetResponse();
    $ftpresponse.Close();
  }
  catch
  {
    return $false;
  }
  
  return $true
}

#################################################################3
#
# creates a directory on the server
#
Function Create-FTPDirectory()
{
  [CmdletBinding()]
  param($Server, $UserName, $Password, $Path)
  
  $directoryPath = $Server + $Path
  $ftprequest = [System.Net.FtpWebRequest]::Create($directoryPath);
  $ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory;
  $ftprequest.Credentials = New-Object System.Net.NetworkCredential($Username,$Password);
  
  $ftpresponse = $ftprequest.GetResponse();
  if ($ftpresponse.StatusCode -ne "PathnameCreated")
  {
    Info -Message "Unable to create directory : $directoryPath";
  }
  $ftpresponse.Close();
}

#################################################################3
#
# will ensure that the directory exists on the server. if the
# directory does not exists, it will create one
#
Function Ensure-FTPDirectory()
{
  [CmdletBinding()]
  param($Server, $UserName, $Password, $UploadPath)
  
  $removeEmpty = [System.StringSplitOptions]::RemoveEmptyEntries;
  $tokens = $UploadPath.Split("/", $removeEmpty);
  $path = ""
  foreach($token in $tokens)
  {
    $path += "/" + $token
    $directoryExist = Does-FTPDirectoryExist -Server $Server -UserName $UserName -Password $Password -Path $path
    
    if ($directoryExist -eq $false)
    {
      Create-FTPDirectory -Server $Server -UserName $UserName -Password $Password -Path $path
    }
  }
}

#################################################################3
#
# uploads one ftp file
#
Function Upload-FTPFile()
{
  [CmdletBinding()]
  param($Server, $UserName, $Password, $UploadPath, $File)
 

  Log -Message "Upload Path: $UploadPath"
  
  Ensure-FTPDirectory -Server $Server -UserName $UserName -Password $Password -UploadPath $UploadPath
  
  $fullUploadPath = $Server + $UploadPath + $File.Name;
  
  Log -Message "Server : $server, UserName : $UserName, fullUploadPath: $fullUploadPath, File : $File.FullName"
    
  #region Upload File using ftpWebrequest
  $ftprequest = [System.Net.FtpWebRequest]::Create($fullUploadPath);
  $ftprequest.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile;
  $ftprequest.Credentials = New-Object System.Net.NetworkCredential($Username,$Password);

  $ftprequest.UsePassive = $true;
  $ftprequest.UseBinary = $true;
  $ftprequest.KeepAlive = $false;

  Log -Message "Reading file $file"

  $sourceStream = [System.IO.File]::OpenRead($File.FullName);
  Log -Message "Source File length: $sourceStream.Length"
  $buffer = New-Object byte[] $sourceStream.Length
  
  #read the file contents in buffer
  $sourceStream.Read($buffer,0, $buffer.Length);
  $sourceStream.Close();
  
  #write the file
  $requestStream = $ftprequest.GetRequestStream();
  $requestStream.Write($buffer,0, $buffer.Length)      
  $requestStream.Flush();  
  $requestStream.Close();
  
  $ftpresponse = $ftprequest.GetResponse();
  Log -Message "Ftp Response : $ftpresponse.StatusDescription"
  $ftpresponse.Close();
}

########################################################################
# function that generates the upload path
#
Function CalculateUploadPath
{
  [CmdletBinding()]
  param($BaseFrom, $BaseTo, $fileName)
  
  $uploadPath = $BaseTo
    
  $index = $fileName.IndexOf($BaseFrom) + $BaseFrom.Length + 1
  $endindex = $fileName.LastIndexOf("\")
  
  
  if ($endindex -lt 0 )
  {
    $endindex = $fileName.Length
  }
  
  $extraPath = $fileName.Substring( $index, $endindex - $index + 1)   
  $extraPath = $extraPath.Replace('\', '/');
    
  $uploadPath = $BaseTo + $extraPath
    
  return $uploadPath
}

########################################################################
#
# uploads based on manifest file
#
Function Upload()
{
  [CmdletBinding()]
  param($FtpManifestFile)
  
  #make sure the manifest file exists
  if ((test-path $FtpManifestFile) -eq $false)
  {
    Info -Message "File `"$FtpManifestFile`" does not exists."
    return -1;
  }
  
  Info -Message "Manifest file: $FtpManifestFile"
  
  [xml]$manifestXml=get-content $FtpManifestFile
  
  $ftpNode = $manifestXml.DocumentElement
  $server = $ftpNode.server.host
  $username = $ftpNode.server.login
  $password = $ftpNode.server.password
  
  $transfersNodeList = $ftpNode.transfers
  
  foreach($transfer in $transfersNodeList.ChildNodes)
  {
      if($transfer -eq $null)            
      {
        continue;
      }
      
      if ($transfer.NodeType -ne "Element" )
      {
        continue;
      }
      
      Log -Message "child node : $transfer"
      
      $to = $transfer.to
      Log -Message "To : $to"
      
      $recursive = $false
      
      if ($transfer.recursive -eq "true")
      {
        $recursive = $true
        Log -Message "Using recursive mode"
      }
      
      $from = $transfer.from
      $files = Get-FileList -from $from -recursive $recursive
      foreach($file in $files)
      {
        if ($file -eq $null) 
        {
          continue;
        }
        
        $uploadPath = CalculateUploadPath -BaseFrom $from.directory -BaseTo $to -File $file.FullName
        
        Info -Message "Uploading file: $file to: $uploadPath"
        Upload-FTPFile -Server $server -UserName $username -Password $password -UploadPath $uploadPath -File $file
      }
  }
}



#############################################
# main execution
#############################################
if ($args.Length -lt 1 )
{
    help
    return;
}

$returnValue = Upload -FtpManifestFile $args[0]
exit $returnValue
