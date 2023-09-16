; NOTE: Prior to compiling this file, run "git clone https://github.com/DomGries/InnoDependencyInstaller"

#define public Dependency_Path_NetCoreCheck "InnoDependencyInstaller\dependencies\"
#include "InnoDependencyInstaller\CodeDependencies.iss"

#define AppName "Volume Control"
#define AppPublisher "radj307"
#define AppURL "https://github.com/radj307/volume-control"
#define AppExeName "VolumeControl.exe"
; #define AppVersion "" ;< DEFINE THIS VIA COMMANDLINE (/dAppVersion="VERSION_NUMBER")

[Setup]
AppId={{33DFCEE8-022C-4C66-A366-79A7415320F2}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} v{#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
ArchitecturesInstallIn64BitMode=x64
DefaultDirName={autopf}\VolumeControl
DisableProgramGroupPage=yes
LicenseFile=LICENSE
PrivilegesRequiredOverridesAllowed=dialog
; OutputDir=publish
OutputBaseFilename=VolumeControl-Installer
SetupIconFile=VolumeControl\Resources\icons\iconSilveredInstall.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "publish\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet60;
  Result := True;
end;
