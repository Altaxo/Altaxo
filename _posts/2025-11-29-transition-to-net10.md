---
layout: post
title:  "Transition to .NET 10.0 completed"
date:   2025-11-29 22:10:00 +0100
categories: 
---

As I announced in [my previous post](https://altaxo.github.io/Altaxo/2024/12/06/transition-to-net9.html),
it was time to move forward, and leave the old-fashioned .NET framework 4.8 behind!

Thus, version 4.8.3351.0 was the last version of Altaxo that used the .NET framework.
It is still available for download in order to enable a smooth and automatic transition to the new .NET based versions for all users.

**Was I able to keep the copy-paste installation without admin rights?**

In my previous post, I explained the two different distributions of Altaxo that I would provide.
So now there are to types of distributions of Altaxo: 
1. A distribution that requires that .NET 10.0 is installed on the computer (which would require admin-rights beforehand): You will detected in in the download folder by its name ending 'DotNet10.0', for instance 'AltaxoBinaries-4.8.3410.0-WINDOWS-DotNet10.0.zip'. 
2. A so-called self-contained version, that comes with local files of .NET 10.0, and thus does **not require admin-rights** for installation:
   You will detect it in in the download folder by its name ending 'WINDOWS-X64', for instance 'AltaxoBinaries-4.8.3410.0-WINDOWS-X64.zip'.  
   As you may guess from the name, the self-contained version only supports 64 bit operating systems. But I feel that nowadays, almost all Windows installations are 64 bit anyway. 

The update installer that came with the last .NET framework based version of Altaxo detects automatically whether .NET 10.0 is installed on your computer or not, and whether it is a 32 or 64 bit operating system.
If .NET 10.0 is not installed, the update is done with the self-contained version. Later on, when you have installed .NET 10.0, the update installer will then switch to the version that uses the .NET 10.0 installation.
And yes, if you have a 32 bit operating system without an installation of .NET 10.0, you will no longer receive updates of Altaxo, until you install .NET 10.0.

**Here again are the advantages and disadvantages of both distributions:**   

*Distribution that requires .NET 10.0 installed*

Advantages:
- both 32 bit and 64 bit operating system supported
- .NET 10.0 receives regular security updates by Windows Update
- easy integration of Add-Ons

Disadvantages:
- Copy-Paste installation only possible if .NET 10.0 is already installed
- installation of .NET 10.0 requires admin rights
- switching from .NET 10.0 to a higher .NET version again requires installation with admin rights

*Self-contained distribution*

Advantages:
- Copy-Paste installation without admin rights is possible
- Smooth transition from one version of .NET to a higher version

Disadvantages:
- only 64 bit operating system supported 
- the underlying .NET 10.0 will receive security updates only when Altaxo is updated (although this is very frequently)
- Add-Ons are harder to integrate because they also need local files of .NET 10.0

**What's new?**

Apart from the transition to .NET 10.0, I also took the opportunity to integrate some new features into Altaxo:

- added a lot of fit functions (diffusion functions, transition functions, two segmented polynomials, etc.
- added a two-point and four-point curve tool, and tools derived from this: one four-point curve tool is used for evaluation of steps (e.g. glass transition step in a DSC curve), and there is another four-point tool used for the evaluation of peaks (e.g. melt peak in a DSC curve).
- make the quantities of the fluid state calculations dimension aware
- update code editor to use latest C# version 14
 
**So what's next?**

I will continue to develop Altaxo based on .NET 10.0. I already started to update all libraries, but are not done yet.
Now that Windows 7 and 8 is no longer supported by Microsoft, I can safely switch to DirectX 12 for 3D rendering, which
opens new possibilities like raytracing.
Another recurring topic is reducing the need for scripts, by hardcoding often used functionality into Altaxo itself.


