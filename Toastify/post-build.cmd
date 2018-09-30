@ECHO OFF
SetLocal EnableExtensions EnableDelayedExpansion

SET nsisInstallerFileName=ToastifyInstaller.exe


REM ----------[ POST-BUILD ]--------------------------------
REM [in]ConfigurationName  [in]DevEnvDir  [in]SolutionDir  [in]TargetDir  [in]TargetFileName

SET "ConfigurationName=%~1"
SET "DevEnvDir=%~2"
SET "SolutionDir=%~3"
SET "TargetDir=%~4"
SET "TargetFileName=%~5"


REM ===============
REM  CALL VsDevCmd
REM ===============
ECHO;
ECHO [POST-BUILD] Start VS development console
CALL "%DevEnvDir%..\Tools\VsDevCmd.bat"

CD /D "%SolutionDir%"


REM ==============
REM  Dependencies
REM ==============
ECHO;
ECHO [POST-BUILD] Processing dependencies...
IF EXIST _dependencies (
    CD _dependencies

    REM –––––––––––––––––––––––––––––––
    REM  [OPTIONAL] ToastifyWebAuthAPI
    REM –––––––––––––––––––––––––––––––
    IF EXIST toastify.webauthapi (
        CD toastify.webauthapi
        SET ToastifyWebAuthAPI_Path=!CD!

        ECHO [POST-BUILD]   - Building ToastifyWebAuthAPI...

        REM Build ToastifyWebAuthAPI's PreBuildTool
        ECHO [POST-BUILD]     - PreBuildTool
        msbuild PreBuildTool\PreBuildTool.vcxproj /t:Clean,Build /p:Configuration=Release /p:Platform=x64 >"PreBuildTool\bin\x64\Release\build.log""
        IF NOT EXIST "PreBuildTool\bin\x64\Release\PreBuildTool.exe" (
            ECHO;
            ECHO ERROR: Build failed^^! See "!ToastifyWebAuthAPI_Path!\PreBuildTool\bin\x64\Release\build.log"
            ECHO;
            GOTO EOF
        ) ELSE (
            DEL /Q /F "PreBuildTool\bin\x64\Release\build.log"
        )

        REM Use PreBuildTool
        PreBuildTool\bin\x64\Release\PreBuildTool.exe "!ToastifyWebAuthAPI_Path!\ToastifyWebAuthAPI" "%TargetDir%Toastify.exe"
        IF ERRORLEVEL 1 (
            ECHO;
            ECHO ERROR in PreBuildTool.exe!
            ECHO;
            GOTO EOF
        )

        REM Build ToastifyWebAuthAPI
        ECHO [POST-BUILD]     - ToastifyWebAuthAPI
        msbuild ToastifyWebAuthAPI\ToastifyWebAuthAPI.vcxproj /t:Clean,Build /p:Configuration=Release /p:Platform=x64 >"ToastifyWebAuthAPI\bin\x64\Release\build.log""
        IF NOT EXIST "ToastifyWebAuthAPI\bin\x64\Release\ToastifyWebAuthAPI.dll" (
            ECHO;
            ECHO ERROR: Build failed^^! See "!ToastifyWebAuthAPI_Path!\ToastifyWebAuthAPI\bin\x64\Release\build.log"
            ECHO;
            GOTO EOF
        ) ELSE (
            DEL /Q /F "ToastifyWebAuthAPI\bin\x64\Release\build.log"
        )

        REM Copy ToastifyWebAuthAPI.dll to TargetDir
        COPY /Y "ToastifyWebAuthAPI\bin\x64\Release\ToastifyWebAuthAPI.dll" "%TargetDir%ToastifyWebAuthAPI.dll"
    )
    CD "%SolutionDir%"
)


REM ================================================================
REM  Check if it's a Windows Release build; continue only if it is!
REM ================================================================
IF NOT ["%ConfigurationName:~0,7%"]==["Windows"] GOTO EOF
IF ["%ConfigurationName:Release=%"]==["%ConfigurationName%"] GOTO EOF


REM ========================================
REM  Copy installation scripts to TargetDir
REM ========================================
ECHO; 
ECHO [POST-BUILD] Copy install script 
COPY /Y "%SolutionDir%InstallationScript\Install.nsi" "%TargetDir%Install.nsi" 
COPY /Y "%SolutionDir%InstallationScript\*.nsh" "%TargetDir%"


REM =====================================
REM  Check if makensis.exe is available
REM =====================================
makensis /VERSION 1>NUL 2>&1
IF ERRORLEVEL 9009 (
    ECHO;
    ECHO ERROR: Couldn't find "makensis"!
    ECHO        Make sure that NSIS is installed in your system and that its "Bin" directory is in the PATH environment variable.
    ECHO;
    GOTO EOF
)

CD "%TargetDir%"


REM ========================
REM  Compile NSIS installer
REM ========================
ECHO;
ECHO [POST-BUILD] Compile NSIS installer
makensis Install.nsi || GOTO EOF

:EOF
ECHO;
