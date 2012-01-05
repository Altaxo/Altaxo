rmdir /S /Q .\rtDir
@IF exist .\rtDir GOTO err
mkdir rtDir
%windir%\microsoft.net\Framework\v4.0.30319\msbuild /m Altaxo.sln /p:Configuration=Debug "/p:Platform=Any CPU"
IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1