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

namespace Altaxo.Gui.Serialization.Ascii
{
	using Altaxo.Collections;

	/// <summary>
	/// Interaction logic for AsciiImportOptionsControl.xaml
	/// </summary>
	public partial class AsciiImportOptionsControl : UserControl, IAsciiImportOptionsView
	{
		public event Action DoAnalyze;
		public event Action SeparationStrategyChanged;

		public AsciiImportOptionsControl()
		{
			InitializeComponent();
		}

		private void EhAnalyzeAscii(object sender, RoutedEventArgs e)
		{
			if (null != DoAnalyze)
				DoAnalyze();
		}

	

		public int? NumberOfMainHeaderLines
		{
			get
			{
				if (true == _guiKnownNumberOfHeaderLines.IsChecked)
					return _guiNumberOfHeaderLines.Value;
				else
					return null;
			}
			set
			{
				_guiKnownNumberOfHeaderLines.IsChecked = value.HasValue;
				if (value.HasValue)
					_guiNumberOfHeaderLines.Value = value.Value;
				else
					_guiNumberOfHeaderLines.Value = 0;
			}
		}

		public int? IndexOfCaptionLine
		{
			get
			{
				if (true == _guiKnownIndexOfCaptionLine.IsChecked)
					return _guiIndexOfCaptionLine.Value - 1; // for the Gui, we have a 1 based index
				else
					return null;
			}
			set
			{
				_guiKnownIndexOfCaptionLine.IsChecked = value.HasValue;
				if (value.HasValue)
					_guiIndexOfCaptionLine.Value = value.Value + 1; // for the Gui, we have a 1 based index
				else
					_guiIndexOfCaptionLine.Value = 0;
			}
		}

		public void SetGuiSeparationStrategy(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiSeparationStrategy, list);
		}

	

		public bool RenameColumnsWithHeaderNames
		{
			get
			{
				return _guiRenameColumnsWithHeaderNames.IsChecked == true;
			}
			set
			{
				_guiRenameColumnsWithHeaderNames.IsChecked = value;
			}
		}

		public bool RenameWorksheetWithFileName
		{
			get
			{
				return _guiRenameWorksheetWithFileName.IsChecked == true;
			}
			set
			{
				_guiRenameWorksheetWithFileName.IsChecked = value;
			}
		}

		public SelectableListNodeList HeaderLinesDestination
		{
			set
			{
				_guiHeaderLinesDestination.Initialize(value);
			}
		}

		public object AsciiSeparationStrategyDetailView
		{
			set { _guiSeparationStrategyDetailsHost.Content = value; }
		}


		private void EhSeparationStrategyChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiSeparationStrategy);
			if (null != SeparationStrategyChanged)
				SeparationStrategyChanged();
		}


		public bool GuiSeparationStrategyIsKnown
		{
			get
			{
				return true == _guiKnownSeparationStrategy.IsChecked;
			}
			set
			{
				_guiKnownSeparationStrategy.IsChecked = value;
			}
		}

		public void SetNumberFormatCulture(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiNumberFormats, list);
		}

		private void EhNumberFormatChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiNumberFormats);
		}

		public bool NumberFormatCultureIsKnowm
		{
			get
			{
				return true == _guiKnownNumberFormat.IsChecked;
			}
			set
			{
				_guiKnownNumberFormat.IsChecked = value;
			}
		}

		public void SetDateTimeFormatCulture(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiDateTimeFormats, list);
		}

		private void EhDateTimeFormatChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiDateTimeFormats);
		}

		public bool DateTimeFormatCultureIsKnown
		{
			get
			{
				return true == _guiKnownDateTimeFormat.IsChecked;
			}
			set
			{
				_guiKnownDateTimeFormat.IsChecked = value;
			}
		}








		public bool TableStructureIsKnown
		{
			get
			{
				return true == _guiKnownTableColumns.IsChecked;
			}
			set
			{
				_guiKnownTableColumns.IsChecked = value;
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Boxed<Altaxo.Serialization.Ascii.AsciiColumnType>> TableStructure
		{
			set 
			{
				_guiColumnTypes.ItemsSource = null;
				_guiColumnTypes.ItemsSource = value;
			}
		}




	

		public object AsciiDocumentAnalysisOptionsView
		{
			get { return _guiAnalysisControl; }
		}
	}
}
