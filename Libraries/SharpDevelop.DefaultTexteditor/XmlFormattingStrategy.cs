// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.Xml;
using System;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor
{
	/// <summary>
	/// This class currently only inserts the closing tags to 
	/// typed openening tags.
	/// </summary>
	public class XmlFormattingStrategy : DefaultFormattingStrategy
	{
		public override int FormatLine(TextArea textArea, int lineNr, int caretOffset, char charTyped) // used for comment tag formater/inserter
		{
			try {
				if (charTyped == '>') {
					StringBuilder stringBuilder = new StringBuilder();
					int offset = Math.Min(caretOffset - 2, textArea.Document.TextLength - 1);
					while (true) {
						if (offset < 0) {
							break;
						}
						char ch = textArea.Document.GetCharAt(offset);
						if (ch == '<') {
							string reversedTag = stringBuilder.ToString().Trim();
							if (!reversedTag.StartsWith("/") && !reversedTag.EndsWith("/")) {
								bool validXml = true;
								try {
									XmlDocument doc = new XmlDocument();
									doc.LoadXml(textArea.Document.TextContent);
								} catch (Exception) {
									validXml = false;
								}
								// only insert the tag, if something is missing
								if (!validXml) {
									StringBuilder tag = new StringBuilder();
									for (int i = reversedTag.Length - 1; i >= 0 && !Char.IsWhiteSpace(reversedTag[i]); --i) {
										tag.Append(reversedTag[i]);
									}
									string tagString = tag.ToString();
									if (tagString.Length > 0) {
										textArea.Document.Insert(caretOffset, "</" + tagString + ">");
									}
								}
							}
							break;
						}
						stringBuilder.Append(ch);
						--offset;
					}
				}
			} catch (Exception e) { // Insanity check
				Debug.Assert(false, e.ToString());
			}
			return charTyped == '\n' ? IndentLine(textArea, lineNr) : 0;
		}
	}	
}
