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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Common;
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  #region Interfaces

  /// <summary>
  /// Provides the view contract for <see cref="BarGraphPlotStyleController"/>.
  /// </summary>
  public interface IBarGraphPlotStyleView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for <see cref="BarGraphPlotStyle"/>.
  /// </summary>
  [UserControllerForObject(typeof(BarGraphPlotStyle))]
  [ExpectedTypeOfView(typeof(IBarGraphPlotStyleView))]
  public class BarGraphPlotStyleController : MVCANControllerEditOriginalDocBase<BarGraphPlotStyle, IBarGraphPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _independentColor;

    /// <summary>
    /// Gets or sets a value indicating whether this instance has independent color.
    /// </summary>
    /// <value><c>true</c> if this instance has independent color; otherwise, <c>false</c>.</value>
    public bool IndependentColor
    {
      get => _independentColor;
      set
      {
        if (!(_independentColor == value))
        {
          _independentColor = value;
          OnPropertyChanged(nameof(IndependentColor));
          EhIndependentColorChanged();
        }
      }
    }

    private PenAllPropertiesController _penController;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public PenAllPropertiesController PenController
    {
      get => _penController;
      set
      {
        if (!(_penController == value))
        {
          _penController = value;
          OnPropertyChanged(nameof(PenController));
        }
      }
    }

    private bool _useUniformCrossSectionThickness;

    /// <summary>
    /// Gets or sets a value indicating whether [use uniform cross section thickness].
    /// </summary>
    /// <value><c>true</c> if [use uniform cross section thickness]; otherwise, <c>false</c>.</value>
    public bool UseUniformCrossSectionThickness
    {
      get => _useUniformCrossSectionThickness;
      set
      {
        if (!(_useUniformCrossSectionThickness == value))
        {
          _useUniformCrossSectionThickness = value;
          OnPropertyChanged(nameof(UseUniformCrossSectionThickness));
        }
      }
    }

    private ItemsController<BarShiftStrategy3D> _barShiftStrategy;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public ItemsController<BarShiftStrategy3D> BarShiftStrategy
    {
      get => _barShiftStrategy;
      set
      {
        if (!(_barShiftStrategy == value))
        {
          _barShiftStrategy = value;
          OnPropertyChanged(nameof(BarShiftStrategy));
        }
      }
    }

    private bool _isEnabledNumberOfBarsInOneDirection;

    /// <summary>
    /// Gets or sets a value indicating whether [is enabled number of bars in one direction].
    /// </summary>
    /// <value><c>true</c> if [is enabled number of bars in one direction]; otherwise, <c>false</c>.</value>
    public bool IsEnabledNumberOfBarsInOneDirection
    {
      get => _isEnabledNumberOfBarsInOneDirection;
      set
      {
        if (!(_isEnabledNumberOfBarsInOneDirection == value))
        {
          _isEnabledNumberOfBarsInOneDirection = value;
          OnPropertyChanged(nameof(IsEnabledNumberOfBarsInOneDirection));
        }
      }
    }


    private int _barShiftMaxItemsInOneDirection;

    /// <summary>
    /// Gets or sets the maximum number of items in one direction for bar shift.
    /// </summary>
    /// <value>The maximum number of items in one direction for bar shift.</value>
    public int BarShiftMaxItemsInOneDirection
    {
      get => _barShiftMaxItemsInOneDirection;
      set
      {
        if (!(_barShiftMaxItemsInOneDirection == value))
        {
          _barShiftMaxItemsInOneDirection = value;
          OnPropertyChanged(nameof(BarShiftMaxItemsInOneDirection));
        }
      }
    }


    /// <summary>
    /// Gets or sets the g ap en vi ro nm en t.
    /// </summary>
    public QuantityWithUnitGuiEnvironment GapEnvironment => RelationEnvironment.Instance;



    private DimensionfulQuantity _innerGapX;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public DimensionfulQuantity InnerGapX
    {
      get => _innerGapX;
      set
      {
        if (!(_innerGapX == value))
        {
          _innerGapX = value;
          OnPropertyChanged(nameof(InnerGapX));
        }
      }
    }

    private DimensionfulQuantity _outerGapX;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public DimensionfulQuantity OuterGapX
    {
      get => _outerGapX;
      set
      {
        if (!(_outerGapX == value))
        {
          _outerGapX = value;
          OnPropertyChanged(nameof(OuterGapX));
        }
      }
    }
    private DimensionfulQuantity _innerGapY;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public DimensionfulQuantity InnerGapY
    {
      get => _innerGapY;
      set
      {
        if (!(_innerGapY == value))
        {
          _innerGapY = value;
          OnPropertyChanged(nameof(InnerGapY));
        }
      }
    }
    private DimensionfulQuantity _outerGapY;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public DimensionfulQuantity OuterGapY
    {
      get => _outerGapY;
      set
      {
        if (!(_outerGapY == value))
        {
          _outerGapY = value;
          OnPropertyChanged(nameof(OuterGapY));
        }
      }
    }

    private bool _usePhysicalBaseValueV;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public bool UsePhysicalBaseValueV
    {
      get => _usePhysicalBaseValueV;
      set
      {
        if (!(_usePhysicalBaseValueV == value))
        {
          _usePhysicalBaseValueV = value;
          OnPropertyChanged(nameof(UsePhysicalBaseValueV));
        }
      }
    }

    /// <summary>
    /// Gets or sets the l og ic al ba se va lu ev en vi ro nm en t.
    /// </summary>
    public QuantityWithUnitGuiEnvironment LogicalBaseValueVEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _logicalBaseValueV;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public DimensionfulQuantity LogicalBaseValueV
    {
      get => _logicalBaseValueV;
      set
      {
        if (!(_logicalBaseValueV == value))
        {
          _logicalBaseValueV = value;
          OnPropertyChanged(nameof(LogicalBaseValueV));
        }
      }
    }


    private bool _startAtPreviousItem;

    /// <summary>
    /// Gets or sets a value indicating whether [start at previous item].
    /// </summary>
    /// <value><c>true</c> if [start at previous item]; otherwise, <c>false</c>.</value>
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

    /// <summary>
    /// Gets or sets the g ap ve nv ir on me nt.
    /// </summary>
    public QuantityWithUnitGuiEnvironment GapVEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _gapV;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public DimensionfulQuantity GapV
    {
      get => _gapV;
      set
      {
        if (!(_gapV == value))
        {
          _gapV = value;
          OnPropertyChanged(nameof(GapV));
        }
      }
    }





    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
        PenController = new PenAllPropertiesController(_doc.Pen)
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor)
        };

        BarShiftStrategy = new ItemsController<BarShiftStrategy3D>(new SelectableListNodeList(_doc.BarShiftStrategy), EhBarShiftStrategyChanged);

        IndependentColor = _doc.IndependentColor;
        UseUniformCrossSectionThickness = _doc.UseUniformCrossSectionThickness;

        BarShiftStrategy = _barShiftStrategy;

        IsEnabledNumberOfBarsInOneDirection = _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstXThenY || _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstYThenX;
        BarShiftMaxItemsInOneDirection = _doc.BarShiftMaxItemsInOneDirection;

        InnerGapX = new DimensionfulQuantity(_doc.InnerGapX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapEnvironment.DefaultUnit);
        OuterGapX = new DimensionfulQuantity(_doc.OuterGapX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapEnvironment.DefaultUnit);
        InnerGapY = new DimensionfulQuantity(_doc.InnerGapY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapEnvironment.DefaultUnit);
        OuterGapY = new DimensionfulQuantity(_doc.OuterGapY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapEnvironment.DefaultUnit);
        UsePhysicalBaseValueV = _doc.UsePhysicalBaseValue;
        LogicalBaseValueV = new DimensionfulQuantity(_doc.UsePhysicalBaseValue ? 0 : _doc.BaseValue, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LogicalBaseValueVEnvironment.DefaultUnit);
        StartAtPreviousItem = _doc.StartAtPreviousItem;
        GapV = new DimensionfulQuantity(_doc.PreviousItemGapV, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapVEnvironment.DefaultUnit);
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      _doc.IndependentColor = IndependentColor;
      _doc.Pen = PenController.Pen;
      _doc.UseUniformCrossSectionThickness = UseUniformCrossSectionThickness;

      _doc.BarShiftStrategy = BarShiftStrategy.SelectedValue;
      _doc.BarShiftMaxItemsInOneDirection = BarShiftMaxItemsInOneDirection;

      _doc.InnerGapX = InnerGapX.AsValueInSIUnits;
      _doc.OuterGapX = OuterGapX.AsValueInSIUnits;
      _doc.InnerGapY = InnerGapY.AsValueInSIUnits;
      _doc.OuterGapY = OuterGapY.AsValueInSIUnits;

      _doc.UsePhysicalBaseValue = UsePhysicalBaseValueV;
      if (UsePhysicalBaseValueV)
      {
        // who can parse this string? Only the v (z) -scale know how to parse it
      }
      else
      {
        _doc.BaseValue = LogicalBaseValueV.AsValueInSIUnits;
      }

      _doc.StartAtPreviousItem = StartAtPreviousItem;
      _doc.PreviousItemGapV = GapV.AsValueInSIUnits;

      return ApplyEnd(true, disposeController);
    }

    private void EhColorGroupStyleAddedOrRemoved()
    {
      _doc.IndependentColor = IndependentColor;
      PenController.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
    }

    private void EhBarShiftStrategyChanged(BarShiftStrategy3D newValue)
    {

      _doc.BarShiftStrategy = newValue;
      IsEnabledNumberOfBarsInOneDirection = _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstXThenY || _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstYThenX;
    }

    private void EhIndependentColorChanged()
    {
      _doc.IndependentColor = IndependentColor;
      PenController.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
    }
  }
}
