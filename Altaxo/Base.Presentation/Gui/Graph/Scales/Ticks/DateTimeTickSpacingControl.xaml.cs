#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  /// <summary>
  /// Interaction logic for DateTimeTickSpacingControl.xaml
  /// </summary>
  public partial class DateTimeTickSpacingControl : UserControl, IDateTimeTickSpacingView
  {
    public DateTimeTickSpacingControl()
    {
      InitializeComponent();
    }

    private void _edMajorSpan_Validating(object sender, ValidationEventArgs<string> e)
    {
      var c = new System.ComponentModel.CancelEventArgs();
      if (MajorTicksValidating is not null)
        MajorTicksValidating(_edMajorSpan.Text, c);
      if (c.Cancel)
        e.AddError("The provided text can not be converted");
    }

    private void _cbSnapTicksToOrg_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      GuiHelper.SynchronizeSelectionFromGui(_cbSnapTicksToOrg);
    }

    private void _cbSnapTicksToEnd_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      GuiHelper.SynchronizeSelectionFromGui(_cbSnapTicksToEnd);
    }


    #region IDateTimeTickSpacingView

    public string MajorTicks
    {
      set { _edMajorSpan.Text = value; }
    }

    public int? MinorTicks
    {
      get
      {
        if (_rbMinorTicksManual.IsChecked == true)
          return _edMinorTicks.Value;
        else
          return null;
      }
      set
      {
        if (value is null)
          _rbMinorTicksAutomatic.IsChecked = true;
        else
        {
          _edMinorTicks.Value = value.Value;
          _rbMinorTicksManual.IsChecked = true;
        }
      }
    }

    private void EhMinorTicks_ModeChanged(object sender, RoutedEventArgs e)
    {
      _edMinorTicks.IsEnabled = true == _rbMinorTicksManual.IsChecked;
    }

    public double MinGrace
    {
      get { return _edMinGrace.SelectedQuantityAsValueInSIUnits; }
      set { _edMinGrace.SelectedQuantityAsValueInSIUnits = value; }
    }

    public double MaxGrace
    {
      get { return _edMaxGrace.SelectedQuantityAsValueInSIUnits; }
      set { _edMaxGrace.SelectedQuantityAsValueInSIUnits = value; }
    }

    public int TargetNumberMajorTicks
    {
      get
      {
        return _edTargetNumberMajorTicks.Value;
      }
      set
      {
        _edTargetNumberMajorTicks.Value = value;
      }
    }

    public int TargetNumberMinorTicks
    {
      get
      {
        return _edTargetNumberMinorTicks.Value;
      }
      set
      {
        _edTargetNumberMinorTicks.Value = value;
      }
    }

    public Collections.SelectableListNodeList SnapTicksToOrg
    {
      set { GuiHelper.Initialize(_cbSnapTicksToOrg, value); }
    }

    public Collections.SelectableListNodeList SnapTicksToEnd
    {
      set { GuiHelper.Initialize(_cbSnapTicksToEnd, value); }
    }

    public string SuppressMajorTickValues
    {
      get
      {
        return _edSuppressMajorValues.Text;
      }
      set
      {
        _edSuppressMajorValues.Text = value;
      }
    }

    public string SuppressMinorTickValues
    {
      get
      {
        return _edSuppressMinorValues.Text;
      }
      set
      {
        _edSuppressMinorValues.Text = value;
      }
    }

    public string SuppressMajorTicksByNumber
    {
      get
      {
        return _edSuppressMajorTicksByNumber.Text;
      }
      set
      {
        _edSuppressMajorTicksByNumber.Text = value;
      }
    }

    public string SuppressMinorTicksByNumber
    {
      get
      {
        return _edSuppressMinorTicksByNumber.Text;
      }
      set
      {
        _edSuppressMinorTicksByNumber.Text = value;
      }
    }

    public string AddMajorTickValues
    {
      get
      {
        return _edAddMajorTickValues.Text;
      }
      set
      {
        _edAddMajorTickValues.Text = value;
      }
    }

    public string AddMinorTickValues
    {
      get
      {
        return _edAddMinorTickValues.Text;
      }
      set
      {
        _edAddMinorTickValues.Text = value;
      }
    }

    public event Action<string, System.ComponentModel.CancelEventArgs>? MajorTicksValidating;

    public event Action<string, System.ComponentModel.CancelEventArgs> DivideByValidating { add { } remove { } }

    public event Action<string, System.ComponentModel.CancelEventArgs> TransfoOffsetValidating { add { } remove { } }

    public event Action<bool> TransfoOperationChanged { add { } remove { } }

    #endregion ILinearTickSpacingView
  }
}
