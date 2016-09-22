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

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Graph.Gdi.Plot.Styles;
	using Altaxo.Graph.Scales;
	using Scales;

	public interface IColumnDrivenSymbolSizePlotStyleView
	{
		IDensityScaleView ScaleView { get; }

		event Action ChooseDataColumn;

		event Action ClearDataColumn;

		string DataColumnName { set; }

		double SymbolSizeAt0 { get; set; }

		double SymbolSizeAt1 { get; set; }

		double SymbolSizeAbove { get; set; }

		double SymbolSizeBelow { get; set; }

		double SymbolSizeInvalid { get; set; }

		int NumberOfSteps { get; set; }
	}

	[UserControllerForObject(typeof(ColumnDrivenSymbolSizePlotStyle))]
	[ExpectedTypeOfView(typeof(IColumnDrivenSymbolSizePlotStyleView))]
	public class ColumnDrivenSymbolSizePlotStyleController : MVCANControllerEditOriginalDocBase<ColumnDrivenSymbolSizePlotStyle, IColumnDrivenSymbolSizePlotStyleView>
	{
		private DensityScaleController _scaleController;
		private INumericColumn _tempDataColumn;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
		}

		public override void Dispose(bool isDisposing)
		{
			_tempDataColumn = null;
			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_scaleController = new DensityScaleController(newScale => _doc.Scale = (NumericalScale)newScale);
				_scaleController.InitializeDocument(_doc.Scale);
			}

			if (null != _view)
			{
				_scaleController.ViewObject = _view.ScaleView;
				_view.DataColumnName = _doc.DataColumn.FullName;
				_view.SymbolSizeAt0 = _doc.SymbolSizeAt0;
				_view.SymbolSizeAt1 = _doc.SymbolSizeAt1;
				_view.SymbolSizeBelow = _doc.SymbolSizeBelow;
				_view.SymbolSizeAbove = _doc.SymbolSizeAbove;
				_view.SymbolSizeInvalid = _doc.SymbolSizeInvalid;
				_view.NumberOfSteps = _doc.NumberOfSteps;
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_scaleController.Apply(disposeController))
				return false;

			if (!(_scaleController.ModelObject is Altaxo.Graph.Scales.NumericalScale))
			{
				Current.Gui.ErrorMessageBox("Please choose a numerical scale, since only those scales are supported here");
				return false;
			}

			_doc.Scale = (Altaxo.Graph.Scales.NumericalScale)_scaleController.ModelObject;

			if (null != _tempDataColumn)
				_doc.DataColumn = _tempDataColumn;

			_doc.SymbolSizeAt0 = _view.SymbolSizeAt0;
			_doc.SymbolSizeAt1 = _view.SymbolSizeAt1;
			_doc.SymbolSizeBelow = _view.SymbolSizeBelow;
			_doc.SymbolSizeAbove = _view.SymbolSizeAbove;
			_doc.SymbolSizeInvalid = _view.SymbolSizeInvalid;
			_doc.NumberOfSteps = _view.NumberOfSteps;

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.ChooseDataColumn += EhChooseDataColumn;
			_view.ClearDataColumn += EhClearDataColumn;
		}

		protected override void DetachView()
		{
			_view.ChooseDataColumn -= EhChooseDataColumn;
			_view.ClearDataColumn -= EhClearDataColumn;
			base.DetachView();
		}

		private void EhChooseDataColumn()
		{
			SingleColumnChoice choice = new SingleColumnChoice();
			choice.SelectedColumn = _tempDataColumn != null ? _tempDataColumn as DataColumn : _doc.DataColumn as DataColumn;
			object choiceAsObject = choice;
			if (Current.Gui.ShowDialog(ref choiceAsObject, "Select data column"))
			{
				choice = (SingleColumnChoice)choiceAsObject;
				if (choice.SelectedColumn is INumericColumn)
				{
					_tempDataColumn = (INumericColumn)choice.SelectedColumn;
					_view.DataColumnName = _tempDataColumn.FullName;
				}
			}
		}

		private void EhClearDataColumn()
		{
		}
	}
}