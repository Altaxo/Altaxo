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
using System.Collections.Generic;
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
  [Altaxo.Gui.UserControllerForObject(typeof(Altaxo.Worksheet.WorksheetLayout))]
  [Altaxo.Gui.ExpectedTypeOfView(typeof(IWinFormsWorksheetView))]
  public class WinFormsWorksheetController :
		Altaxo.Gui.Worksheet.Viewing.IGuiDependentWorksheetController,
		IWinFormsWorksheetViewEventSink, 
		Altaxo.Gui.IMVCController
  {
    public enum SelectionType { Nothing, DataRowSelection, DataColumnSelection, PropertyColumnSelection, PropertyRowSelection }


    #region Member variables

		Altaxo.Gui.Worksheet.Viewing.WorksheetController _guiIndependentController;

    /// <summary>
    /// Used to indicate that deserialization has not finished, and holds some deserialized values.
    /// </summary>
    private object _deserializationMemento;

    /// <summary>Holds the data table cached from the layout.</summary>
    protected Altaxo.Data.DataTable _table;


    protected Altaxo.Worksheet.WorksheetLayout _worksheetLayout;

    /// <summary>Holds the view (the window where the graph is visualized).</summary>
    protected IWinFormsWorksheetView _view;
    

    /// <summary>Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
    protected SelectionType _lastSelectionType;

    
    /// <summary>
    /// holds the positions (int) of the right boundarys of the __visible__ (!) columns
    /// i.e. columnBordersCache[0] is the with of the rowHeader plus the width of column[0]
    /// </summary>
    protected ColumnStyleCache _columnStyleCache;
    
    
    /// <summary>
    /// Horizontal scroll position; number of first column that is shown.
    /// </summary>
    private int _scrollHorzPos;
    /// <summary>
    /// Vertical scroll position; Positive values: number of first data column
    /// that is shown. Negative Values scroll more up in case of property columns.
    /// </summary>
    private int _scrollVertPos;
    private int _scrollHorzMax;
    private int _scrollVertMax;

    private int  _lastVisibleColumn;
    private int  _lastFullyVisibleColumn;

    
    /// <summary>
    /// Holds the indizes to the selected data columns.
    /// </summary>
    protected IndexSelection _selectedDataColumns; // holds the selected columns
    
    /// <summary>
    /// Holds the indizes to the selected rows.
    /// </summary>
    protected IndexSelection _selectedDataRows; // holds the selected rows
    
    /// <summary>
    /// Holds the indizes to the selected property columns.
    /// </summary>
    protected IndexSelection _selectedPropertyColumns; // holds the selected property columns


    /// <summary>
    /// Holds the indizes to the selected property rows (but only in case property cells are selected alone).
    /// </summary>
    protected IndexSelection _selectedPropertyRows; // holds the selected property rows


    /// <summary>
    /// Cached number of table rows.
    /// </summary>
    protected int _numberOfTableRows; // cached number of rows of the table
    /// <summary>
    /// Cached number of table columns.
    /// </summary>
    protected int _numberOfTableCols;
    
    /// <summary>
    /// Cached number of property columns.
    /// </summary>
    protected int _numberOfPropertyCols; // cached number of property  columnsof the table
    
  
    private ClickedCellInfo _mouseInfo = new ClickedCellInfo();

    private Point _mouseDownPosition; // holds the position of a double click
    private int  _dragColumnWidth_ColumnNumber; // stores the column number if mouse hovers over separator
    private int  _dragColumnWidth_OriginalPos;
    private int  _dragColumnWidth_OriginalWidth;
    private bool _dragColumnWidth_InCapture;
  

    protected bool                         _cellEdit_IsArmed;
    private ClickedCellInfo              _cellEdit_EditedCell;
    protected System.Windows.Forms.TextBox _cellEditControl; 


    /// <summary>
    /// Set the member variables to default values. Intended only for use in constructors and deserialization code.
    /// </summary>
    protected virtual void SetMemberVariablesToDefault()
    {
      _deserializationMemento=null;

      _table=null;
      _worksheetLayout=null;
      _view = null;
    
      // The main menu of this controller.

      // Which selection was done last: selection (i) a data column, (ii) a data row, or (iii) a property column.</summary>
      _lastSelectionType = SelectionType.Nothing;

    
      // holds the positions (int) of the right boundarys of the __visible__ (!) columns
      _columnStyleCache = new ColumnStyleCache();
    
    
      // Horizontal scroll position; number of first column that is shown.
      _scrollHorzPos=0;
    
      // Vertical scroll position; Positive values: number of first data column
      _scrollVertPos=0;
      _scrollHorzMax=1;
      _scrollVertMax=1;

      _lastVisibleColumn=0;
      _lastFullyVisibleColumn=0;

    
      // Holds the indizes to the selected data columns.
      _selectedDataColumns = new Altaxo.Worksheet.IndexSelection(); // holds the selected columns
    
      // Holds the indizes to the selected rows.
      _selectedDataRows    = new Altaxo.Worksheet.IndexSelection(); // holds the selected rows
    
      // Holds the indizes to the selected property columns.
      _selectedPropertyColumns = new Altaxo.Worksheet.IndexSelection(); // holds the selected property columns

      // Holds the indizes to the selected property columns.
      _selectedPropertyRows = new Altaxo.Worksheet.IndexSelection(); // holds the selected property columns

      // Cached number of table rows.
      _numberOfTableRows=0; // cached number of rows of the table

      // Cached number of table columns.
      _numberOfTableCols=0;
    
      // Cached number of property columns.
      _numberOfPropertyCols=0; // cached number of property  columnsof the table
    
        

      _mouseDownPosition = new Point(0,0); // holds the position of a double click
      _dragColumnWidth_ColumnNumber=int.MinValue; // stores the column number if mouse hovers over separator
      _dragColumnWidth_OriginalPos = 0;
      _dragColumnWidth_OriginalWidth=0;
      _dragColumnWidth_InCapture=false;
  

      _cellEdit_IsArmed=false;
      _cellEdit_EditedCell = new ClickedCellInfo();


      _cellEditControl = new System.Windows.Forms.TextBox();
      _cellEditControl.AcceptsTab = true;
      _cellEditControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      _cellEditControl.Location = new System.Drawing.Point(392, 0);
      _cellEditControl.Multiline = true;
      _cellEditControl.Name = "m_CellEditControl";
      _cellEditControl.TabIndex = 0;
      _cellEditControl.Text = "";
      _cellEditControl.Hide();
      _cellEdit_IsArmed = false;
      _cellEditControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnCellEditControl_KeyDown);
      _cellEditControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnCellEditControl_KeyPress);
      //m_View.TableViewWindow.Controls.Add(m_CellEditControl);

    }


    #endregion
  
    #region Constructors

		public WinFormsWorksheetController(Altaxo.Gui.Worksheet.Viewing.WorksheetController worksheet)
		{
			SetMemberVariablesToDefault();
			_guiIndependentController = worksheet;
			worksheet.InternalSetGuiController(this);
		}

		public void InternalInitializeWorksheetLayout(WorksheetLayout layout)
		{
			if (null == layout)
				throw new ArgumentNullException("layout");
			if (null != _worksheetLayout)
				throw new ApplicationException("This Gui controller has already a layout!");

			this.WorksheetLayout = layout;
		}


    #endregion // Constructors

  

 
   

    #region Menu Handler

   

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
      Altaxo.Serialization.Galactic.Import.ShowDialog(this.DataTable);
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
        return this._table;
      }
    }


    public WorksheetLayout WorksheetLayout
    {
      get { return _worksheetLayout; }
      set 
      {
        _worksheetLayout = value; 
      
        Altaxo.Data.DataTable oldTable = _table;
        Altaxo.Data.DataTable newTable = null==_worksheetLayout ? null : _worksheetLayout.DataTable;
      
        if(null!=oldTable)
        {
          oldTable.DataColumns.Changed -= new EventHandler(this.EhTableDataChanged);
          oldTable.PropCols.Changed -= new EventHandler(this.EhPropertyDataChanged);
          oldTable.NameChanged -= new Main.NameChangedEventHandler(this.EhTableNameChanged);
        }

        _table = newTable;
        if(null!=newTable)
        {
          newTable.DataColumns.Changed += new EventHandler(this.EhTableDataChanged);
          newTable.PropCols.Changed += new EventHandler(this.EhPropertyDataChanged);
          newTable.NameChanged += new Main.NameChangedEventHandler(this.EhTableNameChanged);
          this.SetCachedNumberOfDataColumns();
          this.SetCachedNumberOfDataRows();
          this.SetCachedNumberOfPropertyColumns();
          OnTitleNameChanged(EventArgs.Empty);
        }
        else // Data table is null
        {
          this._numberOfTableCols = 0;
          this._numberOfTableRows = 0;
          this._numberOfPropertyCols = 0;
          _columnStyleCache.Clear();
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
      get { return _selectedDataColumns; }
    }

    /// <summary>
    /// Returns the currently selected data rows.
    /// </summary>
    public IndexSelection SelectedDataRows
    {
      get { return _selectedDataRows; }
    }

    /// <summary>
    /// Returns the currently selected property columns.
    /// </summary>
    public IndexSelection SelectedPropertyColumns
    {
      get { return _selectedPropertyColumns; }
    }

    /// <summary>
    /// Returns the currently selected property rows if property cells are selected alone. If not selected alone, the SelectedColumn property is returned.
    /// </summary>
    /// <remarks>Normally, if you select one or more data column, the corresponding property rows are selected by this. So it would be not possible to selected property rows without selecting the
    /// data column also. In order to fix this, you can first select property columns and then columns. In this case the selection is not stored into 
    /// SelectedColumns, but in SelectedPropertyRows, and SelectedColumns.Count returns 0.</remarks>
    public IndexSelection SelectedPropertyRows
    {
      get { return _selectedPropertyRows.Count>0 ? _selectedPropertyRows : _selectedDataColumns; }
    }



    /// <summary>
    /// Returns true if one or more property columns or rows are selected.
    /// </summary>
    public bool ArePropertyCellsSelected
    {
      get
      {
        return this.DataTable.PropCols.ColumnCount>0 && (SelectedPropertyColumns.Count>0 || _selectedPropertyRows.Count>0); 
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
      if(_worksheetLayout.ColumnStyles.TryGetValue(dc, out colstyle))
        return colstyle;
      
      // second look to the defaultcolumnstyles hash table, key is the type of the column style

      System.Type searchstyletype = dc.GetColumnStyleType();
      if(null==searchstyletype)
      {
        throw new ApplicationException("Error: Column of type +" + dc.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
      }
      else
      {
        if(_worksheetLayout.DefaultColumnStyles.TryGetValue(searchstyletype, out colstyle))
          return colstyle;

        // if not successfull yet, we will create a new defaultColumnStyle
        colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
        _worksheetLayout.DefaultColumnStyles.Add(searchstyletype,colstyle);
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
      if(_worksheetLayout.ColumnStyles.TryGetValue(dc, out colstyle))
        return colstyle;
      
      // second look to the defaultcolumnstyles hash table, key is the type of the column style

      System.Type searchstyletype = dc.GetColumnStyleType();
      if(null==searchstyletype)
      {
        throw new ApplicationException("Error: Column of type +" + dc.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
      }
      else
      {
        if(_worksheetLayout.DefaultPropertyColumnStyles.TryGetValue(searchstyletype, out colstyle))
          return colstyle;

        // if not successfull yet, we will create a new defaultColumnStyle
        colstyle = (Altaxo.Worksheet.ColumnStyle)Activator.CreateInstance(searchstyletype);
        _worksheetLayout.DefaultPropertyColumnStyles.Add(searchstyletype,colstyle);
        colstyle.ChangeTypeTo(ColumnStyleType.PropertyCell);
        return colstyle;
      }
    }

    #endregion

    #region Data event handlers


    public void EhTableDataChanged(object sender, EventArgs e)
    {
      if(this._numberOfTableRows!=DataTable.DataColumns.RowCount)
        this.SetCachedNumberOfDataRows();
      
      if(this._numberOfTableCols!=DataTable.DataColumns.ColumnCount)
        this.SetCachedNumberOfDataColumns();

      if(View!=null)
        View.TableAreaInvalidate();
    }

  

    public void AdjustYScrollBarMaximum()
    {
      VertScrollMaximum = _numberOfTableRows>0 ? _numberOfTableRows-1 : 0;

      if(this.VertScrollPos>=_numberOfTableRows)
        VertScrollPos = _numberOfTableRows>0 ? _numberOfTableRows-1 : 0;

      if(View!=null)
        View.TableAreaInvalidate();
    }

    public void AdjustXScrollBarMaximum()
    {

      this.HorzScrollMaximum = _numberOfTableCols>0 ? _numberOfTableCols-1 : 0;

      if(HorzScrollPos+1>_numberOfTableCols)
        HorzScrollPos = _numberOfTableCols>0 ? _numberOfTableCols-1 : 0;
  
      if(View!=null)
      {
        _columnStyleCache.ForceUpdate(this);
        View.TableAreaInvalidate();
      }
    }


    protected virtual void SetCachedNumberOfDataColumns()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldDataCols = this._numberOfTableCols;
      this._numberOfTableCols = DataTable.DataColumns.ColumnCount;
      if(this._numberOfTableCols!=oldDataCols)
      {
        AdjustXScrollBarMaximum();
      }
    }


    protected virtual void SetCachedNumberOfDataRows()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldDataRows = this._numberOfTableRows;
      this._numberOfTableRows = DataTable.DataColumns.RowCount;

      if(_numberOfTableRows != oldDataRows)
      {
        AdjustYScrollBarMaximum();
      }

    }

    protected virtual void SetCachedNumberOfPropertyColumns()
    {
      // ask for table dimensions, compare with cached dimensions
      // and adjust the scroll bars appropriate
      int oldPropCols = this._numberOfPropertyCols;
      this._numberOfPropertyCols=_table.PropCols.ColumnCount;

      if(oldPropCols!=this._numberOfPropertyCols)
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
      if(this._numberOfPropertyCols != DataTable.PropCols.ColumnCount)
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
      _cellEditControl.Hide();
      _cellEdit_IsArmed = false;
    }

    private void OnCellEditControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      if(e.KeyChar == (char)13) // Don't use the enter key, event is handled by KeyDown
      {
        e.Handled=true;
      }
      else if(e.KeyChar == (char)9) // Tab key pressed
      {
        if(_cellEditControl.SelectionStart+_cellEditControl.SelectionLength>=_cellEditControl.TextLength)
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
        if(_cellEditControl.SelectionStart==0)
        {
          e.Handled=true;
          // Navigate to the left
          NavigateCellEdit(-1,0);
        }
      }
      else if(e.KeyData==System.Windows.Forms.Keys.Right)
      {
        if(_cellEditControl.SelectionStart+_cellEditControl.SelectionLength>=_cellEditControl.TextLength)
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
        if(_cellEditControl.SelectionLength>0)
        {
          _cellEditControl.SelectionLength=0;
          _cellEditControl.SelectionStart=_cellEditControl.TextLength;
        }
        else
        {
          NavigateCellEdit(0,1);
        }
      }
      else if(e.KeyData==System.Windows.Forms.Keys.Escape)
      {
        e.Handled=true;
        _cellEdit_IsArmed=false;
        _cellEditControl.Hide();
      }
    }

    private void ReadCellEditContent()
    {
      if(this._cellEdit_IsArmed && this._cellEditControl.Modified)
      {
        if(this._cellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
        {
          GetDataColumnStyle(_cellEdit_EditedCell.Column).SetColumnValueAtRow(_cellEditControl.Text,_cellEdit_EditedCell.Row,DataTable[_cellEdit_EditedCell.Column]);
        }
        else if(this._cellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
        {
          GetPropertyColumnStyle(_cellEdit_EditedCell.Column).SetColumnValueAtRow(_cellEditControl.Text,_cellEdit_EditedCell.Row,DataTable.PropCols[_cellEdit_EditedCell.Column]);
        }
        this._cellEditControl.Hide();
        this._cellEdit_IsArmed=false;
      }
    }

    private void SetCellEditContent()
    {
      
      if(this._cellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
      {
        _cellEditControl.Text = GetDataColumnStyle(_cellEdit_EditedCell.Column).GetColumnValueAtRow(_cellEdit_EditedCell.Row,DataTable[_cellEdit_EditedCell.Column]);
      }
      else if(this._cellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
      {
        _cellEditControl.Text = this.GetPropertyColumnStyle(_cellEdit_EditedCell.Column).GetColumnValueAtRow(_cellEdit_EditedCell.Row,DataTable.PropCols[_cellEdit_EditedCell.Column]);
      }

      _cellEditControl.Parent = this.View.TableViewWindow;
      _cellEditControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      _cellEditControl.SelectAll();
      _cellEditControl.Modified=false;
      _cellEditControl.BringToFront();
      _cellEditControl.Show();
      _cellEditControl.Focus();
      this._cellEdit_IsArmed=true;
    }


    /// <summary>
    /// NavigateCellEdit moves the cell edit control to the next cell
    /// </summary>
    /// <param name="dx">move dx cells to the right</param>
    /// <param name="dy">move dy cells down</param>
    /// <returns>True when the cell was moved to a new position, false if moving was not possible.</returns>
    protected bool NavigateCellEdit(int dx, int dy)
    {
      if(this._cellEdit_EditedCell.ClickedArea == ClickedAreaType.DataCell)
      {
        return NavigateTableCellEdit(dx,dy);
      }
      else if(this._cellEdit_EditedCell.ClickedArea == ClickedAreaType.PropertyCell)
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
      int newCellCol = this._cellEdit_EditedCell.Column + dx;
      if(newCellCol>=DataTable.DataColumns.ColumnCount)
      {
        newCellCol=0;
        dy+=1;
      }
      else if(newCellCol<0)
      {
        if(this._cellEdit_EditedCell.Row>0) // move to the last cell only if not on cell 0
        {
          newCellCol=DataTable.DataColumns.ColumnCount-1;
          dy-=1;
        }
        else
        {
          newCellCol=0;
        }
      }

      int newCellRow = _cellEdit_EditedCell.Row + dy;
      if(newCellRow<0)
        newCellRow=0;
      // note: we do not catch the condition newCellRow>rowCount here since we want to add new rows
  
    
      // look if the cell position has changed
      if(newCellRow==_cellEdit_EditedCell.Row && newCellCol==_cellEdit_EditedCell.Column)
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
      _cellEdit_EditedCell.Column=newCellCol;
      _cellEdit_EditedCell.Row=newCellRow;
      _cellEditControl.Parent = View.TableViewWindow;
      Rectangle cellRect = this.GetCoordinatesOfDataCell(_cellEdit_EditedCell.Column,_cellEdit_EditedCell.Row);
      _cellEditControl.Location = cellRect.Location;
      _cellEditControl.Size = cellRect.Size;
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
      int newCellCol = this._cellEdit_EditedCell.Column + dy;
      if(newCellCol>=DataTable.PropCols.ColumnCount)
      {
        if(_cellEdit_EditedCell.Row+1<DataTable.DataColumns.ColumnCount)
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
        if(this._cellEdit_EditedCell.Row>0) // move to the last cell only if not on cell 0
        {
          newCellCol=DataTable.PropCols.ColumnCount-1;
          dx-=1;
        }
        else
        {
          newCellCol=0;
        }
      }

      int newCellRow = _cellEdit_EditedCell.Row + dx;
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
        if(this._cellEdit_EditedCell.Column>0) // move to the last cell only if not on cell 0
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
      if(newCellRow==_cellEdit_EditedCell.Row && newCellCol==_cellEdit_EditedCell.Column)
        return false; // moving was not possible, so returning false, and do nothing

      // 1. Read content of the cell edit, if neccessary write data back
      ReadCellEditContent();    
  


      int navigateToCol;
      int navigateToRow;


      if(newCellCol<FirstVisiblePropertyColumn)
        navigateToCol = newCellCol-_numberOfPropertyCols;
      else if (newCellCol>LastFullyVisiblePropertyColumn)
        navigateToCol = newCellCol - this.FullyVisiblePropertyColumns + 1-_numberOfPropertyCols;
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
      _cellEdit_EditedCell.Column=newCellCol;
      _cellEdit_EditedCell.Row=newCellRow;
      _cellEditControl.Parent = View.TableViewWindow;
      Rectangle cellRect = this.GetCoordinatesOfPropertyCell(_cellEdit_EditedCell.Column,_cellEdit_EditedCell.Row);
      _cellEditControl.Location = cellRect.Location;
      _cellEditControl.Size = cellRect.Size;
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
      get { return _scrollVertPos; }
      set
      {
        int oldValue = _scrollVertPos;
        int newValue = value;
        newValue = Math.Min(this._scrollVertMax,newValue);
        newValue = Math.Max(-this.TotalEnabledPropertyColumns,newValue);
        _scrollVertPos=newValue;

        if(newValue!=oldValue)
        {
          if(_cellEditControl.Visible)
          {
            this.ReadCellEditContent();
            _cellEditControl.Hide();
            _cellEdit_IsArmed = false;
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
      get { return this._scrollVertMax; }
      set 
      {
        this._scrollVertMax = value;
        
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
        return this._worksheetLayout.ColumnHeaderStyle.Height + (VertScrollPos>=0 ? 0 : -VertScrollPos*this._worksheetLayout.PropertyColumnHeaderStyle.Height); 
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
      int firstVis = (int)Math.Floor((top-posOfDataRow0)/(double)_worksheetLayout.RowHeaderStyle.Height);
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

      int firstRow = (int)Math.Floor((top-posOfDataRow0)/(double)_worksheetLayout.RowHeaderStyle.Height);
      int lastRow  = (int)Math.Ceiling((bottom-posOfDataRow0)/(double)_worksheetLayout.RowHeaderStyle.Height)-1;
      return Math.Max(0,1 + lastRow - firstRow);
    }

    public int GetFullyVisibleTableRows(int top, int bottom)
    {
      int posOfDataRow0 = this.VerticalPositionOfFirstVisibleDataRow;

      if(top<posOfDataRow0)
        top = posOfDataRow0;

      int firstRow = (int)Math.Floor((top-posOfDataRow0)/(double)_worksheetLayout.RowHeaderStyle.Height);
      int lastRow  = (int)Math.Floor((bottom-posOfDataRow0)/(double)_worksheetLayout.RowHeaderStyle.Height)-1;
      return Math.Max(0, 1+ lastRow - firstRow);
    }

    public int GetTopCoordinateOfTableRow(int nRow)
    {
      return  this.VerticalPositionOfFirstVisibleDataRow + (nRow- (VertScrollPos<0?0:VertScrollPos)) * _worksheetLayout.RowHeaderStyle.Height;
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
        return _worksheetLayout.ShowPropertyColumns ? Math.Max(0,-VertScrollPos) : 0;
      }
    }

    /// <summary>Returns number of property columns that are enabled for been shown on the grid.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int TotalEnabledPropertyColumns
    {
      get { return _worksheetLayout.ShowPropertyColumns ? this._numberOfPropertyCols : 0; }
    }



    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FirstVisiblePropertyColumn
    {
      get
      {
        if (_worksheetLayout.ShowPropertyColumns && VertScrollPos < 0)
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
      int firstTotRow = (int)Math.Max(0,Math.Floor((top-_worksheetLayout.ColumnHeaderStyle.Height)/(double)_worksheetLayout.PropertyColumnHeaderStyle.Height));
      int result = _worksheetLayout.ShowPropertyColumns ? firstTotRow+FirstVisiblePropertyColumn : 0;
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
      return _worksheetLayout.ColumnHeaderStyle.Height + (nCol-FirstVisiblePropertyColumn)*_worksheetLayout.PropertyColumnHeaderStyle.Height;
    }

    public int GetVisiblePropertyColumns(int top, int bottom)
    {
      if(this._worksheetLayout.ShowPropertyColumns)
      {
        int firstTotRow = (int)Math.Max(0,Math.Floor((top-_worksheetLayout.ColumnHeaderStyle.Height)/(double)_worksheetLayout.PropertyColumnHeaderStyle.Height));
        int lastTotRow  = (int)Math.Ceiling((bottom-_worksheetLayout.ColumnHeaderStyle.Height)/(double)_worksheetLayout.PropertyColumnHeaderStyle.Height)-1;
        int maxPossRows = Math.Max(0,RemainingEnabledPropertyColumns-firstTotRow);
        return Math.Min(maxPossRows,Math.Max(0,1 + lastTotRow - firstTotRow));
      }
      else
        return 0;
    }

    public int GetFullyVisiblePropertyColumns(int top, int bottom)
    {
      if(_worksheetLayout.ShowPropertyColumns)
      {
        int firstTotRow = (int)Math.Max(0,Math.Floor((top-_worksheetLayout.ColumnHeaderStyle.Height)/(double)_worksheetLayout.PropertyColumnHeaderStyle.Height));
        int lastTotRow  = (int)Math.Floor((bottom-_worksheetLayout.ColumnHeaderStyle.Height)/(double)_worksheetLayout.PropertyColumnHeaderStyle.Height)-1;
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
      get { return _scrollHorzPos; }
      set
      {
        int oldValue = _scrollHorzPos;
        _scrollHorzPos=value;

        if(value!=oldValue)
        {

          if(_cellEditControl.Visible)
          {
            this.ReadCellEditContent();
            _cellEditControl.Hide();
            _cellEdit_IsArmed = false;
          }
          
          if(View!=null)
            View.TableViewHorzScrollValue = value;
          
          this._columnStyleCache.ForceUpdate(this);
          
          if(View!=null)
            View.TableAreaInvalidate();
        }
      }
    }

    public int HorzScrollMaximum
    {
      get { return this._scrollHorzMax; }
      set 
      {
        this._scrollHorzMax = value;
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
        return this._lastVisibleColumn>=FirstVisibleColumn ? 1+_lastVisibleColumn-FirstVisibleColumn : 0;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FullyVisibleColumns
    {
      get
      {
        return _lastFullyVisibleColumn>=FirstVisibleColumn ? 1+_lastFullyVisibleColumn-FirstVisibleColumn : 0;
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
      int nLastCol = _numberOfTableCols;
      ColumnStyleCacheItem csci;
      
      for(int nCol=FirstVisibleColumn,i=0 ; i<_columnStyleCache.Count ; nCol++,i++)
      {
        csci = ((ColumnStyleCacheItem)_columnStyleCache[i]);
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
      cellRect.X = ((ColumnStyleCacheItem)_columnStyleCache[colOffs]).leftBorderPosition;
      cellRect.Width = ((ColumnStyleCacheItem)_columnStyleCache[colOffs]).rightBorderPosition - cellRect.X;
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
      cellRect.Height = this._worksheetLayout.RowHeaderStyle.Height;
      return cellRect;
    }
  
    private Rectangle GetCoordinatesOfPropertyCell(int nCol, int nRow)
    {
      Rectangle cellRect = GetXCoordinatesOfColumn(nRow);

      cellRect.Y = this.GetTopCoordinateOfPropertyColumn(nCol);
      cellRect.Height = this._worksheetLayout.PropertyColumnHeaderStyle.Height;
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
      int horzSize = this.TableAreaWidth-_worksheetLayout.RowHeaderStyle.Width;
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

      _columnStyleCache.Update(this);

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
        return this._table;
      }
    }

    public IWinFormsWorksheetView View
    {
      get
      {
        return _view;
      }
      set
      {
        IWinFormsWorksheetView oldView = _view;
        _view = value;

        if(null!=oldView)
        {
          oldView.TableViewMenu = null; // don't let the old view have the menu
          oldView.WorksheetController = null; // no longer the controller of this view
          oldView.TableViewWindow.Controls.Remove(_cellEditControl);
        }

        if(null!=_view)
        {
          _view.WorksheetController = this;
          _view.TableViewWindow.Controls.Add(_cellEditControl);

      
          // Werte für gerade vorliegende Scrollpositionen und Scrollmaxima zum (neuen) View senden
      
          this.VertScrollMaximum = this._scrollVertMax;
          this.HorzScrollMaximum = this._scrollHorzMax;

          this.VertScrollPos     = this._scrollVertPos;
          this.HorzScrollPos     = this._scrollHorzPos;

      
          
          // Simulate a SizeChanged event 
          this.EhView_TableAreaSizeChanged(new EventArgs());
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
      this._mouseInfo.MouseUp(e,Control.MouseButtons);
      
      if(this._dragColumnWidth_InCapture)
      {
        int sizediff = e.X - this._dragColumnWidth_OriginalPos;
        Altaxo.Worksheet.ColumnStyle cs;
        if(-1==_dragColumnWidth_ColumnNumber)
        {
          cs = this._worksheetLayout.RowHeaderStyle;
        }
        else
        {
					_worksheetLayout.ColumnStyles.TryGetValue(DataTable[_dragColumnWidth_ColumnNumber], out cs);
          if(null==cs)
          {
            Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(this._dragColumnWidth_ColumnNumber);
            cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
            _worksheetLayout.ColumnStyles.Add(DataTable[_dragColumnWidth_ColumnNumber],cs);
          }
        }
        int newWidth = this._dragColumnWidth_OriginalWidth + sizediff;
        if(newWidth<10)
          newWidth=10;
        cs.Width=newWidth;
        this._columnStyleCache.ForceUpdate(this);

        this._dragColumnWidth_InCapture = false;
        this._dragColumnWidth_ColumnNumber = int.MinValue;
        this.View.TableAreaCapture=false;
        this.View.TableAreaCursor = System.Windows.Forms.Cursors.Default;
        this.View.TableAreaInvalidate();

      }
    }

    public void EhView_TableAreaMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
      this._mouseInfo.MouseDown(e);

      // base.OnMouseDown(e);
      this._mouseDownPosition = new Point(e.X, e.Y);
      this.ReadCellEditContent();
      _cellEditControl.Hide();
      _cellEdit_IsArmed = false;

      if(this._dragColumnWidth_ColumnNumber>=-1)
      {
        this.View.TableAreaCapture=true;
        _dragColumnWidth_OriginalPos = e.X;
        _dragColumnWidth_InCapture=true;
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

      if(this._dragColumnWidth_InCapture)
      {
        int sizediff = X - this._dragColumnWidth_OriginalPos;
        
        Altaxo.Worksheet.ColumnStyle cs;
        if(-1==_dragColumnWidth_ColumnNumber)
          cs = this._worksheetLayout.RowHeaderStyle;
        else
        {
					if(!_worksheetLayout.ColumnStyles.TryGetValue(DataTable[_dragColumnWidth_ColumnNumber], out cs))
          {
            Altaxo.Worksheet.ColumnStyle template = GetDataColumnStyle(this._dragColumnWidth_ColumnNumber);
            cs = (Altaxo.Worksheet.ColumnStyle)template.Clone();
            _worksheetLayout.ColumnStyles.Add(DataTable[_dragColumnWidth_ColumnNumber],cs);
          }
        }

        int newWidth = this._dragColumnWidth_OriginalWidth + sizediff;
        if(newWidth<10)
          newWidth=10;
        cs.Width=newWidth;
        this._columnStyleCache.ForceUpdate(this);
        this.View.TableAreaInvalidate();
      }
      else // not in Capture mode
      {
        if(Y<this._worksheetLayout.ColumnHeaderStyle.Height)
        {
          for(int i=this._columnStyleCache.Count-1;i>=0;i--)
          {
            ColumnStyleCacheItem csc = (ColumnStyleCacheItem)_columnStyleCache[i];

            if(csc.rightBorderPosition-5 < X && X < csc.rightBorderPosition+5)
            {
              this.View.TableAreaCursor = System.Windows.Forms.Cursors.VSplit;
              this._dragColumnWidth_ColumnNumber = i+FirstVisibleColumn;
              this._dragColumnWidth_OriginalWidth = csc.columnStyle.Width;
              return;
            }
          } // end for

          if(this._worksheetLayout.RowHeaderStyle.Width -5 < X && X < _worksheetLayout.RowHeaderStyle.Width+5)
          {
            this.View.TableAreaCursor = System.Windows.Forms.Cursors.VSplit;
            this._dragColumnWidth_ColumnNumber = -1;
            this._dragColumnWidth_OriginalWidth = this._worksheetLayout.RowHeaderStyle.Width;
            return;
          }
        }

        this._dragColumnWidth_ColumnNumber=int.MinValue;
        this.View.TableAreaCursor = System.Windows.Forms.Cursors.Default;
      } // end else
    }

    #region MouseClick functions
    protected virtual void OnLeftClickDataCell(ClickedCellInfo clickedCell)
    {
      //m_CellEditControl = new TextBox();
      _cellEdit_EditedCell=clickedCell;
      _cellEditControl.Parent = View.TableViewWindow;
      _cellEditControl.Location = clickedCell.CellRectangle.Location;
      _cellEditControl.Size = clickedCell.CellRectangle.Size;
      _cellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
      this.SetCellEditContent();
    }

    protected virtual void OnLeftClickPropertyCell(ClickedCellInfo clickedCell)
    {
      _cellEdit_EditedCell=clickedCell;
      _cellEditControl.Parent = View.TableViewWindow;
      _cellEditControl.Location = clickedCell.CellRectangle.Location;
      _cellEditControl.Size = clickedCell.CellRectangle.Size;
      _cellEditControl.LostFocus += new System.EventHandler(this.OnTextBoxLostControl);
      this.SetCellEditContent();
    }

    protected virtual void OnLeftClickDataColumnHeader(ClickedCellInfo clickedCell)
    {
      if(!this._dragColumnWidth_InCapture)
      {
        bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
        bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
            
        bool bWasSelectedBefore = this.SelectedDataColumns.IsSelected(clickedCell.Column);

        /*
            if(m_LastSelectionType==SelectionType.DataRowSelection && !bControlKey)
              m_SelectedRows.Clear(); // if we click a column, we remove row selections
            */

        if((!bControlKey && !bShiftKey) || (_lastSelectionType!=SelectionType.DataColumnSelection && _lastSelectionType!=SelectionType.PropertyRowSelection && !bControlKey))
        {
          _selectedDataColumns.Clear();
          _selectedDataRows.Clear(); // if we click a column, we remove row selections
          _selectedPropertyColumns.Clear();
          _selectedPropertyRows.Clear();
        }

        if(_lastSelectionType==SelectionType.PropertyRowSelection)
        {
          _selectedPropertyRows.Select(clickedCell.Column,bShiftKey,bControlKey);
          _lastSelectionType=SelectionType.PropertyRowSelection;
        }
          // if the last selection has only selected any property cells then add the current selection to the property rows
        else if(!this.AreDataCellsSelected && this.ArePropertyCellsSelected && bControlKey)
        {
          _selectedPropertyRows.Select(clickedCell.Column,bShiftKey,bControlKey);
          _lastSelectionType = SelectionType.PropertyRowSelection;
        }
        else
        {
          if(this.SelectedDataColumns.Count!=0 || !bWasSelectedBefore)
            _selectedDataColumns.Select(clickedCell.Column,bShiftKey,bControlKey);
          _lastSelectionType = SelectionType.DataColumnSelection;
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
      if((!bControlKey && !bShiftKey) || (_lastSelectionType!=SelectionType.DataRowSelection && !bControlKey))
      {
        _selectedDataColumns.Clear(); // if we click a column, we remove row selections
        _selectedDataRows.Clear();
        _selectedPropertyColumns.Clear();
        _selectedPropertyRows.Clear();
      }

      // if we had formerly selected property rows, we clear them but add them before as column selection
      if(_selectedPropertyRows.Count>0)
      {
        if(_selectedDataColumns.Count==0)
        {
          for(int kk=0;kk<_selectedPropertyRows.Count;kk++)
            _selectedDataColumns.Add(_selectedPropertyRows[kk]);
        }
        _selectedPropertyRows.Clear();
      }
          
      if(this.SelectedDataRows.Count!=0 || !bWasSelectedBefore)
        _selectedDataRows.Select(clickedCell.Row,bShiftKey,bControlKey);
      _lastSelectionType = SelectionType.DataRowSelection;
      this.View.TableAreaInvalidate();
    }

    protected virtual void OnLeftClickPropertyColumnHeader(ClickedCellInfo clickedCell)
    {
      bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
      bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
          
      bool bWasSelectedBefore = this.SelectedPropertyColumns.IsSelected(clickedCell.Column);

      if((!bControlKey && !bShiftKey) || (_lastSelectionType!=SelectionType.PropertyColumnSelection && !bControlKey))
      {
        _selectedDataColumns.Clear();
        _selectedDataRows.Clear(); // if we click a column, we remove row selections
        _selectedPropertyColumns.Clear();
        _selectedPropertyRows.Clear();
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
        _selectedPropertyColumns.Select(clickedCell.Column,bShiftKey,bControlKey);
          
      _lastSelectionType = SelectionType.PropertyColumnSelection;
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
      _mouseInfo.MouseClick(this, this._mouseDownPosition);

      //ClickedCellInfo clickedCell = new ClickedCellInfo(this,this.m_MouseDownPosition);

      if(_mouseInfo.MouseButtonFirstDown==MouseButtons.Left)
      {
        switch(_mouseInfo.ClickedArea)
        {
          case ClickedAreaType.DataCell:
            OnLeftClickDataCell(_mouseInfo);
            break;
          case ClickedAreaType.PropertyCell:
            OnLeftClickPropertyCell(_mouseInfo);
            break;
          case ClickedAreaType.PropertyColumnHeader:
            OnLeftClickPropertyColumnHeader(_mouseInfo);
            break;
          case ClickedAreaType.DataColumnHeader:
            OnLeftClickDataColumnHeader(_mouseInfo);
            break;
          case ClickedAreaType.DataRowHeader:
            OnLeftClickDataRowHeader(_mouseInfo);
            break;
          case ClickedAreaType.TableHeader:
            OnLeftClickTableHeader(_mouseInfo);
            break;
          case ClickedAreaType.OutsideAll:
            OnLeftClickOutsideAll(_mouseInfo);
            break;
        }
      }
      else if(_mouseInfo.MouseButtonFirstDown==MouseButtons.Right)
      {
        switch(_mouseInfo.ClickedArea)
        {
          case ClickedAreaType.DataCell:
            OnRightClickDataCell(_mouseInfo);
            break;
          case ClickedAreaType.PropertyCell:
            OnRightClickPropertyCell(_mouseInfo);
            break;
          case ClickedAreaType.PropertyColumnHeader:
            OnRightClickPropertyColumnHeader(_mouseInfo);
            break;
          case ClickedAreaType.DataColumnHeader:
            OnRightClickDataColumnHeader(_mouseInfo);
            break;
          case ClickedAreaType.DataRowHeader:
            OnRightClickDataRowHeader(_mouseInfo);
            break;
          case ClickedAreaType.TableHeader:
            OnRightClickTableHeader(_mouseInfo);
            break;
          case ClickedAreaType.OutsideAll:
            OnRightClickOutsideAll(_mouseInfo);
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

      bool bAreColumnsSelected = _selectedDataColumns.Count>0;
      bool bAreRowsSelected =    _selectedDataRows.Count>0;
      bool bAreCellsSelected =  bAreRowsSelected || bAreColumnsSelected;
      
      bool bArePropertyColsSelected = _selectedPropertyColumns.Count>0;
      bool bArePropertyRowsSelected = SelectedPropertyRows.Count>0;
      bool bArePropertyCellsSelected = this.ArePropertyCellsSelected;


      int yShift=0;



      dc.FillRectangle(SystemBrushes.Window,e.ClipRectangle); // first set the background
      
      if(null==DataTable)
        return;

      Rectangle cellRectangle = new Rectangle();


      if(e.ClipRectangle.Top<_worksheetLayout.ColumnHeaderStyle.Height)
      {
        bDrawColumnHeader = true;
      }

      // if neccessary, draw the row header (the most left column)
      if(e.ClipRectangle.Left<_worksheetLayout.RowHeaderStyle.Width)
      {
        cellRectangle.Height = _worksheetLayout.ColumnHeaderStyle.Height;
        cellRectangle.Width = _worksheetLayout.RowHeaderStyle.Width;
        cellRectangle.X=0;
        
        // if visible, draw the top left corner of the table
        if (bDrawColumnHeader)
        {
          cellRectangle.Y = 0;
          _worksheetLayout.RowHeaderStyle.PaintBackground(dc, cellRectangle, false);
        }

        // if visible, draw property column header items
        yShift=this.GetTopCoordinateOfPropertyColumn(firstPropertyColumnToDraw);
        cellRectangle.Height = _worksheetLayout.PropertyColumnHeaderStyle.Height;
        for(int nPropCol=firstPropertyColumnToDraw, nInc=0;nInc<numberOfPropertyColumnsToDraw;nPropCol++,nInc++)
        {
          cellRectangle.Y = yShift+nInc*_worksheetLayout.PropertyColumnHeaderStyle.Height;
          bool bPropColSelected = bArePropertyColsSelected && _selectedPropertyColumns.Contains(nPropCol);
          this._worksheetLayout.PropertyColumnHeaderStyle.Paint(dc,cellRectangle,nPropCol,this.DataTable.PropCols[nPropCol],bPropColSelected);
        }
      }

      // draw the table row Header Items
      yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
      cellRectangle.Height = _worksheetLayout.RowHeaderStyle.Height;
      for(int nRow = firstTableRowToDraw,nInc=0; nInc<numberOfTableRowsToDraw; nRow++,nInc++)
      {
        cellRectangle.Y = yShift+nInc*_worksheetLayout.RowHeaderStyle.Height;
        _worksheetLayout.RowHeaderStyle.Paint(dc,cellRectangle,nRow,null, bAreRowsSelected && _selectedDataRows.Contains(nRow));
      }
      

      if(e.ClipRectangle.Bottom>=_worksheetLayout.ColumnHeaderStyle.Height || e.ClipRectangle.Right>=_worksheetLayout.RowHeaderStyle.Width)   
      {
        int numberOfColumnsToDraw;
        int firstColToDraw =this.GetFirstAndNumberOfVisibleColumn(e.ClipRectangle.Left,e.ClipRectangle.Right, out numberOfColumnsToDraw);

        // draw the property columns
        IndexSelection selectedPropertyRows = this.SelectedPropertyRows;
        for(int nPropCol=firstPropertyColumnToDraw, nIncPropCol=0; nIncPropCol<numberOfPropertyColumnsToDraw; nPropCol++, nIncPropCol++)
        {
          Altaxo.Worksheet.ColumnStyle cs = GetPropertyColumnStyle(nPropCol);
          bool bPropColSelected = bArePropertyColsSelected && _selectedPropertyColumns.Contains(nPropCol);
          bool bPropColIncluded = bArePropertyColsSelected  ? bPropColSelected : true; // Property cells are only included if the column is explicite selected

          cellRectangle.Y=this.GetTopCoordinateOfPropertyColumn(nPropCol);
          cellRectangle.Height = _worksheetLayout.PropertyColumnHeaderStyle.Height;
          
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

          bool bColumnSelected = bAreColumnsSelected && _selectedDataColumns.Contains(nCol);
          bool bDataColumnIncluded = bAreColumnsSelected  ? bColumnSelected : true;
          bool bPropertyRowSelected = bArePropertyRowsSelected && _selectedPropertyRows.Contains(nCol);

          if(bDrawColumnHeader) // must the column Header been drawn?
          {
            cellRectangle.Height = _worksheetLayout.ColumnHeaderStyle.Height;
            cellRectangle.Y=0;
            _worksheetLayout.ColumnHeaderStyle.Paint(dc,cellRectangle,0,DataTable[nCol],bColumnSelected || bPropertyRowSelected);
          }

  
          yShift=this.GetTopCoordinateOfTableRow(firstTableRowToDraw);
          cellRectangle.Height = _worksheetLayout.RowHeaderStyle.Height;
          for(int nRow=firstTableRowToDraw, nIncRow=0;nIncRow<numberOfTableRowsToDraw;nRow++,nIncRow++)
          {
            bool bRowSelected = bAreRowsSelected && _selectedDataRows.Contains(nRow);
            bool bDataRowIncluded = bAreRowsSelected ? bRowSelected : true;
            cellRectangle.Y= yShift+nIncRow*_worksheetLayout.RowHeaderStyle.Height;
            cs.Paint(dc,cellRectangle,nRow,DataTable[nCol],bAreCellsSelected && bDataColumnIncluded && bDataRowIncluded);
          }
        }
      }   
    }

    public void EhView_TableAreaSizeChanged(EventArgs e)
    {
      _columnStyleCache.Update(this);
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
				if(false == Current.Gui.YesNoMessageBox("Do you really want to close this worksheet and delete the corresponding table?", "Attention", false))
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
				if(false == Current.Gui.YesNoMessageBox("Do you really want to close this worksheet and delete the corresponding table?","Attention", false))
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


    public class ColumnStyleCache : IList<ColumnStyleCacheItem>
    {
      protected int _cachedFirstVisibleColumn=0; // the column number of the first cached item, i.e. for this[0]
      protected int _cachedWidthOfPaintingArea=0; // cached width of painting area
			protected List<ColumnStyleCacheItem> _items = new List<ColumnStyleCacheItem>();
 
      public void Update(WinFormsWorksheetController dg)
      {
        if( (this.Count==0)
          ||(dg.TableAreaWidth!=this._cachedWidthOfPaintingArea)
          ||(dg.FirstVisibleColumn != this._cachedFirstVisibleColumn) )
        {
          ForceUpdate(dg);
        }
      }

      public void ForceUpdate(WinFormsWorksheetController dg)
      {
        dg._lastVisibleColumn=0;
        dg._lastFullyVisibleColumn = 0;

        this.Clear(); // clear all items

        if(null==dg.DataTable)
          return;
    
        int actualColumnLeft = 0; 
        int actualColumnRight = dg._worksheetLayout.RowHeaderStyle.Width;
      
        this._cachedWidthOfPaintingArea = dg.TableAreaWidth;
        dg._lastFullyVisibleColumn = dg.FirstVisibleColumn;

        for(int i=dg.FirstVisibleColumn;i<dg.DataTable.DataColumns.ColumnCount && actualColumnLeft<this._cachedWidthOfPaintingArea;i++)
        {
          actualColumnLeft = actualColumnRight;
          Altaxo.Worksheet.ColumnStyle cs = dg.GetDataColumnStyle(i);
          actualColumnRight = actualColumnLeft+cs.Width;
          this.Add(new ColumnStyleCacheItem(cs,actualColumnLeft,actualColumnRight));

          if(actualColumnLeft<this._cachedWidthOfPaintingArea)
            dg._lastVisibleColumn = i;

          if(actualColumnRight<=this._cachedWidthOfPaintingArea)
            dg._lastFullyVisibleColumn = i;
        }
      }

			#region IList<ColumnStyleCacheItem> Members

			public int IndexOf(ColumnStyleCacheItem item)
			{
				return _items.IndexOf(item);
			}

			public void Insert(int index, ColumnStyleCacheItem item)
			{
				_items.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				_items.RemoveAt(index);
			}

			public ColumnStyleCacheItem this[int index]
			{
				get
				{
					return _items[index];
				}
				set
				{
					_items[index] = value;
				}
			}

			#endregion

			#region ICollection<ColumnStyleCacheItem> Members

			public void Add(ColumnStyleCacheItem item)
			{
				_items.Add(item);
			}

			public void Clear()
			{
				_items.Clear();
			}

			public bool Contains(ColumnStyleCacheItem item)
			{
				return _items.Contains(item);
			}

			public void CopyTo(ColumnStyleCacheItem[] array, int arrayIndex)
			{
				_items.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return _items.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(ColumnStyleCacheItem item)
			{
				return _items.Remove(item);
			}

			#endregion

			#region IEnumerable<ColumnStyleCacheItem> Members

			public IEnumerator<ColumnStyleCacheItem> GetEnumerator()
			{
				return _items.GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _items.GetEnumerator();
			}

			#endregion
		}

    #endregion

    #region IMVCController
    /// <summary>
    /// Returns the view that shows the model.
    /// </summary>
    public object ViewObject
    {
      get { return View; }
      set { View = value as IWinFormsWorksheetView; }
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
      get { return _cellEdit_IsArmed; }
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
      get { return !_cellEdit_IsArmed; }
    }

    public bool EnableSelectAll
    {
      get { return true; }
    }

    public void Cut()
    {
      if (this._cellEdit_IsArmed)
      {
        this._cellEditControl.Cut();
      }
      else if (this.AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        Commands.EditCommands.CopyToClipboard(this);
      }
    }

    public void Copy()
    {
      if (this._cellEdit_IsArmed)
      {
        this._cellEditControl.Copy();
      }
      else if (this.AreColumnsOrRowsSelected)
      {
        // Copy the selected Columns to the clipboard
        Commands.EditCommands.CopyToClipboard(this);
      }

    }

    public void Paste()
    {
      if (this._cellEdit_IsArmed)
      {
        this._cellEditControl.Paste();
      }
      else
      {
        Commands.EditCommands.PasteFromClipboard(this);
      }
    }

    public void Delete()
    {
      if (this._cellEdit_IsArmed)
      {
        this._cellEditControl.Clear();
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

  }
}
