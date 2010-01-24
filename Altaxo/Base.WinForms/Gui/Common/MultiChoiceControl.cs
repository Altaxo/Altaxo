using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Common
{
	public partial class MultiChoiceControl : UserControl, IMultiChoiceView
	{
		public MultiChoiceControl()
		{
			InitializeComponent();
		}

		#region IMultiChoiceView Members

		public void InitializeDescription(string value)
		{
			_edDescription.Text = value;
		}

		public void InitializeListColumns(string[] colNames)
		{
			_lvItems.Columns.Clear();
			foreach (var name in colNames)
				_lvItems.Columns.Add(name);
		}

		public void InitializeList(Altaxo.Collections.SelectableListNodeList list)
		{
			GuiHelper.UpdateList(_lvItems, list);
		}

		#endregion

		private void EhList_IndexSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_lvItems);
		}
	}
}
