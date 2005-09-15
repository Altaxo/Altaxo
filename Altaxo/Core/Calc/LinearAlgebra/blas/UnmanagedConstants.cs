/*
 * Constants.cs
 * 
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

namespace Altaxo.Calc.LinearAlgebra.Blas
{
  internal enum Order { RowMajor = 101, ColumnMajor = 102 };
  internal enum Transpose { NoTrans = 111, Trans = 112, ConjTrans = 113 };
  internal enum UpLo { Upper = 121, Lower = 122 };
  internal enum Diag { NonUnit = 131, Unit = 132 };
  internal enum Side { Left = 141, Right = 142 };
  internal enum Vector { P = 'P', Q = 'Q' };
  internal enum Factored { F = 'F', N = 'N', E = 'E' };
  internal enum Norm { One = '1', O = 'O', I = 'I' };
  internal enum Job { N = 'N', P = 'P', S = 'S', B = 'B' };
  internal enum CompZ { N = 'N', I = 'I', V = 'V' };
  internal enum CompQ { V = 'V', N = 'N' };
  internal enum EigSrc { Q = 'Q', N = 'N' };
  internal enum InitV { N = 'N', U = 'U' };
  internal enum HowMany { A = 'A', B = 'B', S = 'S' };
  internal enum Sign { Positive = 1, Negative = -1 };
  internal enum ESide { R = 'R', L = 'L', B = 'B' };
}
