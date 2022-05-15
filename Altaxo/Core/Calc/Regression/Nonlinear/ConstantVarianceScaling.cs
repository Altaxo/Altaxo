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

#nullable enable

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// The default scaling variance returns always 1.
  /// </summary>
  public class ConstantVarianceScaling : IVarianceScaling
  {
    private double _scaling = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.ConstantVarianceScaling", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConstantVarianceScaling), 1)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ConstantVarianceScaling)obj;

        info.AddValue("ScalingFactor", s._scaling);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ConstantVarianceScaling s = (ConstantVarianceScaling?)o ?? new ConstantVarianceScaling();

        s._scaling = info.GetDouble("ScalingFactor");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Returns true when the scaling factor is 1 (one).
    /// </summary>
    public bool IsDefault
    {
      get
      {
        return _scaling == 1;
      }
    }

    public double GetWeight(double yr, int i)
    {
      return _scaling;
    }

    public string ShortName
    {
      get { return "N2"; }
    }

    public object Clone()
    {
      var result = new ConstantVarianceScaling
      {
        _scaling = _scaling
      };
      return result;
    }
  }
}
