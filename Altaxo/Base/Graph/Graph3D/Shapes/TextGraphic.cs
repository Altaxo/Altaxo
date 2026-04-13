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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.Shapes
{
  using System.Diagnostics.CodeAnalysis;
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
    /// <summary>
    /// Stores the text including formatting codes.
    /// </summary>
    protected string _text = string.Empty; // the text, which contains the formatting symbols
    /// <summary>
    /// Stores the font used for rendering.
    /// </summary>
    protected FontX3D _font;
    /// <summary>
    /// Stores the material used to draw the text.
    /// </summary>
    protected IMaterial _textBrush = Materials.GetSolidMaterial(NamedColors.Black);
    /// <summary>
    /// Stores the optional background style.
    /// </summary>
    protected IBackgroundStyle? _background = null;
    /// <summary>
    /// Stores the line spacing factor.
    /// </summary>
    protected double _lineSpacingFactor = 1.25f; // multiplicator for the line space, i.e. 1, 1.5 or 2

    #region Cached or temporary variables

    /// <summary>Hashtable where the keys are graphic paths giving the position of a symbol into the list, and the values are the plot items.</summary>
    protected Dictionary<RectangleTransformedD3D, IGPlotItem> _cachedSymbolPositions = new Dictionary<RectangleTransformedD3D, IGPlotItem>();

    private StructuralGlyph? _rootNode;
    /// <summary>
    /// Indicates whether the parsed text structure is up to date.
    /// </summary>
    protected bool _isStructureInSync = false; // true when the text was interpretet and the structure created
    /// <summary>
    /// Indicates whether all layout measurements are up to date.
    /// </summary>
    protected bool _isMeasureInSync = false; // true when all items are measured

    /// <summary>The size of the text rectangle as is - i.e. without padding, background, etc.</summary>
    protected VectorD3D CachedTextSizeWithoutPadding { get { return _rootNode is not null ? new VectorD3D(_rootNode.SizeX, _rootNode.SizeY, _rootNode.SizeZ) : VectorD3D.Empty; } }

    /// <summary>
    /// Caches the text padding contributed by the background style.
    /// </summary>
    protected Margin2D _cachedTextPadding;
    /// <summary>
    /// Caches the offset from the drawing origin to the unpadded text rectangle.
    /// </summary>
    protected PointD3D _cachedTextOffset; // offset from the lower left corner of the background or drawing origin to the lower left corner of the unpadded text rectangle
    /// <summary>
    /// Caches the total text bounds including background margins.
    /// </summary>
    protected RectangleD3D _cachedExtendedTextBounds; // the text bounds extended by some margin around it

    #endregion Cached or temporary variables

    #region Serialization

    /// <summary>
    /// 2015-09-11 initial version
    /// </summary>
    /// <summary>
    /// Serializes <see cref="TextGraphic"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextGraphic), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextGraphic)o;

        info.AddBaseValueEmbedded(s, typeof(TextGraphic).BaseType!);
        info.AddValue("Text", s._text);
        info.AddValue("Font", s._font);
        info.AddValue("Brush", s._textBrush);
        info.AddValueOrNull("BackgroundStyle", s._background);
        info.AddValue("LineSpacing", s._lineSpacingFactor);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (TextGraphic?)o ?? new TextGraphic(info);

        info.GetBaseValueEmbedded(s, typeof(TextGraphic).BaseType!, parent);

        s._text = info.GetString("Text");
        s._font = (FontX3D)info.GetValue("Font", s);
        s._textBrush = (IMaterial)info.GetValue("Brush", s);
        s.Background = info.GetValueOrNull<IBackgroundStyle>("BackgroundStyle", s);
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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected TextGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextGraphic"/> class from a property context.
    /// </summary>
    /// <param name="context">The property context.</param>
    public TextGraphic(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirectAutoSize())
    {
      if (context is null)
        context = PropertyExtensions.GetPropertyContextOfProject();

      _font = GraphDocument.GetDefaultFont(context);
      _textBrush = Materials.GetSolidMaterial(GraphDocument.GetDefaultForeColor(context));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="text">The text content.</param>
    /// <param name="textFont">The text font.</param>
    /// <param name="textColor">The text color.</param>
    public TextGraphic(PointD3D graphicPosition, string text, FontX3D textFont, NamedColor textColor)
      : base(new ItemLocationDirectAutoSize())
    {
      SetPosition(graphicPosition, Main.EventFiring.Suppressed);
      Font = textFont;
      Text = text;
      Color = textColor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextGraphic"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The text graphic to copy from.</param>
    public TextGraphic(TextGraphic from)
      : base(from) // all is done here, since CopyFrom is virtual!
    {
      CopyFrom(from, false);
    }

    #endregion Constructors

    #region Copying
    /// <summary>
    /// Copies values from another <see cref="TextGraphic"/> instance.
    /// </summary>
    /// <param name="from">The text graphic to copy from.</param>
    /// <param name="withBaseMembers">If set to <c>true</c>, base members are copied as well.</param>
    [MemberNotNull(nameof(_font))]
    protected void CopyFrom(TextGraphic from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _text = from._text;
      _font = from._font;

      _textBrush = from._textBrush;
      ChildCloneToMember(ref _background, from._background);
      _lineSpacingFactor = from._lineSpacingFactor;

      // don't clone the cached items
      _isStructureInSync = false;
      _isMeasureInSync = false;
    }

    /// <inheritdoc/>
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



    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
    }

    #region Background

    /// <summary>
    /// Measures the background layout for the current text content.
    /// </summary>
    /// <param name="g">The graphics context used for measurement.</param>
    /// <param name="itemSizeX">The measured text width.</param>
    /// <param name="itemSizeY">The measured text height.</param>
    /// <param name="itemSizeZ">The measured text depth.</param>
    protected void MeasureBackground(IGraphicsContext3D g, double itemSizeX, double itemSizeY, double itemSizeZ)
    {
      var fontInfo = FontManager3D.Instance.GetFontInformation(_font);

      double widthOfOne_n = Glyph.MeasureString("n", _font).X;
      double widthOfThree_M = Glyph.MeasureString("MMM", _font).X;

      if (_background is not null)
      {
        _cachedTextPadding = new Margin2D(
          0.25 * widthOfOne_n,
          fontInfo.cyDescent,
          0.25 * widthOfOne_n,
          fontInfo.cyDescent
          );
      }
      else
      {
        _cachedTextPadding = new Margin2D(0, 0, 0, 0);
      }

      var paddedTextSize = new VectorD3D((itemSizeX + _cachedTextPadding.Left + _cachedTextPadding.Right), (itemSizeY + _cachedTextPadding.Bottom + _cachedTextPadding.Top), itemSizeZ);
      var textRectangle = new RectangleD3D(PointD3D.Empty, paddedTextSize); // the origin of the padded text rectangle is always 0

      if (_background is not null)
      {
        var backgroundRect = _background.Measure(textRectangle);
        _cachedExtendedTextBounds = backgroundRect.WithRectangleIncluded(textRectangle); //  _cachedExtendedTextBounds.WithOffset(textRectangle.X - backgroundRect.X, textRectangle.Y - backgroundRect.Y, 0);

        ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(_cachedExtendedTextBounds.Size, false);
        _cachedTextOffset = new PointD3D(textRectangle.X - backgroundRect.X + _cachedTextPadding.Left, textRectangle.Y - backgroundRect.Y + _cachedTextPadding.Bottom, 0);
      }
      else
      {
        ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(paddedTextSize, false);
        _cachedExtendedTextBounds = textRectangle;
        _cachedTextOffset = PointD3D.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the background style.
    /// </summary>
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

    /// <summary>
    /// Paints the configured background.
    /// </summary>
    /// <param name="g">The graphics context used for drawing.</param>
    protected virtual void PaintBackground(IGraphicsContext3D g)
    {
      // Assumptions:
      // 1. the overall size of the structure must be measured before, i.e. bMeasureInSync is true
      // 2. the graphics object was translated and rotated before, so that the paining starts at (0,0)

      if (!_isMeasureInSync)
        return;

      if (_background is not null)
      {
        var textSizeWithPadding = CachedTextSizeWithoutPadding + new VectorD3D(_cachedTextPadding.Left + _cachedTextPadding.Right, _cachedTextPadding.Top + _cachedTextPadding.Bottom, 0);

        _background.Draw(g, new RectangleD3D(PointD3D.Empty, textSizeWithPadding));
      }
    }

    #endregion Background

    #region Properties

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("TextGraphics Text: <<{0}>>", _text);
    }

    /// <inheritdoc/>
    public override bool AutoSize
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    public FontX3D Font
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

    /// <summary>
    /// Gets a value indicating whether the text is empty.
    /// </summary>
    public bool Empty
    {
      get { return _text is null || _text.Length == 0; }
    }

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
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
          _isStructureInSync = false;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the material used to fill the text.
    /// </summary>
    public IMaterial TextFillBrush
    {
      get
      {
        return _textBrush;
      }
      set
      {
        if (value is null)
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

    /// <summary>
    /// Gets or sets the line-spacing factor.
    /// </summary>
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
      var parser = new Altaxo_LabelV1();
      parser.SetSource(_text);
      bool bMatches = parser.MainSentence();
      var tree = parser.GetRoot();

      var walker = new TreeWalker(_text);
      var style = new StyleContext(_font, _textBrush, baseFontId: _font);
      _rootNode = walker.VisitTree(tree, style, _lineSpacingFactor, true);
    }

    private void MeasureGlyphs(IGraphicsContext3D g, FontCache cache, Altaxo.Graph.IPaintContext paintContext)
    {
      var mc = new MeasureContext
      (
        fontCache: cache,
        linkedObject: Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(this) ?? throw new InvalidProgramException(),
        tabStop: Glyph.MeasureString("MMMM", _font).X
      );

      _rootNode?.Measure(mc, 0);
    }

    private void DrawGlyphs(IGraphicsContext3D g, DrawContext dc, double x, double y, double z)
    {
      _rootNode?.Draw(g, dc, x, y + _rootNode.ExtendBelowBaseline, z);
    }

    /// <inheritdoc/>
    public override void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext)
    {
      Paint(g, paintContext, false);
    }

    /// <summary>
    /// Paints the text graphic.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="paintContext">The paint context.</param>
    /// <param name="bForPreview">If set to <c>true</c>, paints in preview mode.</param>
    public void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, bool bForPreview)
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

          MeasureBackground(g, _rootNode.SizeX, _rootNode.SizeY, _rootNode.SizeZ);

          _isMeasureInSync = true;
        }

        _cachedSymbolPositions.Clear();

        var gs = g.SaveGraphicsState();

        var bounds = Bounds;

        var transformmatrix = Matrix4x3.FromScaleShearRotationDegreeTranslation(
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

        var dc = new DrawContext
        (
          fontCache: fontCache,
          isForPreview: bForPreview,
          linkedObject: Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<HostLayer>(this) ?? throw new InvalidProgramException(),
          transformationMatrix: transformmatrix,
          cachedSymbolPositions: _cachedSymbolPositions
        );
        DrawGlyphs(g, dc, _cachedTextOffset.X, _cachedTextOffset.Y, _cachedTextOffset.Z);
        g.RestoreGraphicsState(gs);
      }
    }

    #endregion Interpreting and Painting

    #region Hit testing and handling

    /// <summary>
    /// Gets or sets the editor callback for plot-item placeholders.
    /// </summary>
    public static DoubleClickHandler? PlotItemEditorMethod;
    /// <summary>
    /// Gets or sets the editor callback for text graphics.
    /// </summary>
    public static DoubleClickHandler? TextGraphicsEditorMethod;

    /// <inheritdoc/>
    public override IHitTestObject? HitTest(HitTestPointData parentHitData)
    {
      //			HitTestPointData layerHitTestData = pageC.NewFromTranslationRotationScaleShear(Position.X, Position.Y, -Rotation, ScaleX, ScaleY, ShearX);
      var localHitData = parentHitData.NewFromAdditionalTransformation(_transformation);

      if (localHitData.IsHit(Bounds, out var z))
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

    /// <summary>
    /// Deserializes the deprecated <c>BackgroundStyle</c> enum.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Graph3D.Shapes.BackgroundStyle", 0)]
    public class BackgroundStyleXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("This class is deprecated and no longer supported to serialize");
        // info.SetNodeContent(obj.ToString());
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(BackgroundStyle), val, true);
      }
    }

    #endregion Deprecated classes

    #region IRoutedPropertyReceiver Members

    /// <inheritdoc/>
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
