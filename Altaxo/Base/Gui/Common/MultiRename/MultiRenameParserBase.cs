#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

/* created on 06.09.2011 14:00:27 from peg generator V1.0 using 'Altaxo_MultiRename_PEG.txt' as input*/

/* The following PEG grammar was used to generate this file with the PEG grammar explorer
<<Grammar Name="Altaxo_MultiRename">>

// root element: the first part is the regular parsed text, the second part is the rest which can not be interpreted properly
[1]^^MainSentence:  (Template / EscBracket / NormalChar)*;

[2]^Template: 	    (IntegerTemplate / StringTemplate / ArrayTemplate / DateTimeTemplate);

[3]NormalChar:     [#x20-#xFFFF];

[4]NonNegativeInteger:	[0-9]+;

[5]PositiveInteger:	[1-9][0-9]*;

[6]NegativeInteger:	'-'[1-9][0-9]*;

[7]Integer:		PositiveInteger / NegativeInteger / ('0');

[8]QuotedString:  '"' StringContent '"';

[9]^IntArg1st:		Integer;

[10]^IntArg2nd:		Integer;

[11]^IntArgOnly:	Integer;

[12]^IntArgNumberOfDigits:	PositiveInteger / ('0');

[13]^^StringContent: ( '\\'
                           ( 	'u'([0-9A-Fa-f]{4}) /
				["\\/]
                           )
                        / [#x20-#x21#x23-#xFFFF]
                        )*	;

[14]^EscBracket:		'[[';

[15]^ArraySeparator:	QuotedString;

[16]^DateTimeArguments:	QuotedString;

[17]^DateTimeKind:	('U'\i)/('L'\i);

[18]^^IntegerTemplate:	('[' IntegerTChar IntArgNumberOfDigits? (',' IntArg1st ',' IntArg2nd)? ']');

[19]^^StringTemplate:	('[' StringTChar (	(IntArg1st ',' IntArg2nd)/
						(IntArg1st ',' ) /
						(',' IntArg2nd) /
						(IntArgOnly?)
					 )  ']');

[20]^^ArrayTemplate:	('[' ArrayTChar	ArraySeparator?	(	(IntArg1st ',' IntArg2nd) /
								(IntArg1st ',' ) /
								(',' IntArg2nd) /
								(IntArgOnly?)
							)  ']');

[21]^^DateTimeTemplate:	('[' DateTimeTChar (DateTimeArguments)? (',' DateTimeKind)? ']');

[22]^IntegerTChar:  ('C') / ('KK');

[23]^StringTChar: ('N') / ('MM');

[24]^ArrayTChar: ('A');

[25]^DateTimeTChar: ('T');

<</Grammar>>

*/

// Please note that whenever you generate a new version of this file with the PEG grammar explorer, the following things have to be modified:
// 1. using Altaxo.Main.PegParser instead of using Peg.Base
// 2. The namespace must be changed to Altaxo.Gui.Common.MultiRename
// 3. The class name have to be changed to MultiRenameParserBase and must be made public
// 4. The functions IntegerTChar, StringTChar, ArrayTChar, and DateTimeTChar must be made virtual, so that they can be overridden
// 5. The line endings must be corrected and the file must be formatted

using Altaxo.Main.PegParser;
using System;
using System.IO;
using System.Text;

namespace Altaxo.Gui.Common.MultiRename
{
  internal enum EAltaxo_MultiRename
  {
    MainSentence = 1, Template = 2, NormalChar = 3, NonNegativeInteger = 4,
    PositiveInteger = 5, NegativeInteger = 6, Integer = 7, QuotedString = 8,
    IntArg1st = 9, IntArg2nd = 10, IntArgOnly = 11, IntArgNumberOfDigits = 12,
    StringContent = 13, EscBracket = 14, ArraySeparator = 15, DateTimeArguments = 16,
    DateTimeKind = 17, IntegerTemplate = 18, StringTemplate = 19, ArrayTemplate = 20,
    DateTimeTemplate = 21, IntegerTChar = 22, StringTChar = 23, ArrayTChar = 24,
    DateTimeTChar = 25
  };

  public class MultiRenameParserBase : PegCharParser
  {
    #region Input Properties

    public static EncodingClass encodingClass = EncodingClass.ascii;
    public static UnicodeDetection unicodeDetection = UnicodeDetection.notApplicable;

    #endregion Input Properties

    #region Constructors

    public MultiRenameParserBase()
      : base()
    {
    }

    public MultiRenameParserBase(string src, TextWriter FerrOut)
      : base(src, FerrOut)
    {
    }

    #endregion Constructors

    #region Overrides

    public override string GetRuleNameFromId(int id)
    {
      try
      {
        EAltaxo_MultiRename ruleEnum = (EAltaxo_MultiRename)id;
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

    public bool MainSentence()    /*[1]^^MainSentence:  (Template / EscBracket / NormalChar)*;*/
    {
      return TreeNT((int)EAltaxo_MultiRename.MainSentence, () =>
           OptRepeat(() =>
                 Template() || EscBracket() || NormalChar()));
    }

    public bool Template()    /*[2]^Template: 	    (IntegerTemplate / StringTemplate / ArrayTemplate / DateTimeTemplate);*/
    {
      return TreeAST((int)EAltaxo_MultiRename.Template, () =>

                IntegerTemplate()
             || StringTemplate()
             || ArrayTemplate()
             || DateTimeTemplate());
    }

    public bool NormalChar()    /*[3]NormalChar:     [#x20-#xFFFF];*/
    {
      return In('\u0020', '\uffff');
    }

    public bool NonNegativeInteger()    /*[4]NonNegativeInteger:	[0-9]+;*/
    {
      return PlusRepeat(() => In('0', '9'));
    }

    public bool PositiveInteger()    /*[5]PositiveInteger:	[1-9][0-9]*;*/
    {
      return And(() => In('1', '9') && OptRepeat(() => In('0', '9')));
    }

    public bool NegativeInteger()    /*[6]NegativeInteger:	'-'[1-9][0-9]*;*/
    {
      return And(() =>
                Char('-')
             && In('1', '9')
             && OptRepeat(() => In('0', '9')));
    }

    public bool Integer()    /*[7]Integer:		PositiveInteger / NegativeInteger / ('0');*/
    {
      return PositiveInteger() || NegativeInteger() || Char('0');
    }

    public bool QuotedString()    /*[8]QuotedString:  '"' StringContent '"';*/
    {
      return And(() => Char('"') && StringContent() && Char('"'));
    }

    public bool IntArg1st()    /*[9]^IntArg1st:		Integer;*/
    {
      return TreeAST((int)EAltaxo_MultiRename.IntArg1st, () =>
           Integer());
    }

    public bool IntArg2nd()    /*[10]^IntArg2nd:		Integer;*/
    {
      return TreeAST((int)EAltaxo_MultiRename.IntArg2nd, () =>
           Integer());
    }

    public bool IntArgOnly()    /*[11]^IntArgOnly:	Integer;*/
    {
      return TreeAST((int)EAltaxo_MultiRename.IntArgOnly, () =>
           Integer());
    }

    public bool IntArgNumberOfDigits()    /*[12]^IntArgNumberOfDigits:	PositiveInteger / ('0');*/
    {
      return TreeAST((int)EAltaxo_MultiRename.IntArgNumberOfDigits, () =>
               PositiveInteger() || Char('0'));
    }

    public bool StringContent()    /*[13]^^StringContent: ( '\\'
                           ( 	'u'([0-9A-Fa-f]{4}) /
				["\\/]
                           )
                        / [#x20-#x21#x23-#xFFFF]
                        )*	;*/
    {
      return TreeNT((int)EAltaxo_MultiRename.StringContent, () =>
           OptRepeat(() =>

                    And(() =>
                          Char('\\')
                       && (
                                  And(() =>
                                            Char('u')
                                         && ForRepeat(4, 4, () =>
                                                     In('0', '9', 'A', 'F', 'a', 'f')))
                               || OneOf("\"\\/")))
                 || In('\u0020', '\u0021', '\u0023', '\uffff')));
    }

    public bool EscBracket()    /*[14]^EscBracket:		'[[';*/
    {
      return TreeAST((int)EAltaxo_MultiRename.EscBracket, () =>
           Char('[', '['));
    }

    public bool ArraySeparator()    /*[15]^ArraySeparator:	QuotedString;*/
    {
      return TreeAST((int)EAltaxo_MultiRename.ArraySeparator, () =>
           QuotedString());
    }

    public bool DateTimeArguments()    /*[16]^DateTimeArguments:	QuotedString;*/
    {
      return TreeAST((int)EAltaxo_MultiRename.DateTimeArguments, () =>
           QuotedString());
    }

    public bool DateTimeKind()    /*[17]^DateTimeKind:	('U'\i)/('L'\i);*/
    {
      return TreeAST((int)EAltaxo_MultiRename.DateTimeKind, () =>
               IChar('U') || IChar('L'));
    }

    public bool IntegerTemplate()    /*[18]^^IntegerTemplate:	('[' IntegerTChar IntArgNumberOfDigits? (',' IntArg1st ',' IntArg2nd)? ']');*/
    {
      return TreeNT((int)EAltaxo_MultiRename.IntegerTemplate, () =>
           And(() =>
                Char('[')
             && IntegerTChar()
             && Option(() => IntArgNumberOfDigits())
             && Option(() =>
                 And(() =>
                          Char(',')
                       && IntArg1st()
                       && Char(',')
                       && IntArg2nd()))
             && Char(']')));
    }

    public bool StringTemplate()    /*[19]^^StringTemplate:	('[' StringTChar (	(IntArg1st ',' IntArg2nd)/
						(IntArg1st ',' ) /
						(',' IntArg2nd) /
						(IntArgOnly?)
					 )  ']');*/
    {
      return TreeNT((int)EAltaxo_MultiRename.StringTemplate, () =>
           And(() =>
                Char('[')
             && StringTChar()
             && (
                    And(() => IntArg1st() && Char(',') && IntArg2nd())
                 || And(() => IntArg1st() && Char(','))
                 || And(() => Char(',') && IntArg2nd())
                 || Option(() => IntArgOnly()))
             && Char(']')));
    }

    public bool ArrayTemplate()    /*[20]^^ArrayTemplate:	('[' ArrayTChar	ArraySeparator?	(	(IntArg1st ',' IntArg2nd) /
								(IntArg1st ',' ) /
								(',' IntArg2nd) /
								(IntArgOnly?)
							)  ']');*/
    {
      return TreeNT((int)EAltaxo_MultiRename.ArrayTemplate, () =>
           And(() =>
                Char('[')
             && ArrayTChar()
             && Option(() => ArraySeparator())
             && (
                    And(() => IntArg1st() && Char(',') && IntArg2nd())
                 || And(() => IntArg1st() && Char(','))
                 || And(() => Char(',') && IntArg2nd())
                 || Option(() => IntArgOnly()))
             && Char(']')));
    }

    public bool DateTimeTemplate()    /*[21]^^DateTimeTemplate:	('[' DateTimeTChar (DateTimeArguments)? (',' DateTimeKind)? ']');*/
    {
      return TreeNT((int)EAltaxo_MultiRename.DateTimeTemplate, () =>
           And(() =>
                Char('[')
             && DateTimeTChar()
             && Option(() => DateTimeArguments())
             && Option(() => And(() => Char(',') && DateTimeKind()))
             && Char(']')));
    }

    public virtual bool IntegerTChar()    /*[21]^IntegerTChar:  ('C') / ('KK');*/
    {
      return TreeAST((int)EAltaxo_MultiRename.IntegerTChar, () =>
               Char('C') || Char('K', 'K'));
    }

    public virtual bool StringTChar()    /*[22]^StringTChar: ('N') / ('MM');*/
    {
      return TreeAST((int)EAltaxo_MultiRename.StringTChar, () =>
               Char('N') || Char('M', 'M'));
    }

    public virtual bool ArrayTChar()    /*[23]^ArrayTChar: ('A');*/
    {
      return TreeAST((int)EAltaxo_MultiRename.ArrayTChar, () =>
           Char('A'));
    }

    public virtual bool DateTimeTChar()    /*[24]^DateTimeTChar: ('T');*/
    {
      return TreeAST((int)EAltaxo_MultiRename.DateTimeTChar, () =>
           Char('T'));
    }

    #endregion Grammar Rules
  }
}
