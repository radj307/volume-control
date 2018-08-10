!define APPNAME "Toastify"
!define PUBLISHER "Jesper Palm, Oren Nachman, Alessandro Attard Barbini"
!define DESCRIPTION "Toastify adds global hotkeys and toast notifications to Spotify"
!define VERSIONMAJOR 1
!define VERSIONMINOR 10
!define VERSIONBUILD 9
!define HELPURL "https://github.com/aleab/toastify/issues"
!define UPDATEURL "https://github.com/aleab/toastify/releases"
!define ABOUTURL "https://aleab.github.io/toastify/"
!define ESTIMATEDSIZE 4510

!define RegUninstallKey "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"


!include LogicLib.nsh
!include MUI.nsh
!include x64.nsh
!include WinVer.nsh
!include .\NSISpcre.nsh
!include .\DotNetChecker.nsh
!include .\Functions.nsh

Name "${APPNAME} Installer"
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
  !define MUI_FINISHPAGE_RUN_TEXT "Launch ${APPNAME} now"
  !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchApplication"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

UninstPage components
UninstPage uninstConfirm
UninstPage instfiles


;--------------------------------
; Installer

Var /GLOBAL PrevDesktopShortcutArgs
Var /GLOBAL PrevSMShortcutArgs
Var /GLOBAL PrevAutostartArgs

Section "${APPNAME} (required)"
  SectionIn RO
  AddSize ${EstimatedSize}
  
  # Since process termination is non-destructive for Toastify, just kill it
  DetailPrint "Shutting down ${APPNAME}..."
  KillProcWMI::KillProc "Toastify.exe"
  Sleep 2000
  
  # Uninstall previous versions
  DetailPrint "Uninstalling previous versions of ${APPNAME}..."
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
  WriteRegStr HKLM "${RegUninstallKey}" "DisplayName" "${APPNAME}"
  WriteRegStr HKLM "${RegUninstallKey}" "DisplayIcon" "$INSTDIR\Toastify.exe,0"
  WriteRegStr HKLM "${RegUninstallKey}" "Publisher" "${PUBLISHER}"
  WriteRegStr HKLM "${RegUninstallKey}" "Version" "${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}"
  WriteRegStr HKLM "${RegUninstallKey}" "DisplayVersion" "${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}"
  WriteRegStr HKLM "${RegUninstallKey}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKLM "${RegUninstallKey}" "UninstallString" '"$INSTDIR\uninst.exe"'
  WriteRegStr HKLM "${RegUninstallKey}" "QuietUninstallString" '"$INSTDIR\uninst.exe" /S'
  WriteRegStr HKLM "${RegUninstallKey}" "HelpLink" "${HELPURL}"
  WriteRegStr HKLM "${RegUninstallKey}" "URLUpdateInfo" "${UPDATEURL}"
  WriteRegStr HKLM "${RegUninstallKey}" "URLInfoAbout" "${ABOUTURL}"
  WriteRegDWORD HKLM "${RegUninstallKey}" "EstimatedSize" ${EstimatedSize}
  WriteRegDWORD HKLM "${RegUninstallKey}" "NoModify" 1
  WriteRegDWORD HKLM "${RegUninstallKey}" "NoRepair" 1
  WriteUninstaller "uninst.exe"
SectionEnd

Section "Desktop icon"
  CreateShortCut "$DESKTOP\Toastify.lnk" "$INSTDIR\Toastify.exe" "$PrevDesktopShortcutArgs" "$INSTDIR\Toastify.exe" 0
SectionEnd

Section "Start Menu icon"
  CreateShortCut "$SMPROGRAMS\Toastify.lnk" "$INSTDIR\Toastify.exe" "$PrevSMShortcutArgs" "$INSTDIR\Toastify.exe" 0
SectionEnd

Section /o "Autostart"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify" '"$INSTDIR\Toastify.exe"$PrevAutostartArgs'
SectionEnd


;--------------------------------
; Uninstaller

Section "un.Toastify"
  SectionIn RO

  # Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Toastify"
  DeleteRegValue HKCU "Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Run" "Toastify"
  DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify"
  DeleteRegValue HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify"

  # Remove files
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

  # Remove log files
  Delete "$LOCALAPPDATA\Toastify\Toastify.log*"

  # Remove shortcuts
  Delete "$DESKTOP\Toastify.lnk"
  Delete "$SMPROGRAMS\Toastify.lnk"

  # Remove directories
  RMDir /r "$INSTDIR"
SectionEnd

Section /o "un.Settings"
  Delete "$APPDATA\Toastify\Toastify.cfg"
  Delete "$LOCALAPPDATA\Toastify\proxy.sec"
SectionEnd


;--------------------------------
; Functions

Function .onInit
  Push $R0
  Push $R1

  StrCpy $INSTDIR "$PROGRAMFILES64\Toastify"

  StrCpy $PrevDesktopShortcutArgs ""
  StrCpy $PrevSMShortcutArgs ""
  StrCpy $PrevAutostartArgs ""

  # Get previous install location
  ReadRegStr $R0 HKLM "${RegUninstallKey}" "InstallLocation"
  ${If} $R0 != ""
    StrCpy $INSTDIR "$R0"
  ${Else}
    ReadRegStr $R0 HKLM "${RegUninstallKey}" "UninstallString"
    ${If} $R0 != ""
      ${TrimQuotes} $R0 $R0
      ${GetParent} $R0 $R1
      StrCpy $INSTDIR "$R1"
    ${EndIf}
  ${EndIf}
  
  # Get the arguments of the previous Desktop shortcut
  ${If} ${FileExists} "$DESKTOP\Toastify.lnk"
    ClearErrors
    ShellLink::GetShortCutArgs "$DESKTOP\Toastify.lnk"
    ${IfNot} ${Errors}
      Pop $PrevDesktopShortcutArgs
    ${EndIf}
  ${EndIf}

  # Get the arguments of the previous StartMenu shortcut
  ${If} ${FileExists} "$SMPROGRAMS\Toastify.lnk"
    ClearErrors
    ShellLink::GetShortCutArgs "$SMPROGRAMS\Toastify.lnk"
    ${IfNot} ${Errors}
      Pop $PrevSMShortcutArgs
    ${EndIf}
  ${EndIf}

  # Get the arguments of the previous Autostart entry
  ReadRegStr $R0 HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "Toastify"
  ${If} $R0 != ""
    ${RECaptureMatches} $0 '.*Toastify.exe"?(.*)' $R0 0
    StrCpy $PrevAutostartArgs $1
  ${EndIf}

  Pop $R1
  Pop $R0
FunctionEnd

Function LaunchApplication
  # Should we use some command line argument?
  ${If} $PrevAutostartArgs != ""
    StrCpy $0 $PrevAutostartArgs
  ${ElseIf} $PrevSMShortcutArgs != ""
    StrCpy $0 $PrevSMShortcutArgs
  ${Else}
    StrCpy $0 $PrevDesktopShortcutArgs
  ${EndIf}

  ShellExecAsUser::ShellExecAsUser "" "$INSTDIR\Toastify.exe" "$0"
FunctionEnd
