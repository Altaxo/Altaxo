using System;

namespace Altaxo.Worksheet
{
	/// <summary>
	/// Summary description for TableController.
	/// </summary>
	public class TableController : ITableController
	{
		#region Member variables
		
		/// <summary>Holds the data table.</summary>
		protected Altaxo.Data.DataTable m_Table;

		/// <summary>Holds the view (the window where the graph is visualized).</summary>
		protected ITableView m_View;
		
		/// <summary>The main menu of this controller.</summary>
		protected System.Windows.Forms.MainMenu m_MainMenu; 


		#endregion


		#region Constructors


		/// <summary>
		/// Set the member variables to default values. Intended only for use in constructors and deserialization code.
		/// </summary>
		protected virtual void SetMemberVariablesToDefault()
		{

		}


		/// <summary>
		/// Creates a TableController for control of the View <paramref	name="view"/>. 
		/// </summary>
		/// <param name="view">The view this controller has to control.</param>
		public TableController(ITableView view)
			: this(view, null)
	{
	}

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
			m_View.Controller = this;

			if(null!=table)
				this.m_Table = table;
			else
				throw new ArgumentNullException("Leaving the table null in constructor is not supported here");

			this.InitializeMenu();


			// Calculate the zoom if Autozoom is on - simulate a SizeChanged event of the view to force calculation of new zoom factor
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
			m_MainMenu.MenuItems[index].MenuItems.Add(mi);

			// Edit - Copy
			mi = new MenuItem("Cooy");
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

	
		}


		/// <summary>
		/// Updates a special menu item, the data item, with the currently available plot names. The active plot is marked with a
		/// check.
		/// </summary>
		public void UpdateDataPopup()
		{
			if(null==this.m_MenuDataPopup)
				return; // as long there is no menu, we cannot do it

			// first delete old menuitems
			this.m_MenuDataPopup.MenuItems.Clear();


			// check there is at least one layer
			if(m_Graph.Layers.Count==0)
				return; // there is no layer, we can not have items in the data menu

			// now it is save to get the active layer
			int actLayerNum = this.CurrentLayerNumber;
			Layer actLayer = this.Layers[actLayerNum];

			// then append the plot associations of the actual layer

			int actPA = CurrentPlotNumber;
			int len = actLayer.PlotItems.Count;
			for(int i = 0; i<len; i++)
			{
				PlotItem pa = actLayer.PlotItems[i];
				DataMenuItem mi = new DataMenuItem(pa.ToString(), new EventHandler(EhMenuData_Data));
				mi.Checked = (i==actPA);
				mi.PlotItemNumber = i;
				this.m_MenuDataPopup.MenuItems.Add(mi);
			}
		}

		#endregion // Menu definition

		#region ITableController Members

		public Altaxo.Data.DataTable Doc
		{
			get
			{
				// TODO:  Add TableController.Doc getter implementation
				return null;
			}
		}

		public ITableView View
		{
			get
			{
				// TODO:  Add TableController.View getter implementation
				return null;
			}
			set
			{
				// TODO:  Add TableController.View setter implementation
			}
		}

		public void EhView_VertScrollBarScroll(System.Windows.Forms.ScrollEventArgs e)
		{
			// TODO:  Add TableController.EhView_VertScrollBarScroll implementation
		}

		public void EhView_HorzScrollBarScroll(System.Windows.Forms.ScrollEventArgs e)
		{
			// TODO:  Add TableController.EhView_HorzScrollBarScroll implementation
		}

		public void EhView_TableAreaMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaMouseUp implementation
		}

		public void EhView_TableAreaMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaMouseDown implementation
		}

		public void EhView_TableAreaMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaMouseMove implementation
		}

		public void EhView_TableAreaMouseClick(EventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaMouseClick implementation
		}

		public void EhView_TableAreaMouseDoubleClick(EventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaMouseDoubleClick implementation
		}

		public void EhView_TableAreaPaint(System.Windows.Forms.PaintEventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaPaint implementation
		}

		public void EhView_TableAreaSizeChanged(EventArgs e)
		{
			// TODO:  Add TableController.EhView_TableAreaSizeChanged implementation
		}

		public void EhView_Closed(EventArgs e)
		{
			// TODO:  Add TableController.EhView_Closed implementation
		}

		public void EhView_Closing(System.ComponentModel.CancelEventArgs e)
		{
			// TODO:  Add TableController.EhView_Closing implementation
		}

		#endregion
	}
}
