// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// This class represents a category with its text content used in the 
	/// output pad (CompilerMessageView).
	/// </summary>
	public class MessageViewCategory
	{
		string        category;
		string        displayCategory;
		StringBuilder textBuilder = new StringBuilder();
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string DisplayCategory {
			get {
				return displayCategory;
			}
		}
		
		public string Text {
			get {
				return textBuilder.ToString();
			}
		}
		
		public MessageViewCategory(string category) : this(category, category)
		{
		}
		
		public MessageViewCategory(string category, string displayCategory)
		{
			this.category        = category;
			this.displayCategory = displayCategory;
		}
		
		public void AppendText(string text)
		{
			textBuilder.Append(text);
			OnTextChanged(EventArgs.Empty);
		}
		
		public void SetText(string text)
		{
			textBuilder = new StringBuilder(text);
			OnTextChanged(EventArgs.Empty);
		}
		
		public void ClearText()
		{
			textBuilder = new StringBuilder();
			OnTextChanged(EventArgs.Empty);
		}
		
		protected virtual void OnTextChanged(EventArgs e)
		{
			if (TextChanged != null) {
				TextChanged(this, e);
			}
		}
		
		public event EventHandler TextChanged;
	}
}
