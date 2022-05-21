$SCRIPTVERSION = "3" # The version number of this script file

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

$EXTRA = $Matches.EXTRA
if ($EXTRA)
{
    if ($EXTRA -like "*pre*" -or $EXTRA -like "*pr*")
    {
        "Is a prerelease."
        $global:TYPE = 'PRERELEASE'
    }
    elseif ($EXTRA -like "*rc*")
    {
        "Is a release candidate."
        $global:TYPE = 'CANDIDATE'
    }
    else
    {
        "Is a revision of a release."
        $global:TYPE = 'REVISION'
    }
}
else {
    "Is a normal release."
    $global:TYPE = 'NORMAL'
}

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
    $oldtype = $CONTENT.Project.PropertyGroup.ReleaseType

    if ($oldversion -eq $global:TAG -and $oldextversion -eq $global:GIT_TAG_RAW -and $oldtype -eq $global:TYPE)
    {
        "  No changes, skipping."
        return
    }
    
    "  Outgoing file version:  '$oldversion'`t|   '$oldextversion'"
    "  Incoming file version:  '$global:TAG'`t|   '$global:GIT_TAG_RAW'"

    $CONTENT.Project.PropertyGroup.Version = "$global:TAG"
    $CONTENT.Project.PropertyGroup.ExtendedVersion = "$global:GIT_TAG_RAW"
    $CONTENT.Project.PropertyGroup.ReleaseType = "$global:TYPE"
    $CONTENT.Save("$file")
}

$LOCATION = "$(Get-Location)"

SetVersion("$LOCATION\VolumeControl\VolumeControl.csproj")
#SetVersion("$LOCATION\VolumeControl.CLI\VolumeControl.CLI.csproj")

"SetVersion.ps1 Finished."
