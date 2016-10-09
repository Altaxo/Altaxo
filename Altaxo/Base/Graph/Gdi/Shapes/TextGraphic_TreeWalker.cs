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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/* the Grammar used here is the following PEG grammar

<<Grammar Name="Altaxo_LabelV1">>

// root element: the first part is the regular parsed text, the second part is the rest which can not be interpreted properly
[1]^^MainSentence:  (EscSeq / WordSpanExt / Space)*  (WordSpanExt / EscChar / Space / '\\')*;

[2]^Sentence:       (EscSeq / WordSpan / Space)+;

[3]^SentenceNC:     (EscSeq / WordSpanNC / Space)+;

[4]^^WordSpanExt:   (Word / EscChar / ',' / ')')+;

[5]^^WordSpan:      (Word / EscChar / ',')+;

[6]^^WordSpanNC:    (Word / EscChar)+;

[7]^EscChar:        '\\\\' / '\\)' / '\\(';

[8]^EscSeq:   	    (EscSeq3 / EscSeq2 / EscSeq1);

[9]^Number:         [0-9]+ ('.' [0-9]+)?([eE][+-][0-9]+)?;

[10]Word:           [#x20-#x28#x2A-#x2B#x2D-#x5B#x5D-#xFFFF]+;

[11]^^Space:        '\t' / '\r\n' / '\n';

[12]^PositiveInteger: 	[0-9]+;

[13]^^EscSeq3:      ('\\L('\i PositiveInteger ',' PositiveInteger ',' PositiveInteger ')') /
                    ('\\%(' PositiveInteger ',' PositiveInteger ',' QuotedString ')');

[14]^^EscSeq2:      ( '\\' ( 'P'\i / 'F'\i / 'C'\i / '=' ) '(' SentenceNC ',' Sentence ')' ) /
                    ( '\\' ( 'L'\i                       ) '(' PositiveInteger ',' PositiveInteger ')' ) /
                    ( '\\' ( '%'                         ) '(' PositiveInteger ',' (PositiveInteger / QuotedString) ')' )
                    ;

[15]^^EscSeq1:      '\\' ('AB'\i / 'AD'\i / 'ID'\i / '+' / '-' /  '%' / '#' /  'B'\i / 'G'\i / 'I'\i / 'L'\i / 'N'\i / 'S'\i / 'U'\i / 'V'\i ) '(' Sentence ')';

[16]QuotedString:  '"' StringContent '"';

[17]^^StringContent: ( '\\'
                           ( 'u'([0-9A-Fa-f]{4}/FATAL<"4 hex digits expected">)
                           / ["\\/bfnrt]/FATAL<"illegal escape">
                           )
                        / [#x20-#x21#x23-#xFFFF]
                        )*	;

<</Grammar>>

*/

namespace Altaxo.Graph.Gdi.Shapes
{
	using Altaxo.Main.PegParser;
	using Drawing;

	public partial class TextGraphic : GraphicBase
	{
		#region Regex expressions

		private static Regex _regexIntArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*\)");
		private static Regex _regexIntIntArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\d+)\n*\)");
		private static Regex _regexIntQstrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*\""(?<argtwo>([^\\\""]*(\\\"")*(\\\\)*)+)\""\n*\)");
		private static Regex _regexIntStrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\w+)\n*\)");
		private static Regex _regexIntIntStrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\d+)\n*,\n*(?<argthree>\w+)\n*\)");

		// Be aware that double quote characters is in truth only one quote character, this is the syntax of a verbatim literal string
		private static Regex _regexIntIntQstrgArgument = new Regex(@"\G\(\n*(?<argone>\d+)\n*,\n*(?<argtwo>\d+)\n*,\n*\""(?<argthree>([^\\\""]*(\\\"")*(\\\\)*)+)\""\n*\)");

		#endregion Regex expressions

		private class TreeWalker
		{
			private string _sourceText;

			public TreeWalker(string sourceText)
			{
				_sourceText = sourceText;
			}

			public StructuralGlyph VisitTree(PegNode root, StyleContext context, double lineSpacingFactor, bool isFixedLineSpacing)
			{
				var rootGlyph = new VerticalStack();
				rootGlyph.Style = context;
				rootGlyph.LineSpacingFactor = lineSpacingFactor;
				rootGlyph.FixedLineSpacing = isFixedLineSpacing;

				var line = new GlyphLine();
				line.Style = context;

				rootGlyph.Add(line);

				if (null != root && null != root.child_)
					VisitNode(root.child_, context, line);

				return rootGlyph;
			}

			private StructuralGlyph VisitNode(PegNode node, StyleContext context, StructuralGlyph parent)
			{
				StructuralGlyph nextparent = parent;

				switch ((EAltaxo_LabelV1)node.id_)
				{
					case EAltaxo_LabelV1.WordSpan:
					case EAltaxo_LabelV1.WordSpanExt:
					case EAltaxo_LabelV1.WordSpanNC:
						HandleWordSpan(node, context, parent);
						break;

					case EAltaxo_LabelV1.Sentence:
					case EAltaxo_LabelV1.SentenceNC:
						HandleSentence(node, context, parent);
						break;

					case EAltaxo_LabelV1.Space:
						nextparent = HandleSpace(node, context, parent);
						break;

					case EAltaxo_LabelV1.EscSeq1:
						HandleEscSeq1(node, context, parent);
						break;

					case EAltaxo_LabelV1.EscSeq2:
						HandleEscSeq2(node, context, parent);
						break;

					case EAltaxo_LabelV1.EscSeq3:
						HandleEscSeq3(node, context, parent);
						break;
				}

				if (null != node.next_)
					nextparent = VisitNode(node.next_, context, nextparent);

				return nextparent;
			}

			private void HandleWordSpan(PegNode node, StyleContext context, StructuralGlyph parent)
			{
				int posBeg = node.match_.posBeg_;
				int posEnd = node.match_.posEnd_;
				var childNode = node.child_;

				string str = string.Empty;
				if (null == childNode) // no escape sequences
				{
					str = _sourceText.Substring(posBeg, posEnd - posBeg);
				}
				else // at least one child node (Esc seq)
				{
					int beg = posBeg;
					int end = childNode.match_.posBeg_;
					while (childNode != null)
					{
						str += _sourceText.Substring(beg, end - beg);
						str += _sourceText.Substring(childNode.match_.posBeg_ + 1, 1);
						beg = childNode.match_.posEnd_;
						childNode = childNode.next_;
						end = null != childNode ? childNode.match_.posBeg_ : posEnd;
					}
					str += _sourceText.Substring(beg, end - beg);
				}
				parent.Add(new TextGlyph(str, context));
			}

			private StructuralGlyph HandleSpace(PegNode node, StyleContext context, StructuralGlyph parent)
			{
				if (_sourceText[node.match_.posBeg_] == '\t')
				{
					HandleTab(parent);
					return parent;
				}
				else // newline
				{
					return HandleNewline(parent, context);
				}
			}

			private void HandleTab(StructuralGlyph parent)
			{
				parent.Add(new TabGlpyh());
			}

			private StructuralGlyph HandleNewline(StructuralGlyph parent, StyleContext context)
			{
				StructuralGlyph newcontext;

				if (parent is GlyphLine) // normal case
				{
					if (parent.Parent is VerticalStack)
					{
						newcontext = new GlyphLine();
						newcontext.Style = context;
						parent.Parent.Add(newcontext);
					}
					else // parent.Parent is not a VerticalStack
					{
						var vertStack = new VerticalStack();
						parent.Parent.Exchange(parent, vertStack);
						vertStack.Add(parent);
						newcontext = new GlyphLine();
						newcontext.Style = context;
						vertStack.Add(newcontext);
					}
				}
				else
				{
					throw new NotImplementedException();
				}
				return newcontext;
			}

			private void HandleSentence(PegNode node, StyleContext context, StructuralGlyph parent)
			{
				var line = new GlyphLine();
				parent.Add(line);
				if (node.child_ != null)
					VisitNode(node.child_, context, line);
			}

			private void HandleEscSeq1(PegNode node, StyleContext context, StructuralGlyph parent)
			{
				int posBeg = node.match_.posBeg_;
				var childNode = node.child_;

				if (childNode == null)
					throw new ArgumentNullException("childNode");

				string escHeader = _sourceText.Substring(posBeg, childNode.match_.posBeg_ - posBeg);

				switch (escHeader.ToLowerInvariant())
				{
					case @"\id(":
						{
							const string DefPropertyHead = "$Property[\"";
							const string DefPropertyTail = "\"]";

							string s = GetText(childNode).Trim();
							if (s == "$DI")
							{
								parent.Add(new DocumentIdentifier(context));
							}
							else if (s.StartsWith(DefPropertyHead) && s.EndsWith(DefPropertyTail))
							{
								string propertyName = s.Substring(DefPropertyHead.Length, s.Length - DefPropertyHead.Length - DefPropertyTail.Length);
								if (!string.IsNullOrEmpty(propertyName))
									parent.Add(new ValueOfProperty(context, propertyName));
							}
						}
						break;

					case @"\g(":
						{
							var newContext = context.Clone();
							newContext.SetFont(context.FontId.WithFamily("Symbol"));
							VisitNode(childNode, newContext, parent);
						}
						break;

					case @"\i(":
						{
							var newContext = context.Clone();
							newContext.MergeFontStyle(FontXStyle.Italic);
							VisitNode(childNode, newContext, parent);
						}
						break;

					case @"\b(":
						{
							var newContext = context.Clone();
							newContext.MergeFontStyle(FontXStyle.Bold);
							VisitNode(childNode, newContext, parent);
						}
						break;

					case @"\u(":
						{
							var newContext = context.Clone();
							newContext.MergeFontStyle(FontXStyle.Underline);
							VisitNode(childNode, newContext, parent);
						}
						break;

					case @"\s(":
						{
							var newContext = context.Clone();
							newContext.MergeFontStyle(FontXStyle.Strikeout);
							VisitNode(childNode, newContext, parent);
						}
						break;

					case @"\n(":
						{
							var newContext = context.Clone();
							newContext.SetFontStyle(FontXStyle.Regular);
							VisitNode(childNode, newContext, parent);
						}
						break;

					case @"\+(":
						{
							var newParent = new Superscript();
							newParent.Style = context;
							parent.Add(newParent);

							var newContext = context.Clone();
							newContext.ScaleFont(0.65);
							VisitNode(childNode, newContext, newParent);
						}
						break;

					case @"\-(":
						{
							var newParent = new Subscript();
							newParent.Style = context;
							parent.Add(newParent);

							var newContext = context.Clone();
							newContext.ScaleFont(0.65);
							VisitNode(childNode, newContext, newParent);
						}
						break;

					case @"\l(":
						{
							string s = GetText(childNode);
							int plotNumber;
							if (int.TryParse(s, out plotNumber))
							{
								parent.Add(new PlotSymbol(context, plotNumber));
							}
						}
						break;

					case @"\%(":
						{
							string s = GetText(childNode);
							int plotNumber;
							if (int.TryParse(s, out plotNumber))
							{
								parent.Add(new PlotName(context, plotNumber));
							}
						}
						break;

					case @"\ad(":
						{
							var newParent = new DotOverGlyph();
							newParent.Style = context;
							parent.Add(newParent);
							VisitNode(childNode, context, newParent);
						}
						break;

					case @"\ab(":
						{
							var newParent = new BarOverGlyph();
							newParent.Style = context;
							parent.Add(newParent);
							VisitNode(childNode, context, newParent);
						}
						break;
				}
			}

			private void HandleEscSeq2(PegNode node, StyleContext context, StructuralGlyph parent)
			{
				int posBeg = node.match_.posBeg_;
				var childNode = node.child_;

				if (childNode == null)
					throw new ArgumentNullException("childNode");

				string escHeader = _sourceText.Substring(posBeg, childNode.match_.posBeg_ - posBeg);

				switch (escHeader.ToLowerInvariant())
				{
					case @"\=(":
						{
							var newParent = new SubSuperScript();
							newParent.Style = context;
							parent.Add(newParent);

							var newContext = context.Clone();
							newContext.ScaleFont(0.65);
							VisitNode(childNode, newContext, newParent);
						}
						break;

					case @"\p(":
						{
							double val;
							string s1 = GetText(childNode).Trim();
							var newContext = context.Clone();
							string numberString;
							Altaxo.Serialization.LengthUnit lengthUnit;

							if (s1.EndsWith("%"))
							{
								numberString = s1.Substring(0, s1.Length - 1);
								if (double.TryParse(numberString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out val))
								{
									newContext.BaseFontId = context.BaseFontId.WithSize(context.BaseFontId.Size * val / 100);
									newContext.ScaleFont(val / 100);
								}
							}
							else if (Altaxo.Serialization.LengthUnit.TryParse(s1, out lengthUnit, out numberString) &&
								double.TryParse(numberString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out val)
								)
							{
								double newSize = val * (double)(lengthUnit.UnitInMeter / Altaxo.Serialization.LengthUnit.Point.UnitInMeter);
								newContext.BaseFontId = context.BaseFontId.WithSize(newSize);
								newContext.FontId = context.FontId.WithSize(newSize);
							}
							else if (double.TryParse(s1, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out val)
								)
							{
								double newSize = val;
								newContext.BaseFontId = context.BaseFontId.WithSize(newSize);
								newContext.FontId = context.FontId.WithSize(newSize);
							}
							VisitNode(childNode.next_, newContext, parent);
						}
						break;

					case @"\c(":
						{
							string s1 = GetText(childNode).Trim();
							var newContext = context.Clone();
							var conv = new ColorConverter();

							try
							{
								object result = conv.ConvertFromInvariantString(s1);
								newContext.brush = new SolidBrush((Color)result);
							}
							catch (Exception)
							{
							}

							VisitNode(childNode.next_, newContext, parent);
						}
						break;

					case @"\l(":
						{
							string s1 = GetText(childNode);
							string s2 = GetText(childNode.next_);
							int plotNumber, plotLayer;
							if (int.TryParse(s1, out plotLayer) && int.TryParse(s2, out plotNumber))
							{
								parent.Add(new PlotSymbol(context, plotNumber, plotLayer));
							}
						}
						break;

					case @"\%(":
						{
							string s1 = GetText(childNode);
							string s2 = GetText(childNode.next_);
							int plotNumber, plotLayer;
							if (int.TryParse(s1, out plotLayer) && int.TryParse(s2, out plotNumber))
							{
								parent.Add(new PlotName(context, plotNumber, plotLayer));
							}
							else if (int.TryParse(s1, out plotNumber))
							{
								var label = new PlotName(context, plotNumber);
								label.SetPropertyColumnName(s2);
								parent.Add(label);
							}
						}
						break;
				}
			}

			private void HandleEscSeq3(PegNode node, StyleContext context, StructuralGlyph parent)
			{
				int posBeg = node.match_.posBeg_;
				var childNode = node.child_;

				if (childNode == null)
					throw new ArgumentNullException("childNode");

				string escHeader = _sourceText.Substring(posBeg, childNode.match_.posBeg_ - posBeg);

				switch (escHeader.ToLowerInvariant())
				{
					case @"\%(":
						{
							string s1 = GetText(childNode);
							string s2 = GetText(childNode.next_);
							string s3 = GetText(childNode.next_.next_);
							int plotNumber, plotLayer;
							if (int.TryParse(s1, out plotLayer) && int.TryParse(s2, out plotNumber))
							{
								var label = new PlotName(context, plotNumber, plotLayer);
								label.SetPropertyColumnName(s3);
								parent.Add(label);
							}
						}
						break;
				}
			}

			private string GetText(PegNode node)
			{
				return _sourceText.Substring(node.match_.posBeg_, node.match_.Length);
			}
		} // end class TreeWalker
	}
}