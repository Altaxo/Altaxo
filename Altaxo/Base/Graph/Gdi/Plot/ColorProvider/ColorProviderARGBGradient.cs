using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{

  /// <summary>
  /// Interpolates linearly between two colors by linearly interpolate the A, the R, the G and the B value of the two colors.
  /// </summary>
  public class ColorProviderARGBGradient : ColorProviderBase
  {
    const double maxColorComponent = 255.999;

    double _alpha0 = 1;
    double _alpha1 = 1;
    double _red0 = 0;
    double _red1 = 1;
    double _green0 = 0;
    double _green1 = 1;
    double _blue0 = 0;
    double _blue1 = 1;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorProviderARGBGradient), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ColorProviderARGBGradient)obj;
        info.AddBaseValueEmbedded(s, typeof(ColorProviderBase));
        info.AddValue("Alpha0", s._alpha0);
        info.AddValue("Alpha1", s._alpha1);
        info.AddValue("Red0", s._red0);
        info.AddValue("Red1", s._red1);
        info.AddValue("Green0", s._green0);
        info.AddValue("Green1", s._green1);
        info.AddValue("Blue0", s._blue0);
        info.AddValue("Blue1", s._blue1);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        var s = null != o ? (ColorProviderARGBGradient)o : new ColorProviderARGBGradient();
        info.GetBaseValueEmbedded(s, typeof(ColorProviderBase), parent);
        s._alpha0 = info.GetDouble("Alpha0");
        s._alpha1 = info.GetDouble("Alpha1");
        s._red0 = info.GetDouble("Red0");
        s._red1 = info.GetDouble("Red1");
        s._green0 = info.GetDouble("Green0");
        s._green1 = info.GetDouble("Green1");
        s._blue0 = info.GetDouble("Blue0");
        s._blue1 = info.GetDouble("Blue1");

        return s;
      }
    }

    #endregion


    public override bool CopyFrom(object obj)
    {
      bool hasCopied = base.CopyFrom(obj);
      var from = obj as ColorProviderARGBGradient;
      if (null != from)
      {
        _alpha0 = from._alpha0;
        _alpha1 = from._alpha1;
        _red0 = from._red0;
        _red1 = from._red1;
        _green0 = from._green0;
        _green1 = from._green1;
        _blue0 = from._blue0;
        _blue1 = from._blue1;
       
        hasCopied = true;
      }
      return hasCopied;
    }

    public Color ColorAtR0
    {
      get
      {
        return GetColorFrom0To1Continuously(0);
      }
      set
      {
        double a, r, g, b;
        a = value.A / 255.0;
        r = value.R / 255.0;
        g = value.G / 255.0;
        b = value.B / 255.0;
        bool changed = a!=_alpha0 || r!=_red0 || g!=_green0 || b!=_blue0;
        _alpha0 = a;
        _red0 = r;
        _green0 = g;
        _blue0 = b;
        if(changed)
          OnChanged();
      }
    }

    public Color ColorAtR1
    {
      get
      {
        return GetColorFrom0To1Continuously(1);
      }
      set
      {
        double a, r, g, b;
        a = value.A / 255.0;
        r = value.R / 255.0;
        g = value.G / 255.0;
        b = value.B / 255.0;
        bool changed = a != _alpha1 || r != _red1 || g != _green1 || b != _blue1;
        _alpha1 = a;
        _red1 = r;
        _green1 = g;
        _blue1 = b;
        if (changed)
          OnChanged();
      }
    }


    /// <summary>
    /// Calculates a color from the provided relative value, that is guaranteed to be between 0 and 1
    /// </summary>
    /// <param name="relVal">Value used for color calculation. Guaranteed to be between 0 and 1.</param>
    /// <returns>A color associated with the relative value.</returns>
    protected override Color GetColorFrom0To1Continuously(double relVal)
    {

      double r0 = 1 - relVal;
      double r1 = relVal;

      return Color.FromArgb(
        (int)Math.Floor((_alpha0 * r0 + _alpha1 * r1)*(1-Transparency)*maxColorComponent),
        (int)Math.Floor((_red0 * r0 + _red1 * r1)*maxColorComponent),
        (int)Math.Floor((_green0 * r0 + _green1 * r1)*maxColorComponent),
        (int)Math.Floor((_blue0 * r0 + _blue1 * r1) * maxColorComponent)
        );
    }

    public override object Clone()
    {
      var result = new ColorProviderARGBGradient();
      result.CopyFrom(this);
      return result;
    }
  }

}
