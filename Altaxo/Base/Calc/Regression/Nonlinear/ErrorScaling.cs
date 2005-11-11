using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Interface to how to scale the differences between real quantities (dependent variables) and fitted values.
  /// </summary>
  public interface IVarianceScaling : ICloneable
  {
    /// <summary>
    /// Gets the variance in dependence of the real data and the fitted data. Note: for nonlinear regression, the function must only depend on
    /// the real data. Set <c>yfit</c> to double.NaN in this case to make sure it is not used.
    /// </summary>
    /// <param name="yreal">The real (measured) data.</param>
    /// <param name="yfit">The fitted data.</param>
    /// <returns>The variance used to scale (yreal-yfit)²</returns>
    double GetVarianceScaling(double yreal, double yfit);
    string ShortName { get; }
  }

  /// <summary>
  /// The default scaling variance returns always 1.
  /// </summary>
  public class ConstantVarianceScaling : IVarianceScaling
  {
    double _scaling = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConstantVarianceScaling), 0)]
    public  class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ConstantVarianceScaling s = (ConstantVarianceScaling)obj;

        info.AddValue("ScalingFactor", s._scaling);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ConstantVarianceScaling s = o != null ? (ConstantVarianceScaling)o : new ConstantVarianceScaling();
        
        s._scaling = info.GetDouble("ScalingFactor");

        return s;
      }
    }

    #endregion

    public double GetVarianceScaling(double yr, double yf)
    {
      return _scaling;
    }

    public string ShortName
    {
      get { return "N2"; }
    }

    public object Clone()
    {
      ConstantVarianceScaling result = new ConstantVarianceScaling();
      result._scaling = this._scaling;
      return result;
    }
  }

  /// <summary>
  /// This is a variance which scales linearly with the measured value. Useful for
  /// functions with a broad range of y-values. Make sure that no y-value is zero.
  /// </summary>
  public class RelativeVarianceScaling : IVarianceScaling
  {
    double _scaling = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeVarianceScaling), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RelativeVarianceScaling s = (RelativeVarianceScaling)obj;

        info.AddValue("ScalingFactor", s._scaling);
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RelativeVarianceScaling s = o != null ? (RelativeVarianceScaling)o : new RelativeVarianceScaling();

        s._scaling = info.GetDouble("ScalingFactor");

        return s;
      }
    }

    #endregion

    public double GetVarianceScaling(double yr, double yf)
    {
      return _scaling*Math.Abs(yr);
    }

    public string ShortName
    {
      get { return "N1"; }
    }

    public object Clone()
    {
      RelativeVarianceScaling result = new RelativeVarianceScaling();
      result._scaling = this._scaling;
      return result;
    }

  }


}
