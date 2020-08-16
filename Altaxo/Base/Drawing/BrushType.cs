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

namespace Altaxo.Drawing
{
  /// <summary>
  /// Designates the type of the brush.
  /// </summary>
  [Serializable]
  public enum BrushType
  {
    /// <summary>Brush that has a single uniform color.</summary>
    SolidBrush,

    /// <summary></summary>
    HatchBrush,

    /// <summary>Brush using an image.</summary>
    TextureBrush,

    /// <summary>Brush that changes color along a linear path.</summary>
    LinearGradientBrush,

    /// <summary>Brush that changes color using a path (typically the path of the shape).</summary>
    PathGradientBrush,

    /// <summary>Brush that changes color with a sigma bell shaped function along a linear path.</summary>
    SigmaBellShapeLinearGradientBrush,

    /// <summary>Brush that changes color with a triangular shaped function along a linear path.</summary>
    TriangularShapeLinearGradientBrush,

    /// <summary>Brush that changes color with a sigma bell shaped function along a path (typically the path of the shape).</summary>
    SigmaBellShapePathGradientBrush,

    /// <summary>Brush that changes color with a triangular shaped function along a path (typically the path of the shape).</summary>
    TriangularShapePathGradientBrush,

    /// <summary>Brush that used a synthetic (calculated) texture.</summary>
    SyntheticTextureBrush,
  };

  /// <summary>
  /// 2020-03-30 Version2: Moved from Altaxo.Graph.Gdi namespace to Altaxo.Drawing namespace
  /// </summary>
  /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
  [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BrushType", 0)]
  [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.BrushType", 1)]
  [Serialization.Xml.XmlSerializationSurrogateFor(typeof(BrushType), 2)]
  public class BrushTypeXmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString() ?? string.Empty);
    }

    public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
    {
      var val = info.GetNodeContent();
      return Enum.Parse(typeof(BrushType), val, true);
    }
  }


} // end of namespace
