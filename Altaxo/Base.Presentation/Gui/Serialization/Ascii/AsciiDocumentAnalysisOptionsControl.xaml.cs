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
namespace Altaxo.Gui.Serialization.Ascii
{
	/// <summary>
	/// Interaction logic for AsciiDocumentAnalysisOptionsControl.xaml
	/// </summary>
	public partial class AsciiDocumentAnalysisOptionsControl : UserControl, IAsciiDocumentAnalysisOptionsView
	{
		public AsciiDocumentAnalysisOptionsControl()
		{
			InitializeComponent();
		}

		public int NumberOfLinesToAnalyze
		{
			get
			{
				return _guiNumberOfLinesToAnalyze.Value;
			}
			set
			{
				_guiNumberOfLinesToAnalyze.Value = value;
			}
		}


		public void SetNumberFormatsToAnalyze(SelectableListNodeList availableFormats, System.Collections.ObjectModel.ObservableCollection<Boxed<SelectableListNode>> currentlySelectedItems)
		{
			_guiNumberFormatsForAnalysisColumn.ItemsSource = null;
			_guiNumberFormatsForAnalysis.ItemsSource = null;
			_guiNumberFormatsForAnalysisColumn.ItemsSource = availableFormats;
			_guiNumberFormatsForAnalysis.ItemsSource = currentlySelectedItems;
		}


		public void SetDateTimeFormatsToAnalyze(SelectableListNodeList availableFormats, System.Collections.ObjectModel.ObservableCollection<Boxed<SelectableListNode>> currentlySelectedItems)
		{
			_guiDateTimeFormatsForAnalysis.ItemsSource = null;
			_guiDateTimeFormatsForAnalysisColumn.ItemsSource = null;
			_guiDateTimeFormatsForAnalysisColumn.ItemsSource = availableFormats;
			_guiDateTimeFormatsForAnalysis.ItemsSource = currentlySelectedItems;
		}
	}
}
