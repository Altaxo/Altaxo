#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;
using Altaxo.Science.Spectroscopy.PeakEnhancement;
using Altaxo.Science.Spectroscopy.PeakSearching;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakSearching
{
  public interface IPeakSearchingByTopologyView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(PeakSearchingByTopology))]
  [ExpectedTypeOfView(typeof(IPeakSearchingByTopologyView))]
  public class PeakSearchingByTopologyController : MVCANControllerEditImmutableDocBase<PeakSearchingByTopology, IPeakSearchingByTopologyView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_subControllerPeakEnhancement, () => SubControllerPeakEnhancement = null);
    }

    #region Bindings

    public QuantityWithUnitGuiEnvironment ProminenceEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _minimalProminence;

    public DimensionfulQuantity MinimalProminence
    {
      get => _minimalProminence;
      set
      {
        if (!(_minimalProminence == value))
        {
          _minimalProminence = value;
          OnPropertyChanged(nameof(MinimalProminence));
        }
      }
    }

    private bool _useMinimalProminence;

    public bool UseMinimalProminence
    {
      get => _useMinimalProminence;
      set
      {
        if (!(_useMinimalProminence == value))
        {
          _useMinimalProminence = value;
          OnPropertyChanged(nameof(UseMinimalProminence));
        }
      }
    }

    private int? _maximalNumberOfPeaks;

    public int? MaximalNumberOfPeaks
    {
      get => _maximalNumberOfPeaks;
      set
      {
        if (!(_maximalNumberOfPeaks == value))
        {
          _maximalNumberOfPeaks = value;
          OnPropertyChanged(nameof(MaximalNumberOfPeaks));
        }
      }
    }

    private ItemsController<Type> _availablePeakEnhancementMethods;

    public ItemsController<Type> AvailablePeakEnhancementMethods
    {
      get => _availablePeakEnhancementMethods;
      set
      {
        if (!(_availablePeakEnhancementMethods == value))
        {
          _availablePeakEnhancementMethods = value;
          OnPropertyChanged(nameof(AvailablePeakEnhancementMethods));
        }
      }
    }


    private IMVCANController? _subControllerPeakEnhancement;

    public IMVCANController? SubControllerPeakEnhancement
    {
      get => _subControllerPeakEnhancement;
      set
      {
        if (!(_subControllerPeakEnhancement == value))
        {
          _subControllerPeakEnhancement?.Dispose();
          _subControllerPeakEnhancement = value;
          OnPropertyChanged(nameof(SubControllerPeakEnhancement));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        UseMinimalProminence = _doc.MinimalProminence.HasValue;
        MinimalProminence = new DimensionfulQuantity(_doc.MinimalProminence ?? 0.02, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ProminenceEnvironment.DefaultUnit);
        MaximalNumberOfPeaks = _doc.MaximalNumberOfPeaks;

        // PeakEnhancement
        CreateSubControllerPeakEnhancement();

        var methodTypes = new List<Type>(ReflectionService.GetNonAbstractSubclassesOf(typeof(IPeakEnhancement)));
        methodTypes.Remove(typeof(Altaxo.Science.Spectroscopy.PeakSearching.PeakSearchingNone));
        methodTypes.Sort(new TypeSorter());

        var methods = new SelectableListNodeList();
        foreach (var methodType in methodTypes)
        {
          methods.Add(new SelectableListNode(methodType.Name, methodType, methodType == _doc.GetType()));
        }
        AvailablePeakEnhancementMethods = new ItemsController<Type>(methods, EhPeakEnhancementMethodTypeChanged);
      }
    }


    private void CreateSubControllerPeakEnhancement()
    {
      var subController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.PeakEnhancement }, typeof(IMVCANController));
      if (subController?.GetType() == GetType())
      {
        subController = null;
      }
      if (subController is not null)
      {
        Current.Gui.FindAndAttachControlTo(subController);
      }
      SubControllerPeakEnhancement = subController;
    }

    private void EhPeakEnhancementMethodTypeChanged(Type newMethodType)
    {
      _doc = _doc with { PeakEnhancement = (IPeakEnhancement)Activator.CreateInstance(newMethodType) };
      CreateSubControllerPeakEnhancement();
    }

    private class TypeSorter : IComparer<Type>
    {
      public int Compare(Type x, Type y)
      {
        var xn = x.Name.EndsWith("None");
        var yn = y.Name.EndsWith("None");

        if (xn != yn)
        {
          return xn ? -1 : 1;
        }
        else
        {
          return string.Compare(x.Name, y.Name);
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        MinimalProminence = UseMinimalProminence ? MinimalProminence.AsValueInSIUnits : null,
        MaximalNumberOfPeaks = MaximalNumberOfPeaks,
      };

      return ApplyEnd(true, disposeController);
    }


  }
}
