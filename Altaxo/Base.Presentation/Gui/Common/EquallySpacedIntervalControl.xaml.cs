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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for EquallySpacedIntervalControl.xaml
	/// </summary>
	public partial class EquallySpacedIntervalControl : UserControl, IEquallySpacedIntervalView
	{
		public EquallySpacedIntervalControl()
		{
			InitializeComponent();
		}

		private void _rbStartEndCount_CheckedChanged(object sender, RoutedEventArgs e)
		{
			EquallySpacedIntervalSpecificationMethod method = EquallySpacedIntervalSpecificationMethod.StartEndCount;
			if (true==_rbEndCountIntv.IsChecked)
				method = EquallySpacedIntervalSpecificationMethod.EndCountInterval;
			else if (true == _rbStartEndCount.IsChecked)
				method = EquallySpacedIntervalSpecificationMethod.StartEndCount;
			else if (true == _rbStartCountInterval.IsChecked)
				method = EquallySpacedIntervalSpecificationMethod.StartCountInterval;
			else if (true == _rbStartEndIntv.IsChecked)
				method = EquallySpacedIntervalSpecificationMethod.StartEndInterval;

			if (null != MethodChanged)
				MethodChanged(method);
		}

		private void _edStart_TextChanged(object sender, ValidationEventArgs<string> e)
		{
			if (null != StartChanged)
				StartChanged(_edStart.Text);
		}

		private void _edEnd_TextChanged(object sender, ValidationEventArgs<string> e)
		{
			if (null != EndChanged)
				EndChanged(_edEnd.Text);
		}

		private void _edIntv_TextChanged(object sender, ValidationEventArgs<string> e)
		{
			if (null != IntervalChanged)
				IntervalChanged(_edIntv.Text);
		}

		private void _edCount_TextChanged(object sender, ValidationEventArgs<string> e)
		{
			if (null != CountChanged)
				CountChanged(_edCount.Text);
		}

		#region IEquallySpacedIntervalView

		public event Action<EquallySpacedIntervalSpecificationMethod> MethodChanged;

		public event Action<string> StartChanged;

		public event Action<string> EndChanged;

		public event Action<string> CountChanged;

		public event Action<string> IntervalChanged;

		public event Action<System.ComponentModel.CancelEventArgs> CountValidating;

		public event Action<System.ComponentModel.CancelEventArgs> IntervalValidating;

		public void EnableEditBoxes(bool start, bool end, bool count, bool interval)
		{
			_edStart.IsEnabled = start;
			_edEnd.IsEnabled = end;
			_edCount.IsEnabled = count;
			_edIntv.IsEnabled = interval;
		}

		public void InitializeMethod(EquallySpacedIntervalSpecificationMethod method)
		{
			switch (method)
			{
				case EquallySpacedIntervalSpecificationMethod.StartEndCount:
					_rbStartEndCount.IsChecked = true;
					break;
				case EquallySpacedIntervalSpecificationMethod.StartCountInterval:
					_rbStartCountInterval.IsChecked = true;
					break;
				case EquallySpacedIntervalSpecificationMethod.EndCountInterval:
					_rbEndCountIntv.IsChecked = true;
					break;
				case EquallySpacedIntervalSpecificationMethod.StartEndInterval:
					_rbStartEndIntv.IsChecked = true;
					break;
				default:
					throw new ArgumentException("method unknown");
			}
		}

		public void InitializeStart(string text)
		{
			_edStart.Text = text;
		}

		public void InitializeEnd(string text)
		{
			_edEnd.Text = text;
		}

		public void InitializeCount(string text)
		{
			_edCount.Text = text;
		}

		public void InitializeInterval(string text)
		{
			_edIntv.Text = text;
		}

		#endregion
	}
}
