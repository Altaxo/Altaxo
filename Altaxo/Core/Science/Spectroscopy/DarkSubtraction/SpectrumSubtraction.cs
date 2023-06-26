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
using System.Collections.Immutable;

namespace Altaxo.Science.Spectroscopy.DarkSubtraction
{
  public record SpectrumSubtraction : IDarkSubtraction, IReferencingXYColumns
  {
    /// <inheritdoc/>
    public (string TableName, int GroupNumber, string XColumnName, string YColumnName)? XYDataOrigin { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<(double x, double y)> XYCurve { get; init; }


    #region Serialization

    /// <summary>
    /// 2023-03-29 Initial version
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectrumSubtraction), 0)]
    public class SerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectrumSubtraction)obj;

        info.AddValue("TableName", s.XYDataOrigin?.TableName);
        info.AddValue("GroupNumber", (int?)s.XYDataOrigin?.GroupNumber);
        info.AddValue("XColumnName", s.XYDataOrigin?.XColumnName);
        info.AddValue("YColumnName", s.XYDataOrigin?.YColumnName);

        info.CreateArray("Spectrum", s.XYCurve.Length);
        {
          foreach (var ele in s.XYCurve)
          {
            info.CreateElement("e");
            {
              info.AddValue("x", ele.x);
              info.AddValue("y", ele.y);
            }
            info.CommitElement(); // e
          }
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var tableName = info.GetString("TableName");
        var groupNumber = info.GetNullableInt32("GroupNumber");
        var xcolumnname = info.GetString("XColumnName");
        var ycolumnname = info.GetString("YColumnName");



        var count = info.OpenArray("CalibrationTable");
        var array = new (double x_uncalibrated, double x_calibrated)[count];
        for (var i = 0; i < count; i++)
        {
          info.OpenElement();
          {
            array[i] = (info.GetDouble("x"), info.GetDouble("y"));
          }
          info.CloseElement();
        }
        info.CloseArray(count);


        return new SpectrumSubtraction()
        {
          XYDataOrigin = !string.IsNullOrEmpty(tableName) &&
                        groupNumber.HasValue &&
                       !string.IsNullOrEmpty(xcolumnname) &&
                       !string.IsNullOrEmpty(ycolumnname) ? (tableName, groupNumber.Value, xcolumnname, ycolumnname) : null,
          XYCurve = array.ToImmutableArray(),
        };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yy = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ReadOnlySpan<double>(x, start, end - start).ToArray();
        var ySpan = new ReadOnlySpan<double>(y, start, end - start).ToArray();
        var yySpan = new double[end - start];
        Execute(xSpan, ySpan, yySpan);
        Array.Copy(yySpan, 0, yy, start, yySpan.Length);
      }
      return (x, yy, regions);
    }

    public void Execute(double[] x, double[] y, double[] yResult)
    {
      var spl = new Calc.Interpolation.AkimaCubicSpline();
      var xsub = new double[XYCurve.Length];
      var ysub = new double[XYCurve.Length];
      for (int i = 0; i < XYCurve.Length; i++)
      {
        xsub[i] = XYCurve[i].x;
        ysub[i] = XYCurve[i].y;
      }

      spl.Interpolate(xsub, ysub);

      for (var i = 0; i < yResult.Length; ++i)
      {
        yResult[i] = y[i] - spl.GetYOfX(x[i]);
      }
    }

    IReferencingXYColumns IReferencingXYColumns.WithXYDataOrigin((string TableName, int GroupNumber, string XColumnName, string YColumnName) xyDataOrigin)
    {
      return this with { XYDataOrigin = xyDataOrigin };
    }

    IReferencingXYColumns IReferencingXYColumns.WithXYCurve(ImmutableArray<(double x, double y)> xyCurve)
    {
      return this with { XYCurve = xyCurve };
    }

    public override string ToString()
    {
      return $"{this.GetType().Name} Table={XYDataOrigin}";
    }
  }
}
