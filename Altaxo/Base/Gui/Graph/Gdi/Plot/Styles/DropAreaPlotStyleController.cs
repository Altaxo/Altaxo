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
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{

  /// <summary>
  /// This view interface is for showing the options of the XYZPlotLineStyle
  /// </summary>
  public interface IDropAreaPlotStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for XYPlotLineStyleController.
  /// </summary>
  [UserControllerForObject(typeof(DropAreaPlotStyle))]
  [ExpectedTypeOfView(typeof(IDropAreaPlotStyleView))]
  public class DropAreaPlotStyleController : MVCANControllerEditOriginalDocBase<DropAreaPlotStyle, IDropAreaPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_framePen, () => FramePen = null);
    }

    #region Bindings

    private bool _connectCircular;

    public bool ConnectCircular
    {
      get => _connectCircular;
      set
      {
        if (!(_connectCircular == value))
        {
          _connectCircular = value;
          OnPropertyChanged(nameof(ConnectCircular));
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

    private ItemsController<Type> _lineConnectChoices;

    public ItemsController<Type> LineConnectChoices
    {
      get => _lineConnectChoices;
      set
      {
        if (!(_lineConnectChoices == value))
        {
          _lineConnectChoices = value;
          OnPropertyChanged(nameof(LineConnectChoices));
        }
      }
    }

    private ItemsController<CSPlaneID> _areaFillDirectionChoices;

    public ItemsController<CSPlaneID> AreaFillDirectionChoices
    {
      get => _areaFillDirectionChoices;
      set
      {
        if (!(_areaFillDirectionChoices == value))
        {
          _areaFillDirectionChoices = value;
          OnPropertyChanged(nameof(AreaFillDirectionChoices));
        }
      }
    }



    private ItemsController<ColorLinkage> _fillColorLinkageChoices;

    public ItemsController<ColorLinkage> FillColorLinkageChoices
    {
      get => _fillColorLinkageChoices;
      set
      {
        if (!(_fillColorLinkageChoices == value))
        {
          _fillColorLinkageChoices = value;
          OnPropertyChanged(nameof(FillColorLinkageChoices));
        }
      }
    }

    private ItemsController<ColorLinkage> _frameColorLinkageChoices;

    public ItemsController<ColorLinkage> FrameColorLinkageChoices
    {
      get => _frameColorLinkageChoices;
      set
      {
        if (!(_frameColorLinkageChoices == value))
        {
          _frameColorLinkageChoices = value;
          OnPropertyChanged(nameof(FrameColorLinkageChoices));
        }
      }
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
          EhFillBrushChanged();
        }
      }
    }

    private ColorTypeThicknessPenController _framePen;

    public ColorTypeThicknessPenController FramePen
    {
      get => _framePen;
      set
      {
        if (!(_framePen == value))
        {
          if (_framePen is not null)
            _framePen.MadeDirty -= EhFramePenChanged;
          _framePen?.Dispose();

          _framePen = value;

          if (_framePen is not null)
            _framePen.MadeDirty += EhFramePenChanged;

          OnPropertyChanged(nameof(FramePen));
        }
      }
    }



    #endregion

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
        FillColorLinkageChoices = new ItemsController<ColorLinkage>(new SelectableListNodeList(_doc.FillColorLinkage), EhFillColorLinkageChanged);
        FrameColorLinkageChoices = new ItemsController<ColorLinkage>(new SelectableListNodeList(_doc.FrameColorLinkage), EhFrameColorLinkageChanged);
        InitializeFillDirectionChoices();
        // Line properties
        ConnectCircular = _doc.ConnectCircular;
        IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;
        FramePen = new ColorTypeThicknessPenController(_doc.FramePen);

        // Fill area
        
        ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
        FramePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FrameColorLinkage);
        FillBrush = _doc.FillBrush ?? new BrushX(NamedColors.Transparent);
      }
    }

    private void InitializeLineConnectionChoices()
    {
       var lineConnectChoices = new SelectableListNodeList();

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ILineConnectionStyle));

      foreach (var t in types)
      {
        lineConnectChoices.Add(new SelectableListNode(t.Name, t, t == _doc.Connection.GetType()));
      }

      LineConnectChoices = new ItemsController<Type>(lineConnectChoices);
    }

    public override bool Apply(bool disposeController)
    {
      // don't trust user input, so all into a try statement
      try
      {
        _doc.ConnectCircular = ConnectCircular;
        _doc.IgnoreMissingDataPoints = IgnoreMissingDataPoints;
        _doc.IndependentOnShiftingGroupStyles = IndependentOnShiftingGroupStyles;

        // Line Connect

        var connectionType = LineConnectChoices.SelectedValue;
        if (connectionType == typeof(NoConnection))
          _doc.Connection = NoConnection.Instance;
        else
          _doc.Connection = (ILineConnectionStyle)Activator.CreateInstance(connectionType);

        // Fill Area

        //  fill direction
        _doc.FillDirection = AreaFillDirectionChoices.SelectedValue;

        // Line fill color
        _doc.FillBrush =FillBrush;
        _doc.FramePen = FramePen.Pen;
        // _doc.FillColorLinkage = _view.FillColorLinkage; // already done during showing the view, see EhFillColorLinkageChanged()

        return ApplyEnd(true, disposeController);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occured. " + ex.Message);
        return false;
      }
    }

    public void InitializeFillDirectionChoices()
    {
      var areaFillDirectionChoices = new SelectableListNodeList();
      var layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;
      if (layer is not null)
      {
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _doc.FillDirection }))
        {
          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          areaFillDirectionChoices.Add(new SelectableListNode(info.Name, id, id == _doc.FillDirection));
        }
      }
      AreaFillDirectionChoices = new ItemsController<CSPlaneID>(areaFillDirectionChoices);
    }

    #region Color management

   

    private void EhColorGroupStyleAddedOrRemoved()
    {
      if (_view is not null)
      {
        _doc.FillColorLinkage = FillColorLinkageChoices.SelectedValue;
        ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
        FramePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FrameColorLinkage);
      }
    }

    private void EhFillColorLinkageChanged(ColorLinkage value)
    {
      _doc.FillColorLinkage = value;
        if (ColorLinkage.Dependent == _doc.FillColorLinkage)
          InternalSetFillColorToFrameColor();
        if (ColorLinkage.PreserveAlpha == _doc.FillColorLinkage)
          InternalSetFillColorRGBToLineColor();

        ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
    }

    private void EhFrameColorLinkageChanged(ColorLinkage value)
    {
      _doc.FrameColorLinkage = value;
        if (ColorLinkage.Dependent == _doc.FrameColorLinkage)
          InternalSetFrameColorToFillColor();
        if (ColorLinkage.PreserveAlpha == _doc.FrameColorLinkage)
          InternalSetFrameColorRGBToFillColor();

        FramePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FrameColorLinkage);
     
    }

    private void EhFillBrushChanged()
    {
      if (_view is not null)
      {
        if (ColorLinkage.Dependent == _doc.FrameColorLinkage)
        {
          if (FramePen.Pen.Color != FillBrush.Color)
            InternalSetFrameColorToFillColor();
        }
        else if (ColorLinkage.PreserveAlpha == _doc.FrameColorLinkage)
        {
          if (FramePen.Pen.Color != FillBrush.Color)
            InternalSetFrameColorRGBToFillColor();
        }
      }
    }

    private void EhFramePenChanged(object _)
    {
      if (_view is not null)
      {
        if (ColorLinkage.Dependent == _doc.FillColorLinkage)
        {
          if (FillBrush.Color != FramePen.Pen.Color)
            InternalSetFillColorToFrameColor();
        }
        else if (ColorLinkage.PreserveAlpha == _doc.FillColorLinkage)
        {
          if (FillBrush.Color !=FramePen.Pen.Color)
            InternalSetFillColorRGBToLineColor();
        }
      }
    }

    /// <summary>
    /// Internal sets the fill color to the color of the line.
    /// </summary>
    private void InternalSetFillColorToFrameColor()
    {
      FillBrush = FillBrush.WithColor(FramePen.Pen.Color);
    }

    /// <summary>
    /// Internal sets the frame color to the color of the fill brush.
    /// </summary>
    private void InternalSetFrameColorToFillColor()
    {
      FramePen.Pen = FramePen.Pen.WithColor(FillBrush.Color);
    }

    /// <summary>
    /// Internal sets the fill color to the color of the line, but here only the RGB component is used from the line color. The A component of the fill color remains unchanged.
    /// </summary>
    private void InternalSetFillColorRGBToLineColor()
    {
      var newBrush =FillBrush;
      var c = FramePen.Pen.Color.NewWithAlphaValue(newBrush.Color.Color.A);
      FillBrush = newBrush.WithColor(c);
    }

    /// <summary>
    /// Internal sets the frame color to the color of the fill brush, but here only the RGB component is used. The A component of the color remains unchanged.
    /// </summary>
    private void InternalSetFrameColorRGBToFillColor()
    {
      FramePen.Pen = FramePen.Pen.WithColor(FillBrush.Color.NewWithAlphaValue(FramePen.Pen.Color.Color.A));
    }

    #endregion Color management
  } // end of class XYPlotLineStyleController
} // end of namespace
