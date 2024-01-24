#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Contains the option for data output into the table containing the master curve.
  /// </summary>
  public record MasterCurveTableOutputOptions : Main.IImmutable
  {
    /// <summary>
    /// If true, the original curves that were used for creating the master curve are put into the table.
    /// </summary>
    public bool OutputOriginalCurves { get; init; } = true;

    /// <summary>
    /// If true, the curves with the shifted x-axis are put into the table.
    /// </summary>
    public bool OutputShiftedCurves { get; init; } = true;

    /// <summary>
    /// If true, the data from all shifted curves are merged, then sorted by x-values, and put into the table.
    /// </summary>
    public bool OutputMergedShiftedCurve { get; init; } = true;

    /// <summary>
    /// If true, the interpolated curve is put into the table.
    /// </summary>
    public bool OutputInterpolatedCurve { get; init; } = true;

    #region Serialization

    /// <summary>
    /// V0: 2024-01-24 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MasterCurveTableOutputOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MasterCurveTableOutputOptions)obj;

        info.AddValue("OriginalCurves", s.OutputOriginalCurves);
        info.AddValue("ShiftedCurves", s.OutputShiftedCurves);
        info.AddValue("MergedShiftedCurve", s.OutputMergedShiftedCurve);
        info.AddValue("InterpolatedCurve", s.OutputInterpolatedCurve);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var originalCurves = info.GetBoolean("OriginalCurves");
        var shiftedCurves = info.GetBoolean("ShiftedCurves");
        var mergedShiftedCurve = info.GetBoolean("MergedShiftedCurve");
        var interpolatedCurve = info.GetBoolean("InterpolatedCurve");

        return new MasterCurveTableOutputOptions()
        {
          OutputOriginalCurves = originalCurves,
          OutputShiftedCurves = shiftedCurves,
          OutputMergedShiftedCurve = mergedShiftedCurve,
          OutputInterpolatedCurve = interpolatedCurve,
        };
      }
    }

    #endregion
  }
}
