#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Base class that can be used to derive a numeric abel formatting class
  /// </summary>
  public abstract class NumericLabelFormattingBase : LabelFormattingBase
  {
    protected int _decimalPlaces;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LabelFormatting.AbstractNumericLabelFormatting", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingBase), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NumericLabelFormattingBase)obj;
        info.AddBaseValueEmbedded(s, typeof(LabelFormattingBase));
        info.AddValue("DecimalPlaces", s._decimalPlaces);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericLabelFormattingBase)(o ?? throw new ArgumentNullException(nameof(o)));
        info.GetBaseValueEmbedded(s, typeof(LabelFormattingBase), parent);
        s._decimalPlaces = info.GetInt32("DecimalPlaces");
        return s;
      }
    }

    #endregion Serialization

    protected NumericLabelFormattingBase()
    {
    }

    protected NumericLabelFormattingBase(NumericLabelFormattingBase from)
      : base(from) // everything is done here, since CopyFrom is virtual
    {
    }

    public override bool CopyFrom(object obj)
    {
      var isCopied = base.CopyFrom(obj);
      if (isCopied && !object.ReferenceEquals(this, obj))
      {
        var from = obj as NumericLabelFormattingBase;
        if (null != from)
        {
          _decimalPlaces = from._decimalPlaces;
        }
      }
      return isCopied;
    }
  }
}
