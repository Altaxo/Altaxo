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
	/// Interaction logic for PLSStartAnalysisControl.xaml
	/// </summary>
	public partial class PLSStartAnalysisControl : UserControl, IPLSStartAnalysisView
	{
		public PLSStartAnalysisControl()
		{
			InitializeComponent();
		}

		private void cbAnalysisMethod_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{

		}

		private void edMaxNumFactors_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (MaxNumberOfFactorsChanged != null)
				MaxNumberOfFactorsChanged(this.edMaxNumFactors.Value);
		}

		private void rbCrossValidationNone_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (CrossValidationSelected != null)
				CrossValidationSelected(CrossPRESSCalculationType.None);
		}

		private void rbCrossValidationEvery_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (CrossValidationSelected != null)
				CrossValidationSelected(CrossPRESSCalculationType.ExcludeEveryMeasurement);
		}

		private void rbCrossValidationGroups_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (CrossValidationSelected != null)
				CrossValidationSelected(CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements);
		}

		private void rbCrossValidationHalfEnsemble_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (CrossValidationSelected != null)
				CrossValidationSelected(CrossPRESSCalculationType.ExcludeHalfEnsemblyOfMeasurements);
		}

		#region IPLSStartAnalysisView

		public void InitializeNumberOfFactors(int numFactors)
		{
			edMaxNumFactors.Minimum = 1;
			edMaxNumFactors.Value = numFactors;
		}

		public void InitializeAnalysisMethod(string[] methods, int actMethod)
		{
			cbAnalysisMethod.Items.Clear();
			cbAnalysisMethod.ItemsSource = methods;
			cbAnalysisMethod.SelectedIndex = actMethod;
		}

		public void InitializeCrossPressCalculation(CrossPRESSCalculationType val)
		{
			switch (val)
			{
				case CrossPRESSCalculationType.None:
					rbCrossValidationNone.IsChecked = true;
					break;
				case CrossPRESSCalculationType.ExcludeEveryMeasurement:
					rbCrossValidationEvery.IsChecked = true;
					break;
				case CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements:
					rbCrossValidationGroups.IsChecked = true;
					break;
				case CrossPRESSCalculationType.ExcludeHalfEnsemblyOfMeasurements:
					this.rbCrossValidationHalfEnsemble.IsChecked = true;
					break;
			}
		}

		public event Action<int> MaxNumberOfFactorsChanged;

		public event Action<Calc.Regression.Multivariate.CrossPRESSCalculationType> CrossValidationSelected;

		public event Action<int> AnalysisMethodChanged;

		#endregion
	}
}
