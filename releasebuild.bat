rmdir /S /Q .\rtDir
@IF exist .\rtDir GOTO err
mkdir rtDir
"%ProgramFiles(x86)%\msbuild\14.0\bin\msbuild.exe" /m Altaxo.sln /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1