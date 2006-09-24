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


namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// GraphicsObject is the abstract base class for general graphical objects on the layer,
  /// for instance text elements, lines, pictures, rectangles and so on.
  /// </summary>
  [Serializable()]
  public abstract class GraphicsObject 
    :
    System.Runtime.Serialization.ISerializable,
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChangedEventSource,
    System.ICloneable,
    IGrippableObject
  {
    /// <summary>
    /// If true, the graphical object sizes itself, for instance simple text objects.
    /// </summary>
    protected bool   m_AutoSize = true;

    /// <summary>
    /// The bounds of this object.
    /// </summary>
    protected RectangleF m_Bounds = new RectangleF(0,0,0,0);

    /// <summary>
    /// The parent collection this graphical object belongs to.
    /// </summary>
    protected GraphicsObjectCollection m_Container=null;

    /// <summary>
    /// The position of the graphical object, normally the upper left corner. Strictly spoken,
    /// this is the position of the anchor point of the object.
    /// </summary>
    protected PointF m_Position = new PointF(0, 0);
    /// <summary>
    /// The rotation angle of the graphical object in reference to the layer.
    /// </summary>
    protected float  m_Rotation = 0;




    #region Serialization

    protected GraphicsObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this,info,context,null);
    }
    public virtual object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
    {
      m_Position = (PointF)info.GetValue("Position", typeof(PointF));
      m_Bounds = (RectangleF)info.GetValue("Bounds", typeof(RectangleF));
      m_Rotation = info.GetSingle("Rotation");
      m_AutoSize = info.GetBoolean("AutoSize");
      return this;
    }
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      info.AddValue("Position", m_Position);
      info.AddValue("Bounds",   m_Bounds);
      info.AddValue("Rotation", m_Rotation);
      info.AddValue("AutoSize", m_AutoSize);
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicsObject),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphicsObject s = (GraphicsObject)obj;
        info.AddValue("Position",s.m_Position);  
        info.AddValue("Bounds",s.m_Bounds);
        info.AddValue("Rotation",s.m_Rotation);
        info.AddValue("AutoSize",s.m_AutoSize);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        GraphicsObject s = (GraphicsObject)o;

        s.m_Position = (PointF)info.GetValue("Position",s);  
        s.m_Bounds = (RectangleF)info.GetValue("Bounds",s);
        s.m_Rotation = info.GetSingle("Rotation");
        s.m_AutoSize = info.GetBoolean("AutoSize");

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


    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The object to copy the data from.</param>
    protected GraphicsObject(GraphicsObject from)
    {
      this.m_AutoSize = from.m_AutoSize;
      this.m_Bounds  = from.m_Bounds;
      this.m_Container = null;
      this.m_Position  = from.m_Position;
      this.m_Rotation  = from.m_Rotation;
    }

    /// <summary>
    /// Initializes with default values.
    /// </summary>
    protected GraphicsObject()
    {
    }

    /// <summary>
    /// Initializes with a certain position in points (1/72 inch).
    /// </summary>
    /// <param name="graphicPosition">The initial position of the graphical object.</param>
    protected GraphicsObject(PointF graphicPosition)
    {
      SetPosition(graphicPosition);
    }

    /// <summary>
    /// Initializes the GraphicsObject with a certain position in points (1/72 inch).
    /// </summary>
    /// <param name="posX">The initial x position of the graphical object.</param>
    /// <param name="posY">The initial y position of the graphical object.</param>
    protected GraphicsObject(float posX, float posY)
      : this(new PointF(posX,posY))
    {
    }

    protected GraphicsObject(PointF graphicPosition, SizeF graphicSize)
      : this(graphicPosition)
    {
      SetSize(graphicSize);
      this.AutoSize = false;
    }
    protected GraphicsObject(float posX, float posY, SizeF graphicSize)
      : this(new PointF(posX, posY), graphicSize)
    {
    }

    protected GraphicsObject(float posX, float posY,
      float width, float height)
      : this(new PointF(posX, posY), new SizeF(width, height))
    {
    }

    protected GraphicsObject(PointF graphicPosition, float Rotation)
    {
      this.SetPosition(graphicPosition);
      this.Rotation = Rotation;
    }

    protected GraphicsObject(float posX, float posY, float Rotation)
      : this(new PointF(posX, posY), Rotation)
    {
    }

    protected GraphicsObject(PointF graphicPosition, SizeF graphicSize, float Rotation)
      : this(graphicPosition, Rotation)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }
    protected GraphicsObject(float posX, float posY, SizeF graphicSize, float Rotation)
      : this(new PointF(posX, posY), graphicSize, Rotation)
    {
    }

    protected GraphicsObject(float posX, float posY, float width, float height, float Rotation)
      : this(new PointF(posX, posY), new SizeF(width, height), Rotation)
    {
    }

    public GraphicsObjectCollection Container
    {
      get
      {
        return m_Container;
      }
      set
      {
        m_Container = value;
      }
    }


  

    public virtual bool AutoSize
    {
      get
      {
        return m_AutoSize;
      }
      set
      {
        if(value != m_AutoSize)
          m_AutoSize = value;
      }
    }
    public virtual float X
    {
      get
      {
        return m_Position.X;
      }
      set
      {
        m_Position.X = value;
      }
    }
    public virtual float Y
    {
      get
      {
        return m_Position.Y;
      }
      set
      {
        m_Position.Y = value;
      }
    }

    public virtual bool AllowNegativeSize
    {
      get
      {
        return false;
      }
    }

    public virtual PointF GetPosition()
    {
      return this.m_Position;
    }

    public virtual void SetPosition(PointF Value)
    {
      this.m_Position = Value;
    }

    public PointF Position
    {
      get
      {
        return GetPosition();
      }
      set
      {
        SetPosition(value);
      }
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
      return RelativeToAbsolutePosition(new PointF(1, 1), true);
    }

    public void SetEndPosition(PointF Value)
    {
      if (m_Rotation == 0)
      {
        SizeF siz = new SizeF(
          Value.X - this.m_Position.X,
          Value.Y - this.m_Position.Y);
        SetSize(siz);
      }
      else
      {
        double cosphi = Math.Cos(m_Rotation * Math.PI / 180);
        double sinphi = Math.Sin(m_Rotation * Math.PI / 180);
        double dx = Value.X - this.X;
        double dy = Value.Y - this.Y;
        // now we have to rotate backward to get the endpoint
        SizeF siz = new SizeF();
        siz.Width = (float)(dx * cosphi + dy * sinphi);
        siz.Height = (float)(-dx * sinphi + dy * cosphi);
        SetSize(siz);

      }
    }

   
    /// <summary>
    /// Scales the position of an item according to the provided xscale and yscale. Can be called with null for the item (in this case nothing happens).
    /// </summary>
    /// <param name="o">The graphics object whose position is scaled.</param>
    /// <param name="xscale">The xscale ratio.</param>
    /// <param name="yscale">The yscale ratio.</param>
    public static void ScalePosition(GraphicsObject o, double xscale, double yscale)
    {
      if(o!=null)
      {
        PointF oldP = o.Position;
        o.SetPosition(new PointF((float)(oldP.X*xscale),(float)(oldP.Y*yscale)));
      }
    }


    public virtual float Height
    {
      get
      {
        return m_Bounds.Height;
      }
      set
      {
        m_Bounds.Height = value;
      }
    }
    public virtual float Width
    {
      get
      {
        return m_Bounds.Width;
      }
      set
      {
        m_Bounds.Width = value;
      }
    }

    public virtual void SetSize(SizeF Value)
    {
      m_Bounds.Size = Value;
    }
    public virtual SizeF GetSize()
    {
      return this.m_Bounds.Size;
    }

    public SizeF Size 
    {
      get
      {
        return GetSize();
      }
      set
      {
        SetSize(value);
      }
    }

    public virtual float Rotation
    {
      get
      {
        return m_Rotation;
      }
      set
      {
        m_Rotation = value;
      }
    }

    public abstract void Paint(Graphics g, object obj);
    #region IChangedEventSource Members

    public event System.EventHandler Changed;


    protected void EhChildChanged(object sender, EventArgs e)
    {
      OnChanged();
    }


    protected virtual void OnChanged()
    {
      if(null!=this.m_Container )
        m_Container.EhChildChanged(this,new Main.ChangedEventArgs(this,null));

      if(null!=Changed)
        Changed(this,new Main.ChangedEventArgs(this,null));
    }

    #endregion

    /// <summary>
    /// Creates a cloned copy of this object.
    /// </summary>
    /// <returns>The cloned copy of this object.</returns>
    public abstract object Clone();

    #region HitTesting
    public virtual IHitTestObject HitTest(PointF pt)
    {
      GraphicsPath gp = GetSelectionPath();
      if (gp.IsVisible(pt))
      {
        return new HitTestObject(gp, this);
      }
      else
        return null;
    }


    public virtual GraphicsPath GetSelectionPath()
    {
      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();

      gp.AddRectangle(new RectangleF(X + m_Bounds.X, Y + m_Bounds.Y, Width, Height));
      if (this.Rotation != 0)
      {
        myMatrix.RotateAt(this.Rotation, new PointF(X, Y), MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
    }


    public virtual bool HitTest(RectangleF rect)
    {
      // is this object contained within the supplied rectangle

      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();


      gp.AddRectangle(new RectangleF(X + m_Bounds.X, Y + m_Bounds.Y, Width, Height));
      if (this.Rotation != 0)
      {
        myMatrix.RotateAt(this.Rotation, new PointF(this.X, this.Y), MatrixOrder.Append);
      }
      gp.Transform(myMatrix);
      RectangleF gpRect = gp.GetBounds();
      return rect.Contains(gpRect);
    }
    #endregion

    #region Hitting Helper functions

    /// <summary>
    /// Converts relative positions (0..1, 0..1) to absolute position of the rectangle, taking into account
    /// the current rotation.
    /// </summary>
    /// <param name="p">Relative coordinates of the rectangle (0,0 is the upper left corner, 1,1 is the lower right corner).</param>
    /// <param name="withRotation">If true, the coordinates are calculated taking the rotation into account.</param>
    /// <returns>The coordinates of this point.</returns>
    public PointF RelativeToAbsolutePosition(PointF p, bool withRotation)
    {
      double dx = p.X * m_Bounds.Width;
      double dy = p.Y * m_Bounds.Height;

      
     dx += m_Bounds.X;
     dy += m_Bounds.Y;
     

      if (withRotation && m_Rotation != 0)
      {
        double cosphi = Math.Cos(m_Rotation * Math.PI / 180);
        double sinphi = Math.Sin(m_Rotation * Math.PI / 180);

        double helpdx = (dx * cosphi - dy * sinphi);
        dy = (dy * cosphi + dx * sinphi);
        dx = helpdx;

      }

      if (withRotation)
        return new PointF((float)(m_Position.X + dx), (float)(m_Position.Y + dy));
      else
        return new PointF((float)(dx), (float)(dy));
    }

    public SizeF ToUnrotatedDifference(PointF pivot, PointF point)
    {
      double dx = point.X - pivot.X;
      double dy = point.Y - pivot.Y;

      if (m_Rotation != 0)
      {
        double cosphi = Math.Cos(m_Rotation * Math.PI / 180);
        double sinphi = Math.Sin(m_Rotation * Math.PI / 180);
        // now we have to rotate backward to get the endpoint
        double helpdx = (dx * cosphi + dy * sinphi);
        dy = (-dx * sinphi + dy * cosphi);
        dx = helpdx;
      }

      return new SizeF((float)dx, (float)dy);
    }

    public PointF ToUnrotatedCoordinates(PointF pivot, PointF point)
    {
      double dx = point.X - pivot.X;
      double dy = point.Y - pivot.Y;

      if (m_Rotation != 0)
      {
        double cosphi = Math.Cos(m_Rotation * Math.PI / 180);
        double sinphi = Math.Sin(m_Rotation * Math.PI / 180);
        // now we have to rotate backward to get the endpoint
        double helpdx = (dx * cosphi + dy * sinphi);
        dy = (-dx * sinphi + dy * cosphi);
        dx = helpdx;
      }
      return new PointF((float)(pivot.X + dx), (float)(pivot.Y + dy));
    }


    public SizeF ToRotatedDifference(PointF pivot, PointF point)
    {
      double dx = point.X - pivot.X;
      double dy = point.Y - pivot.Y;

      if (m_Rotation != 0)
      {
        double cosphi = Math.Cos(m_Rotation * Math.PI / 180);
        double sinphi = Math.Sin(m_Rotation * Math.PI / 180);
        // now we have to rotate backward to get the endpoint
        double helpdx = (dx * cosphi - dy * sinphi);
        dy = (dx * sinphi + dy * cosphi);
        dx = helpdx;
      }
      return new SizeF((float)(dx), (float)(dy));
    }

    public void SetBoundsFrom(PointF relPivot, PointF relDrawGrip, SizeF diff)
    {
      double dx = relDrawGrip.X - relPivot.X;
      double dy = relDrawGrip.Y - relPivot.Y;

      if (dx == 1 && (diff.Width > 0 || AllowNegativeSize))
      {
        this.m_Bounds.Width = diff.Width;
      }
      else if (dx == -1 && (diff.Width < 0 || AllowNegativeSize))
      {
        SizeF s = ToRotatedDifference(PointF.Empty, new PointF(diff.Width + m_Bounds.Width, 0));
        this.m_Position.X += s.Width;
        this.m_Position.Y += s.Height;
        this.m_Bounds.Width = -diff.Width;
      }

      if (dy == 1 && (diff.Height > 0 || AllowNegativeSize))
      {
        this.m_Bounds.Height = diff.Height;
      }
      else if (dy == -1 && (diff.Height < 0 || AllowNegativeSize))
      {
        SizeF s = ToRotatedDifference(PointF.Empty, new PointF(0, diff.Height + m_Bounds.Height));
        this.m_Position.X += s.Width;
        this.m_Position.Y += s.Height;
        this.m_Bounds.Height = -diff.Height;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="relPivot">Pivot point in relative coordinates.</param>
    /// <param name="absPivot">Pivot point in absolute coordinates.</param>
    /// <param name="relDrawGrip">Grip point in relative coordinates.</param>
    /// <param name="diff">Difference between absolute grip point and absolute pivot point, in unrotated absolute coordinates.</param>
    public void SetRotationFrom(PointF relPivot, PointF absPivot, PointF relDrawGrip, SizeF diff)
    {
      double dx = (relDrawGrip.X - relPivot.X) * m_Bounds.Width;
      double dy = (relDrawGrip.Y - relPivot.Y) * m_Bounds.Height;
      double a1 = Math.Atan2(dy, dx);
      double a2 = Math.Atan2(diff.Height, diff.Width);

      this.m_Rotation = (float)(180 * (a2 - a1) / Math.PI);

      //SizeF s = ToRotatedDifference(PointF.Empty, new PointF(-m_Bounds.Width / 2, -m_Bounds.Height / 2));
      SizeF s = ToRotatedDifference(RelativeToAbsolutePosition(relPivot, false), Point.Empty);
      this.m_Position.X = absPivot.X + s.Width;
      this.m_Position.Y = absPivot.Y + s.Height;

    }

    public RectangleF GetRectangularGrip(PointF relPos)
    {
      PointF pos = RelativeToAbsolutePosition(relPos, false);

      RectangleF rect = new RectangleF(pos, SizeF.Empty);
      rect.Inflate(6, 6);

      return rect;
    }

    public void DrawRectangularGrip(Graphics g, PointF relPos)
    {
      RectangleF r = GetRectangularGrip(relPos);
      g.DrawRectangle(Pens.Blue, r.X, r.Y, r.Width, r.Height);
    }

    public bool IsRectangularGripHitted(PointF relPos, PointF hittest)
    {
      PointF pos = RelativeToAbsolutePosition(relPos, true);
      double dx = hittest.X - pos.X;
      double dy = hittest.Y - pos.Y;
      double r = dx * dx + dy * dy;
      return r <= 36;
    }

    public void DrawRotationGrip(Graphics g, PointF relPos)
    {
      RectangleF r = GetRectangularGrip(relPos);
      Pen pen = new Pen(Color.Blue);
      pen.StartCap = LineCap.ArrowAnchor;
      pen.EndCap = LineCap.ArrowAnchor;
      g.DrawArc(pen, r.X, r.Y, r.Width, r.Height, 0, 320);
    }

    public bool IsRotationGripHitted(PointF relPos, PointF hittest)
    {
      return IsRectangularGripHitted(relPos, hittest);
    }


    #endregion

    #region IGrippableObject Members

    public virtual void ShowGrips(Graphics g)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (m_Rotation != 0)
        g.RotateTransform(m_Rotation);

      DrawRectangularGrip(g, new PointF(0, 0));
      DrawRectangularGrip(g, new PointF(0, 1));
      DrawRectangularGrip(g, new PointF(1, 0));
      DrawRectangularGrip(g, new PointF(1, 1));

      DrawRectangularGrip(g, new PointF(0.5f, 0));
      DrawRectangularGrip(g, new PointF(1, 0.5f));
      DrawRectangularGrip(g, new PointF(0.5f, 1));
      DrawRectangularGrip(g, new PointF(0, 0.5f));

      DrawRotationGrip(g, new PointF(0.2f, 0.2f));
      DrawRotationGrip(g, new PointF(0.8f, 0.2f));
      DrawRotationGrip(g, new PointF(0.8f, 0.8f));
      DrawRotationGrip(g, new PointF(0.2f, 0.8f));

      g.DrawRectangle(Pens.Blue, m_Bounds.X,m_Bounds.Y,m_Bounds.Width,m_Bounds.Height);

      g.Restore(gs);
    }

    public virtual IGripManipulationHandle GripHitTest(PointF point)
    {
      PointF rel;

      rel = new PointF(0, 0);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(1, 0);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(0, 1);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(1, 1);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(0.5f, 0);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(1, 0.5f);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(0.5f, 1);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(0, 0.5f);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(0.2f, 0.2f);
      if (IsRotationGripHitted(rel, point))
        return new RotationGripHandle(this, rel);

      rel = new PointF(0.8f, 0.2f);
      if (IsRotationGripHitted(rel, point))
        return new RotationGripHandle(this, rel);

      rel = new PointF(0.8f, 0.8f);
      if (IsRotationGripHitted(rel, point))
        return new RotationGripHandle(this, rel);

      rel = new PointF(0.2f, 0.8f);
      if (IsRotationGripHitted(rel, point))
        return new RotationGripHandle(this, rel);

      return null;
    }

    #endregion

    #region GripHandle

    protected class SizeMoveGripHandle : IGripManipulationHandle
    {
      GraphicsObject _parent;
      PointF _drawrPosition;
      PointF _fixrPosition;
      PointF _fixaPosition;
      bool _allowNegativeSize;

      public SizeMoveGripHandle(GraphicsObject parent, PointF relPos)
      {
        _parent = parent;
        _drawrPosition = relPos;
        _fixrPosition = new PointF(relPos.X == 0 ? 1 : 0, relPos.Y == 0 ? 1 : 0);
        _fixaPosition = _parent.RelativeToAbsolutePosition(_fixrPosition, true);
      }

      public void MoveGrip(PointF newPosition)
      {
        SizeF diff = _parent.ToUnrotatedDifference(_fixaPosition, newPosition);
        _parent.SetBoundsFrom(_fixrPosition, _drawrPosition, diff);
      }
    }

    protected class RotationGripHandle : IGripManipulationHandle
    {
      GraphicsObject _parent;
      PointF _drawrPosition;
      PointF _fixrPosition;
      PointF _fixaPosition;

      public RotationGripHandle(GraphicsObject parent, PointF relPos)
      {
        _parent = parent;
        _drawrPosition = relPos;
        _fixrPosition = new PointF(0.5f, 0.5f);
        _fixaPosition = _parent.RelativeToAbsolutePosition(_fixrPosition, true);
      }

      public void MoveGrip(PointF newPosition)
      {
        SizeF diff = new SizeF();

        diff.Width = newPosition.X - _fixaPosition.X;
        diff.Height = newPosition.Y - _fixaPosition.Y;
        _parent.SetRotationFrom(_fixrPosition, _fixaPosition, _drawrPosition, diff);
      }
    }

    #endregion

  }
}
