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
using System.ComponentModel;
using System.Text;
using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  #region Interfaces

  public interface ICumulativeProbabilityTickSpacingView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(CumulativeProbabilityTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(ICumulativeProbabilityTickSpacingView))]
  public class CumulativeProbabilityTickSpacingController : MVCANControllerEditOriginalDocBase<CumulativeProbabilityTickSpacing, ICumulativeProbabilityTickSpacingView>
  {
    private SelectableListNodeList _snapTicksToOrg = new SelectableListNodeList();
    private SelectableListNodeList _snapTicksToEnd = new SelectableListNodeList();

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    //double MinGrace { get; set; }

    //double MaxGrace { get; set; }

    //int TargetNumberMajorTicks { get; set; }

    //int TargetNumberMinorTicks { get; set; }

    private int _TargetNumberOfMajorTicks;

    public int TargetNumberOfMajorTicks
    {
      get => _TargetNumberOfMajorTicks;
      set
      {
        if (!(_TargetNumberOfMajorTicks == value))
        {
          _TargetNumberOfMajorTicks = value;
          OnPropertyChanged(nameof(TargetNumberOfMajorTicks));
        }
      }
    }
    private int _TargetNumberOfMinorTicks;

    public int TargetNumberOfMinorTicks
    {
      get => _TargetNumberOfMinorTicks;
      set
      {
        if (!(_TargetNumberOfMinorTicks == value))
        {
          _TargetNumberOfMinorTicks = value;
          OnPropertyChanged(nameof(TargetNumberOfMinorTicks));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment GraceEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _MinGrace;

    public DimensionfulQuantity MinGrace
    {
      get => _MinGrace;
      set
      {
        if (!(_MinGrace == value))
        {
          _MinGrace = value;
          OnPropertyChanged(nameof(MinGrace));
        }
      }
    }

    private DimensionfulQuantity _MaxGrace;

    public DimensionfulQuantity MaxGrace
    {
      get => _MaxGrace;
      set
      {
        if (!(_MaxGrace == value))
        {
          _MaxGrace = value;
          OnPropertyChanged(nameof(MaxGrace));
        }
      }
    }

    private ItemsController<bool> _TransformationIsMultiply;

    public ItemsController<bool> TransformationIsMultiply
    {
      get => _TransformationIsMultiply;
      set
      {
        if (!(_TransformationIsMultiply == value))
        {
          _TransformationIsMultiply = value;
          OnPropertyChanged(nameof(TransformationIsMultiply));
        }
      }
    }

    private double _DivideBy;

    public double DivideBy
    {
      get => _DivideBy;
      set
      {
        if (!(_DivideBy == value))
        {
          _DivideBy = value;
          OnPropertyChanged(nameof(DivideBy));
        }
      }
    }

   
    private string  _SuppressMajorTickValues;

    public string  SuppressMajorTicksByValue
    {
      get => _SuppressMajorTickValues;
      set
      {
        if (!(_SuppressMajorTickValues == value))
        {
          _SuppressMajorTickValues = value;
          OnPropertyChanged(nameof(SuppressMajorTicksByValue));
        }
      }
    }

    private string _SuppressMajorTicksByNumber;

    public string SuppressMajorTicksByNumber
    {
      get => _SuppressMajorTicksByNumber;
      set
      {
        if (!(_SuppressMajorTicksByNumber == value))
        {
          _SuppressMajorTicksByNumber = value;
          OnPropertyChanged(nameof(SuppressMajorTicksByNumber));
        }
      }
    }

    private string _SuppressMinorTicksByValue;

    public string SuppressMinorTicksByValue
    {
      get => _SuppressMinorTicksByValue;
      set
      {
        if (!(_SuppressMinorTicksByValue == value))
        {
          _SuppressMinorTicksByValue = value;
          OnPropertyChanged(nameof(SuppressMinorTicksByValue));
        }
      }
    }
    private string _SuppressMinorTicksByNumber;

    public string SuppressMinorTicksByNumber
    {
      get => _SuppressMinorTicksByNumber;
      set
      {
        if (!(_SuppressMinorTicksByNumber == value))
        {
          _SuppressMinorTicksByNumber = value;
          OnPropertyChanged(nameof(SuppressMinorTicksByNumber));
        }
      }
    }
    private string _AddMajorTickValues;

    public string AddMajorTickValues
    {
      get => _AddMajorTickValues;
      set
      {
        if (!(_AddMajorTickValues == value))
        {
          _AddMajorTickValues = value;
          OnPropertyChanged(nameof(AddMajorTickValues));
        }
      }
    }

    private string  _AddMinorTickValues;

    public string  AddMinorTickValues
    {
      get => _AddMinorTickValues;
      set
      {
        if (!(_AddMinorTickValues == value))
        {
          _AddMinorTickValues = value;
          OnPropertyChanged(nameof(AddMinorTickValues));
        }
      }
    }


    private ItemsController<BoundaryTickSnapping> _SnapTicksToOrg;

    public ItemsController<BoundaryTickSnapping> SnapTicksToOrg
    {
      get => _SnapTicksToOrg;
      set
      {
        if (!(_SnapTicksToOrg == value))
        {
          _SnapTicksToOrg = value;
          OnPropertyChanged(nameof(SnapTicksToOrg));
        }
      }
    }
    private ItemsController<BoundaryTickSnapping> _SnapTicksToEnd;

    public ItemsController<BoundaryTickSnapping> SnapTicksToEnd
    {
      get => _SnapTicksToEnd;
      set
      {
        if (!(_SnapTicksToEnd == value))
        {
          _SnapTicksToEnd = value;
          OnPropertyChanged(nameof(SnapTicksToEnd));
        }
      }
    }


    #endregion Bindings
    public override void Dispose(bool isDisposing)
    {
      _snapTicksToOrg = null;
      _snapTicksToEnd = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MinGrace = new DimensionfulQuantity(_doc.OrgGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);
        MaxGrace = new DimensionfulQuantity(_doc.EndGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);

        SnapTicksToOrg = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapOrgToTick, useUserFriendlyName:true));
        SnapTicksToEnd = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapEndToTick, useUserFriendlyName: true));

       TargetNumberOfMajorTicks = _doc.TargetNumberOfMajorTicks;
       TargetNumberOfMinorTicks = _doc.TargetNumberOfMinorTicks;

       DivideBy =_doc.TransformationDivider;
        var transformList = new SelectableListNodeList
        {
          new SelectableListNode(" X /  ", false, _doc.TransformationOperationIsMultiply==false),
          new SelectableListNode(" X *  ", true, _doc.TransformationOperationIsMultiply==true)
        };
        TransformationIsMultiply = new ItemsController<bool>(transformList);

       SuppressMajorTicksByValue = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByValues);
       SuppressMinorTicksByValue = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByValues);
       SuppressMajorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByNumbers);
       SuppressMinorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByNumbers);

       AddMajorTickValues = GUIConversion.ToString(_doc.AdditionalMajorTicks.Values);
       AddMinorTickValues = GUIConversion.ToString(_doc.AdditionalMinorTicks.Values);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (GUIConversion.TryParseMultipleAltaxoVariant(SuppressMajorTicksByValue, out var varVals))
      {
        _doc.SuppressedMajorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMajorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(SuppressMinorTicksByValue, out varVals))
      {
        _doc.SuppressedMinorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMinorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(SuppressMajorTicksByNumber, out var intVals))
      {
        _doc.SuppressedMajorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMajorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(SuppressMinorTicksByNumber, out intVals))
      {
        _doc.SuppressedMinorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMinorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(AddMajorTickValues, out varVals))
      {
        _doc.AdditionalMajorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMajorTicks.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(AddMinorTickValues, out varVals))
      {
        _doc.AdditionalMinorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMinorTicks.Add(v);
      }
      else
      {
        return false;
      }

      _doc.TargetNumberOfMajorTicks = TargetNumberOfMajorTicks;
      _doc.TargetNumberOfMinorTicks = TargetNumberOfMinorTicks;

      _doc.OrgGrace = MinGrace.AsValueInSIUnits;
      _doc.EndGrace = MaxGrace.AsValueInSIUnits;

      _doc.SnapOrgToTick = (BoundaryTickSnapping)_snapTicksToOrg.FirstSelectedNode.Tag;
      _doc.SnapEndToTick = (BoundaryTickSnapping)_snapTicksToEnd.FirstSelectedNode.Tag;

      _doc.TransformationDivider = DivideBy;
      _doc.TransformationOperationIsMultiply = TransformationIsMultiply.SelectedValue;

      return ApplyEnd(true, disposeController);
    }
  }
}
