@ECHO OFF
SetLocal EnableExtensions EnableDelayedExpansion


REM ----------[ PRE-BUILD ]--------------------------------
REM [in]ConfigurationName  [in]SolutionDir  [in]DevEnvDir

SET "ConfigurationName=%~1"
SET "SolutionDir=%~2"
SET "DevEnvDir=%~3"


REM ===============
REM  CALL VsDevCmd
REM ===============
ECHO;
ECHO [PRE-BUILD] Start VS development console
CALL "%DevEnvDir%..\Tools\VsDevCmd.bat"

CD /D "%SolutionDir%"

SET Configuration=Release
IF ["%ConfigurationName:Release=%"]==["%ConfigurationName%"] (
    SET Configuration=Debug
)

REM ==============
REM  Dependencies
REM ==============
ECHO;
ECHO [PRE-BUILD] Fetching dependencies...
IF NOT EXIST _dependencies MKDIR _dependencies
CD _dependencies

REM ==============================================
REM  [OPTIONAL] ToastifyWebAuthAPI (private repo)
REM ==============================================
ECHO [PRE-BUILD]   - ToastifyWebAuthAPI
IF NOT EXIST toastify.webauthapi (
    git clone git@bitbucket.org-aleab:aleab/toastify.webauthapi.git
)
IF EXIST toastify.webauthapi (
    CD toastify.webauthapi

    REM Reset remotes
    git remote remove origin 2>nul
    git remote remove origin-aleab 2>nul
    git remote add origin git@bitbucket.org:aleab/toastify.webauthapi.git
    git remote add origin-aleab git@bitbucket.org-aleab:aleab/toastify.webauthapi.git

    REM Pull remote only if this is a Release build
    IF ["%Configuration%"]==["Release"] (
        git fetch origin-aleab
        git reset --hard origin-aleab/master
    )
    CD ..
)

CD "%SolutionDir%"


:EOF
ECHO;
