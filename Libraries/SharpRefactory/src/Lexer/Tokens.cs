// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpRefactory.Parser
{
	public sealed class Tokens
	{
		/*----- terminal classes -----*/
		public const int EOF                = 0;
		public const int Identifier         = 1;
		public const int Literal            = 2;
		
		/*----- special character -----*/
		public const int Assign             = 3;
		public const int Plus               = 4;
		public const int Minus              = 5;
		public const int Times              = 6;
		public const int Div                = 7;
		public const int Mod                = 8;
		
		public const int Colon              = 9;
		public const int Semicolon          = 10;
		public const int Question           = 11;
		public const int Comma              = 12;
		public const int Dot                = 13;
		
		public const int OpenCurlyBrace     = 14;
		public const int CloseCurlyBrace    = 15;
		
		public const int OpenSquareBracket  = 16;
		public const int CloseSquareBracket = 17;
		
		public const int OpenParenthesis    = 18;
		public const int CloseParenthesis   = 19;
		
		public const int GreaterThan        = 20;
		public const int LessThan           = 21;
		
		public const int Not                = 22;
		public const int LogicalAnd         = 23;
		public const int LogicalOr          = 24;
		
		public const int BitwiseComplement  = 25;
		public const int BitwiseAnd         = 26;
		public const int BitwiseOr          = 27;
		public const int Xor                = 28;
		
		/*----- special character sequences -----*/
		public const int Increment          = 29;
		public const int Decrement          = 30;
		public const int Equal              = 31;
		public const int NotEqual           = 32;
		public const int GreaterEqual       = 33;
		public const int LessEqual          = 34;
		
		public const int ShiftLeft          = 35;
		public const int ShiftRight         = 36;
		
		public const int PlusAssign         = 37;
		public const int MinusAssign        = 38;
		public const int TimesAssign        = 39;
		public const int DivAssign          = 40;
		public const int ModAssign          = 41;
		public const int BitwiseAndAssign   = 42;
		public const int BitwiseOrAssign    = 43;
		public const int XorAssign          = 44;
		public const int ShiftLeftAssign    = 45;
		public const int ShiftRightAssign   = 46;
		
		public const int Pointer            = 47;
		
		/*----- C# keywords -----*/
		public const int Abstract           = 48;
		public const int As                 = 49;
		public const int Base               = 50;
		public const int Bool               = 51;
		public const int Break              = 52;
		public const int Byte               = 53;
		public const int Case               = 54;
		public const int Catch              = 55;
		public const int Char               = 56;
		public const int Checked            = 57;
		
		public const int Class              = 58;
		public const int Const              = 59;
		public const int Continue           = 60;
		public const int Decimal            = 61;
		public const int Default            = 62;
		public const int Delegate           = 63;
		public const int Do                 = 64;
		public const int Double             = 65;
		public const int Else               = 66;
		public const int Enum               = 67;
		
		public const int Event              = 68;
		public const int Explicit           = 69;
		public const int Extern             = 70;
		public const int False              = 71;
		public const int Finally            = 72;
		public const int Fixed              = 73;
		public const int Float              = 74;
		public const int For                = 75;
		public const int Foreach            = 76;
		public const int Goto               = 77;
		
		public const int If                 = 78;
		public const int Implicit           = 79;
		public const int In                 = 80;
		public const int Int                = 81;
		public const int Interface          = 82;
		public const int Internal           = 83;
		public const int Is                 = 84;
		public const int Lock               = 85;
		public const int Long               = 86;
		public const int Namespace          = 87;
		
		public const int New                = 88;
		public const int Null               = 89;
		public const int Object             = 90;
		public const int Operator           = 91;
		public const int Out                = 92;
		public const int Override           = 93;
		public const int Params             = 94;
		public const int Private            = 95;
		public const int Protected          = 96;
		public const int Public             = 97;
		
		public const int Readonly           = 98;
		public const int Ref                = 99;
		public const int Return             = 100;
		public const int Sbyte              = 101;
		public const int Sealed             = 102;
		public const int Short              = 103;
		public const int Sizeof             = 104;
		public const int Stackalloc         = 105;
		public const int Static             = 106;
		public const int String             = 107;
		
		public const int Struct             = 108;
		public const int Switch             = 109;
		public const int This               = 110;
		public const int Throw              = 111;
		public const int True               = 112;
		public const int Try                = 113;
		public const int Typeof             = 114;
		public const int Uint               = 115;
		public const int Ulong              = 116;
		public const int Unchecked          = 117;
		
		public const int Unsafe             = 118;
		public const int Ushort             = 119;
		public const int Using              = 120;
		public const int Virtual            = 121;
		public const int Void               = 122;
		public const int Volatile           = 123;
		public const int While              = 124;
	}
}
