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
  /// SNIP algorithm for background estimation (SNIP = Statistical sensitive Non-Linear Iterative Procedure).
  /// Before execution of the algorithm, the data are twice logarithmized, as described in Ref.[1], and backtransformed afterwards.
  /// </summary>
  /// <remarks>
  /// In difference to the procedure described in Ref. [1], no previous smoothing is applied to the data.
  /// As described in the paper, after execution the number of regular stages of the algorithm, the window width is sucessivly decreased, until it reaches 1.
  /// This results in a smoothing of the background signal.
  /// 
  /// <para>References:</para>
  /// <para>[1] C.G. Ryan et al., SNIP, A STATISTICS-SENSITIVE BACKGROUND TREATMENT FOR THE QUANTITATIVE 
  /// ANALYSIS OF PIXE SPECTRA IN GEOSCIENCE APPLICATIONS, Nuclear Instruments and Methods in Physics Research 934 (1988) 396-402 
  /// North-Holland, Amsterdam</para>
  // </remarks>
  public record SNIP_LogLog : SNIP_Base, IBaselineEstimation
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SNIP_Linear), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SNIP_Linear)obj;
        info.AddValue("HalfWidth", s.HalfWidth);
        info.AddValue("IsHalfWidthInXUnits", s.IsHalfWidthInXUnits);
        info.AddValue("NumberOfIterations", s.NumberOfRegularIterations);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var halfWidth = info.GetDouble("HalfWidth");
        var isHalfWidthInXUnits = info.GetBoolean("IsHalfWidthInXUnits");
        var numberOfIterations = info.GetInt32("NumberOfIterations");

        return o is null ? new SNIP_Linear
        {
          HalfWidth = halfWidth,
          IsHalfWidthInXUnits = isHalfWidthInXUnits,
          NumberOfRegularIterations = numberOfIterations
        } :
          ((SNIP_Linear)o) with
          {
            HalfWidth = halfWidth,
            IsHalfWidthInXUnits = isHalfWidthInXUnits,
            NumberOfRegularIterations = numberOfIterations
          };
      }
    }
    #endregion


    /// <summary>
    /// Executes the algorithm with the provided spectrum.
    /// </summary>
    /// <param name="xArray">The x values of the spectral values.</param>
    /// <param name="yArray">The array of spectral values. All values y[i] in the spectrum must be y[i] &gt; 0</param>
    /// <returns>The evaluated background of the provided spectrum.</returns>
    public override double[] Execute(double[] xArray, double[] yArray)
    {
      const double sqrt2 = 1.41421356237;

      // Log-Log the data
      var srcY = yArray.Select(x => Math.Log(Math.Log(x + 1) + 1)).ToArray();
      var tmpY = new double[srcY.Length];

      int last = srcY.Length - 1;
      var w = _isHalfWidthInXUnits ? Math.Max(1, (int)(_halfWidth / (xArray[1] - xArray[0]))) : Math.Max(1, (int)_halfWidth);

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
          var yLeft = iLeft >= 0 ? srcY[iLeft] : double.PositiveInfinity;
          var yRight = iRight <= last ? srcY[iRight] : double.PositiveInfinity;
          var yMid = 0.5 * (yLeft + yRight);
          tmpY[i] = Math.Min(yMid, srcY[i]);
        }

        (tmpY, srcY) = (srcY, tmpY);
      }

      // Back transform
      for (int i = 0; i < srcY.Length; ++i)
      {
        srcY[i] = Math.Exp(Math.Exp(srcY[i]) - 1) - 1;
      }

      return srcY;
    }
  }
}
