#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.G2D.Shapes
{

  #region ShapeGraphic

  [Serializable]
  public abstract class ShapeGraphic : GraphicsObject
  {
    protected BrushHolder m_fillBrush;
    protected PenHolder m_linePen;

    #region Serialization

    #region Clipboard serialization

    protected ShapeGraphic(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      ShapeGraphic s = this;
      base.GetObjectData(info, context);

     info.AddValue("LinePen", s.m_linePen);
     info.AddValue("FillBrush", s.m_fillBrush);

    }
    public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      ShapeGraphic s = (ShapeGraphic)base.SetObjectData(obj, info, context, selector);

      s.Pen = (PenHolder)info.GetValue("LinePen", typeof(PenHolder));
      s.Brush = (BrushHolder)info.GetValue("FillBrush", typeof(BrushHolder));

      return s;
    } // end of SetObjectData

    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }

    #endregion


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ShapeGraphic), 0)]
    public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ShapeGraphic s = (ShapeGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(ShapeGraphic).BaseType);

        info.AddValue("LinePen", s.m_linePen);
        info.AddValue("Fill", s.m_fillBrush.IsVisible);
        info.AddValue("FillBrush", s.m_fillBrush);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        ShapeGraphic s = (ShapeGraphic)o;
        info.GetBaseValueEmbedded(s, typeof(ShapeGraphic).BaseType, parent);


        s.Pen = (PenHolder)info.GetValue("LinePen", s);
        bool fill = info.GetBoolean("Fill");
        s.Brush = (BrushHolder)info.GetValue("FillBrush", s);
        return s;
      }
    }


    #endregion

    public ShapeGraphic()
    {
      Brush = new BrushHolder(Color.Transparent);
      Pen = new PenHolder(Color.Black);
    }

    public ShapeGraphic(ShapeGraphic from)
      :
      base(from)
    {
      this.m_fillBrush = (BrushHolder)from.m_fillBrush.Clone();
      this.m_linePen = (PenHolder)from.m_linePen.Clone();
    }
   

    public virtual PenHolder Pen
    {
      get
      {
        return m_linePen;
      }
      set
      {
        if (value == null)
           throw new ArgumentNullException("The line pen must not be null");

         if (m_linePen != null)
           m_linePen.Changed -= this.EhChildChanged;


        m_linePen = (PenHolder)value.Clone();
        m_linePen.Changed += this.EhChildChanged;
        OnChanged();
         
      }
    }

    public virtual BrushHolder Brush
    {
      get
      {
        return m_fillBrush;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("The fill brush must not be null");

        if (m_fillBrush != null)
          m_fillBrush.Changed -= this.EhChildChanged;


       
        m_fillBrush = (BrushHolder)value.Clone();
        m_fillBrush.Changed += this.EhChildChanged;
        OnChanged();

       
      }
    }

     
    public override IHitTestObject HitTest(PointF pt)
    {
      IHitTestObject result = base.HitTest(pt);
      if(result!=null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Line properties");
      ((ShapeGraphic)hitted).OnChanged();
      return true;
    }


  } //  End Class

  #endregion

  #region LineGraphic

  [Serializable]
  public class LineGraphic : ShapeGraphic, IGrippableObject
  {
    #region Serialization

    #region Clipboard serialization

    protected LineGraphic(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    /// <summary>
    /// Serializes LineGraphic. 
    /// </summary>
    /// <param name="obj">The LineGraphic to serialize.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      LineGraphic s = this;
      base.GetObjectData(info, context);
    }
    /// <summary>
    /// Deserializes the LineGraphic Version 0.
    /// </summary>
    /// <param name="obj">The empty SLineGraphic object to deserialize into.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    /// <param name="selector">The deserialization surrogate selector.</param>
    /// <returns>The deserialized LineGraphic.</returns>
    public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      LineGraphic s = (LineGraphic)base.SetObjectData(obj, info, context, selector);
      return s;
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }
    #endregion

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineGraphic), 0)]
    public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LineGraphic s = (LineGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(LineGraphic).BaseType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        LineGraphic s = null != o ? (LineGraphic)o : new LineGraphic();
        info.GetBaseValueEmbedded(s, typeof(LineGraphic).BaseType, parent);

        return s;
      }
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
      this.Pen.Width = lineWidth;
      this.Pen.Color = lineColor;
      this.AutoSize = false;
    }

    public LineGraphic(float startX, float startY, float endX, float endY, float lineWidth, Color lineColor)
      :
      this(new PointF(startX, startY), new PointF(endX, endY))
    {
      this.Pen.Width = lineWidth;
      this.Pen.Color = lineColor;
      this.AutoSize = false;
    }

    public LineGraphic(LineGraphic from)
      : base(from)
    {
    }

    #endregion

    public override bool AllowNegativeSize
    {
      get
      {
        return true;
      }
    }

    public override object Clone()
    {
      return new LineGraphic(this);
    }


    public override GraphicsPath GetSelectionPath()
    {
      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();

      gp.AddLine(X + m_Bounds.X, Y + m_Bounds.Y, X + m_Bounds.X + Width, Y + m_Bounds.Y+Height);
      if (Pen.Width < 5)
        gp.Widen(new Pen(Color.Black, 5));
      else
        gp.Widen(Pen);

      if (this.Rotation != 0)
      {
        myMatrix.RotateAt(this.Rotation, new PointF(X, Y), MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
    }

    public override IHitTestObject HitTest(PointF pt)
    {
      IHitTestObject result = base.HitTest(pt);
      if(result!=null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
      
    }

    static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Line properties");
      ((LineGraphic)hitted).OnChanged();
      return true;
    }


   
    

    public override void Paint(Graphics g, object obj)
    {/*
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      g.RotateTransform(this.m_Rotation);
      g.DrawLine(this.Pen, 0, 0, Width, Height);
      g.Restore(gs);
      */

      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (m_Rotation != 0)
        g.RotateTransform(m_Rotation);
      Pen.BrushRectangle = this.m_Bounds;
      g.DrawLine(Pen, 0,0,Width, Height);
      g.Restore(gs);
    }
    #region IGrippableObject Members

    public override void ShowGrips(Graphics g)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (m_Rotation != 0)
        g.RotateTransform(m_Rotation);

      DrawRectangularGrip(g, new PointF(0, 0));
      DrawRectangularGrip(g, new PointF(1, 1));

      g.DrawLine(Pens.Blue, 0, 0, Width,  Height);
      
      g.Restore(gs);
    }

    public override IGripManipulationHandle GripHitTest(PointF point)
    {
      PointF rel;

      rel = new PointF(0, 0);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(1, 1);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      return null;
    }

    #endregion

  } // End Class

  #endregion

  #region RectangleGraphic

  [Serializable]
  public class RectangleGraphic : ShapeGraphic
  {
    #region Serialization

    #region Clipboard serialization

    protected RectangleGraphic(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    /// <summary>
    /// Serializes RectangleGraphic Version 0.
    /// </summary>
    /// <param name="obj">The RectangleGraphic to serialize.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      RectangleGraphic s = this;
      base.GetObjectData(info, context);
    }

    /// <summary>
    /// Deserializes the RectangleGraphic Version 0.
    /// </summary>
    /// <param name="obj">The empty RectangleGraphic object to deserialize into.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    /// <param name="selector">The deserialization surrogate selector.</param>
    /// <returns>The deserialized RectangleGraphic.</returns>
    public object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      RectangleGraphic s = (RectangleGraphic)base.SetObjectData(obj, info, context, selector);
      return s;
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }
    #endregion

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleGraphic), 0)]
    public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RectangleGraphic s = (RectangleGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(RectangleGraphic).BaseType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        RectangleGraphic s = null != o ? (RectangleGraphic)o : new RectangleGraphic();
        info.GetBaseValueEmbedded(s, typeof(RectangleGraphic).BaseType, parent);

        return s;
      }
    }


    #endregion


    #region Constructors
    public RectangleGraphic()
    {
    }

    public RectangleGraphic(PointF graphicPosition)
      :
      this()
    {
      this.SetPosition(graphicPosition);
    }

    public RectangleGraphic(float posX, float posY)
      :
      this(new PointF(posX, posY))
    {
    }


    public RectangleGraphic(PointF graphicPosition, SizeF graphicSize)
      :
      this(graphicPosition)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public RectangleGraphic(float posX, float posY, SizeF graphicSize)
      :
      this(new PointF(posX, posY), graphicSize)
    {
    }

    public RectangleGraphic(float posX, float posY, float width, float height)
      :
      this(new PointF(posX, posY), new SizeF(width, height))
    {
    }

    public RectangleGraphic(PointF graphicPosition, float Rotation)
      :
      this()
    {

      this.SetPosition(graphicPosition);
      this.Rotation = Rotation;
    }

    public RectangleGraphic(float posX, float posY, float Rotation)
      :
      this(new PointF(posX, posY), Rotation)
    {
    }

    public RectangleGraphic(PointF graphicPosition, SizeF graphicSize, float Rotation)
      :
      this(graphicPosition, Rotation)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public RectangleGraphic(float posX, float posY, SizeF graphicSize, float Rotation)
      :
      this(new PointF(posX, posY), graphicSize, Rotation)
    {
    }

    public RectangleGraphic(float posX, float posY, float width, float height, float Rotation)
      :
      this(new PointF(posX, posY), new SizeF(width, height), Rotation)
    {
    }

    static void Exchange(ref float x, ref float y)
    {
      float h = x;
      x = y;
      y = h;
    }
    public static RectangleGraphic FromLTRB(float left, float top, float right, float bottom)
    {
      if (left > right)
        Exchange(ref left, ref right);
      if (top > bottom)
        Exchange(ref top, ref bottom);

      return new RectangleGraphic(left, top, right - left, bottom - top);
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


    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (m_Rotation != -0)
        g.RotateTransform(m_Rotation);
      
      if (Brush.IsVisible)
      {
        Brush.Rectangle = m_Bounds;
        g.FillRectangle(Brush, m_Bounds);
      }

      Pen.BrushRectangle = m_Bounds;
      g.DrawRectangle(Pen, m_Bounds.X, m_Bounds.Y, m_Bounds.Width, m_Bounds.Height);
      g.Restore(gs);
    }
  } // End Class

  #endregion

  #region EllipseGraphic
  [Serializable]
  public class EllipseGraphic : ShapeGraphic
  {

    #region Serialization

    #region Clipboard serialization

    protected EllipseGraphic(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    /// <summary>
    /// Serializes EllipseGraphic Version 0.
    /// </summary>
    /// <param name="obj">The EllipseGraphic to serialize.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      EllipseGraphic s = this;
      base.GetObjectData(info, context);
    }


    /// <summary>
    /// Deserializes the EllipseGraphic Version 0.
    /// </summary>
    /// <param name="obj">The empty EllipseGraphic object to deserialize into.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    /// <param name="selector">The deserialization surrogate selector.</param>
    /// <returns>The deserialized EllipseGraphic.</returns>
    public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      EllipseGraphic s = (EllipseGraphic)base.SetObjectData(obj, info, context, selector);
      return s;
    }
    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }
    #endregion

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EllipseGraphic), 0)]
    public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        EllipseGraphic s = (EllipseGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(EllipseGraphic).BaseType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        EllipseGraphic s = null != o ? (EllipseGraphic)o : new EllipseGraphic();
        info.GetBaseValueEmbedded(s, typeof(EllipseGraphic).BaseType, parent);

        return s;
      }
    }

    #endregion


    #region Constructors

    public EllipseGraphic()
    {
    }

    public EllipseGraphic(PointF graphicPosition)
      :
      this()
    {
      this.SetPosition(graphicPosition);
    }
    public EllipseGraphic(float posX, float posY)
      :
      this(new PointF(posX, posY))
    {
    }

    public EllipseGraphic(PointF graphicPosition, SizeF graphicSize)
      :
      this(graphicPosition)
    {

      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public EllipseGraphic(float posX, float posY, SizeF graphicSize)
      :
      this(new PointF(posX, posY), graphicSize)
    {
    }

    public EllipseGraphic(float posX, float posY, float width, float height)
      :
      this(new PointF(posX, posY), new SizeF(width, height))
    {
    }

    public EllipseGraphic(PointF graphicPosition, float Rotation)
      :
      this()
    {
      this.SetPosition(graphicPosition);
      this.Rotation = Rotation;
    }

    public EllipseGraphic(float posX, float posY, float Rotation)
      :
      this(new PointF(posX, posY), Rotation)
    {
    }

    public EllipseGraphic(PointF graphicPosition, SizeF graphicSize, float Rotation)
      :
      this(graphicPosition, Rotation)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public EllipseGraphic(float posX, float posY, SizeF graphicSize, float Rotation)
      :
      this(new PointF(posX, posY), graphicSize, Rotation)
    {
    }

    public EllipseGraphic(float posX, float posY, float width, float height, float Rotation)
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

    public override GraphicsPath GetSelectionPath()
    {
      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();

      gp.AddEllipse(new RectangleF(X + m_Bounds.X, Y + m_Bounds.Y, Width, Height));
      if (this.Rotation != 0)
      {
        myMatrix.RotateAt(this.Rotation, new PointF(X, Y), MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
    }

    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (m_Rotation != 0)
        g.RotateTransform(m_Rotation);
     
      if (Brush.IsVisible)
      {
        Brush.Rectangle = m_Bounds;
        g.FillEllipse(Brush, m_Bounds);
      }

      Pen.BrushRectangle = m_Bounds;
      g.DrawEllipse(Pen, m_Bounds);
      g.Restore(gs);
    }
  } // end class

  #endregion

} // end Namespace
