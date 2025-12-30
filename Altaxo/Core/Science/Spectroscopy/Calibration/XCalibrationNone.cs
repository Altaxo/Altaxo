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

namespace Altaxo.Science.Spectroscopy.Calibration
{
  /// <summary>
  /// Does nothing (null operation).
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.BaselineEstimation.IBaselineEstimation" />
  public class XCalibrationNone : IXCalibration
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="XCalibrationNone"/>.
    /// </summary>
    /// <remarks>
    /// 2023-03-30 Version 1: renaming to <see cref="XCalibrationNone"/>.
    /// </remarks>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Science.Spectroscopy.Calibration.CalibrationNone", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XCalibrationNone), 1)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new XCalibrationNone();
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      return (x, y, regions);
    }

    /// <summary>
    /// Executes the calibrator for baseline-style APIs; since this implementation is a no-op, the resulting baseline is set to zero.
    /// </summary>
    /// <param name="xArray">The x values of the spectrum.</param>
    /// <param name="yArray">The y values of the spectrum.</param>
    /// <param name="resultingBaseline">The resulting baseline to fill.</param>
    public void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> resultingBaseline)
    {
      for (int i = 0; i < resultingBaseline.Length; i++)
      {
        resultingBaseline[i] = 0;
      }
    }
  }
}
