#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#endregion Copyright

using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Main;
using System;

namespace Altaxo.Gui.Graph3D.Plot.Data
{
	#region Interfaces

	public interface IXYZColumnPlotDataView
	{
		void Tables_Initialize(SelectableListNodeList items);

		void Columns_Initialize(SelectableListNodeList items);

		void XColumn_Initialize(string colname);

		void YColumn_Initialize(string colname);

		void ZColumn_Initialize(string colname);

		void PlotRangeFrom_Initialize(int from);

		void PlotRangeTo_Initialize(int to);

		event Action TableSelectionChanged;

		event Action Request_ToX;

		event Action Request_ToY;

		event Action Request_ToZ;

		event Action Request_EraseX;

		event Action Request_EraseY;

		event Action Request_EraseZ;

		event Action<int> RangeFromChanged;

		event Action<int> RangeToChanged;
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for LineScatterPlotDataController.
	/// </summary>
	[UserControllerForObject(typeof(XYZColumnPlotData))]
	[ExpectedTypeOfView(typeof(IXYZColumnPlotDataView))]
	public class XYZColumnPlotDataController
		:
		MVCANControllerEditOriginalDocBase<XYZColumnPlotData, IXYZColumnPlotDataView>
	{
		private bool _isDirty = false;

		private int _plotRangeFrom;
		private int _plotRangeTo;
		private Altaxo.Data.IReadableColumn _xColumn;
		private Altaxo.Data.IReadableColumn _yColumn;
		private Altaxo.Data.IReadableColumn _zColumn;
		private int _maxPossiblePlotRangeTo;

		private SelectableListNodeList _tableItems = new SelectableListNodeList();
		private SelectableListNodeList _columnItems = new SelectableListNodeList();

		public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_xColumn = null;
			_yColumn = null;
			_zColumn = null;

			_tableItems = null;
			_columnItems = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_xColumn = _doc.XColumn;
				_yColumn = _doc.YColumn;
				_zColumn = _doc.ZColumn;
				_plotRangeFrom = _doc.PlotRangeStart;
				_plotRangeTo = _doc.PlotRangeLength == int.MaxValue ? int.MaxValue : _doc.PlotRangeStart + _doc.PlotRangeLength - 1;
				CalcMaxPossiblePlotRangeTo();

				// Initialize tables
				string[] tables = Current.Project.DataTableCollection.GetSortedTableNames();

				DataTable t1 = DataTable.GetParentDataTableOf(_xColumn as IDocumentLeafNode);
				DataTable t2 = DataTable.GetParentDataTableOf(_yColumn as IDocumentLeafNode);
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
				if (null != tg)
				{
					for (int i = 0; i < tg.DataColumnCount; ++i)
						_columnItems.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
				}
			}

			if (null != _view)
			{
				_view.Tables_Initialize(_tableItems);
				_view.Columns_Initialize(_columnItems);

				_view.XColumn_Initialize(_xColumn == null ? String.Empty : _xColumn.FullName);
				_view.YColumn_Initialize(_yColumn == null ? String.Empty : _yColumn.FullName);
				_view.ZColumn_Initialize(_yColumn == null ? String.Empty : _zColumn.FullName);
				_view.PlotRangeFrom_Initialize(_plotRangeFrom);
				CalcMaxPossiblePlotRangeTo();
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (_isDirty)
			{
				_doc.XColumn = _xColumn;
				_doc.YColumn = _yColumn;
				_doc.ZColumn = _zColumn;
				_doc.PlotRangeStart = this._plotRangeFrom;
				_doc.PlotRangeLength = this._plotRangeTo >= this._maxPossiblePlotRangeTo ? int.MaxValue : this._plotRangeTo + 1 - this._plotRangeFrom;
			}
			_isDirty = false;

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.TableSelectionChanged += EhView_TableSelectionChanged;

			_view.Request_ToX += EhView_ToX;

			_view.Request_ToY += EhView_ToY;

			_view.Request_ToZ += EhView_ToZ;

			_view.Request_EraseX += EhView_EraseX;

			_view.Request_EraseY += EhView_EraseY;

			_view.Request_EraseZ += EhView_EraseZ;

			_view.RangeFromChanged += EhView_RangeFrom;

			_view.RangeToChanged += EhView_RangeTo;
		}

		protected override void DetachView()
		{
			_view.TableSelectionChanged -= EhView_TableSelectionChanged;

			_view.Request_ToX -= EhView_ToX;

			_view.Request_ToY -= EhView_ToY;

			_view.Request_ToZ -= EhView_ToZ;

			_view.Request_EraseX -= EhView_EraseX;

			_view.Request_EraseY -= EhView_EraseY;

			_view.Request_EraseZ -= EhView_EraseZ;

			_view.RangeFromChanged -= EhView_RangeFrom;

			_view.RangeToChanged -= EhView_RangeTo;

			base.DetachView();
		}

		public void SetDirty()
		{
			_isDirty = true;
		}

		#region ILineScatterPlotDataController Members

		private void CalcMaxPossiblePlotRangeTo()
		{
			int len = int.MaxValue;
			if (_xColumn is Altaxo.Data.IDefinedCount)
				len = Math.Min(len, ((Altaxo.Data.IDefinedCount)_xColumn).Count);
			if (_yColumn is Altaxo.Data.IDefinedCount)
				len = Math.Min(len, ((Altaxo.Data.IDefinedCount)_yColumn).Count);

			_maxPossiblePlotRangeTo = len - 1;

			if (null != _view)
				_view.PlotRangeTo_Initialize(Math.Min(this._plotRangeTo, _maxPossiblePlotRangeTo));
		}

		public void EhView_TableSelectionChanged()
		{
			_columnItems.Clear();

			var node = _tableItems.FirstSelectedNode;
			DataTable tg = node == null ? null : node.Tag as DataTable;

			if (null != tg)
			{
				for (int i = 0; i < tg.DataColumnCount; ++i)
					_columnItems.Add(new SelectableListNode(tg.DataColumns.GetColumnName(i), tg.DataColumns[i], false));
			}

			if (null != _view)
			{
				_view.Columns_Initialize(_columnItems);
			}
		}

		public void EhView_ToX()
		{
			SetDirty();

			var node = _columnItems.FirstSelectedNode;
			_xColumn = node == null ? null : node.Tag as DataColumn;
			if (null != _view)
				_view.XColumn_Initialize(_xColumn == null ? String.Empty : _xColumn.FullName);
		}

		public void EhView_ToY()
		{
			SetDirty();
			var node = _columnItems.FirstSelectedNode;
			_yColumn = node == null ? null : node.Tag as DataColumn;

			if (null != _view)
				_view.YColumn_Initialize(_yColumn == null ? String.Empty : _yColumn.FullName);
		}

		public void EhView_ToZ()
		{
			SetDirty();
			var node = _columnItems.FirstSelectedNode;
			_zColumn = node == null ? null : node.Tag as DataColumn;

			if (null != _view)
				_view.ZColumn_Initialize(_zColumn == null ? String.Empty : _zColumn.FullName);
		}

		public void EhView_EraseX()
		{
			SetDirty();
			_xColumn = null;
			if (null != _view)
				_view.XColumn_Initialize(_xColumn == null ? String.Empty : _xColumn.FullName);
		}

		public void EhView_EraseY()
		{
			SetDirty();
			_yColumn = null;
			if (null != _view)
				_view.YColumn_Initialize(_yColumn == null ? String.Empty : _yColumn.FullName);
		}

		public void EhView_EraseZ()
		{
			SetDirty();
			_zColumn = null;
			if (null != _view)
				_view.ZColumn_Initialize(_zColumn == null ? String.Empty : _zColumn.FullName);
		}

		public void EhView_RangeFrom(int val)
		{
			SetDirty();
			this._plotRangeFrom = val;
		}

		public void EhView_RangeTo(int val)
		{
			SetDirty();
			this._plotRangeTo = val;
		}

		#endregion ILineScatterPlotDataController Members
	}
}