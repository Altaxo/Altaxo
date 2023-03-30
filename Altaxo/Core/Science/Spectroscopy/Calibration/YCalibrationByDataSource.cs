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

using System.Collections.Immutable;
using System.Linq;

namespace Altaxo.Science.Spectroscopy.Calibration
{

  public record YCalibrationByDataSource : IYCalibration, Main.IImmutable, IYCalibrationTable, IReferencingTable
  {
    /// <inheritdoc/>
    public string? TableName { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<(double x, double yScalingFactor)> CalibrationTable { get; init; }


    #region Serialization

    /// <summary>
    /// 2023-03-27 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YCalibrationByDataSource), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (YCalibrationByDataSource)obj;
        info.AddValue("TableName", s.TableName);
        info.CreateArray("CalibrationTable", s.CalibrationTable.Length);
        {
          foreach (var ele in s.CalibrationTable)
          {
            info.CreateElement("e");
            {
              info.AddValue("x", ele.x);
              info.AddValue("ys", ele.yScalingFactor);
            }
            info.CommitElement(); // e
          }
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var tableName = info.GetString("TableName");

        var count = info.OpenArray("CalibrationTable");
        var array = new (double x_uncalibrated, double x_calibrated)[count];
        for (int i = 0; i < count; i++)
        {
          info.OpenElement();
          {
            array[i] = (info.GetDouble("x"), info.GetDouble("ys"));
          }
          info.CloseElement();
        }
        info.CloseArray(count);


        return new YCalibrationByDataSource()
        {
          TableName = string.IsNullOrEmpty(tableName) ? null : tableName,
          CalibrationTable = array.ToImmutableArray(),
        };
      }
    }
    #endregion


    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {

      var ux = CalibrationTable.Select(p => p.x).ToArray();
      var uy = CalibrationTable.Select(p => p.yScalingFactor).ToArray();

      var spline = new Altaxo.Calc.Interpolation.AkimaCubicSpline();
      spline.Interpolate(ux, uy);

      var yy = new double[y.Length];

      for (int i = 0; i < x.Length; ++i)
      {
        yy[i] = y[i] * spline.GetYOfX(x[i]);
      }
      return (x, yy, regions);
    }

    /// <inheritdoc/>
    IYCalibrationTable IYCalibrationTable.WithCalibrationTable(ImmutableArray<(double x, double yScalingFactor)> calibrationTable)
    {
      return this with { CalibrationTable = calibrationTable };
    }

    /// <inheritdoc/>
    IReferencingTable IReferencingTable.WithTableName(string tableName)
    {
      return this with { TableName = tableName };
    }
  }
}
