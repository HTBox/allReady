@if "%SCM_TRACE_LEVEL%" NEQ "4" @echo off

:: ----------------------
:: KUDU Deployment Script
:: Version: 1.0.7
:: ----------------------

:: Prerequisites
:: -------------

:: Verify node.js installed
where node 2>nul >nul
IF %ERRORLEVEL% NEQ 0 (
  echo Missing node.js executable, please install node.js, if already installed make sure it can be reached from current environment.
  goto error
)

:: Setup
:: -----

setlocal enabledelayedexpansion

SET ARTIFACTS=%~dp0%..\artifacts

IF NOT DEFINED DEPLOYMENT_SOURCE (
  SET DEPLOYMENT_SOURCE=%~dp0%.
)

IF NOT DEFINED DEPLOYMENT_TARGET (
  SET DEPLOYMENT_TARGET=%ARTIFACTS%\wwwroot
)

IF NOT DEFINED NEXT_MANIFEST_PATH (
  SET NEXT_MANIFEST_PATH=%ARTIFACTS%\manifest

  IF NOT DEFINED PREVIOUS_MANIFEST_PATH (
    SET PREVIOUS_MANIFEST_PATH=%ARTIFACTS%\manifest
  )
)

IF NOT DEFINED KUDU_SYNC_CMD (
  :: Install kudu sync
  echo Installing Kudu Sync
  call npm install kudusync -g --silent
  IF !ERRORLEVEL! NEQ 0 goto error

  :: Locally just running "kuduSync" would also work
  SET KUDU_SYNC_CMD=%appdata%\npm\kuduSync.cmd
)
IF NOT DEFINED DEPLOYMENT_TEMP (
  SET DEPLOYMENT_TEMP=%temp%\___deployTemp%random%
  SET CLEAN_LOCAL_DEPLOYMENT_TEMP=true
)

IF DEFINED CLEAN_LOCAL_DEPLOYMENT_TEMP (
  IF EXIST "%DEPLOYMENT_TEMP%" rd /s /q "%DEPLOYMENT_TEMP%"
  mkdir "%DEPLOYMENT_TEMP%"
)

IF DEFINED MSBUILD_PATH goto MsbuildPathDefined
SET MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe
:MsbuildPathDefined
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: Deployment
:: ----------

echo Handling ASP.NET Core Web Application deployment.

:: 1. Restore nuget packages
echo Restoring nuget packages for all project.json projects
call :ExecuteCmd nuget.exe restore -packagesavemode nuspec
IF !ERRORLEVEL! NEQ 0 goto error

:: 2a. Build
echo Storing Git Version Info for Runtime Display
call :ExecuteCmd PowerShell -NoProfile -NoLogo -ExecutionPolicy unrestricted -Command "(Get-Content AllReadyApp\Web-App\AllReady\version.json).replace('GITVERSION', (git rev-parse --short HEAD)) | Set-Content AllReadyApp\Web-App\AllReady\version.json"

echo Building AllReady.Core Project (project.json)
call :ExecuteCmd dotnet build "%DEPLOYMENT_SOURCE%\AllReadyApp\AllReady.Core\project.json" --configuration Debug
IF !ERRORLEVEL! NEQ 0 goto error

echo Not Building AllReady Project (it gets built with publish command)

echo Building Web Jobs Project (project.json)
call :ExecuteCmd dotnet build "%DEPLOYMENT_SOURCE%\AllReadyApp\AllReady.NotificationsWebJob\project.json" --configuration Debug
IF !ERRORLEVEL! NEQ 0 goto error

:: 2b. Publish AllReady
echo Publishing Allready Project (project.json) which includes building it
call :ExecuteCmd dotnet publish "%DEPLOYMENT_SOURCE%\AllReadyApp\Web-App\AllReady" --output "%DEPLOYMENT_TEMP%" --configuration Debug
IF !ERRORLEVEL! NEQ 0 goto error

:: 2c. Publish WebJobs (AllReady.NotificationsWebJob)
echo Publishing AllReady.NotificationsWebJob WebJob
call :ExecuteCmd mkdir "%DEPLOYMENT_TEMP%\app_data\jobs\continuous\notificationsprocessor\"
call :ExecuteCmd xcopy /S "%DEPLOYMENT_SOURCE%\AllReadyApp\AllReady.NotificationsWebJob\bin\debug\net451" "%DEPLOYMENT_TEMP%\app_data\jobs\continuous\notificationsprocessor\"


:: 3. KuduSync
call :ExecuteCmd "%KUDU_SYNC_CMD%" -v 50 -f "%DEPLOYMENT_TEMP%" -t "%DEPLOYMENT_TARGET%" -n "%NEXT_MANIFEST_PATH%" -p "%PREVIOUS_MANIFEST_PATH%" -i ".git;.hg;.deployment;deploy.cmd"
IF !ERRORLEVEL! NEQ 0 goto error

::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
goto end

:: Execute command routine that will echo out when error
:ExecuteCmd
setlocal
set _CMD_=%*
call %_CMD_%
if "%ERRORLEVEL%" NEQ "0" echo Failed exitCode=%ERRORLEVEL%, command=%_CMD_%
exit /b %ERRORLEVEL%

:error
endlocal
echo An error has occurred during web site deployment.
call :exitSetErrorLevel
call :exitFromFunction 2>nul

:exitSetErrorLevel
exit /b 1

:exitFromFunction
()

:end
endlocal
echo Finished successfully.
