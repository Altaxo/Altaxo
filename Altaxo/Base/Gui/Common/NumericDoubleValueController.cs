#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Summary description for NumericDoubleValueController.
  /// </summary>
  [UserControllerForObject(typeof(double),100)]
  public class NumericDoubleValueController : SingleValueController
  {
    protected double _value1Double;
    protected double _value1DoubleTemporary;

    /// <summary>The minimum value that can be reached. If the minimum value itself is included, is determined by the flag <see cref="_isMinimumValueIncluded" />.</summary>
    protected double _minimumValue = double.MinValue;
    /// <summary>If true, the minimum value itself is valid for the entered number. If false, only values greater than the minimum value are valid.</summary>
    protected bool _isMinimumValueIncluded=true;
    /// <summary>The maximum value that can be reached. If the maximum value itself is included, is determined by the flag <see cref="_isMinimumValueIncluded" />.</summary>
    protected double _maximumValue = double.MaxValue;
    /// <summary>If true, the maximum value itself is valid for the entered number. If false, only values lesser than the maximum value are valid.</summary>
    protected bool   _isMaximumValueIncluded=true;

    public NumericDoubleValueController(double val)
      : base(Altaxo.Serialization.GUIConversion.ToString(val))
    {
      _value1Double = _value1DoubleTemporary = val;
    }

    public override object ModelObject
    {
      get
      {
        return _value1Double;
      }
    }

    public override bool Apply()
    {
      this._value1Double = this._value1DoubleTemporary;
      return base.Apply();
    }

    public override void EhView_ValidatingValue1(object sender, System.ComponentModel.CancelEventArgs e)
    {
      string val = _view.ValueText;
      double vald;
      if(Altaxo.Serialization.GUIConversion.IsDouble(val,out vald))
      {
        if(vald<_minimumValue || (!_isMinimumValueIncluded && vald==_minimumValue))
        {
          e.Cancel=true;
          Current.Gui.ErrorMessageBox(string.Format("Value must be {0} than {1}!",(_isMinimumValueIncluded?"greater or equal":"greater"), Altaxo.Serialization.GUIConversion.ToString(_minimumValue)));
        }
        else if(vald>_maximumValue || (!_isMaximumValueIncluded && vald==_maximumValue))
        {
          e.Cancel=true;
          Current.Gui.ErrorMessageBox(string.Format("Value must be {0} than {1}!",(_isMaximumValueIncluded?"less or equal":"less"),Altaxo.Serialization.GUIConversion.ToString(_maximumValue)));
        }
        else
        {
          _value1DoubleTemporary = vald;
        }
      }
      else
      {
        e.Cancel = true;
      }

      base.EhView_ValidatingValue1(val,e);
    }

  }
}
