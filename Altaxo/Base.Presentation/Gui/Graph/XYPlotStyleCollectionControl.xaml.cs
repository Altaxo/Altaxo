using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotStyleCollectionControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYPlotStyleCollectionViewEventSink))]
	public partial class XYPlotStyleCollectionControl : UserControl, IXYPlotStyleCollectionView
	{
		public XYPlotStyleCollectionControl()
		{
			InitializeComponent();
		}


		private void EhPredefinedSets_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (_controller != null && null != _predefinedSetsAvailable.SelectedItem)
			{
				GuiHelper.SynchronizeSelectionFromGui(_predefinedSetsAvailable);
				_controller.EhView_PredefinedStyleSelected();
			}
		}

		private void EhSingleStylesAvailable_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (_controller != null && _singleStylesAvailable.SelectedItem != null)
			{
				GuiHelper.SynchronizeSelectionFromGui(_singleStylesAvailable);
				_controller.EhView_AddStyle();
			}
		}

		private void EhCurrentStyles_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleEdit();
			}
		}


		private void EhStyleUp_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleUp();
			}
		}

		private void EhStyleDown_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleDown();
			}
		}

		private void EhStyleRemove_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
			{
				_controller.EhView_StyleRemove();
			}
		}

		#region  IXYPlotStyleCollectionView

		IXYPlotStyleCollectionViewEventSink _controller;
		public IXYPlotStyleCollectionViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void InitializePredefinedStyles(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_predefinedSetsAvailable, list);
		}

		public void InitializeStyleList(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_lbStyles, list);
		}

		public void InitializeAvailableStyleList(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_singleStylesAvailable, list);
		}

		#endregion

	
	

		
	}
}
