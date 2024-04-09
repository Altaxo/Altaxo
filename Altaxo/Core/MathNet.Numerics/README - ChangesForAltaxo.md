Math.NET Numerics
=================

Math.NET Numerics is an opensource **numerical library for .NET and Mono**. See Readme.md in this folder for more information.


Math.NET Numerics was adopted for the needs of Altaxo. The following changes were made, compared with the original code:

- The namespace Â´MathNet.NumericsÂ´ was transformed to `Altaxo.Calc`.

- to Vector<T> the interfaces `IReadOnlyList<T>` and `IVector<T>` were added (the latter defined in Altaxo)

- to Matrix<T> the interface `IMatrix<T>` was added (defined in Altaxo)

- in Correlation.cs the Fourier class must be explicitely specified with IntegralTransforms.Fourier

- different improvements in NelderMeadSimplex (for instance, cancellation) 

- NonlinearMinimizationResult: add infrastructure for parameters at their parameter boundaries

- LinearAlgebraControl: change namespace path to provider
- SparseSolverControl: change namespace path to provider

License
================

The files in this folder and in the subfolders are licensed 
under the terms of the licence of Math.NET Numerics (see file LICENSE.md in this folder).

Since many files in AltaxoCore and Altaxo are subject to the GPL license, 
AltaxoCore itself is licensed to you under the terms of the GPL license (version 3).
