#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Fourier.Windows
{
  /// <summary>
  /// Common interface for 2D window functions to apply before.
  /// </summary>
  public interface IWindows2D
  {
    /// <summary>
    /// Applies the window function to the specified matrix <paramref name="m"/> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    void Apply(IMatrix<double> m);
  }

  /// <summary>
  /// Implements a 2D Hanning window for data pretreatment prior to a 2D Fourier transformation.
  /// </summary>
  public class HanningWindow2D : IWindows2D
  {
    /// <summary>
    /// Applies the Hanning window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public void Apply(IMatrix<double> m)
    {
      Application(m);
    }

    /// <summary>
    /// Applies the Hanning window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public static void Application(IMatrix<double> m)
    {
      var rows = m.RowCount;
      var cols = m.ColumnCount;

      double facI = 2.0 / (rows - 1);
      double facJ = 2.0 / (cols - 1);

      for (int i = 0; i < rows; ++i)
      {
        double ri = i * facI - 1;
        for (int j = 0; j < cols; ++j)
        {
          double rj = j * facJ - 1;
          double rij = Math.Sqrt(ri * ri + rj * rj);
          double windowFunc = rij <= 1 ? 0.5 * (1 + Math.Cos(Math.PI * rij)) : 0;
          m[i, j] *= windowFunc;
        }
      }
    }
  }

  /// <summary>
  /// Implements a 2D Bartlett window for data pretreatment prior to a 2D Fourier transformation.
  /// </summary>
  public class BartlettWindow2D : IWindows2D
  {
    /// <summary>
    /// Applies the Bartlett window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public void Apply(IMatrix<double> m)
    {
      Application(m);
    }

    /// <summary>
    /// Applies the Bartlett window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public static void Application(IMatrix<double> m)
    {
      var rows = m.RowCount;
      var cols = m.ColumnCount;

      double facI = 2.0 / (rows - 1);
      double facJ = 2.0 / (cols - 1);

      for (int i = 0; i < rows; ++i)
      {
        double ri = i * facI - 1;
        for (int j = 0; j < cols; ++j)
        {
          double rj = j * facJ - 1;
          double rij = Math.Sqrt(ri * ri + rj * rj);
          double windowFunc = rij <= 1 ? 1 - rij : 0;
          m[i, j] *= windowFunc;
        }
      }
    }
  }

  /// <summary>
  /// Implements a 2D Parzen window for data pretreatment prior to a 2D Fourier transformation.
  /// </summary>
  public class ParzenWindow2D : IWindows2D
  {
    /// <summary>
    /// Applies the Parzen window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public void Apply(IMatrix<double> m)
    {
      Application(m);
    }

    /// <summary>
    /// Applies the Parzen window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public static void Application(IMatrix<double> m)
    {
      var rows = m.RowCount;
      var cols = m.ColumnCount;

      double facI = 2.0 / (rows - 1);
      double facJ = 2.0 / (cols - 1);

      for (int i = 0; i < rows; ++i)
      {
        double ri = i * facI - 1;
        for (int j = 0; j < cols; ++j)
        {
          double rj = j * facJ - 1;
          double rij = Math.Sqrt(ri * ri + rj * rj);
          double windowFunc;
          if (rij <= 0.5)
            windowFunc = 1 - 6 * rij * rij + 6 * rij * rij * rij;
          else if (rij <= 1)
            windowFunc = 2 * RMath.Pow3(1 - rij);
          else
            windowFunc = 0;

          m[i, j] *= windowFunc;
        }
      }
    }
  }

  /// <summary>
  /// Implements a 2D Gauss window (with a default sigma of 0.35) for data pretreatment prior to a 2D Fourier transformation.
  /// </summary>
  public class GaussWindow2D : IWindows2D
  {
    public const double DefaultSigma = 0.35;

    public double _sigma = DefaultSigma;

    /// <summary>
    /// Gets or sets the sigma value. This value  is usually in the range of 0.3 .. 0.4.
    /// </summary>
    /// <value>
    /// The sigma value.
    /// </value>
    public double Sigma { get { return _sigma; } set { _sigma = value; } }

    /// <summary>
    /// Applies the Gauss window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor. The <see cref="Sigma"/> value is used to calculate the window.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public void Apply(IMatrix<double> m)
    {
      Application(m, _sigma);
    }

    /// <summary>
    /// Applies the Gauss window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor. The default sigma value (<see cref="DefaultSigma"/>) is used.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public static void Application(IMatrix<double> m)
    {
      Application(m, DefaultSigma);
    }

    /// <summary>
    /// Applies the Gauss window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    /// <param name="sigma">The sigma value to calculate the window function.</param>
    public static void Application(IMatrix<double> m, double sigma)
    {
      var rows = m.RowCount;
      var cols = m.ColumnCount;

      double facI = 2.0 / (rows - 1);
      double facJ = 2.0 / (cols - 1);

      for (int i = 0; i < rows; ++i)
      {
        double ri = i * facI - 1;
        for (int j = 0; j < cols; ++j)
        {
          double rj = j * facJ - 1;
          double rij = Math.Sqrt(ri * ri + rj * rj);
          double windowFunc = Math.Exp(-0.5 * RMath.Pow2(rij / sigma));
          m[i, j] *= windowFunc;
        }
      }
    }
  }

  /// <summary>
  /// Implements a 2D Gauss window (with a default sigma of 0.35) for data pretreatment prior to a 2D Fourier transformation.
  /// </summary>
  public class SuperGaussWindow2D : IWindows2D
  {
    public const double DefaultKappa = 0.35;

    public double _kappa = DefaultKappa;

    /// <summary>
    /// Gets or sets the kappa value. This value is usually  in the range of 0.3 .. 0.4.
    /// </summary>
    /// <value>
    /// The kappa value.
    /// </value>
    public double Kappa { get { return _kappa; } set { _kappa = value; } }

    /// <summary>
    /// Applies the Supergauss window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor. The <see cref="Kappa"/> value is used to calculate the window.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public void Apply(IMatrix<double> m)
    {
      Application(m, _kappa);
    }

    /// <summary>
    /// Applies the Supergauss window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor. The default sigma value (<see cref="DefaultKappa"/>) is used.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public static void Application(IMatrix<double> m)
    {
      Application(m, DefaultKappa);
    }

    /// <summary>
    /// Applies the Supergauss window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    /// <param name="kappa">The sigma value to calculate the window function.</param>
    public static void Application(IMatrix<double> m, double kappa)
    {
      var rows = m.RowCount;
      var cols = m.ColumnCount;

      double facI = 2.0 / (rows - 1);
      double facJ = 2.0 / (cols - 1);

      for (int i = 0; i < rows; ++i)
      {
        double ri = i * facI - 1;
        for (int j = 0; j < cols; ++j)
        {
          double rj = j * facJ - 1;
          double rij = Math.Sqrt(ri * ri + rj * rj);
          double windowFunc = Math.Exp(-RMath.Pow6(rij) / kappa);
          m[i, j] *= windowFunc;
        }
      }
    }
  }

  /// <summary>
  /// Implements a 2D Elliptic window for data pretreatment prior to a 2D Fourier transformation.
  /// </summary>
  public class EllipticWindow2D : IWindows2D
  {
    /// <summary>
    /// Applies the Elliptic window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public void Apply(IMatrix<double> m)
    {
      Application(m);
    }

    /// <summary>
    /// Applies the Elliptic window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public static void Application(IMatrix<double> m)
    {
      var rows = m.RowCount;
      var cols = m.ColumnCount;

      double facI = 2.0 / (rows - 1);
      double facJ = 2.0 / (cols - 1);

      for (int i = 0; i < rows; ++i)
      {
        double ri = i * facI - 1;
        for (int j = 0; j < cols; ++j)
        {
          double rj = j * facJ - 1;
          double rij = Math.Sqrt(ri * ri + rj * rj);
          if (rij > 1)
            m[i, j] = 0;
        }
      }
    }
  }

  /// <summary>
  /// Implements a 2D Hanning window for data pretreatment prior to a 2D Fourier transformation.
  /// </summary>
  public class CosineWindow2D : IWindows2D
  {
    /// <summary>
    /// Applies the Cosine window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public void Apply(IMatrix<double> m)
    {
      Application(m);
    }

    /// <summary>
    /// Applies the Cosine window function to the specified matrix <paramref name="m" /> by multiplying each matrix element with a factor.
    /// </summary>
    /// <param name="m">The matrix to modify.</param>
    public static void Application(IMatrix<double> m)
    {
      var rows = m.RowCount;
      var cols = m.ColumnCount;

      double facI = 2.0 / (rows - 1);
      double facJ = 2.0 / (cols - 1);

      for (int i = 0; i < rows; ++i)
      {
        double ri = i * facI - 1;
        for (int j = 0; j < cols; ++j)
        {
          double rj = j * facJ - 1;
          if (Math.Abs(rj) <= 1 && Math.Abs(ri) <= 1)
          {
            m[i, j] *= Math.Cos(0.5 * Math.PI * ri) * Math.Cos(0.5 * Math.PI * rj);
          }
          else
            m[i, j] = 0;
        }
      }
    }
  }
}
