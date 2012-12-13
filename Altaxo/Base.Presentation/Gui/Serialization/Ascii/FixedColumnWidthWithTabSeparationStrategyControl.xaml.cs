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
	/// Interaction logic for FixedColumnWidthWithTabSeparationStrategyControl.xaml
	/// </summary>
	public partial class FixedColumnWidthWithTabSeparationStrategyControl : UserControl, IFixedColumnWidthWithTabSeparationStrategyView
	{
		public FixedColumnWidthWithTabSeparationStrategyControl()
		{
			InitializeComponent();
		}

		public int TabSize
		{
			get
			{
				return _guiTabSize.Value;
			}
			set
			{
				_guiTabSize.Value = value;
			}
		}

		public System.Collections.ObjectModel.ObservableCollection<Boxed<int>> StartPositions
		{
			set
			{
				_guiStartPositions.ItemsSource = null;
				_guiStartPositions.ItemsSource = value;
			}
		}
	}
}
