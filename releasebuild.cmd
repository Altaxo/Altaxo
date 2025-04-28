rmdir /S /Q .\rtnet48
rmdir /S /Q .\rtnet9.0-windows

@IF exist .\rtnet48 GOTO err
@IF exist .\rtnet9.0-windows GOTO err
mkdir rtnet48
mkdir rtnet9.0-windows

"%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe" /m Altaxo.slnx /t:Restore;Build /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1