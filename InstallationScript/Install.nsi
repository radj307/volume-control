!include LogicLib.nsh
!include MUI.nsh
!include x64.nsh
!include WinVer.nsh

!define ProgramName "Toastify"
!define RegUninstallKey "Software\Microsoft\Windows\CurrentVersion\Uninstall\${ProgramName}"

!include .\DotNetChecker.nsh
!include .\Functions.nsh

!define EstimatedSize 4510

Name "${ProgramName} Installer"
RequestExecutionLevel admin
CRCCheck force

ManifestSupportedOS "{35138b9a-5d96-4fbd-8e2d-a2440225f93a}"  # Windows 7
ManifestSupportedOS "{1f676c76-80e1-4239-95bb-83d0f6d0da78}"  # Windows 8.1
ManifestSupportedOS "{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}"  # Windows 10

OutFile "ToastifyInstaller.exe"
InstallDir $PROGRAMFILES64\Toastify

;--------------------------------
; Pages

Page components
Page directory
Page instfiles

# These statements modify settings for MUI_PAGE_FINISH
!define MUI_FINISHPAGE_AUTOCLOSE
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_NOTCHECKED
!define MUI_FINISHPAGE_RUN_TEXT "Launch ${ProgramName} now"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchApplication"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; Installer

Section "${ProgramName} (required)"
  SectionIn RO
  AddSize ${EstimatedSize}
  
  # Since process termination is non-destructive for Toastify, just kill it
  DetailPrint "Shutting down ${ProgramName}..."
  KillProcWMI::KillProc "Toastify.exe"
  Sleep 2000
  
  # Uninstall previous versions
  DetailPrint "Uninstalling previous versions of ${ProgramName}..."
  Call UninstallPreviousVersions

  # Check .NET Framework
  !insertmacro CheckNetFramework 45
  
  # Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  # Bundle the files
  ${If} ${IsWin10}
    File /oname=ToastifyAPI.dll "ToastifyAPI_UWP.dll"
    File /oname=ToastifyAPI.pdb "ToastifyAPI_UWP.pdb"
  ${Else}
    File /oname=ToastifyAPI.dll "ToastifyAPI_Win32.dll"
    File /oname=ToastifyAPI.pdb "ToastifyAPI_Win32.pdb"
  ${EndIf}
  
  File "Toastify.exe"
  File "Toastify.exe.config"
  File "Toastify.pdb"
  File "GoogleMeasurementProtocol.dll"
  File "log4net.dll"
  File "ManagedWinapi.dll"
  File "Resources\ManagedWinapiNativeHelper.dll"
  File "Newtonsoft.Json.dll"
  File "PowerArgs.dll"
  File "SpotifyAPI.dll"
  File "Xceed.Wpf.Toolkit.dll"
  File "LICENSE"
  File "LICENSE-3RD-PARTY"

  # Remove files belonging to old versions
  Delete "$INSTDIR\Garlic.dll"
  Delete "$INSTDIR\uninstall.exe"
  
  # Write the uninstall keys for Windows
  WriteRegStr HKLM "${RegUninstallKey}" "DisplayName" "Toastify"
  WriteRegStr HKLM "${RegUninstallKey}" "UninstallString" '"$INSTDIR\uninst.exe"'
  WriteRegStr HKLM "${RegUninstallKey}" "DisplayIcon" "$INSTDIR\Toastify.exe,0"
  WriteRegStr HKLM "${RegUninstallKey}" "Publisher" "Jesper Palm, Oren Nachman, Alessandro Attard Barbini"
  WriteRegStr HKLM "${RegUninstallKey}" "Version" "1.10.8"  
  WriteRegStr HKLM "${RegUninstallKey}" "DisplayVersion" "1.10.8"
  WriteRegDWORD HKLM "${RegUninstallKey}" "EstimatedSize" ${EstimatedSize}
  WriteRegDWORD HKLM "${RegUninstallKey}" "NoModify" 1
  WriteRegDWORD HKLM "${RegUninstallKey}" "NoRepair" 1
  WriteUninstaller "uninst.exe"

SectionEnd

Section "Desktop icon"
  CreateShortCut "$DESKTOP\Toastify.lnk" "$INSTDIR\Toastify.exe" "" "$INSTDIR\Toastify.exe" 0
SectionEnd

Section "Start Menu icon"
  CreateShortCut "$SMPROGRAMS\Toastify.lnk" "$INSTDIR\Toastify.exe" "" "$INSTDIR\Toastify.exe" 0
SectionEnd

Section /o "Autostart"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify" '"$INSTDIR\Toastify.exe"'
SectionEnd

;--------------------------------
; Uninstaller

Section "Uninstall"
  
  # Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify"
  DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify"

  # Remove files and uninstaller
  Delete "$INSTDIR\Toastify.exe"
  Delete "$INSTDIR\Toastify.exe.config"
  Delete "$INSTDIR\Toastify.pdb"
  Delete "$INSTDIR\ToastifyAPI.dll"
  Delete "$INSTDIR\ToastifyAPI.pdb"
  Delete "$INSTDIR\GoogleMeasurementProtocol.dll"
  Delete "$INSTDIR\log4net.dll"
  Delete "$INSTDIR\ManagedWinapi.dll"
  Delete "$INSTDIR\ManagedWinapiNativeHelper.dll"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\PowerArgs.dll"
  Delete "$INSTDIR\SpotifyAPI.dll"
  Delete "$INSTDIR\Xceed.Wpf.Toolkit.dll"
  Delete "$INSTDIR\LICENSE"
  Delete "$INSTDIR\LICENSE-3RD-PARTY"
  Delete "$INSTDIR\uninst.exe"
  
  # remove the settings directory
  Delete "$APPDATA\Toastify.xml"
  RMDir "$APPDATA\Toastify"

  # Remove shortcuts
  Delete "$DESKTOP\Toastify.lnk"
  Delete "$SMPROGRAMS\Toastify.lnk"

  # Remove directories used
  RMDir "$INSTDIR"

SectionEnd


;--------------------------------
; Functions

Function .onInit
FunctionEnd

Function LaunchApplication
  ShellExecAsUser::ShellExecAsUser "" "$INSTDIR\Toastify.exe" ""
FunctionEnd
