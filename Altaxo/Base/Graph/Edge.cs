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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
  /// <summary>
  /// Designates the 4 edges of a rectangular area.
  /// </summary>
  [Serializable]
  public enum EdgeType { Left=0, Bottom=1, Right=2, Top=3 }


  
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EdgeType),0)]
  public class EdgeTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString()); 
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(EdgeType),val,true);
    }
  }


  /// <summary>
  /// Edge provides some common functions that apply to one of the
  /// edges of a rectangular area (left, right, bottom, top).
  /// </summary>
  [Serializable]
  public struct Edge
  {
    private EdgeType m_StyleType;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Edge),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Edge s = (Edge)obj;
        info.AddValue("EdgeType",s.m_StyleType);  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        
        EdgeType type = (EdgeType)info.GetValue("EdgeType",null);
        Edge s = null!=o ? (Edge)o : new Edge(type);
        return s;
      }
    }
    #endregion


    public Edge(EdgeType st)
    {
      m_StyleType = st;
    }

    public EdgeType TypeOfEdge
    {
      get { return m_StyleType; }
      set { m_StyleType = value; }
    }

    public PointF GetOrg(SizeF layerSize)
    {
      switch(m_StyleType)
      {
        case EdgeType.Left:
          return new PointF(0,layerSize.Height);
          
        case EdgeType.Right:
          return new PointF(layerSize.Width,layerSize.Height);
          
        case EdgeType.Bottom:
          return new PointF(0,layerSize.Height);
          
        case EdgeType.Top:
          return new PointF(0,0);
          
      } // end of switch
      return new PointF(0,0);
    }

    public PointF GetEnd(SizeF layerSize)
    {
      switch(m_StyleType)
      {
        case EdgeType.Left:
          return new PointF(0,0);
          
        case EdgeType.Right:
          return new PointF(layerSize.Width,0);
          
        case EdgeType.Bottom:
          return new PointF(layerSize.Width,layerSize.Height);
          
        case EdgeType.Top:
          return new PointF(layerSize.Width,0);
          
      } // end of switch
      return new PointF(0,0);
    }


    public static PointF GetPointBetween(PointF p1, PointF p2, double rel)
    {
      return new PointF((float)(p1.X+rel*(p2.X-p1.X)),(float)(p1.Y+rel*(p2.Y-p1.Y)));
    }

    public PointF GetEdgePoint(SizeF layerSize, double rel)
    {
      switch(m_StyleType)
      {
        case EdgeType.Left:
          return new PointF(0,(float)((1-rel)*layerSize.Height));
          
        case EdgeType.Right:
          return new PointF(layerSize.Width,(float)((1-rel)*layerSize.Height));
          
        case EdgeType.Bottom:
          return new PointF((float)(rel*layerSize.Width),layerSize.Height);
        
        case EdgeType.Top:
          return new PointF((float)(rel*layerSize.Width),0);
      
      } // end of switch
      return new PointF(0,0);
  
    }

    public float GetEdgeLength(SizeF layerSize)
    {
      switch(m_StyleType)
      {
        case EdgeType.Left:
        case EdgeType.Right:
          return layerSize.Height;
          
        case EdgeType.Bottom:
        case EdgeType.Top:
          return layerSize.Width;
      
      } // end of switch
      return 0;
    }

    public float GetOppositeEdgeLength(SizeF layerSize)
    {
      switch(m_StyleType)
      {
        case EdgeType.Left:
        case EdgeType.Right:
          return layerSize.Width;
          
        case EdgeType.Bottom:
        case EdgeType.Top:
          return layerSize.Height;
      } // end of switch
      return 0;
    }

    public PointF OuterVector
    {
      get
      {
        switch(m_StyleType)
        {
          case EdgeType.Left:
            return new PointF(-1,0);
            
          case EdgeType.Right:
            return new PointF(1,0);
          
          case EdgeType.Bottom:
            return new PointF(0,1);
          
          case EdgeType.Top:
            return new PointF(0,-1);
          
        } // end of switch
        return new PointF(0,0);
      }
    }

    public PointF InnerVector
    {
      get
      {
        switch(m_StyleType)
        {
          case EdgeType.Left:
            return new PointF(1,0);
            
          case EdgeType.Right:
            return new PointF(-1,0);
            
          case EdgeType.Bottom:
            return new PointF(0,-1);
            
          case EdgeType.Top:
            return new PointF(0,1);
          
        } // end of switch
        return new PointF(0,0);
      }
    }
  
  } // end of struct Edge
}
