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
		/// <summary>The font of the label.</summary>
		protected FontX _font;

		/// <summary>
		/// True if the color of the label is not dependent on the color of the parent plot style.
		/// </summary>
		protected bool _independentColor;

		/// <summary>The brush for the label.</summary>
		protected BrushX _brush;

		/// <summary>The x offset in EM units.</summary>
		protected double _xOffset;

		/// <summary>The y offset in EM units.</summary>
		protected double _yOffset;

		/// <summary>The rotation of the label.</summary>
		protected double _rotation;

		/// <summary>The style for the background.</summary>
		protected Gdi.Background.IBackgroundStyle _backgroundStyle;

		protected ColorLinkage _backgroundColorLinkage;

		/// <summary>The axis where the label is attached to (if it is attached).</summary>
		protected CSPlaneID _attachedPlane;

		protected Altaxo.Data.IReadableColumnProxy _labelColumnProxy;

		// cached values:
		[NonSerialized]
		protected System.Drawing.StringFormat _cachedStringFormat;

		/// <summary>If this function is set, the label color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

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
				s._xOffset = info.GetDouble("XOffset");
				s._yOffset = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment));
				s.VerticalAlignment = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment));
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
				s._xOffset = info.GetDouble("XOffset");
				s._yOffset = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment));
				s.VerticalAlignment = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment));
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
				s._xOffset = info.GetDouble("XOffset");
				s._yOffset = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment));
				s.VerticalAlignment = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment));
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
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelPlotStyle), 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SSerialize(obj, info);
			}

			public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
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
				s._xOffset = info.GetDouble("XOffset");
				s._yOffset = info.GetDouble("YOffset");
				s._rotation = info.GetDouble("Rotation");
				s.HorizontalAlignment = (System.Drawing.StringAlignment)info.GetEnum("HorizontalAlignment", typeof(System.Drawing.StringAlignment));
				s.VerticalAlignment = (System.Drawing.StringAlignment)info.GetEnum("VerticalAlignment", typeof(System.Drawing.StringAlignment));
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
				this._font = from._font;
				this._independentColor = from._independentColor;
				ChildCopyToMember(ref _brush, from._brush);
				this._xOffset = from._xOffset;
				this._yOffset = from._yOffset;
				this._rotation = from._rotation;
				ChildCopyToMember(ref _backgroundStyle, from._backgroundStyle);
				this._backgroundColorLinkage = from._backgroundColorLinkage;
				this._cachedStringFormat = (System.Drawing.StringFormat)from._cachedStringFormat.Clone();
				this._attachedPlane = from._attachedPlane;

				if (copyWithDataReferences)
				{
					this.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)from._labelColumnProxy.Clone();
				}

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
			this._cachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
			this._cachedStringFormat.Alignment = System.Drawing.StringAlignment.Center;
			this._cachedStringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
			this._backgroundColorLinkage = ColorLinkage.Independent;
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
			var color = GraphDocument.GetDefaultPlotColor(context);

			this._independentColor = false;
			this._brush = new BrushX(color) { ParentObject = this };
			this._xOffset = 0;
			this._yOffset = 0;
			this._rotation = 0;
			this._backgroundStyle = null;
			this._backgroundColorLinkage = ColorLinkage.Independent;
			this._cachedStringFormat = new StringFormat(StringFormatFlags.NoWrap);
			this._cachedStringFormat.Alignment = System.Drawing.StringAlignment.Center;
			this._cachedStringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
			this._attachedPlane = null;
			this.LabelColumnProxy = Altaxo.Data.ReadableColumnProxyBase.FromColumn(labelColumn);
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
					_font = _font.GetFontWithNewSize(newValue);
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
		public System.Drawing.StringAlignment HorizontalAlignment
		{
			get
			{
				return this._cachedStringFormat.Alignment;
			}
			set
			{
				System.Drawing.StringAlignment oldValue = this.HorizontalAlignment;
				this._cachedStringFormat.Alignment = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>Vertical aligment of the label.</summary>
		public System.Drawing.StringAlignment VerticalAlignment
		{
			get { return this._cachedStringFormat.LineAlignment; }
			set
			{
				System.Drawing.StringAlignment oldValue = this.VerticalAlignment;
				this._cachedStringFormat.LineAlignment = value;
				if (value != oldValue)
				{
					EhSelfChanged(EventArgs.Empty);
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

		protected void SetCachedValues()
		{
		}

		/// <summary>
		/// Paints one label.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="label"></param>
		/// <param name="variableTextBrush">If not null, this argument provides the text brush that should be used now. If null, then the <see cref="_brush"/> is used instead.</param>
		/// <param name="variableBackBrush"></param>
		public void Paint(Graphics g, string label, BrushX variableTextBrush, BrushX variableBackBrush)
		{
			var fontSize = this.FontSize;
			float xpos = (float)(_xOffset * fontSize);
			float ypos = (float)(-_yOffset * fontSize);
			var gdiFont = GdiFontManager.ToGdi(_font);
			SizeF stringsize = g.MeasureString(label, gdiFont, new PointF(xpos, ypos), _cachedStringFormat);

			if (this._backgroundStyle != null)
			{
				float x = xpos, y = ypos;
				switch (_cachedStringFormat.Alignment)
				{
					case StringAlignment.Center:
						x -= stringsize.Width / 2;
						break;

					case StringAlignment.Far:
						x -= stringsize.Width;
						break;
				}
				switch (_cachedStringFormat.LineAlignment)
				{
					case StringAlignment.Center:
						y -= stringsize.Height / 2;
						break;

					case StringAlignment.Far:
						y -= stringsize.Height;
						break;
				}
				if (null == variableBackBrush)
				{
					this._backgroundStyle.Draw(g, new RectangleF(x, y, stringsize.Width, stringsize.Height));
				}
				else
				{
					this._backgroundStyle.Draw(g, variableBackBrush, new RectangleF(x, y, stringsize.Width, stringsize.Height));
				}
			}

			var brush = null != variableTextBrush ? variableTextBrush : _brush;
			brush.SetEnvironment(new RectangleF(new PointF(xpos, ypos), stringsize), BrushX.GetEffectiveMaximumResolution(g, 1));
			g.DrawString(label, gdiFont, brush, xpos, ypos, _cachedStringFormat);
		}

		public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData prevItemData, Processed2DPlotData nextItemData)
		{
			if (this._labelColumnProxy.Document == null)
				return;

			if (null != _attachedPlane)
				_attachedPlane = layer.UpdateCSPlaneID(_attachedPlane);

			PlotRangeList rangeList = pdata.RangeList;
			PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
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
			System.Drawing.Drawing2D.GraphicsState gs = g.Save();
			/*
			double bottomPosition = 0;
			double topPosition = 0;
			double leftPosition = 0;
			double rightPosition = 0;

			layer.CoordinateSystem.LogicalToLayerCoordinates(0, 0, out leftPosition, out bottomPosition);
			layer.CoordinateSystem.LogicalToLayerCoordinates(1, 1, out rightPosition, out topPosition);
 */
			double xpos = 0, ypos = 0;
			double xpre, ypre;
			double xdiff, ydiff;
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
						Color c = _cachedColorForIndexFunction(j + offset);

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

					xpre = ptArray[j].X;
					ypre = ptArray[j].Y;

					if (null != this._attachedPlane)
					{
						Logical3D r3d = layer.GetLogical3D(pdata, j + offset);
						var pp = layer.CoordinateSystem.GetPointOnPlane(this._attachedPlane, r3d);
						xpre = pp.X;
						ypre = pp.Y;
					}

					xdiff = xpre - xpos;
					ydiff = ypre - ypos;
					xpos = xpre;
					ypos = ypre;
					g.TranslateTransform((float)xdiff, (float)ydiff);
					if (this._rotation != 0)
						g.RotateTransform((float)-this._rotation);

					this.Paint(g, label, clonedTextBrush, clonedBackBrush);

					if (this._rotation != 0)
						g.RotateTransform((float)this._rotation);
				} // end for
			}

			g.Restore(gs); // Restore the graphics state
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
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.LabelBrush.Color; });
			else if (this.IsBackgroundColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this._backgroundStyle.Brush.Color; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
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

		#endregion IDocumentNode Members
	}
}