using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;

using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
	public class MultipleChoiceListView : ListView
	{
		SelectableListNodeList _choices;

		public void Initialize(SelectableListNodeList choices)
		{
			this.SelectionChanged -= EhSelectionChanged; // prevent firing event here

			_choices = choices;
			this.Items.Clear();
			foreach (var choice in _choices)
			{
				var item = new ListViewItem();
				item.Content = choice.Name;
				item.Tag = choice;
				Items.Add(item);
			}
			int selIndex = _choices.FirstSelectedNodeIndex;
			if (selIndex >= 0)
				this.SelectedIndex = selIndex;

			this.SelectionChanged += EhSelectionChanged; // now allow event again
		}

		void EhSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			_choices.ClearSelectionsAll();
			if (SelectedIndex >= 0)
				_choices[SelectedIndex].IsSelected = true;
		}
	}
}
