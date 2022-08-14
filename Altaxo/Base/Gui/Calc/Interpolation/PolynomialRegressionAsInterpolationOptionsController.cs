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

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// Controls the Smoothing parameter of a rational cubic spline.
  /// </summary>
  [UserControllerForObject(typeof(Altaxo.Calc.Interpolation.PolynomialRegressionAsInterpolationOptions), 100)]
  public class PolynomialRegressionAsInterpolationOptionsController : PropertyGridController
  {
    private PolynomialRegressionAsInterpolationOptions Spline => (PolynomialRegressionAsInterpolationOptions)_doc;

    protected override void InitializeValueInfos()
    {
      var controller = new Altaxo.Gui.Common.BasicTypes.IntegerValueController(Spline.Order);
      controller.UserMinimum = 0;
      Current.Gui.FindAndAttachControlTo(controller);
      ValueInfos.Add(new ValueInfo("Order of the polynom:", controller));
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
        _doc = Spline with { Order = (int)controller.ModelObject };
        return ApplyEnd(true, disposeController);
      }
    }
  }
}
