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
$config = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "..\config.ps1"
. $config

# --------------------------------------------------------------------- 
# snappins
# ---------------------------------------------------------------------

$ErrorCodes = @{
    "WebAdminModule" = "Module WebAdministration not found";
    "SQLSnapin" = "SqlServerCmdletSnapin100 not found";
}

function Load-ModulesAndSnapins()
{
  
  $success = $true
  $error = $null

  #check if sqlsnappin is registered
  $sqlSnapin = Get-PSSnapin -Registered -Name SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue
  
  if ( $sqlSnapin -ne $null)
  {

    if ( (Get-PSSnapin -Name SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue) -eq $null )
    {
        Add-PsSnapin SqlServerCmdletSnapin100 | Out-null
    }
  
  }
  else
  {
      $error = $ErrorCodes["SQLSnapin"]
      $success = $false
  }

  if ( $success )
  {
    if ( (Get-Module -Name WebAdministration -ErrorAction SilentlyContinue) -eq $null )
    {
        $available = Get-Module -ListAvailable | where { $_.Name -eq "WebAdministration" }
        if (-not $available)
        {
            $error = $ErrorCodes["WebAdminModule"]
            $success = $false
        }
        else
        {
            Import-Module WebAdministration | Out-null
        }
    }
  }
  
  if (-not $success)
  {
      Log -Level Error -Message "Prereq failed - Error: $error"
  }
  
  return $success

}


# --------------------------------------------------------------------- 
# common functions
# ---------------------------------------------------------------------

function Log()
{
    param(
        [string] $Level,
        [string] $Message
    )
    
    $error = $false
    
    if ( "error".Equals( $Level, [System.StringComparison]::OrdinalIgnoreCase) )
    {
        Write-Host -Foreground Red $Message
        $error = $true
    }
    else 
    {
       if ("warning".Equals( $Level, [System.StringComparison]::OrdinalIgnoreCase ) )
       {
            Write-Host -Foreground Yellow $Message
            $error = $true
       }
    }
    
    if (-not $error)
    {
      Write-Output $Message
    }
    
}


###
# checks if the user is executing the command in admin mode or not.
###
function IsAdminMode()
{
  $wid=[System.Security.Principal.WindowsIdentity]::GetCurrent()
  $prp=new-object System.Security.Principal.WindowsPrincipal($wid)
  $adm=[System.Security.Principal.WindowsBuiltInRole]::Administrator
  $IsAdmin=$prp.IsInRole($adm)
  return $IsAdmin
}

function CheckDatabaseExists()
{
   [CmdletBinding()]
     param
     (
          [parameter(Mandatory=$true)]
          [string] $Server,

          [parameter(Mandatory=$true)]
          [string] $Database
     )
    
     $exists = $false
     $SqlConnection = $null


   $data = Invoke-SqlCmd -Server $Server -Database master -Query "SELECT name FROM master.dbo.sysdatabases WHERE name = '$Database'"
   if (( $data -ne $null ) -and ( $data[0] -eq $Database))
   {
      $exists = $true
   }

   return $exists
}

#-----------------------------------------------------
# db functions

function Invoke-SqlCmdForConfiguration
{
    [CmdletBinding()]
    param
    (
        $InputFile,
        $Query
    )
    
    $output = $null
    
    if ($Config_UseUserName)
    {
       if ($InputFile -ne $null)
       {
          $output = Invoke-SqlCmd -Server $Config_sqlServer -Database $Config_dbName -UserName $Config_UserName -Password $Config_Password -InputFile $InputFile
       }
       else
       {
          $output = Invoke-SqlCmd -Server $Config_sqlServer -Database $Config_dbName -UserName $Config_UserName -Password $Config_Password -Query $Query
       }
    }
    else
    {
       if ($InputFile -ne $null)
       {
          $output = Invoke-SqlCmd -Server $Config_sqlServer -Database $Config_dbName -InputFile $InputFile
       }
       else
       {
          $output = Invoke-SqlCmd -Server $Config_sqlServer -Database $Config_dbName -Query $Query
       }    
    }
    
    return $output
}

##--------------------- xml functions

function __CloneNode()
{
    param(
        $parent,
        $node,
        $prefix,
        $nsUri,
        $xmlDocument
    )
       

    if ($node -eq $null)
    {
        throw "node is null"
    }

    if ( $node.NodeType -ne [System.Xml.XmlNodeType]::Element )
    {
         $parent.AppendChild($node.CloneNode($false));
    }
    else
    {
        $newNode = $xmlDocument.CreateElement($prefix, $node.LocalName, $nsUri)
        $parent.AppendChild( $newNode )

        if ($node.HasAttributes)
        {
            $node.Attributes | foreach {
                $attr = $_
                $newNode.SetAttribute($attr.Name, $attr.Value)
            }
        }

        if ($node.ChildNodes -ne $null)
        {
            foreach($child in $node.ChildNodes)
            {
                __CloneNode -parent $newNode -node $child -prefix $prefix -nsUri $nsUri -xmlDocument $xmlDocument  | Out-Null
            }
        }
    }
    
}

function CloneNode()
{
   param(
        $node,
        $prefix,
        $nsUri,
        $xmlDocument
    )

    if ($node -eq $null)
    {
        throw "CloneNode - null node"
    }

     $parent = $xmlDocument.CreateElement($prefix, $node.LocalName, $nsUri)

    if ($node.HasAttributes)
    {
        $node.Attributes | foreach {
            $attr = $_
            $parent.SetAttribute($attr.Name, $attr.Value)
        }
    }
   
    $node.ChildNodes | foreach {
        __CloneNode -parent $parent -node $_ -prefix $prefix -nsUri $nsUri -xmlDocument $xmlDocument | Out-Null
    }

    return $parent
}
