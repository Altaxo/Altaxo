#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Main;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Data;


namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IXYColumnPlotDataViewEventSink
  {
    void EhView_TableSelectionChanged();

    void EhView_ToX();
    void EhView_ToY();
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
    IXYColumnPlotDataViewEventSink Controller { set; }

    void Tables_Initialize(SelectableListNodeList items);

    void Columns_Initialize(SelectableListNodeList items);

    void XColumn_Initialize(string colname);
    void YColumn_Initialize(string colname);


    void PlotRangeFrom_Initialize(int from);
    void PlotRangeTo_Initialize(int to);

  }

 
  #endregion

  /// <summary>
  /// Summary description for LineScatterPlotDataController.
  /// </summary>
  [UserControllerForObject(typeof(XYColumnPlotData))]
  [ExpectedTypeOfView(typeof(IXYColumnPlotDataView))]
  public class XYColumnPlotDataController :  IXYColumnPlotDataViewEventSink, IMVCANController
  {
    IXYColumnPlotDataView _view=null;
    XYColumnPlotData _originalDoc;
		UseDocument _useDocumentCopy;


    bool _isDirty =false;

    int _plotRangeFrom;
    int _plotRangeTo;
    Altaxo.Data.IReadableColumn _xColumn;
    Altaxo.Data.IReadableColumn _yColumn;
    Altaxo.Data.IReadableColumn _labelColumn;
    int _maxPossiblePlotRangeTo;


		SelectableListNodeList _tableItems = new SelectableListNodeList();
		SelectableListNodeList _columnItems = new SelectableListNodeList();

    public void SetDirty()
    {
      _isDirty = true;
    }

    #region ILineScatterPlotDataController Members

    public void Initialize(bool initData)
    {
      if(initData)
      {
        _xColumn = _originalDoc.XColumn;
        _yColumn = _originalDoc.YColumn;
        _labelColumn = _originalDoc.LabelColumn;
        _plotRangeFrom = _originalDoc.PlotRangeStart;
        _plotRangeTo   = _originalDoc.PlotRangeLength==int.MaxValue ? int.MaxValue : _originalDoc.PlotRangeStart+_originalDoc.PlotRangeLength-1;
        CalcMaxPossiblePlotRangeTo();

				// Initialize tables
				string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

				DataTable t1 = DataTable.GetParentDataTableOf(_xColumn as IDocumentNode);
				DataTable t2 = DataTable.GetParentDataTableOf(_yColumn as IDocumentNode);
				DataTable tg = null;
				if (t1 != null && t2 != null && t1 == t2)
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

				_tableItems.Clear();
				foreach (var tableName in tables)
				{
					_tableItems.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], tg != null && tg.Name == tableName));
				}


				// Initialize columns
				_columnItems.Clear();
				if (null!=tg)
				{
					for(int i=0;i<tg.DataColumnCount;++i)
						_columnItems.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
				}
      }

      if(null!=_view)
      {
       
     
        _view.Tables_Initialize(_tableItems);
				_view.Columns_Initialize(_columnItems);

        _view.XColumn_Initialize(_xColumn==null ? String.Empty : _xColumn.FullName);
        _view.YColumn_Initialize(_yColumn==null ? String.Empty : _yColumn.FullName);
        _view.PlotRangeFrom_Initialize(_plotRangeFrom);
        CalcMaxPossiblePlotRangeTo();
      }
    }

    void CalcMaxPossiblePlotRangeTo()
    {
      int len = int.MaxValue;
      if(_xColumn is Altaxo.Data.IDefinedCount)
        len = Math.Min(len,((Altaxo.Data.IDefinedCount)_xColumn).Count);
      if(_yColumn is Altaxo.Data.IDefinedCount)
        len = Math.Min(len,((Altaxo.Data.IDefinedCount)_yColumn).Count);

      _maxPossiblePlotRangeTo = len-1;

      if(null!=_view)
        _view.PlotRangeTo_Initialize(Math.Min(this._plotRangeTo, _maxPossiblePlotRangeTo));
    }

    public void EhView_TableSelectionChanged()
    {
			_columnItems.Clear();

			var node = _tableItems.FirstSelectedNode;
			DataTable tg = node==null ? null : node.Tag as DataTable;
		
			if (null != tg)
			{
				for (int i = 0; i < tg.DataColumnCount; ++i)
					_columnItems.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
			}

      if(null!=_view)
      {
        _view.Columns_Initialize(_columnItems);
      }
    }

    public void EhView_ToX()
    {
      SetDirty();

			var node = _columnItems.FirstSelectedNode;
			_xColumn = node == null ? null : node.Tag as DataColumn;
      if(null!=_view)
        _view.XColumn_Initialize(_xColumn==null ? String.Empty : _xColumn.FullName);
    }

    public void EhView_ToY()
    {
      SetDirty();
			var node = _columnItems.FirstSelectedNode;
			_yColumn = node == null ? null : node.Tag as DataColumn;

			if (null != _view)
        _view.YColumn_Initialize(_yColumn==null ? String.Empty : _yColumn.FullName);
    }

    public void EhView_EraseX()
    {
      SetDirty();
      _xColumn = null;
      if(null!=_view)
        _view.XColumn_Initialize(_xColumn==null ? String.Empty : _xColumn.FullName);
    }

    public void EhView_EraseY()
    {
      SetDirty();
      _yColumn = null;
      if(null!=_view)
        _view.YColumn_Initialize(_yColumn==null ? String.Empty : _yColumn.FullName);
    }

    public bool EhView_RangeFrom(int val)
    {
      SetDirty();
      this._plotRangeFrom = val;
      return false;
    }

    public bool EhView_RangeTo(int val)
    {
      SetDirty();
      this._plotRangeTo = val;
      return false;
    }


    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if(_isDirty)
      {
        _originalDoc.XColumn = _xColumn;
        _originalDoc.YColumn = _yColumn;
        _originalDoc.PlotRangeStart = this._plotRangeFrom;
        _originalDoc.PlotRangeLength = this._plotRangeTo >= this._maxPossiblePlotRangeTo ? int.MaxValue : this._plotRangeTo+1-this._plotRangeFrom;
      }
      _isDirty = false;
      return true; // successfull
    }

    #endregion

    #region IMVCController Members

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || !(args[0] is XYColumnPlotData))
				return false;

			_originalDoc = (XYColumnPlotData)args[0];

			Initialize(true);

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = value; }
		}

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
				if (_view != null)
				{
					_view.Controller = null;
				}

				_view = value as IXYColumnPlotDataView;


				if (_view != null)
				{
					Initialize(false);
					_view.Controller = this;
				}
       
      }
    }

    public object ModelObject
    {
      get { return this._originalDoc; }
    }

    #endregion

	
	}
}
