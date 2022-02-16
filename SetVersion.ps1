# VARIABLES

cd ..

$global:VC = "$(Get-Location)\VolumeControl\VolumeControl.csproj"
$global:VCCLI = "$(Get-Location)\VolumeControlCLI\VolumeControlCLI.csproj"
$RGX=[regex]"\d+?\.\d+?\.\d+"
$TAG = $(git describe --tags --abbrev=0)
$global:TAG = $RGX.Match($TAG).Value

$SCRIPTVERSION = "0.0.0" # The version number of this script file

# SCRIPT

Write-Host "Running SetVersion.ps1 $SCRIPTVERSION" # LOG

"Working Directory:      `"$(Get-Location)`""
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

    "Project File Location:  `"$file`""
    "Current file version:   `"$oldversion`""
    "Incoming file version:  `"$global:TAG`""

    $CONTENT.Project.PropertyGroup.Version = $global:TAG
    $CONTENT.Save("$file")
}

SetVersion($global:VC)
SetVersion($global:VCCLI)

"SetVersion.ps1 Finished."
