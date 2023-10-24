; NOTE: Prior to compiling this file, run "git clone https://github.com/DomGries/InnoDependencyInstaller"

#define public Dependency_Path_NetCoreCheck "InnoDependencyInstaller\dependencies\"
#include "InnoDependencyInstaller\CodeDependencies.iss"

#define AppID               "{33DFCEE8-022C-4C66-A366-79A7415320F2}"
#define AppName             "Volume Control"
#define AppPublisher        "radj307"
#define CurrentYear         GetDateTimeString('yyyy','','')
#define StartYearCopyright  "2011"
#define AppURL              "https://github.com/radj307/volume-control"
#define AppExeName          "VolumeControl.exe"

#define AppMutex            "VolumeControlSingleInstance"
; #define AppVersion "" ;< DEFINE THIS VIA COMMANDLINE (/dAppVersion="VERSION_NUMBER")

[Setup]
AppId={#AppID}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}

AppPublisher=(c){#StartYearCopyright}-{#CurrentYear} {#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}

VersionInfoDescription={#AppName} installer
VersionInfoProductName={#AppName}
VersionInfoVersion={#AppVersion}

UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}

ShowLanguageDialog=yes
UsePreviousLanguage=no
LanguageDetectionMethod=uilanguage

WizardStyle=modern
WizardSizePercent=100

;PrivilegesRequiredOverridesAllowed=dialog

ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\VolumeControl
DisableProgramGroupPage=yes
LicenseFile=LICENSE
; OutputDir=publish
OutputBaseFilename=VolumeControl-Installer
SetupIconFile=VolumeControl\Resources\icons\iconSilveredInstall.ico
Compression=lzma
SolidCompression=yes
AppMutex={#AppMutex}

[Languages]
Name: "english";    MessagesFile: "compiler:Default.isl"
Name: "french";     MessagesFile: "compiler:Languages\French.isl"
Name: "german";     MessagesFile: "compiler:Languages\German.isl"
Name: "italian";    MessagesFile: "compiler:Languages\Italian.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "publish\installer\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "VolumeControl\Resources\icons\iconSilvered.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: dirifempty; Name: "{app}"

[Code]
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;

function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;

function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
{ Return Values: }
{ 1 - uninstall string is empty }
{ 2 - error executing the UnInstallString }
{ 3 - successfully executed the UnInstallString }

  { default return value }
  Result := 0;

  { get the uninstall string of the old app }
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;

function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet60;
  Result := True;
end;
