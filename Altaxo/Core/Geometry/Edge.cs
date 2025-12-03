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
  /// V1: 2015-11-15 Move to Altaxo.Geometry namespace.
  /// V2: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
  /// </summary>
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.EdgeType", 0)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.EdgeType", 1)]
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EdgeType), 2)]
  public class EdgeTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      info.SetNodeContent(obj.ToString() ?? "Left");
    }

    public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
    {
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(EdgeType), val, true);
    }
  }

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
    /// 2015-11-15 Move to Altaxo.Geometry namespace.
    /// V2: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Edge", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.Edge", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Edge), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Edge)obj;
        info.AddValue("EdgeType", s._styleType);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var type = (EdgeType)info.GetValue("EdgeType", null);
        Edge s = (Edge?)o ?? new Edge(type);
        s._styleType = type;
        return s;
      }
    }

    #endregion Serialization

    public Edge(EdgeType st)
    {
      _styleType = st;
    }

    public EdgeType TypeOfEdge
    {
      get { return _styleType; }
      set { _styleType = value; }
    }

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

    public static PointD2D GetPointBetween(PointD2D p1, PointD2D p2, double rel)
    {
      return new PointD2D((float)(p1.X + rel * (p2.X - p1.X)), (float)(p1.Y + rel * (p2.Y - p1.Y)));
    }

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
