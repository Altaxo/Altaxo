// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core.AddIns.Codons
{
	[CodonNameAttribute("Pad")]
	public class PadCodon : ClassCodon
	{
		[XmlMemberAttribute("category")]
		string category   = null;
		
		[XmlMemberArrayAttribute("shortcut", Separator = new char[]{ '|'})]
		string[] shortcut = null;
		
		public string Category {
			get {
				return category;
			}
			set {
				category = value;
			}
		}
		
		public string[] Shortcut {
			get {
				return shortcut;
			}
			set {
				shortcut = value;
			}
		}
		
		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			IPadContent pad = (IPadContent)base.BuildItem(owner, subItems, conditions);
			pad.Category = category;
			pad.Shortcut = shortcut;
			return pad;
		}
	}
}
