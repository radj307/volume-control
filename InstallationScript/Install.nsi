!define APPNAME "Toastify"
!define PUBLISHER "Jesper Palm, Oren Nachman, Alessandro Attard Barbini"
!define DESCRIPTION "Toastify adds global hotkeys and toast notifications to Spotify"
!define VERSIONMAJOR 1
!define VERSIONMINOR 11
!define VERSIONBUILD 0
!define HELPURL "https://github.com/aleab/toastify/issues"
!define UPDATEURL "https://github.com/aleab/toastify/releases"
!define ABOUTURL "https://aleab.github.io/toastify/"

!define RegUninstallKey "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

!define OldMutexName "{B8F3CA50-CE27-4ffa-A812-BBE1435C9485}"
!define AppMutexName "Toastify-{3E929742-424C-46BE-9ED0-9FB410E4FFC0}"


SetCompressor /SOLID /FINAL lzma  # zlib|bzip2|lzma

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

# Installer's Version Information
VIProductVersion ${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}.0
VIFileVersion ${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}.0
VIAddVersionKey "ProductVersion" "${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}"
VIAddVersionKey "FileVersion" "${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}"
VIAddVersionKey "ProductName" "${APPNAME}"
VIAddVersionKey "FileDescription" "${APPNAME} Installer"
VIAddVersionKey "LegalCopyright" "(C) 2018 Alessandro Attard Barbini"


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

Var /GLOBAL EstimatedSize

Var /GLOBAL PrevDesktopShortcutArgs
Var /GLOBAL PrevSMShortcutArgs
Var /GLOBAL PrevAutostartArgs

Section "${APPNAME} (required)"
  SectionIn RO

  # Check if Toastify is running
  ${TerminateToastify} "${OldMutexName}"
  ${TerminateToastify} "${AppMutexName}"

  # Uninstall previous versions
  DetailPrint "Uninstalling previous versions of ${APPNAME}..."
  Call UninstallPreviousVersions

  # Check .NET Framework
  !insertmacro CheckNetFramework 472

  # Set output path to the installation directory.
  SetOutPath $INSTDIR

  # Bundle the files
    # Dependencies
    File /R "lib"

    # ToastifyAPI.dll
    ${If} ${IsWin10}
      File /oname=ToastifyAPI.dll "ToastifyAPI_UWP.dll"
      File /oname=ToastifyAPI.pdb "ToastifyAPI_UWP.pdb"
    ${Else}
      File /oname=ToastifyAPI.dll "ToastifyAPI_Win32.dll"
      File /oname=ToastifyAPI.pdb "ToastifyAPI_Win32.pdb"
    ${EndIf}

    # Toastify.exe
    File "Toastify.exe"
    File "Toastify.exe.config"
    File "Toastify.pdb"

    # ToastifyWebAuthAPI.dll
    File /nonfatal "ToastifyWebAuthAPI.dll"

    # Resources
    File /oname=LICENSE.txt "LICENSES\LICENSE"
    File /oname=LICENSE-3RD-PARTY.txt "LICENSES\LICENSE-3RD-PARTY"

    # /wwwroot
    File /R "Resources\wwwroot"
    Rename "$INSTDIR\wwwroot" "$INSTDIR\res"

  # Create directories in AppData
  CreateDirectory "$APPDATA\Toastify"
  CreateDirectory "$LOCALAPPDATA\Toastify"

  # Remove files belonging to old versions
  Delete "$INSTDIR\LICENSE"
  Delete "$INSTDIR\LICENSE-3RD-PARTY"
  Delete "$INSTDIR\Garlic.dll"
  Delete "$INSTDIR\uninstall.exe"

  # Get EstimatedSize
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $EstimatedSize "0x%08X" $0

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
  WriteRegDWORD HKLM "${RegUninstallKey}" "EstimatedSize" $EstimatedSize
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

  # Remove files from $INSTDIR
    # Dependencies
    RMDir /r "$INSTDIR\lib"

    # ToastifyAPI.dll
    Delete "$INSTDIR\ToastifyAPI.dll"
    Delete "$INSTDIR\ToastifyAPI.pdb"

    # Toastify.exe
    Delete "$INSTDIR\Toastify.exe"
    Delete "$INSTDIR\Toastify.exe.config"
    Delete "$INSTDIR\Toastify.pdb"

    # ToastifyWebAuthAPI.dll
    Delete "$INSTDIR\ToastifyWebAuthAPI.dll"

    # Resources
    RMDir /r "$INSTDIR\res"
    Delete "$INSTDIR\LICENSE.txt"
    Delete "$INSTDIR\LICENSE-3RD-PARTY.txt"

    # Uninstaller
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

Function un.onUninstSuccess
  RMDir "$APPDATA\Toastify"
  RMDir "$LOCALAPPDATA\Toastify"
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
