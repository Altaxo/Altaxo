/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using Altaxo.Serialization;


namespace Altaxo.Worksheet
{


	/// <summary>
	/// Summary description for Altaxo.Worksheet.
	/// </summary>
	[SerializationSurrogate(0,typeof(Worksheet.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version.")]
	public class Worksheet : System.Windows.Forms.Form
	{
		protected internal DataGrid altaxoDataGrid1;
		private AltaxoDocument parentDocument =null;
		private System.Windows.Forms.MainMenu wksMenu;
		private System.Windows.Forms.MenuItem mnuAddColumn;
		private System.Windows.Forms.MenuItem menuSetColumnValues;
		private System.Windows.Forms.MenuItem mnuWorksheet;
		private System.Windows.Forms.MenuItem mnuWorksheetImportAscii;
		private System.Windows.Forms.MenuItem menuEditRemove;
		private System.Windows.Forms.MenuItem menuEditPopup;
		private System.Windows.Forms.MenuItem menuColumnPopup;
		private System.Windows.Forms.MenuItem menuColumnSetAsX;
		private System.Windows.Forms.MenuItem menuPlotPopup;
		private System.Windows.Forms.MenuItem menuPlotLine;
		private System.Windows.Forms.MenuItem menuAnalysisPopup;
		private System.Windows.Forms.MenuItem menuAnalysisFFT;
		private System.Windows.Forms.MenuItem menuWorksheetImportPicture;
		private System.Windows.Forms.MenuItem menuWorksheetTranspose;
		private System.Windows.Forms.MenuItem menuEditCopy;
		private System.Windows.Forms.MenuItem menuEditPaste;
		private System.Windows.Forms.MenuItem m_Menu_Analysis_StatisticsOnColumns;
		private System.Windows.Forms.MenuItem menuFilePopup;
		private System.Windows.Forms.MenuItem menuFile_ExportAscii;
		private System.Windows.Forms.MenuItem menuColumn_AddPropertyColumn;
		private System.Windows.Forms.MenuItem menuColumn_ExtractPropertyValues;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Serialization
		public class SerializationSurrogate0 : IDeserializationSubstitute, System.Runtime.Serialization.ISerializationSurrogate, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
		{
			protected Point m_Location;
			protected Size m_Size;
			protected object m_Grid=null;

			// we need a empty constructor
			public SerializationSurrogate0() {}

			// not used for deserialization, since the ISerializable constructor is used for that
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector){return obj;}
			// not used for serialization, instead the ISerializationSurrogate is used for that
			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)	{}

			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				info.SetType(this.GetType());
				Worksheet s = (Worksheet)obj;
				info.AddValue("Location",s.Location);
				info.AddValue("Size",s.Size);
				info.AddValue("DataGrid",s.altaxoDataGrid1);
				
			}

			public SerializationSurrogate0(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
			{
				m_Location = (Point)info.GetValue("Location",typeof(Point));
				m_Size     = (Size)info.GetValue("Size",typeof(Size));
				m_Grid = info.GetValue("DataGrid",typeof(object));
			}

			public void OnDeserialization(object o)
			{
			}

			public object GetRealObject(object parent)
			{
				// Create a new worksheet, parent window is the application window
				Worksheet wks = new Worksheet(App.CurrentApplication,null,null);
				wks.Location = m_Location;
				wks.Size = m_Size;

				// Change the IDeserializationSurrogate of the data grid control to the real object
				// parent of the data grid is the worksheet created above
				m_Grid = ((IDeserializationSubstitute)m_Grid).GetRealObject(wks);
				wks.DataGrid = m_Grid as DataGrid;
				wks.DataGrid.Size = wks.ClientSize;
				wks.Text = wks.DataGrid.DataTable.TableName;
				m_Grid = null;
				return wks;
			}
		}
		#endregion


		public Worksheet(System.Windows.Forms.Form parent, AltaxoDocument doc, Altaxo.Data.DataTable theTable)
		{
			
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			parentDocument = doc;
			altaxoDataGrid1.DataTable = theTable;
			
			this.MdiParent=parent;
			this.Show();
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.altaxoDataGrid1 = new Altaxo.Worksheet.DataGrid();
			this.wksMenu = new System.Windows.Forms.MainMenu();
			this.menuFilePopup = new System.Windows.Forms.MenuItem();
			this.menuFile_ExportAscii = new System.Windows.Forms.MenuItem();
			this.menuEditPopup = new System.Windows.Forms.MenuItem();
			this.menuEditRemove = new System.Windows.Forms.MenuItem();
			this.menuEditCopy = new System.Windows.Forms.MenuItem();
			this.menuEditPaste = new System.Windows.Forms.MenuItem();
			this.menuPlotPopup = new System.Windows.Forms.MenuItem();
			this.menuPlotLine = new System.Windows.Forms.MenuItem();
			this.mnuWorksheet = new System.Windows.Forms.MenuItem();
			this.mnuWorksheetImportAscii = new System.Windows.Forms.MenuItem();
			this.menuWorksheetImportPicture = new System.Windows.Forms.MenuItem();
			this.menuWorksheetTranspose = new System.Windows.Forms.MenuItem();
			this.menuColumnPopup = new System.Windows.Forms.MenuItem();
			this.mnuAddColumn = new System.Windows.Forms.MenuItem();
			this.menuSetColumnValues = new System.Windows.Forms.MenuItem();
			this.menuColumnSetAsX = new System.Windows.Forms.MenuItem();
			this.menuAnalysisPopup = new System.Windows.Forms.MenuItem();
			this.menuAnalysisFFT = new System.Windows.Forms.MenuItem();
			this.m_Menu_Analysis_StatisticsOnColumns = new System.Windows.Forms.MenuItem();
			this.menuColumn_AddPropertyColumn = new System.Windows.Forms.MenuItem();
			this.menuColumn_ExtractPropertyValues = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// altaxoDataGrid1
			// 
			this.altaxoDataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.altaxoDataGrid1.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.altaxoDataGrid1.Location = new System.Drawing.Point(0, 0);
			this.altaxoDataGrid1.Name = "altaxoDataGrid1";
			this.altaxoDataGrid1.Size = new System.Drawing.Size(392, 250);
			this.altaxoDataGrid1.TabIndex = 0;
			// 
			// wksMenu
			// 
			this.wksMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																						this.menuFilePopup,
																																						this.menuEditPopup,
																																						this.menuPlotPopup,
																																						this.mnuWorksheet,
																																						this.menuColumnPopup,
																																						this.menuAnalysisPopup});
			// 
			// menuFilePopup
			// 
			this.menuFilePopup.Index = 0;
			this.menuFilePopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																									this.menuFile_ExportAscii});
			this.menuFilePopup.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuFilePopup.Text = "File";
			// 
			// menuFile_ExportAscii
			// 
			this.menuFile_ExportAscii.Index = 0;
			this.menuFile_ExportAscii.Text = "Export Ascii...";
			this.menuFile_ExportAscii.Click += new System.EventHandler(this.OnFile_ExportAscii);
			// 
			// menuEditPopup
			// 
			this.menuEditPopup.Index = 1;
			this.menuEditPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																									this.menuEditRemove,
																																									this.menuEditCopy,
																																									this.menuEditPaste});
			this.menuEditPopup.MergeOrder = 1;
			this.menuEditPopup.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuEditPopup.Text = "Edit";
			this.menuEditPopup.Popup += new System.EventHandler(this.menuEditPopup_Popup);
			// 
			// menuEditRemove
			// 
			this.menuEditRemove.Index = 0;
			this.menuEditRemove.Text = "Remove";
			this.menuEditRemove.Click += new System.EventHandler(this.menuEditRemove_Click);
			// 
			// menuEditCopy
			// 
			this.menuEditCopy.Index = 1;
			this.menuEditCopy.Text = "Copy";
			this.menuEditCopy.Click += new System.EventHandler(this.menuEditCopy_Click);
			// 
			// menuEditPaste
			// 
			this.menuEditPaste.Index = 2;
			this.menuEditPaste.Text = "Paste";
			this.menuEditPaste.Click += new System.EventHandler(this.menuEditPaste_Click);
			// 
			// menuPlotPopup
			// 
			this.menuPlotPopup.Index = 2;
			this.menuPlotPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																									this.menuPlotLine});
			this.menuPlotPopup.MergeOrder = 2;
			this.menuPlotPopup.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuPlotPopup.Text = "Plot";
			// 
			// menuPlotLine
			// 
			this.menuPlotLine.Index = 0;
			this.menuPlotLine.Text = "Line";
			this.menuPlotLine.Click += new System.EventHandler(this.menuPlotLine_Click);
			// 
			// mnuWorksheet
			// 
			this.mnuWorksheet.Index = 3;
			this.mnuWorksheet.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																								 this.mnuWorksheetImportAscii,
																																								 this.menuWorksheetImportPicture,
																																								 this.menuWorksheetTranspose});
			this.mnuWorksheet.MergeOrder = 3;
			this.mnuWorksheet.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.mnuWorksheet.Text = "Worksheet";
			// 
			// mnuWorksheetImportAscii
			// 
			this.mnuWorksheetImportAscii.Index = 0;
			this.mnuWorksheetImportAscii.Text = "Import Ascii";
			this.mnuWorksheetImportAscii.Click += new System.EventHandler(this.mnuWorksheetImportAscii_Click);
			// 
			// menuWorksheetImportPicture
			// 
			this.menuWorksheetImportPicture.Index = 1;
			this.menuWorksheetImportPicture.Text = "Import Picture";
			this.menuWorksheetImportPicture.Click += new System.EventHandler(this.menuWorksheetImportPicture_Click);
			// 
			// menuWorksheetTranspose
			// 
			this.menuWorksheetTranspose.Index = 2;
			this.menuWorksheetTranspose.Text = "Transpose";
			this.menuWorksheetTranspose.Click += new System.EventHandler(this.menuWorksheetTranspose_Click);
			// 
			// menuColumnPopup
			// 
			this.menuColumnPopup.Index = 4;
			this.menuColumnPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																										this.mnuAddColumn,
																																										this.menuSetColumnValues,
																																										this.menuColumnSetAsX,
																																										this.menuColumn_AddPropertyColumn,
																																										this.menuColumn_ExtractPropertyValues});
			this.menuColumnPopup.MergeOrder = 4;
			this.menuColumnPopup.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuColumnPopup.Text = "Column";
			this.menuColumnPopup.Popup += new System.EventHandler(this.menuColumnPopup_Popup);
			// 
			// mnuAddColumn
			// 
			this.mnuAddColumn.Index = 0;
			this.mnuAddColumn.Text = "Add";
			this.mnuAddColumn.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuSetColumnValues
			// 
			this.menuSetColumnValues.Index = 1;
			this.menuSetColumnValues.Text = "Set Column Values ...";
			this.menuSetColumnValues.Click += new System.EventHandler(this.SetColumnValues_Click);
			// 
			// menuColumnSetAsX
			// 
			this.menuColumnSetAsX.Index = 2;
			this.menuColumnSetAsX.Text = "Set as X";
			this.menuColumnSetAsX.Click += new System.EventHandler(this.menuColumnSetAsX_Click);
			// 
			// menuAnalysisPopup
			// 
			this.menuAnalysisPopup.Index = 5;
			this.menuAnalysisPopup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																											this.menuAnalysisFFT,
																																											this.m_Menu_Analysis_StatisticsOnColumns});
			this.menuAnalysisPopup.MergeOrder = 5;
			this.menuAnalysisPopup.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuAnalysisPopup.Text = "Analysis";
			// 
			// menuAnalysisFFT
			// 
			this.menuAnalysisFFT.Index = 0;
			this.menuAnalysisFFT.Text = "FFT";
			this.menuAnalysisFFT.Click += new System.EventHandler(this.menuAnalysisFFT_Click);
			// 
			// m_Menu_Analysis_StatisticsOnColumns
			// 
			this.m_Menu_Analysis_StatisticsOnColumns.Index = 1;
			this.m_Menu_Analysis_StatisticsOnColumns.Text = "Statistics on Columns";
			this.m_Menu_Analysis_StatisticsOnColumns.Click += new System.EventHandler(this.OnAnalysis_StatisticsOnColumns);
			// 
			// menuColumn_AddPropertyColumn
			// 
			this.menuColumn_AddPropertyColumn.Index = 3;
			this.menuColumn_AddPropertyColumn.Text = "Add property column";
			this.menuColumn_AddPropertyColumn.Click += new System.EventHandler(this.menuColumn_AddPropertyColumn_Click);
			// 
			// menuColumn_ExtractPropertyValues
			// 
			this.menuColumn_ExtractPropertyValues.Index = 4;
			this.menuColumn_ExtractPropertyValues.Text = "Extract property values";
			this.menuColumn_ExtractPropertyValues.Click += new System.EventHandler(this.menuColumn_ExtractPropertyValues_Click);
			// 
			// Worksheet
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 248);
			this.Controls.Add(this.altaxoDataGrid1);
			this.Menu = this.wksMenu;
			this.Name = "Worksheet";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		

		public Altaxo.Worksheet.DataGrid DataGrid
		{
			get { return altaxoDataGrid1; }
			set
			{
				if(null!=altaxoDataGrid1)
				{
					altaxoDataGrid1.Hide();
					altaxoDataGrid1.Dispose();
				}
				altaxoDataGrid1 = value;
				altaxoDataGrid1.Parent=this;
				this.altaxoDataGrid1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
					| System.Windows.Forms.AnchorStyles.Left) 
					| System.Windows.Forms.AnchorStyles.Right);
				this.altaxoDataGrid1.BackColor = System.Drawing.SystemColors.AppWorkspace;
				this.altaxoDataGrid1.FirstVisibleColumn = 0;
				this.altaxoDataGrid1.FirstVisibleTableRow = 0;
				this.altaxoDataGrid1.Name = "altaxoDataGrid1";
				this.altaxoDataGrid1.TabIndex = 0;
				altaxoDataGrid1.Size = this.Size;
				altaxoDataGrid1.Show();
			}
		}

		private void hScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
		
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			Altaxo.Data.DoubleColumn nc = new Altaxo.Data.DoubleColumn(altaxoDataGrid1.DataTable.FindNewColumnName());
			altaxoDataGrid1.DataTable.Add(nc);
			altaxoDataGrid1.Invalidate();
		}

		private void SetColumnValues_Click(object sender, System.EventArgs e)
		{
	
			if(altaxoDataGrid1.SelectedColumns.Count<=0)
				return; // no column selected

			Altaxo.Data.DataColumn dataCol = altaxoDataGrid1.DataTable[altaxoDataGrid1.SelectedColumns[0]];
			if(null==dataCol)
				return;

			//Data.ColumnScript colScript = (Data.ColumnScript)altaxoDataGrid1.columnScripts[dataCol];

			Data.ColumnScript colScript = altaxoDataGrid1.DataTable.ColumnScripts[dataCol];

			SetColumnValuesDialog dlg = new SetColumnValuesDialog(altaxoDataGrid1.DataTable,dataCol,colScript);
			DialogResult dres = dlg.ShowDialog(this);
			if(dres==DialogResult.OK)
			{
				if(colScript==null)	// store the column script in the hash table if not already there
				{
					//altaxoDataGrid1.columnScripts.Add(dataCol,dlg.columnScript);
					altaxoDataGrid1.DataTable.ColumnScripts[dataCol]=dlg.columnScript;
				}
				else
				{
					//altaxoDataGrid1.columnScripts[dataCol] = (Data.ColumnScript)dlg.columnScript.Clone(); // if in the hash table already, simply copy the data
					altaxoDataGrid1.DataTable.ColumnScripts[dataCol] = (Data.ColumnScript)dlg.columnScript.Clone(); // if in the hash table already, simply copy the data
				}
			}
			dlg.Dispose();
		}

		private void mnuWorksheetImportAscii_Click(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\" ;
			openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 2 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile())!= null)
				{
					AltaxoAsciiImporter importer = new AltaxoAsciiImporter(myStream);
					AsciiImportOptions recognizedOptions = importer.Analyze(30, new AsciiImportOptions());
					importer.ImportAscii(recognizedOptions,this.altaxoDataGrid1.DataTable);
					myStream.Close();
				}
			}

		}

		private void menuEditRemove_Click(object sender, System.EventArgs e)
		{
			this.altaxoDataGrid1.RemoveSelected();
		}

		private void menuEditPopup_Popup(object sender, System.EventArgs e)
		{
			this.menuEditRemove.Enabled = (altaxoDataGrid1.SelectedColumns.Count>0 || altaxoDataGrid1.SelectedRows.Count>0);
		}

		private void menuColumnPopup_Popup(object sender, System.EventArgs e)
		{
			this.menuSetColumnValues.Enabled = 1==altaxoDataGrid1.SelectedColumns.Count;
		}

		private void menuColumnSetAsX_Click(object sender, System.EventArgs e)
		{
			altaxoDataGrid1.SetSelectedColumnAsX();
		}

		private void menuPlotLine_Click(object sender, System.EventArgs e)
		{
			DataGridOperations.PlotLine(altaxoDataGrid1);
		}

		private void menuAnalysisFFT_Click(object sender, System.EventArgs e)
		{
			DataGridOperations.FFT(altaxoDataGrid1);
		}

		private void menuWorksheetImportPicture_Click(object sender, System.EventArgs e)
		{
			DataGridOperations.ImportPicture(altaxoDataGrid1);
		}

		private void menuWorksheetTranspose_Click(object sender, System.EventArgs e)
		{
			string msg = altaxoDataGrid1.DataTable.Transpose();

			if(null!=msg)
				System.Windows.Forms.MessageBox.Show(this,msg);
		}

		private void menuEditCopy_Click(object sender, System.EventArgs e)
		{
		
			// Copy the selected Columns to the clipboard
			DataGridOperations.CopyToClipboard(this.altaxoDataGrid1);

		}

		private void menuEditPaste_Click(object sender, System.EventArgs e)
		{
		
		}

		private void OnAnalysis_StatisticsOnColumns(object sender, System.EventArgs e)
		{
			DataGridOperations.StatisticsOnColumns(this.DataGrid);
		}

		private void OnFile_ExportAscii(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream ;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
			saveFileDialog1.Filter = "Ascii files (*.txt)|*.txt|All files (*.*)|*.*"  ;
			saveFileDialog1.FilterIndex = 2 ;
			saveFileDialog1.RestoreDirectory = true ;
 
			if(saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = saveFileDialog1.OpenFile()) != null)
				{
					try
					{
						Altaxo.Serialization.AsciiExporter.ExportAscii(myStream, this.altaxoDataGrid1.DataTable,'\t');
					}
					catch(Exception ex)
					{
						System.Windows.Forms.MessageBox.Show(this,"There was an error during ascii export, details follow:\n" + ex.ToString());
					}
					finally
					{
						myStream.Close();
					}
				}
	
			}
		}

		private void menuColumn_AddPropertyColumn_Click(object sender, System.EventArgs e)
		{
			Altaxo.Data.TextColumn nc = new Altaxo.Data.TextColumn(altaxoDataGrid1.DataTable.PropCols.FindNewColumnName());
			altaxoDataGrid1.DataTable.PropCols.Add(nc);
			altaxoDataGrid1.Invalidate();
		}

		private void menuColumn_ExtractPropertyValues_Click(object sender, System.EventArgs e)
		{
			// extract the properties from the (first) selected property column
			if(altaxoDataGrid1.SelectedPropertyColumns.Count==0)
				return;

			Altaxo.Data.DataColumn col = altaxoDataGrid1.DataTable.PropCols[altaxoDataGrid1.SelectedPropertyColumns[0]];

			DataGridOperations.ExtractPropertiesFromColumn(col,altaxoDataGrid1.DataTable.PropCols);
		}

	} // end of class
}

