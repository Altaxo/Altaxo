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

namespace Altaxo.Graph
{

  [SerializationSurrogate(0,typeof(ShapeGraphic.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public abstract class ShapeGraphic : GraphicsObject
  {

    protected Color m_fillColor  = Color.White;
    protected Color m_lineColor  = Color.Black;
    protected float m_lineWidth = 1;
    protected bool m_fill  = false;

    #region Serialization
    /// <summary>Used to serialize the ShapeGraphic Version 0.</summary>
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes ShapeGraphic Version 0.
      /// </summary>
      /// <param name="obj">The ShapeGraphic to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        ShapeGraphic s = (ShapeGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss= AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
  
          // serialize the base class
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        info.AddValue("LineColor",s.m_lineColor);
        info.AddValue("LineWidth",s.m_lineWidth);

        info.AddValue("Fill",s.m_fill);
        info.AddValue("FillColor",s.m_fillColor);
      }
      /// <summary>
      /// Deserializes the ShapeGraphic Version 0.
      /// </summary>
      /// <param name="obj">The empty ShapeGraphic object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized ShapeGraphic.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        ShapeGraphic s = (ShapeGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // deserialize the base class
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        s.m_lineColor = (Color)info.GetValue("LineColor",typeof(Color));
        s.m_lineWidth = info.GetSingle("LineWidth");

        s.m_fill = info.GetBoolean("Fill");
        s.m_fillColor = (Color)info.GetValue("FillColor",typeof(Color));
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ShapeGraphic),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ShapeGraphic s = (ShapeGraphic)obj;
        info.AddBaseValueEmbedded(s,typeof(ShapeGraphic).BaseType);

        info.AddValue("LineColor",s.m_lineColor);
        info.AddValue("LineWidth",s.m_lineWidth);

        info.AddValue("Fill",s.m_fill);
        info.AddValue("FillColor",s.m_fillColor);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        ShapeGraphic s = (ShapeGraphic)o; 
        info.GetBaseValueEmbedded(s,typeof(ShapeGraphic).BaseType,parent);


        s.m_lineColor = (Color)info.GetValue("LineColor",s);
        s.m_lineWidth = info.GetSingle("LineWidth");

        s.m_fill = info.GetBoolean("Fill");
        s.m_fillColor = (Color)info.GetValue("FillColor",s);
        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }
    #endregion

    public ShapeGraphic()
    {
      m_fillColor  = Color.White;
      m_lineColor  = Color.Black;
      m_lineWidth = 1;
      m_fill  = false;
    }

    public ShapeGraphic(ShapeGraphic from)
      :
      base(from)
    {
      this.m_fillColor = from.m_fillColor;
      this.m_lineColor = from.m_lineColor;
      this.m_lineWidth = from.m_lineWidth;
      this.m_fill       = from.m_fill;
    }

    public virtual float LineWidth
    { 
      get
      {
        return m_lineWidth;
      }
      set
      {
        if(value > 0)
          m_lineWidth = value;
        else
          throw new ArgumentOutOfRangeException("LineWidth", "Line Width must be > 0");
      }
    }

    public virtual Color LineColor
    {
      get
      {
        return m_lineColor;
      }
      set
      {
        m_lineColor = value;
      }
    }

    public virtual  bool Fill
    {
      get
      {
        return m_fill;
      }
      set
      {
        m_fill = value;
      }
    }
    public virtual Color FillColor
    {
      get
      {
        return m_fillColor;
      }
      set
      {
        m_fillColor = value;
      }
    }
  } //  End Class


  [SerializationSurrogate(0,typeof(LineGraphic.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class LineGraphic : ShapeGraphic
  {
    #region Serialization
    /// <summary>Used to serialize the LineGraphic Version 0.</summary>
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes LineGraphic Version 0.
      /// </summary>
      /// <param name="obj">The LineGraphic to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        LineGraphic s = (LineGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
  
          // serialize the base class
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
      }
      /// <summary>
      /// Deserializes the LineGraphic Version 0.
      /// </summary>
      /// <param name="obj">The empty SLineGraphic object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized LineGraphic.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        LineGraphic s = (LineGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // deserialize the base class
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
  
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineGraphic),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LineGraphic s = (LineGraphic)obj;
        info.AddBaseValueEmbedded(s,typeof(LineGraphic).BaseType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        LineGraphic s = null!=o ? (LineGraphic)o : new LineGraphic(); 
        info.GetBaseValueEmbedded(s,typeof(LineGraphic).BaseType,parent);

        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }
    #endregion


    #region Constructors
    public LineGraphic()
    {
    }

    public LineGraphic(PointF startPosition)
    {
      this.SetStartPosition(startPosition);
    }

    public LineGraphic(float posX, float posY)
      : this(new PointF(posX, posY))
    {
    }

    public LineGraphic(PointF startPosition, PointF endPosition)
      :
      this(startPosition)
    {
      this.SetEndPosition(endPosition);
      this.AutoSize = false;
    }


    public LineGraphic(float startX, float startY, PointF endPosition)
      :
      this(new PointF(startX, startY), endPosition)
    {
    }

    public LineGraphic(float startX, float startY, float endX, float endY)
      :
      this(new PointF(startX, startY), new PointF(endX, endY))
    {
    }

    public LineGraphic(PointF startPosition, PointF endPosition, float lineWidth, Color lineColor)
      :
      this(startPosition)
    {
      this.SetEndPosition(endPosition);
      this.LineWidth = lineWidth;
      this.LineColor = lineColor;
      this.AutoSize = false;
    }

    public LineGraphic(float startX, float startY, float endX, float endY, float lineWidth, Color lineColor)
      :
      this(new PointF(startX, startY), new PointF(endX, endY))
    {
      this.LineWidth = lineWidth;
      this.LineColor = lineColor;
      this.AutoSize = false;
    }

    public LineGraphic(LineGraphic from)
      : base(from)
    {
    }

    #endregion

    public override object Clone()
    {
      return new LineGraphic(this);
    }


    public override IHitTestObject HitTest(PointF pt)
    {
      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();
      Pen myPen = new Pen(this.LineColor, this.LineWidth);
      gp.AddLine(0, 0, Width, Height);
      myMatrix.Translate(X, Y);
      myMatrix.Rotate(this.Rotation);
      gp.Transform(myMatrix);
      return gp.IsOutlineVisible(pt, myPen) ? new HitTestObject(gp,this) : null;
    }


    public PointF GetStartPosition()
    {
      return this.GetPosition();
    }

    public void SetStartPosition(PointF Value)
    {
      this.SetPosition(Value);
    }


    public PointF GetEndPosition()
    {
      PointF endPosition = new PointF(this.m_Position.X, this.m_Position.Y);
      endPosition.X += this.Size.Width;
      endPosition.Y += this.Size.Height;
      return endPosition;
    }

    public void SetEndPosition(PointF Value)
    {
      SizeF siz = new SizeF(
        Value.X - this.m_Position.X,
        Value.Y - this.m_Position.Y);
      SetSize(siz);
    }

    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      g.RotateTransform(this.m_Rotation);
      Pen myPen = new Pen(this.LineColor, this.LineWidth);
      g.DrawLine(myPen, X, Y, X + Width, Y + Height);
      g.Restore(gs);
    }

  } // End Class


  [SerializationSurrogate(0,typeof(RectangleGraphic.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class RectangleGraphic : ShapeGraphic
  {
    #region Serialization
    /// <summary>Used to serialize the RectangleGraphic Version 0.</summary>
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes RectangleGraphic Version 0.
      /// </summary>
      /// <param name="obj">The RectangleGraphic to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        RectangleGraphic s = (RectangleGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
  
          // serialize the base class
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
      
      }
      /// <summary>
      /// Deserializes the RectangleGraphic Version 0.
      /// </summary>
      /// <param name="obj">The empty RectangleGraphic object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized RectangleGraphic.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        RectangleGraphic s = (RectangleGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // deserialize the base class
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
      
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleGraphic),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RectangleGraphic s = (RectangleGraphic)obj;
        info.AddBaseValueEmbedded(s,typeof(RectangleGraphic).BaseType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        RectangleGraphic s = null!=o ? (RectangleGraphic)o : new RectangleGraphic(); 
        info.GetBaseValueEmbedded(s,typeof(RectangleGraphic).BaseType,parent);

        return s;
      }
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }
    #endregion


    #region Constructors
    public RectangleGraphic()
    {
    }

    public RectangleGraphic( PointF graphicPosition)
      :
      this()
    {
      this.SetPosition(graphicPosition);
    }

    public RectangleGraphic( float posX, float posY)
      :
      this(new PointF(posX, posY))
    {
    }
      

    public RectangleGraphic( PointF graphicPosition , SizeF graphicSize)
      :
      this(graphicPosition)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public RectangleGraphic( float posX , float posY, SizeF graphicSize)
      :
      this(new PointF(posX, posY), graphicSize)
    {
    }

    public RectangleGraphic( float posX , float posY , float width , float height )
      :
      this(new PointF(posX, posY), new SizeF(width, height))
    {
    }

    public RectangleGraphic( PointF graphicPosition, float Rotation)
      :
      this()
    {

      this.SetPosition(graphicPosition);
      this.Rotation = Rotation;
    }

    public RectangleGraphic( float posX , float posY , float Rotation )
      :
      this(new PointF(posX, posY), Rotation)
    {
    }

    public RectangleGraphic( PointF graphicPosition , SizeF graphicSize , float Rotation )
      :
      this(graphicPosition, Rotation)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }
    
    public RectangleGraphic( float posX , float posY , SizeF graphicSize , float Rotation)
      :
      this(new PointF(posX, posY), graphicSize, Rotation)
    {
    }

    public RectangleGraphic( float posX , float posY , float width , float height , float Rotation)
      :
      this(new PointF(posX, posY), new SizeF(width, height), Rotation)
    {
    }

    public RectangleGraphic(RectangleGraphic from)
      :
      base(from)
    {
    }

    #endregion

    public override object Clone()
    {
      return new RectangleGraphic(this);
    }


    public override void Paint( Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X,Y);
      if(m_Rotation !=- 0)
        g.RotateTransform(m_Rotation);
      RectangleF rect = new RectangleF(X, Y, Width, Height);
      if( this.Fill)
      {
        g.FillRectangle(new SolidBrush(this.FillColor), rect);
      }
      Pen myPen = new Pen(this.LineColor, this.LineWidth);
      g.DrawRectangle(myPen, rect.X,rect.Y,rect.Width,rect.Height);
      g.Restore(gs);
    }
    
  } // End Class

  
  [SerializationSurrogate(0,typeof(EllipseGraphic.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class EllipseGraphic : ShapeGraphic
  {

    #region Serialization
    /// <summary>Used to serialize the EllipseGraphic Version 0.</summary>
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes EllipseGraphic Version 0.
      /// </summary>
      /// <param name="obj">The EllipseGraphic to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        EllipseGraphic s = (EllipseGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
  
          // serialize the base class
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        
      }
      /// <summary>
      /// Deserializes the EllipseGraphic Version 0.
      /// </summary>
      /// <param name="obj">The empty EllipseGraphic object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized EllipseGraphic.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        EllipseGraphic s = (EllipseGraphic)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // deserialize the base class
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EllipseGraphic),0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        EllipseGraphic s = (EllipseGraphic)obj;
        info.AddBaseValueEmbedded(s,typeof(EllipseGraphic).BaseType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        EllipseGraphic s = null!=o ? (EllipseGraphic)o : new EllipseGraphic(); 
        info.GetBaseValueEmbedded(s,typeof(EllipseGraphic).BaseType,parent);

        return s;
      }
    }
    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }
    #endregion


    #region Constructors
  
    public EllipseGraphic()
    {
    }

    public EllipseGraphic( PointF graphicPosition)
      :
      this()
    {
      this.SetPosition(graphicPosition);
    }
    public EllipseGraphic( float posX, float posY)
      :
      this(new PointF(posX, posY))
    {
    }

    public EllipseGraphic( PointF graphicPosition, SizeF graphicSize)
      :
      this(graphicPosition)
    {

      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public EllipseGraphic( float posX, float posY , SizeF graphicSize)
      :
      this(new PointF(posX, posY), graphicSize)
    {
    }

    public EllipseGraphic( float posX , float posY , float width , float height )
      :
      this(new PointF(posX, posY), new SizeF(width, height))
    {
    }
  
    public EllipseGraphic( PointF graphicPosition, float Rotation)
      :
      this()
    {
      this.SetPosition(graphicPosition);
      this.Rotation = Rotation;
    }

    public EllipseGraphic( float posX , float posY , float Rotation)
      :
      this(new PointF(posX, posY), Rotation)
    {
    }

    public EllipseGraphic( PointF graphicPosition , SizeF graphicSize, float Rotation)
      :
      this(graphicPosition, Rotation)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public EllipseGraphic( float posX , float posY , SizeF graphicSize , float Rotation)
      :
      this(new PointF(posX, posY), graphicSize, Rotation)
    {
    }

    public EllipseGraphic( float posX, float posY, float width, float height, float Rotation)
      :
      this(new PointF(posX, posY), new SizeF(width, height), Rotation)
    {
    }

    public EllipseGraphic(EllipseGraphic from)
      :
      base(from)
    {
    }

    #endregion

    public override object Clone()
    {
      return new EllipseGraphic(this);
    }


    public override void Paint( Graphics g, object obj )
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X,Y);
      if( m_Rotation != 0)
        g.RotateTransform(m_Rotation);

      RectangleF rect = new RectangleF(X, Y, Width, Height);
      if( this.Fill )
        g.FillEllipse(new SolidBrush(this.FillColor), rect);

      Pen myPen = new Pen(this.LineColor, this.LineWidth);
      g.DrawEllipse(myPen, rect);
      g.Restore(gs);
    }
  } // end class

} // end Namespace
