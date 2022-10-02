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

using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  public class PeakFitFunctions
  {
    public class FunctionWrapper : IScalarFunctionDD
    {
      private IFitFunction _f;
      private double[] _param;
      private double[] _x;
      private double[] _y;

      public FunctionWrapper(IFitFunction f, IReadOnlyList<double> param)
      {
        _f = f;
        _param = param.ToArray();
        _x = new double[1];
        _y = new double[1];
      }
      public double Evaluate(double x)
      {
        _x[0] = x;
        _f.Evaluate(_x, _param, _y);
        return _y[0];
      }
    }
  }
}
