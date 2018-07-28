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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data.Transformations
{
  public class OffsetTransformation : IVariantToVariantTransformation
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
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OffsetTransformation)obj;
        info.AddValue("Scale", s._offset);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
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

    public OffsetTransformation()
    {
      _offset = 0;
    }

    public OffsetTransformation(double offset)
    {
      _offset = offset;
    }

    public AltaxoVariant Transform(AltaxoVariant value)
    {
      return _offset + value;
    }

    public string RepresentationAsFunction
    {
      get { return GetRepresentationAsFunction("x"); }
    }

    public string GetRepresentationAsFunction(string arg)
    {
      return Altaxo.Serialization.GUIConversion.ToString(_offset) + " + " + arg;
    }

    public string RepresentationAsOperator
    {
      get
      {
        return Altaxo.Serialization.GUIConversion.ToString(_offset) + " +";
      }
    }

    public IVariantToVariantTransformation BackTransformation
    {
      get
      {
        return new OffsetTransformation(-_offset);
      }
    }

    public double Offset
    {
      get
      {
        return _offset;
      }
    }

    public OffsetTransformation WithOffset(double offset)
    {
      if (offset == _offset)
        return this;
      else
        return new OffsetTransformation(offset);
    }

    public override bool Equals(object obj)
    {
      var from = obj as OffsetTransformation;
      if (null == from)
        return false;

      if (this._offset != from._offset)
        return false;

      return true;
    }

    public override int GetHashCode()
    {
      return this.GetType().GetHashCode() + 17 * _offset.GetHashCode();
    }

    public bool IsEditable { get { return true; } }
  }
}
