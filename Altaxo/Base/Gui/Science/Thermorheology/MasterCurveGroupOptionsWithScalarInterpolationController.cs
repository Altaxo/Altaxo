#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2024 Dr. Dirk Lellinger
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
using Altaxo.Calc.Interpolation;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Thermorheology.MasterCurves;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Thermorheology
{
  public interface IMasterCurveGroupOptionsWithScalarInterpolationView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IMasterCurveGroupOptionsWithScalarInterpolationView))]
  [UserControllerForObject(typeof(MasterCurveGroupOptionsWithScalarInterpolation))]
  public class MasterCurveGroupOptionsWithScalarInterpolationController : MVCANControllerEditImmutableDocBase<MasterCurveGroupOptionsWithScalarInterpolation, IMasterCurveGroupOptionsWithScalarInterpolationView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_interpolationDetails, () => InterpolationDetails = null);
    }

    #region Bindings

    private ItemsController<ShiftXBy> _shiftX;

    public ItemsController<ShiftXBy> ShiftX
    {
      get => _shiftX;
      set
      {
        if (!(_shiftX == value))
        {
          _shiftX = value;
          OnPropertyChanged(nameof(ShiftX));
        }
      }
    }

    private bool _logarithmizeXForInterpolation;

    public bool LogarithmizeXForInterpolation
    {
      get => _logarithmizeXForInterpolation;
      set
      {
        if (!(_logarithmizeXForInterpolation == value))
        {
          _logarithmizeXForInterpolation = value;
          OnPropertyChanged(nameof(LogarithmizeXForInterpolation));
        }
      }
    }


    private bool _logarithmizeYForInterpolation;

    public bool LogarithmizeYForInterpolation
    {
      get => _logarithmizeYForInterpolation;
      set
      {
        if (!(_logarithmizeYForInterpolation == value))
        {
          _logarithmizeYForInterpolation = value;
          OnPropertyChanged(nameof(LogarithmizeYForInterpolation));
        }
      }
    }

    private ItemsController<System.Type> _interpolationFunction0;

    public ItemsController<System.Type> InterpolationFunction0
    {
      get => _interpolationFunction0;
      set
      {
        if (!(_interpolationFunction0 == value))
        {
          _interpolationFunction0 = value;
          OnPropertyChanged(nameof(InterpolationFunction0));
        }
      }
    }

    private object _interpolationFunctionInstance;

    private IMVCANController _interpolationDetails;

    public IMVCANController InterpolationDetails
    {
      get => _interpolationDetails;
      set
      {
        if (!(_interpolationDetails == value))
        {
          _interpolationDetails?.Dispose();
          _interpolationDetails = value;
          OnPropertyChanged(nameof(InterpolationDetails));
        }
      }
    }



    public QuantityWithUnitGuiEnvironment FittingWeightEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _fittingWeight;

    public DimensionfulQuantity FittingWeight
    {
      get => _fittingWeight;
      set
      {
        if (!(_fittingWeight == value))
        {
          _fittingWeight = value;
          OnPropertyChanged(nameof(FittingWeight));
        }
      }
    }

    private bool _isParticipatingInFit;

    public bool IsParticipatingInFit
    {
      get => _isParticipatingInFit;
      set
      {
        if (!(_isParticipatingInFit == value))
        {
          _isParticipatingInFit = value;
          OnPropertyChanged(nameof(IsParticipatingInFit));
          if (value == false)
          {
            FittingWeight = new DimensionfulQuantity(0, FittingWeight.PrefixedUnit);
          }
          else if (!(FittingWeight.AsValueInSIUnits > 0))
          {
            FittingWeight = new DimensionfulQuantity(1, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(FittingWeight.PrefixedUnit);
          }
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _interpolationFunctionInstance = _doc.InterpolationFunction;

        ShiftX = new ItemsController<ShiftXBy>(new SelectableListNodeList(
          new[] {
          new SelectableListNode(ShiftXBy.Factor.ToString(), ShiftXBy.Factor,false),
          new SelectableListNode(ShiftXBy.Offset.ToString(), ShiftXBy.Factor, false)
          }));
        ShiftX.SelectedValue = _doc.XShiftBy;

        LogarithmizeXForInterpolation = _doc.LogarithmizeXForInterpolation;
        LogarithmizeYForInterpolation = _doc.LogarithmizeYForInterpolation;
        FittingWeight = new DimensionfulQuantity(_doc.FittingWeight, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(FittingWeightEnvironment.DefaultUnit);
        IsParticipatingInFit = _doc.FittingWeight > 0;
        InitializeInterpolationFunctionChoices();
      }
    }

    private void InitializeInterpolationFunctionChoices()
    {

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.Interpolation.IInterpolationFunctionOptions));
      InterpolationFunction0 = new ItemsController<System.Type>(new SelectableListNodeList(types.Select(t => new SelectableListNode(t.Name, t, false))), EhInterpolationFunctionChanged);
      InterpolationFunction0.SelectedValue = _interpolationFunctionInstance.GetType();
    }

    private void EhInterpolationFunctionChanged(Type type)
    {
      var intp = Activator.CreateInstance(type);
      InterpolationDetails = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { intp }, typeof(IMVCANController));
    }

    public override bool Apply(bool disposeController)
    {
      if (InterpolationDetails is not null)
      {
        if (!InterpolationDetails.Apply(disposeController))
        {
          return ApplyEnd(false, disposeController);
        }
        _interpolationFunctionInstance = InterpolationDetails.ModelObject;
      }

      _doc = _doc with
      {
        LogarithmizeXForInterpolation = LogarithmizeXForInterpolation,
        LogarithmizeYForInterpolation = LogarithmizeYForInterpolation,
        XShiftBy = ShiftX.SelectedValue,
        FittingWeight = FittingWeight.AsValueInSIUnits,
        InterpolationFunction = (IInterpolationFunctionOptions)_interpolationFunctionInstance,

      };

      return ApplyEnd(true, disposeController);
    }

  }
}
