using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
	public class SingleChoiceComboBoxControl : ComboBox
	{
		SelectableListNodeList _choices;

		public void Initialize(SelectableListNodeList choices)
		{
			this.SelectionChanged -= EhSelectionChanged; // prevent firing event here

			_choices = choices;
			this.Items.Clear();
			foreach (var choice in _choices)
			{
				var rb = new ComboBoxItem();
				rb.Content = choice.Name;
				rb.Tag = choice;
				Items.Add(rb);
			}
			int selIndex = _choices.FirstSelectedNodeIndex;
			if(selIndex>=0)
				this.SelectedIndex = selIndex;

			this.SelectionChanged += EhSelectionChanged; // now allow event again
		}

		void EhSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_choices.ClearSelectionsAll();
			if (SelectedIndex >= 0)
				_choices[SelectedIndex].Selected = true;
		}
	}
}
