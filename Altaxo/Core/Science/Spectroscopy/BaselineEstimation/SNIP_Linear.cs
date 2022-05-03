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

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// SNIP algorithm for background estimation on linear (unmodified) data. SNIP = Statistical sensitive Non-Linear Iterative Procedure.
  /// </summary>
  /// <remarks>
  /// In difference to the procedure described in Ref. 1, no previous smoothing is applied to the data. Furthermore,
  /// the paper suggests to twice logarithmize the data beforehand, which is also not done here.
  /// As described in the paper, after execution the number of regular stages of the algorithm, the window width is sucessivly decreased, until it reaches 1.
  /// This results in a smoothing of the background signal.
  /// 
  /// <para>References:</para>
  /// <para>[1] C.G. Ryan et al., SNIP, A STATISTICS-SENSITIVE BACKGROUND TREATMENT FOR THE QUANTITATIVE 
  /// ANALYSIS OF PIXE SPECTRA IN GEOSCIENCE APPLICATIONS, Nuclear Instruments and Methods in Physics Research 934 (1988) 396-402 
  /// North-Holland, Amsterdam</para>
  // </remarks>
  public class SNIP_Linear : IBaselineEstimation
  {
    int _halfWidth = 15;
    int _numberOfRegularStages = 40;

    /// <summary>
    /// Initializes a new instance of the <see cref="SNIP_Linear"/> class.
    /// </summary>
    /// <param name="halfWidth">Half of the width of the averaging window. This value should be set to
    /// roughly the FWHM (full width half maximum) of the broadest peak in the spectrum.</param>
    public SNIP_Linear(int halfWidth)
    {
      _halfWidth = halfWidth;
    }

    /// <summary>
    /// Gets or sets the number of regular stages. Default is 40.
    /// </summary>
    /// <value>
    /// The number of regular stages.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">Number of stages must be at least one. - NumberOfRegularStages</exception>
    public int NumberOfRegularStages
    {
      get { return _numberOfRegularStages; }
      set
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException("Number of stages must be at least one.", nameof(NumberOfRegularStages));
        _numberOfRegularStages = value;
      }
    }

    /// <summary>
    /// Executes the algorithm with the provided spectrum.
    /// </summary>
    /// <param name="array">The array of spectral values.</param>
    /// <returns>The evaluated background of the provided spectrum.</returns>
    public double[] Execute(IEnumerable<double> array)
    {
      const double sqrt2 = 1.41421356237;
      var srcY = array.ToArray();
      var tmpY = new double[srcY.Length];

      int last = srcY.Length - 1;
      int w = _halfWidth;

      for (int iStage = _numberOfRegularStages - 1; ; --iStage)
      {
        if (iStage < 0)
        {
          w = Math.Min(w - 1, (int)(w / sqrt2));
          if (w < 1)
          {
            break;
          }
        }

        for (int i = 0; i <= last; i++)
        {
          var iLeft = i - w;
          var iRight = i + w;
          var yLeft = iLeft < 0 ? srcY[0] : srcY[iLeft];
          var yRight = iRight > last ? srcY[last] : srcY[iRight];
          var yMid = 0.5 * (yLeft + yRight);
          tmpY[i] = Math.Min(yMid, srcY[i]);
        }

        (tmpY, srcY) = (srcY, tmpY);
      }

      return srcY;
    }

  }
}
