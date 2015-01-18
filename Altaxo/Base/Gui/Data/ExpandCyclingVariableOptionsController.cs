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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data
{
	using Altaxo.Collections;
	using Altaxo.Data;

	public interface IExpandCyclingVariableOptionsView
	{
		void InitializeDestinationOutputFormat(SelectableListNodeList list);

		void InitializeDestinationX(SelectableListNodeList list);

		void InitializeDestinationColumnSorting(SelectableListNodeList list);

		void InitializeDestinationRowSorting(SelectableListNodeList list);
	}

	[UserControllerForObject(typeof(ExpandCyclingVariableColumnOptions))]
	[ExpectedTypeOfView(typeof(IExpandCyclingVariableOptionsView))]
	public class ExpandCyclingVariableOptionsController : MVCANControllerBase<ExpandCyclingVariableColumnOptions, IExpandCyclingVariableOptionsView>
	{
		private SelectableListNodeList _choicesDestinationOutputFormat;
		private SelectableListNodeList _choicesDestinationX;
		private SelectableListNodeList _choicesDestinationColSort;
		private SelectableListNodeList _choicesDestinationRowSort;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
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
				_view.InitializeDestinationOutputFormat(_choicesDestinationOutputFormat);
				_view.InitializeDestinationX(_choicesDestinationX);
				_view.InitializeDestinationColumnSorting(_choicesDestinationColSort);
				_view.InitializeDestinationRowSorting(_choicesDestinationRowSort);
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.DestinationOutput = (ExpandCyclingVariableColumnOptions.OutputFormat)_choicesDestinationOutputFormat.FirstSelectedNode.Tag;
			_doc.DestinationX = (ExpandCyclingVariableColumnOptions.DestinationXColumn)_choicesDestinationX.FirstSelectedNode.Tag;
			_doc.DestinationColumnSorting = (ExpandCyclingVariableColumnOptions.OutputSorting)_choicesDestinationColSort.FirstSelectedNode.Tag;
			_doc.DestinationRowSorting = (ExpandCyclingVariableColumnOptions.OutputSorting)_choicesDestinationRowSort.FirstSelectedNode.Tag;

			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}