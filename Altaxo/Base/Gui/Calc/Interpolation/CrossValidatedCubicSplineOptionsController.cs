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

#nullable disable
using Altaxo.Calc.Interpolation;
using Altaxo.Gui.Common.PropertyGrid;

namespace Altaxo.Gui.Calc.Interpolation
{
  /// <summary>
  /// Controls the Smoothing parameter of a rational cubic spline.
  /// </summary>
  [UserControllerForObject(typeof(CrossValidatedCubicSplineOptions), 100)]
  public class CrossValidatedCubicSplineOptionsController : PropertyGridController
  {
    internal CrossValidatedCubicSplineOptions Spline => (CrossValidatedCubicSplineOptions)_doc;


    protected override void InitializeValueInfos()
    {
      var controller = new Altaxo.Gui.Common.BasicTypes.NumericDoubleValueController(Spline.ErrorStandardDeviation);
      controller.Minimum = -1;
      controller.IsMinimumValueInclusive = true;
      Current.Gui.FindAndAttachControlTo(controller);
      ValueInfos.Add(new ValueInfo("Standard deviation (if unknown, set it to -1) :", controller));
    }

    public override bool Apply(bool disposeController)
    {
      var controller = ValueInfos[0].Controller;
      if (false == controller.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }
      else
      {
        _doc = Spline with { ErrorStandardDeviation = (double)controller.ModelObject };
        return ApplyEnd(true, disposeController);
      }
    }
  }
}
