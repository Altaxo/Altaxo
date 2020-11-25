#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Analysis.Statistics
{
  /// <summary>
  /// Interaction logic for HistogramCreationControl.xaml
  /// </summary>
  public partial class HistogramCreationControl : UserControl, IHistogramCreationView
  {
    public event Action? BinningTypeChanged;

    public event Action? AutomaticBinningTypeChanged;

    public HistogramCreationControl()
    {
      InitializeComponent();
    }

    public IEnumerable<string> Errors
    {
      set { _guiErrors.ItemsSource = value; }
    }

    public IEnumerable<string> Warnings
    {
      set { _guiWarnings.ItemsSource = value; }
    }

    public double NumberOfValuesOriginal
    {
      set { _guiNumberOfValuesOriginal.SelectedValue = value; }
    }

    public double NumberOfValuesFiltered
    {
      set { _guiNumberOfValuesFiltered.SelectedValue = value; }
    }

    public double NumberOfNaNValues
    {
      set { _guiNumberOfNaNValues.SelectedValue = value; }
    }

    public double NumberOfInfiniteValues
    {
      set { _guiNumberOfInfiniteValues.SelectedValue = value; }
    }

    public double MinimumValue
    {
      set { _guiMinimumValue.SelectedValue = value; }
    }

    public double MaximumValue
    {
      set { _guiMaximumValue.SelectedValue = value; }
    }

    public bool IgnoreNaNValues
    {
      get
      {
        return true == _guiIgnoreNaNValues.IsChecked;
      }
      set
      {
        _guiIgnoreNaNValues.IsChecked = value;
      }
    }

    public bool IgnoreInfiniteValues
    {
      get
      {
        return true == _guiIgnoreInfiniteValues.IsChecked;
      }
      set
      {
        _guiIgnoreInfiniteValues.IsChecked = value;
      }
    }

    public bool IgnoreValuesBelowLowerBoundary
    {
      get
      {
        return true == _guiIgnoreValuesBelowLowerBoundary.IsChecked;
      }
      set
      {
        _guiIgnoreValuesBelowLowerBoundary.IsChecked = value;
      }
    }

    public bool IsLowerBoundaryInclusive
    {
      get
      {
        return true == _guiIsLowerBoundaryInclusive.IsChecked;
      }
      set
      {
        _guiIsLowerBoundaryInclusive.IsChecked = value;
      }
    }

    public double LowerBoundary
    {
      get
      {
        return _guiLowerBoundaryToIgnore.SelectedValue;
      }
      set
      {
        _guiLowerBoundaryToIgnore.SelectedValue = value;
      }
    }

    public bool IgnoreValuesAboveUpperBoundary
    {
      get
      {
        return true == _guiIgnoreValuesAboveUpperBoundary.IsChecked;
      }
      set
      {
        _guiIgnoreValuesAboveUpperBoundary.IsChecked = value;
      }
    }

    public bool IsUpperBoundaryInclusive
    {
      get
      {
        return true == _guiIsUpperBoundaryInclusive.IsChecked;
      }
      set
      {
        _guiIsUpperBoundaryInclusive.IsChecked = value;
      }
    }

    public double UpperBoundary
    {
      get
      {
        return _guiUpperBoundaryToIgnore.SelectedValue;
      }
      set
      {
        _guiUpperBoundaryToIgnore.SelectedValue = value;
      }
    }

    public bool UseAutomaticBinning
    {
      get
      {
        return true == _guiUseAutomaticBinning.IsChecked;
      }
      set
      {
        _guiUseAutomaticBinning.IsChecked = value;
      }
    }

    public Collections.SelectableListNodeList BinningType
    {
      set { GuiHelper.Initialize(_guiBinningType, value); }
    }

    public object BinningView
    {
      set { _guiBinningControlHost.Child = value as UIElement; }
    }

    private void EhBinningTypeChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiBinningType);

      var ev = BinningTypeChanged;
      if (ev is not null)
        ev();
    }

    private void EhAutomaticBinningTypeChanged(object sender, RoutedEventArgs e)
    {
      var ev = AutomaticBinningTypeChanged;
      if (ev is not null)
        ev();
    }
  }
}
