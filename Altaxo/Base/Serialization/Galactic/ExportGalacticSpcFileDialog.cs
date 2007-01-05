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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Altaxo.Collections;

namespace Altaxo.Serialization.Galactic
{
  /// <summary>
  /// ExportGalacticSpcFileDialog asks for the options to export files in the Galactic SPC format (<see ref="http://www.galactic.com"/> for details about the file format).
  /// </summary>
  public class ExportGalacticSpcFileDialog : System.Windows.Forms.Form
  {
    private System.Windows.Forms.Label m_lbl_BasicFileNameAndPath;
    private System.Windows.Forms.TextBox m_edBasicFileNameAndPath;
    private System.Windows.Forms.Button m_btChooseBasicFileNameAndPath;
    private System.Windows.Forms.GroupBox m_grpXValues;
    private System.Windows.Forms.GroupBox m_grpExtendFileName;
    private System.Windows.Forms.Button m_btOK;
    private System.Windows.Forms.Button m_btCancel;
    private System.Windows.Forms.RadioButton m_rbXValuesContinuousNumber;
    private System.Windows.Forms.RadioButton m_rbXValues_FromColumn;
    private System.Windows.Forms.RadioButton m_rbExtFileName_ContinuousNumber;
    private System.Windows.Forms.RadioButton m_rbFileName_FromColumn;
    private System.Windows.Forms.ComboBox m_cbXValues_Column;
    private System.Windows.Forms.ComboBox m_cbExtFileName_Column;
    private ExportGalacticSpcFileDialogController m_Controller;
    private System.Windows.Forms.GroupBox m_grpCreateSpectrum;
    private System.Windows.Forms.RadioButton m_rbCreateSpectrum_FromRow;
    private System.Windows.Forms.RadioButton m_rbCreateSpectrum_FromColumn;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;


    /// <summary>
    /// Creates the export dialog for Galactic SPC file export.
    /// </summary>
    public ExportGalacticSpcFileDialog()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();
    }


    /// <summary>
    /// This is neccessary to use the dialog.
    /// </summary>
    /// <param name="table">The table which contains the data to export.</param>
    /// <param name="selectedRows">The rows selected in the table.</param>
    /// <param name="selectedColumns">The columns selected in the table.</param>
    public void Initialize(Altaxo.Data.DataTable table, IAscendingIntegerCollection selectedRows, IAscendingIntegerCollection selectedColumns)
    {
      m_Controller = new ExportGalacticSpcFileDialogController(this,table,selectedRows,selectedColumns);
    }


    /// <summary>
    /// Gets / set the basic file name for export.
    /// </summary>
    /// <value>Basic file name for exporting to Galactic SPC format.</value>
    public string BasicFileName
    {
      get { return this.m_edBasicFileNameAndPath.Text; }
      set { this.m_edBasicFileNameAndPath.Text = value; }
    }


    /// <summary>
    /// Selection state of the radio button "Create Spectrum from row."
    /// </summary>
    public bool CreateSpectrumFromRow
    {
      get { return this.m_rbCreateSpectrum_FromRow.Checked; }
      set { this.m_rbCreateSpectrum_FromRow.Checked = value; }
    }


    /// <summary>
    /// Selection state of the radio button "Create Spectrum from column."
    /// </summary>
    public bool CreateSpectrumFromColumn
    {
      get { return this.m_rbCreateSpectrum_FromColumn.Checked; }
      set { this.m_rbCreateSpectrum_FromColumn.Checked = value; }
    }

    /// <summary>
    /// Selection state of the radio button "X values from continuous number."
    /// </summary>
    public bool XValuesContinuousNumber
    {
      get { return this.m_rbXValuesContinuousNumber.Checked; }
      set { this.m_rbXValuesContinuousNumber.Checked = value; }
    }

    /// <summary>
    /// Selection state of the radio button "X values from column."
    /// </summary>
    public bool XValuesFromColumn
    {
      get { return this.m_rbXValues_FromColumn.Checked; }
      set { this.m_rbXValues_FromColumn.Checked = value; }
    }


    /// <summary>
    /// Selection state of the radio button "Extend file name by continuous number".
    /// </summary>
    public bool ExtendFileName_ContinuousNumber
    {
      get { return this.m_rbExtFileName_ContinuousNumber.Checked; }
      set { this.m_rbExtFileName_ContinuousNumber.Checked = value; }
    }

    /// <summary>
    /// Selection state of the radio button "Extend file name by column".
    /// </summary>
    public bool ExtendFileName_ByColumn
    {
      get { return this.m_rbFileName_FromColumn.Checked; }
      set { this.m_rbFileName_FromColumn.Checked = value; }
    }

    /// <summary>
    /// This fills the x values column combobox with column names and selects the first index in the column as default.
    /// </summary>
    /// <param name="colnames">The array of column names the combobox is filled with.</param>
    public void FillXValuesColumnBox(string [] colnames)
    {
      this.m_cbXValues_Column.Items.Clear();
      this.m_cbXValues_Column.Items.AddRange(colnames);

      if(colnames.Length>0)
        this.m_cbXValues_Column.SelectedIndex=0;
      else
        this.m_cbXValues_Column.SelectedIndex=-1;
    }

    /// <summary>
    /// Returns the selected x values column name.
    /// </summary>
    public string XValuesColumnName
    {
      get { return (string)this.m_cbXValues_Column.SelectedItem; }
    }

    /// <summary>
    /// Sets the enabled state of the x values column combobox.
    /// </summary>
    public bool EnableXValuesColumnBox
    {
      set { this.m_cbXValues_Column.Enabled=value; }
    }

    /// <summary>
    /// Fills the "Extend file name by column" combobox with column names.
    /// </summary>
    /// <param name="colnames">The array of column names the combobox is filled with.</param>
    public void FillExtFileNameColumnBox(string [] colnames)
    {
      this.m_cbExtFileName_Column.Items.Clear();
      this.m_cbExtFileName_Column.Items.AddRange(colnames);

      if(colnames.Length>0)
        this.m_cbExtFileName_Column.SelectedIndex=0;
      else
        this.m_cbExtFileName_Column.SelectedIndex=-1;
    }

    /// <summary>
    /// Returns the selected column name of the "Extend file name by column" combobox.
    /// </summary>
    public string ExtFileNameColumnName
    {
      get { return (string)this.m_cbExtFileName_Column.SelectedItem; }
    }

    /// <summary>
    /// Sets the enabled / disabled state of the "Extend file name by column" combobox.
    /// </summary>
    public bool EnableExtFileNameColumnBox
    {
      set { this.m_cbExtFileName_Column.Enabled=value; }
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

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_lbl_BasicFileNameAndPath = new System.Windows.Forms.Label();
      this.m_edBasicFileNameAndPath = new System.Windows.Forms.TextBox();
      this.m_btChooseBasicFileNameAndPath = new System.Windows.Forms.Button();
      this.m_grpXValues = new System.Windows.Forms.GroupBox();
      this.m_cbXValues_Column = new System.Windows.Forms.ComboBox();
      this.m_rbXValues_FromColumn = new System.Windows.Forms.RadioButton();
      this.m_rbXValuesContinuousNumber = new System.Windows.Forms.RadioButton();
      this.m_grpExtendFileName = new System.Windows.Forms.GroupBox();
      this.m_cbExtFileName_Column = new System.Windows.Forms.ComboBox();
      this.m_rbFileName_FromColumn = new System.Windows.Forms.RadioButton();
      this.m_rbExtFileName_ContinuousNumber = new System.Windows.Forms.RadioButton();
      this.m_btOK = new System.Windows.Forms.Button();
      this.m_btCancel = new System.Windows.Forms.Button();
      this.m_grpCreateSpectrum = new System.Windows.Forms.GroupBox();
      this.m_rbCreateSpectrum_FromColumn = new System.Windows.Forms.RadioButton();
      this.m_rbCreateSpectrum_FromRow = new System.Windows.Forms.RadioButton();
      this.m_grpXValues.SuspendLayout();
      this.m_grpExtendFileName.SuspendLayout();
      this.m_grpCreateSpectrum.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_lbl_BasicFileNameAndPath
      // 
      this.m_lbl_BasicFileNameAndPath.Location = new System.Drawing.Point(16, 8);
      this.m_lbl_BasicFileNameAndPath.Name = "m_lbl_BasicFileNameAndPath";
      this.m_lbl_BasicFileNameAndPath.Size = new System.Drawing.Size(168, 16);
      this.m_lbl_BasicFileNameAndPath.TabIndex = 0;
      this.m_lbl_BasicFileNameAndPath.Text = "Basic file name and path:";
      // 
      // m_edBasicFileNameAndPath
      // 
      this.m_edBasicFileNameAndPath.Location = new System.Drawing.Point(16, 24);
      this.m_edBasicFileNameAndPath.Name = "m_edBasicFileNameAndPath";
      this.m_edBasicFileNameAndPath.Size = new System.Drawing.Size(320, 20);
      this.m_edBasicFileNameAndPath.TabIndex = 1;
      this.m_edBasicFileNameAndPath.Text = "";
      // 
      // m_btChooseBasicFileNameAndPath
      // 
      this.m_btChooseBasicFileNameAndPath.Location = new System.Drawing.Point(344, 24);
      this.m_btChooseBasicFileNameAndPath.Name = "m_btChooseBasicFileNameAndPath";
      this.m_btChooseBasicFileNameAndPath.Size = new System.Drawing.Size(32, 20);
      this.m_btChooseBasicFileNameAndPath.TabIndex = 2;
      this.m_btChooseBasicFileNameAndPath.Text = "...";
      this.m_btChooseBasicFileNameAndPath.Click += new System.EventHandler(this.EhChooseBasicFileNameAndPath_Click);
      // 
      // m_grpXValues
      // 
      this.m_grpXValues.Controls.Add(this.m_cbXValues_Column);
      this.m_grpXValues.Controls.Add(this.m_rbXValues_FromColumn);
      this.m_grpXValues.Controls.Add(this.m_rbXValuesContinuousNumber);
      this.m_grpXValues.Location = new System.Drawing.Point(16, 120);
      this.m_grpXValues.Name = "m_grpXValues";
      this.m_grpXValues.Size = new System.Drawing.Size(360, 88);
      this.m_grpXValues.TabIndex = 3;
      this.m_grpXValues.TabStop = false;
      this.m_grpXValues.Text = "X-values:";
      // 
      // m_cbXValues_Column
      // 
      this.m_cbXValues_Column.Enabled = false;
      this.m_cbXValues_Column.Location = new System.Drawing.Point(80, 48);
      this.m_cbXValues_Column.Name = "m_cbXValues_Column";
      this.m_cbXValues_Column.Size = new System.Drawing.Size(272, 21);
      this.m_cbXValues_Column.TabIndex = 2;
      this.m_cbXValues_Column.Text = "";
      // 
      // m_rbXValues_FromColumn
      // 
      this.m_rbXValues_FromColumn.Location = new System.Drawing.Point(16, 48);
      this.m_rbXValues_FromColumn.Name = "m_rbXValues_FromColumn";
      this.m_rbXValues_FromColumn.Size = new System.Drawing.Size(64, 16);
      this.m_rbXValues_FromColumn.TabIndex = 1;
      this.m_rbXValues_FromColumn.Text = "Column";
      this.m_rbXValues_FromColumn.CheckedChanged += new System.EventHandler(this.EhXValuesChooseOptions_CheckedChanged);
      // 
      // m_rbXValuesContinuousNumber
      // 
      this.m_rbXValuesContinuousNumber.Checked = true;
      this.m_rbXValuesContinuousNumber.Location = new System.Drawing.Point(16, 24);
      this.m_rbXValuesContinuousNumber.Name = "m_rbXValuesContinuousNumber";
      this.m_rbXValuesContinuousNumber.Size = new System.Drawing.Size(136, 16);
      this.m_rbXValuesContinuousNumber.TabIndex = 0;
      this.m_rbXValuesContinuousNumber.TabStop = true;
      this.m_rbXValuesContinuousNumber.Text = "Continuous number";
      this.m_rbXValuesContinuousNumber.CheckedChanged += new System.EventHandler(this.EhXValuesChooseOptions_CheckedChanged);
      // 
      // m_grpExtendFileName
      // 
      this.m_grpExtendFileName.Controls.Add(this.m_cbExtFileName_Column);
      this.m_grpExtendFileName.Controls.Add(this.m_rbFileName_FromColumn);
      this.m_grpExtendFileName.Controls.Add(this.m_rbExtFileName_ContinuousNumber);
      this.m_grpExtendFileName.Location = new System.Drawing.Point(16, 224);
      this.m_grpExtendFileName.Name = "m_grpExtendFileName";
      this.m_grpExtendFileName.Size = new System.Drawing.Size(360, 88);
      this.m_grpExtendFileName.TabIndex = 4;
      this.m_grpExtendFileName.TabStop = false;
      this.m_grpExtendFileName.Text = "Extend file name by:";
      // 
      // m_cbExtFileName_Column
      // 
      this.m_cbExtFileName_Column.Enabled = false;
      this.m_cbExtFileName_Column.Location = new System.Drawing.Point(80, 48);
      this.m_cbExtFileName_Column.Name = "m_cbExtFileName_Column";
      this.m_cbExtFileName_Column.Size = new System.Drawing.Size(272, 21);
      this.m_cbExtFileName_Column.TabIndex = 2;
      this.m_cbExtFileName_Column.Text = "";
      // 
      // m_rbFileName_FromColumn
      // 
      this.m_rbFileName_FromColumn.Location = new System.Drawing.Point(16, 48);
      this.m_rbFileName_FromColumn.Name = "m_rbFileName_FromColumn";
      this.m_rbFileName_FromColumn.Size = new System.Drawing.Size(64, 16);
      this.m_rbFileName_FromColumn.TabIndex = 1;
      this.m_rbFileName_FromColumn.Text = "Column";
      this.m_rbFileName_FromColumn.CheckedChanged += new System.EventHandler(this.EhFileNameExtendOptions1_CheckedChanged);
      // 
      // m_rbExtFileName_ContinuousNumber
      // 
      this.m_rbExtFileName_ContinuousNumber.Checked = true;
      this.m_rbExtFileName_ContinuousNumber.Location = new System.Drawing.Point(16, 24);
      this.m_rbExtFileName_ContinuousNumber.Name = "m_rbExtFileName_ContinuousNumber";
      this.m_rbExtFileName_ContinuousNumber.Size = new System.Drawing.Size(128, 16);
      this.m_rbExtFileName_ContinuousNumber.TabIndex = 0;
      this.m_rbExtFileName_ContinuousNumber.TabStop = true;
      this.m_rbExtFileName_ContinuousNumber.Text = "Continuous number";
      this.m_rbExtFileName_ContinuousNumber.CheckedChanged += new System.EventHandler(this.EhFileNameExtendOptions1_CheckedChanged);
      // 
      // m_btOK
      // 
      this.m_btOK.Location = new System.Drawing.Point(208, 344);
      this.m_btOK.Name = "m_btOK";
      this.m_btOK.Size = new System.Drawing.Size(80, 24);
      this.m_btOK.TabIndex = 5;
      this.m_btOK.Text = "OK";
      this.m_btOK.Click += new System.EventHandler(this.EhOkButton_Click);
      // 
      // m_btCancel
      // 
      this.m_btCancel.Location = new System.Drawing.Point(296, 344);
      this.m_btCancel.Name = "m_btCancel";
      this.m_btCancel.Size = new System.Drawing.Size(80, 24);
      this.m_btCancel.TabIndex = 6;
      this.m_btCancel.Text = "Cancel";
      this.m_btCancel.Click += new System.EventHandler(this.EhCancelButton_Click);
      // 
      // m_grpCreateSpectrum
      // 
      this.m_grpCreateSpectrum.Controls.Add(this.m_rbCreateSpectrum_FromColumn);
      this.m_grpCreateSpectrum.Controls.Add(this.m_rbCreateSpectrum_FromRow);
      this.m_grpCreateSpectrum.Location = new System.Drawing.Point(16, 56);
      this.m_grpCreateSpectrum.Name = "m_grpCreateSpectrum";
      this.m_grpCreateSpectrum.Size = new System.Drawing.Size(360, 56);
      this.m_grpCreateSpectrum.TabIndex = 7;
      this.m_grpCreateSpectrum.TabStop = false;
      this.m_grpCreateSpectrum.Text = "Create a spectrum from:";
      // 
      // m_rbCreateSpectrum_FromColumn
      // 
      this.m_rbCreateSpectrum_FromColumn.Location = new System.Drawing.Point(136, 24);
      this.m_rbCreateSpectrum_FromColumn.Name = "m_rbCreateSpectrum_FromColumn";
      this.m_rbCreateSpectrum_FromColumn.Size = new System.Drawing.Size(144, 16);
      this.m_rbCreateSpectrum_FromColumn.TabIndex = 1;
      this.m_rbCreateSpectrum_FromColumn.Text = "Data on a column";
      this.m_rbCreateSpectrum_FromColumn.CheckedChanged += new System.EventHandler(this.EhCreateSpectrumFrom_CheckedChanged);
      // 
      // m_rbCreateSpectrum_FromRow
      // 
      this.m_rbCreateSpectrum_FromRow.Checked = true;
      this.m_rbCreateSpectrum_FromRow.Location = new System.Drawing.Point(16, 24);
      this.m_rbCreateSpectrum_FromRow.Name = "m_rbCreateSpectrum_FromRow";
      this.m_rbCreateSpectrum_FromRow.Size = new System.Drawing.Size(96, 16);
      this.m_rbCreateSpectrum_FromRow.TabIndex = 0;
      this.m_rbCreateSpectrum_FromRow.TabStop = true;
      this.m_rbCreateSpectrum_FromRow.Text = "Data on a row";
      this.m_rbCreateSpectrum_FromRow.CheckedChanged += new System.EventHandler(this.EhCreateSpectrumFrom_CheckedChanged);
      // 
      // ExportGalacticSpcFileDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(384, 386);
      this.Controls.Add(this.m_grpCreateSpectrum);
      this.Controls.Add(this.m_btCancel);
      this.Controls.Add(this.m_btOK);
      this.Controls.Add(this.m_grpExtendFileName);
      this.Controls.Add(this.m_grpXValues);
      this.Controls.Add(this.m_btChooseBasicFileNameAndPath);
      this.Controls.Add(this.m_edBasicFileNameAndPath);
      this.Controls.Add(this.m_lbl_BasicFileNameAndPath);
      this.Name = "ExportGalacticSpcFileDialog";
      this.Text = "Export Galactic Spectrum (Spc) Files";
      this.m_grpXValues.ResumeLayout(false);
      this.m_grpExtendFileName.ResumeLayout(false);
      this.m_grpCreateSpectrum.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    private void EhChooseBasicFileNameAndPath_Click(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.ChooseBasicFileNameAndPath();    
    }

    private void EhCreateSpectrumFrom_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhChange_CreateSpectrumFrom();
    }

    private void EhXValuesChooseOptions_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhChange_XValuesFromOptions();
    }

    private void EhFileNameExtendOptions1_CheckedChanged(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhChange_ExtendFileNameOptions();
    }

    private void EhOkButton_Click(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhOk();
    }

    private void EhCancelButton_Click(object sender, System.EventArgs e)
    {
      if(null!=m_Controller)
        m_Controller.EhCancel();
    }

  } // end of class ExportGalacticSpcFileDialog


}
