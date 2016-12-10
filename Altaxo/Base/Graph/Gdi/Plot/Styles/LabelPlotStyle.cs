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

using Altaxo.Graph.Gdi.Background;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	using Altaxo.Main;
	using Drawing;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using Plot.Data;
	using Plot.Groups;

	public class LabelPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG2DPlotStyle
	{
		protected Altaxo.Data.IReadableColumnProxy _labelColumnProxy;

		/// <summary>The axis where the label is attached to (if it is attached).</summary>
		protected CSPlaneID _attachedPlane;

		protected bool _independentSkipFrequency;

		/// <summary>
		/// Skip frequency.
		/// </summary>
		protected int _skipFrequency;

		/// <summary>
		/// If true, treat missing points as if not present (e.g. connect lines over missing points, count skip seamlessly over missing points)
		/// </summary>
		protected bool _ignoreMissingDataPoints;

		/// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
		protected bool _independentOnShiftingGroupStyles;

		/// <summary>
		/// The label format string (C# format).
		/// </summary>
		protected string _labelFormatString;

		/// <summary>The font of the label.</summary>
		protected FontX _font;

		/// <summary>
		/// Offset used to calculate the font size in dependence on the symbol size., according to the formula:
		/// fontSize = <see cref="_fontSizeOffset"/> + <see cref="_fontSizeFactor"/> * <see cref="_symbolSize"/>;
		/// </summary>
		protected double _fontSizeOffset;

		/// <summary>
		/// Factor used to calculate the font size in dependence on the symbol size, according to the formula:
		/// fontSize = <see cref="_fontSizeOffset"/> + <see cref="_fontSizeFactor"/> * <see cref="_symbolSize"/>;
		/// </summary>
		protected double _fontSizeFactor;

		protected bool _independentSymbolSize;

		protected double _symbolSize;

		/// <summary>The brush for the label.</summary>
		protected BrushX _brush;

		/// <summary>
		/// True if the color of the label is not dependent on the color of the parent plot style.
		/// </summary>
		protected bool _independentColor;

		protected Alignment _alignmentX = Alignment.Center;
		protected Alignment _alignmentY = Alignment.Center;

		/// <summary>The rotation of the label.</summary>
		protected double _rotation;

		/// Total offset is calculated according to:
		/// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
		protected double _offsetX_EmUnits;

		/// <summary>The x offset int points.</summary>
		/// Total offset is calculated according to:
		/// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
		protected double _offsetX_Points;

		/// <summary>The x offset factor to be multiplied with the symbol size.
		/// Total offset is calculated according to:
		/// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
		protected double _offsetX_SymbolSizeUnits;

		/// <summary>The y offset in EM units.</summary>
		protected double _offsetY_EmUnits;

		/// <summary>The y offset int points.</summary>
		/// Total offset is calculated according to:
		/// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
		protected double _offsetY_Points;

		/// <summary>The y offset factor to be multiplied with the symbol size.
		/// Total offset is calculated according to:
		/// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
		protected double _offsetY_SymbolSizeUnits;

		protected ColorLinkage _backgroundColorLinkage;

		/// <summary>The style for the background.</summary>
		protected Gdi.Background.IBackgroundStyle _backgroundStyle;

		// cached values:
		/// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
		[field: NonSerialized]
		protected Func<int, double> _cachedSymbolSizeForIndexFunction;

		/// <summary>If this function is set, the label color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

		/// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
		private double _cachedLogicalShiftX;

		/// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
		private double _cachedLogicalShiftY;

		[NonSerialized]
		protected System.Drawing.StringFormat _cachedStringFormat;

		#region Serialization

		private CSLineID GetDirection(EdgeType fillDir)
		{
			switch (fillDir)
			{
				case EdgeType.Bottom:
					return CSLineID.X0;

				case EdgeType.Top:
					return CSLineID.X1;

				case EdgeType.Left:
					return CSLineID.Y0;

				case EdgeType.Right:
					return CSLineID.Y1;
			}
			return null;
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLabelStyle", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public static CSPlaneID GetDirection(EdgeType fillDir)
			{
				switch (fillDir)
				{
					case EdgeType.Bottom:
						return CSPlaneID.Bottom;

					case EdgeType.Top:
						return CSPlaneID.Top;

					case EdgeType.Left:
						return CSPlaneID.Left;

					case EdgeType.Right:
						return CSPlaneID.Right;
				}
				return null;
			}

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Serialization of old versions not supported, probably a programming error");
				/*
				XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
				info.AddValue("Font", s.m_Font);
				info.AddValue("IndependentColor", s.m_IndependentColor);
				info.AddValue("Brush", s.m_Brush);
				info.AddValue("XOffset", s.m_XOffset);
				info.AddValue("YOffset", s.m_YOffset);
				info.AddValue("Rotation", s.m_Rotation);
				info.AddEnum("HorizontalAlignment", s.HorizontalAlignment);
				info.AddEnum("VerticalAlignment", s.VerticalAlignment);
				info.AddValue("AttachToAxis", s.m_AttachToAxis);
				info.AddValue("AttachedAxis", s.m_AttachedAxis);
				//info.AddValue("WhiteOut",s.m_WhiteOut);
				//info.AddValue("BackgroundBrush",s.m_BackgroundBrush);
				 */
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent, true);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
			{
				LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle(info);

				s._font = (FontX)info.GetValue("Font", s);
				s._independentColor = info.GetBoolean("IndependentColor");
				s._brush = (BrushX)info.GetValue("Brush", s);
				s._offsetX_EmUnits = info.GetDouble("XOffset");
				s._offsetY_EmUnits = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.AlignmentX = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment)));
				s.AlignmentY = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment)));
				bool attachToAxis = info.GetBoolean("AttachToAxis");
				EdgeType attachedAxis = (EdgeType)info.GetValue("AttachedAxis", s);
				bool whiteOut = info.GetBoolean("WhiteOut");
				BrushX backgroundBrush = (BrushX)info.GetValue("BackgroundBrush", s);

				if (attachToAxis)
					s._attachedPlane = GetDirection(attachedAxis);
				else
					s._attachedPlane = null;

				if (whiteOut)
					s._backgroundStyle = new FilledRectangle(backgroundBrush.Color) { ParentObject = s };

				if (nativeCall)
				{
					// restore the cached values
					s.SetCachedValues();
				}

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLabelStyle", 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Serialization of old versions not supported, probably a programming error");

				/*
				XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
				XmlSerializationSurrogate0.SSerialize(obj, info);
				info.AddValue("LabelColumn", s.m_LabelColumn);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LabelPlotStyle s = (LabelPlotStyle)XmlSerializationSurrogate0.SDeserialize(o, info, parent, false);

				s.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)info.GetValue("LabelColumn", s);

				// restore the cached values
				s.SetCachedValues();

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLabelStyle", 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Serialization of old versions not supported, probably a programming error");
				/*
				XYPlotLabelStyle s = (XYPlotLabelStyle)obj;
				info.AddValue("Font", s.m_Font);
				info.AddValue("IndependentColor", s.m_IndependentColor);
				info.AddValue("Brush", s.m_Brush);
				info.AddValue("XOffset", s.m_XOffset);
				info.AddValue("YOffset", s.m_YOffset);
				info.AddValue("Rotation", s.m_Rotation);
				info.AddEnum("HorizontalAlignment", s.HorizontalAlignment);
				info.AddEnum("VerticalAlignment", s.VerticalAlignment);
				info.AddValue("AttachToAxis", s.m_AttachToAxis);
				info.AddValue("AttachedAxis", s.m_AttachedAxis);
				info.AddValue("Background", s._backgroundStyle);
				info.AddValue("LabelColumn", s.m_LabelColumn);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent, true);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
			{
				LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle(info);

				s._font = (FontX)info.GetValue("Font", s);
				s._independentColor = info.GetBoolean("IndependentColor");
				s._brush = (BrushX)info.GetValue("Brush", s);
				s._offsetX_EmUnits = info.GetDouble("XOffset");
				s._offsetY_EmUnits = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.AlignmentX = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment)));
				s.AlignmentY = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment)));
				bool attachToAxis = info.GetBoolean("AttachToAxis");
				EdgeType attachedAxis = (EdgeType)info.GetValue("AttachedAxis", s);
				s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background", s);
				if (null != s._backgroundStyle) s._backgroundStyle.ParentObject = s;

				s.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)info.GetValue("LabelColumn", s);

				if (attachToAxis)
					s._attachedPlane = XmlSerializationSurrogate0.GetDirection(attachedAxis);
				else
					s._attachedPlane = null;

				if (nativeCall)
				{
					// restore the cached values
					s.SetCachedValues();
				}

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelPlotStyle), 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotSupportedException("Serialization of old versions not supported, probably a programming error");
				// SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version not allowed!");
				/*
				LabelPlotStyle s = (LabelPlotStyle)obj;
				info.AddValue("Font", s._font);
				info.AddValue("IndependentColor", s._independentColor);
				info.AddValue("Brush", s._brush);
				info.AddValue("XOffset", s._xOffset);
				info.AddValue("YOffset", s._yOffset);
				info.AddValue("Rotation", s._rotation);
				info.AddEnum("HorizontalAlignment", s.HorizontalAlignment);
				info.AddEnum("VerticalAlignment", s.VerticalAlignment);
				info.AddValue("AttachedAxis", s._attachedPlane);
				info.AddValue("Background", s._backgroundStyle);
				info.AddValue("LabelColumn", s._labelColumnProxy);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent, true);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
			{
				LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle(info);

				s._font = (FontX)info.GetValue("Font", s);
				s._independentColor = info.GetBoolean("IndependentColor");
				s._brush = (BrushX)info.GetValue("Brush", s);
				s._offsetX_EmUnits = info.GetDouble("XOffset");
				s._offsetY_EmUnits = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.AlignmentX = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment)));
				s.AlignmentY = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment)));
				s.AttachedAxis = (CSPlaneID)info.GetValue("AttachedAxis", s);
				s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background", s);
				if (null != s._backgroundStyle) s._backgroundStyle.ParentObject = s;

				s.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)info.GetValue("LabelColumn", s);

				if (nativeCall)
				{
					// restore the cached values
					s.SetCachedValues();
				}

				return s;
			}
		}

		/// <summary>
		/// <para>Date: 2012-10-11</para>
		/// <para>Added: BackgroundColorLinkage</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.LabelPlotStyle", 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version not allowed!");

				/*
				LabelPlotStyle s = (LabelPlotStyle)obj;
				info.AddValue("Font", s._font);
				info.AddValue("IndependentColor", s._independentColor);
				info.AddValue("Brush", s._brush);
				info.AddValue("XOffset", s._xOffset);
				info.AddValue("YOffset", s._yOffset);
				info.AddValue("Rotation", s._rotation);
				info.AddEnum("HorizontalAlignment", s.HorizontalAlignment);
				info.AddEnum("VerticalAlignment", s.VerticalAlignment);
				info.AddValue("AttachedAxis", s._attachedPlane);
				info.AddEnum("BackgroundColorLinkage", s._backgroundColorLinkage);
				info.AddValue("Background", s._backgroundStyle);
				info.AddValue("LabelColumn", s._labelColumnProxy);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent, true);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
			{
				LabelPlotStyle s = null != o ? (LabelPlotStyle)o : new LabelPlotStyle(info);

				s._font = (FontX)info.GetValue("Font", s);
				s._independentColor = info.GetBoolean("IndependentColor");
				s._brush = (BrushX)info.GetValue("Brush", s);
				s._offsetX_EmUnits = info.GetDouble("XOffset");
				s._offsetY_EmUnits = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.AlignmentX = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment)));
				s.AlignmentY = GdiExtensionMethods.ToAltaxo((System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment)));
				s.AttachedAxis = (CSPlaneID)info.GetValue("AttachedAxis", s);
				s._backgroundColorLinkage = (ColorLinkage)info.GetEnum("BackgroundColorLinkage", typeof(ColorLinkage));

				s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background", s);
				if (null != s._backgroundStyle) s._backgroundStyle.ParentObject = s;

				s.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)info.GetValue("LabelColumn", s);

				if (nativeCall)
				{
					// restore the cached values
					s.SetCachedValues();
				}

				return s;
			}
		}

		/// <summary>
		/// <para>Date: 2012-10-11</para>
		/// <para>Added: BackgroundColorLinkage</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelPlotStyle), 5)]
		private class XmlSerializationSurrogate5 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
				info.AddValue("IgnoreMissingDataPoints", s._ignoreMissingDataPoints);
				info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);
				info.AddValue("LabelFormat", s._labelFormatString);

				info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
				info.AddValue("SymbolSize", s._symbolSize);

				info.AddValue("FontSizeOffset", s._fontSizeOffset);
				info.AddValue("FontSizeFactor", s._fontSizeFactor);
				info.AddValue("Font", s._font);
				info.AddValue("Material", s._brush);
				info.AddValue("IndependentColor", s._independentColor);

				info.AddEnum("AlignmentX", s._alignmentX);
				info.AddEnum("AlignmentY", s._alignmentY);

				info.AddValue("Rotation", s._rotation);

				info.AddValue("OffsetXPoints", s._offsetX_Points);
				info.AddValue("OffsetXEm", s._offsetX_EmUnits);
				info.AddValue("OffsetXSymbolSize", s._offsetX_SymbolSizeUnits);
				info.AddValue("OffsetYPoints", s._offsetY_Points);
				info.AddValue("OffsetYEm", s._offsetY_EmUnits);
				info.AddValue("OffsetYSymbolSize", s._offsetY_SymbolSizeUnits);

				info.AddEnum("BackgroundColorLinkage", s._backgroundColorLinkage);
				info.AddValue("Background", s._backgroundStyle);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return SDeserialize(o, info, parent, true);
			}

			public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
			{
				var s = (LabelPlotStyle)o ?? new LabelPlotStyle(info);

				s.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)info.GetValue("LabelColumn", s);
				s._attachedPlane = (CSPlaneID)info.GetValue("AttachedPlane", s);
				s._independentSkipFrequency = info.GetBoolean("IndependentSkipFreq");
				s._skipFrequency = info.GetInt32("SkipFreq");
				s._ignoreMissingDataPoints = info.GetBoolean("IgnoreMissingDataPoints");
				s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");
				s._labelFormatString = info.GetString("LabelFormat");

				s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
				s._symbolSize = info.GetDouble("SymbolSize");

				s._fontSizeOffset = info.GetDouble("FontSizeOffset");
				s._fontSizeFactor = info.GetDouble("FontSizeFactor");

				s._font = (FontX)info.GetValue("Font", s);
				s._brush = (BrushX)info.GetValue("Material", s);
				s._independentColor = info.GetBoolean("IndependentColor");

				s._alignmentX = (Alignment)info.GetEnum("AlignmentX", typeof(Alignment));
				s._alignmentY = (Alignment)info.GetEnum("AlignmentY", typeof(Alignment));

				s._rotation = info.GetDouble("Rotation");

				s._offsetX_Points = info.GetDouble("OffsetXPoints");
				s._offsetX_EmUnits = info.GetDouble("OffsetXEm");
				s._offsetX_SymbolSizeUnits = info.GetDouble("OffsetXSymbolSize");

				s._offsetY_Points = info.GetDouble("OffsetYPoints");
				s._offsetY_EmUnits = info.GetDouble("OffsetYEm");
				s._offsetY_SymbolSizeUnits = info.GetDouble("OffsetYSymbolSize");

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

		public bool CopyFrom(object obj, bool copyWithDataReferences)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as LabelPlotStyle;
			if (null == from)
				return false;

			using (var suspendToken = SuspendGetToken())
			{
				this._attachedPlane = from._attachedPlane;
				this._independentSkipFrequency = from._independentSkipFrequency;
				this._skipFrequency = from._skipFrequency;
				this._ignoreMissingDataPoints = from._ignoreMissingDataPoints;
				this._independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;
				this._labelFormatString = from._labelFormatString;

				this._independentSymbolSize = from._independentSymbolSize;
				this._symbolSize = from._symbolSize;

				this._fontSizeOffset = from._fontSizeOffset;
				this._fontSizeFactor = from._fontSizeFactor;

				this._font = from._font;
				ChildCopyToMember(ref _brush, from._brush);
				this._independentColor = from._independentColor;

				this._alignmentX = from._alignmentX;
				this._alignmentY = from._alignmentY;

				this._rotation = from._rotation;

				this._offsetX_Points = from._offsetX_Points;
				this._offsetX_EmUnits = from._offsetX_EmUnits;
				this._offsetX_SymbolSizeUnits = from._offsetX_SymbolSizeUnits;

				this._offsetY_Points = from._offsetY_Points;
				this._offsetY_EmUnits = from._offsetY_EmUnits;
				this._offsetY_SymbolSizeUnits = from._offsetY_SymbolSizeUnits;

				this._backgroundColorLinkage = from._backgroundColorLinkage;
				ChildCopyToMember(ref _backgroundStyle, from._backgroundStyle);

				this._cachedLogicalShiftX = from._cachedLogicalShiftX;
				this._cachedLogicalShiftY = from._cachedLogicalShiftY;

				this._cachedStringFormat = (System.Drawing.StringFormat)from._cachedStringFormat.Clone();

				if (copyWithDataReferences)
					this.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)from._labelColumnProxy.Clone();

				EhSelfChanged(EventArgs.Empty);
				suspendToken.Resume();
			}
			return true;
		}

		/// <inheritdoc/>
		public bool CopyFrom(object obj)
		{
			return CopyFrom(obj, true);
		}

		/// <inheritdoc/>
		public object Clone(bool copyWithDataReferences)
		{
			return new LabelPlotStyle(this, copyWithDataReferences);
		}

		/// <inheritdoc/>
		public object Clone()
		{
			return new LabelPlotStyle(this, true);
		}

		/// <summary>
		/// For deserialization purposes.
		/// </summary>
		protected LabelPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			this._backgroundColorLinkage = ColorLinkage.Independent;

			this._cachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
		}

		public LabelPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
	: this((Altaxo.Data.IReadableColumn)null, context)
		{
		}

		public LabelPlotStyle(LabelPlotStyle from, bool copyWithDataReferences)
		{
			CopyFrom(from, copyWithDataReferences);
		}

		public LabelPlotStyle(Altaxo.Data.IReadableColumn labelColumn, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			this._font = GraphDocument.GetDefaultFont(context);
			this._fontSizeOffset = _font.Size;

			var color = GraphDocument.GetDefaultPlotColor(context);
			this._independentColor = false;
			this._brush = new BrushX(color) { ParentObject = this };
			this._backgroundColorLinkage = ColorLinkage.Independent;
			this.LabelColumnProxy = Altaxo.Data.ReadableColumnProxyBase.FromColumn(labelColumn);

			this._cachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _brush)
				yield return new Main.DocumentNodeAndName(_brush, "Brush");

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

		/// <summary>
		/// true if the symbol size is independent, i.e. is not published nor updated by a group style.
		/// </summary>
		public bool IndependentSymbolSize
		{
			get { return _independentSymbolSize; }
			set
			{
				if (!(_independentSymbolSize == value))
				{
					_independentSymbolSize = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Controls the length of the end bar.</summary>
		public double SymbolSize
		{
			get { return _symbolSize; }
			set
			{
				if (!Calc.RMath.IsFinite(value))
					throw new ArgumentException(nameof(value), "Value must be a finite number");

				if (!(_symbolSize == value))
				{
					_symbolSize = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>The font of the label.</summary>
		public FontX Font
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

		/// <summary>
		/// Offset used to calculate the font size in dependence on the symbol size., according to the formula:
		/// fontSize = <see cref="FontSizeOffset"/> + <see cref="FontSizeFactor"/> * <see cref="_symbolSize"/>;
		/// </summary>
		public double FontSizeOffset
		{
			get
			{
				return _fontSizeOffset;
			}
			set
			{
				if (!(_fontSizeOffset == value))
				{
					_fontSizeOffset = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>
		/// Offset used to calculate the font size in dependence on the symbol size., according to the formula:
		/// fontSize = <see cref="FontSizeOffset"/> + <see cref="FontSizeFactor"/> * <see cref="_symbolSize"/>;
		/// </summary>
		public double FontSizeFactor
		{
			get
			{
				return _fontSizeFactor;
			}
			set
			{
				if (!(_fontSizeFactor == value))
				{
					_fontSizeFactor = value;
					EhSelfChanged();
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
				if (!(_independentColor == value))
				{
					_independentColor = value;

					if (true == _independentColor)
						_cachedColorForIndexFunction = null;

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public BrushX LabelBrush
		{
			get
			{
				return _brush;
			}
			set
			{
				if (ChildSetMember(ref _brush, value))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>The background style.</summary>
		public Gdi.Background.IBackgroundStyle BackgroundStyle
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

		/// <summary>The x offset in points.</summary>
		/// Total offset is calculated according to:
		/// totalOffset = <see cref="OffsetXPoints"/> +  <see cref="OffsetXEmUnits"/> * emSize + <see cref="OffsetXSymbolSizeUnits"/> * symbolSize</summary>
		public double OffsetXPoints
		{
			get
			{
				return _offsetX_Points;
			}
			set
			{
				if (!(_offsetX_Points == value))
				{
					_offsetX_Points = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>The x offset relative to font size, i.e. a value of 1 is 1*FontSize.
		/// Total offset is calculated according to:
		/// totalOffset = <see cref="OffsetXPoints"/> +  <see cref="OffsetXEmUnits"/> * emSize + <see cref="OffsetXSymbolSizeUnits"/> * symbolSize</summary>
		public double OffsetXEmUnits
		{
			get { return this._offsetX_EmUnits; }
			set
			{
				double oldValue = this._offsetX_EmUnits;
				this._offsetX_EmUnits = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The x offset in symbol size units.</summary>
		/// Total offset is calculated according to:
		/// totalOffset = <see cref="OffsetXPoints"/> +  <see cref="OffsetXEmUnits"/> * emSize + <see cref="OffsetXSymbolSizeUnits"/> * symbolSize</summary>
		public double OffsetXSymbolSizeUnits
		{
			get
			{
				return _offsetX_SymbolSizeUnits;
			}
			set
			{
				if (!(_offsetX_SymbolSizeUnits == value))
				{
					_offsetX_SymbolSizeUnits = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>The y offset in points.</summary>
		/// Total offset is calculated according to:
		/// totalOffset = <see cref="OffsetYPoints"/> +  <see cref="OffsetYEmUnits"/> * emSize + <see cref="OffsetYSymbolSizeUnits"/> * symbolSize</summary>
		public double OffsetYPoints
		{
			get
			{
				return _offsetY_Points;
			}
			set
			{
				if (!(_offsetY_Points == value))
				{
					_offsetY_Points = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
		public double OffsetYEmUnits
		{
			get { return this._offsetY_EmUnits; }
			set
			{
				double oldValue = this._offsetY_EmUnits;
				this._offsetY_EmUnits = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>The y offset in symbol size units.</summary>
		/// Total offset is calculated according to:
		/// totalOffset = <see cref="OffsetYPoints"/> +  <see cref="OffsetYEmUnits"/> * emSize + <see cref="OffsetYSymbolSizeUnits"/> * symbolSize</summary>
		public double OffsetYSymbolSizeUnits
		{
			get
			{
				return _offsetY_SymbolSizeUnits;
			}
			set
			{
				if (!(_offsetY_SymbolSizeUnits == value))
				{
					_offsetY_SymbolSizeUnits = value;
					EhSelfChanged();
				}
			}
		}

		/// <summary>The angle of the label.</summary>
		public double Rotation
		{
			get { return this._rotation; }
			set
			{
				double oldValue = this._rotation;
				this._rotation = value;
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

		/// <summary>Gets/sets the axis this label is attached to. If set to null, the label is positioned normally.</summary>
		public CSPlaneID AttachedAxis
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
		/// Gets or sets a value indicating whether to ignore missing data points. If the value is set to true,
		/// the line is plotted even if there is a gap in the data points.
		/// </summary>
		/// <value>
		/// <c>true</c> if missing data points should be ignored; otherwise, if <c>false</c>, no line is plotted between a gap in the data.
		/// </value>
		public bool IgnoreMissingDataPoints
		{
			get
			{
				return _ignoreMissingDataPoints;
			}
			set
			{
				if (!(_ignoreMissingDataPoints == value))
				{
					_ignoreMissingDataPoints = value;
					EhSelfChanged(EventArgs.Empty);
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



		public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			// adjust the skip frequency if it was not set appropriate
			if (_skipFrequency <= 0)
				_skipFrequency = 1;

			if (_independentOnShiftingGroupStyles)
			{
				_cachedLogicalShiftX = _cachedLogicalShiftY = 0;
			}


			PlotRangeList rangeList = pdata.RangeList;



			if (this._ignoreMissingDataPoints)
			{
				// in case we ignore the missing points, all ranges can be plotted
				// as one range, i.e. continuously
				// for this, we create the totalRange, which contains all ranges
				var totalRange = new PlotRangeCompound(rangeList);
				this.PaintOneRange(g, layer, totalRange, pdata);
			}
			else // we not ignore missing points, so plot all ranges separately
			{
				for (int i = 0; i < rangeList.Count; i++)
				{
					this.PaintOneRange(g, layer, rangeList[i], pdata);
				}
			}
		}

		public void PaintOneRange(Graphics g, IPlotArea layer, IPlotRange range, Processed2DPlotData pdata)
		{
			if (this._labelColumnProxy.Document == null)
				return;

			_cachedStringFormat.Alignment = GdiExtensionMethods.ToGdi(_alignmentX);
			_cachedStringFormat.LineAlignment = GdiExtensionMethods.ToGdi(_alignmentY);

			if (null != _attachedPlane)
				_attachedPlane = layer.UpdateCSPlaneID(_attachedPlane);

			var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
			Altaxo.Data.IReadableColumn labelColumn = this._labelColumnProxy.Document;

			bool isUsingVariableColorForLabelText = null != _cachedColorForIndexFunction && IsColorReceiver;
			bool isUsingVariableColorForLabelBackground = null != _cachedColorForIndexFunction &&
				(null != _backgroundStyle && _backgroundStyle.SupportsBrush && (_backgroundColorLinkage == ColorLinkage.Dependent || _backgroundColorLinkage == ColorLinkage.PreserveAlpha));
			bool isUsingVariableColor = isUsingVariableColorForLabelText || isUsingVariableColorForLabelBackground;
			BrushX clonedTextBrush = null;
			BrushX clonedBackBrush = null;
			if (isUsingVariableColorForLabelText)
				clonedTextBrush = _brush.Clone();
			if (isUsingVariableColorForLabelBackground)
				clonedBackBrush = _backgroundStyle.Brush.Clone();

			// save the graphics stat since we have to translate the origin
			var gs = g.Save();

			double xpos = 0, ypos = 0;
			double xpre, ypre;
			double xdiff, ydiff;

			bool isFormatStringContainingBraces = _labelFormatString?.IndexOf('{') >= 0;
			var culture = System.Threading.Thread.CurrentThread.CurrentCulture;

			bool mustUseLogicalCoordinates = null != this._attachedPlane || 0 != _cachedLogicalShiftX || 0 != _cachedLogicalShiftY;

			int lower = range.LowerBound;
			int upper = range.UpperBound;
			for (int j = lower; j < upper; j+=_skipFrequency)
			{
				int originalRowIndex = range.GetOriginalRowIndexFromPlotPointIndex(j);
				string label;
				if (string.IsNullOrEmpty(_labelFormatString))
				{
					label = labelColumn[originalRowIndex].ToString();
				}
				else if (!isFormatStringContainingBraces)
				{
					label = labelColumn[originalRowIndex].ToString(_labelFormatString, culture);
				}
				else
				{
					// the label format string can contain {0} for the label column item, {1} for the row index, {2} .. {4} for the x, y and z component of the data point
					label = string.Format(_labelFormatString, labelColumn[originalRowIndex], originalRowIndex, pdata.GetPhysical(0, originalRowIndex), pdata.GetPhysical(1, originalRowIndex), pdata.GetPhysical(2, originalRowIndex));
				}

				if (string.IsNullOrEmpty(label))
					continue;

				double localSymbolSize = _symbolSize;
				if (null != _cachedSymbolSizeForIndexFunction)
				{
					localSymbolSize = _cachedSymbolSizeForIndexFunction(originalRowIndex);
				}

				double localFontSize = _fontSizeOffset + _fontSizeFactor * localSymbolSize;
				if (!(localFontSize > 0))
					continue;

				_font = _font.WithSize(localFontSize);

				// Start of preparation of brushes, if a variable color is used
				if (isUsingVariableColor)
				{
					Color c = _cachedColorForIndexFunction(originalRowIndex);

					if (isUsingVariableColorForLabelText)
					{
						clonedTextBrush.Color = new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B), "e");
					}
					if (isUsingVariableColorForLabelBackground)
					{
						if (_backgroundColorLinkage == ColorLinkage.PreserveAlpha)
							clonedBackBrush.Color = new NamedColor(AxoColor.FromArgb(clonedBackBrush.Color.Color.A, c.R, c.G, c.B), "e");
						else
							clonedBackBrush.Color = new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B), "e");
					}
				}
				// end of preparation of brushes for variable colors

				if (mustUseLogicalCoordinates) // we must use logical coordinates because either there is a shift of logical coordinates, or an attached plane
				{
					Logical3D r3d = layer.GetLogical3D(pdata, originalRowIndex);
					r3d.RX += _cachedLogicalShiftX;
					r3d.RY += _cachedLogicalShiftY;

					if (null != this._attachedPlane)
					{
						var pp = layer.CoordinateSystem.GetPointOnPlane(this._attachedPlane, r3d);
						xpre = pp.X;
						ypre = pp.Y;
					}
					else
					{
						PointD3D pt;
						layer.CoordinateSystem.LogicalToLayerCoordinates(r3d, out xpre, out ypre);
					}
				}
				else // no shifting, thus we can use layer coordinates
				{
					xpre = ptArray[j].X;
					ypre = ptArray[j].Y;
				}

				xdiff = xpre - xpos;
				ydiff = ypre - ypos;
				xpos = xpre;
				ypos = ypre;
				g.TranslateTransform((float)xdiff, (float)ydiff);
				if (this._rotation != 0)
					g.RotateTransform((float)-this._rotation);

				this.PaintOneItem(g, label, localSymbolSize, clonedTextBrush, clonedBackBrush);

				if (this._rotation != 0)
					g.RotateTransform((float)this._rotation);

			}

			g.Restore(gs); // Restore the graphics state
		}

		/// <summary>
		/// Paints one label.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="label"></param>
		/// <param name="variableTextBrush">If not null, this argument provides the text brush that should be used now. If null, then the <see cref="_brush"/> is used instead.</param>
		/// <param name="variableBackBrush"></param>
		public void PaintOneItem(Graphics g, string label, double symbolSize, BrushX variableTextBrush, BrushX variableBackBrush)
		{
			var fontSize = _font.Size;

			var xpos = _offsetX_Points + (_offsetX_EmUnits * fontSize) + (_offsetX_SymbolSizeUnits * symbolSize / 2);
			var ypos = -(_offsetY_Points + (_offsetY_EmUnits * fontSize) + (_offsetY_SymbolSizeUnits * symbolSize / 2));

			var gdiFont = GdiFontManager.ToGdi(_font);
			SizeF stringsize = g.MeasureString(label, gdiFont, PointF.Empty, _cachedStringFormat);

			if (this._backgroundStyle != null)
			{
				var x = xpos;
				var y = ypos;
				switch (_alignmentX)
				{
					case Alignment.Center:
						x -= stringsize.Width / 2;
						break;

					case Alignment.Far:
						x -= stringsize.Width;
						break;
				}
				switch (_alignmentY)
				{
					case Alignment.Center:
						y -= stringsize.Height / 2;
						break;

					case Alignment.Far:
						y -= stringsize.Height;
						break;
				}
				if (null == variableBackBrush)
				{
					this._backgroundStyle.Draw(g, new RectangleF((float)x, (float)y, stringsize.Width, stringsize.Height));
				}
				else
				{
					this._backgroundStyle.Draw(g, variableBackBrush, new RectangleF((float)x, (float)y, stringsize.Width, stringsize.Height));
				}
			}

			var brush = null != variableTextBrush ? variableTextBrush : _brush;
			brush.SetEnvironment(new RectangleF(new PointF((float)xpos, (float)ypos), stringsize), BrushX.GetEffectiveMaximumResolution(g, 1));
			g.DrawString(label, gdiFont, brush, (float)xpos, (float)ypos, _cachedStringFormat);
		}

		public RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
		{
			return bounds;
		}

		/// <summary>
		/// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
		/// </summary>
		/// <param name="layer">The parent layer.</param>
		public void PrepareScales(IPlotArea layer)
		{
		}

		#region I2DPlotStyle Members

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
					_backgroundStyle.SupportsBrush &&
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
					_backgroundStyle.SupportsBrush &&
					(_backgroundColorLinkage == ColorLinkage.Dependent || _backgroundColorLinkage == ColorLinkage.PreserveAlpha);
			}
		}

		#region IG2DPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);

			SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group style only)
			IgnoreMissingDataPointsGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.LabelBrush.Color; });
			else if (this.IsBackgroundColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this._backgroundStyle.Brush.Color; });

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			if (!_independentSkipFrequency)
				SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return _skipFrequency; });

			// IgnoreMissingDataPoints should be the same for all sub plot styles, so there is no "private" property
			IgnoreMissingDataPointsGroupStyle.PrepareStyle(externalGroups, localGroups, () => _ignoreMissingDataPoints);
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			// IgnoreMissingDataPoints is the same for all sub plot styles
			IgnoreMissingDataPointsGroupStyle.ApplyStyle(externalGroups, localGroups, (ignoreMissingDataPoints) => this._ignoreMissingDataPoints = ignoreMissingDataPoints);


			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			if (!_independentSkipFrequency)
			{
				_skipFrequency = 1;
				SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c) { this._skipFrequency = c; });
			}

			// Symbol size
			if (!_independentSymbolSize)
			{
				_symbolSize = 0;
				SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this._symbolSize = size; });
				// but if there is an symbol size evaluation function, then use this with higher priority.
				if (!VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc) { _cachedSymbolSizeForIndexFunction = evalFunc; }))
					_cachedSymbolSizeForIndexFunction = null;
			}

			// Color
			_cachedColorForIndexFunction = null;

			if (this.IsColorReceiver)
			{
				// try to get a constant color ...
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this.LabelBrush.Color = c; });
			}

			if (this.IsBackgroundColorReceiver)
			{
				if (this._backgroundColorLinkage == ColorLinkage.Dependent)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this._backgroundStyle.Brush.Color = c; });
				else if (this._backgroundColorLinkage == ColorLinkage.PreserveAlpha)
					ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this._backgroundStyle.Brush.Color = c.NewWithAlphaValue(_backgroundStyle.Brush.Color.Color.A); });
			}

			if (this.IsColorReceiver || this.IsBackgroundColorReceiver)
			{
				// but if there is a color evaluation function, then use that function with higher priority
				VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; });
			}

			// Shift the items ?
			_cachedLogicalShiftX = 0;
			_cachedLogicalShiftY = 0;
			if (!_independentOnShiftingGroupStyles)
			{
				var shiftStyle = PlotGroupStyle.GetFirstStyleToApplyImplementingInterface<IShiftLogicalXYGroupStyle>(externalGroups, localGroups);
				if (null != shiftStyle)
				{
					shiftStyle.Apply(out _cachedLogicalShiftX, out _cachedLogicalShiftY);
				}
			}
		}

		#endregion IG2DPlotStyle Members

		#endregion I2DPlotStyle Members

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