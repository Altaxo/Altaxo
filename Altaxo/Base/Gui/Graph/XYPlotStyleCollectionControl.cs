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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;


namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for XYPlotStyleCollectionControl.
  /// </summary>
  [UserControlForController(typeof(IXYPlotStyleCollectionViewEventSink))]
  public class XYPlotStyleCollectionControl : System.Windows.Forms.UserControl, IXYPlotStyleCollectionView
  {

    private System.Windows.Forms.ListBox _lbStyles;
    private System.Windows.Forms.Button _btStyleUp;
    private System.Windows.Forms.Button _btStyleDown;
    private System.Windows.Forms.Button _btStyleEdit;
    private System.Windows.Forms.Button _btStyleRemove;
    private System.Windows.Forms.Button _btAddStyle;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox _cbPredefinedStyleSets;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public XYPlotStyleCollectionControl()
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
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XYPlotStyleCollectionControl));
      this._lbStyles = new System.Windows.Forms.ListBox();
      this._btStyleUp = new System.Windows.Forms.Button();
      this._btStyleDown = new System.Windows.Forms.Button();
      this._btStyleEdit = new System.Windows.Forms.Button();
      this._btStyleRemove = new System.Windows.Forms.Button();
      this._btAddStyle = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this._cbPredefinedStyleSets = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // _lbStyles
      // 
      this._lbStyles.Location = new System.Drawing.Point(8, 136);
      this._lbStyles.Name = "_lbStyles";
      this._lbStyles.Size = new System.Drawing.Size(232, 173);
      this._lbStyles.TabIndex = 0;
      // 
      // _btStyleUp
      // 
      this._btStyleUp.Image = ((System.Drawing.Image)(resources.GetObject("_btStyleUp.Image")));
      this._btStyleUp.Location = new System.Drawing.Point(248, 168);
      this._btStyleUp.Name = "_btStyleUp";
      this._btStyleUp.Size = new System.Drawing.Size(32, 23);
      this._btStyleUp.TabIndex = 1;
      this._btStyleUp.Click += new System.EventHandler(this.EhStyleUp_Click);
      // 
      // _btStyleDown
      // 
      this._btStyleDown.Image = ((System.Drawing.Image)(resources.GetObject("_btStyleDown.Image")));
      this._btStyleDown.Location = new System.Drawing.Point(248, 200);
      this._btStyleDown.Name = "_btStyleDown";
      this._btStyleDown.Size = new System.Drawing.Size(32, 23);
      this._btStyleDown.TabIndex = 2;
      this._btStyleDown.Click += new System.EventHandler(this.EhStyleDown_Click);
      // 
      // _btStyleEdit
      // 
      this._btStyleEdit.Image = ((System.Drawing.Image)(resources.GetObject("_btStyleEdit.Image")));
      this._btStyleEdit.Location = new System.Drawing.Point(248, 248);
      this._btStyleEdit.Name = "_btStyleEdit";
      this._btStyleEdit.Size = new System.Drawing.Size(32, 23);
      this._btStyleEdit.TabIndex = 3;
      this._btStyleEdit.Click += new System.EventHandler(this.EhStyleEdit_Click);
      // 
      // _btStyleRemove
      // 
      this._btStyleRemove.Image = ((System.Drawing.Image)(resources.GetObject("_btStyleRemove.Image")));
      this._btStyleRemove.Location = new System.Drawing.Point(248, 280);
      this._btStyleRemove.Name = "_btStyleRemove";
      this._btStyleRemove.Size = new System.Drawing.Size(32, 23);
      this._btStyleRemove.TabIndex = 4;
      this._btStyleRemove.Click += new System.EventHandler(this.EhStyleRemove_Click);
      // 
      // _btAddStyle
      // 
      this._btAddStyle.AccessibleDescription = "Adds a new style.";
      this._btAddStyle.Image = ((System.Drawing.Image)(resources.GetObject("_btAddStyle.Image")));
      this._btAddStyle.Location = new System.Drawing.Point(248, 136);
      this._btAddStyle.Name = "_btAddStyle";
      this._btAddStyle.Size = new System.Drawing.Size(32, 23);
      this._btAddStyle.TabIndex = 5;
      this._btAddStyle.Click += new System.EventHandler(this.EhAddStyle_Click);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 56);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(120, 16);
      this.label2.TabIndex = 8;
      this.label2.Text = "Predefined style sets :";
      // 
      // _cbPredefinedStyleSets
      // 
      this._cbPredefinedStyleSets.Location = new System.Drawing.Point(8, 72);
      this._cbPredefinedStyleSets.Name = "_cbPredefinedStyleSets";
      this._cbPredefinedStyleSets.Size = new System.Drawing.Size(272, 21);
      this._cbPredefinedStyleSets.TabIndex = 9;
      this._cbPredefinedStyleSets.Text = "comboBox1";
      this._cbPredefinedStyleSets.SelectionChangeCommitted += new System.EventHandler(this.EhPredefinedStyleSets_SelectionChange);
      // 
      // XYPlotStyleCollectionControl
      // 
      this.Controls.Add(this._cbPredefinedStyleSets);
      this.Controls.Add(this.label2);
      this.Controls.Add(this._btAddStyle);
      this.Controls.Add(this._btStyleRemove);
      this.Controls.Add(this._btStyleEdit);
      this.Controls.Add(this._btStyleDown);
      this.Controls.Add(this._btStyleUp);
      this.Controls.Add(this._lbStyles);
      this.Name = "XYPlotStyleCollectionControl";
      this.Size = new System.Drawing.Size(288, 312);
      this.ResumeLayout(false);

    }
    #endregion



    #region IXYPlotStyleCollectionView Members
    IXYPlotStyleCollectionViewEventSink _controller;
    public IXYPlotStyleCollectionViewEventSink Controller
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

    public void InitializePredefinedStyles(string[] names, int selindex)
    {
      this._cbPredefinedStyleSets.BeginUpdate();
      this._cbPredefinedStyleSets.Items.Clear();
      this._cbPredefinedStyleSets.Items.AddRange(names);
      this._cbPredefinedStyleSets.SelectedIndex = selindex;
      this._cbPredefinedStyleSets.EndUpdate();
    }

    public void InitializeStyleList(string[] names, int[] selindices)
    {
      this._lbStyles.BeginUpdate();
      this._lbStyles.Items.Clear();
      this._lbStyles.Items.AddRange(names);
      for(int i=0;i<selindices.Length;++i)
      {
        _lbStyles.SetSelected(selindices[i],true);
      }
      this._lbStyles.EndUpdate();
    
    }

    MenuItem[] _availablePlotStyles;
    public void InitializeAvailableStyleList(List<string> names)
    {
      _availablePlotStyles = new MenuItem[names.Count];
      for(int i=0;i<_availablePlotStyles.Length;i++)
      {
        MenuItem item = new MenuItem(names[i],new EventHandler(this.EhAddSingleStyle_Click));
        _availablePlotStyles[i] = item;
      }
    }

    int[] GetSelectedStyles()
    {
      ListBox.SelectedIndexCollection coll = _lbStyles.SelectedIndices;
      int[] result = new int[coll.Count];
      for(int i=0;i<result.Length;i++)
      {
        result[i] = coll[i];
      }
      return result;
    }
    #endregion

    private void EhAddStyle_Click(object sender, System.EventArgs e)
    {
      ContextMenu popup = new ContextMenu(_availablePlotStyles);
      popup.Show(this,this._btAddStyle.Location);
    }

    private void EhAddSingleStyle_Click(object sender, System.EventArgs e)
    {
      for(int i=0;i<_availablePlotStyles.Length;i++)
      {
        if(object.ReferenceEquals(sender,_availablePlotStyles[i]))
        {
          if(_controller!=null)
            _controller.EhView_AddStyle(this.GetSelectedStyles(),i);

          break;
        }
      }
    }

    private void EhStyleUp_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_StyleUp(GetSelectedStyles());
    }

    private void EhStyleDown_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_StyleDown(GetSelectedStyles());
    }

    private void EhStyleEdit_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_StyleEdit(GetSelectedStyles());
    }

    private void EhStyleRemove_Click(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_StyleRemove(GetSelectedStyles());
    }

    private void EhPredefinedStyleSets_SelectionChange(object sender, System.EventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_PredefinedStyleSelected(this._cbPredefinedStyleSets.SelectedIndex);
    }
  }
}
