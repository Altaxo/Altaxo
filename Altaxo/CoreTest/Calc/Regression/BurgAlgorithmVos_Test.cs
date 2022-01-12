#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Calc.Regression
{
  public class BurgAlgorithmVos_Test
  {
    [Fact]
    public void TestSinus2Coefficients()
    {
      var x = new double[880];
      var fdt = 1 / 8.0;
      var ampl = 7.0;
      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = ampl * Math.Sin(2 * Math.PI * i * fdt);
      }

      var burg = new BurgAlgorithmVos();
      burg.Execute(x, 2);
      Assert.Equal(2, burg.NumberOfCoefficients);
      Assert.Equal(2, burg.Coefficients.Length);
      AssertEx.AreEqual(-Math.Sin(2 * Math.PI * 2 * fdt) / Math.Sin(2 * Math.PI * fdt), burg.Coefficients[0], 1E-3, 1E-3);
      AssertEx.AreEqual(1.0, burg.Coefficients[1], 1E-3, 1E-3);
      AssertEx.AreEqual(ampl * ampl / 2, burg.MeanSquareSignal, 1E-3, 1E-3);
      AssertEx.AreEqual(ampl / Math.Sqrt(2), burg.RMSSignal,1E-3, 1E-3);

      var al = burg.GetFrequencyResponse(fdt * 0.99);
      var am = burg.GetFrequencyResponse(fdt * 1.00);
      var au = burg.GetFrequencyResponse(fdt * 1.01);

      AssertEx.Greater(am, al);
      AssertEx.Greater(am, au);
    }

    [Fact]
    public void TestSinus3Coefficients()
    {
      var x = new double[32*20];
      var fdt1 = 1 / 32.0;
      var fdt2 = 1 / 8.0;
      var ampl1 = 7.0;
      var ampl2 = 3.0;

      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = ampl1 * Math.Sin(2 * Math.PI * i * fdt1) + ampl2 * Math.Sin(2 * Math.PI * i * fdt2);
      }

      var burg = new BurgAlgorithmVos();
      burg.Execute(x, 3);
      Assert.Equal(3, burg.NumberOfCoefficients);
      Assert.Equal(3, burg.Coefficients.Length);
      AssertEx.AreEqual(-2.4732382663812893, burg.Coefficients[0], 1E-3, 1E-3);
      AssertEx.AreEqual(2.3850036460605875, burg.Coefficients[1], 1E-3, 1E-3);
      AssertEx.AreEqual(-0.90057116638002777, burg.Coefficients[2], 1E-3, 1E-3);
      AssertEx.AreEqual(ampl1 * ampl1 / 2 + ampl2 * ampl2 / 2, burg.MeanSquareSignal, 1E-3, 1E-3);
      AssertEx.AreEqual(Math.Sqrt(ampl1 * ampl1 / 2 + ampl2 * ampl2 / 2), burg.RMSSignal, 1E-3, 1E-3);
    }

    [Fact]
    public void TestReuseOfBurgSinus3Coefficients()
    {
      var x = new double[32 * 20];
      var fdt1 = 1 / 32.0;
      var fdt2 = 1 / 8.0;
      var ampl1 = 7.0;
      var ampl2 = 3.0;

      // first, we call burg with another signal, in order to populate the arrays
      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = 55 * Math.Sin(2 * Math.PI * i /17.0) + 11 * Math.Sin(2 * Math.PI * i /7.0);
      }
      var burg = new BurgAlgorithmVos();
      burg.Execute(x, 3);
      Assert.Equal(3, burg.NumberOfCoefficients);
      Assert.Equal(3, burg.Coefficients.Length);


      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = ampl1 * Math.Sin(2 * Math.PI * i * fdt1) + ampl2 * Math.Sin(2 * Math.PI * i * fdt2);
      }

      // now reuse burg, it should result the same parameter as in TestSinus3Coefficients
      burg.Execute(x, 3);
      Assert.Equal(3, burg.NumberOfCoefficients);
      Assert.Equal(3, burg.Coefficients.Length);
      AssertEx.AreEqual(-2.4732382663812893, burg.Coefficients[0], 1E-3, 1E-3);
      AssertEx.AreEqual(2.3850036460605875, burg.Coefficients[1], 1E-3, 1E-3);
      AssertEx.AreEqual(-0.90057116638002777, burg.Coefficients[2], 1E-3, 1E-3);
      AssertEx.AreEqual(ampl1 * ampl1 / 2 + ampl2 * ampl2 / 2, burg.MeanSquareSignal, 1E-3, 1E-3);
      AssertEx.AreEqual(Math.Sqrt(ampl1 * ampl1 / 2 + ampl2 * ampl2 / 2), burg.RMSSignal, 1E-3, 1E-3);
    }
  }
}


