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

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for PLSPredictValueControl.
  /// </summary>
  public class PLSPredictValueControl : System.Windows.Forms.UserControl
  {
    PLSPredictValueController _controller;

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cbCalibrationModelTable;
    private System.Windows.Forms.ComboBox cbDestinationTable;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public PLSPredictValueControl()
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
      this.cbCalibrationModelTable = new System.Windows.Forms.ComboBox();
      this.cbDestinationTable = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(168, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "PLS calibration model:";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 112);
      this.label2.Name = "label2";
      this.label2.TabIndex = 1;
      this.label2.Text = "Destination table:";
      // 
      // cbCalibrationModelTable
      // 
      this.cbCalibrationModelTable.Location = new System.Drawing.Point(8, 48);
      this.cbCalibrationModelTable.Name = "cbCalibrationModelTable";
      this.cbCalibrationModelTable.Size = new System.Drawing.Size(184, 21);
      this.cbCalibrationModelTable.TabIndex = 2;
      this.cbCalibrationModelTable.Text = "comboBox1";
      // 
      // cbDestinationTable
      // 
      this.cbDestinationTable.Location = new System.Drawing.Point(8, 136);
      this.cbDestinationTable.Name = "cbDestinationTable";
      this.cbDestinationTable.Size = new System.Drawing.Size(184, 21);
      this.cbDestinationTable.TabIndex = 3;
      this.cbDestinationTable.Text = "comboBox2";
      // 
      // PLSPredictValueControl
      // 
      this.Controls.Add(this.cbDestinationTable);
      this.Controls.Add(this.cbCalibrationModelTable);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "PLSPredictValueControl";
      this.Size = new System.Drawing.Size(200, 168);
      this.ResumeLayout(false);

    }
    #endregion


    public PLSPredictValueController Controller
    {
      set { _controller = value; }
    }

    public void InitializeCalibrationModelTables(string[] tables)
    {
      this.cbCalibrationModelTable.Items.Clear();
      this.cbCalibrationModelTable.Items.AddRange(tables);
      if(tables.Length>0)
        this.cbCalibrationModelTable.SelectedIndex=0;
    }

    public void InitializeDestinationTables(string[] tables)
    {
      this.cbDestinationTable.Items.Clear();
      this.cbDestinationTable.Items.AddRange(tables);
      this.cbDestinationTable.SelectedIndex=0;
    }

    public int GetCalibrationTableChoice()
    {
      return this.cbCalibrationModelTable.SelectedIndex;
    }

    public int GetDestinationTableChoice()
    {
      return this.cbDestinationTable.SelectedIndex;
    }
  }
}
