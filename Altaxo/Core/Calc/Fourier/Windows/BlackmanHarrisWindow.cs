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

using Altaxo.Calc.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Fourier.Windows
{
  public class BlackmanHarrisWindow : AbstractWindow
  {
    public BlackmanHarrisWindow(int count, bool periodic)
      : base(count, periodic)
    {
    }

    protected override void InternalCompute(IVector<double> array, bool periodic)
    {
      int len = array.Length;
      int N = periodic ? len : len - 1;
      double scale1 = 2 * Math.PI / N;
      double scale2 = 4 * Math.PI / N;
      double scale3 = 6 * Math.PI / N;
      double ic;
      int i;
      for (i = 0, ic = -N / 2.0; i < len; ++i, ic += 1)
        array[i] = 0.35875 + 0.48829 * Math.Cos(ic * scale1) + 0.14128 * Math.Cos(ic * scale2) + 0.01168 * Math.Cos(ic * scale3);
    }

    /// <summary>
    /// Returns the window as an array of doubles.
    /// </summary>
    /// <param name="count">Length of the window.</param>
    /// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
    /// <returns>The window as array of doubles.</returns>
    public static double[] AsDoubleArray(int count, bool periodic)
    {
      return new BlackmanHarrisWindow(count, periodic).AsDoubleArray();
    }

    /// <summary>
    /// Returns the window as an read only vector.
    /// </summary>
    /// <param name="count">Length of the window.</param>
    /// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
    /// <returns>The window as read only vector.</returns>
    public static IROVector AsROVector(int count, bool periodic)
    {
      return new BlackmanHarrisWindow(count, periodic).AsROVector();
    }

    /// <summary>
    /// Returns the window as writeable vector.
    /// </summary>
    /// <param name="count">Length of the window.</param>
    /// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
    /// <returns>The window as writeable vector.</returns>
    public static IVector AsVector(int count, bool periodic)
    {
      return new BlackmanHarrisWindow(count, periodic).AsVector();
    }
  }
}