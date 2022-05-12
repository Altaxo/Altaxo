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
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  public abstract record ALSBase
  {
    public void FillBandMatrixOrder1(IMatrix<double> m, double[] weights, double lambda, int countM1)
    {
      // Fill the (1,1) band matrix with (W + lambda D'D) (Eq.(6) in Ref.[1])
      m[0, 0] = weights[0] + lambda;
      m[0, 1] = -lambda;

      for (int i = 1; i < countM1; ++i)
      {
        m[i, i - 1] = -lambda;
        m[i, i] = weights[i] + 2 * lambda;
        m[i, i + 1] = -lambda;
      }
      m[countM1, countM1 - 1] = -lambda;
      m[countM1, countM1] = weights[countM1] + lambda;
    }

    public void UpdateBandMatrixDiagonalOrder1(IMatrix<double> m, double[] weights, double lambda, int countM1)
    {
      m[0,0] = weights[0] + lambda;
      for (int i = 1; i < countM1; ++i)
      {
        m[i, i] = weights[i] + 2 * lambda;
      }
      m[countM1, countM1] = weights[countM1] + lambda;
    }

      public void FillBandMatrixOrder2(IMatrix<double> m, double[] weights, double lambda, int countM1)
    {
      // Fill the (2,2) band matrix with (W + lambda D'D) (Eq.(6) in Ref.[1])
      m[0, 0] = weights[0] + lambda;
      m[0, 1] = -2 * lambda;
      m[0, 2] = lambda;

      m[1, 0] = -2 * lambda;
      m[1, 1] = weights[1] + 5 * lambda;
      m[1, 2] = -4 * lambda;
      m[1, 3] = lambda;

      for (int i = 2; i < countM1 - 1; ++i)
      {
        m[i, i - 2] = lambda;
        m[i, i - 1] = -4 * lambda;
        m[i, i] = weights[i] + 6 * lambda;
        m[i, i + 1] = -4 * lambda;
        m[i, i + 2] = lambda;
      }

      m[countM1 - 1, countM1 - 3] = lambda;
      m[countM1 - 1, countM1 - 2] = -4 * lambda;
      m[countM1 - 1, countM1 - 1] = weights[countM1 - 1] + 5 * lambda;
      m[countM1 - 1, countM1] = -2 * lambda;


      m[countM1, countM1 - 2] = lambda;
      m[countM1, countM1 - 1] = -2 * lambda;
      m[countM1, countM1] = weights[countM1] + lambda;
    }

    public void UpdateBandMatrixDiagonalOrder2(IMatrix<double> m, double[] weights, double lambda, int countM1)
    {
      // Fill the (2,2) band matrix with (W + lambda D'D) (Eq.(6) in Ref.[1])
      m[0, 0] = weights[0] + lambda;
      m[1, 1] = weights[1] + 5 * lambda;

      for (int i = 2; i < countM1 - 1; ++i)
      {
        m[i, i] = weights[i] + 6 * lambda;
      }

      m[countM1 - 1, countM1 - 1] = weights[countM1 - 1] + 5 * lambda;
      m[countM1, countM1] = weights[countM1] + lambda;
    }

  }
}
