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
	public class SdMenuComboBox : CommandBarComboBox
	{
		static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		object caller;
		ConditionCollection conditionCollection;
		string description   = String.Empty;
		ICommand menuCommand = null;
		
		public string Description
		{
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public ICommand Command
		{
			get {
				return menuCommand;
			}
			set {
				menuCommand = value;
			}
		}
		
		public SdMenuComboBox(ConditionCollection conditionCollection, object caller, ICommand menuCommand) : base(String.Empty, new ComboBox())
		{
			comboBox = new ComboBox();
			comboBox.DropDownStyle= ComboBoxStyle.DropDownList;
			comboBox.SelectionChangeCommitted += new EventHandler(selectionChanged);
			
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
			this.menuCommand = menuCommand;
		}
		
		void selectionChanged(object sender, EventArgs e)
		{
			if (menuCommand != null) {
				menuCommand.Run();
			}
		}
		
		public ComboBox ComboBox
		{
			get {
				return comboBox;
			}
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand != null) {
				menuCommand.Run();
			}
		}
		
		public override bool IsVisible
		{
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
		
		public override bool IsEnabled
		{
			get {
				bool isEnabled = true;
				if (conditionCollection != null) {
					ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
					isEnabled &= failedAction != ConditionFailedAction.Disable;
				}
				if (menuCommand != null && menuCommand is IComboBoxCommand) {
					isEnabled &= ((IComboBoxCommand)menuCommand).IsEnabled;
				}
				return isEnabled;
			}
			set {
				base.IsEnabled = value;
			}
		}
	}
}
