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
$lib = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "..\..\library\library.ps1"
. $lib


function UpgradeTemplate()
{
    $sql = "SELECT UId, RawData FROM Tbl_Ratna_PluginData WHERE PluginId = 'D668791F-9377-40EA-9E38-B6DDF12C94FC' AND RawData LIKE '%Jardalu.Ratna.Templates%'"
    
    # Replace Jardalu.Ratna.Templates with Jardalu.Ratna.Web.Template
    $resultset = Invoke-SqlCmdForConfiguration -Query $sql
    
    if ($resultset -ne $null)
    {
        $resultset | foreach {
            $result = $_
            
            $rawData  = $result["RawData"]
            $uid = $result["UId"]
            
            $modifiedRawData = $rawData.Replace("Jardalu.Ratna.Templates", "Jardalu.Ratna.Web.Templates")
            
            $mSql = "UPDATE Tbl_Ratna_PluginData SET RawData = '$modifiedRawData' WHERE UId = '$uid'"
            Invoke-SqlCmdForConfiguration -Query $mSql
        }
    }
    
}

function UpdateForms()
{

    # old form XML
    # <?xml version="1.0" encoding="utf-16"?><Form xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/Jardalu.Ratna.Core.Forms">
    # <displayName>Contact Us</displayName><fields><Field><FieldType>String</FieldType><IsRequired>false</IsRequired><Name>name</Name></Field><Field><FieldType>String</FieldType>
    # <IsRequired>false</IsRequired><Name>email</Name></Field><Field><FieldType>String</FieldType><IsRequired>false</IsRequired><Name>message</Name></Field></fields><name>contact_us</name>
    # </Form>

    # new from XML
    # <?xml version="1.0" encoding="utf-16"?><Form xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/Jardalu.Ratna.Core.Forms">
    # <displayName>mc</displayName><fields xmlns:d2p1="http://schemas.datacontract.org/2004/07/Jardalu.Ratna.Core"><d2p1:Field><d2p1:FieldType>String</d2p1:FieldType>
    # <d2p1:IsCollection>false</d2p1:IsCollection><d2p1:IsRequired>true</d2p1:IsRequired><d2p1:Name>name</d2p1:Name></d2p1:Field><d2p1:Field><d2p1:FieldType>Integer</d2p1:FieldType>
    # <d2p1:IsCollection>false</d2p1:IsCollection><d2p1:IsRequired>true</d2p1:IsRequired><d2p1:Name>age</d2p1:Name></d2p1:Field></fields><name>mc</name></Form>
    
    # Changes - New field is from different namespace
    # <fields> and its child are in the namespace http://schemas.datacontract.org/2004/07/Jardalu.Ratna.Core
    #

    $sql = "SELECT UId, RawData FROM Tbl_Ratna_PluginData WHERE PluginId = 'FFA99FEC-29FB-42AC-A0E9-DB189E543573'"

    $resultset = Invoke-SqlCmdForConfiguration -Query $sql
    
    if ($resultset -ne $null)
    {
        $resultset | foreach {
            $result = $_
            
            $rawData  = $result["RawData"]
            $uid = $result["UId"]

            if ( $rawData.Contains("<fields>") )
            {
            
                #create the xml from rawData
                [xml] $xml = $rawData

                $prefix = "d2p1"
                $nsurn = "http://schemas.datacontract.org/2004/07/Jardalu.Ratna.Core"

                #get the namespace manager
                $nsmgr = new-object -type "System.Xml.XmlNamespaceManager" -ArgumentList $xml.NameTable;
                $nsmgr.AddNamespace("i", "http://schemas.datacontract.org/2004/07/Jardalu.Ratna.Core.Forms");
                $nsmgr.AddNamespace($prefix, $nsurn)

                $fieldsNode = $xml.SelectSingleNode("//i:fields", $nsmgr);
                $cloned = CloneNode -node $fieldsNode -prefix $prefix -nsUri $nsurn -xmlDocument $xml

                $fieldsNode.ParentNode.ReplaceChild( $cloned, $fieldsNode) | Out-Null

                $modifiedRawData = $xml.OuterXml

                # replace the root nodes
                $modifiedRawData = $modifiedRawData.Replace("d2p1:fields", "fields");
            
                $mSql = "UPDATE Tbl_Ratna_PluginData SET RawData = '$modifiedRawData' WHERE UId = '$uid'"
                Invoke-SqlCmdForConfiguration -Query $mSql
            }
        }
    }
}

function UpdateFormEntries()
{
    # Jardalu.Ratna.Core.Forms.Response was renamed to Jardalu.Ratna.Core.Forms.FormEntry

    $sql = "SELECT UId, RawData FROM Tbl_Ratna_PluginData WHERE PluginId = '04C2417E-BC6E-411B-B129-E8352DECD057' AND RawData LIKE '%<Response %'"
    
    # Replace Jardalu.Ratna.Templates with Jardalu.Ratna.Web.Template
    $resultset = Invoke-SqlCmdForConfiguration -Query $sql
    
    if ($resultset -ne $null)
    {
        $resultset | foreach {
            $result = $_
            
            $rawData  = $result["RawData"]
            $uid = $result["UId"]
            
            $modifiedRawData = $rawData.Replace("<Response ", "<FormEntry ")
            $modifiedRawData = $modifiedRawData.Replace("</Response>", "</FormEntry>")
            
            $mSql = "UPDATE Tbl_Ratna_PluginData SET RawData = '$modifiedRawData' WHERE UId = '$uid'"
            Invoke-SqlCmdForConfiguration -Query $mSql
        }
    }
}

function GetInstalledFolder()
{
  return $Config_installat
}

function GetUpgradeFolder()
{
    $installedFolder = GetInstalledFolder
    $upgradeFolder = Join-Path $installedFolder "__upgradedfiles"
    if (-not (Test-Path $upgradeFolder))
    {
        New-Item $upgradeFolder -Type Directory | Out-Null
    }
    
    return $upgradeFolder
}

function SetSitesLocation()
{
    $installedFolder = GetInstalledFolder
    
    # Templates folder fix
    $oldTemplatesFolder = (Join-Path $installedFolder "templates")
    if (Test-Path $oldTemplatesFolder)
    {
        $folders = Get-ChildItem $oldTemplatesFolder
        $folders | foreach {
           $folder = $_
           
           if ( $folder.Name -ne "System")
           {
                      
             $siteFolder = EnsureSiteFolder -siteId $folder.Name
             
             $templatesFolder = (Join-Path $siteFolder.FullName "templates")
             if (-not (Test-Path $templatesFolder))
             {
                New-Item $templatesFolder -Type Directory | Out-Null
             }
             
             #copy templates
             xcopy /s /r /q $folder.FullName $templatesFolder
             
             $upgradeFolder = GetUpgradeFolder
             $upgradeTemplatesFolder = Join-Path $upgradeFolder "templates"

             if (-not (Test-Path $upgradeTemplatesFolder))
             {
                New-Item $upgradeTemplatesFolder -Type Directory | Out-Null
             }
                        
             #move template to upgradedfolder
             Move-Item $folder.FullName $upgradeTemplatesFolder | out-Null
           }
        }

    }
    
    #Contents folder fix
    $oldContentsFolder = (Join-Path $installedFolder "r-content")
    if (Test-Path $oldContentsFolder)
    {
         $folders = Get-ChildItem $oldContentsFolder
         $folders | foreach {
           $folder = $_
           $siteFolder = EnsureSiteFolder -siteId $folder.Name
           
           $contentsFolder = (Join-Path $siteFolder.FullName "content")
           if (-not (Test-Path $contentsFolder))
           {
              New-Item $contentsFolder -Type Directory | Out-Null
           }
           
           #copy templates
           xcopy /s /r /q $folder.FullName $contentsFolder
           
           $upgradeFolder = GetUpgradeFolder
           $upgradeContentsFolder = Join-Path $upgradeFolder "r-content"
           
           if (-not (Test-Path $upgradeContentsFolder))
           {
              New-Item $upgradeContentsFolder -Type Directory | Out-Null
           }
           
           #move template to upgradedfolder
           Move-Item $folder.FullName $upgradeContentsFolder | out-Null
           
         }#foreach
         
         #delete old contents folder
         if (Test-Path $oldContentsFolder)
         {
            Remove-Item $oldContentsFolder -Force:$true | Out-Null
         }
    }
    
}

function EnsureSitesFolder()
{
    $installedFolder = GetInstalledFolder
        
    #make sure the sites folder exists
    if (-not (Test-Path (Join-Path $installedFolder sites)))
    {
        New-Item (Join-Path $installedFolder sites) -Type Directory | Out-Null
    }
}

function EnsureSiteFolder()
{
    param(
       $siteId
    )
    
    $installedFolder = GetInstalledFolder
    $sitesFolder = Join-Path $installedFolder "sites"
    
    $siteFolder = Join-Path $sitesFolder $siteId
    
    if (-not (Test-Path $siteFolder))
    {
        New-Item $siteFolder -Type Directory | Out-Null
    }
    
    return Get-Item $siteFolder
    
}

UpdateForms
UpdateFormEntries
UpgradeTemplate
EnsureSitesFolder
SetSitesLocation

