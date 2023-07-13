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

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  public record XToXLine
    : IBaselineEstimation
  {
    protected double _x0;
    protected double _x1;

    /// <summary>
    /// Half of the width of the averaging window. This value should be set to
    /// roughly the FWHM (full width half maximum) of the broadest peak in the spectrum.
    /// </summary>
    public double X0
    {
      get { return _x0; }
      init
      {
        _x0 = value;
      }
    }

    /// <summary>
    /// Half of the width of the averaging window. This value should be set to
    /// roughly the FWHM (full width half maximum) of the broadest peak in the spectrum.
    /// </summary>
    public double X1
    {
      get { return _x1; }
      init
      {
        _x1 = value;
      }
    }


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XToXLine), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XToXLine)obj;
        info.AddValue("X0", s._x0);
        info.AddValue("X1", s._x1);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var x0 = info.GetDouble("X0");
        var x1 = info.GetDouble("X1");
        return new XToXLine() { _x0 = x0, _x1 = x1 };
      }
    }
    #endregion




    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yBaseline = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ReadOnlySpan<double>(x, start, end - start);
        var ySpan = new ReadOnlySpan<double>(y, start, end - start);
        var yBaselineSpan = new Span<double>(yBaseline, start, end - start);
        Execute(xSpan, ySpan, yBaselineSpan);
      }

      // subtract baseline
      var yy = new double[y.Length];
      for (int i = 0; i < y.Length; i++)
      {
        yy[i] = y[i] - yBaseline[i];
      }

      return (x, yy, regions);
    }

    /// <summary>
    /// Executes the algorithm with the provided spectrum.
    /// </summary>
    /// <param name="xArray">The x values of the spectral values.</param>
    /// <param name="yArray">The array of spectral values.</param>
    /// <param name="result">The location where the baseline corrected spectrum should be stored.</param>
    /// <returns>The evaluated background of the provided spectrum.</returns>
    public virtual void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> result)
    {
      int minIdx0 = 0, minIdx1 = xArray.Length - 1;
      double min0 = double.PositiveInfinity, min1 = double.PositiveInfinity;
      for (int i = 0; i < xArray.Length; ++i)
      {
        var dist0 = Math.Abs(xArray[i] - _x0);
        var dist1 = Math.Abs(xArray[i] - _x1);
        if (dist0 < min0) { minIdx0 = i; min0 = dist0; }
        if (dist1 < min1) { minIdx1 = i; min1 = dist1; }
      }

      if (minIdx0 == minIdx1)
        throw new InvalidOperationException($"{this.GetType().Name}: Can not proceed because both anchor points have the same index, {_x0}=>{minIdx0}; {_x1}=>{minIdx1}");

      var xleft = xArray[minIdx0];
      var xright = xArray[minIdx1];
      var yleft = yArray[minIdx0];
      var yright = yArray[minIdx1];
      var xspan = xright - xleft;
      for (int i = 0; i < xArray.Length; ++i)
      {
        result[i] = yleft * (xright - xArray[i]) / xspan + yright * (xArray[i] - xleft) / xspan;
      }
    }
  }
}
