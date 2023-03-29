using System.Collections.Immutable;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  /// <summary>
  /// Options for spectroscopic intensity calibration by means of a light source with known intensity curve.
  /// </summary>
  public record IntensityCalibrationOptions
  {
    /// <summary>
    /// Gets the intensity curve of calibration source. This is usually a peak function, for instance a Gaussian shape with one or more terms (and baseline polynomial).
    /// </summary>
    public IFitFunction CurveShape { get; init; } = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude(1, -1);

    /// <summary>
    /// Gets the curve parameters for the <see cref="CurveShape"/> function.
    /// </summary>
    public ImmutableArray<(string Name, double Value)> CurveParameters { get; init; } = ImmutableArray<(string Name, double Value)>.Empty;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IntensityCalibrationOptions), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (IntensityCalibrationOptions)obj;
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


        return new IntensityCalibrationOptions
        {
          CurveShape = intensityCurve,
          CurveParameters = arr.ToImmutableArray(),
        };
      }
    }
    #endregion

  }
}
