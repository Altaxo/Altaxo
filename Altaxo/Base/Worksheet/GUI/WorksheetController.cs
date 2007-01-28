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
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph.Gdi;
using Altaxo.Data;
using Altaxo.Serialization;
using Altaxo.Serialization.Ascii;
using Altaxo.Collections;
//using ICSharpCode.SharpDevelop.Gui;


namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Default controller which implements IWorksheetController.
  /// </summary>
  [SerializationSurrogate(0,typeof(WorksheetController.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Altaxo.Gui.UserControllerForObject(typeof(Altaxo.Worksheet.WorksheetLayout))]
  [Altaxo.Gui.ExpectedTypeOfView(typeof(IWorksheetView))]
  public class WorksheetController :
    IWorksheetController,
    System.Runtime.Serialization.IDeserializationCallback
  {
    public enum SelectionType { Nothing, DataRowSelection, DataColumnSelection, PropertyColumnSelection, PropertyRowSelection }


    #region Member variables
    
    /// <summary>
    /// Used to indicate that deserialization has not finished, and holds some deserialized values.
    /// </summary>
    private object m_DeserializationSurrogate;

    /// <summary>Holds the data table cached from the layout.</summary>
    protected Altaxo.Data.DataTable m_Table;


    protected Altaxo.Worksheet.WorksheetLayout m_TableLayout;

    /// <summary>Holds the view (the window where the graph is visualized).</summary>
    protected IWorksheetView m_View;
    

    /// <summary>Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
    protected SelectionType m_LastSelectionType;

    
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
    protected IndexSelection m_SelectedDataColumns; // holds the selected columns
    
    /// <summary>
    /// Holds the indizes to the selected rows.
    /// </summary>
    protected IndexSelection m_SelectedDataRows; // holds the selected rows
    
    /// <summary>
    /// Holds the indizes to the selected property columns.
    /// </summary>
    protected IndexSelection m_SelectedPropertyColumns; // holds the selected property columns


    /// <summary>
    /// Holds the indizes to the selected property rows (but only in case property cells are selected alone).
    /// </summary>
    protected IndexSelection m_SelectedPropertyRows; // holds the selected property rows


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
    
  
    private ClickedCellInfo m_MouseInfo = new ClickedCellInfo();

    private Point m_MouseDownPosition; // holds the position of a double click
    private int  m_DragColumnWidth_ColumnNumber; // stores the column number if mouse hovers over separator
    private int  m_DragColumnWidth_OriginalPos;
    private int  m_DragColumnWidth_OriginalWidth;
    private bool m_DragColumnWidth_InCapture;
  

    protected bool                         m_CellEdit_IsArmed;
    private ClickedCellInfo              m_CellEdit_EditedCell;
    protected System.Windows.Forms.TextBox m_CellEditControl; 


    /// <summary>
    /// Set the member variables to default values. Intended only for use in constructors and deserialization code.
    /// </summary>
    protected virtual void SetMemberVariablesToDefault()
    {
      m_DeserializationSurrogate=null;

      m_Table=null;
      m_TableLayout=null;
      m_View = null;
    
      // The main menu of this controller.
      m_MainMenu = null; 
      m_MenuItemEditRemove = null;
      m_MenuItemColumnSetColumnValues = null;

      // Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
      m_LastSelectionType = SelectionType.Nothing;

    
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
      m_SelectedDataColumns = new Altaxo.Worksheet.IndexSelection(); // holds the selected columns
    
      // Holds the indizes to the selected rows.
      m_SelectedDataRows    = new Altaxo.Worksheet.IndexSelection(); // holds the selected rows
    
      // Holds the indizes to the selected property columns.
      m_SelectedPropertyColumns = new Altaxo.Worksheet.IndexSelection(); // holds the selected property columns

      // Holds the indizes to the selected property columns.
      m_SelectedPropertyRows = new Altaxo.Worksheet.IndexSelection(); // holds the selected property columns

      // Cached number of table rows.
      m_NumberOfTableRows=0; // cached number of rows of the table

      // Cached number of table columns.
      m_NumberOfTableCols=0;
    
      // Cached number of property columns.
      m_NumberOfPropertyCols=0; // cached number of property  columnsof the table
    
        

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
      m_CellEdit_IsArmed = false;
      m_CellEditControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnCellEditControl_KeyDown);
      m_CellEditControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnCellEditControl_KeyPress);
      //m_View.TableViewWindow.Controls.Add(m_CellEditControl);

    }


    #endregion

    #region Serialization
    /// <summary>Used to serialize the WorksheetController Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes the WorksheetController (version 0).
      /// </summary>
      /// <param name="obj">The WorksheetController to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        WorksheetController s = (WorksheetController)obj;

        info.AddValue("DataTable",s.m_Table);
        info.AddValue("WorksheetLayout",s.m_TableLayout);
        //info.AddValue("DefColumnStyles",s.m_TableLayout.DefaultColumnStyles);
        //info.AddValue("ColumnStyles",s.m_TableLayout.ColumnStyles);
        //info.AddValue("RowHeaderStyle",s.m_TableLayout.RowHeaderStyle);
        //info.AddValue("ColumnHeaderStyle",s.m_TableLayout.ColumnHeaderStyle);
        //info.AddValue("PropertyColumnHeaderStyle",s.m_TableLayout.PropertyColumnHeaderStyle);
      }
      /// <summary>
      /// Deserializes the WorksheetController (version 0).
      /// </summary>
      /// <param name="obj">The empty WorksheetController object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized WorksheetController.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        WorksheetController s = (WorksheetController)obj;
        s.SetMemberVariablesToDefault();

        s.m_Table = (Altaxo.Data.DataTable)info.GetValue("DataTable",typeof(Altaxo.Data.DataTable));
        s.m_TableLayout = (Altaxo.Worksheet.WorksheetLayout)info.GetValue("WorksheetLayout",typeof(Altaxo.Worksheet.WorksheetLayout)); 
        //s.m_TableLayout.DefaultColumnStyles= (System.Collections.Hashtable)info.GetValue("DefColumnStyles",typeof(System.Collections.Hashtable));
        //s.m_TableLayout.ColumnStyles = (System.Collections.Hashtable)info.GetValue("ColumnStyles",typeof(System.Collections.Hashtable));
        //s.m_TableLayout.RowHeaderStyle = (RowHeaderStyle)info.GetValue("RowHeaderStyle",typeof(RowHeaderStyle));
        //s.m_TableLayout.ColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("ColumnHeaderStyle",typeof(ColumnHeaderStyle));
        //s.m_TableLayout.PropertyColumnHeaderStyle = (ColumnHeaderStyle)info.GetValue("PropertyColumnHeaderStyle",typeof(ColumnHeaderStyle));


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
        m_View.TableViewTitle = this.m_Table.Name;


        // restore the event chain to the Table
        //this.DataTable = this.m_Table;

      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetController),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      Main.DocumentPath _PathToLayout;
      WorksheetController   _TableController;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        WorksheetController s = (WorksheetController)obj;
        info.AddValue("Layout",Main.DocumentPath.GetAbsolutePath(s.m_TableLayout));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        WorksheetController s = null!=o ? (WorksheetController)o : new WorksheetController(null,true);
        
        XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
        surr._TableController = s;
        surr._PathToLayout = (Main.DocumentPath)info.GetValue("Layout",s);
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
        
        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {

        if(null!=_PathToLayout)
        {
          object o = Main.DocumentPath.GetObject(_PathToLayout,documentRoot,_TableController);
          if(o is Altaxo.Worksheet.WorksheetLayout)
          {
            _TableController.WorksheetLayout = o as Altaxo.Worksheet.WorksheetLayout;
            _PathToLayout=null;
          }
        }
        
        if(null==_PathToLayout)
        {
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
        }
      }
    }

    #endregion

    #region Constructors


    public WorksheetController(Altaxo.Worksheet.WorksheetLayout layout)
      : this(layout, false)
    {
    }
  
    /// <summary>
    /// Creates a WorksheetController which shows the table data into the 
    /// View <paramref name="view"/>.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="bDeserializationConstructor">If true, no layout has to be provided, since this is used as deserialization constructor.</param>
    public WorksheetController(Altaxo.Worksheet.WorksheetLayout layout, bool bDeserializationConstructor)
    {
      SetMemberVariablesToDefault();

      if(null!=layout)
        this.WorksheetLayout = layout; // Using DataTable here wires the event chain also
      else if(!bDeserializationConstructor)
        throw new ArgumentNullException("Leaving the layout null in constructor is not supported here");

      this.InitializeMenu();
    }

    #endregion // Constructors

    #region Menu member variables
    /// <summary>The main menu of this controller.</summary>
    protected System.Windows.Forms.MainMenu m_MainMenu;
    protected System.Windows.Forms.MenuItem m_MenuItemEditRemove;
    protected System.Windows.Forms.MenuItem m_MenuItemColumnRename;
    protected System.Windows.Forms.MenuItem m_MenuItemColumnSetGroupNumber;
    protected System.Windows.Forms.MenuItem m_MenuItemColumnSetColumnValues;



    #endregion

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


      // ------------------------------------------------------------------
      // File - New (Popup)
      // ------------------------------------------------------------------
      mi = new MenuItem("New");
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);
      index2 = m_MainMenu.MenuItems[index].MenuItems.Count-1;

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

      // File - Save Table As
      mi = new MenuItem("Save Table As..");
      mi.Click += new EventHandler(EhMenuFileSaveTableAs_OnClick);
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

      // File - Import - Galactic SPC 
      mi = new MenuItem("Galactic SPC...");
      mi.Click += new EventHandler(EhMenuFileImportGalacticSPC_OnClick);
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
      mi = new MenuItem("Line");
      mi.Click += new EventHandler(EhMenuPlotLine_OnClick);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Plot - Line&Scatter
      mi = new MenuItem("Scatter");
      mi.Click += new EventHandler(EhMenuPlotScatter_OnClick);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Plot - Line&Scatter
      mi = new MenuItem("Line+Scatter");
      mi.Click += new EventHandler(EhMenuPlotLineAndScatter_OnClick);
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Plot - Density Image
      mi = new MenuItem("Density Image");
      mi.Click += new EventHandler(EhMenuPlotDensityImage_OnClick);
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

      // Worksheet - Rename Worksheet
      mi = new MenuItem("Rename Worksheet..");
      mi.Click += new EventHandler(EhMenuWorksheetRename_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

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
      
      // Column - Rename column
      mi = new MenuItem("Rename column..");
      mi.Click += new EventHandler(EhMenuColumnRename_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MenuItemColumnRename=mi;
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Column - Set group number
      mi = new MenuItem("Set group number..");
      mi.Click += new EventHandler(EhMenuColumnSetGroupNumber_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MenuItemColumnSetGroupNumber=mi;
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

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


      // Analysis - 2 Dimensional FFT
      mi = new MenuItem("2-dimensional FFT");
      mi.Click += new EventHandler(EhMenuAnalysis2DFFT_OnClick);
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
  
      // Analysis - Multiply Columns to Matrix
      mi = new MenuItem("Multiply columns to matrix");
      mi.Click += new EventHandler(EhMenuAnalysisMultiplyColumnsToMatrix_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);


      // Analysis -PCAOnRows
      mi = new MenuItem("PCA on rows");
      mi.Click += new EventHandler(EhMenuAnalysisPCAOnRows_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);
      
      // Analysis -PCAOnCols
      mi = new MenuItem("PCA on columns");
      mi.Click += new EventHandler(EhMenuAnalysisPCAOnCols_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

      // Analysis -PLSOnRows
      mi = new MenuItem("PLS on rows");
      mi.Click += new EventHandler(EhMenuAnalysisPLSOnRows_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);
      
      // Analysis -PLSOnCols
      mi = new MenuItem("PLS on columns");
      mi.Click += new EventHandler(EhMenuAnalysisPLSOnCols_OnClick);
      //mi.Shortcut = ShortCuts.
      m_MainMenu.MenuItems[index].MenuItems.Add(mi);

    }

    #endregion // Menu definition

    #region Menu functions


  
    #endregion

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

    protected void EhMenuFileSaveTableAs_OnClick(object sender, System.EventArgs e)
    {
      Commands.FileCommands.SaveAs(this,false);
    }

    // ------------------------------------------------------------------
    // File - Import (Popup)
    // ------------------------------------------------------------------

    protected void EhMenuFileImportAscii_OnClick(object sender, System.EventArgs e)
    {
      Commands.FileCommands.ImportAscii(this);
    }

    protected void EhMenuFileImportPicture_OnClick(object sender, System.EventArgs e)
    {
      Commands.FileCommands.ImportImage(this);

    }


    protected void EhMenuFileImportGalacticSPC_OnClick(object sender, System.EventArgs e)
    {
      Altaxo.Serialization.Galactic.Import.ShowDialog(this.View.TableViewForm, this.DataTable);
    }

    // ------------------------------------------------------------------
    // File - Export (Popup)
    // ------------------------------------------------------------------

    protected void EhMenuFileExportAscii_OnClick(object sender, System.EventArgs e)
    {
      Commands.FileCommands.ExportAscii(this);
    }

    protected void EhMenuFileExportGalacticSPC_OnClick(object sender, System.EventArgs e)
    {
      Commands.FileCommands.ExportGalacticSPC(this);
    }

    // ******************************************************************
    // ******************************************************************
    // Edit (Popup)
    // ******************************************************************
    // ****************************************************************** 
    protected void EhMenuEdit_OnPopup(object sender, System.EventArgs e)
    {
      this.m_MenuItemEditRemove.Enabled = (this.SelectedDataColumns.Count>0 || this.SelectedDataRows.Count>0);
    }

    protected void EhMenuEditRemove_OnClick(object sender, System.EventArgs e)
    {
      Commands.EditCommands.RemoveSelected(this);
    }

    protected void EhMenuEditCopy_OnClick(object sender, System.EventArgs e)
    {     // Copy the selected Columns to the clipboard
      Commands.EditCommands.CopyToClipboard(this);
    }

    protected void EhMenuEditPaste_OnClick(object sender, System.EventArgs e)
    {
      Commands.EditCommands.PasteFromClipboard(this);
    }

    // ******************************************************************
    // ******************************************************************
    // Plot (Popup)
    // ******************************************************************
    // ******************************************************************
    protected void EhMenuPlotLine_OnClick(object sender, System.EventArgs e)
    {
      Commands.PlotCommands.PlotLine(this, true, false);
    }

    protected void EhMenuPlotScatter_OnClick(object sender, System.EventArgs e)
    {
      Commands.PlotCommands.PlotLine(this, false, true);
    }

    protected void EhMenuPlotLineAndScatter_OnClick(object sender, System.EventArgs e)
    {
      Commands.PlotCommands.PlotLine(this, true, true);
    }

    protected void EhMenuPlotDensityImage_OnClick(object sender, System.EventArgs e)
    {
      Commands.PlotCommands.PlotDensityImage(this, true, true);
    }


    // ******************************************************************
    // ******************************************************************
    // Worksheet (Popup)
    // ******************************************************************
    // ******************************************************************

    protected void EhMenuWorksheetRename_OnClick(object sender, System.EventArgs e)
    {
      Commands.WorksheetCommands.Rename(this);
    }


    protected void EhMenuWorksheetDuplicate_OnClick(object sender, System.EventArgs e)
    {
      Commands.WorksheetCommands.Duplicate(this);
    }

    protected void EhMenuWorksheetTranspose_OnClick(object sender, System.EventArgs e)
    {
      Commands.WorksheetCommands.Transpose(this);
    }
    
    protected void EhMenuWorksheetAddColumns_OnClick(object sender, System.EventArgs e)
    {
      Commands.WorksheetCommands.AddDataColumns(this);
    }

    protected void EhMenuWorksheetAddPropertyColumns_OnClick(object sender, System.EventArgs e)
    {
      Commands.WorksheetCommands.AddPropertyColumns(this);
    }

    // ******************************************************************
    // ******************************************************************
    // Column (Popup)
    // ******************************************************************
    // ******************************************************************
    protected void EhMenuColumn_OnPopup(object sender, System.EventArgs e)
    {
      this.m_MenuItemColumnSetColumnValues.Enabled = 1==this.SelectedDataColumns.Count;
      this.m_MenuItemColumnRename.Enabled = 1==this.SelectedDataColumns.Count;
      this.m_MenuItemColumnSetGroupNumber.Enabled = this.SelectedDataColumns.Count>=1;
    }

    protected void EhMenuColumnSetColumnValues_OnClick(object sender, System.EventArgs e)
    {
      //Commands.ColumnCommands.SetColumnValues(this);
    }

    protected void EhMenuColumnSetColumnAsX_OnClick(object sender, System.EventArgs e)
    {
      Commands.ColumnCommands.SetSelectedColumnAsX(this);
    }

    protected void EhMenuColumnRename_OnClick(object sender, System.EventArgs e)
    {
      Commands.ColumnCommands.RenameSelectedColumn(this);
    }

    
    protected void EhMenuColumnSetGroupNumber_OnClick(object sender, System.EventArgs e)
    {
      Commands.ColumnCommands.SetSelectedColumnGroupNumber(this);
    }
  

    protected void EhMenuColumnExtractPropertyValues_OnClick(object sender, System.EventArgs e)
    { 
      Commands.ColumnCommands.ExtractPropertyValues(this);
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
      Commands.Analysis.FourierCommands.FFT(this);
    }

    // Analysis - 2 Dimensional FFT
    protected void EhMenuAnalysis2DFFT_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.FourierCommands.TwoDimensionalFFT(this);
    }


    protected void EhMenuAnalysisStatisticsOnColumns_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.StatisticCommands.StatisticsOnColumns(this);
    }

    protected void EhMenuAnalysisStatisticsOnRows_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.StatisticCommands.StatisticsOnRows(this);
    }

    // Analysis - Multiply Columns to Matrix
    protected void EhMenuAnalysisMultiplyColumnsToMatrix_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.ChemometricCommands.MultiplyColumnsToMatrix(this);
    }

    // Analysis - PCA on rows
    protected void EhMenuAnalysisPCAOnRows_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.ChemometricCommands.PCAOnRows(this);
    }

    // Analysis - PCA on cols
    protected void EhMenuAnalysisPCAOnCols_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.ChemometricCommands.PCAOnColumns(this);
    }

    // Analysis - PLS on rows
    protected void EhMenuAnalysisPLSOnRows_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.ChemometricCommands.PLSOnRows(this);
    }
    // Analysis - PLS on cols
    protected void EhMenuAnalysisPLSOnCols_OnClick(object sender, System.EventArgs e)
    {
      Commands.Analysis.ChemometricCommands.PLSOnColumns(this);
    }

    #endregion
  
    #region public properties

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Altaxo.Data.DataTable DataTable
    {
      get
      {
        return this.m_Table;
      }
    }


    public WorksheetLayout WorksheetLayout
    {
      get { return m_TableLayout; }
      set 
      {
        m_TableLayout = value; 
      
        Altaxo.Data.DataTable oldTable = m_Table;
        Altaxo.Data.DataTable newTable = null==m_TableLayout ? null : m_TableLayout.DataTable;
      
        if(null!=oldTable)
        {
          oldTable.DataColumns.Changed -= new EventHandler(this.EhTableDataChanged);
          oldTable.PropCols.Changed -= new EventHandler(this.EhPropertyDataChanged);
          oldTable.NameChanged -= new Main.NameChangedEventHandler(this.EhTableNameChanged);
        }

        m_Table = newTable;
        if(null!=newTable)
        {
          newTable.DataColumns.Changed += new EventHandler(this.EhTableDataChanged);
          newTable.PropCols.Changed += new EventHandler(this.EhPropertyDataChanged);
          newTable.NameChanged += new Main.NameChangedEventHandler(this.EhTableNameChanged);
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

    #endregion

    #region Selection related

    /// <summary>
    /// Returns the currently selected data columns
    /// </summary>
    public IndexSelection SelectedDataColumns
    {
      get { return m_SelectedDataColumns; }
    }

    /// <summary>
    /// Returns the currently selected data rows.
    /// </summary>
    public IndexSelection SelectedDataRows
    {
      get { return m_SelectedDataRows; }
    }

    /// <summary>
    /// Returns the currently selected property columns.
    /// </summary>
    public IndexSelection SelectedPropertyColumns
    {
      get { return m_SelectedPropertyColumns; }
    }

    /// <summary>
    /// Returns the currently selected property rows if property cells are selected alone. If not selected alone, the SelectedColumn property is returned.
    /// </summary>
    /// <remarks>Normally, if you select one or more data column, the corresponding property rows are selected by this. So it would be not possible to selected property rows without selecting the
    /// data column also. In order to fix this, you can first select property columns and then columns. In this case the selection is not stored into 
    /// SelectedColumns, but in SelectedPropertyRows, and SelectedColumns.Count returns 0.</remarks>
    public IndexSelection SelectedPropertyRows
    {
      get { return m_SelectedPropertyRows.Count>0 ? m_SelectedPropertyRows : m_SelectedDataColumns; }
    }



    /// <summary>
    /// Returns true if one or more property columns or rows are selected.
    /// </summary>
    public bool ArePropertyCellsSelected
    {
      get
      {
        return this.DataTable.PropCols.ColumnCount>0 && (SelectedPropertyColumns.Count>0 || m_SelectedPropertyRows.Count>0); 
      }
    }


    /// <summary>
    /// Returns true if one or more data columns or rows are selected.
    /// </summary>
    public bool AreDataCellsSelected
    {
      get { return this.DataTable.DataColumns.ColumnCount>0 && SelectedDataColumns.Count>0 || SelectedDataRows.Count>0; }
    }


    /// <summary>
    /// Returns true if one or more columns, rows or property columns or rows are selected.
    /// </summary>
    public bool AreColumnsOrRowsSelected
    {
      get { return AreDataCellsSelected || ArePropertyCellsSelected; }
    }

    /// <summary>
    /// Clears all selections of columns, rows or property columns.
    /// </summary>
    public void ClearAllSelections()
    {
      SelectedDataColumns.Clear();
      SelectedDataRows.Clear();
      SelectedPropertyColumns.Clear();
      SelectedPropertyRows.Clear();

      if(this.View!=null)
        this.View.TableAreaInvalidate();
    }

    /// <summary>
    /// Remove the selected columns, rows or property columns.
    /// </summary>
    public void RemoveSelected()
    {
      Commands.EditCommands.RemoveSelected(this);
    }


    #endregion

    /// <summary>
    /// Forces a redraw of the table view.
    /// </summary>
    public void UpdateTableView()
    {
      if(View!=null)
        View.TableAreaInvalidate();
    }

    #region "style related public methods"

    /// <summary>
    /// Retrieves the column style for the data column with index i.
    /// </summary>
    /// <param name="i">The index of the data column for which the style has to be returned.</param>
    /// <returns>The column style of the data column.</returns>
    public Altaxo.Worksheet.ColumnStyle GetDataColumnStyle(int i)
    {
      // zuerst in der ColumnStylesCollection nach dem passenden Namen
      // suchen, ansonsten default-Style zurückgeben
      Altaxo.Data.DataColumn dc = DataTable[i];
      Altaxo.Worksheet.ColumnStyle colstyle;

      // first look at the column styles hash table, column itself is the key
      colstyle = (Altaxo.Worksheet.ColumnStyle)m_TableLayout.ColumnStyles[dc];
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
        if(null!=(colstyle = (Altaxo.Worksheet.ColumnStyle)m_TableLayout.DefaultColumnStyles[searchstyletype]))
          return colstyle;

        // if not successfull yet, we will create a new defaultColumnStyle
        colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
        m_TableLayout.DefaultColumnStyles.Add(searchstyletype,colstyle);
        return colstyle;
      }
    }



    /// <summary>
    /// Retrieves the column style for the property column with index i.
    /// </summary>
    /// <param name="i">The index of the property column for which the style has to be returned.</param>
    /// <returns>The column style of the property column.</returns>
    public Altaxo.Worksheet.ColumnStyle GetPropertyColumnStyle(int i)
    {
      // zuerst in der ColumnStylesCollection nach dem passenden Namen
      // suchen, ansonsten default-Style zurückgeben
      Altaxo.Data.DataColumn dc = DataTable.PropCols[i];
      Altaxo.Worksheet.ColumnStyle colstyle;

      // first look at the column styles hash table, column itself is the key
      colstyle = (Altaxo.Worksheet.ColumnStyle)m_TableLayout.ColumnStyles[dc];
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
        if(null!=(colstyle = (Altaxo.Worksheet.ColumnStyle)m_TableLayout.DefaultPropertyColumnStyles[searchstyletype]))
          return colstyle;

        // if not successfull yet, we will create a new defaultColumnStyle
        colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
        m_TableLayout.DefaultPropertyColumnStyles.Add(searchstyletype,colstyle);
        colstyle.ChangeTypeTo(ColumnStyleType.PropertyCell);
        return colstyle;
      }
    }

    #endregion

    #region Data event handlers


    public void EhTableDataChanged(object sender, EventArgs e)
    {
      if(this.m_NumberOfTableRows!=DataTable.DataColumns.RowCount)
        this.SetCachedNumberOfDataRows();
      
      if(this.m_NumberOfTableCols!=DataTable.DataColumns.ColumnCount)
        this.SetCachedNumberOfDataColumns();

      if(View!=null)
        View.TableAreaInvalidate();
    }

  

    public void AdjustYScrollBarMaximum()
    {
      VertScrollMaximum = m_NumberOfTableRows>0 ? m_NumberOfTableRows-1 : 0;

      if(this.VertScrollPos>=m_NumberOfTableRows)
        VertScrollPos = m_NumberOfTableRows>0 ? m_NumberOfTableRows-1 : 0;

      if(View!=null)
        View.TableAreaInvalidate();
    }

    public void AdjustXScrollBarMaximum()
    {

      this.HorzScrollMaximum = m_NumberOfTableCols>0 ? m_NumberOfTableCols-1 : 0;

      if(HorzScrollPos+1>m_NumberOfTableCols)
        HorzScrollPos = m_NumberOfTableCols>0 ? m_NumberOfTableCols-1 : 0;
  
      if(View!=null)
      {
        m_ColumnStyleCache.ForceUpdate(this);
        View.TableAreaInvalidate();
      }
    }


    protected virtual void SetCachedNumberOfDataColumns()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldDataCols = this.m_NumberOfTableCols;
      this.m_NumberOfTableCols = DataTable.DataColumns.ColumnCount;
      if(this.m_NumberOfTableCols!=oldDataCols)
      {
        AdjustXScrollBarMaximum();
      }
    }


    protected virtual void SetCachedNumberOfDataRows()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldDataRows = this.m_NumberOfTableRows;
      this.m_NumberOfTableRows = DataTable.DataColumns.RowCount;

      if(m_NumberOfTableRows != oldDataRows)
      {
        AdjustYScrollBarMaximum();
      }

    }

    protected virtual void SetCachedNumberOfPropertyColumns()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldPropCols = this.m_NumberOfPropertyCols;
      this.m_NumberOfPropertyCols=m_Table.PropCols.ColumnCount;

      if(oldPropCols!=this.m_NumberOfPropertyCols)
      {
        // if we was scrolled to the most upper position, we later scroll
        // to the most upper position again
        bool bUpperPosition = (oldPropCols == -this.VertScrollPos);

        // Adjust Y ScrollBar Maximum();
        AdjustYScrollBarMaximum();

        if(bUpperPosition) // we scroll again to the most upper position
        {
          this.VertScrollPos = -this.TotalEnabledPropertyColumns;
        }
      }
    }

    public void EhPropertyDataChanged(object sender, EventArgs e)
    {
      if(this.m_NumberOfPropertyCols != DataTable.PropCols.ColumnCount)
        SetCachedNumberOfPropertyColumns();

      if(View!=null)
        View.TableAreaInvalidate();
    }

    public void EhTableNameChanged(object sender, Main.NameChangedEventArgs e)
    {
      if(View!=null)
        View.TableViewTitle = Doc.Name;

      this.TitleName = Doc.Name;
    }

    /// <summary>
    /// This is the whole name of the content, e.g. the file name or
    /// the url depending on the type of the content.
    /// </summary>
    public string TitleName 
    {
      get 
      { 
        return this.Doc.Name; 
      }
      set 
      {
        OnTitleNameChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Is called each time the name for the content has changed.
    /// </summary>
    public event EventHandler TitleNameChanged;

    protected virtual void OnTitleNameChanged(System.EventArgs e)
    {
      if(null!=TitleNameChanged)
        TitleNameChanged(this,e);
    }

    #endregion

    #region Edit box event handlers

    private void OnTextBoxLostControl(object sender, System.EventArgs e)
    {
      this.ReadCellEditContent();
      m_CellEditControl.Hide();
      m_CellEdit_IsArmed = false;
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
        m_CellEditControl.Hide();
      }
    }

    private void ReadCellEditContent()
    {
      if(this.m_CellEdit_IsArmed && this.m_CellEditControl.Modified)
      {
        if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
        {
          GetDataColumnStyle(m_CellEdit_EditedCell.Column).SetColumnValueAtRow(m_CellEditControl.Text,m_CellEdit_EditedCell.Row,DataTable[m_CellEdit_EditedCell.Column]);
        }
        else if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
        {
          GetPropertyColumnStyle(m_CellEdit_EditedCell.Column).SetColumnValueAtRow(m_CellEditControl.Text,m_CellEdit_EditedCell.Row,DataTable.PropCols[m_CellEdit_EditedCell.Column]);
        }
        this.m_CellEditControl.Hide();
        this.m_CellEdit_IsArmed=false;
      }
    }

    private void SetCellEditContent()
    {
      
      if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
      {
        m_CellEditControl.Text = GetDataColumnStyle(m_CellEdit_EditedCell.Column).GetColumnValueAtRow(m_CellEdit_EditedCell.Row,DataTable[m_CellEdit_EditedCell.Column]);
      }
      else if(this.m_CellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
      {
        m_CellEditControl.Text = this.GetPropertyColumnStyle(m_CellEdit_EditedCell.Column).GetColumnValueAtRow(m_CellEdit_EditedCell.Row,DataTable.PropCols[m_CellEdit_EditedCell.Column]);
      }

      m_CellEditControl.Parent = this.View.TableViewWindow;
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
      if(newCellCol>=DataTable.DataColumns.ColumnCount)
      {
        newCellCol=0;
        dy+=1;
      }
      else if(newCellCol<0)
      {
        if(this.m_CellEdit_EditedCell.Row>0) // move to the last cell only if not on cell 0
        {
          newCellCol=DataTable.DataColumns.ColumnCount-1;
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
        navigateToRow = newCellRow + 1 - FullyVisibleTableRows - this.FullyVisiblePropertyColumns;
      else
        navigateToRow = this.VertScrollPos;

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
        if(m_CellEdit_EditedCell.Row+1<DataTable.DataColumns.ColumnCount)
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
      if(newCellRow>=DataTable.DataColumns.ColumnCount)
      {
        if(newCellCol+1<DataTable.PropCols.ColumnCount) // move to the first cell only if not on the very last cell
        {
          newCellRow=0;
          newCellCol+=1;
        }
        else // we where on the last cell
        {
          newCellRow=DataTable.DataColumns.ColumnCount-1;
          newCellCol=DataTable.PropCols.ColumnCount-1;
        }
      }
      else if(newCellRow<0)
      {
        if(this.m_CellEdit_EditedCell.Column>0) // move to the last cell only if not on cell 0
        {
          newCellRow=DataTable.DataColumns.ColumnCount-1;
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
        navigateToCol = newCellCol-m_NumberOfPropertyCols;
      else if (newCellCol>LastFullyVisiblePropertyColumn)
        navigateToCol = newCellCol - this.FullyVisiblePropertyColumns + 1-m_NumberOfPropertyCols;
      else
        navigateToCol = this.VertScrollPos;


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

    /// <summary>
    /// The vertical scroll position is defined as following:
    /// If 0 (zero), the data row 0 is the first visible line (after the column header).
    /// If positive, the data row with the number of VertScrollPos is the first visible row.
    /// If negative, the property column with index PropertyColumnCount+VertScrollPos is the first visible line.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int VertScrollPos
    {
      get { return m_VertScrollPos; }
      set
      {
        int oldValue = m_VertScrollPos;
        int newValue = value;
        newValue = Math.Min(this.m_VertScrollMax,newValue);
        newValue = Math.Max(-this.TotalEnabledPropertyColumns,newValue);
        m_VertScrollPos=newValue;

        if(newValue!=oldValue)
        {
          if(m_CellEditControl.Visible)
          {
            this.ReadCellEditContent();
            m_CellEditControl.Hide();
            m_CellEdit_IsArmed = false;
          }

          // The value of the ScrollBar in the view has an offset, since he
          // can not have negative values;
          if(View!=null)
          {
            this.View.TableViewVertScrollValue = newValue + this.TotalEnabledPropertyColumns;
            this.View.TableAreaInvalidate();
          }
        }
      }
    }

    public int VertScrollMaximum
    {
      get { return this.m_VertScrollMax; }
      set 
      {
        this.m_VertScrollMax = value;
        
        if(View!=null)
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
        return this.m_TableLayout.ColumnHeaderStyle.Height + (VertScrollPos>=0 ? 0 : -VertScrollPos*this.m_TableLayout.PropertyColumnHeaderStyle.Height); 
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

      //int firstTotRow = (int)Math.Max(RemainingEnabledPropertyColumns,Math.Floor((top-m_TableLayout.ColumnHeaderStyle.Height)/(double)m_TableLayout.RowHeaderStyle.Height));
      //return FirstVisibleTableRow + Math.Max(0,firstTotRow-RemainingEnabledPropertyColumns);
      int firstVis = (int)Math.Floor((top-posOfDataRow0)/(double)m_TableLayout.RowHeaderStyle.Height);
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

      int firstRow = (int)Math.Floor((top-posOfDataRow0)/(double)m_TableLayout.RowHeaderStyle.Height);
      int lastRow  = (int)Math.Ceiling((bottom-posOfDataRow0)/(double)m_TableLayout.RowHeaderStyle.Height)-1;
      return Math.Max(0,1 + lastRow - firstRow);
    }

    public int GetFullyVisibleTableRows(int top, int bottom)
    {
      int posOfDataRow0 = this.VerticalPositionOfFirstVisibleDataRow;

      if(top<posOfDataRow0)
        top = posOfDataRow0;

      int firstRow = (int)Math.Floor((top-posOfDataRow0)/(double)m_TableLayout.RowHeaderStyle.Height);
      int lastRow  = (int)Math.Floor((bottom-posOfDataRow0)/(double)m_TableLayout.RowHeaderStyle.Height)-1;
      return Math.Max(0, 1+ lastRow - firstRow);
    }

    public int GetTopCoordinateOfTableRow(int nRow)
    {
      return  this.VerticalPositionOfFirstVisibleDataRow + (nRow- (VertScrollPos<0?0:VertScrollPos)) * m_TableLayout.RowHeaderStyle.Height;
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
        return m_TableLayout.ShowPropertyColumns ? Math.Max(0,-VertScrollPos) : 0;
      }
    }

    /// <summary>Returns number of property columns that are enabled for been shown on the grid.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int TotalEnabledPropertyColumns
    {
      get { return m_TableLayout.ShowPropertyColumns ? this.m_NumberOfPropertyCols : 0; }
    }



    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstVisiblePropertyColumn
    {
      get
      {
        if (m_TableLayout.ShowPropertyColumns && VertScrollPos < 0)
        {
          // make sure that VertScrollPos does not exceed TotalEnabledPropertyColumns
          if (VertScrollPos < -TotalEnabledPropertyColumns)
            VertScrollPos = -TotalEnabledPropertyColumns;
          return TotalEnabledPropertyColumns + VertScrollPos;
        }
        else
          return -1; 
      }
    }


    public int GetFirstVisiblePropertyColumn(int top)
    {
      int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_TableLayout.ColumnHeaderStyle.Height)/(double)m_TableLayout.PropertyColumnHeaderStyle.Height));
      int result = m_TableLayout.ShowPropertyColumns ? firstTotRow+FirstVisiblePropertyColumn : 0;
      return result;
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
      return m_TableLayout.ColumnHeaderStyle.Height + (nCol-FirstVisiblePropertyColumn)*m_TableLayout.PropertyColumnHeaderStyle.Height;
    }

    public int GetVisiblePropertyColumns(int top, int bottom)
    {
      if(this.m_TableLayout.ShowPropertyColumns)
      {
        int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_TableLayout.ColumnHeaderStyle.Height)/(double)m_TableLayout.PropertyColumnHeaderStyle.Height));
        int lastTotRow  = (int)Math.Ceiling((bottom-m_TableLayout.ColumnHeaderStyle.Height)/(double)m_TableLayout.PropertyColumnHeaderStyle.Height)-1;
        int maxPossRows = Math.Max(0,RemainingEnabledPropertyColumns-firstTotRow);
        return Math.Min(maxPossRows,Math.Max(0,1 + lastTotRow - firstTotRow));
      }
      else
        return 0;
    }

    public int GetFullyVisiblePropertyColumns(int top, int bottom)
    {
      if(m_TableLayout.ShowPropertyColumns)
      {
        int firstTotRow = (int)Math.Max(0,Math.Floor((top-m_TableLayout.ColumnHeaderStyle.Height)/(double)m_TableLayout.PropertyColumnHeaderStyle.Height));
        int lastTotRow  = (int)Math.Floor((bottom-m_TableLayout.ColumnHeaderStyle.Height)/(double)m_TableLayout.PropertyColumnHeaderStyle.Height)-1;
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
            m_CellEdit_IsArmed = false;
          }
          
          if(View!=null)
            View.TableViewHorzScrollValue = value;
          
          this.m_ColumnStyleCache.ForceUpdate(this);
          
          if(View!=null)
            View.TableAreaInvalidate();
        }
      }
    }

    public int HorzScrollMaximum
    {
      get { return this.m_HorzScrollMax; }
      set 
      {
        this.m_HorzScrollMax = value;
        if(View!=null)
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
      cellRect.Height = this.m_TableLayout.RowHeaderStyle.Height;
      return cellRect;
    }
  
    private Rectangle GetCoordinatesOfPropertyCell(int nCol, int nRow)
    {
      Rectangle cellRect = GetXCoordinatesOfColumn(nRow);

      cellRect.Y = this.GetTopCoordinateOfPropertyColumn(nCol);
      cellRect.Height = this.m_TableLayout.PropertyColumnHeaderStyle.Height;
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
      int horzSize = this.TableAreaWidth-m_TableLayout.RowHeaderStyle.Width;
      while(i>=0)
      {
        horzSize -= GetDataColumnStyle(i).Width;
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

    #region IWorksheetController Members

    public Altaxo.Data.DataTable Doc
    {
      get
      {
        return this.m_Table;
      }
    }

    public IWorksheetView View
    {
      get
      {
        return m_View;
      }
      set
      {
        IWorksheetView oldView = m_View;
        m_View = value;

        if(null!=oldView)
        {
          oldView.TableViewMenu = null; // don't let the old view have the menu
          oldView.WorksheetController = null; // no longer the controller of this view
          oldView.TableViewWindow.Controls.Remove(m_CellEditControl);
        }

        if(null!=m_View)
        {
          m_View.WorksheetController = this;
          m_View.TableViewMenu = m_MainMenu;
          m_View.TableViewWindow.Controls.Add(m_CellEditControl);

      
          // Werte für gerade vorliegende Scrollpositionen und Scrollmaxima zum (neuen) View senden
      
          this.VertScrollMaximum = this.m_VertScrollMax;
          this.HorzScrollMaximum = this.m_HorzScrollMax;

          this.VertScrollPos     = this.m_VertScrollPos;
          this.HorzScrollPos     = this.m_HorzScrollPos;

      
          
          // Simulate a SizeChanged event 
          this.EhView_TableAreaSizeChanged(new EventArgs());

          // set the menu of this class
          m_View.TableViewMenu = this.m_MainMenu;

        }
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
      this.m_MouseInfo.MouseUp(e,Control.MouseButtons);
      
      if(this.m_DragColumnWidth_InCapture)
      {
        int sizediff = e.X - this.m_DragColumnWidth_OriginalPos;
        Altaxo.Worksheet.ColumnStyle cs;
        if(-1==m_DragColumnWidth_ColumnNumber)
        {
          cs = this.m_TableLayout.RowHeaderStyle;
        }
        else
        {
          cs = (Altaxo.Worksheet.ColumnStyle)m_TableLayout.ColumnStyles[DataTable[m_DragColumnWidth_ColumnNumber]];
          if(null==cs)
          {
            Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(this.m_DragColumnWidth_ColumnNumber);
            cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
            m_TableLayout.ColumnStyles.Add(DataTable[m_DragColumnWidth_ColumnNumber],cs);
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
      this.m_MouseInfo.MouseDown(e);

      // base.OnMouseDown(e);
      this.m_MouseDownPosition = new Point(e.X, e.Y);
      this.ReadCellEditContent();
      m_CellEditControl.Hide();
      m_CellEdit_IsArmed = false;

      if(this.m_DragColumnWidth_ColumnNumber>=-1)
      {
        this.View.TableAreaCapture=true;
        m_DragColumnWidth_OriginalPos = e.X;
        m_DragColumnWidth_InCapture=true;
      }
    }

    /// <summary>
    /// Handles the mouse wheel event.
    /// </summary>
    /// <param name="e">MouseEventArgs.</param>
    public void EhView_TableAreaMouseWheel(System.Windows.Forms.MouseEventArgs e)
    {
      
      int oldScrollPos = VertScrollPos;
      VertScrollPos = VertScrollPos - SystemInformation.MouseWheelScrollLines*e.Delta/120;
      // Current.Console.WriteLine("MouseWheel {0}, {1}, {2}",e.Delta,oldScrollPos,VertScrollPos);
      
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
          cs = this.m_TableLayout.RowHeaderStyle;
        else
        {
          cs = (Altaxo.Worksheet.ColumnStyle)m_TableLayout.ColumnStyles[DataTable[m_DragColumnWidth_ColumnNumber]];
        
          if(null==cs)
          {
            Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(this.m_DragColumnWidth_ColumnNumber);
            cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
            m_TableLayout.ColumnStyles.Add(DataTable[m_DragColumnWidth_ColumnNumber],cs);
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
        if(Y<this.m_TableLayout.ColumnHeaderStyle.Height)
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

          if(this.m_TableLayout.RowHeaderStyle.Width -5 < X && X < m_TableLayout.RowHeaderStyle.Width+5)
          {
            this.View.TableAreaCursor = System.Windows.Forms.Cursors.VSplit;
            this.m_DragColumnWidth_ColumnNumber = -1;
            this.m_DragColumnWidth_OriginalWidth = this.m_TableLayout.RowHeaderStyle.Width;
            return;
          }
        }

        this.m_DragColumnWidth_ColumnNumber=int.MinValue;
        this.View.TableAreaCursor = System.Windows.Forms.Cursors.Default;
      } // end else
    }

    #region MouseClick functions
    protected virtual void OnLeftClickDataCell(ClickedCellInfo clickedCell)
    {
      //m_CellEditControl = new TextBox();
      m_CellEdit_EditedCell=clickedCell;
      m_CellEditControl.Parent = View.TableViewWindow;
      m_CellEditControl.Location = clickedCell.CellRectangle.Location;
      m_CellEditControl.Size = clickedCell.CellRectangle.Size;
      m_CellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
      this.SetCellEditContent();
    }

    protected virtual void OnLeftClickPropertyCell(ClickedCellInfo clickedCell)
    {
      m_CellEdit_EditedCell=clickedCell;
      m_CellEditControl.Parent = View.TableViewWindow;
      m_CellEditControl.Location = clickedCell.CellRectangle.Location;
      m_CellEditControl.Size = clickedCell.CellRectangle.Size;
      m_CellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
      this.SetCellEditContent();
    }

    protected virtual void OnLeftClickDataColumnHeader(ClickedCellInfo clickedCell)
    {
      if(!this.m_DragColumnWidth_InCapture)
      {
        bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
        bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
            
        bool bWasSelectedBefore = this.SelectedDataColumns.IsSelected(clickedCell.Column);

        /*
            if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
              m_SelectedRows.Clear(); // if we click a column, we remove row selections
            */

        if((!bControlKey && !bShiftKey) || (m_LastSelectionType!=SelectionType.DataColumnSelection && m_LastSelectionType!=SelectionType.PropertyRowSelection && !bControlKey))
        {
          m_SelectedDataColumns.Clear();
          m_SelectedDataRows.Clear(); // if we click a column, we remove row selections
          m_SelectedPropertyColumns.Clear();
          m_SelectedPropertyRows.Clear();
        }

        if(m_LastSelectionType==SelectionType.PropertyRowSelection)
        {
          m_SelectedPropertyRows.Select(clickedCell.Column,bShiftKey,bControlKey);
          m_LastSelectionType=SelectionType.PropertyRowSelection;
        }
          // if the last selection has only selected any property cells then add the current selection to the property rows
        else if(!this.AreDataCellsSelected && this.ArePropertyCellsSelected && bControlKey)
        {
          m_SelectedPropertyRows.Select(clickedCell.Column,bShiftKey,bControlKey);
          m_LastSelectionType = SelectionType.PropertyRowSelection;
        }
        else
        {
          if(this.SelectedDataColumns.Count!=0 || !bWasSelectedBefore)
            m_SelectedDataColumns.Select(clickedCell.Column,bShiftKey,bControlKey);
          m_LastSelectionType = SelectionType.DataColumnSelection;
        }

        this.View.TableAreaInvalidate();
      }
    }

    protected virtual void OnLeftClickDataRowHeader(ClickedCellInfo clickedCell)
    {
      bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
      bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));

      bool bWasSelectedBefore = this.SelectedDataRows.IsSelected(clickedCell.Row);

      /*
          if(m_LastSelectionType==SelectionType.DataColumnSelection && !bControlKey)
            m_SelectedColumns.Clear(); // if we click a column, we remove row selections
          */
      if((!bControlKey && !bShiftKey) || (m_LastSelectionType!=SelectionType.DataRowSelection && !bControlKey))
      {
        m_SelectedDataColumns.Clear(); // if we click a column, we remove row selections
        m_SelectedDataRows.Clear();
        m_SelectedPropertyColumns.Clear();
        m_SelectedPropertyRows.Clear();
      }

      // if we had formerly selected property rows, we clear them but add them before as column selection
      if(m_SelectedPropertyRows.Count>0)
      {
        if(m_SelectedDataColumns.Count==0)
        {
          for(int kk=0;kk<m_SelectedPropertyRows.Count;kk++)
            m_SelectedDataColumns.Add(m_SelectedPropertyRows[kk]);
        }
        m_SelectedPropertyRows.Clear();
      }
          
      if(this.SelectedDataRows.Count!=0 || !bWasSelectedBefore)
        m_SelectedDataRows.Select(clickedCell.Row,bShiftKey,bControlKey);
      m_LastSelectionType = SelectionType.DataRowSelection;
      this.View.TableAreaInvalidate();
    }

    protected virtual void OnLeftClickPropertyColumnHeader(ClickedCellInfo clickedCell)
    {
      bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
      bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
          
      bool bWasSelectedBefore = this.SelectedPropertyColumns.IsSelected(clickedCell.Column);

      if((!bControlKey && !bShiftKey) || (m_LastSelectionType!=SelectionType.PropertyColumnSelection && !bControlKey))
      {
        m_SelectedDataColumns.Clear();
        m_SelectedDataRows.Clear(); // if we click a column, we remove row selections
        m_SelectedPropertyColumns.Clear();
        m_SelectedPropertyRows.Clear();
      }

      /*
          if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
            m_SelectedRows.Clear(); // if we click a column, we remove row selections
          if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
            m_SelectedPropertyRows.Clear();
          if(m_LastSelectionType==SelectionType.DataColumnSelection && !bControlKey)
            m_SelectedColumns.Clear(); // if we click a column, we remove row selections
          */

      if(this.SelectedPropertyColumns.Count!=0 || !bWasSelectedBefore)
        m_SelectedPropertyColumns.Select(clickedCell.Column,bShiftKey,bControlKey);
          
      m_LastSelectionType = SelectionType.PropertyColumnSelection;
      this.View.TableAreaInvalidate();
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> TableHeaderLeftClicked;
    protected virtual void OnLeftClickTableHeader(ClickedCellInfo clickedCell)
    {
      if (null != TableHeaderLeftClicked)
        TableHeaderLeftClicked(this, clickedCell);
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> OutsideAllLeftClicked;
    protected virtual void OnLeftClickOutsideAll(ClickedCellInfo clickedCell)
    {
      if (null != OutsideAllLeftClicked)
        OutsideAllLeftClicked(this, clickedCell);
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> DataCellRightClicked;
    protected virtual void OnRightClickDataCell(ClickedCellInfo clickedCell)
    {
      if (null != DataCellRightClicked)
        DataCellRightClicked(this, clickedCell);
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> PropertyCellRightClicked;
    protected virtual void OnRightClickPropertyCell(ClickedCellInfo clickedCell)
    {
      if (null != PropertyCellRightClicked)
        PropertyCellRightClicked(this, clickedCell);
    }

    [field:NonSerialized]
    public event EventHandler<ClickedCellInfo> DataColumnHeaderRightClicked;
    protected virtual void OnRightClickDataColumnHeader(ClickedCellInfo clickedCell)
    {
      if (null != DataColumnHeaderRightClicked)
        DataColumnHeaderRightClicked(this, clickedCell);
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> DataRowHeaderRightClicked;
    protected virtual void OnRightClickDataRowHeader(ClickedCellInfo clickedCell)
    {
      if (null != DataRowHeaderRightClicked)
        DataRowHeaderRightClicked(this, clickedCell);
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> PropertyColumnHeaderRightClicked;
    protected virtual void OnRightClickPropertyColumnHeader(ClickedCellInfo clickedCell)
    {
      if (null != PropertyColumnHeaderRightClicked)
        PropertyColumnHeaderRightClicked(this, clickedCell);
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> TableHeaderRightClicked;
    protected virtual void OnRightClickTableHeader(ClickedCellInfo clickedCell)
    {
      if (null != TableHeaderRightClicked)
        TableHeaderRightClicked(this, clickedCell);
    }

    [field: NonSerialized]
    public event EventHandler<ClickedCellInfo> OutsideAllRightClicked;
    protected virtual void OnRightClickOutsideAll(ClickedCellInfo clickedCell)
    {
      if (null != OutsideAllRightClicked)
        OutsideAllRightClicked(this, clickedCell);
    }
    #endregion

    public void EhView_TableAreaMouseClick(EventArgs e)
    {
      m_MouseInfo.MouseClick(this, this.m_MouseDownPosition);

      //ClickedCellInfo clickedCell = new ClickedCellInfo(this,this.m_MouseDownPosition);

      if(m_MouseInfo.MouseButtonFirstDown==MouseButtons.Left)
      {
        switch(m_MouseInfo.ClickedArea)
        {
          case ClickedAreaType.DataCell:
            OnLeftClickDataCell(m_MouseInfo);
            break;
          case ClickedAreaType.PropertyCell:
            OnLeftClickPropertyCell(m_MouseInfo);
            break;
          case ClickedAreaType.PropertyColumnHeader:
            OnLeftClickPropertyColumnHeader(m_MouseInfo);
            break;
          case ClickedAreaType.DataColumnHeader:
            OnLeftClickDataColumnHeader(m_MouseInfo);
            break;
          case ClickedAreaType.DataRowHeader:
            OnLeftClickDataRowHeader(m_MouseInfo);
            break;
          case ClickedAreaType.TableHeader:
            OnLeftClickTableHeader(m_MouseInfo);
            break;
          case ClickedAreaType.OutsideAll:
            OnLeftClickOutsideAll(m_MouseInfo);
            break;
        }
      }
      else if(m_MouseInfo.MouseButtonFirstDown==MouseButtons.Right)
      {
        switch(m_MouseInfo.ClickedArea)
        {
          case ClickedAreaType.DataCell:
            OnRightClickDataCell(m_MouseInfo);
            break;
          case ClickedAreaType.PropertyCell:
            OnRightClickPropertyCell(m_MouseInfo);
            break;
          case ClickedAreaType.PropertyColumnHeader:
            OnRightClickPropertyColumnHeader(m_MouseInfo);
            break;
          case ClickedAreaType.DataColumnHeader:
            OnRightClickDataColumnHeader(m_MouseInfo);
            break;
          case ClickedAreaType.DataRowHeader:
            OnRightClickDataRowHeader(m_MouseInfo);
            break;
          case ClickedAreaType.TableHeader:
            OnRightClickTableHeader(m_MouseInfo);
            break;
          case ClickedAreaType.OutsideAll:
            OnRightClickOutsideAll(m_MouseInfo);
            break;
        }
      }
    }

    public void EhView_TableAreaMouseDoubleClick(EventArgs e)
    {
      // TODO:  Add WorksheetController.EhView_TableAreaMouseDoubleClick implementation
    }

    public void EhView_TableAreaPaint(System.Windows.Forms.PaintEventArgs e)
    {
      Graphics dc=e.Graphics;

      bool bDrawColumnHeader = false;

      int firstTableRowToDraw     = this.GetFirstVisibleTableRow(e.ClipRectangle.Top);
      int numberOfTableRowsToDraw = this.GetVisibleTableRows(e.ClipRectangle.Top,e.ClipRectangle.Bottom);

      int firstPropertyColumnToDraw = this.GetFirstVisiblePropertyColumn(e.ClipRectangle.Top);
      int numberOfPropertyColumnsToDraw = this.GetVisiblePropertyColumns(e.ClipRectangle.Top,e.ClipRectangle.Bottom);

      bool bAreColumnsSelected = m_SelectedDataColumns.Count>0;
      bool bAreRowsSelected =    m_SelectedDataRows.Count>0;
      bool bAreCellsSelected =  bAreRowsSelected || bAreColumnsSelected;
      
      bool bArePropertyColsSelected = m_SelectedPropertyColumns.Count>0;
      bool bArePropertyRowsSelected = SelectedPropertyRows.Count>0;
      bool bArePropertyCellsSelected = this.ArePropertyCellsSelected;


      int yShift=0;



      dc.FillRectangle(SystemBrushes.Window,e.ClipRectangle); // first set the background
      
      if(null==DataTable)
        return;

      Rectangle cellRectangle = new Rectangle();


      if(e.ClipRectangle.Top<m_TableLayout.ColumnHeaderStyle.Height)
      {
        bDrawColumnHeader = true;
      }

      // if neccessary, draw the row header (the most left column)
      if(e.ClipRectangle.Left<m_TableLayout.RowHeaderStyle.Width)
      {
        cellRectangle.Height = m_TableLayout.ColumnHeaderStyle.Height;
        cellRectangle.Width = m_TableLayout.RowHeaderStyle.Width;
        cellRectangle.X=0;
        
        // if visible, draw the top left corner of the table
        if (bDrawColumnHeader)
        {
          cellRectangle.Y = 0;
          m_TableLayout.RowHeaderStyle.PaintBackground(dc, cellRectangle, false);
        }

        // if visible, draw property column header items
        yShift=this.GetTopCoordinateOfPropertyColumn(firstPropertyColumnToDraw);
        cellRectangle.Height = m_TableLayout.PropertyColumnHeaderStyle.Height;
        for(int nPropCol=firstPropertyColumnToDraw, nInc=0;nInc<numberOfPropertyColumnsToDraw;nPropCol++,nInc++)
        {
          cellRectangle.Y = yShift+nInc*m_TableLayout.PropertyColumnHeaderStyle.Height;
          bool bPropColSelected = bArePropertyColsSelected && m_SelectedPropertyColumns.Contains(nPropCol);
          this.m_TableLayout.PropertyColumnHeaderStyle.Paint(dc,cellRectangle,nPropCol,this.DataTable.PropCols[nPropCol],bPropColSelected);
        }
      }

      // draw the table row Header Items
      yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
      cellRectangle.Height = m_TableLayout.RowHeaderStyle.Height;
      for(int nRow = firstTableRowToDraw,nInc=0; nInc<numberOfTableRowsToDraw; nRow++,nInc++)
      {
        cellRectangle.Y = yShift+nInc*m_TableLayout.RowHeaderStyle.Height;
        m_TableLayout.RowHeaderStyle.Paint(dc,cellRectangle,nRow,null, bAreRowsSelected && m_SelectedDataRows.Contains(nRow));
      }
      

      if(e.ClipRectangle.Bottom>=m_TableLayout.ColumnHeaderStyle.Height || e.ClipRectangle.Right>=m_TableLayout.RowHeaderStyle.Width)   
      {
        int numberOfColumnsToDraw;
        int firstColToDraw =this.GetFirstAndNumberOfVisibleColumn(e.ClipRectangle.Left,e.ClipRectangle.Right, out numberOfColumnsToDraw);

        // draw the property columns
        IndexSelection selectedPropertyRows = this.SelectedPropertyRows;
        for(int nPropCol=firstPropertyColumnToDraw, nIncPropCol=0; nIncPropCol<numberOfPropertyColumnsToDraw; nPropCol++, nIncPropCol++)
        {
          Altaxo.Worksheet.ColumnStyle cs = GetPropertyColumnStyle(nPropCol);
          bool bPropColSelected = bArePropertyColsSelected && m_SelectedPropertyColumns.Contains(nPropCol);
          bool bPropColIncluded = bArePropertyColsSelected  ? bPropColSelected : true; // Property cells are only included if the column is explicite selected

          cellRectangle.Y=this.GetTopCoordinateOfPropertyColumn(nPropCol);
          cellRectangle.Height = m_TableLayout.PropertyColumnHeaderStyle.Height;
          
          for(int nCol=firstColToDraw, nIncCol=0; nIncCol<numberOfColumnsToDraw; nCol++,nIncCol++)
          {
            bool bPropRowSelected = bArePropertyRowsSelected && selectedPropertyRows.Contains(nCol);
            bool bPropRowIncluded = bArePropertyRowsSelected ? bPropRowSelected : true;

            cellRectangle = this.GetXCoordinatesOfColumn(nCol,cellRectangle);
            cs.Paint(dc,cellRectangle,nCol,DataTable.PropCols[nPropCol],bArePropertyCellsSelected && bPropColIncluded && bPropRowIncluded);
          }
        }


        // draw the cells
        //int firstColToDraw = firstVisibleColumn+(e.ClipRectangle.Left-m_TableLayout.RowHeaderStyle.Width)/columnWidth;
        //int lastColToDraw  = firstVisibleColumn+(int)Math.Ceiling((e.ClipRectangle.Right-m_TableLayout.RowHeaderStyle.Width)/columnWidth);

        for(int nCol=firstColToDraw, nIncCol=0; nIncCol<numberOfColumnsToDraw; nCol++,nIncCol++)
        {
          Altaxo.Worksheet.ColumnStyle cs = GetDataColumnStyle(nCol);
          cellRectangle = this.GetXCoordinatesOfColumn(nCol,cellRectangle);

          bool bColumnSelected = bAreColumnsSelected && m_SelectedDataColumns.Contains(nCol);
          bool bDataColumnIncluded = bAreColumnsSelected  ? bColumnSelected : true;
          bool bPropertyRowSelected = bArePropertyRowsSelected && m_SelectedPropertyRows.Contains(nCol);

          if(bDrawColumnHeader) // must the column Header been drawn?
          {
            cellRectangle.Height = m_TableLayout.ColumnHeaderStyle.Height;
            cellRectangle.Y=0;
            m_TableLayout.ColumnHeaderStyle.Paint(dc,cellRectangle,0,DataTable[nCol],bColumnSelected || bPropertyRowSelected);
          }

  
          yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
          cellRectangle.Height = m_TableLayout.RowHeaderStyle.Height;
          for(int nRow=firstTableRowToDraw, nIncRow=0;nIncRow<numberOfTableRowsToDraw;nRow++,nIncRow++)
          {
            bool bRowSelected = bAreRowsSelected && m_SelectedDataRows.Contains(nRow);
            bool bDataRowIncluded = bAreRowsSelected ? bRowSelected : true;
            cellRectangle.Y= yShift+nIncRow*m_TableLayout.RowHeaderStyle.Height;
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
      if(null!=Data.DataTableCollection.GetParentDataTableCollectionOf(DataTable))
        Data.DataTableCollection.GetParentDataTableCollectionOf(DataTable).Remove(DataTable);
      DataTable.Dispose();

      // we then remove the view from the list of windows
      Current.ProjectService.RemoveWorksheet(this);
    }

    /// <summary>
    /// Called if the host window is about to be closed.
    /// </summary>
    /// <returns>True if the closing should be canceled, false otherwise.</returns>
    public bool HostWindowClosing()
    {
      if(!Current.ApplicationIsClosing)
      {
        System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.View.TableViewForm,"Do you really want to close this worksheet and delete the corresponding table?","Attention",System.Windows.Forms.MessageBoxButtons.YesNo);

        if(dlgres==System.Windows.Forms.DialogResult.No)
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Called by the host window after the host window was closed.
    /// </summary>
    public void HostWindowClosed()
    {
      // if the view is closed, we delete the corresponding table
      if(null!=Data.DataTableCollection.GetParentDataTableCollectionOf(DataTable))
        Data.DataTableCollection.GetParentDataTableCollectionOf(DataTable).Remove(DataTable);
      DataTable.Dispose();

      
      // we then remove the view from the list of windows
      Current.ProjectService.RemoveWorksheet(this);
    }

    public void EhView_Closing(System.ComponentModel.CancelEventArgs e)
    {
      if(!Current.ApplicationIsClosing)
      {
        System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.View.TableViewForm,"Do you really want to close this worksheet and delete the corresponding table?","Attention",System.Windows.Forms.MessageBoxButtons.YesNo);

        if(dlgres==System.Windows.Forms.DialogResult.No)
        {
          e.Cancel = true;
        }
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

  

      public void Update(WorksheetController dg)
      {
        if( (this.Count==0)
          ||(dg.TableAreaWidth!=this.m_CachedWidth)
          ||(dg.FirstVisibleColumn != this.m_CachedFirstVisibleColumn) )
        {
          ForceUpdate(dg);
        }
      }

      public void ForceUpdate(WorksheetController dg)
      {
        dg.m_LastVisibleColumn=0;
        dg.m_LastFullyVisibleColumn = 0;

        this.Clear(); // clear all items

        if(null==dg.DataTable)
          return;
    
        int actualColumnLeft = 0; 
        int actualColumnRight = dg.m_TableLayout.RowHeaderStyle.Width;
      
        this.m_CachedWidth = dg.TableAreaWidth;
        dg.m_LastFullyVisibleColumn = dg.FirstVisibleColumn;

        for(int i=dg.FirstVisibleColumn;i<dg.DataTable.DataColumns.ColumnCount && actualColumnLeft<this.m_CachedWidth;i++)
        {
          actualColumnLeft = actualColumnRight;
          Altaxo.Worksheet.ColumnStyle cs = dg.GetDataColumnStyle(i);
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

    #region IMVCController
    /// <summary>
    /// Returns the view that shows the model.
    /// </summary>
    public object ViewObject
    {
      get { return View; }
      set { View = value as IWorksheetView; }
    }
    /// <summary>
    /// Returns the model (document) that this controller controls
    /// </summary>
    public object ModelObject 
    {
      get { return this.DataTable; }
    }

    /// <summary>
    /// Creates a default view object.
    /// </summary>
    /// <returns>The default view object, or null if there is no default view object.</returns>
    public virtual object CreateDefaultViewObject()
    {
      this.View = new WorksheetView();
      return this.View;
    }
    #endregion

    #region IWorkbenchContentController Members

#if FormerGuiState
    IWorkbenchContentView IWorkbenchContentController.WorkbenchContentView
    {
      get
      {
        return m_View;
      }
      set
      {
        this.View = value as Altaxo.Worksheet.GUI.IWorksheetView;
      }
    }

    protected ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow m_ParentWorkbenchWindowController;
    public IWorkbenchWindowController ParentWorkbenchWindowController 
    { 
      get { return m_ParentWorkbenchWindowController as IWorkbenchWindowController; }
      set { m_ParentWorkbenchWindowController = value; }
    }

#endif

    public void CloseView()
    {
      this.View = null;
    }

    public void CreateView()
    {
      this.View = new WorksheetView();
    }

    #endregion

    #region ClipboardHandler Members

    public bool EnableCut
    {
      get { return m_CellEdit_IsArmed; }
    }

    public bool EnableCopy
    {
      get { return true; }
    }

    public bool EnablePaste
    {
      get { return true; }
    }

    public bool EnableDelete
    {
      get { return true; }
    }

    public bool EnableSelectAll
    {
      get { return true; }
    }

    public void Cut()
    {
      if (this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Cut();
      }
      else if (this.AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        Commands.EditCommands.CopyToClipboard(this);
      }
    }

    public void Copy()
    {
      if (this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Copy();
      }
      else if (this.AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        Commands.EditCommands.CopyToClipboard(this);
      }

    }

    public void Paste()
    {
      if (this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Paste();
      }
      else
      {
        Commands.EditCommands.PasteFromClipboard(this);
      }
    }

    public void Delete()
    {
      if (this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Clear();
      }
      else if (this.AreColumnsOrRowsSelected)
      {
        this.RemoveSelected();
      }
      else
      {
        // nothing is selected, we assume that the user wants to delete the worksheet itself
        Current.ProjectService.DeleteTable(this.DataTable, false);
      }
    }
    public void SelectAll()
    {
      if (this.DataTable.DataColumns.ColumnCount > 0)
      {
        this.SelectedDataColumns.Select(0, false, false);
        this.SelectedDataColumns.Select(this.DataTable.DataColumns.ColumnCount - 1, true, false);
        if (View != null)
          View.TableAreaInvalidate();
      }
    }

    #endregion


#if FormerGuiState

    #region ICSharpCode.SharpDevelop.Gui

    public void Dispose()
    {
    }

    /// <summary>
    /// This is the Windows.Forms control for the view.
    /// </summary>
    public System.Windows.Forms.Control Control 
    {
      get { return this.View as System.Windows.Forms.Control; }
    }

    /// <summary>
    /// The workbench window in which this view is displayed.
    /// </summary>
    public ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow  WorkbenchWindow 
    {
      get
      {
        return this.m_ParentWorkbenchWindowController;
      }
      set 
      { 
        this.m_ParentWorkbenchWindowController = value; 
      }
    }
    
    /// <summary>
    /// A generic name for the file, when it does have no file name
    /// (e.g. newly created files)
    /// </summary>
    public string UntitledName 
    {
      get { return "UntitledTable"; }
      set {}
    }
    
    

    
    
    /// <summary>
    /// The text on the tab page when more than one view content
    /// is attached to a single window.
    /// </summary>
    public string TabPageText 
    {
      get { return ContentName; }
    }

    /// <summary>
    /// If this property returns true the view is untitled.
    /// </summary>
    public bool IsUntitled 
    {
      get { return this.Doc.Name==null || this.Doc.Name==String.Empty; }
    }
    
    /// <summary>
    /// If this property returns true the content has changed since
    /// the last load/save operation.
    /// </summary>
    public bool IsDirty 
    {
      get { return false; }
      set {}
    }
    
    /// <summary>
    /// If this property returns true the content could not be altered.
    /// </summary>
    public bool IsReadOnly 
    {
      get { return false; }
    }
    
    /// <summary>
    /// If this property returns true the content can't be written.
    /// </summary>
    public bool IsViewOnly 
    {
      get { return true; }
    }
    
    /// <summary>
    /// Reinitializes the content. (Re-initializes all add-in tree stuff)
    /// and redraws the content. Call this not directly unless you know
    /// what you do.
    /// </summary>
    public void RedrawContent()
    {
    }
    
    /// <summary>
    /// Saves this content to the last load/save location.
    /// </summary>
    public void Save()
    {
    }

    
    /// <summary>
    /// Saves the content to the location <code>fileName</code>
    /// </summary>
    public void Save(string fileName)
    {
    }
    
    /// <summary>
    /// Loads the content from the location <code>fileName</code>
    /// </summary>
    public void Load(string fileName)
    {
    }

    protected virtual void OnBeforeSave(EventArgs e)
    {
      if (BeforeSave != null) 
      {
        BeforeSave(this, e);
      }
    }
    
    
    
    /// <summary>
    /// Is called when the content is changed after a save/load operation
    /// and this signals that changes could be saved.
    /// </summary>
    public event EventHandler DirtyChanged;

    public event EventHandler BeforeSave;

    #endregion
  
    #region ICSharpCode.SharpDevelop.Gui.IEditable
    
    public IClipboardHandler ClipboardHandler 
    {
      get { return this; }
    }
    
    public string Text 
    {
      get { return null; }
      set {}
    }
    
    public void Undo()
    {
    }
    public void Redo()
    {
    }
    #endregion

    #region ICSharpCode.SharpDevelop.Gui.IClipboardHandler
    
    public bool EnableCut 
    {
      get { return false; }
    }
    public bool EnableCopy 
    {
      get { return true; }
    }
    public bool EnablePaste 
    {
      get { return true; }
    }
    public bool EnableDelete 
    {
      get { return true; }
    }
    public bool EnableSelectAll 
    {
      get { return true; }
    }
    
    public void Cut(object sender, EventArgs e)
    {
      if(this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Cut();
      }
      else if(this.AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        DataGridOperations.CopyToClipboard(this);
      }
    }

    public void Copy(object sender, EventArgs e)
    {
      if(this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Copy();
      }
      else if(this.AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        DataGridOperations.CopyToClipboard(this);
      }
    
    }
    public void Paste(object sender, EventArgs e)
    {
      if(this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Paste();
      }
      else
      {
        DataGridOperations.PasteFromClipboard(this);
      }
    }
    public void Delete(object sender, EventArgs e)
    {
      if(this.m_CellEdit_IsArmed)
      {
        this.m_CellEditControl.Clear();
      }
      else if(this.AreColumnsOrRowsSelected)
      {
        this.RemoveSelected();
      }
      else
      {
        // nothing is selected, we assume that the user wants to delete the worksheet itself
        Current.ProjectService.DeleteTable(this.DataTable,false);
      }
    }
    public void SelectAll(object sender, EventArgs e)
    {
      if(this.DataTable.DataColumns.ColumnCount>0)
      {
        this.SelectedColumns.Select(0,false,false);
        this.SelectedColumns.Select(this.DataTable.DataColumns.ColumnCount-1,true,false);
        if(View!=null)
          View.TableAreaInvalidate();
      }
    }
    #endregion

#endif
  }
}
