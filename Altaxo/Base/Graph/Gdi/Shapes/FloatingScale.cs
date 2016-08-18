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

using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Graph.Scales.Ticks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
	/// <summary>Enumerates the kind of span that determines the length of the floating scale.</summary>
	public enum FloatingScaleSpanType
	{
		/// <summary>
		/// The span value is a logical value. This is the ratio corresponding to the length of the underlying scale. Thus, a value of 0.5 means half the length of the underlying scale.
		/// </summary>
		IsLogicalValue,

		/// <summary>
		/// The span value is a physical value, and is given as difference of end and org of the floating scale. Thus, if the span value is for example 3 and the org of the floating scale is 2, then the end of the floating scale will be 2 + 3 = 5.
		/// </summary>
		IsPhysicalEndOrgDifference,

		/// <summary>
		/// The span value is a physical value, and is given as ratio of end to org of the floating scale. Thus, if the span value is for example 3 and the org of the floating scale is 2, then the end of the floating scale will be 2 * 3 =6.
		/// </summary>
		IsPhysicalEndOrgRatio
	}

	[Serializable]
	public class FloatingScale : GraphicBase
	{
		/// <summary>Number of the scale to measure (0: x-axis, 1: y-axis, 2: z-axis).</summary>
		private int _scaleNumber;

		/// <summary>Designates the type of scale span value, i.e. whether it is interpreted as a logical value, or a physical value (either as a span difference or as an end/org ratio).</summary>
		private FloatingScaleSpanType _scaleSpanType;

		/// <summary>The span this scale should show. It is either a physical or a logical value, depending on <see cref="_scaleSpanType"/>.</summary>
		private double _scaleSpanValue;

		private ScaleSegmentType _scaleSegmentType;

		private TickSpacing _tickSpacing;

		private AxisStyle _axisStyle;

		private Margin2D _backgroundPadding;

		private IBackgroundStyle _background;

		// Cached members
		/// <summary>Cached path of the isoline.</summary>
		private GraphicsPath _cachedPath;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FloatingScale), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				FloatingScale s = (FloatingScale)obj;
				info.AddBaseValueEmbedded(s, typeof(FloatingScale).BaseType);

				info.AddValue("ScaleNumber", s._scaleNumber);
				info.AddEnum("ScaleSpanType", s._scaleSpanType);
				info.AddValue("ScaleSpanValue", s._scaleSpanValue);
				info.AddEnum("ScaleType", s._scaleSegmentType);
				info.AddValue("TickSpacing", s._tickSpacing);
				info.AddValue("AxisStyle", s._axisStyle);

				info.AddValue("Background", s._background);
				if (null != s._background)
					info.AddValue("BackgroundPadding", s._backgroundPadding);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				FloatingScale s = null != o ? (FloatingScale)o : new FloatingScale(info);
				info.GetBaseValueEmbedded(s, typeof(FloatingScale).BaseType, parent);

				s._scaleNumber = info.GetInt32("ScaleNumber");
				s._scaleSpanType = (FloatingScaleSpanType)info.GetEnum("ScaleSpanType", typeof(FloatingScaleSpanType));
				s._scaleSpanValue = info.GetDouble("ScaleSpanValue");
				s._scaleSegmentType = (ScaleSegmentType)info.GetEnum("ScaleType", typeof(ScaleSegmentType));

				s._tickSpacing = (TickSpacing)info.GetValue("TickSpacing", s);
				if (null != s._tickSpacing) s._tickSpacing.ParentObject = s;

				s._axisStyle = (AxisStyle)info.GetValue("AxisStyle", s);
				if (null != s._axisStyle) s._axisStyle.ParentObject = s;

				s._background = (IBackgroundStyle)info.GetValue("Background", s);
				if (null != s._background)
				{
					s._background.ParentObject = s;
					s._backgroundPadding = (Margin2D)info.GetValue("BackgroundPadding", s);
				}

				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		/// <summary>Constructor only for deserialization purposes.</summary>
		/// <param name="info">Not used here.</param>
		private FloatingScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
			: base(new ItemLocationDirectAutoSize())
		{
		}

		public FloatingScale(Main.Properties.IReadOnlyPropertyBag context)
			: base(new ItemLocationDirectAutoSize())
		{
			_scaleSpanValue = 0.25;
			_tickSpacing = new SpanTickSpacing();
			_axisStyle = new AxisStyle(new CSLineID(0, 0), true, false, true, null, context);
		}

		public FloatingScale(FloatingScale from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		public override bool CopyFrom(object obj)
		{
			bool isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as FloatingScale;
				if (null != from)
				{
					_cachedPath = null;

					_scaleSpanValue = from._scaleSpanValue;
					_scaleSpanType = from._scaleSpanType;
					_scaleNumber = from._scaleNumber;
					_scaleSegmentType = from._scaleSegmentType;

					CopyHelper.Copy(ref _tickSpacing, from._tickSpacing);
					CopyHelper.Copy(ref _axisStyle, from._axisStyle);

					_backgroundPadding = from._backgroundPadding;
					CopyHelper.Copy(ref _background, from._background);
				}
			}
			return isCopied;
		}

		public override object Clone()
		{
			return new FloatingScale(this);
		}

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _tickSpacing)
				yield return new Main.DocumentNodeAndName(_tickSpacing, "TickSpacing");
			if (null != _axisStyle)
				yield return new Main.DocumentNodeAndName(_axisStyle, "AxisStyle");
			if (null != _background)
				yield return new Main.DocumentNodeAndName(_background, "Background");
		}

		#endregion Constructors

		public override void FixupInternalDataStructures()
		{
			base.FixupInternalDataStructures();

			var layer = Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(this);

			_axisStyle.FixupInternalDataStructures(layer);
		}

		public override bool IsCompatibleWithParent(object parentObject)
		{
			return parentObject is XYPlotLayer;
		}

		public AxisStyle AxisStyle
		{
			get
			{
				return _axisStyle;
			}
		}

		public ScaleSegmentType ScaleType
		{
			get
			{
				return _scaleSegmentType;
			}
			set
			{
				var oldValue = _scaleSegmentType;
				_scaleSegmentType = value;
				if (oldValue != value)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public TickSpacing TickSpacing
		{
			get
			{
				return _tickSpacing;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException();

				_tickSpacing = (TickSpacing)value.Clone();

				EhSelfChanged(EventArgs.Empty);
			}
		}

		public int ScaleNumber
		{
			get
			{
				return _scaleNumber;
			}
			set
			{
				_scaleNumber = value;
			}
		}

		public double ScaleSpanValue
		{
			get
			{
				return _scaleSpanValue;
			}
			set
			{
				_scaleSpanValue = value;
			}
		}

		public FloatingScaleSpanType ScaleSpanType
		{
			get
			{
				return _scaleSpanType;
			}
			set
			{
				_scaleSpanType = value;
			}
		}

		public Margin2D BackgroundPadding
		{
			get
			{
				return _backgroundPadding;
			}
			set
			{
				var oldValue = _backgroundPadding;
				_backgroundPadding = value;
				if (!value.Equals(oldValue))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public IBackgroundStyle Background
		{
			get
			{
				return _background;
			}
			set
			{
				var oldValue = _background;
				_background = value;
				if (!object.ReferenceEquals(value, oldValue))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public override bool AllowNegativeSize
		{
			get
			{
				return true;
			}
		}

		public override bool AutoSize
		{
			get
			{
				return true;
			}
		}

		protected override void SetPosition(PointD2D value, Main.EventFiring eventFiring)
		{
			var oldPosition = this.GetPosition();
			base.SetPosition(value, eventFiring);

			if (_axisStyle.Title != null)
			{
				var oldTitlePos = _axisStyle.Title.Position;
				_axisStyle.Title.SilentSetPosition(oldTitlePos + (GetPosition() - oldPosition));
			}
		}

		public override void SilentSetPosition(PointD2D newPosition)
		{
			var oldPosition = this.GetPosition();
			base.SilentSetPosition(newPosition);
			if (_axisStyle.Title != null)
			{
				var oldTitlePos = _axisStyle.Title.Position;
				_axisStyle.Title.SilentSetPosition(oldTitlePos + (GetPosition() - oldPosition));
			}
		}

		public GraphicsPath GetSelectionPath()
		{
			return (GraphicsPath)_cachedPath.Clone();
		}

		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return (GraphicsPath)_cachedPath.Clone();
		}

		protected GraphicsPath GetPath(double minWidth)
		{
			GraphicsPath gp = (GraphicsPath)_cachedPath.Clone();

			return gp;
		}

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			if (_axisStyle.Title != null)
			{
				var titleResult = _axisStyle.Title.HitTest(htd);
				if (null != titleResult)
				{
					titleResult.Remove = EhTitleRemove;
					return titleResult;
				}
			}

			var pt = htd.GetHittedPointInWorldCoord();
			HitTestObjectBase result = null;
			GraphicsPath gp = GetSelectionPath();
			if (gp.IsVisible((PointF)pt))
			{
				result = new MyHitTestObject(this);
			}

			if (result != null)
				result.DoubleClick = EhHitDoubleClick;

			return result;
		}

		private static bool EhHitDoubleClick(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			Current.Gui.ShowDialog(ref hitted, "Floating scale properties", true);
			((FloatingScale)hitted).EhSelfChanged(EventArgs.Empty);
			return true;
		}

		private static bool EhTitleRemove(IHitTestObject o)
		{
			object hitted = o.HittedObject;
			var axStyle = ((TextGraphic)hitted).ParentObject as AxisStyle;
			axStyle.Title = null;
			return true;
		}

		public override void Paint(Graphics g, IPaintContext paintContext)
		{
			var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(this);

			if (null == layer)
			{
				PaintErrorInvalidLayerType(g, paintContext);
				return;
			}

			Logical3D rBegin;
			layer.CoordinateSystem.LayerToLogicalCoordinates(X, Y, out rBegin);

			Logical3D rEnd = rBegin;
			switch (_scaleSpanType)
			{
				case FloatingScaleSpanType.IsLogicalValue:
					rEnd[_scaleNumber] = rBegin[_scaleNumber] + _scaleSpanValue;
					break;

				case FloatingScaleSpanType.IsPhysicalEndOrgDifference:
					{
						var physValue = layer.Scales[_scaleNumber].NormalToPhysicalVariant(rBegin[this._scaleNumber]);
						physValue += _scaleSpanValue; // to be replaced by the scale span
						var logValue = layer.Scales[_scaleNumber].PhysicalVariantToNormal(physValue);
						rEnd[_scaleNumber] = logValue;
					}
					break;

				case FloatingScaleSpanType.IsPhysicalEndOrgRatio:
					{
						var physValue = layer.Scales[_scaleNumber].NormalToPhysicalVariant(rBegin[this._scaleNumber]);
						physValue *= _scaleSpanValue; // to be replaced by the scale span
						var logValue = layer.Scales[_scaleNumber].PhysicalVariantToNormal(physValue);
						rEnd[_scaleNumber] = logValue;
					}
					break;
			}

			// axis style
			var csLineId = new CSLineID(_scaleNumber, rBegin);
			if (_axisStyle.StyleID != csLineId)
			{
				var propertyContext = this.GetPropertyContext();
				var axStyle = new AxisStyle(new CSLineID(_scaleNumber, rBegin), false, false, false, null, propertyContext);
				axStyle.CopyWithoutIdFrom(_axisStyle);
				_axisStyle = axStyle;
			}

			var privScale = new ScaleSegment(layer.Scales[_scaleNumber], rBegin[_scaleNumber], rEnd[_scaleNumber], _scaleSegmentType);
			_tickSpacing.FinalProcessScaleBoundaries(privScale.OrgAsVariant, privScale.EndAsVariant, privScale);
			privScale.TickSpacing = _tickSpacing;
			var privLayer = new LayerSegment(layer, privScale, rBegin, rEnd, _scaleNumber);

			if (_background == null)
			{
				_axisStyle.Paint(g, paintContext, privLayer, privLayer.GetAxisStyleInformation);
			}
			else
			{
				// if we have a background, we paint in a dummy bitmap in order to measure all items
				// the real painting is done later on after painting the background.
				using (var bmp = new Bitmap(4, 4))
				{
					using (Graphics gg = Graphics.FromImage(bmp))
					{
						_axisStyle.Paint(gg, paintContext, privLayer, privLayer.GetAxisStyleInformation);
					}
				}
			}

			_cachedPath = _axisStyle.AxisLineStyle.GetObjectPath(privLayer, true);

			// calculate size information
			RectangleD2D bounds1 = _cachedPath.GetBounds();

			if (_axisStyle.AreMinorLabelsEnabled)
			{
				var path = _axisStyle.MinorLabelStyle.GetSelectionPath();
				if (path.PointCount > 0)
				{
					_cachedPath.AddPath(path, false);
					RectangleD2D bounds2 = path.GetBounds();
					bounds1.ExpandToInclude(bounds2);
				}
			}
			if (_axisStyle.AreMajorLabelsEnabled)
			{
				var path = _axisStyle.MajorLabelStyle.GetSelectionPath();
				if (path.PointCount > 0)
				{
					_cachedPath.AddPath(path, false);
					RectangleD2D bounds2 = path.GetBounds();
					bounds1.ExpandToInclude(bounds2);
				}
			}

			((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(bounds1.Size);
			//this._leftTop = bounds1.Location - this.GetPosition();
			//throw new NotImplementedException("debug the previous statement");
			if (_background != null)
			{
				bounds1.Expand(_backgroundPadding);
				_background.Draw(g, bounds1);
				_axisStyle.Paint(g, paintContext, privLayer, privLayer.GetAxisStyleInformation);
			}
		}

		private void PaintErrorInvalidLayerType(Graphics g, object obj)
		{
			string errorMsg = "FloatingScale:Error: Invalid layer type";
			var font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.World);
			var size = g.MeasureString(errorMsg, font);
			if (obj is HostLayer)
			{
				var destSizeX = 0.2 * ((HostLayer)obj).Size.X;
				var factor = destSizeX / size.Width;
				font = new Font(FontFamily.GenericSansSerif, (float)(font.Size * factor), FontStyle.Regular, GraphicsUnit.World);
			}

			g.DrawString(errorMsg, font, Brushes.Red, (PointF)this.Position);
			size = g.MeasureString(errorMsg, font);

			_cachedPath = new GraphicsPath();
			_cachedPath.AddRectangle(new RectangleF((PointF)this.Position, size));

			((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(size);
		}

		#region Inner classes

		private class LayerSegment : IPlotArea
		{
			private IPlotArea _underlyingArea;
			private Logical3D _org;
			private Logical3D _end;
			private int _scaleNumber;

			private ScaleCollection _scaleCollection = new ScaleCollection();

			public LayerSegment(IPlotArea underlyingArea, Scale scale, Logical3D org, Logical3D end, int scaleNumber)
			{
				_underlyingArea = underlyingArea;
				_org = org;
				_end = end;
				_scaleNumber = scaleNumber;

				for (int i = 0; i < _underlyingArea.Scales.Count; ++i)
				{
					if (i == _scaleNumber)
						_scaleCollection[i] = scale;
					else
						_scaleCollection[i] = (Scale)underlyingArea.Scales[i].Clone();
				}
			}

			public CSAxisInformation GetAxisStyleInformation(CSLineID lineId)
			{
				var result = _underlyingArea.CoordinateSystem.GetAxisStyleInformation(lineId).WithIdentifier(new CSLineID(lineId.ParallelAxisNumber, _org));
				result = result.WithLogicalValuesForAxisOrgAndEnd(
									LogicalValueAxisOrg: _org[_scaleNumber],
									LogicalValueAxisEnd: _end[_scaleNumber]);

				return result;
			}

			public bool Is3D
			{
				get { return _underlyingArea.Is3D; }
			}

			public Scale XAxis
			{
				get { return _scaleCollection[0]; }
			}

			public Scale YAxis
			{
				get { return _scaleCollection[1]; }
			}

			public ScaleCollection Scales
			{
				get { return _scaleCollection; }
			}

			public G2DCoordinateSystem CoordinateSystem
			{
				get { return _underlyingArea.CoordinateSystem; }
			}

			public PointD2D Size
			{
				get { throw new NotImplementedException(); }
			}

			public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
			{
				throw new NotImplementedException();
			}

			public System.Collections.Generic.IEnumerable<CSLineID> AxisStyleIDs
			{
				get { throw new NotImplementedException(); }
			}

			public CSPlaneID UpdateCSPlaneID(CSPlaneID id)
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Enumerates the type of scale segment
		/// </summary>
		public enum ScaleSegmentType
		{
			/// <summary>Scale segment corresponds to the segment of the parent scale.</summary>
			Normal,

			/// <summary>Measures differences from org, thus the physical value of org is evaluated to zero (0).</summary>
			DifferenceToOrg,

			/// <summary>Measures ratios to org, thus the physical value of org is evaluated to one (1).</summary>
			RatioToOrg
		}

		private class ScaleSegment : Scale
		{
			private double _relOrg;
			private double _relEnd;
			private Scale _underlyingScale;
			private ScaleSegmentType _segmentScaling;
			private TickSpacing _tickSpacing;

			public ScaleSegment(Scale underlyingScale, double relOrg, double relEnd, ScaleSegmentType scaling)
			{
				if (null == underlyingScale)
					throw new ArgumentNullException("underlyingScale");

				_underlyingScale = underlyingScale;
				_relOrg = relOrg;
				_relEnd = relEnd;
				_segmentScaling = scaling;
			}

			public override bool CopyFrom(object obj)
			{
				if (object.ReferenceEquals(this, obj))
					return true;

				var from = obj as ScaleSegment;

				if (null == from)
					return false;

				using (var suspendToken = SuspendGetToken())
				{
					this._relOrg = from._relOrg;
					this._relEnd = from._relEnd;
					this._underlyingScale = from._underlyingScale;
					this._segmentScaling = from._segmentScaling;

					EhSelfChanged(EventArgs.Empty);
					suspendToken.Resume();
				}

				return true;
			}

			public override object Clone()
			{
				return new ScaleSegment(_underlyingScale, _relOrg, _relEnd, _segmentScaling);
			}

			protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
			{
				yield break; // do not dispose _underlyingScale !! we are not the owner (the owner is the layer the scale belongs to)
			}

			public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
			{
				switch (_segmentScaling)
				{
					case ScaleSegmentType.DifferenceToOrg:
						x += _underlyingScale.NormalToPhysicalVariant(_relOrg);
						break;

					case ScaleSegmentType.RatioToOrg:
						x *= _underlyingScale.NormalToPhysicalVariant(_relOrg);
						break;
				}

				double r = _underlyingScale.PhysicalVariantToNormal(x);
				return (r - _relOrg) / (_relEnd - _relOrg);
			}

			public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
			{
				double r = _relOrg * (1 - x) + _relEnd * x;
				var y = _underlyingScale.NormalToPhysicalVariant(r);
				switch (_segmentScaling)
				{
					case ScaleSegmentType.DifferenceToOrg:
						y -= _underlyingScale.NormalToPhysicalVariant(_relOrg);
						break;

					case ScaleSegmentType.RatioToOrg:
						y /= _underlyingScale.NormalToPhysicalVariant(_relOrg);
						break;
				}
				return y;
			}

			public override IScaleRescaleConditions RescalingObject
			{
				get { return _underlyingScale.RescalingObject; }
			}

			public override Altaxo.Graph.Scales.Boundaries.IPhysicalBoundaries DataBoundsObject
			{
				get { return _underlyingScale.DataBoundsObject; }
			}

			public override Altaxo.Data.AltaxoVariant OrgAsVariant
			{
				get
				{
					return NormalToPhysicalVariant(0);
				}
			}

			public override Altaxo.Data.AltaxoVariant EndAsVariant
			{
				get
				{
					return NormalToPhysicalVariant(1);
				}
			}

			protected override string SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
			{
				_relOrg = _underlyingScale.PhysicalVariantToNormal(org);
				_relEnd = _underlyingScale.PhysicalVariantToNormal(end);
				return null;
			}

			public override void OnUserRescaled()
			{
			}

			public override void OnUserZoomed(Data.AltaxoVariant newZoomOrg, Data.AltaxoVariant newZoomEnd)
			{
			}

			public override TickSpacing TickSpacing
			{
				get
				{
					return _tickSpacing;
				}
				set
				{
					_tickSpacing = value;
				}
			}
		}

		#endregion Inner classes

		#region HitTestObject

		/// <summary>Creates a new hit test object. Here, a special hit test object is constructed, which suppresses the resize, rotate, scale and shear grips.</summary>
		/// <returns>A newly created hit test object.</returns>
		protected override IHitTestObject GetNewHitTestObject()
		{
			return new MyHitTestObject(this);
		}

		private class MyHitTestObject : GraphicBaseHitTestObject
		{
			public MyHitTestObject(FloatingScale obj)
				: base(obj)
			{
			}

			public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
			{
				return ((FloatingScale)_hitobject).GetGrips(this, pageScale, GripKind.Move);
			}
		}

		#endregion HitTestObject
	} // End Class
} // end Namespace