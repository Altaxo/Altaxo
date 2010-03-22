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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// The abstract base class for general graphical objects on the layer,
  /// for instance text elements, lines, pictures, rectangles and so on.
  /// </summary>
  [Serializable]
  public abstract class GraphicBase 
    :
    System.Runtime.Serialization.ISerializable,
    System.Runtime.Serialization.IDeserializationCallback,
    Main.IChangedEventSource,
    Main.IDocumentNode,
    System.ICloneable,
    IGrippableObject
  {
    /// <summary>
    /// If true, the graphical object sizes itself, for instance simple text objects.
    /// </summary>
    protected bool   _autoSize = true;

    /// <summary>
    /// The bounds of this object.
    /// </summary>
    protected RectangleF _bounds = new RectangleF(0,0,0,0);

    /// <summary>
    /// The position of the anchor point of the object, normally the upper left corner.
    /// </summary>
    protected PointF _position = new PointF(0, 0);
  
    /// <summary>
    /// The rotation angle of the graphical object in reference to the layer.
    /// </summary>
    protected float  _rotation = 0;

		/// <summary>
		/// The shear of the object. This is the deviation of x when y is incremented by 1.
		/// </summary>
		protected float _shear = 0;

		/// <summary>X scale factor.</summary>
		protected float _scaleX = 1;

		/// <summary>Y scale factor.</summary>
		protected float _scaleY = 1;

		/// <summary>Cached matrix which transforms from own coordinates to parent (layer) coordinates.</summary>
		protected TransformationMatrix2D _transfoToLayerCoord = new TransformationMatrix2D();


    /// <summary>
    /// The parent collection this graphical object belongs to.
    /// </summary>
    [NonSerialized]
    protected object _parent = null;

    [field:NonSerialized]
    public event System.EventHandler Changed;


    protected static GraphicsPath _outsideArrow;
		protected static PointF[] _outsideArrowPoints;
		protected static GraphicsPath _rotationGripShape;

    #region Serialization

    protected GraphicBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this,info,context,null);
    }
    public virtual object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
    {
      _position = (PointF)info.GetValue("Position", typeof(PointF));
      _bounds = (RectangleF)info.GetValue("Bounds", typeof(RectangleF));
      _rotation = info.GetSingle("Rotation");
      _autoSize = info.GetBoolean("AutoSize");
			UpdateTransformationMatrices();
      return this;
    }
    public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      info.AddValue("Position", _position);
      info.AddValue("Bounds",   _bounds);
      info.AddValue("Rotation", _rotation);
      info.AddValue("AutoSize", _autoSize);
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.GraphicsObject", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
        /*
        GraphicBase s = (GraphicBase)obj;
        info.AddValue("Position",s._position);  
        info.AddValue("Bounds",s._bounds);
        info.AddValue("Rotation",s._rotation);
        info.AddValue("AutoSize",s._autoSize);
        */
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        GraphicBase s = (GraphicBase)o;

        s._position = (PointF)info.GetValue("Position",s);  
        s._bounds = (RectangleF)info.GetValue("Bounds",s);
        s._rotation = -info.GetSingle("Rotation"); // meaning of rotation reversed in version 2
        s._autoSize = info.GetBoolean("AutoSize");
				s.UpdateTransformationMatrices();

        return s;
      }
    }

    // 2007-01-10 meaning of rotation was reversed
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase), 2)]
    class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GraphicBase s = (GraphicBase)obj;
        info.AddValue("Position", s._position);
        info.AddValue("Bounds", s._bounds);
        info.AddValue("Rotation", s._rotation);
        info.AddValue("AutoSize", s._autoSize);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        GraphicBase s = (GraphicBase)o;

        s._position = (PointF)info.GetValue("Position", s);
        s._bounds = (RectangleF)info.GetValue("Bounds", s);
        s._rotation = info.GetSingle("Rotation");
        s._autoSize = info.GetBoolean("AutoSize");
				s.UpdateTransformationMatrices();

        return s;
      }
    }


		// 2010-03-16 ScaleX, ScaleY, and Shear added
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase), 3)]
		class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphicBase s = (GraphicBase)obj;
				info.AddValue("Position", s._position);
				info.AddValue("Bounds", s._bounds);
				info.AddValue("Rotation", s._rotation);
				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
				info.AddValue("Shear", s._shear);
				info.AddValue("AutoSize", s._autoSize);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				GraphicBase s = (GraphicBase)o;

				s._position = (PointF)info.GetValue("Position", s);
				s._bounds = (RectangleF)info.GetValue("Bounds", s);
				s._rotation = info.GetSingle("Rotation");
				s._scaleX = info.GetSingle("ScaleX");
				s._scaleY = info.GetSingle("ScaleY");
				s._shear = info.GetSingle("Shear");
				s._autoSize = info.GetBoolean("AutoSize");
				s.UpdateTransformationMatrices();

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
    protected GraphicBase(GraphicBase from)
    {
      CopyFrom(from);
    }

    protected virtual void CopyFrom(GraphicBase from)
    {
      this._autoSize = from._autoSize;
      this._bounds = from._bounds;
      this._position = from._position;
      this._rotation = from._rotation;
			this._scaleX = from._scaleX;
			this._scaleY = from._scaleY;
			this._shear = from._shear;
      bool wasUsed = (null != this._parent);
      this._parent = from._parent;
			this.UpdateTransformationMatrices();

      if(wasUsed)
        OnChanged();
    }

    static GraphicBase()
    {
      // The arrow has a length of 1 and a maximum witdth of 2*arrowWidth and a shaft width of 2*arrowShaft
      const float arrowShaft=0.15f;
      const float arrowWidth=0.3f;
      _outsideArrowPoints = new PointF[] {
        new PointF(0,arrowShaft),
        new PointF(1-arrowWidth,arrowShaft),
        new PointF(1-arrowWidth,arrowWidth),
        new PointF(1,0),
        new PointF(1-arrowWidth, -arrowWidth),
        new PointF(1-arrowWidth, -arrowShaft),
        new PointF(0,-arrowShaft)
      };
      _outsideArrow = new GraphicsPath();
			_outsideArrow.AddPolygon(_outsideArrowPoints);


			const float ra = 2.1f / 2;
			const float ri = 1.5f / 2;
			const float rotArrowWidth = 0.6f;
			float rii = (ra+ri-rotArrowWidth)/2;
			float raa = (ra+ri+rotArrowWidth)/2;
			float cos45 = (float)Math.Sqrt(0.5);
			float sin45 = cos45;
			_rotationGripShape = new GraphicsPath();

			_rotationGripShape.AddArc(-ri, -ri, 2*ri, 2*ri, -45, 90); // mit Innenradius beginnen
			
			_rotationGripShape.AddLines(new PointF[] 
			{
				new PointF(rii*cos45, rii*sin45),
				new PointF(rii*cos45, rii*sin45 + rotArrowWidth*cos45),
				new PointF(raa*cos45, raa*sin45)
			});
			
			_rotationGripShape.AddArc(-ra, -ra, 2*ra, 2*ra, 45, -90); // Auﬂenradius
			
			_rotationGripShape.AddLines(new PointF[] 
			{
				new PointF(raa*cos45, -raa*sin45),
				new PointF(rii*cos45, -rii*sin45 - rotArrowWidth*cos45),
				new PointF(rii*cos45, -rii*sin45),
			});
			
			_rotationGripShape.CloseFigure();
    }

    /// <summary>
    /// Initializes with default values.
    /// </summary>
    protected GraphicBase()
    {
    }

    /// <summary>
    /// Initializes with a certain position in points (1/72 inch).
    /// </summary>
    /// <param name="graphicPosition">The initial position of the graphical object.</param>
    protected GraphicBase(PointF graphicPosition)
    {
      SetPosition(graphicPosition);
    }

    /// <summary>
    /// Initializes the GraphicsObject with a certain position in points (1/72 inch).
    /// </summary>
    /// <param name="posX">The initial x position of the graphical object.</param>
    /// <param name="posY">The initial y position of the graphical object.</param>
    protected GraphicBase(float posX, float posY)
      : this(new PointF(posX,posY))
    {
    }

    protected GraphicBase(PointF graphicPosition, SizeF graphicSize)
      : this(graphicPosition)
    {
      SetSize(graphicSize);
      this.AutoSize = false;
    }
    protected GraphicBase(float posX, float posY, SizeF graphicSize)
      : this(new PointF(posX, posY), graphicSize)
    {
    }

    protected GraphicBase(float posX, float posY,
      float width, float height)
      : this(new PointF(posX, posY), new SizeF(width, height))
    {
    }

    protected GraphicBase(PointF graphicPosition, float Rotation)
    {
      this.SetPosition(graphicPosition);
      this.Rotation = Rotation;
    }

    protected GraphicBase(float posX, float posY, float Rotation)
      : this(new PointF(posX, posY), Rotation)
    {
    }

    protected GraphicBase(PointF graphicPosition, SizeF graphicSize, float Rotation)
      : this(graphicPosition, Rotation)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }
    protected GraphicBase(float posX, float posY, SizeF graphicSize, float Rotation)
      : this(new PointF(posX, posY), graphicSize, Rotation)
    {
    }

    protected GraphicBase(float posX, float posY, float width, float height, float Rotation)
      : this(new PointF(posX, posY), new SizeF(width, height), Rotation)
    {
    }


    public virtual bool AutoSize
    {
      get
      {
        return _autoSize;
      }
      set
      {
        if (value != _autoSize)
        {
          _autoSize = value;
          OnChanged();
        }
      }
    }
    public virtual float X
    {
      get
      {
        return _position.X;
      }
      set
      {
        float oldvalue = _position.X;
        _position.X = value;
        if (value != oldvalue)
          OnChanged();
      }
    }
    public virtual float Y
    {
      get
      {
        return _position.Y;
      }
      set
      {
        float oldvalue = _position.Y;
        _position.Y = value;
        if (value != oldvalue)
          OnChanged();
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
      return this._position;
    }

    public virtual void SetPosition(PointF Value)
    {
      PointF oldvalue = _position;
      this._position = Value;
      if (Value != oldvalue)
        OnChanged();
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
      if (_rotation == 0)
      {
        SizeF siz = new SizeF(
          Value.X - this._position.X,
          Value.Y - this._position.Y);
        SetSize(siz);
      }
      else
      {
        double cosphi = Math.Cos(_rotation * Math.PI / 180);
        double sinphi = Math.Sin(-_rotation * Math.PI / 180);
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
    public static void ScalePosition(GraphicBase o, double xscale, double yscale)
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
        return _bounds.Height;
      }
      set
      {
        float oldvalue = _bounds.Height;
        _bounds.Height = value;
        if (value != oldvalue)
          OnChanged();
      }
    }
    public virtual float Width
    {
      get
      {
        return _bounds.Width;
      }
      set
      {
        float oldvalue = _bounds.Width;
        _bounds.Width = value;
        if (value != oldvalue)
          OnChanged();
      }
    }

    public virtual void SetSize(SizeF Value)
    {
      SizeF oldvalue = _bounds.Size;
      _bounds.Size = Value;
      if (Value != oldvalue)
        OnChanged();
    }
    public virtual SizeF GetSize()
    {
      return this._bounds.Size;
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
        return _rotation;
      }
      set
      {
        float oldvalue = _rotation;
        _rotation = value;
        if (value != oldvalue)
          OnChanged();
      }
    }


    protected void TransformGraphics(Graphics g)
    {
      g.TranslateTransform(X, Y);
      if (_rotation!=0)
        g.RotateTransform(-_rotation);
      if (_scaleX != 1 || _scaleY != 1)
        g.ScaleTransform(_scaleX, _scaleY);
      if(_shear!=0)
        g.MultiplyTransform(new Matrix(1,0,_shear,1,0,0));
    }


		protected void UpdateTransformationMatrices()
		{
      _transfoToLayerCoord.SetTranslationRotationScaleShear(X, Y, -_rotation, _scaleX, _scaleY, _shear);
		}


    public abstract void Paint(Graphics g, object obj);
    #region IChangedEventSource Members



    protected void EhChildChanged(object sender, EventArgs e)
    {
      OnChanged();
    }


    protected virtual void OnChanged()
    {
			UpdateTransformationMatrices();

      if(this._parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this,new Main.ChangedEventArgs(this,null));

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
    public virtual IHitTestObject HitTest(HitTestData hitData)
    {
			PointF pt = hitData.GetHittedPointInWorldCoord();
      GraphicsPath gp = GetObjectPath();
			if (gp.IsVisible(pt))
			{
				return new HitTestObject(gp, this);
			}
			else
			{
				return null;
			}
    }

		public virtual IHitTestObject HitTest(RectangleF rect)
		{
			// is this object contained within the supplied rectangle

			GraphicsPath gp = new GraphicsPath();
			Matrix myMatrix = new Matrix();


			gp.AddRectangle(new RectangleF(X + _bounds.X, Y + _bounds.Y, Width, Height));
			if (this.Rotation != 0)
			{
				myMatrix.RotateAt(-this._rotation, new PointF(this.X, this.Y), MatrixOrder.Append);
			}
			gp.Transform(myMatrix);
			RectangleF gpRect = gp.GetBounds();

			return rect.Contains(gpRect) ? new HitTestObject(gp, this) : null;
		}

    public virtual GraphicsPath GetSelectionPath()
    {
      return GetObjectPath();
    }
    public virtual GraphicsPath GetObjectPath()
    {
      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();

      gp.AddRectangle(new RectangleF(X + _bounds.X, Y + _bounds.Y, Width, Height));
      if (this.Rotation != 0)
      {
        myMatrix.RotateAt(-this._rotation, new PointF(X, Y), MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
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
      double dx = p.X * _bounds.Width;
      double dy = p.Y * _bounds.Height;

      
     dx += _bounds.X;
     dy += _bounds.Y;
     

      if (withRotation && _rotation != 0)
      {
        double cosphi = Math.Cos(_rotation * Math.PI / 180);
        double sinphi = Math.Sin(-_rotation * Math.PI / 180);

        double helpdx = (dx * cosphi - dy * sinphi);
        dy = (dy * cosphi + dx * sinphi);
        dx = helpdx;

      }

      if (withRotation)
        return new PointF((float)(_position.X + dx), (float)(_position.Y + dy));
      else
        return new PointF((float)(dx), (float)(dy));
    }

    public SizeF ToUnrotatedDifference(PointF pivot, PointF point)
    {
      double dx = point.X - pivot.X;
      double dy = point.Y - pivot.Y;

      if (_rotation != 0)
      {
        double cosphi = Math.Cos(_rotation * Math.PI / 180);
        double sinphi = Math.Sin(-_rotation * Math.PI / 180);
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

      if (_rotation != 0)
      {
        double cosphi = Math.Cos(_rotation * Math.PI / 180);
        double sinphi = Math.Sin(-_rotation * Math.PI / 180);
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

      if (_rotation != 0)
      {
        double cosphi = Math.Cos(_rotation * Math.PI / 180);
        double sinphi = Math.Sin(-_rotation * Math.PI / 180);
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
        this._bounds.Width = diff.Width;
      }
      else if (dx == -1 && (diff.Width < 0 || AllowNegativeSize))
      {
        SizeF s = ToRotatedDifference(PointF.Empty, new PointF(diff.Width + _bounds.Width, 0));
        this._position.X += s.Width;
        this._position.Y += s.Height;
        this._bounds.Width = -diff.Width;
      }

      if (dy == 1 && (diff.Height > 0 || AllowNegativeSize))
      {
        this._bounds.Height = diff.Height;
      }
      else if (dy == -1 && (diff.Height < 0 || AllowNegativeSize))
      {
        SizeF s = ToRotatedDifference(PointF.Empty, new PointF(0, diff.Height + _bounds.Height));
        this._position.X += s.Width;
        this._position.Y += s.Height;
        this._bounds.Height = -diff.Height;
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
      double dx = (relDrawGrip.X - relPivot.X) * _bounds.Width;
      double dy = (relDrawGrip.Y - relPivot.Y) * _bounds.Height;
      double a1 = Math.Atan2(dy, dx);
      double a2 = Math.Atan2(diff.Height, diff.Width);

      this._rotation = -(float)(180 * (a2 - a1) / Math.PI);

      //SizeF s = ToRotatedDifference(PointF.Empty, new PointF(-m_Bounds.Width / 2, -m_Bounds.Height / 2));
      SizeF s = ToRotatedDifference(RelativeToAbsolutePosition(relPivot, false), Point.Empty);
      this._position.X = absPivot.X + s.Width;
      this._position.Y = absPivot.Y + s.Height;
			UpdateTransformationMatrices();

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

		static readonly PointF[] _gripRelPositions = new PointF[]
			{
			new PointF(0.5f, 0.5f),
			new PointF(0, 0),
			new PointF(0.5f, 0),
			new PointF(1, 0), 
			new PointF(1, 0.5f), 
			 new PointF(1, 1),
			new PointF(0.5f, 1),
			new PointF(0, 1), 
			new PointF(0, 0.5f), 
			};

		public virtual IGripManipulationHandle[] ShowGrips(Graphics g)
		{
			return ShowGrips(g, true, true, true);
		}


		public static void TransformToUnscaledPageCoordinates(Graphics g, PointF[] pts)
		{
			// Transform points to page coordinates
			g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, pts);
			for (int i = 0; i < pts.Length; i++)
				pts[i] = pts[i].Scale(g.PageScale); // transform to absolute page coordinates, thus 1 pt is really 1 pt at the monitor

		}


    public virtual IGripManipulationHandle[] ShowGrips(Graphics g, bool showResizeGrips, bool showRotationGrips, bool showEnclosingRectangle)
    {
			List<IGripManipulationHandle> list = new List<IGripManipulationHandle>();
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (_rotation != 0)
        g.RotateTransform(-_rotation);


      PointF[] pts = new PointF[_gripRelPositions.Length];
			for(int i=0;i<pts.Length;i++)
				pts[i] = RelativeToAbsolutePosition(_gripRelPositions[i], false);
			
      // Transform points to page coordinates
			TransformToUnscaledPageCoordinates(g, pts);

			if (showResizeGrips)
			{
				for (int i = 1; i < pts.Length; i++)
				{
					PointF outVec = pts[i].Subtract(pts[0]);
					outVec = outVec.Scale(10 / outVec.VectorLength());
					PointF altVec = new PointF(-outVec.Y, outVec.X);
					PointF ptStart = pts[i].AddScaled(outVec, 0.25f);
					using (Matrix m = new Matrix(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y))
					{
						var gripShape = (GraphicsPath)_outsideArrow.Clone();
						gripShape.Transform(m);
						list.Add(new SizeMoveGripHandle(this, _gripRelPositions[i], gripShape));
					}
				}
			}

			if (showRotationGrips)
			{
				// Rotation grips
				for (int i = 1; i < pts.Length; i += 2)
				{
					PointF outVec = pts[i].Subtract(pts[0]);
					outVec = outVec.Scale(10 / outVec.VectorLength());
					PointF altVec = new PointF(-outVec.Y, outVec.X);
					PointF ptStart = pts[i].AddScaled(outVec, 0.5f);
					using (Matrix m = new Matrix(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y))
					{
						var gripShape = (GraphicsPath)_rotationGripShape.Clone();
						gripShape.Transform(m);
						list.Add(new RotationGripHandle(this, _gripRelPositions[i], gripShape));
					}
				}
			}

			if (showEnclosingRectangle)
			{
				g.DrawRectangle(Pens.Blue, _bounds.X, _bounds.Y, _bounds.Width, _bounds.Height);
			}

			g.Restore(gs);
/*
			var g2 = g.Save();
			g.ResetTransform();
			g.PageScale = 1;
      using(var pen = new Pen(Color.Red,0))
      {
        for (int i = 1; i < pts.Length; i++)
        {
          PointF outVec = pts[i].Subtract(pts[0]);
          outVec = outVec.Scale(10 / outVec.VectorLength());
          PointF altVec = new PointF(-outVec.Y, outVec.X);
          PointF ptStart = pts[i].AddScaled(outVec, 0.25f);
          using (Matrix m = new Matrix(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y))
          {
            //g.Transform = m;
            //g.FillPath(Brushes.Red, _outsideArrow);
						//var gripShape = (GraphicsPath)_outsideArrow.Clone();
						//gripShape.Transform(m);
						//list.Add(new SizeMoveGripHandle(this, _gripRelPositions[i], gripShape));
          }
        }
      }
     
			g.Restore(g2);


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

      g.DrawRectangle(Pens.Blue, _bounds.X,_bounds.Y,_bounds.Width,_bounds.Height);

      g.Restore(gs);
			*/
			return list.ToArray();
    }

  
    #endregion

    #region GripHandle

    protected class SizeMoveGripHandle : IGripManipulationHandle
    {
      GraphicBase _parent;
      PointF _drawrPosition;
      PointF _fixrPosition;
      PointF _fixaPosition;
			GraphicsPath _gripPath;
			static Pen _penForInsidePathTesting = new Pen(Color.Black, 0);


      public SizeMoveGripHandle(GraphicBase parent, PointF relPos, GraphicsPath path)
      {
        _parent = parent;
        _drawrPosition = relPos;
        _fixrPosition = new PointF(relPos.X == 0 ? 1 : 0, relPos.Y == 0 ? 1 : 0);
        _fixaPosition = _parent.RelativeToAbsolutePosition(_fixrPosition, true);

				_gripPath = path;
      }

      public void MoveGrip(PointF newPosition)
      {
        SizeF diff = _parent.ToUnrotatedDifference(_fixaPosition, newPosition);
        _parent.SetBoundsFrom(_fixrPosition, _drawrPosition, diff);
      }

			#region IGripManipulationHandle Members


			public void Show(Graphics g)
			{
				g.FillPath(Brushes.Blue, _gripPath);
			}

			public bool IsGripHitted(PointF point)
			{
				return _gripPath.IsVisible(point);
			}

			public IGrippableObject ManipulatedObject
			{
				get { return _parent; }
			}

			#endregion

		


			

		
		}

		

    protected class RotationGripHandle : IGripManipulationHandle
    {
      GraphicBase _parent;
      PointF _drawrPosition;
      PointF _fixrPosition;
      PointF _fixaPosition;
			GraphicsPath _gripPath;
			static Pen _penForInsidePathTesting = new Pen(Color.Black, 0);

			public RotationGripHandle(GraphicBase parent, PointF relPos, GraphicsPath path)
      {
        _parent = parent;
        _drawrPosition = relPos;
        _fixrPosition = new PointF(0.5f, 0.5f);
        _fixaPosition = _parent.RelativeToAbsolutePosition(_fixrPosition, true);
				_gripPath = path;
      }

      public void MoveGrip(PointF newPosition)
      {
        SizeF diff = new SizeF();

        diff.Width = newPosition.X - _fixaPosition.X;
        diff.Height = newPosition.Y - _fixaPosition.Y;
        _parent.SetRotationFrom(_fixrPosition, _fixaPosition, _drawrPosition, diff);
      }

			public void Show(Graphics g)
			{
				g.FillPath(Brushes.Blue, _gripPath);
			}

			public bool IsGripHitted(PointF point)
			{
				return _gripPath.IsVisible(point);
			}

			public IGrippableObject ManipulatedObject
			{
				get { return _parent; }
			}
    }


		/// <summary>
		/// Shows a single round grip, which can be customized to a move action.
		/// </summary>
		protected class PathNodeGripHandle : IGripManipulationHandle
		{
			PointF _gripCenter;
			float _gripRadius=3;
			GraphicBase _parent;
			PointF _drawrPosition;
			PointF _fixrPosition;
			PointF _fixaPosition;

			Action<PointF> _moveAction;

			public static Pen PathOutlinePen = new Pen(Color.Blue, 0);

			public PathNodeGripHandle(GraphicBase parent, PointF relPos, PointF gripCenter)
      {
        _parent = parent;
        _drawrPosition = relPos;
        _fixrPosition = new PointF(relPos.X == 0 ? 1 : 0, relPos.Y == 0 ? 1 : 0);
        _fixaPosition = _parent.RelativeToAbsolutePosition(_fixrPosition, true);

				_gripCenter = gripCenter;
      }


			public PathNodeGripHandle(GraphicBase parent, PointF relPos, PointF gripCenter, Action<PointF> moveAction)
				: this(parent,relPos,gripCenter)
			{
				_moveAction = moveAction;
			}

      

			#region IGripManipulationHandle Members

			public void MoveGrip(PointF newPosition)
			{
				if (_moveAction != null)
				{
					_moveAction(newPosition);
				}
				else
				{
					SizeF diff = _parent.ToUnrotatedDifference(_fixaPosition, newPosition);
					_parent.SetBoundsFrom(_fixrPosition, _drawrPosition, diff);
				}
			}


			public void Show(Graphics g)
			{
				g.FillEllipse(Brushes.Blue, _gripCenter.X - _gripRadius, _gripCenter.Y - _gripRadius, 2 * _gripRadius, 2 * _gripRadius);
			}

			public bool IsGripHitted(PointF point)
			{
				return (Calc.RMath.Pow2(point.X - _gripCenter.X) + Calc.RMath.Pow2(point.Y - _gripCenter.Y)) < Calc.RMath.Pow2(_gripRadius);
			}

			public IGrippableObject ManipulatedObject
			{
				get { return _parent; }
			}

			#endregion
		}

    #endregion


    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public virtual string Name
    {
      get { return this.GetType().ToString(); }
    }

    #endregion

		

  }
}
