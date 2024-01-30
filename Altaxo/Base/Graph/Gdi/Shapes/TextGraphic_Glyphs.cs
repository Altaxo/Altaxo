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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
  using Altaxo.Data;
  using Altaxo.Drawing;
  using Geometry;
  using Plot;

  public partial class TextGraphic : GraphicBase
  {
    /// <summary>
    /// Holds Information about the metrics of a font.
    /// </summary>
    private class FontInfo
    {
      public double cyLineSpace { get; private set; } // cached linespace value of the font

      public double cyAscent { get; private set; }    // cached ascent value of the font

      public double cyDescent { get; private set; } /// cached descent value of the font

      public double Size { get; private set; }

      private FontInfo()
      {
      }

      public static FontInfo Create(Graphics g, FontX font)
      {
        var result = new FontInfo();
        InternalGetInformation(g, result, font);
        return result;
      }

      private static void InternalGetInformation(Graphics g, FontInfo result, FontX font)
      {
        // get some properties of the font
        var gdiFont = GdiFontManager.ToGdi(font);
        result.Size = gdiFont.Size;
        result.cyLineSpace = gdiFont.GetHeight(g); // space between two lines
        int iCellSpace = gdiFont.FontFamily.GetLineSpacing(gdiFont.Style);
        int iCellAscent = gdiFont.FontFamily.GetCellAscent(gdiFont.Style);
        int iCellDescent = gdiFont.FontFamily.GetCellDescent(gdiFont.Style);
        result.cyAscent = result.cyLineSpace * iCellAscent / iCellSpace;
        result.cyDescent = result.cyLineSpace * iCellDescent / iCellSpace;
      }
    }

    private class FontCache : IDisposable
    {
      private Dictionary<FontX, FontInfo> _fontInfoDictionary = new Dictionary<FontX, FontInfo>();

      public FontInfo GetFontInfo(Graphics g, FontX id)
      {
        if (!_fontInfoDictionary.TryGetValue(id, out var result))
        {
          result = FontInfo.Create(g, id);
          _fontInfoDictionary.Add(id, result);
        }
        return result;
      }

      public void Clear()
      {
        _fontInfoDictionary.Clear();
      }

      #region IDisposable Members

      public void Dispose()
      {
        Clear();
      }

      #endregion IDisposable Members
    }

    private class StyleContext
    {
      public FontX BaseFontId { get; set; }

      public FontX FontId { get; set; }

      public BrushX brush;

      public StyleContext(FontX font, BrushX brush, FontX baseFontId)
      {
        FontId = font;
        this.brush = brush;
        BaseFontId = baseFontId;
      }

      public StyleContext Clone()
      {
        return (StyleContext)MemberwiseClone();
      }

      public void SetFont(FontX font)
      {
        FontId = font;
      }

      public void ScaleFont(double scale)
      {
        FontId = FontId.WithSize(scale * FontId.Size);
      }

      public void SetFontStyle(FontXStyle style)
      {
        FontId = FontId.WithStyle(style);
      }

      /// <summary>
      /// Merges the providedstyle into the present style. Example: if the present style is Bold, and the style parameter is Italic, then the merged style is 'Bold Italic'.
      /// </summary>
      /// <param name="style">The style to merge with the present style.</param>
      public void MergeFontStyle(FontXStyle style)
      {
        var newStyle = FontId.Style | style;
        FontId = FontId.WithStyle(newStyle);
      }
    }

    private class MeasureContext
    {
      public object LinkedObject { get; }

      public FontCache FontCache { get; }

      public double TabStop { get; }

      public MeasureContext(FontCache fontCache, object linkedObject, double tabStop)
      {
        FontCache = fontCache;
        LinkedObject = linkedObject;
        TabStop = tabStop;
      }
    }

    private class DrawContext
    {
      public object LinkedObject { get; }

      public FontCache FontCache { get; }

      public bool IsForPreview { get; }

      public Dictionary<GraphicsPath, IGPlotItem> CachedSymbolPositions { get; }
      public Matrix TransformationMatrix { get; }

      public DrawContext(FontCache fontCache, bool isForPreview, object linkedObject, Matrix transformationMatrix, Dictionary<GraphicsPath, IGPlotItem> cachedSymbolPositions)
      {
        FontCache = fontCache;
        IsForPreview = isForPreview;
        LinkedObject = linkedObject;
        TransformationMatrix = transformationMatrix;
        CachedSymbolPositions = cachedSymbolPositions;
      }
    }

    private class Glyph
    {
      // Modification of StringFormat is necessary to avoid
      // too big spaces between successive words
      protected static StringFormat _stringFormat;

      /// <summary>Parent of this object.</summary>
      public StructuralGlyph? Parent { get; set; }

      /// <summary>Style of this object.</summary>
      public StyleContext Style { get; set; }

      /// <summary>X position.</summary>
      public double X { get; set; }

      /// <summary>Y position.</summary>
      public double Y { get; set; }

      /// <summary>Width of the object.</summary>
      public double Width { get; set; }

      /// <summary>Height of the object. Setting this propery, you will set <see cref="ExtendAboveBaseline" /> and <see cref="ExtendBelowBaseline" /> both to Height/2.</summary>
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

      /// <summary>Measures the string with the appropriate generic typographic format.</summary>
      /// <param name="g">The graphics context.</param>
      /// <param name="text">The text to measure.</param>
      /// <param name="font">The font used.</param>
      /// <returns>Width and height of the text packed into a <see cref="PointD2D"/> structure.</returns>
      public static PointD2D MeasureString(Graphics g, string text, FontX font)
      {
        var result = g.MeasureString(text, GdiFontManager.ToGdi(font), PointF.Empty, _stringFormat);
        return new PointD2D(result.Width, result.Height);
      }

      public Glyph(StyleContext styleContext)
      {
        Style = styleContext;
      }
    }

    #region Structural glyphs

    private class StructuralGlyph : Glyph
    {
      public StructuralGlyph(StyleContext style) : base(style)
      {
      }

      public virtual void Add(Glyph g)
      {
      }

      public virtual void Exchange(StructuralGlyph presentchildnode, StructuralGlyph newchildnode)
      {
      }
    }

    private class MultiChildGlyph : StructuralGlyph
    {
      protected List<Glyph> _childs = new List<Glyph>();

      public MultiChildGlyph(StyleContext style) : base(style)
      {
      }

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

    private class VerticalStack : MultiChildGlyph
    {
      public double LineSpacingFactor = 1;
      public bool FixedLineSpacing = false;

      public VerticalStack(StyleContext style) : base(style)
      {
      }

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

    private class GlyphLine : MultiChildGlyph
    {
      public GlyphLine(StyleContext style) : base(style)
      {
      }

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

    private class SingleChildGlyph : StructuralGlyph
    {
      protected Glyph? _child;

      public SingleChildGlyph(StyleContext style) : base(style)
      {
      }

      public override void Add(Glyph g)
      {
        if (_child is not null)
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

    private class Subscript : SingleChildGlyph
    {
      public Subscript(StyleContext context) : base(context) { }

      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        Width = 0;
        if (_child is not null)
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
        if (_child is not null)
        {
          var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
          _child.Draw(g, dc, xbase, ybase + 0.35 * fontInfo.cyAscent);
        }
      }
    }

    private class Superscript : SingleChildGlyph
    {
      public Superscript(StyleContext context) : base(context) { }

      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        Width = 0;
        if (_child is not null)
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
        if (_child is not null)
        {
          var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
          _child.Draw(g, dc, xbase, ybase - 0.35 * fontInfo.cyAscent);
        }
      }
    }

    private class DotOverGlyph : SingleChildGlyph
    {
      public DotOverGlyph(StyleContext context) : base(context) { }
      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        Width = 0;
        if (_child is not null)
        {
          _child.Measure(g, mc, x);
          ExtendBelowBaseline = _child.ExtendBelowBaseline;
          ExtendAboveBaseline = _child.ExtendAboveBaseline;
          Width = _child.Width;
        }
      }

      public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
      {
        if (_child is not null)
        {
          _child.Draw(g, dc, xbase, ybase);
          FontInfo fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
          var gdiFont = GdiFontManager.ToGdi(Style.FontId);
          double psize = g.MeasureString(".", gdiFont, PointF.Empty, StringFormat).Width;
          using (var styleBrushGdi = BrushCacheGdi.Instance.BorrowBrush(Style.brush, new RectangleD2D(xbase + _child.Width / 2 - psize / 2, ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent, Width, ExtendAboveBaseline + ExtendBelowBaseline), g, 1))
          {

            g.DrawString(".", gdiFont, styleBrushGdi, (float)(xbase + _child.Width / 2 - psize / 2), (float)(ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent), StringFormat);
          }
        }
      }
    }

    private class BarOverGlyph : SingleChildGlyph
    {
      public BarOverGlyph(StyleContext context) : base(context) { }


      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        Width = 0;
        if (_child is not null)
        {
          _child.Measure(g, mc, x);
          ExtendBelowBaseline = _child.ExtendBelowBaseline;
          ExtendAboveBaseline = _child.ExtendAboveBaseline;
          Width = _child.Width;
        }
      }

      public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
      {
        if (_child is not null)
        {
          _child.Draw(g, dc, xbase, ybase);
          FontInfo fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
          using (var styleBrushGdi = BrushCacheGdi.Instance.BorrowBrush(Style.brush, new RectangleD2D(xbase, ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent, Width, ExtendAboveBaseline + ExtendBelowBaseline), g, 1))
          {
            g.DrawString("_", GdiFontManager.ToGdi(Style.FontId), styleBrushGdi, (float)(xbase), (float)(ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent), StringFormat);
          }
        }
      }
    }

    private class SubSuperScript : StructuralGlyph
    {
      private Glyph? _subscript;
      private Glyph? _superscript;

      public SubSuperScript(StyleContext context) : base(context)
      {
      }


      public override void Add(Glyph g)
      {
        if (_subscript is null)
        {
          _subscript = g;
          g.Parent = this;
        }
        else if (_superscript is null)
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
          presentchildnode.Parent = null;
        }
        else if (_superscript == presentchildnode)
        {
          _superscript = newchildnode;
          newchildnode.Parent = this;
          presentchildnode.Parent = null;
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
        if (_subscript is not null)
        {
          _subscript.Measure(g, mc, x);

          double shift = (0.35 * fontInfo.cyAscent);
          ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _subscript.ExtendBelowBaseline + shift);
          ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _subscript.ExtendAboveBaseline - shift);
          Width = Math.Max(Width, _subscript.Width);
        }
        if (_superscript is not null)
        {
          _superscript.Measure(g, mc, x);

          double shift = (0.35 * fontInfo.cyAscent);
          ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _superscript.ExtendBelowBaseline - shift);
          ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _superscript.ExtendAboveBaseline + shift);
          Width = Math.Max(Width, _superscript.Width);
        }
      }

      public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
      {
        var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
        if (_subscript is not null)
          _subscript.Draw(g, dc, xbase, ybase + 0.35 * fontInfo.cyAscent);
        if (_superscript is not null)
          _superscript.Draw(g, dc, xbase, ybase - 0.35 * fontInfo.cyAscent);
      }
    }

    #endregion Structural glyphs

    #region Glyph leaves

    private class TextGlyph : Glyph
    {
      protected string? _text;

      public TextGlyph(string? text, StyleContext style) : base(style)
      {
        _text = text;
      }

      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        var fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);
        Width = g.MeasureString(_text, GdiFontManager.ToGdi(Style.FontId), PointF.Empty, _stringFormat).Width;
        ExtendAboveBaseline = fontInfo.cyAscent;
        ExtendBelowBaseline = fontInfo.cyDescent;
      }

      public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
      {
        var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
        using (var styleBrushGdi = BrushCacheGdi.Instance.BorrowBrush(Style.brush, new RectangleD2D(0, 0, Width, ExtendAboveBaseline + ExtendBelowBaseline), g, 1))
        {
          g.DrawString(_text, GdiFontManager.ToGdi(Style.FontId), styleBrushGdi, (float)xbase, (float)(ybase - fontInfo.cyAscent), _stringFormat);
        }
      }

      public override string ToString()
      {
        return _text ?? string.Empty;
      }
    }

    private class TabGlpyh : Glyph
    {
      public TabGlpyh(StyleContext style) : base(style)
      {

      }

      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        Height = 0;
        Width = 0;

        double tab = mc.TabStop;

        if (!(tab > 0))
          tab = g.MeasureString("MMMM", GdiFontManager.ToGdi(Style.BaseFontId), PointF.Empty, _stringFormat).Width;

        if (!(tab > 0))
          tab = Style.BaseFontId.Size * 4;

        if (tab > 0)
        {
          double t = Math.Floor(x / tab);
          Width = (t + 1) * tab - x;
        }
      }
    }

    private class PlotName : TextGlyph
    {
      private int _layerNumber;
      private int _plotNumber;
      private string? _plotLabelStyle;
      private bool _plotLabelStyleIsPropColName;
      private string? _csharpformatstring;

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

      public void SetPropertyColumnName(string name, string? csharpformatstring = null)
      {
        _plotLabelStyle = name;
        _plotLabelStyleIsPropColName = true;
        _csharpformatstring = csharpformatstring;
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
        if (obj is not HostLayer mylayer)
          return result;

        var layer =
          (_layerNumber >= 0 && mylayer.SiblingLayers is not null && _layerNumber < mylayer.SiblingLayers.Count) ?
            mylayer.SiblingLayers[_layerNumber] as XYPlotLayer :
            mylayer as XYPlotLayer;

        if (layer is null)
          return result;

        IGPlotItem? pa = null;
        if (_plotNumber < layer.PlotItems.Flattened.Length)
        {
          pa = layer.PlotItems.Flattened[_plotNumber];
        }
        if (pa is not null)
        {
          result = pa.GetName(0);

          if (_plotLabelStyle is not null && !_plotLabelStyleIsPropColName && pa is XYColumnPlotItem)
          {
            XYColumnPlotItemLabelTextStyle style = XYColumnPlotItemLabelTextStyle.YS;
            try
            { style = (XYColumnPlotItemLabelTextStyle)Enum.Parse(typeof(XYColumnPlotItemLabelTextStyle), _plotLabelStyle, true); }
            catch (Exception) { }
            result = ((XYColumnPlotItem)pa).GetName(style);
          }

          if (_plotLabelStyleIsPropColName && _plotLabelStyle is not null && pa is XYColumnPlotItem xycpi)
          {
            var pb = xycpi.Data;
            var propertyValue = IndependentAndDependentColumns.GetPropertyValueOfCurve(pb, _plotLabelStyle);
            result = string.Empty;
            if (!propertyValue.IsEmpty)
            {
              if (!string.IsNullOrEmpty(_csharpformatstring))
              {
                var documentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (obj is Altaxo.Main.IDocumentLeafNode suspObj)
                {
                  var context = Altaxo.PropertyExtensions.GetPropertyContext(suspObj);
                  var documentCultureSettings = context.GetValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture) ?? throw new InvalidProgramException();
                  documentCulture = documentCultureSettings.Culture;
                }

                bool wasSuccess = false;
                try
                {
                  result = string.Format(documentCulture, "{0," + _csharpformatstring + "}", propertyValue);
                  result = result.Replace(' ', (char)0x2002); // replace normal char by fixed char
                  wasSuccess = true;
                }
                catch { }
                if (!wasSuccess)
                {
                  try
                  {
                    result = propertyValue.ToString(_csharpformatstring, documentCulture);
                    result = result.Replace(' ', (char)0x2002); // replace normal char by fixed char
                    wasSuccess = true;
                  }
                  catch { }
                }
                if (!wasSuccess)
                {
                  result = propertyValue.ToString();
                }
              }
              else
              {
                result = propertyValue.ToString();
              }
            }
          }
        }

        return result;
      }
    }

    private class PlotSymbol : Glyph
    {
      // Modification of StringFormat is necessary to avoid
      // too big spaces between successive words
      protected static new StringFormat _stringFormat;

      private int _layerNumber = -1;
      private int _plotNumber;

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

      public PlotSymbol(StyleContext style, int plotNumber, int layerNumber) : base(style)
      {
        _plotNumber = plotNumber;
        _layerNumber = layerNumber;
      }

      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        Width = 0;
        Height = 0;

        if (mc.LinkedObject is not HostLayer mylayer)
          return;

        var layer =
          (_layerNumber >= 0 && mylayer.SiblingLayers is not null && _layerNumber < mylayer.SiblingLayers.Count) ?
            mylayer.SiblingLayers[_layerNumber] as XYPlotLayer :
            mylayer as XYPlotLayer;

        if (layer is null)
          return;

        if (_plotNumber < layer.PlotItems.Flattened.Length)
        {
          var fontInfo = mc.FontCache.GetFontInfo(g, Style.FontId);
          Width = g.MeasureString("MMM", GdiFontManager.ToGdi(Style.FontId), PointF.Empty, _stringFormat).Width;
          ExtendAboveBaseline = fontInfo.cyAscent;
          ExtendBelowBaseline = fontInfo.cyDescent;
        }
      }

      public override void Draw(Graphics g, DrawContext dc, double xbase, double ybase)
      {
        if (dc.LinkedObject is not HostLayer mylayer)
          return;

        var layer =
          (_layerNumber >= 0 && mylayer.SiblingLayers is not null && _layerNumber < mylayer.SiblingLayers.Count) ?
              (mylayer.SiblingLayers[_layerNumber] as XYPlotLayer) :
              (mylayer as XYPlotLayer);

        if (layer is null)
          return;

        if (_plotNumber < layer.PlotItems.Flattened.Length)
        {
          var fontInfo = dc.FontCache.GetFontInfo(g, Style.FontId);
          IGPlotItem pa = layer.PlotItems.Flattened[_plotNumber];

          var symbolpos = new PointF((float)xbase, (float)(ybase + 0.5 * fontInfo.cyDescent - 0.5 * fontInfo.cyAscent));
          var symbolRect = new RectangleF(symbolpos, new SizeF((float)Width, 0));
          symbolRect.Inflate(0, (float)(fontInfo.Size));
          pa.PaintSymbol(g, symbolRect);

          if (!dc.IsForPreview)
          {
            var gp = new GraphicsPath();
            gp.AddRectangle(new RectangleF(symbolpos.X, (float)(symbolpos.Y - 0.5 * fontInfo.cyLineSpace), (float)Width, (float)(fontInfo.cyLineSpace)));
            gp.Transform(dc.TransformationMatrix);
            dc.CachedSymbolPositions.Add(gp, pa);
          }
        }
      }
    }

    private class DocumentIdentifier : TextGlyph
    {
      public DocumentIdentifier(StyleContext style)
        : base(null, style)
      {
      }

      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        _text = Current.Project.DocumentIdentifier;
        base.Measure(g, mc, x);
      }
    }

    private class ValueOfProperty : TextGlyph
    {
      private string _propertyName;

      public ValueOfProperty(StyleContext style, string propertyName)
        : base(null, style)
      {
        _propertyName = propertyName;
      }

      public override void Measure(Graphics g, MeasureContext mc, double x)
      {
        _text = string.Empty;
        var suspObj = mc.LinkedObject as Altaxo.Main.IDocumentLeafNode;
        if (suspObj is not null)
        {
          var context = Altaxo.PropertyExtensions.GetPropertyContext(suspObj);
          if (context is not null)
          {
            if (context.TryGetValue<object>(_propertyName, out var value, out var bag, out var info))
            {
              if (value is not null)
              {
                var documentCulture = context.GetValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture) ?? throw new InvalidProgramException();
                _text = string.Format(documentCulture.Culture, "{0}", value);
              }
            }
          }
        }

        base.Measure(g, mc, x);
      }
    }

    #endregion Glyph leaves
  }
}
