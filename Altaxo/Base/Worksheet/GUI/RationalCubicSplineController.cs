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

using Altaxo.Calc.Interpolation;
using Altaxo.Gui;
using Altaxo.Gui.Common;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Controls the Smoothing parameter of a rational cubic spline.
  /// </summary>
  [UserControllerForObject(typeof(Altaxo.Calc.Interpolation.RationalCubicSpline),100)]
  public class RationalCubicSplineController : NumericDoubleValueController
  {
    RationalCubicSpline _spline;
    public RationalCubicSplineController(RationalCubicSpline spline)
      : base(spline.Smoothing)
    {
      base._minimumValue = -0.5;
      base._isMinimumValueIncluded=false;
      _descriptionText = "Smoothing parameter p (-0.5<p<0: overshooting; p=0: cubic spline; p=infinity: linear interpolation):";
      _spline = spline;
    }

    public override object ModelObject
    {
      get
      {
        return _spline;
      }
    }

    public override bool Apply()
    {
      if(base.Apply())
      {
        this._spline.Smoothing = base._value1Double;
        return true;
      }
      else
        return false;
    }
  }
}
