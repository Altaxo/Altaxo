#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright


using System;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using Complex64T = System.Numerics.Complex;
using Complex32T = Altaxo.Calc.Complex32;

namespace AltaxoTest.Calc.LinearAlgebra
{
  internal sealed class Comparer
  {
    private Comparer()
    {
    }

    public static bool AreEqual(Complex32T f1, Complex32T f2)
    {
      return f1 == f2;
    }

    public static bool AreEqual(Complex64T f1, Complex64T f2)
    {
      return f1 == f2;
    }

    public static bool AreEqual(Complex32T f1, Complex32T f2, float delta)
    {
      if (System.Math.Abs(f1.Imaginary - f2.Imaginary) > delta)
        return false;
      if (System.Math.Abs(f1.Real - f2.Real) > delta)
        return false;
      return true;
    }

    public static bool AreEqual(Complex64T f1, Complex64T f2, double delta)
    {
      if (System.Math.Abs(f1.Imaginary - f2.Imaginary) > delta)
        return false;
      if (System.Math.Abs(f1.Real - f2.Real) > delta)
        return false;
      return true;
    }

    public static bool AreEqual(Matrix<Complex32T> f1, Matrix<Complex32T> f2)
    {
      return f1 == f2;
    }

    public static bool AreEqual(Matrix<Complex64T> f1, Matrix<Complex64T> f2)
    {
      return f1 == f2;
    }

    public static bool AreEqual(Matrix<Complex32T> f1, Matrix<Complex32T> f2, float delta)
    {
      if (f1.RowCount != f2.RowCount)
        return false;
      if (f1.ColumnCount != f2.ColumnCount)
        return false;
      for (int i = 0; i < f1.RowCount; i++)
      {
        for (int j = 0; j < f1.ColumnCount; j++)
        {
          if (!AreEqual(f1[i, j], f2[i, j], delta))
            return false;
        }
      }
      return true;
    }

    public static bool AreEqual(Matrix<Complex64T> f1, Matrix<Complex64T> f2, float delta)
    {
      if (f1.RowCount != f2.RowCount)
        return false;
      if (f1.ColumnCount != f2.ColumnCount)
        return false;
      for (int i = 0; i < f1.RowCount; i++)
      {
        for (int j = 0; j < f1.ColumnCount; j++)
        {
          if (!AreEqual(f1[i, j], f2[i, j], delta))
            return false;
        }
      }
      return true;
    }
  }
}
