using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;
using Altaxo.Serialization;



namespace Altaxo.Worksheet
{
	/// <summary>
	/// Default controller which implements ITableController.
	/// </summary>
	[SerializationSurrogate(0,typeof(TableController.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class TableController : ITableController,  System.Runtime.Serialization.IDeserializationCallback
	{
		public enum SelectionType { Nothing, DataRowSelection, DataColumnSelection, PropertyColumnSelection }


		#region Member variables
		
		/// <summary>
		/// Used to indicate that deserialization has not finished, and holds some deserialized values.
		/// </summary>
		private object m_DeserializationSurrogate;

		/// <summary>Holds the data table.</summary>
		protected Altaxo.Data.DataTable m_Table;

		/// <summary>Holds the view (the window where the graph is visualized).</summary>
		protected ITableView m_View;
		
		/// <summary>The main menu of this controller.</summary>
		protected System.Windows.Forms.MainMenu m_MainMenu; 
		protected System.Windows.Forms.MenuItem m_MenuItemEditRemove;
		protected System.Windows.Forms.MenuItem m_MenuItemColumnSetColumnValues;

		/// <summary>Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
		protected SelectionType m_LastSelectionType;

		/// <summary>
		/// defaultColumnsStyles stores the default column Styles in a Hashtable
		/// the key for the hash table is the Type of the ColumnStyle
		/// </summary>
		protected System.Collections.Hashtable m_DefaultColumnStyles;

		/// <summary>
		/// m_ColumnStyles stores the column styles for each data column individually,
		/// key is the data column itself.
		/// There is no need to store a column style here if the column is styled as default,
		/// instead the defaultColumnStyle is used in this case
		/// </summary>
		protected internal System.Collections.Hashtable m_ColumnStyles;


		/// <summary>
		/// The style of the row header. This is the leftmost column that shows usually the row number.
		/// </summary>
		protected RowHeaderStyle m_RowHeaderStyle; // holds the style of the row header (leftmost column of data grid)
	
		/// <summary>
		/// The style of the column header. This is the upmost row that shows the name of the columns.
		/// </summary>
		protected ColumnHeaderStyle m_ColumnHeaderStyle; // the style of the column header (uppermost row of datagrid)
	
		/// <summary>
		/// The style of the property column header. This is the leftmost column in the left of the property columns,
		/// that shows the names of the property columns.
		/// </summary>
		protected ColumnHeaderStyle m_PropertyColumnHeaderStyle;
		
		/// <summary>
		/// holds the positions (int) of the right boundarys of the __visible__ (!) columns
		/// i.e. columnBordersCache[0] is the with of the rowHeader plus the width of column[0]
		/// </summary>
		protected ColumnStyleCache m_ColumnStyleCache;
		
		
		/// <summary>
		/// Horizontal scroll position; number of first column that is shown.
		/// </summary>
		private int m_HorzScrollPos;
		/// <summary>
		/// Vertical scroll position; Positive values: number of first data column
		/// that is shown. Negative Values scroll more up in case of property columns.
		/// </summary>
		private int m_VertScrollPos;
		private int m_HorzScrollMax;
		private int m_VertScrollMax;

		private int  m_LastVisibleColumn;
		private int  m_LastFullyVisibleColumn;

		
		/// <summary>
		/// Holds the indizes to the selected data columns.
		/// </summary>
		protected IndexSelection m_SelectedColumns; // holds the selected columns
		
		/// <summary>
		/// Holds the indizes to the selected rows.
		/// </summary>
		protected IndexSelection m_SelectedRows; // holds the selected rows
		
		/// <summary>
		/// Holds the indizes to the selected property columns.
		/// </summary>
		protected IndexSelection m_SelectedPropertyColumns; // holds the selected property columns


		/// <summary>
		/// Cached number of table rows.
		/// </summary>
		protected int m_NumberOfTableRows; // cached number of rows of the table
		/// <summary>
		/// Cached number of table columns.
		/// </summary>
		protected int m_NumberOfTableCols;
		
		/// <summary>
		/// Cached number of property columns.
		/// </summary>
		protected int m_NumberOfPropertyCols; // cached number of property  columnsof the table
		
		/// <summary>
		/// The visibility of the property columns in the view. If true, the property columns are shown in the view.
		/// </summary>
		protected bool m_ShowPropertyColumns; // are the property columns visible?
		
		

		private Point m_MouseDownPosition; // holds the position of a double click
		private int  m_DragColumnWidth_ColumnNumber; // stores the column number if mouse hovers over separator
		private int  m_DragColumnWidth_OriginalPos;
		private int  m_DragColumnWidth_OriginalWidth;
		private bool m_DragColumnWidth_InCapture;
	

		private bool                         m_CellEdit_IsArmed;
		private ClickedCellInfo							 m_CellEdit_EditedCell;
		private System.Windows.Forms.TextBox m_CellEditControl; 


		/// <summary>
		/// Set the member variables to default values. Intended only for use in constructors and deserialization code.
		/// </summary>
		protected virtual void SetMemberVariablesToDefault()
		{
		m_DeserializationSurrogate=null;

		m_Table=null;

		m_View = null;
		
		// The main menu of this controller.
		m_MainMenu = null; 
		m_MenuItemEditRemove = null;
		m_MenuItemColumnSetColumnValues = null;

		// Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
		m_LastSelectionType = SelectionType.Nothing;

		// defaultColumnsStyles stores the default column Styles in a Hashtable
		m_DefaultColumnStyles = new System.Collections.Hashtable();

		// m_ColumnStyles stores the column styles for each data column individually,
		m_ColumnStyles = new System.Collections.Hashtable();


		// The style of the row header. This is the leftmost column that shows usually the row number.
		m_RowHeaderStyle = new RowHeaderStyle(); // holds the style of the row header (leftmost column of data grid)
	
		// The style of the column header. This is the upmost row that shows the name of the columns.
		m_ColumnHeaderStyle = new ColumnHeaderStyle(); // the style of the column header (uppermost row of datagrid)
	
		// The style of the property column header. This is the leftmost column in the left of the property columns,
		m_PropertyColumnHeaderStyle = new ColumnHeaderStyle();
		
		// holds the positions (int) of the right boundarys of the __visible__ (!) columns
		m_ColumnStyleCache = new ColumnStyleCache();
		
		
		// Horizontal scroll position; number of first column that is shown.
		m_HorzScrollPos=0;
		
		// Vertical scroll position; Positive values: number of first data column
		m_VertScrollPos=0;
		m_HorzScrollMax=1;
		m_VertScrollMax=1;

		m_LastVisibleColumn=0;
		m_LastFullyVisibleColumn=0;

		
		// Holds the indizes to the selected data columns.
		m_SelectedColumns = new Altaxo.Worksheet.IndexSelection(); // holds the selected columns
		
		// Holds the indizes to the selected rows.
		m_SelectedRows    = new Altaxo.Worksheet.IndexSelection(); // holds the selected rows
		
		// Holds the indizes to the selected property columns.
		m_SelectedPropertyColumns = new Altaxo.Worksheet.IndexSelection(); // holds the selected property columns


		// Cached number of table rows.
		m_NumberOfTableRows=0; // cached number of rows of the table

		// Cached number of table columns.
		m_NumberOfTableCols=0;
		
		// Cached number of property columns.
		m_NumberOfPropertyCols=0; // cached number of property  columnsof the table
		
		// The visibility of the property columns in the view. If true, the property columns are shown in the view.
		m_ShowPropertyColumns=true; // are the property columns visible?
		
		

		m_MouseDownPosition = new Point(0,0); // holds the position of a double click
		m_DragColumnWidth_ColumnNumber=int.MinValue; // stores the column number if mouse hovers over separator
		m_DragColumnWidth_OriginalPos = 0;
		m_DragColumnWidth_OriginalWidth=0;
		m_DragColumnWidth_InCapture=false;
	

		m_CellEdit_IsArmed=false;
		m_CellEdit_EditedCell = new ClickedCellInfo();


			m_CellEditControl = new System.Windows.Forms.TextBox();
			m_CellEditControl.AcceptsTab = true;
			m_CellEditControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			m_CellEditControl.Location = new System.Drawing.Point(392, 0);
			m_CellEditControl.Multiline = true;
			m_CellEditControl.Name = "m_CellEditControl";
			m_CellEditControl.TabIndex = 0;
			m_CellEditControl.Text = "";
			m_CellEditControl.Hide();
			m_CellEditControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnCellEditControl_KeyDown);
			m_CellEditControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnCellEditControl_KeyPress);
			//m_View.TableViewWindow.Controls.Add(m_CellEditControl);

		}


		#endregion

		#region Serialization
		/// <summary>Used to serialize the TableController Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes the TableController (version 0).
			/// </summary>
			/// <param name="obj">The TableController to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				TableController s = (TableController)obj;

				info.AddValue("DataTable",s.m_Table);
				info.AddValue("DefColumnStyles",s.m_DefaultColumnStyles);
				info.AddValue("ColumnStyles",s.m_ColumnStyles);
				info.AddValue("RowHeaderStyle",s.m_RowHeaderStyle);
				info.AddValue("ColumnHeaderStyle",s.m_ColumnHeaderStyle);
				info.AddValue("PropertyColumnHeaderStyle",s.m_PropertyColumnHeaderStyle);
			}
			/// <summary>
			/// Deserializes the TableController (version 0).
			/// </summary>
			/// <param name="obj">The empty TableController object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized TableController.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				TableController s = (TableController)obj;
				s.SetMemberVariablesToDefault();

				s.m_Table = (Altaxo.Data.DataTable)info.GetValue("DataTable",typeof(Altaxo.Data.DataTable));
				s.m_DefaultColumnStyles= (System.Collections.Hashtable)info.GetValue("DefColumnStyles",typeof(System.Collections.Hashtable));
				s.m_ColumnStyles = (System.Collections.Hashtable)info.GetValue("ColumnStyles",typeof(System.Collections.Hashtable));
				s.m_RowHeaderStyle = (RowHeaderStyle)info.GetValue("RowHeaderStyle",typeof(RowHeaderStyle));
				s.m_ColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("ColumnHeaderStyle",typeof(ColumnHeaderStyle));
				s.m_PropertyColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("PropertyColumnHeaderStyle",typeof(ColumnHeaderStyle));


				s.m_DeserializationSurrogate = this;
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			if(null!=this.m_DeserializationSurrogate && obj is DeserializationFinisher)
			{
				m_DeserializationSurrogate=null;

				// first finish the document
				DeserializationFinisher finisher = new DeserializationFinisher(this);
				
				m_Table.OnDeserialization(finisher);


				// create the menu
				this.InitializeMenu();

				// set the menu of this class
				m_View.TableViewMenu = this.m_MainMenu;


				// restore the event chain to the Table
				this.DataTable = this.m_Table;
			}
		}
		#endregion

		#region Constructors



	
		/// <summary>
		/// Creates a TableController which shows the table data into the 
		/// View <paramref name="view"/>.
		/// </summary>
		/// <param name="view">The view to show the graph into.</param>
		/// <param name="table">The data table.</param>
		public TableController(ITableView view, Altaxo.Data.DataTable table)
		{
			SetMemberVariablesToDefault();
			m_View = view;
			m_View.TableController = this;
			m_View.TableViewWindow.Controls.Add(m_CellEditControl);

			if(null!=table)
				this.DataTable = table; // Using DataTable here wires the event chain also
			else
				throw new ArgumentNullException("Leaving the table null in constructor is not supported here");

			this.InitializeMenu();


			// Simulate a SizeChanged event 
			this.EhView_TableAreaSizeChanged(new EventArgs());

			// set the menu of this class
			m_View.TableViewMenu = this.m_MainMenu;
		}

		#endregion // Constructors


		#region Menu Definition


		/// <summary>
		/// Creates the default menu of a graph view.
		/// </summary>
		/// <remarks>In case there is already a menu here, the old menu is overwritten.</remarks>
		public void InitializeMenu()
		{
			int index=0, index2=0;
			MenuItem mi;

			m_MainMenu = new MainMenu();
			// ******************************************************************
			// ******************************************************************
			// File Menu
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("&File");
			mi.Index=0;
			mi.MergeOrder=0;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// File - Page Setup
			mi = new MenuItem("Page Setup..");
			mi.Click += new EventHandler(EhMenuFilePageSetup_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Print Preview
			mi = new MenuItem("Print Preview..");
			mi.Click += new EventHandler(EhMenuFilePrintPreview_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// File - Print 
			mi = new MenuItem("Print..");
			mi.Click += new EventHandler(EhMenuFilePrint_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// ------------------------------------------------------------------
			// File - Import (Popup)
			// ------------------------------------------------------------------
			mi = new MenuItem("Import");
			//mi.Popup += new EventHandler(MenuFileExport_OnPopup);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
			index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

			// File - Import - Ascii 
			mi = new MenuItem("Ascii...");
			mi.Click += new EventHandler(EhMenuFileImportAscii_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// File - Import - Picture 
			mi = new MenuItem("Picture as data...");
			mi.Click += new EventHandler(EhMenuFileImportPicture_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);


			// ------------------------------------------------------------------
			// File - Export (Popup)
			// ------------------------------------------------------------------
			mi = new MenuItem("Export");
			//mi.Popup += new EventHandler(MenuFileExport_OnPopup);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
			index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

			// File - Export - Ascii 
			mi = new MenuItem("Ascii...");
			mi.Click += new EventHandler(EhMenuFileExportAscii_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// File - Export - Galactic SPC 
			mi = new MenuItem("Galactic SPC...");
			mi.Click += new EventHandler(EhMenuFileExportGalacticSPC_OnClick);
			m_MainMenu.MenuItems[index].MenuItems[index2].MenuItems.Add(mi);

			// ******************************************************************
			// ******************************************************************
			// Edit (Popup)
			// ******************************************************************
			// ****************************************************************** 
			mi = new MenuItem("Edit");
			mi.Index=1;
			mi.MergeOrder=1;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			mi.Popup += new System.EventHandler(this.EhMenuEdit_OnPopup);
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Edit - Remove
			mi = new MenuItem("Remove");
			mi.Click += new EventHandler(EhMenuEditRemove_OnClick);
			m_MenuItemEditRemove = mi;
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Edit - Copy
			mi = new MenuItem("Copy");
			mi.Click += new EventHandler(EhMenuEditCopy_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Edit - Paste
			mi = new MenuItem("Paste");
			mi.Click += new EventHandler(EhMenuEditPaste_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);


			// ******************************************************************
			// ******************************************************************
			// Plot (Popup)
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("Plot");
			mi.Index=2;
			mi.MergeOrder=2;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Plot - Line&Scatter
			mi = new MenuItem("Line+Scatter");
			mi.Click += new EventHandler(EhMenuPlotLineAndScatter_OnClick);
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);


			// ******************************************************************
			// ******************************************************************
			// Worksheet (Popup)
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("Worksheet");
			mi.Index=3;
			mi.MergeOrder=3;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Worksheet - Duplicate Worksheet
			mi = new MenuItem("Duplicate Worksheet");
			mi.Click += new EventHandler(EhMenuWorksheetDuplicate_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Worksheet - Transpose
			mi = new MenuItem("Transpose");
			mi.Click += new EventHandler(EhMenuWorksheetTranspose_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Worksheet - AddColumn
			mi = new MenuItem("Add data columns...");
			mi.Click += new EventHandler(EhMenuWorksheetAddColumns_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Worksheet - AddPropertyColumns
			mi = new MenuItem("Add property columns...");
			mi.Click += new EventHandler(EhMenuWorksheetAddPropertyColumns_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
		
			// ******************************************************************
			// ******************************************************************
			// Column (Popup)
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("Column");
			mi.Index=4;
			mi.MergeOrder=4;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			mi.Popup += new System.EventHandler(this.EhMenuColumn_OnPopup);
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;
			
			// Column - SetColumnValues
			mi = new MenuItem("Set column values");
			mi.Click += new EventHandler(EhMenuColumnSetColumnValues_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MenuItemColumnSetColumnValues=mi;
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
	
			// Column - SetAsX
			mi = new MenuItem("Set column as X");
			mi.Click += new EventHandler(EhMenuColumnSetColumnAsX_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
	
			// Column - Extract Property values
			mi = new MenuItem("Extract property values");
			mi.Click += new EventHandler(EhMenuColumnExtractPropertyValues_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// ******************************************************************
			// ******************************************************************
			// Analysis (Popup)
			// ******************************************************************
			// ******************************************************************
			mi = new MenuItem("Analysis");
			mi.Index=5;
			mi.MergeOrder=5;
			mi.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			mi.Popup += new System.EventHandler(this.EhMenuAnalysis_OnPopup);
			m_MainMenu.MenuItems.Add(mi);
			index = m_MainMenu.MenuItems.Count-1;

			// Analysis - FFT
			mi = new MenuItem("FFT");
			mi.Click += new EventHandler(EhMenuAnalysisFFT_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Analysis - StatisticsOnColumns
			mi = new MenuItem("Statistics on columns");
			mi.Click += new EventHandler(EhMenuAnalysisStatisticsOnColumns_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Analysis - StatisticsOnRows
			mi = new MenuItem("Statistics on rows");
			mi.Click += new EventHandler(EhMenuAnalysisStatisticsOnRows_OnClick);
			//mi.Shortcut = ShortCuts.
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);
	
		}

		#endregion // Menu definition


		#region Menu Handler

		// ******************************************************************
		// ******************************************************************
		// File Menu
		// ******************************************************************
		// ******************************************************************
		protected void EhMenuFilePageSetup_OnClick(object sender, System.EventArgs e)
		{
		}

		protected void EhMenuFilePrintPreview_OnClick(object sender, System.EventArgs e)
		{
		}

		protected void EhMenuFilePrint_OnClick(object sender, System.EventArgs e)
		{
		}
		// ------------------------------------------------------------------
		// File - Import (Popup)
		// ------------------------------------------------------------------

		protected void EhMenuFileImportAscii_OnClick(object sender, System.EventArgs e)
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
					importer.ImportAscii(recognizedOptions,this.DataTable);
					myStream.Close();
				}
			}
		}

		protected void EhMenuFileImportPicture_OnClick(object sender, System.EventArgs e)
		{
			DataGridOperations.ImportPicture(this.DataTable);

		}

		// ------------------------------------------------------------------
		// File - Export (Popup)
		// ------------------------------------------------------------------

		protected void EhMenuFileExportAscii_OnClick(object sender, System.EventArgs e)
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
						Altaxo.Serialization.AsciiExporter.ExportAscii(myStream, this.DataTable,'\t');
					}
					catch(Exception ex)
					{
						System.Windows.Forms.MessageBox.Show(this.View.TableViewWindow,"There was an error during ascii export, details follow:\n" + ex.ToString());
					}
					finally
					{
						myStream.Close();
					}
				}
	
			}
		}

		protected void EhMenuFileExportGalacticSPC_OnClick(object sender, System.EventArgs e)
		{
			Altaxo.Serialization.Galactic.ExportGalacticSpcFileDialog dlg =
				new Altaxo.Serialization.Galactic.ExportGalacticSpcFileDialog();

			dlg.Initialize(this.DataTable,this.SelectedRows,this.SelectedColumns);

			dlg.ShowDialog(this.View.TableViewWindow);
		}

		// ******************************************************************
		// ******************************************************************
		// Edit (Popup)
		// ******************************************************************
		// ****************************************************************** 
		protected void EhMenuEdit_OnPopup(object sender, System.EventArgs e)
		{
			this.m_MenuItemEditRemove.Enabled = (this.SelectedColumns.Count>0 || this.SelectedRows.Count>0);
		}

		protected void EhMenuEditRemove_OnClick(object sender, System.EventArgs e)
		{
			this.RemoveSelected();

		}
		protected void EhMenuEditCopy_OnClick(object sender, System.EventArgs e)
		{			// Copy the selected Columns to the clipboard
			DataGridOperations.CopyToClipboard(this);

		}

		protected void EhMenuEditPaste_OnClick(object sender, System.EventArgs e)
		{
		}

		// ******************************************************************
		// ******************************************************************
		// Plot (Popup)
		// ******************************************************************
		// ******************************************************************
		protected void EhMenuPlotLineAndScatter_OnClick(object sender, System.EventArgs e)
		{
			DataGridOperations.PlotLine(this);
		}

		// ******************************************************************
		// ******************************************************************
		// Worksheet (Popup)
		// ******************************************************************
		// ******************************************************************
		protected void EhMenuWorksheetDuplicate_OnClick(object sender, System.EventArgs e)
		{
			Altaxo.Data.DataTable clonedTable = (Altaxo.Data.DataTable)this.DataTable.Clone();

			// find a new name for the cloned table and add it to the DataSet
			clonedTable.TableName = DataTable.ParentDataSet.FindNewTableName();
			DataTable.ParentDataSet.Add(clonedTable);
			App.Current.CreateNewWorksheet(clonedTable);
		}

		protected void EhMenuWorksheetTranspose_OnClick(object sender, System.EventArgs e)
		{
			string msg = this.DataTable.Transpose();

			if(null!=msg)
				System.Windows.Forms.MessageBox.Show(this.View.TableViewWindow,msg);
		}
		protected void EhMenuWorksheetAddColumns_OnClick(object sender, System.EventArgs e)
		{
			Altaxo.Data.DoubleColumn nc = new Altaxo.Data.DoubleColumn(this.DataTable.FindNewColumnName());
			this.DataTable.Add(nc);
			this.View.TableAreaInvalidate();

		}
		protected void EhMenuWorksheetAddPropertyColumns_OnClick(object sender, System.EventArgs e)
		{
			Altaxo.Data.TextColumn nc = new Altaxo.Data.TextColumn(this.DataTable.PropCols.FindNewColumnName());
			this.DataTable.PropCols.Add(nc);
			this.View.TableAreaInvalidate();

		}

		// ******************************************************************
		// ******************************************************************
		// Column (Popup)
		// ******************************************************************
		// ******************************************************************
		protected void EhMenuColumn_OnPopup(object sender, System.EventArgs e)
		{
			this.m_MenuItemColumnSetColumnValues.Enabled = 1==this.SelectedColumns.Count;
		}
		protected void EhMenuColumnSetColumnValues_OnClick(object sender, System.EventArgs e)
		{
			if(this.SelectedColumns.Count<=0)
				return; // no column selected

			Altaxo.Data.DataColumn dataCol = this.DataTable[this.SelectedColumns[0]];
			if(null==dataCol)
				return;

			//Data.ColumnScript colScript = (Data.ColumnScript)altaxoDataGrid1.columnScripts[dataCol];

			Data.ColumnScript colScript = this.DataTable.ColumnScripts[dataCol];

			SetColumnValuesDialog dlg = new SetColumnValuesDialog(this.DataTable,dataCol,colScript);
			DialogResult dres = dlg.ShowDialog(this.View.TableViewWindow);
			if(dres==DialogResult.OK)
			{
				if(colScript==null)	// store the column script in the hash table if not already there
				{
					//altaxoDataGrid1.columnScripts.Add(dataCol,dlg.columnScript);
					this.DataTable.ColumnScripts[dataCol]=dlg.columnScript;
				}
				else
				{
					//altaxoDataGrid1.columnScripts[dataCol] = (Data.ColumnScript)dlg.columnScript.Clone(); // if in the hash table already, simply copy the data
					this.DataTable.ColumnScripts[dataCol] = (Data.ColumnScript)dlg.columnScript.Clone(); // if in the hash table already, simply copy the data
				}
			}
			dlg.Dispose();
		}
		protected void EhMenuColumnSetColumnAsX_OnClick(object sender, System.EventArgs e)
		{
			this.SetSelectedColumnAsX();

		}
		protected void EhMenuColumnExtractPropertyValues_OnClick(object sender, System.EventArgs e)
		{			// extract the properties from the (first) selected property column
			if(this.SelectedPropertyColumns.Count==0)
				return;

			Altaxo.Data.DataColumn col = this.DataTable.PropCols[this.SelectedPropertyColumns[0]];

			DataGridOperations.ExtractPropertiesFromColumn(col,this.DataTable.PropCols);

		}
		// ******************************************************************
		// ******************************************************************
		// Analysis (Popup)
		// ******************************************************************
		// ******************************************************************
		protected void EhMenuAnalysis_OnPopup(object sender, System.EventArgs e)
		{
		}
		protected void EhMenuAnalysisFFT_OnClick(object sender, System.EventArgs e)
		{
			DataGridOperations.FFT(this);

		}
		protected void EhMenuAnalysisStatisticsOnColumns_OnClick(object sender, System.EventArgs e)
		{
			DataGridOperations.StatisticsOnColumns(App.Current.Doc,this.Doc,this.SelectedColumns,SelectedRows);
		}

		protected void EhMenuAnalysisStatisticsOnRows_OnClick(object sender, System.EventArgs e)
		{
			DataGridOperations.StatisticsOnRows(App.Current.Doc,this.Doc,this.SelectedColumns,SelectedRows);
		}

		#endregion
	
		#region "public properties"

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Altaxo.Data.DataTable DataTable
		{
			get
			{
				return this.m_Table;
			}
			set
			{
				if(null!=m_Table)
				{
					m_Table.FireDataChanged -= new Altaxo.Data.DataTable.OnDataChanged(this.OnTableDataChanged);
					m_Table.PropCols.FireDataChanged -= new Altaxo.Data.DataTable.OnDataChanged(this.OnPropertyDataChanged);
				}

				m_Table = value;
				if(null!=m_Table)
				{
					m_Table.FireDataChanged += new Altaxo.Data.DataTable.OnDataChanged(this.OnTableDataChanged);
					m_Table.PropCols.FireDataChanged += new Altaxo.Data.DataTable.OnDataChanged(this.OnPropertyDataChanged);
					this.SetCachedNumberOfDataColumns();
					this.SetCachedNumberOfDataRows();
					this.SetCachedNumberOfPropertyColumns();

				}
				else // Data table is null
				{
					this.m_NumberOfTableCols = 0;
					this.m_NumberOfTableRows = 0;
					this.m_NumberOfPropertyCols = 0;
					m_ColumnStyleCache.Clear();
					SetScrollPositionTo(0,0);
					this.View.TableAreaInvalidate();
				}
			}	
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TableAreaWidth
		{
			get { return View.TableAreaSize.Width; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TableAreaHeight
		{
			get { return View.TableAreaSize.Height; }
		}

		public IndexSelection SelectedColumns
		{
			get	{	return m_SelectedColumns;	}
		}

		public IndexSelection SelectedRows
		{
			get { return m_SelectedRows; }
		}

		public IndexSelection SelectedPropertyColumns
		{
			get { return m_SelectedPropertyColumns; }
		}

		#endregion


		#region "public methods"


		public void RemoveSelected()
		{
			this.DataTable.SuspendDataChangedNotifications();


			// delete the selected columns
			if(this.m_SelectedColumns.Count>0)
			{

				int len = m_SelectedColumns.Count;
				int begin=-1;
				int end=-1; // note this _after_ the end of deleted columns
				int i;
				for(i=len-1;i>=0;i--)
				{
					int idx = m_SelectedColumns[i];
					if(begin<0)
					{
						begin=idx;
						end=idx+1;
					}
					else if(begin>=0 && idx==(begin-1))
					{
						begin=idx;
					}
					else
					{
						this.DataTable.RemoveColumns(begin,end-begin);
						begin=idx;
						end=idx+1;
					}
				} // end for
				// the last index must also be deleted, if not done already
				if(begin>=0 && end>=0)
					this.DataTable.RemoveColumns(begin,end-begin);


				this.m_SelectedColumns.Clear(); // now the columns are deleted, so they cannot be selected
			}


			// place here the code for selected rows
			if(this.m_SelectedRows.Count>0)
			{
				int begin=-1;
				int end=-1; // note this _after_ the end of deleted columns
				int i;
				for(i=m_SelectedRows.Count-1;i>=0;i--)
				{
					int idx = m_SelectedRows[i];
					if(begin<0)
					{
						begin=idx;
						end=idx+1;
					}
					else if(begin>=0 && idx==(begin-1))
					{
						begin=idx;
					}
					else
					{
						this.DataTable.DeleteRows(begin,end-begin);
						begin=idx;
						end=idx+1;
					}
				} // end for
				// the last index must also be deleted, if not done already
				if(begin>=0 && end>=0)
					this.DataTable.DeleteRows(begin,end-begin);


				this.m_SelectedRows.Clear(); // now the columns are deleted, so they cannot be selected
			}


			// end code for the selected rows
			this.DataTable.ResumeDataChangedNotifications();
			this.View.TableAreaInvalidate(); // necessary because we changed the selections



		}


		public void SetSelectedColumnAsX()
		{
			if(SelectedColumns.Count>0)
			{
				this.DataTable[SelectedColumns[0]].XColumn=true;
				SelectedColumns.Clear();
				this.View.TableAreaInvalidate(); // draw new because 
			}
		}

		public void SetSelectedColumnsGroup(int nGroup)
		{
			int len = SelectedColumns.Count;
			for(int i=0;i<len;i++)
			{
				DataTable[SelectedColumns[i]].Group = nGroup;
			}
			SelectedColumns.Clear();
			this.View.TableAreaInvalidate();
		}


		public Altaxo.Worksheet.ColumnStyle GetColumnStyle(int i)
		{
			// zuerst in der ColumnStylesCollection nach dem passenden Namen
			// suchen, ansonsten default-Style zurückgeben
			Altaxo.Data.DataColumn dc = DataTable[i];
			Altaxo.Worksheet.ColumnStyle colstyle;

			// first look at the column styles hash table, column itself is the key
			colstyle = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[dc];
			if(null!=colstyle)
				return colstyle;
			
			// second look to the defaultcolumnstyles hash table, key is the type of the column style

			System.Type searchstyletype = dc.GetColumnStyleType();
			if(null==searchstyletype)
			{
				throw new ApplicationException("Error: Column of type +" + dc.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
			}
			else
			{
				if(null!=(colstyle = (Altaxo.Worksheet.ColumnStyle)m_DefaultColumnStyles[searchstyletype]))
					return colstyle;

				// if not successfull yet, we will create a new defaultColumnStyle
				colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
				m_DefaultColumnStyles.Add(searchstyletype,colstyle);
				return colstyle;
			}
		}



		public Altaxo.Worksheet.ColumnStyle GetPropertyColumnStyle(int i)
		{
			// zuerst in der ColumnStylesCollection nach dem passenden Namen
			// suchen, ansonsten default-Style zurückgeben
			Altaxo.Data.DataColumn dc = DataTable.PropCols[i];
			Altaxo.Worksheet.ColumnStyle colstyle;

			// first look at the column styles hash table, column itself is the key
			colstyle = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[dc];
			if(null!=colstyle)
				return colstyle;
			
			// second look to the defaultcolumnstyles hash table, key is the type of the column style

			System.Type searchstyletype = dc.GetColumnStyleType();
			if(null==searchstyletype)
			{
				throw new ApplicationException("Error: Column of type +" + dc.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
			}
			else
			{
				if(null!=(colstyle = (Altaxo.Worksheet.ColumnStyle)m_DefaultColumnStyles[searchstyletype]))
					return colstyle;

				// if not successfull yet, we will create a new defaultColumnStyle
				colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
				m_DefaultColumnStyles.Add(searchstyletype,colstyle);
				return colstyle;
			}
		}

		#endregion

		#region Data event handlers
		public void OnTableDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow)
		{
			if(this.m_NumberOfTableRows!=DataTable.RowCount)
				this.SetCachedNumberOfDataRows();
			
			if(this.m_NumberOfTableCols!=DataTable.ColumnCount)
				this.SetCachedNumberOfDataColumns();
		}

	

		public void AdjustYScrollBarMaximum()
		{
			VertScrollMaximum = m_NumberOfTableRows>0 ? m_NumberOfTableRows-1 : 0;

			if(this.VertScrollPos>=m_NumberOfTableRows)
				VertScrollPos = m_NumberOfTableRows>0 ? m_NumberOfTableRows-1 : 0;

			this.View.TableAreaInvalidate();
		}

		public void AdjustXScrollBarMaximum()
		{

			this.HorzScrollMaximum = m_NumberOfTableCols>0 ? m_NumberOfTableCols-1 : 0;

			if(HorzScrollPos+1>m_NumberOfTableCols)
				HorzScrollPos = m_NumberOfTableCols>0 ? m_NumberOfTableCols-1 : 0;
	
			m_ColumnStyleCache.ForceUpdate(this);

			this.View.TableAreaInvalidate();
		}


		protected virtual void SetCachedNumberOfDataColumns()
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate
		if(DataTable.ColumnCount != this.m_NumberOfTableCols)
			{
				this.m_NumberOfTableCols = DataTable.ColumnCount;
				AdjustXScrollBarMaximum();
			}

		}


		protected virtual void SetCachedNumberOfDataRows()
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate
			int nOldDataRows = this.m_NumberOfTableRows;

			if(DataTable.RowCount != nOldDataRows)
			{
				this.m_NumberOfTableRows = DataTable.RowCount;
				AdjustYScrollBarMaximum();
			}

		}

		protected virtual void SetCachedNumberOfPropertyColumns()
		{
			// ask for table dimensions, compare with cached dimensions
			// and adjust the scroll bars appropriate
			int nOldPropCols = this.m_NumberOfPropertyCols;
			
			// if we was scrolled to the most upper position, we later scroll
			// to the most upper position again
			bool bUpperPosition = (nOldPropCols == -this.VertScrollPos);

			this.m_NumberOfPropertyCols=m_Table.PropCols.ColumnCount;

			if(nOldPropCols!=this.m_NumberOfPropertyCols)
			{
				// Adjust Y ScrollBar Maximum();
				AdjustYScrollBarMaximum();

				if(bUpperPosition) // we scroll again to the most upper position
				{
					this.VertScrollPos = -this.TotalEnabledPropertyColumns;
				}
			}
		}

		public void OnPropertyDataChanged(Altaxo.Data.DataColumnCollection sender, int nMinCol, int nMaxCol, int nMinRow, int nMaxRow)
		{
			if(this.m_NumberOfPropertyCols != sender.ColumnCount)
				SetCachedNumberOfPropertyColumns();
		}
		#endregion

		#region Edit box event handlers

		private void OnTextBoxLostControl(object sender, System.EventArgs e)
		{
			this.ReadCellEditContent();
			m_CellEditControl.Hide();
		}

		private void OnCellEditControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)13) // Don't use the enter key, event is handled by KeyDown
			{
				e.Handled=true;
			}
			else if(e.KeyChar == (char)9) // Tab key pressed
			{
				if(m_CellEditControl.SelectionStart+m_CellEditControl.SelectionLength>=m_CellEditControl.TextLength)
				{
					e.Handled=true;
					// Navigate to the right
					NavigateCellEdit(1,0);
				}
			}

		}

		private void OnCellEditControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyData==System.Windows.Forms.Keys.Left)
			{
				// Navigate to the left if the cursor is already left
				//if(m_CellEditControl.SelectionStart==0 && (m_CellEdit_EditedCell.Row>0 || m_CellEdit_EditedCell.Column>0) )
				if(m_CellEditControl.SelectionStart==0)
				{
					e.Handled=true;
					// Navigate to the left
					NavigateCellEdit(-1,0);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Right)
			{
				if(m_CellEditControl.SelectionStart+m_CellEditControl.SelectionLength>=m_CellEditControl.TextLength)
				{
					e.Handled=true;
					// Navigate to the right
					NavigateCellEdit(1,0);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Up)
			{
				e.Handled=true;
				// Navigate up
				NavigateCellEdit(0,-1);
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Down)
			{
				e.Handled=true;
				// Navigate down
				NavigateCellEdit(0,1);
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Enter)
			{
				// if some text is selected, deselect it and move the cursor to the end
				// else same action like keys.Down
				e.Handled=true;
				if(m_CellEditControl.SelectionLength>0)
				{
					m_CellEditControl.SelectionLength=0;
					m_CellEditControl.SelectionStart=m_CellEditControl.TextLength;
				}
				else
				{
					NavigateCellEdit(0,1);
				}
			}
			else if(e.KeyData==System.Windows.Forms.Keys.Escape)
			{
				e.Handled=true;
				m_CellEdit_IsArmed=false;
				this.m_CellEditControl.Hide();
			}
		}

		private void ReadCellEditContent()
		{
			if(this.m_CellEdit_IsArmed && this.m_CellEditControl.Modified)
			{
				if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
				{
					GetColumnStyle(m_CellEdit_EditedCell.Column).SetColumnValueAtRow(m_CellEditControl.Text,m_CellEdit_EditedCell.Row,DataTable[m_CellEdit_EditedCell.Column]);
				}
				else if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
				{
					GetPropertyColumnStyle(m_CellEdit_EditedCell.Column).SetColumnValueAtRow(m_CellEditControl.Text,m_CellEdit_EditedCell.Row,DataTable.PropCols[m_CellEdit_EditedCell.Column]);
				}
				
				this.m_CellEdit_IsArmed=false;
			}
		}

		private void SetCellEditContent()
		{
			
			if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
			{
				m_CellEditControl.Text = GetColumnStyle(m_CellEdit_EditedCell.Column).GetColumnValueAtRow(m_CellEdit_EditedCell.Row,DataTable[m_CellEdit_EditedCell.Column]);
			}
			else if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
			{
				m_CellEditControl.Text = this.GetPropertyColumnStyle(m_CellEdit_EditedCell.Column).GetColumnValueAtRow(m_CellEdit_EditedCell.Row,DataTable.PropCols[m_CellEdit_EditedCell.Column]);
			}

			m_CellEditControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			m_CellEditControl.SelectAll();
			m_CellEditControl.Modified=false;
			m_CellEditControl.BringToFront();
			m_CellEditControl.Show();
			m_CellEditControl.Focus();
			this.m_CellEdit_IsArmed=true;
		}


		/// <summary>
		/// NavigateCellEdit moves the cell edit control to the next cell
		/// </summary>
		/// <param name="dx">move dx cells to the right</param>
		/// <param name="dy">move dy cells down</param>
		/// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
		protected bool NavigateCellEdit(int dx, int dy)
		{
			if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
			{
				return NavigateTableCellEdit(dx,dy);
			}
			else if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
			{
				return NavigatePropertyCellEdit(dx,dy);
			}
			return false;
		}

		/// <summary>
		/// NavigateTableCellEdit moves the cell edit control to the next cell
		/// </summary>
		/// <param name="dx">move dx cells to the right</param>
		/// <param name="dy">move dy cells down</param>
		/// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
		protected bool NavigateTableCellEdit(int dx, int dy)
		{
			bool bScrolled = false;

			// Calculate the position of the new cell		
			int newCellCol = this.m_CellEdit_EditedCell.Column + dx;
			if(newCellCol>=DataTable.ColumnCount)
			{
				newCellCol=0;
				dy+=1;
			}
			else if(newCellCol<0)
			{
				if(this.m_CellEdit_EditedCell.Row>0) // move to the last cell only if not on cell 0
				{
					newCellCol=DataTable.ColumnCount-1;
					dy-=1;
				}
				else
				{
					newCellCol=0;
				}
			}

			int newCellRow = m_CellEdit_EditedCell.Row + dy;
			if(newCellRow<0)
				newCellRow=0;
			// note: we do not catch the condition newCellRow>rowCount here since we want to add new rows
	
		
			// look if the cell position has changed
			if(newCellRow==m_CellEdit_EditedCell.Row && newCellCol==m_CellEdit_EditedCell.Column)
				return false; // moving was not possible, so returning false, and do nothing

			// if the cell position has changed, read the old cell content
			// 1. Read content of the cell edit, if neccessary write data back
			ReadCellEditContent();		

			int navigateToCol;
			int navigateToRow;

			if(newCellCol<FirstVisibleColumn)
				navigateToCol = newCellCol;
			else if(newCellCol>LastFullyVisibleColumn)
				navigateToCol = GetFirstVisibleColumnForLastVisibleColumn(newCellCol);
			else
				navigateToCol = FirstVisibleColumn;

			if(newCellRow<FirstVisibleTableRow)
				navigateToRow = newCellRow;
			else if (newCellRow>LastFullyVisibleTableRow)
				navigateToRow = newCellRow + 1 - FullyVisibleTableRows;
			else
				navigateToRow = FirstVisibleTableRow;

			if(navigateToCol!=FirstVisibleColumn || navigateToRow!=FirstVisibleTableRow)
			{
				SetScrollPositionTo(navigateToCol,navigateToRow);
				bScrolled=true;
			}
			// 3. Fill the cell edit control with new content
			m_CellEdit_EditedCell.Column=newCellCol;
			m_CellEdit_EditedCell.Row=newCellRow;
			m_CellEditControl.Parent = View.TableViewWindow;
			Rectangle cellRect = this.GetCoordinatesOfDataCell(m_CellEdit_EditedCell.Column,m_CellEdit_EditedCell.Row);
			m_CellEditControl.Location = cellRect.Location;
			m_CellEditControl.Size = cellRect.Size;
			SetCellEditContent();

			// 4. Invalidate the client area if scrolled in step (2)
			if(bScrolled)
				this.View.TableAreaInvalidate();

			return true;
		}


		/// <summary>
		/// NavigatePropertyCellEdit moves the cell edit control to the next cell
		/// </summary>
		/// <param name="dx">move dx cells to the right</param>
		/// <param name="dy">move dy cells down</param>
		/// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
		protected bool NavigatePropertyCellEdit(int dx, int dy)
		{
			bool bScrolled = false;

		
			// 2. look whether the new cell coordinates lie inside the client area, if
			// not scroll the worksheet appropriate
			int newCellCol = this.m_CellEdit_EditedCell.Column + dy;
			if(newCellCol>=DataTable.PropCols.ColumnCount)
			{
				if(m_CellEdit_EditedCell.Row+1<DataTable.ColumnCount)
				{
					newCellCol=0;
					dx+=1;
				}
				else
				{
					newCellCol=DataTable.PropCols.ColumnCount-1;
					dx=0;
				}
			}
			else if(newCellCol<0)
			{
				if(this.m_CellEdit_EditedCell.Row>0) // move to the last cell only if not on cell 0
				{
					newCellCol=DataTable.PropCols.ColumnCount-1;
					dx-=1;
				}
				else
				{
					newCellCol=0;
				}
			}

			int newCellRow = m_CellEdit_EditedCell.Row + dx;
			if(newCellRow>=DataTable.ColumnCount)
			{
				if(newCellCol+1<DataTable.PropCols.ColumnCount) // move to the first cell only if not on the very last cell
				{
					newCellRow=0;
					newCellCol+=1;
				}
				else // we where on the last cell
				{
					newCellRow=DataTable.ColumnCount-1;
					newCellCol=DataTable.PropCols.ColumnCount-1;
				}
			}
			else if(newCellRow<0)
			{
				if(this.m_CellEdit_EditedCell.Column>0) // move to the last cell only if not on cell 0
				{
					newCellRow=DataTable.ColumnCount-1;
					newCellCol-=1;
				}
				else
				{
					newCellRow=0;
				}
			}

			// Fix if newCellCol is outside valid area
			if(newCellCol<0)
				newCellCol=0;
			else if(newCellCol>=DataTable.PropCols.ColumnCount)
				newCellCol=DataTable.PropCols.ColumnCount-1;
			
			// look if the cell position has changed
			if(newCellRow==m_CellEdit_EditedCell.Row && newCellCol==m_CellEdit_EditedCell.Column)
				return false; // moving was not possible, so returning false, and do nothing

			// 1. Read content of the cell edit, if neccessary write data back
			ReadCellEditContent();		
	


			int navigateToCol;
			int navigateToRow;


			if(newCellCol<FirstVisiblePropertyColumn)
				navigateToCol = newCellCol;
			else if (newCellCol>LastFullyVisiblePropertyColumn)
				navigateToCol = newCellRow + 1 - this.FullyVisiblePropertyColumns;
			else
				navigateToCol = FirstVisibleTableRow;


			if(newCellRow<FirstVisibleColumn)
				navigateToRow = newCellRow;
			else if (newCellRow>LastFullyVisibleColumn)
				navigateToRow = GetFirstVisibleColumnForLastVisibleColumn(newCellRow);
			else
				navigateToRow = FirstVisibleColumn;

			if(navigateToRow!=FirstVisibleColumn || navigateToCol!=FirstVisibleTableRow)
			{
				SetScrollPositionTo(navigateToRow,navigateToCol);
				bScrolled=true;
			}
			// 3. Fill the cell edit control with new content
			m_CellEdit_EditedCell.Column=newCellCol;
			m_CellEdit_EditedCell.Row=newCellRow;
			m_CellEditControl.Parent = View.TableViewWindow;
			Rectangle cellRect = this.GetCoordinatesOfPropertyCell(m_CellEdit_EditedCell.Column,m_CellEdit_EditedCell.Row);
			m_CellEditControl.Location = cellRect.Location;
			m_CellEditControl.Size = cellRect.Size;
			SetCellEditContent();

			// 4. Invalidate the client area if scrolled in step (2)
			if(bScrolled)
				this.View.TableAreaInvalidate();

			return true;
		}



		#endregion

		#region Row positions (vertical scroll logic)

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VertScrollPos
		{
			get { return m_VertScrollPos; }
			set
			{
				int oldValue = m_VertScrollPos;
				m_VertScrollPos=value;

				if(value!=oldValue)
				{
					if(m_CellEditControl.Visible)
					{
						this.ReadCellEditContent();
						m_CellEditControl.Hide();
					}

					// The value of the ScrollBar in the view has an offset, since he
					// can not have negative values;
					this.View.TableViewVertScrollValue = value + this.TotalEnabledPropertyColumns;
					this.View.TableAreaInvalidate();
				}
			}
		}

		public int VertScrollMaximum
		{
			get { return this.m_VertScrollMax; }
			set 
			{
				this.m_VertScrollMax = value;
				View.TableViewVertScrollMaximum = value + this.TotalEnabledPropertyColumns;
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisibleTableRow
		{
			get
			{
				return Math.Max(0,VertScrollPos);
			}
			set
			{
				VertScrollPos = Math.Max(0,value);
			}
		}


		/// <summary>
		/// This returns the vertical position of the first visible data row.;
		/// </summary>
		public int VerticalPositionOfFirstVisibleDataRow
		{
			get 
			{
				return this.m_ColumnHeaderStyle.Height + (VertScrollPos>=0 ? 0 : -VertScrollPos*this.m_PropertyColumnHeaderStyle.Height); 
			}
		}
		/// <summary>
		/// Gets the first table row that is visible under the coordinate top.
		/// </summary>
		/// <param name="top">The upper coordinate of the cliping rectangle.</param>
		/// <returns>The first table row that is visible below the top coordinate.</returns>
		public int GetFirstVisibleTableRow(int top)
		{
			int posOfDataRow0 = this.VerticalPositionOfFirstVisibleDataRow;

			//int firstTotRow = (int)Math.Max(RemainingEnabledPropertyColumns,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_RowHeaderStyle.Height));
			//return FirstVisibleTableRow + Math.Max(0,firstTotRow-RemainingEnabledPropertyColumns);
		int firstVis = (int)Math.Floor((top-posOfDataRow0)/(double)m_RowHeaderStyle.Height);
		return (firstVis<0? 0 : firstVis ) + FirstVisibleTableRow;
		}

		/// <summary>
		/// How many data rows are visible between top and bottom (in pixel)?
		/// </summary>
		/// <param name="top">The top y coordinate.</param>
		/// <param name="bottom">The bottom y coordinate.</param>
		/// <returns>The number of data rows visible between these two coordinates.</returns>
		public int GetVisibleTableRows(int top, int bottom)
		{
			int posOfDataRow0 = this.VerticalPositionOfFirstVisibleDataRow;

			if(top<posOfDataRow0)
				top = posOfDataRow0;

			int firstRow = (int)Math.Floor((top-posOfDataRow0)/(double)m_RowHeaderStyle.Height);
			int lastRow  = (int)Math.Ceiling((bottom-posOfDataRow0)/(double)m_RowHeaderStyle.Height)-1;
			return Math.Max(0,1 + lastRow - firstRow);
		}

		public int GetFullyVisibleTableRows(int top, int bottom)
		{
			int posOfDataRow0 = this.VerticalPositionOfFirstVisibleDataRow;

			if(top<posOfDataRow0)
				top = posOfDataRow0;

			int firstRow = (int)Math.Floor((top-posOfDataRow0)/(double)m_RowHeaderStyle.Height);
			int lastRow  = (int)Math.Floor((bottom-posOfDataRow0)/(double)m_RowHeaderStyle.Height)-1;
			return Math.Max(0, 1+ lastRow - firstRow);
		}

		public int GetTopCoordinateOfTableRow(int nRow)
		{
			return	this.VerticalPositionOfFirstVisibleDataRow + (nRow- (VertScrollPos<0?0:VertScrollPos)) * m_RowHeaderStyle.Height;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleTableRows
		{
			get
			{
				return GetVisibleTableRows(0,this.TableAreaHeight);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisibleTableRows
		{
			get
			{
				return GetFullyVisibleTableRows(0,this.View.TableAreaSize.Height);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastVisibleTableRow
		{
			get
			{
				return FirstVisibleTableRow + VisibleTableRows -1;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisibleTableRow
		{
			get
			{
				return FirstVisibleTableRow + FullyVisibleTableRows - 1;
			}
		}

		/// <summary>Returns the remaining number of property columns that could be shown below the current scroll position.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RemainingEnabledPropertyColumns
		{
			get
			{
				return m_ShowPropertyColumns ? Math.Max(0,-VertScrollPos) : 0;
			}
		}

		/// <summary>Returns number of property columns that are enabled for been shown on the grid.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TotalEnabledPropertyColumns
		{
			get { return m_ShowPropertyColumns ? this.m_NumberOfPropertyCols : 0; }
		}



		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisiblePropertyColumn
		{
			get
			{
				return (m_ShowPropertyColumns && VertScrollPos<0) ? TotalEnabledPropertyColumns+VertScrollPos : -1;
			}
		}


		public int GetFirstVisiblePropertyColumn(int top)
		{
			int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_PropertyColumnHeaderStyle.Height));
			return m_ShowPropertyColumns ? firstTotRow+FirstVisiblePropertyColumn : 0;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisiblePropertyColumn
		{
			get
			{
				return FirstVisiblePropertyColumn + this.FullyVisiblePropertyColumns -1;
			}
		}


		public int GetTopCoordinateOfPropertyColumn(int nCol)
		{
			return m_ColumnHeaderStyle.Height + (nCol-FirstVisiblePropertyColumn)*m_PropertyColumnHeaderStyle.Height;
		}

		public int GetVisiblePropertyColumns(int top, int bottom)
		{
			if(this.m_ShowPropertyColumns)
			{
				int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_PropertyColumnHeaderStyle.Height));
				int lastTotRow  = (int)Math.Ceiling((bottom-m_ColumnHeaderStyle.Height)/(double)m_PropertyColumnHeaderStyle.Height)-1;
				int maxPossRows = Math.Max(0,RemainingEnabledPropertyColumns-firstTotRow);
				return Math.Min(maxPossRows,Math.Max(0,1 + lastTotRow - firstTotRow));
			}
			else
				return 0;
		}

		public int GetFullyVisiblePropertyColumns(int top, int bottom)
		{
			if(m_ShowPropertyColumns)
			{
				int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_ColumnHeaderStyle.Height)/(double)m_PropertyColumnHeaderStyle.Height));
				int lastTotRow  = (int)Math.Floor((bottom-m_ColumnHeaderStyle.Height)/(double)m_PropertyColumnHeaderStyle.Height)-1;
				int maxPossRows = Math.Max(0,RemainingEnabledPropertyColumns-firstTotRow);
				return Math.Min(maxPossRows,Math.Max(0,1 + lastTotRow - firstTotRow));
			}
			else
				return 0;
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisiblePropertyColumns
		{
			get
			{
				return GetVisiblePropertyColumns(0,this.TableAreaHeight);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisiblePropertyColumns
		{
			get
			{
				return GetFullyVisiblePropertyColumns(0,this.TableAreaHeight);
			}
		}

		

		#endregion

		#region Column positions (horizontal scroll logic)


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HorzScrollPos
		{
			get { return m_HorzScrollPos; }
			set
			{
				int oldValue = m_HorzScrollPos;
				m_HorzScrollPos=value;

				if(value!=oldValue)
				{

					if(m_CellEditControl.Visible)
					{
						this.ReadCellEditContent();
						m_CellEditControl.Hide();
					}

					this.View.TableViewHorzScrollValue = value;
					this.m_ColumnStyleCache.ForceUpdate(this);
					this.View.TableAreaInvalidate();
				}
			}
		}

		public int HorzScrollMaximum
		{
			get { return this.m_HorzScrollMax; }
			set 
			{
				this.m_HorzScrollMax = value;
				View.TableViewHorzScrollMaximum = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FirstVisibleColumn
		{
			get
			{
				return HorzScrollPos;
			}
			set
			{
				HorzScrollPos=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleColumns
		{
			get
			{
				return this.m_LastVisibleColumn>=FirstVisibleColumn ? 1+m_LastVisibleColumn-FirstVisibleColumn : 0;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FullyVisibleColumns
		{
			get
			{
				return m_LastFullyVisibleColumn>=FirstVisibleColumn ? 1+m_LastFullyVisibleColumn-FirstVisibleColumn : 0;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastVisibleColumn
		{
			get
			{
				return FirstVisibleColumn + VisibleColumns -1;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LastFullyVisibleColumn
		{
			get
			{
				return FirstVisibleColumn + FullyVisibleColumns -1;
			}
		}


		private int GetFirstAndNumberOfVisibleColumn(int left, int right, out int numVisibleColumns)
		{
			int nFirstCol = -1;
			int nLastCol = m_NumberOfTableCols;
			ColumnStyleCacheItem csci;
			
			for(int nCol=FirstVisibleColumn,i=0 ; i<m_ColumnStyleCache.Count ; nCol++,i++)
			{
				csci = ((ColumnStyleCacheItem)m_ColumnStyleCache[i]);
				if(csci.rightBorderPosition>left && nFirstCol<0)
					nFirstCol = nCol;
			
				if(csci.leftBorderPosition>=right)
				{
					nLastCol = nCol;
					break;
				}
			}

			numVisibleColumns = nFirstCol<0 ? 0 :  Math.Max(0,nLastCol-nFirstCol);
			return nFirstCol;
		}



		private Rectangle GetXCoordinatesOfColumn(int nCol, Rectangle cellRect)
		{
			int colOffs = nCol-FirstVisibleColumn;
			cellRect.X = ((ColumnStyleCacheItem)m_ColumnStyleCache[colOffs]).leftBorderPosition;
			cellRect.Width = ((ColumnStyleCacheItem)m_ColumnStyleCache[colOffs]).rightBorderPosition - cellRect.X;
			return cellRect;
		}

		private Rectangle GetXCoordinatesOfColumn(int nCol)
		{
			return GetXCoordinatesOfColumn(nCol,new Rectangle());
		}


		private Rectangle GetCoordinatesOfDataCell(int nCol, int nRow)
		{
			Rectangle cellRect = GetXCoordinatesOfColumn(nCol);

			cellRect.Y = this.GetTopCoordinateOfTableRow(nRow);
			cellRect.Height = this.m_RowHeaderStyle.Height;
			return cellRect;
		}
	
		private Rectangle GetCoordinatesOfPropertyCell(int nCol, int nRow)
		{
			Rectangle cellRect = GetXCoordinatesOfColumn(nRow);

			cellRect.Y = this.GetTopCoordinateOfPropertyColumn(nCol);
			cellRect.Height = this.m_RowHeaderStyle.Height;
			return cellRect;
		}

		/// <summary>
		/// retrieves, to which column should be scrolled in order to make
		/// the column nForLastCol the last visible column
		/// </summary>
		/// <param name="nForLastCol">the column number which should be the last visible column</param>
		/// <returns>the number of the first visible column</returns>
		public int GetFirstVisibleColumnForLastVisibleColumn(int nForLastCol)
		{
			
			int i = nForLastCol;
			int retv = nForLastCol;
			int horzSize = this.TableAreaWidth-m_RowHeaderStyle.Width;
			while(i>=0)
			{
				horzSize -= GetColumnStyle(i).Width;
				if(horzSize>0 && i>0)
					i--;
				else
					break;
			}

			if(horzSize<0)
				i++; // increase one colum if size was bigger than available size

			return i<=nForLastCol ? i : nForLastCol;
		}

		/// <summary>
		/// SetScrollPositions only sets the scroll positions, and not Invalidates the 
		/// Area!
		/// </summary>
		/// <param name="nCol">first visible column (i.e. column at the left)</param>
		/// <param name="nRow">first visible row (i.e. row at the top)</param>
		protected void SetScrollPositionTo(int nCol, int nRow)
		{
			int oldCol = HorzScrollPos;
			if(this.HorzScrollMaximum<nCol)
				this.HorzScrollMaximum = nCol;
			this.HorzScrollPos=nCol;

			m_ColumnStyleCache.Update(this);

			if(this.VertScrollMaximum<nRow)
				this.VertScrollMaximum=nRow;
			this.VertScrollPos=nRow;
		}


		#endregion

		#region ITableController Members

		public Altaxo.Data.DataTable Doc
		{
			get
			{
				return this.m_Table;
			}
		}

		public ITableView View
		{
			get
			{
				return m_View;
			}
			set
			{
				if(value==null)
					throw new ArgumentNullException("The view for this TableController must not be null!");

				ITableView oldView = m_View;
				m_View = value;

				if(null!=oldView)
				{
					oldView.TableViewMenu = null; // don't let the old view have the menu
					oldView.TableController = null; // no longer the controller of this view
					oldView.TableViewWindow.Controls.Remove(m_CellEditControl);
				}

				m_View.TableController = this;
				m_View.TableViewMenu = m_MainMenu;
				m_View.TableViewWindow.Controls.Add(m_CellEditControl);

			
				// Werte für gerade vorliegende Scrollpositionen und Scrollmaxima zum (neuen) View senden
			
				this.VertScrollMaximum = this.m_VertScrollMax;
				this.HorzScrollMaximum = this.m_HorzScrollMax;

				this.VertScrollPos     = this.m_VertScrollPos;
				this.HorzScrollPos     = this.m_HorzScrollPos;


			}
		}

		public void EhView_VertScrollBarScroll(System.Windows.Forms.ScrollEventArgs e)
		{
			VertScrollPos = e.NewValue - this.TotalEnabledPropertyColumns;
		}

		public void EhView_HorzScrollBarScroll(System.Windows.Forms.ScrollEventArgs e)
		{
			HorzScrollPos = e.NewValue;
		}

		public void EhView_TableAreaMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			if(this.m_DragColumnWidth_InCapture)
			{
				int sizediff = e.X - this.m_DragColumnWidth_OriginalPos;
				Altaxo.Worksheet.ColumnStyle cs;
				if(-1==m_DragColumnWidth_ColumnNumber)
				{
					cs = this.m_RowHeaderStyle;
				}
				else
				{
					cs = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[DataTable[m_DragColumnWidth_ColumnNumber]];
					if(null==cs)
					{
						Altaxo.Worksheet.ColumnStyle template = GetColumnStyle(this.m_DragColumnWidth_ColumnNumber);
						cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
						m_ColumnStyles.Add(DataTable[m_DragColumnWidth_ColumnNumber],cs);
					}
				}
				int newWidth = this.m_DragColumnWidth_OriginalWidth + sizediff;
				if(newWidth<10)
					newWidth=10;
				cs.Width=newWidth;
				this.m_ColumnStyleCache.ForceUpdate(this);

				this.m_DragColumnWidth_InCapture = false;
				this.m_DragColumnWidth_ColumnNumber = int.MinValue;
				this.View.TableAreaCapture=false;
				this.View.TableAreaCursor = System.Windows.Forms.Cursors.Default;
				this.View.TableAreaInvalidate();

			}
		}

		public void EhView_TableAreaMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			// base.OnMouseDown(e);
			this.m_MouseDownPosition = new Point(e.X, e.Y);
			this.ReadCellEditContent();
			m_CellEditControl.Hide();

			if(this.m_DragColumnWidth_ColumnNumber>=-1)
			{
				this.View.TableAreaCapture=true;
				m_DragColumnWidth_OriginalPos = e.X;
				m_DragColumnWidth_InCapture=true;
			}
		}

		public void EhView_TableAreaMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			int Y = e.Y;
			int X = e.X;

			if(this.m_DragColumnWidth_InCapture)
			{
				int sizediff = X - this.m_DragColumnWidth_OriginalPos;
				
				Altaxo.Worksheet.ColumnStyle cs;
				if(-1==m_DragColumnWidth_ColumnNumber)
					cs = this.m_RowHeaderStyle;
				else
				{
					cs = (Altaxo.Worksheet.ColumnStyle)m_ColumnStyles[DataTable[m_DragColumnWidth_ColumnNumber]];
				
					if(null==cs)
					{
						Altaxo.Worksheet.ColumnStyle template = GetColumnStyle(this.m_DragColumnWidth_ColumnNumber);
						cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
						m_ColumnStyles.Add(DataTable[m_DragColumnWidth_ColumnNumber],cs);
					}
				}

				int newWidth = this.m_DragColumnWidth_OriginalWidth + sizediff;
				if(newWidth<10)
					newWidth=10;
				cs.Width=newWidth;
				this.m_ColumnStyleCache.ForceUpdate(this);
				this.View.TableAreaInvalidate();
			}
			else // not in Capture mode
			{
				if(Y<this.m_ColumnHeaderStyle.Height)
				{
					for(int i=this.m_ColumnStyleCache.Count-1;i>=0;i--)
					{
						ColumnStyleCacheItem csc = (ColumnStyleCacheItem)m_ColumnStyleCache[i];

						if(csc.rightBorderPosition-5 < X && X < csc.rightBorderPosition+5)
						{
							this.View.TableAreaCursor = System.Windows.Forms.Cursors.VSplit;
							this.m_DragColumnWidth_ColumnNumber = i+FirstVisibleColumn;
							this.m_DragColumnWidth_OriginalWidth = csc.columnStyle.Width;
							return;
						}
					} // end for

					if(this.m_RowHeaderStyle.Width -5 < X && X < m_RowHeaderStyle.Width+5)
					{
						this.View.TableAreaCursor = System.Windows.Forms.Cursors.VSplit;
						this.m_DragColumnWidth_ColumnNumber = -1;
						this.m_DragColumnWidth_OriginalWidth = this.m_RowHeaderStyle.Width;
						return;
					}
				}

				this.m_DragColumnWidth_ColumnNumber=int.MinValue;
				this.View.TableAreaCursor = System.Windows.Forms.Cursors.Default;
			} // end else
		}

		public void EhView_TableAreaMouseClick(EventArgs e)
		{
			ClickedCellInfo clickedCell = new ClickedCellInfo(this,this.m_MouseDownPosition);

			switch(clickedCell.ClickedArea)
			{
				case ClickedAreaType.DataCell:
				{
					//m_CellEditControl = new TextBox();
					m_CellEdit_EditedCell=clickedCell;
					m_CellEditControl.Parent = View.TableViewWindow;
					m_CellEditControl.Location = clickedCell.CellRectangle.Location;
					m_CellEditControl.Size = clickedCell.CellRectangle.Size;
					m_CellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
					this.SetCellEditContent();
				}
					break;
				case ClickedAreaType.PropertyCell:
				{
					m_CellEdit_EditedCell=clickedCell;
					m_CellEditControl.Parent = View.TableViewWindow;
					m_CellEditControl.Location = clickedCell.CellRectangle.Location;
					m_CellEditControl.Size = clickedCell.CellRectangle.Size;
					m_CellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
					this.SetCellEditContent();
				}
					break;
				case ClickedAreaType.PropertyColumnHeader:
				{
					bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
					bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
					if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
						m_SelectedRows.Clear(); // if we click a column, we remove row selections
					if(m_LastSelectionType==SelectionType.DataColumnSelection && !bControlKey)
						m_SelectedColumns.Clear(); // if we click a column, we remove row selections
					m_SelectedPropertyColumns.Select(clickedCell.Column,bShiftKey,bControlKey);
					m_LastSelectionType = SelectionType.PropertyColumnSelection;
					this.View.TableAreaInvalidate();
				}
					break;
				case ClickedAreaType.DataColumnHeader:
				{
					if(!this.m_DragColumnWidth_InCapture)
					{
						bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
						bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
						if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
							m_SelectedRows.Clear(); // if we click a column, we remove row selections
						m_SelectedColumns.Select(clickedCell.Column,bShiftKey,bControlKey);
						m_LastSelectionType = SelectionType.DataColumnSelection;
						this.View.TableAreaInvalidate();
					}
				}
					break;
				case ClickedAreaType.DataRowHeader:
				{
					bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
					bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
					if(m_LastSelectionType==SelectionType.DataColumnSelection && !bControlKey)
						m_SelectedColumns.Clear(); // if we click a column, we remove row selections
					m_SelectedRows.Select(clickedCell.Row,bShiftKey,bControlKey);
					m_LastSelectionType = SelectionType.DataRowSelection;
					this.View.TableAreaInvalidate();
				}
					break;
			}
		}

		public void EhView_TableAreaMouseDoubleClick(EventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaMouseDoubleClick implementation
		}

		public void EhView_TableAreaPaint(System.Windows.Forms.PaintEventArgs e)
		{
			Graphics dc=e.Graphics;
			Pen bluePen = new Pen(Color.Blue, 1);
			Brush brownBrush = new SolidBrush(Color.Aquamarine);

			bool bDrawColumnHeader = false;

			int firstTableRowToDraw     = this.GetFirstVisibleTableRow(e.ClipRectangle.Top);
			int numberOfTableRowsToDraw = this.GetVisibleTableRows(e.ClipRectangle.Top,e.ClipRectangle.Bottom);

			int firstPropertyColumnToDraw = this.GetFirstVisiblePropertyColumn(e.ClipRectangle.Top);
			int numberOfPropertyColumnsToDraw = this.GetVisiblePropertyColumns(e.ClipRectangle.Top,e.ClipRectangle.Bottom);

			bool bAreColumnsSelected = m_SelectedColumns.Count>0;
			bool bAreRowsSelected =    m_SelectedRows.Count>0;
			bool bAreCellsSelected =  bAreRowsSelected || bAreColumnsSelected;
			bool bArePropColsSelected = m_SelectedPropertyColumns.Count>0;


			int yShift=0;



			dc.FillRectangle(brownBrush,e.ClipRectangle); // first set the background
			
			if(null==DataTable)
				return;

			Rectangle cellRectangle = new Rectangle();


			if(e.ClipRectangle.Top<m_ColumnHeaderStyle.Height)
			{
				bDrawColumnHeader = true;
			}

			// if neccessary, draw the row header (the most left column)
			if(e.ClipRectangle.Left<m_RowHeaderStyle.Width)
			{
				cellRectangle.Height = m_RowHeaderStyle.Height;
				cellRectangle.Width = m_RowHeaderStyle.Width;
				cellRectangle.X=0;
				

				// if visible, draw property column header items
				yShift=this.GetTopCoordinateOfPropertyColumn(firstPropertyColumnToDraw);
				cellRectangle.Height = m_PropertyColumnHeaderStyle.Height;
				for(int nPropCol=firstPropertyColumnToDraw, nInc=0;nInc<numberOfPropertyColumnsToDraw;nPropCol++,nInc++)
				{
					cellRectangle.Y = yShift+nInc*m_PropertyColumnHeaderStyle.Height;
					bool bPropColSelected = bArePropColsSelected && m_SelectedPropertyColumns.ContainsKey(nPropCol);
					this.m_PropertyColumnHeaderStyle.Paint(dc,cellRectangle,nPropCol,this.DataTable.PropCols[nPropCol],bPropColSelected);
				}
			}

			// draw the table row Header Items
			yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
			cellRectangle.Height = m_RowHeaderStyle.Height;
			for(int nRow = firstTableRowToDraw,nInc=0; nInc<numberOfTableRowsToDraw; nRow++,nInc++)
			{
				cellRectangle.Y = yShift+nInc*m_RowHeaderStyle.Height;
				m_RowHeaderStyle.Paint(dc,cellRectangle,nRow,null, bAreRowsSelected && m_SelectedRows.ContainsKey(nRow));
			}
			

			if(e.ClipRectangle.Bottom>=m_ColumnHeaderStyle.Height || e.ClipRectangle.Right>=m_RowHeaderStyle.Width)		
			{
				int numberOfColumnsToDraw;
				int firstColToDraw =this.GetFirstAndNumberOfVisibleColumn(e.ClipRectangle.Left,e.ClipRectangle.Right, out numberOfColumnsToDraw);

				// draw the property columns
				for(int nPropCol=firstPropertyColumnToDraw, nIncPropCol=0; nIncPropCol<numberOfPropertyColumnsToDraw; nPropCol++, nIncPropCol++)
				{
					Altaxo.Worksheet.ColumnStyle cs = GetPropertyColumnStyle(nPropCol);
					bool bPropColSelected = bArePropColsSelected && m_SelectedPropertyColumns.ContainsKey(nPropCol);
					cellRectangle.Y=this.GetTopCoordinateOfPropertyColumn(nPropCol);
					cellRectangle.Height = m_PropertyColumnHeaderStyle.Height;
					
					for(int nCol=firstColToDraw, nIncCol=0; nIncCol<numberOfColumnsToDraw; nCol++,nIncCol++)
					{
						cellRectangle = this.GetXCoordinatesOfColumn(nCol,cellRectangle);
						cs.Paint(dc,cellRectangle,nCol,DataTable.PropCols[nPropCol],bPropColSelected);
					}
				}


				// draw the cells
				//int firstColToDraw = firstVisibleColumn+(e.ClipRectangle.Left-m_RowHeaderStyle.Width)/columnWidth;
				//int lastColToDraw  = firstVisibleColumn+(int)Math.Ceiling((e.ClipRectangle.Right-m_RowHeaderStyle.Width)/columnWidth);

				for(int nCol=firstColToDraw, nIncCol=0; nIncCol<numberOfColumnsToDraw; nCol++,nIncCol++)
				{
					Altaxo.Worksheet.ColumnStyle cs = GetColumnStyle(nCol);
					cellRectangle = this.GetXCoordinatesOfColumn(nCol,cellRectangle);

					bool bColumnSelected = bAreColumnsSelected && m_SelectedColumns.ContainsKey(nCol);
					bool bDataColumnIncluded = bAreColumnsSelected  ? bColumnSelected : true;


					if(bDrawColumnHeader) // must the column Header been drawn?
					{
						cellRectangle.Height = m_ColumnHeaderStyle.Height;
						cellRectangle.Y=0;
						m_ColumnHeaderStyle.Paint(dc,cellRectangle,0,DataTable[nCol],bColumnSelected);
					}

	
					yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
					cellRectangle.Height = m_RowHeaderStyle.Height;
					for(int nRow=firstTableRowToDraw, nIncRow=0;nIncRow<numberOfTableRowsToDraw;nRow++,nIncRow++)
					{
						bool bRowSelected = bAreRowsSelected && m_SelectedRows.ContainsKey(nRow);
						bool bDataRowIncluded = bAreRowsSelected ? bRowSelected : true;
						cellRectangle.Y= yShift+nIncRow*m_RowHeaderStyle.Height;
						cs.Paint(dc,cellRectangle,nRow,DataTable[nCol],bAreCellsSelected && bDataColumnIncluded && bDataRowIncluded);
					}
				}
			}		
		}

		public void EhView_TableAreaSizeChanged(EventArgs e)
		{
			m_ColumnStyleCache.Update(this);
		}

		public void EhView_Closed(EventArgs e)
		{
			// if the view is closed, we delete the corresponding table
			DataTable.ParentDataSet.Remove(DataTable);
			DataTable.Dispose();
		}

		public void EhView_Closing(System.ComponentModel.CancelEventArgs e)
		{
			System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.View.TableViewForm,"Do you really want to close this worksheet and delete the corresponding table?","Attention",System.Windows.Forms.MessageBoxButtons.YesNo);

			if(dlgres==System.Windows.Forms.DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		#endregion

		#region Column style cache

		public class ColumnStyleCacheItem
		{
			public Altaxo.Worksheet.ColumnStyle columnStyle;
			public int leftBorderPosition;
			public int rightBorderPosition;


			public ColumnStyleCacheItem(Altaxo.Worksheet.ColumnStyle cs, int leftBorderPosition, int rightBorderPosition)
			{
				this.columnStyle = cs;
				this.leftBorderPosition = leftBorderPosition;
				this.rightBorderPosition = rightBorderPosition;
			}

		}


		public class ColumnStyleCache : Altaxo.Data.CollectionBase
		{
			protected int m_CachedFirstVisibleColumn=0; // the column number of the first cached item, i.e. for this[0]
			protected int m_CachedWidth=0; // cached width of painting area
 
			public ColumnStyleCacheItem this[int i]
			{
				get { return (ColumnStyleCacheItem)base.InnerList[i]; }
			}

			public void Add(ColumnStyleCacheItem item)
			{
				base.InnerList.Add(item);
			}

	

			public void Update(TableController dg)
			{
				if(	(this.Count==0)
					||(dg.TableAreaWidth!=this.m_CachedWidth)
					||(dg.FirstVisibleColumn != this.m_CachedFirstVisibleColumn) )
				{
					ForceUpdate(dg);
				}
			}

			public void ForceUpdate(TableController dg)
			{
				dg.m_LastVisibleColumn=0;
				dg.m_LastFullyVisibleColumn = 0;

				this.Clear(); // clear all items

				if(null==dg.DataTable)
					return;
		
				int actualColumnLeft = 0; 
				int actualColumnRight = dg.m_RowHeaderStyle.Width;
			
				this.m_CachedWidth = dg.TableAreaWidth;
				dg.m_LastFullyVisibleColumn = dg.FirstVisibleColumn;

				for(int i=dg.FirstVisibleColumn;i<dg.DataTable.ColumnCount && actualColumnLeft<this.m_CachedWidth;i++)
				{
					actualColumnLeft = actualColumnRight;
					Altaxo.Worksheet.ColumnStyle cs = dg.GetColumnStyle(i);
					actualColumnRight = actualColumnLeft+cs.Width;
					this.Add(new ColumnStyleCacheItem(cs,actualColumnLeft,actualColumnRight));

					if(actualColumnLeft<this.m_CachedWidth)
						dg.m_LastVisibleColumn = i;

					if(actualColumnRight<=this.m_CachedWidth)
						dg.m_LastFullyVisibleColumn = i;
				}
			}
		}

		#endregion

		#region Class ClickedCellInfo



		/// <summary>The type of area we have clicked into, used by ClickedCellInfo.</summary>
		public enum ClickedAreaType 
		{ 
			/// <summary>Outside of all relevant areas.</summary>
			OutsideAll,
			/// <summary>On the table header (top left corner of the data grid).</summary>
			TableHeader,
			/// <summary>Inside a data cell.</summary>
			DataCell,
			/// <summary>Inside a property cell.</summary>
			PropertyCell,
			/// <summary>On the column header.</summary>
			DataColumnHeader,
			/// <summary>On the row header.</summary>
			DataRowHeader,
			/// <summary>On the property column header.</summary>
			PropertyColumnHeader
		}


		/// <remarks>
		/// ClickedCellInfo retrieves (from mouse coordinates of a click), which cell has clicked onto. 
		/// </remarks>
		public struct ClickedCellInfo
		{

			/// <summary>The enclosing Rectangle of the clicked cell</summary>
			private Rectangle m_CellRectangle;

			/// <summary>The data row clicked onto.</summary>
			private int m_Row;
			/// <summary>The data column number clicked onto.</summary>
			private int m_Column;

			/// <summary>What have been clicked onto.</summary>
			private ClickedAreaType m_ClickedArea;


			/// <value>The enclosing Rectangle of the clicked cell</value>
			public Rectangle CellRectangle { get { return m_CellRectangle; }}
			/// <value>The row number clicked onto.</value>
			public int Row 
			{
				get { return m_Row; }
				set { m_Row = value; }
			}
			/// <value>The column number clicked onto.</value>
			public int Column 
			{
				get { return m_Column; }
				set { m_Column = value; }
			}
			/// <value>The type of area clicked onto.</value>
			public ClickedAreaType ClickedArea { get { return m_ClickedArea; }}
 
			/// <summary>
			/// Retrieves the column number clicked onto 
			/// </summary>
			/// <param name="dg">The parent data grid</param>
			/// <param name="mouseCoord">The coordinates of the mouse click.</param>
			/// <param name="cellRect">The function sets the x-properties (X and Width) of the cell rectangle.</param>
			/// <returns>Either -1 when clicked on the row header area, column number when clicked in the column range, or int.MinValue when clicked outside of all.</returns>
			public static int GetColumnNumber(TableController dg, Point mouseCoord, ref Rectangle cellRect)
			{
				int firstVisibleColumn = dg.FirstVisibleColumn;
				int actualColumnRight = dg.m_RowHeaderStyle.Width;
				int columnCount = dg.DataTable.ColumnCount;

				if(mouseCoord.X<actualColumnRight)
				{
					cellRect.X=0; cellRect.Width=actualColumnRight;
					return -1;
				}

				for(int i=firstVisibleColumn;i<columnCount;i++)
				{
					cellRect.X=actualColumnRight;
					Altaxo.Worksheet.ColumnStyle cs = dg.GetColumnStyle(i);
					actualColumnRight += cs.Width;
					if(actualColumnRight>mouseCoord.X)
					{
						cellRect.Width = cs.Width;
						return i;
					}
				} // end for
				return int.MinValue;
			}

			/// <summary>
			/// Returns the row number of the clicked cell.
			/// </summary>
			/// <param name="dg">The parent TableController.</param>
			/// <param name="mouseCoord">The mouse coordinates of the click.</param>
			/// <param name="cellRect">Returns the bounding rectangle of the clicked cell.</param>
			/// <param name="bPropertyCol">True if clicked on either the property column header or a property column, else false.</param>
			/// <returns>The row number of the clicked cell, or -1 if clicked on the column header.</returns>
			/// <remarks>If clicked onto a property cell, the function returns the property column number.</remarks>
			public static int GetRowNumber(TableController dg, Point mouseCoord, ref Rectangle cellRect, out bool bPropertyCol)
			{
				int firstVisibleColumn = dg.FirstVisibleColumn;
				int actualColumnRight = dg.m_RowHeaderStyle.Width;
				int columnCount = dg.DataTable.ColumnCount;

				if(mouseCoord.Y<dg.m_ColumnHeaderStyle.Height)
				{
					cellRect.Y=0; cellRect.Height=dg.m_ColumnHeaderStyle.Height;
					bPropertyCol=false;
					return -1;
				}

				if(mouseCoord.Y<dg.VerticalPositionOfFirstVisibleDataRow && dg.VisiblePropertyColumns>0)
				{
					// calculate the raw row number
					int rawrow = (int)Math.Floor((mouseCoord.Y-dg.m_ColumnHeaderStyle.Height)/(double)dg.m_PropertyColumnHeaderStyle.Height);

					cellRect.Y= dg.m_ColumnHeaderStyle.Height + rawrow * dg.m_PropertyColumnHeaderStyle.Height;
					cellRect.Height = dg.m_PropertyColumnHeaderStyle.Height;

					bPropertyCol=true;
					return dg.FirstVisiblePropertyColumn+rawrow;
				}
				else
				{
					int rawrow = (int)Math.Floor((mouseCoord.Y-dg.VerticalPositionOfFirstVisibleDataRow)/(double)dg.m_RowHeaderStyle.Height);

					cellRect.Y= dg.VerticalPositionOfFirstVisibleDataRow + rawrow * dg.m_RowHeaderStyle.Height;
					cellRect.Height = dg.m_RowHeaderStyle.Height;
					bPropertyCol=false;
					return dg.FirstVisibleTableRow + rawrow;
				}
			}




			/// <summary>
			/// Creates the ClickedCellInfo from the data grid and the mouse coordinates of the click.
			/// </summary>
			/// <param name="dg">The data grid.</param>
			/// <param name="mouseCoord">The mouse coordinates of the click.</param>
			public ClickedCellInfo(TableController dg, Point mouseCoord)
			{

				bool bIsPropertyColumn=false;
				m_CellRectangle = new Rectangle(0,0,0,0);
				m_Column = GetColumnNumber(dg,mouseCoord, ref m_CellRectangle);
				m_Row    = GetRowNumber(dg,mouseCoord,ref m_CellRectangle, out bIsPropertyColumn);

				if(bIsPropertyColumn)
				{
					if(m_Column==-1)
						m_ClickedArea = ClickedAreaType.PropertyColumnHeader;
					else if(m_Column>=0)
						m_ClickedArea = ClickedAreaType.PropertyCell;
					else
						m_ClickedArea = ClickedAreaType.OutsideAll;

					int h=m_Column; m_Column = m_Row; m_Row = h; // Swap columns and rows since it is a property column
				}
				else // it is not a property related cell
				{
					if(m_Row==-1 && m_Column==-1)
						m_ClickedArea = ClickedAreaType.TableHeader;
					else if(m_Row==-1 && m_Column>=0)
						m_ClickedArea = ClickedAreaType.DataColumnHeader;
					else if(m_Row>=0 && m_Column==-1)
						m_ClickedArea = ClickedAreaType.DataRowHeader;
					else if(m_Row>=0 && m_Column>=0)
						m_ClickedArea = ClickedAreaType.DataCell;
					else
						m_ClickedArea = ClickedAreaType.OutsideAll;
				}
			}
		} // end of class ClickedCellInfo

		#endregion Class ClickedCellInfo
	}
}
