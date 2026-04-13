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
  /// <summary>
  /// Adds a constant offset to a value.
  /// </summary>
  public class OffsetTransformation : IDoubleToDoubleTransformation
  {
    /// <summary>
    /// The transformations. The innermost (i.e. first transformation to carry out, the rightmost transformation) is located at index 0.
    /// </summary>
    private double _offset;

    #region Serialization

    /// <summary>
    /// 2016-06-27 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OffsetTransformation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OffsetTransformation)o;
        info.AddValue("Scale", s._offset);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var scale = info.GetDouble("Scale");
        return new OffsetTransformation(scale);
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public Type InputValueType { get { return typeof(double); } }

    /// <inheritdoc/>
    public Type OutputValueType { get { return typeof(double); } }

    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetTransformation"/> class with zero offset.
    /// </summary>
    public OffsetTransformation()
    {
      _offset = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OffsetTransformation"/> class.
    /// </summary>
    /// <param name="offset">The offset to add.</param>
    public OffsetTransformation(double offset)
    {
      _offset = offset;
    }

    /// <inheritdoc/>
    public AltaxoVariant Transform(AltaxoVariant value)
    {
      return _offset + value;
    }

    /// <inheritdoc/>
    public double Transform(double y)
    {
      return _offset + y;
    }

    /// <inheritdoc/>
    public (double ytrans, double dydxtrans) Derivative(double y, double dydx)
    {
      return (_offset + y, dydx);
    }


    /// <inheritdoc/>
    public string RepresentationAsFunction
    {
      get { return GetRepresentationAsFunction("x"); }
    }

    /// <inheritdoc/>
    public string GetRepresentationAsFunction(string arg)
    {
      return Altaxo.Serialization.GUIConversion.ToString(_offset) + " + " + arg;
    }

    /// <inheritdoc/>
    public string RepresentationAsOperator
    {
      get
      {
        return Altaxo.Serialization.GUIConversion.ToString(_offset) + " +";
      }
    }

    /// <inheritdoc/>
    public IVariantToVariantTransformation BackTransformation
    {
      get
      {
        return new OffsetTransformation(-_offset);
      }
    }

    /// <summary>
    /// Gets the offset added by the transformation.
    /// </summary>
    public double Offset
    {
      get
      {
        return _offset;
      }
    }

    /// <summary>
    /// Returns a transformation with the specified offset.
    /// </summary>
    /// <param name="offset">The offset to apply.</param>
    /// <returns>The current instance if the offset is unchanged; otherwise, a new instance.</returns>
    public OffsetTransformation WithOffset(double offset)
    {
      if (offset == _offset)
        return this;
      else
        return new OffsetTransformation(offset);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is OffsetTransformation from ? _offset == from._offset : false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return GetType().GetHashCode() + 17 * _offset.GetHashCode();
    }

    /// <inheritdoc/>
    public bool IsEditable { get { return true; } }
  }
}
