using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Graph.Gdi.Plot.Styles;
	using Altaxo.Data;

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
	public class ColumnDrivenSymbolSizePlotStyleController : IMVCANController
	{
		ColumnDrivenSymbolSizePlotStyle _doc;
		IColumnDrivenSymbolSizePlotStyleView _view;
		DensityScaleController _scaleController;
		INumericColumn _tempDataColumn;

		void Initialize(bool initData)
		{
			if (initData)
			{
				_scaleController = new DensityScaleController();
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

		void EhChooseDataColumn()
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

		void EhClearDataColumn()
		{
		}


		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is ColumnDrivenSymbolSizePlotStyle))
				return false;
			_doc = (ColumnDrivenSymbolSizePlotStyle)args[0];

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

				_view = value as IColumnDrivenSymbolSizePlotStyleView;

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

		public bool Apply()
		{
			if (!_scaleController.Apply())
				return false;

			if (!(_scaleController.ModelObject is Altaxo.Graph.Scales.NumericalScale))
			{
				Current.Gui.ErrorMessageBox("Please choose a numerical scale, since only those scales are supported here");
				return false;
			}

			_doc.Scale = (Altaxo.Graph.Scales.NumericalScale)_scaleController.ModelObject;

			if(null!=_tempDataColumn)
				_doc.DataColumn = _tempDataColumn;

			_doc.SymbolSizeAt0 = _view.SymbolSizeAt0;
			_doc.SymbolSizeAt1 = _view.SymbolSizeAt1;
			_doc.SymbolSizeBelow = _view.SymbolSizeBelow;
			_doc.SymbolSizeAbove = _view.SymbolSizeAbove;
			_doc.SymbolSizeInvalid = _view.SymbolSizeInvalid;
			_doc.NumberOfSteps = _view.NumberOfSteps;

			return true;
		}
	}
}
