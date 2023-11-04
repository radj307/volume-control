# SetVersion.ps1
#   Powershell helper script for CI scripts to update .csproj files.
#
# Usage:
#   SetVersion -Path <CSPROJ_PATH> [<PROPERTY>=<VALUE>]...
#
# Example:
#   SetVersion -Path MyProject/MyProject.csproj  Version=$(git describe --tags --abbrev=0)
param(
    [Parameter(Mandatory=$true,HelpMessage="Path to the target .csproj file.")][String]$Path="",
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$PropertySetters
)

function Add-XMLAttribute([System.Xml.XmlNode] $Node, $Name, $Value)
{
    $attrib = $Node.OwnerDocument.CreateAttribute($Name)
    $attrib.Value = $Value
    $Node.Attributes.Append($attrib)
}

# Resolve Path
$Path = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($(Get-Location), $Path))

"Reading '$Path'"

# Read XML file
[xml]$CONTENT = New-Object xml ; $CONTENT.Load("$Path")

# doing this prevents failure when there are multiple PropertyGroup nodes:
$TARGET_NODE = $CONTENT.SelectSingleNode("//Project/PropertyGroup")

foreach ($SETTER in $PropertySetters)
{
    $v = $SETTER -split "=",2

    $nodeName = "$($v[0])"

    $previousValue = $TARGET_NODE.$nodeName
    if ($v[1] -eq $previousValue)
    {
        "$($v[0]) was already set to $previousValue"
        continue
    }

    if ($null -eq $previousValue)
    { # Create the element if it doesn't exist
        $newNode = $CONTENT.CreateElement($nodeName)
        $TARGET_NODE.AppendChild($newNode).InnerText = $v[1]
    }
    else
    { # Set the existing element
        $TARGET_NODE.$nodeName = $v[1]
    }
    
    if ($previousValue.Length -eq 0) {
        "Set $($v[0]) to $($v[1])"
    }
    else {
        "Set $($v[0]) to $($v[1]) (Was $previousValue)"
    }
}

$CONTENT.Save("$Path")

"Saved '$Path'"
