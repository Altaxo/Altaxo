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

#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  #region Interfaces

  public interface ILog10TickSpacingView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(Log10TickSpacing), 200)]
  [ExpectedTypeOfView(typeof(ILog10TickSpacingView))]
  public class Log10TickSpacingController : MVCANControllerEditOriginalDocBase<Log10TickSpacing, ILog10TickSpacingView>
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

    private bool _decadesPerMajorTickUserSpecified;

    public bool DecadesPerMajorTickUserSpecified
    {
      get => _decadesPerMajorTickUserSpecified;
      set
      {
        if (!(_decadesPerMajorTickUserSpecified == value))
        {
          _decadesPerMajorTickUserSpecified = value;
          OnPropertyChanged(nameof(DecadesPerMajorTickUserSpecified));
        }
      }
    }


    private int _decadesPerMajorTick;

    public int DecadesPerMajorTick
    {
      get => _decadesPerMajorTick;
      set
      {
        if (!(_decadesPerMajorTick == value))
        {
          _decadesPerMajorTick = value;
          OnPropertyChanged(nameof(DecadesPerMajorTick));
        }
      }
    }

    enum MinorTickChoices
    {
      Automatic,
      UserSpecified,
      Ticks123456789,
      Ticks147,
    }
    private MinorTickChoices _minorTickChoice;
    public bool IsMinorTickAutomatic
    {
      get => _minorTickChoice == MinorTickChoices.Automatic;
      set
      {
        if (IsMinorTickAutomatic == false && value == true)
        {
          _minorTickChoice = MinorTickChoices.Automatic;
          OnPropertyChanged(nameof(IsMinorTickAutomatic));
          OnPropertyChanged(nameof(IsMinorTickUserSpecified));
          OnPropertyChanged(nameof(IsMinorTick123456789));
          OnPropertyChanged(nameof(IsMinorTick147));
        }
      }
    }
    public bool IsMinorTickUserSpecified
    {
      get => _minorTickChoice == MinorTickChoices.UserSpecified;
      set
      {
        if (IsMinorTickUserSpecified == false && value == true)
        {
          _minorTickChoice = MinorTickChoices.UserSpecified;
          OnPropertyChanged(nameof(IsMinorTickAutomatic));
          OnPropertyChanged(nameof(IsMinorTickUserSpecified));
          OnPropertyChanged(nameof(IsMinorTick123456789));
          OnPropertyChanged(nameof(IsMinorTick147));
        }
      }
    }
    public bool IsMinorTick123456789
    {
      get => _minorTickChoice == MinorTickChoices.Ticks123456789;
      set
      {
        if (IsMinorTick123456789 == false && value == true)
        {
          _minorTickChoice = MinorTickChoices.Ticks123456789;
          OnPropertyChanged(nameof(IsMinorTickAutomatic));
          OnPropertyChanged(nameof(IsMinorTickUserSpecified));
          OnPropertyChanged(nameof(IsMinorTick123456789));
          OnPropertyChanged(nameof(IsMinorTick147));
        }
      }
    }
    public bool IsMinorTick147
    {
      get => _minorTickChoice == MinorTickChoices.Ticks147;
      set
      {
        if (IsMinorTick147 == false && value == true)
        {
          _minorTickChoice = MinorTickChoices.Ticks147;
          OnPropertyChanged(nameof(IsMinorTickAutomatic));
          OnPropertyChanged(nameof(IsMinorTickUserSpecified));
          OnPropertyChanged(nameof(IsMinorTick123456789));
          OnPropertyChanged(nameof(IsMinorTick147));
        }
      }
    }

    private int _minorTicks = 1;

    public int MinorTicks
    {
      get => _minorTicks;
      set
      {
        if (!(_minorTicks == value))
        {
          _minorTicks = value;
          OnPropertyChanged(nameof(MinorTicks));
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

    public QuantityWithUnitGuiEnvironment OneLeverEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _oneLever;

    public DimensionfulQuantity OneLever
    {
      get => _oneLever;
      set
      {
        if (!(_oneLever == value))
        {
          _oneLever = value;
          OnPropertyChanged(nameof(OneLever));
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
        DecadesPerMajorTickUserSpecified = _doc.DecadesPerMajorTick is not null;
        DecadesPerMajorTick = _doc.DecadesPerMajorTick ?? 1;
        _minorTickChoice = _doc.MinorTicks switch
        {
          null => MinorTickChoices.Automatic,
          -2 => MinorTickChoices.Ticks123456789,
          -1 => MinorTickChoices.Ticks147,
          _ => MinorTickChoices.UserSpecified,
        };
        MinorTicks = _doc.MinorTicks ?? 9;
        OneLever = new DimensionfulQuantity(_doc.OneLever, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OneLeverEnvironment.DefaultUnit);
        MinGrace = new DimensionfulQuantity(_doc.OrgGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);
        MaxGrace = new DimensionfulQuantity(_doc.EndGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);

        _snapTicksToOrg = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapOrgToTick, useUserFriendlyName: true));
        _snapTicksToEnd = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapEndToTick, useUserFriendlyName: true));


        TargetNumberOfMajorTicks = _doc.TargetNumberOfMajorTicks;
        TargetNumberOfMinorTicks = _doc.TargetNumberOfMinorTicks;

        DivideBy = _doc.TransformationDivider;
        TransformationIsMultiply = new ItemsController<bool>(
          new SelectableListNodeList
        {
          new SelectableListNode(" X /  ", false, _doc.TransformationOperationIsMultiply==false),
          new SelectableListNode(" X *  ", true, _doc.TransformationOperationIsMultiply==true)
        });

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

      _doc.DecadesPerMajorTick = DecadesPerMajorTickUserSpecified ? DecadesPerMajorTick : null;
      _doc.MinorTicks = _minorTickChoice switch
      {
        MinorTickChoices.Automatic => null,
        MinorTickChoices.UserSpecified => MinorTicks,
        MinorTickChoices.Ticks147 => -1,
        MinorTickChoices.Ticks123456789 => -2,
        _ => throw new NotImplementedException()
      };

      _doc.TargetNumberOfMajorTicks = TargetNumberOfMajorTicks;
      _doc.TargetNumberOfMinorTicks = TargetNumberOfMinorTicks;

      _doc.OneLever = OneLever.AsValueInSIUnits;
      _doc.OrgGrace = MinGrace.AsValueInSIUnits;
      _doc.EndGrace = MaxGrace.AsValueInSIUnits;

      _doc.TransformationDivider = DivideBy;
      _doc.TransformationOperationIsMultiply = _transformationIsMultiply.SelectedValue;

      _doc.SnapOrgToTick = _snapTicksToOrg.SelectedValue;
      _doc.SnapEndToTick = _snapTicksToEnd.SelectedValue;

      return ApplyEnd(true, disposeController);
    }



    private void EhMajorSpanValidating(string txt, CancelEventArgs e)
    {
      if (GUIConversion.IsInt32OrNull(txt, out var val))
        _doc.DecadesPerMajorTick = val;
      else
        e.Cancel = true;
    }

    private void EhMinorTicksValidating(string txt, CancelEventArgs e)
    {
      if (GUIConversion.IsInt32OrNull(txt, out var val))
        _doc.MinorTicks = val;
      else
        e.Cancel = true;
    }
  }
}
