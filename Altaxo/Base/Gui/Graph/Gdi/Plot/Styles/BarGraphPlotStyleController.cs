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
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  public interface IBarGraphPlotStyleView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(BarGraphPlotStyle))]
  [ExpectedTypeOfView(typeof(IBarGraphPlotStyleView))]
  public class BarGraphPlotStyleController : MVCANControllerEditOriginalDocBase<BarGraphPlotStyle, IBarGraphPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_framePen, () => _framePen = null);
    }

    #region Bindings

    private bool _useFill;

    public bool UseFill
    {
      get => _useFill;
      set
      {
        if (!(_useFill == value))
        {
          _useFill = value;
          OnPropertyChanged(nameof(UseFill));
          EhUseFillChanged(value);
        }
      }
    }
    private void EhUseFillChanged(bool newValue)
    {
      if (true == newValue)
      {
        if (UseFrame && false == IndependentFrameColor)
        {
          InternalSetFillColorToFrameColor();
        }
        else if (FillBrush is null || FillBrush.IsInvisible)
        {
          FillBrush = new BrushX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
        }
      }
    }

    private bool _independentFillColor;

    public bool IndependentFillColor
    {
      get => _independentFillColor;
      set
      {
        if (!(_independentFillColor == value))
        {
          _independentFillColor = value;
          OnPropertyChanged(nameof(IndependentFillColor));
          EhIndependentFillColorChanged(value);
        }
      }
    }
    private void EhIndependentFillColorChanged(bool value)
    {
      _doc.IndependentFillColor = value;
      if (false == value && UseFrame && false == IndependentFrameColor)
        InternalSetFillColorToFrameColor();
      ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFillColor);
    }


    private bool _showPlotColorsOnlyForFillBrush;
    public bool ShowPlotColorsOnlyForFillBrush
    {
      get => _showPlotColorsOnlyForFillBrush;
      set
      {
        if (!(_showPlotColorsOnlyForFillBrush == value))
        {
          _showPlotColorsOnlyForFillBrush = value;
          OnPropertyChanged(nameof(ShowPlotColorsOnlyForFillBrush));
        }
      }
    }


    private BrushX _fillBrush;

    public BrushX FillBrush
    {
      get => _fillBrush;
      set
      {
        if (!(_fillBrush == value))
        {
          _fillBrush = value;
          OnPropertyChanged(nameof(FillBrush));
          EhFillBrushChanged(value);
        }
      }
    }
    private void EhFillBrushChanged(BrushX value)
    {
      if (UseFill && false == IndependentFillColor && UseFrame && false == IndependentFrameColor)
      {
        if (FramePen.Pen.Color != FillBrush.Color)
          InternalSetFrameColorToFillColor();
      }
    }


    private bool _useFrame;

    public bool UseFrame
    {
      get => _useFrame;
      set
      {
        if (!(_useFrame == value))
        {
          _useFrame = value;
          OnPropertyChanged(nameof(UseFrame));
          EhUseFrameChanged(value);
        }
      }
    }
    private void EhUseFrameChanged(bool newValue)
    {
      if (true == newValue)
      {
        if (UseFill && false == IndependentFillColor)
        {
          InternalSetFrameColorToFillColor();
        }
        else if (FramePen is null || FramePen.Pen.IsInvisible)
        {
          FramePen.Pen = new PenX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
        }
      }
    }

    private bool _independentFrameColor;

    public bool IndependentFrameColor
    {
      get => _independentFrameColor;
      set
      {
        if (!(_independentFrameColor == value))
        {
          _independentFrameColor = value;
          OnPropertyChanged(nameof(IndependentFrameColor));
          EhIndependentFrameColorChanged(value);
        }
      }
    }
    private void EhIndependentFrameColorChanged(bool value)
    {
      _doc.IndependentFrameColor = value;
      if (false == value && UseFill && false == IndependentFillColor)
        InternalSetFrameColorToFillColor();
      FramePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFrameColor);
    }

    private ColorTypeThicknessPenController _framePen;

    public ColorTypeThicknessPenController FramePen
    {
      get => _framePen;
      set
      {
        if (!(_framePen == value))
        {
          if (_framePen is { } oldC)
            oldC.MadeDirty -= EhFramePenChanged;
          _framePen?.Dispose();
          _framePen = value;
          if (_framePen is { } newC)
            newC.MadeDirty += EhFramePenChanged;
          OnPropertyChanged(nameof(FramePen));
        }
      }
    }
    private void EhFramePenChanged(IMVCANDController _)
    {
      if (UseFill && false == IndependentFillColor && UseFrame && false == IndependentFrameColor)
      {
        if (FillBrush.Color != FramePen.Pen.Color)
          InternalSetFillColorToFrameColor();
      }
    }

    public QuantityWithUnitGuiEnvironment GapEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _innerGap;

    public DimensionfulQuantity InnerGap
    {
      get => _innerGap;
      set
      {
        if (!(_innerGap == value))
        {
          _innerGap = value;
          OnPropertyChanged(nameof(InnerGap));
        }
      }
    }


    private DimensionfulQuantity _outerGap;

    public DimensionfulQuantity OuterGap
    {
      get => _outerGap;
      set
      {
        if (!(_outerGap == value))
        {
          _outerGap = value;
          OnPropertyChanged(nameof(OuterGap));
        }
      }
    }

    private bool _usePhysicalBaseValue;

    public bool UsePhysicalBaseValue
    {
      get => _usePhysicalBaseValue;
      set
      {
        if (!(_usePhysicalBaseValue == value))
        {
          _usePhysicalBaseValue = value;
          OnPropertyChanged(nameof(UsePhysicalBaseValue));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment BaseValueEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _baseValue;

    public DimensionfulQuantity BaseValue
    {
      get => _baseValue;
      set
      {
        if (!(_baseValue == value))
        {
          _baseValue = value;
          OnPropertyChanged(nameof(BaseValue));
        }
      }
    }


    private bool _startAtPreviousItem;

    public bool StartAtPreviousItem
    {
      get => _startAtPreviousItem;
      set
      {
        if (!(_startAtPreviousItem == value))
        {
          _startAtPreviousItem = value;
          OnPropertyChanged(nameof(StartAtPreviousItem));
        }
      }
    }


    private DimensionfulQuantity _yGap;

    public DimensionfulQuantity YGap
    {
      get => _yGap;
      set
      {
        if (!(_yGap == value))
        {
          _yGap = value;
          OnPropertyChanged(nameof(YGap));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);

        UseFill = _doc.FillBrush is not null && _doc.FillBrush.IsVisible;
        IndependentFillColor = _doc.IndependentFillColor;
        ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFillColor);
        FillBrush = _doc.FillBrush ?? new BrushX(NamedColors.Transparent);

        UseFrame = _doc.FramePen is not null && _doc.FramePen.IsVisible;
        IndependentFrameColor = _doc.IndependentFrameColor;
        FramePen = new ColorTypeThicknessPenController(_doc.FramePen is not null ? _doc.FramePen : new PenX(NamedColors.Transparent))
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFrameColor)
        };
      

      InnerGap = new DimensionfulQuantity(_doc.InnerGap, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapEnvironment.DefaultUnit);
      OuterGap = new DimensionfulQuantity(_doc.OuterGap, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapEnvironment.DefaultUnit);
      UsePhysicalBaseValue = _doc.UsePhysicalBaseValue;
      BaseValue = new DimensionfulQuantity(_doc.UsePhysicalBaseValue ? 0 : _doc.BaseValue, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(BaseValueEnvironment.DefaultUnit);
      StartAtPreviousItem = _doc.StartAtPreviousItem;
      YGap = new DimensionfulQuantity(_doc.PreviousItemYGap, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapEnvironment.DefaultUnit);
    }
  }


    public override bool Apply(bool disposeController)
    {
      if (UseFill)
      {
        _doc.IndependentFillColor = IndependentFillColor;
        _doc.FillBrush = UseFill ? FillBrush : null;
      }
      else
      {
        _doc.IndependentFillColor = true;
        _doc.FillBrush = null;
      }

      if (UseFrame)
      {
        _doc.IndependentFrameColor = IndependentFrameColor;
        _doc.FramePen = FramePen.Pen;
      }
      else
      {
        _doc.IndependentFrameColor = true;
        _doc.FramePen = null;
      }

      _doc.InnerGap = InnerGap.AsValueInSIUnits;
      _doc.OuterGap = OuterGap.AsValueInSIUnits;

      _doc.UsePhysicalBaseValue = UsePhysicalBaseValue;
      if (UsePhysicalBaseValue)
      {
        // who can parse this string? Only the y-scale know how to parse it
      }
      else
      {
        _doc.BaseValue = BaseValue.AsValueInSIUnits;
      }

      _doc.StartAtPreviousItem = StartAtPreviousItem;
      _doc.PreviousItemYGap = YGap.AsValueInSIUnits;

      return ApplyEnd(true, disposeController);
    }

   

    private void EhColorGroupStyleAddedOrRemoved()
    {
      if (_view is not null)
      {
        _doc.IndependentFillColor = IndependentFillColor;
        _doc.IndependentFrameColor = IndependentFrameColor;
        if (UseFill)
          ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFillColor);
        if (UseFrame)
          FramePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFrameColor);
      }
    }

    
   

  

   

    private void InternalSetFillColorToFrameColor()
    {
      FillBrush = FillBrush.WithColor(FramePen.Pen.Color);
    }

    private void InternalSetFrameColorToFillColor()
    {
      FramePen.Pen = FramePen.Pen.WithColor(FillBrush.Color);
    }

    

   
  }
}
