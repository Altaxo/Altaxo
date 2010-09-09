using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;

using Altaxo.Collections;


namespace Altaxo.Gui.Common
{
	public class SingleChoiceRadioStackPanel : StackPanel
	{
		SelectableListNodeList _choices;

		public void Initialize(SelectableListNodeList choices)
		{
			_choices = choices;
			Children.Clear();
			foreach (var choice in _choices)
			{
				var rb = new RadioButton();
				rb.Content = choice.Name;
				rb.Tag = choice;
				rb.IsChecked = choice.Selected;
				rb.Checked += EhRadioButtonChecked;
				Children.Add(rb);
			}

		}

		void EhRadioButtonChecked(object sender, RoutedEventArgs e)
		{
			var rb = (RadioButton)sender;
			var node = rb.Tag as SelectableListNode;
			if (node != null)
			{
				_choices.ClearSelectionsAll();
				node.Selected = true == rb.IsChecked;
			}
		}
	}
}
