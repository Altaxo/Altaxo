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
	/// <summary>
	/// Interaction logic for AsciiImportOptionsControl.xaml
	/// </summary>
	public partial class AsciiImportOptionsControl : UserControl, IAsciiImportOptionsView
	{
		public event Action DoAnalyze;

		public AsciiImportOptionsControl()
		{
			InitializeComponent();
		}

		private void EhAnalyzeAscii(object sender, RoutedEventArgs e)
		{
			if (null != DoAnalyze)
				DoAnalyze();
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

		public int NumberOfMainHeaderLines
		{
			get
			{
				return _guiNumberOfMainHeaderLines.Value;
			}
			set
			{
				_guiNumberOfMainHeaderLines.Value = value;
			}
		}

		public int IndexOfCaptionLine
		{
			get
			{
				return _guiIndexOfCaptionLine.Value;
			}
			set
			{
				_guiIndexOfCaptionLine.Value = value;
			}
		}

		public void SetGuiSeparationStrategy(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiSeparationStrategy, list);
		}

		public int NumberOfDecimalSeparatorDots
		{
			set { _guiNumberOfDecimalSeparatorDots.Value = value; }
		}

		public int NumberOfDecimalSeparatorCommas
		{
			set { _guiNumberOfDecimalSeparatorCommas.Value = value; }
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

		
	}
}
