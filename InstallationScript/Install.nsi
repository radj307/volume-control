!include "DotNET.nsh"
!include LogicLib.nsh
!define DOTNET_VERSION "3.5"
!include "MUI.nsh"

; The name of the installer
Name "Toastify Installer"

; The file to write
OutFile "ToastifyInstaller.exe"

; The default installation directory
InstallDir $PROGRAMFILES\Toastify

; Request application privileges for Windows Vista
RequestExecutionLevel admin

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

  ; Likewise for ChromeDriver
  DetailPrint "Shutting down ChromeDriver..."
  KillProcWMI::KillProc "chromedriver.exe"
  
  ; Let the process shutdown
  Sleep 1000
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File "Toastify.exe"	
  File "ToastifyApi.dll"
  File "ManagedWinapi.dll"
  File "Resources\ManagedWinapiNativeHelper.dll"
  File "AutoHotkey.Interop.dll"
  File "chromedriver.exe"
  File "WebDriver.dll"
  File "LICENSE"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayName" "Toastify"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayIcon" "$INSTDIR\Toastify.exe,0"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "Publisher" "Jesper Palm"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "Version" "1.6"  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "DisplayVersion" "1.6"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
SectionEnd

Section "Desktop icon"
  CreateShortCut "$DESKTOP\Toastify.lnk" "$INSTDIR\Toastify.exe" "" "$INSTDIR\Toastify.exe" 0
SectionEnd

Section "Start Menu icon"
  # Start Menu
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
  Delete "$INSTDIR\ToastifyApi.dll"
  Delete "$INSTDIR\ManagedWinapi.dll"
  Delete "$INSTDIR\ManagedWinapiNativeHelper.dll"
  Delete "$INSTDIR\AutoHotkey.Interop.dll"
  Delete "$INSTDIR\LICENSE"
  
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
