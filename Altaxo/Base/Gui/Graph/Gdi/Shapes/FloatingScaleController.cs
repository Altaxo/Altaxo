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
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  using Altaxo.Graph.Gdi.Background;
  using Altaxo.Gui.Graph.Gdi.Background;
  using Altaxo.Gui.Graph.Scales.Ticks;
  using Altaxo.Units;
  using Gdi.Axis;
  using Geometry;

  public interface IFloatingScaleView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(FloatingScale), 110)]
  [ExpectedTypeOfView(typeof(IFloatingScaleView))]
  public class FloatingScaleController : MVCANControllerEditOriginalDocBase<FloatingScale, IFloatingScaleView>
  {
    protected AxisStyleController _axisStyleController;
    protected TickSpacing _tempTickSpacing;
    protected TickSpacingController _tickSpacingController;
    protected BackgroundStyleController _backgroundStyleController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_tickSpacingController, () => _tickSpacingController = null);
      yield return new ControllerAndSetNullMethod(_axisStyleController, () => _axisStyleController = null);
      yield return new ControllerAndSetNullMethod(_backgroundStyleController, () => _backgroundStyleController = null);
    }

    #region Bindings

    public QuantityWithUnitGuiEnvironment PositionEnvironment => Altaxo.Gui.PositionEnvironment.Instance;

    private DimensionfulQuantity _PositionX;

    public DimensionfulQuantity PositionX
    {
      get => _PositionX;
      set
      {
        if (!(_PositionX == value))
        {
          _PositionX = value;
          OnPropertyChanged(nameof(PositionX));
        }
      }
    }

    private DimensionfulQuantity _PositionY;

    public DimensionfulQuantity PositionY
    {
      get => _PositionY;
      set
      {
        if (!(_PositionY == value))
        {
          _PositionY = value;
          OnPropertyChanged(nameof(PositionY));
        }
      }
    }

    private int _scaleNumber;

    public bool IsScaleToMeasureXScale
    {
      get => _scaleNumber == 0;
      set
      {
        if (IsScaleToMeasureXScale != value && value is true)
        {
          _scaleNumber = 0;
          OnPropertyChanged(nameof(IsScaleToMeasureXScale));
          OnPropertyChanged(nameof(IsScaleToMeasureYScale));
        }
      }
    }
    public bool IsScaleToMeasureYScale
    {
      get => _scaleNumber == 1;
      set
      {
        if (IsScaleToMeasureYScale != value && value is true)
        {
          _scaleNumber = 1;
          OnPropertyChanged(nameof(IsScaleToMeasureXScale));
          OnPropertyChanged(nameof(IsScaleToMeasureYScale));
        }
      }
    }


    private FloatingScaleSpanType _scaleSpanType;
    public bool IsScaleSpanTypeLogicalValue
    {
      get => _scaleSpanType == FloatingScaleSpanType.IsLogicalValue;
      set
      {
        if (IsScaleSpanTypeLogicalValue != value && value is true)
        {
          _scaleSpanType = FloatingScaleSpanType.IsLogicalValue;
          OnPropertyChanged(nameof(IsScaleSpanTypeLogicalValue));
          OnPropertyChanged(nameof(IsScaleSpanTypePhysicalEndOrgDifference));
          OnPropertyChanged(nameof(IsScaleSpanTypePhysicalEndOrgRatio));
        }
      }
    }
    public bool IsScaleSpanTypePhysicalEndOrgDifference
    {
      get => _scaleSpanType == FloatingScaleSpanType.IsPhysicalEndOrgDifference;
      set
      {
        if (IsScaleSpanTypePhysicalEndOrgDifference != value && value is true)
        {
          _scaleSpanType = FloatingScaleSpanType.IsPhysicalEndOrgDifference;
          OnPropertyChanged(nameof(IsScaleSpanTypeLogicalValue));
          OnPropertyChanged(nameof(IsScaleSpanTypePhysicalEndOrgDifference));
          OnPropertyChanged(nameof(IsScaleSpanTypePhysicalEndOrgRatio));
        }
      }
    }
    public bool IsScaleSpanTypePhysicalEndOrgRatio
    {
      get => _scaleSpanType == FloatingScaleSpanType.IsPhysicalEndOrgRatio;
      set
      {
        if (IsScaleSpanTypePhysicalEndOrgRatio != value && value is true)
        {
          _scaleSpanType = FloatingScaleSpanType.IsPhysicalEndOrgRatio;
          OnPropertyChanged(nameof(IsScaleSpanTypeLogicalValue));
          OnPropertyChanged(nameof(IsScaleSpanTypePhysicalEndOrgDifference));
          OnPropertyChanged(nameof(IsScaleSpanTypePhysicalEndOrgRatio));
        }
      }
    }



    public QuantityWithUnitGuiEnvironment LogicalScaleSpanEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _LogicalScaleSpan;

    public DimensionfulQuantity LogicalScaleSpan
    {
      get => _LogicalScaleSpan;
      set
      {
        if (!(_LogicalScaleSpan == value))
        {
          _LogicalScaleSpan = value;
          OnPropertyChanged(nameof(LogicalScaleSpan));
        }
      }
    }

    private double _ScaleSpanPhysicalEndOrgDifference = 1;

    public double ScaleSpanPhysicalEndOrgDifference
    {
      get => _ScaleSpanPhysicalEndOrgDifference;
      set
      {
        if (!(_ScaleSpanPhysicalEndOrgDifference == value))
        {
          _ScaleSpanPhysicalEndOrgDifference = value;
          OnPropertyChanged(nameof(ScaleSpanPhysicalEndOrgDifference));
        }
      }
    }

    private double _ScaleSpanPhysicalEndOrgRatio = 2;

    public double ScaleSpanPhysicalEndOrgRatio
    {
      get => _ScaleSpanPhysicalEndOrgRatio;
      set
      {
        if (!(_ScaleSpanPhysicalEndOrgRatio == value))
        {
          _ScaleSpanPhysicalEndOrgRatio = value;
          OnPropertyChanged(nameof(ScaleSpanPhysicalEndOrgRatio));
        }
      }
    }

    private FloatingScale.ScaleSegmentType _scaleSegmentType;
    public bool IsScaleSegmentTypeNormal
    {
      get => _scaleSegmentType == FloatingScale.ScaleSegmentType.Normal;
      set
      {
        if (IsScaleSegmentTypeNormal != value && value is true)
        {
          _scaleSegmentType = FloatingScale.ScaleSegmentType.Normal;
          OnPropertyChanged(nameof(IsScaleSegmentTypeNormal));
          OnPropertyChanged(nameof(IsScaleSegmentTypeDifferenceToOrg));
          OnPropertyChanged(nameof(IsScaleSegmentTypeRatioToOrg));
        }
      }
    }

    public bool IsScaleSegmentTypeDifferenceToOrg
    {
      get => _scaleSegmentType == FloatingScale.ScaleSegmentType.DifferenceToOrg;
      set
      {
        if (IsScaleSegmentTypeDifferenceToOrg != value && value is true)
        {
          _scaleSegmentType = FloatingScale.ScaleSegmentType.DifferenceToOrg;
          OnPropertyChanged(nameof(IsScaleSegmentTypeNormal));
          OnPropertyChanged(nameof(IsScaleSegmentTypeDifferenceToOrg));
          OnPropertyChanged(nameof(IsScaleSegmentTypeRatioToOrg));
        }
      }
    }
    public bool IsScaleSegmentTypeRatioToOrg
    {
      get => _scaleSegmentType == FloatingScale.ScaleSegmentType.RatioToOrg;
      set
      {
        if (IsScaleSegmentTypeRatioToOrg != value && value is true)
        {
          _scaleSegmentType = FloatingScale.ScaleSegmentType.RatioToOrg;
          OnPropertyChanged(nameof(IsScaleSegmentTypeNormal));
          OnPropertyChanged(nameof(IsScaleSegmentTypeDifferenceToOrg));
          OnPropertyChanged(nameof(IsScaleSegmentTypeRatioToOrg));
        }
      }
    }
    public AxisStyleController AxisStyleController => _axisStyleController;

    public BackgroundStyleController BackgroundStyleController => _backgroundStyleController;

    private DimensionfulQuantity _LeftMargin;


    public QuantityWithUnitGuiEnvironment MarginEnvironment => Altaxo.Gui.SizeEnvironment.Instance;
    public DimensionfulQuantity LeftMargin
    {
      get => _LeftMargin;
      set
      {
        if (!(_LeftMargin == value))
        {
          _LeftMargin = value;
          OnPropertyChanged(nameof(LeftMargin));
        }
      }
    }
    private DimensionfulQuantity _TopMargin;

    public DimensionfulQuantity TopMargin
    {
      get => _TopMargin;
      set
      {
        if (!(_TopMargin == value))
        {
          _TopMargin = value;
          OnPropertyChanged(nameof(TopMargin));
        }
      }
    }
    private DimensionfulQuantity _RightMargin;

    public DimensionfulQuantity RightMargin
    {
      get => _RightMargin;
      set
      {
        if (!(_RightMargin == value))
        {
          _RightMargin = value;
          OnPropertyChanged(nameof(RightMargin));
        }
      }
    }
    private DimensionfulQuantity _BottomMargin;

    public DimensionfulQuantity BottomMargin
    {
      get => _BottomMargin;
      set
      {
        if (!(_BottomMargin == value))
        {
          _BottomMargin = value;
          OnPropertyChanged(nameof(BottomMargin));
        }
      }
    }


    public TickSpacingController TickSpacingController => _tickSpacingController;


    #endregion

    public override void Dispose(bool isDisposing)
    {
      _tempTickSpacing = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _tempTickSpacing = (TickSpacing)_doc.TickSpacing.Clone();
        _axisStyleController = new AxisStyleController();
        _axisStyleController.InitializeDocument(_doc.AxisStyle);
        Current.Gui.FindAndAttachControlTo(_axisStyleController);

        _LogicalScaleSpan = new DimensionfulQuantity(0.5, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LogicalScaleSpanEnvironment.DefaultUnit);

        PositionX = new DimensionfulQuantity(_doc.Position.X, Altaxo.Units.Length.Point.Instance).AsQuantityIn(PositionEnvironment.DefaultUnit); ;
        PositionY = new DimensionfulQuantity(_doc.Position.Y, Altaxo.Units.Length.Point.Instance).AsQuantityIn(PositionEnvironment.DefaultUnit); ;
        _scaleNumber = _doc.ScaleNumber;
        _scaleSpanType = _doc.ScaleSpanType;
        switch (_scaleSpanType)
        {
          case FloatingScaleSpanType.IsLogicalValue:
            LogicalScaleSpan = new DimensionfulQuantity(_doc.ScaleSpanValue, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LogicalScaleSpanEnvironment.DefaultUnit);
            break;
          case FloatingScaleSpanType.IsPhysicalEndOrgDifference:
            ScaleSpanPhysicalEndOrgDifference = _doc.ScaleSpanValue;
            break;
          case FloatingScaleSpanType.IsPhysicalEndOrgRatio:
            ScaleSpanPhysicalEndOrgRatio = _doc.ScaleSpanValue;
            break;
          default:
            throw new NotImplementedException();
        }

        _scaleSegmentType = _doc.ScaleType;

        _tickSpacingController = new TickSpacingController();
        _tickSpacingController.InitializeDocument(_tempTickSpacing);
        _backgroundStyleController = new BackgroundStyleController(_doc.Background);
        LeftMargin = new DimensionfulQuantity(_doc.BackgroundPadding.Left, Altaxo.Units.Length.Point.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
        TopMargin = new DimensionfulQuantity(_doc.BackgroundPadding.Top, Altaxo.Units.Length.Point.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
        RightMargin = new DimensionfulQuantity(_doc.BackgroundPadding.Right, Altaxo.Units.Length.Point.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
        BottomMargin = new DimensionfulQuantity(_doc.BackgroundPadding.Bottom, Altaxo.Units.Length.Point.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.Position = new PointD2D(PositionX.AsValueIn(Altaxo.Units.Length.Point.Instance), PositionY.AsValueIn(Altaxo.Units.Length.Point.Instance));
      _doc.ScaleNumber = _scaleNumber;
      _doc.ScaleSpanType = _scaleSpanType;
      _doc.ScaleSpanValue = _scaleSpanType switch
      {
        FloatingScaleSpanType.IsLogicalValue => LogicalScaleSpan.AsValueInSIUnits,
        FloatingScaleSpanType.IsPhysicalEndOrgDifference => ScaleSpanPhysicalEndOrgDifference,
        FloatingScaleSpanType.IsPhysicalEndOrgRatio => ScaleSpanPhysicalEndOrgRatio,
        _ => throw new NotImplementedException()
      };

      // Scale/ticks
      _doc.ScaleType = _scaleSegmentType;
      if (_tickSpacingController is not null && false == _tickSpacingController.Apply(disposeController))
        return false;
      _doc.TickSpacing = (TickSpacing)_tickSpacingController.ModelObject;

      // Title/format
      if (false == _axisStyleController.Apply(disposeController))
        return false;

      _doc.BackgroundPadding = new Margin2D(
        LeftMargin.AsValueIn(Altaxo.Units.Length.Point.Instance),
        TopMargin.AsValueIn(Altaxo.Units.Length.Point.Instance),
        RightMargin.AsValueIn(Altaxo.Units.Length.Point.Instance),
        BottomMargin.AsValueIn(Altaxo.Units.Length.Point.Instance)
        );


      if (!_backgroundStyleController.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      _doc.Background = (IBackgroundStyle?)_backgroundStyleController.ModelObject;

      return ApplyEnd(true, disposeController);
    }
  }
}
