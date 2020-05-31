del "bin\Release\Altaxo.msi"

%windir%\microsoft.net\framework\v4.0.30319\msbuild Altaxo.Setup.sln /p:Configuration=Release "/p:AltaxoBinPath=%CD%\..\rtnet48\bin"
@IF %ERRORLEVEL% NEQ 0 PAUSE