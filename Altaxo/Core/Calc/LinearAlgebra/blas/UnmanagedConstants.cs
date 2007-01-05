#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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
