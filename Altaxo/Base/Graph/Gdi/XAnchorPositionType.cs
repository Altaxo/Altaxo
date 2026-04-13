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
using System.Text;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Specifies the horizontal anchor position.
  /// </summary>
  [Serializable]
  public enum XAnchorPositionType
  {
    /// <summary>
    /// Anchor at the left side.
    /// </summary>
    Left,
    /// <summary>
    /// Anchor at the center.
    /// </summary>
    Center,
    /// <summary>
    /// Anchor at the right side.
    /// </summary>
    Right
  }

  /// <summary>
  /// XML serialization surrogate for <see cref="XAnchorPositionType"/>.
  /// </summary>
  /// <remarks>2015-11-14 Version 1: moved to the <c>Altaxo.Graph.Gdi</c> namespace.</remarks>
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.TextGraphics+XAnchorPositionType", 0)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XAnchorPositionType", 0)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XAnchorPositionType), 1)]
  public class XAnchorPositionTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    /// <inheritdoc/>
    public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var s = (XAnchorPositionType)o;
      info.SetNodeContent(s.ToString());
    }

    /// <inheritdoc/>
    public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
    {
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(XAnchorPositionType), val, true);
    }
  }
}
