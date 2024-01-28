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
using Altaxo.Science.Thermorheology.MasterCurves;
using Altaxo.Science.Thermorheology.MasterCurves.ShiftOrder;

namespace Altaxo.Gui.Science.Thermorheology
{
  public interface IMasterCurveImprovementOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IMasterCurveImprovementOptionsView))]
  [UserControllerForObject(typeof(MasterCurveCreationOptions))]
  public class MasterCurveImprovementOptionsController : MVCANDControllerEditImmutableDocBase<MasterCurveImprovementOptions, IMasterCurveImprovementOptionsView>
  {
    IMVCAController? _selectedController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    public void TriggerOnMadeDirty()
    {
      OnMadeDirty();
    }

    /// <summary>
    /// Gets or sets the number of groups. This property must be set by the superior controller.
    /// </summary>
    public int NumberOfGroups { get; set; } = 1;

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
        var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IShiftOrder));
        var instances = types.Select(t => (IShiftOrder)Activator.CreateInstance(t));
        ShiftOrder = new ItemsController<IShiftOrder>(new SelectableListNodeList(
          instances.Where(i => !i.IsOnlySuitableForRefinement).Select(i => new SelectableListNode(i.GetType().Name, i, false))), EhShiftOrderChanged);
        ShiftOrder.SelectedValue = _doc.ShiftOrder.WithPivotIndex(null);

        UseManualPivotCurveIndex = _doc.ShiftOrder.PivotIndex.HasValue;
        if (_doc.ShiftOrder.PivotIndex.HasValue)
          ManualPivotCurveIndex = _doc.ShiftOrder.PivotIndex.Value;

        OptimizationMethod = new ItemsController<OptimizationMethod>(new SelectableListNodeList(_doc.OptimizationMethod));

        NumberOfIterations = _doc.NumberOfIterations;

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
      _selectedController = controller;
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


      var numIterations = NumberOfIterations;
      var optimizationMethod = OptimizationMethod.SelectedValue;
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
        NumberOfIterations = numIterations,
        MasterCurveGroupOptionsChoice = choice,
        GroupOptions = options,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
