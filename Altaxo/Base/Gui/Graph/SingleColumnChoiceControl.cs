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
using Altaxo.Gui;



namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for SingleColumnChoiceControl.
  /// </summary>
  [UserControlForController(typeof(ISingleColumnChoiceViewEventSink))]
  public class SingleColumnChoiceControl : System.Windows.Forms.UserControl, ISingleColumnChoiceView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public SingleColumnChoiceControl()
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
      this._tvColumns = new System.Windows.Forms.TreeView();
      this.SuspendLayout();
      // 
      // _tvColumns
      // 
      this._tvColumns.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tvColumns.ImageIndex = -1;
      this._tvColumns.Location = new System.Drawing.Point(0, 0);
      this._tvColumns.Name = "_tvColumns";
      this._tvColumns.SelectedImageIndex = -1;
      this._tvColumns.Size = new System.Drawing.Size(256, 296);
      this._tvColumns.TabIndex = 0;
      this._tvColumns.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._tvColumns_AfterSelect);
      this._tvColumns.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._tvColumns_BeforeExpand);
      // 
      // SingleColumnChoiceControl
      // 
      this.Controls.Add(this._tvColumns);
      this.Name = "SingleColumnChoiceControl";
      this.Size = new System.Drawing.Size(256, 296);
      this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.TreeView _tvColumns;

    #region ISingleColumnChoiceView Members

    ISingleColumnChoiceViewEventSink _controller;
    public ISingleColumnChoiceViewEventSink Controller
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

    public void Initialize(TreeNode[] nodes)
    {
      this._tvColumns.BeginUpdate();
      // Clear the TreeView each time the method is called.
      this._tvColumns.Nodes.Clear();

      this._tvColumns.Nodes.AddRange(nodes);

      this._tvColumns.EndUpdate();
    }

    public void SelectNode(TreeNode node)
    {
      node.EnsureVisible();
      this._tvColumns.SelectedNode = node;
    }

    #endregion

    private void _tvColumns_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
    {
      if(_controller != null)
        _controller.EhView_AfterSelectNode(e.Node);
    }

    private void _tvColumns_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
    {
      if(_controller != null)
        _controller.EhView_BeforeExpand(e.Node);
    }
  }
}
