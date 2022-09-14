Math.NET Numerics
=================

Math.NET Numerics is an opensource **numerical library for .NET and Mono**. See Readme.md in this folder for more information.


Math.NET Numerics was adopted for the needs of Altaxo. The following changes were made, compared with the original code:

- The namespace ´MathNet.Numerics´ was transformed to `Altaxo.Calc`.

- to Vector<T> the interfaces `IReadOnlyList<T>` and `IVector<T>` were added (the latter defined in Altaxo)

- to Matrix<T> the interface `IMatrix<T>` was added (defined in Altaxo)



License
================

The files in this folder and in the subfolders are licensed 
under the terms of the licence of Math.NET Numerics (see file LICENSE.md in this folder).

Since many files in AltaxoCore and Altaxo are subject to the GPL license, 
AltaxoCore itself is licensed to you under the terms of the GPL license (version 3).
