set ALTAXOSRC=V:\C\Altaxo\src
set CONFIG=rtdebug
xcopy %ALTAXOSRC%\Content\AddIns\*.*   %ALTAXOSRC%\%CONFIG%\AddIns /E /I /Y
xcopy %ALTAXOSRC%\Content\data\*.*     %ALTAXOSRC%\%CONFIG%\data /E /I /Y
xcopy %ALTAXOSRC%\Content\doc\*.*      %ALTAXOSRC%\%CONFIG%\doc /E /I /Y


