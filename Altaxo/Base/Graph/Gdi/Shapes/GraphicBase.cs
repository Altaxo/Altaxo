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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
	/// <summary>
	/// The abstract base class for general graphical objects on the layer,
	/// for instance text elements, lines, pictures, rectangles and so on.
	/// </summary>
	[Serializable]
	public abstract partial class GraphicBase
		:
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>,
		Altaxo.Graph.Gdi.Shapes.IGraphicBase
	{
		/// <summary>
		/// The size of the parent object.
		/// </summary>
		protected PointD2D _cachedParentSize;

		/// <summary>
		/// The item's location (size, position, rotation, shear, scale ..)
		/// </summary>
		/// <remarks>The location is the vector from the reference point of the parent (normally the left upper corner of the parent) to the reference point of this object (normally
		/// also the left upper corner of the object).</remarks>
		protected ItemLocationDirect _location;

		/// <summary>Cached matrix which transforms from own coordinates to parent (layer) coordinates.</summary>
		protected TransformationMatrix2D _transformation = new TransformationMatrix2D();

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphicsObject", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

				var position = (PointF)info.GetValue("Position", s);
				var bounds = (RectangleF)info.GetValue("Bounds", s);
				var rotation = -info.GetSingle("Rotation"); // meaning of rotation reversed in version 2

				var locationAsAutoSized = s._location as ItemLocationDirectAutoSize;
				if (null != locationAsAutoSized)
				{
					locationAsAutoSized.SetSizeInAutoSizeMode(new PointD2D(bounds.Width, bounds.Height));
				}
				else
				{
					s._location.SizeX = RADouble.NewAbs(bounds.Width);
					s._location.SizeY = RADouble.NewAbs(bounds.Height);
				}

				s._location.PositionX = RADouble.NewAbs(position.X);
				s._location.PositionY = RADouble.NewAbs(position.Y);
				s._location.Rotation = rotation;

				/*s._autoSize =*/
				info.GetBoolean("AutoSize");
				s.UpdateTransformationMatrix();

				return s;
			}
		}

		// 2007-01-10 meaning of rotation was reversed
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
				/*
				 GraphicBase s = (GraphicBase)obj;
				info.AddValue("Position", s._position);
				info.AddValue("Bounds", s._bounds);
				info.AddValue("Rotation", s._rotation);
				info.AddValue("AutoSize", s._autoSize);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphicBase s = (GraphicBase)o;

				var position = (PointF)info.GetValue("Position", s);
				var bounds = (RectangleF)info.GetValue("Bounds", s);
				var rotation = info.GetSingle("Rotation");

				var locationAsAutoSized = s._location as ItemLocationDirectAutoSize;
				if (null != locationAsAutoSized)
				{
					locationAsAutoSized.SetSizeInAutoSizeMode(new PointD2D(bounds.Width, bounds.Height));
				}
				else
				{
					s._location.SizeX = RADouble.NewAbs(bounds.Width);
					s._location.SizeY = RADouble.NewAbs(bounds.Height);
				}

				s._location.PositionX = RADouble.NewAbs(position.X);
				s._location.PositionY = RADouble.NewAbs(position.Y);
				s._location.Rotation = rotation;

				/*s._autoSize = */
				info.GetBoolean("AutoSize");
				s.UpdateTransformationMatrix();

				return s;
			}
		}

		// 2010-03-16 ScaleX, ScaleY, and Shear added
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase), 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
				/*
				GraphicBase s = (GraphicBase)obj;
				info.AddValue("Position", s._position);
				info.AddValue("Bounds", s._bounds);
				info.AddValue("Rotation", s._rotation);
				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
				info.AddValue("Shear", s._shear);
				info.AddValue("AutoSize", s._autoSize);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphicBase s = (GraphicBase)o;

				var position = (PointF)info.GetValue("Position", s);
				var bounds = (RectangleF)info.GetValue("Bounds", s);
				var rotation = info.GetSingle("Rotation");
				var scaleX = info.GetSingle("ScaleX");
				var scaleY = info.GetSingle("ScaleY");
				var shear = info.GetSingle("Shear");

				var locationAsAutoSized = s._location as ItemLocationDirectAutoSize;
				if (null != locationAsAutoSized)
				{
					locationAsAutoSized.SetSizeInAutoSizeMode(new PointD2D(bounds.Width, bounds.Height));
				}
				else
				{
					s._location.SizeX = RADouble.NewAbs(bounds.Width);
					s._location.SizeY = RADouble.NewAbs(bounds.Height);
				}

				s._location.PositionX = RADouble.NewAbs(position.X);
				s._location.PositionY = RADouble.NewAbs(position.Y);
				s._location.Rotation = rotation;
				s._location.ScaleX = scaleX;
				s._location.ScaleY = scaleY;
				s._location.ShearX = shear;

				/*s._autoSize = */
				info.GetBoolean("AutoSize");
				s.UpdateTransformationMatrix();

				return s;
			}
		}

		// 2010-03-16 ScaleX, ScaleY, and Shear added
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.GraphicBase", 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				GraphicBase s = (GraphicBase)obj;
				info.AddValue("Width", s.Width);
				info.AddValue("Height", s.Height);
				info.AddValue("OffsetX", s._leftUpper.X);
				info.AddValue("OffsetY", s._leftUpper.Y);
				info.AddValue("X", s.X);
				info.AddValue("Y", s.Y);
				info.AddValue("Rotation", s._rotation);
				info.AddValue("ScaleX", s._scaleX);
				info.AddValue("ScaleY", s._scaleY);
				info.AddValue("Shear", s._shear);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphicBase s = (GraphicBase)o;

				double w = info.GetDouble("Width");
				double h = info.GetDouble("Height");
				double left = info.GetDouble("OffsetX");
				double top = info.GetDouble("OffsetY");
				double x = info.GetDouble("X");
				double y = info.GetDouble("Y");

				var rotation = info.GetSingle("Rotation");
				var scaleX = info.GetSingle("ScaleX");
				var scaleY = info.GetSingle("ScaleY");
				var shear = info.GetSingle("Shear");
				var locationAsAutoSized = s._location as ItemLocationDirectAutoSize;
				if (null != locationAsAutoSized)
				{
					locationAsAutoSized.SetSizeInAutoSizeMode(new PointD2D(w, h));
				}
				else
				{
					s._location.SizeX = RADouble.NewAbs(w);
					s._location.SizeY = RADouble.NewAbs(h);
				}

				s._location.PositionX = RADouble.NewAbs(x);
				s._location.PositionY = RADouble.NewAbs(y);
				s._location.Rotation = rotation;
				s._location.ScaleX = scaleX;
				s._location.ScaleY = scaleY;
				s._location.ShearX = shear;

				s.UpdateTransformationMatrix();

				return s;
			}
		}

		// 2013-11-27
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicBase), 5)]
		private class XmlSerializationSurrogate5 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphicBase s = (GraphicBase)obj;
				info.AddValue("Location", s._location);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphicBase s = (GraphicBase)o;

				if (null != s._location) s._location.Dispose(); // because location probably is set already in the derived object
				s._location = (ItemLocationDirect)info.GetValue("Location", s);
				if (null != s._location) s._location.ParentObject = s;

				s.UpdateTransformationMatrix();

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initializes a fresh instance of this class with default values
		/// </summary>
		protected GraphicBase(ItemLocationDirect location)
		{
			if (null == location)
				throw new ArgumentNullException("location");

			_location = location;
			_location.ParentObject = this;
		}

		protected GraphicBase(GraphicBase from)
		{
			if (null == from)
				throw new ArgumentNullException("from");

			_location = from._location.Clone();
			_location.ParentObject = this;

			CopyFrom(from);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as GraphicBase;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				this._cachedParentSize = from._cachedParentSize;
				this._location.CopyFrom(from._location);
				bool wasUsed = (null != this._parent);
				//this._parent = from._parent;
				this.UpdateTransformationMatrix();

				if (wasUsed)
				{
					_accumulatedEventData = EventArgs.Empty;
					suspendToken.Resume();
				}
				else
				{
					suspendToken.ResumeSilently();
				}
			}

			return true;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _location)
				yield return new Main.DocumentNodeAndName(_location, () => _location = null, "Location");
		}

		#region Suspend/Resume

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			_accumulatedEventData = EventArgs.Empty;
		}

		protected override bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			UpdateTransformationMatrix();

			return base.HandleLowPriorityChildChangeCases(sender, ref e);
		}

		#endregion Suspend/Resume

		public void SetParentSize(PointD2D parentSize, bool shouldTriggerChangeEvent)
		{
			var oldParentSize = _location.ParentSize;
			_location.SetParentSize(parentSize, false); // do not trigger change event here

			if (oldParentSize != parentSize)
			{
				UpdateTransformationMatrix(); // update the matrix in every case

				if (shouldTriggerChangeEvent)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public PointD2D ParentSize
		{
			get
			{
				return _location.ParentSize;
			}
		}

		public virtual bool AutoSize
		{
			get
			{
				return false;
			}
		}

		public ItemLocationDirect Location
		{
			get
			{
				return _location;
			}
		}

		/// <summary>
		/// Get/sets the x position of the reference point of the object in layer coordinates.
		/// </summary>
		public virtual double X
		{
			get
			{
				return _location.AbsolutePositionX;
			}
			set
			{
				_location.AbsolutePositionX = value;
			}
		}

		/// <summary>
		/// Get/sets the y position of the reference point of the object in layer coordinates.
		/// </summary>
		public virtual double Y
		{
			get
			{
				return _location.AbsolutePositionY;
			}
			set
			{
				_location.AbsolutePositionY = value;
			}
		}

		/// <summary>
		/// Gets the bound of the object. The X and Y positions depend on the transformation model chosen for this graphic object: if the transformation takes into account the local anchor point,
		/// then the X and Y of the bounds are always 0. If the transformation does not take the local anchor point into account, then (X and Y) is the vector from the local anchor point to the
		/// upper left corner of the graphical object.
		/// </summary>
		/// <value>
		/// The bounds of the graphical object.
		/// </value>
		public virtual RectangleD Bounds
		{
			get
			{
				return new RectangleD(_location.AbsoluteVectorPivotToLeftUpper, Size);
			}
		}

		/// <summary>
		/// Returns the information if this object allows negative sizes.
		/// </summary>
		public virtual bool AllowNegativeSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Returns the position of the pivot point of the object in parent coordinates (strictly speaking: as vector from the parent's reference point to the pivot point of the object).
		/// </summary>
		/// <returns>The position of the object (the object's pivot point) with reference to the parent's reference point).</returns>
		protected virtual PointD2D GetPosition()
		{
			return _location.AbsolutePivotPosition;
		}

		/// <summary>
		/// Sets the position of the object's pivot point.
		/// </summary>
		/// <param name="value">The position to set (the object's pivot point) with reference to the parent's reference point).</param>
		/// <param name="eventFiring">Designates whether or not the change event should be fired if the value has changed.</param>
		protected virtual void SetPosition(PointD2D value, Main.EventFiring eventFiring)
		{
			_location.SetAbsolutePivotPosition(value, eventFiring);
			if (eventFiring == Main.EventFiring.Suppressed)
				UpdateTransformationMatrix(); // even if events are suppressed - update at least the transformation matrix
		}

		/// <summary>
		/// Sets the position of the object without causing a Changed event.
		/// </summary>
		/// <param name="newPosition"></param>
		public virtual void SilentSetPosition(PointD2D newPosition)
		{
			this.SetPosition(newPosition, Main.EventFiring.Suppressed);
		}

		/// <summary>
		/// Get/set the position of the object. This is defined as the vector from the parent's reference point to the object's pivot point.
		/// </summary>
		public PointD2D Position
		{
			get
			{
				return GetPosition();
			}
			set
			{
				SetPosition(value, Main.EventFiring.Enabled);
			}
		}

		/// <summary>
		/// Scales the position of an item according to the provided xscale and yscale. Can be called with null for the item (in this case nothing happens).
		/// </summary>
		/// <param name="o">The graphics object whose position is scaled.</param>
		/// <param name="xscale">The xscale ratio.</param>
		/// <param name="yscale">The yscale ratio.</param>
		public static void ScalePosition(IGraphicBase o, double xscale, double yscale)
		{
			if (o != null)
			{
				PointD2D oldP = o.Position;
				o.Position = new PointD2D((oldP.X * xscale), (oldP.Y * yscale));
			}
		}

		/// <summary>
		/// Gets/sets the height of the item. This is the unscaled height.
		/// </summary>
		public virtual double Height
		{
			get
			{
				return _location.AbsoluteSizeY;
			}
			set
			{
				_location.AbsoluteSizeY = value;
			}
		}

		/// <summary>
		/// Get/sets the width of the item. This is the unscaled width.
		/// </summary>
		public virtual double Width
		{
			get
			{
				return _location.AbsoluteSizeX;
			}
			set
			{
				_location.AbsoluteSizeX = value;
			}
		}

		/// <summary>
		/// Sets the size of the item.
		/// </summary>
		/// <param name="width">Unscaled width of the item.</param>
		/// <param name="height">Unscaled height of the item.</param>
		/// <param name="eventFiring">Designates whether the change event should be fired.</param>
		protected virtual void SetSize(double width, double height, Main.EventFiring eventFiring)
		{
			var oldWidth = Width;
			var oldHeight = Height;

			_location.SetAbsoluteSize(new PointD2D(width, height), eventFiring);

			if (eventFiring == Main.EventFiring.Enabled && (width != oldWidth || height != oldHeight))
				EhSelfChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Sets the size and position of this item in relative units, calculated from absolute values. Note that the ParentSize must be set prior to calling this function.
		/// </summary>
		/// <param name="absSize">Absolute size of the item.</param>
		/// <param name="absPos">Absolute position of the item.</param>
		public virtual void SetRelativeSizePositionFromAbsoluteValues(PointD2D absSize, PointD2D absPos)
		{
			_location.SetRelativeSizePositionFromAbsoluteValues(absSize, absPos);
		}

		/// <summary>
		/// Get/set the unscaled size of the item.
		/// </summary>
		public PointD2D Size
		{
			get
			{
				return new PointD2D(Width, Height);
			}
			set
			{
				SetSize(value.X, value.Y, Main.EventFiring.Enabled);
			}
		}

		/// <summary>
		/// Get/sets the rotation value, measured in degrees in counterclockwise direction.
		/// </summary>
		public virtual double Rotation
		{
			get
			{
				return _location.Rotation;
			}
			set
			{
				_location.Rotation = value;
			}
		}

		/// <summary>
		/// Get/sets the scale for the width of the item. Normally this number is one (1).
		/// </summary>
		public double ScaleX
		{
			get
			{
				return _location.ScaleX;
			}
		}

		/// <summary>
		/// Get/sets the scale for the height of the item. Normally this number is one (1).
		/// </summary>
		public virtual double ScaleY
		{
			get
			{
				return _location.ScaleY;
			}
		}

		public virtual PointD2D Scale
		{
			get
			{
				return new PointD2D(_location.ScaleX, _location.ScaleY);
			}
			set
			{
				_location.ScaleX = value.X;
				_location.ScaleY = value.Y;
			}
		}

		/// <summary>
		/// Get/sets the shear of the item. This is the factor, by which the item points are shifted in x direction, when doing a unit step in y direction.
		/// The shear is the tangents of the shear angle.
		/// </summary>
		public virtual double Shear
		{
			get
			{
				return _location.ShearX;
			}
			set
			{
				_location.ShearX = value;
			}
		}

		/// <summary>
		/// Transforms the graphics context is such a way, that the object can be drawn in local coordinates.
		/// </summary>
		/// <param name="g">Graphics context (should be saved beforehand).</param>
		protected virtual void TransformGraphics(Graphics g)
		{
			g.TranslateTransform((float)_location.AbsolutePivotPositionX, (float)_location.AbsolutePivotPositionY);
			if (Rotation != 0)
				g.RotateTransform((float)(-Rotation));
			if (Shear != 0)
				g.MultiplyTransform(new Matrix(1, 0, (float)Shear, 1, 0, 0));
			if (ScaleX != 1 || ScaleY != 1)
				g.ScaleTransform((float)ScaleX, (float)ScaleY);
		}

		/// <summary>
		/// Updates the internal transformation matrix to reflect the settings for position, rotation, scaleX, scaleY and shear. It is designed here by default so that
		/// the local anchor point of the object is located at the world coordinates (0,0). The transformation matrix update can be overridden in derived classes so
		/// that for instance the left upper corner of the object is located at (0,0).
		/// </summary>
		protected virtual void UpdateTransformationMatrix()
		{
			_transformation.SetTranslationRotationShearxScale(_location.AbsolutePivotPositionX, _location.AbsolutePivotPositionY, -Rotation, Shear, ScaleX, ScaleY);
		}

		/// <summary>
		/// Determines whether this graphical object is compatible with the parent specified in the argument.
		/// </summary>
		/// <param name="parentObject">The parent object.</param>
		/// <returns>
		///   <c>True</c> if this object is compatible with the parent object; otherwise <c>false</c>.
		/// </returns>
		public virtual bool IsCompatibleWithParent(object parentObject)
		{
			return true;
		}

		public virtual void FixupInternalDataStructures()
		{
		}

		/// <summary>
		/// Is called before the paint procedure is executed.
		/// </summary>
		/// <param name="context">The paint context.</param>
		public virtual void PaintPreprocessing(IPaintContext context)
		{
		}

		/// <summary>
		/// Paint the object in the graphics context.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="context">Additional information used to draw the object.</param>
		public abstract void Paint(Graphics g, IPaintContext context);

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		public abstract object Clone();

		#region HitTesting

		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public abstract GraphicsPath GetObjectOutlineForArrangements();

		/// <summary>
		/// Get the path that is often used in HitTestObject to show the selected outline of the object
		/// if multiple objects are selected.
		/// </summary>
		/// <returns>The object outline in object world coordinates.</returns>
		public GraphicsPath GetRectangularObjectOutline()
		{
			var result = new GraphicsPath();
			result.AddRectangle((RectangleF)Bounds);
			return result;
		}

		protected virtual IHitTestObject GetNewHitTestObject()
		{
			return new GraphicBaseHitTestObject(this);
		}

		/// <summary>
		/// Tests a mouse click, whether or not it hits the object.
		/// </summary>
		/// <param name="hitData">Data containing the position of the click and the transformations.</param>
		/// <returns>Null if the object is not hitted. Otherwise data to further process the hitted object.</returns>
		public virtual IHitTestObject HitTest(HitTestPointData hitData)
		{
			var pt = hitData.GetHittedPointInWorldCoord(_transformation);
			if (Bounds.Contains(pt))
			{
				return GetNewHitTestObject();
			}
			else
			{
				return null;
			}
		}

		public virtual IHitTestObject HitTest(HitTestRectangularData rectHit)
		{
			if (rectHit.IsCovering(Bounds))
				return GetNewHitTestObject();
			else
				return null;
		}

		#endregion HitTesting

		#region Hitting Helper functions

		/// <summary>
		/// Converts relative positions (0..1, 0..1) to absolute coordinates in the world coordinate system of the object.
		/// To convert this to layer coordinates, you have to transform it with the transformation matrix of this object.
		/// </summary>
		/// <param name="p">Relative coordinates of the rectangle (0,0 is the upper left corner, 1,1 is the lower right corner).</param>
		/// <returns>The absolute object coordinates of this point (not layer coordinates!).</returns>
		public PointD2D RelativeLocalToAbsoluteLocalCoordinates(PointD2D p)
		{
			var bounds = this.Bounds;
			return new PointD2D(bounds.Left + p.X * bounds.Width, bounds.Top + p.Y * bounds.Height);
		}

		/// <summary>
		/// Converts relative positions of the object (0..1, 0..1) to coordinates in the world coordinate system of the parent (normally the layer).
		/// </summary>
		/// <param name="p">Relative coordinates of the rectangle (0,0 is the upper left corner, 1,1 is the lower right corner).</param>
		/// <returns>The absolute parent coordinates of this point (i.e. normally layer coordinates).</returns>
		public PointD2D RelativeLocalToAbsoluteParentCoordinates(PointD2D p)
		{
			var bounds = this.Bounds;
			return _transformation.TransformPoint(new PointD2D(bounds.Left + p.X * bounds.Width, bounds.Top + p.Y * bounds.Height));
		}

		/// <summary>
		/// Converts relative positions (0..1, 0..1) to coordinates in the world coordinate system of the parent (normally the layer).
		/// </summary>
		/// <param name="p">Relative coordinates of the rectangle (0,0 is the upper left corner, 1,1 is the lower right corner).</param>
		/// <returns>The absolute parent coordinates of this point.</returns>
		public PointD2D RelativeLocalToAbsoluteParentVector(PointD2D p)
		{
			var bounds = this.Bounds;
			return _transformation.TransformVector(new PointD2D(p.X * bounds.Width + bounds.X, p.Y * bounds.Height + bounds.Y));
		}

		/// <summary>
		/// Calculates the difference vector between two points in parent coordinates and transforms the vector to local coordinates.
		/// </summary>
		/// <param name="pivot">Start point of the difference vector.</param>
		/// <param name="point">End point of the difference vector.</param>
		/// <returns>The difference vector in local coordinates.</returns>
		public PointD2D ParentCoordinatesToLocalDifference(PointD2D pivot, PointD2D point)
		{
			double dx = point.X - pivot.X;
			double dy = point.Y - pivot.Y;

			_transformation.InverseTransformVector(ref dx, ref dy);

			return new PointD2D(dx, dy);
		}

		/// <summary>
		/// Converts relative positions (0..1, 0..1) to absolute position of the rectangle, taking into account
		/// the current rotation.
		/// </summary>
		/// <param name="p">Relative coordinates of the rectangle (0,0 is the upper left corner, 1,1 is the lower right corner).</param>
		/// <param name="withRotation">If true, the coordinates are calculated taking the rotation into account.</param>
		/// <returns>The coordinates of this point.</returns>
		public PointD2D RelativeToAbsolutePosition(PointD2D p, bool withRotation)
		{
			var bounds = this.Bounds;

			double dx = p.X * bounds.Width;
			double dy = p.Y * bounds.Height;

			dx += bounds.X;
			dy += bounds.Y;

			if (withRotation && Rotation != 0)
			{
				double cosphi = Math.Cos(Rotation * Math.PI / 180);
				double sinphi = Math.Sin(-Rotation * Math.PI / 180);

				double helpdx = (dx * cosphi - dy * sinphi);
				dy = (dy * cosphi + dx * sinphi);
				dx = helpdx;
			}

			if (withRotation)
				return new PointD2D(X + dx, Y + dy);
			else
				return new PointD2D(dx, dy);
		}

		public PointD2D ToUnrotatedDifference(PointD2D pivot, PointD2D point)
		{
			double dx = point.X - pivot.X;
			double dy = point.Y - pivot.Y;

			if (Rotation != 0)
			{
				double cosphi = Math.Cos(Rotation * Math.PI / 180);
				double sinphi = Math.Sin(-Rotation * Math.PI / 180);
				// now we have to rotate backward to get the endpoint
				double helpdx = (dx * cosphi + dy * sinphi);
				dy = (-dx * sinphi + dy * cosphi);
				dx = helpdx;
			}

			return new PointD2D(dx, dy);
		}

		public PointD2D ToUnrotatedDifference(PointD2D diff)
		{
			double dx = diff.X;
			double dy = diff.Y;

			if (Rotation != 0)
			{
				double cosphi = Math.Cos(Rotation * Math.PI / 180);
				double sinphi = Math.Sin(-Rotation * Math.PI / 180);
				// now we have to rotate backward to get the endpoint
				double helpdx = (dx * cosphi + dy * sinphi);
				dy = (-dx * sinphi + dy * cosphi);
				dx = helpdx;
			}

			return new PointD2D(dx, dy);
		}

		public static PointD2D ToUnrotatedDifference(double rotation, PointD2D diff)
		{
			double dx = diff.X;
			double dy = diff.Y;

			if (rotation != 0)
			{
				double cosphi = Math.Cos(rotation * Math.PI / 180);
				double sinphi = Math.Sin(-rotation * Math.PI / 180);
				// now we have to rotate backward to get the endpoint
				double helpdx = (dx * cosphi + dy * sinphi);
				dy = (-dx * sinphi + dy * cosphi);
				dx = helpdx;
			}

			return new PointD2D(dx, dy);
		}

		public PointD2D ToUnrotatedCoordinates(PointD2D pivot, PointD2D point)
		{
			double dx = point.X - pivot.X;
			double dy = point.Y - pivot.Y;

			if (Rotation != 0)
			{
				double cosphi = Math.Cos(Rotation * Math.PI / 180);
				double sinphi = Math.Sin(-Rotation * Math.PI / 180);
				// now we have to rotate backward to get the endpoint
				double helpdx = (dx * cosphi + dy * sinphi);
				dy = (-dx * sinphi + dy * cosphi);
				dx = helpdx;
			}
			return new PointD2D((pivot.X + dx), (pivot.Y + dy));
		}

		public PointD2D ToRotatedDifference(PointD2D pivot, PointD2D point)
		{
			double dx = point.X - pivot.X;
			double dy = point.Y - pivot.Y;

			if (Rotation != 0)
			{
				double cosphi = Math.Cos(Rotation * Math.PI / 180);
				double sinphi = Math.Sin(-Rotation * Math.PI / 180);
				// now we have to rotate backward to get the endpoint
				double helpdx = (dx * cosphi - dy * sinphi);
				dy = (dx * sinphi + dy * cosphi);
				dx = helpdx;
			}
			return new PointD2D((dx), (dy));
		}

		/// <summary>
		/// Sets the bounds from.
		/// </summary>
		/// <param name="fixrPosition">The relative position of the object's edge or vertex, which is held fixed during the operation.</param>
		/// <param name="fixaPosition">The paramter <paramref name="fixrPosition"/>, converted to parent's coordinates.</param>
		/// <param name="relDrawGrip">The relative position of the draw grip (0..1, 0..1).</param>
		/// <param name="diff">The movement vector of the grip handle.</param>
		/// <param name="initialSize">The initial size of the object.</param>
		/// <param name="eventFiring">Designates whether or not the change event should be fired if the value has changed.</param>
		public void SetBoundsFrom(PointD2D fixrPosition, PointD2D fixaPosition, PointD2D relDrawGrip, PointD2D diff, PointD2D initialSize, Main.EventFiring eventFiring)
		{
			using (var suspendToken = SuspendGetToken())
			{
				var dx = relDrawGrip.X - fixrPosition.X;
				var dy = relDrawGrip.Y - fixrPosition.Y;

				var newWidth = initialSize.X + diff.X / (dx);
				var newHeight = initialSize.Y + diff.Y / (dy);

				var size = this.Size;
				if (Math.Abs(dx) == 1 && (newWidth > 0 || AllowNegativeSize))
					size.X = newWidth;
				if (Math.Abs(dy) == 1 && (newHeight > 0 || AllowNegativeSize))
					size.Y = newHeight;

				this.SetSize(size.X, size.Y, Main.EventFiring.Suppressed);

				var currFixaPos = RelativeLocalToAbsoluteParentCoordinates(fixrPosition);

				PointD2D currPos = GetPosition();
				this.SetPosition(new PointD2D(currPos.X + fixaPosition.X - currFixaPos.X, currPos.Y + fixaPosition.Y - currFixaPos.Y), Main.EventFiring.Suppressed);
				UpdateTransformationMatrix();

				suspendToken.Resume(eventFiring);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="relPivot">Pivot point in relative coordinates.</param>
		/// <param name="absPivot">Pivot point in absolute coordinates.</param>
		/// <param name="relDrawGrip">Grip point in relative coordinates.</param>
		/// <param name="diff">Difference between absolute grip point and absolute pivot point, in unrotated absolute coordinates.</param>
		/// <param name="eventFiring">Designates whether or not the change event should be fired if the value has changed.</param>
		public void SetRotationFrom(PointD2D relPivot, PointD2D absPivot, PointD2D relDrawGrip, PointD2D diff, Main.EventFiring eventFiring)
		{
			using (var token = SuspendGetToken())
			{
				double dx = (relDrawGrip.X - relPivot.X) * Width * ScaleX
									+ (relDrawGrip.Y - relPivot.Y) * Height * ScaleY * Shear;
				double dy = (relDrawGrip.Y - relPivot.Y) * Height * ScaleY;

				double a1 = Math.Atan2(dy, dx);
				double a2 = Math.Atan2(diff.Y, diff.X);

				this.Rotation = -(180 * (a2 - a1) / Math.PI);
				UpdateTransformationMatrix();
				var currFixaPos = RelativeLocalToAbsoluteParentCoordinates(relPivot);
				var currPos = this.GetPosition();
				this.SetPosition(new PointD2D(currPos.X + absPivot.X - currFixaPos.X, currPos.Y + absPivot.Y - currFixaPos.Y), Main.EventFiring.Suppressed);
				UpdateTransformationMatrix();

				token.Resume(eventFiring);
			}
		}

		public void SetScalesFrom(PointD2D fixrPosition, PointD2D fixaPosition, PointD2D relDrawGrip, PointD2D diff, double initialScaleX, double initialScaleY, Main.EventFiring eventFiring)
		{
			using (var token = SuspendGetToken())
			{
				double newScaleX = this.ScaleX;
				double newScaleY = this.ScaleY;

				double initialWidth = this.Width * initialScaleX;
				double initialHeight = this.Height * initialScaleY;

				double dx = relDrawGrip.X - fixrPosition.X;
				double dy = relDrawGrip.Y - fixrPosition.Y;

				if (dy != 0)
					newScaleY = initialScaleY + diff.Y / (dy * Height);
				if (dx != 0)
					newScaleX = initialScaleX + (diff.X - Shear * diff.Y) / (dx * Width);

				this.Scale = new PointD2D(newScaleX, newScaleY);
				UpdateTransformationMatrix();

				var currFixaPos = RelativeLocalToAbsoluteParentCoordinates(fixrPosition);
				var currPos = this.GetPosition();
				this.SetPosition(new PointD2D(currPos.X + fixaPosition.X - currFixaPos.X, currPos.Y + fixaPosition.Y - currFixaPos.Y), Main.EventFiring.Suppressed);
				UpdateTransformationMatrix();

				token.Resume(eventFiring);
			}
		}

		public void SetShearFrom(PointD2D fixrPosition, PointD2D fixaPosition, PointD2D relDrawGrip, PointD2D diff, double initialRotation, double initialShear, double initialScaleX, double initialScaleY, Main.EventFiring eventFiring)
		{
			using (var token = SuspendGetToken())
			{
				var newShear = this.Shear;
				var newRot = this.Rotation;
				var newScaleX = this.ScaleX;
				var newScaleY = this.ScaleY;

				double dx = relDrawGrip.X - fixrPosition.X;
				double dy = relDrawGrip.Y - fixrPosition.Y;

				// complicated case
				if (Math.Abs(dx) == 1)
				{
					double shearAngle = Math.Atan(initialShear);
					var diffHeight = dx * ToUnrotatedDifference(initialRotation + shearAngle * 180 / Math.PI, diff).Y;

					double b = Width * initialScaleX / Math.Sqrt(1 + initialShear * initialShear); // Width of the object perpendicular to the right side

					newShear = (initialShear + diffHeight / b);
					newRot = (initialRotation + (shearAngle - Math.Atan(newShear)) * 180 / Math.PI);
					newScaleX = (initialScaleX / Math.Sqrt((1 + initialShear * initialShear) / (1 + newShear * newShear)));
					newScaleY = (initialScaleY * Math.Sqrt((1 + initialShear * initialShear) / (1 + newShear * newShear)));
				}

				// simple case
				if (Math.Abs(dy) == 1)
				{
					var diffWidth = ToUnrotatedDifference(diff).X;
					newShear = (initialShear + diffWidth / (dy * ScaleY * Height));
				}

				this.Shear = newShear;
				this.Rotation = newRot;
				this.Scale = new PointD2D(newScaleX, newScaleY);
				UpdateTransformationMatrix();

				var currFixaPos = RelativeLocalToAbsoluteParentCoordinates(fixrPosition);
				var currPos = this.GetPosition();
				this.SetPosition(new PointD2D(currPos.X + fixaPosition.X - currFixaPos.X, currPos.Y + fixaPosition.Y - currFixaPos.Y), Main.EventFiring.Suppressed);
				UpdateTransformationMatrix();

				token.Resume(eventFiring);
			}
		}

		protected internal void SetCoordinatesByAppendTransformation(TransformationMatrix2D transform, Main.EventFiring eventFiring)
		{
			using (var token = SuspendGetToken())
			{
				var loctransform = _transformation.Clone(); // because the _transformation member will be overwritten when setting Position, Rotation, Scale and so on, we create a copy of it
				loctransform.AppendTransform(transform);
				this.SetPosition(new PointD2D(loctransform.X, loctransform.Y), eventFiring);
				this.Rotation = -loctransform.Rotation;
				this.Scale = new PointD2D(loctransform.ScaleX, loctransform.ScaleY);
				this.Shear = loctransform.Shear;

				token.Resume(eventFiring);
			}
		}

		protected internal void SetCoordinatesByAppendInverseTransformation(TransformationMatrix2D transform, Main.EventFiring eventFiring)
		{
			using (var token = SuspendGetToken())
			{
				_transformation.AppendInverseTransform(transform);
				this.SetPosition(new PointD2D(_transformation.X, _transformation.Y), eventFiring);
				this.Rotation = -_transformation.Rotation;
				this.Scale = new PointD2D(_transformation.ScaleX, _transformation.ScaleY);
				this.Shear = _transformation.Shear;

				token.Resume(eventFiring);
			}
		}

		protected internal void ShiftPosition(PointD2D dp)
		{
			ShiftPosition(dp.X, dp.Y);
		}

		protected internal void ShiftPosition(double dx, double dy)
		{
			var currPos = GetPosition();
			this.SetPosition(new PointD2D(currPos.X + dx, currPos.Y + dy), Main.EventFiring.Suppressed);
			UpdateTransformationMatrix();
		}

		#endregion Hitting Helper functions

		#region IGrippableObject Members

		private static readonly PointD2D[] _gripRelPositions = new PointD2D[]
			{
			new PointD2D(0.5, 0.5),
			new PointD2D(0, 0),
			new PointD2D(0.5, 0),
			new PointD2D(1, 0),
			new PointD2D(1, 0.5),
			 new PointD2D(1, 1),
			new PointD2D(0.5, 1),
			new PointD2D(0, 1),
			new PointD2D(0, 0.5),
			};

		[Flags]
		protected enum GripKind { Move = 1, Resize = 2, Rotate = 4, Rescale = 8, Shear = 16 }

		/// <summary>
		/// Gets an vector (not normalized), which points outward of the relative point in page coordinates.
		/// </summary>
		/// <param name="rel">Relative point. Should be located at an edge or corner.</param>
		/// <param name="hitTest">Hit test object used to transform to page coordinates.</param>
		/// <param name="pageCoord">Location of point rel in page coordinates.</param>
		/// <param name="outVec">An vector in page coordinates pointing outwards of the object.</param>
		private void GetOutVector(PointD2D rel, IHitTestObject hitTest, out PointD2D outVec, out PointD2D pageCoord)
		{
			// we distinguish between points at the edge and points at the corners of the rectangle
			// at the edges the outer vector is perpendicular to the edge
			// at the corner the outer vector is angle bisector of the two edges
			// inside the rectangle it is a line starting from the mid of the rectangle to the point

			bool isXZeroOrOne = rel.X == 0 || rel.X == 1;
			bool isYZeroOrOne = rel.Y == 0 || rel.Y == 1;

			if (isXZeroOrOne && isYZeroOrOne) // located at a corner
			{
				GetCornerOutVector(rel, hitTest, out outVec, out pageCoord);
			}
			else if (isXZeroOrOne || isYZeroOrOne) // located at an edge
			{
				GetEdgeOutVector(rel, hitTest, out outVec, out pageCoord);
			}
			else // located elsewhere
			{
				GetMiddleRayOutVector(rel, hitTest, out outVec, out pageCoord);
			}
		}

		/// <summary>
		/// Gets an vector (not normalized), which assumes that the given point is a corner.
		/// The calculated vector points in the direction of the angle bisector of the two edges.
		/// The returned vector and coordinates are in page coordinates.
		/// </summary>
		/// <param name="rel">Relative point. Should be located at an edge or corner.</param>
		/// <param name="hitTest">Hit test object used to transform to page coordinates.</param>
		/// <param name="pageCoord">Location of point rel in page coordinates.</param>
		/// <param name="outVec">An vector in page coordinates pointing outwards of the object.</param>
		protected void GetCornerOutVector(PointD2D rel, IHitTestObject hitTest, out PointD2D outVec, out PointD2D pageCoord)
		{
			PointD2D pt1 = rel;
			PointD2D pt2 = new PointD2D(1 - rel.X, rel.Y);
			PointD2D pt3 = new PointD2D(rel.X, 1 - rel.Y);

			pt1 = RelativeLocalToAbsoluteLocalCoordinates(pt1);
			pt1 = _transformation.TransformPoint(pt1);
			pt1 = pageCoord = hitTest.Transformation.TransformPoint(pt1);

			pt2 = RelativeLocalToAbsoluteLocalCoordinates(pt2);
			pt2 = _transformation.TransformPoint(pt2);
			pt2 = hitTest.Transformation.TransformPoint(pt2);

			pt3 = RelativeLocalToAbsoluteLocalCoordinates(pt3);
			pt3 = _transformation.TransformPoint(pt3);
			pt3 = hitTest.Transformation.TransformPoint(pt3);

			outVec = (pt1 - pt2).GetNormalized() + (pt1 - pt3).GetNormalized();
		}

		/// <summary>
		/// Gets an vector (not normalized), which points outward perpendicular to that edge where the given relative point is located.
		/// The returned vector and coordinates are in page coordinates.
		/// </summary>
		/// <param name="rel">Relative point. Should be located at an edge or corner.</param>
		/// <param name="hitTest">Hit test object used to transform to page coordinates.</param>
		/// <param name="pageCoord">Location of point rel in page coordinates.</param>
		/// <param name="outVec">An vector in page coordinates pointing outwards of the object.</param>
		private void GetEdgeOutVector(PointD2D rel, IHitTestObject hitTest, out PointD2D outVec, out PointD2D pageCoord)
		{
			// calculate the location of the middle point
			var pt0 = RelativeLocalToAbsoluteLocalCoordinates(new PointD2D(0.5, 0.5));
			pt0 = _transformation.TransformPoint(pt0);
			pt0 = hitTest.Transformation.TransformPoint(pt0);

			PointD2D pt1 = rel;
			PointD2D pt2 = (0 == rel.X || 1 == rel.X) ? new PointD2D(rel.X, rel.Y + 1) : new PointD2D(rel.X + 1, rel.Y);

			pt1 = RelativeLocalToAbsoluteLocalCoordinates(pt1);
			pt1 = _transformation.TransformPoint(pt1);
			pt1 = pageCoord = hitTest.Transformation.TransformPoint(pt1);

			pt2 = RelativeLocalToAbsoluteLocalCoordinates(pt2);
			pt2 = _transformation.TransformPoint(pt2);
			pt2 = hitTest.Transformation.TransformPoint(pt2);

			outVec = (pt1 - pt2).Get90DegreeRotated();
			PointD2D otherVec = pt1 - pt0;
			if (outVec.DotProduct(otherVec) < 0)
				outVec = outVec.GetSignFlipped();
		}

		/// <summary>
		/// Gets an vector (not normalized), which points outward in the direction of a ray
		/// between the middle point of the rectangle and the given point.
		/// The returned vector and coordinates are in page coordinates.
		/// </summary>
		/// <param name="rel">Relative point. Should be located at an edge or corner.</param>
		/// <param name="hitTest">Hit test object used to transform to page coordinates.</param>
		/// <param name="pageCoord">Location of point rel in page coordinates.</param>
		/// <param name="outVec">An vector in page coordinates pointing outwards of the object.</param>
		private void GetMiddleRayOutVector(PointD2D rel, IHitTestObject hitTest, out PointD2D outVec, out PointD2D pageCoord)
		{
			// calculate the location of the middle point
			var pt0 = RelativeLocalToAbsoluteLocalCoordinates(new PointD2D(0.5, 0.5));
			pt0 = _transformation.TransformPoint(pt0);
			pt0 = hitTest.Transformation.TransformPoint(pt0);

			var pt = RelativeLocalToAbsoluteLocalCoordinates(rel);
			pt = _transformation.TransformPoint(pt);
			pt = pageCoord = hitTest.Transformation.TransformPoint(pt);
			outVec = pt - pt0;
		}

		protected virtual IGripManipulationHandle[] GetGrips(IHitTestObject hitTest, double pageScale, GripKind gripKind)
		{
			List<IGripManipulationHandle> list = new List<IGripManipulationHandle>();
			const double gripNominalSize = 10; // 10 Points nominal size on the screen

			if ((GripKind.Resize & gripKind) != 0)
			{
				double gripSize = gripNominalSize / pageScale; // 10 Points, but we have to consider the current pageScale
				for (int i = 1; i < _gripRelPositions.Length; i++)
				{
					PointD2D outVec, pos;
					if (1 == i % 2)
						GetCornerOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);
					else
						GetMiddleRayOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

					outVec *= (gripSize / outVec.VectorLength);
					PointD2D altVec = outVec.Get90DegreeRotated();
					PointD2D ptStart = pos;
					list.Add(new ResizeGripHandle(hitTest, _gripRelPositions[i], new TransformationMatrix2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
				}
			}

			if ((GripKind.Rotate & gripKind) != 0)
			{
				double gripSize = 10 / pageScale;
				// Rotation grips
				for (int i = 1; i < _gripRelPositions.Length; i += 2)
				{
					PointD2D outVec, pos;
					GetCornerOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

					outVec *= (gripSize / outVec.VectorLength);
					PointD2D altVec = outVec.Get90DegreeRotated();
					PointD2D ptStart = pos;
					list.Add(new RotationGripHandle(hitTest, _gripRelPositions[i], new TransformationMatrix2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
				}
			}

			if ((GripKind.Rescale & gripKind) != 0)
			{
				double gripSize = 10 / pageScale; // 10 Points, but we have to consider the current pageScale
				for (int i = 1; i < _gripRelPositions.Length; i++)
				{
					PointD2D outVec, pos;
					if (1 == i % 2)
						GetCornerOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);
					else
						GetMiddleRayOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

					outVec *= (gripSize / outVec.VectorLength);
					PointD2D altVec = outVec.Get90DegreeRotated();
					PointD2D ptStart = pos;
					list.Add(new RescaleGripHandle(hitTest, _gripRelPositions[i], new TransformationMatrix2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
				}
			}

			if ((GripKind.Shear & gripKind) != 0)
			{
				double gripSize = 10 / pageScale; // 10 Points, but we have to consider the current pageScale
				for (int i = 2; i < _gripRelPositions.Length; i += 2)
				{
					PointD2D outVec, pos;
					GetEdgeOutVector(_gripRelPositions[i], hitTest, out outVec, out pos);

					outVec *= (gripSize / outVec.VectorLength);
					PointD2D altVec = outVec.Get90DegreeRotated();
					PointD2D ptStart = pos;
					list.Add(new ShearGripHandle(hitTest, _gripRelPositions[i], new TransformationMatrix2D(outVec.X, outVec.Y, altVec.X, altVec.Y, ptStart.X, ptStart.Y)));
				}
			}

			if ((GripKind.Move & gripKind) != 0)
			{
				var pts = new PointD2D[] { new PointD2D(0, 0), new PointD2D(1, 0), new PointD2D(1, 1), new PointD2D(0, 1) };
				var pathPts = new PointF[4];
				for (int i = 0; i < pts.Length; i++)
				{
					var pt = RelativeLocalToAbsoluteLocalCoordinates(pts[i]);
					pt = _transformation.TransformPoint(pt);
					pt = hitTest.Transformation.TransformPoint(pt);
					pathPts[i] = (PointF)pt;
				}
				GraphicsPath path = new GraphicsPath();
				path.AddPolygon(pathPts);
				list.Add(new MovementGripHandle(hitTest, path, null));
			}

			return list.ToArray();
		}

		#endregion IGrippableObject Members
	}
}