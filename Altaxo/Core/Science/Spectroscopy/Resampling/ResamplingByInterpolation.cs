using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;
using Altaxo.Science.Spectroscopy.Cropping;

namespace Altaxo.Science.Spectroscopy.Resampling
{
  /// <summary>
  /// Resamples a function by using an interpolation function, and sampling points at which the interpolation function is evaluated.
  /// </summary>
  public record ResamplingByInterpolation : IResampling, ICropping, IImmutable
  {
    /// <summary>
    /// Gets the interpolation function to be used.
    /// </summary>
    public IInterpolationFunctionOptions Interpolation { get; init; } = new AkimaCubicSplineOptions();

    /// <summary>
    /// Gets the sampling points.
    /// </summary>
    IROVector<double> SamplingPoints { get; init; } = new LinearlySpacedIntervalByStartEndStep(0, 1, 1);

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ResamplingByInterpolation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ResamplingByInterpolation)obj;
        info.AddValue("Interpolation", s.Interpolation);
        info.AddValue("SamplingPoints", s.SamplingPoints);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var interpolation = info.GetValue<IInterpolationFunctionOptions>("Interpolation",null);
        var samplingPoints = info.GetValue<IROVector<double>>("SamplingPoints",null);
        return new ResamplingByInterpolation() { Interpolation = interpolation, SamplingPoints = samplingPoints };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var spline = Interpolation.Interpolate(x, y);

      var xx = new double[SamplingPoints.Length];
      var yy = new double[SamplingPoints.Length];

      for(int i=0;i<xx.Length;++i)
      {
        xx[i] = SamplingPoints[i];
        yy[i] = spline.GetYOfX(xx[i]);
      }

      return (xx, yy, null);
    }
  }
}
