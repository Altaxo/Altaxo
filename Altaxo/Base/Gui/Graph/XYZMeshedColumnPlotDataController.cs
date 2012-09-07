#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Main;
using Altaxo.Collections;
using Altaxo.Graph.Plot.Data;
using Altaxo.Data;

namespace Altaxo.Gui.Graph
{
	public interface IXYZMeshedColumnPlotDataView
	{
		event Action SelectedTableChanged;
		event Action SelectedColumnKindChanged;
		event Action UseSelectedItemAsXColumn;
		event Action UseSelectedItemAsYColumn;
		event Action UseSelectedItemAsVColumns;

		event Action ClearXColumn;
		event Action ClearYColumn;
		event Action ClearVColumns;

		/// <summary>Gets a value indicating whether data columns or property columns are shown in the view.</summary>
		/// <value><see langword="true"/> if data columns are shown; otherwise, <see langword="false"/>.</value>
		bool AreDataColumnsShown { get; }

		void InitializeAvailableTables(SelectableListNodeList items);

		void InitializeAvailableColumns(SelectableListNodeList items);

		void Initialize_XColumn(string colname);
		void Initialize_YColumn(string colname);
		void Initialize_VColumns(SelectableListNodeList items);
		void EnableUseButtons(bool enableUseAsXColumn, bool enableUseAsYColumn, bool enableUseAsVColumns);


		void Initialize_PlotRangeFrom(int from);
		void Initialize_PlotRangeTo(int to);

	}

	[ExpectedTypeOfView(typeof(IXYZMeshedColumnPlotDataView))]
	[UserControllerForObject(typeof(XYZMeshedColumnPlotData))]
	public class XYZMeshedColumnPlotDataController : MVCANControllerBase<XYZMeshedColumnPlotData, IXYZMeshedColumnPlotDataView>
	{
		int _plotRangeFrom;
		int _plotRangeTo;
	
		Altaxo.Data.IReadableColumn _xColumn;
		Altaxo.Data.IReadableColumn _yColumn;
		SelectableListNodeList _valueColumns = new SelectableListNodeList();

		int _maxPossiblePlotRangeTo;


		SelectableListNodeList _availableTables = new SelectableListNodeList();
		SelectableListNodeList _availableColumns = new SelectableListNodeList();
		bool _areDataColumnsShown = true;

	

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_xColumn = _originalDoc.XColumn;
				_yColumn = _originalDoc.YColumn;
				// Initialize value columns
				_valueColumns.Clear();
				for (int i = 0; i < _doc.ColumnCount; ++i)
				{
					var col = _doc.GetDataColumn(i);
					_valueColumns.Add(new SelectableListNode(col.FullName, col, false));
				}

				_plotRangeFrom = _originalDoc.PlotRangeStart;
				_plotRangeTo = _originalDoc.PlotRangeLength == int.MaxValue ? int.MaxValue : _originalDoc.PlotRangeStart + _originalDoc.PlotRangeLength - 1;
				CalcMaxPossiblePlotRangeTo();

				// Initialize tables
				string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

				
				var parentTables = new HashSet<DataTable>();
				DataTable parent;

				parent = DataTable.GetParentDataTableOf(_xColumn as IDocumentNode);
				if (null != parent)
					parentTables.Add(parent);
				parent = DataTable.GetParentDataTableOf(_yColumn as IDocumentNode);
				if (null != parent)
					parentTables.Add(parent);
				parentTables.UnionWith(_originalDoc.DataColumns.Select(proxy => DataTable.GetParentDataTableOf(proxy.Document as IDocumentNode)).Where(dt => dt != null));

				parent = parentTables.Count == 1 ? parentTables.First() : null;

				int seltable = -1;
				if (parent != null)
				{
					seltable = Array.IndexOf(tables, parent.Name);
				}

				_availableTables.Clear();
				foreach (var tableName in tables)
				{
					_availableTables.Add(new SelectableListNode(tableName, Current.Project.DataTableCollection[tableName], parent != null && parent.Name == tableName));
				}


				// Initialize columns
				FillAvailableColumnList();
			}

			if (null != _view)
			{
				EhSelectedColumnKindChanged(); // ask view which column kind is now selected
				UpdateButtonEnablingInView(); // do that in every case, even if nothing has changed

				_view.InitializeAvailableTables(_availableTables);
				_view.InitializeAvailableColumns(_availableColumns);

				_view.Initialize_XColumn(_xColumn == null ? String.Empty : _xColumn.FullName);
				_view.Initialize_YColumn(_yColumn == null ? String.Empty : _yColumn.FullName);
				_view.Initialize_PlotRangeFrom(_plotRangeFrom);
				_view.Initialize_VColumns(_valueColumns);
				CalcMaxPossiblePlotRangeTo();
			}
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.SelectedTableChanged += EhSelectedTableChanged;
			_view.SelectedColumnKindChanged += EhSelectedColumnKindChanged;
			_view.UseSelectedItemAsXColumn += EhUseSelectedItemAsXColumn;
			_view.UseSelectedItemAsYColumn += EhUseSelectedItemAsYColumn;
			_view.UseSelectedItemAsVColumns += EhUseSelectedItemAsVColumns;
			_view.ClearXColumn += EhClearXColumn;
			_view.ClearYColumn += EhClearYColumn;
			_view.ClearVColumns += EhClearVColumns;
		}

		protected override void DetachView()
		{
			_view.SelectedTableChanged -= EhSelectedTableChanged;
			_view.SelectedColumnKindChanged -= EhSelectedColumnKindChanged;
			_view.UseSelectedItemAsXColumn -= EhUseSelectedItemAsXColumn;
			_view.UseSelectedItemAsYColumn -= EhUseSelectedItemAsYColumn;
			_view.ClearXColumn -= EhClearXColumn;
			_view.ClearYColumn -= EhClearYColumn;

			base.DetachView();
		}

		

		void CalcMaxPossiblePlotRangeTo()
		{
			int len = int.MaxValue;
			if (_xColumn is Altaxo.Data.IDefinedCount)
				len = Math.Min(len, ((Altaxo.Data.IDefinedCount)_xColumn).Count);
			if (_yColumn is Altaxo.Data.IDefinedCount)
				len = Math.Min(len, ((Altaxo.Data.IDefinedCount)_yColumn).Count);

			_maxPossiblePlotRangeTo = len - 1;

			if (null != _view)
				_view.Initialize_PlotRangeTo(Math.Min(this._plotRangeTo, _maxPossiblePlotRangeTo));
		}


		void FillAvailableColumnList()
		{
			_availableColumns.Clear();

			var node = _availableTables.FirstSelectedNode;
			DataTable tg = node == null ? null : node.Tag as DataTable;

			if (null != tg)
			{
				if (_areDataColumnsShown)
				{
					for (int i = 0; i < tg.DataColumnCount; ++i)
						_availableColumns.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
				}
				else
				{
					for (int i = 0; i < tg.PropertyColumnCount; ++i)
						_availableColumns.Add(new SelectableListNode(tg.PropertyColumns.GetColumnName(i), tg.PropertyColumns[i], false));
				}
			}

			if (null != _view)
			{
				_view.InitializeAvailableColumns(_availableColumns);
			}
		}

		void UpdateButtonEnablingInView()
		{
			if (null != _view)
			{
				_view.EnableUseButtons(_areDataColumnsShown, !_areDataColumnsShown, _areDataColumnsShown);
			}
		}

		void EhSelectedTableChanged()
		{
			FillAvailableColumnList();
		}


		void EhSelectedColumnKindChanged()
		{
			var newValue = _view.AreDataColumnsShown;
			if (_areDataColumnsShown != newValue)
			{
				_areDataColumnsShown = newValue;
				FillAvailableColumnList();
				UpdateButtonEnablingInView();
			}
		}

		void EhUseSelectedItemAsXColumn()
		{
			var node = _availableColumns.FirstSelectedNode;
			_xColumn = node == null ? null : node.Tag as DataColumn;
			if (null != _view)
				_view.Initialize_XColumn(_xColumn == null ? String.Empty : _xColumn.FullName);
		}

		void EhUseSelectedItemAsYColumn()
		{
			var node = _availableColumns.FirstSelectedNode;
			_yColumn = node == null ? null : node.Tag as DataColumn;

			if (null != _view)
				_view.Initialize_YColumn(_yColumn == null ? String.Empty : _yColumn.FullName);
		}

		void EhUseSelectedItemAsVColumns()
		{
			foreach (var node in _availableColumns.Where(n => n.IsSelected))
			{
				var colToAdd = node.Tag as IReadableColumn;
				if (colToAdd == null)
					continue;

				// before adding this node, check that it is not already present
				if (_valueColumns.Any(n => object.ReferenceEquals(n.Tag, colToAdd)))
					continue;

				_valueColumns.Add(new SelectableListNode(colToAdd.FullName, colToAdd, false));
			}
		}

		void EhClearXColumn()
		{
			_xColumn = null;
			if (null != _view)
				_view.Initialize_XColumn(_xColumn == null ? String.Empty : _xColumn.FullName);
		}

		void EhClearYColumn()
		{
			_yColumn = null;
			if (null != _view)
				_view.Initialize_YColumn(_yColumn == null ? String.Empty : _yColumn.FullName);
		}

		void EhClearVColumns()
		{
			if (null != _valueColumns.FirstSelectedNode) // if anything selected, clear only the selected nodes
			{
				_valueColumns.RemoveSelectedItems();
			}
			else // if nothing selected, clear all nodes
			{
				_valueColumns.Clear();
			}
		}


		public override bool Apply()
		{
				_originalDoc.XColumn = _xColumn;
				_originalDoc.YColumn = _yColumn;
				_originalDoc.SetDataColumns(_valueColumns.Select(n => (IReadableColumn)n.Tag));
				_originalDoc.PlotRangeStart = this._plotRangeFrom;
				_originalDoc.PlotRangeLength = this._plotRangeTo >= this._maxPossiblePlotRangeTo ? int.MaxValue : this._plotRangeTo + 1 - this._plotRangeFrom;

			return true; // successfull
		}
	}
}
