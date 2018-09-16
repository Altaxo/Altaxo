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
using System.Windows.Controls;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Interaction logic for FitPolynomialDialogControl.xaml
  /// </summary>
  public partial class FitPolynomialDialogControl : UserControl, IFitPolynomialDialogControl
  {
    public FitPolynomialDialogControl()
    {
      InitializeComponent();
    }

    #region IFitPolynomialDialogControl

    public int Order
    {
      get
      {
        return _edOrder.Value;
      }
      set
      {
        _edOrder.Value = value;
      }
    }

    public double FitCurveXmin
    {
      get
      {
        if (Altaxo.Serialization.NumberConversion.IsDouble(_edFitCurveXmin.Text, out var result))
          return result;
        else
          return double.MinValue;
      }
      set
      {
        _edFitCurveXmin.Text = Altaxo.Serialization.NumberConversion.ToString(value);
      }
    }

    public double FitCurveXmax
    {
      get
      {
        if (Altaxo.Serialization.NumberConversion.IsDouble(_edFitCurveXmax.Text, out var result))
          return result;
        else
          return double.MaxValue;
      }
      set
      {
        _edFitCurveXmax.Text = Altaxo.Serialization.NumberConversion.ToString(value);
      }
    }

    public bool ShowFormulaOnGraph
    {
      get
      {
        return true == _chkShowFormulaOnGraph.IsChecked;
      }
      set
      {
        _chkShowFormulaOnGraph.IsChecked = value;
      }
    }

    #endregion IFitPolynomialDialogControl
  }
}
