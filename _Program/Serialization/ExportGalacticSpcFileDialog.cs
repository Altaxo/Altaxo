using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Serialization.Galactic
{
	/// <summary>
	/// ExportGalacticSpcFileDialog asks for the options to export files in the Galactic SPC format (<see cref="http://www.galactic.com"/> for details about the file format).
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
		private System.Windows.Forms.RadioButton m_grpCreateSpectrum_FromColumn;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ExportGalacticSpcFileDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}


		public void Initialize(Altaxo.Data.DataTable table, Altaxo.Worksheet.IndexSelection selectedRows, Altaxo.Worksheet.IndexSelection selectedColumns)
		{
			m_Controller = new ExportGalacticSpcFileDialogController(this,table,selectedRows,selectedColumns);
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
			this.m_grpExtendFileName = new System.Windows.Forms.GroupBox();
			this.m_btOK = new System.Windows.Forms.Button();
			this.m_btCancel = new System.Windows.Forms.Button();
			this.m_rbXValuesContinuousNumber = new System.Windows.Forms.RadioButton();
			this.m_rbXValues_FromColumn = new System.Windows.Forms.RadioButton();
			this.m_rbExtFileName_ContinuousNumber = new System.Windows.Forms.RadioButton();
			this.m_rbFileName_FromColumn = new System.Windows.Forms.RadioButton();
			this.m_cbXValues_Column = new System.Windows.Forms.ComboBox();
			this.m_cbExtFileName_Column = new System.Windows.Forms.ComboBox();
			this.m_grpCreateSpectrum = new System.Windows.Forms.GroupBox();
			this.m_rbCreateSpectrum_FromRow = new System.Windows.Forms.RadioButton();
			this.m_grpCreateSpectrum_FromColumn = new System.Windows.Forms.RadioButton();
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
			// m_btOK
			// 
			this.m_btOK.Location = new System.Drawing.Point(208, 344);
			this.m_btOK.Name = "m_btOK";
			this.m_btOK.Size = new System.Drawing.Size(80, 24);
			this.m_btOK.TabIndex = 5;
			this.m_btOK.Text = "OK";
			// 
			// m_btCancel
			// 
			this.m_btCancel.Location = new System.Drawing.Point(296, 344);
			this.m_btCancel.Name = "m_btCancel";
			this.m_btCancel.Size = new System.Drawing.Size(80, 24);
			this.m_btCancel.TabIndex = 6;
			this.m_btCancel.Text = "Cancel";
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
			// 
			// m_rbXValues_FromColumn
			// 
			this.m_rbXValues_FromColumn.Location = new System.Drawing.Point(16, 48);
			this.m_rbXValues_FromColumn.Name = "m_rbXValues_FromColumn";
			this.m_rbXValues_FromColumn.Size = new System.Drawing.Size(64, 16);
			this.m_rbXValues_FromColumn.TabIndex = 1;
			this.m_rbXValues_FromColumn.Text = "Column";
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
			// 
			// m_rbFileName_FromColumn
			// 
			this.m_rbFileName_FromColumn.Location = new System.Drawing.Point(16, 48);
			this.m_rbFileName_FromColumn.Name = "m_rbFileName_FromColumn";
			this.m_rbFileName_FromColumn.Size = new System.Drawing.Size(64, 16);
			this.m_rbFileName_FromColumn.TabIndex = 1;
			this.m_rbFileName_FromColumn.Text = "Column";
			// 
			// m_cbXValues_Column
			// 
			this.m_cbXValues_Column.Enabled = false;
			this.m_cbXValues_Column.Location = new System.Drawing.Point(80, 48);
			this.m_cbXValues_Column.Name = "m_cbXValues_Column";
			this.m_cbXValues_Column.Size = new System.Drawing.Size(272, 21);
			this.m_cbXValues_Column.TabIndex = 2;
			this.m_cbXValues_Column.Text = "comboBox1";
			// 
			// m_cbExtFileName_Column
			// 
			this.m_cbExtFileName_Column.Enabled = false;
			this.m_cbExtFileName_Column.Location = new System.Drawing.Point(80, 48);
			this.m_cbExtFileName_Column.Name = "m_cbExtFileName_Column";
			this.m_cbExtFileName_Column.Size = new System.Drawing.Size(272, 21);
			this.m_cbExtFileName_Column.TabIndex = 2;
			this.m_cbExtFileName_Column.Text = "comboBox1";
			// 
			// m_grpCreateSpectrum
			// 
			this.m_grpCreateSpectrum.Controls.Add(this.m_grpCreateSpectrum_FromColumn);
			this.m_grpCreateSpectrum.Controls.Add(this.m_rbCreateSpectrum_FromRow);
			this.m_grpCreateSpectrum.Location = new System.Drawing.Point(16, 56);
			this.m_grpCreateSpectrum.Name = "m_grpCreateSpectrum";
			this.m_grpCreateSpectrum.Size = new System.Drawing.Size(360, 56);
			this.m_grpCreateSpectrum.TabIndex = 7;
			this.m_grpCreateSpectrum.TabStop = false;
			this.m_grpCreateSpectrum.Text = "Create a spectrum from:";
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
			// 
			// m_grpCreateSpectrum_FromColumn
			// 
			this.m_grpCreateSpectrum_FromColumn.Location = new System.Drawing.Point(136, 24);
			this.m_grpCreateSpectrum_FromColumn.Name = "m_grpCreateSpectrum_FromColumn";
			this.m_grpCreateSpectrum_FromColumn.Size = new System.Drawing.Size(144, 16);
			this.m_grpCreateSpectrum_FromColumn.TabIndex = 1;
			this.m_grpCreateSpectrum_FromColumn.Text = "Data on a column";
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
	} // end of class ExportGalacticSpcFileDialog

	public class ExportGalacticSpcFileDialogController
	{
		private ExportGalacticSpcFileDialog m_Form;
		/// <summary>The table where the data stems from.</summary>
		protected Altaxo.Data.DataTable m_Table;

		/// <summary>The selected rows of the table (only this data are exported).</summary>
		protected Altaxo.Worksheet.IndexSelection m_SelectedRows;

		/// <summary>The selected columns of the table (only this data are exported).</summary>
		protected Altaxo.Worksheet.IndexSelection m_SelectedColumns;


		public ExportGalacticSpcFileDialogController(
			ExportGalacticSpcFileDialog dlg,
			Altaxo.Data.DataTable table,
			Altaxo.Worksheet.IndexSelection selectedRows,
			Altaxo.Worksheet.IndexSelection selectedColumns)
		{
			m_Form = dlg;
			m_Table = table;
			m_SelectedRows = selectedRows;
			m_SelectedColumns = selectedColumns;

			InitializeElements();
		}


		public void InitializeElements
		{
			
		}

	

	} // end of class ExportGalacticSpcFileDialogController
}
