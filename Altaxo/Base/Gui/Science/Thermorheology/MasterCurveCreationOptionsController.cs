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
using Altaxo.Science.Thermorheology.MasterCurves.ShiftOrder;
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
      if (ImprovementOptionsController is not null)
        yield return new ControllerAndSetNullMethod(ImprovementOptionsController, () => ImprovementOptionsController = null);
      if (TableOutputOptionsController is not null)
        yield return new ControllerAndSetNullMethod(TableOutputOptionsController, () => TableOutputOptionsController = null);
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
          EhNumberOfGroupsChanged(value);
          OnPropertyChanged(nameof(NumberOfGroups));
          OnMadeDirty();
        }
      }
    }



    public void TriggerOnMadeDirty()
    {
      OnMadeDirty();
    }

    private bool _useManualPivotCurveIndex;

    public bool UseManualPivotCurveIndex
    {
      get => _useManualPivotCurveIndex;
      set
      {
        if (!(_useManualPivotCurveIndex == value))
        {
          _useManualPivotCurveIndex = value;
          OnPropertyChanged(nameof(UseManualPivotCurveIndex));
          OnPropertyChanged(nameof(IsManualPivotIndexRequired));
        }
      }
    }


    private int _indexOfPivotCurve;

    public int ManualPivotCurveIndex
    {
      get => _indexOfPivotCurve;
      set
      {
        if (!(_indexOfPivotCurve == value))
        {
          _indexOfPivotCurve = value;
          OnPropertyChanged(nameof(ManualPivotCurveIndex));
        }
      }
    }

    private void EhShiftOrderChanged(IShiftOrder order)
    {
      OnPropertyChanged(nameof(IsPivotIndexRequired));
      OnPropertyChanged(nameof(IsManualPivotIndexRequired));
    }

    public bool IsPivotIndexRequired => ShiftOrder?.SelectedValue?.IsPivotIndexRequired ?? false;

    public bool IsManualPivotIndexRequired => IsPivotIndexRequired && UseManualPivotCurveIndex;

    private ItemsController<IShiftOrder> _shiftOrder;

    public ItemsController<IShiftOrder> ShiftOrder
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

    private bool _UseImprovementOptions;

    public bool UseImprovementOptions
    {
      get => _UseImprovementOptions;
      set
      {
        if (!(_UseImprovementOptions == value))
        {
          _UseImprovementOptions = value;
          OnPropertyChanged(nameof(UseImprovementOptions));
          if (value == true)
          {
            EhCreateImprovementOptionsAndControllerIfNeccessary();
          }
        }
      }
    }



    private MasterCurveImprovementOptionsController? _improvementOptionsController;

    public MasterCurveImprovementOptionsController? ImprovementOptionsController
    {
      get => _improvementOptionsController;
      set
      {
        if (!(_improvementOptionsController == value))
        {
          _improvementOptionsController?.Dispose();
          _improvementOptionsController = value;
          OnPropertyChanged(nameof(ImprovementOptionsController));
        }
      }
    }

    private IMVCANController _tableOutputOptionsController;

    public IMVCANController TableOutputOptionsController
    {
      get => _tableOutputOptionsController;
      set
      {
        if (!(_tableOutputOptionsController == value))
        {
          _tableOutputOptionsController?.Dispose();
          _tableOutputOptionsController = value;
          OnPropertyChanged(nameof(TableOutputOptionsController));
        }
      }
    }




    #endregion Bindings

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _numberOfGroups = Math.Max(1, _doc.GroupOptions.Count); // must be set silently in order to avoid calling EhNumberOfGroupsChanged
        var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IShiftOrder));
        var instances = types.Select(t => (IShiftOrder)Activator.CreateInstance(t));
        ShiftOrder = new ItemsController<IShiftOrder>(new SelectableListNodeList(
          instances.Select(i => new SelectableListNode(i.GetType().Name, i, false))), EhShiftOrderChanged);
        ShiftOrder.SelectedValue = _doc.ShiftOrder.WithPivotIndex(null);

        UseManualPivotCurveIndex = _doc.ShiftOrder.PivotIndex.HasValue;
        if (_doc.ShiftOrder.PivotIndex.HasValue)
          ManualPivotCurveIndex = _doc.ShiftOrder.PivotIndex.Value;

        IndexOfReferenceColumn = _doc.IndexOfReferenceColumnInColumnGroup;
        OptimizationMethod = new ItemsController<OptimizationMethod>(new SelectableListNodeList(_doc.OptimizationMethod));

        NumberOfIterations = _doc.NumberOfIterations;
        RelativeOverlap = new DimensionfulQuantity(_doc.RequiredRelativeOverlap, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelativeOverlapEnvironment.DefaultUnit);

        Property1 = _doc.Property1Name;
        Property1IsTemperature = _doc.Property1TemperatureRepresentation is not null;
        Property1TemperatureRepresentation = new ItemsController<TemperatureRepresentation>(new SelectableListNodeList(_doc.Property1TemperatureRepresentation ?? TemperatureRepresentation.DegreeCelsius));
        if (_doc.Property1TemperatureRepresentation.HasValue)
        {
          Property1TemperatureRepresentation.SelectedValue = _doc.Property1TemperatureRepresentation.Value;
        }

        Property2 = _doc.Property2Name;

        IsReferenceValueUsed = _doc.ReferenceValue.HasValue;
        if (_doc.ReferenceValue.HasValue)
        {
          ReferenceValue = _doc.ReferenceValue.Value;
        }
        IndexOfReferenceColumn = _doc.IndexOfReferenceColumnInColumnGroup;
        UseExactReferenceValue = _doc.UseExactReferenceValue;

        GroupOptionsChoice = new ItemsController<MasterCurveGroupOptionsChoice>(new SelectableListNodeList(_doc.MasterCurveGroupOptionsChoice), EhCurveGroupOptionsChanged);
        GroupOptionsChoice.SelectedValue = _doc.MasterCurveGroupOptionsChoice;

        AddControllers(_doc.MasterCurveGroupOptionsChoice);
        TabControllers.SelectedValue = _selectedController;


        if (_doc.MasterCurveImprovementOptions is not null)
        {
          CreateImprovementOptionsController(_doc.MasterCurveImprovementOptions);
        }

        TableOutputOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.TableOutputOptions }, typeof(IMVCANController));
      }
    }

    private void EhNumberOfGroupsChanged(int value)
    {
      if (TabControllers.Items.Count != value)
      {
        if (GroupOptionsChoice.SelectedValue == MasterCurveGroupOptionsChoice.SeparateForEachGroup)
        {
          if (value < TabControllers.Items.Count)
          {
            for (int i = TabControllers.Items.Count - 1; i >= value; i--)
            {
              _selectedController = null;
              var oldIndex = TabControllers.SelectedIndex;
              TabControllers.Items.RemoveAt(i);
              var newIndex = Math.Min(TabControllers.Items.Count - 1, oldIndex);
              if (newIndex >= 0 && newIndex != oldIndex)
              {
                _selectedController = null;
                TabControllers.SelectedItem = TabControllers.Items[newIndex];
              }
            }
          }
          else if (value > TabControllers.Items.Count)
          {
            AddControllers(GroupOptionsChoice.SelectedValue);
          }
        }
      }

      if (ImprovementOptionsController is not null)
      {
        ImprovementOptionsController.NumberOfGroups = value;
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
                if (i > 0 && _doc.GroupOptions[0] is MasterCurveGroupOptionsWithScalarInterpolation template)
                  doc = template; // is safe, because is immutable
                else
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

            var controller = new MasterCurveGroupOptionsWithComplexInterpolationController();
            controller.InitializeDocument(doc);
            Current.Gui.FindAndAttachControlTo(controller);
            list.Add(new SelectableListNodeWithController("For all groups", controller, false) { Controller = controller, ControllerTag = 0 });
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
      _selectedController = controller;
    }

    private void EhGroupOptionsTabChanged(MasterCurveGroupOptionsChoice choice)
    {
    }

    private void CreateImprovementOptionsController(MasterCurveImprovementOptions improvementOptions)
    {
      var improvementOptionsController = new MasterCurveImprovementOptionsController();
      improvementOptionsController.InitializeDocument(improvementOptions);
      improvementOptionsController.NumberOfGroups = NumberOfGroups;
      Current.Gui.FindAndAttachControlTo(improvementOptionsController);
      ImprovementOptionsController = improvementOptionsController;
      UseImprovementOptions = true;
    }

    private void EhCreateImprovementOptionsAndControllerIfNeccessary()
    {
      if (ImprovementOptionsController is null)
      {
        // per default, we set the improvement options to the options here
        _selectedController?.Apply(false); // Update the latest group options
        var groupOptions = TabControllers.Items.Select(x => (MasterCurveGroupOptions)(((SelectableListNodeWithController)x).Controller.ModelObject)).ToImmutableList();
        var shiftOrder = ShiftOrder.SelectedValue;
        if (shiftOrder.IsPivotIndexRequired && UseManualPivotCurveIndex)
        {
          shiftOrder = shiftOrder.WithPivotIndex(ManualPivotCurveIndex);
        }

        var improvementOptions = new MasterCurveImprovementOptions()
        {
          ShiftOrder = shiftOrder,
          OptimizationMethod = OptimizationMethod.SelectedValue,
          NumberOfIterations = NumberOfIterations,
          MasterCurveGroupOptionsChoice = GroupOptionsChoice.SelectedValue,
          GroupOptions = groupOptions
        };

        CreateImprovementOptionsController(improvementOptions);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_selectedController is not null && !_selectedController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }



      MasterCurveImprovementOptions? improvementOptions = null;
      if (UseImprovementOptions && (ImprovementOptionsController is not null))
      {
        if (!ImprovementOptionsController.Apply(disposeController))
        {
          return ApplyEnd(false, disposeController);
        }
        else
        {
          improvementOptions = (MasterCurveImprovementOptions)(ImprovementOptionsController.ModelObject);
        }
      }

      if (!TableOutputOptionsController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }
      var tableOutputOptions = (MasterCurveTableOutputOptions)TableOutputOptionsController.ModelObject;

      var prop1 = Property1;
      var prop2 = Property2;
      var numIterations = NumberOfIterations;
      var optimizationMethod = OptimizationMethod.SelectedValue;
      var relOverlap = RelativeOverlap.AsValueInSIUnits;
      var choice = GroupOptionsChoice.SelectedValue;
      var options = TabControllers.Items.Select(x => (MasterCurveGroupOptions)(((SelectableListNodeWithController)x).Controller.ModelObject)).ToImmutableList();
      var shiftOrder = ShiftOrder.SelectedValue;
      if (shiftOrder.IsPivotIndexRequired && UseManualPivotCurveIndex)
      {
        shiftOrder = shiftOrder.WithPivotIndex(ManualPivotCurveIndex);
      }


      _doc = _doc with
      {
        ShiftOrder = shiftOrder,
        OptimizationMethod = optimizationMethod,
        RequiredRelativeOverlap = relOverlap,
        NumberOfIterations = numIterations,
        UseExactReferenceValue = UseExactReferenceValue,
        ReferenceValue = IsReferenceValueUsed ? ReferenceValue : null,
        IndexOfReferenceColumnInColumnGroup = IndexOfReferenceColumn,
        MasterCurveGroupOptionsChoice = choice,
        GroupOptions = options,

        Property1Name = prop1,
        Property1TemperatureRepresentation = Property1IsTemperature ? Property1TemperatureRepresentation.SelectedValue : null,
        Property2Name = prop2,
        MasterCurveImprovementOptions = improvementOptions,
        TableOutputOptions = tableOutputOptions,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
