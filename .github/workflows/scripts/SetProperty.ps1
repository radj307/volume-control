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
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$Properties,
    [Parameter(Mandatory=$false,HelpMessage="Whether to write output or not.")][switch]$Quiet
)

function Add-XMLAttribute([System.Xml.XmlNode] $Node, $Name, $Value)
{
    $attrib = $Node.OwnerDocument.CreateAttribute($Name)
    $attrib.Value = $Value
    $Node.Attributes.Append($attrib)
}

# Resolve Path
$Path = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($(Get-Location), $Path))

if (-not $Quiet) {
    "Reading `"$Path`""
}

# Read XML file
[xml]$CONTENT = New-Object xml ; $CONTENT.Load("$Path")

# doing this prevents failure when there are multiple PropertyGroup nodes:
$TARGET_NODE = $CONTENT.SelectSingleNode("//Project/PropertyGroup")

$hasFileChanged = $false

foreach ($prop in $Properties)
{
    $v = $prop -split "=",2
    if ($null -ne $v[1])
    { # Property Setter
        $v = $prop -split "=",2

        $nodeName = "$($v[0])"

        $previousValue = $TARGET_NODE.$nodeName
        if ($v[1] -eq $previousValue)
        {
            if (-not $Quiet) {
                "$($v[0]) was already set to $previousValue"
            }
            continue
        }

        if ($v[1].Length -eq 0)
        { # Delete the element if the incoming value is blank
            $nodeToRemove = $TARGET_NODE.SelectSingleNode("./$nodeName")
            if ($null -ne $nodeToRemove) {
                $TARGET_NODE.RemoveChild($nodeToRemove) > $null

                if (-not $Quiet) { "Removed $nodeName" }
            }
            elseif (-not $Quiet) { "Node $nodeName doesn't exist." }
        }
        elseif ($null -eq $previousValue)
        { # Create the element if it doesn't exist
            $newNode = $CONTENT.CreateElement($nodeName)
            $TARGET_NODE.AppendChild($newNode).InnerText = $v[1]

            if (-not $Quiet) { "Created $nodeName with value `"$($v[1])`"" }
        }
        else
        { # Set the existing element
            $TARGET_NODE.$nodeName = $v[1]
            
            if (-not $Quiet) { "Set $nodeName to `"$($v[1])`" (Was `"$previousValue`")" }
        }

        $hasFileChanged = $true
    }
    else {
        Write-Output $TARGET_NODE.$v
    }
}

if ($hasFileChanged){
    $CONTENT.Save("$Path")

    if (-not $Quiet) {
        "Saved changes to `"$Path`""
    }
}
elseif (-not $Quiet) {
    "No changes were made."
}
