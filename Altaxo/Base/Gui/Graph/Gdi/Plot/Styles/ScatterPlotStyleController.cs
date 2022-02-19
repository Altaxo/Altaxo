#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph.Graph2D.Plot.Styles;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{

  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotScatterStyle
  /// </summary>
  public interface IScatterPlotStyleView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Summary description for XYPlotScatterStyleController.
  /// </summary>
  [UserControllerForObject(typeof(ScatterPlotStyle))]
  [ExpectedTypeOfView(typeof(IScatterPlotStyleView))]
  public class ScatterPlotStyleController : MVCANControllerEditOriginalDocBase<ScatterPlotStyle, IScatterPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;



    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_overriddenPlotColorInfluence, () => OverriddenPlotColorInfluence = null);
    }

    public ScatterPlotStyleController()
    {
      CmdCreateNewScatterSymbolSet = new RelayCommand(EhCreateNewSymbolSetFromOverrides);
    }

    #region Bindings

    private bool _enableMain = true;

    public bool EnableMain
    {
      get => _enableMain;
      set
      {
        if (!(_enableMain == value))
        {
          _enableMain = value;
          OnPropertyChanged(nameof(EnableMain));
        }
      }
    }


    private bool _independentSkipFrequency;

    public bool IndependentSkipFrequency
    {
      get => _independentSkipFrequency;
      set
      {
        if (!(_independentSkipFrequency == value))
        {
          _independentSkipFrequency = value;
          OnPropertyChanged(nameof(IndependentSkipFrequency));
        }
      }
    }

    private int _skipFrequency;

    public int SkipFrequency
    {
      get => _skipFrequency;
      set
      {
        if (!(_skipFrequency == value))
        {
          _skipFrequency = value;
          OnPropertyChanged(nameof(SkipFrequency));
        }
      }
    }

    private bool _ignoreMissingDataPoints;

    public bool IgnoreMissingDataPoints
    {
      get => _ignoreMissingDataPoints;
      set
      {
        if (!(_ignoreMissingDataPoints == value))
        {
          _ignoreMissingDataPoints = value;
          OnPropertyChanged(nameof(IgnoreMissingDataPoints));
        }
      }
    }


    private bool _independentOnShiftingGroupStyles;

    public bool IndependentOnShiftingGroupStyles
    {
      get => _independentOnShiftingGroupStyles;
      set
      {
        if (!(_independentOnShiftingGroupStyles == value))
        {
          _independentOnShiftingGroupStyles = value;
          OnPropertyChanged(nameof(IndependentOnShiftingGroupStyles));
        }
      }
    }

    private bool _independentScatterSymbol;

    public bool IndependentScatterSymbol
    {
      get => _independentScatterSymbol;
      set
      {
        if (!(_independentScatterSymbol == value))
        {
          _independentScatterSymbol = value;
          OnPropertyChanged(nameof(IndependentScatterSymbol));
        }
      }
    }



    /// <summary>
    /// Initializes the symbol shape combobox.
    /// </summary>
    private IScatterSymbol _scatterSymbol;

    public IScatterSymbol ScatterSymbol
    {
      get => _scatterSymbol;
      set
      {
        if (!(_scatterSymbol == value))
        {
          _scatterSymbol = value;
          OnPropertyChanged(nameof(ScatterSymbol));
          EhScatterSymbolChanged();
        }
      }
    }

    private ItemsController<IScatterSymbol> _similarSymbolChoices;

    /// <summary>List with symbol lists containing the same order of symbol shapes. The text here is the name of the symbol list (!),
    /// whereas the tag is the symbol with the same shape as the currently choosen shape. Because of this, the content must change every time
    /// the user choose another symbol</summary>
    public ItemsController<IScatterSymbol> SimilarSymbolChoices
    {
      get => _similarSymbolChoices;
      set
      {
        if (!(_similarSymbolChoices == value))
        {
          _similarSymbolChoices?.Dispose();
          _similarSymbolChoices = value;
          OnPropertyChanged(nameof(SimilarSymbolChoices));
        }
      }
    }


    private bool _independentColor;

    public bool IndependentColor
    {
      get => _independentColor;
      set
      {
        if (!(_independentColor == value))
        {
          _independentColor = value;
          OnPropertyChanged(nameof(IndependentColor));
          EhIndependentColorChanged(value);
        }
      }
    }

    private void EhIndependentColorChanged(bool value)
    {
      _doc.IndependentColor = value;
      ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
    }
    private void EhIndependentColorChanged() => EhIndependentColorChanged(IndependentColor);


    private NamedColor _color;
    /// <summary>
    /// Material for the scatter symbol.
    /// </summary>
    public NamedColor Color
    {
      get => _color;
      set
      {
        if (!(_color == value))
        {
          _color = value;
          OnPropertyChanged(nameof(Color));
        }
      }
    }

    /// <summary>
    /// Indicates, whether only colors of plot color sets should be shown.
    /// </summary>
    private bool _showPlotColorsOnly;

    public bool ShowPlotColorsOnly
    {
      get => _showPlotColorsOnly;
      set
      {
        if (!(_showPlotColorsOnly == value))
        {
          _showPlotColorsOnly = value;
          OnPropertyChanged(nameof(ShowPlotColorsOnly));
        }
      }
    }




    private bool _independentSymbolSize;
    /// <summary>
    /// Initializes the independent symbol size check box.
    /// </summary>
    public bool IndependentSymbolSize
    {
      get => _independentSymbolSize;
      set
      {
        if (!(_independentSymbolSize == value))
        {
          _independentSymbolSize = value;
          OnPropertyChanged(nameof(IndependentSymbolSize));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment SymbolSizeEnvironment => LineCapSizeEnvironment.Instance;


    private DimensionfulQuantity _symbolSize;
    /// <summary>
    /// Initializes the symbol size combobox.
    /// </summary>
    public DimensionfulQuantity SymbolSize
    {
      get => _symbolSize;
      set
      {
        if (!(_symbolSize == value))
        {
          _symbolSize = value;
          OnPropertyChanged(nameof(SymbolSize));
        }
      }
    }


    private bool _overrideInset;

    public bool OverrideInset
    {
      get => _overrideInset;
      set
      {
        if (!(_overrideInset == value))
        {
          _overrideInset = value;
          OnPropertyChanged(nameof(OverrideInset));
        }
      }
    }

    private ItemsController<Type?> _insets;

    public ItemsController<Type?> Insets
    {
      get => _insets;
      set
      {
        if (!(_insets == value))
        {
          _insets = value;
          OnPropertyChanged(nameof(Insets));
        }
      }
    }


    private bool _overrideFrame;

    public bool OverrideFrame
    {
      get => _overrideFrame;
      set
      {
        if (!(_overrideFrame == value))
        {
          _overrideFrame = value;
          OnPropertyChanged(nameof(OverrideFrame));
        }
      }
    }

    private ItemsController<Type?> _frames;

    public ItemsController<Type?> Frames
    {
      get => _frames;
      set
      {
        if (!(_frames == value))
        {
          _frames?.Dispose();
          _frames = value;
          OnPropertyChanged(nameof(Frames));
        }
      }
    }



    private bool _overrideAbsoluteStructureWidth;

    public bool OverrideAbsoluteStructureWidth
    {
      get => _overrideAbsoluteStructureWidth;
      set
      {
        if (!(_overrideAbsoluteStructureWidth == value))
        {
          _overrideAbsoluteStructureWidth = value;
          OnPropertyChanged(nameof(OverrideAbsoluteStructureWidth));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment AbsoluteWidthEnvironment => SizeEnvironment.Instance;

    private DimensionfulQuantity _overriddenAbsoluteStructureWidth;

    public DimensionfulQuantity OverriddenAbsoluteStructureWidth
    {
      get => _overriddenAbsoluteStructureWidth;
      set
      {
        if (!(_overriddenAbsoluteStructureWidth == value))
        {
          _overriddenAbsoluteStructureWidth = value;
          OnPropertyChanged(nameof(OverriddenAbsoluteStructureWidth));
        }
      }
    }


    private bool _overrideRelativeStructureWidth;

    public bool OverrideRelativeStructureWidth
    {
      get => _overrideRelativeStructureWidth;
      set
      {
        if (!(_overrideRelativeStructureWidth == value))
        {
          _overrideRelativeStructureWidth = value;
          OnPropertyChanged(nameof(OverrideRelativeStructureWidth));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment RelativeWidthEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _overriddenRelativeStructureWidth;

    public DimensionfulQuantity OverriddenRelativeStructureWidth
    {
      get => _overriddenRelativeStructureWidth;
      set
      {
        if (!(_overriddenRelativeStructureWidth == value))
        {
          _overriddenRelativeStructureWidth = value;
          OnPropertyChanged(nameof(OverriddenRelativeStructureWidth));
        }
      }
    }



    private bool _overridePlotColorInfluence;

    public bool OverridePlotColorInfluence
    {
      get => _overridePlotColorInfluence;
      set
      {
        if (!(_overridePlotColorInfluence == value))
        {
          _overridePlotColorInfluence = value;
          OnPropertyChanged(nameof(OverridePlotColorInfluence));
        }
      }
    }


    private PlotColorInfluenceController _overriddenPlotColorInfluence;

    public PlotColorInfluenceController OverriddenPlotColorInfluence
    {
      get => _overriddenPlotColorInfluence;
      set
      {
        if (!(_overriddenPlotColorInfluence == value))
        {
          _overriddenPlotColorInfluence?.Dispose();
          _overriddenPlotColorInfluence = value;
          OnPropertyChanged(nameof(OverriddenPlotColorInfluence));
        }
      }
    }


    private bool _overrideFillColor;

    public bool OverrideFillColor
    {
      get => _overrideFillColor;
      set
      {
        if (!(_overrideFillColor == value))
        {
          _overrideFillColor = value;
          OnPropertyChanged(nameof(OverrideFillColor));
        }
      }
    }

    private NamedColor _OverriddenFillColor;

    public NamedColor OverriddenFillColor
    {
      get => _OverriddenFillColor;
      set
      {
        if (!(_OverriddenFillColor == value))
        {
          _OverriddenFillColor = value;
          OnPropertyChanged(nameof(OverriddenFillColor));
        }
      }
    }


    private bool _overrideFrameColor;

    public bool OverrideFrameColor
    {
      get => _overrideFrameColor;
      set
      {
        if (!(_overrideFrameColor == value))
        {
          _overrideFrameColor = value;
          OnPropertyChanged(nameof(OverrideFrameColor));
        }
      }
    }

    private NamedColor _overriddenFrameColor;

    public NamedColor OverriddenFrameColor
    {
      get => _overriddenFrameColor;
      set
      {
        if (!(_overriddenFrameColor == value))
        {
          _overriddenFrameColor = value;
          OnPropertyChanged(nameof(OverriddenFrameColor));
        }
      }
    }


    private bool _overrideInsetColor;

    public bool OverrideInsetColor
    {
      get => _overrideInsetColor;
      set
      {
        if (!(_overrideInsetColor == value))
        {
          _overrideInsetColor = value;
          OnPropertyChanged(nameof(OverrideInsetColor));
        }
      }
    }

    private NamedColor _overriddenInsetColor;

    public NamedColor OverriddenInsetColor
    {
      get => _overriddenInsetColor;
      set
      {
        if (!(_overriddenInsetColor == value))
        {
          _overriddenInsetColor = value;
          OnPropertyChanged(nameof(OverriddenInsetColor));
        }
      }
    }

    public ICommand CmdCreateNewScatterSymbolSet { get; }

    private bool _areOverridesExpanded;

    /// <summary>
    /// Reflects the state of the overrides expander control (has no influence on the document)
    /// </summary>
    public bool AreOverridesExpanded
    {
      get => _areOverridesExpanded;
      set
      {
        if (!(_areOverridesExpanded == value))
        {
          _areOverridesExpanded = value;
          OnPropertyChanged(nameof(AreOverridesExpanded));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

        {
          // Frame
          var frameTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolFrame));
          var symbolFrameChoices = new SelectableListNodeList
        {
          new SelectableListNode("No frame", null, _doc.OverriddenFrame is null)
        };
          foreach (var ty in frameTypes)
          {
            symbolFrameChoices.Add(new SelectableListNode(ty.Name, ty, _doc.OverriddenFrame?.GetType() == ty));
          }

          Frames = new ItemsController<Type?>(symbolFrameChoices);
        }

        {
          // Insets
          var insetTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolInset));
          var symbolInsetChoices = new SelectableListNodeList
            {
            new SelectableListNode("No inset", null, _doc.OverriddenInset is null)
            };
          foreach (var ty in insetTypes)
          {
            symbolInsetChoices.Add(new SelectableListNode(ty.Name, ty, _doc.OverriddenInset?.GetType() == ty));
          }
          Insets = new ItemsController<Type?>(symbolInsetChoices);
        }

        OverrideFrame = _doc.OverrideFrame;
        OverrideInset = _doc.OverrideInset;

        OverrideAbsoluteStructureWidth = _doc.OverrideStructureWidthOffset.HasValue;
        OverriddenAbsoluteStructureWidth = new DimensionfulQuantity(_doc.OverrideStructureWidthOffset ?? 0, Altaxo.Units.Length.Point.Instance).AsQuantityIn(AbsoluteWidthEnvironment.DefaultUnit);

        OverrideRelativeStructureWidth = _doc.OverrideStructureWidthFactor.HasValue;
        OverriddenRelativeStructureWidth = new DimensionfulQuantity(_doc.OverrideStructureWidthFactor ?? 0, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelativeWidthEnvironment.DefaultUnit);

        OverridePlotColorInfluence = _doc.OverridePlotColorInfluence.HasValue;
        OverriddenPlotColorInfluence = new PlotColorInfluenceController(_doc.OverridePlotColorInfluence ?? PlotColorInfluence.None);

        OverrideFillColor = _doc.OverrideFillColor.HasValue;
        OverriddenFillColor = _doc.OverrideFillColor ?? _doc.ScatterSymbol.FillColor;
        OverrideFrameColor = _doc.OverrideFrameColor.HasValue;
        OverriddenFrameColor = _doc.OverrideFrameColor ?? _doc.ScatterSymbol.Frame?.Color ?? NamedColors.Transparent;
        OverrideInsetColor = _doc.OverrideInsetColor.HasValue;
        OverriddenInsetColor = _doc.OverrideInsetColor ?? _doc.ScatterSymbol.Inset?.Color ?? NamedColors.Transparent;

        IndependentSkipFrequency = _doc.IndependentSkipFrequency;
        SkipFrequency = _doc.SkipFrequency;
        IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;


        // now we have to set all dialog elements to the right values
        IndependentColor = _doc.IndependentColor;
        ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        Color = _doc.Color;

        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = new DimensionfulQuantity(_doc.SymbolSize, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);

        IndependentScatterSymbol = _doc.IndependentScatterSymbol;
        ScatterSymbol = _doc.ScatterSymbol;
      }
    }

    public override bool Apply(bool disposeController)
    {
      // bool applyResult = true;
      // don't trust user input, so all into a try statement
      try
      {
        // Skip points

        _doc.IndependentSkipFrequency = IndependentSkipFrequency;
        _doc.SkipFrequency = SkipFrequency;
        _doc.IgnoreMissingDataPoints = IgnoreMissingDataPoints;
        _doc.IndependentOnShiftingGroupStyles = IndependentOnShiftingGroupStyles;

        // Symbol Shape
        _doc.IndependentScatterSymbol = IndependentScatterSymbol;
        _doc.ScatterSymbol = ScatterSymbol;

        // Symbol Color
        _doc.IndependentColor = IndependentColor;
        _doc.Color = Color;

        // Symbol Size
        _doc.IndependentSymbolSize = IndependentSymbolSize;
        _doc.SymbolSize = SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);

        _doc.OverrideFrame = OverrideFrame;
        _doc.OverriddenFrame = Frames.SelectedValue is null ? null : (IScatterSymbolFrame)Activator.CreateInstance(Frames.SelectedValue);

        _doc.OverrideInset = OverrideInset;
        _doc.OverriddenInset = Insets.SelectedValue is null ? null : (IScatterSymbolInset)Activator.CreateInstance(Insets.SelectedValue);

        _doc.OverrideStructureWidthOffset = OverrideAbsoluteStructureWidth ? OverriddenAbsoluteStructureWidth.AsValueIn(Altaxo.Units.Length.Point.Instance) : (double?)null;
        _doc.OverrideStructureWidthFactor = OverrideRelativeStructureWidth ? OverriddenRelativeStructureWidth.AsValueInSIUnits : (double?)null;

        _doc.OverridePlotColorInfluence = OverridePlotColorInfluence ? (PlotColorInfluence)OverriddenPlotColorInfluence.ModelObject : (PlotColorInfluence?)null;
        _doc.OverrideFillColor = OverrideFillColor ? OverriddenFillColor : (NamedColor?)null;
        _doc.OverrideFrameColor = OverrideFrameColor ? OverriddenFrameColor : (NamedColor?)null;
        _doc.OverrideInsetColor = OverrideInsetColor ? OverriddenInsetColor : (NamedColor?)null;

        if (!disposeController)
        {
          ScatterSymbol = _doc.ScatterSymbol;
          EhScatterSymbolChanged();
        }
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occured: " + ex.Message);
        return false;
      }

      return ApplyEnd(true, disposeController);
    }

    private void EhScatterSymbolChanged()
    {
      var symbol = ScatterSymbol;

      // Frame
      if (!OverrideFrame)
      {
        Frames.SelectedValue = symbol.Frame?.GetType();
      }

      // Inset
      if (!OverrideInset)
      {
        Insets.SelectedValue = symbol.Inset?.GetType();
      }

      // Structure width
      if (!OverrideRelativeStructureWidth)
        OverriddenRelativeStructureWidth = new DimensionfulQuantity(symbol.RelativeStructureWidth, Altaxo.Units.Dimensionless.Unity.Instance);

      // Plot color influence
      if (!OverridePlotColorInfluence)
        OverriddenPlotColorInfluence.InitializeDocument(symbol.PlotColorInfluence);

      // Fill color
      if (!OverrideFillColor)
        OverriddenFillColor = symbol.FillColor;

      // FrameColor
      if (!OverrideFrameColor && symbol.Frame is not null)
        OverriddenFrameColor = symbol.Frame.Color;

      // InsetColor
      if (!OverrideInsetColor && symbol.Inset is not null)
        OverriddenInsetColor = symbol.Inset.Color;

      // Initialize the list of similar symbols
      Initialize_SimilarSymbolList();
    }

    private void EhCreateNewSymbolSetFromOverrides()
    {
      var newSymbol = CreateNewSymbolSetFromOverrides(_doc.ScatterSymbol, out var cancellationRequested);
      ClearAllOverridesThatAreEqualToScatterSymbol(newSymbol);
      _doc.ScatterSymbol = newSymbol;
      ScatterSymbol = newSymbol;
      EhScatterSymbolChanged();
    }

    private void ClearAllOverridesThatAreEqualToScatterSymbol(IScatterSymbol symbol)
    {
      if (symbol.Frame?.GetType() == Frames.SelectedValue)
        OverrideFrame = false;

      if (symbol.Inset?.GetType() == Insets.SelectedValue)
        OverrideInset = false;

      if ((OverrideRelativeStructureWidth == false || symbol.RelativeStructureWidth == OverriddenRelativeStructureWidth.AsValueInSIUnits) && (OverrideAbsoluteStructureWidth == false || OverriddenAbsoluteStructureWidth.AsValueIn(Altaxo.Units.Length.Point.Instance) == 0))
      {
        OverrideAbsoluteStructureWidth = false;
        OverrideRelativeStructureWidth = false;
      }

      if (symbol.PlotColorInfluence == OverriddenPlotColorInfluence.Doc)
        OverridePlotColorInfluence = false;

      if (symbol.FillColor == OverriddenFillColor)
        OverrideFillColor = false;

      if (symbol.Frame is null || symbol.Frame.Color == OverriddenFrameColor)
        OverrideFrameColor = false;

      if (symbol.Inset is null || symbol.Inset.Color == OverriddenInsetColor)
        OverrideInsetColor = false;
    }

    private IScatterSymbol CreateNewSymbolSetFromOverrides(IScatterSymbol symbol, out bool cancellationRequested)
    {
      cancellationRequested = false;

      double overriddenAbsoluteStructureWidth = OverrideAbsoluteStructureWidth ? OverriddenAbsoluteStructureWidth.AsValueIn(Altaxo.Units.Length.Point.Instance) : 0;
      double overriddenRelativeStructureWidth = OverrideRelativeStructureWidth ? OverriddenRelativeStructureWidth.AsValueInSIUnits : symbol.RelativeStructureWidth;

      double resultingRelativeStructureWidth = overriddenRelativeStructureWidth;

      if (OverrideAbsoluteStructureWidth && OverriddenAbsoluteStructureWidth.AsValueIn(Altaxo.Units.Length.Point.Instance) != 0)
      {
        if (overriddenRelativeStructureWidth <= 0)
        {
          var dlgResult = Current.Gui.YesNoCancelMessageBox(
            "Currently the absolute structure width has been overriden.\r\n" +
            "However, in the new symbol set to be created, only the relative structure width can be stored.\r\n" +
            "This is especially problematic, since the relative structure width is set to 0 (zero).\r\n" +
            "Do you want to convert the absolute structure width into a relative value?\r\n" +
            "Yes:    Converts absolute structure width into relative width, using current symbol size\r\n" +
            "No:     Sets relative structure width to zero (probably not very useful)\r\n" +
            "Cancel: Cancels the creation of a new symbol set",
            "Question concerning absolute/relative structure width",
            true);

          if (dlgResult is null)
          {
            cancellationRequested = true;
            return symbol;
          }
          else if (true == dlgResult)
          {
            resultingRelativeStructureWidth = overriddenAbsoluteStructureWidth / SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);
          }
          else
          {
            resultingRelativeStructureWidth = 0;
          }
        }
        else
        {
          var dlgResult = Current.Gui.YesNoCancelMessageBox(
            "Currently the absolute structure width has been overriden.\r\n" +
            "However, in the new symbol set to be created, only the relative structure width can be stored.\r\n" +
            "Do you want to take both the absolute and the relative structure width into account?\r\n" +
            "Yes:    Converts the combined absolute and relative structure width into relative width, using current symbol size\r\n" +
            "No:     Sets relative structure, using the overridden relative structure width or the default value.\r\n" +
            "Cancel: Cancels the creation of a new symbol set",
            "Question concerning absolute/relative structure width",
            false);

          if (dlgResult is null)
          {
            cancellationRequested = true;
            return symbol;
          }
          else if (true == dlgResult)
          {
            resultingRelativeStructureWidth = overriddenRelativeStructureWidth + overriddenAbsoluteStructureWidth / SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);
          }
          else
          {
            resultingRelativeStructureWidth = overriddenRelativeStructureWidth;
          }
        }
      }

      // we have to create a new symbol - if IsIndependent symbol is not checked, we have to create a new series of symbols

      bool createNewSymbolList;
      int originalItemIndex;
      IEnumerable<IScatterSymbol> scatterSymbolsToModify;
      if (IndependentScatterSymbol)
      {
        scatterSymbolsToModify = new IScatterSymbol[] { symbol };
        originalItemIndex = 0;
        createNewSymbolList = false;
      }
      else
      {
        var parentList = ScatterSymbolListManager.Instance.GetParentList(symbol);
        if (parentList is not null)
        {
          scatterSymbolsToModify = parentList;
          originalItemIndex = parentList.IndexOf(symbol);
          createNewSymbolList = true;
        }
        else
        {
          scatterSymbolsToModify = new IScatterSymbol[] { symbol };
          originalItemIndex = 0;
          createNewSymbolList = false;
        }
      }

      var newSymbols = new List<IScatterSymbol>();
      foreach (var symbolToModify in scatterSymbolsToModify)
      {
        var newSymbol = symbolToModify;

        if (OverrideInset)
        {
          var newInsetType = Insets.SelectedValue;
          if (newInsetType != newSymbol.Inset?.GetType())
          {
            var newInset = newInsetType is null ? null : (IScatterSymbolInset)Activator.CreateInstance(newInsetType);
            newSymbol = newSymbol.WithInset(newInset);
          }
        }

        if (OverrideFrame)
        {
          var newFrameType = Frames.SelectedValue;
          if (newFrameType != newSymbol.Frame?.GetType())
          {
            var newFrame = newFrameType is null ? null : (IScatterSymbolFrame)Activator.CreateInstance(newFrameType);
            newSymbol = newSymbol.WithFrame(newFrame);
          }
        }

        if (OverrideRelativeStructureWidth || OverrideRelativeStructureWidth)
          newSymbol = newSymbol.WithRelativeStructureWidth(resultingRelativeStructureWidth);

        if (OverridePlotColorInfluence)
          newSymbol = newSymbol.WithPlotColorInfluence(OverriddenPlotColorInfluence.Doc);

        if (OverrideFillColor)
          newSymbol = newSymbol.WithFillColor(OverriddenFillColor);

        if (OverrideFrameColor && newSymbol.Frame is not null)
          newSymbol = newSymbol.WithFrame(newSymbol.Frame.WithColor(OverriddenFrameColor));

        if (OverrideInsetColor && newSymbol.Inset is not null)
          newSymbol = newSymbol.WithInset(newSymbol.Inset.WithColor(OverriddenInsetColor));

        newSymbols.Add(newSymbol);
      }

      if (createNewSymbolList)
      {
        if (ScatterSymbolListManager.Instance.TryGetListByMembers(newSymbols, null, out var existingListName))
        {
          Current.Gui.InfoMessageBox("A symbol set with the chosen parameters already exists under the name: " + existingListName, "Symbol set exists");
          return ScatterSymbolListManager.Instance.GetList(existingListName)[originalItemIndex];
        }
        else
        {
          string newName = "Custom";
          if (!Current.Gui.ShowDialog(ref newName, "Enter a name for the new scatter symbol set", false))
          {
            cancellationRequested = true;
            return symbol;
          }

          var newScatterSymbolList = new ScatterSymbolList(newName, newSymbols);
          ScatterSymbolListManager.Instance.TryRegisterList(newScatterSymbolList, Altaxo.Main.ItemDefinitionLevel.Project, out var resultList);
          // return the item at the original list index.
          return resultList[originalItemIndex];
        }
      }
      else
      {
        if (ScatterSymbolListManager.Instance.TryFindListContaining(newSymbols[originalItemIndex], out var dummyList, out var result))
          return result;
        else
          return newSymbols[originalItemIndex];
      }
    }

    #region Similar symbols

    private bool HasSameShapes(ScatterSymbolList l1, ScatterSymbolList l2)
    {
      if (l1.Count != l2.Count)
        return false;

      for (int i = 0; i < l1.Count; ++i)
      {
        if (l1[i].GetType() != l2[i].GetType())
          return false;
      }

      return true;
    }

    private IScatterSymbol _lastSymbolForWhichSimilarSetsWhereSearched;

    /// <summary>
    /// Initializes the list with symbol sets that are similar (== have the same shapes in the same order) as the symbol set that
    /// is currently chosen. The text of the SelectableListNode contains the name of the symbol list (!), whereas the tag
    /// contains the symbol with the same shape, but from the other symbol set.
    /// </summary>
    private void Initialize_SimilarSymbolList()
    {
      var currentSymbol = ScatterSymbol ?? _doc.ScatterSymbol;

      if (object.ReferenceEquals(currentSymbol, _lastSymbolForWhichSimilarSetsWhereSearched))
        return; // then _similarSymbolChoices is already up-to-date
      _lastSymbolForWhichSimilarSetsWhereSearched = currentSymbol;



      var currentList = ScatterSymbolListManager.Instance.GetParentList(currentSymbol);
      if (currentList is null)
        return; // return with an empty _similarSymbolChoices

      var currentIndex = currentList.IndexOf(currentSymbol); // index of the current symbol in the symbol set

      // we need a sorted dictionary in order to sort first by level, then by name.
      var list = new List<Tuple<string, ScatterSymbolList>>();

      foreach (var entry in ScatterSymbolListManager.Instance.GetEntryValues())
      {
        if (HasSameShapes(currentList, entry.List))
        {
          list.Add(new Tuple<string, ScatterSymbolList>(ScatterSymbolListManager.Instance.GetListLevelName(entry.Level) + "\\" + entry.List.Name, entry.List));
        }
      }

      // now bring this into the SelectableListNodeList
      var similarSymbolChoices = new SelectableListNodeList();
      foreach (var entry in list)
      {
        similarSymbolChoices.Add(new SelectableListNode(
          entry.Item1,
          entry.Item2[currentIndex], object.ReferenceEquals(currentList, entry.Item2)));
      }
      SimilarSymbolChoices = new ItemsController<IScatterSymbol>(similarSymbolChoices, EhSimilarShapeChosen);
    }

    private void EhSimilarShapeChosen(IScatterSymbol value)
    {
      if (value is IScatterSymbol newSymbol)
      {
        ScatterSymbol = newSymbol;
      }
    }

    #endregion Similar symbols
  } // end of class XYPlotScatterStyleController
} // end of namespace
