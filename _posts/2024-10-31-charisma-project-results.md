---
layout: post
title:  "Processing of optical spectra (Raman, NIR, etc.)"
date:   2024-10-31 23:15:00 +0200
categories: 
---

A great project has come to an end! Many thanks to the European Union’s Horizon 2020 Research and Innovation Program for the funding
of the [CHARISMA project](https://www.h2020charisma.eu/)!

The CHARISMA project is set to harmonize Raman Spectroscopy for characterization across the life cycle of a material, from product design and manufacture to lifetime performance and end-of-life stage.

Harmonizing Raman spectra requires software! In the course of the project we have developed these three
packets:

- a Python library which contains all the basic and advanced operations called [ramanchada2](https://github.com/h2020charisma/ramanchada2).
- a Gui that is based on the Python library called [Oranchada](https://github.com/h2020charisma/oranchada), in which
you can create your own workflows based on widgets
- Integration of the same functionality in [Altaxo](https://github.com/Altaxo/Altaxo/), but not based on widgets, but on dialogs.

These functionality includes (both in [Altaxo](https://altaxo.github.io/AltaxoClassReference/html/C05A56A60B14F185E807BF8B9E97276F.htm) as well as in [ramanchada2](https://github.com/h2020charisma/ramanchada2):

- import of spectra, ranging from standard spectral formats (Galactic SPC, Jcamp-DX, NeXus format) to proprietary formats of 
instrument manufacturers like Bruker Opus .0 files, Nicolet .SPA files, Princeton Instruments .SPE, Renishaw .WDF and WiTec .WIP project files. 
- preprocessing of the spectra, using a preprocessing pipeline. The pipeline could either be predefined or custom.
The possible steps include: sanitzing (removing of zeros, NaNs), dark subtraction, removal of cosmic rays (especially for Raman spectra),
x- and relative y-calibration, resampling, smoothing, cropping, baseline evaluation / baseline subtraction, and normalization. 
For each step, a multitude of algorithms can be chosen.
- peak finding and fitting. First of all, the approximate position of the peaks must be determined by using either a topological
search, or a search using continuous wavelet transformation. Then, by using nonlinear fitting, the peaks are fitted in order to find
their exact parameters like position, height, area, and shape. Different fit functions are at your hand, among them
Lorentzian, Gaussian, Voigt, Pearson IV, Pearson VII, and more. I have spent a lot of effort to enable the simultaneous fitting 
of multiple peaks at once (for a Raman spectrum for instance 50 or more). This was only possible by re-parameterization
of some of the fit functions, and by modifying the non-linear fit routines to enable an efficient fitting with parameter constraints.
- further processing of spectra by chemometric methods, like PCA, PCR, and PLS. I integrated the preprocessing pipeline
mentioned above also into those methods, to enable a seamless evaluation with those methods.

Thus try it out now! If you are Python fan, then probably [ramanchada2](https://github.com/h2020charisma/ramanchada2) is right for you. 
Or if you do not want to program by yourself, then either use [Oranchada](https://github.com/h2020charisma/oranchada) or [Altaxo](https://github.com/Altaxo/Altaxo/)!

Disclaimer:

<img src="/Altaxo/assets/img/EU+flag.png" alt="EU flag" style="width:64px; height:43px;">
The CHARISMA project has received  funding from the European Union’s Horizon 2020 Research and Innovation Program under Grant Agreement No. 952921.
