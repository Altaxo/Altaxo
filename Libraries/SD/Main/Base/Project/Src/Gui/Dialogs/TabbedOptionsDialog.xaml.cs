﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5529 $</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Displays TabbedOptions in a dialog.
	/// </summary>
	public partial class TabbedOptionsDialog : Window
	{
		TabbedOptions tabbedOptions = new TabbedOptions();
		
		public TabbedOptionsDialog(IEnumerable<IOptionPanelDescriptor> optionPanels)
		{
			InitializeComponent();
			tabbedOptions.AddOptionPanels(optionPanels);
			grid.Children.Add(tabbedOptions);
		}
		
		void okButtonClick(object sender, RoutedEventArgs e)
		{
			foreach (IOptionPanel op in tabbedOptions.OptionPanels) {
				if (!op.SaveOptions())
					return;
			}
			this.DialogResult = true;
			Close();
		}
		
		void cancelButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			foreach (IDisposable op in tabbedOptions.OptionPanels.OfType<IDisposable>()) {
				op.Dispose();
			}
		}
	}
}
