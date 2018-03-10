!include LogicLib.nsh
!include MUI.nsh
!include x64.nsh
!include WinVer.nsh
!include DotNetChecker.nsh

Name "Toastify Installer"
OutFile "ToastifyInstaller.exe"
InstallDir $PROGRAMFILES64\Toastify
RequestExecutionLevel admin
ManifestSupportedOS "{35138b9a-5d96-4fbd-8e2d-a2440225f93a}"  # Windows 7
ManifestSupportedOS "{1f676c76-80e1-4239-95bb-83d0f6d0da78}"  # Windows 8.1
ManifestSupportedOS "{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}"  # Windows 10

;--------------------------------
; Pages

Page components
Page directory
Page instfiles
 
# These statements modify settings for MUI_PAGE_FINISH
!define MUI_FINISHPAGE_AUTOCLOSE
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_CHECKED
!define MUI_FINISHPAGE_RUN_TEXT "Launch Toastify Now"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; Functions

Function LaunchLink
  ExecShell "" "$INSTDIR\Toastify.exe"
FunctionEnd

;--------------------------------
; Installer

Section "Toastify (required)"
  SectionIn RO
  
  # Since process termination is non-destructive for Toastify, just kill it
  DetailPrint "Shutting down Toastify..."
  KillProcWMI::KillProc "Toastify.exe"
  
  # Let the process shutdown
  Sleep 1000

  # Check .NET Framework
  !insertmacro CheckNetFramework 45
  
  # Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  # Bundle the files
  ${If} ${IsWin10}
    File /oname=ToastifyAPI.dll "ToastifyAPI_UWP.dll"
  ${Else}
    File /oname=ToastifyAPI.dll "ToastifyAPI_Win32.dll"
  ${EndIf}

  # Remove files belonging to old versions
  Delete "$INSTDIR\Garlic.dll"
  
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
  
  # Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayName" "Toastify"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayIcon" "$INSTDIR\Toastify.exe,0"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "Publisher" "Jesper Palm, Oren Nachman, Alessandro Attard Barbini"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "Version" "1.10.4"  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayVersion" "1.10.4"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "NoRepair" 1
  WriteUninstaller "uninstall.exe"

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
  
  # Remove Start Menu launcher
  Delete "$SMPROGRAMS\Toastify.lnk"
  
  # Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify"
  DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify"

  # Remove files and uninstaller
  Delete "$INSTDIR\Toastify.exe"
  Delete "$INSTDIR\Toastify.exe.config"
  Delete "$INSTDIR\Toastify.pdb"
  Delete "$INSTDIR\ToastifyAPI.dll"
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
  Delete "$INSTDIR\uninstall.exe"
  
  # remove the settings directory
  Delete "$APPDATA\Toastify.xml"
  RMDir "$APPDATA\Toastify"

  # Remove shortcuts, if any
  Delete "$DESKTOP\Toastify.lnk"

  # Remove directories used
  RMDir "$INSTDIR"

SectionEnd

Function .onInit
FunctionEnd
