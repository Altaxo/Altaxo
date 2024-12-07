---
layout: post
title:  "Plans for transition to .NET 9.0"
date:   2024-12-06 23:15:00 +0200
categories: 
---

It is time to move forward! For a long time, the .NET framework was a 
great foundation to base Altaxo on. In fact, the Altaxo project started as
early as 2002 with the .NET framework version 1.0! In the meantime, Altaxo
uses version 4.8. 

But I feel that now it's time to move on. The development of the .NET framework has stopped,
and the progress has shifted to Net Core.

The reason why I stuck for so long to .NET framework is that I wanted a simple copy-paste installation
of Altaxo without requiring admin rights. This was possible because the .NET framework was already
installed on all computers running Windows by default, and it was regularily updated.

In contrast, the .NET Core series (currently .NET 9.0) is not installed on Windows by default,
and worse, the installation requires admin rights. I see this as a big disadvantage. Fortunately,
at least once it is installed, it receives monthly security updates. But every year, the you have
to install the next version of .NET again.

So why change at all? The reason is that more and more libraries have ceased support for .NET framework.
In Altaxo this is for instance the Vortex library, responsible for 3D-rendering, and the PureHDF library,
responsible for reading NeXus formatted files. Furthermore, as mentioned above, the further development
is done on the .NET series.

**What are the plans then? Can we keep to the copy-paste installation without admin rights?**

In January 2025, I will completely switch to .NET 9.0. The last version of Altaxo using the .NET framework
will remain for some time available, to make sure that all installations can update properly.
Then, we will have two distibutions of Altaxo: (i) a distribution that requires that .NET 9.0 is
installed on the computer, and (ii) a standalone version, that comes with local files of
.NET 9.0. The advantages and disadvantages are:

**Distribution that requires .NET 9.0 installed**

Advantages:
- both 32 bit and 64 bit operating system supported
- .NET 9.0 receives regular security updates
- easy integration of Add-Ons

Disadvantages:
- Copy-Paste installation only possible if .NET 9.0 is already installed
- installation of .NET 9.0 requires admin rights
- switching from .NET 9.0 to .NET 10.0 again requires installation with admin rights

**Standalone distribution**

Advantages:
- Copy-Paste installation without admin rights is possible
- Smooth transition from one version of .NET to a higher version

Disadvantages:
- only 64 bit operating system supported (I feel that it is no longer neccessary to have a 32 bit distro)
- the underlying .NET 9.0 will receive security updates only when Altaxo is updated
- Add-Ons are harder to integrate because they also need local files of .NET 9.0

In the last weeks I have made a lot of effort in the automatic update system. It can now detect whether
.NET 9.0 is installed on your computer or not. If it is installed, the version that requires .NET 9.0 is preferred.
If .NET 9.0 is not detected, then the update is made with the standalone version of Altaxo.
Thus, I hope it will be a smooth transition from .NET framework 4.8 to .NET 9.0!




