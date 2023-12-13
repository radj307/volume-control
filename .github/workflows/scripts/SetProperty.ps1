# SetProperty.ps1 by radj307
#   Powershell helper script for CI scripts to get or set properties in a .csproj or .props file.
param(
    [Parameter(Mandatory = $true, HelpMessage = "Path to the target file.")][String]$Path = "",
    [Parameter(ValueFromRemainingArguments = $true)][String[]]$Properties,
    [Parameter(Mandatory = $false, HelpMessage = "Prevents log messages from being written to the console. Useful with property getters.")][switch]$Quiet,
    [Parameter(Mandatory = $false, HelpMessage = "Removes properties when setting them to an empty value.")][switch]$CanRemove,
    [Parameter(Mandatory = $false, HelpMessage = "Trims single & double quotes from values.")][switch]$TrimQuotes
)

function Add-XMLAttribute([System.Xml.XmlNode] $Node, $Name, $Value) {
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

foreach ($prop in $Properties) {
    $v = $prop -split "=", 2

    if ($null -ne $v[1]) {
        # Property Setter
        $v = $prop -split "=", 2

        if ($TrimQuotes) {
            $v[1] = $v[1].Trim("`'", "`"")
        }

        $nodeName = "$($v[0])"
        $valueIsEmpty = $($v[1].Length -eq 0)

        $previousValue = $TARGET_NODE.$nodeName
        if (-not $valueIsEmpty -and $v[1] -eq $previousValue) {
            if (-not $Quiet) {
                "$($v[0]) was already set to $previousValue"
            }
            continue
        }

        if ($valueIsEmpty) {
            if ($CanRemove) {
                # Delete the element if the incoming value is blank
                $nodeToRemove = $TARGET_NODE.SelectSingleNode("./$nodeName")
                if ($null -ne $nodeToRemove) {
                    $TARGET_NODE.RemoveChild($nodeToRemove) > $null

                    if (-not $Quiet) { "Removed $nodeName" }
                }
                elseif (-not $Quiet) {
                    if ($CanRemove) {
                        "Not creating $nodeName. (Value is blank & '-CanRemove' was specified)"
                    }
                    else {
                        "Node $nodeName doesn't exist."
                    }
                }
            }
            elseif (-not $Quiet) { "Not removing $nodeName. (Specify the '-CanRemove' switch to enable removing properties." }
        }
        elseif ($null -eq $previousValue) {
            # Create the element if it doesn't exist
            $newNode = $CONTENT.CreateElement($nodeName)
            $TARGET_NODE.AppendChild($newNode).InnerText = $v[1]

            if (-not $Quiet) { "Created $nodeName with value `"$($v[1])`"" }
        }
        else {
            # Set the existing element
            $TARGET_NODE.$nodeName = $v[1]
            
            if (-not $Quiet) { "Set $nodeName to `"$($v[1])`" (Was `"$previousValue`")" }
        }

        $hasFileChanged = $true
    }
    else {
        # Property Getter
        Write-Host $TARGET_NODE.$v
    }
}

if ($hasFileChanged) {
    $CONTENT.Save("$Path")

    if (-not $Quiet) {
        "Saved changes to `"$Path`""
    }
}
elseif (-not $Quiet) {
    "No changes were made."
}
