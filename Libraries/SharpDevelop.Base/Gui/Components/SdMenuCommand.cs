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
using ICSharpCode.SharpDevelop.Services;

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
		
		public override bool IsVisible {
			get {
				bool isVisible = base.IsVisible;
				if (conditionCollection != null) {
					ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
					isVisible &= failedAction != ConditionFailedAction.Exclude;
				}
				return isVisible;
			}
			set {
				base.IsVisible = value;
			}
		}
		
		public override bool IsEnabled {
			get {
				bool isEnabled = true; //base.IsEnabled;
				if (conditionCollection != null) {
					ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
					isEnabled &= failedAction != ConditionFailedAction.Disable;
				}
				if (menuCommand != null && menuCommand is IMenuCommand) {
					isEnabled &= ((IMenuCommand)menuCommand).IsEnabled;
				}
				return isEnabled;
			}
			set {
				base.IsEnabled = value;
			}
		}
		
//		protected override void OnSelect(System.EventArgs e)
//		{
//			base.OnSelect(e);
//			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
//			statusBarService.SetMessage(description);
//		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				bool isVisible = failedAction != ConditionFailedAction.Exclude;
				if (base.IsVisible != isVisible) {
					base.IsVisible = isVisible;
				}
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				if (base.IsEnabled != isEnabled) {
					base.IsEnabled = isEnabled;
				}
			}
			if (menuCommand != null && menuCommand is IMenuCommand) {
				bool isEnabled = IsEnabled & ((IMenuCommand)menuCommand).IsEnabled;
				if (base.IsEnabled != isEnabled) {
					base.IsEnabled = isEnabled;
				}
			}
			Text = stringParserService.Parse(localizedText);
		}
	}
}
