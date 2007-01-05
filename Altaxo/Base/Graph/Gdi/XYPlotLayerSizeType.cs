#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion
using System;
using System.Text;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// The type of the size (i.e. width and height values.
  /// </summary>
  [Serializable]
  public enum XYPlotLayerSizeType
  {
    /// <summary>
    ///  the value is a absolute value (not relative) in points (1/72 inch).
    /// </summary>
    AbsoluteValue,
    /// <summary>
    /// The value is relative to the graph document. This means that for instance the width of the layer
    /// is relative to the width of the graph document.
    /// </summary>
    RelativeToGraphDocument,
    /// <summary>
    /// The value is relative to the linked layer. This means that for instance the width of the layer
    /// is relative to the width of the linked layer.
    /// </summary>
    RelativeToLinkedLayer
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayer+SizeType", 0)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerSizeType", 1)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerSizeType), 2)]
  public class SizeTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString());
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {

      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(XYPlotLayerSizeType), val, true);
    }
  }

}
