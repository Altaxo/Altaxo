// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3805 $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	sealed class ToolBarComboBox : ComboBox
	{
		IComboBoxCommand menuCommand;
		
		public ToolBarComboBox(Codon codon, object caller)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.IsEditable = false;
			menuCommand = (IComboBoxCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			menuCommand.ComboBox = this;
			menuCommand.Owner = caller;
			
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.ComboBoxStyleKey);
		}
		
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			menuCommand.Run();
		}
	}
}
