#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
	using Altaxo.Main;
	using Background;
	using Drawing;
	using Drawing.D3D;
	using Drawing.D3D.Material;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using GraphicsContext;
	using Plot.Data;
	using Plot.Groups;

	public class LabelPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG3DPlotStyle
	{
		protected Altaxo.Data.IReadableColumnProxy _labelColumnProxy;

		/// <summary>The axis where the label is attached to (if it is attached).</summary>
		protected CSPlaneID _attachedPlane;

		protected bool _independentSkipFrequency;

		/// <summary>
		/// Skip frequency.
		/// </summary>
		protected int _skipFrequency;

		/// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
		protected bool _independentOnShiftingGroupStyles;

		/// <summary>
		/// The label format string (C# format).
		/// </summary>
		protected string _labelFormatString;

		/// <summary>The font of the label.</summary>
		protected FontX3D _font;

		/// <summary>The brush for the label.</summary>
		protected IMaterial _material;

		/// <summary>
		/// True if the color of the label is not dependent on the color of the parent plot style.
		/// </summary>
		protected bool _independentColor;

		protected Alignment _alignmentX;
		protected Alignment _alignmentY;
		protected Alignment _alignmentZ;

		/// <summary>The rotation around x-axis of the label.</summary>
		protected double _rotationX;

		/// <summary>The rotation around y-axis of the label.</summary>
		protected double _rotationY;

		/// <summary>The rotation around z-axis of the label.</summary>
		protected double _rotationZ;

		/// <summary>The x offset in EM units.</summary>
		protected double _offsetX;

		/// <summary>The y offset in EM units.</summary>
		protected double _offsetY;

		/// <summary>The z offset in EM units.</summary>
		protected double _offsetZ;

		protected ColorLinkage _backgroundColorLinkage;

		/// <summary>The style for the background.</summary>
		protected IBackgroundStyle _backgroundStyle;

		/// <summary>If this function is set, the label color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, System.Drawing.Color> _cachedColorForIndexFunction;

		/// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
		private double _cachedLogicalShiftX;

		/// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
		private double _cachedLogicalShiftY;

		/// <summary>Logical z shift between the location of the real data point and the point where the item is finally drawn.</summary>
		private double _cachedLogicalShiftZ;

		#region Serialization

		/// <summary>
		/// <para>Date: 2016-06-22</para>
		/// <para>Added: BackgroundColorLinkage</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelPlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LabelPlotStyle s = (LabelPlotStyle)obj;

				info.AddValue("LabelColumn", s._labelColumnProxy);
				info.AddValue("AttachedAxis", s._attachedPlane);
				info.AddValue("IndependentSkipFreq", s._independentSkipFrequency);
				info.AddValue("SkipFreq", s._skipFrequency);
				info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);
				info.AddValue("LabelFormat", s._labelFormatString);

				info.AddValue("Font", s._font);
				info.AddValue("Material", s._material);
				info.AddValue("IndependentColor", s._independentColor);

				info.AddEnum("AlignmentX", s._alignmentX);
				info.AddEnum("AlignmentY", s._alignmentY);
				info.AddEnum("AlignmentZ", s._alignmentY);

				info.AddValue("RotationX", s._rotationX);
				info.AddValue("RotationY", s._rotationY);
				info.AddValue("RotationZ", s._rotationZ);

				info.AddValue("OffsetX", s._offsetX);
				info.AddValue("OffsetY", s._offsetY);
				info.AddValue("OffsetZ", s._offsetZ);

				info.AddEnum("BackgroundColorLinkage", s._backgroundColorLinkage);
				info.AddValue("Background", s._backgroundStyle);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent, true);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
			{
				LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle(info);

				s.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)info.GetValue("LabelColumn", s);
				s.AttachedPlane = (CSPlaneID)info.GetValue("AttachedPlane", s);
				s._independentSkipFrequency = info.GetBoolean("IndependentSkipFreq");
				s._skipFrequency = info.GetInt32("SkipFreq");
				s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");
				s._labelFormatString = info.GetString("LabelFormat");

				s._font = (FontX3D)info.GetValue("Font", s);
				s._material = (IMaterial)info.GetValue("Material", s);
				s._independentColor = info.GetBoolean("IndependentColor");

				s._alignmentX = (Alignment)info.GetEnum("AlignmentX", typeof(Alignment));
				s._alignmentY = (Alignment)info.GetEnum("AlignmentY", typeof(Alignment));
				s._alignmentZ = (Alignment)info.GetEnum("AlignmentZ", typeof(Alignment));

				s._rotationX = info.GetDouble("RotationX");
				s._rotationY = info.GetDouble("RotationY");
				s._rotationZ = info.GetDouble("RotationZ");

				s._offsetX = info.GetDouble("OffsetX");
				s._offsetY = info.GetDouble("OffsetY");
				s._offsetZ = info.GetDouble("OffsetZ");

				s._backgroundColorLinkage = (ColorLinkage)info.GetEnum("BackgroundColorLinkage", typeof(ColorLinkage));

				s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background", s);
				if (null != s._backgroundStyle) s._backgroundStyle.ParentObject = s;

				if (nativeCall)
				{
					// restore the cached values
					s.SetCachedValues();
				}

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info">The deserialization information.</param>
		protected LabelPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			this._backgroundColorLinkage = ColorLinkage.Independent;
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as LabelPlotStyle;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				this.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)from._labelColumnProxy.Clone();
				this._attachedPlane = from._attachedPlane;
				this._independentSkipFrequency = from._independentSkipFrequency;
				this._skipFrequency = from._skipFrequency;
				this._independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;
				this._labelFormatString = from._labelFormatString;

				this._font = from._font;
				this._material = from._material;
				this._independentColor = from._independentColor;

				this._alignmentX = from._alignmentX;
				this._alignmentY = from._alignmentY;
				this._alignmentZ = from._alignmentZ;

				this._rotationX = from._rotationX;
				this._rotationY = from._rotationY;
				this._rotationZ = from._rotationZ;

				this._offsetX = from._offsetX;
				this._offsetY = from._offsetY;
				this._offsetZ = from._offsetZ;

				this._backgroundColorLinkage = from._backgroundColorLinkage;
				ChildCopyToMember(ref _backgroundStyle, from._backgroundStyle);

				this._cachedLogicalShiftX = from._cachedLogicalShiftX;
				this._cachedLogicalShiftY = from._cachedLogicalShiftY;
				this._cachedLogicalShiftZ = from._cachedLogicalShiftZ;

				EhSelfChanged(EventArgs.Empty);
				suspendToken.Resume();
			}
			return true;
		}

		public LabelPlotStyle(LabelPlotStyle from)
		{
			CopyFrom(from);
		}

		public LabelPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: this((Altaxo.Data.IReadableColumn)null, context)
		{
		}

		public LabelPlotStyle(Altaxo.Data.IReadableColumn labelColumn, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			this._font = GraphDocument.GetDefaultFont(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			this._independentColor = false;
			this._material = new MaterialWithUniformColor(color);
			this._offsetX = 0;
			this._offsetY = 0;
			this._offsetZ = 0;
			this._rotationX = 0;
			this._rotationY = 0;
			this._rotationZ = 0;
			this._backgroundStyle = null;
			this._backgroundColorLinkage = ColorLinkage.Independent;
			this._attachedPlane = null;
			this.LabelColumnProxy = Altaxo.Data.ReadableColumnProxyBase.FromColumn(labelColumn);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _backgroundStyle)
				yield return new Main.DocumentNodeAndName(_backgroundStyle, "Background");

			if (null != _labelColumnProxy)
				yield return new Main.DocumentNodeAndName(_labelColumnProxy, "LabelColumn");
		}

		private void EhLabelColumnProxyChanged(object sender, EventArgs e)
		{
			EhSelfChanged(EventArgs.Empty);
		}

		protected Altaxo.Data.IReadableColumnProxy LabelColumnProxy
		{
			set
			{
				if (ChildSetMember(ref _labelColumnProxy, value))
				{
					if (null != _labelColumnProxy)
						EhChildChanged(_labelColumnProxy, EventArgs.Empty);
					else
						EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public Altaxo.Data.IReadableColumn LabelColumn
		{
			get
			{
				return _labelColumnProxy == null ? null : _labelColumnProxy.Document;
			}
			set
			{
				if (object.ReferenceEquals(LabelColumn, value))
					return;

				LabelColumnProxy = Altaxo.Data.ReadableColumnProxyBase.FromColumn(value);
			}
		}

		/// <summary>
		/// Gets the name of the label column, if it is a data column. Otherwise, null is returned.
		/// </summary>
		/// <value>
		/// The name of the label column if it is a data column. Otherwise, null.
		/// </value>
		public string LabelColumnDataColumnName
		{
			get
			{
				return _labelColumnProxy.DocumentPath.LastPartOrDefault;
			}
		}

		public IEnumerable<Tuple<string, Altaxo.Data.IReadableColumn, string, Action<Altaxo.Data.IReadableColumn>>> GetAdditionallyUsedColumns()
		{
			yield return new Tuple<string, Altaxo.Data.IReadableColumn, string, Action<Altaxo.Data.IReadableColumn>>(
				"Label",
			LabelColumn,
			LabelColumnDataColumnName,
			(col) => this.LabelColumn = col);
		}

		public string LabelFormatString
		{
			get
			{
				return _labelFormatString;
			}
			set
			{
				if (!(_labelFormatString == value))
				{
					_labelFormatString = value;
					EhSelfChanged();
				}
			}
		}

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
				if (!value.Equals(oldValue))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>The font size of the label.</summary>
		public double FontSize
		{
			get { return _font.Size; }
			set
			{
				var oldValue = _font.Size;
				var newValue = Math.Max(0, value);

				if (newValue != oldValue)
				{
					_font = _font.WithSize(newValue);
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		/// <summary>
		/// Determines whether or not the color of the label is independent of the color of the parent plot style.
		/// </summary>
		public bool IndependentColor
		{
			get { return _independentColor; }
			set
			{
				bool oldValue = _independentColor;
				_independentColor = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public IMaterial Material
		{
			get
			{
				return _material;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(value, _material))
				{
					_material = value;

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The background style.</summary>
		public IBackgroundStyle BackgroundStyle
		{
			get
			{
				return _backgroundStyle;
			}
			set
			{
				IBackgroundStyle oldValue = this._backgroundStyle;
				if (!object.ReferenceEquals(value, oldValue))
				{
					this._backgroundStyle = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public ColorLinkage BackgroundColorLinkage
		{
			get
			{
				return _backgroundColorLinkage;
			}
			set
			{
				var oldValue = _backgroundColorLinkage;
				_backgroundColorLinkage = value;
				if (oldValue != value)
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

		/// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
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

		/// <summary>The angle of the label around x-axis.</summary>
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

		/// <summary>The angle of the label around y-axis.</summary>
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

		/// <summary>The angle of the label around z-axis.</summary>
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

		/// <summary>Horizontal alignment of the label.</summary>
		public Alignment AlignmentX
		{
			get
			{
				return this._alignmentX;
			}
			set
			{
				if (!(_alignmentX == value))
				{
					_alignmentX = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>Vertical aligment of the label.</summary>
		public Alignment AlignmentY
		{
			get
			{
				return this._alignmentY;
			}
			set
			{
				if (!(_alignmentY == value))
				{
					_alignmentY = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>Vertical aligment of the label.</summary>
		public Alignment AlignmentZ
		{
			get
			{
				return this._alignmentZ;
			}
			set
			{
				if (!(_alignmentZ == value))
				{
					_alignmentZ = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Gets/sets the axis this label is attached to. If set to null, the label is positioned normally.</summary>
		public CSPlaneID AttachedPlane
		{
			get { return this._attachedPlane; }
			set
			{
				CSPlaneID oldValue = this._attachedPlane;
				this._attachedPlane = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Controls how many items are plotted. A value of 1 means every item, a value of 2 every other item, and so on.</summary>
		public int SkipFrequency
		{
			get { return _skipFrequency; }
			set
			{
				if (!(_skipFrequency == value))
				{
					_skipFrequency = Math.Max(1, value);
					EhSelfChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the skip frequency is independent on other sub group styles using <see cref="SkipFrequency"/>.
		/// </summary>
		/// <value>
		/// <c>true</c> if the skip frequency is independent on other sub group styles using <see cref="SkipFrequency"/>; otherwise, <c>false</c>.
		/// </value>
		public bool IndependentSkipFrequency
		{
			get { return _independentSkipFrequency; }
			set
			{
				if (!(_independentSkipFrequency == value))
				{
					_independentSkipFrequency = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>
		/// True when we don't want to shift the position of the items, for instance due to the bar graph plot group.
		/// </summary>
		public bool IndependentOnShiftingGroupStyles
		{
			get
			{
				return _independentOnShiftingGroupStyles;
			}
			set
			{
				if (!(_independentOnShiftingGroupStyles == value))
				{
					_independentOnShiftingGroupStyles = value;
					EhSelfChanged();
				}
			}
		}

		protected void SetCachedValues()
		{
		}

		/// <summary>
		/// Paints one label.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="label"></param>
		/// <param name="variableTextBrush">If not null, this argument provides the text brush that should be used now. If null, then the <see cref="_material"/> is used instead.</param>
		/// <param name="variableBackBrush"></param>
		public void Paint(IGraphicsContext3D g, string label, IMaterial variableTextBrush, IMaterial variableBackBrush)
		{
			var fontSize = this.FontSize;
			var xpos = (_offsetX * fontSize);
			var ypos = (_offsetY * fontSize);
			var zpos = (_offsetZ * fontSize);
			var stringsize = g.MeasureString(label, _font, new PointD3D(xpos, ypos, zpos));

			if (this._backgroundStyle != null)
			{
				var x = xpos;
				var y = ypos;
				var z = zpos;

				switch (_alignmentX)
				{
					case Alignment.Center:
						x -= stringsize.X / 2;
						break;

					case Alignment.Far:
						x -= stringsize.X;
						break;
				}

				switch (_alignmentY)
				{
					case Alignment.Center:
						y -= stringsize.Y / 2;
						break;

					case Alignment.Far:
						y -= stringsize.Y;
						break;
				}
				if (null == variableBackBrush)
				{
					this._backgroundStyle.Draw(g, new RectangleD3D(x, y, z, stringsize.X, stringsize.Y, stringsize.Z));
				}
				else
				{
					this._backgroundStyle.Draw(g, new RectangleD3D(x, y, z, stringsize.X, stringsize.Y, stringsize.Z), variableBackBrush);
				}
			}

			var brush = null != variableTextBrush ? variableTextBrush : _material;
			g.DrawString(label, _font, brush, new PointD3D(xpos, ypos, zpos), _alignmentX, _alignmentY, _alignmentZ);
		}

		public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData prevItemData, Processed3DPlotData nextItemData)
		{
			if (this._labelColumnProxy.Document == null)
				return;

			if (null != _attachedPlane)
				_attachedPlane = layer.UpdateCSPlaneID(_attachedPlane);

			PlotRangeList rangeList = pdata.RangeList;
			var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
			Altaxo.Data.IReadableColumn labelColumn = this._labelColumnProxy.Document;

			bool isUsingVariableColorForLabelText = null != _cachedColorForIndexFunction && IsColorReceiver;
			bool isUsingVariableColorForLabelBackground = null != _cachedColorForIndexFunction &&
				(null != _backgroundStyle && _backgroundStyle.SupportsUserDefinedMaterial && (_backgroundColorLinkage == ColorLinkage.Dependent || _backgroundColorLinkage == ColorLinkage.PreserveAlpha));
			bool isUsingVariableColor = isUsingVariableColorForLabelText || isUsingVariableColorForLabelBackground;
			IMaterial clonedTextBrush = _material;
			IMaterial clonedBackBrush = null;
			if (isUsingVariableColorForLabelBackground)
				clonedBackBrush = _backgroundStyle.Material;

			// save the graphics stat since we have to translate the origin
			var gs = g.SaveGraphicsState();

			double xpos = 0, ypos = 0, zpos = 0;
			double xpre, ypre, zpre;
			double xdiff, ydiff, zdiff;

			bool mustUseLogicalCoordinates = null != this._attachedPlane || 0 != _cachedLogicalShiftX || 0 != _cachedLogicalShiftY || 0 != _cachedLogicalShiftZ;

			for (int r = 0; r < rangeList.Count; r++)
			{
				int lower = rangeList[r].LowerBound;
				int upper = rangeList[r].UpperBound;
				int offset = rangeList[r].OffsetToOriginal;
				for (int j = lower; j < upper; j++)
				{
					string label = labelColumn[j + offset].ToString();
					if (label == null || label == string.Empty)
						continue;

					// Start of preparation of brushes, if a variable color is used
					if (isUsingVariableColor)
					{
						var c = _cachedColorForIndexFunction(j + offset);

						if (isUsingVariableColorForLabelText)
						{
							clonedTextBrush = clonedTextBrush.WithColor(new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B), "e"));
						}
						if (isUsingVariableColorForLabelBackground)
						{
							if (_backgroundColorLinkage == ColorLinkage.PreserveAlpha)
								clonedBackBrush = clonedBackBrush.WithColor(new NamedColor(AxoColor.FromArgb(clonedBackBrush.Color.Color.A, c.R, c.G, c.B), "e"));
							else
								clonedBackBrush = clonedBackBrush.WithColor(new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B), "e"));
						}
					}
					// end of preparation of brushes for variable colors

					if (mustUseLogicalCoordinates) // we must use logical coordinates because either there is a shift of logical coordinates, or an attached plane
					{
						Logical3D r3d = layer.GetLogical3D(pdata, j + offset);
						r3d.RX += _cachedLogicalShiftX;
						r3d.RY += _cachedLogicalShiftY;
						r3d.RZ += _cachedLogicalShiftZ;

						if (null != this._attachedPlane)
						{
							var pp = layer.CoordinateSystem.GetPointOnPlane(this._attachedPlane, r3d);
							xpre = pp.X;
							ypre = pp.Y;
							zpre = pp.Z;
						}
						else
						{
							PointD3D pt;
							layer.CoordinateSystem.LogicalToLayerCoordinates(r3d, out pt);
							xpre = pt.X;
							ypre = pt.Y;
							zpre = pt.Z;
						}
					}
					else // no shifting, thus we can use layer coordinates
					{
						xpre = ptArray[j].X;
						ypre = ptArray[j].Y;
						zpre = ptArray[j].Z;
					}

					xdiff = xpre - xpos;
					ydiff = ypre - ypos;
					zdiff = zpre - zpos;
					xpos = xpre;
					ypos = ypre;
					zpos = zpre;
					g.TranslateTransform(xdiff, ydiff, zdiff);
					g.RotateTransform(_rotationX, _rotationY, _rotationZ);

					this.Paint(g, label, clonedTextBrush, clonedBackBrush);

					g.RotateTransform(-_rotationX, -_rotationY, -_rotationZ);
				} // end for
			}

			g.RestoreGraphicsState(gs); // Restore the graphics state
		}

		public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
		{
			return bounds;
		}

		public object Clone()
		{
			return new LabelPlotStyle(this);
		}

		#region I3DPlotStyle Members

		public bool IsColorProvider
		{
			get { return this._independentColor == false; }
		}

		public bool IsColorReceiver
		{
			get { return this.IndependentColor == false; }
		}

		/// <summary>
		/// Gets a value indicating whether the background can provide a color for use by other plot styles.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the background is able to provide a color; otherwise, <c>false</c>.
		/// </value>
		public bool IsBackgroundColorProvider
		{
			get
			{
				return
					_backgroundStyle != null &&
					_backgroundStyle.SupportsUserDefinedMaterial &&
					_backgroundColorLinkage == ColorLinkage.Dependent;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the background can receive a color value.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is a receiver for background color; otherwise, <c>false</c>.
		/// </value>
		public bool IsBackgroundColorReceiver
		{
			get
			{
				return
					_backgroundStyle != null &&
					_backgroundStyle.SupportsUserDefinedMaterial &&
					(_backgroundColorLinkage == ColorLinkage.Dependent || _backgroundColorLinkage == ColorLinkage.PreserveAlpha);
			}
		}

		public bool IsSymbolSizeProvider
		{
			get { return false; }
		}

		public bool IsSymbolSizeReceiver
		{
			get { return false; }
		}

		public float SymbolSize
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		#region IG3DPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.Material.Color; });
			else if (this.IsBackgroundColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this._backgroundStyle.Material.Color; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			_cachedColorForIndexFunction = null;

			if (this.IsColorReceiver)
			{
				// try to get a constant color ...
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this.Material = this.Material.WithColor(c); });
			}

			if (this.IsBackgroundColorReceiver)
			{
				if (this._backgroundColorLinkage == ColorLinkage.Dependent)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this._backgroundStyle.Material = this._backgroundStyle.Material.WithColor(c); });
				else if (this._backgroundColorLinkage == ColorLinkage.PreserveAlpha)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this._backgroundStyle.Material = this._backgroundStyle.Material.WithColor(c.NewWithAlphaValue(_backgroundStyle.Material.Color.Color.A)); });
			}

			if (this.IsColorReceiver || this.IsBackgroundColorReceiver)
			{
				// but if there is a color evaluation function, then use that function with higher priority
				VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, System.Drawing.Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; });
			}

			// Shift the items ?
			_cachedLogicalShiftX = 0;
			_cachedLogicalShiftY = 0;
			if (!_independentOnShiftingGroupStyles)
			{
				var shiftStyle = PlotGroupStyle.GetFirstStyleToApplyImplementingInterface<IShiftLogicalXYZGroupStyle>(externalGroups, localGroups);
				if (null != shiftStyle)
				{
					shiftStyle.Apply(out _cachedLogicalShiftX, out _cachedLogicalShiftY, out _cachedLogicalShiftZ);
				}
			}
		}

		#endregion IG3DPlotStyle Members

		#endregion I3DPlotStyle Members

		#region IDocumentNode Members

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Information what to replace.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			Report(_labelColumnProxy, this, "LabelColumn");
		}

		#endregion IDocumentNode Members
	}
}