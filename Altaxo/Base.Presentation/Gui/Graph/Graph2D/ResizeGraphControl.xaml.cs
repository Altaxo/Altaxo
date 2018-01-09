using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Collections;
using Altaxo.Geometry;

namespace Altaxo.Gui.Graph.Graph2D
{
	using Altaxo.Units;
	using AUL = Altaxo.Units.Length;

	/// <summary>
	/// Interaction logic for ResizeGraphControl.xaml
	/// </summary>
	public partial class ResizeGraphControl : UserControl, IResizeGraphView
	{
		public event Action FontChanged;

		public ResizeGraphControl()
		{
			InitializeComponent();
		}

		public bool IsNewRootLayerSizeChosen => _guiIsNewRootLayerSizeChosen.IsChecked == true;

		public PointD2D NewRootLayerSize => new PointD2D(_guiNewRootLayerSizeX.SelectedQuantity.AsValueIn(AUL.Point.Instance), _guiNewRootLayerSizeY.SelectedQuantity.AsValueIn(AUL.Point.Instance));

		public bool IsNewStandardFontFamilyChosen => _guiIsNewStandardFontFamilyChosen.IsChecked == true;

		public string NewStandardFontFamily => _guiNewStandardFontFamily.SelectedFontFamilyName;

		public bool IsResetAllFontsToStandardFontFamilyChosen => _guiIsResetAllFontsToStandardFontFamilyChosen.IsChecked == true;

		public bool IsNewStandardFontSizeChosen => _guiIsNewStandardFontSizeChosen.IsChecked == true;

		public double NewStandardFontSize => _guiNewStandardFontSize.SelectedQuantityAsValueInPoints;

		public bool IsUserDefinedLineThicknessChosen => _guiIsUserDefinedLineThicknessChosen.IsChecked == true;

		public double UserDefinedLineThickness => _guiUserDefinedLineThicknessValue.SelectedQuantityAsValueInPoints;

		public SelectableListNodeList ActionsForFontSize { set => _guiActionForFontSize.Initialize(value); }
		public SelectableListNodeList ActionsForSymbolSize { set => _guiActionForSymbolSize.Initialize(value); }
		public SelectableListNodeList ActionsForLineThickness { set => _guiActionForLineThickness.Initialize(value); }

		public SelectableListNodeList ActionsForTickLength { set => _guiActionForTickLength.Initialize(value); }
		public bool IsUserDefinedMajorTickLengthChosen => _guiIsUserDefinedMajorTickLengthChosen.IsChecked == true;
		public double UserDefinedMajorTickLength => _guiUserDefinedMajorTickLength.SelectedQuantityAsValueInPoints;

		public void SetOldRootLayerSize(PointD2D size)
		{
			_guiNewRootLayerSizeX.SelectedQuantity = new DimensionfulQuantity(size.X, AUL.Point.Instance).AsQuantityIn(_guiNewRootLayerSizeX.UnitEnvironment.DefaultUnit);
			_guiNewRootLayerSizeY.SelectedQuantity = new DimensionfulQuantity(size.Y, AUL.Point.Instance).AsQuantityIn(_guiNewRootLayerSizeY.UnitEnvironment.DefaultUnit);
		}

		public void SetOldStandardFont(string font)
		{
			if (!string.IsNullOrEmpty(font))
				_guiNewStandardFontFamily.SelectedFontFamilyName = font;
		}

		public void SetOldStandardFontSize(double size)
		{
			_guiNewStandardFontSize.SelectedQuantityAsValueInPoints = size;
		}

		public void SetOldStandardLineThickness(double thickness)
		{
			if (false == _guiIsUserDefinedLineThicknessChosen.IsChecked)
				_guiUserDefinedLineThicknessValue.SelectedQuantityAsValueInPoints = thickness;
		}

		public void SetOldStandardMajorTickLength(double tickLength)
		{
			if (false == _guiIsUserDefinedMajorTickLengthChosen.IsChecked)
				_guiUserDefinedMajorTickLength.SelectedQuantityAsValueInPoints = tickLength;
		}

		private void EhSelectedFontFamilyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			FontChanged?.Invoke();
		}

		private void EhSelectedFontSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			FontChanged?.Invoke();
		}

		private void EhFontChoiceCheckedChanged(object sender, RoutedEventArgs e)
		{
			FontChanged?.Invoke();
		}

		public void SetReportOfOldValues(string report)
		{
			_guiOldSituation.Text = report;
		}

		public void SetReportOfDerivedValues(string report)
		{
			_guiValuesDerivedFromNewStandard.Text = report;
		}
	}
}