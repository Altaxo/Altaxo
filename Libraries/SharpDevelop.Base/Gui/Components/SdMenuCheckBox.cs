// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SdMenuCheckBox : CommandBarCheckBox, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
		object caller;
		ConditionCollection conditionCollection;
		string description   = String.Empty;
		string localizedText = String.Empty;
		ICheckableMenuCommand menuCommand;
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public SdMenuCheckBox(ConditionCollection conditionCollection, object caller, string label) : base(stringParserService.Parse(label))
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			this.localizedText       = label;
			UpdateStatus();
		}
		
		public SdMenuCheckBox(ConditionCollection conditionCollection, object caller, string label, ICheckableMenuCommand menuCommand) : base(stringParserService.Parse(label))
		{
			this.menuCommand         = menuCommand;
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			this.localizedText       = label;
			UpdateStatus();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand != null) {
				menuCommand.IsChecked = IsChecked;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.IsEnabled = failedAction != ConditionFailedAction.Disable;
				this.IsVisible = failedAction != ConditionFailedAction.Exclude;
			}
			Text = stringParserService.Parse(localizedText);
			if (menuCommand != null) {
				IsChecked = menuCommand.IsChecked;
			}
		}
	}
}
