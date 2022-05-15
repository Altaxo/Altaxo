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
using System;

namespace Altaxo.Calc.Regression.Nonlinear
{

  /// <summary>
  /// This is a variance which scales linearly with the measured value. Useful for
  /// functions with a broad range of y-values. Make sure that no y-value is zero.
  /// </summary>
  public class RelativeVarianceScaling : IVarianceScaling
  {
    private double _scaling = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.RelativeVarianceScaling", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeVarianceScaling), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RelativeVarianceScaling)obj;

        info.AddValue("ScalingFactor", s._scaling);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        RelativeVarianceScaling s = (RelativeVarianceScaling?)o ?? new RelativeVarianceScaling();

        s._scaling = info.GetDouble("ScalingFactor");

        return s;
      }
    }

    #endregion Serialization

    public double GetWeight(double yr, int i)
    {
      if (yr == 0)
        return _scaling;
      else
        return _scaling / Math.Abs(yr);
    }

    public string ShortName
    {
      get { return "N1"; }
    }

    public object Clone()
    {
      var result = new RelativeVarianceScaling
      {
        _scaling = _scaling
      };
      return result;
    }
  }
}
