using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

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
		/// Gets / set the basic file name for export.
		/// </summary>
		/// <value>Basic file name for exporting to Galactic SPC format.</value>
		public string BasicFileName
		{
			get { return this.m_edBasicFileNameAndPath.Text; }
			set { this.m_edBasicFileNameAndPath.Text = value; }
		}


		public bool CreateSpectrumFromRow
		{
			get { return this.m_rbCreateSpectrum_FromRow.Checked; }
			set { this.m_rbCreateSpectrum_FromRow.Checked = value; }
		}

		public bool CreateSpectrumFromColumn
		{
			get { return this.m_rbCreateSpectrum_FromColumn.Checked; }
			set { this.m_rbCreateSpectrum_FromColumn.Checked = value; }
		}

		public bool XValuesContinuousNumber
		{
			get { return this.m_rbXValuesContinuousNumber.Checked; }
			set { this.m_rbXValuesContinuousNumber.Checked = value; }
		}

		public bool XValuesFromColumn
		{
			get { return this.m_rbXValues_FromColumn.Checked; }
			set { this.m_rbXValues_FromColumn.Checked = value; }
		}

		public bool ExtendFileName_ContinuousNumber
		{
			get { return this.m_rbExtFileName_ContinuousNumber.Checked; }
			set { this.m_rbExtFileName_ContinuousNumber.Checked = value; }
		}

		public bool ExtendFileName_ByColumn
		{
			get { return this.m_rbFileName_FromColumn.Checked; }
			set { this.m_rbFileName_FromColumn.Checked = value; }
		}

		public void FillXValuesColumnBox(string [] colnames)
		{
			this.m_cbXValues_Column.Items.Clear();
			this.m_cbXValues_Column.Items.AddRange(colnames);
		}

		public bool EnableXValuesColumnBox
		{
			set { this.m_cbXValues_Column.Enabled=value; }
		}

		public void FillExtFileNameColumnBox(string [] colnames)
		{
			this.m_cbExtFileName_Column.Items.Clear();
			this.m_cbExtFileName_Column.Items.AddRange(colnames);
		}

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
			this.m_cbXValues_Column.Text = "comboBox1";
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
			this.m_cbExtFileName_Column.Text = "comboBox1";
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
			m_Controller.ChooseBasicFileNameAndPath();		
		}

		private void EhCreateSpectrumFrom_CheckedChanged(object sender, System.EventArgs e)
		{
			m_Controller.EhChange_CreateSpectrumFrom();
		}

		private void EhXValuesChooseOptions_CheckedChanged(object sender, System.EventArgs e)
		{
			m_Controller.EhChange_XValuesFromOptions();
		}

		private void EhFileNameExtendOptions1_CheckedChanged(object sender, System.EventArgs e)
		{
			m_Controller.EhChange_ExtendFileNameOptions();
		}

		private void EhOkButton_Click(object sender, System.EventArgs e)
		{
			m_Controller.EhOk();
		}

		private void EhCancelButton_Click(object sender, System.EventArgs e)
		{
			m_Controller.EhCancel();
		}

	} // end of class ExportGalacticSpcFileDialog


	namespace Options
	{

		/// <summary>
		/// Creating a spectrum either from either a row or from a column.
		/// </summary>
		/// <remarks>Choosing creating a spectrum from a row, the values of a single table row (or parts of it) are used to create
		/// a spectrum, i.e. the y-values of the spectrum. In this case the x-values can come from a numeric property column.<para/>
		/// Creating a spectrum from a column (or parts of a column, i.e. some rows of it) means the y-values
		/// of the spectrum stem from a single column of the data table. The x-values of the spectrum then have
		/// to come from another (numeric) column of the table.</remarks>
		public enum CreateSpectrumFrom
		{
			/// <summary>The y-values of one spectrum stem from one single table row.</summary>
			Row,
			/// <summary>The y-values of one spectrum stem from one single table column.</summary>
			Column 
		};

		public enum XValuesFrom { ContinuousNumber, Column };

		public enum ExtendFileNameWith { ContinuousNumber, Column };
	}


	public class ExportGalacticSpcFileDialogController
	{
		private ExportGalacticSpcFileDialog m_Form;
		/// <summary>The table where the data stems from.</summary>
		protected Altaxo.Data.DataTable m_Table;

		/// <summary>The selected rows of the table (only this data are exported).</summary>
		protected Altaxo.Worksheet.IndexSelection m_SelectedRows;

		/// <summary>The selected columns of the table (only this data are exported).</summary>
		protected Altaxo.Worksheet.IndexSelection m_SelectedColumns;




		protected Options.CreateSpectrumFrom m_CreateSpectrumFrom;

		protected Options.XValuesFrom m_XValuesFrom;

		protected Options.ExtendFileNameWith m_ExtendFileNameWith;

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


		public void InitializeElements()
		{
			this.m_CreateSpectrumFrom = Options.CreateSpectrumFrom.Row;
			m_Form.CreateSpectrumFromRow=true;

			this.m_XValuesFrom = Options.XValuesFrom.ContinuousNumber;
			m_Form.XValuesContinuousNumber=true;

			this.m_ExtendFileNameWith = Options.ExtendFileNameWith.ContinuousNumber;
			m_Form.ExtendFileName_ContinuousNumber=true;
			m_Form.BasicFileName="";
		}


		public void FillXValuesColumnBox()
		{
			Altaxo.Data.DataColumnCollection colcol;
			if(this.m_CreateSpectrumFrom==Options.CreateSpectrumFrom.Row)
				colcol = m_Table.PropCols;
			else
				colcol = m_Table;

			// count the number of numeric columns
			int numnumeric=0;
			for(int i=0;i<colcol.ColumnCount;i++)
				if(colcol[i] is Altaxo.Data.INumericColumn)
					++numnumeric;

			// Fill the Combo Box with Column names
			string[] colnames = new string[numnumeric];
			for(int i=0,j=0;i<colcol.ColumnCount;i++)
				if(colcol[i] is Altaxo.Data.INumericColumn)
					colnames[j++]= colcol[i].ColumnName;
				
			// now set the contents of the combo box
			m_Form.FillXValuesColumnBox(colnames);
			m_Form.EnableXValuesColumnBox=true;
		}

		public void FillExtFileNameColumnBox()
		{
			Altaxo.Data.DataColumnCollection colcol;
			if(this.m_CreateSpectrumFrom==Options.CreateSpectrumFrom.Row)
				colcol = m_Table;
			else
				colcol = m_Table.PropCols;

			// Fill the Combo Box with Column names
			string[] colnames = new string[colcol.ColumnCount];
			for(int i=0;i<colcol.ColumnCount;i++)
				colnames[i]= colcol[i].ColumnName;
				
			// now set the contents of the combo box
			m_Form.FillExtFileNameColumnBox(colnames);
			m_Form.EnableExtFileNameColumnBox=true;
		}


		public void ChooseBasicFileNameAndPath()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
 
			saveFileDialog.Filter = "All files (*.*)|*.*"  ;
			saveFileDialog.FilterIndex = 1 ;
			saveFileDialog.RestoreDirectory = true ;
 
			if(saveFileDialog.ShowDialog(m_Form) == DialogResult.OK)
			{
				m_Form.BasicFileName = saveFileDialog.FileName;
			}
		}

		public void EhChange_CreateSpectrumFrom()
		{
			Options.CreateSpectrumFrom oldValue = m_CreateSpectrumFrom;

			if(m_Form.CreateSpectrumFromRow)
				m_CreateSpectrumFrom = Options.CreateSpectrumFrom.Row;
			else if(m_Form.CreateSpectrumFromColumn)
				m_CreateSpectrumFrom = Options.CreateSpectrumFrom.Column;
		
			if(m_CreateSpectrumFrom!=oldValue)
			{
				if(this.m_XValuesFrom == Options.XValuesFrom.Column)
					this.FillXValuesColumnBox();

				if(this.m_ExtendFileNameWith == Options.ExtendFileNameWith.Column)
					this.FillExtFileNameColumnBox();
			}
		}

		public void EhChange_XValuesFromOptions()
		{
			Options.XValuesFrom oldValue = m_XValuesFrom;

			if(m_Form.XValuesContinuousNumber)
				m_XValuesFrom = Options.XValuesFrom.ContinuousNumber;
			else if(m_Form.XValuesFromColumn)
				m_XValuesFrom = Options.XValuesFrom.Column;

			if(m_XValuesFrom != oldValue)
			{
				if(m_XValuesFrom==Options.XValuesFrom.Column)
				{
					this.FillXValuesColumnBox();
				}
				else
				{
					m_Form.EnableXValuesColumnBox=false;
				}
			}
		}


		public void EhChange_ExtendFileNameOptions()
		{
			Options.ExtendFileNameWith oldValue = this.m_ExtendFileNameWith;

			if(m_Form.ExtendFileName_ContinuousNumber)
				this.m_ExtendFileNameWith = Options.ExtendFileNameWith.ContinuousNumber;
			else if(m_Form.ExtendFileName_ByColumn)
				this.m_ExtendFileNameWith = Options.ExtendFileNameWith.Column;

			if(this.m_ExtendFileNameWith != oldValue)
			{
				if(this.m_ExtendFileNameWith==Options.ExtendFileNameWith.Column)
				{
					// now set the contents of the combo box
					this.FillExtFileNameColumnBox();
				}
				else
				{
					m_Form.EnableExtFileNameColumnBox=false;
				}

			}
		}

		public void EhOk()
		{
			m_Form.DialogResult = DialogResult.OK;
			m_Form.Close();
		}

		public void EhCancel()
		{
			m_Form.DialogResult = DialogResult.Cancel;
			m_Form.Close();
		}

	} // end of class ExportGalacticSpcFileDialogController
}
