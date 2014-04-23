@echo off

if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

SET DIR=%cd%
SET BUILD_DIR=%~d0%~p0%
SET NANT="%BUILD_DIR%ThirdParty\nant-0.92\NAnt.exe"
SET BUILD_NUMBER="0.0.0.0""

%NANT% -logger:NAnt.Core.DefaultLogger /f:%BUILD_DIR%Sriracha.Deploy.build BuildSolution

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:usage
echo.
echo Usage: build.bat
echo.
goto finish

:errors
EXIT /B %ERRORLEVEL%

:finish