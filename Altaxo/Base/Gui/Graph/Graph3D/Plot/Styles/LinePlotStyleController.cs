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

using Altaxo.Collections;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Graph3D.Plot.Styles.LineConnectionStyles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  #region Interfaces

  /// <summary>
  /// This view interface is for showing the options of the XYZPlotLineStyle
  /// </summary>
  public interface ILinePlotStyleView
  {
    bool ShowPlotColorsOnlyForLinePen { set; }

    bool IndependentLineColor { get; set; }

    bool IndependentDashStyle { get; set; }

    PenX3D LinePen { get; set; }

    bool IndependentSymbolSize { get; set; }

    double SymbolSize { get; set; }

    bool UseSymbolGap { get; set; }

    double SymbolGapOffset { get; set; }

    double SymbolGapFactor { get; set; }

    bool ConnectCircular { get; set; }

    bool IgnoreMissingDataPoints { get; set; }

    /// <summary>
    /// Initializes the Line connection combobox.
    /// </summary>
    /// <param name="list">List of possible selections.</param>
    void InitializeLineConnect(SelectableListNodeList list);

    #region events

    /// <summary>Occurs when the user choice for IndependentColor of the frame pen has changed.</summary>
    event Action IndependentLineColorChanged;

    /// <summary>Occurs when the user checked or unchecked the "use frame" checkbox.</summary>
    event Action UseLineChanged;

    /// <summary>Occurs when the  frame pen has changed by user interaction.</summary>
    event Action LinePenChanged;

    #endregion events
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for XYPlotLineStyleController.
  /// </summary>
  [UserControllerForObject(typeof(LinePlotStyle))]
  [ExpectedTypeOfView(typeof(ILinePlotStyleView))]
  public class LinePlotStyleController : MVCANControllerEditOriginalDocBase<LinePlotStyle, ILinePlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    private SelectableListNodeList _lineConnectChoices;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _lineConnectChoices = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
        InitializeLineConnectionChoices();
      }

      if (_view != null)
      {
        // Line properties
        _view.InitializeLineConnect(_lineConnectChoices);
        _view.ConnectCircular = _doc.ConnectCircular;
        _view.IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        _view.IndependentLineColor = _doc.IndependentLineColor;
        _view.IndependentDashStyle = _doc.IndependentDashStyle;
        _view.ShowPlotColorsOnlyForLinePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
        _view.LinePen = _doc.LinePen;

        _view.IndependentSymbolSize = _doc.IndependentSymbolSize;
        _view.SymbolSize = _doc.SymbolSize;
        _view.UseSymbolGap = _doc.UseSymbolGap;
        _view.SymbolGapOffset = _doc.SymbolGapOffset;
        _view.SymbolGapFactor = _doc.SymbolGapFactor;
      }
    }

    private void InitializeLineConnectionChoices()
    {
      if (null == _lineConnectChoices)
        _lineConnectChoices = new SelectableListNodeList();
      else
        _lineConnectChoices.Clear();

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ILineConnectionStyle));

      foreach (var t in types)
      {
        _lineConnectChoices.Add(new SelectableListNode(t.Name, t, t == _doc.Connection.GetType()));
      }
    }

    public override bool Apply(bool disposeController)
    {
      // don't trust user input, so all into a try statement
      try
      {
        _doc.ConnectCircular = _view.ConnectCircular;
        _doc.IgnoreMissingDataPoints = _view.IgnoreMissingDataPoints;

        // Pen
        _doc.IndependentLineColor = _view.IndependentLineColor;
        _doc.IndependentDashStyle = _view.IndependentDashStyle;
        _doc.LinePen = _view.LinePen;

        // Line Connect

        var selNode = _lineConnectChoices.FirstSelectedNode;
        var connectionType = (Type)(selNode.Tag);
        if (connectionType == typeof(NoConnection))
          _doc.Connection = NoConnection.Instance;
        else
          _doc.Connection = (ILineConnectionStyle)Activator.CreateInstance(connectionType);

        _doc.IndependentSymbolSize = _view.IndependentSymbolSize;
        _doc.SymbolSize = _view.SymbolSize;
        _doc.UseSymbolGap = _view.UseSymbolGap;
        _doc.SymbolGapOffset = _view.SymbolGapOffset;
        _doc.SymbolGapFactor = _view.SymbolGapFactor;

        return ApplyEnd(true, disposeController);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occured. " + ex.Message);
        return false;
      }
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.IndependentLineColorChanged += EhIndependentLineColorChanged;
    }

    protected override void DetachView()
    {
      _view.IndependentLineColorChanged -= EhIndependentLineColorChanged;
      base.DetachView();
    }

    #region Color management

    private void EhColorGroupStyleAddedOrRemoved()
    {
      if (null != _view)
      {
        _doc.IndependentLineColor = _view.IndependentLineColor;
        _view.ShowPlotColorsOnlyForLinePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
      }
    }

    private void EhIndependentLineColorChanged()
    {
      if (null != _view)
      {
        _doc.IndependentLineColor = _view.IndependentLineColor;
        _view.ShowPlotColorsOnlyForLinePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
      }
    }

    #endregion Color management
  } // end of class XYPlotLineStyleController
} // end of namespace
