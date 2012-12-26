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
#######################################################################
$tab = "`t";
$newline = [environment]::newline

function AddUsing([string]$use)
{    
    $script:code = $script:code + "using " + $use + ";" + $newline;
}

function SetNamespace
{
    param ([string]$namespace)
    
    $script:code += "namespace " + $namespace + $newline;
}

function SetClass([string]$class)
{
    AddTab
    $script:code += "public sealed class " + $class + $newline
}

function AddProperty([string] $propertyType, [string] $propertyName, [string] $propertyValue)
{    
    AddTab 
    $script:code += "public const " + $propertyType + " " + $propertyName + " = ";
    if ( $propertyType -eq "int")
    {
        $script:code += $propertyValue 
    }
    else
    {
        $script:code += "'" + $propertyValue + "'"
    }
    
    $script:code += ";" + $newline;
}

function AddStartBrace()
{
    AddTab
    $script:code += "{" + $newline;
}

function AddEndBrace()
{
    AddTab
    $script:code += "}" + $newline;
}

function AddTab()
{
    for($i=0; $i -lt $script:tabCount; $i++)
    {
        $script:code += $tab;
    } 
}

function IncreaseIndent()
{
    $script:tabCount++;
}

function DecreaseIndent()
{
    $script:tabCount--;
}

function ProcessClass($classNode)
{
    IncreaseIndent
    SetClass($classNode.Name);
    AddStartBrace
    
    #recurse on class
    foreach($node in $classNode.ChildNodes)
    {
        if ($node.LocalName -Eq "Class")
        {
            ProcessClass $node
        }
    }
    
    IncreaseIndent
    # add the properties

    foreach($node in $classNode.ChildNodes)
    {
        if ($node.LocalName -Eq "Property")
        {
            AddProperty $node.Type $node.Name $node.Value
        }
    }

    DecreaseIndent
    
    AddEndBrace
    DecreaseIndent
}

function ProcessTokens($classNode)
{

    foreach($node in $classNode.ChildNodes)
    {
        if ($node.LocalName -Eq "Class")
        {
            ProcessTokens $node
        }
    }
   
    # read the token
   
    foreach($node in $classNode.ChildNodes)
    {
        if ($node.LocalName -Eq "Property")
        {
            if ($node.Token -Ne $null)
            { 
                $script:code += $node.Token + $tab + $node.Value + $newline
            }
            
            #check if there are other tokens
            if ($node.Tokens -Ne $null)
            {
                foreach($tokenNode in $node.Tokens.ChildNodes)
                {
                    if ($tokenNode.LocalName -Eq "Token")
                    {
                        $script:code += $tokenNode.Name + $tab + $node.Value + $newline
                    }
                }
            }    
        }
    }    
}

function GenerateCode($xml)
{
    $script:code += "// auto generated code" + $newline;
    
    $namespaceNode = $xml.Namespace
    $classNode = $namespaceNode.class
    
    AddUsing( "System" );
    SetNamespace( $namespaceNode.Name );
    AddStartBrace
    
    ProcessClass $classNode

    DecreaseIndent
    AddEndBrace;
}

function GenerateTokens($xml)
{
    $classNode = $xml.Namespace.class
    
    ProcessTokens($classNode)
    
}

#######################################################################

$token = "-token"
$class = "-class"

$xmlFile = $args[0]
$type = $args[1]
$filename = $args[2]

Write-Host "Xml File :"  $xmlFile  " Type: "  $type  " Filename "  $filename

[xml]$xml=get-content $xmlFile

$tabCount = 0;
$code = ""

if ($type -Eq $class)
{
    GenerateCode $xml
}
else 
{
    if ($type -Eq $token)
    {
        GenerateTokens $xml
    }
}


if ($filename -Eq $null)
{
    $code
}
else
{
    #write code to the file
    $code | Out-File -encoding ascii $filename 
}    

exit
