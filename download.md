---
layout: page
title: Downloading Altaxo
permalink: /download.html
---

### Offical release
The current officially released version of Altaxo can be found
in the stable folder, which is
[here at SourceForge](https://sourceforge.net/projects/altaxo/files/Altaxo/Altaxo-Latest-Stable/).
For the .msi installer files, you need .NET-Framework 4.8 on your computer prior to 
the installation. On most systems, it is already installed. Furthermore, you need admin rights for the installation.
Double-click on the .msi file in order to install Altaxo.
Alternatively, you can download the .zip file with the binaries. Admin rights are not needed here - 
see the text below how to handle this.

Altaxo will not register itself in the registry. This is e.g. neccessary for embedding Altaxo graphs as an editable object in Word, Excel, or Powerpoint.
If you want to have this functionality, start Altaxo, and from the menu choose Tools->Registry->Register Altaxo for OLE/COM.
If you are logged on as administrator, this will register Altaxo for all users;
if you are logged on as user only, Altaxo is registered only for you (the current user).

### If you always want to use the newest release, use one of the binary .ZIP files

You can also use a binary .zip file to make a copy-paste installation of Altaxo.
Starting from December 2024, there are three binary .zip files:

- `AltaxoBinaries-4.8.xxxx.0.zip`: This file contains the Altaxo version that requires the .NET Framework 4.8. The framework should be installed on
 all recent Windows systems by default, so this is the safest option but after January 2025, it is no longer updated.
- `AltaxoBinaries-4.8.xxxx.0-WINDOWS-DotNet9.0.zip`: This file contains the Altaxo version that requires the .NET 9.0 desktop runtime installed on your computer.
  .NET 9.0 desktop runtime can be downloaded [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0). The installation of it requires adminstrative rights.
- `AltaxoBinaries-4.8.xxxx.0-WINDOWS-X64.zip`: This file contains the Altaxo version that is self-contained, i.e. it comes with the neccessary files of .NET 9.0. It requires
  a Windows 64 bit operating system, but nothing more.

You can download either the stable or the unstable version.
Navigate either to the folder
[ Altaxo-Latest-Stable](https://sourceforge.net/projects/altaxo/files/Altaxo/Altaxo-Latest-Stable/)
for the stable version or to the folder
[ Altaxo-Latest-Unstable](https://sourceforge.net/projects/altaxo/files/Altaxo/Altaxo-Latest-Unstable/)
for the unstable version.

Download one of the AltaxoBinaries-....zip files described above.
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
Altaxo is not registered for OLE/COM functionality, and the file extension `.axoprj` is not linked to the application. 
To do this, start Altaxo, and then from the menu choose Tools->Registry->Register Altaxo for OLE/COM.

P.S.: The auto update options (see `Tools -> Options` menu in Altaxo) 
are set to download only stable versions. 
If you really want to keep up-to-date, 
you should alter these options to allow the download of
both stable and unstable versions.
