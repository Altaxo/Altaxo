%windir%\microsoft.net\Framework\v4.0.30319\msbuild /m Altaxo.slnx /t:clean "/p:Platform=Any CPU" /p:Configuration=Debug
@IF %ERRORLEVEL% NEQ 0 PAUSE
%windir%\microsoft.net\Framework\v4.0.30319\msbuild /m Altaxo.slnx /t:clean "/p:Platform=Any CPU" /p:Configuration=Release
@IF %ERRORLEVEL% NEQ 0 PAUSE