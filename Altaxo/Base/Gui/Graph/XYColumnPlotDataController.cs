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
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Data;


namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IXYColumnPlotDataViewEventSink
  {
   
  
  

    void EhView_TableSelectionChanged(int selindex, string seltable);

    void EhView_ToX(int tableindex, string tablename, int columnindex, string columnname);
    void EhView_ToY(int tableindex, string tablename, int columnindex, string columnname);
    void EhView_EraseX();
    void EhView_EraseY();
 

    bool EhView_RangeFrom(int val);
    bool EhView_RangeTo(int val);
  }

  public interface IXYColumnPlotDataView 
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IXYColumnPlotDataViewEventSink Controller { get; set; }

    void Tables_Initialize(string[] tables, int selectedTable);

    void Columns_Initialize(string[] colnames, int selectedColumn);

    void XColumn_Initialize(string colname);
    void YColumn_Initialize(string colname);


    void PlotRangeFrom_Initialize(int from);
    void PlotRangeTo_Initialize(int to);

  }

  public interface IXYColumnPlotDataController : IMVCAController
  {
    void SetElements(bool bInit);
  }
  #endregion

  /// <summary>
  /// Summary description for LineScatterPlotDataController.
  /// </summary>
  [UserControllerForObject(typeof(XYColumnPlotData))]
  [ExpectedTypeOfView(typeof(IXYColumnPlotDataView))]
  public class XYColumnPlotDataController :  IXYColumnPlotDataViewEventSink, IXYColumnPlotDataController
  {
    IXYColumnPlotDataView _view=null;
    XYColumnPlotData m_PlotAssociation;


    bool m_bDirty =false;

    int m_PlotRange_From;
    int m_PlotRange_To;
    Altaxo.Data.IReadableColumn m_xCol;
    Altaxo.Data.IReadableColumn m_yCol;
    Altaxo.Data.IReadableColumn m_labelCol;
    int m_MaxPossiblePlotRange_To;

    public XYColumnPlotDataController(XYColumnPlotData pa)
    {
      m_PlotAssociation = pa;
      SetElements(true);
    }

    public void SetDirty()
    {
      m_bDirty = true;
    }

    #region ILineScatterPlotDataController Members

    public IXYColumnPlotDataView View
    { 
      get 
      {
        return _view;
      }

      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value;
        
        SetElements(false);

        if(_view!=null)
          _view.Controller = this;
        
      }
    }


    public void SetElements(bool bInit)
    {
      if(bInit)
      {
        m_xCol = m_PlotAssociation.XColumn;
        m_yCol = m_PlotAssociation.YColumn;
        m_labelCol = m_PlotAssociation.LabelColumn;
        m_PlotRange_From = m_PlotAssociation.PlotRangeStart;
        m_PlotRange_To   = m_PlotAssociation.PlotRangeLength==int.MaxValue ? int.MaxValue : m_PlotAssociation.PlotRangeStart+m_PlotAssociation.PlotRangeLength-1;
        CalcMaxPossiblePlotRangeTo();
      }

      if(null!=View)
      {
        string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

        Data.DataTable t1 = Data.DataTable.GetParentDataTableOf(m_xCol as Main.IDocumentNode);
        Data.DataTable t2 = Data.DataTable.GetParentDataTableOf(m_yCol as Main.IDocumentNode);
        Data.DataTable tg = null;
        if (t1 != null && t2!=null && t1 == t2)
          tg = t1;
        else if (t1 == null)
          tg = t2;
        else if (t2 == null)
          tg = t1;

        int seltable = -1;
        if (tg != null)
        {
          seltable = Array.IndexOf(tables, tg.Name);
        }
     
        View.Tables_Initialize(tables,seltable);

        if (seltable >= 0)
        {
          string[] columns = Current.Project.DataTableCollection[tables[seltable]].DataColumns.GetColumnNames();
          View.Columns_Initialize(columns, -1);
        }
        else
        {
          View.Columns_Initialize(new string[]{}, -1);
        }

        View.XColumn_Initialize(m_xCol==null ? String.Empty : m_xCol.FullName);
        View.YColumn_Initialize(m_yCol==null ? String.Empty : m_yCol.FullName);
        View.PlotRangeFrom_Initialize(m_PlotRange_From);
        CalcMaxPossiblePlotRangeTo();
      }
    }

    void CalcMaxPossiblePlotRangeTo()
    {
      int len = int.MaxValue;
      if(m_xCol is Altaxo.Data.IDefinedCount)
        len = Math.Min(len,((Altaxo.Data.IDefinedCount)m_xCol).Count);
      if(m_yCol is Altaxo.Data.IDefinedCount)
        len = Math.Min(len,((Altaxo.Data.IDefinedCount)m_yCol).Count);

      m_MaxPossiblePlotRange_To = len-1;

      if(null!=View)
        View.PlotRangeTo_Initialize(Math.Min(this.m_PlotRange_To, m_MaxPossiblePlotRange_To));
    }

    public void EhView_TableSelectionChanged(int tableindex, string tablename)
    {
      if(null!=View)
      {
        string[] columns = Current.Project.DataTableCollection[tablename].DataColumns.GetColumnNames();
        View.Columns_Initialize(columns,0);
      }
    }

    public void EhView_ToX(int tableindex, string tablename, int columnindex, string columnname)
    {
      SetDirty();
      m_xCol = Current.Project.DataTableCollection[tablename][columnname];
      if(null!=View)
        View.XColumn_Initialize(m_xCol==null ? String.Empty : m_xCol.FullName);
    }
    public void EhView_ToY(int tableindex, string tablename, int columnindex, string columnname)
    {
      SetDirty();
      m_yCol = Current.Project.DataTableCollection[tablename][columnname];
      if(null!=View)
        View.YColumn_Initialize(m_yCol==null ? String.Empty : m_yCol.FullName);
    }
    public void EhView_EraseX()
    {
      SetDirty();
      m_xCol = null;
      if(null!=View)
        View.XColumn_Initialize(m_xCol==null ? String.Empty : m_xCol.FullName);
    }
    public void EhView_EraseY()
    {
      SetDirty();
      m_yCol = null;
      if(null!=View)
        View.YColumn_Initialize(m_yCol==null ? String.Empty : m_yCol.FullName);
    }
    public bool EhView_RangeFrom(int val)
    {
      SetDirty();
      this.m_PlotRange_From = val;
      return false;
    }
    public bool EhView_RangeTo(int val)
    {
      SetDirty();
      this.m_PlotRange_To = val;
      return false;
    }


    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if(m_bDirty)
      {
        m_PlotAssociation.XColumn = m_xCol;
        m_PlotAssociation.YColumn = m_yCol;
        m_PlotAssociation.PlotRangeStart = this.m_PlotRange_From;
        m_PlotAssociation.PlotRangeLength = this.m_PlotRange_To >= this.m_MaxPossiblePlotRange_To ? int.MaxValue : this.m_PlotRange_To+1-this.m_PlotRange_From;
      }
      m_bDirty = false;
      return true; // successfull
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return View;
      }
      set
      {
        View = value as IXYColumnPlotDataView;
      }
    }

    public object ModelObject
    {
      get { return this.m_PlotAssociation; }
    }

    #endregion
  }
}
