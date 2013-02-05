del "bin\Altaxo.msi"
rem "..\Tools\UpdateAssemblyInfo\bin\Debug\UpdateAssemblyInfo.exe"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Altaxo.Setup.sln "/p:AltaxoBinPath=%CD%\..\rtDir\bin"
@IF %ERRORLEVEL% NEQ 0 PAUSE