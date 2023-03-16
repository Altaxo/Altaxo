#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Numerics;

namespace Altaxo.Calc.IntegralTransforms
{
  /// <summary>
  /// Hilbert transformation for real valued arrays.
  /// </summary>
  public class HilbertTransformation
  {
    /// <summary>
    /// Executes a Hilbert transformation of a real valued signal.
    /// </summary>
    /// <param name="xr">The signal to transform.</param>
    /// <returns>The complex Hilbert transform of the signal.</returns>
    public static Complex[] Transformation(ReadOnlySpan<double> xr)
    {
      var x = new Complex[xr.Length];
      for (int i = 0; i < x.Length; ++i)
      {
        x[i] = new Complex(xr[i], 0);
      }
      Fourier.Forward(x, FourierOptions.Default);

      var h = new double[x.Length];
      var fftLengthIsOdd = (x.Length | 1) == 1;
      if (fftLengthIsOdd)
      {
        h[0] = 1;
        for (var i = 1; i < xr.Length / 2; i++) h[i] = 2;
      }
      else
      {
        h[0] = 1;
        h[(xr.Length / 2)] = 1;
        for (var i = 1; i < xr.Length / 2; i++) h[i] = 2;
      }

      for (var i = 0; i < x.Length; i++)
      {
        x[i] *= h[i];
      }

      Fourier.Inverse(x, FourierOptions.Default);
      return x;
    }

    /// <summary>
    /// Calculates the instantaneous frequencies.
    /// </summary>
    /// <param name="hilbertC">The result of the Hilbert transformation.</param>
    /// <param name="xIncrement">X-axis increment of the signal (default is 1).</param>
    /// <returns>Array of the instantaneuous frequencies.</returns>
    public static double[] GetInstantaneousFrequencies(ReadOnlySpan<Complex> hilbertC, double xIncrement = 1)
    {
      var result = new double[hilbertC.Length];
      for (int i = 0; i < hilbertC.Length; i++)
      {
        double phi;

        phi = i > 0 ? hilbertC[i].Phase - hilbertC[i - 1].Phase : hilbertC[i + 1].Phase - hilbertC[i].Phase;
        if (phi > Math.PI)
          phi -= 2 * Math.PI;
        else if (phi < -Math.PI)
          phi += 2 * Math.PI;
        phi = Math.Abs(phi);

        result[i] = phi / (2 * Math.PI * xIncrement);
      }
      return result;
    }
  }
}
