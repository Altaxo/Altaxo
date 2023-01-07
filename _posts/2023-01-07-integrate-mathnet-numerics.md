---
layout: post
title:  "Integration of Math.Net Numerics"
date:   2023-01-07 23:15:00 +0200
categories: 
---

I'm proud to announce that the great mathematical library [Math.Net Numerics](https://numerics.mathdotnet.com/) is now integrated in
Altaxo. In order to have a smooth experience when converting the scripts from the old linear algebra library to this new one,
I changed the namespace from `MathNet.Numerics` to `Altaxo.Calc`. This means for you, that e.g. the linear algebra classes can be
found (as was before) in the namespace `Altaxo.Calc.LinearAlgebra`. Additionally, the 
[Intel Math Kernel Library (MKL) provider](https://numerics.mathdotnet.com/MKL.html) was added, so that linear algebra functions
and Fourier transformations can use the full power of the CPU.

Because of the integration, some of the classes in the core library now are redundant. In the next months I will clean
up the code and remove the redundant classes/functions. 
