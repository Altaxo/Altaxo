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

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for PlotCommonColumnsControl.xaml
	/// </summary>
	public partial class PlotCommonColumnsControl : UserControl, IPlotCommonColumnsView
	{
		public PlotCommonColumnsControl()
		{
			InitializeComponent();
		}

		public bool UseCurrentXColumn
		{
			get
			{
				return true == _guiUseXColumnCurrent.IsChecked;
			}
			set
			{
				_guiUseXColumnCurrent.IsChecked = value;
				_guiUseXColumnUserDefined.IsChecked = !value;
				_guiCommonXColumn.IsEnabled = !value;
			}
		}

		public void InitializeXCommonColumns(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiCommonXColumn, list);
		}
		public void InitializeYCommonColumns(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiCommonYColumns, list);
		}

		private void EhUseCurrentXColumnChecked(object sender, RoutedEventArgs e)
		{
			_guiCommonXColumn.IsEnabled = false;
		}

		private void EhUseUserDefinedXColumnChecked(object sender, RoutedEventArgs e)
		{
			_guiCommonXColumn.IsEnabled = true;
		}

		private void EhXColumnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiCommonXColumn);
		}

		private void EhYColumnsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiCommonYColumns);
		}
	}
}
