rmdir /S /Q .\rtnet48
rmdir /S /Q .\rtnetcoreapp3.1

@IF exist .\rtnet48 GOTO err
@IF exist .\rtnetcoreapp3.1 GOTO err
mkdir rtnet48
mkdir rtnetcoreapp3.1

"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe" /m Altaxo.sln /t:build -restore /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1