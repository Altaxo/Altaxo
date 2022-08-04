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

using System.Collections.Immutable;
using System.Linq;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  public record XCalibrationByDataSource : ICalibration, Main.IImmutable
  {
    public string? RelativeTableName { get; init; }
    public string? AbsoluteTableName { get; init; }

    public ImmutableArray<(double x_uncalibrated, double x_calibrated)> CalibrationTable { get; init; }


    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {

      var ux = CalibrationTable.Select(p => p.x_uncalibrated).ToArray();
      var uy = CalibrationTable.Select(p => p.x_calibrated - p.x_uncalibrated).ToArray();

      var spline = new Altaxo.Calc.Interpolation.CrossValidatedCubicSpline();
      spline.Interpolate(ux, uy);

      var xx = new double[x.Length];

      for (int i = 0; i < x.Length; ++i)
      {
        xx[i] = x[i] + spline.GetYOfX(x[i]);
      }
      return (xx, y, regions);
    }
  }
}
