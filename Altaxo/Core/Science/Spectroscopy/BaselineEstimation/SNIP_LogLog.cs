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
  /// </remarks>
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
    /// <param name="yArray">The array of spectral values.</param>
    /// <param name="result">The location where the baseline corrected spectrum should be stored.</param>
    /// <returns>The evaluated background of the provided spectrum.</returns>
    public override void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> result)
    {
      var srcY = new double[yArray.Length];
      var tmpY = new double[yArray.Length];

      // Forward transform the data
      var yStatistics = new Altaxo.Calc.Regression.QuickStatistics();
      double yOffset = yStatistics.Min - 1;
      for (int i = 0; i < yArray.Length; i++)
      {
        srcY[i] = Math.Log(Math.Log(yArray[i] - yOffset) + 1);
      }


      var stat = GetStatisticsOfInterPointDistance(xArray);
      if (_isHalfWidthInXUnits && 0.5 * (stat.Max - stat.Min) / stat.Max > 1.0 / xArray.Length)
      {
        // if the interpoint distant is not uniform, we need to use the algorithm with locally calculated half width
        EvaluateBaselineWithLocalHalfWidth(xArray, srcY, tmpY, result);
        return;
      }
      else
      {
        var w = _isHalfWidthInXUnits ? Math.Max(1, (int)Math.Abs(_halfWidth / stat.Mean)) : Math.Max(1, (int)_halfWidth);
        EvaluateBaselineWithConstantHalfWidth(xArray, srcY, tmpY, w, result);
      }

      // Back transform
      for (int i = 0; i < srcY.Length; ++i)
      {
        result[i] = Math.Exp(Math.Exp(result[i]) - 1) + yOffset;
      }

      srcY.CopyTo(result);
    }

    public override string ToString()
    {
      return $"{this.GetType().Name} HW={HalfWidth}{(IsHalfWidthInXUnits ? 'X' : 'P')} Iterations={NumberOfRegularIterations}";
    }
  }
}
