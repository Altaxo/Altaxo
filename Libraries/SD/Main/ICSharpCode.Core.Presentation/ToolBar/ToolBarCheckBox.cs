﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5203 $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ICSharpCode.Core.Presentation
{
	sealed class ToolBarCheckBox : CheckBox, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		BindingExpressionBase isCheckedBinding;
		
		public ToolBarCheckBox(Codon codon, object caller)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;
			this.Command = CommandWrapper.GetCommand(codon, caller, true);
			CommandWrapper wrapper = this.Command as CommandWrapper;
			if (wrapper != null) {
				ICheckableMenuCommand cmd = wrapper.GetAddInCommand() as ICheckableMenuCommand;
				if (cmd != null) {
#if ModifiedForAltaxo
					isCheckedBinding = SetBinding(IsCheckedProperty, new Binding("IsChecked") { Source = cmd, Mode = BindingMode.TwoWay });
#else
					isCheckedBinding = SetBinding(IsCheckedProperty, new Binding("IsChecked") { Source = cmd, Mode = BindingMode.OneWay });
#endif
				}
			}
			
			if (codon.Properties.Contains("icon")) {
				var image = PresentationResourceService.GetImage(StringParser.Parse(codon.Properties["icon"]));
				image.Height = 16;
				image.SetResourceReference(StyleProperty, ToolBarService.ImageStyleKey);
				this.Content = image;
			} else {
				this.Content = codon.Id;
			}
			UpdateText();
			
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.CheckBoxStyleKey);
		}
		
		public void UpdateText()
		{
			if (codon.Properties.Contains("tooltip")) {
				this.ToolTip = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
		
		public void UpdateStatus()
		{
			if (codon.GetFailedAction(caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
		
		protected override void OnClick()
		{
			base.OnClick();
			isCheckedBinding.UpdateTarget();
		}
	}
}
