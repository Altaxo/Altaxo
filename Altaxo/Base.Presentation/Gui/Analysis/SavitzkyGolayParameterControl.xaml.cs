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

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// Interaction logic for SavitzkyGolayParameterControl.xaml
  /// </summary>
  [UserControlForController(typeof(ISavitzkyGolayParameterViewEventSink))]
  public partial class SavitzkyGolayParameterControl : UserControl, ISavitzkyGolayParameterView
  {
    public SavitzkyGolayParameterControl()
    {
      InitializeComponent();
    }

    private void _edNumberOfPoints_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
      if (null != Controller)
        Controller.EhValidatingNumberOfPoints(_edNumberOfPoints.Value);
    }

    private void _edPolynomialOrder_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
      if (null != Controller)
        Controller.EhValidatingPolynomialOrder(_edPolynomialOrder.Value);
    }

    private void _edDerivativeOrder_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
      if (null != Controller)
        Controller.EhValidatingDerivativeOrder(_edDerivativeOrder.Value);
    }

    #region ISavitzkyGolayParameterView

    public ISavitzkyGolayParameterViewEventSink _controller;

    public ISavitzkyGolayParameterViewEventSink Controller
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

    public void InitializeNumberOfPoints(int val, int max)
    {
      _edNumberOfPoints.Maximum = max;
      _edNumberOfPoints.Value = val;
    }

    public void InitializeDerivativeOrder(int val, int max)
    {
      _edDerivativeOrder.Maximum = max;
      _edDerivativeOrder.Value = val;
    }

    public void InitializePolynomialOrder(int val, int max)
    {
      _edPolynomialOrder.Maximum = max;
      _edPolynomialOrder.Value = val;
    }

    public int GetNumberOfPoints()
    {
      return _edNumberOfPoints.Value;
    }

    public int GetDerivativeOrder()
    {
      return _edDerivativeOrder.Value;
    }

    public int GetPolynomialOrder()
    {
      return _edPolynomialOrder.Value;
    }

    #endregion ISavitzkyGolayParameterView
  }
}
