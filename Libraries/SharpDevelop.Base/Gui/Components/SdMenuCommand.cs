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
	public class SdMenuCommand : CommandBarButton, IStatusUpdate
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
		object caller;
		ConditionCollection conditionCollection;
		string description   = String.Empty;
		string localizedText = String.Empty;
		ICommand menuCommand = null;
		
		public ICommand Command {
			get {
				return menuCommand;
			}
			set {
				menuCommand = value;
				UpdateStatus();
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
		
		public SdMenuCommand(ConditionCollection conditionCollection, object caller, string label) : base(stringParserService.Parse(label))
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			this.localizedText       = label;
			UpdateStatus();
		}
		
		public SdMenuCommand(ConditionCollection conditionCollection, object caller, string label, ICommand menuCommand) : base(stringParserService.Parse(label))
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			this.localizedText       = label;
			this.menuCommand = menuCommand;
			UpdateStatus();
		}
		
		public SdMenuCommand(ConditionCollection conditionCollection, object caller, string label, EventHandler handler) : base(stringParserService.Parse(label), handler)
		{
			this.caller = caller;
			this.conditionCollection = conditionCollection;
			this.localizedText       = label;
			UpdateStatus();
		}
		
		public SdMenuCommand(object caller, string label, EventHandler handler) : base(stringParserService.Parse(label), handler)
		{
			this.caller = caller;
			this.localizedText       = label;
			UpdateStatus();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand != null) {
				menuCommand.Run();
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.IsEnabled = failedAction != ConditionFailedAction.Disable;
				this.IsVisible = failedAction != ConditionFailedAction.Exclude;
			}
			if (menuCommand != null && menuCommand is IMenuCommand) {
				IsEnabled &= ((IMenuCommand)menuCommand).IsEnabled;
			}
			Text = stringParserService.Parse(localizedText);
		}
	}
}
