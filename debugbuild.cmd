rmdir /S /Q .\rtnet

@IF exist .\rtnet GOTO err
mkdir rtnet

"%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe" /m Altaxo.slnx /t:Restore;Build /p:Configuration=Debug "/p:Platform=Any CPU" -fl -flp:logfile=..\AltaxoBuildLog.txt;verbosity=detailed
IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1