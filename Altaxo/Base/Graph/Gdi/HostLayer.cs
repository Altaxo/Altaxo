#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2013 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace Altaxo.Graph.Gdi
{
	using Axis;
	using Plot;
	using Shapes;

	public class HostLayer
		:
		ITreeListNodeWithParent<HostLayer>,
		IGraphicBase,
		Altaxo.Main.IDocumentNode,
		Altaxo.Main.INamedObjectCollection,
		Altaxo.Main.IChildChangedEventSink
	{
		#region Constants

		protected const double _xDefPositionLandscape = 0.14;
		protected const double _yDefPositionLandscape = 0.14;
		protected const double _xDefSizeLandscape = 0.76;
		protected const double _yDefSizeLandscape = 0.7;

		protected const double _xDefPositionPortrait = 0.14;
		protected const double _yDefPositionPortrait = 0.14;
		protected const double _xDefSizePortrait = 0.7;
		protected const double _yDefSizePortrait = 0.76;

		#endregion Constants

		#region Cached member variables

		/// <summary>
		/// The size of the area of the entire graph document.
		/// Needed to calculate "relative to page size" layer size values.
		/// </summary>
		protected PointD2D _cachedParentLayerSize;

		/// <summary>
		/// The cached layer position in points (1/72 inch) relative to the upper left corner
		/// of the graph document (upper left corner of the printable area).
		/// </summary>
		protected PointD2D _cachedLayerPosition;

		/// <summary>
		/// The size of the layer in points (1/72 inch).
		/// </summary>
		/// <remarks>
		/// In case the size is absolute (see <see cref="XYPlotLayerSizeType"/>), this is the size of the layer. Otherwise
		/// it is only the cached value for the size, since the size is calculated then.
		/// </remarks>
		protected PointD2D _cachedLayerSize = new PointD2D(0, 0);

		protected TransformationMatrix2D _transformation = new TransformationMatrix2D();

		[NonSerialized]
		protected Main.EventSuppressor _changeEventSuppressor;

		/// <summary>
		/// The child layers of this layers (this is a partial view of the <see cref="_graphObjects"/> collection).
		/// </summary>
		protected IObservableList<HostLayer> _childLayers;

		#endregion Cached member variables

		#region Member variables

		protected XYPlotLayerPositionAndSize _location;

		protected GraphicCollection _graphObjects;

		/// <summary>
		/// The parent layer collection which contains this layer (or null if not member of such collection).
		/// </summary>
		[NonSerialized]
		protected object _parent;

		#endregion Member variables

		#region Event definitions

		[field: NonSerialized]
		public event EventHandler Changed;

		/// <summary>Fired when the size of the layer changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler SizeChanged;

		/// <summary>Fired when the position of the layer changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler PositionChanged;

		/// <summary>Fired when the child layer collection changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler LayerCollectionChanged;

		#endregion Event definitions

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerCollection", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.XYPlotLayerCollection", 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				var s = (HostLayer)obj;

				info.CreateArray("LayerArray", s.Count);
				for (int i = 0; i < s.Count; i++)
					info.AddValue("XYPlotLayer", s[i]);
				info.CommitArray();
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (HostLayer)o : new HostLayer();

				int count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					XYPlotLayer l = (XYPlotLayer)info.GetValue("XYPlotLayer", s);
					s.Layers.Add(l);
				}
				info.CloseArray(count);

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.XYPlotLayerCollection", 2)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				XYPlotLayerCollection s = (XYPlotLayerCollection)obj;

				info.AddValue("Size", s._graphSize);

				info.CreateArray("LayerArray", s.Count);
				for (int i = 0; i < s.Count; i++)
					info.AddValue("XYPlotLayer", s[i]);
				info.CommitArray();
				 */
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (HostLayer)o : new HostLayer();

				var size = (SizeF)info.GetValue("Size", parent);
				s.SetSize(size.Width, XYPlotLayerSizeType.AbsoluteValue, size.Height, XYPlotLayerSizeType.AbsoluteValue);

				int count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					XYPlotLayer l = (XYPlotLayer)info.GetValue("XYPlotLayer", s);
					s.Layers.Add(l);
				}
				info.CloseArray(count);

				return s;
			}
		}

		#region Version 0

		/// <summary>
		/// In Version 0 we changed the Scales and divided into pure Scale and TickSpacing
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HostLayer), 0)]
		private class XmlSerializationSurrogate5 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				HostLayer s = (HostLayer)obj;

				throw new InvalidOperationException("Set cachedSize and cachedPosition to double objects");

				// size, position, rotation and scale
				info.AddValue("LocationAndSize", s._location);
				info.AddValue("CachedSize", s._cachedLayerSize);
				info.AddValue("CachedPosition", s._cachedLayerPosition);

				// Graphic objects
				info.AddValue("GraphObjects", s._graphObjects);
			}

			protected virtual HostLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				HostLayer s = (o == null ? new HostLayer() : (HostLayer)o);

				// size, position, rotation and scale
				s.Location = (XYPlotLayerPositionAndSize)info.GetValue("LocationAndSize", s);
				s._cachedLayerSize = (SizeF)info.GetValue("CachedSize", typeof(SizeF));
				s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition", typeof(PointF));

				// Graphic objects
				s.GraphObjects.AddRange((IEnumerable<IGraphicBase>)info.GetValue("GraphObjects", s));

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				s.CalculateMatrix();
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		#region Constructors

		#region Copying

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as HostLayer;
			if (null != from)
			{
				CopyFrom(from, GraphCopyOptions.All);
				return true;
			}
			return false;
		}

		public virtual void CopyFrom(HostLayer from, GraphCopyOptions options)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (IDisposable updateLock = BeginUpdate())
			{
				InternalCopyFrom(from, options);
			}

			// 2008-12-12: parent is neccessary for the layer dialog, otherwise linked layer properties are broken
			this._parent = from._parent; // outside the update, because clone operations should not cause an update of the old parent
			OnChanged(); // make sure that the change event is called
		}

		/// <summary>
		/// Internal copy from operation. It is presumed, that the events are already suspended. Additionally,
		/// it is not neccessary to call the OnChanged event, since this is called in the calling routine.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="options"></param>
		protected virtual void InternalCopyFrom(HostLayer from, GraphCopyOptions options)
		{
			// XYPlotLayer style
			//this.LayerBackground = from._layerBackground == null ? null : (LayerBackground)from._layerBackground.Clone();

			// size, position, rotation and scale
			if (0 != (options & GraphCopyOptions.CopyLayerSizePosition))
			{
				this.Location = from._location.Clone();
				this._cachedLayerSize = from._cachedLayerSize;
				this._cachedLayerPosition = from._cachedLayerPosition;
				this._cachedParentLayerSize = from._cachedParentLayerSize;
			}

			if (GraphCopyOptions.CopyLayerAll == (options & GraphCopyOptions.CopyLayerAll))
			{
				// The layers and graph itens should be cloned -> this is easy -> just clone the _graphObject collection
				using (this._graphObjects.GetEventDisableToken())
				{
					this._graphObjects.Clear();
					for (int i = 0; i < from._graphObjects.Count; i++)
					{
						var fromobj = from._graphObjects[i];
						if (!(fromobj is ILayerItemPlaceHolder) || ((ILayerItemPlaceHolder)fromobj).IsUsedForLayer(this)) // don't copy placeholders that are not intended for our type of layer
							this._graphObjects.Add((IGraphicBase)fromobj.Clone());
					}
				}
			}
			else if (0 != (options & GraphCopyOptions.CopyLayerAll))
			{
				// not all properties of the child layers should be cloned -> just copy the layers one by one
				int len = Math.Min(this._childLayers.Count, from._childLayers.Count);
				for (int i = 0; i < len; i++)
				{
					this._childLayers[i].CopyFrom(from._childLayers[i], options);
					this._childLayers[i].ParentLayer = this;
				}
			}

			_transformation = new TransformationMatrix2D();
			CalculateMatrix();
		}

		public virtual object Clone()
		{
			return new HostLayer(this);
		}

		#endregion Copying

		/// <summary>
		/// The copy constructor.
		/// </summary>
		/// <param name="from"></param>
		public HostLayer(HostLayer from)
		{
			_changeEventSuppressor = new Altaxo.Main.EventSuppressor(EhChangeEventResumed);
			InternalInitializeGraphObjectsCollection();
			CopyFrom(from, GraphCopyOptions.All);
		}

		/// <summary>
		/// Constructor for deserialization purposes only.
		/// </summary>
		protected HostLayer()
		{
			this._changeEventSuppressor = new Altaxo.Main.EventSuppressor(EhChangeEventResumed);
			InternalInitializeGraphObjectsCollection();
		}

		/// <summary>
		/// Creates a layer with position <paramref name="position"/> and size <paramref name="size"/>.
		/// </summary>
		/// <param name="position">The position of this layer on the parent in points (1/72 inch).</param>
		/// <param name="size">The size of this layer in points (1/72 inch).</param>
		public HostLayer(PointD2D position, PointD2D size)
		{
			this._changeEventSuppressor = new Altaxo.Main.EventSuppressor(EhChangeEventResumed);
			this.Location = new XYPlotLayerPositionAndSize();
			this.Size = size;
			this.Position = position;
			InternalInitializeGraphObjectsCollection();

			CalculateMatrix();
		}

		#endregion Constructors

		#region Position and Size

		/// <summary>
		/// Gets the default child layer position in points (1/72 inch).
		/// </summary>
		/// <value>The default position of a (new) layer in points (1/72 inch).</value>
		public PointD2D DefaultChildLayerPosition
		{
			get { return new PointD2D(0.145 * Size.X, 0.139 * Size.Y); }
		}

		/// <summary>
		/// Gets the default child layer size in points (1/72 inch).
		/// </summary>
		/// <value>The default size of a (new) layer in points (1/72 inch).</value>
		public PointD2D DefaultChildLayerSize
		{
			get { return new PointD2D(0.763 * Size.X, 0.708 * Size.Y); }
		}

		public XYPlotLayerPositionAndSize Location
		{
			get
			{
				return _location;
			}
			set
			{
				XYPlotLayerPositionAndSize oldvalue = _location;
				_location = value;
				value.ParentObject = this;

				if (!object.ReferenceEquals(oldvalue, value))
					OnChanged();
			}
		}

		/// <summary>
		/// Set this layer to the default size and position.
		/// </summary>
		/// <param name="parentSize">The size of the parent's area.</param>
		public void SizeToDefault(PointD2D parentSize)
		{
			if (parentSize.X > parentSize.Y)
			{
				this.Size = new PointD2D(parentSize.X * _xDefSizeLandscape, parentSize.Y * _yDefSizeLandscape);
				this.Position = new PointD2D(parentSize.X * _xDefPositionLandscape, parentSize.Y * _yDefPositionLandscape);
			}
			else // Portrait
			{
				this.Size = new PointD2D(parentSize.X * _xDefSizePortrait, parentSize.Y * _yDefSizePortrait);
				this.Position = new PointD2D(parentSize.X * _xDefPositionPortrait, parentSize.Y * _yDefPositionPortrait);
			}
			this.CalculateMatrix();
		}

		/// <summary>
		/// The boundaries of the printable area of the page in points (1/72 inch).
		/// </summary>
		public PointD2D ParentLayerSize
		{
			get { return _cachedParentLayerSize; }
		}

		public void SetParentLayerSize(PointD2D val, bool bRescale)
		{
			var oldSize = _cachedParentLayerSize;
			var newSize = val;
			_cachedParentLayerSize = val;

			if (_cachedParentLayerSize != oldSize && bRescale)
			{
				var oldLayerSize = this._cachedLayerSize;

				double oldxdefsize = oldSize.X * (oldSize.X > oldSize.Y ? _xDefSizeLandscape : _xDefSizePortrait);
				double newxdefsize = newSize.X * (newSize.X > newSize.Y ? _xDefSizeLandscape : _xDefSizePortrait);
				double oldydefsize = oldSize.Y * (oldSize.X > oldSize.Y ? _yDefSizeLandscape : _yDefSizePortrait);
				double newydefsize = newSize.Y * (newSize.X > newSize.Y ? _yDefSizeLandscape : _yDefSizePortrait);

				double oldxdeforg = oldSize.X * (oldSize.X > oldSize.Y ? _xDefPositionLandscape : _xDefPositionPortrait);
				double newxdeforg = newSize.X * (newSize.X > newSize.Y ? _xDefPositionLandscape : _xDefPositionPortrait);
				double oldydeforg = oldSize.Y * (oldSize.X > oldSize.Y ? _yDefPositionLandscape : _yDefPositionPortrait);
				double newydeforg = newSize.Y * (newSize.X > newSize.Y ? _yDefPositionLandscape : _yDefPositionPortrait);

				double xscale = newxdefsize / oldxdefsize;
				double yscale = newydefsize / oldydefsize;

				double xoffs = newxdeforg - oldxdeforg * xscale;
				double yoffs = newydeforg - oldydeforg * yscale;

				if (this._location.XPositionType == XYPlotLayerPositionType.AbsoluteValue)
					this._location.XPosition = xoffs + this._location.XPosition * xscale;

				if (this._location.WidthType == XYPlotLayerSizeType.AbsoluteValue)
					this._location.Width *= xscale;

				if (this._location.YPositionType == XYPlotLayerPositionType.AbsoluteValue)
					this._location.YPosition = yoffs + this._location.YPosition * yscale;

				if (this._location.HeightType == XYPlotLayerSizeType.AbsoluteValue)
					this._location.Height *= yscale;

				CalculateMatrix();
				this.CalculateCachedSize();
				this.CalculateCachedPosition();

				// scale the position of the inner items according to the ratio of the new size to the old size
				// note: only the size is important here, since all inner items are relative to the layer origin
				var newLayerSize = this._cachedLayerSize;
				xscale = newLayerSize.X / oldLayerSize.X;
				yscale = newLayerSize.Y / oldLayerSize.Y;

				RescaleInnerItemPositions(xscale, yscale);
			}
		}

		/// <summary>
		/// Recalculates the positions of inner items in case the layer has changed its size.
		/// </summary>
		/// <param name="xscale">The ratio the layer has changed its size in horizontal direction.</param>
		/// <param name="yscale">The ratio the layer has changed its size in vertical direction.</param>
		public virtual void RescaleInnerItemPositions(double xscale, double yscale)
		{
			foreach (IGraphicBase o in _graphObjects)
			{
				GraphicBase.ScalePosition(o, xscale, yscale);
			}
		}

		public PointD2D Position
		{
			get { return this._cachedLayerPosition; }
			set
			{
				SetPosition(value.X, XYPlotLayerPositionType.AbsoluteValue, value.Y, XYPlotLayerPositionType.AbsoluteValue);
			}
		}

		public PointD2D Size
		{
			get { return this._cachedLayerSize; }
			set
			{
				SetSize(value.X, XYPlotLayerSizeType.AbsoluteValue, value.Y, XYPlotLayerSizeType.AbsoluteValue);
			}
		}

		public double Rotation
		{
			get { return this._location.Angle; }
			set
			{
				this._location.Angle = value;
				this.CalculateMatrix();
				this.OnChanged();
			}
		}

		public double Scale
		{
			get { return this._location.Scale; }
			set
			{
				this._location.Scale = value;
				this.CalculateMatrix();
				this.OnChanged();
			}
		}

		protected void CalculateMatrix()
		{
			_transformation.Reset();
			_transformation.SetTranslationRotationShearxScale(_cachedLayerPosition.X, _cachedLayerPosition.Y, -_location.Angle, 0, _location.Scale, _location.Scale);
		}

		public PointD2D TransformCoordinatesFromParentToHere(PointD2D pagecoordinates)
		{
			return _transformation.InverseTransformPoint(pagecoordinates);
		}

		public PointD2D TransformCoordinatesFromRootToHere(PointD2D pagecoordinates)
		{
			foreach (var layer in this.TakeFromRootToHere())
				pagecoordinates = _transformation.InverseTransformPoint(pagecoordinates);
			return pagecoordinates;
		}

		public CrossF GraphToLayerCoordinates(CrossF x)
		{
			return new CrossF()
			{
				Center = _transformation.InverseTransformPoint(x.Center),
				Top = _transformation.InverseTransformPoint(x.Top),
				Bottom = _transformation.InverseTransformPoint(x.Bottom),
				Left = _transformation.InverseTransformPoint(x.Left),
				Right = _transformation.InverseTransformPoint(x.Right)
			};
		}

		/// <summary>
		/// This switches the graphics context from printable area coordinates to layer coordinates.
		/// </summary>
		/// <param name="g">The graphics state to change.</param>
		public void TransformCoordinatesFromParentToHere(Graphics g)
		{
			g.MultiplyTransform(_transformation);
		}

		/// <summary>
		/// Converts X,Y differences in page units to X,Y differences in layer units
		/// </summary>
		/// <param name="pagediff">X,Y coordinate differences in graph units</param>
		/// <returns>the convertes X,Y coordinate differences in layer units</returns>
		public PointD2D TransformCoordinateDifferencesFromParentToHere(PointD2D pagediff)
		{
			return _transformation.InverseTransformVector(pagediff);
		}

		/// <summary>
		/// Transforms a graphics path from layer coordinates to graph (page) coordinates
		/// </summary>
		/// <param name="gp">the graphics path to convert</param>
		/// <returns>graphics path now in graph coordinates</returns>
		public GraphicsPath TransformCoordinatesFromHereToParent(GraphicsPath gp)
		{
			gp.Transform(_transformation);
			return gp;
		}

		/// <summary>
		/// Transforms a <see cref="PointD2D" /> from layer coordinates to graph (=printable area) coordinates
		/// </summary>
		/// <param name="layerCoordinates">The layer coordinates to convert.</param>
		/// <returns>graphics path now in graph coordinates</returns>
		public PointD2D TransformCoordinatesFromHereToParent(PointD2D layerCoordinates)
		{
			return _transformation.TransformPoint(layerCoordinates);
		}

		public PointD2D TransformCoordinatesFromHereToRoot(PointD2D coordinates)
		{
			foreach (var layer in this.TakeFromHereToRoot())
				coordinates = _transformation.TransformPoint(coordinates);
			return coordinates;
		}

		public void SetPosition(double x, XYPlotLayerPositionType xpostype, double y, XYPlotLayerPositionType ypostype)
		{
			this._location.XPosition = x;
			this._location.XPositionType = xpostype;
			this._location.YPosition = y;
			this._location.YPositionType = ypostype;

			CalculateCachedPosition();
		}

		/// <summary>
		/// Calculates from the x position value, which can be absolute or relative, the
		/// x position in points.
		/// </summary>
		/// <param name="x">The horizontal position value of type xpostype.</param>
		/// <param name="xpostype">The type of the horizontal position value, see <see cref="XYPlotLayerPositionType"/>.</param>
		/// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double XPositionToPointUnits(double x, XYPlotLayerPositionType xpostype)
		{
			switch (xpostype)
			{
				case XYPlotLayerPositionType.AbsoluteValue:
					break;

				case XYPlotLayerPositionType.RelativeToGraphDocument:
					x = x * ParentLayerSize.X;
					break;

				default:
					throw new NotImplementedException(xpostype.ToString());
			}
			return x;
		}

		/// <summary>
		/// Calculates from the y position value, which can be absolute or relative, the
		///  y position in points.
		/// </summary>
		/// <param name="y">The vertical position value of type xpostype.</param>
		/// <param name="ypostype">The type of the vertical position value, see <see cref="XYPlotLayerPositionType"/>.</param>
		/// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double YPositionToPointUnits(double y, XYPlotLayerPositionType ypostype)
		{
			switch (ypostype)
			{
				case XYPlotLayerPositionType.AbsoluteValue:
					break;

				case XYPlotLayerPositionType.RelativeToGraphDocument:
					y = y * ParentLayerSize.Y;
					break;

				default:
					throw new NotImplementedException(ypostype.ToString());
			}

			return y;
		}

		/// <summary>
		/// Calculates from the x position value in points (1/72 inch), the corresponding value in user units.
		/// </summary>
		/// <param name="x">The vertical position value in points.</param>
		/// <param name="xpostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="XYPlotLayerPositionType"/>.</param>
		/// <returns>Calculated value of x in user units.</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double XPositionToUserUnits(double x, XYPlotLayerPositionType xpostype_to_convert_to)
		{
			switch (xpostype_to_convert_to)
			{
				case XYPlotLayerPositionType.AbsoluteValue:
					break;

				case XYPlotLayerPositionType.RelativeToGraphDocument:
					x = x / ParentLayerSize.X;
					break;

				default:
					throw new NotImplementedException(xpostype_to_convert_to.ToString());
			}

			return x;
		}

		/// <summary>
		/// Calculates from the y position value in points (1/72 inch), the corresponding value in user units.
		/// </summary>
		/// <param name="y">The vertical position value in points.</param>
		/// <param name="ypostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="XYPlotLayerPositionType"/>.</param>
		/// <returns>Calculated value of y in user units.</returns>
		/// <remarks>The function does not change the member variables of the layer and can therefore used
		/// for position calculations without changing the layer. The function is not static because it has to use either the parent
		/// graph or the linked layer for the calculations.</remarks>
		public double YPositionToUserUnits(double y, XYPlotLayerPositionType ypostype_to_convert_to)
		{
			switch (ypostype_to_convert_to)
			{
				case XYPlotLayerPositionType.AbsoluteValue:
					break;

				case XYPlotLayerPositionType.RelativeToGraphDocument:
					y = y / ParentLayerSize.Y;
					break;

				default:
					throw new NotImplementedException(ypostype_to_convert_to.ToString());
			}

			return y;
		}

		/// <summary>
		/// Sets the cached position value in <see cref="_cachedLayerPosition"/> by calculating it
		/// from the position values (<see cref="_location"/>.XPosition and .YPosition)
		/// and the position types (<see cref="_location"/>.XPositionType and YPositionType).
		/// </summary>
		protected void CalculateCachedPosition()
		{
			var newPos = new PointD2D(
				XPositionToPointUnits(this._location.XPosition, this._location.XPositionType),
				YPositionToPointUnits(this._location.YPosition, this._location.YPositionType));
			if (newPos != this._cachedLayerPosition)
			{
				this._cachedLayerPosition = newPos;
				this.CalculateMatrix();
				OnPositionChanged();
			}
		}

		public void SetSize(double width, XYPlotLayerSizeType widthtype, double height, XYPlotLayerSizeType heighttype)
		{
			this._location.Width = width;
			this._location.WidthType = widthtype;
			this._location.Height = height;
			this._location.HeightType = heighttype;

			CalculateCachedSize();
		}

		public double WidthToPointUnits(double width, XYPlotLayerSizeType widthtype)
		{
			switch (widthtype)
			{
				case XYPlotLayerSizeType.AbsoluteValue:
					break;

				case XYPlotLayerSizeType.RelativeToGraphDocument:
					width *= ParentLayerSize.X;
					break;

				default:
					throw new NotImplementedException(widthtype.ToString());
			}
			return width;
		}

		public double HeightToPointUnits(double height, XYPlotLayerSizeType heighttype)
		{
			switch (heighttype)
			{
				case XYPlotLayerSizeType.AbsoluteValue:
					break;

				case XYPlotLayerSizeType.RelativeToGraphDocument:
					height *= ParentLayerSize.Y;
					break;

				default:
					throw new NotImplementedException(heighttype.ToString());
			}
			return height;
		}

		/// <summary>
		/// Convert the width in points (1/72 inch) to user units of the type <paramref name="widthtype_to_convert_to"/>.
		/// </summary>
		/// <param name="width">The height value to convert (in point units).</param>
		/// <param name="widthtype_to_convert_to">The user unit type to convert to.</param>
		/// <returns>The value of the width in user units.</returns>
		public double WidthToUserUnits(double width, XYPlotLayerSizeType widthtype_to_convert_to)
		{
			switch (widthtype_to_convert_to)
			{
				case XYPlotLayerSizeType.AbsoluteValue:
					break;

				case XYPlotLayerSizeType.RelativeToGraphDocument:
					width /= ParentLayerSize.X;
					break;

				default:
					throw new NotImplementedException(widthtype_to_convert_to.ToString());
			}
			return width;
		}

		/// <summary>
		/// Convert the heigth in points (1/72 inch) to user units of the type <paramref name="heighttype_to_convert_to"/>.
		/// </summary>
		/// <param name="height">The height value to convert (in point units).</param>
		/// <param name="heighttype_to_convert_to">The user unit type to convert to.</param>
		/// <returns>The value of the height in user units.</returns>
		public double HeightToUserUnits(double height, XYPlotLayerSizeType heighttype_to_convert_to)
		{
			switch (heighttype_to_convert_to)
			{
				case XYPlotLayerSizeType.AbsoluteValue:
					break;

				case XYPlotLayerSizeType.RelativeToGraphDocument:
					height /= ParentLayerSize.Y;
					break;

				default:
					throw new NotImplementedException(heighttype_to_convert_to.ToString());
			}
			return height;
		}

		/// <summary>
		/// Sets the cached size value in <see cref="_cachedLayerSize"/> by calculating it
		/// from the position values (<see cref="_location"/>.Width and .Height)
		/// and the size types (<see cref="_location"/>.WidthType and .HeightType).
		/// </summary>
		protected void CalculateCachedSize()
		{
			var newSize = new PointD2D(
				WidthToPointUnits(this._location.Width, this._location.WidthType),
				HeightToPointUnits(this._location.Height, this._location.HeightType));
			if (newSize != this._cachedLayerSize)
			{
				this._cachedLayerSize = newSize;
				this.CalculateMatrix();
				OnSizeChanged();
			}
		}

		/// <summary>Returns the user x position value of the layer.</summary>
		/// <value>User x position value of the layer.</value>
		public double UserXPosition
		{
			get { return this._location.XPosition; }
		}

		/// <summary>Returns the user y position value of the layer.</summary>
		/// <value>User y position value of the layer.</value>
		public double UserYPosition
		{
			get { return this._location.YPosition; }
		}

		/// <summary>Returns the user width value of the layer.</summary>
		/// <value>User width value of the layer.</value>
		public double UserWidth
		{
			get { return this._location.Width; }
		}

		/// <summary>Returns the user height value of the layer.</summary>
		/// <value>User height value of the layer.</value>
		public double UserHeight
		{
			get { return this._location.Height; }
		}

		/// <summary>Returns the type of the user x position value of the layer.</summary>
		/// <value>Type of the user x position value of the layer.</value>
		public XYPlotLayerPositionType UserXPositionType
		{
			get { return this._location.XPositionType; }
		}

		/// <summary>Returns the type of the user y position value of the layer.</summary>
		/// <value>Type of the User y position value of the layer.</value>
		public XYPlotLayerPositionType UserYPositionType
		{
			get { return this._location.YPositionType; }
		}

		/// <summary>Returns the type of the the user width value of the layer.</summary>
		/// <value>Type of the User width value of the layer.</value>
		public XYPlotLayerSizeType UserWidthType
		{
			get { return this._location.WidthType; }
		}

		/// <summary>Returns the the type of the user height value of the layer.</summary>
		/// <value>Type of the User height value of the layer.</value>
		public XYPlotLayerSizeType UserHeightType
		{
			get { return this._location.HeightType; }
		}

		/// <summary>
		/// Measures to do when the position of the linked layer changed.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event args.</param>
		protected void EhLinkedLayerPositionChanged(object sender, System.EventArgs e)
		{
			CalculateCachedPosition();
		}

		/// <summary>
		/// Measures to do when the size of the linked layer changed.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event args.</param>
		protected void EhLinkedLayerSizeChanged(object sender, System.EventArgs e)
		{
			CalculateCachedSize();
			CalculateCachedPosition();
		}

		#endregion Position and Size

		#region XYPlotLayer properties and methods

		/// <summary>
		/// Gets the child layers of this layer.
		/// </summary>
		/// <value>
		/// The child layers.
		/// </value>
		public IList<HostLayer> Layers
		{
			get
			{
				return _childLayers;
			}
		}

		/// <summary>
		/// The layer number.
		/// </summary>
		/// <value>The layer number, i.e. the position of the layer in the layer collection.</value>
		public int Number
		{
			get
			{
				if (_parent is HostLayer)
				{
					var hl = _parent as HostLayer;
					var childLayers = hl._childLayers;
					for (int i = 0; i < childLayers.Count; ++i)
						if (object.ReferenceEquals(this, childLayers[i]))
							return i;
				}
				return 0;
			}
		}

		public IList<HostLayer> ParentLayerList
		{
			get
			{
				var hl = _parent as HostLayer;
				return hl == null ? null : hl._childLayers;
			}
		}

		public HostLayer ParentLayer
		{
			get { return _parent as HostLayer; }
			set { _parent = value; }
		}

		public GraphicCollection GraphObjects
		{
			get { return _graphObjects; }
		}

		/// <summary>
		/// Initialize the graph objects collection internally.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// _graphObjects was already set!
		/// or
		/// _childLayers was already set!
		/// </exception>
		private void InternalInitializeGraphObjectsCollection()
		{
			if (null != _graphObjects)
				throw new InvalidOperationException("_graphObjects was already set!");
			if (null != _childLayers)
				throw new InvalidOperationException("_childLayers was already set!");

			_graphObjects = new GraphicCollection(x => x.ParentObject = this);

			_childLayers = _graphObjects.CreatePartialViewOfType<HostLayer>((Action<HostLayer>)EhBeforeInsertChildLayer);
			_childLayers.CollectionChanged += EhChildLayers_CollectionChanged;
			OnGraphObjectsCollectionInstanceInitialized();
		}

		/// <summary>
		/// Called after the instance of the <see cref="GraphicCollection"/> <see cref="GraphObjects"/> has been initialized.
		/// </summary>
		protected virtual void OnGraphObjectsCollectionInstanceInitialized()
		{
		}

		private void EhBeforeInsertChildLayer(HostLayer child)
		{
			child.ParentLayer = this;
		}

		private void EhChildLayers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (null != LayerCollectionChanged)
				LayerCollectionChanged(this, EventArgs.Empty);

			var pl = ParentLayer;
			if (null != pl)
			{
				pl.EhChildLayers_CollectionChanged(sender, e);
			}
		}

		public virtual void Remove(GraphicBase go)
		{
			if (_graphObjects.Contains(go))
				_graphObjects.Remove(go);
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public virtual void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
		{
			foreach (var hl in _childLayers)
				hl.VisitDocumentReferences(Report);
		}

		#endregion XYPlotLayer properties and methods

		#region Painting and Hit testing

		/// <summary>
		/// This function is called by the graph document before _any_ layer is painted. We have to make sure that all of our cached data becomes valid.
		///
		/// </summary>

		public virtual void PreparePainting()
		{
			foreach (var obj in _childLayers)
				obj.PreparePainting();
		}

		/// <summary>
		/// This function is called when painting is finished. Can be used to release the resources
		/// not neccessary any more.
		/// </summary>
		public virtual void FinishPainting()
		{
			foreach (var obj in _childLayers)
				obj.FinishPainting();
		}

		public virtual void Paint(Graphics g, object o)
		{
			Paint(g);
		}

		public virtual void Paint(Graphics g)
		{
			GraphicsState savedgstate = g.Save();

			g.MultiplyTransform(_transformation);

			int len = _graphObjects.Count;
			for (int i = 0; i < len; i++)
			{
				_graphObjects[i].Paint(g, this);
			}

			g.Restore(savedgstate);
		}

		protected IHitTestObject ForwardTransform(IHitTestObject o)
		{
			o.Transform(_transformation);
			o.ParentLayer = this;
			return o;
		}

		public virtual IHitTestObject HitTest(HitTestPointData hitData)
		{
			return HitTest(hitData, false);
		}

		public virtual IHitTestObject HitTest(HitTestPointData pageC, bool plotItemsOnly)
		{
			IHitTestObject hit;

			HitTestPointData layerHitTestData = pageC.NewFromTranslationRotationScaleShear(Position.X, Position.Y, -Rotation, Scale, Scale, 0);

			var layerC = layerHitTestData.GetHittedPointInWorldCoord();

			if (!plotItemsOnly)
			{
				// first hit testing all four corners of the layer
				GraphicsPath layercorners = new GraphicsPath();
				float catchrange = 6;
				layercorners.AddEllipse(-catchrange, -catchrange, 2 * catchrange, 2 * catchrange);
				layercorners.AddEllipse((float)(_cachedLayerSize.X - catchrange), 0 - catchrange, 2 * catchrange, 2 * catchrange);
				layercorners.AddEllipse(0 - catchrange, (float)(_cachedLayerSize.Y - catchrange), 2 * catchrange, 2 * catchrange);
				layercorners.AddEllipse((float)(_cachedLayerSize.X - catchrange), (float)(_cachedLayerSize.Y - catchrange), 2 * catchrange, 2 * catchrange);
				layercorners.CloseAllFigures();
				if (layercorners.IsVisible((PointF)layerC))
				{
					hit = new HitTestObject(layercorners, this);
					hit.DoubleClick = LayerPositionEditorMethod;
					return ForwardTransform(hit);
				}

				// hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
				for (int i = _graphObjects.Count - 1; i >= 0; --i)
				{
					hit = _graphObjects[i].HitTest(layerHitTestData);
					if (null != hit)
					{
						if (null == hit.Remove && (hit.HittedObject is IGraphicBase))
							hit.Remove = new DoubleClickHandler(EhGraphicsObject_Remove);
						return ForwardTransform(hit);
					}
				}
			}

			return null;
		}

		private static bool EhGraphicsObject_Remove(IHitTestObject o)
		{
			var go = (IGraphicBase)o.HittedObject;
			o.ParentLayer.GraphObjects.Remove(go);
			return true;
		}

		#endregion Painting and Hit testing

		#region Editor methods

		public static DoubleClickHandler LayerPositionEditorMethod;

		#endregion Editor methods

		#region Event firing

		protected virtual void OnSizeChanged()
		{
			// now inform other listeners
			if (null != SizeChanged)
				SizeChanged(this, new System.EventArgs());

			OnChanged();
		}

		protected void OnPositionChanged()
		{
			if (null != PositionChanged)
				PositionChanged(this, new System.EventArgs());

			OnChanged();
		}

		public IDisposable BeginUpdate()
		{
			return _changeEventSuppressor.Suspend();
		}

		public void EndUpdate(ref IDisposable locker)
		{
			_changeEventSuppressor.Resume(ref locker);
		}

		protected void EhChangeEventResumed()
		{
			if (_parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)this._parent).EhChildChanged(this, EventArgs.Empty);
		}

		protected void OnChanged()
		{
			if (_changeEventSuppressor.GetEnabledWithCounting())
			{
				if (_parent is Main.IChildChangedEventSink)
					((Main.IChildChangedEventSink)this._parent).EhChildChanged(this, EventArgs.Empty);
			}
		}

		#endregion Event firing

		#region Handler of child events

		public void EhChildChanged(object sender, EventArgs e)
		{
			OnChanged();
		}

		#endregion Handler of child events

		#region IDocumentNode Members

		public object ParentObject
		{
			get
			{
				return this._parent;
			}
			set
			{
				var oldValue = _parent;
				this._parent = value;
			}
		}

		public string Name
		{
			get
			{
				if (ParentObject is Main.INamedObjectCollection)
					return ((Main.INamedObjectCollection)ParentObject).GetNameOfChildObject(this);
				else
					return GetDefaultNameOfLayer(this.IndexOf());
			}
		}

		/// <summary>
		/// Returns the document name of the layer at index i. Actually, this is a name of the form L0, L1, L2 and so on.
		/// </summary>
		/// <param name="layerIndex">The layer index.</param>
		/// <returns>The name of the layer at index i.</returns>
		public static string GetDefaultNameOfLayer(IList<int> layerIndex)
		{
			if (layerIndex.Count == 0)
				return "RL";

			var stb = new System.Text.StringBuilder();
			stb.AppendFormat("L{0}", layerIndex[0]);
			for (int k = 1; k < layerIndex.Count; ++k)
				stb.AppendFormat("-{0}", layerIndex[k]);
			return stb.ToString();
		}

		/// <summary>
		/// retrieves the object with the name <code>name</code>.
		/// </summary>
		/// <param name="name">The objects name.</param>
		/// <returns>The object with the specified name.</returns>
		public virtual object GetChildObjectNamed(string name)
		{
			foreach (var childLayer in _childLayers)
			{
				if (GetDefaultNameOfLayer(childLayer.IndexOf()) == name)
					return childLayer;
			}
			return null;
		}

		/// <summary>
		/// Retrieves the name of the provided object.
		/// </summary>
		/// <param name="o">The object for which the name should be found.</param>
		/// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
		public virtual string GetNameOfChildObject(object o)
		{
			if (o is HostLayer)
				return GetDefaultNameOfLayer(((HostLayer)o).IndexOf());
			return string.Empty;
		}

		public virtual bool FixAndTestParentChildRelationShipOfLayers()
		{
			return this.FixAndTestParentChildRelations((l, p) => l.ParentLayer = p);
		}

		#endregion IDocumentNode Members

		#region Enumeration through layers

		/// <summary>
		/// Executes an action on each child layer, including this layer, beginning with the topmost child (the first child of the first child of...).
		/// </summary>
		/// <param name="action">The action to execute.</param>
		public void ExecuteFromTopmostChildToRoot(Action<HostLayer> action)
		{
			ExecuteFromTopmostChildToRoot(this, action);
		}

		private static void ExecuteFromTopmostChildToRoot(HostLayer start, Action<HostLayer> action)
		{
			foreach (var l in start._childLayers)
			{
				ExecuteFromTopmostChildToRoot(l, action);
			}
			action(start);
		}

		#endregion Enumeration through layers

		#region ITreeListNodeWithParent implementation

		IList<HostLayer> ITreeListNode<HostLayer>.Nodes
		{
			get { return _childLayers; }
		}

		IEnumerable<HostLayer> ITreeNode<HostLayer>.Nodes
		{
			get { return _childLayers; }
		}

		HostLayer INodeWithParentNode<HostLayer>.ParentNode
		{
			get { return _parent as HostLayer; }
		}

		#endregion ITreeListNodeWithParent implementation
	}
}