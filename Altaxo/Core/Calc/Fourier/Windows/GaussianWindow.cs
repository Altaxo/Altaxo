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
using System.Collections.Generic;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Fourier.Windows
{
  /// <summary>
  /// Implements the Gaussian window function.
  /// </summary>
  public class GaussianWindow : AbstractWindow
  {
    private double _alpha = 2.5;

    /// <summary>
    /// Initializes a new instance of the <see cref="GaussianWindow"/> class with the default alpha.
    /// </summary>
    /// <param name="count">The number of samples of the window.</param>
    /// <param name="periodic">If set to <c>true</c> the window is periodic.</param>
    public GaussianWindow(int count, bool periodic)
      : base(count, periodic)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaussianWindow"/> class with the specified alpha.
    /// </summary>
    /// <param name="count">The number of samples of the window.</param>
    /// <param name="periodic">If set to <c>true</c> the window is periodic.</param>
    /// <param name="alpha">The alpha parameter controlling the width of the Gaussian window.</param>
    public GaussianWindow(int count, bool periodic, double alpha)
      : base(count, periodic)
    {
      _alpha = alpha;
    }

    /// <inheritdoc/>
    protected override void InternalCompute(IVector<double> array, bool periodic)
    {
      int len = array.Count;
      int N = periodic ? len : len - 1;
      double N2 = N / 2.0;
      for (int i = 0; i < len; ++i)
      {
        double arg = _alpha * (i - N2) / N2;
        array[i] = Math.Exp(-0.5 * arg * arg);
      }
    }

    /// <summary>
    /// Returns the window as an array of doubles.
    /// </summary>
    /// <param name="count">Length of the window.</param>
    /// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
    /// <returns>The window as array of doubles.</returns>
    public static double[] AsDoubleArray(int count, bool periodic)
    {
      return new GaussianWindow(count, periodic).AsDoubleArray();
    }

    /// <summary>
    /// Returns the window as an read only vector.
    /// </summary>
    /// <param name="count">Length of the window.</param>
    /// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
    /// <returns>The window as read only vector.</returns>
    public static IReadOnlyList<double> AsROVector(int count, bool periodic)
    {
      return new GaussianWindow(count, periodic).AsROVector();
    }

    /// <summary>
    /// Returns the window as writeable vector.
    /// </summary>
    /// <param name="count">Length of the window.</param>
    /// <param name="periodic">Periodic conditions, see remarks in the base class.</param>
    /// <returns>The window as writeable vector.</returns>
    public static IVector<double> AsVector(int count, bool periodic)
    {
      return new GaussianWindow(count, periodic).AsVector();
    }
  }
}
