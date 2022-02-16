$SCRIPTVERSION = "1" # The version number of this script file

$ROOTPATH=$args[0]

if ($ROOTPATH)
{
    Set-Location -Path $ROOTPATH
}
else
{
    Set-Location -Path '..'
}

Write-Host "Running SetVersion.ps1 $SCRIPTVERSION" # LOG

# .csproj locations
$global:VC = "$(Get-Location)\VolumeControl\VolumeControl.csproj"
$global:VCCLI = "$(Get-Location)\VolumeControlCLI\VolumeControlCLI.csproj"

$GIT_TAG_RAW = $(git describe --tags --abbrev=0)

"Latest Git Tag:           `"$GIT_TAG_RAW`""

$GIT_TAG_RAW -cmatch '(?<MAJOR>\d+?)\.(?<MINOR>\d+?)\.(?<PATCH>\d+?)(?<EXTRA>.*)'

$global:TAG = $Matches.MAJOR + '.' + $Matches.MINOR + '.' + $Matches.PATCH

"Major Version Number:     `"" + $Matches.MAJOR + "`""
"Minor Version Number:     `"" + $Matches.MINOR + "`""
"Patch Version Number:     `"" + $Matches.PATCH + "`""
"Version Number Extras:    `"" + $Matches.EXTRA + "`""

$EXTRA = $Matches.EXTRA
if ($EXTRA)
{
    if ($EXTRA -like "*-pr*")
    {
        $global:PRERELEASE = 'true'
        
        "Tag indicates that this is a pre-release version."
    }
    
    $EXTRA -cmatch '(?<DIGITS>\d+)'
    
    $global:TAG = $global:TAG + '.' + $Matches.DIGITS
}

"Working Directory:        `"$(Get-Location)`""
"Tag Version Number:       `"$TAG`""
"VolumeControl.csproj:     `"$VC`""
"VolumeControlCLI.csproj:  `"$VCCLI`""

# @brief            Set the version number in the specified csproj file.
# @param file       Target File Path.
# @param version    Incoming Version Number.
function SetVersion
{
    param($file)

    [xml]$CONTENT = Get-Content -Path $file

    $oldversion = $CONTENT.Project.PropertyGroup.Version
    $isPreRelease = $CONTENT.Project.PropertyGroup.IsPreRelease
    
    "Project File Location:  `"$file`""
    "Current file version:   `"$oldversion`""
    "Incoming file version:  `"$global:TAG`""

    if ($oldversion -ne $global:TAG -or $isPreRelease -ne $global:PRERELEASE)
    {
        if ($global:PRERELEASE)
        {
            $CONTENT.Project.PropertyGroup.IsPreRelease = 'true'
        }
        else
        {   
            $CONTENT.Project.PropertyGroup.IsPreRelease = 'false'
        }
        
        $CONTENT.Project.PropertyGroup.Version = $global:TAG
        $CONTENT.Save("$file")
    }
    else
    {
        "Version Numbers Match. Nothing happened."
    }
}

SetVersion($global:VC)
SetVersion($global:VCCLI)

"SetVersion.ps1 Finished."
