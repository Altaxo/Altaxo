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
using Altaxo.Graph.XYPlotScatterStyles;
using Altaxo.Serialization;

namespace Altaxo.Graph
{
  namespace XYPlotScatterStyles
  {
    [Serializable]
    public enum Shape 
    {
      NoSymbol,
      Square,
      Circle,
      UpTriangle,
      DownTriangle,
      Diamond,
      CrossPlus,
      CrossTimes,
      Star,
      BarHorz,
      BarVert
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Shape),0)]
    public class ShapeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(Shape),val,true);
      }
    }

    [Serializable]
    public enum Style
    {
      Solid,
      Open,
      DotCenter,
      Hollow,
      Plus,
      Times,
      BarHorz,
      BarVert
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Style),0)]
    public class StyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());  
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(Style),val,true);
      }
    }

    [Flags]
    [Serializable]
    public enum DropLine
    {
      NoDrop=0,
      Top=1,
      Bottom=2,
      Left=4,
      Right=8,
      All=Top|Bottom|Left|Right
    }
  
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DropLine),0)]
    public class DropLineXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(DropLine),val,true);
      }
    }
  
  } // end of class XYPlotScatterStyles



  [SerializationSurrogate(0,typeof(XYPlotScatterStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYPlotScatterStyle 
    : 
    I2DPlotStyle,
    ICloneable,
    Main.IChangedEventSource,
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChildChangedEventSink
  {
    protected XYPlotScatterStyles.Shape     m_Shape;
    protected XYPlotScatterStyles.Style     m_Style;
    protected XYPlotScatterStyles.DropLine  m_DropLine;
    protected PenHolder               m_Pen;
    protected float                   m_SymbolSize;
    protected float                   m_RelativePenWidth;

    // cached values:
    protected GraphicsPath m_Path;
    protected bool         m_bFillPath;
    protected BrushHolder  m_FillBrush;


    #region Serialization
    /// <summary>Used to serialize the XYPlotScatterStyle Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYPlotScatterStyle Version 0.
      /// </summary>
      /// <param name="obj">The XYPlotScatterStyle to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
        info.AddValue("Shape",s.m_Shape);  
        info.AddValue("Style",s.m_Style);  
        info.AddValue("DropLine",s.m_DropLine);
        info.AddValue("Pen",s.m_Pen);
        info.AddValue("SymbolSize",s.m_SymbolSize);
        info.AddValue("RelativePenWidth",s.m_RelativePenWidth);
      }
      /// <summary>
      /// Deserializes the XYPlotScatterStyle Version 0.
      /// </summary>
      /// <param name="obj">The empty XYPlotScatterStyle object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYPlotScatterStyle.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
        s.m_Shape = (XYPlotScatterStyles.Shape)info.GetValue("Shape",typeof(XYPlotScatterStyles.Shape));  
        s.m_Style = (XYPlotScatterStyles.Style)info.GetValue("Style",typeof(XYPlotScatterStyles.Style));  
        s.m_DropLine = (XYPlotScatterStyles.DropLine)info.GetValue("DropLine",typeof(XYPlotScatterStyles.DropLine));
        s.m_Pen = (PenHolder)info.GetValue("Pen",typeof(PenHolder));
        s.m_SymbolSize = info.GetSingle("SymbolSize");
        s.m_RelativePenWidth = info.GetSingle("RelativePenWidth");
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotScatterStyle),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotScatterStyle s = (XYPlotScatterStyle)obj;
        info.AddValue("Shape",s.m_Shape);  
        info.AddValue("Style",s.m_Style);  
        info.AddValue("DropLine",s.m_DropLine);
        info.AddValue("Pen",s.m_Pen);
        info.AddValue("SymbolSize",s.m_SymbolSize);
        info.AddValue("RelativePenWidth",s.m_RelativePenWidth);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYPlotScatterStyle s = null!=o ? (XYPlotScatterStyle)o : new XYPlotScatterStyle();

        s.m_Shape = (XYPlotScatterStyles.Shape)info.GetValue("Shape",typeof(XYPlotScatterStyles.Shape));  
        s.m_Style = (XYPlotScatterStyles.Style)info.GetValue("Style",typeof(XYPlotScatterStyles.Style));  
        s.m_DropLine = (XYPlotScatterStyles.DropLine)info.GetValue("DropLine",typeof(XYPlotScatterStyles.DropLine));
        s.m_Pen = (PenHolder)info.GetValue("Pen",typeof(PenHolder));
        s.m_SymbolSize = info.GetSingle("SymbolSize");
        s.m_RelativePenWidth = info.GetSingle("RelativePenWidth");

        // restore the cached values
        s.SetCachedValues();
        s.CreateEventChain();

        return s;
      }
    }
    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // restore the cached values
      SetCachedValues();
      CreateEventChain();
    }
    #endregion


    public void CopyFrom(XYPlotScatterStyle from, bool suppressChangeEvent)
    {
      this.m_Shape      = from.m_Shape;
      this.m_Style      = from.m_Style;
      this.m_DropLine   = from.m_DropLine;
      this.m_Pen        = null==from.m_Pen?null:(PenHolder)from.m_Pen.Clone();

      this.m_Path       = null==from.m_Path?null:(GraphicsPath)from.m_Path.Clone();
      this.m_bFillPath  = from.m_bFillPath;
      this.m_FillBrush  = null==from.m_FillBrush?null:(BrushHolder)from.m_FillBrush.Clone();
      this.m_SymbolSize = from.m_SymbolSize;
      this.m_RelativePenWidth = from.m_RelativePenWidth;

      if(!suppressChangeEvent)
        OnChanged();
    }

    public XYPlotScatterStyle(XYPlotScatterStyle from)
    {
      CopyFrom(from,true);
      CreateEventChain();
    }

    public XYPlotScatterStyle(XYPlotScatterStyles.Shape shape, XYPlotScatterStyles.Style style, float size, float penWidth, Color penColor)
    {
      m_Shape = shape;
      m_Style = style;
      m_DropLine = XYPlotScatterStyles.DropLine.NoDrop;
      m_Pen = new PenHolder(penColor,penWidth);
      m_SymbolSize = size;
      m_RelativePenWidth = penWidth/size;

      // Cached values
      SetCachedValues();
      CreateEventChain();
    }


    public XYPlotScatterStyle()
    {
      this.m_Shape = XYPlotScatterStyles.Shape.Square;
      this.m_Style = XYPlotScatterStyles.Style.Solid;
      this.m_DropLine = XYPlotScatterStyles.DropLine.NoDrop;
      this.m_Pen    = new PenHolder(Color.Black);
      this.m_SymbolSize = 8;
      this.m_RelativePenWidth = 0.1f;
      this.m_bFillPath = true; // since default is solid
      this.m_FillBrush = new BrushHolder(Color.Black);
      this.m_Path = GetPath(m_Shape,m_Style,m_SymbolSize);
      CreateEventChain();
    }

    protected void CreateEventChain()
    {
      if(null!=m_Pen)
        m_Pen.Changed += new EventHandler(this.EhChildChanged);
    }

    public void SetToNextStyle(XYPlotScatterStyle template)
    {
      SetToNextStyle(template, 1);
    }
    public void SetToNextStyle(XYPlotScatterStyle template, int step)
    {
      CopyFrom(template,true);
      // first increase the shape value,
      // if this is not possible set shape to first shape, and increase the
      // style value
      // note that the first member of the shape enum is NoSymbol, which should not be 
      // used here

      int nshapes = System.Enum.GetValues(typeof(XYPlotScatterStyles.Shape)).Length-1;
      int nstyles = System.Enum.GetValues(typeof(XYPlotScatterStyles.Style)).Length;

      int current = ((int)template.Style)*nshapes + ((int)template.Shape)-1;

      int next = Calc.BasicFunctions.PMod(current+step, nshapes*nstyles);

      int nstyle = Calc.BasicFunctions.PMod(next/nshapes,nstyles);
      int nshape = 1+Calc.BasicFunctions.PMod(next, nshapes);

      Shape = (XYPlotScatterStyles.Shape)nshape;
      Style = (XYPlotScatterStyles.Style)nstyle;
    }

    public XYPlotScatterStyles.Shape Shape
    {
      get { return this.m_Shape; }
      set
      {
        if(value!=this.m_Shape)
        {
          this.m_Shape = value;
          
          // ensure that a pen is set if Shape is other than nosymbol
          if(value!=XYPlotScatterStyles.Shape.NoSymbol && null==this.m_Pen)
            m_Pen = new PenHolder(Color.Black);

          SetCachedValues();

          OnChanged(); // Fire Changed event
        }
      }
    }





    public XYPlotScatterStyles.Style Style
    {
      get { return this.m_Style; }
      set 
      {
        if(value!=this.m_Style)
        {
          this.m_Style = value;
          SetCachedValues();

          OnChanged(); // Fire Changed event
        }
      }
    }

    public XYPlotScatterStyles.DropLine DropLine
    {
      get { return m_DropLine; }
      set 
      {
        if(m_DropLine!=value)
        {
          m_DropLine = value;
          OnChanged(); // Fire Changed event
        }
      }
    }

    public PenHolder Pen
    {
      get { return this.m_Pen; }
      set
      {
        // ensure pen can be only set to null if NoSymbol
        if(value!=null || XYPlotScatterStyles.Shape.NoSymbol==this.m_Shape)
        {
          m_Pen = null==value?null:(PenHolder)value.Clone();
          m_Pen.Changed += new EventHandler(this.EhChildChanged);
          OnChanged(); // Fire Changed event
        }
      }
    }


    public System.Drawing.Color Color
    {
      get { return this.m_Pen.Color; }
      set 
      {
        this.m_Pen.Color = value;
        this.m_FillBrush.SetSolidBrush( value );
        OnChanged(); // Fire Changed event
      }
    }

    public float SymbolSize
    {
      get { return m_SymbolSize; }
      set
      {
        if(value!=m_SymbolSize)
        {
          m_SymbolSize = value;
          m_Path = GetPath(this.m_Shape,this.m_Style,this.m_SymbolSize);
          m_Pen.Width = m_SymbolSize*m_RelativePenWidth;
          OnChanged(); // Fire Changed event
        }
      }
    }


    protected void SetCachedValues()
    {
      m_Path = GetPath(this.m_Shape,this.m_Style,this.m_SymbolSize);

      m_bFillPath = m_Style==XYPlotScatterStyles.Style.Solid || m_Style==XYPlotScatterStyles.Style.Open || m_Style==XYPlotScatterStyles.Style.DotCenter;
    
      if(this.m_Style!=XYPlotScatterStyles.Style.Solid)
        m_FillBrush = new BrushHolder(Color.White);
      else if(this.m_Pen.PenType==PenType.SolidColor)
        m_FillBrush = new BrushHolder(m_Pen.Color);
      else
        m_FillBrush = new BrushHolder(m_Pen.BrushHolder);
    }


    public void Paint(Graphics g)
    {
      if(m_bFillPath)
        g.FillPath(m_FillBrush,m_Path);

      g.DrawPath(m_Pen,m_Path);
    }

    public object Clone()
    {
      return new XYPlotScatterStyle(this);
    }
  
    public static GraphicsPath GetPath(XYPlotScatterStyles.Shape sh, XYPlotScatterStyles.Style st, float size)
    {
      float sizeh = size/2;
      GraphicsPath gp = new GraphicsPath();


      switch(sh)
      {
        case Shape.Square:
          gp.AddRectangle(new RectangleF(-sizeh,-sizeh,size,size));
          break;
        case Shape.Circle:
          gp.AddEllipse(-sizeh,-sizeh,size,size);
          break;
        case Shape.UpTriangle:
          gp.AddLine(0,-sizeh,0.3301270189f*size,0.5f*sizeh);
          gp.AddLine(0.43301270189f*size,0.5f*sizeh,-0.43301270189f*size,0.5f*sizeh);
          gp.CloseFigure();
          break;
        case Shape.DownTriangle:
          gp.AddLine(-0.43301270189f*sizeh,-0.5f*sizeh,0.43301270189f*size,-0.5f*sizeh);
          gp.AddLine(0.43301270189f*size,-0.5f*sizeh,0,sizeh);
          gp.CloseFigure();
          break;
        case Shape.Diamond:
          gp.AddLine(0,-sizeh,sizeh,0);
          gp.AddLine(sizeh,0,0,sizeh);
          gp.AddLine(0,sizeh,-sizeh,0);
          gp.CloseFigure();
          break;
        case Shape.CrossPlus:
          gp.AddLine(-sizeh,0,sizeh,0);
          gp.StartFigure();
          gp.AddLine(0,sizeh,0,-sizeh);
          gp.StartFigure();
          break;
        case Shape.CrossTimes:
          gp.AddLine(-sizeh,-sizeh,sizeh,sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh,sizeh,sizeh,-sizeh);
          gp.StartFigure();
          break;
        case Shape.Star:
          gp.AddLine(-sizeh,0,sizeh,0);
          gp.StartFigure();
          gp.AddLine(0,sizeh,0,-sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh,-sizeh,sizeh,sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh,sizeh,sizeh,-sizeh);
          gp.StartFigure();
          break;
        case Shape.BarHorz:
          gp.AddLine(-sizeh,0,sizeh,0);
          gp.StartFigure();
          break;
        case Shape.BarVert:
          gp.AddLine(0,-sizeh,0,sizeh);
          gp.StartFigure();
          break;
      }

      switch(st)
      {
        case Style.DotCenter:
          gp.AddEllipse(-0.125f*sizeh,-0.125f*sizeh,0.125f*size,0.125f*size);
          break;
        case Style.Plus:
          gp.AddLine(-sizeh,0,sizeh,0);
          gp.StartFigure();
          gp.AddLine(0,sizeh,0,-sizeh);
          gp.StartFigure();
          break;
        case Style.Times:
          gp.AddLine(-sizeh,-sizeh,sizeh,sizeh);
          gp.StartFigure();
          gp.AddLine(-sizeh,sizeh,sizeh,-sizeh);
          gp.StartFigure();
          break;
        case Style.BarHorz:
          gp.AddLine(-sizeh,0,sizeh,0);
          gp.StartFigure();
          break;
        case Style.BarVert:
          gp.AddLine(0,-sizeh,0,sizeh);
          gp.StartFigure();
          break;
      }
      return gp;
    }
    #region IChangedEventSource Members

    public event System.EventHandler Changed;

    protected virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this,new EventArgs());
    }

    #endregion

    #region IChildChangedEventSink Members

    public void EhChildChanged(object child, EventArgs e)
    {
      OnChildChanged(child, e);
    }

    public void OnChildChanged(object child, EventArgs e)
    {
      if(null!=Changed)
        Changed(this,e);
    }

    #endregion

    #region I2DPlotItemStyle Members

    public bool IsColorProvider
    {
      get { return true; }
    }

    public bool IsColorReceiver
    {
      get { return true; }
    }

    public bool IsSymbolSizeProvider
    {
      get 
      {
        return this.m_Shape != Shape.NoSymbol; 
      }
    }

    public bool IsSymbolSizeReceiver
    {
      get
      {
        return this.m_Shape != Shape.NoSymbol; ; 
      }
    }

   

    #endregion


    public void Paint(Graphics g, IPlotArea layer, PlotRangeList rangeList, PointF[] ptArray)
    {
      // paint the drop style
      if (this.DropLine != XYPlotScatterStyles.DropLine.NoDrop)
      {
        PenHolder ph = this.Pen;
        ph.Cached = true;
        Pen pen = ph.Pen; // do not dispose this pen, since it is cached
        //       float xe=layer.Size.Width;
        //       float ye=layer.Size.Height;

        double xleft, xright, ytop, ybottom;
        layer.LogicalToAreaConversion.Convert(0, 0, out xleft, out ybottom);
        layer.LogicalToAreaConversion.Convert(1, 1, out xright, out ytop);
        float xe = (float)xright;
        float ye = (float)ybottom;

        if ((0 != (this.DropLine & XYPlotScatterStyles.DropLine.Top)) && (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Bottom)))
        {
          for (int j = 0; j < ptArray.Length; j++)
          {
            float x = ptArray[j].X;
            g.DrawLine(pen, x, 0, x, ye);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Top))
        {
          for (int j = 0; j < ptArray.Length; j++)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, x, 0, x, y);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Bottom))
        {
          for (int j = 0; j < ptArray.Length; j++)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, x, y, x, ye);
          }
        }

        if ((0 != (this.DropLine & XYPlotScatterStyles.DropLine.Left)) && (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Right)))
        {
          for (int j = 0; j < ptArray.Length; j++)
          {
            float y = ptArray[j].Y;
            g.DrawLine(pen, 0, y, xe, y);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Right))
        {
          for (int j = 0; j < ptArray.Length; j++)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, x, y, xe, y);
          }
        }
        else if (0 != (this.DropLine & XYPlotScatterStyles.DropLine.Left))
        {
          for (int j = 0; j < ptArray.Length; j++)
          {
            float x = ptArray[j].X;
            float y = ptArray[j].Y;
            g.DrawLine(pen, 0, y, x, y);
          }
        }
      } // end paint the drop style


      // paint the scatter style
      if (this.Shape != XYPlotScatterStyles.Shape.NoSymbol)
      {
        // save the graphics stat since we have to translate the origin
        System.Drawing.Drawing2D.GraphicsState gs = g.Save();


        float xpos = 0, ypos = 0;
        float xdiff, ydiff;
        for (int j = 0; j < ptArray.Length; j++)
        {
          xdiff = ptArray[j].X - xpos;
          ydiff = ptArray[j].Y - ypos;
          xpos = ptArray[j].X;
          ypos = ptArray[j].Y;
          g.TranslateTransform(xdiff, ydiff);
          this.Paint(g);
        } // end for

        g.Restore(gs); // Restore the graphics state

      }
    }

}
}
