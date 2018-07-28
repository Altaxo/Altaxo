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

using Altaxo.Calc.Interpolation;
using Altaxo.Gui.Common;
using System;

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// Controls the Smoothing parameter of a rational cubic spline.
  /// </summary>
  [UserControllerForObject(typeof(Altaxo.Calc.Interpolation.ExponentialSpline), 100)]
  public class ExponentialSplineController : NumericDoubleValueController
  {
    private ExponentialSpline _spline;

    public ExponentialSplineController(ExponentialSpline spline)
      : base(spline.Smoothing)
    {
      base._minimumValue = 0;
      base._isMinimumValueIncluded = false;
      _descriptionText = "Smoothing parameter p (p>0; default is 1) :";
      _spline = spline;
    }

    public override object ModelObject
    {
      get
      {
        return _spline;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (base.Apply(disposeController))
      {
        this._spline.Smoothing = base._value1Double;
        return true;
      }
      else
        return false;
    }
  }
}
