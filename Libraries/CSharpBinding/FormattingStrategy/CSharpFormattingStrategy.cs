// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using SharpDevelop.Internal.Parser;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class CSharpFormattingStrategy : DefaultFormattingStrategy
	{
		public CSharpFormattingStrategy()
		{
		}
		
		/// <summary>
		/// Define CSharp specific smart indenting for a line :)
		/// </summary>
		protected override int SmartIndentLine(TextArea textArea, int lineNr)
		{
			if (lineNr > 0) {
				LineSegment lineAbove = textArea.Document.GetLineSegment(lineNr - 1);
				string  lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove).Trim();
				
				LineSegment curLine = textArea.Document.GetLineSegment(lineNr);
				string  curLineText = textArea.Document.GetText(curLine).Trim();
				
				if ((lineAboveText.EndsWith(")") && curLineText.StartsWith("{")) ||   // after for, while, etc.
					(lineAboveText.EndsWith("else") && curLineText.StartsWith("{")))  // after else
				{
					string indentation = GetIndentation(textArea, lineNr - 1);
					textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				if (curLineText.StartsWith("}")) { // indent closing bracket.
					int closingBracketOffset = TextUtilities.SearchBracketBackward(textArea.Document, curLine.Offset + textArea.Document.GetText(curLine).IndexOf('}') - 1, '{', '}');
					if (closingBracketOffset == -1) {  // no closing bracket found -> autoindent
						return AutoIndentLine(textArea, lineNr);
					}
					
					string indentation = GetIndentation(textArea, textArea.Document.GetLineNumberForOffset(closingBracketOffset));
					
					textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				if (lineAboveText.EndsWith(";")) { // expression ended, reset to valid indent.
					int closingBracketOffset = TextUtilities.SearchBracketBackward(textArea.Document, curLine.Offset + textArea.Document.GetText(curLine).IndexOf('}') - 1, '{', '}');
					
					if (closingBracketOffset == -1) {  // no closing bracket found -> autoindent
						return AutoIndentLine(textArea, lineNr);
					}
					
					int closingBracketLineNr = textArea.Document.GetLineNumberForOffset(closingBracketOffset);
					LineSegment closingBracketLine = textArea.Document.GetLineSegment(closingBracketLineNr);
					string  closingBracketLineText = textArea.Document.GetText(closingBracketLine).Trim();
					
					string indentation = GetIndentation(textArea, closingBracketLineNr);
					
					// special handling for switch statement formatting.
					if (closingBracketLineText.StartsWith("switch")) {
						if (lineAboveText.StartsWith("break;") || 
						    lineAboveText.StartsWith("goto")   || 
						    lineAboveText.StartsWith("return")) {
							// nothing
						} else {
						indentation += ICSharpCode.TextEditor.Actions.Tab.GetIndentationString(textArea.Document);
						}
					}
					indentation += ICSharpCode.TextEditor.Actions.Tab.GetIndentationString(textArea.Document);
					
					textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				if (lineAboveText.EndsWith("{") || // indent opening bracket.
				    lineAboveText.EndsWith(":") || // indent case xyz:
				    (lineAboveText.EndsWith(")") &&  // indent single line if, for ... etc
				    (lineAboveText.StartsWith("if") ||
				     lineAboveText.StartsWith("while") ||
				     lineAboveText.StartsWith("for"))) ||
				     lineAboveText.EndsWith("else")) {
						string indentation = GetIndentation(textArea, lineNr - 1) + ICSharpCode.TextEditor.Actions.Tab.GetIndentationString(textArea.Document);
						textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
						return indentation.Length;
			} else {
				// try to indent linewrap
				ArrayList bracketPos = new ArrayList();
				for (int i = 0; i < lineAboveText.Length; ++i) { // search for a ( bracket that isn't closed
					switch (lineAboveText[i]) {
						case '(':
							bracketPos.Add(i);
							break;
						case ')':
							if (bracketPos.Count > 0) {
								bracketPos.RemoveAt(bracketPos.Count - 1);
							}
							break;
					}
				}
				
				if (bracketPos.Count > 0) {
					int bracketIndex = (int)bracketPos[bracketPos.Count - 1];
					string indentation = GetIndentation(textArea, lineNr - 1);
					
					for (int i = 0; i <= bracketIndex; ++i) { // insert enough spaces to match
					indentation += " ";                   // brace start in the next line
					}
					
					textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
			}
			}
			return AutoIndentLine(textArea, lineNr);
		}
		
		bool NeedCurlyBracket(string text)
		{
			int curlyCounter = 0;
			
			bool inString = false;
			bool inChar   = false;
			
			bool lineComment  = false;
			bool blockComment = false;
			
			for (int i = 0; i < text.Length; ++i) {
				switch (text[i]) {
					case '\r':
					case '\n':
						lineComment = false;
						break;
					case '/':
						if (blockComment) {
							Debug.Assert(i > 0);
							if (text[i - 1] == '*') {
								blockComment = false;
							}
						}
						if (!inString && !inChar && i + 1 < text.Length) {
							if (!blockComment && text[i + 1] == '/') {
								lineComment = true;
							}
							if (!lineComment && text[i + 1] == '*') {
								blockComment = true;
							}
						}
						break;
					case '"':
						if (!(inChar || lineComment || blockComment)) {
							inString = !inString;
						}
						break;
					case '\'':
						if (!(inString || lineComment || blockComment)) {
							inChar = !inChar;
						}
						break;
					case '{':
						if (!(inString || inChar || lineComment || blockComment)) {
							++curlyCounter;
						}
						break;
					case '}':
						if (!(inString || inChar || lineComment || blockComment)) {
							--curlyCounter;
						}
						break;
				}
			}
			return curlyCounter > 0;
		}
		
		bool IsInsideStringOrComment(TextArea textArea, LineSegment curLine, int cursorOffset)
		{
			// scan cur line if it is inside a string or single line comment (//)
			bool isInsideString  = false;
			bool isInsideComment = false;
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = textArea.Document.GetCharAt(i);
				if (ch == '"') {
					isInsideString = !isInsideString;
				}
				if (ch == '/' && i + 1 < cursorOffset && textArea.Document.GetCharAt(i + 1) == '/') {
					isInsideComment = true;
					break;
				}
			}
			
			return isInsideString || isInsideComment;
		}

		bool IsInsideDocumentationComment(TextArea textArea, LineSegment curLine, int cursorOffset)
		{
			// scan cur line if it is inside a string or single line comment (//)
			bool isInsideString  = false;
			bool isInsideComment = false;
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = textArea.Document.GetCharAt(i);
				if (ch == '"') {
					isInsideString = !isInsideString;
				}
				if (!isInsideString) {
					if (ch == '/' && i + 2 < cursorOffset && textArea.Document.GetCharAt(i + 1) == '/' && textArea.Document.GetCharAt(i + 2) == '/') {
						isInsideComment = true;
						break;
					}
				}
			}
			
			return isInsideComment;
		}
		IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
		
		bool IsBeforeRegion(TextArea textArea, IRegion region, int lineNr)
		{
			if (region == null) {
				return false;
			}
			return region.BeginLine - 2 <= lineNr && lineNr <= region.BeginLine;
		}
		
		object GetClassMember(TextArea textArea, int lineNr, IClass c)
		{
			if (IsBeforeRegion(textArea, c.Region, lineNr)) {
				return c;
			}
			
			foreach (IClass inner in c.InnerClasses) {
				object o = GetClassMember(textArea, lineNr, inner);
				if (o != null) {
					return o;
				}
			}
			
			foreach (IField f in c.Fields) {
				if (IsBeforeRegion(textArea, f.Region, lineNr)) {
					return f;
				}
			}
			foreach (IProperty p in c.Properties) {
				if (IsBeforeRegion(textArea, p.Region, lineNr)) {
					return p;
				}
			}
			foreach (IIndexer i in c.Indexer) {
				if (IsBeforeRegion(textArea, i.Region, lineNr)) {
					return i;
				}
			}
			foreach (IEvent e in c.Events) {
				if (IsBeforeRegion(textArea, e.Region, lineNr)) {
					return e;
				}
			}
			foreach (IMethod m in c.Methods) {
				if (IsBeforeRegion(textArea, m.Region, lineNr)) {
					return m;
				}
			}
			return null;
		}
			
		object GetMember(TextArea textArea, int lineNr)
		{
			string fileName = textArea.MotherTextEditorControl.FileName;
			if (fileName != null && fileName.Length > 0 ) {
				string fullPath = Path.GetFullPath(fileName);
				IParseInformation parseInfo = parserService.GetParseInformation(fullPath);
				if (parseInfo != null) {
					ICompilationUnit currentCompilationUnit = (ICompilationUnit)parseInfo.BestCompilationUnit;
					if (currentCompilationUnit != null) {
						foreach (IClass c in currentCompilationUnit.Classes) {
							object o = GetClassMember(textArea, lineNr, c);
							if (o != null) {
								return o;
							}
						}
					}
				}
			}
			return null;
		}
		
		public override int FormatLine(TextArea textArea, int lineNr, int cursorOffset, char ch) // used for comment tag formater/inserter
		{
			LineSegment curLine   = textArea.Document.GetLineSegment(lineNr);
			LineSegment lineAbove = lineNr > 0 ? textArea.Document.GetLineSegment(lineNr - 1) : null;
			
			//// local string for curLine segment
			string curLineText="";
			if (ch == '/') {
				curLineText   = textArea.Document.GetText(curLine);
				string lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove);
				if (curLineText != null && curLineText.EndsWith("///") && (lineAboveText == null || !lineAboveText.Trim().StartsWith("///"))) {
					string indentation = base.GetIndentation(textArea, lineNr);
					object member = GetMember(textArea, lineNr);
					if (member != null) {
						StringBuilder sb = new StringBuilder();
						sb.Append(" <summary>\n");
						sb.Append(indentation);
						sb.Append("/// \n");
						sb.Append(indentation);
						sb.Append("/// </summary>");
												
						if (member is IMethod) {
							IMethod method = (IMethod)member;
							if (method.Parameters != null && method.Parameters.Count > 0) {
								for (int i = 0; i < method.Parameters.Count; ++i) {
									sb.Append("\n");
									sb.Append(indentation);
									sb.Append("/// <param name=\"");
									sb.Append(method.Parameters[i].Name);
									sb.Append("\"></param>");
								}
							}
							if (method.ReturnType != null && method.ReturnType.FullyQualifiedName != "System.Void") {
								sb.Append("\n");
								sb.Append(indentation);
								sb.Append("/// <returns></returns>");
							}
						}
						textArea.Document.Insert(cursorOffset, sb.ToString());
						
						textArea.Refresh();
						textArea.Caret.Position = textArea.Document.OffsetToPosition(cursorOffset + indentation.Length + "/// ".Length + " <summary>\n".Length);
						return 0;
					}
				}
				return 0;
			}
			if (ch != '\n' && ch != '>') {
				if (IsInsideStringOrComment(textArea, curLine, cursorOffset)) {
					return 0;
				}
			}
			
			switch (ch) {
				case '>':
					if (IsInsideDocumentationComment(textArea, curLine, cursorOffset)) {
						curLineText  = textArea.Document.GetText(curLine);
						int column = textArea.Caret.Offset - curLine.Offset;
						int index = Math.Min(column - 1, curLineText.Length - 1);
						
						while (index >= 0 && curLineText[index] != '<') {
							--index;
							if(curLineText[index] == '/')
								return 0; // the tag was an end tag or already 
						}
						
						if (index > 0) {
							StringBuilder commentBuilder = new StringBuilder("");
							for (int i = index; i < curLineText.Length && i < column && !Char.IsWhiteSpace(curLineText[i]); ++i) {
								commentBuilder.Append(curLineText[ i]);
							}
							string tag = commentBuilder.ToString().Trim();
							if (!tag.EndsWith(">")) {
								tag += ">";
							}
							if (!tag.StartsWith("/")) {
								textArea.Document.Insert(textArea.Caret.Offset, "</" + tag.Substring(1));
							}
						}
					}
					break;
				case '}':
				case '{':
					return textArea.Document.FormattingStrategy.IndentLine(textArea, lineNr);
				case '\n':
					if (lineNr <= 0) {
						return IndentLine(textArea, lineNr);
					}
					
					if (textArea.TextEditorProperties.AutoInsertCurlyBracket) {
						string oldLineText = TextUtilities.GetLineAsString(textArea.Document, lineNr - 1);
						if (oldLineText.EndsWith("{")) {
							if (NeedCurlyBracket(textArea.Document.TextContent)) {
								textArea.Document.Insert(curLine.Offset + curLine.Length, "\n}");
								IndentLine(textArea, lineNr + 1);
							}
						}
					}
					
					string  lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove);
					//// curLine might have some text which should be added to indentation
					curLineText = "";
					if (curLine.Length > 0) {
						curLineText = textArea.Document.GetText(curLine);
					}
					
					LineSegment nextLine      = lineNr + 1 < textArea.Document.TotalNumberOfLines ? textArea.Document.GetLineSegment(lineNr + 1) : null;
					string      nextLineText  = lineNr + 1 < textArea.Document.TotalNumberOfLines ? textArea.Document.GetText(nextLine) : "";
					
					if (lineAbove.HighlightSpanStack != null && lineAbove.HighlightSpanStack.Count > 0) {			
						if (!((Span)lineAbove.HighlightSpanStack.Peek()).StopEOL) {	// case for /* style comments
							int index = lineAboveText.IndexOf("/*");
							if (index > 0) {
								string indentation = GetIndentation(textArea, lineNr - 1);
								for (int i = indentation.Length; i < index; ++ i) {
									indentation += ' ';
								}
								//// adding curline text
								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + " * " + curLineText);
								return indentation.Length + 3 + curLineText.Length;
							}
							
							index = lineAboveText.IndexOf("*");
							if (index > 0) {
								string indentation = GetIndentation(textArea, lineNr - 1);
								for (int i = indentation.Length; i < index; ++ i) {
									indentation += ' ';
								}
								//// adding curline if present
								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + "* " + curLineText);
								return indentation.Length + 2 + curLineText.Length;
							}
						} else { // don't handle // lines, because they're only one lined comments
							int indexAbove = lineAboveText.IndexOf("///");
							int indexNext  = nextLineText.IndexOf("///");
							if (indexAbove > 0 && (indexNext != -1 || indexAbove + 4 < lineAbove.Length)) {
								string indentation = GetIndentation(textArea, lineNr - 1);
								for (int i = indentation.Length; i < indexAbove; ++ i) {
									indentation += ' ';
								}
								//// adding curline text if present
								textArea.Document.Replace(curLine.Offset, cursorOffset - curLine.Offset, indentation + "/// " + curLineText);
								return indentation.Length + 4 /*+ curLineText.Length*/;
							}
						}
					}
					return IndentLine(textArea, lineNr);
			}
			return 0;
		}
	}
}
