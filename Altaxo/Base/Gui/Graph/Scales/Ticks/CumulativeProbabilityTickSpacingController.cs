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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  public interface ICumulativeProbabilityTickSpacingView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(CumulativeProbabilityTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(ICumulativeProbabilityTickSpacingView))]
  public class CumulativeProbabilityTickSpacingController : MVCANControllerEditOriginalDocBase<CumulativeProbabilityTickSpacing, ICumulativeProbabilityTickSpacingView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _targetNumberOfMajorTicks;

    public int TargetNumberOfMajorTicks
    {
      get => _targetNumberOfMajorTicks;
      set
      {
        if (!(_targetNumberOfMajorTicks == value))
        {
          _targetNumberOfMajorTicks = value;
          OnPropertyChanged(nameof(TargetNumberOfMajorTicks));
        }
      }
    }
    private int _targetNumberOfMinorTicks;

    public int TargetNumberOfMinorTicks
    {
      get => _targetNumberOfMinorTicks;
      set
      {
        if (!(_targetNumberOfMinorTicks == value))
        {
          _targetNumberOfMinorTicks = value;
          OnPropertyChanged(nameof(TargetNumberOfMinorTicks));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment GraceEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _minGrace;

    public DimensionfulQuantity MinGrace
    {
      get => _minGrace;
      set
      {
        if (!(_minGrace == value))
        {
          _minGrace = value;
          OnPropertyChanged(nameof(MinGrace));
        }
      }
    }

    private DimensionfulQuantity _maxGrace;

    public DimensionfulQuantity MaxGrace
    {
      get => _maxGrace;
      set
      {
        if (!(_maxGrace == value))
        {
          _maxGrace = value;
          OnPropertyChanged(nameof(MaxGrace));
        }
      }
    }

    private ItemsController<bool> _transformationIsMultiply;

    public ItemsController<bool> TransformationIsMultiply
    {
      get => _transformationIsMultiply;
      set
      {
        if (!(_transformationIsMultiply == value))
        {
          _transformationIsMultiply = value;
          OnPropertyChanged(nameof(TransformationIsMultiply));
        }
      }
    }

    private double _divideBy;

    public double DivideBy
    {
      get => _divideBy;
      set
      {
        if (!(_divideBy == value))
        {
          _divideBy = value;
          OnPropertyChanged(nameof(DivideBy));
        }
      }
    }


    private string _suppressMajorTickValues;

    public string SuppressMajorTicksByValue
    {
      get => _suppressMajorTickValues;
      set
      {
        if (!(_suppressMajorTickValues == value))
        {
          _suppressMajorTickValues = value;
          OnPropertyChanged(nameof(SuppressMajorTicksByValue));
        }
      }
    }

    private string _suppressMajorTicksByNumber;

    public string SuppressMajorTicksByNumber
    {
      get => _suppressMajorTicksByNumber;
      set
      {
        if (!(_suppressMajorTicksByNumber == value))
        {
          _suppressMajorTicksByNumber = value;
          OnPropertyChanged(nameof(SuppressMajorTicksByNumber));
        }
      }
    }

    private string _suppressMinorTicksByValue;

    public string SuppressMinorTicksByValue
    {
      get => _suppressMinorTicksByValue;
      set
      {
        if (!(_suppressMinorTicksByValue == value))
        {
          _suppressMinorTicksByValue = value;
          OnPropertyChanged(nameof(SuppressMinorTicksByValue));
        }
      }
    }
    private string _suppressMinorTicksByNumber;

    public string SuppressMinorTicksByNumber
    {
      get => _suppressMinorTicksByNumber;
      set
      {
        if (!(_suppressMinorTicksByNumber == value))
        {
          _suppressMinorTicksByNumber = value;
          OnPropertyChanged(nameof(SuppressMinorTicksByNumber));
        }
      }
    }
    private string _addMajorTickValues;

    public string AddMajorTickValues
    {
      get => _addMajorTickValues;
      set
      {
        if (!(_addMajorTickValues == value))
        {
          _addMajorTickValues = value;
          OnPropertyChanged(nameof(AddMajorTickValues));
        }
      }
    }

    private string _addMinorTickValues;

    public string AddMinorTickValues
    {
      get => _addMinorTickValues;
      set
      {
        if (!(_addMinorTickValues == value))
        {
          _addMinorTickValues = value;
          OnPropertyChanged(nameof(AddMinorTickValues));
        }
      }
    }


    private ItemsController<BoundaryTickSnapping> _snapTicksToOrg;

    public ItemsController<BoundaryTickSnapping> SnapTicksToOrg
    {
      get => _snapTicksToOrg;
      set
      {
        if (!(_snapTicksToOrg == value))
        {
          _snapTicksToOrg = value;
          OnPropertyChanged(nameof(SnapTicksToOrg));
        }
      }
    }
    private ItemsController<BoundaryTickSnapping> _snapTicksToEnd;

    public ItemsController<BoundaryTickSnapping> SnapTicksToEnd
    {
      get => _snapTicksToEnd;
      set
      {
        if (!(_snapTicksToEnd == value))
        {
          _snapTicksToEnd = value;
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

        SnapTicksToOrg = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapOrgToTick, useUserFriendlyName: true));
        SnapTicksToEnd = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapEndToTick, useUserFriendlyName: true));

        TargetNumberOfMajorTicks = _doc.TargetNumberOfMajorTicks;
        TargetNumberOfMinorTicks = _doc.TargetNumberOfMinorTicks;

        DivideBy = _doc.TransformationDivider;
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

      _doc.SnapOrgToTick = _snapTicksToOrg.SelectedValue;
      _doc.SnapEndToTick = _snapTicksToEnd.SelectedValue;

      _doc.TransformationDivider = DivideBy;
      _doc.TransformationOperationIsMultiply = TransformationIsMultiply.SelectedValue;

      return ApplyEnd(true, disposeController);
    }
  }
}
