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
using System.ComponentModel;   
using System.Text;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// The type of the position values  (i.e. x and y position of the layer).
  /// </summary>
  [Serializable]
  public enum XYPlotLayerPositionType
  {
    /// <summary>
    /// The value is a absolute value (not relative) in points (1/72 inch).
    /// </summary>
    [Description("Absolute value (points)")]
    AbsoluteValue,


    /// <summary>
    /// The value is relative to the graph document. This means that for instance the x position of the layer
    /// is relative to the width of the graph document. A x value of 0 would position the layer at the left edge of the
    /// graph document, a value of 1 on the right edge of the graph.
    /// </summary>
    [Description("Relative to graph size (0..1)")]
    RelativeToGraphDocument,

    /// <summary>
    /// The value relates the near edge (either upper or left) of this layer to the near edge of the linked layer.
    /// </summary>
    /// <remarks> The values are relative to the size of the linked layer.
    /// This means that for instance for a x position value of 0 the left edges of both layers are on the same position, for a value of 1
    /// this means that the left edge of this layer is on the right edge of the linked layer.
    /// </remarks>
    RelativeThisNearToLinkedLayerNear,


    /// <summary>
    /// The value relates the near edge (either upper or left) of this layer to the far edge (either right or bottom) of the linked layer.
    /// </summary>
    /// <remarks> The values are relative to the size of the linked layer.
    /// This means that for instance for a x position value of 0 the left edges of this layer is on the right edge of the linked layer,
    /// for a value of 1
    /// this means that the left edge of this layer is one width away from the right edge of the linked layer.
    /// </remarks>
    RelativeThisNearToLinkedLayerFar,
    /// <summary>
    /// The value relates the far edge (either right or bottom) of this layer to the near edge (either left or top) of the linked layer.
    /// </summary>
    /// <remarks> The values are relative to the size of the linked layer.
    /// This means that for instance for a x position value of 0 the right edge of this layer is on the left edge of the linked layer,
    /// for a value of 1
    /// this means that the right edge of this layer is one width away (to the right) from the leftt edge of the linked layer.
    /// </remarks>
    RelativeThisFarToLinkedLayerNear,
    /// <summary>
    /// The value relates the far edge (either right or bottom) of this layer to the far edge (either right or bottom) of the linked layer.
    /// </summary>
    /// <remarks> The values are relative to the size of the linked layer.
    /// This means that for instance for a x position value of 0 the right edge of this layer is on the right edge of the linked layer,
    /// for a value of 1
    /// this means that the right edge of this layer is one width away from the right edge of the linked layer, for a x value of -1 this
    /// means that the right edge of this layer is one width away to the left from the right edge of the linked layer and this falls together
    /// with the left edge of the linked layer.
    /// </remarks>
    RelativeThisFarToLinkedLayerFar
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayer+PositionType",0)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerPositionType), 1)]
  public class PositionTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString());
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {

      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(XYPlotLayerPositionType), val, true);
    }
  }
}
