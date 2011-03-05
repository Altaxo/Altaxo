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

using Altaxo.Calc.Regression.Multivariate;

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for SpectralPreprocessingControl.xaml
	/// </summary>
	public partial class SpectralPreprocessingControl : UserControl, ISpectralPreprocessingView
	{
		public SpectralPreprocessingControl()
		{
			InitializeComponent();
		}

		private void _rbMethodNone_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (MethodChanged != null)
				MethodChanged(SpectralPreprocessingMethod.None);
		}

		private void _rbMethodMSC_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (MethodChanged != null)
				MethodChanged(SpectralPreprocessingMethod.MultiplicativeScatteringCorrection);
		}

		private void _rbMethodSNV_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (MethodChanged != null)
				MethodChanged(SpectralPreprocessingMethod.StandardNormalVariate);
		}

		private void _rbMethod1stDer_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (MethodChanged != null)
				MethodChanged(SpectralPreprocessingMethod.FirstDerivative);
		}

		private void _rbMethod2ndDer_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (MethodChanged != null)
				MethodChanged(SpectralPreprocessingMethod.SecondDerivative);
		}

		private void _rbDetrendingNone_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (DetrendingChanged != null)
				DetrendingChanged(-1);
		}

		private void _rbDetrendingZero_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (DetrendingChanged != null )
				DetrendingChanged(0);
		}

		private void _rbDetrending1st_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (DetrendingChanged != null)
				DetrendingChanged(1);
		}

		private void _rbDetrending2nd_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (DetrendingChanged != null)
				DetrendingChanged(2);
		}

		private void _chkEnsembleScale_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (EnsembleScaleChanged != null)
				EnsembleScaleChanged(true==_chkEnsembleScale.IsChecked);
		}

	


		#region  ISpectralPreprocessingView

		public void InitializeMethod(SpectralPreprocessingMethod method)
		{
			switch (method)
			{
				case SpectralPreprocessingMethod.None:
					this._rbMethodNone.IsChecked = true;
					break;
				case SpectralPreprocessingMethod.MultiplicativeScatteringCorrection:
					this._rbMethodMSC.IsChecked = true;
					break;
				case SpectralPreprocessingMethod.StandardNormalVariate:
					this._rbMethodSNV.IsChecked = true;
					break;
				case SpectralPreprocessingMethod.FirstDerivative:
					this._rbMethod1stDer.IsChecked = true;
					break;
				case SpectralPreprocessingMethod.SecondDerivative:
					this._rbMethod2ndDer.IsChecked = true;
					break;
			}
		}

		public void InitializeDetrending(int detrending)
		{
			switch (detrending)
			{
				case 0:
					this._rbDetrendingZero.IsChecked = true;
					break;
				case 1:
					this._rbDetrending1st.IsChecked = true;
					break;
				case 2:
					this._rbDetrending2nd.IsChecked = true;
					break;
				default:
					this._rbDetrendingNone.IsChecked = true;
					break;
			}
		}

		public void InitializeEnsembleScale(bool ensScale)
		{
			this._chkEnsembleScale.IsChecked = ensScale;
		}

		public event Action<SpectralPreprocessingMethod> MethodChanged;

		public event Action<int> DetrendingChanged;

		public event Action<bool> EnsembleScaleChanged;

		#endregion
	}
}
