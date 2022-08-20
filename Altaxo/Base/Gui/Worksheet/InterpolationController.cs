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
using System;
using System.Collections.Generic;
using Altaxo.Calc;
using Altaxo.Calc.Interpolation;
using Altaxo.Collections;
using Altaxo.Gui.Calc.Interpolation;

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

  public interface IInterpolationParameterView : IDataContextAwareView
  {
  }

  public record InterpolationParameters
  {
    public IInterpolationFunctionOptions Interpolation { get; init; } = new FritschCarlsonCubicSplineOptions();
    public double XOrg { get; init; } = 0;
    public double XEnd { get; init; } = 1;
    public int NumberOfPoints { get; init; } = 100;
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for InterpolationParameterController.
  /// </summary>
  [UserControllerForObject(typeof(InterpolationParameters), 100)]
  [ExpectedTypeOfView(typeof(IInterpolationParameterView))]
  public class InterpolationParameterController : MVCANControllerEditImmutableDocBase<InterpolationParameters, IInterpolationParameterView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_interpolationMethod, () => InterpolationMethod = null);
    }      

    #region Bindings

    private InterpolationFunctionOptionsController _interpolationMethod;

    public InterpolationFunctionOptionsController InterpolationMethod
    {
      get => _interpolationMethod;
      set
      {
        if (!(_interpolationMethod == value))
        {
          _interpolationMethod?.Dispose();
          _interpolationMethod = value;
          OnPropertyChanged(nameof(InterpolationMethod));
        }
      }
    }

    private double _xOrg;

    public double XOrg
    {
      get => _xOrg;
      set
      {
        if (!(_xOrg == value))
        {
          _xOrg = value;
          OnPropertyChanged(nameof(XOrg));
        }
      }
    }

    private double _xEnd;

    public double XEnd
    {
      get => _xEnd;
      set
      {
        if (!(_xEnd == value))
        {
          _xEnd = value;
          OnPropertyChanged(nameof(XEnd));
        }
      }
    }

    private int _numberOfPoints;

    public int NumberOfPoints
    {
      get => _numberOfPoints;
      set
      {
        if (!(_numberOfPoints == value))
        {
          _numberOfPoints = value;
          OnPropertyChanged(nameof(NumberOfPoints));
        }
      }
    }




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        InterpolationMethod = new InterpolationFunctionOptionsController(_doc.Interpolation);
        XOrg = _doc.XOrg;
        XEnd = _doc.XEnd;
        NumberOfPoints = _doc.NumberOfPoints;
      }
    }

    public override bool Apply(bool disposeController)
    {
      IInterpolationFunctionOptions interpolation = null;

      if (InterpolationMethod.Apply(disposeController))
      {
        interpolation = (IInterpolationFunctionOptions)InterpolationMethod.ModelObject;
      }
      else
      {
        return ApplyEnd(false, disposeController);
      }

      _doc = _doc with { Interpolation = interpolation, XOrg = XOrg, XEnd = XEnd, NumberOfPoints = NumberOfPoints };

      return ApplyEnd(true, disposeController);
    }
  }
}
