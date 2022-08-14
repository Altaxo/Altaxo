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

#nullable disable
using Altaxo.Calc.Interpolation;
using Altaxo.Gui.Common.PropertyGrid;

namespace Altaxo.Gui.Calc.Interpolation
{
  /// <summary>
  /// Controls the Smoothing parameter of a rational cubic spline.
  /// </summary>
  [UserControllerForObject(typeof(Altaxo.Calc.Interpolation.SmoothingCubicSplineOptions), 100)]
  public class SmoothingCubicSplineOptionsController : PropertyGridController
  {
    private SmoothingCubicSplineOptions Spline => (SmoothingCubicSplineOptions)_doc;

    protected override void InitializeValueInfos()
    {
      {
        var controller = new Altaxo.Gui.Common.BasicTypes.NumericDoubleValueController(Spline.ErrorVariance);
        controller.Minimum = -1;
        controller.IsMinimumValueInclusive = true;
        Current.Gui.FindAndAttachControlTo(controller);
        ValueInfos.Add(new ValueInfo("Error variance (if unknown, set it to -1) :", controller));
      }

      {
        var controller = new Altaxo.Gui.Common.BasicTypes.NumericDoubleValueController(Spline.Smoothness);
        controller.Minimum = 0;
        controller.IsMinimumValueInclusive = true;
        Current.Gui.FindAndAttachControlTo(controller);
        ValueInfos.Add(new ValueInfo("Smoothness (Range: 0 to infinity; 0: cubic spline; infinity: linear regression)  :", controller));
      }
    }

    public override bool Apply(bool disposeController)
    {
      var controller = ValueInfos[0].Controller;

      double errorVariance, smoothness;
      if (false == controller.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }
      else
      {
        errorVariance = (double)controller.ModelObject;
      }

      controller = ValueInfos[1].Controller;
      if (false == controller.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }
      else
      {
        smoothness = (double)controller.ModelObject;
      }

      _doc = Spline with { ErrorVariance = errorVariance, Smoothness = smoothness };


      return ApplyEnd(true, disposeController);
    }
  }
}
