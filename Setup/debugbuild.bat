del "bin\Debug\Altaxo.msi"

%windir%\microsoft.net\framework\v4.0.30319\msbuild Altaxo.Setup.sln /p:Configuration=Debug "/p:AltaxoBinPath=%CD%\..\rtnet48\bin"
@IF %ERRORLEVEL% NEQ 0 PAUSE