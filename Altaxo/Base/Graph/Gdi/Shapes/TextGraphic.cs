﻿#region Copyright

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
using System.Linq;

namespace Altaxo.Graph.Gdi.Shapes
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Drawing;
  using Background;
  using Geometry;
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
    protected FontX _font;
    protected BrushX _textBrush = new BrushX(NamedColors.Black);
    protected IBackgroundStyle? _background = null;
    protected double _lineSpacingFactor = 1.25f; // multiplicator for the line space, i.e. 1, 1.5 or 2

    #region Cached or temporary variables

    /// <summary>Hashtable where the keys are graphic paths giving the position of a symbol into the list, and the values are the plot items.</summary>
    protected Dictionary<GraphicsPath, IGPlotItem> _cachedSymbolPositions = new Dictionary<GraphicsPath, IGPlotItem>();

    private StructuralGlyph? _rootNode;
    protected bool _isStructureInSync = false; // true when the text was interpretet and the structure created
    protected bool _isMeasureInSync = false; // true when all items are measured
    protected PointD2D _cachedTextOffset; // offset of text to left upper corner of outer rectangle
    protected RectangleD2D _cachedExtendedTextBounds; // the text bounds extended by some margin around it

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

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TextGraphic?)o ?? new TextGraphic(info);
#pragma warning disable CS0618 // Type or member is obsolete
        info.GetBaseValueEmbeddedOrNull(s, "AltaxoBase,Altaxo.Graph.GraphicsObject,0", parent);
#pragma warning restore CS0618 // Type or member is obsolete

        // we have changed the meaning of rotation in the meantime, This is not handled in GetBaseValueEmbedded,
        // since the former versions did not store the version number of embedded bases
        //s._rotation = -s._rotation;

        s._text = info.GetString("Text");
        s._font = (FontX)info.GetValue("Font", s);
        s._textBrush = (BrushX)info.GetValue("Brush", s);
        s.BackgroundStyleOld = (BackgroundStyle)info.GetValue("BackgroundStyle", s);
        s._lineSpacingFactor = info.GetSingle("LineSpacing");
        info.GetSingle("ShadowLength");
        var xAnchorType = (XAnchorPositionType)info.GetValue("XAnchor", s);
        var yAnchorType = (YAnchorPositionType)info.GetValue("YAnchor", s);
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

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TextGraphic?)o ?? new TextGraphic(info);
        if (info.CurrentElementName == "BaseType") // that was included since 2006-06-20
        {
          info.GetBaseValueEmbedded(s, typeof(TextGraphic).BaseType!, parent);
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
          info.GetBaseValueEmbeddedOrNull(s, "AltaxoBase,Altaxo.Graph.GraphicsObject,0", parent); // before 2006-06-20, it was version 0 of the GraphicsObject
#pragma warning restore CS0618 // Type or member is obsolete
        }

        s._text = info.GetString("Text");
        s._font = (FontX)info.GetValue("Font", s);
        s._textBrush = (BrushX)info.GetValue("Brush", s);
        s.ChildSetMember(ref s._background, info.GetValueOrNull<IBackgroundStyle>("BackgroundStyle", s));
        s._lineSpacingFactor = info.GetSingle("LineSpacing");
        var xAnchorType = (XAnchorPositionType)info.GetValue("XAnchor", s);
        var yAnchorType = (YAnchorPositionType)info.GetValue("YAnchor", s);
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
        info.AddBaseValueEmbedded(s, typeof(TextGraphic).BaseType!);

        info.AddValue("Text", s._text);
        info.AddValue("Font", s._font);
        info.AddValue("Brush", s._textBrush);
        info.AddValueOrNull("BackgroundStyle", s._background);
        info.AddValue("LineSpacing", s._lineSpacingFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TextGraphic?)o ?? new TextGraphic(info);

        info.GetBaseValueEmbedded(s, typeof(TextGraphic).BaseType!, parent);

        s._text = info.GetString("Text");
        s._font = (FontX)info.GetValue("Font", s);
        s._textBrush = (BrushX)info.GetValue("Brush", s);

        s.Background = info.GetValueOrNull<IBackgroundStyle>("BackgroundStyle", s);

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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected TextGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(new ItemLocationDirectAutoSize())
    {
    }

    public TextGraphic(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirectAutoSize())
    {
      if (context is null)
        context = PropertyExtensions.GetPropertyContextOfProject();

      _font = context.GetValue(GraphDocument.PropertyKeyDefaultFont);
      _textBrush = new BrushX(context.GetValue(GraphDocument.PropertyKeyDefaultForeColor));
    }

    public TextGraphic(PointD2D graphicPosition, string text,
      FontX textFont, NamedColor textColor)
      : base(new ItemLocationDirectAutoSize())
    {
      SetPosition(graphicPosition, Main.EventFiring.Suppressed);
      Font = textFont;
      Text = text;
      Color = textColor;
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
      : base(from)
    {
      CopyFrom(from, false);
    }

    #endregion Constructors

    #region Copying
    [MemberNotNull(nameof(_font))]
    protected void CopyFrom(TextGraphic from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _text = from._text;
      _font = from._font;

      _textBrush = from._textBrush;

      _background = from._background is null ? null : (IBackgroundStyle)from._background.Clone();
      if (_background is not null)
        _background.ParentObject = this;

      _lineSpacingFactor = from._lineSpacingFactor;

      // don't clone the cached items
      _isStructureInSync = false;
      _isMeasureInSync = false;
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is TextGraphic from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    public override object Clone()
    {
      return new TextGraphic(this);
    }

    #endregion Copying

    private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
    {

      if (_background is not null)
        yield return new Main.DocumentNodeAndName(_background, "Background");
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
    }

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

      if (_background is not null)
      {
        // the distance to the sides should be like the character n
        distanceXL = 0.25 * widthOfOne_n; // left distance bounds-text
        distanceXR = distanceXL; // right distance text-bounds
        distanceYU = fontInfo.cyDescent;   // upper y distance bounding rectangle-string
        distanceYL = 0; // lower y distance
      }

      var size = new PointD2D((textWidth + distanceXL + distanceXR), (textHeight + distanceYU + distanceYL));
      _cachedExtendedTextBounds = new RectangleD2D(PointD2D.Empty, size);
      var textRectangle = new RectangleD2D(new PointD2D(-distanceXL, -distanceYU), size);

      if (_background is not null)
      {
        var backgroundRect = _background.MeasureItem(g, textRectangle);
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
      ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(size, false);

      _cachedTextOffset = new PointD2D(distanceXL, distanceYU);
    }

    public IBackgroundStyle? Background
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

    private BackgroundStyle BackgroundStyleOld
    {
      get
      {
        if (_background is null)
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
            _background = new BlackLine() { ParentObject = this };
            break;

          case BackgroundStyle.BlackOut:
            _background = new BlackOut() { ParentObject = this };
            break;

          case BackgroundStyle.DarkMarbel:
            _background = new DarkMarbel() { ParentObject = this };
            break;

          case BackgroundStyle.WhiteOut:
            _background = new WhiteOut() { ParentObject = this };
            break;

          case BackgroundStyle.Shadow:
            _background = new RectangleWithShadow() { ParentObject = this };
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

      if (!_isMeasureInSync)
        return;

      if (_background is not null)
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
      [MemberNotNull(nameof(_font))]
      set
      {
        if (!object.ReferenceEquals(_font, value))
        {
          _font = value;
          _isStructureInSync = false; // since the font is cached in the structure, it must be renewed
          _isMeasureInSync = false;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public bool Empty
    {
      get { return _text is null || _text.Length == 0; }
    }

    public string Text
    {
      get
      {
        return _text;
      }
      set
      {
        if (!(_text == value))
        {
          _text = value;
          _isStructureInSync = false;
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
        _textBrush = new BrushX(value);
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
        if (value is null)
          throw new ArgumentNullException();

        if (!(_textBrush == value))
        {
          _textBrush = value;
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

    [MemberNotNull(nameof(_rootNode))]
    private void InterpretText()
    {
      InterpretText(_text);
    }

    [MemberNotNull(nameof(_rootNode))]
    private void InterpretText(string text, bool isErrorMessage = false)
    {
      try
      {
        var parser = new Altaxo_LabelV1();
        parser.SetSource(text);
        bool bMatches = parser.MainSentence();
        var tree = parser.GetRoot();

        var walker = new TreeWalker(text);
        var style = new StyleContext(_font, _textBrush, _font);

        _rootNode = walker.VisitTree(tree, style, _lineSpacingFactor, true);
      }
      catch (Exception ex)
      {
        if (!isErrorMessage)
        {
          InterpretText(ex.Message);
        }
        else
        {
          InterpretText("Repeating error in parsing the text");
        }
      }
    }

    private void MeasureGlyphs(Graphics g, FontCache cache, IPaintContext paintContext)
    {
      var mc = new MeasureContext
      (
        fontCache: cache,
        linkedObject: GetLinkedObject(),
        tabStop: Glyph.MeasureString(g, "MMMM", _font).X
      );

      if (_rootNode is not null)
        _rootNode.Measure(g, mc, 0);
    }

    private object GetLinkedObject()
    {
      // Here, for the linked object we not only use HostLayer from 2D, but also
      // from 3D, because in order to show text preview an intermediate 3D TextGraphic object is created out of a 3D TextGraphic object
      return
         Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(this) ??
         Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Graph3D.HostLayer>(this) ??
         new object();
    }

    private void DrawGlyphs(Graphics g, DrawContext dc, double x, double y)
    {
      _rootNode?.Draw(g, dc, x, y + _rootNode.ExtendAboveBaseline);
    }

    /// <summary>
    /// Get the object outline for arrangements in object world coordinates.
    /// </summary>
    /// <returns>Object outline for arrangements in object world coordinates</returns>
    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      return GetRectangularObjectOutline();
    }

    public override void Paint(Graphics g, IPaintContext paintContext)
    {
      Paint(g, paintContext, false);
    }

    public void Paint(Graphics g, IPaintContext paintContext, bool bForPreview)
    {
      //_isStructureInSync = false;
      _isMeasureInSync = false;  // Change: interpret text every time in order to update plot items and \ID

      if (_rootNode is null || !_isStructureInSync)
      {
        // this.Interpret(g);
        InterpretText();

        _isStructureInSync = true;
        _isMeasureInSync = false;
      }

      using (var fontCache = new FontCache())
      {
        if (!_isMeasureInSync)
        {
          // this.MeasureStructure(g, obj);
          MeasureGlyphs(g, fontCache, paintContext);

          MeasureBackground(g, _rootNode.Width, _rootNode.Height);

          _isMeasureInSync = true;
        }

        _cachedSymbolPositions.Clear();

        System.Drawing.Drawing2D.GraphicsState gs = g.Save();
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        var bounds = Bounds;

        var transformmatrix = new Matrix();
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

        var dc = new DrawContext
        (
          fontCache: fontCache,
          isForPreview: bForPreview,
          linkedObject: GetLinkedObject(),
          transformationMatrix: transformmatrix,
          cachedSymbolPositions: _cachedSymbolPositions
        );
        DrawGlyphs(g, dc, _cachedTextOffset.X, _cachedTextOffset.Y);
        g.Restore(gs);
      }
    }

    #endregion Interpreting and Painting

    #region Hit testing and handling

    public static DoubleClickHandler? PlotItemEditorMethod;
    public static DoubleClickHandler? TextGraphicsEditorMethod;

    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      IHitTestObject? result;

      var pt = htd.GetHittedPointInWorldCoord(_transformation);

      foreach (GraphicsPath gp in _cachedSymbolPositions.Keys)
      {
        if (gp.IsVisible(pt.ToGdi()))
        {
          result = new HitTestObject(gp, _cachedSymbolPositions[gp])
          {
            DoubleClick = PlotItemEditorMethod
          };
          return result;
        }
      }

      result = base.HitTest(htd);
      if (result is not null)
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

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(BackgroundStyle), val, true);
      }
    }

    #endregion Deprecated classes

    #region IRoutedPropertyReceiver Members

    public IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "FontSize":
          yield return (propertyName, _font.Size, (value) => Font = _font.WithSize((double)value));
          break;

        case "FontFamily":
          yield return (propertyName, _font.FontFamilyName, (value) => Font = _font.WithFamily((string)value));
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  }
}
