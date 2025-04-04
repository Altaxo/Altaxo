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

/* created on 05.02.2024 (file Altaxo_LabelV3_PEG.txt)
 * Instructions:
 * - Use the program "PEG Explorer" from here: https://www.codeproject.com/Articles/29713/Parsing-Expression-Grammar-Support-for-C-3-0-Part
 * - Save the grammar below in a file.
 * - Load the file in PEG Explorer.
 * - in the combobox "Grammar", choose "PEG Parser Generator"
 * - Click on the button "Parse"
 * - if parsing was successful, click button "C#" to generate C# file
 * - copy the enum and the class contained in this generated file to the file here
 * - the Grammar used here is the following PEG grammar (save it to a file, then load it into PEG Explorer):

<<Grammar Name="Altaxo_LabelV3">>

// root element: the first part is the regular parsed text, the second part is the rest which can not be interpreted properly
[1]^^MainSentence:  (EscSeq / WordSpanExt / Space)*  (WordSpanExt / EscChar / Space / '\\')*;

[2]^Sentence:       (EscSeq / WordSpan / Space)+;

[3]^SentenceNC:     (EscSeq / WordSpanNC / Space)+;

[4]^^WordSpanExt:   (Word / EscChar / ',' / ')')+;

[5]^^WordSpan:      (Word / EscChar / ',')+;

[6]^^WordSpanNC:    (Word / EscChar)+;

[7]^EscChar:        '\\\\' / '\\)' / '\\(';

[8]^EscSeq:   	    (EscSeq4 / EscSeq3 / EscSeq2 / EscSeq1);

[9]^Number:         [0-9]+ ('.' [0-9]+)?([eE][+-][0-9]+)?;

[10]Word:           [#x20-#x28#x2A-#x2B#x2D-#x5B#x5D-#xFFFF]+;

[11]^^Space:        '\t' / '\r\n' / '\n'; 

[12]^PositiveInteger: 	[0-9]+;

[13]^^EscSeq4:      ('\\%(' PositiveInteger ',' PositiveInteger ',' QuotedString ',' QuotedString ')');

[14]^^EscSeq3:      ('\\L('\i PositiveInteger ',' PositiveInteger ',' PositiveInteger ')') /
                    ('\\%(' PositiveInteger ',' PositiveInteger ',' QuotedString ')') /
		                ('\\%(' PositiveInteger ',' QuotedString ',' QuotedString ')');

[15]^^EscSeq2:      ( '\\' ( 'P'\i / 'F'\i / 'C'\i / '=' ) '(' SentenceNC ',' Sentence ')' ) /
                    ( '\\' ( 'L'\i                       ) '(' PositiveInteger ',' PositiveInteger ')' ) /
                    ( '\\' ( '%'                         ) '(' PositiveInteger ',' (PositiveInteger / QuotedString) ')' ) 
                    ;

[16]^^EscSeq1:      '\\' ('AB'\i / 'AD'\i / 'ID'\i / '+' / '-' /  '%' / '#' /  'B'\i / 'G'\i / 'I'\i / 'L'\i / 'N'\i / 'S'\i / 'U'\i / 'V'\i ) '(' Sentence ')';

[17]QuotedString:  '"' StringContent '"';

[18]^^StringContent: ( '\\' 
                           ( 'u'([0-9A-Fa-f]{4}/FATAL<"4 hex digits expected">)
                           / ["\\/bfnrt]/FATAL<"illegal escape"> 
                           ) 
                        / [#x20-#x21#x23-#xFFFF]
                        )*	;

<</Grammar>>


*/
#nullable enable
using System;
using System.IO;
using Altaxo.Main.PegParser;

namespace Altaxo.Graph.Gdi.Shapes
{
  enum EAltaxo_LabelV1
  {
    MainSentence = 1, Sentence = 2, SentenceNC = 3, WordSpanExt = 4,
    WordSpan = 5, WordSpanNC = 6, EscChar = 7, EscSeq = 8, Number = 9,
    Word = 10, Space = 11, PositiveInteger = 12, EscSeq4 = 13, EscSeq3 = 14,
    EscSeq2 = 15, EscSeq1 = 16, QuotedString = 17, StringContent = 18
  };
  class Altaxo_LabelV1 : PegCharParser
  {

    #region Input Properties
    public static EncodingClass encodingClass = EncodingClass.ascii;
    public static UnicodeDetection unicodeDetection = UnicodeDetection.notApplicable;
    #endregion Input Properties
    #region Constructors
    public Altaxo_LabelV1()
        : base()
    {

    }
    public Altaxo_LabelV1(string src, TextWriter FerrOut)
  : base(src, FerrOut)
    {

    }
    #endregion Constructors
    #region Overrides
    public override string GetRuleNameFromId(int id)
    {
      try
      {
        EAltaxo_LabelV1 ruleEnum = (EAltaxo_LabelV1)id;
        string s = ruleEnum.ToString();
        int val;
        if (int.TryParse(s, out val))
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
    public override void GetProperties(out EncodingClass encoding, out UnicodeDetection detection)
    {
      encoding = encodingClass;
      detection = unicodeDetection;
    }
    #endregion Overrides
    #region Grammar Rules
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
    public bool Sentence()    /*[2]^Sentence:       (EscSeq / WordSpan / Space)+;*/
    {

      return TreeAST((int)EAltaxo_LabelV1.Sentence, () =>
           PlusRepeat(() => EscSeq() || WordSpan() || Space()));
    }
    public bool SentenceNC()    /*[3]^SentenceNC:     (EscSeq / WordSpanNC / Space)+;*/
    {

      return TreeAST((int)EAltaxo_LabelV1.SentenceNC, () =>
           PlusRepeat(() => EscSeq() || WordSpanNC() || Space()));
    }
    public bool WordSpanExt()    /*[4]^^WordSpanExt:   (Word / EscChar / ',' / ')')+;*/
    {

      return TreeNT((int)EAltaxo_LabelV1.WordSpanExt, () =>
           PlusRepeat(() =>
                 Word() || EscChar() || Char(',') || Char(')')));
    }
    public bool WordSpan()    /*[5]^^WordSpan:      (Word / EscChar / ',')+;*/
    {

      return TreeNT((int)EAltaxo_LabelV1.WordSpan, () =>
           PlusRepeat(() => Word() || EscChar() || Char(',')));
    }
    public bool WordSpanNC()    /*[6]^^WordSpanNC:    (Word / EscChar)+;*/
    {

      return TreeNT((int)EAltaxo_LabelV1.WordSpanNC, () =>
           PlusRepeat(() => Word() || EscChar()));
    }
    public bool EscChar()    /*[7]^EscChar:        '\\\\' / '\\)' / '\\(';*/
    {

      return TreeAST((int)EAltaxo_LabelV1.EscChar, () =>
               Char('\\', '\\') || Char('\\', ')') || Char('\\', '('));
    }
    public bool EscSeq()    /*[8]^EscSeq:   	    (EscSeq4 / EscSeq3 / EscSeq2 / EscSeq1);*/
    {

      return TreeAST((int)EAltaxo_LabelV1.EscSeq, () =>
               EscSeq4() || EscSeq3() || EscSeq2() || EscSeq1());
    }
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
    public bool Word()    /*[10]Word:           [#x20-#x28#x2A-#x2B#x2D-#x5B#x5D-#xFFFF]+;*/
    {

      return PlusRepeat(() =>
             In('\u0020', '\u0028', '\u002a', '\u002b', '\u002d', '\u005b', '\u005d', '\uffff'));
    }
    public bool Space()    /*[11]^^Space:        '\t' / '\r\n' / '\n';*/
    {

      return TreeNT((int)EAltaxo_LabelV1.Space, () =>
               Char('\t') || Char('\r', '\n') || Char('\n'));
    }
    public bool PositiveInteger()    /*[12]^PositiveInteger: 	[0-9]+;*/
    {

      return TreeAST((int)EAltaxo_LabelV1.PositiveInteger, () =>
           PlusRepeat(() => In('0', '9')));
    }
    public bool EscSeq4()    /*[13]^^EscSeq4:      ('\\%(' PositiveInteger ',' PositiveInteger ',' QuotedString ',' QuotedString ')');*/
    {

      return TreeNT((int)EAltaxo_LabelV1.EscSeq4, () =>
           And(() =>
                Char('\\', '%', '(')
             && PositiveInteger()
             && Char(',')
             && PositiveInteger()
             && Char(',')
             && QuotedString()
             && Char(',')
             && QuotedString()
             && Char(')')));
    }
    public bool EscSeq3()    /*[14]^^EscSeq3:      ('\\L('\i PositiveInteger ',' PositiveInteger ',' PositiveInteger ')') /
                    ('\\%(' PositiveInteger ',' PositiveInteger ',' QuotedString ')') /
		    ('\\%(' PositiveInteger ',' QuotedString ',' QuotedString ')');*/
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
                 && Char(')'))
             || And(() =>
                    Char('\\', '%', '(')
                 && PositiveInteger()
                 && Char(',')
                 && QuotedString()
                 && Char(',')
                 && QuotedString()
                 && Char(')')));
    }
    public bool EscSeq2()    /*[15]^^EscSeq2:      ( '\\' ( 'P'\i / 'F'\i / 'C'\i / '=' ) '(' SentenceNC ',' Sentence ')' ) /
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
    public bool EscSeq1()    /*[16]^^EscSeq1:      '\\' ('AB'\i / 'AD'\i / 'ID'\i / '+' / '-' /  '%' / '#' /  'B'\i / 'G'\i / 'I'\i / 'L'\i / 'N'\i / 'S'\i / 'U'\i / 'V'\i ) '(' Sentence ')';*/
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
    public bool QuotedString()    /*[17]QuotedString:  '"' StringContent '"';*/
    {

      return And(() => Char('"') && StringContent() && Char('"'));
    }
    public bool StringContent()    /*[18]^^StringContent: ( '\\' 
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
