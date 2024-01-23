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
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science;
using Altaxo.Science.Thermorheology.MasterCurves;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Thermorheology
{
  public interface IMasterCurveCreationOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IMasterCurveCreationOptionsView))]
  [UserControllerForObject(typeof(MasterCurveCreationOptions))]
  public class MasterCurveCreationOptionsController : MVCANDControllerEditImmutableDocBase<MasterCurveCreationOptions, IMasterCurveCreationOptionsView>
  {
    IMVCAController? _selectedController;

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
          OnMadeDirty();
        }
      }
    }

    private bool _manualPivotCurveIndex;

    public bool ManualPivotCurveIndex
    {
      get => _manualPivotCurveIndex;
      set
      {
        if (!(_manualPivotCurveIndex == value))
        {
          _manualPivotCurveIndex = value;
          OnPropertyChanged(nameof(ManualPivotCurveIndex));
        }
      }
    }


    private int _indexOfPivotCurve;

    public int IndexOfPivotCurve
    {
      get => _indexOfPivotCurve;
      set
      {
        if (!(_indexOfPivotCurve == value))
        {
          _indexOfPivotCurve = value;
          OnPropertyChanged(nameof(IndexOfPivotCurve));
        }
      }
    }

    private ItemsController<ShiftOrder> _shiftOrder;

    public ItemsController<ShiftOrder> ShiftOrder
    {
      get => _shiftOrder;
      set
      {
        if (!(_shiftOrder == value))
        {
          _shiftOrder = value;
          OnPropertyChanged(nameof(ShiftOrder));
        }
      }
    }


    private double? _ReferenceValue;

    public double? ReferenceValue
    {
      get => _ReferenceValue;
      set
      {
        if (!(_ReferenceValue == value))
        {
          _ReferenceValue = value;
          OnPropertyChanged(nameof(ReferenceValue));
        }
      }
    }

    private bool _isReferenceValueUsed;

    public bool IsReferenceValueUsed
    {
      get => _isReferenceValueUsed;
      set
      {
        if (!(_isReferenceValueUsed == value))
        {
          _isReferenceValueUsed = value;
          OnPropertyChanged(nameof(IsReferenceValueUsed));
        }
      }
    }


    private bool _useExactReferenceValue;

    public bool UseExactReferenceValue
    {
      get => _useExactReferenceValue;
      set
      {
        if (!(_useExactReferenceValue == value))
        {
          _useExactReferenceValue = value;
          OnPropertyChanged(nameof(UseExactReferenceValue));
        }
      }
    }

    private int _indexOfReferenceColumn;

    public int IndexOfReferenceColumn
    {
      get => _indexOfReferenceColumn;
      set
      {
        if (!(_indexOfReferenceColumn == value))
        {
          _indexOfReferenceColumn = value;
          OnPropertyChanged(nameof(IndexOfReferenceColumn));
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
          OnMadeDirty();
        }
      }
    }

    private bool _Property1IsTemperature;

    public bool Property1IsTemperature
    {
      get => _Property1IsTemperature;
      set
      {
        if (!(_Property1IsTemperature == value))
        {
          _Property1IsTemperature = value;
          OnPropertyChanged(nameof(Property1IsTemperature));
        }
      }
    }

    private ItemsController<TemperatureRepresentation> _Property1TemperatureRepresentation;

    public ItemsController<TemperatureRepresentation> Property1TemperatureRepresentation
    {
      get => _Property1TemperatureRepresentation;
      set
      {
        if (!(_Property1TemperatureRepresentation == value))
        {
          _Property1TemperatureRepresentation = value;
          OnPropertyChanged(nameof(Property1TemperatureRepresentation));
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
          OnMadeDirty();
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

    private ItemsController<MasterCurveGroupOptionsChoice> _interpolationFunctionSpecification;

    public ItemsController<MasterCurveGroupOptionsChoice> InterpolationFunctionSpecification
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

    private ItemsController<MasterCurveGroupOptionsChoice> _GroupOptionsChoice;

    public ItemsController<MasterCurveGroupOptionsChoice> GroupOptionsChoice
    {
      get => _GroupOptionsChoice;
      set
      {
        if (!(_GroupOptionsChoice == value))
        {
          _GroupOptionsChoice?.Dispose();
          _GroupOptionsChoice = value;
          OnPropertyChanged(nameof(GroupOptionsChoice));
        }
      }
    }


    ItemsController<IMVCAController> _tabControllers;
    public ItemsController<IMVCAController> TabControllers
    {
      get => _tabControllers;
      set
      {
        if (!(_tabControllers == value))
        {
          _tabControllers = value;
          OnPropertyChanged(nameof(TabControllers));
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
        IndexOfReferenceColumn = _doc.IndexOfReferenceColumnInColumnGroup;
        OptimizationMethod = new ItemsController<OptimizationMethod>(new SelectableListNodeList(_doc.OptimizationMethod));

        NumberOfIterations = _doc.NumberOfIterations;
        InterpolationFunctionSpecification = new ItemsController<MasterCurveGroupOptionsChoice>(new Collections.SelectableListNodeList(_doc.MasterCurveGroupOptionsChoice));
        RelativeOverlap = new DimensionfulQuantity(_doc.RequiredRelativeOverlap, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelativeOverlapEnvironment.DefaultUnit);

        Property1 = _doc.Property1;
        Property1IsTemperature = _doc.Property1TemperatureRepresentation is not null;
        Property1TemperatureRepresentation = new ItemsController<TemperatureRepresentation>(new SelectableListNodeList(_doc.Property1TemperatureRepresentation ?? TemperatureRepresentation.DegreeCelsius));
        if (_doc.Property1TemperatureRepresentation.HasValue)
        {
          Property1TemperatureRepresentation.SelectedValue = _doc.Property1TemperatureRepresentation.Value;
        }

        Property2 = _doc.Property2;

        GroupOptionsChoice = new ItemsController<MasterCurveGroupOptionsChoice>(new SelectableListNodeList(_doc.MasterCurveGroupOptionsChoice), EhCurveGroupOptionsChanged);
        GroupOptionsChoice.SelectedValue = _doc.MasterCurveGroupOptionsChoice;

        AddControllers(_doc.MasterCurveGroupOptionsChoice);
        TabControllers.SelectedValue = _selectedController;
      }
    }

    private void EhCurveGroupOptionsChanged(MasterCurveGroupOptionsChoice choice)
    {
      AddControllers(choice);
    }

    private void EhSelectedTabChanged(IMVCANController controller)
    {
      if (!object.ReferenceEquals(controller, _selectedController))
      {
        if (!_selectedController.Apply(false))
        {
          TabControllers.SelectedValue = _selectedController;
          return;
        }
        _selectedController = controller;
      }
    }

    protected virtual void AddControllers(MasterCurveGroupOptionsChoice choice)
    {
      var list = new SelectableListNodeList();
      switch (choice)
      {
        case MasterCurveGroupOptionsChoice.SameForAllGroups:
          {
            if (!(_doc.GroupOptions.Count >= 1 && _doc.GroupOptions[0] is MasterCurveGroupOptionsWithScalarInterpolation doc))
            {
              doc = new MasterCurveGroupOptionsWithScalarInterpolation();
            }
            var controller = new MasterCurveGroupOptionsWithScalarInterpolationController();
            controller.InitializeDocument(doc);
            Current.Gui.FindAndAttachControlTo(controller);
            list.Add(new SelectableListNodeWithController("For all groups", controller, false) { Controller = controller, ControllerTag = 0 });
          }
          break;
        case MasterCurveGroupOptionsChoice.SeparateForEachGroup:
          {
            for (int i = 0; i < NumberOfGroups; ++i)
            {
              if (!(i < _doc.GroupOptions.Count && _doc.GroupOptions[i] is MasterCurveGroupOptionsWithScalarInterpolation doc))
              {
                doc = new MasterCurveGroupOptionsWithScalarInterpolation();
              }

              var controller = new MasterCurveGroupOptionsWithScalarInterpolationController();
              controller.InitializeDocument(doc);
              Current.Gui.FindAndAttachControlTo(controller);
              list.Add(new SelectableListNodeWithController($"Group {i}", controller, false) { Controller = controller, ControllerTag = i });
            }
          }
          break;
        case MasterCurveGroupOptionsChoice.ForComplex:
          {
            if (!(_doc.GroupOptions.Count >= 1 && _doc.GroupOptions[0] is MasterCurveGroupOptionsWithComplexInterpolation doc))
            {
              doc = new MasterCurveGroupOptionsWithComplexInterpolation();
            }

            /*
            var controller = new MasterCurveGroupOptionsWithComplexInterpolationController();
            controller.InitializeDocument(doc);
            Current.Gui.FindAndAttachControlTo(controller);
            list.Add(new SelectableListNodeWithController("For all groups", controller, false) { Controller = controller, ControllerTag = 0 });
            */

          }
          break;
        default:
          throw new NotImplementedException();
      }
      TabControllers = new ItemsController<IMVCAController>(list, EhGroupOptionsTabChanged);
      TabControllers.SelectedValue = _selectedController = ((SelectableListNodeWithController)list[0]).Controller;

    }

    private void EhGroupOptionsTabChanged(IMVCAController controller)
    {
    }

    private void EhGroupOptionsTabChanged(MasterCurveGroupOptionsChoice choice)
    {
    }

    public override bool Apply(bool disposeController)
    {
      if (_selectedController is not null && !_selectedController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }


      var prop1 = Property1;
      var prop2 = Property2;
      var numIterations = NumberOfIterations;
      var optimizationMethod = OptimizationMethod.SelectedValue;
      var relOverlap = RelativeOverlap.AsValueInSIUnits;
      var choice = GroupOptionsChoice.SelectedValue;
      var options = TabControllers.Items.Select(x => (MasterCurveGroupOptions)(((SelectableListNodeWithController)x).Controller.ModelObject)).ToImmutableList();

      _doc = _doc with
      {
        IndexOfReferenceColumnInColumnGroup = IndexOfReferenceColumn,
        OptimizationMethod = optimizationMethod,
        RequiredRelativeOverlap = relOverlap,
        NumberOfIterations = numIterations,
        MasterCurveGroupOptionsChoice = choice,
        GroupOptions = options,


        Property1 = prop1,
        Property1TemperatureRepresentation = Property1IsTemperature ? Property1TemperatureRepresentation.SelectedValue : null,
        Property2 = prop2,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
