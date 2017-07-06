$ScriptPath = $MyInvocation.MyCommand.Path | Split-Path
Set-Location "$ScriptPath"

$AsciiEncoding = New-Object System.Text.ASCIIEncoding

Write-Host -NoNewLine "Current version: "
Get-Content "Toastify\version"
$NewVersion = Read-Host -Prompt "New version"

Write-Host

Write-Host "Update: InstallationScript\Install.nsi"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "InstallationScript\Install.nsi"))
$Lines = (Get-Content 'InstallationScript\Install.nsi') -replace '("(Display)?Version") ".*"', "`$1 `"$NewVersion`""
[System.IO.File]::WriteAllLines($FilePath, $Lines)

Write-Host "Update: Toastify\version"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "Toastify\version"))
$Lines = "$NewVersion"
[System.IO.File]::WriteAllText($FilePath, $Lines, $AsciiEncoding)

Write-Host "Update: Toastify\Properties\AssemblyInfo.cs"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "Toastify\Properties\AssemblyInfo.cs"))
$Lines = (Get-Content 'Toastify\Properties\AssemblyInfo.cs' -Encoding UTF8) -replace '^\[(assembly: AssemblyVersion)\(".*"\)\]$', "[`$1(`"$NewVersion.*`")]"
[System.IO.File]::WriteAllLines($FilePath, $Lines)

Write-Host "Update: ToastifyAPI\Properties\AssemblyInfo.cs"
$FilePath = [System.IO.Path]::GetFullPath((Join-Path (pwd) "ToastifyAPI\Properties\AssemblyInfo.cs"))
$Lines = (Get-Content 'ToastifyAPI\Properties\AssemblyInfo.cs' -Encoding UTF8) -replace '^\[(assembly: AssemblyVersion)\(".*"\)\]$', "[`$1(`"$NewVersion.*`")]"
[System.IO.File]::WriteAllLines($FilePath, $Lines)

Write-Host "Press any key to continue ..."
$x = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")