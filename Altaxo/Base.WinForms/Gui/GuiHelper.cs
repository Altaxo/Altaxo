using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Altaxo.Collections;

namespace Altaxo.Gui
{
	public static class GuiHelper
	{
		#region Old collection interfaces

		#region Combobox

		public static void UpdateList(ComboBox comboBox, SelectableListNodeList items)
		{
			comboBox.SelectedIndexChanged -= EhComboBoxSelectionChanged;
			comboBox.BeginUpdate();
			comboBox.Items.Clear();
			for (int i = 0; i < items.Count; i++)
			{
				comboBox.Items.Add(items[i]);
				if (items[i].Selected)
					comboBox.SelectedIndex = i;
			}
			comboBox.EndUpdate();
			comboBox.SelectedIndexChanged += EhComboBoxSelectionChanged;
		}

		public static void SynchronizeSelectionFromGui(ComboBox comboBox)
		{
			for (int i = 0; i < comboBox.Items.Count; i++)
				((SelectableListNode)comboBox.Items[i]).Selected = (comboBox.SelectedIndex == i);
		}

		private static void EhComboBoxSelectionChanged(object sender, EventArgs e)
		{
			SynchronizeSelectionFromGui((ComboBox)sender);
		}

		#endregion

		#endregion

		#region New collection interfaces
#if(false)
		#region Combobox

		public static void UpdateList(ComboBox comboBox, SelectableListNodeList<SelectableListNode> items)
		{
			comboBox.SelectedIndexChanged -= EhComboBoxSelectionChanged;
			comboBox.BeginUpdate();
			comboBox.Items.Clear();
			for (int i = 0; i < items.Count; i++)
			{
				comboBox.Items.Add(items[i]);
				if (items[i].Selected)
					comboBox.SelectedIndex = i;
			}
			comboBox.EndUpdate();
			comboBox.SelectedIndexChanged += EhComboBoxSelectionChanged;
		}

		public static void SynchronizeSelectionFromGui(ComboBox comboBox)
		{
			for (int i = 0; i < comboBox.Items.Count; i++)
				((SelectableListNode)comboBox.Items[i]).Selected = (comboBox.SelectedIndex == i);
		}

		private static void EhComboBoxSelectionChanged(object sender, EventArgs e)
		{
			SynchronizeSelectionFromGui((ComboBox)sender);
		}

		#endregion

		#region ListBox

		public static void UpdateList(ListBox listView, SelectableListNodeList<SelectableListNode> items)
		{
			listView.SelectedIndexChanged -= EhListBoxSelectedIndexChanged;
			listView.BeginUpdate();
			listView.Items.Clear();
			for (int i = 0; i < items.Count; i++)
			{
				listView.Items.Add(items[i]);
				if (items[i].Selected)
					listView.SelectedIndex = i;
			}

			listView.EndUpdate();
			listView.SelectedIndexChanged += EhListBoxSelectedIndexChanged;
		}

		private static void EhListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			SynchronizeSelectionFromGui((ListBox)sender);
		}

		public static void SynchronizeSelectionFromGui(ListBox gui)
		{
			int j = gui.SelectedIndices.Count == 0 ? -1 : 0;
			for (int i = 0; i < gui.Items.Count; i++)
			{
				bool match = i == (j < 0 ? -1 : gui.SelectedIndices[j]);
				((SelectableListNode)gui.Items[i]).Selected = match;
				if (match && j >= 0)
				{
					j++;
					if (j >= gui.SelectedIndices.Count)
						j = -1;
				}
			}
		}

		#endregion

		#region ListView

		public static void UpdateList( ListView listView, SelectableListNodeList<SelectableListNode> items)
    {
      listView.BeginUpdate();

     if (listView.Items.Count > items.Count)
      {
        // delete some items
        for (int i = listView.Items.Count - 1; i >= items.Count; --i)
          listView.Items.RemoveAt(i);
      }

      // Update all listitems now
      for (int i = 0; i < items.Count; i++)
      {
        ListViewItem litem;

        if (listView.Items.Count <= i)
        {
          litem = new ListViewItem(string.Empty);
          for(int j=0;j<items[i].SubItemCount;j++)
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, string.Empty));
          listView.Items.Add(litem);
        }
        else
        {
          litem = listView.Items[i];
        }

        litem.Text = items[i].Name;
				litem.ToolTipText = items[i].Description;
				for (int j = 0; j < items[i].SubItemCount; j++)
				{
					litem.SubItems[j + 1].Text = items[i].SubItemText(j);
					System.Drawing.Color? col = items[i].SubItemBackColor(j);
					if (col == null)
					{
						litem.SubItems[j + 1].BackColor = litem.BackColor;
					}
					else
					{
						litem.UseItemStyleForSubItems = false;
						litem.SubItems[j + 1].BackColor = (System.Drawing.Color)col;
					}
				}
        litem.Tag = items[i];
        litem.Selected = items[i].Selected;
      }
      listView.EndUpdate();

    }

	

    public static void SynchronizeSelectionFromGui(ListView listView, object items)
    {
      foreach (ListViewItem lvitem in listView.Items)
      {
        if (lvitem.Tag is SelectableListNode)
        {
          ((SelectableListNode)lvitem.Tag).Selected = lvitem.Selected;
        }
      }
		}

		#endregion

#endif

		#endregion
	}
}
