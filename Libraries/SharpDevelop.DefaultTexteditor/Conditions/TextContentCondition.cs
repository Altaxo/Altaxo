// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;


using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Conditions
{
	[ConditionAttribute()]
	public class TextContentCondition : AbstractCondition
	{
		[XmlMemberAttribute("textcontent", IsRequired = true)]
		string textcontent;
		
		public string TextContent {
			get {
				return textcontent;
			}
			set {
				textcontent = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			if (owner is TextEditorControl) {
				TextEditorControl ctrl = (TextEditorControl)owner;
				if (ctrl.Document != null && ctrl.Document.HighlightingStrategy != null) {
					return textcontent == ctrl.Document.HighlightingStrategy.Name;
				}
			}
			return false;
		}
	}
}
