!define DOTNET_VERSION "4.5.2"
!include DotNET.nsh
!include LogicLib.nsh
!include MUI.nsh
!include x64.nsh
!include WinVer.nsh

Name "Toastify Installer"
OutFile "ToastifyInstaller.exe"
InstallDir $PROGRAMFILES64\Toastify
RequestExecutionLevel admin
ManifestSupportedOS "{35138b9a-5d96-4fbd-8e2d-a2440225f93a}"  # Windows 7
ManifestSupportedOS "{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}"  # Windows 10

;--------------------------------
; Pages

Page components
Page directory
Page instfiles
 
    # These indented statements modify settings for MUI_PAGE_FINISH
    !define MUI_FINISHPAGE_AUTOCLOSE
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_CHECKED
    !define MUI_FINISHPAGE_RUN_TEXT "Launch Toastify Now"
    !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
  !insertmacro MUI_PAGE_FINISH
  
  ;Languages
!insertmacro MUI_LANGUAGE "English"

;--------------------------------
Function LaunchLink
  ExecShell "" "$INSTDIR\Toastify.exe"
FunctionEnd

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

Section "Toastify (required)"
  SectionIn RO
  
  ; Since process termination is non-destructive for Toastify, just kill it
  DetailPrint "Shutting down Toastify..."
  KillProcWMI::KillProc "Toastify.exe"
  
  ; Let the process shutdown
  Sleep 1000
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Bundle the files
  ${If} ${IsWin10}
    File /oname=ToastifyAPI.dll "ToastifyAPI_UWP.dll"
  ${Else}
    File /oname=ToastifyAPI.dll "ToastifyAPI_Win32.dll"
  ${EndIf}
  
  File "Toastify.exe"	
  File "Toastify.exe.config"	
  File "ManagedWinapi.dll"
  File "Resources\ManagedWinapiNativeHelper.dll"
  File "AutoHotkey.Interop.dll"
  File "Garlic.dll"
  File "Newtonsoft.Json.dll"
  File "SpotifyAPI.dll"
  File "LICENSE"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayName" "Toastify"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayIcon" "$INSTDIR\Toastify.exe,0"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "Publisher" "Jesper Palm, Oren Nachman, Alessandro Attard Barbini"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "Version" "1.9.2"  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayVersion" "1.9.2"
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

Section "Autostart"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify" '"$INSTDIR\Toastify.exe"'
SectionEnd

;--------------------------------
; Uninstaller

Section "Uninstall"
  
  # Remove Start Menu launcher
  Delete "$SMPROGRAMS\Toastify.lnk"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify"
  DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify"

  ; Remove files and uninstaller
  Delete "$INSTDIR\Toastify.exe"
  Delete "$INSTDIR\Toastify.config"
  Delete "$INSTDIR\ToastifyAPI.dll"
  Delete "$INSTDIR\ManagedWinapi.dll"
  Delete "$INSTDIR\ManagedWinapiNativeHelper.dll"
  Delete "$INSTDIR\AutoHotkey.Interop.dll"
  Delete "$INSTDIR\Garlic.dll"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\SpotifyAPI.dll"
  Delete "$INSTDIR\LICENSE"
  Delete "$INSTDIR\uninstall.exe"
  
  ; remove the settings directory
  Delete "$APPDATA\Toastify.xml"
  RMDir "$APPDATA\Toastify"

  ; Remove shortcuts, if any
  Delete "$DESKTOP\Toastify.lnk"

  ; Remove directories used
  RMDir "$INSTDIR"
SectionEnd

Function .onInit
FunctionEnd
