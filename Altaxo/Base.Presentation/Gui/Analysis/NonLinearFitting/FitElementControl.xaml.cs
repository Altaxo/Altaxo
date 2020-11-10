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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Data;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Interaction logic for FitElementControl.xaml
  /// </summary>
  public partial class FitElementControl : UserControl, IFitElementView
  {
    #region events to controller

    public event Action<int>? ChooseErrorFunction;

    public event Action? ChooseFitFunction;

    public event Action<int>? ChooseExternalParameter;

    public event Action? SetupVariablesAndRange;

    public event Action? EditFitFunction;

    public event Action? DeleteThisFitElement;

    #endregion events to controller

    #region Members

    private const int idxVariablesColumn = 0;

    /// <summary>In this column and the next are located the blue circles and the buttons to choose the error scaling</summary>
    private const int idxErrorFuncColumn = 1;

    private const int idxRectLeftColumn = 2;

    /// <summary>Holds the variable names on the fit function side. Spans to the middle of the fit function button.</summary>
    private const int idxVariablesNameColumn = 3;

    /// <summary>Holds the paramenter names. Spans from the middle of the fit function button.</summary>
    private const int idxParameterNameColumn = 4;

    /// <summary>This column and the next holds the blue circles on the parameter side.</summary>
    private const int idxParameterSymbolColumn = 5; // and 6

    /// <summary>Holds the parameter names.</summary>
    private const int idxParameterColumn = 7;

    private FitElement _fitElement;
    private int _numberOfX;
    private int _numberOfY;
    private int _numberOfParameter;
    private int _totalSlots;
    private bool _fitFunctionSelected;

    #endregion Members

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
      _colL1.MinWidth = circleDiameter / 2;
      _colL2.MinWidth = circleDiameter / 2;
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
        if (_fitElement.FitFunction is null)
          fitFuncName = "?";
        else if (_fitElement.FitFunction is Altaxo.Scripting.IFitFunctionScriptText ffct)
          fitFuncName = string.Format("{0} (created {1})", ffct.FitFunctionName, ffct.CreationTime.ToString("yyyy-dd-MM HH:mm:ss"));
        else
          fitFuncName = _fitElement.FitFunction.ToString();

        item.Content = fitFuncName;
        item.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
        item.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        item.Click += new RoutedEventHandler(EhClickOnFitFunction);

        // context menu for the button
        var menuItem = new MenuItem() { Header = "Delete this fit element" };
        menuItem.Click += EhDeleteFitElement;
        var contextMenu = new ContextMenu();
        contextMenu.Items.Add(menuItem);
        item.ContextMenu = contextMenu;
        _grid.Children.Add(item);
      }

      // internal independent variable names
      for (int i = 0; i < _numberOfX; ++i)
      {
        string name = _fitElement.FitFunction is not null && i < _fitElement.FitFunction.NumberOfIndependentVariables ? _fitElement.FitFunction.IndependentVariableName(i) : string.Empty;
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
        var item = new Label()
        {
          Content = GetTextShownForIndependentVariable(i),
          Margin = leftButtonMargin,
          Tag = i,
          HorizontalAlignment = HorizontalAlignment.Right
        };
        item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
        item.SetValue(Grid.RowProperty, i);
        _grid.Children.Add(item);
      }

      // Background for table, group number, Row selection and Button

      {
        var item = new Border() { Background = Brushes.Bisque };
        item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
        item.SetValue(Grid.RowProperty, _numberOfX);
        item.SetValue(Grid.RowSpanProperty, 4);
        _grid.Children.Add(item);
      }

      // Table name
      {
        var item = new Label()
        {
          Content = "Table: " + (_fitElement?.DataTable?.Name ?? "??Unknown??"),
          Margin = leftButtonMargin
        };
        item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
        item.SetValue(Grid.RowProperty, _numberOfX);
        _grid.Children.Add(item);
      }
      // Group Number
      {
        var item = new Label()
        {
          Content = "GroupNumber: " + (_fitElement?.GroupNumber.ToString() ?? "??Unknown??"),
          Margin = leftButtonMargin
        };
        item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
        item.SetValue(Grid.RowProperty, _numberOfX + 1);
        _grid.Children.Add(item);
      }
      // plot range
      {
        var item = new Label()
        {
          Content = GetTextShownForPlotRange(),
          Margin = leftButtonMargin
        };
        item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
        item.SetValue(Grid.RowProperty, _numberOfX + 2);
        _grid.Children.Add(item);
      }

      // setup button
      {
        var item = new Button()
        {
          Content = "Setup variables/range",
          Margin = new Thickness(4)
        };
        item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
        item.SetValue(Grid.RowProperty, _numberOfX + 3);
        item.Click += new RoutedEventHandler(EhSetupVariablesAndRange);
        _grid.Children.Add(item);
      }

      // internal dependent variable names
      for (int i = 0; i < _numberOfY; ++i)
      {
        string name = _fitElement.FitFunction is not null && i < _fitElement.FitFunction.NumberOfDependentVariables ? _fitElement.FitFunction.DependentVariableName(i) : string.Empty;
        var item = new TextBlock() { Text = name };
        item.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        item.VerticalAlignment = System.Windows.VerticalAlignment.Center;
        item.Margin = standardLeftInnerLabelMargin;
        item.SetValue(Grid.ColumnProperty, idxVariablesNameColumn);
        item.SetValue(Grid.RowProperty, _totalSlots - _numberOfY + i);
        _grid.Children.Add(item);

        var errorScaleButton = new Button()
        {
          Content = GetTextShownForErrorEvaluation(i),
          Padding = new Thickness(0),
          HorizontalAlignment = HorizontalAlignment.Center,
          VerticalAlignment = VerticalAlignment.Center,
          Tag = i
        };
        errorScaleButton.Click += new RoutedEventHandler(EhClickOnErrorFunction);
        errorScaleButton.SetValue(Grid.ColumnProperty, 1);
        errorScaleButton.SetValue(Grid.ColumnSpanProperty, 2);
        errorScaleButton.SetValue(Grid.RowProperty, _totalSlots - _numberOfY + i);
        _grid.Children.Add(errorScaleButton);
      }

      // external dependent variable names
      for (int i = 0; i < _numberOfY; ++i)
      {
        var item = new Label()
        {
          Content = GetTextShownForDependentVariable(i),
          Margin = leftButtonMargin,
          Tag = i,
          HorizontalAlignment = HorizontalAlignment.Right
        };
        item.SetValue(Grid.ColumnProperty, idxVariablesColumn);
        item.SetValue(Grid.RowProperty, _totalSlots - _numberOfY + i);
        _grid.Children.Add(item);
      }

      // internal parameter names
      for (int i = 0; i < _numberOfParameter; ++i)
      {
        string name = _fitElement.FitFunction is not null && i < _fitElement.FitFunction.NumberOfParameters ? _fitElement.FitFunction.ParameterName(i) : string.Empty;
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

    private string GetTextShownForIndependentVariable(int i)
    {
      var col = _fitElement.IndependentVariables(i);
      var colColl = DataColumnCollection.GetParentDataColumnCollectionOf(col as DataColumn);
      return colColl?.GetColumnName(col as DataColumn) ?? col?.FullName ?? "??Unassigned??";
    }

    private string GetTextShownForDependentVariable(int i)
    {
      var col = _fitElement.DependentVariables(i);
      var colColl = DataColumnCollection.GetParentDataColumnCollectionOf(col as DataColumn);
      return colColl?.GetColumnName(col as DataColumn) ?? col?.FullName ?? "??Unassigned??";
    }

    private string GetTextShownForParameter(int i)
    {
      return _fitElement.ParameterName(i);
    }

    private string GetTextShownForErrorEvaluation(int i)
    {
      string name = _fitElement.GetErrorEvaluation(i) is not null ? _fitElement.GetErrorEvaluation(i).ShortName : string.Empty;
      return name;
    }

    private string GetTextShownForPlotRange()
    {
      var plotRange = _fitElement.DataRowSelection;

      if (plotRange is Altaxo.Data.Selections.AllRows)
      {
        return "All data rows selected";
      }
      else if (plotRange is Altaxo.Data.Selections.RangeOfRowIndices)
      {
        var range = plotRange as Altaxo.Data.Selections.RangeOfRowIndices;
        return "Row index range: " + range.Start.ToString() + " to " + range.LastInclusive.ToString();
      }
      else if (plotRange is Altaxo.Data.Selections.RangeOfNumericalValues)
      {
        var range = plotRange as Altaxo.Data.Selections.RangeOfNumericalValues;
        return string.Format("Range of column {0} from {1} to {2}", range.ColumnName, range.LowerValue, range.UpperValue);
      }
      else
      {
        return "Non-trivial data row selection";
      }
    }

    private void EhDeleteFitElement(object sender, RoutedEventArgs e)
    {
      DeleteThisFitElement?.Invoke();
    }

    private void EhClickOnFitFunction(object sender, RoutedEventArgs e)
    {
      ChooseFitFunction?.Invoke();
      EditFitFunction?.Invoke();
    }

    private void EhSetupVariablesAndRange(object sender, RoutedEventArgs e)
    {
      SetupVariablesAndRange?.Invoke();
    }

    private void EhClickOnParameter(object sender, RoutedEventArgs e)
    {
      int idx = (int)((FrameworkElement)sender).Tag;
      ChooseExternalParameter?.Invoke(idx);
      ((ContentControl)sender).Content = GetTextShownForParameter(idx);
    }

    private void EhClickOnErrorFunction(object sender, RoutedEventArgs e)
    {
      int idx = (int)((FrameworkElement)sender).Tag;
      ChooseErrorFunction?.Invoke(idx);
      ((ContentControl)sender).Content = GetTextShownForErrorEvaluation(idx);
    }

    #endregion Work

    #region IFitElementView

    public void Initialize(Calc.Regression.Nonlinear.FitElement fitElement)
    {
      _fitElement = fitElement;

      _numberOfX = _fitElement.NumberOfIndependentVariables;
      _numberOfY = _fitElement.NumberOfDependentVariables;
      _numberOfParameter = _fitElement.NumberOfParameters;

      _totalSlots = Math.Max(_numberOfParameter, _numberOfX + _numberOfY + 4);
      SetupElements();
    }

    public void Refresh()
    {
      SetupElements();
    }

    public bool FitFunctionSelected
    {
      set
      {
        _fitFunctionSelected = value;
        //this.Invalidate();
      }
    }

    #endregion IFitElementView
  }
}
