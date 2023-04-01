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
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  /// <summary>
  /// Options for spectroscopic intensity calibration by means of a light source with known intensity curve.
  /// </summary>
  public record YCalibrationOptions
  {
    /// <summary>
    /// Gets the intensity curve of calibration source. This is usually a peak function, for instance a Gaussian shape with one or more terms (and baseline polynomial).
    /// </summary>
    public IFitFunction CurveShape { get; init; } = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, -1);

    /// <summary>
    /// Gets the curve parameters for the <see cref="CurveShape"/> function.
    /// </summary>
    public ImmutableArray<(string Name, double Value)> CurveParameters { get; init; } = ImmutableArray<(string Name, double Value)>.Empty;

    public SpectralPreprocessingOptionsBase Preprocessing { get; init; } = new SpectralPreprocessingOptions();

    #region Serialization

    /// <summary>
    /// 2023-03-30 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YCalibrationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (YCalibrationOptions)obj;
        info.AddValue("SpectralPreprocessing", s.Preprocessing);
        info.AddValue("CurveShape", s.CurveShape);

        info.CreateArray("CurveParameters", s.CurveParameters.Length);
        {
          for (int i = 0; i < s.CurveParameters.Length; i++)
          {
            info.CreateElement("e");
            {
              info.AddValue("Name", s.CurveParameters[i].Name);
              info.AddValue("Value", s.CurveParameters[i].Value);
            }
            info.CommitElement();
          }
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var spectralPreprocessing = info.GetValue<SpectralPreprocessingOptionsBase>("SpectralPreprocessing", parent);
        var intensityCurve = info.GetValue<IFitFunction>("CurveShape", null);

        var count = info.OpenArray("CurveParameters");
        var arr = new (string Name, double Value)[count];
        {
          for (int i = 0; i < count; ++i)
          {
            info.OpenElement(); // "e"
            var s = info.GetString("Name");
            var v = info.GetDouble("Value");
            info.CloseElement();
            arr[i] = (s, v);
          }
        }
        info.CloseArray(count);


        return new YCalibrationOptions
        {
          Preprocessing = spectralPreprocessing,
          CurveShape = intensityCurve,
          CurveParameters = arr.ToImmutableArray(),
        };
      }
    }
    #endregion

  }
}
