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
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy;

namespace Altaxo.Gui.Science.Spectroscopy.Calibration
{
  public record YCalibrationSetup
  {
    /// <summary>
    /// Gets or sets the x column. The x-column is shared among both the YSignal column and the YDark column.
    /// </summary>
    public DataColumn XColumn { get; set; }

    /// <summary>
    /// Gets or sets the column containing the spectrum of the known source.
    /// </summary>
    public DataColumn YColumn { get; set; }

    public SpectralPreprocessingOptionsBase SpectralPreprocessing { get; set; }

    public IFitFunction CurveShape { get; set; } = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, -1);

    public (string Name, double Value)[] CurveParameter { get; set; } = Array.Empty<(string Name, double Value)>();
  }
}
