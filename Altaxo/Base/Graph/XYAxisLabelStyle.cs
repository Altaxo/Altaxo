#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Serialization;
using Altaxo.Graph.Axes;
using Altaxo.Data;

namespace Altaxo.Graph
{
  using AxisLabeling;

  /// <summary>
  /// Summary description for AbstractLabelFormatting.
  /// </summary>
  public class XYAxisLabelStyle : AbstractXYAxisLabelStyle, ICloneable
  {
    protected Font m_Font = new Font(FontFamily.GenericSansSerif,18,GraphicsUnit.World);
    protected Edge m_Edge = new Edge(EdgeType.Left); 
    protected StringFormat strfmt;
    ILabelFormatting _labelFormatting = new AxisLabeling.NumericAxisLabelFormattingAuto();

    #region Serialization
    /// <summary>Used to serialize the XYAxisLabelStyle Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYAxisLabelStyle Version 0.
      /// </summary>
      /// <param name="obj">The XYAxisLabelStyle to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        XYAxisLabelStyle s = (XYAxisLabelStyle)obj;
        info.AddValue("Font",s.m_Font);  
        info.AddValue("Edge",s.m_Edge);  
      }
      /// <summary>
      /// Deserializes the XYAxisLabelStyle Version 0.
      /// </summary>
      /// <param name="obj">The empty XYAxisLabelStyle object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYAxisLabelStyle.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYAxisLabelStyle s = (XYAxisLabelStyle)obj;

        s.m_Font = (Font)info.GetValue("Font",typeof(Font));
        s.m_Edge = (Edge)info.GetValue("Edge",typeof(Edge));
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYAxisLabelStyle),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYAxisLabelStyle s = (XYAxisLabelStyle)obj;
        info.AddValue("Edge",s.m_Edge);  
        info.AddValue("Font",s.m_Font);  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYAxisLabelStyle s = null!=o ? (XYAxisLabelStyle)o : new XYAxisLabelStyle(EdgeType.Left);

        s.m_Edge = (Edge)info.GetValue("Edge",s);
        s.m_Font = (Font)info.GetValue("Font",s);

        return s;
      }
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
    }
    #endregion



    public XYAxisLabelStyle(EdgeType st)
    {
      m_Edge = new Edge(st);
    }

    public XYAxisLabelStyle(XYAxisLabelStyle from)
    {
      m_Edge = from.m_Edge;
      m_Font = null==m_Font ? null : (Font)m_Font.Clone();
    }

    public override object Clone()
    {
      return new XYAxisLabelStyle(this);
    }


    public override IHitTestObject HitTest(XYPlotLayer layer, PointF pt)
    {
      GraphicsPath gp = GetSelectionPath(layer);
      return gp.IsVisible(pt) ? new HitTestObject(gp,this) : null;
    }


    public void AdjustRectangle(ref RectangleF r, StringFormat fmt)
    {
      switch(fmt.LineAlignment)
      {
        case StringAlignment.Near:
          break;
        case StringAlignment.Center:
          r.Y -= 0.5f*r.Height;
          break;
        case StringAlignment.Far:
          r.Y -= r.Height;
          break;
      }
      switch(fmt.Alignment)
      {
        case StringAlignment.Near:
          break;
        case StringAlignment.Center:
          r.X -= 0.5f*r.Width;
          break;
        case StringAlignment.Far:
          r.X -= r.Width;
          break;
      }
    }
    /// <summary>
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public virtual GraphicsPath GetSelectionPath(XYPlotLayer layer)
    {
      GraphicsPath gp = new GraphicsPath();
      gp.AddRectangle(_enclosingRectangle);
      return gp;
    }


    private RectangleF _enclosingRectangle;
    public override void Paint(Graphics g, XYPlotLayer layer, Axis raxis, XYAxisStyle axisstyle)
    {
      SizeF layerSize = layer.Size;
      PointF orgP = m_Edge.GetOrg(layerSize);
      PointF endP = m_Edge.GetEnd(layerSize);
      PointF outVector = m_Edge.OuterVector;
      float dist_x = axisstyle.OuterDistance+axisstyle.GetOffset(layerSize); // Distance from axis tick point to label
      float dist_y = axisstyle.OuterDistance+axisstyle.GetOffset(layerSize); // y distance from axis tick point to label

      dist_x += this.m_Font.SizeInPoints/3; // add some space to the horizontal direction in order to separate the chars a little from the ticks

      // Modification of StringFormat is necessary to avoid 
      // too big spaces between successive words
      strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
      strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      // next statement is necessary to have a consistent string length both
      // on 0 degree rotated text and rotated text
      // without this statement, the text is fitted to the pixel grid, which
      // leads to "steps" during scaling
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      // set the alignment and line alignment of the strings
      switch(this.m_Edge.TypeOfEdge)
      {
        case EdgeType.Bottom:
          strfmt.LineAlignment = StringAlignment.Near;
          strfmt.Alignment     = StringAlignment.Center;
          dist_x = 0;
          break;
        case EdgeType.Top:
          strfmt.LineAlignment = StringAlignment.Far;
          strfmt.Alignment     = StringAlignment.Center;
          dist_x = 0;
          dist_y = -dist_y;
          break;
        case EdgeType.Left:
          strfmt.LineAlignment = StringAlignment.Center;
          strfmt.Alignment     = StringAlignment.Far;
          dist_x = -dist_x;
          dist_y = 0;
          break;
        case EdgeType.Right:
          strfmt.LineAlignment = StringAlignment.Center;
          strfmt.Alignment     = StringAlignment.Near;
          dist_y = 0;
          break;
      }

      double[] relpositions = raxis.GetMajorTicksNormal();
      AltaxoVariant[] ticks = raxis.GetMajorTicksAsVariant();
      IMeasuredLabelItem[] labels = _labelFormatting.GetMeasuredItems(g,m_Font,strfmt,ticks);

      _enclosingRectangle = RectangleF.Empty;
      for(int i=0;i<ticks.Length;i++)
      {
        double r = relpositions[i];
        PointF tickorg = m_Edge.GetEdgePoint(layerSize,r);

        PointF morg = new PointF(tickorg.X + dist_x, tickorg.Y + dist_y);
        SizeF msize = labels[i].Size;
        RectangleF mrect = new RectangleF(morg,msize);
        AdjustRectangle(ref mrect, strfmt);
        _enclosingRectangle = _enclosingRectangle.IsEmpty ? mrect : RectangleF.Union(_enclosingRectangle,mrect);
        labels[i].Draw(g,morg);
      }

    }
  

  }
}
