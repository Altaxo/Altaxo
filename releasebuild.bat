rmdir /S /Q .\rtDir
mkdir rtDir
%windir%\microsoft.net\Framework\v4.0.30319\msbuild /m Altaxo.sln /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1