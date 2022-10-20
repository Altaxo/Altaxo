#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis.Classification;

namespace Altaxo.Gui.CodeEditing.SemanticHighlighting
{
  public class TextHighlightingColorsAltaxoStyle : ISemanticHighlightingColors
  {
    public static TextHighlightingColorsAltaxoStyle Instance { get; } = new TextHighlightingColorsAltaxoStyle();

    public HighlightingColor DefaultColor { get { return DefaultTextColor; } }

    private static readonly HighlightingColor DefaultTextColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Black) }.AsFrozen();
    private static readonly HighlightingColor ValueTypeColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Teal), FontWeight = System.Windows.FontWeights.Bold }.AsFrozen();
    private static readonly HighlightingColor ReferenceTypeColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Teal) }.AsFrozen();
    private static readonly HighlightingColor CommentColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Green) }.AsFrozen();
    private static readonly HighlightingColor XmlCommentColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Gray) }.AsFrozen();
    private static readonly HighlightingColor KeywordColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Blue), FontWeight = System.Windows.FontWeights.Bold }.AsFrozen();
    private static readonly HighlightingColor PreprocessorKeywordColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Green) }.AsFrozen();
    private static readonly HighlightingColor StringColor = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.DarkViolet) }.AsFrozen();

    private static readonly ImmutableDictionary<string, HighlightingColor> _map = new Dictionary<string, HighlightingColor>
    {
      [ClassificationTypeNames.Identifier] = DefaultTextColor,
      [ClassificationTypeNames.NumericLiteral] = DefaultTextColor,
      [ClassificationTypeNames.Operator] = DefaultTextColor,
      [ClassificationTypeNames.Keyword] = KeywordColor,
      [ClassificationTypeNames.ClassName] = ReferenceTypeColor,
      [ClassificationTypeNames.StructName] = ValueTypeColor,
      [ClassificationTypeNames.InterfaceName] = ReferenceTypeColor,
      [ClassificationTypeNames.DelegateName] = ReferenceTypeColor,
      [ClassificationTypeNames.EnumName] = ValueTypeColor,
      [ClassificationTypeNames.ModuleName] = ReferenceTypeColor,
      [ClassificationTypeNames.TypeParameterName] = ReferenceTypeColor,
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
