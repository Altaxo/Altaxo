rmdir /S /Q .\rtDir
@IF exist .\rtDir GOTO err
mkdir rtDir
"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe" /m Altaxo.sln /t:build -restore /p:Configuration=Debug "/p:Platform=Any CPU"
IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1