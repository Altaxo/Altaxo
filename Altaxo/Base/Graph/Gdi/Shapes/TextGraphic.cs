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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace Altaxo.Graph.Gdi.Shapes
{
	using Background;
	using Graph.Plot.Data;
	using Plot;
	using Plot.Data;

	/// <summary>
	/// TextGraphics provides not only simple text on a graph,
	/// but also some formatting of the text, and quite important - the plot symbols
	/// to be used either in the legend or in the axis titles
	/// </summary>
	[Serializable]
	public partial class TextGraphic : GraphicBase, IRoutedPropertyReceiver
	{
		protected string _text = ""; // the text, which contains the formatting symbols
		protected FontX _font;
		protected BrushX _textBrush = new BrushX(NamedColors.Black);
		protected IBackgroundStyle _background = null;
		protected double _lineSpacingFactor = 1.25f; // multiplicator for the line space, i.e. 1, 1.5 or 2

		#region Cached or temporary variables

		/// <summary>Hashtable where the keys are graphic paths giving the position of a symbol into the list, and the values are the plot items.</summary>
		protected Dictionary<GraphicsPath, IGPlotItem> _cachedSymbolPositions = new Dictionary<GraphicsPath, IGPlotItem>();

		private StructuralGlyph _rootNode;
		protected bool _isStructureInSync = false; // true when the text was interpretet and the structure created
		protected bool _isMeasureInSync = false; // true when all items are measured
		protected PointD2D _cachedTextOffset; // offset of text to left upper corner of outer rectangle
		protected RectangleD _cachedExtendedTextBounds; // the text bounds extended by some margin around it

		#endregion Cached or temporary variables

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.TextGraphics", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new ApplicationException("This serializer is not the actual version, and should therefore not be called");
				/*
				TextGraphics s = (TextGraphics)obj;
				info.AddBaseValueEmbedded(s,typeof(TextGraphics).BaseType);

				info.AddValue("Text",s.m_Text);
				info.AddValue("Font",s.m_Font);
				info.AddValue("Brush",s.m_BrushHolder);
				info.AddValue("BackgroundStyle",s.m_BackgroundStyle);
				info.AddValue("LineSpacing",s.m_LineSpacingFactor);
				info.AddValue("ShadowLength",s.m_ShadowLength);
				info.AddValue("XAnchor",s.m_XAnchorType);
				info.AddValue("YAnchor",s.m_YAnchorType);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				TextGraphic s = null != o ? (TextGraphic)o : new TextGraphic(info);
				info.GetBaseValueEmbedded(s, "AltaxoBase,Altaxo.Graph.GraphicsObject,0", parent);

				// we have changed the meaning of rotation in the meantime, This is not handled in GetBaseValueEmbedded,
				// since the former versions did not store the version number of embedded bases
				//s._rotation = -s._rotation;

				s._text = info.GetString("Text");
				s._font = (FontX)info.GetValue("Font", typeof(FontX));
				s._textBrush = (BrushX)info.GetValue("Brush", typeof(BrushX));
				s.BackgroundStyleOld = (BackgroundStyle)info.GetValue("BackgroundStyle", typeof(BackgroundStyle));
				s._lineSpacingFactor = info.GetSingle("LineSpacing");
				info.GetSingle("ShadowLength");
				var xAnchorType = (XAnchorPositionType)info.GetValue("XAnchor", typeof(XAnchorPositionType));
				var yAnchorType = (YAnchorPositionType)info.GetValue("YAnchor", typeof(YAnchorPositionType));
				s._location.LocalAnchorX = RADouble.NewRel(0.5 * (int)xAnchorType);
				s._location.LocalAnchorY = RADouble.NewRel(0.5 * (int)yAnchorType);

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.TextGraphics", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.TextGraphic", 2)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old type");
				/*
				TextGraphic s = (TextGraphic)obj;
				info.AddBaseValueEmbedded(s, typeof(TextGraphic).BaseType);

				info.AddValue("Text", s._text);
				info.AddValue("Font", s._font);
				info.AddValue("Brush", s._textBrush);
				info.AddValue("BackgroundStyle", s._background);
				info.AddValue("LineSpacing", s._lineSpacingFactor);
				info.AddValue("XAnchor", s._xAnchorType);
				info.AddValue("YAnchor", s._yAnchorType);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				TextGraphic s = null != o ? (TextGraphic)o : new TextGraphic(info);
				if (info.CurrentElementName == "BaseType") // that was included since 2006-06-20
				{
					info.GetBaseValueEmbedded(s, typeof(TextGraphic).BaseType, parent);
				}
				else
				{
					info.GetBaseValueEmbedded(s, "AltaxoBase,Altaxo.Graph.GraphicsObject,0", parent); // before 2006-06-20, it was version 0 of the GraphicsObject
				}

				s._text = info.GetString("Text");
				s._font = (FontX)info.GetValue("Font", typeof(FontX));
				s._textBrush = (BrushX)info.GetValue("Brush", typeof(BrushX));
				s._background = (IBackgroundStyle)info.GetValue("BackgroundStyle", typeof(IBackgroundStyle));
				s._lineSpacingFactor = info.GetSingle("LineSpacing");
				var xAnchorType = (XAnchorPositionType)info.GetValue("XAnchor", typeof(XAnchorPositionType));
				var yAnchorType = (YAnchorPositionType)info.GetValue("YAnchor", typeof(YAnchorPositionType));
				s._location.LocalAnchorX = RADouble.NewRel(0.5 * (int)xAnchorType);
				s._location.LocalAnchorY = RADouble.NewRel(0.5 * (int)yAnchorType);
				return s;
			}
		}

		/// <summary>
		/// 2013-10-15 XAnchor and YAnchor now are superfluous and thus are removed
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextGraphic), 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (TextGraphic)obj;
				info.AddBaseValueEmbedded(s, typeof(TextGraphic).BaseType);

				info.AddValue("Text", s._text);
				info.AddValue("Font", s._font);
				info.AddValue("Brush", s._textBrush);
				info.AddValue("BackgroundStyle", s._background);
				info.AddValue("LineSpacing", s._lineSpacingFactor);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (TextGraphic)o : new TextGraphic(info);

				info.GetBaseValueEmbedded(s, typeof(TextGraphic).BaseType, parent);

				s._text = info.GetString("Text");
				s._font = (FontX)info.GetValue("Font", typeof(FontX));
				s._textBrush = (BrushX)info.GetValue("Brush", typeof(BrushX));
				s._textBrush.ParentObject = s;

				s.Background = (IBackgroundStyle)info.GetValue("BackgroundStyle", typeof(IBackgroundStyle));

				s._lineSpacingFactor = info.GetSingle("LineSpacing");
				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TextGraphic"/> class for deserialization purposes.
		/// </summary>
		/// <param name="info">The information.</param>
		protected TextGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
			: base(new ItemLocationDirectAutoSize())
		{
		}

		public TextGraphic(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: base(new ItemLocationDirectAutoSize())
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			_font = context.GetValue(GraphDocument.PropertyKeyDefaultFont);
			_textBrush = new BrushX(context.GetValue(GraphDocument.PropertyKeyDefaultForeColor));
		}

		public TextGraphic(PointD2D graphicPosition, string text,
			FontX textFont, NamedColor textColor)
			: base(new ItemLocationDirectAutoSize())
		{
			this.SetPosition(graphicPosition, Main.EventFiring.Suppressed);
			this.Font = textFont;
			this.Text = text;
			this.Color = textColor;
		}

		public TextGraphic(double posX, double posY,
			string text, FontX textFont, NamedColor textColor)
			: this(new PointD2D(posX, posY), text, textFont, textColor)
		{
		}

		public TextGraphic(PointD2D graphicPosition,
			string text, FontX textFont,
			NamedColor textColor, double Rotation)
			: this(graphicPosition, text, textFont, textColor)
		{
			this.Rotation = Rotation;
		}

		public TextGraphic(double posX, double posY,
			string text,
			FontX textFont,
			NamedColor textColor, double Rotation)
			: this(new PointD2D(posX, posY), text, textFont, textColor, Rotation)
		{
		}

		public TextGraphic(TextGraphic from)
			: base(from) // all is done here, since CopyFrom is virtual!
		{
		}

		#endregion Constructors

		#region Copying

		public override bool CopyFrom(object obj)
		{
			var isCopied = base.CopyFrom(obj);
			if (isCopied && !object.ReferenceEquals(this, obj))
			{
				var from = obj as TextGraphic;
				if (from != null)
				{
					this._text = from._text;
					this._font = from._font;
					this._textBrush = from._textBrush == null ? null : (BrushX)from._textBrush.Clone();
					this._background = from._background == null ? null : (IBackgroundStyle)from._background.Clone();
					this._lineSpacingFactor = from._lineSpacingFactor;

					// don't clone the cached items
					this._isStructureInSync = false;
					this._isMeasureInSync = false;
				}
			}
			return isCopied;
		}

		public void CopyFrom(TextGraphic from)
		{
			CopyFrom((GraphicBase)from);
		}

		public override object Clone()
		{
			return new TextGraphic(this);
		}

		#endregion Copying

		#region Background

		protected void MeasureBackground(Graphics g, double textWidth, double textHeight)
		{
			var fontInfo = FontInfo.Create(g, _font);

			double widthOfOne_n = Glyph.MeasureString(g, "n", _font).X;
			double widthOfThree_M = Glyph.MeasureString(g, "MMM", _font).X;

			double distanceXL = 0; // left distance bounds-text
			double distanceXR = 0; // right distance text-bounds
			double distanceYU = 0;   // upper y distance bounding rectangle-string
			double distanceYL = 0; // lower y distance

			if (this._background != null)
			{
				// the distance to the sides should be like the character n
				distanceXL = 0.25 * widthOfOne_n; // left distance bounds-text
				distanceXR = distanceXL; // right distance text-bounds
				distanceYU = fontInfo.cyDescent;   // upper y distance bounding rectangle-string
				distanceYL = 0; // lower y distance
			}

			PointD2D size = new PointD2D((textWidth + distanceXL + distanceXR), (textHeight + distanceYU + distanceYL));
			_cachedExtendedTextBounds = new RectangleD(PointD2D.Empty, size);
			RectangleD textRectangle = new RectangleD(new PointD2D(-distanceXL, -distanceYU), size);

			if (this._background != null)
			{
				var backgroundRect = this._background.MeasureItem(g, textRectangle);
				_cachedExtendedTextBounds.Offset(textRectangle.X - backgroundRect.X, textRectangle.Y - backgroundRect.Y);

				size = backgroundRect.Size;
				distanceXL = -backgroundRect.Left;
				distanceXR = (backgroundRect.Right - textWidth);
				distanceYU = -backgroundRect.Top;
				distanceYL = (backgroundRect.Bottom - textHeight);
			}

			//var xanchor = _location.PivotX.GetValueRelativeTo(size.X);
			//var yanchor = _location.PivotY.GetValueRelativeTo(size.Y);

			// this._leftTop = new PointD2D(-xanchor, -yanchor);
			((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(size, true);

			this._cachedTextOffset = new PointD2D(distanceXL, distanceYU);
		}

		public IBackgroundStyle Background
		{
			get
			{
				return _background;
			}
			set
			{
				if (object.ReferenceEquals(_background, value))
					return;

				if (null != _background)
					_background.ParentObject = null;

				_background = value;

				if (null != _background)
					_background.ParentObject = this;

				_isMeasureInSync = false;
				EhSelfChanged(EventArgs.Empty);
			}
		}

		private BackgroundStyle BackgroundStyleOld
		{
			get
			{
				if (null == _background)
					return BackgroundStyle.None;
				else if (_background is BlackLine)
					return BackgroundStyle.BlackLine;
				else if (_background is BlackOut)
					return BackgroundStyle.BlackOut;
				else if (_background is DarkMarbel)
					return BackgroundStyle.DarkMarbel;
				else if (_background is RectangleWithShadow)
					return BackgroundStyle.Shadow;
				else if (_background is WhiteOut)
					return BackgroundStyle.WhiteOut;
				else
					return BackgroundStyle.None;
			}
			set
			{
				_isMeasureInSync = false;

				switch (value)
				{
					case BackgroundStyle.BlackLine:
						_background = new BlackLine();
						break;

					case BackgroundStyle.BlackOut:
						_background = new BlackOut();
						break;

					case BackgroundStyle.DarkMarbel:
						_background = new DarkMarbel();
						break;

					case BackgroundStyle.WhiteOut:
						_background = new WhiteOut();
						break;

					case BackgroundStyle.Shadow:
						_background = new RectangleWithShadow();
						break;

					case BackgroundStyle.None:
						_background = null;
						break;
				}
			}
		}

		protected virtual void PaintBackground(Graphics g)
		{
			// Assumptions:
			// 1. the overall size of the structure must be measured before, i.e. bMeasureInSync is true
			// 2. the graphics object was translated and rotated before, so that the paining starts at (0,0)

			if (!this._isMeasureInSync)
				return;

			if (_background != null)
				_background.Draw(g, _cachedExtendedTextBounds);
		}

		#endregion Background

		#region Properties

		public override string ToString()
		{
			return string.Format("TextGraphics Text: <<{0}>>", _text);
		}

		public override bool AutoSize
		{
			get
			{
				return true;
			}
		}

		public FontX Font
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
				this._isStructureInSync = false; // since the font is cached in the structure, it must be renewed
				this._isMeasureInSync = false;
			}
		}

		public bool Empty
		{
			get { return _text == null || _text.Length == 0; }
		}

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				this._isStructureInSync = false;
			}
		}

		public NamedColor Color
		{
			get
			{
				return _textBrush.Color;
			}
			set
			{
				_textBrush = new BrushX(value) { ParentObject = this };
				_isStructureInSync = false; // we must invalidate the structure, because the color is part of the structures temp storage
				EhSelfChanged(EventArgs.Empty);
			}
		}

		public BrushX TextFillBrush
		{
			get
			{
				return _textBrush;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();

				_textBrush = value.Clone();
				_textBrush.ParentObject = this;
				_isStructureInSync = false; // we must invalidate the structure, because the color is part of the structures temp storage
				EhSelfChanged(EventArgs.Empty);
			}
		}

		public double LineSpacing
		{
			get
			{
				return _lineSpacingFactor;
			}
			set
			{
				if (double.IsNaN(value) || double.IsInfinity(value))
					throw new ArgumentException("LineSpacing is NaN or Infinity");
				if (value < 0)
					throw new ArgumentOutOfRangeException("LineSpacing should be a non-negative value, but is: " + value.ToString());

				var oldValue = _lineSpacingFactor;
				_lineSpacingFactor = value;

				if (value != oldValue)
				{
					_isStructureInSync = false; // TODO: LineSpacing should not affect the structure, but only the measurement
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		#endregion Properties

		#region Interpreting and Painting

		private void InterpretText()
		{
			var parser = new Altaxo_LabelV1();
			parser.SetSource(_text);
			bool bMatches = parser.MainSentence();
			var tree = parser.GetRoot();

			TreeWalker walker = new TreeWalker(_text);
			StyleContext style = new StyleContext(_font, _textBrush);
			style.BaseFontId = _font;

			_rootNode = walker.VisitTree(tree, style, _lineSpacingFactor, true);
		}

		private void MeasureGlyphs(Graphics g, FontCache cache, object linkedObject)
		{
			MeasureContext mc = new MeasureContext();
			mc.FontCache = cache;
			mc.LinkedObject = linkedObject;
			mc.TabStop = Glyph.MeasureString(g, "MMMM", _font).X;

			if (null != _rootNode)
				_rootNode.Measure(g, mc, 0);
		}

		private void DrawGlyphs(Graphics g, DrawContext dc, double x, double y)
		{
			_rootNode.Draw(g, dc, x, y + _rootNode.ExtendAboveBaseline);
		}

		/// <summary>
		/// Get the object outline for arrangements in object world coordinates.
		/// </summary>
		/// <returns>Object outline for arrangements in object world coordinates</returns>
		public override GraphicsPath GetObjectOutlineForArrangements()
		{
			return GetRectangularObjectOutline();
		}

		public override void Paint(Graphics g, object obj)
		{
			Paint(g, obj, false);
		}

		public void Paint(Graphics g, object obj, bool bForPreview)
		{
			//_isStructureInSync = false;
			_isMeasureInSync = false;  // Change: interpret text every time in order to update plot items and \ID

			if (!this._isStructureInSync)
			{
				// this.Interpret(g);
				this.InterpretText();

				_isStructureInSync = true;
				_isMeasureInSync = false;
			}

			using (FontCache fontCache = new FontCache())
			{
				if (!this._isMeasureInSync)
				{
					// this.MeasureStructure(g, obj);
					this.MeasureGlyphs(g, fontCache, obj);

					MeasureBackground(g, _rootNode.Width, _rootNode.Height);

					_isMeasureInSync = true;
				}

				_cachedSymbolPositions.Clear();

				System.Drawing.Drawing2D.GraphicsState gs = g.Save();
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

				var bounds = Bounds;

				Matrix transformmatrix = new Matrix();
				transformmatrix.Translate((float)_location.AbsolutePivotPositionX, (float)_location.AbsolutePivotPositionY);
				transformmatrix.Rotate((float)(-Rotation));
				transformmatrix.Shear((float)Shear, 0);
				transformmatrix.Scale((float)ScaleX, (float)ScaleY);
				transformmatrix.Translate((float)bounds.X, (float)bounds.Y);

				if (!bForPreview)
				{
					TransformGraphics(g);
					g.TranslateTransform((float)bounds.X, (float)bounds.Y);
				}

				// first of all paint the background
				PaintBackground(g);

				DrawContext dc = new DrawContext();
				dc.FontCache = fontCache;
				dc.bForPreview = bForPreview;
				dc.LinkedObject = obj;
				dc.transformMatrix = transformmatrix;
				dc._cachedSymbolPositions = _cachedSymbolPositions;
				DrawGlyphs(g, dc, _cachedTextOffset.X, _cachedTextOffset.Y);
				g.Restore(gs);
			}
		}

		#endregion Interpreting and Painting

		#region Hit testing and handling

		public static DoubleClickHandler PlotItemEditorMethod;
		public static DoubleClickHandler TextGraphicsEditorMethod;

		public override IHitTestObject HitTest(HitTestPointData htd)
		{
			IHitTestObject result;

			var pt = htd.GetHittedPointInWorldCoord(_transformation);

			foreach (GraphicsPath gp in this._cachedSymbolPositions.Keys)
			{
				if (gp.IsVisible((PointF)pt))
				{
					result = new HitTestObject(gp, _cachedSymbolPositions[gp]);
					result.DoubleClick = PlotItemEditorMethod;
					return result;
				}
			}

			result = base.HitTest(htd);
			if (null != result)
				result.DoubleClick = TextGraphicsEditorMethod;
			return result;
		}

		#endregion Hit testing and handling

		#region Deprecated classes

		[Serializable]
		private enum BackgroundStyle
		{
			None,
			BlackLine,
			Shadow,
			DarkMarbel,
			WhiteOut,
			BlackOut
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyle", 0)]
		public class BackgroundStyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new NotImplementedException("This class is deprecated and no longer supported to serialize");
				// info.SetNodeContent(obj.ToString());
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string val = info.GetNodeContent();
				return System.Enum.Parse(typeof(BackgroundStyle), val, true);
			}
		}

		#endregion Deprecated classes

		#region IRoutedPropertyReceiver Members

		public void SetRoutedProperty(IRoutedSetterProperty property)
		{
			switch (property.Name)
			{
				case "FontSize":
					{
						var prop = (RoutedSetterProperty<double>)property;
						this.Font = _font.GetFontWithNewSize(prop.Value);
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
							_isStructureInSync = false;
							EhSelfChanged(EventArgs.Empty);
						}
						catch (Exception)
						{
						}
					}
					break;
			}
		}

		public void GetRoutedProperty(IRoutedGetterProperty property)
		{
			switch (property.Name)
			{
				case "FontSize":
					((RoutedGetterProperty<double>)property).Merge(this.Font.Size);
					break;

				case "FontFamily":
					((RoutedGetterProperty<string>)property).Merge(this.Font.FontFamilyName);
					break;
			}
		}

		#endregion IRoutedPropertyReceiver Members
	}
}