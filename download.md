---
layout: page
title: Downloading Altaxo
permalink: /download.html
---

### Offical release
The current officially released version of Altaxo can be found
in the stable folder, which is
[here at SourceForge](https://sourceforge.net/projects/altaxo/files/Altaxo/Altaxo-Latest-Stable/).
You will need the .NET-Framework 4.7.1 on your computer prior to 
the installation.
Double-click on the .msi file in order to install Altaxo.
Alternatively, you can download the .zip file with the binaries - 
see the text below how to handle this.

Since the new functions to embed Altaxo Graphs into COM-Containers (e.g. WORD-documents) are still experimental, Altaxo will not register itself in the registry. If you want to use this functionality, start Altaxo, and from the menu choose Tools->Registry->Register Altaxo for OLE/COM. If you are logged on as administrator, this will register Altaxo for all users; if you are logged on as user only, Altaxo is registered only for you (the current user).

### If you always want to use the newest release

Important note: the newest release of Altaxo is using
the .NET-Framework 4.7.1. 
If your computer meets this requirement,
then go to the file release page of the Altaxo project,
and navigate to the folder
[ Altaxo-Latest-Unstable](http://sourceforge.net/projects/altaxo/files/Altaxo/Altaxo-Latest-Unstable/).
Download the file named 'AltaxoBinaries-4.7.x.x.zip'.
**Important: before unzipping the downloaded file**,
make sure that you right-click on the zip-file in Explorer, 
choose Properties, and then click Unblock.
If you miss this step, all executable files will 
be blocked after unzipping! 
Now you can unzip the content of the .zip file into 
an empty folder of your choice. 
Start Altaxo by double-clicking on AltaxoStartup.exe 
located in the 'bin' folder of the installation. 
That's it! Of course, after those steps, 
Altaxo is not registered for OLE/COM functionality. 
See the paragraph 'Offical release' how to do it.

P.S.: The auto update options (see `Tools -> Options` menu in Altaxo) 
are set to download only stable versions. 
If you really want to keep up-to-date, 
you should alter these options to allow the download of
both stable and unstable versions.
