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

namespace Altaxo.Geometry
{


  /// <summary>
  /// Edge provides some common functions that apply to one of the
  /// edges of a rectangular area (left, right, bottom, top).
  /// </summary>
  [Serializable]
  public struct Edge
  {
    private EdgeType _styleType;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Edge"/>.
    /// 2015-11-15 Move to Altaxo.Geometry namespace.
    /// V2: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Edge", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.Edge", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Edge), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Edge)obj;
        info.AddValue("EdgeType", s._styleType);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var type = (EdgeType)info.GetValue("EdgeType", null);
        Edge s = (Edge?)o ?? new Edge(type);
        s._styleType = type;
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="Edge"/> struct.
    /// </summary>
    /// <param name="st">The edge type.</param>
    public Edge(EdgeType st)
    {
      _styleType = st;
    }

    /// <summary>
    /// Gets or sets the type of the edge.
    /// </summary>
    public EdgeType TypeOfEdge
    {
      get { return _styleType; }
      set { _styleType = value; }
    }

    /// <summary>
    /// Gets the origin point of the edge for the given layer size.
    /// </summary>
    /// <param name="layerSize">The size of the layer.</param>
    /// <returns>The origin point of the edge.</returns>
    public PointD2D GetOrg(VectorD2D layerSize)
    {
      switch (_styleType)
      {
        case EdgeType.Left:
          return new PointD2D(0, layerSize.Y);

        case EdgeType.Right:
          return new PointD2D(layerSize.X, layerSize.Y);

        case EdgeType.Bottom:
          return new PointD2D(0, layerSize.Y);

        case EdgeType.Top:
          return new PointD2D(0, 0);
      } // end of switch
      return new PointD2D(0, 0);
    }

    /// <summary>
    /// Gets the end point of the edge for the given layer size.
    /// </summary>
    /// <param name="layerSize">The size of the layer.</param>
    /// <returns>The end point of the edge.</returns>
    public PointD2D GetEnd(VectorD2D layerSize)
    {
      switch (_styleType)
      {
        case EdgeType.Left:
          return new PointD2D(0, 0);

        case EdgeType.Right:
          return new PointD2D(layerSize.X, 0);

        case EdgeType.Bottom:
          return new PointD2D(layerSize.X, layerSize.Y);

        case EdgeType.Top:
          return new PointD2D(layerSize.X, 0);
      } // end of switch
      return new PointD2D(0, 0);
    }

    /// <summary>
    /// Gets a point between two points at a relative position.
    /// </summary>
    /// <param name="p1">The first point.</param>
    /// <param name="p2">The second point.</param>
    /// <param name="rel">The relative position between p1 and p2 (0=start, 1=end).</param>
    /// <returns>The interpolated point.</returns>
    public static PointD2D GetPointBetween(PointD2D p1, PointD2D p2, double rel)
    {
      return new PointD2D((float)(p1.X + rel * (p2.X - p1.X)), (float)(p1.Y + rel * (p2.Y - p1.Y)));
    }

    /// <summary>
    /// Gets a point on the edge at a relative position.
    /// </summary>
    /// <param name="layerSize">The size of the layer.</param>
    /// <param name="rel">The relative position along the edge (0=start, 1=end).</param>
    /// <returns>The point on the edge.</returns>
    public PointD2D GetEdgePoint(VectorD2D layerSize, double rel)
    {
      switch (_styleType)
      {
        case EdgeType.Left:
          return new PointD2D(0, (float)((1 - rel) * layerSize.Y));

        case EdgeType.Right:
          return new PointD2D(layerSize.X, (float)((1 - rel) * layerSize.Y));

        case EdgeType.Bottom:
          return new PointD2D((float)(rel * layerSize.X), layerSize.Y);

        case EdgeType.Top:
          return new PointD2D((float)(rel * layerSize.X), 0);
      } // end of switch
      return new PointD2D(0, 0);
    }

    /// <summary>
    /// Gets the length of the edge for the given layer size.
    /// </summary>
    /// <param name="layerSize">The size of the layer.</param>
    /// <returns>The length of the edge.</returns>
    public double GetEdgeLength(VectorD2D layerSize)
    {
      switch (_styleType)
      {
        case EdgeType.Left:
        case EdgeType.Right:
          return layerSize.Y;

        case EdgeType.Bottom:
        case EdgeType.Top:
          return layerSize.X;
      } // end of switch
      return 0;
    }

    /// <summary>
    /// Gets the length of the edge opposite to this edge for the given layer size.
    /// </summary>
    /// <param name="layerSize">The size of the layer.</param>
    /// <returns>The length of the opposite edge.</returns>
    public double GetOppositeEdgeLength(VectorD2D layerSize)
    {
      switch (_styleType)
      {
        case EdgeType.Left:
        case EdgeType.Right:
          return layerSize.X;

        case EdgeType.Bottom:
        case EdgeType.Top:
          return layerSize.Y;
      } // end of switch
      return 0;
    }

    /// <summary>
    /// Gets the outward-pointing normal vector for this edge.
    /// </summary>
    public VectorD2D OuterVector
    {
      get
      {
        switch (_styleType)
        {
          case EdgeType.Left:
            return new VectorD2D(-1, 0);

          case EdgeType.Right:
            return new VectorD2D(1, 0);

          case EdgeType.Bottom:
            return new VectorD2D(0, 1);

          case EdgeType.Top:
            return new VectorD2D(0, -1);
        } // end of switch
        return new VectorD2D(0, 0);
      }
    }

    /// <summary>
    /// Gets the inward-pointing normal vector for this edge.
    /// </summary>
    public VectorD2D InnerVector
    {
      get
      {
        switch (_styleType)
        {
          case EdgeType.Left:
            return new VectorD2D(1, 0);

          case EdgeType.Right:
            return new VectorD2D(-1, 0);

          case EdgeType.Bottom:
            return new VectorD2D(0, -1);

          case EdgeType.Top:
            return new VectorD2D(0, 1);
        } // end of switch
        return new VectorD2D(0, 0);
      }
    }
  } // end of struct Edge
}
