// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using ICSharpCode.SharpRefactory.Parser;

namespace ICSharpCode.SharpRefactory.Parser
{
	public class Token
	{
		public int kind;
		
		public int col;
		public int line;
		
		public object literalValue = null;
		public string val;
		public Token  next;
//		public ArrayList specials;
		
		public Point EndLocation {
			get {
				return new Point(col + val.Length, line);
			}
		}
		public Point Location {
			get {
				return new Point(col, line);
			}
		}
		
		public Token()
		{
		}
		
		public Token(int kind)
		{
			this.kind = kind;
		}
		
//		public Token(Tokens kind, int col, int line)
//		{
//			this.kind = kind;
//			this.col  = col;
//			this.line = line;
//		}
		
		public Token(int kind, int col, int line, string val)
		{
			this.kind = kind;
			this.col  = col;
			this.line = line;
			this.val  = val;
		}
		
		public Token(int kind, int col, int line, string val, object literalValue)
		{
			this.kind         = kind;
			this.col          = col;
			this.line         = line;
			this.val          = val;
			this.literalValue = literalValue;
		}
	}
	
	public class Lexer
	{
		IReader reader;
		static  Hashtable keywords = new Hashtable();
		
		int col  = 1;
		int line = 1;
		
		Errors errors   = new Errors();
		
//		SpecialTracker specialTracker = new SpecialTracker();
		
		Token lastToken = null;
		Token curToken  = null;
		Token peekToken = null;
		
		public Errors Errors {
			get {
				return errors;
			}
		}
		
		public Token Token {
			get {
				return lastToken;
			}
		}
		
		public Token LookAhead {
			get {
				return curToken;
			}
		}
		
		public void StartPeek()
		{
			peekToken = curToken;
		}
		
		public Token Peek()
		{
			if (peekToken.next == null) {
				peekToken.next = Next();
//				peekToken.next.specials = this.specialTracker.RetrieveSpecials();
			}
			peekToken = peekToken.next;
			return peekToken;
		}
		
		public Token NextToken()
		{
			if (curToken == null) {
				curToken = Next();
//				curToken.specials = this.specialTracker.RetrieveSpecials();
				return curToken;
			}
			
//			if (lastToken != null && lastToken.specials != null) {
//				curToken.specials.InsertRange(0, lastToken.specials);
//			}
			
			lastToken = curToken;
			
			if (curToken.next == null) {
				curToken.next = Next();
//				curToken.next.specials = this.specialTracker.RetrieveSpecials();
			}
			
			curToken  = curToken.next;
			return curToken;
		}
		
//		public ArrayList RetrieveSpecials()
//		{
//			if (lastToken == null) {
//				return this.specialTracker.RetrieveSpecials();
//			}
//			
//			Debug.Assert(lastToken.specials != null);
//			
//			ArrayList tmp = lastToken.specials;
//			lastToken.specials = null;
//			return tmp;
//		}
		
		static string[] keywordStrings = {
			"abstract",
			"as",
			"base",
			"bool",
			"break",
			"byte",
			"case",
			"catch",
			"char",
			"checked",
			"class",
			"const",
			"continue",
			"decimal",
			"default",
			"delegate",
			"do",
			"double",
			"else",
			"enum",
			"event",
			"explicit",
			"extern",
			"false",
			"finally",
			"fixed",
			"float",
			"for",
			"foreach",
			"goto",
			"if",
			"implicit",
			"in",
			"int",
			"interface",
			"internal",
			"is",
			"lock",
			"long",
			"namespace",
			"new",
			"null",
			"object",
			"operator",
			"out",
			"override",
			"params",
			"private",
			"protected",
			"public",
			"readonly",
			"ref",
			"return",
			"sbyte",
			"sealed",
			"short",
			"sizeof",
			"stackalloc",
			"static",
			"string",
			"struct",
			"switch",
			"this",
			"throw",
			"true",
			"try",
			"typeof",
			"uint",
			"ulong",
			"unchecked",
			"unsafe",
			"ushort",
			"using",
			"virtual",
			"void",
			"volatile",
			"while",
		};
		
		static Lexer()
		{
			for (int i = 0 ; i < keywordStrings.Length; ++i) {
				keywords.Add(keywordStrings[i], i + Tokens.Abstract);
			}
		}
		
		public Lexer(IReader reader)
		{
			this.reader = reader;
		}
		
		Token Next()
		{
			while (!reader.Eos()) {
				char ch = reader.GetNext();
				
				if (Char.IsWhiteSpace(ch)) {
					++col;
					
					if (ch == '\n') {
//						specialTracker.AddEndOfLine();
						++line;
						col = 1;
					}
					continue;
				}
				
				if (Char.IsLetter(ch) || ch == '_') {
					int x = col;
					int y = line;
					string s = ReadIdent(ch);
					if (keywords[s] != null) {
						return new Token((int)keywords[s], x, y, s);
					}
					return new Token(Tokens.Identifier, x, y, s);
				}
				
				if (Char.IsDigit(ch)) {
					return ReadDigit(ch, col);
				}
				
				if (ch == '/') {
					if (reader.Peek() == '/' || reader.Peek() == '*') {
						++col;
						ReadComment();
						continue;
					}
				} else if (ch == '#') {
					++col;
					string directive = ReadIdent('#');
					string argument  = ReadToEOL();
//					this.specialTracker.AddPreProcessingDirective(directive, argument);
					continue;
				}
				
				if (ch == '"') {
					++col;
					return ReadString();
				}
				
				if (ch == '\'') {
					++col;
					return ReadChar();
				}
				
				if (ch == '@') {
					int x = col;
					int y = line;
					ch = reader.GetNext();
					++col;
					if (ch == '"') {
						return ReadVerbatimString();
					}
					if (Char.IsLetterOrDigit(ch)) {
						return new Token(Tokens.Identifier, x, y, ReadIdent(ch));
					}
					errors.Error(y, x, String.Format("Unexpected char in Lexer.Next() : {0}", ch));
				}
				
				Token token = ReadOperator(ch);
				
				// try error recovery :)
				if (token == null) {
					return Next();
				}
				return token;
			}
			
			return new Token(Tokens.EOF, col, line, String.Empty);
		}
		
		string ReadIdent(char ch)
		{
			StringBuilder s = new StringBuilder(ch.ToString());
			++col;
			while (!reader.Eos() && (Char.IsLetterOrDigit(ch = reader.GetNext()) || ch == '_')) {
				s.Append(ch.ToString());
				++col;
			}
			if (!reader.Eos()) {
				reader.UnGet();
			}
			return s.ToString();
		}
		
		Token ReadDigit(char ch, int x)
		{
			int y = line;
			++col;
			StringBuilder sb = new StringBuilder(ch.ToString());
			
			bool ishex      = false;
			bool isunsigned = false;
			bool islong     = false;
			bool isfloat    = false;
			bool isdouble   = false;
			bool isdecimal  = false;
			
			if (ch == '0' && Char.ToUpper(reader.Peek()) == 'X') {
				const string hex = "0123456789ABCDEF";
				reader.GetNext(); // skip 'x'
				++col;
				while (hex.IndexOf(Char.ToUpper(reader.Peek())) != -1) {
					sb.Append(Char.ToUpper(reader.GetNext()));
					++col;
				}
				ishex = true;
			} else {
				while (Char.IsDigit(reader.Peek())) {
					sb.Append(reader.GetNext());
					++col;
				}
			}
			
			if (reader.Peek() == '.') { // read floating point number
				isdouble = true; // double is default
				if (ishex) {
					errors.Error(y, x, String.Format("No hexadecimal floating point values allowed"));
				}
				sb.Append(reader.GetNext());
				++col;
				
				while (Char.IsDigit(reader.Peek())) { // read decimal digits beyond the dot
					sb.Append(reader.GetNext());
					++col;
				}
			}
			
			if (Char.ToUpper(reader.Peek()) == 'E') { // read exponent
				isdouble = true;
				sb.Append(reader.GetNext());
				++col;
				if (reader.Peek() == '-' || reader.Peek() == '+') {
					sb.Append(reader.GetNext());
					++col;
				}
				while (Char.IsDigit(reader.Peek())) { // read exponent value
					sb.Append(reader.GetNext());
					++col;
				}
				isunsigned = true;
			}
			
			if (Char.ToUpper(reader.Peek()) == 'F') { // float value
				reader.GetNext();
				++col;
				isfloat = true;
			} else if (Char.ToUpper(reader.Peek()) == 'M') { // double type suffix (obsolete, double is default)
				reader.GetNext();
				++col;
				isdouble = true;
			} else if (Char.ToUpper(reader.Peek()) == 'D') { // decimal value
				reader.GetNext();
				++col;
				isdecimal = true;
			} else if (!isdouble) {
				if (Char.ToUpper(reader.Peek()) == 'U') {
					reader.GetNext();
					++col;
					isunsigned = true;
				}
				
				if (Char.ToUpper(reader.Peek()) == 'L') {
					reader.GetNext();
					++col;
					islong = true;
					if (!isunsigned && Char.ToUpper(reader.Peek()) == 'U') {
						reader.GetNext();
						++col;
						isunsigned = true;
					}
				}
			}
			
			string digit = sb.ToString();
			if (isfloat) {
				try {
					NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
					numberFormatInfo.CurrencyDecimalSeparator = ".";
					return new Token(Tokens.Literal, x, y, digit, Single.Parse(digit, numberFormatInfo));
				} catch (Exception) {
					errors.Error(y, x, String.Format("Can't parse float {0}", digit));
					return new Token(Tokens.Literal, x, y, digit, 0f);
				}
			}
			if (isdecimal) {
				try {
					NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
					numberFormatInfo.CurrencyDecimalSeparator = ".";
					return new Token(Tokens.Literal, x, y, digit, Decimal.Parse(digit, numberFormatInfo));
				} catch (Exception) {
					errors.Error(y, x, String.Format("Can't parse decimal {0}", digit));
					return new Token(Tokens.Literal, x, y, digit, 0m);
				}
			}
			if (isdouble) {
				try {
					NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
					numberFormatInfo.CurrencyDecimalSeparator = ".";
					return new Token(Tokens.Literal, x, y, digit, Double.Parse(digit, numberFormatInfo));
				} catch (Exception) {
					errors.Error(y, x, String.Format("Can't parse double {0}", digit));
					return new Token(Tokens.Literal, x, y, digit, 0d);
				}
			}
			if (islong) {
				if (isunsigned) {
					try {
						return new Token(Tokens.Literal, x, y, digit, UInt64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					} catch (Exception) {
						errors.Error(y, x, String.Format("Can't parse unsigned long {0}", digit));
						return new Token(Tokens.Literal, x, y, digit, 0UL);
					}
				} else {
					try {
						return new Token(Tokens.Literal, x, y, digit, Int64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					} catch (Exception) {
						errors.Error(y, x, String.Format("Can't parse long {0}", digit));
						return new Token(Tokens.Literal, x, y, digit, 0L);
					}
				}
			} else {
				if (isunsigned) {
					try {
						return new Token(Tokens.Literal, x, y, digit, UInt32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					} catch (Exception) {
						errors.Error(y, x, String.Format("Can't parse unsigned int {0}", digit));
						return new Token(Tokens.Literal, x, y, digit, 0U);
					}
				} else {
					try {
						return new Token(Tokens.Literal, x, y, digit, Int32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					} catch (Exception) {
						errors.Error(y, x, String.Format("Can't parse int {0}", digit));
						return new Token(Tokens.Literal, x, y, digit, 0);
					}
				}
			}
		}
		
		Token ReadString()
		{
			int x = col;
			int y = line;
			
			char ch = '\0';
			StringBuilder s             = new StringBuilder();
			StringBuilder originalValue = new StringBuilder();
			originalValue.Append('"');
			while (!reader.Eos() && ((ch = reader.GetNext()) != '"')) {
				++col;
				if (ch == '\\') {
					originalValue.Append('\\');
					originalValue.Append(ReadEscapeSequence(out ch));
					s.Append(ch);
				} else if (ch == '\n') {
					errors.Error(y, x, String.Format("No new line is allowed inside a string literal"));
					break;
				} else {
					originalValue.Append(ch);
					s.Append(ch);
				}
			}
			if (ch != '"') {
				errors.Error(y, x, String.Format("End of file reached inside string literal"));
			}
			originalValue.Append('"');
			return new Token(Tokens.Literal, x, y, originalValue.ToString(), s.ToString());
		}
		
		Token ReadVerbatimString()
		{
			int x = col;
			int y = line;
			char ch = '\0';
			StringBuilder s = new StringBuilder();
			while (!reader.Eos() && (ch = reader.GetNext()) != '"') {
				++col;
				if (ch == '\n') {
					++line;
					col = 1;
				}
				s.Append(ch);
			}
			if (ch != '"') {
				errors.Error(y, x, String.Format("End of file reached inside verbatim string literal"));
			}
			return new Token(Tokens.Literal, x, y, String.Concat("@\"", s.ToString(), '"'), s.ToString());
		}
		
		string hexdigits = "0123456789ABCDEF";
		
		string ReadEscapeSequence(out char ch)
		{
			StringBuilder s = new StringBuilder();
			if (reader.Eos()) {
				errors.Error(line, col, String.Format("End of file reached inside escape sequence"));
			}
			char c = reader.GetNext();
			s.Append(c);
			++col;
			switch (c)  {
				case '\'':
					ch = '\'';
					break;
				case '\"':
					ch = '\"';
					break;
				case '\\':
					ch = '\\';
					break;
				case '0':
					ch = '\0';
					break;
				case 'a':
					ch = '\a';
					break;
				case 'b':
					ch = '\b';
					break;
				case 'f':
					ch = '\f';
					break;
				case 'n':
					ch = '\n';
					break;
				case 'r':
					ch = '\r';
					break;
				case 't':
					ch = '\t';
					break;
				case 'v':
					ch = '\v';
					break;
				case 'u':
				case 'x':
					c = reader.GetNext();
					int number = hexdigits.IndexOf(Char.ToUpper(c));
					if (number < 0) {
						errors.Error(line, col, String.Format("Invalid char in literal : {0}", c));
					}
					s.Append(c);
					for (int i = 0; i < 3; ++i) {
						c = reader.GetNext();
						int idx = hexdigits.IndexOf(Char.ToUpper(c));
						if (idx >= 0) {
							s.Append(c);
							number = idx * (16 * (i + 1)) + number;
						} else {
							reader.UnGet();
							break;
						}
					}
					ch = (char)number;
					break;
				default:
					errors.Error(line, col, String.Format("Unexpected escape sequence : {0}", c));
					ch = '\0';
					break;
			}
			return s.ToString();
		}
		
		Token ReadChar()
		{
			int x = col;
			int y = line;
			
			if (reader.Eos()) {
				errors.Error(y, x, String.Format("End of file reached inside character literal"));
			}
			StringBuilder originalValue = new StringBuilder();
			char  ch = reader.GetNext();
			originalValue.Append("'");
			originalValue.Append(ch);
			++col;
			
			if (ch == '\\') {
				originalValue.Append(ReadEscapeSequence(out ch));
			}
			
			if (reader.Eos()) {
				errors.Error(y, x, String.Format("End of file reached inside character literal"));
			}
			if (reader.GetNext() != '\'') {
				errors.Error(y, x, String.Format("Char not terminated"));
			}
			originalValue.Append("'");
			return new Token(Tokens.Literal, x, y, originalValue.ToString(), ch);
		}
		
		Token ReadOperator(char ch)
		{
			int x = col;
			int y = line;
			++col;
			switch (ch) {
				case '+':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '+':
								++col;
								return new Token(Tokens.Increment, x, y, "++");
							case '=':
								++col;
								return new Token(Tokens.PlusAssign, x, y, "+=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Plus, x, y, "+");
				case '-':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '-':
								++col;
								return new Token(Tokens.Decrement, x, y, "--");
							case '=':
								++col;
								return new Token(Tokens.MinusAssign, x, y, "-=");
							case '>':
								++col;
								return new Token(Tokens.Pointer, x, y, "->");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Minus, x, y, "-");
				case '*':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.TimesAssign, x, y, "*=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Times, x, y, "*");
				case '/':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.DivAssign, x, y, "/=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Div, x, y, "/");
				case '%':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.ModAssign, x, y, "%=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Mod, x, y, "%");
				case '&':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '&':
								++col;
								return new Token(Tokens.LogicalAnd, x, y, "&&");
							case '=':
								++col;
								return new Token(Tokens.BitwiseAndAssign, x, y, "&=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.BitwiseAnd, x, y, "&");
				case '|':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '|':
								++col;
								return new Token(Tokens.LogicalOr, x, y, "||");
							case '=':
								++col;
								return new Token(Tokens.BitwiseOrAssign, x, y, "|=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.BitwiseOr, x, y, "|");
				case '^':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.XorAssign, x, y, "^=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Xor, x, y, "^");
				case '!':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.NotEqual, x, y, "!=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Not, x, y, "!");
				case '~':
					return new Token(Tokens.BitwiseComplement, x, y, "~");
				case '=':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '=':
								++col;
								return new Token(Tokens.Equal, x, y, "==");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.Assign, x, y, "=");
				case '<':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '<':
								if (!reader.Eos()) {
									switch (reader.GetNext()) {
										case '=':
											col += 2;
											return new Token(Tokens.ShiftLeftAssign, x, y, "<<=");
										default:
											++col;
											reader.UnGet();
											break;
									}
								}
								return new Token(Tokens.ShiftLeft, x, y, "<<");
							case '=':
								++col;
								return new Token(Tokens.LessEqual, x, y, "<=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.LessThan, x, y, "<");
				case '>':
					if (!reader.Eos()) {
						switch (reader.GetNext()) {
							case '>':
								if (!reader.Eos()) {
									switch (reader.GetNext()) {
										case '=':
											col += 2;
											return new Token(Tokens.ShiftRightAssign, x, y, ">>=");
										default:
											++col;
											reader.UnGet();
											break;
									}
								}
								return new Token(Tokens.ShiftRight, x, y, ">>");
							case '=':
								++col;
								return new Token(Tokens.GreaterEqual, x, y, ">=");
							default:
								reader.UnGet();
								break;
						}
					}
					return new Token(Tokens.GreaterThan, x, y, ">");
				case '?':
					return new Token(Tokens.Question, x, y, "?");
				case ';':
					return new Token(Tokens.Semicolon, x, y, ";");
				case ':':
					return new Token(Tokens.Colon, x, y, ":");
				case ',':
					return new Token(Tokens.Comma, x, y, ",");
				case '.':
					if (Char.IsDigit(reader.Peek())) {
						 reader.UnGet();
						 col -= 2;
						 return ReadDigit('0', col + 1);
					}
					return new Token(Tokens.Dot, x, y, ".");
				case ')':
					return new Token(Tokens.CloseParenthesis, x, y, ")");
				case '(':
					return new Token(Tokens.OpenParenthesis, x, y, "(");
				case ']':
					return new Token(Tokens.CloseSquareBracket, x, y, "]");
				case '[':
					return new Token(Tokens.OpenSquareBracket, x, y, "[");
				case '}':
					return new Token(Tokens.CloseCurlyBrace, x, y, "}");
				case '{':
					return new Token(Tokens.OpenCurlyBrace, x, y, "{");
				default:
					--col;
					return null;
			}
		}
		
		void ReadComment()
		{
			char ch = reader.GetNext();
			++col;
			switch (ch) {
				case '*':
					ReadMultiLineComment();
					break;
				case '/':
					if (reader.GetNext() == '/') {
						ReadSingleLineComment(CommentType.Documentation);
					} else {
						reader.UnGet();
						ReadSingleLineComment(CommentType.SingleLine);
					}
					break;
				default:
					errors.Error(line, col, String.Format("Error while reading comment"));
					break;
			}
		}
		
		string ReadToEOL()
		{
			StringBuilder sb = new StringBuilder();
			if (!reader.Eos()) {
				char ch = reader.GetNext();
				while (!reader.Eos()) {
					if (ch == '\n') {
						++line;
						col = 1;
						return sb.ToString();;
					} else {
						sb.Append(ch);
					}
					ch = reader.GetNext();
					++col;
				}
			}
			return sb.ToString();
		}
		
		void ReadSingleLineComment(CommentType commentType)
		{
			string comment = ReadToEOL();
//			specialTracker.StartComment(commentType, new Point(line, col));
//			specialTracker.AddString(ReadToEOL());
//			specialTracker.FinishComment();
		}
		
		void ReadMultiLineComment()
		{
//			specialTracker.StartComment(CommentType.Block, new Point(line, col));
			int x = col;
			int y = line;
			while (!reader.Eos()) {
				char ch;
				switch (ch = reader.GetNext()) {
					case '\n':
//						specialTracker.AddChar('\n');
						++line;
						col = 1;
						break;
					case '*':
						++col;
						switch (reader.Peek()) {
							case '/':
								reader.GetNext();
								++col;
//								specialTracker.FinishComment();
								return;
							default:
//								specialTracker.AddChar('*');
								continue;
						}
					default:
//						specialTracker.AddChar(ch);
						++col;
						break;
				}
			}
//			specialTracker.FinishComment();
		}
	}
}
