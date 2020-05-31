# Altaxo - Plotting and data analysis made easy

Altaxo is a plotting and data analysis program with a nice graphical user interface. Data analysis is convenient and fast. If the many built-in possibilities are still not fit your requirements: Almost everything in Altaxo can be scripted, using compiled C# code. Modern features like syntax highlighting and code completion renders scripting really easy. And you have access to a huge mathematical and signal processing library, which is constantly improved. If you want to permanently extend the features of Altaxo, you can even do this by writing an extension to Altaxo, which integrates seamlessly with the main program.

## Homepage

The [homepage](http://altaxo.github.io/Altaxo/) of the Altaxo project is hosted on [Github pages](http://altaxo.github.io/Altaxo/).

## Downloads

The downloads are hosted on SourceForge. 
There is a [folder for the latest stable version](https://sourceforge.net/projects/altaxo/files/Altaxo/Altaxo-Latest-Stable/) 
and a [folder for the latest unstable version](https://sourceforge.net/projects/altaxo/files/Altaxo/Altaxo-Latest-Unstable/).
Grab the file with the highest version number, either the `.msi` file or the `..Binaries..zip` file. 
The different versions in this folder require different versions of the .NET framework.
If in doubt, have a look into the `VersionInfo.txt` file in the download folder.
See [here for download details](https://altaxo.github.io/Altaxo/download.html) 
and [for installation instructions](https://altaxo.sourceforge.io/AltaxoClassRef/html/1F4C428AAA53AFE4CCEE7744AB1CB94F.htm).


## Building from source

Clone the sources: `git clone https://github.com/Altaxo/Altaxo.git`

Open the solution file in the Altaxo source folder with the **latest** version of Visual Studio 2019.
Visual Studio 2019 Community edition is sufficient.
The installation of Visual Studio 2019 should include the C# desktop development workload, the .NET core workload
and furthermore should include the T4 text templates feature. 


## Contribute!

Help by other developers is greatly appreciated! 

**Attention developers**: has anyone made a **successfull connection** to a **COM local server running as 64 bit WPF** process? You? Then please mail me how to do it! I could only get a connection if WPF is running as 32 bit process.

Help is especially welcome in the following areas: 
- Mathematics: non-linear curve fitting with parameter boundary conditions
- Mathematics: add a basic set of commonly used fitting functions
- Mathematics: tidy up Altaxo's linear algebra library
- Mathematics: implement statistical functions, improve linear and non-linear curve fitting, and add special mathematical functions 
- Help system: since the scripts can use all public classes and objects from the program, a good documentation of these classes is absolutely necessary 



