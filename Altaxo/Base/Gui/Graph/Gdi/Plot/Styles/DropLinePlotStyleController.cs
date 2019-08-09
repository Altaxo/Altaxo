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

using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  #region Interfaces

  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotScatterStyle
  /// </summary>
  public interface IDropLinePlotStyleView
  {
    bool AdditionalDropTargetIsEnabled { get; set; }

    int AdditionalDropTargetPerpendicularAxisNumber { get; set; }

    /// <summary>
    /// Indicates whether _baseValue is a physical value or a logical value.
    /// </summary>
    bool AdditionalDropTargetUsePhysicalBaseValue { get; set; }

    /// <summary>
    /// The y-value where the item normally starts. This is either a logical value (_usePhysicalBaseValue==false) or a physical value.
    /// </summary>
    Altaxo.Data.AltaxoVariant AdditionalDropTargetBaseValue { get; set; }

    /// <summary>
    /// Initializes the plot style color combobox.
    /// </summary>
    PenX Pen { get; set; }

    /// <summary>
    /// Indicates, whether only colors of plot color sets should be shown.
    /// </summary>
    bool ShowPlotColorsOnly { set; }

    bool IndependentColor { get; set; }

    bool IndependentSymbolSize { get; set; }

    double SymbolSize { get; set; }

    double LineWidth1Offset { get; set; }
    double LineWidth1Factor { get; set; }

    /// <summary>
    /// Intitalizes the drop line checkboxes.
    /// </summary>
    /// <param name="names">List of names plus the information if they are selected or not.</param>
    void InitializeDropLineConditions(SelectableListNodeList names);

    int SkipFrequency { get; set; }

    bool IndependentSkipFrequency { get; set; }

    bool IgnoreMissingDataPoints { get; set; }

    bool IndependentOnShiftingGroupStyles { get; set; }

    double GapAtStartOffset { get; set; }
    double GapAtStartFactor { get; set; }
    double GapAtEndOffset { get; set; }
    double GapAtEndFactor { get; set; }

    #region events

    event Action IndependentColorChanged;

    #endregion events
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for XYPlotScatterStyleController.
  /// </summary>
  [UserControllerForObject(typeof(DropLinePlotStyle))]
  [ExpectedTypeOfView(typeof(IDropLinePlotStyleView))]
  public class DropLinePlotStyleController : MVCANControllerEditOriginalDocBase<DropLinePlotStyle, IDropLinePlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    private SelectableListNodeList _dropLineChoices;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break; // no subcontrollers
    }

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _dropLineChoices = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

        InitializeDropLineChoices();
      }
      if (_view != null)
      {
        _view.IndependentSkipFrequency = _doc.IndependentSkipFrequency;
        _view.SkipFrequency = _doc.SkipFrequency;
        _view.IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        _view.IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;
        _view.InitializeDropLineConditions(_dropLineChoices);

        _view.AdditionalDropTargetIsEnabled = _doc.AdditionalDropTargetIsEnabled;
        _view.AdditionalDropTargetPerpendicularAxisNumber = _doc.AdditionalDropTargetPerpendicularAxisNumber;
        _view.AdditionalDropTargetUsePhysicalBaseValue = _doc.AdditionalDropTargetUsePhysicalBaseValue;
        _view.AdditionalDropTargetBaseValue = _doc.AdditionalDropTargetBaseValue;

        // now we have to set all dialog elements to the right values
        _view.IndependentColor = _doc.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        _view.Pen = _doc.Pen;

        _view.IndependentSymbolSize = _doc.IndependentSymbolSize;
        _view.SymbolSize = _doc.SymbolSize;

        _view.LineWidth1Offset = _doc.LineWidth1Offset;
        _view.LineWidth1Factor = _doc.LineWidth1Factor;

        _view.GapAtStartOffset = _doc.GapAtStartOffset;
        _view.GapAtStartFactor = _doc.GapAtStartFactor;
        _view.GapAtEndOffset = _doc.GapAtEndOffset;
        _view.GapAtEndFactor = _doc.GapAtEndFactor;
      }
    }

    public override bool Apply(bool disposeController)
    {
      // don't trust user input, so all into a try statement
      try
      {
        // Skip frequency
        _doc.IndependentSkipFrequency = _view.IndependentSkipFrequency;
        _doc.SkipFrequency = _view.SkipFrequency;
        _doc.IgnoreMissingDataPoints = _view.IgnoreMissingDataPoints;
        _doc.IndependentOnShiftingGroupStyles = _view.IndependentOnShiftingGroupStyles;

        // Drop targets
        _doc.DropTargets = new CSPlaneIDList(_dropLineChoices.Where(node => node.IsSelected).Select(node => (CSPlaneID)node.Tag));

        _doc.AdditionalDropTargetIsEnabled = _view.AdditionalDropTargetIsEnabled;
        _doc.AdditionalDropTargetPerpendicularAxisNumber = _view.AdditionalDropTargetPerpendicularAxisNumber;
        _doc.AdditionalDropTargetUsePhysicalBaseValue = _view.AdditionalDropTargetUsePhysicalBaseValue;
        _doc.AdditionalDropTargetBaseValue = _view.AdditionalDropTargetBaseValue;

        // Symbol Color
        _doc.Pen = _view.Pen;
        _doc.IndependentColor = _view.IndependentColor;

        _doc.IndependentSymbolSize = _view.IndependentSymbolSize;
        _doc.SymbolSize = _view.SymbolSize;

        _doc.LineWidth1Offset = _view.LineWidth1Offset;
        _doc.LineWidth1Factor = _view.LineWidth1Factor;

        // gap
        _doc.GapAtStartOffset = _view.GapAtStartOffset;
        _doc.GapAtStartFactor = _view.GapAtStartFactor;
        _doc.GapAtEndOffset = _view.GapAtEndOffset;
        _doc.GapAtEndFactor = _view.GapAtEndFactor;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occurred: " + ex.Message);
        return false;
      }

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.IndependentColorChanged += EhIndependentColorChanged;
    }

    protected override void DetachView()
    {
      _view.IndependentColorChanged -= EhIndependentColorChanged;
      base.DetachView();
    }

    public void InitializeDropLineChoices()
    {
      var layer = AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(_doc);

      _dropLineChoices = new SelectableListNodeList();
      foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyles.AxisStyleIDs, _doc.DropTargets))
      {
        bool sel = _doc.DropTargets.Contains(id);
        CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
        _dropLineChoices.Add(new SelectableListNode(info.Name, id, sel));
      }
    }

    private void EhIndependentColorChanged()
    {
      if (null != _view)
      {
        _doc.IndependentColor = _view.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
      }
    }
  } // end of class XYPlotScatterStyleController
} // end of namespace
