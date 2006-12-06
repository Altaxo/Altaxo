using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  public partial class PlotGroupCollectionControl : UserControl, IPlotGroupCollectionView
  {
    public PlotGroupCollectionControl()
    {
      InitializeComponent();
    }

    private void _cbCoordTransfoStyle_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (null != _controller)
      {
        SynchronizeComboBoxWithSelectableListNodes(_cbCoordTransfoStyle);
        _controller.EhView_CoordinateTransformingGroupStyleChanged();
      }
    }
  
    private void _btEditCSTransfoStyle_Click(object sender, EventArgs e)
    {

    }


    private void _btRemoveNormalGroupStyle_Click(object sender, EventArgs e)
    {
      if (null != _controller)
      {
        SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
        _controller.EhView_RemoveNormalGroupStyle();
      }

    }

    private void _btAddNormalGroupStyle_Click(object sender, EventArgs e)
    {
      if (null != _controller)
      {
        SynchronizeListBoxWithSelectableListNodes(_lbGroupStylesAvailable);
        _controller.EhView_AddNormalGroupStyle();
      }

    }

    private void _btIndent_Click(object sender, EventArgs e)
    {
      if (null != _controller)
      {
        SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
        _controller.EhView_IndentGroupStyle();
      }

    }

    private void _btUnindent_Click(object sender, EventArgs e)
    {
      if (null != _controller)
      {
        SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
        _controller.EhView_UnindentGroupStyle();
      }

    }

    private void _btUp_Click(object sender, EventArgs e)
    {
      if (null != _controller)
      {
        SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
        _controller.EhView_MoveUpGroupStyle();
      }

    }

    private void _btDown_Click(object sender, EventArgs e)
    {
      if (null != _controller)
      {
        SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
        _controller.EhView_MoveDownGroupStyle();
      }
    }

    #region IPlotGroupCollectionView Members

    IPlotGroupCollectionViewEventSink _controller;
    public IPlotGroupCollectionViewEventSink Controller
    {
      set { _controller = value; }
    }

    void InitializeListBox(ListBox box,SelectableListNodeList list)
    {
      box.BeginUpdate();
      box.Items.Clear();


      for (int i = 0; i < list.Count; i++)
      {
        SelectableListNode node = list[i];

        box.Items.Add(node);
        if (node.Selected)
          box.SelectedIndices.Add(i);
      }

      box.EndUpdate();
    }

    void InitializeComboBox(ComboBox box, SelectableListNodeList list)
    {
      box.BeginUpdate();
      box.Items.Clear();


      for (int i = 0; i < list.Count; i++)
      {
        SelectableListNode node = list[i];

        box.Items.Add(node);
        if (node.Selected)
          box.SelectedIndex=i;
      }

      box.EndUpdate();
    }


    void SynchronizeListBoxWithSelectableListNodes(ListBox box)
    {
      foreach (SelectableListNode node in box.Items)
        node.Selected = false;
      foreach (SelectableListNode node in box.SelectedItems)
        node.Selected = true;
    }
    void SynchronizeComboBoxWithSelectableListNodes(ComboBox box)
    {
      foreach (SelectableListNode node in box.Items)
        node.Selected = object.ReferenceEquals(node, box.SelectedItem);
    }
    void SynchronizeListBoxWithCheckableSelectableListNodes(CheckedListBox box)
    {
      foreach (CheckableSelectableListNode node in box.Items)
      { node.Selected = false; node.Checked = false; }
      foreach (CheckableSelectableListNode node in box.SelectedItems)
        node.Selected = true;
      foreach (CheckableSelectableListNode node in box.CheckedItems)
        node.Checked = true;
    }

    void InitializeCheckedListBox(CheckedListBox box, CheckableSelectableListNodeList list)
    {
      box.BeginUpdate();
      box.Items.Clear();
      for(int i=0;i<list.Count;i++)
      {
        CheckableSelectableListNode node=list[i];
      
        box.Items.Add(node, node.Checked);
        if (node.Selected)
          box.SelectedIndices.Add(i);
      }

      box.EndUpdate();
    }

    public void InitializeAvailableCoordinateTransformingGroupStyles(Altaxo.Collections.SelectableListNodeList list)
    {
      InitializeComboBox(this._cbCoordTransfoStyle, list);
    }

    public void InitializeAvailableNormalGroupStyles(Altaxo.Collections.SelectableListNodeList list)
    {
      InitializeListBox(_lbGroupStylesAvailable, list);

    }

    public void InitializeCurrentNormalGroupStyles(Altaxo.Collections.CheckableSelectableListNodeList list)
    {
      InitializeCheckedListBox(_lbGroupStyles, list);
    }

    public void InitializeUpdateMode(SelectableListNodeList list, bool inheritFromParent, bool distributeToChilds)
    {
      InitializeComboBox(_cbGroupStrictness, list);
      _chkUpdateFromParentGroups.Checked = inheritFromParent;
      _chkDistributeToSubGroups.Checked = distributeToChilds;
    }
    
    public void QueryUpdateMode(out bool inheritFromParent, out bool distributeToChilds)
    {
      SynchronizeComboBoxWithSelectableListNodes(_cbGroupStrictness);
      inheritFromParent = _chkUpdateFromParentGroups.Checked;
      distributeToChilds = _chkDistributeToSubGroups.Checked;
    }


    public void SynchronizeCurrentNormalGroupStyles()
    {
      SynchronizeListBoxWithCheckableSelectableListNodes(_lbGroupStyles);
    }

    #endregion

   


  }
}
