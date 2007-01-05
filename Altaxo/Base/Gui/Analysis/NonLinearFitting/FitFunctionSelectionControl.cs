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

using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main.Services;


namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Summary description for FitFunctionSelectionControl.
  /// </summary>
  [UserControlForController(typeof(IFitFunctionSelectionViewEventSink))] 
  public class FitFunctionSelectionControl : System.Windows.Forms.UserControl, IFitFunctionSelectionView
  {
    IFitFunctionSelectionViewEventSink _controller;
    private System.Windows.Forms.TreeView _twFitFunctions;
    private SplitContainer _splitContainer;
    private RichTextBox _rtfDescription;

    TreeNode _lastClickedNode;
    private ContextMenu _userFileLeafNodeContextMenu;
    private MenuItem menuContextEdit;
    private MenuItem menuContextRemove;
    private ContextMenu _appFileLeafNodeContextMenu;
    private MenuItem menuItem1;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public FitFunctionSelectionControl()
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
      this._twFitFunctions = new System.Windows.Forms.TreeView();
      this._splitContainer = new System.Windows.Forms.SplitContainer();
      this._rtfDescription = new System.Windows.Forms.RichTextBox();
      this._userFileLeafNodeContextMenu = new System.Windows.Forms.ContextMenu();
      this.menuContextEdit = new System.Windows.Forms.MenuItem();
      this.menuContextRemove = new System.Windows.Forms.MenuItem();
      this._appFileLeafNodeContextMenu = new System.Windows.Forms.ContextMenu();
      this.menuItem1 = new System.Windows.Forms.MenuItem();
      this._splitContainer.Panel1.SuspendLayout();
      this._splitContainer.Panel2.SuspendLayout();
      this._splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // _twFitFunctions
      // 
      this._twFitFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
      this._twFitFunctions.Location = new System.Drawing.Point(0, 0);
      this._twFitFunctions.Name = "_twFitFunctions";
      this._twFitFunctions.Size = new System.Drawing.Size(153, 344);
      this._twFitFunctions.TabIndex = 1;
      this._twFitFunctions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._twFitFunctions_AfterSelect);
      this._twFitFunctions.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._twFitFunctions_NodeMouseClick);
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
      this._splitContainer.Panel1.Controls.Add(this._twFitFunctions);
      // 
      // _splitContainer.Panel2
      // 
      this._splitContainer.Panel2.Controls.Add(this._rtfDescription);
      this._splitContainer.Size = new System.Drawing.Size(349, 344);
      this._splitContainer.SplitterDistance = 153;
      this._splitContainer.TabIndex = 2;
      // 
      // _rtfDescription
      // 
      this._rtfDescription.Cursor = System.Windows.Forms.Cursors.IBeam;
      this._rtfDescription.Dock = System.Windows.Forms.DockStyle.Fill;
      this._rtfDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this._rtfDescription.Location = new System.Drawing.Point(0, 0);
      this._rtfDescription.Name = "_rtfDescription";
      this._rtfDescription.ReadOnly = true;
      this._rtfDescription.Size = new System.Drawing.Size(192, 344);
      this._rtfDescription.TabIndex = 0;
      this._rtfDescription.Text = "";
      // 
      // _userFileLeafNodeContextMenu
      // 
      this._userFileLeafNodeContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuContextEdit,
            this.menuContextRemove});
      // 
      // menuContextEdit
      // 
      this.menuContextEdit.Index = 0;
      this.menuContextEdit.Text = "Edit";
      this.menuContextEdit.Click += new System.EventHandler(this.menuContextEdit_Click);
      // 
      // menuContextRemove
      // 
      this.menuContextRemove.Index = 1;
      this.menuContextRemove.Text = "Remove";
      this.menuContextRemove.Click += new System.EventHandler(this.menuContextRemove_Click);
      // 
      // _appFileLeafNodeContextMenu
      // 
      this._appFileLeafNodeContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
      // 
      // menuItem1
      // 
      this.menuItem1.Index = 0;
      this.menuItem1.Text = "Edit";
      // 
      // FitFunctionSelectionControl
      // 
      this.Controls.Add(this._splitContainer);
      this.Name = "FitFunctionSelectionControl";
      this.Size = new System.Drawing.Size(349, 344);
      this._splitContainer.Panel1.ResumeLayout(false);
      this._splitContainer.Panel2.ResumeLayout(false);
      this._splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    #region IFitFunctionSelectionView Members

    public IFitFunctionSelectionViewEventSink Controller 
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }

    public void SetRtfDocumentation(string rtfString)
    {
      this._rtfDescription.Rtf = rtfString;
    }

    public Color GetRtfBackgroundColor()
    {
      return this._rtfDescription.BackColor;
    }

    TreeNode GetPathNode(TreeNodeCollection coll, string path)
    {
      foreach(TreeNode node in coll)
      {
        if(node.Text==path && node.Tag==null)
          return node;
      }
      return null;
    }


    #region Node classes

    enum RootNodeType { Builtin, Document, User };

    class RootNode : TreeNode
    {
      public RootNodeType RootNodeType;

      public RootNode(string text, RootNodeType type)
        :
        base(text)
      {
        RootNodeType = type;
        this.Tag = type;
      }
    }

    class CategoryNode : TreeNode
    {
      public CategoryNode(string text) : base(text) { }
    }

    class LeafNode : TreeNode
    {
      public LeafNode(string text) : base(text) { }  
    }

    class BuiltinLeafNode : LeafNode
    {
      public object FunctionType;

      public BuiltinLeafNode(string text, object functionType)
        : base(text)
      {
        FunctionType = functionType;
        this.Tag = functionType;
      }
    }

    class DocumentLeafNode : LeafNode
    {
      public Altaxo.Main.Services.IFitFunctionInformation FunctionInstance;
      public DocumentLeafNode(string text, Altaxo.Main.Services.IFitFunctionInformation func)
        : base(text)
      {
        FunctionInstance = func;
        this.Tag = func;
      }
    }

    class UserFileLeafNode : LeafNode
    {
      public Altaxo.Main.Services.FileBasedFitFunctionInformation FunctionInfo;
      public UserFileLeafNode(string text, Altaxo.Main.Services.FileBasedFitFunctionInformation func)
        : base(text)
      {
        FunctionInfo = func;
        this.Tag = func;
      }
    }

    #endregion

    public void ClearFitFunctionList()
    {
      this._twFitFunctions.Nodes.Clear();
    }

    public void AddFitFunctionList(string rootname, Altaxo.Main.Services.IFitFunctionInformation[] entries, FitFunctionContextMenuStyle menustyle)
    {
      // The key of the entries is the FitFunctionAttribute, the value is the type of the fitting function

      this._twFitFunctions.BeginUpdate();
      

      RootNode rnode = new RootNode(rootname, RootNodeType.Builtin);
      this._twFitFunctions.Nodes.Add(rnode);
      TreeNodeCollection root = rnode.Nodes;


      foreach (Altaxo.Main.Services.IFitFunctionInformation entry in entries)
      {

        string[] path = entry.Category.Split(new char[]{'\\','/'});

        TreeNodeCollection where = root;
        for(int j=0;j<path.Length;j++)
        {
          TreeNode node = GetPathNode(where,path[j]);
          if(node==null)
          {
            node = new CategoryNode(path[j]);
            where.Add(node);
          }
          where = node.Nodes;
        }

        BuiltinLeafNode leaf = new BuiltinLeafNode(entry.Name,entry);

        switch (menustyle)
        {
          case FitFunctionContextMenuStyle.None:
            break;
          case FitFunctionContextMenuStyle.EditAndDelete:
            leaf.ContextMenu = _userFileLeafNodeContextMenu;
            break;
          case FitFunctionContextMenuStyle.Edit:
            leaf.ContextMenu = _appFileLeafNodeContextMenu;
            break;
        }
        where.Add(leaf);
      }
      this._twFitFunctions.EndUpdate();
    }


  

   
    #endregion

    private Graphics _rtfGraphics;
    private void _twFitFunctions_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_SelectionChanged(e.Node.Tag as IFitFunctionInformation);

      if (e.Node.Tag is IFitFunctionInformation)
      {
        Altaxo.Main.Services.IFitFunctionInformation info = e.Node.Tag as IFitFunctionInformation;
        if (_rtfGraphics == null)
          _rtfGraphics = this.CreateGraphics();
        _rtfDescription.Rtf = Altaxo.Main.Services.RtfComposerService.GetRtfText(info.Description, _rtfGraphics, _rtfDescription.BackColor, 12);
      }
    }

    private void EhEditItem(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_EditItem(_twFitFunctions.SelectedNode.Tag as IFitFunctionInformation);
    }

    private void menuContextEdit_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_EditItem(_lastClickedNode.Tag as IFitFunctionInformation);
    }

    private void menuContextRemove_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_RemoveItem(_lastClickedNode.Tag as IFitFunctionInformation);
    }

    private void _twFitFunctions_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      _lastClickedNode = e.Node;
    }

  }
}
