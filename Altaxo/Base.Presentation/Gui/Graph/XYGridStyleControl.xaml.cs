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
	[UserControlForController(typeof(IXYGridStyleViewEventSink))]
	public partial class XYGridStyleControl : UserControl, IXYGridStyleView
	{
		public XYGridStyleControl()
		{
			InitializeComponent();
		}

		private void EhEnableChecked(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_ShowGridChanged(true==_chkEnable.IsChecked);
		}

		private void EhEnableUnchecked(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_ShowGridChanged(true == _chkEnable.IsChecked);
		}

		private void EhShowZeroOnlyChecked(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_ShowZeroOnly(true==_chkShowZeroOnly.IsChecked);
		}

		private void EhShowZeroOnlyUnchecked(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_ShowZeroOnly(true == _chkShowZeroOnly.IsChecked);
		}

		private void ShowMinorChecked(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_ShowMinorGridChanged(true==_chkShowMinor.IsChecked);
		}

		private void EhShowMinorUnchecked(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_ShowMinorGridChanged(true == _chkShowMinor.IsChecked);
		}

		#region IXYGridStyleView
		
			IXYGridStyleViewEventSink _controller;
		public IXYGridStyleViewEventSink Controller
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
