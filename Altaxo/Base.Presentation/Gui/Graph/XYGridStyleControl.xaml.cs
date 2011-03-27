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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYGridStyleControl.xaml
	/// </summary>
	public partial class XYGridStyleControl : UserControl, IXYGridStyleView
	{
		NotifyChangedValue<bool> _isGridStyleEnabled = new NotifyChangedValue<bool>();
		NotifyChangedValue<string> _headerTitle = new NotifyChangedValue<string>();

		public XYGridStyleControl()
		{
			InitializeComponent();
		}

		private void EhEnableCheckChanged(object sender, RoutedEventArgs e)
		{
			if(null!=ShowGridChanged)
				ShowGridChanged(true==_chkEnable.IsChecked);

		}

		

		private void EhShowZeroOnlyCheckChanged(object sender, RoutedEventArgs e)
		{
			if (null != ShowZeroOnlyChanged)
				ShowZeroOnlyChanged(true==_chkShowZeroOnly.IsChecked);
		}

		

		private void EhShowMinorCheckChanged(object sender, RoutedEventArgs e)
		{
			if(null!=ShowMinorGridChanged)
				ShowMinorGridChanged(true==_chkShowMinor.IsChecked);
		}

	

		#region IXYGridStyleView
		
		public event Action<bool> ShowGridChanged;
		public event Action<bool> ShowMinorGridChanged;
		public event Action<bool> ShowZeroOnlyChanged;
		

		public void InitializeBegin()
		{
			
		}

		public void InitializeEnd()
		{
			
		}

		public void InitializeMajorGridStyle(Common.Drawing.IColorTypeThicknessPenController controller)
		{
			controller.ViewObject = this._majorStyle;
		}

		public void InitializeMinorGridStyle(Common.Drawing.IColorTypeThicknessPenController controller)
		{
			controller.ViewObject = this._minorStyle;
		}

		public void InitializeShowGrid(bool value)
		{
			this._chkEnable.IsChecked = value;
		}

		public void InitializeShowMinorGrid(bool value)
		{
			this._chkShowMinor.IsChecked = value;
		}

		public void InitializeShowZeroOnly(bool value)
		{
			this._chkShowZeroOnly.IsChecked = value;
		}

		public void InitializeElementEnabling(bool majorstyle, bool minorstyle, bool showminor, bool showzeroonly)
		{
			this._majorStyle.IsEnabled = majorstyle;
			this._minorStyle.IsEnabled = minorstyle;
			this._chkShowMinor.IsEnabled = showminor;
			this._chkShowZeroOnly.IsEnabled = showzeroonly;
		}

		#endregion
	}
}
