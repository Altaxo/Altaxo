// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Conditions;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Commands;
using Reflector.UserInterface;

namespace ICSharpCode.Core.AddIns.Codons
{
	[CodonName("MenuItem")]
	public class MenuItemCodon : AbstractCodon
	{
		[XmlMemberAttribute("label", IsRequired=true)]
		string label       = null;
		
		[XmlMemberAttribute("description")]
		string description = null;
		
		[XmlMemberArrayAttribute("shortcut",Separator=new char[]{ '|'})]
		string[] shortcut    = null;
		
		[XmlMemberAttribute("icon")]
		string icon        = null;
		
		[XmlMemberAttribute("link")]
		string link        = null;
		
		public string Link {
			get {
				return link;
			}
			set {
				link = value;
			}
		}
		
		public override bool HandleConditions {
			get {
				return true;
			}
		}
		
		public string Label {
			get {
				return label;
			}
			set {
				label = value;
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
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
		
		public string[] Shortcut {
			get {
				return shortcut;
			}
			set {
				shortcut = value;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public override object BuildItem(object owner, ArrayList subItems, ConditionCollection conditions)
		{
			CommandBarItem newItem = null;
			if (Label == "-") {
				newItem = new SdMenuSeparator(conditions, owner);
			} else  if (Link != null) {
				newItem = new SdMenuCommand(conditions, null, Label,  Link.StartsWith("http") ? (IMenuCommand)new GotoWebSite(Link) : (IMenuCommand)new GotoLink(Link));
			} else {
				object o = null;
				if (Class != null) {
					o = AddIn.CreateObject(Class);
				}
				if (o != null) {
					if (o is IMenuCommand) {
						IMenuCommand menuCommand = (IMenuCommand)o;
						menuCommand.Owner = owner;
						if (o is ICheckableMenuCommand) {
							newItem = new SdMenuCheckBox(conditions, owner, Label, (ICheckableMenuCommand)menuCommand);
						} else {
							newItem = new SdMenuCommand(conditions, owner, Label, menuCommand);
						}
					} else if (o is ISubmenuBuilder) {
						return o;//((ISubmenuBuilder)o).BuildSubmenu(owner);
					}
				}
			}
			if (newItem == null) {
				SdMenu newMenu = new SdMenu(conditions, owner, Label);
				if (subItems != null && subItems.Count > 0) {
					foreach (object item in subItems) {
						if (item != null) {
							newMenu.SubItems.Add(item);
						}
					}
				}
				newItem = newMenu;
			}
			Debug.Assert(newItem != null);
			
			if (Icon != null) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				newItem.Image = resourceService.GetBitmap(Icon);
			}
			
			if (newItem is SdMenuCommand) {
				((SdMenuCommand)newItem).Description = description;
			}
			
			if (Shortcut != null && newItem is SdMenuCommand) {
				try {
					foreach (string key in this.shortcut) {
						((SdMenuCommand)newItem).Shortcut |= (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), key);
					}
				} catch (Exception) {
					((SdMenuCommand)newItem).Shortcut = System.Windows.Forms.Keys.None;
				}
			}
			newItem.IsEnabled = true; //action != ConditionFailedAction.Disable;
			return newItem;
		}
	}
}
