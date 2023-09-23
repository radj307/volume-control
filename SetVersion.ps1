# ARGUMENT: FILEPATH
param([String]$Path="")

$Path = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($(Get-Location), $Path))

"Setting Version Attributes in '$Path'"

# Get Git Tag
$TAG = "$(git describe --tags --abbrev=0)"
$TAG -cmatch '(?<MAJOR>\d+?)\.(?<MINOR>\d+?)\.(?<PATCH>\d+?)(?<EXTRA>.*)' > $null
$TAG_3_PART = $Matches.MAJOR + '.' + $Matches.MINOR + '.' + $Matches.PATCH

"Tag:         '$TAG'"
if ($TAG -ne $TAG_3_PART) {
    "3-Part Tag:  '$TAG_3_PART'"
}

# Read file
[xml]$CONTENT = Get-Content -Path "$Path"

$CONTENT.Project.PropertyGroup.FileVersion  = $TAG_3_PART
$CONTENT.Project.PropertyGroup.Version      = $TAG
$CONTENT.Project.PropertyGroup.Copyright    = "Copyright © $((Get-Date).Year) by `$`(Authors`)"

$CONTENT.Save("$Path")

"Saved '$Path'"
