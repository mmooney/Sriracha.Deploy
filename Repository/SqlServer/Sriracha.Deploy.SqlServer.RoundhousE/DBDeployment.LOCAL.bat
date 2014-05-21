@echo off

SET DIR=%~d0%~p0%

SET database.name="Sriracha"
SET sql.files.directory="%DIR%db"
SET server.database="(local)"
REM SET repository.path="${repository.path}"
REM SET version.file="${file.version}"
REM SET version.xpath="//buildInfo/version"
REM SET environment="${environment}"

REM "%DIR%Console\rh.exe" /d=%database.name% /f=%sql.files.directory% /s=%server.database% /vf=%version.file% /vx=%version.xpath% /r=%repository.path% /env=%environment% /simple
"..\..\..\packages\roundhouse.0.8.6\bin\rh.exe" /d=%database.name% /f=%sql.files.directory% /s=%server.database% /simple

pause