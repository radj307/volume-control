@ECHO OFF
SetLocal EnableExtensions EnableDelayedExpansion


REM ----------[ PRE-BUILD ]--------------------------------
REM [in]SolutionDir  [in]DevEnvDir

SET "SolutionDir=%~1"
SET "DevEnvDir=%~2"


REM ===============
REM  CALL VsDevCmd
REM ===============
ECHO;
ECHO [PRE-BUILD] Start VS development console
CALL "%DevEnvDir%..\Tools\VsDevCmd.bat"

CD /D "%SolutionDir%"


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

    git fetch origin-aleab
    git reset --hard origin-aleab/master
    CD ..
)

CD "%SolutionDir%"


:EOF
ECHO;
