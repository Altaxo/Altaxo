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

using Altaxo.Serialization;


namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for ArrangeLayersControl.
  /// </summary>
  [UserControlForController(typeof(IArrangeLayersViewEventSink))]
  public class ArrangeLayersControl : System.Windows.Forms.UserControl, IArrangeLayersView
  {
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox _edNumberOfRows;
    private System.Windows.Forms.TextBox _edNumberOfColumns;
    private System.Windows.Forms.TextBox _edRowSpacing;
    private System.Windows.Forms.TextBox _edColumnSpacing;
    private System.Windows.Forms.TextBox _edTopMargin;
    private System.Windows.Forms.TextBox _edLeftMargin;
    private System.Windows.Forms.TextBox _edBottomMargin;
    private System.Windows.Forms.TextBox _edRightMargin;
    private ComboBox _cbSuperfluousLayersAction;
    private Label label9;
    private TableLayoutPanel tableLayoutPanel1;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ArrangeLayersControl()
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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this._edNumberOfRows = new System.Windows.Forms.TextBox();
      this._edNumberOfColumns = new System.Windows.Forms.TextBox();
      this._edRowSpacing = new System.Windows.Forms.TextBox();
      this._edColumnSpacing = new System.Windows.Forms.TextBox();
      this._edTopMargin = new System.Windows.Forms.TextBox();
      this._edLeftMargin = new System.Windows.Forms.TextBox();
      this._edBottomMargin = new System.Windows.Forms.TextBox();
      this._edRightMargin = new System.Windows.Forms.TextBox();
      this._cbSuperfluousLayersAction = new System.Windows.Forms.ComboBox();
      this.label9 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(84, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Number of rows:";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(101, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Number of columns:";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 58);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(89, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Row spacing (%):";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 84);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(102, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Column spacing (%):";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 110);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(80, 13);
      this.label5.TabIndex = 4;
      this.label5.Text = "Top margin (%):";
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(3, 136);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(79, 13);
      this.label6.TabIndex = 5;
      this.label6.Text = "Left margin (%):";
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(3, 162);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(94, 13);
      this.label7.TabIndex = 6;
      this.label7.Text = "Bottom margin (%):";
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(3, 188);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(86, 13);
      this.label8.TabIndex = 7;
      this.label8.Text = "Right margin (%):";
      // 
      // _edNumberOfRows
      // 
      this._edNumberOfRows.Location = new System.Drawing.Point(111, 3);
      this._edNumberOfRows.Name = "_edNumberOfRows";
      this._edNumberOfRows.Size = new System.Drawing.Size(119, 20);
      this._edNumberOfRows.TabIndex = 8;
      this._edNumberOfRows.Text = "1";
      this._edNumberOfRows.Validating += new System.ComponentModel.CancelEventHandler(this._edNumberOfRows_Validating);
      // 
      // _edNumberOfColumns
      // 
      this._edNumberOfColumns.Location = new System.Drawing.Point(111, 29);
      this._edNumberOfColumns.Name = "_edNumberOfColumns";
      this._edNumberOfColumns.Size = new System.Drawing.Size(119, 20);
      this._edNumberOfColumns.TabIndex = 9;
      this._edNumberOfColumns.Text = "1";
      this._edNumberOfColumns.Validating += new System.ComponentModel.CancelEventHandler(this._edNumberOfColumns_Validating);
      // 
      // _edRowSpacing
      // 
      this._edRowSpacing.Location = new System.Drawing.Point(111, 55);
      this._edRowSpacing.Name = "_edRowSpacing";
      this._edRowSpacing.Size = new System.Drawing.Size(119, 20);
      this._edRowSpacing.TabIndex = 10;
      this._edRowSpacing.Text = "0";
      this._edRowSpacing.Validating += new System.ComponentModel.CancelEventHandler(this._edHorizontalSpacing_Validating);
      // 
      // _edColumnSpacing
      // 
      this._edColumnSpacing.Location = new System.Drawing.Point(111, 81);
      this._edColumnSpacing.Name = "_edColumnSpacing";
      this._edColumnSpacing.Size = new System.Drawing.Size(119, 20);
      this._edColumnSpacing.TabIndex = 11;
      this._edColumnSpacing.Text = "0";
      this._edColumnSpacing.Validating += new System.ComponentModel.CancelEventHandler(this._edVerticalSpacing_Validating);
      // 
      // _edTopMargin
      // 
      this._edTopMargin.Location = new System.Drawing.Point(111, 107);
      this._edTopMargin.Name = "_edTopMargin";
      this._edTopMargin.Size = new System.Drawing.Size(119, 20);
      this._edTopMargin.TabIndex = 12;
      this._edTopMargin.Text = "10";
      this._edTopMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edTopMargin_Validating);
      // 
      // _edLeftMargin
      // 
      this._edLeftMargin.Location = new System.Drawing.Point(111, 133);
      this._edLeftMargin.Name = "_edLeftMargin";
      this._edLeftMargin.Size = new System.Drawing.Size(119, 20);
      this._edLeftMargin.TabIndex = 13;
      this._edLeftMargin.Text = "10";
      this._edLeftMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edLeftMargin_Validating);
      // 
      // _edBottomMargin
      // 
      this._edBottomMargin.Location = new System.Drawing.Point(111, 159);
      this._edBottomMargin.Name = "_edBottomMargin";
      this._edBottomMargin.Size = new System.Drawing.Size(119, 20);
      this._edBottomMargin.TabIndex = 14;
      this._edBottomMargin.Text = "10";
      this._edBottomMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edBottomMargin_Validating);
      // 
      // _edRightMargin
      // 
      this._edRightMargin.Location = new System.Drawing.Point(111, 185);
      this._edRightMargin.Name = "_edRightMargin";
      this._edRightMargin.Size = new System.Drawing.Size(119, 20);
      this._edRightMargin.TabIndex = 15;
      this._edRightMargin.Text = "10";
      this._edRightMargin.Validating += new System.ComponentModel.CancelEventHandler(this._edRightMargin_Validating);
      // 
      // _cbSuperfluousLayersAction
      // 
      this._cbSuperfluousLayersAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._cbSuperfluousLayersAction.FormattingEnabled = true;
      this._cbSuperfluousLayersAction.Location = new System.Drawing.Point(111, 211);
      this._cbSuperfluousLayersAction.Name = "_cbSuperfluousLayersAction";
      this._cbSuperfluousLayersAction.Size = new System.Drawing.Size(122, 21);
      this._cbSuperfluousLayersAction.TabIndex = 16;
      this._cbSuperfluousLayersAction.SelectionChangeCommitted += new System.EventHandler(this._cbSuperfluousLayersAction_SelectionChangeCommitted);
      // 
      // label9
      // 
      this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(3, 215);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(96, 13);
      this.label9.TabIndex = 17;
      this.label9.Text = "Superfluous layers:";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this._cbSuperfluousLayersAction, 1, 8);
      this.tableLayoutPanel1.Controls.Add(this.label9, 0, 8);
      this.tableLayoutPanel1.Controls.Add(this._edNumberOfRows, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this._edRightMargin, 1, 7);
      this.tableLayoutPanel1.Controls.Add(this._edNumberOfColumns, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label8, 0, 7);
      this.tableLayoutPanel1.Controls.Add(this._edBottomMargin, 1, 6);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this._edLeftMargin, 1, 5);
      this.tableLayoutPanel1.Controls.Add(this.label7, 0, 6);
      this.tableLayoutPanel1.Controls.Add(this._edRowSpacing, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this._edTopMargin, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this._edColumnSpacing, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 9;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(248, 235);
      this.tableLayoutPanel1.TabIndex = 18;
      // 
      // ArrangeLayersControl
      // 
      this.AutoSize = true;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "ArrangeLayersControl";
      this.Size = new System.Drawing.Size(254, 241);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    #region IArrangeLayersView Members

    IArrangeLayersViewEventSink _controller;
    public IArrangeLayersViewEventSink Controller
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

    public void InitializeRowsColumns(int numRows, int numColumns)
    {
      this._edNumberOfRows.Text = GUIConversion.ToString(numRows);
      this._edNumberOfColumns.Text = GUIConversion.ToString(numColumns);
    }

    public void InitializeSpacing(double horzSpacing, double vertSpacing)
    {
      this._edRowSpacing.Text = GUIConversion.ToString(horzSpacing);
      this._edColumnSpacing.Text = GUIConversion.ToString(vertSpacing);
    }

    public void InitializeMargins(double top, double left, double bottom, double right)
    {
      this._edTopMargin.Text =  GUIConversion.ToString(top);
      this._edBottomMargin.Text =  GUIConversion.ToString(bottom);
      this._edRightMargin.Text =  GUIConversion.ToString(right);
      this._edLeftMargin.Text =  GUIConversion.ToString(left);
    }

    public void InitializeSuperfluosLayersQuestion(Altaxo.Collections.SelectableListNodeList list)
    {
      _cbSuperfluousLayersAction.BeginUpdate();
      _cbSuperfluousLayersAction.Items.Clear();
      for (int i = 0; i < list.Count; i++)
      {
        _cbSuperfluousLayersAction.Items.Add(list[i]);
        if (list[i].Selected)
          _cbSuperfluousLayersAction.SelectedIndex = i;
      }
      _cbSuperfluousLayersAction.EndUpdate();
    }

    public void InitializeEnableConditions(bool rowSpacingEnabled, bool columnSpacingEnabled, bool superfluousEnabled)
    {
      _edRowSpacing.Enabled = rowSpacingEnabled;
      _edColumnSpacing.Enabled = columnSpacingEnabled;
      _cbSuperfluousLayersAction.Enabled = superfluousEnabled;
    }


    #endregion

    private void _edNumberOfRows_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhNumberOfRowsChanged(this._edNumberOfRows.Text);
    }

    private void _edNumberOfColumns_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhNumberOfColumnsChanged(this._edNumberOfColumns.Text);
    
    }

    private void _edHorizontalSpacing_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhRowSpacingChanged(this._edRowSpacing.Text);

    }

    private void _edVerticalSpacing_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhColumnSpacingChanged(this._edColumnSpacing.Text);
    }

    private void _edTopMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhTopMarginChanged(this._edTopMargin.Text);

    }

    private void _edLeftMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhLeftMarginChanged(this._edLeftMargin.Text);
    
    }

    private void _edBottomMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhBottomMarginChanged(this._edBottomMargin.Text);
    
    }

    private void _edRightMargin_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(_controller!=null)
        e.Cancel |= _controller.EhRightMarginChanged(this._edRightMargin.Text);
    
    }

    private void _cbSuperfluousLayersAction_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhSuperfluousLayersActionChanged((Altaxo.Collections.SelectableListNode)_cbSuperfluousLayersAction.SelectedItem);
    }
  }
}
