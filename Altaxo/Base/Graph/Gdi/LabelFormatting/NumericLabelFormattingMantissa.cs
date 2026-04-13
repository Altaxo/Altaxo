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
using System.Collections.Generic;

namespace Altaxo.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Displays only the mantissa of a number. Usefull for minor ticks on logarithmic axes.
  /// </summary>
  public class NumericLabelFormattingMantissa : NumericLabelFormattingBase
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LabelFormatting.NumericLabelFormattingMantissa", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingMantissa), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NumericLabelFormattingMantissa)o;
        info.AddBaseValueEmbedded(s, typeof(NumericLabelFormattingBase));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericLabelFormattingMantissa?)o ?? new NumericLabelFormattingMantissa();
        info.GetBaseValueEmbedded(s, typeof(NumericLabelFormattingBase), parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericLabelFormattingMantissa"/> class.
    /// </summary>
    public NumericLabelFormattingMantissa()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericLabelFormattingMantissa"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public NumericLabelFormattingMantissa(NumericLabelFormattingMantissa from)
      : base(from) // everything is done here, since CopyFrom is virtual
    {
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new NumericLabelFormattingMantissa();
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <inheritdoc />
    protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
    {
      if (item.IsType(Altaxo.Data.AltaxoVariant.Content.VDouble))
        return FormatItem(item);
      else
        return item.ToString();
    }

    /// <summary>
    /// Formats a numeric value by returning only its mantissa.
    /// </summary>
    /// <param name="tick">The numeric value.</param>
    /// <returns>The formatted mantissa string.</returns>
    public string FormatItem(double tick)
    {
      string result = string.Format("{0:E0}", tick);
      int pos = result.IndexOf('E');
      if (pos >= 0)
        return result.Substring(0, pos);
      else
        return result;
    }
  }
}
