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
	using ColorProvider;
	using Scales;

	public interface IColumnDrivenColorPlotStyleView
	{
		IDensityScaleView ScaleView { get; }

		IColorProviderView ColorProviderView { get; }

		event Action ChooseDataColumn;

		event Action ClearDataColumn;

		string DataColumnName { set; }
	}

	[UserControllerForObject(typeof(ColumnDrivenColorPlotStyle))]
	[ExpectedTypeOfView(typeof(IColumnDrivenColorPlotStyleView))]
	public class ColumnDrivenColorPlotStyleController : MVCANControllerEditOriginalDocBase<ColumnDrivenColorPlotStyle, IColumnDrivenColorPlotStyleView>
	{
		private DensityScaleController _scaleController;
		private ColorProviderController _colorProviderController;

		private INumericColumn _tempDataColumn;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
			yield return new ControllerAndSetNullMethod(_colorProviderController, () => _colorProviderController = null);
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
				_scaleController = new DensityScaleController(newScale => _doc.Scale = (NumericalScale)newScale) { UseDocumentCopy = UseDocument.Directly };
				_scaleController.InitializeDocument(_doc.Scale);

				_colorProviderController = new ColorProviderController(newColorProvider => _doc.ColorProvider = newColorProvider) { UseDocumentCopy = UseDocument.Directly };
				_colorProviderController.InitializeDocument(_doc.ColorProvider);
			}

			if (null != _view)
			{
				_scaleController.ViewObject = _view.ScaleView;
				_colorProviderController.ViewObject = _view.ColorProviderView;
				_view.DataColumnName = _doc.DataColumn.FullName;
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

			if (!_colorProviderController.Apply(disposeController))
				return false;
			_doc.ColorProvider = (Altaxo.Graph.Gdi.Plot.IColorProvider)_colorProviderController.ModelObject;

			if (null != _tempDataColumn)
				_doc.DataColumn = _tempDataColumn;

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