@ECHO OFF
SetLocal EnableExtensions EnableDelayedExpansion

SET nsisInstallerFileName=ToastifyInstaller.exe
SET sfxArchiveName=ToastifyInstaller


:: ----------[ POST-BUILD ]--------------------------------
:: [in]ConfigurationName  [in]DevEnvDir  [in]SolutionDir  [in]TargetDir  [in]TargetFileName

SET "ConfigurationName=%~1"
SET "DevEnvDir=%~2"
SET "SolutionDir=%~3"
SET "TargetDir=%~4"
SET "TargetFileName=%~5"

:: Remove Xceed.Wpf.Toolkit's useless resource files
ECHO;
ECHO [POST-BUILD] Remove Xceed.Wpf.Toolkit's useless resource files
FOR %%d IN (de es fr hu it pt-BR ro ru sv zh-Hans) DO (
    RMDIR /S /Q "%TargetDir%%%d"
)

IF NOT ["%ConfigurationName:~0,7%"]==["Windows"] (
    GOTO :EOF
)
IF ["%ConfigurationName:Release=%"]==["%ConfigurationName%"] (
    GOTO :EOF
)

:: It's a Windows Release configuration

:: CALL VsDevCmd
ECHO;
ECHO [POST-BUILD] CALL "%DevEnvDir%..\Tools\VsDevCmd.bat"
CALL "%DevEnvDir%..\Tools\VsDevCmd.bat"

:: Manifest
::ECHO;
::ECHO - mt.exe -nologo -manifest "%TargetDir%%TargetFileName%.manifest" -outputresource:"%TargetDir%%TargetFileName%;#1"
::mt.exe -nologo -manifest "%TargetDir%%TargetFileName%.manifest" -outputresource:"%TargetDir%%TargetFileName%;#1"
::DEL "%TargetDir%%TargetFileName%.manifest"

:: Copy installation scripts 
ECHO; 
ECHO [POST-BUILD] Copy install script 
COPY /Y "%SolutionDir%InstallationScript\Install.nsi" "%TargetDir%Install.nsi" 
COPY /Y "%SolutionDir%InstallationScript\*.nsh" "%TargetDir%"

:: Check if makensis.exe is in the PATH
makensis /VERSION 1>NUL 2>&1
IF ERRORLEVEL 9009 (
    ECHO;
    ECHO ERROR: Couldn't find 'makensis' in the PATH!
    ECHO        Make sure NSIS is installed in your system and its 'Bin' directory is in the PATH environment variable.
    ECHO;
    GOTO :EOF
)

CD "%TargetDir%"

:: Compile NSIS installer
ECHO;
ECHO [POST-BUILD] Compile NSIS installer
makensis Install.nsi || GOTO :EOF
