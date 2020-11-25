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
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  #region Interfaces

  /// <summary>
  /// This view interface is for showing the options of the XYZPlotLineStyle
  /// </summary>
  public interface IDropAreaPlotStyleView
  {
    bool ConnectCircular { get; set; }

    bool IgnoreMissingDataPoints { get; set; }

    bool IndependentOnShiftingGroupStyles { get; set; }

    /// <summary>
    /// Initializes the Line connection combobox.
    /// </summary>
    /// <param name="list">List of possible selections.</param>
    void InitializeLineConnect(SelectableListNodeList list);

    #region events

    /// <summary>Occurs when the user choice for IndependentColor of the frame pen has changed.</summary>
    event Action FrameColorLinkageChanged;

    /// <summary>Occurs when the  frame pen has changed by user interaction.</summary>
    event Action FramePenChanged;

    #endregion events

    #region Fill

    /// <summary>Sets a value indicating whether plot colors only should be shown for the fill brush.</summary>
    /// <value><c>true</c> if only plot colors should be shown for fill brush; otherwise, <c>false</c>.</value>
    bool ShowPlotColorsOnlyForFillBrush { set; }

    bool ShowPlotColorsOnlyForFramePen { set; }

    void InitializeFillColorLinkage(SelectableListNodeList list);

    void InitializeFrameColorLinkage(SelectableListNodeList list);

    /// <summary>
    /// Gets/sets the contents of the fill color combobox.
    /// </summary>
    BrushX FillBrush { get; set; }

    PenX FramePen { get; set; }

    /// <summary>
    /// Initializes the fill direction combobox.
    /// </summary>
    /// <param name="list">List of possible selections.</param>
    void InitializeFillDirection(SelectableListNodeList list);

    /// <summary>Occurs when the user choice for IndependentColor of the fill brush has changed.</summary>
    event Action FillColorLinkageChanged;

    /// <summary>Occurs when the fill brush has changed by user interaction.</summary>
    event Action FillBrushChanged;

    #endregion Fill
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for XYPlotLineStyleController.
  /// </summary>
  [UserControllerForObject(typeof(DropAreaPlotStyle))]
  [ExpectedTypeOfView(typeof(IDropAreaPlotStyleView))]
  public class DropAreaPlotStyleController : MVCANControllerEditOriginalDocBase<DropAreaPlotStyle, IDropAreaPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    private SelectableListNodeList _lineConnectChoices;

    private SelectableListNodeList _areaFillDirectionChoices;
    private SelectableListNodeList _fillColorLinkageChoices;
    private SelectableListNodeList _frameColorLinkageChoices;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _lineConnectChoices = null;
      _areaFillDirectionChoices = null;
      _fillColorLinkageChoices = null;
      _frameColorLinkageChoices = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
        InitializeLineConnectionChoices();
        _fillColorLinkageChoices = new SelectableListNodeList(_doc.FillColorLinkage);
        _frameColorLinkageChoices = new SelectableListNodeList(_doc.FrameColorLinkage);
        InitializeFillDirectionChoices();
      }

      if (_view is not null)
      {
        // Line properties
        _view.InitializeLineConnect(_lineConnectChoices);
        _view.ConnectCircular = _doc.ConnectCircular;
        _view.IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        _view.IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;
        _view.FramePen = _doc.FramePen;

        // Fill area
        _view.InitializeFillColorLinkage(_fillColorLinkageChoices);
        _view.InitializeFrameColorLinkage(_frameColorLinkageChoices);
        _view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
        _view.ShowPlotColorsOnlyForFramePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FrameColorLinkage);
        _view.FillBrush = _doc.FillBrush ?? new BrushX(NamedColors.Transparent);
        _view.InitializeFillDirection(_areaFillDirectionChoices);
      }
    }

    private void InitializeLineConnectionChoices()
    {
      if (_lineConnectChoices is null)
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
        _doc.IndependentOnShiftingGroupStyles = _view.IndependentOnShiftingGroupStyles;

        // Line Connect

        var selNode = _lineConnectChoices.FirstSelectedNode;
        var connectionType = (Type)(selNode.Tag);
        if (connectionType == typeof(NoConnection))
          _doc.Connection = NoConnection.Instance;
        else
          _doc.Connection = (ILineConnectionStyle)Activator.CreateInstance(connectionType);

        // Fill Area

        // Line fill direction
        selNode = _areaFillDirectionChoices.FirstSelectedNode;
        if (selNode is not null)
          _doc.FillDirection = ((CSPlaneID)selNode.Tag);
        else
          _doc.FillDirection = null;

        // Line fill color
        _doc.FillBrush = _view.FillBrush;
        _doc.FramePen = _view.FramePen;
        // _doc.FillColorLinkage = _view.FillColorLinkage; // already done during showing the view, see EhFillColorLinkageChanged()

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

      _view.FillColorLinkageChanged += EhFillColorLinkageChanged;
      _view.FrameColorLinkageChanged += EhFrameColorLinkageChanged;
      _view.FillBrushChanged += EhFillBrushChanged;
      _view.FramePenChanged += EhFramePenChanged;
    }

    protected override void DetachView()
    {
      _view.FillColorLinkageChanged -= EhFillColorLinkageChanged;
      _view.FrameColorLinkageChanged -= EhFrameColorLinkageChanged;
      _view.FillBrushChanged -= EhFillBrushChanged;
      _view.FramePenChanged -= EhFramePenChanged;
      base.DetachView();
    }

    public void InitializeFillDirectionChoices()
    {
      _areaFillDirectionChoices = new SelectableListNodeList();
      var layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;
      if (layer is not null)
      {
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _doc.FillDirection }))
        {
          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          _areaFillDirectionChoices.Add(new SelectableListNode(info.Name, id, id == _doc.FillDirection));
        }
      }
    }

    #region Color management

    /// <summary>
    /// Gets or sets a value indicating whether the line is shown or not. By definition here, the line is not shown only if the connection style is "Noline".
    /// When setting this property, this influences not the connection style in the _view, but only the IsEnabled property of all Gui items associated with the line.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the line used; otherwise, <c>false</c>.
    /// </value>
    private bool IsLineUsed
    {
      get
      {
        var selNode = _lineConnectChoices.FirstSelectedNode;
        return !NoConnection.Instance.Equals(selNode.Tag);
      }
      set
      {
        //if(null!=_view)	_view.EnableLineControls = value;
      }
    }

    private void EhColorGroupStyleAddedOrRemoved()
    {
      if (_view is not null)
      {
        _doc.FillColorLinkage = (ColorLinkage)_fillColorLinkageChoices.FirstSelectedNode.Tag;
        _view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
        _view.ShowPlotColorsOnlyForFramePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FrameColorLinkage);
      }
    }

    private void EhFillColorLinkageChanged()
    {
      if (_view is not null)
      {
        _doc.FillColorLinkage = (ColorLinkage)_fillColorLinkageChoices.FirstSelectedNode.Tag;
        if (ColorLinkage.Dependent == _doc.FillColorLinkage)
          InternalSetFillColorToFrameColor();
        if (ColorLinkage.PreserveAlpha == _doc.FillColorLinkage)
          InternalSetFillColorRGBToLineColor();

        _view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
      }
    }

    private void EhFrameColorLinkageChanged()
    {
      if (_view is not null)
      {
        _doc.FrameColorLinkage = (ColorLinkage)_frameColorLinkageChoices.FirstSelectedNode.Tag;
        if (ColorLinkage.Dependent == _doc.FrameColorLinkage)
          InternalSetFrameColorToFillColor();
        if (ColorLinkage.PreserveAlpha == _doc.FrameColorLinkage)
          InternalSetFrameColorRGBToFillColor();

        _view.ShowPlotColorsOnlyForFramePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FrameColorLinkage);
      }
    }

    private void EhFillBrushChanged()
    {
      if (_view is not null)
      {
        if (ColorLinkage.Dependent == _doc.FrameColorLinkage)
        {
          if (_view.FramePen.Color != _view.FillBrush.Color)
            InternalSetFrameColorToFillColor();
        }
        else if (ColorLinkage.PreserveAlpha == _doc.FrameColorLinkage)
        {
          if (_view.FramePen.Color != _view.FillBrush.Color)
            InternalSetFrameColorRGBToFillColor();
        }
      }
    }

    private void EhFramePenChanged()
    {
      if (_view is not null)
      {
        if (ColorLinkage.Dependent == _doc.FillColorLinkage)
        {
          if (_view.FillBrush.Color != _view.FramePen.Color)
            InternalSetFillColorToFrameColor();
        }
        else if (ColorLinkage.PreserveAlpha == _doc.FillColorLinkage)
        {
          if (_view.FillBrush.Color != _view.FramePen.Color)
            InternalSetFillColorRGBToLineColor();
        }
      }
    }

    /// <summary>
    /// Internal sets the fill color to the color of the line.
    /// </summary>
    private void InternalSetFillColorToFrameColor()
    {
      var newBrush = _view.FillBrush.WithColor(_view.FramePen.Color);

      // Change fill brush without notification
      _view.FillBrushChanged -= EhFillBrushChanged;
      _view.FillBrush = newBrush;
      _view.FillBrushChanged += EhFillBrushChanged;
    }

    /// <summary>
    /// Internal sets the frame color to the color of the fill brush.
    /// </summary>
    private void InternalSetFrameColorToFillColor()
    {
      var newPen = _view.FramePen.WithColor(_view.FillBrush.Color);

      // Change frame pen without notification
      _view.FramePenChanged -= EhFramePenChanged;
      _view.FramePen = newPen;
      _view.FramePenChanged += EhFramePenChanged;
    }

    /// <summary>
    /// Internal sets the fill color to the color of the line, but here only the RGB component is used from the line color. The A component of the fill color remains unchanged.
    /// </summary>
    private void InternalSetFillColorRGBToLineColor()
    {
      var newBrush = _view.FillBrush;
      var c = _view.FramePen.Color.NewWithAlphaValue(newBrush.Color.Color.A);

      newBrush = newBrush.WithColor(c);

      // Change fill brush without notification
      _view.FillBrushChanged -= EhFillBrushChanged;
      _view.FillBrush = newBrush;
      _view.FillBrushChanged += EhFillBrushChanged;
    }

    /// <summary>
    /// Internal sets the frame color to the color of the fill brush, but here only the RGB component is used. The A component of the color remains unchanged.
    /// </summary>
    private void InternalSetFrameColorRGBToFillColor()
    {
      var newPen = _view.FramePen.WithColor(_view.FillBrush.Color.NewWithAlphaValue(_view.FramePen.Color.Color.A));

      // Change frame pen without notification
      _view.FramePenChanged -= EhFramePenChanged;
      _view.FramePen = newPen;
      _view.FramePenChanged += EhFramePenChanged;
    }

    #endregion Color management
  } // end of class XYPlotLineStyleController
} // end of namespace
