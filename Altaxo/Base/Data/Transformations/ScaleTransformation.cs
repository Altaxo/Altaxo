#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Data.Transformations
{
  public class ScaleTransformation : IDoubleToDoubleTransformation
  {
    /// <summary>
    /// The transformations. The innermost (i.e. first transformation to carry out, the rightmost transformation) is located at index 0.
    /// </summary>
    private double _scale;

    #region Serialization

    /// <summary>
    /// 2016-06-27 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaleTransformation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScaleTransformation)obj;
        info.AddValue("Scale", s._scale);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var scale = info.GetDouble("Scale");
        return new ScaleTransformation(scale);
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public Type InputValueType { get { return typeof(double); } }

    /// <inheritdoc/>
    public Type OutputValueType { get { return typeof(double); } }

    public ScaleTransformation()
    {
      _scale = 1;
    }

    public ScaleTransformation(double scale)
    {
      _scale = scale;
    }

    public AltaxoVariant Transform(AltaxoVariant value)
    {
      return _scale * value;
    }

    public double Transform(double value)
    {
      return _scale * value;
    }

    /// <inheritdoc/>
    public (double ytrans, double dydxtrans) Derivative(double y, double dydx)
    {
      return (_scale * y, _scale * dydx);
    }

    public string RepresentationAsFunction
    {
      get { return GetRepresentationAsFunction("x"); }
    }

    public string GetRepresentationAsFunction(string arg)
    {
      return Altaxo.Serialization.GUIConversion.ToString(_scale) + " * " + arg;
    }

    public string RepresentationAsOperator
    {
      get
      {
        return Altaxo.Serialization.GUIConversion.ToString(_scale) + " * ";
      }
    }

    public IVariantToVariantTransformation BackTransformation
    {
      get
      {
        return new ScaleTransformation(1 / _scale);
      }
    }

    public double Scale
    {
      get
      {
        return _scale;
      }
    }

    public ScaleTransformation WithScale(double scale)
    {
      if (scale == _scale)
        return this;
      else
        return new ScaleTransformation(scale);
    }

    public override bool Equals(object? obj)
    {
      return obj is ScaleTransformation from ? _scale == from._scale : false;
    }

    public override int GetHashCode()
    {
      return GetType().GetHashCode() + 17 * _scale.GetHashCode();
    }

    public bool IsEditable { get { return true; } }
  }
}
