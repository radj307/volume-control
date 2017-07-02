@ECHO OFF

CD /D %~dp0
powershell -File "update-version.ps1"