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

  /// <summary>
  /// X-axis calibration that applies a correction curve derived from a referenced data source/table.
  /// </summary>
  public record XCalibrationByDataSource : IXCalibration, Main.IImmutable, IXCalibrationTable, IReferencingTable
  {
    /// <inheritdoc/>
    public string? TableName { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<(double x_uncalibrated, double x_calibrated)> CalibrationTable { get; init; }


    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="XCalibrationByDataSource"/>.
    /// </summary>
    /// <remarks>
    /// 2022-08-06 Initial version.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XCalibrationByDataSource), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XCalibrationByDataSource)obj;
        info.AddValue("TableName", s.TableName);
        info.CreateArray("CalibrationTable", s.CalibrationTable.Length);
        {
          foreach (var ele in s.CalibrationTable)
          {
            info.CreateElement("e");
            {
              info.AddValue("xu", ele.x_uncalibrated);
              info.AddValue("xc", ele.x_calibrated);
            }
            info.CommitElement(); // e
          }
        }
        info.CommitArray();
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var tableName = info.GetString("TableName");

        var count = info.OpenArray("CalibrationTable");
        var array = new (double x_uncalibrated, double x_calibrated)[count];
        for (int i = 0; i < count; i++)
        {
          info.OpenElement();
          {
            array[i] = (info.GetDouble("xu"), info.GetDouble("xc"));
          }
          info.CloseElement();
        }
        info.CloseArray(count);


        return new XCalibrationByDataSource()
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

      var ux = CalibrationTable.Select(p => p.x_uncalibrated).ToArray();
      var uy = CalibrationTable.Select(p => p.x_calibrated - p.x_uncalibrated).ToArray();

      var spline = new Altaxo.Calc.Interpolation.AkimaCubicSpline();
      spline.Interpolate(ux, uy);

      var xx = new double[x.Length];

      for (int i = 0; i < x.Length; ++i)
      {
        xx[i] = x[i] + spline.GetYOfX(x[i]);
      }
      return (xx, y, regions);
    }

    /// <inheritdoc/>
    IXCalibrationTable IXCalibrationTable.WithCalibrationTable(ImmutableArray<(double x_uncalibrated, double x_calibrated)> calibrationTable)
    {
      return this with { CalibrationTable = calibrationTable };
    }

    /// <inheritdoc/>
    IReferencingTable IReferencingTable.WithTableName(string tableName)
    {
      return this with { TableName = tableName };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name} Table={TableName}";
    }
  }
}
