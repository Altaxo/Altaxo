using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Graph.Gdi.Plot.Styles;
	using Altaxo.Data;

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
		ColumnDrivenColorPlotStyle _doc;
		IColumnDrivenColorPlotStyleView _view;
		DensityScaleController _scaleController;
		ColorProviderController _colorProviderController;

		INumericColumn _tempDataColumn;

		void Initialize(bool initData)
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

			if (!_colorProviderController.Apply())
				return false;
			_doc.ColorProvider = (Altaxo.Graph.Gdi.Plot.IColorProvider)_colorProviderController.ModelObject;

			if(null!=_tempDataColumn)
				_doc.DataColumn = _tempDataColumn;


			return true;
		}
	}
}
