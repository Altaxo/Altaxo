// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns.Codons;
using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public interface IStatusUpdate
	{
		void UpdateStatus();
	}
	public class SdMenu : CommandBarMenu, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		ConditionCollection conditionCollection;
		object caller;
		string localizedText = String.Empty;
		public ArrayList SubItems = new ArrayList();
		
		public SdMenu(ConditionCollection conditionCollection, object caller, string text) : base(stringParserService.Parse(text))
		{
			this.conditionCollection = conditionCollection;
			this.caller              = caller;
			this.localizedText       = text;
		}
		
		protected override void OnDropDown(System.EventArgs e)
		{
			base.OnDropDown(e);
			foreach (object o in Items) {
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.IsEnabled = failedAction != ConditionFailedAction.Disable;
				this.IsVisible = failedAction != ConditionFailedAction.Exclude;
			}
			
			if (IsVisible) {
				Items.Clear();
				foreach (object item in SubItems) {
					if (item is CommandBarItem) {
						if (item is IStatusUpdate) {
							((IStatusUpdate)item).UpdateStatus();
						}
						Items.Add((CommandBarItem)item);
					} else {
						ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
						Items.AddRange(submenuBuilder.BuildSubmenu(conditionCollection, caller));
					}
				}
			}
			Text = stringParserService.Parse(localizedText);
		}
	}
}
