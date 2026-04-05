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
  /// <summary>
  /// Provides the view contract for linear tick-spacing controllers.
  /// </summary>
  public interface ILinearTickSpacingView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="LinearTickSpacing"/>.
  /// </summary>
  [UserControllerForObject(typeof(LinearTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(ILinearTickSpacingView))]
  public class LinearTickSpacingController : MVCANControllerEditOriginalDocBase<LinearTickSpacing, ILinearTickSpacingView>
  {
    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _targetNumberOfMajorTicks;

    /// <summary>
    /// Gets or sets the target number of major ticks.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the target number of minor ticks.
    /// </summary>
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

    private double? _majorTickSpan;

    /// <summary>
    /// Gets or sets the major tick spacing.
    /// </summary>
    public double? MajorTickSpan
    {
      get => _majorTickSpan;
      set
      {
        if (!(_majorTickSpan == value))
        {
          _majorTickSpan = value;
          OnPropertyChanged(nameof(MajorTickSpan));
        }
      }
    }


    private bool _minorTicksUserSpecified;

    /// <summary>
    /// Gets or sets a value indicating whether the number of minor ticks is user-specified.
    /// </summary>
    public bool MinorTicksUserSpecified
    {
      get => _minorTicksUserSpecified;
      set
      {
        if (!(_minorTicksUserSpecified == value))
        {
          _minorTicksUserSpecified = value;
          OnPropertyChanged(nameof(MinorTicksUserSpecified));
        }
      }
    }

    private int _minorTicks = 1;

    /// <summary>
    /// Gets or sets the number of minor ticks.
    /// </summary>
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



    /// <summary>
    /// Gets the quantity environment used for the grace values.
    /// </summary>
    public QuantityWithUnitGuiEnvironment GraceEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _minGrace;

    /// <summary>
    /// Gets or sets the grace at the origin boundary.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the grace at the end boundary.
    /// </summary>
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

    /// <summary>
    /// Gets the quantity environment used for the zero-lever value.
    /// </summary>
    public QuantityWithUnitGuiEnvironment ZeroLeverEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _zeroLever;

    /// <summary>
    /// Gets or sets the zero-lever value.
    /// </summary>
    public DimensionfulQuantity ZeroLever
    {
      get => _zeroLever;
      set
      {
        if (!(_zeroLever == value))
        {
          _zeroLever = value;
          OnPropertyChanged(nameof(ZeroLever));
        }
      }
    }



    private ItemsController<bool> _transformationIsMultiply;

    /// <summary>
    /// Gets or sets the transformation mode that determines whether the transformation multiplies or divides.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the transformation divisor or factor.
    /// </summary>
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

    private double _transformationOffset;

    /// <summary>
    /// Gets or sets the transformation offset.
    /// </summary>
    public double TransformationOffset
    {
      get => _transformationOffset;
      set
      {
        if (!(_transformationOffset == value))
        {
          _transformationOffset = value;
          OnPropertyChanged(nameof(TransformationOffset));
        }
      }
    }



    private string _suppressMajorTickValues;

    /// <summary>
    /// Gets or sets the list of major tick values to suppress.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the list of major tick indices to suppress.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the list of minor tick values to suppress.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the list of minor tick indices to suppress.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the additional major tick values.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the additional minor tick values.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the snapping behavior for ticks at the origin boundary.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the snapping behavior for ticks at the end boundary.
    /// </summary>
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


    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      _snapTicksToOrg = null;
      _snapTicksToEnd = null;
      base.Dispose(isDisposing);
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MajorTickSpan = _doc.MajorTickSpan;
        MinorTicksUserSpecified = _doc.MinorTicks is not null;
        MinorTicks = _doc.MinorTicks ?? 1;
        ZeroLever = new DimensionfulQuantity(_doc.ZeroLever, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ZeroLeverEnvironment.DefaultUnit);
        MinGrace = new DimensionfulQuantity(_doc.OrgGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);
        MaxGrace = new DimensionfulQuantity(_doc.EndGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);

        _snapTicksToOrg = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapOrgToTick, useUserFriendlyName: true));
        _snapTicksToEnd = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapEndToTick, useUserFriendlyName: true));


        TargetNumberOfMajorTicks = _doc.TargetNumberOfMajorTicks;
        TargetNumberOfMinorTicks = _doc.TargetNumberOfMinorTicks;

        TransformationOffset = _doc.TransformationOffset;
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

    /// <inheritdoc />
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

      _doc.MajorTickSpan = MajorTickSpan;
      _doc.MinorTicks = MinorTicksUserSpecified ? MinorTicks : null;

      _doc.TargetNumberOfMajorTicks = TargetNumberOfMajorTicks;
      _doc.TargetNumberOfMinorTicks = TargetNumberOfMinorTicks;

      _doc.ZeroLever = ZeroLever.AsValueInSIUnits;
      _doc.OrgGrace = MinGrace.AsValueInSIUnits;
      _doc.EndGrace = MaxGrace.AsValueInSIUnits;

      _doc.TransformationOffset = TransformationOffset;
      _doc.TransformationDivider = DivideBy;
      _doc.TransformationOperationIsMultiply = _transformationIsMultiply.SelectedValue;

      _doc.SnapOrgToTick = _snapTicksToOrg.SelectedValue;
      _doc.SnapEndToTick = _snapTicksToEnd.SelectedValue;

      return ApplyEnd(true, disposeController);
    }
  }
}
