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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data
{
	using Altaxo.Collections;
	using Altaxo.Data;

	public interface IExpandCyclingVariableView
	{
		void InitializeCyclingVarColumn(SelectableListNodeList list);
		void InitializeColumnsToAverage(SelectableListNodeList list);
		void InitializeDestinationOutputFormat(SelectableListNodeList list);
		void InitializeDestinationX(SelectableListNodeList list);
		void InitializeDestinationColumnSorting(SelectableListNodeList list);
		void InitializeDestinationRowSorting(SelectableListNodeList list);
	}

	[UserControllerForObject(typeof(ExpandCyclingVariableColumnOptions))]
	[ExpectedTypeOfView(typeof(IExpandCyclingVariableView))]
	public class ExpandCyclingVariableController : IMVCANController
	{
		ExpandCyclingVariableColumnOptions _doc;
		IExpandCyclingVariableView _view;

		SelectableListNodeList _choicesCyclingVar;
		SelectableListNodeList _choicesColsToAvarage;
		SelectableListNodeList _choicesDestinationOutputFormat;
		SelectableListNodeList _choicesDestinationX;
		SelectableListNodeList _choicesDestinationColSort;
		SelectableListNodeList _choicesDestinationRowSort;


		void Initialize(bool initData)
		{
			if (initData)
			{
				_doc.EnsureCoherence(false);

				var srcData = _doc.SourceTable.DataColumns;

				_choicesCyclingVar = new SelectableListNodeList();
				foreach(var nCol in _doc.ColumnsToProcess)
					_choicesCyclingVar.Add(new SelectableListNode(srcData.GetColumnName(nCol),nCol,_doc.ColumnWithCyclingVariable==nCol));

				_choicesColsToAvarage = new SelectableListNodeList();
				foreach(var nCol in _doc.ColumnsToProcess)
					_choicesColsToAvarage.Add(new SelectableListNode(srcData.GetColumnName(nCol), nCol, _doc.ColumnsToAverageOverRepeatPeriod.Contains(nCol)));


				_choicesDestinationOutputFormat = new SelectableListNodeList();
				_choicesDestinationOutputFormat.FillWithEnumeration(_doc.DestinationOutput);

				_choicesDestinationX = new SelectableListNodeList();
				_choicesDestinationX.FillWithEnumeration(_doc.DestinationX);

				_choicesDestinationColSort = new SelectableListNodeList();
				_choicesDestinationColSort.FillWithEnumeration(_doc.DestinationColumnSorting);

				_choicesDestinationRowSort = new SelectableListNodeList();
				_choicesDestinationRowSort.FillWithEnumeration(_doc.DestinationRowSorting);

			}
			if (null != _view)
			{
				_view.InitializeCyclingVarColumn(_choicesCyclingVar);
				_view.InitializeColumnsToAverage(_choicesColsToAvarage);
				_view.InitializeDestinationOutputFormat(_choicesDestinationOutputFormat);
				_view.InitializeDestinationX(_choicesDestinationX);
				_view.InitializeDestinationColumnSorting(_choicesDestinationColSort);
				_view.InitializeDestinationRowSorting(_choicesDestinationRowSort);
			}
		}
		

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is ExpandCyclingVariableColumnOptions))
				return false;
			_doc = (ExpandCyclingVariableColumnOptions)args[0];

			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IExpandCyclingVariableView;
				Initialize(false);
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public bool Apply()
		{
			_doc.ColumnWithCyclingVariable = (int)_choicesCyclingVar.FirstSelectedNode.Tag;
			
			_doc.ColumnsToAverageOverRepeatPeriod.Clear();
			foreach (var node in _choicesColsToAvarage)
			{
				if (node.IsSelected)
					_doc.ColumnsToAverageOverRepeatPeriod.Add((int)node.Tag);
			}

			_doc.DestinationOutput = (ExpandCyclingVariableColumnOptions.OutputFormat)_choicesDestinationOutputFormat.FirstSelectedNode.Tag;
			_doc.DestinationX = (ExpandCyclingVariableColumnOptions.DestinationXColumn)_choicesDestinationX.FirstSelectedNode.Tag;
			_doc.DestinationColumnSorting = (ExpandCyclingVariableColumnOptions.OutputSorting)_choicesDestinationColSort.FirstSelectedNode.Tag;
			_doc.DestinationRowSorting = (ExpandCyclingVariableColumnOptions.OutputSorting)_choicesDestinationRowSort.FirstSelectedNode.Tag;

			return true;
		}
	}
}
