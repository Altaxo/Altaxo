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
#endregion

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

using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Data;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	/// <summary>
	/// Interaction logic for FitElementControl.xaml
	/// </summary>
	[UserControlForController(typeof(IFitElementViewEventSink))]
	public partial class FitElementControl : UserControl, IFitElementView
	{
		#region Members

		const int idxVariablesColumn = 0;
		const int idxErrorFuncColumn = 1;
		const int idxRectLeftColumn = 2;
		const int idxVariablesNameColumn = 3;
		const int idxParameterNameColumn = 4;
		const int idxParameterSymbolColumn = 5; // and 6
		const int idxParameterColumn = 7;


		FitElement _fitElement;
		int _numberOfX;
		int _numberOfY;
		int _numberOfParameter;
		int _totalSlots;

		/// <summary>Width of the IVarianceScaling box.</summary>
		int _errorFunctionWidth;
		/// <summary>X position of the IVarianceScaling box.</summary>
		int _errorFunctionX;

		/// <summary>X coordinate of the left edge of the external parameters boxes.</summary>
		int _externalParametersX;
		/// <summary>With of the external parameters boxes.</summary>
		int _externalParametersWidth;

		int _VariablesX = 0;
		int _DependentVariablesWidth;
		int _DependentVariablesY;
		int _IndependentVariablesWidth;
		bool _fitFunctionSelected;

		#endregion

		public FitElementControl()
		{
			InitializeComponent();
		}


		#region Work

		private void SetupElements()
		{
			var leftButtonMargin = new Thickness(4, 2, 0, 2);
			var rightButtonMargin = new Thickness(0, 2, 4, 2);
			var standardLeftInnerLabelMargin = new Thickness(2, 0, 0, 0);
			var standardRightInnerLabelMargin = new Thickness(0, 0, 2, 0);
			double circleDiameter = FontSize;
			double circleThickness = 1;


			_grid.Children.Clear();
			_grid.RowDefinitions.Clear();
			for (int i = 0; i < _totalSlots; ++i)
			{
				_grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			}

			var colW = new GridLength(circleDiameter / 2);
			_colL1.Width = colW;
			_colL2.Width = colW;
			_colR1.Width = colW;
			_colR2.Width = colW;


			// Button with fit function name
			{
				var item = new Button();
				item.SetValue(Grid.ColumnProperty, idxRectLeftColumn);
				item.SetValue(Grid.RowProperty, 0);
				item.SetValue(Grid.ColumnSpanProperty, 4);
				item.SetValue(Grid.RowSpanProperty, _totalSlots);

				string fitFuncName = null;
				if (_fitElement.FitFunction == null)
					fitFuncName = "?";
				else if (_fitElement.FitFunction is Altaxo.Scripting.IFitFunctionScriptText)
					fitFuncName = (_fitElement.FitFunction as Altaxo.Scripting.IFitFunctionScriptText).FitFunctionName;
				else
					fitFuncName = _fitElement.FitFunction.ToString();

				item.Content = fitFuncName;
				item.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
				item.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
				item.Click += new RoutedEventHandler(EhClickOnFitFunction);
				_grid.Children.Add(item);
			}

			// internal independent variable names
			for (int i = 0; i < _numberOfX; ++i)
			{
				string name = null != _fitElement.FitFunction && i < _fitElement.FitFunction.NumberOfIndependentVariables ? _fitElement.FitFunction.IndependentVariableName(i) : string.Empty;
				var item = new TextBlock() { Text = name };
				item.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				item.VerticalAlignment = System.Windows.VerticalAlignment.Center;
				item.Margin = standardLeftInnerLabelMargin;
				item.SetValue(Grid.ColumnProperty, idxVariablesNameColumn);
				item.SetValue(Grid.RowProperty, i);
				_grid.Children.Add(item);

				var circle = new Ellipse()
				{
					Stroke = Brushes.Blue,
					StrokeThickness = circleThickness,
					Width = circleDiameter,
					Height = circleDiameter,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				circle.SetValue(Grid.ColumnProperty, 1);
				circle.SetValue(Grid.ColumnSpanProperty, 2);
				circle.SetValue(Grid.RowProperty, i);
				_grid.Children.Add(circle);
			}

			// external independent variable names
			for (int i = 0; i < _numberOfX; ++i)
			{
				var item = new Button()
				{
					Content = GetTextShownForIndependentVariable(i),
					Margin = leftButtonMargin,
					Tag = i
				};
				item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
				item.SetValue(Grid.RowProperty, i);
				item.Click += new RoutedEventHandler(EhClickOnIndependentVariable);
				_grid.Children.Add(item);
			}

			// plot range
			{
				var item = new Button()
				{
					Content = GetTextShownForPlotRange(),
					Margin = leftButtonMargin
				};
				item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
				item.SetValue(Grid.RowProperty, _numberOfX);
				item.Click += new RoutedEventHandler(EhClickOnPlotRange);
				_grid.Children.Add(item);
			}

			// internal dependent variable names
			for (int i = 0; i < _numberOfY; ++i)
			{
				var item = new TextBlock()
				{
					Text = null != _fitElement.FitFunction && i < _fitElement.FitFunction.NumberOfDependentVariables ? _fitElement.FitFunction.DependentVariableName(i) : string.Empty,
					Margin = standardLeftInnerLabelMargin,
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center
				};
				item.SetValue(Grid.ColumnProperty, idxVariablesNameColumn);
				item.SetValue(Grid.RowProperty, _totalSlots - _numberOfY + i);
				_grid.Children.Add(item);

				var circle = new Ellipse()
				{
					Stroke = Brushes.Blue,
					StrokeThickness = circleThickness,
					Width = circleDiameter,
					Height = circleDiameter,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				circle.SetValue(Grid.ColumnProperty, 1);
				circle.SetValue(Grid.ColumnSpanProperty, 2);
				circle.SetValue(Grid.RowProperty, _totalSlots - _numberOfY + i);
				_grid.Children.Add(circle);
			}

			// external dependent variable names
			for (int i = 0; i < _numberOfY; ++i)
			{
				var panel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
				panel.SetValue(Grid.ColumnProperty, idxVariablesColumn);
				panel.SetValue(Grid.ColumnSpanProperty, 1);
				panel.SetValue(Grid.RowProperty, _totalSlots - _numberOfY + i);

				var item = new Button()
				{
					Content = GetTextShownForDependentVariable(i),
					Margin = leftButtonMargin,
					Tag = i
				};
				item.Click += new RoutedEventHandler(EhClickOnDependentVariable);
				item.ContextMenu = new ContextMenu();
				var menuItem = new MenuItem { Header = "Remove", Tag = item };
				menuItem.Click += EhRemoveDependentVariable;
				item.ContextMenu.Items.Add(menuItem);
				panel.Children.Add(item);

				item = new Button()
				{
					Content = GetTextShownForErrorEvaluation(i),
					Padding = new Thickness(0),
					Margin = leftButtonMargin,
					Tag = i
				};
				item.Click += new RoutedEventHandler(EhClickOnErrorFunction);
				panel.Children.Add(item);

				_grid.Children.Add(panel);
			}

			// internal parameter names
			for (int i = 0; i < _numberOfParameter; ++i)
			{
				string name = null != _fitElement.FitFunction && i < _fitElement.FitFunction.NumberOfParameters ? _fitElement.FitFunction.ParameterName(i) : string.Empty;
				var item = new TextBlock()
				{
					Text = name,
					HorizontalAlignment = HorizontalAlignment.Right,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = standardRightInnerLabelMargin
				};
				item.SetValue(Grid.ColumnProperty, idxParameterNameColumn);
				item.SetValue(Grid.RowProperty, i);
				_grid.Children.Add(item);

				var circle = new Ellipse()
				{
					Stroke = Brushes.DarkTurquoise,
					StrokeThickness = circleThickness,
					Width = circleDiameter,
					Height = circleDiameter,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
				};
				circle.SetValue(Grid.ColumnProperty, idxParameterSymbolColumn);
				circle.SetValue(Grid.ColumnSpanProperty, 2);
				circle.SetValue(Grid.RowProperty, i);
				_grid.Children.Add(circle);
			}

			// external parameters
			for (int i = 0; i < _numberOfParameter; ++i)
			{
				var item = new Button()
				{
					Content = GetTextShownForParameter(i),
					Margin = rightButtonMargin,
					Tag = i
				};
				item.SetValue(Grid.ColumnProperty, idxParameterColumn);
				item.SetValue(Grid.RowProperty, i);
				item.Click += new RoutedEventHandler(EhClickOnParameter);
				_grid.Children.Add(item);
			}
		}

		private string GetTextShownForParameter(int i)
		{
			return _fitElement.ParameterName(i);
		}

		private string GetTextShownForErrorEvaluation(int i)
		{
			string name = _fitElement.GetErrorEvaluation(i) != null ? _fitElement.GetErrorEvaluation(i).ShortName : string.Empty;
			return name;
		}

		private string GetTextShownForDependentVariable(int i)
		{
			INumericColumn col = _fitElement.DependentVariables(i);
			string name = col != null ? col.FullName : "Choose a column ..";
			return name;
		}

		private string GetTextShownForPlotRange()
		{
			var plotRange = _fitElement.GetRowRange();
			string rangestring = "Range: " + plotRange.Start.ToString() + " to " + (plotRange.IsInfinite ? "infinity" : plotRange.Last.ToString());
			return rangestring;
		}

		private string GetTextShownForIndependentVariable(int i)
		{
			INumericColumn col = _fitElement.IndependentVariables(i);
			string name = col != null ? col.FullName : "Choose a column ..";
			return name;
		}

		void EhClickOnFitFunction(object sender, RoutedEventArgs e)
		{
			_controller.EhView_ChooseFitFunction();
			_controller.EhView_EditFitFunction();
		}

		void EhClickOnPlotRange(object sender, RoutedEventArgs e)
		{
			_controller.EhView_ChooseFitRange();

			((ContentControl)sender).Content = GetTextShownForPlotRange();
		}

		void EhClickOnParameter(object sender, RoutedEventArgs e)
		{
			int idx = (int)((FrameworkElement)sender).Tag;
			_controller.EhView_ChooseExternalParameter(idx);
			((ContentControl)sender).Content = GetTextShownForParameter(idx);
		}

		void EhClickOnErrorFunction(object sender, RoutedEventArgs e)
		{
			int idx = (int)((FrameworkElement)sender).Tag;
			_controller.EhView_ChooseErrorFunction(idx);
			((ContentControl)sender).Content = GetTextShownForErrorEvaluation(idx);
		}

		void EhClickOnDependentVariable(object sender, RoutedEventArgs e)
		{
			int idx = (int)((FrameworkElement)sender).Tag;
			_controller.EhView_ChooseDependentColumn(idx);
			((ContentControl)sender).Content = GetTextShownForDependentVariable(idx);
		}

		void EhRemoveDependentVariable(object sender, RoutedEventArgs e)
		{
			var fwe = (FrameworkElement)((FrameworkElement)sender).Tag; // the menuItem has the corresponding button as tag
			int idx = (int)(fwe.Tag); // the button's tag contains the index
			_controller.EhView_DeleteDependentVariable(idx);
			((ContentControl)fwe).Content = GetTextShownForDependentVariable(idx);
		}

		void EhClickOnIndependentVariable(object sender, RoutedEventArgs e)
		{
			int idx = (int)((FrameworkElement)sender).Tag;
			_controller.EhView_ChooseIndependentColumn(idx);
			((ContentControl)sender).Content = GetTextShownForIndependentVariable(idx);
		}


		#endregion


		#region IFitElementView

		IFitElementViewEventSink _controller;
		public IFitElementViewEventSink Controller
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

		public void Initialize(Calc.Regression.Nonlinear.FitElement fitElement)
		{
			_fitElement = fitElement;

			_numberOfX = _fitElement.NumberOfIndependentVariables;
			_numberOfY = _fitElement.NumberOfDependentVariables;
			_numberOfParameter = _fitElement.NumberOfParameters;

			_totalSlots = Math.Max(_numberOfParameter, _numberOfX + _numberOfY + 1);
			SetupElements();
		}

		public void Refresh()
		{

		}

		public bool FitFunctionSelected
		{
			set
			{
				_fitFunctionSelected = value;
				//this.Invalidate();
			}
		}

		#endregion
	}
}
