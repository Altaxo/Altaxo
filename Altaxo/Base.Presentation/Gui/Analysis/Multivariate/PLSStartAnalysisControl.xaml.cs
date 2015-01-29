#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Calc.Regression.Multivariate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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

		#endregion IPLSStartAnalysisView
	}
}