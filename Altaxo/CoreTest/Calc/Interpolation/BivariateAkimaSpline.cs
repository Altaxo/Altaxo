#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using System;
using Altaxo.Calc;
using Altaxo.Calc.Interpolation;
using NUnit.Framework;

namespace AltaxoTest.Calc.Interpolation
{
  /// <summary>
  /// Test for the bivariate spline from Akima.
  /// </summary>
  /// <remarks> The test data originates from:
  /// <para>Hiroshi Akima</para>
  /// <para>"Algorithm 474: Bivariate Interpolation and smooth surface fitting based on local procedures"</para>
  /// <para>Communications of the ACM, vol.17 (Jan.1974), Number 1</para>
  /// </remarks>
  [TestFixture]
  public class TestBivariateAkima
  {
    double[][] _testvals = {
                             new double[]{58.2, 61.5, 47.9, 62.3, 34.6, 45.5, 38.2, 41.2, 41.7 },
                             new double[]{37.2, 40.0, 27.0, 41.3, 14.1, 24.5, 17.3, 20.2, 20.8 },
                             new double[]{22.4, 22.5, 14.6, 22.5, 4.7, 7.2, 1.8, 2.1, 2.1 },
                             new double[]{21.8, 20.5, 12.8, 17.6, 5.8, 7.6, 0.8, 0.6, 0.6 },
                             new double[]{16.8, 14.4, 8.1, 6.9, 6.2, 0.6, 0.1, 0.0, 0.0 },
                             new double[]{12.0, 8.0, 5.3, 2.9, 0.6, 0.0, 0.0, 0.0, 0.0 },
                             new double[]{7.4, 4.8, 1.4, 0.1, 0.0, 0.0, 0.0, 0.0, 0.0 },
                             new double[]{3.2, 0.7, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                             new double[]{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                             new double[]{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                             new double[]{0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 }
                           };


    double[][] _refvals = {
                            new double[]{50.59, 43.75, 43.73, 36.94, 29.19, 30.29, 28.95, 30.99 }, // x=2.5
                            new double[]{30.35, 24.80, 25.03, 19.15, 11.32, 10.48, 8.35, 9.58 }, // x=7.5
                            new double[]{22.19, 17.47, 16.39, 12.14, 6.11, 3.41, 0.93, 0.68}, // x=12.5
                            new double[]{18.98, 13.78, 10.92, 9.12, 4.79, 1.72, 0.28, 0.21}, // x=17.5
                            new double[]{12.86, 8.73, 5.61, 3.94, 1.77, 0.06, 0.01, 0.0}, // x=22.5
                            new double[]{7.77, 4.71, 2.05, 0.60, -0.02, 0.00, 0.00, 0.00}, // x=27.5
                            new double[]{3.86, 1.34, 0.04, 0.00, 0.00, 0.00, 0.00, 0.00}, // x=32.5
                            new double[]{0.41, -0.01, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00}, // x=37.5
                            new double[]{0.00, 0.00, 0.00, 0.00, 0.00, 0.00,0.00, 0.00}, // x=42.5
                            new double[]{0.00, 0.00, 0.00, 0.00, 0.00, 0.00,0.00, 0.00} // x=47.5
                          };
    double[] _refx = { 2.5, 7.5, 12.5, 17.5, 22.5, 27.5, 32.5, 37.5, 42.5, 47.5 };
    double[] _refy = { 2.5, 7.5, 12.5, 17.5, 22.5, 27.5, 32.5, 37.5};



    double[] _testx = new double[] { 0.0, 5.0, 10.0, 15.0, 20.0, 25.0, 30.0, 35.0, 40.0, 45.0, 50.0 };
    double[] _testy = new double[] { 0.0, 5.0, 10.0, 15.0, 20.0, 25.0, 30.0, 35.0, 40.0 };

    [Test]
    public  void Test1()
    {
      double[] z = new double[_testx.Length * _testy.Length];
      for (int iy = 0; iy < _testy.Length; iy++)
        for (int ix = 0; ix < _testx.Length; ix++)
          z[ix + iy * _testx.Length] = _testvals[ix][iy];

      double[] u = new double[1]; u[0] = 15.00000001;
      double[] v = new double[1]; v[0] = 15.00000001;
      double[] w = new double[1];

      for (int iy = 0; iy < _testy.Length; iy++)
      {
        for (int ix = 0; ix < _testx.Length; ix++)
        {
          u[0] = _testx[ix];
          v[0] = _testy[iy];
          
          BivariateAkimaSpline.Interpolate(_testx, _testy, z, u, v, w);
          Assert.AreEqual(_testvals[ix][iy],w[0],1E-2);
        }
      }

      for (int iy = 0; iy < _refy.Length; iy++)
      {
        for (int ix = 0; ix < _refx.Length ; ix++)
        {
          u[0] = _refx[ix];
          v[0] = _refy[iy];
         
          BivariateAkimaSpline.Interpolate(_testx, _testy, z, u, v, w);
          Assert.AreEqual(_refvals[ix][iy],w[0],1E-2,string.Format("ix={0}, iy={1} u={2}, v={3}",ix,iy,u[0],v[0]));
        }
      }

    
    }



  }
}
