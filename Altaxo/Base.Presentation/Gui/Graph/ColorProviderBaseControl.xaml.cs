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

using sd = System.Drawing;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Graph;

	/// <summary>
	/// Interaction logic for ColorProviderBaseControl.xaml
	/// </summary>
	public partial class ColorProviderBaseControl : UserControl, IColorProviderBaseView
	{
		public ColorProviderBaseControl()
		{
			InitializeComponent();
		}

		protected virtual void OnChoiceChanged()
		{
			if (null != ChoiceChanged)
				ChoiceChanged();
		}


		#region IColorProviderBaseView Members


		public event Action ChoiceChanged;

		public NamedColor ColorBelow
		{
			get
			{
				return _cbColorBelow.SelectedColor;
			}
			set
			{
				_cbColorBelow.SelectedColor = value;
			}
		}

		public NamedColor ColorAbove
		{
			get
			{
				return _cbColorAbove.SelectedColor;
			}
			set
			{
				_cbColorAbove.SelectedColor = value;
			}
		}

		public NamedColor ColorInvalid
		{
			get
			{
				return _cbInvalid.SelectedColor;
			}
			set
			{
				_cbInvalid.SelectedColor = value;
			}
		}

		public double Transparency
		{
			get
			{
				return (double)(_edTransparency.Value / 100);
			}
			set
			{
				_edTransparency.Value = (decimal)(value * 100);
			}
		}

		public int ColorSteps
		{
			get
			{
				return _edColorSteps.Value; ;
			}
			set
			{
				_edColorSteps.Value = value;
			}
		}

		#endregion

		private void EhColorBelowChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OnChoiceChanged();
		}

		private void EhColorAboveChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OnChoiceChanged();
		}

		private void EhColorInvalidChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OnChoiceChanged();
		}

		private void EhTransparencyChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			OnChoiceChanged();
		}

		private void EhColorStepsChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			OnChoiceChanged();
		}

	}
}
