#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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

#region Using directives

using System;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

#endregion

namespace AltaxoTest.Calc.LinearAlgebra 
{
  internal sealed class Comparer 
  {
    private Comparer() { }

    public static bool AreEqual(ComplexFloat f1, ComplexFloat f2) 
    {
      return f1 == f2;
    }

    public static bool AreEqual(Complex f1, Complex f2) 
    {
      return f1 == f2;
    }

    public static bool AreEqual(ComplexFloat f1, ComplexFloat f2, float delta) 
    {
      if (System.Math.Abs(f1.Imag - f2.Imag) > delta) return false;
      if (System.Math.Abs(f1.Real - f2.Real) > delta) return false;
      return true;
    }

    public static bool AreEqual(Complex f1, Complex f2, double delta) 
    {
      if (System.Math.Abs(f1.Imag - f2.Imag) > delta) return false;
      if (System.Math.Abs(f1.Real - f2.Real) > delta) return false;
      return true;
    }

    public static bool AreEqual(ComplexFloatMatrix f1, ComplexFloatMatrix f2) 
    {
      return f1==f2;
    }

    public static bool AreEqual(ComplexDoubleMatrix f1, ComplexDoubleMatrix f2) 
    {
      return f1==f2;
    }

    public static bool AreEqual(ComplexFloatMatrix f1, ComplexFloatMatrix f2, float delta) 
    {
      if (f1.RowLength != f2.RowLength) return false;
      if (f1.ColumnLength != f2.ColumnLength) return false;
      for(int i=0; i<f1.RowLength; i++) 
      {
        for (int j = 0; j < f1.ColumnLength; j++) 
        {
          if (!AreEqual(f1[i, j], f2[i, j], delta)) 
            return false;
        }
      }
      return true;
    }

    public static bool AreEqual(ComplexDoubleMatrix f1, ComplexDoubleMatrix f2, float delta) 
    {
      if (f1.RowLength != f2.RowLength) return false;
      if (f1.ColumnLength != f2.ColumnLength) return false;
      for (int i = 0; i < f1.RowLength; i++) 
      {
        for (int j = 0; j < f1.ColumnLength; j++) 
        {
          if (!AreEqual(f1[i, j], f2[i, j], delta))
            return false;
        }
      }
      return true;
    }
  }
}
