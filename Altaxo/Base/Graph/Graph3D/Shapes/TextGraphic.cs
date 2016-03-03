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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Altaxo.Graph.Graph3D.Shapes
{
	using Background;
	using Drawing;
	using Drawing.D3D;
	using GraphicsContext;
	using Plot;

	/// <summary>
	/// TextGraphics provides not only simple text on a graph,
	/// but also some formatting of the text, and quite important - the plot symbols
	/// to be used either in the legend or in the axis titles
	/// </summary>
	[Serializable]
	public partial class TextGraphic : GraphicBase, IRoutedPropertyReceiver
	{
		protected string _text = ""; // the text, which contains the formatting symbols
		protected FontX3D _font;
		protected IMaterial _textBrush = Materials.GetSolidMaterial(NamedColors.Black);
		protected IBackgroundStyle3D _background = null;
		protected double _lineSpacingFactor = 1.25f; // multiplicator for the line space, i.e. 1, 1.5 or 2

		#region Cached or temporary variables

		/// <summary>Hashtable where the keys are graphic paths giving the position of a symbol into the list, and the values are the plot items.</summary>
		protected Dictionary<RectangleTransformedD3D, IGPlotItem> _cachedSymbolPositions = new Dictionary<RectangleTransformedD3D, IGPlotItem>();

		private StructuralGlyph _rootNode;
		protected bool _isStructureInSync = false; // true when the text was interpretet and the structure created
		protected bool _isMeasureInSync = false; // true when all items are measured
		protected PointD3D _cachedTextOffset; // offset of text to left upper corner of outer rectangle
		protected RectangleD3D _cachedExtendedTextBounds; // the text bounds extended by some margin around it

		#endregion Cached or temporary variables

		#region Serialization

		/// <summary>
		/// 2015-09-11 initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextGraphic), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
				var s = (TextGraphic)o ?? new TextGraphic(info);

				info.GetBaseValueEmbedded(s, typeof(TextGraphic).BaseType, parent);

				s._text = info.GetString("Text");
				s._font = (FontX3D)info.GetValue("Font", s);
				s._textBrush = (IMaterial)info.GetValue("Brush", s);
				s.Background = (IBackgroundStyle3D)info.GetValue("BackgroundStyle", s);
				s._lineSpacingFactor = info.GetSingle("LineSpacing");

				s.UpdateTransformationMatrix();

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
			: base(info)
		{
		}

		public TextGraphic(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
			: base(new ItemLocationDirectAutoSize())
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			_font = GraphDocument.GetDefaultFont(context);
			_textBrush = Materials.GetSolidMaterial(GraphDocument.GetDefaultForeColor(context));
		}

		public TextGraphic(PointD3D graphicPosition, string text, FontX3D textFont, NamedColor textColor)
			: base(new ItemLocationDirectAutoSize())
		{
			this.SetPosition(graphicPosition, Main.EventFiring.Suppressed);
			this.Font = textFont;
			this.Text = text;
			this.Color = textColor;
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

					this._textBrush = from._textBrush;

					this._background = from._background == null ? null : (IBackgroundStyle3D)from._background.Clone();
					if (null != _background) _background.ParentObject = this;

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

		private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
		{
			if (null != _background)
				yield return new Main.DocumentNodeAndName(_background, "Background");
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
		}

		#region Background

		protected void MeasureBackground(IGraphicsContext3D g, double itemSizeX, double itemSizeY, double itemSizeZ)
		{
			var fontInfo = FontManager3D.Instance.GetFontInformation(_font);

			double widthOfOne_n = Glyph.MeasureString("n", _font).X;
			double widthOfThree_M = Glyph.MeasureString("MMM", _font).X;

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

			var size = new VectorD3D((itemSizeX + distanceXL + distanceXR), (itemSizeY + distanceYU + distanceYL), itemSizeZ);
			_cachedExtendedTextBounds = new RectangleD3D(PointD3D.Empty, size);
			var textRectangle = new RectangleD3D(new PointD3D(-distanceXL, -distanceYU, 0), size);

			if (this._background != null)
			{
				var backgroundRect = this._background.MeasureItem(textRectangle);
				_cachedExtendedTextBounds = _cachedExtendedTextBounds.WithOffset(textRectangle.X - backgroundRect.X, textRectangle.Y - backgroundRect.Y, 0);

				size = backgroundRect.Size;
				distanceXL = -backgroundRect.X;
				distanceXR = (backgroundRect.XPlusSizeX - itemSizeX);
				distanceYU = -backgroundRect.YPlusSizeY;
				distanceYL = (backgroundRect.Y - itemSizeY);
			}

			//var xanchor = _location.PivotX.GetValueRelativeTo(size.X);
			//var yanchor = _location.PivotY.GetValueRelativeTo(size.Y);

			// this._leftTop = new PointD2D(-xanchor, -yanchor);
			((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(size, false);

			this._cachedTextOffset = new PointD3D(distanceXL, distanceYU, 0);
		}

		public IBackgroundStyle3D Background
		{
			get
			{
				return _background;
			}
			set
			{
				if (object.ReferenceEquals(_background, value))
					return;

				if (ChildSetMember(ref _background, value))
				{
					_isMeasureInSync = false;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void PaintBackground(IGraphicsContext3D g)
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

		public FontX3D Font
		{
			get
			{
				return _font;
			}
			set
			{
				if (!object.ReferenceEquals(_font, value))
				{
					_font = value;
					this._isStructureInSync = false; // since the font is cached in the structure, it must be renewed
					this._isMeasureInSync = false;
					EhSelfChanged(EventArgs.Empty);
				}
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
				var oldText = _text;
				_text = value;

				if (oldText != _text)
				{
					this._isStructureInSync = false;
					EhSelfChanged(EventArgs.Empty);
				}
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
				var oldValue = _textBrush;
				_textBrush = Materials.GetMaterialWithNewColor(_textBrush, value);

				if (!object.ReferenceEquals(oldValue, _textBrush))
				{
					_isStructureInSync = false; // we must invalidate the structure, because the color is part of the structures temp storage
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public IMaterial TextFillBrush
		{
			get
			{
				return _textBrush;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				var oldValue = _textBrush;
				_textBrush = value;

				if (!object.ReferenceEquals(oldValue, _textBrush))
				{
					_isStructureInSync = false; // we must invalidate the structure, because the color is part of the structures temp storage
					EhSelfChanged(EventArgs.Empty);
				}
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

		private void MeasureGlyphs(IGraphicsContext3D g, FontCache cache, Altaxo.Graph.IPaintContext paintContext)
		{
			MeasureContext mc = new MeasureContext();
			mc.FontCache = cache;
			mc.LinkedObject = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(this);
			mc.TabStop = Glyph.MeasureString("MMMM", _font).X;

			if (null != _rootNode)
				_rootNode.Measure(mc, 0);
		}

		private void DrawGlyphs(IGraphicsContext3D g, DrawContext dc, double x, double y, double z)
		{
			_rootNode.Draw(g, dc, x, y + _rootNode.ExtendBelowBaseline, z);
		}

		public override void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext)
		{
			Paint(g, paintContext, false);
		}

		public void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, bool bForPreview)
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
					this.MeasureGlyphs(g, fontCache, paintContext);

					MeasureBackground(g, _rootNode.SizeX, _rootNode.SizeY, _rootNode.SizeZ);

					_isMeasureInSync = true;
				}

				_cachedSymbolPositions.Clear();

				var gs = g.SaveGraphicsState();

				var bounds = Bounds;

				var transformmatrix = Matrix4x3.NewScalingShearingRotationDegreesTranslation(
				ScaleX, ScaleY, ScaleZ,
					ShearX, ShearY, ShearZ,
					RotationX, RotationY, RotationZ,
					_location.AbsolutePivotPositionX, _location.AbsolutePivotPositionY, _location.AbsolutePivotPositionZ);
				transformmatrix.TranslatePrepend(bounds.X, bounds.Y, bounds.Z);

				if (!bForPreview)
				{
					TransformGraphics(g);
					g.TranslateTransform(bounds.X, bounds.Y, bounds.Z);
				}

				// first of all paint the background
				PaintBackground(g);

				DrawContext dc = new DrawContext();
				dc.FontCache = fontCache;
				dc.bForPreview = bForPreview;
				dc.LinkedObject = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(this);
				dc.transformMatrix = transformmatrix;
				dc._cachedSymbolPositions = _cachedSymbolPositions;
				DrawGlyphs(g, dc, _cachedTextOffset.X, _cachedTextOffset.Y, _cachedTextOffset.Z);
				g.RestoreGraphicsState(gs);
			}
		}

		#endregion Interpreting and Painting

		#region Hit testing and handling

		public static DoubleClickHandler PlotItemEditorMethod;
		public static DoubleClickHandler TextGraphicsEditorMethod;

		public override IHitTestObject HitTest(HitTestPointData parentHitData)
		{
			//			HitTestPointData layerHitTestData = pageC.NewFromTranslationRotationScaleShear(Position.X, Position.Y, -Rotation, ScaleX, ScaleY, ShearX);
			var localHitData = parentHitData.NewFromAdditionalTransformation(this._transformation);

			double z;
			if (localHitData.IsHit(Bounds, out z))
			{
				var result = GetNewHitTestObject(parentHitData.WorldTransformation);
				result.DoubleClick = TextGraphicsEditorMethod;
				return result;
			}
			else
			{
				return null;
			}
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
						this.Font = _font.WithSize(prop.Value);
						EhSelfChanged(EventArgs.Empty);
					}
					break;

				case "FontFamily":
					{
						var prop = (RoutedSetterProperty<string>)property;
						try
						{
							var newFont = _font.WithFamily(prop.Value);
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
					((RoutedGetterProperty<string>)property).Merge(this.Font.Font.FontFamilyName);
					break;
			}
		}

		#endregion IRoutedPropertyReceiver Members
	}
}