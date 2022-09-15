$SCRIPTVERSION = "4" # The version number of this script file

#Set-Location -Path '..'

Write-Host "Running SetVersion.ps1 $SCRIPTVERSION" # LOG

if ( $args[1] )
{
    "Using Argument `"$args[1]`""
    $global:GIT_TAG_RAW = $args[1]
}
else
{
    $global:GIT_TAG_RAW = $(git describe --tags --abbrev=0)
    "Using Git Tag: `"$global:GIT_TAG_RAW`""
}

"Latest Git Tag:           `"$global:GIT_TAG_RAW`""

$global:GIT_TAG_RAW -cmatch '(?<MAJOR>\d+?)\.(?<MINOR>\d+?)\.(?<PATCH>\d+?)(?<EXTRA>.*)'

$global:TAG = $Matches.MAJOR + '.' + $Matches.MINOR + '.' + $Matches.PATCH


# @brief            Set the version number in the specified csproj file.
# @param file       Target File Path.
# @param version    Incoming Version Number.
function SetVersion
{
    param($file)

    "Reading Project File '$file'..."

    [xml]$CONTENT = Get-Content -Path $file

    $oldversion = $CONTENT.Project.PropertyGroup.Version
    $oldextversion = $CONTENT.Project.PropertyGroup.ExtendedVersion

    if ($oldversion -eq $global:TAG -and $oldextversion -eq $global:GIT_TAG_RAW -and $oldtype -eq $global:TYPE)
    {
        "  No changes, skipping."
        return
    }
    
    "  Outgoing file version:  '$oldversion'`t|   '$oldextversion'"
    "  Incoming file version:  '$global:TAG'`t|   '$global:GIT_TAG_RAW'"

    $CONTENT.Project.PropertyGroup.Version = "$global:TAG"
    $CONTENT.Project.PropertyGroup.ExtendedVersion = "$global:GIT_TAG_RAW"
    $CONTENT.Save("$file")
}

$LOCATION = "$(Get-Location)"

SetVersion("$LOCATION\VolumeControl\VolumeControl.csproj")
SetVersion("$LOCATION\VolumeControl.SDK\VolumeControl.SDK.csproj")

"SetVersion.ps1 Finished."
