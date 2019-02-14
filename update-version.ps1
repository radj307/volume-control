$ScriptPath = $MyInvocation.MyCommand.Path | Split-Path
Set-Location "$ScriptPath"

$AsciiEncoding = New-Object System.Text.ASCIIEncoding

Write-Host -NoNewLine "Current version: "
Get-Content "Toastify\version"
$NewVersion = Read-Host -Prompt "New version"
$vMajor,$vMinor,$vBuild = $NewVersion.split('.')

Write-Host

# InstallationScript\Install.nsi
Write-Host "Update: InstallationScript\Install.nsi"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "InstallationScript\Install.nsi"))
$Lines = (Get-Content 'InstallationScript\Install.nsi') -replace '(VERSIONMAJOR) .*', "`$1 $vMajor"
$Lines = ($Lines) -replace '(VERSIONMINOR) .*', "`$1 $vMinor"
$Lines = ($Lines) -replace '(VERSIONBUILD) .*', "`$1 $vBuild"
[System.IO.File]::WriteAllLines($FilePath, $Lines)

# Toastify\version
Write-Host "Update: Toastify\version"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "Toastify\version"))
$Lines = "$NewVersion"
[System.IO.File]::WriteAllText($FilePath, $Lines, $AsciiEncoding)

# Toastify\Toastify.csproj
Write-Host "Update: Toastify\Toastify.csproj"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "Toastify\Toastify.csproj"))
$Lines = (Get-Content 'Toastify\Toastify.csproj' -Encoding UTF8)
$Lines = ($Lines) -replace '^(\s*)<Version>(.*)</Version>$', "`$1<Version>$NewVersion</Version>"
[System.IO.File]::WriteAllLines($FilePath, $Lines)

# ToastifyAPI\Properties\AssemblyInfo.cs
Write-Host "Update: ToastifyAPI\Properties\AssemblyInfo.cs"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "ToastifyAPI\Properties\AssemblyInfo.cs"))
$Lines = (Get-Content 'ToastifyAPI\Properties\AssemblyInfo.cs' -Encoding UTF8)
$Lines = ($Lines) -replace '^\[(assembly: AssemblyVersion)\(".*"\)\]$', "[`$1(`"$NewVersion.*`")]"
$Lines = ($Lines) -replace '^\[(assembly: AssemblyFileVersion)\(".* \[DEBUG BUILD\]"\)\]$', "[`$1(`"$NewVersion [DEBUG BUILD]`")]"
$Lines = ($Lines) -replace '^\[(assembly: AssemblyFileVersion)\(".* \[TEST BUILD\]"\)\]$', "[`$1(`"$NewVersion [TEST BUILD]`")]"
[System.IO.File]::WriteAllLines($FilePath, $Lines)

Write-Host "Press any key to continue ..."
$x = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")