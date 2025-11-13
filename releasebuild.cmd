rmdir /S /Q .\rtnet

@IF exist .\rtnet GOTO err
mkdir rtnet

if exist "%ProgramFiles%\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" (
set buildexe="%ProgramFiles%\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" (
set buildexe="%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
) else (
echo Could not found msbuild.exe on your computer!
pause
GOTO err
)

%buildexe% /m Altaxo.slnx /t:Restore;Build /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1