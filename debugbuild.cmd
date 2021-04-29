rmdir /S /Q .\rtnet48
rmdir /S /Q .\rtnet5.0

@IF exist .\rtnet48 GOTO err
@IF exist .\rtnet5.0 GOTO err
mkdir rtnet48
mkdir rtnet5.0

"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe" /m Altaxo.sln /t:Restore;Build /p:Configuration=Debug "/p:Platform=Any CPU" -maxcpucount:1 -filelogger -fileloggerparameters:LogFile=..\DebugBuildLog.txt;Verbosity=normal;Encoding=UTF-8
IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1