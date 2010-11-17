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
    #region TreeNode

    /// <summary>
    /// Mirrors a Non-Gui-TreeNode to a gui dependent tree node - including all childs and including the parent relation (the parent node has to be already mirrored.
    /// </summary>
    /// <param name="orgNode"></param>
    /// <returns></returns>
    public static TreeNode MirrorTreeNodeAdded(NGTreeNode orgNode)
    {
      // Do not change the order of the creation - it is essential for recursive operation

      // First create the GuiNode itself
      TreeNode guiNode = new TreeNode(orgNode.Text);
      guiNode.Tag = orgNode;
      orgNode.GuiTag = guiNode;

      // second - create the relation to the parent node
      if (null != orgNode.Parent && null == guiNode.Parent && (orgNode.Parent.GuiTag is TreeNode))
        (orgNode.Parent.GuiTag as TreeNode).Nodes.Add(guiNode);

      // third - create the children - they will already see the created parent
      if (orgNode.HasChilds)
      {
        foreach (var childNode in orgNode.Nodes)
          MirrorTreeNodeAdded(childNode);
      }
      return guiNode;
    }

    /// <summary>
    /// Mirrors an array of Non-Gui-TreeNode to a gui dependent tree node - including all childs and including the parent relation (the parent node has to be already mirrored.
    /// </summary>
    /// <param name="orgNodes">Array of Non-Gui-TreeNodes.</param>
    /// <returns>Array of Gui-TreeNodes</returns>
    public static TreeNode[] MirrorTreeNodesAdded(NGTreeNode[] orgNodes)
    {
      TreeNode[] guiNodes = new TreeNode[orgNodes.Length];
      for (int i = 0; i < guiNodes.Length; i++)
        guiNodes[i] = MirrorTreeNodeAdded(orgNodes[i]);

      return guiNodes;
    }

    public static void MirrorTreeNodeRemoved(NGTreeNode orgNode)
    {
      if (orgNode.HasChilds)
      {
        foreach (NGTreeNode child in orgNode.Nodes)
          MirrorTreeNodeRemoved(child);
      }

      var guiNode = (orgNode.GuiTag as TreeNode);
      if (null != guiNode)
      {
        orgNode.GuiTag = null;
        guiNode.Tag = null;
        guiNode.Remove();
      }
    }


 
    #endregion
  
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

    #region ListBox

		public static void UpdateList(ListBox listView, SelectableListNodeList items)
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

		public static void UpdateList( ListView listView, SelectableListNodeList items)
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
				litem.ImageIndex = items[i].ImageIndex;
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

	

    public static void SynchronizeSelectionFromGui(ListView listView)
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

  }
}
