@ECHO OFF
SetLocal EnableExtensions EnableDelayedExpansion


:: ----------[ PRE-BUILD ]--------------------------------
:: [in]ProjectFileName  [in]ProjectDir  [in]DevEnvDir

SET "ProjectFileName=%~1"
SET "ProjectDir=%~2"
SET "DevEnvDir=%~3"

:: CALL VsDevCmd
ECHO;
ECHO [PRE-BUILD] Start VS development console
CALL "%DevEnvDir%..\Tools\VsDevCmd.bat"

:EOF
ECHO;
