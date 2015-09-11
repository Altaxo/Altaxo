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

namespace Altaxo.Graph3D.Axis
{
	using Altaxo.Graph;
	using Altaxo.Graph3D.LabelFormatting;

	/// <summary>
	/// Responsible for setting position, rotation, font, color etc. of axis labels.
	/// </summary>
	public class AxisLabelStyle3D :
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IRoutedPropertyReceiver,
		Main.ICopyFrom
	{
		protected FontX3D _font;

		protected StringAlignment _horizontalAlignment;
		protected StringAlignment _verticalAlignment;

		protected StringFormat _stringFormat;
		protected IMaterial3D _brush;

		/// <summary>The x offset in EM units.</summary>
		protected double _xOffset;

		/// <summary>The y offset in EM units.</summary>
		protected double _yOffset;

		/// <summary>The z offset in EM units.</summary>
		protected double _zOffset;

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

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLabelStyle3D), 0)]
		private class XmlSerializationSurrogate6 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			// 2012-05-28 _prefixText and _postfixText deprecated and moved to LabelFormattingBase

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AxisLabelStyle3D s = (AxisLabelStyle3D)obj;
				info.AddValue("Font", s._font);
				info.AddValue("Brush", s._brush);
				info.AddValue("Background", s._backgroundStyle);

				info.AddValue("AutoAlignment", s._automaticRotationShift);
				info.AddEnum("HorzAlignment", s._horizontalAlignment);
				info.AddEnum("VertAlignment", s._verticalAlignment);

				info.AddValue("RotationX", s._rotationX);
				info.AddValue("RotationY", s._rotationY);
				info.AddValue("RotationZ", s._rotationZ);

				info.AddValue("XOffset", s._xOffset);
				info.AddValue("YOffset", s._yOffset);
				info.AddValue("ZOffset", s._zOffset);

				if (s._suppressedLabels.IsEmpty)
					info.AddValue("SuppressedLabels", (object)null);
				else
					info.AddValue("SuppressedLabels", s._suppressedLabels);

				info.AddValue("LabelFormat", s._labelFormatting);

				info.AddNullableEnum("LabelSide", s._labelSide);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AxisLabelStyle3D s = null != o ? (AxisLabelStyle3D)o : new AxisLabelStyle3D(info);

				s._font = (FontX3D)info.GetValue("Font", s);
				s._brush = (IMaterial3D)info.GetValue("Brush", s);
				s._brush.ParentObject = s;

				s.BackgroundStyle = (Background.IBackgroundStyle3D)info.GetValue("Background", s);

				s._automaticRotationShift = info.GetBoolean("AutoAlignment");
				s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment", typeof(StringAlignment));
				s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment", typeof(StringAlignment));
				s._rotationX = info.GetDouble("RotationX");
				s._rotationY = info.GetDouble("RotationY");
				s._rotationZ = info.GetDouble("RotationZ");
				s._xOffset = info.GetDouble("XOffset");
				s._yOffset = info.GetDouble("YOffset");
				s._zOffset = info.GetDouble("ZOffset");

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

		protected AxisLabelStyle3D(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		public AxisLabelStyle3D(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			_font = Graph3DDocument.GetDefaultFont(context);
			var foreColor = Graph3DDocument.GetDefaultForeColor(context);

			_brush = Materials.GetSolidMaterial(foreColor);
			_brush.ParentObject = this;

			_automaticRotationShift = true;
			_suppressedLabels = new SuppressedTicks() { ParentObject = this };
			_labelFormatting = new Altaxo.Graph3D.LabelFormatting.NumericLabelFormattingAuto() { ParentObject = this };
			SetStringFormat();
		}

		public AxisLabelStyle3D(AxisLabelStyle3D from)
		{
			CopyFrom(from);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as AxisLabelStyle3D;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				_cachedAxisStyleInfo = from._cachedAxisStyleInfo;

				_font = from._font;
				CopyHelper.Copy(ref _stringFormat, from._stringFormat);
				_horizontalAlignment = from._horizontalAlignment;
				_verticalAlignment = from._verticalAlignment;

				ChildCopyToMember(ref _brush, from._brush);

				_automaticRotationShift = from._automaticRotationShift;
				_xOffset = from._xOffset;
				_yOffset = from._yOffset;
				_zOffset = from._zOffset;
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
			return new AxisLabelStyle3D(this);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _brush)
				yield return new Main.DocumentNodeAndName(_brush, "Brush");

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
				var oldColor = this.Color;
				if (value != oldColor)
				{
					if (this._brush.SupportsSetColor)
						this._brush.Color = value;
					else
						this._brush = Materials.GetSolidMaterial(value);

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The brush. During setting, the brush is cloned.</summary>
		public IMaterial3D Brush
		{
			get
			{
				return this._brush; ;
			}
			set
			{
				this._brush = (IMaterial3D)value.Clone();
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
		public double XOffset
		{
			get { return this._xOffset; }
			set
			{
				double oldValue = this._xOffset;
				this._xOffset = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
		public double YOffset
		{
			get { return this._yOffset; }
			set
			{
				double oldValue = this._yOffset;
				this._yOffset = value;
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
		public System.Drawing.StringAlignment HorizontalAlignment
		{
			get
			{
				return this._horizontalAlignment;
			}
			set
			{
				System.Drawing.StringAlignment oldValue = this.HorizontalAlignment;
				this._horizontalAlignment = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Vertical aligment of the label.</summary>
		public System.Drawing.StringAlignment VerticalAlignment
		{
			get { return this._verticalAlignment; }
			set
			{
				System.Drawing.StringAlignment oldValue = this.VerticalAlignment;
				this._verticalAlignment = value;
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

		public void AdjustRectangle(ref RectangleD3D r, StringAlignment horz, StringAlignment vert)
		{
			switch (vert)
			{
				case StringAlignment.Near:
					break;

				case StringAlignment.Center:
					r.Y -= 0.5 * r.SizeY;
					break;

				case StringAlignment.Far:
					r.Y -= r.SizeY;
					break;
			}
			switch (horz)
			{
				case StringAlignment.Near:
					break;

				case StringAlignment.Center:
					r.X -= 0.5 * r.SizeX;
					break;

				case StringAlignment.Far:
					r.X -= r.SizeX;
					break;
			}
		}

		/// <summary>Predicts the side, where the label will be shown using the given axis information.</summary>
		/// <param name="axisInformation">The axis information.</param>
		/// <returns>The side of the axis where the label will be shown.</returns>
		public virtual CSAxisSide PredictLabelSide(CSAxisInformation axisInformation)
		{
			return null != _labelSide ? _labelSide.Value : axisInformation.PreferedLabelSide;
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

			var math = MatrixD3D.Identity;

			Logical3D r0 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisOrg);
			Logical3D r1 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisEnd);

			VectorD3D outVector;
			Logical3D outer;
			var dist_x = outerDistance; // Distance from axis tick point to label
			var dist_y = outerDistance; // y distance from axis tick point to label

			// dist_x += this._font.SizeInPoints/3; // add some space to the horizontal direction in order to separate the chars a little from the ticks

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
			CSAxisSide labelSide = null != _labelSide ? _labelSide.Value : styleInfo.PreferedLabelSide;
			for (int i = 0; i < ticks.Length; i++)
			{
				double r = relpositions[i];

				if (!Altaxo.Calc.RMath.IsInIntervalCC(r, -1000, 1000))
					continue;

				outer = coordSyst.GetLogicalDirection(styleID.ParallelAxisNumber, labelSide);
				PointD3D tickorg = coordSyst.GetNormalizedDirection(r0, r1, r, outer, out outVector);
				PointD3D tickend = tickorg;
				tickend.X += outVector.X * outerDistance;
				tickend.Y += outVector.Y * outerDistance;

				var msize = labels[i].Size;
				var morg = tickend;

				if (_automaticRotationShift)
				{
					double alpha = _rotationZ * Math.PI / 180 - Math.Atan2(outVector.Y, outVector.X);
					double shift = msize.Y * 0.5 * Math.Abs(Math.Sin(alpha)) + (msize.X + _font.Size / 2) * 0.5 * Math.Abs(Math.Cos(alpha));
					morg.X += (outVector.X * shift);
					morg.Y += (outVector.Y * shift);
				}
				else
				{
					morg.X += (outVector.X * _font.Size / 3);
				}

				var mrect = new RectangleD3D(morg, msize);
				if (_automaticRotationShift)
					AdjustRectangle(ref mrect, StringAlignment.Center, StringAlignment.Center);
				else
					AdjustRectangle(ref mrect, _horizontalAlignment, _verticalAlignment);

				math = MatrixD3D.Identity;
				math.TranslatePrepend(morg.X, morg.Y, morg.Z);

				if (this._rotationZ != 0)
					math.RotationZDegreePrepend(-this._rotationZ);
				if (this._rotationY != 0)
					math.RotationYDegreePrepend(-this._rotationZ);
				if (this._rotationX != 0)
					math.RotationXDegreePrepend(-this._rotationZ);

				math.TranslatePrepend((mrect.X - morg.X + emSize * _xOffset), (mrect.Y - morg.Y + emSize * _yOffset), (mrect.Z - morg.Z + emSize * _zOffset));

				var gs = g.SaveGraphicsState();
				g.MultiplyTransform(math);

				if (this._backgroundStyle != null)
					_backgroundStyle.Draw(g, new RectangleD3D(PointD3D.Empty, msize));

				_brush.SetEnvironment(g, new RectangleD3D(PointD3D.Empty, msize));
				labels[i].Draw(g, _brush, PointD3D.Empty);
				g.RestoreGraphicsState(gs); // Restore the graphics state
			}
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