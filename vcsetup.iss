; NOTES:
;  - Prior to compiling this file, run "git clone https://github.com/DomGries/InnoDependencyInstaller"
;  - AppVersion must be defined on the commandline with /dAppVersion=""

#define public Dependency_Path_NetCoreCheck "InnoDependencyInstaller\dependencies\"
#include                                    "InnoDependencyInstaller\CodeDependencies.iss"        

#define SourceExeFilePath                   "publish\installer"

#define AppID                               "{33DFCEE8-022C-4C66-A366-79A7415320F2}"
#define AppName                             "Volume Control"
#define AppPublisher                        "radj307"
#define CurrentYear                         GetDateTimeString('yyyy','','')
#define AppURL                              "https://github.com/radj307/volume-control"
#define AppExeName                          "VolumeControl.exe"
#define AppMutex                            "VolumeControlSingleInstance"
#define AppFileVersion                      GetStringFileInfo(SourceExeFilePath + "\" + AppExeName, "FileVersion")
#define AppVersion                          GetStringFileInfo(SourceExeFilePath + "\" + AppExeName, "ProductVersion")

[Setup]
AppId={{#AppID}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}

AppCopyright=Copyright © 2022-{#CurrentYear} {#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}

VersionInfoDescription={#AppName} installer
VersionInfoProductName={#AppName}
VersionInfoVersion={#AppFileVersion}

UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}
AppPublisher={#AppPublisher}

ShowLanguageDialog=yes
UsePreviousLanguage=no
LanguageDetectionMethod=uilanguage

WizardStyle=modern
WizardSizePercent=100

PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

LicenseFile=LICENSE

ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\VolumeControl
DisableProgramGroupPage=yes
; OutputDir=publish
;DON'T CHANGE THIS ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼ without also changing the name in UpdateChecker.cs
OutputBaseFilename=VolumeControl-Installer_{#AppVersion}
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
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceExeFilePath}\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: dirifempty; Name: "{app}"

[Code]
function GetUninstallKey(): String;
begin
  Result := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
end;

function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := GetUninstallKey();
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
  end else if (CurStep=ssPostInstall) then begin
    RegWriteStringValue(HKEY_AUTO, GetUninstallKey(), 'DisplayVersion', '{#emit SetupSetting('AppVersion')}');
  end;
end;

function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet60;
  Result := True;
end;

procedure InitializeWizard();
var
  i: Integer;
begin
  { Set the license memo text alignment to center & strip preceding whitespace }   
  WizardForm.LicenseMemo.Alignment := taCenter;
  for i:= WizardForm.LicenseMemo.Lines.Count downto 0 do
    WizardForm.LicenseMemo.Lines[i] := Trim(WizardForm.LicenseMemo.Lines[i]);
end;
