#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for LineScatterLayerContentsControl.
  /// </summary>
  public class LineScatterLayerContentsControl : System.Windows.Forms.UserControl, ILineScatterLayerContentsView
  {
    private ILineScatterLayerContentsController m_Ctrl;
    private System.Windows.Forms.CheckBox m_Contents_chkShowRange;
    private System.Windows.Forms.Button m_Contents_btEditRange;
    private System.Windows.Forms.Button m_Contents_btUngroup;
    private System.Windows.Forms.Button m_Contents_btGroup;
    private System.Windows.Forms.Button m_Contents_btPlotAssociations;
    private System.Windows.Forms.Button m_Contents_btListSelDown;
    private System.Windows.Forms.Button m_Contents_btListSelUp;
    private MWControlSuite.MWTreeView   m_Contents_lbContents;
    private System.Windows.Forms.Button m_Contents_btPullData;
    private System.Windows.Forms.Button m_Contents_btPutData;
    private System.Windows.Forms.Label label14;
    private MWControlSuite.MWTreeView m_Content_tvDataAvail;
    private System.Windows.Forms.Label label13;
    private SplitContainer _splitContainer;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public LineScatterLayerContentsControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();


    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LineScatterLayerContentsControl));
      this.m_Contents_chkShowRange = new System.Windows.Forms.CheckBox();
      this.m_Contents_btEditRange = new System.Windows.Forms.Button();
      this.m_Contents_btUngroup = new System.Windows.Forms.Button();
      this.m_Contents_btGroup = new System.Windows.Forms.Button();
      this.m_Contents_btPlotAssociations = new System.Windows.Forms.Button();
      this.m_Contents_btListSelDown = new System.Windows.Forms.Button();
      this.m_Contents_btListSelUp = new System.Windows.Forms.Button();
      this.m_Contents_lbContents = new MWControlSuite.MWTreeView();
      this.m_Contents_btPullData = new System.Windows.Forms.Button();
      this.m_Contents_btPutData = new System.Windows.Forms.Button();
      this.label14 = new System.Windows.Forms.Label();
      this.m_Content_tvDataAvail = new MWControlSuite.MWTreeView();
      this.label13 = new System.Windows.Forms.Label();
      this._splitContainer = new System.Windows.Forms.SplitContainer();
      this._splitContainer.Panel1.SuspendLayout();
      this._splitContainer.Panel2.SuspendLayout();
      this._splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_Contents_chkShowRange
      // 
      this.m_Contents_chkShowRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_chkShowRange.Location = new System.Drawing.Point(147, 240);
      this.m_Contents_chkShowRange.Name = "m_Contents_chkShowRange";
      this.m_Contents_chkShowRange.Size = new System.Drawing.Size(104, 24);
      this.m_Contents_chkShowRange.TabIndex = 25;
      this.m_Contents_chkShowRange.Text = "Show Range";
      this.m_Contents_chkShowRange.CheckedChanged += new System.EventHandler(this.EhShowRange_CheckedChanged);
      // 
      // m_Contents_btEditRange
      // 
      this.m_Contents_btEditRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btEditRange.Location = new System.Drawing.Point(147, 208);
      this.m_Contents_btEditRange.Name = "m_Contents_btEditRange";
      this.m_Contents_btEditRange.Size = new System.Drawing.Size(104, 24);
      this.m_Contents_btEditRange.TabIndex = 24;
      this.m_Contents_btEditRange.Text = "Edit Range...";
      this.m_Contents_btEditRange.Click += new System.EventHandler(this.EhEditRange_Click);
      // 
      // m_Contents_btUngroup
      // 
      this.m_Contents_btUngroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btUngroup.Location = new System.Drawing.Point(147, 176);
      this.m_Contents_btUngroup.Name = "m_Contents_btUngroup";
      this.m_Contents_btUngroup.Size = new System.Drawing.Size(104, 24);
      this.m_Contents_btUngroup.TabIndex = 23;
      this.m_Contents_btUngroup.Text = "Ungroup";
      this.m_Contents_btUngroup.Click += new System.EventHandler(this.EhUngroup_Click);
      // 
      // m_Contents_btGroup
      // 
      this.m_Contents_btGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btGroup.Location = new System.Drawing.Point(147, 144);
      this.m_Contents_btGroup.Name = "m_Contents_btGroup";
      this.m_Contents_btGroup.Size = new System.Drawing.Size(104, 24);
      this.m_Contents_btGroup.TabIndex = 22;
      this.m_Contents_btGroup.Text = "Group";
      this.m_Contents_btGroup.Click += new System.EventHandler(this.EhGroup_Click);
      // 
      // m_Contents_btPlotAssociations
      // 
      this.m_Contents_btPlotAssociations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btPlotAssociations.Location = new System.Drawing.Point(147, 112);
      this.m_Contents_btPlotAssociations.Name = "m_Contents_btPlotAssociations";
      this.m_Contents_btPlotAssociations.Size = new System.Drawing.Size(104, 24);
      this.m_Contents_btPlotAssociations.TabIndex = 21;
      this.m_Contents_btPlotAssociations.Text = "PlotAssociations...";
      this.m_Contents_btPlotAssociations.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.m_Contents_btPlotAssociations.Click += new System.EventHandler(this.EhPlotAssociations_Click);
      // 
      // m_Contents_btListSelDown
      // 
      this.m_Contents_btListSelDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btListSelDown.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btListSelDown.Image")));
      this.m_Contents_btListSelDown.Location = new System.Drawing.Point(147, 72);
      this.m_Contents_btListSelDown.Name = "m_Contents_btListSelDown";
      this.m_Contents_btListSelDown.Size = new System.Drawing.Size(32, 32);
      this.m_Contents_btListSelDown.TabIndex = 20;
      this.m_Contents_btListSelDown.Click += new System.EventHandler(this.EhListSelDown_Click);
      // 
      // m_Contents_btListSelUp
      // 
      this.m_Contents_btListSelUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btListSelUp.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btListSelUp.Image")));
      this.m_Contents_btListSelUp.Location = new System.Drawing.Point(147, 32);
      this.m_Contents_btListSelUp.Name = "m_Contents_btListSelUp";
      this.m_Contents_btListSelUp.Size = new System.Drawing.Size(32, 32);
      this.m_Contents_btListSelUp.TabIndex = 19;
      this.m_Contents_btListSelUp.Click += new System.EventHandler(this.EhListSelUp_Click);
      // 
      // m_Contents_lbContents
      // 
      this.m_Contents_lbContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_lbContents.CheckedNodes = ((System.Collections.Hashtable)(resources.GetObject("m_Contents_lbContents.CheckedNodes")));
      this.m_Contents_lbContents.HideSelection = false;
      this.m_Contents_lbContents.Location = new System.Drawing.Point(0, 24);
      this.m_Contents_lbContents.Name = "m_Contents_lbContents";
      this.m_Contents_lbContents.RubberbandGradientBlend = new MWControlSuite.MWRubberbandGradientBlend[0];
      this.m_Contents_lbContents.RubberbandGradientColorBlend = new MWControlSuite.MWRubberbandGradientColorBlend[0];
      this.m_Contents_lbContents.SelNodes = ((System.Collections.Hashtable)(resources.GetObject("m_Contents_lbContents.SelNodes")));
      this.m_Contents_lbContents.Size = new System.Drawing.Size(136, 240);
      this.m_Contents_lbContents.TabIndex = 18;
      this.m_Contents_lbContents.DoubleClick += new System.EventHandler(this.EhContents_DoubleClick);
      // 
      // m_Contents_btPullData
      // 
      this.m_Contents_btPullData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btPullData.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btPullData.Image")));
      this.m_Contents_btPullData.Location = new System.Drawing.Point(163, 72);
      this.m_Contents_btPullData.Name = "m_Contents_btPullData";
      this.m_Contents_btPullData.Size = new System.Drawing.Size(32, 32);
      this.m_Contents_btPullData.TabIndex = 17;
      this.m_Contents_btPullData.Click += new System.EventHandler(this.EhPullData_Click);
      // 
      // m_Contents_btPutData
      // 
      this.m_Contents_btPutData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Contents_btPutData.Image = ((System.Drawing.Image)(resources.GetObject("m_Contents_btPutData.Image")));
      this.m_Contents_btPutData.Location = new System.Drawing.Point(163, 32);
      this.m_Contents_btPutData.Name = "m_Contents_btPutData";
      this.m_Contents_btPutData.Size = new System.Drawing.Size(32, 32);
      this.m_Contents_btPutData.TabIndex = 16;
      this.m_Contents_btPutData.Click += new System.EventHandler(this.EhPutData_Click);
      // 
      // label14
      // 
      this.label14.Location = new System.Drawing.Point(0, 8);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(88, 16);
      this.label14.TabIndex = 15;
      this.label14.Text = "XYPlotLayer Contents";
      // 
      // m_Content_tvDataAvail
      // 
      this.m_Content_tvDataAvail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Content_tvDataAvail.CheckedNodes = ((System.Collections.Hashtable)(resources.GetObject("m_Content_tvDataAvail.CheckedNodes")));
      this.m_Content_tvDataAvail.HideSelection = false;
      this.m_Content_tvDataAvail.Location = new System.Drawing.Point(3, 24);
      this.m_Content_tvDataAvail.Name = "m_Content_tvDataAvail";
      this.m_Content_tvDataAvail.RubberbandGradientBlend = new MWControlSuite.MWRubberbandGradientBlend[0];
      this.m_Content_tvDataAvail.RubberbandGradientColorBlend = new MWControlSuite.MWRubberbandGradientColorBlend[0];
      this.m_Content_tvDataAvail.SelNodes = ((System.Collections.Hashtable)(resources.GetObject("m_Content_tvDataAvail.SelNodes")));
      this.m_Content_tvDataAvail.Size = new System.Drawing.Size(152, 240);
      this.m_Content_tvDataAvail.TabIndex = 14;
      this.m_Content_tvDataAvail.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.EhDataAvailable_BeforeExpand);
      // 
      // label13
      // 
      this.label13.Location = new System.Drawing.Point(3, 8);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(80, 16);
      this.label13.TabIndex = 13;
      this.label13.Text = "Available data";
      // 
      // _splitContainer
      // 
      this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this._splitContainer.Location = new System.Drawing.Point(0, 0);
      this._splitContainer.Name = "_splitContainer";
      // 
      // _splitContainer.Panel1
      // 
      this._splitContainer.Panel1.Controls.Add(this.m_Content_tvDataAvail);
      this._splitContainer.Panel1.Controls.Add(this.label13);
      this._splitContainer.Panel1.Controls.Add(this.m_Contents_btPutData);
      this._splitContainer.Panel1.Controls.Add(this.m_Contents_btPullData);
      // 
      // _splitContainer.Panel2
      // 
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_lbContents);
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_chkShowRange);
      this._splitContainer.Panel2.Controls.Add(this.label14);
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_btEditRange);
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_btListSelUp);
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_btUngroup);
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_btListSelDown);
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_btGroup);
      this._splitContainer.Panel2.Controls.Add(this.m_Contents_btPlotAssociations);
      this._splitContainer.Size = new System.Drawing.Size(456, 271);
      this._splitContainer.SplitterDistance = 196;
      this._splitContainer.TabIndex = 26;
      // 
      // LineScatterLayerContentsControl
      // 
      this.Controls.Add(this._splitContainer);
      this.Name = "LineScatterLayerContentsControl";
      this.Size = new System.Drawing.Size(456, 271);
      this._splitContainer.Panel1.ResumeLayout(false);
      this._splitContainer.Panel2.ResumeLayout(false);
      this._splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    #region ILineScatterLayerContentsView Members

    public ILineScatterLayerContentsController Controller
    {
      get
      {
        return m_Ctrl;
      }
      set
      {
        m_Ctrl = value;
      }
    }

    public Form Form
    {
      get
      {
        return this.ParentForm;
      }
    }

 

    void Update(MWControlSuite.MWTreeView view, TreeNodeCollection tree, NGTreeNodeCollection coll, Hashtable selectedNGNodes)
    {
      if (tree.Count != 0)
        throw new ApplicationException("Tree must be empty here");

      for (int i = 0; i < coll.Count; i++)
      {
        bool isExpanded = false;
       

        if (null != coll[i].GuiTag)
        {
          TreeNode node = (TreeNode)coll[i].GuiTag;
          isExpanded = node.IsExpanded;
         
          node.Remove();
          node.Nodes.Clear();
          tree.Add(node);
          
        }
        else
        {
          tree.Add(NewNode(coll[i]));
        }

        if (coll[i].Nodes.Count > 0)
          Update(view,tree[i].Nodes, coll[i].Nodes,selectedNGNodes);

        if (isExpanded)
          tree[i].Expand();
        if (null != selectedNGNodes && selectedNGNodes.ContainsKey(coll[i]))
        {
          view.SelectNode(tree[i], false);
          if (tree[i].Parent != null && !(tree[i].Parent.IsExpanded))
            tree[i].Parent.Expand();
        }
      }
    }

    TreeNode NewNode(NGTreeNode node)
    {
      TreeNode tnode = new TreeNode();
      tnode.Text = node.Text;
      tnode.Tag = node;
      node.GuiTag = tnode;
      return tnode;
    }


    Hashtable SelectedNGNodes(MWControlSuite.MWTreeView view)
    {
      Hashtable result = new Hashtable();
      Hashtable selected = view.SelNodes;
      foreach (DictionaryEntry en in selected)
      {
        result.Add(((MWControlSuite.MWTreeNodeWrapper)en.Value).Node.Tag, null);
      }
      return result;
    }



  

    public void DataAvailable_Initialize(NGTreeNodeCollection nodes)
    {
      this.m_Content_tvDataAvail.BeginUpdate();
      // Clear the TreeView each time the method is called.
      this.m_Content_tvDataAvail.Nodes.Clear();

      Update(this.m_Content_tvDataAvail,this.m_Content_tvDataAvail.Nodes, nodes,null);

      this.m_Content_tvDataAvail.EndUpdate();
    }

    public void DataAvailable_ClearSelection()
    {
      this.m_Content_tvDataAvail.ClearSelNodes();
    }
  
    public void Contents_SetItems(NGTreeNodeCollection items)
    {
      // please note:
      // every time we change the count, we have to remove all items and add them again
      // this is because the group items have another heigth, but the MeasureItem routine is
      // only called once when the item is added into the list
      bool isFirstTime = this.m_Contents_lbContents.Nodes.Count == 0;
      this.m_Contents_lbContents.BeginUpdate();
      Hashtable selNGNodes = SelectedNGNodes(this.m_Contents_lbContents);
      this.m_Contents_lbContents.ClearSelNodes();
      this.m_Contents_lbContents.Nodes.Clear();
      Update(this.m_Contents_lbContents,this.m_Contents_lbContents.Nodes, items,selNGNodes);

      if (isFirstTime) // Expand all root items if this is the first time
      {
        for (int i = 0; i < m_Contents_lbContents.Nodes.Count; i++)
          m_Contents_lbContents.Nodes[i].Expand();
      }
      
      this.m_Contents_lbContents.EndUpdate();
      this.m_Contents_lbContents.Focus();



    }

    public void Contents_RemoveItems(NGTreeNode[] items)
    {
      this.m_Contents_lbContents.BeginUpdate();

      foreach (NGTreeNode node in items)
        m_Contents_lbContents.RemoveNode((TreeNode)node.GuiTag);

      this.m_Contents_lbContents.EndUpdate();
    }

    public void Contents_SetSelected(int idx, bool bSelect)
    {
      //this.m_Contents_lbContents.SetSelected(idx,bSelect);
    }

    public void Contents_InvalidateItems(int idx1, int idx2)
    {
      //this.m_Contents_lbContents.Items[idx1] = idx1;
      //this.m_Contents_lbContents.Items[idx2] = idx2;

    }

    #endregion
  
    private void EhDataAvailable_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
    {
      if(null!=Controller)
      {
        this.m_Content_tvDataAvail.BeginUpdate();
        NGTreeNode ngnode = (NGTreeNode)e.Node.Tag;
        Controller.EhView_DataAvailableBeforeExpand(ngnode);
        e.Node.Nodes.Clear();
        Update(this.m_Content_tvDataAvail,e.Node.Nodes, ngnode.Nodes,null);

        this.m_Content_tvDataAvail.EndUpdate();
      }
    }

    private void EhContents_DoubleClick(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
       
        if(this.m_Contents_lbContents.SelNodes.Count==1)
        {
          foreach (DictionaryEntry dict in this.m_Contents_lbContents.SelNodes)
          {
            Controller.EhView_ContentsDoubleClick(((MWControlSuite.MWTreeNodeWrapper)dict.Value).Node.Tag as NGTreeNode);
          }
        }
        this.m_Contents_lbContents.Focus();
      }
    }


    NGTreeNode[] SelectedNodes(MWControlSuite.MWTreeView tree)
    {
      Hashtable hash = tree.SelNodes;
      NGTreeNode[] result = new NGTreeNode[hash.Count];
      int i=0;
      foreach (DictionaryEntry dict in hash)
        result[i++] = ((MWControlSuite.MWTreeNodeWrapper)dict.Value).Node.Tag as NGTreeNode;

      NGTreeNode.SortByOrder(result);
      return result;
    }


    private void EhContents_SelectedIndexChanged(object sender, System.EventArgs e)
    {
    
    }

    private void EhPutData_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
      {
        Controller.EhView_PutData(SelectedNodes(this.m_Content_tvDataAvail));
        this.m_Contents_lbContents.Focus();
      }
    }

    private void EhPullData_Click(object sender, System.EventArgs e)
    {

      if (null != Controller)
      {
        Controller.EhView_PullDataClick(SelectedNodes(this.m_Contents_lbContents));
      }
    }

    private void EhListSelUp_Click(object sender, System.EventArgs e)
    {

      if (null != Controller)
      {
        Controller.EhView_ListSelUpClick(SelectedNodes(this.m_Contents_lbContents));
        this.m_Contents_lbContents.Focus();
      }
      
    }

    private void EhListSelDown_Click(object sender, System.EventArgs e)
    {

      if (null != Controller)
      {
        Controller.EhView_SelDownClick(SelectedNodes(this.m_Contents_lbContents));
        this.m_Contents_lbContents.Focus();
      }
      
    }

    private void EhPlotAssociations_Click(object sender, System.EventArgs e)
    {

      if (null != Controller)
      {
        Controller.EhView_PlotAssociationsClick(SelectedNodes(this.m_Contents_lbContents));
        this.m_Contents_lbContents.Focus();
      }
      
    }

    private void EhGroup_Click(object sender, System.EventArgs e)
    {

      if (null != Controller)
      {
        Controller.EhView_GroupClick(SelectedNodes(this.m_Contents_lbContents));
        this.m_Contents_lbContents.Focus();
      }
      
    
    }

    private void EhUngroup_Click(object sender, System.EventArgs e)
    {

      if (null != Controller)
      {
        Controller.EhView_UngroupClick(SelectedNodes(this.m_Contents_lbContents));
        this.m_Contents_lbContents.Focus();
      }
      
    }

    private void EhEditRange_Click(object sender, System.EventArgs e)
    {

      if (null != Controller)
      {
        Controller.EhView_EditRangeClick(SelectedNodes(this.m_Contents_lbContents));
        this.m_Contents_lbContents.Focus();
      }
      
    }

    private void EhShowRange_CheckedChanged(object sender, System.EventArgs e)
    {
    
    }

    #region IMVCView Members

    public object ControllerObject
    {
      get
      {
        return Controller;
      }
      set
      {
        Controller = value as ILineScatterLayerContentsController;
      }
    }


    #endregion
  }
}
