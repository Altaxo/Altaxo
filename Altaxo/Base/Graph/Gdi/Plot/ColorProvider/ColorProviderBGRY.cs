using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{
  public class ColorProviderBGRY : ColorProviderBase
  {
    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorProviderBGRY), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ColorProviderBGRY s = (ColorProviderBGRY)obj;
        info.AddBaseValueEmbedded(s, typeof(ColorProviderBase));

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        ColorProviderBGRY s = null != o ? (ColorProviderBGRY)o : new ColorProviderBGRY();
        info.GetBaseValueEmbedded(s, typeof(ColorProviderBase), parent);

        return s;
      }
    }

    #endregion

    public override Color GetColor(double relVal)
    {
      const int ramp = 255 * 5;

      int val = 0;
      try
      {
        if (relVal >= 0 && relVal <= 1)
        {
          relVal = 0.1 + relVal * 0.8; // squeeze relVal a little not to have black at one end and white at the other
          if (relVal <= 0.4)
          {
            if (relVal <= 0.2)
            {
              val = (int)(relVal * ramp);
              return Color.FromArgb(0, 0, val);
            }
            else // 0.2<relVal<0.4
            {
              val = (int)((relVal - 0.2) * ramp);
              return Color.FromArgb(0, val, 255 - val);
            }
          }
          else if (relVal >= 0.6)
          {
            if (relVal >= 0.8)
            {
              val = (int)((relVal - 0.8) * ramp);
              return Color.FromArgb(255, 255, val);
            }
            else
            {
              val = (int)((relVal - 0.6) * ramp);
              return Color.FromArgb(255, val, 0);
            }
          }
          else
          {
            val = (int)((relVal - 0.4) * ramp);
            return Color.FromArgb(val, 255 - val, 0);
          }
        }
        else
          return GetOutOfBoundsColor(relVal);
      }
      catch (Exception ex)
      {
        return Color.Black;
      }
    }

    public override object Clone()
    {
      var result = new ColorProviderBGRY();
      result.CopyFrom(this);
      return result;
    }
  }

}
