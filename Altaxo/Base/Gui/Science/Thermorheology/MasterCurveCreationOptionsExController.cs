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
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Gui.Worksheet;
using Altaxo.Science.Thermorheology.MasterCurves;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Thermorheology
{
  public interface IMasterCurveCreationOptionsExView : IDataContextAwareView { }

  public class MasterCurveCreationOptionsExController : MVCANDControllerEditImmutableDocBase<MasterCurveCreationOptionsEx, IMasterCurveCreationDataView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _numberOfGroups;

    public int NumberOfGroups
    {
      get => _numberOfGroups;
      set
      {
        if (!(_numberOfGroups == value))
        {
          _numberOfGroups = value;
          OnPropertyChanged(nameof(NumberOfGroups));
        }
      }
    }


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

    private string _property1;

    public string Property1
    {
      get => _property1;
      set
      {
        if (!(_property1 == value))
        {
          _property1 = value;
          OnPropertyChanged(nameof(Property1));
        }
      }
    }

    private string _property2;

    public string Property2
    {
      get => _property2;
      set
      {
        if (!(_property2 == value))
        {
          _property2 = value;
          OnPropertyChanged(nameof(Property2));
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


    private ItemsController<OptimizationMethod> _optimizationMethod;

    public ItemsController<OptimizationMethod> OptimizationMethod
    {
      get => _optimizationMethod;
      set
      {
        if (!(_optimizationMethod == value))
        {
          _optimizationMethod = value;
          OnPropertyChanged(nameof(OptimizationMethod));
        }
      }
    }

    private int _numberOfIterations;

    public int NumberOfIterations
    {
      get => _numberOfIterations;
      set
      {
        if (!(_numberOfIterations == value))
        {
          _numberOfIterations = value;
          OnPropertyChanged(nameof(NumberOfIterations));
        }
      }
    }


    public QuantityWithUnitGuiEnvironment RelativeOverlapEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _relativeOverlap;

    public DimensionfulQuantity RelativeOverlap
    {
      get => _relativeOverlap;
      set
      {
        if (!(_relativeOverlap == value))
        {
          _relativeOverlap = value;
          OnPropertyChanged(nameof(RelativeOverlap));
        }
      }
    }

    private ItemsController<int> _interpolationFunctionSpecification;

    public ItemsController<int> InterpolationFunctionSpecification
    {
      get => _interpolationFunctionSpecification;
      set
      {
        if (!(_interpolationFunctionSpecification == value))
        {
          _interpolationFunctionSpecification = value;
          OnPropertyChanged(nameof(InterpolationFunctionSpecification));
        }
      }
    }

    private ItemsController<object> _interpolationFunction0;

    public ItemsController<object> InterpolationFunction0
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

    private ItemsController<object> _interpolationFunction1;

    public ItemsController<object> InterpolationFunction1
    {
      get => _interpolationFunction1;
      set
      {
        if (!(_interpolationFunction1 == value))
        {
          _interpolationFunction1 = value;
          OnPropertyChanged(nameof(InterpolationFunction1));
        }
      }
    }

    #endregion Bindings

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NumberOfGroups = 1;

        ShiftX = new ItemsController<ShiftXBy>(new SelectableListNodeList(
          new[] {
          new SelectableListNode(ShiftXBy.Factor.ToString(), ShiftXBy.Factor,false),
          new SelectableListNode(ShiftXBy.Offset.ToString(), ShiftXBy.Factor, false)
          }));
        ShiftX.SelectedValue = _doc.XShiftBy;

        NumberOfIterations = _doc.NumberOfIterations;

        LogarithmizeXForInterpolation = _doc.LogarithmizeXForInterpolation;
        LogarithmizeYForInterpolation = _doc.LogarithmizeYForInterpolation;
        RelativeOverlap = new DimensionfulQuantity(_doc.RequiredRelativeOverlap, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelativeOverlapEnvironment.DefaultUnit);

        Property1 = _doc.Property1;
        Property2 = _doc.Property2;

        InitializeInterpolationFunctionChoices();

      }
    }

    private void InitializeInterpolationFunctionChoices()
    {
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.Interpolation.IInterpolationFunctionOptions));

      InterpolationFunction0 = new ItemsController<object>(new SelectableListNodeList(types.Select(t => new SelectableListNode(t.Name, t, false))));
      InterpolationFunction1 = new ItemsController<object>(new SelectableListNodeList(types.Select(t => new SelectableListNode(t.Name, t, false))));


    }

    public override bool Apply(bool disposeController)
    {
      var xShiftBy = ShiftX.SelectedValue;

      var prop1 = Property1;
      var prop2 = Property2;
      var logXForInterpolation = LogarithmizeXForInterpolation;
      var logYForInterpolation = LogarithmizeYForInterpolation;
      var numIterations = NumberOfIterations;
      var optimizationMethod = OptimizationMethod.SelectedValue;
      var relOverlap = RelativeOverlap.AsValueInSIUnits;

      var interpolation0 = InterpolationFunction0.SelectedValue;
      if (interpolation0 is Type t)
        interpolation0 = Activator.CreateInstance(t);

      _doc = _doc with
      {
        XShiftBy = xShiftBy,
        OptimizationMethod = optimizationMethod,
        RequiredRelativeOverlap = relOverlap,
        NumberOfIterations = numIterations,

        Property1 = prop1,
        Property2 = prop2,

        // Interpolation 1
        LogarithmizeXForInterpolation = logXForInterpolation,
        LogarithmizeYForInterpolation = logYForInterpolation,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
