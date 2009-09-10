using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
	using Plot;
	using Plot.Data;
	using Graph.Plot.Data;
	using Background;

	public partial class TextGraphic : GraphicBase
	{
		/// <summary>
		/// FontIdentifier is an immutable object used to identify a font.
		/// </summary>
		public class FontIdentifier
		{
			public string FamilyName { get; private set; }
			public FontStyle Style { get; private set; }
			public double Size { get; private set; }

			public FontIdentifier(string name, FontStyle style, double size)
			{
				FamilyName = name;
				Style = style;
				Size = size;
			}

			public override bool Equals(object obj)
			{
				var from = obj as FontIdentifier;
				if (from == null)
					return false;
				else
					return this.Size == from.Size && this.Style == from.Style && this.FamilyName == from.FamilyName;
			}

			public override int GetHashCode()
			{
				return Size.GetHashCode() + Style.GetHashCode() + FamilyName.GetHashCode();
			}
		}

		/// <summary>
		/// Holds Information about the metrics of a font.
		/// </summary>
		class FontInfo : FontIdentifier
		{

			public double cyLineSpace { get; private set; } // cached linespace value of the font
			public double cyAscent { get; private set; }    // cached ascent value of the font
			public double cyDescent { get; private set; } /// cached descent value of the font

			private FontInfo(FontIdentifier id)
				: base(id.FamilyName, id.Style, id.Size)
			{
			}

			private FontInfo(string familyName, FontStyle style, double size)
				: base(familyName, style, size)
			{
			}

			public static FontInfo Create(FontIdentifier id, Graphics g, out Font font)
			{
				FontInfo result = new FontInfo(id);
				font = new Font(result.FamilyName, (float)result.Size, result.Style, GraphicsUnit.World);

				// get some properties of the font
				result.cyLineSpace = font.GetHeight(g); // space between two lines
				int iCellSpace = font.FontFamily.GetLineSpacing(font.Style);
				int iCellAscent = font.FontFamily.GetCellAscent(font.Style);
				int iCellDescent = font.FontFamily.GetCellDescent(font.Style);
				result.cyAscent = result.cyLineSpace * iCellAscent / iCellSpace;
				result.cyDescent = result.cyLineSpace * iCellDescent / iCellSpace;

				return result;
			}

			public static FontInfo Create(Graphics g, Font font)
			{
				FontInfo result = new FontInfo(font.FontFamily.Name, font.Style, font.Size);
				InternalGetInformation(g, result, font);
				return result;
			}

			private static void InternalGetInformation(Graphics g, FontInfo result, Font font)
			{
					// get some properties of the font
				result.cyLineSpace = font.GetHeight(g); // space between two lines
				int iCellSpace = font.FontFamily.GetLineSpacing(font.Style);
				int iCellAscent = font.FontFamily.GetCellAscent(font.Style);
				int iCellDescent = font.FontFamily.GetCellDescent(font.Style);
				result.cyAscent = result.cyLineSpace * iCellAscent / iCellSpace;
				result.cyDescent = result.cyLineSpace * iCellDescent / iCellSpace;
			}


		}

		class FontCache : IDisposable
		{
			Dictionary<FontIdentifier, FontInfo> _fontInfoDictionary = new Dictionary<FontIdentifier, FontInfo>();
			Dictionary<FontIdentifier, Font> _fontDictionary = new Dictionary<FontIdentifier, Font>();

			public FontInfo GetFontInfo(Graphics g, FontIdentifier id)
			{
				FontInfo result;
				if (!_fontInfoDictionary.TryGetValue(id, out result))
				{
					Font font;
					result = FontInfo.Create(id, g, out font);
					_fontInfoDictionary.Add(id, result);
					_fontDictionary.Add(id, font);
				}
				return result;
			}

			public Font GetFont(Graphics g, FontIdentifier id)
			{
				Font result;
				if (!_fontDictionary.TryGetValue(id, out result))
				{
					FontInfo info = GetFontInfo(g, id);
					result = _fontDictionary[id];
				}
				return result;
			}

			public void Clear()
			{
				_fontDictionary.Clear();
				_fontInfoDictionary.Clear();
			}

			#region IDisposable Members

			public void Dispose()
			{
				Clear();
			}

			#endregion
		}

		class StyleContext
		{
			public FontIdentifier BaseFontId { get; set; }
			public FontIdentifier FontId { get; set; }
			public Brush brush;

			public StyleContext(FontIdentifier font, Brush brush)
			{
				FontId = font;
				this.brush = brush;
			}

			public StyleContext Clone()
			{
				return (StyleContext)MemberwiseClone();
			}

			public void SetFont(FontFamily family, double size, FontStyle style)
			{
				FontId = new FontIdentifier(family.Name, style, size);
			}

			public void SetFont(string family, double size, FontStyle style)
			{
				FontId = new FontIdentifier(family, style, size);
			}

			public void ScaleFont(double scale)
			{
				FontId = new FontIdentifier(FontId.FamilyName, FontId.Style, scale * FontId.Size);
			}

			public void StyleFont(FontStyle style)
			{
				FontId = new FontIdentifier(FontId.FamilyName, style, FontId.Size);
			}

		}

		class MeasureContext
		{
			public object LinkedObject { get; set; }
			public FontCache FontCache { get; set; }
			public double TabStop { get; set; }
		}

		class DrawContext
		{
			public object LinkedObject { get; set; }
			public FontCache FontCache { get; set; }
			public bool bForPreview { get; set; }
			public Dictionary<GraphicsPath, IGPlotItem> _cachedSymbolPositions = new Dictionary<GraphicsPath, IGPlotItem>();
			public Matrix transformMatrix;
		}

		class Glyph
		{
			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			protected static StringFormat _stringFormat;

			/// <summary>Parent of this object.</summary>
			public StructuralGlyph Parent { get; set; }
			
			/// <summary>Style of this object.</summary>
			public StyleContext Style { get; set; }

			/// <summary>X position.</summary>
			public double X { get; set; }
			
			/// <summary>Y position.</summary>
			public double Y { get; set; }
			
			/// <summary>Width of the object.</summary>
			public double Width { get; set; }

			/// <summary>Height of the object. Setting this propery, you will set <see cref="ExtendAboveLine" /> and <see cref="ExtendBelowLine" /> both to Height/2.</summary>
			public double Height
			{
				get { return ExtendAboveBaseline + ExtendBelowBaseline; } 
				set { ExtendAboveBaseline = value / 2; ExtendBelowBaseline = value / 2; }
			}
			
			/// <summary>Height of the object above the baseline.</summary>
			public double ExtendAboveBaseline { get; set; }
			
			/// <summary>Extend of the object below the baseline. (Normally positive).</summary>
			public double ExtendBelowBaseline { get; set; }

			/// <summary></summary>
			public virtual void Measure(Graphics g, MeasureContext mc, double x)
			{
				Width = 0;
				Height = 0;
			}
		
			/// <summary>Draws the object.</summary>
			public virtual void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
			}

			/// <summary>
			/// Returns the commonly used StringFormat for all glyphs.
			/// </summary>
			public virtual StringFormat StringFormat { get { return _stringFormat; } }

			static Glyph()
			{
				_stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
				_stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

				_stringFormat.LineAlignment = StringAlignment.Near;
				_stringFormat.Alignment = StringAlignment.Near;
			}

		}

		#region Structural glyphs

		class StructuralGlyph : Glyph
		{
			public virtual void Add(Glyph g)
			{

			}
			public virtual void Exchange(StructuralGlyph presentchildnode, StructuralGlyph newchildnode)
			{
			}
		}

		class MultiChildGlyph : StructuralGlyph
		{
			protected List<Glyph> _childs = new List<Glyph>();

			public override void Add(Glyph g)
			{
				g.Parent = this;
				_childs.Add(g);
			}

			public override void Exchange(StructuralGlyph presentchildnode, StructuralGlyph newchildnode)
			{
				int idx = _childs.IndexOf(presentchildnode);
				if (idx < 0)
					throw new ArgumentException("presentchildnode is not a child of this node");

				_childs[idx] = newchildnode;
				newchildnode.Parent = this;
				presentchildnode.Parent = null;
			}

		}

		class VerticalStack : MultiChildGlyph
		{
			public double LineSpacingFactor = 1;
			public bool FixedLineSpacing = false;

			private double GetLineSpacing(Glyph line, FontInfo fontInfo)
			{
				if (FixedLineSpacing)
				{
					return fontInfo.cyLineSpace * LineSpacingFactor;
				}
				else
				{
					return line.Height;
				}
			}

			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				var fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);

				double w = 0, h = 0;
				double y = 0;

				foreach (var ch in _childs)
				{
					ch.Measure(g, mc, x);
					w = Math.Max(w, ch.Width);
					h = y + ch.Height;
					y += GetLineSpacing(ch, fontInfo);
				}

				Width = w;
				if (_childs.Count == 1)
				{
					ExtendAboveBaseline = _childs[0].ExtendAboveBaseline;
					ExtendBelowBaseline = _childs[0].ExtendBelowBaseline;
				}
				else if (_childs.Count == 2)
				{
					double heightDiff = h - (_childs[0].Height + _childs[1].Height);
					ExtendAboveBaseline = _childs[0].Height + heightDiff / 2;
					ExtendBelowBaseline = _childs[1].Height + heightDiff / 2;
				}
				else
				{
					Height = h;
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{

				var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);

				double y = ybase - ExtendAboveBaseline;

				foreach (var ch in _childs)
				{
					ch.Draw(g, dc, xbase, y + ch.ExtendAboveBaseline);
					y += GetLineSpacing(ch, fontInfo);
				}
			}
		}

		class GlyphLine : MultiChildGlyph
		{
			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				ExtendBelowBaseline = 0;
				ExtendAboveBaseline = 0;
				Width = 0;
				foreach (var glyph in _childs)
				{
					glyph.Measure(g, mc, x + Width);
					ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, glyph.ExtendAboveBaseline);
					ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, glyph.ExtendBelowBaseline);
					Width += glyph.Width;
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				double x = xbase;
				foreach (var ch in _childs)
				{
					ch.Draw(g, dc, x, ybase);
					x += ch.Width;
				}
			}
		}

		class SingleChildGlyph : StructuralGlyph
		{
			protected Glyph _child;

			public override void Add(Glyph g)
			{
				if (_child != null)
					throw new ArgumentException("child already present");

				g.Parent = this;
				_child = g;
			}

			public override void Exchange(StructuralGlyph presentchildnode, StructuralGlyph newchildnode)
			{
				if (_child != presentchildnode)
					throw new ArgumentException("presentchildnode is not a child of this node");

				_child = newchildnode;
				newchildnode.Parent = this;
				presentchildnode.Parent = null;
			}
		}


		class Subscript : SingleChildGlyph
		{
			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				ExtendAboveBaseline = 0;
				ExtendBelowBaseline = 0;
				Width = 0;
				if (_child != null)
				{
					_child.Measure(g, mc, x);

					FontInfo fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);
					double shift = (0.35 * fontInfo.cyAscent);
					ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _child.ExtendBelowBaseline + shift);
					ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _child.ExtendAboveBaseline - shift);
					Width = Math.Max(Width, _child.Width);
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				if (null != _child)
				{
					var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
					_child.Draw(g, dc, xbase, ybase + 0.35 * fontInfo.cyAscent);
				}
			}
		}

		class Superscript : SingleChildGlyph
		{
			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				ExtendAboveBaseline = 0;
				ExtendBelowBaseline = 0;
				Width = 0;
				if (_child != null)
				{
					_child.Measure(g, mc, x);
					var fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);
					double shift = (0.35 * fontInfo.cyAscent);
					ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _child.ExtendBelowBaseline - shift);
					ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _child.ExtendAboveBaseline + shift);
					Width = Math.Max(Width, _child.Width);
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				if (_child != null)
				{
					var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
					_child.Draw(g, dc, xbase, ybase - 0.35 * fontInfo.cyAscent);
				}
			}
		}

		class DotOverGlyph : SingleChildGlyph
		{
			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				ExtendAboveBaseline = 0;
				ExtendBelowBaseline = 0;
				Width = 0;
				if (_child != null)
				{
					_child.Measure(g, mc, x);
					ExtendBelowBaseline = _child.ExtendBelowBaseline;
					ExtendAboveBaseline = _child.ExtendAboveBaseline;
					Width = _child.Width;
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				if (_child != null)
				{
					_child.Draw(g, dc, xbase, ybase);
					Font font = dc.FontCache.GetFont(g, Style.FontId);
					FontInfo fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
					double psize = g.MeasureString(".", font, PointF.Empty, this.StringFormat).Width;
					g.DrawString(".", font, Style.brush, (float)(xbase + _child.Width/2 - psize/2), (float)(ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent), this.StringFormat);
				}
			}
		}

		class BarOverGlyph : SingleChildGlyph
		{
			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				ExtendAboveBaseline = 0;
				ExtendBelowBaseline = 0;
				Width = 0;
				if (_child != null)
				{
					_child.Measure(g, mc, x);
					ExtendBelowBaseline = _child.ExtendBelowBaseline;
					ExtendAboveBaseline = _child.ExtendAboveBaseline;
					Width = _child.Width;
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				if (_child != null)
				{
					_child.Draw(g, dc, xbase, ybase);
					Font font = dc.FontCache.GetFont(g, Style.FontId);
					FontInfo fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
					double psize = g.MeasureString(".", font, PointF.Empty, this.StringFormat).Width;
					g.DrawString(".", font, Style.brush, (float)(xbase + _child.Width / 2 - psize / 2), (float)(ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent), this.StringFormat);
				}
			}
		}

		class SubSuperScript : StructuralGlyph
		{
			Glyph _subscript;
			Glyph _superscript;

			public override void Add(Glyph g)
			{

				if (_subscript == null)
				{
					_subscript = g;
					g.Parent = this;
				}
				else if (_superscript == null)
				{
					_superscript = g;
					g.Parent = this;
				}
				else
				{
					throw new ArgumentException("both subscript and superscript are already present");
				}
			}

			public override void Exchange(StructuralGlyph presentchildnode, StructuralGlyph newchildnode)
			{
				if (_subscript == presentchildnode)
				{
					_subscript = newchildnode;
					newchildnode.Parent = this;
					presentchildnode = null;
				}
				else if (_superscript == presentchildnode)
				{
					_superscript = newchildnode;
					newchildnode.Parent = this;
					presentchildnode = null;
				}
				else
				{
					throw new ArgumentException("presentchildnode is not member of this node");
				}
			}

			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				ExtendAboveBaseline = 0;
				ExtendBelowBaseline = 0;
				Width = 0;

				var fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);
				if (_subscript != null)
				{
					_subscript.Measure(g, mc, x);

					double shift = (0.35 * fontInfo.cyAscent);
					ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _subscript.ExtendBelowBaseline + shift);
					ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _subscript.ExtendAboveBaseline - shift);
					Width = Math.Max(Width, _subscript.Width);
				}
				if (_superscript != null)
				{
					_superscript.Measure(g, mc, x);

					double shift = (0.35 * fontInfo.cyAscent);
					ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _subscript.ExtendBelowBaseline - shift);
					ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _subscript.ExtendAboveBaseline + shift);
					Width = Math.Max(Width, _superscript.Width);
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
				if (_subscript != null)
					_subscript.Draw(g, dc, xbase, ybase + 0.35 * fontInfo.cyAscent);
				if (_superscript != null)
					_superscript.Draw(g, dc, xbase, ybase - 0.35 * fontInfo.cyAscent);
			}
		}

		#endregion

		#region Glyph leaves

		class TextGlyph : Glyph
		{
			protected string _text;
		
			public TextGlyph(string text, StyleContext style)
			{
				_text = text;
				Style = style;
			}

			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				var fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);
				var font = mc.FontCache.GetFont(g, Style.FontId);

				Width = g.MeasureString(_text, font, PointF.Empty, _stringFormat).Width;
				ExtendAboveBaseline = fontInfo.cyAscent;
				ExtendBelowBaseline = fontInfo.cyDescent;
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
				var font = dc.FontCache.GetFont(g, Style.FontId);
				g.DrawString(_text, font, Style.brush, (float)xbase, (float)(ybase - fontInfo.cyAscent), _stringFormat);
			}

			public override string ToString()
			{
				return _text;
			}
		}

		class TabGlpyh : Glyph
		{
			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				Height = 0;
				Width = 0;

				double tab = mc.TabStop;
				
				if (!(tab> 0))
					tab = g.MeasureString("MMMM", mc.FontCache.GetFont(g, Style.BaseFontId), PointF.Empty, _stringFormat).Width;

				if(!(tab>0))
					tab = Style.BaseFontId.Size*4;

				if (tab > 0)
				{
					double t = Math.Floor(x / tab);
					Width = (t + 1) * tab - x;
				}
			}
		}

		class PlotName : TextGlyph
		{
			int _layerNumber;
			int _plotNumber;
			string _plotLabelStyle;
			bool _plotLabelStyleIsPropColName;


			public PlotName(StyleContext context, int plotNumber)
				: this(context, plotNumber, -1)
			{
			}
			public PlotName(StyleContext context, int plotNumber, int plotLayer)
				: base(string.Empty, context)
			{
				_plotNumber = plotNumber;
				_layerNumber = plotLayer;
			}

			public void SetPropertyColumnName(string name)
			{
				_plotLabelStyle = name;
				_plotLabelStyleIsPropColName = true;
			}

			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				_text = GetName(mc.LinkedObject);
				base.Measure(g, mc, x);
			}

			private string GetName(object obj)
			{
				string result = string.Empty;

				// first of all, retrieve the actual name
				if (obj is XYPlotLayer)
				{
					XYPlotLayer layer = (XYPlotLayer)obj;
					if (_layerNumber >= 0 && _layerNumber < layer.ParentLayerList.Count)
						layer = layer.ParentLayerList[_layerNumber];

					IGPlotItem pa = null;
					if (_plotNumber < layer.PlotItems.Flattened.Length)
					{
						pa = layer.PlotItems.Flattened[_plotNumber];
					}
					if (pa != null)
					{
						result = pa.GetName(0);

						if (_plotLabelStyle != null && !_plotLabelStyleIsPropColName && pa is XYColumnPlotItem)
						{
							XYColumnPlotItemLabelTextStyle style = XYColumnPlotItemLabelTextStyle.YS;
							try { style = (XYColumnPlotItemLabelTextStyle)Enum.Parse(typeof(XYColumnPlotItemLabelTextStyle), _plotLabelStyle, true); }
							catch (Exception) { }
							result = ((XYColumnPlotItem)pa).GetName(style);
						}

						if (_plotLabelStyleIsPropColName && _plotLabelStyle != null && pa is XYColumnPlotItem)
						{
							XYColumnPlotData pb = ((XYColumnPlotItem)pa).Data;
							Data.DataTable tbl = null;
							if (pb.YColumn is Data.DataColumn)
								tbl = Data.DataTable.GetParentDataTableOf((Data.DataColumn)pb.YColumn);

							if (tbl != null)
							{
								int colNumber = tbl.DataColumns.GetColumnNumber((Data.DataColumn)pb.YColumn);
								if (tbl.PropertyColumns.ContainsColumn(_plotLabelStyle))
									result = tbl.PropertyColumns[_plotLabelStyle][colNumber].ToString();
							}
						}
					}
				}

				return result;
			}



		}

		class PlotSymbol : Glyph
		{
			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			protected static StringFormat _stringFormat;

			int _layerNumber = -1;
			int _plotNumber;

			static PlotSymbol()
			{
				_stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
				_stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

				_stringFormat.LineAlignment = StringAlignment.Near;
				_stringFormat.Alignment = StringAlignment.Near;
			}

			public PlotSymbol(StyleContext style, int plotNumber)
				: this(style, plotNumber, -1)
			{
			}
			public PlotSymbol(StyleContext style, int plotNumber, int layerNumber)
			{
				Style = style;
				_plotNumber = plotNumber;
				_layerNumber = layerNumber;
			}

			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				Width = 0;
				Height = 0;

				object obj = mc.LinkedObject;
				if (obj is XYPlotLayer)
				{
					XYPlotLayer layer = (XYPlotLayer)obj;
					if (_layerNumber >= 0 && _layerNumber < layer.ParentLayerList.Count)
						layer = layer.ParentLayerList[_layerNumber];

					if (_plotNumber < layer.PlotItems.Flattened.Length)
					{
						var fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);
						var font = mc.FontCache.GetFont(g, Style.FontId);
						Width = g.MeasureString("MMM", font, PointF.Empty, _stringFormat).Width;
						ExtendAboveBaseline = fontInfo.cyAscent;
						ExtendBelowBaseline = fontInfo.cyDescent;
					}
				}
			}

			public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
			{
				XYPlotLayer layer = (XYPlotLayer)dc.LinkedObject;

				if (_layerNumber >= 0 && _layerNumber < layer.ParentLayerList.Count)
					layer = layer.ParentLayerList[_layerNumber];

				if (_plotNumber < layer.PlotItems.Flattened.Length)
				{
					var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
					var font = dc.FontCache.GetFont(g, Style.FontId);
					IGPlotItem pa = layer.PlotItems.Flattened[_plotNumber];

					PointF symbolpos = new PointF((float)xbase, (float)(ybase + 0.5f * fontInfo.cyDescent - 0.5f * fontInfo.cyAscent));
					RectangleF symbolRect = new RectangleF(symbolpos, new SizeF((float)Width, 0));
					symbolRect.Inflate(0, (float)(fontInfo.Size));
					pa.PaintSymbol(g, symbolRect);

					if (!dc.bForPreview)
					{
						GraphicsPath gp = new GraphicsPath();
						gp.AddRectangle(new RectangleF((float)(symbolpos.X), (float)(symbolpos.Y - 0.5f * fontInfo.cyLineSpace), (float)Width, (float)(fontInfo.cyLineSpace)));
						gp.Transform(dc.transformMatrix);
						dc._cachedSymbolPositions.Add(gp, pa);
					}
				}
			}
		}

		class DocumentIdentifier : TextGlyph
		{
			public DocumentIdentifier(StyleContext style)
			: base(null,style)
			{
			}
			public override void Measure(Graphics g, MeasureContext mc, double x)
			{
				_text = Current.Project.DocumentIdentifier;
				base.Measure(g, mc, x);
			}
		}

		#endregion

	}
}
