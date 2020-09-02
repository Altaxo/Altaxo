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
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Frames;
using Altaxo.Gui.Graph.Plot.Groups;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  #region Interfaces

  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotScatterStyle
  /// </summary>
  public interface IScatterPlotStyleView
  {
    /// <summary>
    /// Material for the scatter symbol.
    /// </summary>
    NamedColor Color { get; set; }

    /// <summary>
    /// Indicates, whether only colors of plot color sets should be shown.
    /// </summary>
    bool ShowPlotColorsOnly { set; }

    /// <summary>
    /// Initializes the symbol size combobox.
    /// </summary>
    double SymbolSize { get; set; }

    /// <summary>
    /// Initializes the independent symbol size check box.
    /// </summary>
    bool IndependentSymbolSize { get; set; }

    bool IndependentScatterSymbol { get; set; }

    /// <summary>
    /// Initializes the symbol shape combobox.
    /// </summary>
    IScatterSymbol ScatterSymbol { get; set; }

    SelectableListNodeList Frame { set; }

    SelectableListNodeList Inset { set; }

    SelectableListNodeList SimilarSymbols { set; }

    bool IndependentColor { get; set; }

    bool IndependentSkipFrequency { get; set; }

    int SkipFrequency { get; set; }

    bool IgnoreMissingDataPoints { get; set; }

    bool IndependentOnShiftingGroupStyles { get; set; }

    bool OverrideInset { get; set; }

    bool OverrideFrame { get; set; }

    bool OverrideAbsoluteStructureWidth { get; set; }

    double OverriddenAbsoluteStructureWidth { get; set; }

    bool OverrideRelativeStructureWidth { get; set; }

    double OverriddenRelativeStructureWidth { get; set; }

    bool OverridePlotColorInfluence { get; set; }
    PlotColorInfluence OverriddenPlotColorInfluence { get; set; }

    bool OverrideFillColor { get; set; }
    NamedColor OverriddenFillColor { get; set; }
    bool OverrideFrameColor { get; set; }
    NamedColor OverriddenFrameColor { get; set; }
    bool OverrideInsetColor { get; set; }
    NamedColor OverriddenInsetColor { get; set; }

    #region events

    event Action IndependentColorChanged;

    event Action ScatterSymbolChanged;

    event Action CreateNewSymbolSetFromOverrides;

    event Action SimilarSymbolSetChosen;

    #endregion events
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for XYPlotScatterStyleController.
  /// </summary>
  [UserControllerForObject(typeof(ScatterPlotStyle))]
  [ExpectedTypeOfView(typeof(IScatterPlotStyleView))]
  public class ScatterPlotStyleController : MVCANControllerEditOriginalDocBase<ScatterPlotStyle, IScatterPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    private SelectableListNodeList _symbolInsetChoices;
    private SelectableListNodeList _symbolFrameChoices;

    /// <summary>List with symbol lists containing the same order of symbol shapes. The text here is the name of the symbol list (!),
    /// whereas the tag is the symbol with the same shape as the currently choosen shape. Because of this, the content must change every time
    /// the user choose another symbol</summary>
    private SelectableListNodeList _similarSymbolChoices;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break; // no subcontrollers
    }

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _symbolInsetChoices = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

        // Frame
        var frameTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolFrame));
        _symbolFrameChoices = new SelectableListNodeList
        {
          new SelectableListNode("No frame", null, false)
        };
        foreach (var ty in frameTypes)
        {
          _symbolFrameChoices.Add(new SelectableListNode(ty.Name, ty, false));
        }

        // Insets
        var insetTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolInset));
        _symbolInsetChoices = new SelectableListNodeList
        {
          new SelectableListNode("No inset", null, false)
        };
        foreach (var ty in insetTypes)
        {
          _symbolInsetChoices.Add(new SelectableListNode(ty.Name, ty, false));
        }
      }
      if (_view is not null)
      {
        _view.IndependentSkipFrequency = _doc.IndependentSkipFrequency;
        _view.SkipFrequency = _doc.SkipFrequency;
        _view.IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        _view.IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;

        _view.IndependentScatterSymbol = _doc.IndependentScatterSymbol;
        _view.ScatterSymbol = _doc.ScatterSymbol;

        // now we have to set all dialog elements to the right values
        _view.IndependentColor = _doc.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        _view.Color = _doc.Color;

        _view.IndependentSymbolSize = _doc.IndependentSymbolSize;
        _view.SymbolSize = _doc.SymbolSize;

        _view.OverrideFrame = _doc.OverrideFrame;
        _symbolFrameChoices.ForEachDo(node => node.IsSelected = _doc.OverriddenFrame?.GetType() == (Type)node.Tag);
        _view.Frame = _symbolFrameChoices;

        _view.OverrideInset = _doc.OverrideInset;
        _symbolInsetChoices.ForEachDo(node => node.IsSelected = _doc.OverriddenInset?.GetType() == (Type)node.Tag);
        _view.Inset = _symbolInsetChoices;

        _view.OverrideAbsoluteStructureWidth = _doc.OverrideStructureWidthOffset.HasValue;
        _view.OverriddenAbsoluteStructureWidth = _doc.OverrideStructureWidthOffset ?? 0;

        _view.OverrideRelativeStructureWidth = _doc.OverrideStructureWidthFactor.HasValue;
        _view.OverriddenRelativeStructureWidth = _doc.OverrideStructureWidthFactor ?? 0;

        _view.OverridePlotColorInfluence = _doc.OverridePlotColorInfluence.HasValue;
        _view.OverriddenPlotColorInfluence = _doc.OverridePlotColorInfluence ?? PlotColorInfluence.None;

        _view.OverrideFillColor = _doc.OverrideFillColor.HasValue;
        _view.OverriddenFillColor = _doc.OverrideFillColor ?? _doc.ScatterSymbol.FillColor;
        _view.OverrideFrameColor = _doc.OverrideFrameColor.HasValue;
        _view.OverriddenFrameColor = _doc.OverrideFrameColor ?? _doc.ScatterSymbol.Frame?.Color ?? NamedColors.Transparent;
        _view.OverrideInsetColor = _doc.OverrideInsetColor.HasValue;
        _view.OverriddenInsetColor = _doc.OverrideInsetColor ?? _doc.ScatterSymbol.Inset?.Color ?? NamedColors.Transparent;

        EhScatterSymbolChanged();
      }
    }

    public override bool Apply(bool disposeController)
    {
      // bool applyResult = true;
      // don't trust user input, so all into a try statement
      try
      {
        // Skip points

        _doc.IndependentSkipFrequency = _view.IndependentSkipFrequency;
        _doc.SkipFrequency = _view.SkipFrequency;
        _doc.IgnoreMissingDataPoints = _view.IgnoreMissingDataPoints;
        _doc.IndependentOnShiftingGroupStyles = _view.IndependentOnShiftingGroupStyles;

        // Symbol Shape
        _doc.IndependentScatterSymbol = _view.IndependentScatterSymbol;
        _doc.ScatterSymbol = _view.ScatterSymbol;

        // Symbol Color
        _doc.IndependentColor = _view.IndependentColor;
        _doc.Color = _view.Color;

        // Symbol Size
        _doc.IndependentSymbolSize = _view.IndependentSymbolSize;
        _doc.SymbolSize = _view.SymbolSize;

        _doc.OverrideFrame = _view.OverrideFrame;
        _doc.OverriddenFrame = _symbolFrameChoices.FirstSelectedNode?.Tag is null ? null : (IScatterSymbolFrame)Activator.CreateInstance((Type)_symbolFrameChoices.FirstSelectedNode.Tag);

        _doc.OverrideInset = _view.OverrideInset;
        _doc.OverriddenInset = _symbolInsetChoices.FirstSelectedNode?.Tag is null ? null : (IScatterSymbolInset)Activator.CreateInstance((Type)_symbolInsetChoices.FirstSelectedNode.Tag);

        _doc.OverrideStructureWidthOffset = _view.OverrideAbsoluteStructureWidth ? _view.OverriddenAbsoluteStructureWidth : (double?)null;
        _doc.OverrideStructureWidthFactor = _view.OverrideRelativeStructureWidth ? _view.OverriddenRelativeStructureWidth : (double?)null;

        _doc.OverridePlotColorInfluence = _view.OverridePlotColorInfluence ? _view.OverriddenPlotColorInfluence : (PlotColorInfluence?)null;
        _doc.OverrideFillColor = _view.OverrideFillColor ? _view.OverriddenFillColor : (NamedColor?)null;
        _doc.OverrideFrameColor = _view.OverrideFrameColor ? _view.OverriddenFrameColor : (NamedColor?)null;
        _doc.OverrideInsetColor = _view.OverrideInsetColor ? _view.OverriddenInsetColor : (NamedColor?)null;

        if (!disposeController)
        {
          _view.ScatterSymbol = _doc.ScatterSymbol;
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

    protected override void AttachView()
    {
      base.AttachView();
      _view.IndependentColorChanged += EhIndependentColorChanged;
      _view.ScatterSymbolChanged += EhScatterSymbolChanged;
      _view.CreateNewSymbolSetFromOverrides += EhCreateNewSymbolSetFromOverrides;
      _view.SimilarSymbolSetChosen += EhSimilarShapeChosen;
    }

    protected override void DetachView()
    {
      _view.IndependentColorChanged -= EhIndependentColorChanged;
      _view.ScatterSymbolChanged += EhScatterSymbolChanged;
      _view.CreateNewSymbolSetFromOverrides -= EhCreateNewSymbolSetFromOverrides;
      _view.SimilarSymbolSetChosen -= EhSimilarShapeChosen;
      base.DetachView();
    }

    private void EhIndependentColorChanged()
    {
      if (_view is not null)
      {
        _doc.IndependentColor = _view.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
      }
    }

    private void EhScatterSymbolChanged()
    {
      var symbol = _view.ScatterSymbol;

      // Frame
      if (!_view.OverrideFrame)
      {
        _symbolFrameChoices.ForEachDo(n => n.IsSelected = (symbol.Frame?.GetType() == (Type)n.Tag));
        _view.Frame = _symbolFrameChoices;
      }

      // Inset
      if (!_view.OverrideInset)
      {
        _symbolInsetChoices.SetSelection(node => symbol.Inset?.GetType() == (Type)node.Tag);
        _view.Inset = _symbolInsetChoices;
      }

      // Structure width
      if (!_view.OverrideRelativeStructureWidth)
        _view.OverriddenRelativeStructureWidth = symbol.RelativeStructureWidth;

      // Plot color influence
      if (!_view.OverridePlotColorInfluence)
        _view.OverriddenPlotColorInfluence = symbol.PlotColorInfluence;

      // Fill color
      if (!_view.OverrideFillColor)
        _view.OverriddenFillColor = symbol.FillColor;

      // FrameColor
      if (!_view.OverrideFrameColor && symbol.Frame is not null)
        _view.OverriddenFrameColor = symbol.Frame.Color;

      // InsetColor
      if (!_view.OverrideInsetColor && symbol.Inset is not null)
        _view.OverriddenInsetColor = symbol.Inset.Color;

      // Initialize the list of similar symbols
      Initialize_SimilarSymbolList();
      if (_view is not null)
        _view.SimilarSymbols = _similarSymbolChoices;
    }

    private void EhCreateNewSymbolSetFromOverrides()
    {
      var newSymbol = CreateNewSymbolSetFromOverrides(_doc.ScatterSymbol, out var cancellationRequested);
      ClearAllOverridesThatAreEqualToScatterSymbol(newSymbol);
      _doc.ScatterSymbol = newSymbol;
      _view.ScatterSymbol = newSymbol;
      EhScatterSymbolChanged();
    }

    private void ClearAllOverridesThatAreEqualToScatterSymbol(IScatterSymbol symbol)
    {
      if (symbol.Frame?.GetType() == (Type)_symbolFrameChoices.FirstSelectedNode?.Tag)
        _view.OverrideFrame = false;

      if (symbol.Inset?.GetType() == (Type)_symbolInsetChoices.FirstSelectedNode?.Tag)
        _view.OverrideInset = false;

      if ((_view.OverrideRelativeStructureWidth == false || symbol.RelativeStructureWidth == _view.OverriddenRelativeStructureWidth) && (_view.OverrideAbsoluteStructureWidth == false || _view.OverriddenAbsoluteStructureWidth == 0))
      {
        _view.OverrideAbsoluteStructureWidth = false;
        _view.OverrideRelativeStructureWidth = false;
      }

      if (symbol.PlotColorInfluence == _view.OverriddenPlotColorInfluence)
        _view.OverridePlotColorInfluence = false;

      if (symbol.FillColor == _view.OverriddenFillColor)
        _view.OverrideFillColor = false;

      if (symbol.Frame is null || symbol.Frame.Color == _view.OverriddenFrameColor)
        _view.OverrideFrameColor = false;

      if (symbol.Inset is null || symbol.Inset.Color == _view.OverriddenInsetColor)
        _view.OverrideInsetColor = false;
    }

    private IScatterSymbol CreateNewSymbolSetFromOverrides(IScatterSymbol symbol, out bool cancellationRequested)
    {
      cancellationRequested = false;

      double overriddenAbsoluteStructureWidth = _view.OverrideAbsoluteStructureWidth ? _view.OverriddenAbsoluteStructureWidth : 0;
      double overriddenRelativeStructureWidth = _view.OverrideRelativeStructureWidth ? _view.OverriddenRelativeStructureWidth : symbol.RelativeStructureWidth;

      double resultingRelativeStructureWidth = overriddenRelativeStructureWidth;

      if (_view.OverrideAbsoluteStructureWidth && _view.OverriddenAbsoluteStructureWidth != 0)
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
            resultingRelativeStructureWidth = overriddenAbsoluteStructureWidth / _view.SymbolSize;
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
            resultingRelativeStructureWidth = overriddenRelativeStructureWidth + overriddenAbsoluteStructureWidth / _view.SymbolSize;
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
      if (_view.IndependentScatterSymbol)
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

        if (_view.OverrideInset)
        {
          var newInsetType = (Type)_symbolInsetChoices.FirstSelectedNode?.Tag;
          if (newInsetType != newSymbol.Inset?.GetType())
          {
            var newInset = newInsetType is null ? null : (IScatterSymbolInset)Activator.CreateInstance(newInsetType);
            newSymbol = newSymbol.WithInset(newInset);
          }
        }

        if (_view.OverrideFrame)
        {
          var newFrameType = (Type)_symbolFrameChoices.FirstSelectedNode?.Tag;
          if (newFrameType != newSymbol.Frame?.GetType())
          {
            var newFrame = newFrameType is null ? null : (IScatterSymbolFrame)Activator.CreateInstance(newFrameType);
            newSymbol = newSymbol.WithFrame(newFrame);
          }
        }

        if (_view.OverrideRelativeStructureWidth || _view.OverrideRelativeStructureWidth)
          newSymbol = newSymbol.WithRelativeStructureWidth(resultingRelativeStructureWidth);

        if (_view.OverridePlotColorInfluence)
          newSymbol = newSymbol.WithPlotColorInfluence(_view.OverriddenPlotColorInfluence);

        if (_view.OverrideFillColor)
          newSymbol = newSymbol.WithFillColor(_view.OverriddenFillColor);

        if (_view.OverrideFrameColor && newSymbol.Frame is not null)
          newSymbol = newSymbol.WithFrame(newSymbol.Frame.WithColor(_view.OverriddenFrameColor));

        if (_view.OverrideInsetColor && newSymbol.Inset is not null)
          newSymbol = newSymbol.WithInset(newSymbol.Inset.WithColor(_view.OverriddenInsetColor));

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
      var currentSymbol = _view?.ScatterSymbol ?? _doc.ScatterSymbol;

      if (object.ReferenceEquals(currentSymbol, _lastSymbolForWhichSimilarSetsWhereSearched))
        return; // then _similarSymbolChoices is already up-to-date
      _lastSymbolForWhichSimilarSetsWhereSearched = currentSymbol;

      _similarSymbolChoices = new SelectableListNodeList();
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
      foreach (var entry in list)
      {
        _similarSymbolChoices.Add(new SelectableListNode(
          entry.Item1,
          entry.Item2[currentIndex], object.ReferenceEquals(currentList, entry.Item2)));
      }
    }

    private void EhSimilarShapeChosen()
    {
      var node = _similarSymbolChoices.FirstSelectedNode;
      if (node is not null)
      {
        var newSymbol = (IScatterSymbol)node.Tag;
        _view.ScatterSymbol = newSymbol;
      }
    }

    #endregion Similar symbols
  } // end of class XYPlotScatterStyleController
} // end of namespace
