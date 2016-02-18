#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Data;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Graph3D.Axis
{
	using Drawing;
	using Drawing.D3D;
	using Geometry;
	using GraphicsContext;
	using LabelFormatting;

	/// <summary>
	/// Responsible for setting position, rotation, font, color etc. of axis labels.
	/// </summary>
	public class AxisLabelStyle :
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IRoutedPropertyReceiver,
		Main.ICopyFrom
	{
		protected FontX3D _font;

		protected StringAlignment _alignmentX;
		protected StringAlignment _alignmentY;
		protected StringAlignment _alignmentZ;

		protected StringFormat _stringFormat;
		protected IMaterial _brush;

		/// <summary>The x offset in EM units.</summary>
		protected double _offsetX;

		/// <summary>The y offset in EM units.</summary>
		protected double _offsetY;

		/// <summary>The z offset in EM units.</summary>
		protected double _offsetZ;

		/// <summary>The rotation of the label.</summary>
		protected double _rotationX;

		/// <summary>The rotation of the label.</summary>
		protected double _rotationY;

		/// <summary>The rotation of the label.</summary>
		protected double _rotationZ;

		/// <summary>The style for the background.</summary>
		protected Background.IBackgroundStyle3D _backgroundStyle;

		protected bool _automaticRotationShift;

		protected SuppressedTicks _suppressedLabels;

		private ILabelFormatting _labelFormatting;

		/// <summary>
		/// If set, this overrides the preferred label side that comes along with the axis style.
		/// </summary>
		private CSAxisSide? _labelSide;

		private string _prefixText;

		private string _postfixText;

		private CSAxisInformation _cachedAxisStyleInfo;

		/// <summary>
		/// The cached label outlines used for hit testing
		/// </summary>
		private RectangularObjectOutline[] _cachedLabelOutlines;

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLabelStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AxisLabelStyle s = (AxisLabelStyle)obj;
				info.AddValue("Font", s._font);
				info.AddValue("Brush", s._brush);
				info.AddValue("Background", s._backgroundStyle);

				info.AddValue("AutoAlignment", s._automaticRotationShift);
				info.AddEnum("AlignmentX", s._alignmentX);
				info.AddEnum("AlignmentY", s._alignmentY);
				info.AddEnum("AlignmentZ", s._alignmentY);

				info.AddValue("RotationX", s._rotationX);
				info.AddValue("RotationY", s._rotationY);
				info.AddValue("RotationZ", s._rotationZ);

				info.AddValue("OffsetX", s._offsetX);
				info.AddValue("OffsetY", s._offsetY);
				info.AddValue("OffsetZ", s._offsetZ);

				if (s._suppressedLabels.IsEmpty)
					info.AddValue("SuppressedLabels", (object)null);
				else
					info.AddValue("SuppressedLabels", s._suppressedLabels);

				info.AddValue("LabelFormat", s._labelFormatting);

				info.AddNullableEnum("LabelSide", s._labelSide);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AxisLabelStyle s = null != o ? (AxisLabelStyle)o : new AxisLabelStyle(info);

				s._font = (FontX3D)info.GetValue("Font", s);
				s._brush = (IMaterial)info.GetValue("Brush", s);

				s.BackgroundStyle = (Background.IBackgroundStyle3D)info.GetValue("Background", s);

				s._automaticRotationShift = info.GetBoolean("AutoAlignment");
				s._alignmentX = (StringAlignment)info.GetEnum("AlignmentX", typeof(StringAlignment));
				s._alignmentY = (StringAlignment)info.GetEnum("AlignmentY", typeof(StringAlignment));
				s._alignmentZ = (StringAlignment)info.GetEnum("AlignmentZ", typeof(StringAlignment));
				s._rotationX = info.GetDouble("RotationX");
				s._rotationY = info.GetDouble("RotationY");
				s._rotationZ = info.GetDouble("RotationZ");
				s._offsetX = info.GetDouble("OffsetX");
				s._offsetY = info.GetDouble("OffsetY");
				s._offsetZ = info.GetDouble("OffsetZ");

				s._suppressedLabels = (SuppressedTicks)info.GetValue("SuppressedLabels", s);
				if (s._suppressedLabels != null)
					s._suppressedLabels.ParentObject = s;
				else
					s._suppressedLabels = new SuppressedTicks() { ParentObject = s };

				s._labelFormatting = (ILabelFormatting)info.GetValue("LabelFormat", s);
				s._labelFormatting.ParentObject = s;

				s._labelSide = info.GetNullableEnum<CSAxisSide>("LabelSide");

				// Modification of StringFormat is necessary to avoid
				// too big spaces between successive words
				s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
				s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

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

		#endregion Serialization

		protected AxisLabelStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		public AxisLabelStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: this(null, context)
		{
		}

		public AxisLabelStyle(CSAxisSide? labelSide, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			_labelSide = labelSide;

			_font = GraphDocument.GetDefaultFont(context);
			var foreColor = GraphDocument.GetDefaultForeColor(context);

			_brush = Materials.GetSolidMaterial(foreColor);

			_automaticRotationShift = true;
			_rotationX = 90;
			_suppressedLabels = new SuppressedTicks() { ParentObject = this };
			_labelFormatting = new LabelFormatting.NumericLabelFormattingAuto() { ParentObject = this };
			SetStringFormat();
		}

		public AxisLabelStyle(AxisLabelStyle from)
		{
			CopyFrom(from);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as AxisLabelStyle;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				_cachedAxisStyleInfo = from._cachedAxisStyleInfo;

				_font = from._font;
				CopyHelper.Copy(ref _stringFormat, from._stringFormat);
				_alignmentX = from._alignmentX;
				_alignmentY = from._alignmentY;
				_alignmentZ = from._alignmentZ;

				_brush = from._brush;

				_automaticRotationShift = from._automaticRotationShift;
				_offsetX = from._offsetX;
				_offsetY = from._offsetY;
				_offsetZ = from._offsetZ;
				_rotationX = from._rotationX;
				_rotationY = from._rotationY;
				_rotationZ = from._rotationZ;
				ChildCopyToMember(ref _backgroundStyle, from._backgroundStyle);
				ChildCopyToMember(ref _labelFormatting, from._labelFormatting);
				_labelSide = from._labelSide;
				_prefixText = from._prefixText;
				_postfixText = from._postfixText;
				ChildCopyToMember(ref _suppressedLabels, from._suppressedLabels);
				EhSelfChanged(EventArgs.Empty);

				suspendToken.Resume();
			}

			return true;
		}

		public virtual object Clone()
		{
			return new AxisLabelStyle(this);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _labelFormatting)
				yield return new Main.DocumentNodeAndName(_labelFormatting, "LabelFormatting");

			if (null != _backgroundStyle)
				yield return new Main.DocumentNodeAndName(_backgroundStyle, "BackgroundStyle");

			if (null != _suppressedLabels)
				yield return new Main.DocumentNodeAndName(_suppressedLabels, "SuppressedLabels");
		}

		private void SetStringFormat()
		{
			// Modification of StringFormat is necessary to avoid
			// too big spaces between successive words
			_stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
			_stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
		}

		#region Properties

		/// <summary>The font of the label.</summary>
		public FontX3D Font
		{
			get { return _font; }
			set
			{
				if (null == value)
					throw new ArgumentNullException();

				var oldValue = _font;
				_font = value;

				if (value.Equals(oldValue))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>The font size of the label.</summary>
		public virtual double FontSize
		{
			get { return _font.Size; }
			set
			{
				var oldValue = _font.Size;
				var newValue = Math.Max(0, value);

				if (newValue != oldValue)
				{
					FontX3D oldFont = _font;
					_font = oldFont.GetFontWithNewSize(newValue);

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The brush color. If you set this, the font brush will be set to a solid brush.</summary>
		public NamedColor Color
		{
			get { return this._brush.Color; }
			set
			{
				var oldValue = _brush;
				_brush = Materials.GetMaterialWithNewColor(_brush, value);

				if (!object.ReferenceEquals(oldValue, _brush))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>The brush. During setting, the brush is cloned.</summary>
		public IMaterial Brush
		{
			get
			{
				return this._brush; ;
			}
			set
			{
				var oldValue = _brush;
				this._brush = value;

				if (!object.ReferenceEquals(oldValue, _brush))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>The background style.</summary>
		public Background.IBackgroundStyle3D BackgroundStyle
		{
			get
			{
				return _backgroundStyle;
			}
			set
			{
				if (ChildSetMember(ref _backgroundStyle, value))
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public SuppressedTicks SuppressedLabels
		{
			get
			{
				return _suppressedLabels;
			}
		}

		public ILabelFormatting LabelFormat
		{
			get
			{
				return this._labelFormatting;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (object.ReferenceEquals(_labelFormatting, value))
					return;

				_labelFormatting = value;
				_labelFormatting.ParentObject = this;
				EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>Gets or sets the label side. If the value is set to <c>null</c>, the label side is chosen automatically. If set to a value different from <c>null</c>,
		/// the label is shown on this side of the axis.</summary>
		/// <value>The label side where the label should be shown, or <c>null</c> to choose the side automatically.</value>
		public CSAxisSide? LabelSide
		{
			get
			{
				return _labelSide;
			}
			set
			{
				var oldValue = _labelSide;
				_labelSide = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public string PrefixText
		{
			get
			{
				return _labelFormatting.PrefixText;
			}
			set
			{
				_labelFormatting.PrefixText = value;
			}
		}

		public string SuffixText
		{
			get
			{
				return _labelFormatting.SuffixText;
			}
			set
			{
				var oldValue = _labelFormatting.SuffixText;
				_labelFormatting.SuffixText = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>The x offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
		public double OffsetX
		{
			get { return this._offsetX; }
			set
			{
				double oldValue = this._offsetX;
				this._offsetX = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
		public double OffsetY
		{
			get { return this._offsetY; }
			set
			{
				double oldValue = this._offsetY;
				this._offsetY = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The z offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
		public double OffsetZ
		{
			get { return this._offsetZ; }
			set
			{
				double oldValue = this._offsetZ;
				this._offsetZ = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The angle of the label.</summary>
		public double RotationX
		{
			get { return this._rotationX; }
			set
			{
				double oldValue = this._rotationX;
				this._rotationX = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The angle of the label.</summary>
		public double RotationY
		{
			get { return this._rotationY; }
			set
			{
				double oldValue = this._rotationY;
				this._rotationY = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The angle of the label.</summary>
		public double RotationZ
		{
			get { return this._rotationZ; }
			set
			{
				double oldValue = this._rotationZ;
				this._rotationZ = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public bool AutomaticAlignment
		{
			get
			{
				return this._automaticRotationShift;
			}
			set
			{
				bool oldValue = this.AutomaticAlignment;
				this._automaticRotationShift = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Horizontal alignment of the label.</summary>
		public System.Drawing.StringAlignment AlignmentX
		{
			get
			{
				return this._alignmentX;
			}
			set
			{
				System.Drawing.StringAlignment oldValue = _alignmentX;
				this._alignmentX = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Vertical aligment of the label.</summary>
		public System.Drawing.StringAlignment AlignmentY
		{
			get { return this._alignmentY; }
			set
			{
				System.Drawing.StringAlignment oldValue = _alignmentY;
				this._alignmentY = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Vertical aligment of the label.</summary>
		public System.Drawing.StringAlignment AlignmentZ
		{
			get { return this._alignmentZ; }
			set
			{
				System.Drawing.StringAlignment oldValue = _alignmentZ;
				this._alignmentZ = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		#endregion Properties

		public CSLineID AxisStyleID
		{
			get
			{
				return _cachedAxisStyleInfo.Identifier;
			}
		}

		public CSAxisInformation CachedAxisInformation
		{
			get
			{
				return _cachedAxisStyleInfo;
			}
			set
			{
				_cachedAxisStyleInfo = value;
			}
		}

		public RectangleD3D AdjustRectangle(RectangleD3D r, StringAlignment alignmentX, StringAlignment alignmentY, StringAlignment alignmentZ)
		{
			double rX = r.X;
			double rY = r.Y;
			double rZ = r.Z;

			switch (alignmentZ)
			{
				case StringAlignment.Near:
					break;

				case StringAlignment.Center:
					rZ -= 0.5 * r.SizeZ;
					break;

				case StringAlignment.Far:
					rZ -= r.SizeZ;
					break;
			}
			switch (alignmentY)
			{
				case StringAlignment.Near:
					break;

				case StringAlignment.Center:
					rY -= 0.5 * r.SizeY;
					break;

				case StringAlignment.Far:
					rY -= r.SizeY;
					break;
			}
			switch (alignmentX)
			{
				case StringAlignment.Near:
					break;

				case StringAlignment.Center:
					rX -= 0.5 * r.SizeX;
					break;

				case StringAlignment.Far:
					rX -= r.SizeX;
					break;
			}

			return new RectangleD3D(rX, rY, rZ, r.SizeX, r.SizeY, r.SizeZ);
		}

		/// <summary>Predicts the side, where the label will be shown using the given axis information.</summary>
		/// <param name="axisInformation">The axis information.</param>
		/// <returns>The side of the axis where the label will be shown.</returns>
		public virtual CSAxisSide PredictLabelSide(CSAxisInformation axisInformation)
		{
			return null != _labelSide ? _labelSide.Value : axisInformation.PreferredLabelSide;
		}

		/// <summary>
		/// Paints the axis style labels.
		/// </summary>
		/// <param name="g">Graphics environment.</param>
		/// <param name="coordSyst">The coordinate system. Used to get the path along the axis.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="tickSpacing">If not <c>null</c>, this parameter provides a custom tick spacing that is used instead of the default tick spacing of the scale.</param>
		/// <param name="styleInfo">Information about begin of axis, end of axis.</param>
		/// <param name="outerDistance">Distance between axis and labels.</param>
		/// <param name="useMinorTicks">If true, minor ticks are shown.</param>
		public virtual void Paint(IGraphicContext3D g, G3DCoordinateSystem coordSyst, Scale scale, TickSpacing tickSpacing, CSAxisInformation styleInfo, double outerDistance, bool useMinorTicks)
		{
			_cachedAxisStyleInfo = styleInfo;
			CSLineID styleID = styleInfo.Identifier;
			Scale raxis = scale;
			TickSpacing ticking = tickSpacing;

			var math = Matrix4x3.Identity;

			Logical3D r0 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisOrg);
			Logical3D r1 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisEnd);

			VectorD3D outVector;
			Logical3D outer;

			double[] relpositions;
			AltaxoVariant[] ticks;
			if (useMinorTicks)
			{
				relpositions = ticking.GetMinorTicksNormal(raxis);
				ticks = ticking.GetMinorTicksAsVariant();
			}
			else
			{
				relpositions = ticking.GetMajorTicksNormal(raxis);
				ticks = ticking.GetMajorTicksAsVariant();
			}

			if (!_suppressedLabels.IsEmpty)
			{
				List<AltaxoVariant> filteredTicks = new List<AltaxoVariant>();
				List<double> filteredRelPositions = new List<double>();

				for (int i = 0; i < ticks.Length; i++)
				{
					if (_suppressedLabels.ByValues.Contains(ticks[i]))
						continue;
					if (_suppressedLabels.ByNumbers.Contains(i))
						continue;
					if (_suppressedLabels.ByNumbers.Contains(i - ticks.Length))
						continue;

					filteredTicks.Add(ticks[i]);
					filteredRelPositions.Add(relpositions[i]);
				}
				ticks = filteredTicks.ToArray();
				relpositions = filteredRelPositions.ToArray();
			}

			IMeasuredLabelItem[] labels = _labelFormatting.GetMeasuredItems(g, _font, _stringFormat, ticks);

			double emSize = _font.Size;
			CSAxisSide labelSide = null != _labelSide ? _labelSide.Value : styleInfo.PreferredLabelSide;
			var labelOutlines = new RectangularObjectOutline[ticks.Length];
			for (int i = 0; i < ticks.Length; i++)
			{
				double r = relpositions[i];

				if (!Altaxo.Calc.RMath.IsInIntervalCC(r, -1000, 1000))
					continue;

				outer = coordSyst.GetLogicalDirection(styleID.ParallelAxisNumber, labelSide);
				PointD3D tickorg = coordSyst.GetPositionAndNormalizedDirection(r0, r1, r, outer, out outVector);
				PointD3D tickend = tickorg + outVector * outerDistance;

				var msize = labels[i].Size;
				var morg = tickend;

				if (_automaticRotationShift)
				{
					// if this option is choosen, we have to find a shift value that shifts the center of the text outwards so that the bounding box of the text will not cross the plane that is
					// defined by the tickend point and the normal vector outVector

					// Assume that the text is now centered x, y, and z around the point tickend (but here we use origin instead tickend)
					math = Matrix4x3.FromRotation(_rotationX, _rotationY, _rotationZ);
					// we have to find all points with negative distance to the plane spanned by tickend and the vector outVector (but again instead of tickend we use origin)
					var msizePad = msize + new VectorD3D(
					(_font.Size * 1) / 3, // whereas above and below text no padding is neccessary, it is optically nicer to have left and right padding of the string by 1/6 of font size.
					0,
					(_font.Size * 1) / 3 // same padding applies to z
					);
					var crect = new RectangleD3D((PointD3D)(-0.5 * msizePad), msizePad); // our text centered around origin

					double shift = 0;
					foreach (PointD3D p in crect.Vertices)
					{
						PointD3D ps = math.TransformPoint(p);
						double distance = Math3D.GetDistancePointToPlane(ps, PointD3D.Empty, outVector);
						if (-distance > shift)
							shift = -distance; // only negative distances will count here
					}
					morg += outVector * shift;
				}
				else
				{
					morg = morg.WithXPlus(outVector.X * _font.Size / 3);
				}

				var mrect = new RectangleD3D(morg, msize);
				if (_automaticRotationShift)
					mrect = AdjustRectangle(mrect, StringAlignment.Center, StringAlignment.Center, StringAlignment.Center);
				else
					mrect = AdjustRectangle(mrect, _alignmentX, _alignmentY, _alignmentZ);

				math = Matrix4x3.Identity;
				math.TranslatePrepend(morg.X, morg.Y, morg.Z);

				if (this._rotationZ != 0)
					math.RotationZDegreePrepend(this._rotationZ);
				if (this._rotationY != 0)
					math.RotationYDegreePrepend(this._rotationY);
				if (this._rotationX != 0)
					math.RotationXDegreePrepend(this._rotationX);

				math.TranslatePrepend((mrect.X - morg.X + emSize * _offsetX), (mrect.Y - morg.Y + emSize * _offsetY), (mrect.Z - morg.Z + emSize * _offsetZ));

				var gs = g.SaveGraphicsState();
				g.PrependTransform(math);

				if (this._backgroundStyle != null)
					_backgroundStyle.Draw(g, new RectangleD3D(PointD3D.Empty, msize));

				labels[i].Draw(g, _brush, PointD3D.Empty);
				labelOutlines[i] = new RectangularObjectOutline(new RectangleD3D(PointD3D.Empty, msize), math);
				g.RestoreGraphicsState(gs); // Restore the graphics state
			}

			_cachedLabelOutlines = labelOutlines;
		}

		public IHitTestObject HitTest(HitTestPointData hitData)
		{
			var labelOutlines = _cachedLabelOutlines;

			if (null == labelOutlines)
				return null;

			foreach (var outline in labelOutlines)
			{
				if (outline.IsHittedBy(hitData))
					return new HitTestObject(new MultipleRectangularObjectOutlines(hitData.WorldTransformation, labelOutlines), this);
			}
			return null;
		}

		#region IRoutedPropertyReceiver Members

		public virtual void SetRoutedProperty(IRoutedSetterProperty property)
		{
			switch (property.Name)
			{
				case "FontSize":
					{
						var prop = (RoutedSetterProperty<double>)property;
						this.FontSize = prop.Value;
						EhSelfChanged(EventArgs.Empty);
					}
					break;

				case "FontFamily":
					{
						var prop = (RoutedSetterProperty<string>)property;
						try
						{
							var newFont = _font.GetFontWithNewFamily(prop.Value);
							_font = newFont;
							EhSelfChanged(EventArgs.Empty);
						}
						catch (Exception)
						{
						}
					}
					break;
			}
		}

		public virtual void GetRoutedProperty(IRoutedGetterProperty property)
		{
			switch (property.Name)
			{
				case "FontSize":
					((RoutedGetterProperty<double>)property).Merge(this.FontSize);
					break;
			}
		}

		#endregion IRoutedPropertyReceiver Members
	}
}