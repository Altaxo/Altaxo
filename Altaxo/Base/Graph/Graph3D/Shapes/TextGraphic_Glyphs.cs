﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Shapes
{
  using Drawing;
  using Drawing.D3D;
  using Geometry;
  using GraphicsContext;
  using Plot;

  public partial class TextGraphic : GraphicBase
  {
    private class FontCache : IDisposable
    {
      private Dictionary<FontX3D, FontInfo> _fontInfoDictionary = new Dictionary<FontX3D, FontInfo>();

      public FontInfo GetFontInfo(FontX3D id)
      {
        if (!_fontInfoDictionary.TryGetValue(id, out var result))
        {
          result = FontManager3D.Instance.GetFontInformation(id);
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
      public FontX3D BaseFontId { get; set; }

      public FontX3D FontId { get; set; }

      public IMaterial Brush { get; set; }

      public StyleContext(FontX3D font, IMaterial brush, FontX3D baseFontId)
      {
        FontId = font;
        Brush = brush;
        BaseFontId = baseFontId;
      }

      public StyleContext Clone()
      {
        return (StyleContext)MemberwiseClone();
      }

      public void SetFont(FontX3D font)
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

      public Dictionary<RectangleTransformedD3D, IGPlotItem> CachedSymbolPositions { get; }

      public Matrix4x3 TransformationMatrix { get; }

      public DrawContext(FontCache fontCache, bool isForPreview, object linkedObject, Matrix4x3 transformationMatrix, Dictionary<RectangleTransformedD3D, IGPlotItem> cachedSymbolPositions)
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
      /// <summary>Parent of this object.</summary>
      public StructuralGlyph? Parent { get; set; }

      /// <summary>Style of this object.</summary>
      public StyleContext Style { get; set; }

      /// <summary>X position.</summary>
      public double PositionX { get; set; }

      /// <summary>Y position.</summary>
      public double PositionY { get; set; }

      /// <summary>Width of the object.</summary>
      public double SizeX { get; set; }

      public double SizeZ { get; set; }

      /// <summary>Height of the object. Setting this propery, you will set <see cref="ExtendAboveBaseline" /> and <see cref="ExtendBelowBaseline" /> both to Height/2.</summary>
      public double SizeY
      {
        get { return ExtendAboveBaseline + ExtendBelowBaseline; }
        set { ExtendAboveBaseline = value / 2; ExtendBelowBaseline = value / 2; }
      }

      /// <summary>Height of the object above the baseline.</summary>
      public double ExtendAboveBaseline { get; set; }

      /// <summary>Extend of the object below the baseline. (Normally positive).</summary>
      public double ExtendBelowBaseline { get; set; }

      /// <summary></summary>
      public virtual void Measure(MeasureContext mc, double x)
      {
        SizeX = 0;
        SizeY = 0;
        SizeZ = 0;
      }

      /// <summary>Draws the object.</summary>
      public virtual void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
      }

      /// <summary>Measures the string with the appropriate generic typographic format.</summary>
      /// <param name="text">The text to measure.</param>
      /// <param name="font">The font used.</param>
      /// <returns>Width and height of the text packed into a <see cref="VectorD3D"/> structure.</returns>
      public static VectorD3D MeasureString(string text, FontX3D font)
      {
        return FontManager3D.Instance.MeasureString(text, font);
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
          return line.SizeY;
        }
      }

      public override void Measure(MeasureContext mc, double x)
      {
        var fontInfo = mc.FontCache.GetFontInfo(Style.FontId);

        double sizeX = 0, sizeY = 0, sizeZ = 0;
        double y = 0;

        foreach (var ch in _childs)
        {
          ch.Measure(mc, x);
          sizeX = Math.Max(sizeX, ch.SizeX);
          sizeY = y + ch.SizeY;
          sizeZ = Math.Max(sizeZ, ch.SizeZ);
          y += GetLineSpacing(ch, fontInfo);
        }

        SizeX = sizeX;
        if (_childs.Count == 1)
        {
          ExtendAboveBaseline = _childs[0].ExtendAboveBaseline;
          ExtendBelowBaseline = _childs[0].ExtendBelowBaseline;
        }
        else if (_childs.Count == 2)
        {
          double heightDiff = sizeY - (_childs[0].SizeY + _childs[1].SizeY);
          ExtendAboveBaseline = _childs[0].SizeY + heightDiff / 2;
          ExtendBelowBaseline = _childs[1].SizeY + heightDiff / 2;
        }
        else
        {
          SizeY = sizeY;
        }
        SizeZ = sizeZ;
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        var fontInfo = dc.FontCache.GetFontInfo(Style.FontId);

        var y = ybase - ExtendBelowBaseline;

        for (int i = _childs.Count - 1; i >= 0; --i)
        {
          var ch = _childs[i];
          ch.Draw(g, dc, xbase, y + ch.ExtendBelowBaseline, zbase);
          y += GetLineSpacing(ch, fontInfo);
        }
      }
    }

    private class GlyphLine : MultiChildGlyph
    {
      public GlyphLine(StyleContext style) : base(style)
      {

      }

      public override void Measure(MeasureContext mc, double x)
      {
        ExtendBelowBaseline = 0;
        ExtendAboveBaseline = 0;
        SizeX = 0;
        SizeZ = 0;
        foreach (var glyph in _childs)
        {
          glyph.Measure(mc, x + SizeX);
          ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, glyph.ExtendAboveBaseline);
          ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, glyph.ExtendBelowBaseline);
          SizeX += glyph.SizeX;
          SizeZ = Math.Max(SizeZ, glyph.SizeZ);
        }
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        double x = xbase;
        foreach (var ch in _childs)
        {
          ch.Draw(g, dc, x, ybase, zbase);
          x += ch.SizeX;
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

      public override void Measure(MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        SizeX = 0;
        SizeZ = 0;
        if (_child is not null)
        {
          _child.Measure(mc, x);

          FontInfo fontInfo = mc.FontCache.GetFontInfo(Style.FontId);
          double shift = (0.35 * fontInfo.cyAscent);
          ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _child.ExtendBelowBaseline + shift);
          ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _child.ExtendAboveBaseline - shift);
          SizeX = Math.Max(SizeX, _child.SizeX);
          SizeZ = Math.Max(SizeZ, _child.SizeZ);
        }
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        if (_child is not null)
        {
          var fontInfo = dc.FontCache.GetFontInfo(Style.FontId);
          _child.Draw(g, dc, xbase, ybase - 0.35 * fontInfo.cyAscent, zbase);
        }
      }
    }

    private class Superscript : SingleChildGlyph
    {
      public Superscript(StyleContext context) : base(context) { }


      public override void Measure(MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        SizeX = 0;
        SizeZ = 0;
        if (_child is not null)
        {
          _child.Measure(mc, x);
          var fontInfo = mc.FontCache.GetFontInfo(Style.FontId);
          double shift = (0.35 * fontInfo.cyAscent);
          ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _child.ExtendBelowBaseline - shift);
          ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _child.ExtendAboveBaseline + shift);
          SizeX = Math.Max(SizeX, _child.SizeX);
          SizeZ = Math.Max(SizeZ, _child.SizeZ);
        }
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        if (_child is not null)
        {
          var fontInfo = dc.FontCache.GetFontInfo(Style.FontId);
          _child.Draw(g, dc, xbase, ybase + 0.35 * fontInfo.cyAscent, zbase);
        }
      }
    }

    private class DotOverGlyph : SingleChildGlyph
    {
      public DotOverGlyph(StyleContext context) : base(context) { }


      public override void Measure(MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        SizeX = 0;
        SizeZ = 0;
        if (_child is not null)
        {
          _child.Measure(mc, x);
          ExtendBelowBaseline = _child.ExtendBelowBaseline;
          ExtendAboveBaseline = _child.ExtendAboveBaseline;
          SizeX = _child.SizeX;
          SizeZ = _child.SizeZ;
        }
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        if (_child is not null)
        {
          _child.Draw(g, dc, xbase, ybase, zbase);
          FontInfo fontInfo = dc.FontCache.GetFontInfo(Style.FontId);
          double psize = FontManager3D.Instance.MeasureString(".", Style.FontId).X;
          g.DrawString(".", Style.FontId, Style.Brush, new PointD3D((xbase + _child.SizeX / 2 - psize / 2), (ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent), zbase));
        }
      }
    }

    private class BarOverGlyph : SingleChildGlyph
    {
      public BarOverGlyph(StyleContext context) : base(context) { }


      public override void Measure(MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        SizeX = 0;
        SizeZ = 0;
        if (_child is not null)
        {
          _child.Measure(mc, x);
          ExtendBelowBaseline = _child.ExtendBelowBaseline;
          ExtendAboveBaseline = _child.ExtendAboveBaseline;
          SizeX = _child.SizeX;
          SizeZ = _child.SizeZ;
        }
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        if (_child is not null)
        {
          _child.Draw(g, dc, xbase, ybase, zbase);
          FontInfo fontInfo = dc.FontCache.GetFontInfo(Style.FontId);
          g.DrawString("_", Style.FontId, Style.Brush, new PointD3D((xbase), (ybase - _child.ExtendAboveBaseline - fontInfo.cyAscent), zbase));
        }
      }
    }

    private class SubSuperScript : StructuralGlyph
    {
      private Glyph? _subscript;
      private Glyph? _superscript;

      public SubSuperScript(StyleContext context) : base(context) { }


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

      public override void Measure(MeasureContext mc, double x)
      {
        ExtendAboveBaseline = 0;
        ExtendBelowBaseline = 0;
        SizeX = 0;
        SizeZ = 0;

        var fontInfo = mc.FontCache.GetFontInfo(Style.FontId);
        if (_subscript is not null)
        {
          _subscript.Measure(mc, x);

          double shift = (0.35 * fontInfo.cyAscent);
          ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _subscript.ExtendBelowBaseline + shift);
          ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _subscript.ExtendAboveBaseline - shift);
          SizeX = Math.Max(SizeX, _subscript.SizeX);
          SizeZ = Math.Max(SizeZ, _subscript.SizeZ);
        }
        if (_superscript is not null)
        {
          _superscript.Measure(mc, x);

          double shift = (0.35 * fontInfo.cyAscent);
          ExtendBelowBaseline = Math.Max(ExtendBelowBaseline, _superscript.ExtendBelowBaseline - shift);
          ExtendAboveBaseline = Math.Max(ExtendAboveBaseline, _superscript.ExtendAboveBaseline + shift);
          SizeX = Math.Max(SizeX, _superscript.SizeX);
          SizeZ = Math.Max(SizeZ, _superscript.SizeZ);
        }
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        var fontInfo = dc.FontCache.GetFontInfo(Style.FontId);
        if (_subscript is not null)
          _subscript.Draw(g, dc, xbase, ybase - 0.35 * fontInfo.cyAscent, zbase);
        if (_superscript is not null)
          _superscript.Draw(g, dc, xbase, ybase + 0.35 * fontInfo.cyAscent, zbase);
      }
    }

    #endregion Structural glyphs

    #region Glyph leaves

    private class TextGlyph : Glyph
    {
      protected string _text;

      public TextGlyph(string? text, StyleContext style) : base(style)
      {
        _text = text ?? string.Empty;
        Style = style;
      }

      public override void Measure(MeasureContext mc, double x)
      {
        var fontInfo = mc.FontCache.GetFontInfo(Style.FontId);
        var size = FontManager3D.Instance.MeasureString(_text, Style.FontId);
        SizeX = size.X;
        ExtendAboveBaseline = fontInfo.cyAscent;
        ExtendBelowBaseline = fontInfo.cyDescent;
        SizeZ = size.Z;
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        var fontInfo = dc.FontCache.GetFontInfo(Style.FontId);
        g.DrawString(_text, Style.FontId, Style.Brush, new PointD3D(xbase, (ybase - fontInfo.cyDescent), zbase));
      }

      public override string ToString()
      {
        return _text;
      }
    }

    private class TabGlpyh : Glyph
    {
      public TabGlpyh(StyleContext style) : base(style)
      {

      }


      public override void Measure(MeasureContext mc, double x)
      {
        SizeY = 0;
        SizeX = 0;
        SizeZ = 0;

        double tab = mc.TabStop;

        if (!(tab > 0))
          tab = FontManager3D.Instance.MeasureString("MMMM", Style.BaseFontId).X;

        if (!(tab > 0))
          tab = Style.BaseFontId.Size * 4;

        if (tab > 0)
        {
          double t = Math.Floor(x / tab);
          SizeX = (t + 1) * tab - x;
        }
      }
    }

    private class PlotName : TextGlyph
    {
      private int _layerNumber;
      private int _plotNumber;
      private string? _plotLabelStyle;
      private bool _plotLabelStyleIsPropColName;

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

      public override void Measure(MeasureContext mc, double x)
      {
        _text = GetName(mc.LinkedObject);
        base.Measure(mc, x);
      }

      private string GetName(object obj)
      {
        string result = string.Empty;

        // first of all, retrieve the actual name
        var mylayer = obj as HostLayer;
        if (mylayer is null)
          return result;

        var layer = mylayer as XYZPlotLayer;
        if (_layerNumber >= 0 && mylayer.SiblingLayers is not null && _layerNumber < mylayer.SiblingLayers.Count)
          layer = mylayer.SiblingLayers[_layerNumber] as XYZPlotLayer;
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

          if (_plotLabelStyle is not null && !_plotLabelStyleIsPropColName && pa is XYZColumnPlotItem)
          {
            var style = Altaxo.Graph.Gdi.Plot.XYColumnPlotItemLabelTextStyle.YS;
            try
            { style = (Altaxo.Graph.Gdi.Plot.XYColumnPlotItemLabelTextStyle)Enum.Parse(typeof(Altaxo.Graph.Gdi.Plot.XYColumnPlotItemLabelTextStyle), _plotLabelStyle, true); }
            catch (Exception) { }
            result = ((XYZColumnPlotItem)pa).GetName((int)style);
          }

          if (_plotLabelStyleIsPropColName && _plotLabelStyle is not null && pa is XYZColumnPlotItem)
          {
            var pb = ((XYZColumnPlotItem)pa).Data;
            if (pb.YColumn is Data.DataColumn ycol)
            {
              if (Data.DataTable.GetParentDataTableOf(ycol) is { } tbl)
              {
                int colNumber = tbl.DataColumns.GetColumnNumber(ycol);
                if (tbl.PropertyColumns.ContainsColumn(_plotLabelStyle))
                  result = tbl.PropertyColumns[_plotLabelStyle][colNumber].ToString();
              }
            }
          }
        }

        return result;
      }
    }

    private class PlotSymbol : Glyph
    {
      private int _layerNumber = -1;
      private int _plotNumber;

      static PlotSymbol()
      {
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

      public override void Measure(MeasureContext mc, double x)
      {
        SizeX = 0;
        SizeY = 0;
        SizeZ = 0;

        var mylayer = mc.LinkedObject as HostLayer;
        if (mylayer is null)
          return;
        var layer = mylayer as XYZPlotLayer;
        if (_layerNumber >= 0 && mylayer.SiblingLayers is not null && _layerNumber < mylayer.SiblingLayers.Count)
          layer = mylayer.SiblingLayers[_layerNumber] as XYZPlotLayer;

        if (layer is null)
          return;

        if (_plotNumber < layer.PlotItems.Flattened.Length)
        {
          var fontInfo = mc.FontCache.GetFontInfo(Style.FontId);
          var size = FontManager3D.Instance.MeasureString("MMM", Style.FontId);
          SizeX = size.X;
          ExtendAboveBaseline = fontInfo.cyAscent;
          ExtendBelowBaseline = fontInfo.cyDescent;
          SizeZ = size.Z;
        }
      }

      public override void Draw(IGraphicsContext3D g, DrawContext dc, double xbase, double ybase, double zbase)
      {
        var mylayer = (HostLayer)dc.LinkedObject;

        var layer = mylayer as XYZPlotLayer;

        if (_layerNumber >= 0 && mylayer.SiblingLayers is not null && _layerNumber < mylayer.SiblingLayers.Count)
          layer = mylayer.SiblingLayers[_layerNumber] as XYZPlotLayer;

        if (layer is null)
          return;

        if (_plotNumber < layer.PlotItems.Flattened.Length)
        {
          var fontInfo = dc.FontCache.GetFontInfo(Style.FontId);
          IGPlotItem pa = layer.PlotItems.Flattened[_plotNumber];

          var symbolpos = new PointD3D(xbase, (ybase + 0.5 * fontInfo.cyDescent - 0.5 * fontInfo.cyAscent), 0);
          var symbolRect = new RectangleD3D(symbolpos, new VectorD3D(SizeX, 0, 0));
          symbolRect = symbolRect.WithPadding(0, fontInfo.Size, 0);
          pa.PaintSymbol(g, symbolRect);

          if (!dc.IsForPreview)
          {
            var volume = new RectangleTransformedD3D(
              new RectangleD3D(symbolpos.X, symbolpos.Y - 0.5 * fontInfo.cyLineSpace, 0, SizeX, fontInfo.cyLineSpace, 0), dc.TransformationMatrix);
            dc.CachedSymbolPositions.Add(volume, pa);
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

      public override void Measure(MeasureContext mc, double x)
      {
        _text = Current.Project.DocumentIdentifier;
        base.Measure(mc, x);
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

      public override void Measure(MeasureContext mc, double x)
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
                var documentCulture = context.GetValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture);
                _text = string.Format(documentCulture!.Culture, "{0}", value);
              }
            }
          }
        }

        base.Measure(mc, x);
      }
    }

    #endregion Glyph leaves
  }
}
