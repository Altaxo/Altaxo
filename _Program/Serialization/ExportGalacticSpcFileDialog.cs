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

		/// <summary>Designates the source of the x data values.</summary>
		public enum XValuesFrom 
		{
			/// <summary>The x data values are continuous numbers starting from 1.</summary>
			ContinuousNumber,
			/// <summary>The x data values are from a (numeric) column.</summary>
			Column 
		};

		/// <summary>The option for file name extension.</summary>
		public enum ExtendFileNameWith 
		{
			/// <summary>The file name is extended with a continuously increasing number.</summary>
			ContinuousNumber, 
			/// <summary>The file name is extended by the contents of a data column.</summary>
			Column
		};
	}


	/// <summary>
	/// The controller class which controls the <see cref="ExportGalacticSpcFileDialog"/> dialog.
	/// </summary>
	public class ExportGalacticSpcFileDialogController
	{
		/// <summary>
		/// The dialog to control.
		/// </summary>
		private ExportGalacticSpcFileDialog m_Form;
		/// <summary>The table where the data stems from.</summary>
		protected Altaxo.Data.DataTable m_Table;

		/// <summary>The selected rows of the table (only this data are exported).</summary>
		protected Altaxo.Worksheet.IndexSelection m_SelectedRows;

		/// <summary>The selected columns of the table (only this data are exported).</summary>
		protected Altaxo.Worksheet.IndexSelection m_SelectedColumns;

		/// <summary>Stores if one spectrum is created from one table row or from one table column (<see cref="Options.CreateSpectrumFrom"/>).</summary>
		protected Options.CreateSpectrumFrom m_CreateSpectrumFrom;

		/// <summary>Stores if the x values stem from continuous numbering or from a data column (<see cref="Options.XValuesFrom"/></summary>
		protected Options.XValuesFrom m_XValuesFrom;

		/// <summary>Stores if the filename is extended by a continuous number or by the contents of a data column (<see cref="Options.ExtendFileNameWith"/></summary>
		protected Options.ExtendFileNameWith m_ExtendFileNameWith;

		/// <summary>
		/// Creates the controller for the Galactic SPC file export dialog.
		/// </summary>
		/// <param name="dlg">The dialog for which this controller is created.</param>
		/// <param name="table">The data table which is about to be exported.</param>
		/// <param name="selectedRows">The selected rows of the data table.</param>
		/// <param name="selectedColumns">The selected columns of the data table.</param>
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


		/// <summary>
		/// Initialize the elements of the dialog.
		/// </summary>
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


		/// <summary>
		/// Fills the "x values column combobox" with the appropriate column names.
		/// </summary>
		/// <remarks>The column names are either from property columns (if a spectrum is from a row) or from a table column
		///  (if the spectrum is from a column).<para/>
		/// In either case, only DataColumns that have the <see cref="Altaxo.Data.INumericColumn"/> interface are shown in the combobox.</remarks>
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

		/// <summary>
		/// This fills the "Extend file name by column" combobox with appropriate column names.
		/// </summary>
		/// <remarks>The columns shown are either table columns (if a spectrum is a single row), or
		/// a property column (if a spectrum is a single column).</remarks>
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


		/// <summary>
		/// This opens the "Save as" dialog box to choose a basic file name for exporting.
		/// </summary>
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

		/// <summary>
		/// Called if a change in the options "Create spectrum from" occurs.
		/// </summary>
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

		/// <summary>
		/// Called if a change in the options "X values from" occured.
		/// </summary>
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


		/// <summary>
		/// Called if a change in the options "Extend file name by" occured.
		/// </summary>
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

		/// <summary>
		/// Called if user presses the "Ok" button on the dialog box.
		/// </summary>
		public void EhOk()
		{
			if(this.m_CreateSpectrumFrom == Options.CreateSpectrumFrom.Row)
			{
				Altaxo.Data.INumericColumn xcol;

				if(this.m_XValuesFrom == Options.XValuesFrom.Column)
				{
					string colname = m_Form.XValuesColumnName;
					if(null==colname)
					{
						MessageBox.Show(m_Form, "No x-column selected", "Error");
						return;
					}

					xcol = this.m_Table.PropCols[colname] as Altaxo.Data.INumericColumn;
				}
				else // xvalues are continuous number
				{
					xcol = new Altaxo.Data.IndexerColumn();
				}

				Altaxo.Data.DataColumn extFileNameCol=null;
				if(this.m_ExtendFileNameWith == Options.ExtendFileNameWith.Column)
					extFileNameCol = m_Table[m_Form.ExtFileNameColumnName];



				int i,j;
				bool bUseRowSel = (null!=m_SelectedRows && this.m_SelectedRows.Count>0);
				int numOfSpectra = bUseRowSel ? m_SelectedRows.Count : m_Table.RowCount;

				for(j=0;j<numOfSpectra;j++)
				{
					i = bUseRowSel ? m_SelectedRows[j] : j;

					string filename = m_Form.BasicFileName;

					if(null!=extFileNameCol)
						filename += "_" + extFileNameCol[i].ToString();
					else
						filename += "_" + j.ToString();


					string error = Export.FromRow(filename,this.m_Table,i,xcol,this.m_SelectedColumns);

					if(null!=error)
					{
						MessageBox.Show(m_Form,error,"There were error(s) during export!");
						return;
					}
				}
				
				MessageBox.Show(m_Form, string.Format("Export of {0} spectra successfull.",numOfSpectra)); 
			}
			m_Form.DialogResult = DialogResult.OK;
			m_Form.Close();
		}

		/// <summary>
		/// Called if the user pressed the "Cancel" button on the dialog box.
		/// </summary>
		public void EhCancel()
		{
			m_Form.DialogResult = DialogResult.Cancel;
			m_Form.Close();
		}

	} // end of class ExportGalacticSpcFileDialogController



	/// <summary>
	/// This class hosts all routines neccessary to export Galactic SPC files
	/// </summary>
	public class Export
	{


		/// <summary>
		/// Exports a couple of x and y values into a non-evenly spaced Galactic SPC file.
		/// </summary>
		/// <param name="xvalues">The x values of the spectrum.</param>
		/// <param name="yvalues">The y values of the spectrum.</param>
		/// <param name="filename">The filename where to export to.</param>
		/// <returns>Null if successful, otherwise an error description.</returns>
		public static string FromArrays(double [] xvalues, double [] yvalues, string filename)
		{
			int len = xvalues.Length<yvalues.Length ? xvalues.Length:yvalues.Length;

			if(len==0)
			{
				return "Nothing to export - either x-value or y-value array is empty!";
			}

			System.IO.Stream stream=null;

			try
			{
				stream = new System.IO.FileStream(filename,System.IO.FileMode.CreateNew);
				System.IO.BinaryWriter binwriter = new System.IO.BinaryWriter(stream);


				binwriter.Write((byte)0x80); // ftflgs : not-evenly spaced data
				binwriter.Write((byte)0x4B); // fversn : new version
				binwriter.Write((byte)0x00); // fexper : general experimental technique
				binwriter.Write((byte)0x80); // fexp   : fractional scaling exponent (0x80 for floating point)

				binwriter.Write((System.Int32)len); // fnpts  : number of points

				binwriter.Write((double)xvalues[0]); // ffirst : first x-value
				binwriter.Write((double)xvalues[len-1]); // flast : last x-value
				binwriter.Write((System.Int32)1); // fnsub : 1 (one) subfile only
			
				binwriter.Write((byte)0); //  Type of X axis units (see definitions below) 
				binwriter.Write((byte)0); //  Type of Y axis units (see definitions below) 
				binwriter.Write((byte)0); // Type of Z axis units (see definitions below)
				binwriter.Write((byte)0); // Posting disposition (see GRAMSDDE.H)

				binwriter.Write(new byte[0x1E0]); // writing rest of SPC header


				// ---------------------------------------------------------------------
				//   following the x-values array
				// ---------------------------------------------------------------------

				for(int i=0;i<len;i++)
					binwriter.Write((float)xvalues[i]);

				// ---------------------------------------------------------------------
				//   following the y SUBHEADER
				// ---------------------------------------------------------------------

				binwriter.Write((byte)0); // subflgs : always 0
				binwriter.Write((byte)0x80); // subexp : y-values scaling exponent (set to 0x80 means floating point representation)
				binwriter.Write((System.Int16)0); // subindx :  Integer index number of trace subfile (0=first)

				binwriter.Write((float)0); // subtime;	 Floating time for trace (Z axis corrdinate) 
				binwriter.Write((float)0); // subnext;	 Floating time for next trace (May be same as beg) 
				binwriter.Write((float)0); // subnois;	 Floating peak pick noise level if high byte nonzero 

				binwriter.Write((System.Int32)0); // subnpts;	 Integer number of subfile points for TXYXYS type 
				binwriter.Write((System.Int32)0); // subscan;	Integer number of co-added scans or 0 (for collect) 
				binwriter.Write((float)0);        // subwlevel;	 Floating W axis value (if fwplanes non-zero) 
				binwriter.Write((System.Int32)0); // subresv[4];	 Reserved area (must be set to zero) 


				// ---------------------------------------------------------------------
				//   following the y-values array
				// ---------------------------------------------------------------------

				for(int i=0;i<len;i++)
					binwriter.Write((float)yvalues[i]);
			}
			catch(Exception e)
			{
				return e.ToString();
			}
			finally
			{
				if(null!=stream)
					stream.Close();
			}
			
			return null;
		}


		/// <summary>
		/// Exports to a single SPC spectrum from a single table row.
		/// </summary>
		/// <param name="filename">The name of the file where to export to.</param>
		/// <param name="table">The table from which to export.</param>
		/// <param name="rownumber">The number of the table row that contains the data to export.</param>
		/// <param name="xcolumn">The x column that contains the x data.</param>
		/// <param name="selectedColumns">The columns that where selected in the table, i.e. the columns which are exported. If this parameter is null
		/// or no columns are selected, then all data of a row will be exported.</param>
		/// <returns>Null if export was successfull, error description otherwise.</returns>
		public static string FromRow(
			string filename,
			Altaxo.Data.DataTable table,
			int rownumber, 
			Altaxo.Data.INumericColumn xcolumn,
			Altaxo.Worksheet.IndexSelection selectedColumns)
		{

			// test that all x and y cells have numeric values
			bool bUseSel = null!=selectedColumns && selectedColumns.Count>0;
			int spectrumlen = (bUseSel)? selectedColumns.Count : table.ColumnCount ;

			int i,j;

			for(j=0;j<spectrumlen;j++)
			{
				i = bUseSel ? selectedColumns[j] : j;

				if(xcolumn.GetDoubleAt(i) == Double.NaN)
					return string.Format("X column at index {i} has no numeric value!",i);

				if(!(table[i] is Altaxo.Data.INumericColumn))
					return string.Format("Table column[{0}] ({1}) is not a numeric column!",i,table[i].FullName);

				if(((Altaxo.Data.INumericColumn)table[i]).GetDoubleAt(rownumber) == Double.NaN)
					return string.Format("Table cell [{0},{1}] (column {2}) has no numeric value!",i,rownumber,table[i].FullName);
			}


		// this first test was successfull, so start exporting now
	
			double[] xvalues = new double[spectrumlen];
			double[] yvalues = new double[spectrumlen];

			for(j=0;j<spectrumlen;j++)
			{
				i = bUseSel ? selectedColumns[j] : j;
				xvalues[j]= xcolumn.GetDoubleAt(i);
				yvalues[j]= ((Altaxo.Data.INumericColumn)table[i]).GetDoubleAt(rownumber);

			}
			return FromArrays(xvalues,yvalues,filename);
		}


	} // end of class Export
}
