---
layout: page
title: Features of Altaxo
permalink: /features.html
---

### GUI 
* Altaxo has the GUI of SharpDevelop and as such, it features pads and views, including an internal help view.
* Worksheet Views (as many tables as you want)
* Support for a huge number of columns (I tested it with a hundred thousand)
* Property columns (this are "horizontal" columns) that hold meta-data for the columns (for instance column labels, column units, or numbers)
* Simple dragging for change of column width
* Selecting rows, columns and property columns, even non-contiguous regions
* Copy and paste of selected areas, if the region does not match, the transposed region will be pasted in (if this matches better)
* Renaming of columns / worksheets / graph. Allows to use [arbitrary names for the items](http://altaxo.sourceforge.net/AltaxoClassRef/html/2783FED4411F72EE01B8906649201D84.htm).
* Project explorer shows all graphs and tables
* Graph and Worksheet views can be closed without removing the underlying graph / table (they can be opened again using the project explorer)
* Output text pane to show fitting results and messages. Also quite usefull when debugging worksheet scripts
* File pane for browsing the file system and direcly open or import files.
* Graphs can be embedded into MS Word, PowerPoint etc. (all COM Container applications), or linked.

### Plotting 
* Line and scatter plots (and both combined) with a lot of options, including fill styles and drop styles.
* Label style (labels can either be attached to the data point, or to one of the axes)
* Column charts and bar charts, including stacked charts and percentage charts
* Support for different layers. Layers can be rotated, scaled, and linked to other layers.
* Support for different coordinate systems. Currently implemented is 2D cartesic and 2D polar coordinate systems. 3D cartesic is in experimental state.
* Support for graphics elements (only the text element can be edited and created by menu; image, line, rectangle elements are supported by the library)
* Density images, i.e. visualization of 2-dim. meshed data, currently only in cartesic coordinates with linear axes.

### Input / output 
* ASCII import with automatic structure and number format recognition
* ASCII export (tab separated)
* Picture import (as brightness values)
* Galactic® SPC file import (multiple files selectable)
* Galactic® SPC file export wizard
* Import of Origin .OPJ files (only worksheet data), although not toroughly tested.
* Export of graphs as WMF (Windows Metafile)
* Export of partial least squares (PLS) calibration files (XML format)
* The Altaxo document is now stored as ZIP-file. The worksheets and graphs are stored in XML file format inside of the ZIP-file. Support for versioning.

### Data analysis / data processing
* Complex number library (thanks to the Exocortexdsp project)
* Growing collection of special functions (Bessel related functions, Gamma related function, Error functions, spherical functions, scientific functions (most of them are C# translations of Berndt Gammel's C++ Matpack library)
* Random number generators, probability distributions.
* Interpolation functions: polynomial interpolation, rational interpolation, cubic splines and and and...
* Polynomial fitting
* Fourier transformation code (FFT algorithm in 1 or 2 dimensions, 1 dimensional FFT of arbitrary length)
* Chemometric methods, like partial least squares analysis (PLS), and principal component regression (PCR)
* Matrix operations (some are from Lutz Roeder's mapack library)
* Statistics (column statistics, row statistics)
* Non-linear fitting. Multiple fit functions can be simultaneously fitted. Parameters can be shared between multiple fitting functions.

### Scripting / programming
* Column scripts, worksheet scripts, function scripts, fit function scripts and more.
* Script language is C# - you can use the whole bunch of classes of the .NET framework and all the [http://altaxo.sourceforge.net/AltaxoClassRef/Index.html classes of Altaxo] itself!
* Script execution is very fast since the scripts are compiled.
* All scripts with syntax highlighting and code completion feature. You will see the documentation summary of the function/class.
* Help pane with Altaxo class reference
* Addin support (addins are DLLs that are linked to the program, see the SharpDevelop project)

### Localization
Altaxo is designed with an english user interface. The code (the class names and the comments) you can refer to in your scripts is also in english.
So far no plans exist to localize Altaxo to other languages. Due to the nice localization support of the SharpDevelop project used for the Gui, it should be possible to localize Altaxo for other languages.
