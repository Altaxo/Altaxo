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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Summary description for ColorTypeThicknessPenControl.
  /// </summary>
  public class ColorTypeThicknessPenControl : System.Windows.Forms.UserControl, IColorTypeThicknessPenView
  {
    private ColorComboBox _cbColor;
    private DashStyleComboBox _cbLineType;
    private LineThicknessComboBox _cbThickness;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private ContextMenuStrip _controlContextMenu;
    private ToolStripMenuItem _menuShowFullPenDialog;
    private PenControlsGlue _penGlue;
    private TableLayoutPanel _tableLayout;
    private IContainer components;

    public ColorTypeThicknessPenControl()
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
      this.components = new System.ComponentModel.Container();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this._controlContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this._menuShowFullPenDialog = new System.Windows.Forms.ToolStripMenuItem();
      this._cbThickness = new Altaxo.Gui.Common.Drawing.LineThicknessComboBox();
      this._cbLineType = new Altaxo.Gui.Common.Drawing.DashStyleComboBox();
      this._cbColor = new Altaxo.Gui.Common.Drawing.ColorComboBox();
      this._penGlue = new Altaxo.Gui.Common.Drawing.PenControlsGlue();
      this._tableLayout = new System.Windows.Forms.TableLayoutPanel();
      this._controlContextMenu.SuspendLayout();
      this._tableLayout.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(28, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(34, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Color:";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(28, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(34, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Type:";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 61);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(59, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Thickness:";
      // 
      // _controlContextMenu
      // 
      this._controlContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuShowFullPenDialog});
      this._controlContextMenu.Name = "_controlContextMenu";
      this._controlContextMenu.Size = new System.Drawing.Size(209, 26);
      // 
      // _menuShowFullPenDialog
      // 
      this._menuShowFullPenDialog.Name = "_menuShowFullPenDialog";
      this._menuShowFullPenDialog.Size = new System.Drawing.Size(208, 22);
      this._menuShowFullPenDialog.Text = "Show full pen dialog ...";
      this._menuShowFullPenDialog.Click += new System.EventHandler(this._menuShowFullPenDialog_Click);
      // 
      // _cbThickness
      // 
      this._cbThickness.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbThickness.ItemHeight = 15;
    
      this._cbThickness.Location = new System.Drawing.Point(68, 57);
      this._cbThickness.Name = "_cbThickness";
      this._cbThickness.Size = new System.Drawing.Size(121, 21);
      this._cbThickness.TabIndex = 2;
      // 
      // _cbLineType
      // 
      this._cbLineType.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbLineType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbLineType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbLineType.ItemHeight = 15;
      this._cbLineType.Location = new System.Drawing.Point(68, 30);
      this._cbLineType.Name = "_cbLineType";
      this._cbLineType.Size = new System.Drawing.Size(121, 21);
      this._cbLineType.TabIndex = 1;
      // 
      // _cbColor
      // 
      this._cbColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this._cbColor.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      this._cbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this._cbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbColor.FormattingEnabled = true;
      this._cbColor.ItemHeight = 15;
      this._cbColor.Location = new System.Drawing.Point(68, 3);
      this._cbColor.Name = "_cbColor";
      this._cbColor.Size = new System.Drawing.Size(121, 21);
      this._cbColor.TabIndex = 0;
      // 
      // _penGlue
      // 
      this._penGlue.CbBrushColor = this._cbColor;
      this._penGlue.CbBrushColor2 = null;
      this._penGlue.CbBrushHatchStyle = null;
      this._penGlue.CbBrushType = null;
      this._penGlue.CbDashCap = null;
      this._penGlue.CbDashStyle = this._cbLineType;
      this._penGlue.CbEndCap = null;
      this._penGlue.CbEndCapSize = null;
      this._penGlue.CbLineJoin = null;
      this._penGlue.CbLineThickness = this._cbThickness;
      this._penGlue.CbMiterLimit = null;
      this._penGlue.CbStartCap = null;
      this._penGlue.CbStartCapSize = null;
      this._penGlue.ColorType = Altaxo.Graph.ColorType.KnownAndSystemColor;
      // 
      // _tableLayout
      // 
      this._tableLayout.AutoSize = true;
      this._tableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this._tableLayout.ColumnCount = 2;
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this._tableLayout.Controls.Add(this.label1, 0, 0);
      this._tableLayout.Controls.Add(this.label2, 0, 1);
      this._tableLayout.Controls.Add(this._cbThickness, 1, 2);
      this._tableLayout.Controls.Add(this.label3, 0, 2);
      this._tableLayout.Controls.Add(this._cbLineType, 1, 1);
      this._tableLayout.Controls.Add(this._cbColor, 1, 0);
      this._tableLayout.Location = new System.Drawing.Point(3, 3);
      this._tableLayout.Name = "_tableLayout";
      this._tableLayout.RowCount = 3;
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this._tableLayout.Size = new System.Drawing.Size(192, 81);
      this._tableLayout.TabIndex = 0;
      // 
      // ColorTypeThicknessPenControl
      // 
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.ContextMenuStrip = this._controlContextMenu;
      this.Controls.Add(this._tableLayout);
      this.Name = "ColorTypeThicknessPenControl";
      this.Size = new System.Drawing.Size(198, 87);
      this._controlContextMenu.ResumeLayout(false);
      this._tableLayout.ResumeLayout(false);
      this._tableLayout.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    #region IColorTypeThicknessPenView Members


    IColorTypeThicknessPenViewEventSink _controller;
    public IColorTypeThicknessPenViewEventSink Controller
    {
      get { return _controller; }
      set { _controller = value; }
    }

    public PenX DocPen
    {
      get
      {
        return _penGlue.Pen;
      }
      set
      {
        _penGlue.Pen = value;
      }
    }


    public ColorType ColorType
    {
      get
      {
        return _penGlue.ColorType;
      }
      set
      {
        _penGlue.ColorType = value;
      }
    }

    #endregion


    private void _menuShowFullPenDialog_Click(object sender, EventArgs e)
    {
      if (null != _controller)
        _controller.EhView_ShowFullPenDialog();
    }
  }
}
