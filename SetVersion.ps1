# ARGUMENT: FILEPATH
param([String]$Path="")

# Resolve Path
$Path = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($(Get-Location), $Path))

# Get Git Tag, 3-part tag, & copyright notice
$TAG = "$(git describe --tags --abbrev=0)"
$TAG -cmatch '(?<MAJOR>\d+?)\.(?<MINOR>\d+?)\.(?<PATCH>\d+?)(?<EXTRA>.*)' > $null
$TAG_3_PART = $Matches.MAJOR + '.' + $Matches.MINOR + '.' + $Matches.PATCH
$COPYRIGHT = "Copyright © $((Get-Date).Year) by `$`(Authors`)"

"Reading '$Path'"

# Read XML file
[xml]$CONTENT = Get-Content -Path "$Path"

# doing this prevents failure when there are multiple PropertyGroup nodes:
$TARGET_NODE = $CONTENT.SelectSingleNode("//Project/PropertyGroup")

$TARGET_NODE.FileVersion  = $TAG_3_PART
"Set FileVersion to '$TAG_3_PART' (was '$($TARGET_NODE.FileVersion)')"
$TARGET_NODE.Version      = $TAG
"Set Version to '$TAG' (was '$($TARGET_NODE.Version)')"
$TARGET_NODE.Copyright    = $COPYRIGHT
"Set Copyright to '$COPYRIGHT' (was '$($TARGET_NODE.Copyright)')"

$CONTENT.Save("$Path")

"Saved '$Path'"
