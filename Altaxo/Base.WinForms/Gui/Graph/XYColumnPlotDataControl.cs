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


namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Summary description for LineScatterPlotDataControl.
  /// </summary>
  [UserControlForController(typeof(IXYColumnPlotDataViewEventSink))]
  public class XYColumnPlotDataControl : System.Windows.Forms.UserControl, IXYColumnPlotDataView
  {
    IXYColumnPlotDataViewEventSink _controller;
    private System.Windows.Forms.ComboBox m_cbTables;
    private System.Windows.Forms.ListBox m_lbColumns;
    private System.Windows.Forms.Button m_btToX;
    private System.Windows.Forms.Button m_btToY;
    private System.Windows.Forms.TextBox m_edXColumn;
    private System.Windows.Forms.Button m_btEraseX;
    private System.Windows.Forms.TextBox m_edYColumn;
    private System.Windows.Forms.Button m_btEraseY;
    private System.Windows.Forms.NumericUpDown m_nudPlotRangeFrom;
    private System.Windows.Forms.NumericUpDown m_nudPlotRangeTo;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public XYColumnPlotDataControl()
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
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XYColumnPlotDataControl));
      this.m_cbTables = new System.Windows.Forms.ComboBox();
      this.m_lbColumns = new System.Windows.Forms.ListBox();
      this.m_btToX = new System.Windows.Forms.Button();
      this.m_btToY = new System.Windows.Forms.Button();
      this.m_edXColumn = new System.Windows.Forms.TextBox();
      this.m_btEraseX = new System.Windows.Forms.Button();
      this.m_edYColumn = new System.Windows.Forms.TextBox();
      this.m_btEraseY = new System.Windows.Forms.Button();
      this.m_nudPlotRangeFrom = new System.Windows.Forms.NumericUpDown();
      this.m_nudPlotRangeTo = new System.Windows.Forms.NumericUpDown();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.m_nudPlotRangeFrom)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_nudPlotRangeTo)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_cbTables
      // 
      this.m_cbTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.m_cbTables.Location = new System.Drawing.Point(8, 32);
      this.m_cbTables.Name = "m_cbTables";
      this.m_cbTables.Size = new System.Drawing.Size(144, 21);
      this.m_cbTables.TabIndex = 0;
      this.m_cbTables.SelectionChangeCommitted += new System.EventHandler(this.EhTables_SelectionChangeCommit);
      // 
      // m_lbColumns
      // 
      this.m_lbColumns.Location = new System.Drawing.Point(8, 64);
      this.m_lbColumns.Name = "m_lbColumns";
      this.m_lbColumns.Size = new System.Drawing.Size(144, 186);
      this.m_lbColumns.TabIndex = 1;
      // 
      // m_btToX
      // 
      this.m_btToX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.m_btToX.Image = ((System.Drawing.Image)(resources.GetObject("m_btToX.Image")));
      this.m_btToX.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.m_btToX.Location = new System.Drawing.Point(160, 64);
      this.m_btToX.Name = "m_btToX";
      this.m_btToX.Size = new System.Drawing.Size(40, 24);
      this.m_btToX.TabIndex = 2;
      this.m_btToX.Text = "X";
      this.m_btToX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.m_btToX.Click += new System.EventHandler(this.EhToX_Click);
      // 
      // m_btToY
      // 
      this.m_btToY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.m_btToY.Image = ((System.Drawing.Image)(resources.GetObject("m_btToY.Image")));
      this.m_btToY.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.m_btToY.Location = new System.Drawing.Point(160, 104);
      this.m_btToY.Name = "m_btToY";
      this.m_btToY.Size = new System.Drawing.Size(40, 24);
      this.m_btToY.TabIndex = 3;
      this.m_btToY.Text = "Y";
      this.m_btToY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.m_btToY.Click += new System.EventHandler(this.EhToY_Click);
      // 
      // m_edXColumn
      // 
      this.m_edXColumn.Location = new System.Drawing.Point(208, 64);
      this.m_edXColumn.Name = "m_edXColumn";
      this.m_edXColumn.ReadOnly = true;
      this.m_edXColumn.Size = new System.Drawing.Size(192, 20);
      this.m_edXColumn.TabIndex = 4;
      this.m_edXColumn.Text = "textBox1";
      this.m_edXColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // m_btEraseX
      // 
      this.m_btEraseX.Image = ((System.Drawing.Image)(resources.GetObject("m_btEraseX.Image")));
      this.m_btEraseX.Location = new System.Drawing.Point(408, 64);
      this.m_btEraseX.Name = "m_btEraseX";
      this.m_btEraseX.Size = new System.Drawing.Size(24, 24);
      this.m_btEraseX.TabIndex = 5;
      this.m_btEraseX.Click += new System.EventHandler(this.EhEraseX_Click);
      // 
      // m_edYColumn
      // 
      this.m_edYColumn.Location = new System.Drawing.Point(208, 104);
      this.m_edYColumn.Name = "m_edYColumn";
      this.m_edYColumn.ReadOnly = true;
      this.m_edYColumn.Size = new System.Drawing.Size(192, 20);
      this.m_edYColumn.TabIndex = 6;
      this.m_edYColumn.Text = "textBox2";
      this.m_edYColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // m_btEraseY
      // 
      this.m_btEraseY.Image = ((System.Drawing.Image)(resources.GetObject("m_btEraseY.Image")));
      this.m_btEraseY.Location = new System.Drawing.Point(408, 104);
      this.m_btEraseY.Name = "m_btEraseY";
      this.m_btEraseY.Size = new System.Drawing.Size(24, 24);
      this.m_btEraseY.TabIndex = 7;
      this.m_btEraseY.Click += new System.EventHandler(this.EhEraseY_Click);
      // 
      // m_nudPlotRangeFrom
      // 
      this.m_nudPlotRangeFrom.Location = new System.Drawing.Point(64, 24);
      this.m_nudPlotRangeFrom.Name = "m_nudPlotRangeFrom";
      this.m_nudPlotRangeFrom.Size = new System.Drawing.Size(80, 20);
      this.m_nudPlotRangeFrom.TabIndex = 8;
      this.m_nudPlotRangeFrom.Validating += new System.ComponentModel.CancelEventHandler(this.EhPlotRangeFrom_Validating);
      // 
      // m_nudPlotRangeTo
      // 
      this.m_nudPlotRangeTo.Location = new System.Drawing.Point(200, 24);
      this.m_nudPlotRangeTo.Name = "m_nudPlotRangeTo";
      this.m_nudPlotRangeTo.Size = new System.Drawing.Size(72, 20);
      this.m_nudPlotRangeTo.TabIndex = 9;
      this.m_nudPlotRangeTo.Validating += new System.ComponentModel.CancelEventHandler(this.EhPlotRangeTo_Validating);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.m_nudPlotRangeFrom);
      this.groupBox1.Controls.Add(this.m_nudPlotRangeTo);
      this.groupBox1.Location = new System.Drawing.Point(16, 304);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(280, 48);
      this.groupBox1.TabIndex = 10;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Plot Range";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(160, 24);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(32, 16);
      this.label2.TabIndex = 11;
      this.label2.Text = "To:";
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(32, 16);
      this.label1.TabIndex = 10;
      this.label1.Text = "From:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // XYColumnPlotDataControl
      // 
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.m_btEraseY);
      this.Controls.Add(this.m_edYColumn);
      this.Controls.Add(this.m_btEraseX);
      this.Controls.Add(this.m_edXColumn);
      this.Controls.Add(this.m_btToY);
      this.Controls.Add(this.m_btToX);
      this.Controls.Add(this.m_lbColumns);
      this.Controls.Add(this.m_cbTables);
      this.Name = "XYColumnPlotDataControl";
      this.Size = new System.Drawing.Size(432, 368);
      ((System.ComponentModel.ISupportInitialize)(this.m_nudPlotRangeFrom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.m_nudPlotRangeTo)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    #region ILineScatterPlotDataView Members

    public IXYColumnPlotDataViewEventSink Controller
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

  

    public void Tables_Initialize(string[] tables, int selectedTable)
    {
      this.m_cbTables.Items.Clear();
      this.m_cbTables.Items.AddRange(tables);
      this.m_cbTables.SelectedIndex = selectedTable;
    }

    public void Columns_Initialize(string[] colnames, int selectedColumn)
    {
      this.m_lbColumns.Items.Clear();
      this.m_lbColumns.Items.AddRange(colnames);
      if(selectedColumn < colnames.Length)
        this.m_lbColumns.SelectedIndex = selectedColumn;
    }

    public void XColumn_Initialize(string colname)
    {
      this.m_edXColumn.Text = colname;
    }

    public void YColumn_Initialize(string colname)
    {
      this.m_edYColumn.Text = colname;
    }

   
    public void PlotRangeFrom_Initialize(int from)
    {
      this.m_nudPlotRangeFrom.Minimum=0;
      this.m_nudPlotRangeFrom.Maximum = int.MaxValue;
      this.m_nudPlotRangeFrom.Value = from;
    }

    public void PlotRangeTo_Initialize(int to)
    {
      this.m_nudPlotRangeTo.Minimum=0;
      this.m_nudPlotRangeTo.Maximum= int.MaxValue;
      this.m_nudPlotRangeTo.Value = to;
    }

    #endregion

    private void EhToX_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_ToX(this.m_cbTables.SelectedIndex,(string)this.m_cbTables.SelectedItem,m_lbColumns.SelectedIndex,(string)this.m_lbColumns.SelectedItem);
    }

    private void EhToY_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_ToY(this.m_cbTables.SelectedIndex,(string)this.m_cbTables.SelectedItem,m_lbColumns.SelectedIndex,(string)this.m_lbColumns.SelectedItem);

    }



    private void EhEraseX_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_EraseX();
    }

    private void EhEraseY_Click(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_EraseY();
    }

   
    private void EhPlotRangeFrom_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
        e.Cancel |= Controller.EhView_RangeFrom((int)this.m_nudPlotRangeFrom.Value);
    }

    private void EhPlotRangeTo_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(null!=Controller)
        e.Cancel |= Controller.EhView_RangeTo((int)this.m_nudPlotRangeTo.Value);
    }

    private void EhTables_SelectionChangeCommit(object sender, System.EventArgs e)
    {
      if(null!=Controller)
        Controller.EhView_TableSelectionChanged(this.m_cbTables.SelectedIndex,(string)this.m_cbTables.SelectedItem);
    }

 

   
  }
}
