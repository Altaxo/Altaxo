// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns.Conditions;


using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.Core.AddIns.Codons
{
	[CodonNameAttribute("ToolbarItem")]
	public class ToolbarItemCodon : AbstractCodon
	{
		[XmlMemberAttribute("icon")]
		string icon        = null;
		
		[XmlMemberAttributeAttribute("tooltip")]
		string toolTip     = null;
		
		ArrayList subItems = null;
		
		bool      enabled  = true;
		
		ConditionCollection conditions;
		
		public string ToolTip {
			get {
				return toolTip;
			}
			set {
				toolTip = value;
			}
		}
		
		public override bool HandleConditions {
			get {
				return true;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
			set {
				icon = value;
			}
		}
		
		public ArrayList SubItems {
			get {
				return subItems;
			}
			set {
				subItems = value;
			}
		}
		
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				enabled = value;
			}
		}
		public ConditionCollection Conditions {
			get {
				return conditions;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			this.subItems = subItems;
			enabled       = false; //action != ConditionFailedAction.Disable;
			this.conditions = conditions;
			return this;
		}
	}
}
