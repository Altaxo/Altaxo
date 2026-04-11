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

/* created on 9.9.2009 20:57:49 from peg generator V1.0 using 'Altaxo_LabelV1_PEG.txt' as input*/

#nullable enable
using System;
using System.IO;
using System.Text;
using Altaxo.Main.PegParser;

namespace Altaxo.Graph.Graph3D.Shapes
{
  /// <summary>
  /// Enumerates the parser rules for the 3D Altaxo label grammar.
  /// </summary>
  internal enum EAltaxo_LabelV1
  {
    /// <summary>Main sentence rule.</summary>
    MainSentence = 1,
    /// <summary>Sentence rule.</summary>
    Sentence = 2,
    /// <summary>Sentence-without-comma rule.</summary>
    SentenceNC = 3,
    /// <summary>Extended word-span rule.</summary>
    WordSpanExt = 4,
    /// <summary>Word-span rule.</summary>
    WordSpan = 5,
    /// <summary>Word-span-without-comma rule.</summary>
    WordSpanNC = 6,
    /// <summary>Escaped-character rule.</summary>
    EscChar = 7,
    /// <summary>Escape-sequence rule.</summary>
    EscSeq = 8,
    /// <summary>Number rule.</summary>
    Number = 9,
    /// <summary>Word rule.</summary>
    Word = 10,
    /// <summary>Whitespace rule.</summary>
    Space = 11,
    /// <summary>Positive-integer rule.</summary>
    PositiveInteger = 12,
    /// <summary>Three-argument escape-sequence rule.</summary>
    EscSeq3 = 13,
    /// <summary>Two-argument escape-sequence rule.</summary>
    EscSeq2 = 14,
    /// <summary>One-argument escape-sequence rule.</summary>
    EscSeq1 = 15,
    /// <summary>Quoted-string rule.</summary>
    QuotedString = 16,
    /// <summary>Quoted-string-content rule.</summary>
    StringContent = 17
  };

  /// <summary>
  /// Parser for the 3D Altaxo label grammar.
  /// </summary>
  internal class Altaxo_LabelV1 : PegCharParser
  {
    #region Input Properties

    /// <summary>
    /// Gets the encoding class used by the parser.
    /// </summary>
    public static EncodingClass encodingClass = EncodingClass.ascii;

    /// <summary>
    /// Gets the Unicode detection mode used by the parser.
    /// </summary>
    public static UnicodeDetection unicodeDetection = UnicodeDetection.notApplicable;

    #endregion Input Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Altaxo_LabelV1"/> class.
    /// </summary>
    public Altaxo_LabelV1()
      : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Altaxo_LabelV1"/> class.
    /// </summary>
    /// <param name="src">The source text.</param>
    /// <param name="FerrOut">The error writer.</param>
    public Altaxo_LabelV1(string src, TextWriter FerrOut)
      : base(src, FerrOut)
    {
    }

    #endregion Constructors

    #region Overrides

    /// <inheritdoc/>
    public override string GetRuleNameFromId(int id)
    {
      try
      {
        var ruleEnum = (EAltaxo_LabelV1)id;
        string s = ruleEnum.ToString();
        if (int.TryParse(s, out var val))
        {
          return base.GetRuleNameFromId(id);
        }
        else
        {
          return s;
        }
      }
      catch (Exception)
      {
        return base.GetRuleNameFromId(id);
      }
    }

    /// <inheritdoc/>
    public override void GetProperties(out EncodingClass encoding, out UnicodeDetection detection)
    {
      encoding = encodingClass;
      detection = unicodeDetection;
    }

    #endregion Overrides

    #region Grammar Rules

    /// <summary>
    /// Parses the main sentence rule.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool MainSentence()    /*[1]^^MainSentence:  (EscSeq / WordSpanExt / Space)*  (WordSpanExt / EscChar / Space / '\\')*;*/
    {
      return TreeNT((int)EAltaxo_LabelV1.MainSentence, () =>
           And(() =>
                OptRepeat(() =>
                     EscSeq() || WordSpanExt() || Space())
             && OptRepeat(() =>

                          WordSpanExt()
                       || EscChar()
                       || Space()
                       || Char('\\'))));
    }

    /// <summary>
    /// Parses the sentence rule.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool Sentence()    /*[2]^Sentence:       (EscSeq / WordSpan / Space)+;*/
    {
      return TreeAST((int)EAltaxo_LabelV1.Sentence, () =>
           PlusRepeat(() => EscSeq() || WordSpan() || Space()));
    }

    /// <summary>
    /// Parses the sentence-without-comma rule.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool SentenceNC()    /*[3]^SentenceNC:     (EscSeq / WordSpanNC / Space)+;*/
    {
      return TreeAST((int)EAltaxo_LabelV1.SentenceNC, () =>
           PlusRepeat(() => EscSeq() || WordSpanNC() || Space()));
    }

    /// <summary>
    /// Parses the extended word-span rule.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool WordSpanExt()    /*[4]^^WordSpanExt:   (Word / EscChar / ',' / ')')+;*/
    {
      return TreeNT((int)EAltaxo_LabelV1.WordSpanExt, () =>
           PlusRepeat(() =>
                 Word() || EscChar() || Char(',') || Char(')')));
    }

    /// <summary>
    /// Parses the word-span rule.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool WordSpan()    /*[5]^^WordSpan:      (Word / EscChar / ',')+;*/
    {
      return TreeNT((int)EAltaxo_LabelV1.WordSpan, () =>
           PlusRepeat(() => Word() || EscChar() || Char(',')));
    }

    /// <summary>
    /// Parses the word-span-without-comma rule.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool WordSpanNC()    /*[6]^^WordSpanNC:    (Word / EscChar)+;*/
    {
      return TreeNT((int)EAltaxo_LabelV1.WordSpanNC, () =>
           PlusRepeat(() => Word() || EscChar()));
    }

    /// <summary>
    /// Parses an escaped character.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool EscChar()    /*[7]^EscChar:        '\\\\' / '\\)' / '\\(';*/
    {
      return TreeAST((int)EAltaxo_LabelV1.EscChar, () =>
               Char('\\', '\\') || Char('\\', ')') || Char('\\', '('));
    }

    /// <summary>
    /// Parses an escape sequence.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool EscSeq()    /*[8]^EscSeq:   	    (EscSeq3 / EscSeq2 / EscSeq1);*/
    {
      return TreeAST((int)EAltaxo_LabelV1.EscSeq, () =>
               EscSeq3() || EscSeq2() || EscSeq1());
    }

    /// <summary>
    /// Parses a number.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool Number()    /*[9]^Number:         [0-9]+ ('.' [0-9]+)?([eE][+-][0-9]+)?;*/
    {
      return TreeAST((int)EAltaxo_LabelV1.Number, () =>
           And(() =>
                PlusRepeat(() => In('0', '9'))
             && Option(() =>
                 And(() =>
                          Char('.')
                       && PlusRepeat(() => In('0', '9'))))
             && Option(() =>
                 And(() =>
                          OneOf("eE")
                       && OneOf("+-")
                       && PlusRepeat(() => In('0', '9'))))));
    }

    /// <summary>
    /// Parses a word.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool Word()    /*[10]Word:           [#x20-#x28#x2A-#x2B#x2D-#x5B#x5D-#xFFFF]+;*/
    {
      return PlusRepeat(() =>
             In('\u0020', '\u0028', '\u002a', '\u002b', '\u002d', '\u005b', '\u005d', '\uffff'));
    }

    /// <summary>
    /// Parses whitespace.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool Space()    /*[11]^^Space:        '\t' / '\r\n' / '\n';*/
    {
      return TreeNT((int)EAltaxo_LabelV1.Space, () =>
               Char('\t') || Char('\r', '\n') || Char('\n'));
    }

    /// <summary>
    /// Parses a positive integer.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool PositiveInteger()    /*[12]^PositiveInteger: 	[0-9]+;*/
    {
      return TreeAST((int)EAltaxo_LabelV1.PositiveInteger, () =>
           PlusRepeat(() => In('0', '9')));
    }

    /// <summary>
    /// Parses a three-argument escape sequence.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool EscSeq3()    /*[13]^^EscSeq3:      ('\\L('\i PositiveInteger ',' PositiveInteger ',' PositiveInteger ')') /
                    ('\\%(' PositiveInteger ',' PositiveInteger ',' QuotedString ')');*/
    {
      return TreeNT((int)EAltaxo_LabelV1.EscSeq3, () =>

                And(() =>
                    IChar('\\', 'L', '(')
                 && PositiveInteger()
                 && Char(',')
                 && PositiveInteger()
                 && Char(',')
                 && PositiveInteger()
                 && Char(')'))
             || And(() =>
                    Char('\\', '%', '(')
                 && PositiveInteger()
                 && Char(',')
                 && PositiveInteger()
                 && Char(',')
                 && QuotedString()
                 && Char(')')));
    }

    /// <summary>
    /// Parses a two-argument escape sequence.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool EscSeq2()    /*[14]^^EscSeq2:      ( '\\' ( 'P'\i / 'F'\i / 'C'\i / '=' ) '(' SentenceNC ',' Sentence ')' ) /
                    ( '\\' ( 'L'\i                       ) '(' PositiveInteger ',' PositiveInteger ')' ) /
                    ( '\\' ( '%'                         ) '(' PositiveInteger ',' (PositiveInteger / QuotedString) ')' )
                    ;*/
    {
      return TreeNT((int)EAltaxo_LabelV1.EscSeq2, () =>

                And(() =>
                    Char('\\')
                 && (
                          IChar('P')
                       || IChar('F')
                       || IChar('C')
                       || Char('='))
                 && Char('(')
                 && SentenceNC()
                 && Char(',')
                 && Sentence()
                 && Char(')'))
             || And(() =>
                    Char('\\')
                 && IChar('L')
                 && Char('(')
                 && PositiveInteger()
                 && Char(',')
                 && PositiveInteger()
                 && Char(')'))
             || And(() =>
                    Char('\\')
                 && Char('%')
                 && Char('(')
                 && PositiveInteger()
                 && Char(',')
                 && (PositiveInteger() || QuotedString())
                 && Char(')')));
    }

    /// <summary>
    /// Parses a one-argument escape sequence.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool EscSeq1()    /*[15]^^EscSeq1:      '\\' ('AB'\i / 'AD'\i / 'ID'\i / '+' / '-' /  '%' / '#' /  'B'\i / 'G'\i / 'I'\i / 'L'\i / 'N'\i / 'S'\i / 'U'\i / 'V'\i ) '(' Sentence ')';*/
    {
      return TreeNT((int)EAltaxo_LabelV1.EscSeq1, () =>
           And(() =>
                Char('\\')
             && (
                    IChar('A', 'B')
                 || IChar('A', 'D')
                 || IChar('I', 'D')
                 || Char('+')
                 || Char('-')
                 || Char('%')
                 || Char('#')
                 || IChar('B')
                 || IChar('G')
                 || IChar('I')
                 || IChar('L')
                 || IChar('N')
                 || IChar('S')
                 || IChar('U')
                 || IChar('V'))
             && Char('(')
             && Sentence()
             && Char(')')));
    }

    /// <summary>
    /// Parses a quoted string.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool QuotedString()    /*[16]QuotedString:  '"' StringContent '"';*/
    {
      return And(() => Char('"') && StringContent() && Char('"'));
    }

    /// <summary>
    /// Parses the content of a quoted string.
    /// </summary>
    /// <returns><see langword="true"/> if the rule matches; otherwise, <see langword="false"/>.</returns>
    public bool StringContent()    /*[17]^^StringContent: ( '\\'
                           ( 'u'([0-9A-Fa-f]{4}/FATAL<"4 hex digits expected">)
                           / ["\\/bfnrt]/FATAL<"illegal escape">
                           )
                        / [#x20-#x21#x23-#xFFFF]
                        )*	;*/
    {
      return TreeNT((int)EAltaxo_LabelV1.StringContent, () =>
           OptRepeat(() =>

                    And(() =>
                          Char('\\')
                       && (
                                  And(() =>
                                            Char('u')
                                         && (
                                                        ForRepeat(4, 4, () =>
                                                                   In('0', '9', 'A', 'F', 'a', 'f'))
                                                     || Fatal("4 hex digits expected")))
                               || OneOf(optimizedCharset0)
                               || Fatal("illegal escape")))
                 || In('\u0020', '\u0021', '\u0023', '\uffff')));
    }

    #endregion Grammar Rules

    #region Optimization Data

    /// <summary>
    /// Stores the optimized character set used for escape parsing.
    /// </summary>
    internal static OptimizedCharset optimizedCharset0;

    static Altaxo_LabelV1()
    {
      {
        char[] oneOfChars = new char[]    {'"','\\','/','b','f'
                                                  ,'n','r','t'};
        optimizedCharset0 = new OptimizedCharset(null, oneOfChars);
      }
    }

    #endregion Optimization Data
  }
}
