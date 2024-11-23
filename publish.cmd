if "%SolutionDir%"=="" (set SolutionDir=%cd%\)
if "%PublishDir%"=="" (set PublishDir=%SolutionDir%rtPublish\)
if "%Architecture%"=="" (set Architecture=x64)
if "%Configuration%"=="" (set Configuration=debug)

@echo SolutionDir   = %SolutionDir%
@echo PublishDir    = %PublishDir%
@echo Architecture  = %Architecture%
@echo Configuration = %Configuration%


rem ******** Main program ********
if "%Architecture%"=="x86" (
dotnet publish %SolutionDir%\Altaxo\Startup64\AltaxoStartup32.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishDir=%PublishDir%\bin\ -p:SolutionDir=%SolutionDir%
copy %PublishDir%\bin\AltaxoStartup32.exe %PublishDir%\bin\AltaxoStartup.exe 
copy %PublishDir%\bin\AltaxoStartup32.pdb %PublishDir%\bin\AltaxoStartup.pdb
) else (
dotnet publish %SolutionDir%\Altaxo\Startup64\AltaxoStartup64.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishDir=%PublishDir%\bin\ -p:SolutionDir=%SolutionDir%
copy %PublishDir%\bin\AltaxoStartup64.exe %PublishDir%\bin\AltaxoStartup.exe 
copy %PublishDir%\bin\AltaxoStartup64.pdb %PublishDir%\bin\AltaxoStartup.pdb
)

rem ******** Altaxo program addin ********
dotnet publish %SolutionDir%\Altaxo\Dom.Presentation\AltaxoDom.Presentation.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishDir=%PublishDir%\bin -p:SolutionDir=%SolutionDir%
xcopy %SolutionDir%\Altaxo\Dom.Presentation\AltaxoCore.addin %PublishDir%\AddIns\ /I /Y
xcopy %SolutionDir%\Content\data\*.* %PublishDir%\data /E /I /Y
xcopy %SolutionDir%\Content\doc\*.* %PublishDir%\doc /E /I /Y

rem ******** Addin: OriginConnector ********
dotnet publish %SolutionDir%\AddIns\OriginConnector\OriginConnector.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishDir=%PublishDir%\AddIns\OriginConnector -p:SolutionDir=%SolutionDir%
xcopy %SolutionDir%\AddIns\OriginConnector\OriginConnector.addin %PublishDir%\AddIns\OriginConnector /I /Y

rem ******** Addin: D3DAddin ********
dotnet publish %SolutionDir%\AddIns\D3D\D3DPresentation\D3DPresentation.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishDir=%PublishDir%\AddIns\D3D -p:SolutionDir=%SolutionDir%
xcopy %SolutionDir%\AddIns\D3D\D3DPresentation\D3D.addin %PublishDir%\AddIns\D3D /I /Y

rem ******** Addin: OpenXml ********
dotnet publish %SolutionDir%\AddIns\OpenXML\OpenXML.Presentation\OpenXML.Presentation.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishDir=%PublishDir%\AddIns\OpenXMLAddin -p:SolutionDir=%SolutionDir%
xcopy %SolutionDir%\AddIns\OpenXML\OpenXML\OpenXML.addin %PublishDir%\AddIns\OpenXMLAddin /I /Y

rem ******** Addin: ML.Net (is available only for x64 architecture) ********
if "%Architecture%"=="x64" (
dotnet publish %SolutionDir%\AddIns\MLNET\MLNET\MLNET.csproj --arch x64 --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishDir=%PublishDir%\AddIns\ML.Net -p:SolutionDir=%SolutionDir%
xcopy %SolutionDir%\AddIns\MLNET\MLNET\MLNET.addin %PublishDir%\AddIns\ML.Net /I /Y
)

rem ******** Altaxo update downloader (must be published as single file) ********
rem ******** As long as there is no Linux version, we publish a Net48 exe ********
dotnet publish %SolutionDir%\Libraries\UpdateDownloader\AutoUpdateDownloader.csproj --arch %Architecture% --configuration %Configuration% --framework net48 -p:PublishDir=%PublishDir%\bin\ -p:SolutionDir=%SolutionDir%
rem dotnet publish %SolutionDir%\Libraries\UpdateDownloader\AutoUpdateDownloader.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true -p:PublishDir=%PublishDir%\bin\ -p:SolutionDir=%SolutionDir%

rem ******** Altaxo update installer (must be published as single file) ********
rem ******** As long as there is no Linux version, we publish a Net48 exe ********
dotnet publish %SolutionDir%\Libraries\UpdateInstaller\AutoUpdateInstaller.csproj --arch %Architecture% --configuration %Configuration% --framework net48  -p:PublishDir=%PublishDir%\bin\ -p:SolutionDir=%SolutionDir%
rem dotnet publish %SolutionDir%\Libraries\UpdateInstaller\AutoUpdateInstaller.csproj --arch %Architecture% --configuration %Configuration% --framework net9.0-windows --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:PublishDir=%PublishDir%\bin\ -p:SolutionDir=%SolutionDir%

rem ******** Prune the Addin directories ********
%SolutionDir%..\PrunePublishedAddins.exe %PublishDir%\AddIns\OriginConnector %PublishDir%\bin
%SolutionDir%..\PrunePublishedAddins.exe %PublishDir%\AddIns\D3D             %PublishDir%\bin
%SolutionDir%..\PrunePublishedAddins.exe %PublishDir%\AddIns\OpenXMLAddin    %PublishDir%\bin
if "%Architecture%"=="x64" (
%SolutionDir%..\PrunePublishedAddins.exe %PublishDir%\AddIns\ML.Net          %PublishDir%\bin
)

pause