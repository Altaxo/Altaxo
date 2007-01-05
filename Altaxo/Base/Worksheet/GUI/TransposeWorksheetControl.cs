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
using Altaxo.Gui.Common;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for TransposeWorksheetControl.
  /// </summary>
  public class TransposeWorksheetControl : System.Windows.Forms.UserControl
  {
    private MinMaxIntegerControl ctrlNumMovedDataCols;
    private MinMaxIntegerControl ctrlNumMovedPropCols;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public TransposeWorksheetControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      ctrlNumMovedDataCols.MinLabel = "None";
      ctrlNumMovedPropCols.MinLabel = "None";

      ctrlNumMovedDataCols.MaxLabel = "All";
      ctrlNumMovedPropCols.MaxLabel = "All";

      ctrlNumMovedDataCols.Minimum = 0;
      ctrlNumMovedPropCols.Minimum = 0;

      ctrlNumMovedDataCols.Maximum = int.MaxValue;
      ctrlNumMovedPropCols.Maximum = int.MaxValue;
      
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

    /// <summary>
    /// Get/sets the number of data columns that are moved to the property columns before transposing the data columns.
    /// </summary>
    public int DataColumnsMoveToPropertyColumns
    {
      get { return ctrlNumMovedDataCols.Value; }
      set
      {
        ctrlNumMovedDataCols.Value = value;
      }
    }

    /// <summary>
    /// Get/sets the number of property columns that are moved after transposing the data columns to the data columns collection.
    /// </summary>
    public int PropertyColumnsMoveToDataColumns
    {
      get { return ctrlNumMovedPropCols.Value; }
      set
      {
        ctrlNumMovedPropCols.Value = value;
      }
    }



    #region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.ctrlNumMovedDataCols = new MinMaxIntegerControl();
      this.ctrlNumMovedPropCols = new MinMaxIntegerControl();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // ctrlNumMovedDataCols
      // 
      this.ctrlNumMovedDataCols.Location = new System.Drawing.Point(16, 56);
      this.ctrlNumMovedDataCols.Name = "ctrlNumMovedDataCols";
      this.ctrlNumMovedDataCols.Size = new System.Drawing.Size(104, 96);
      this.ctrlNumMovedDataCols.TabIndex = 0;
      // 
      // ctrlNumMovedPropCols
      // 
      this.ctrlNumMovedPropCols.Location = new System.Drawing.Point(16, 56);
      this.ctrlNumMovedPropCols.Name = "ctrlNumMovedPropCols";
      this.ctrlNumMovedPropCols.Size = new System.Drawing.Size(104, 96);
      this.ctrlNumMovedPropCols.TabIndex = 1;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.ctrlNumMovedDataCols);
      this.groupBox1.Location = new System.Drawing.Point(8, 16);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(144, 160);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "number of data columns changing to property columns";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ctrlNumMovedPropCols);
      this.groupBox2.Location = new System.Drawing.Point(176, 16);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(152, 160);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "number of property columns changing to data columns+";
      // 
      // TransposeWorksheetControl
      // 
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "TransposeWorksheetControl";
      this.Size = new System.Drawing.Size(336, 184);
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion
  }
}
