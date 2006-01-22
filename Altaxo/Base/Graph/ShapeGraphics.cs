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

namespace Altaxo.Graph
{


  [Serializable]
  public abstract class ShapeGraphic : GraphicsObject
  {
    protected BrushHolder m_fillBrush;
    protected PenHolder m_linePen;
    //  protected float m_lineWidth = 1;
    protected bool m_fill = false;

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
      info.AddValue("Fill", s.m_fill);
      info.AddValue("FillBrush", s.m_fillBrush);

    }
    public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      ShapeGraphic s = (ShapeGraphic)base.SetObjectData(obj, info, context, selector);

      s.m_linePen = (PenHolder)info.GetValue("LinePen", typeof(PenHolder));
      s.m_fill = info.GetBoolean("Fill");
      s.m_fillBrush = (BrushHolder)info.GetValue("FillBrush", typeof(BrushHolder));

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
        //info.AddValue("LineWidth",s.m_lineWidth);

        info.AddValue("Fill", s.m_fill);
        info.AddValue("FillBrush", s.m_fillBrush);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        ShapeGraphic s = (ShapeGraphic)o;
        info.GetBaseValueEmbedded(s, typeof(ShapeGraphic).BaseType, parent);


        s.m_linePen = (PenHolder)info.GetValue("LinePen", s);
        //s.m_lineWidth = info.GetSingle("LineWidth");

        s.m_fill = info.GetBoolean("Fill");
        s.m_fillBrush = (BrushHolder)info.GetValue("FillBrush", s);
        return s;
      }
    }


    #endregion

    public ShapeGraphic()
    {
      m_fillBrush = new BrushHolder(Color.White);
      m_linePen = new PenHolder(Color.Black);
      //m_lineWidth = 1;
      m_fill = false;
    }

    public ShapeGraphic(ShapeGraphic from)
      :
      base(from)
    {
      this.m_fillBrush = (BrushHolder)from.m_fillBrush.Clone();
      this.m_linePen = (PenHolder)from.m_linePen.Clone();
      //this.m_lineWidth = from.m_lineWidth;
      this.m_fill = from.m_fill;
    }

    public virtual float LineWidth
    {
      get
      {
        return m_linePen.Width;
      }
      set
      {
        if (value > 0)
          m_linePen.Width = value;
        else
          throw new ArgumentOutOfRangeException("LineWidth", "Line Width must be > 0");
      }
    }

    public virtual PenHolder Pen
    {
      get
      {
        return m_linePen;
      }
      set
      {
        if (value != null)
          m_linePen = (PenHolder)value.Clone();
        else
          throw new ArgumentNullException("The line pen must not be null");
      }
    }

    public virtual Color LineColor
    {
      get
      {
        return m_linePen.Color;
      }
      set
      {
        m_linePen.Color = value;
      }
    }

    public virtual bool Fill
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
        return m_fillBrush.Color;
      }
      set
      {
        m_fillBrush = new BrushHolder(value);
      }
    }
  } //  End Class


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
      gp.AddLine(0, 0, Width, Height);
      myMatrix.Translate(X, Y);
      myMatrix.Rotate(this.Rotation);
      gp.Transform(myMatrix);
      using (Pen myPen = new Pen(Color.Black, 5))
      {
        if (gp.IsOutlineVisible(pt, myPen))
        {
          gp.Widen(myPen);
          return new HitTestObject(gp, this);
        }
      }
      return null;
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
      g.DrawLine(this.Pen, 0, 0, Width, Height);
      g.Restore(gs);
    }
    #region IGrippableObject Members

    public void ShowGrips(Graphics g)
    {
      g.DrawRectangle(Pens.Blue, X - 6, Y - 6, 12, 12);
      g.DrawRectangle(Pens.Blue, X + Width - 6, Y + Height - 6, 12, 12);
      g.DrawLine(Pens.Blue, X, Y, X + Width, Y + Height);
    }

    public IGripManipulationHandle GripHitTest(PointF point)
    {
      double dx, dy;
      dx = point.X - this.X;
      dy = point.Y - this.Y;

      if ((dx * dx + dy * dy) < 36)
        return new GripHandle(this, true);

      dx = point.X - this.X - this.Width;
      dy = point.Y - this.Y - this.Height;

      if ((dx * dx + dy * dy) < 36)
        return new GripHandle(this, false);

      return null;
    }

    #endregion


    #region GripHandle

    private class GripHandle : IGripManipulationHandle
    {
      LineGraphic _parent;
      bool _isFirstPoint;

      public GripHandle(LineGraphic parent, bool isFirstPoint)
      {
        _parent = parent;
        _isFirstPoint = isFirstPoint;
      }

      #region IGripManipulationHandle Members

      public void MoveGrip(PointF newPosition)
      {
        if (_isFirstPoint)
        {
          PointF endPoint = _parent.GetEndPosition();
          _parent.SetStartPosition(newPosition);
          _parent.SetEndPosition(endPoint);
        }
        else // second point
        {
          _parent.SetEndPosition(newPosition);
        }
      }

      #endregion

    }

    #endregion


  } // End Class


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
      RectangleF rect = new RectangleF(0, 0, Width, Height);
      if (this.Fill)
      {
        g.FillRectangle(new SolidBrush(this.FillColor), rect);
      }
      Pen myPen = new Pen(this.LineColor, this.LineWidth);
      g.DrawRectangle(myPen, 0, 0, rect.Width, rect.Height);
      g.Restore(gs);
    }

  } // End Class


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


    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (m_Rotation != 0)
        g.RotateTransform(m_Rotation);

      RectangleF rect = new RectangleF(0, 0, Width, Height);
      if (this.Fill)
        g.FillEllipse(new SolidBrush(this.FillColor), rect);

      Pen myPen = new Pen(this.LineColor, this.LineWidth);
      g.DrawEllipse(myPen, rect);
      g.Restore(gs);
    }
  } // end class

} // end Namespace
