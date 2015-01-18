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

namespace Altaxo.Gui.Graph
{
	using Altaxo.Data;
	using Altaxo.Graph.Gdi.Plot.Styles;

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
	public class ColumnDrivenColorPlotStyleController : IMVCANController
	{
		private ColumnDrivenColorPlotStyle _doc;
		private IColumnDrivenColorPlotStyleView _view;
		private DensityScaleController _scaleController;
		private ColorProviderController _colorProviderController;

		private INumericColumn _tempDataColumn;

		private void Initialize(bool initData)
		{
			if (initData)
			{
				_scaleController = new DensityScaleController();
				_scaleController.InitializeDocument(_doc.Scale);

				_colorProviderController = new ColorProviderController();
				_colorProviderController.InitializeDocument(_doc.ColorProvider);
			}

			if (null != _view)
			{
				_scaleController.ViewObject = _view.ScaleView;
				_colorProviderController.ViewObject = _view.ColorProviderView;
				_view.DataColumnName = _doc.DataColumn.FullName;
			}
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

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is ColumnDrivenColorPlotStyle))
				return false;
			_doc = (ColumnDrivenColorPlotStyle)args[0];

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
				if (null != _view)
				{
					_view.ChooseDataColumn -= EhChooseDataColumn;
					_view.ClearDataColumn -= EhClearDataColumn;
				}

				_view = value as IColumnDrivenColorPlotStyleView;

				if (null != _view)
				{
					Initialize(false);
					_view.ChooseDataColumn += EhChooseDataColumn;
					_view.ClearDataColumn += EhClearDataColumn;
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public void Dispose()
		{
		}

		public bool Apply(bool disposeController)
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

			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}
	}
}