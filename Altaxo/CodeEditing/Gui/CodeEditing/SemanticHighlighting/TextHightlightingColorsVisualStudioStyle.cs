// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Windows, ClassificationHighlightColors.cs

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis.Classification;

namespace Altaxo.Gui.CodeEditing.SemanticHighlighting
{
  public class TextHighlightingColorsVisualStudioStyle : ISemanticHighlightingColors
  {
    public static TextHighlightingColorsVisualStudioStyle Instance { get; } = new TextHighlightingColorsVisualStudioStyle();

    public HighlightingColor DefaultColor { get { return DefaultTextColor; } }

    private static readonly HighlightingColor DefaultTextColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Black) }.AsFrozen();
    private static readonly HighlightingColor TypeColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Teal) }.AsFrozen();
    private static readonly HighlightingColor CommentColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Green) }.AsFrozen();
    private static readonly HighlightingColor XmlCommentColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Gray) }.AsFrozen();
    private static readonly HighlightingColor KeywordColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Blue) }.AsFrozen();
    private static readonly HighlightingColor PreprocessorKeywordColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Gray) }.AsFrozen();
    private static readonly HighlightingColor StringColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Maroon) }.AsFrozen();

    private static readonly ImmutableDictionary<string, HighlightingColor> _map = new Dictionary<string, HighlightingColor>
    {
      [ClassificationTypeNames.Identifier] = DefaultTextColor,
      [ClassificationTypeNames.NumericLiteral] = DefaultTextColor,
      [ClassificationTypeNames.Operator] = DefaultTextColor,
      [ClassificationTypeNames.Keyword] = KeywordColor,
      [ClassificationTypeNames.ClassName] = TypeColor,
      [ClassificationTypeNames.StructName] = TypeColor,
      [ClassificationTypeNames.InterfaceName] = TypeColor,
      [ClassificationTypeNames.DelegateName] = TypeColor,
      [ClassificationTypeNames.EnumName] = TypeColor,
      [ClassificationTypeNames.ModuleName] = TypeColor,
      [ClassificationTypeNames.TypeParameterName] = TypeColor,
      [ClassificationTypeNames.Comment] = CommentColor,
      [ClassificationTypeNames.XmlDocCommentAttributeName] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentAttributeQuotes] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentAttributeValue] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentCDataSection] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentComment] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentDelimiter] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentEntityReference] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentName] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentProcessingInstruction] = XmlCommentColor,
      [ClassificationTypeNames.XmlDocCommentText] = CommentColor,
      [ClassificationTypeNames.Keyword] = KeywordColor,
      [ClassificationTypeNames.PreprocessorKeyword] = PreprocessorKeywordColor,
      [ClassificationTypeNames.StringLiteral] = StringColor,
      [ClassificationTypeNames.VerbatimStringLiteral] = StringColor
    }.ToImmutableDictionary();

    public HighlightingColor GetColor(string classificationTypeName)
    {
      _map.TryGetValue(classificationTypeName, out HighlightingColor color);
      return color ?? DefaultColor;
    }
  }
}
