#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface ILayerController : IMVCAController
  {
    void EhView_PageChanged(string firstChoice);
    void EhView_SecondChoiceChanged(int index, string item);
    void EhView_PageEnabledChanged(bool pageEnabled);

  }

  public interface ILayerView
  {

    ILayerController Controller { get; set; }


    void AddTab(string name, string text);

    object CurrentContent { get; set; }
    void SetCurrentContentWithEnable(object guielement, bool enable, string title);
    bool IsPageEnabled { get; set; }

    void SelectTab(string name);
    void InitializeSecondaryChoice(string[] names, string name);
    event CancelEventHandler TabValidating;

  }
  #endregion

  /// <summary>
  /// Summary description for LayerController.
  /// </summary>
  [UserControllerForObject(typeof(XYPlotLayer))]
  [ExpectedTypeOfView(typeof(ILayerView))]
  public class LayerController : ILayerController
  {
    protected ILayerView _view;

    protected XYPlotLayer _doc;
    protected IDisposable _docSuspendLock;
    protected XYPlotLayer _originalDoc;

    private string _currentPageName;

    enum TabType { Unique, Scales, Styles, Planes };
    TabType _primaryChoice; // which tab type is currently choosen
    private int _currentScale; // which scale is choosen 0==X-AxisScale, 1==Y-AxisScale

    private CSLineID _currentAxisID; // which style is currently choosen
    private CSPlaneID _currentPlaneID; // which plane is currently chosen for the grid


    IMVCAController _currentController;


    protected Altaxo.Gui.Graph.CoordinateSystemController _coordinateController;
    protected IMVCAController _layerPositionController;
    protected ILineScatterLayerContentsController _layerContentsController;
    protected IAxisScaleController[] _axisScaleController;

    Dictionary<CSLineID, CSAxisInformation> _axisStyleIds;
    List<CSAxisInformation> _axisStyleInfoSortedByName;
    List<CSPlaneID> _planeIdentifier;

    Dictionary<CSLineID, IMVCANController> _TitleFormatController;
    Dictionary<CSLineID, IMVCANController> _MajorLabelController;
    Dictionary<CSLineID, IMVCANController> _MinorLabelController;
    Dictionary<CSPlaneID, IMVCANController> _GridStyleController;

    Dictionary<CSLineID, bool> _enableAxisStyle;
    Dictionary<CSLineID, bool> _enableMajorLabels;
    Dictionary<CSLineID, bool> _enableMinorLabels;
    object _lastControllerApplied;


    public LayerController(XYPlotLayer layer)
      : this(layer, "Scale", 1, null)
    {
    }
    public LayerController(XYPlotLayer layer, string currentPage, CSLineID id)
      : this(layer, currentPage, id.ParallelAxisNumber, id)
    {
    }


    LayerController(XYPlotLayer layer, string currentPage, int axisScaleIdx, CSLineID id)
    {
      _originalDoc = layer;
      _doc = (XYPlotLayer)layer.Clone();
      _docSuspendLock = _doc.BeginUpdate();

      SetCoordinateSystemDependentObjects(id);

      _currentScale = axisScaleIdx;
      _currentAxisID = id;
      _currentPlaneID = CSPlaneID.Front;
      _currentPageName = currentPage;
      if (null != View)
        SetViewElements();
    }

    void SetCoordinateSystemDependentObjects()
    {
      SetCoordinateSystemDependentObjects(null);
    }
    void SetCoordinateSystemDependentObjects(CSLineID id)
    {
      // collect the AxisStyleIdentifier from the actual layer and also all possible AxisStyleIdentifier
      _axisStyleIds = new Dictionary<CSLineID, CSAxisInformation>();
      _axisStyleInfoSortedByName = new List<CSAxisInformation>();
      foreach (CSLineID ids in _doc.CoordinateSystem.GetJoinedAxisStyleIdentifier(_doc.AxisStyles.AxisStyleIDs, new CSLineID[] { id }))
      {
        CSAxisInformation info = _doc.CoordinateSystem.GetAxisStyleInformation(ids);
        _axisStyleIds.Add(info.Identifier, info);
        _axisStyleInfoSortedByName.Add(info);
      }

      _planeIdentifier = new List<CSPlaneID>();
      _planeIdentifier.Add(CSPlaneID.Front);


      _axisScaleController = new AxisScaleController[2];
      _GridStyleController = new Dictionary<CSPlaneID, IMVCANController>();
      _TitleFormatController = new Dictionary<CSLineID, IMVCANController>();
      _MajorLabelController = new Dictionary<CSLineID, IMVCANController>();
      _MinorLabelController = new Dictionary<CSLineID, IMVCANController>();

      _enableAxisStyle = new Dictionary<CSLineID, bool>();
      _enableMajorLabels = new Dictionary<CSLineID, bool>();
      _enableMinorLabels = new Dictionary<CSLineID, bool>();
      foreach (CSLineID ident in _axisStyleIds.Keys)
      {

        AxisStyle prop = _doc.AxisStyles[ident];
        if (prop == null)
        {
          _enableAxisStyle.Add(ident, false);
          _enableMajorLabels.Add(ident, false);
          _enableMinorLabels.Add(ident, false);
        }
        else
        {
          _enableAxisStyle.Add(ident, true);
          _enableMajorLabels.Add(ident, prop.ShowMajorLabels);
          _enableMinorLabels.Add(ident, prop.ShowMinorLabels);
        }
      }
    }

    public ILayerView View
    {
      get { return _view; }
      set
      {
        this.ViewObject = value;
      }
    }

    void SetViewElements()
    {
      if (null == View)
        return;

      // add all necessary Tabs
      View.AddTab("Scale", "Scale");
      View.AddTab("CS", "Coord.System");
      View.AddTab("Contents", "Contents");
      View.AddTab("Position", "Position");
      View.AddTab("TitleAndFormat", "Title&&Format");
      View.AddTab("MajorLabels", "Major labels");
      View.AddTab("MinorLabels", "Minor labels");
      View.AddTab("GridStyle", "Grid style");

      // Set the controller of the current visible Tab
      SetCurrentTabController(true);
    }



    void SetCurrentTabController(bool pageChanged)
    {
      switch (_currentPageName)
      {
        case "Contents":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetLayerSecondaryChoice();
          }
          if (null == _layerContentsController)
          {
            _layerContentsController = (ILineScatterLayerContentsController)Current.Gui.GetControllerAndControl(new object[] { _doc.PlotItems }, typeof(ILineScatterLayerContentsController), UseDocument.Directly);
          }
          _currentController = _layerContentsController;
          View.CurrentContent = _currentController.ViewObject;
          break;
        case "Position":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetLayerSecondaryChoice();
          }
          if (null == _layerPositionController)
          {
            _layerPositionController = new LayerPositionController(_doc);
            _layerPositionController.ViewObject = new LayerPositionControl();
          }
          _currentController = _layerPositionController;
          View.CurrentContent = _layerPositionController.ViewObject;
          break;


        case "Scale":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetHorzVertSecondaryChoice();
          }
          if (_axisScaleController[_currentScale] == null)
          {
            _axisScaleController[_currentScale] = new AxisScaleController(_doc, _currentScale);
            _axisScaleController[_currentScale].ViewObject = new AxisScaleControl();
          }
          _currentController = _axisScaleController[_currentScale];
          View.CurrentContent = _currentController.ViewObject;
          break;

        case "CS":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetLayerSecondaryChoice();
          }
          if (null == this._coordinateController)
          {
            this._coordinateController = new Altaxo.Gui.Graph.CoordinateSystemController(_doc.CoordinateSystem);
            Current.Gui.FindAndAttachControlTo(this._coordinateController);
          }
          _currentController = this._coordinateController;
          View.CurrentContent = this._coordinateController.ViewObject;
          break;

        case "GridStyle":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetPlaneSecondaryChoice();
          }

          if (!_GridStyleController.ContainsKey(_currentPlaneID))
          {
            GridPlane p = _doc.GridPlanes.Contains(_currentPlaneID) ? _doc.GridPlanes[_currentPlaneID] : new GridPlane(_currentPlaneID);
            GridPlaneController ctrl = new GridPlaneController(p);
            Current.Gui.FindAndAttachControlTo(ctrl);
            _GridStyleController.Add(_currentPlaneID, ctrl);
          }
          _currentController = _GridStyleController[_currentPlaneID];
          View.CurrentContent = this._currentController.ViewObject;


          break;

        case "TitleAndFormat":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetEdgeSecondaryChoice();
          }

          if (!_TitleFormatController.ContainsKey(_currentAxisID))
          {
            AxisStyle ast = _doc.AxisStyles.Contains(_currentAxisID) ? _doc.AxisStyles[_currentAxisID] : null;

            if (null != ast)
            {
              _TitleFormatController.Add(_currentAxisID, (IMVCANController)Current.Gui.GetControllerAndControl(new object[]{ast},typeof(IMVCANController),UseDocument.Directly));
            }
          }

          _currentController = _TitleFormatController.ContainsKey(_currentAxisID) ? _TitleFormatController[_currentAxisID] : null;
          if (null != _currentController && null == _currentController.ViewObject)
            _currentController.ViewObject = new TitleFormatLayerControl();


          View.SetCurrentContentWithEnable(
            _currentController == null ? null : _currentController.ViewObject,
            _currentController != null && _enableAxisStyle[_currentAxisID],
            "Enable axis style");


          break;
        case "MajorLabels":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetEdgeSecondaryChoice();
          }

          if (!_enableAxisStyle[_currentAxisID])
          {
            View.CurrentContent = null; // disable all, dont show any content
          }
          else // the axis style for this axis is at least activated
          {
            if (!_MajorLabelController.ContainsKey(_currentAxisID))
            {
              AxisLabelStyle als = (AxisLabelStyle)_doc.AxisStyles[_currentAxisID].MajorLabelStyle;
              if (null != als)
              {
                _MajorLabelController.Add(_currentAxisID, (IXYAxisLabelStyleController)Current.Gui.GetControllerAndControl(new object[] { als }, typeof(IXYAxisLabelStyleController), UseDocument.Directly));
              }
            }

            _currentController = _MajorLabelController.ContainsKey(_currentAxisID)? _MajorLabelController[_currentAxisID] : null;

            View.SetCurrentContentWithEnable(
              _currentController == null ? null : _currentController.ViewObject,
              _currentController != null && _enableMajorLabels[_currentAxisID],
              "Enable major labels");
          }
          break;
        case "MinorLabels":
          if (pageChanged)
          {
            View.SelectTab(_currentPageName);
            SetEdgeSecondaryChoice();
          }

          if (!_enableAxisStyle[_currentAxisID])
          {
            View.CurrentContent = null; // disable all, dont show any content
          }
          else // the axis style for this axis is at least activated
          {
            if (!_MinorLabelController.ContainsKey(_currentAxisID))
            {
              AxisLabelStyle als = (AxisLabelStyle)_doc.AxisStyles[_currentAxisID].MinorLabelStyle;
              if (null != als)
              {
                _MinorLabelController.Add(_currentAxisID, (IXYAxisLabelStyleController)Current.Gui.GetControllerAndControl(new object[]{als},typeof(IXYAxisLabelStyleController),UseDocument.Directly));
              }
            }


            _currentController = _MinorLabelController.ContainsKey(_currentAxisID) ? _MinorLabelController[_currentAxisID] : null;

            View.SetCurrentContentWithEnable(
              _currentController == null ? null : _currentController.ViewObject,
              _currentController != null && _enableMinorLabels[_currentAxisID],
              "Enable minor labels");
          }
          break;

      }
    }


    void SetLayerSecondaryChoice()
    {
      string[] names = new string[1] { "Common" };
      string name = names[0];
      this._primaryChoice = TabType.Unique;
      View.InitializeSecondaryChoice(names, name);
    }

    void SetHorzVertSecondaryChoice()
    {
      string[] names = new string[2] { "X-Scale", "Y-Scale" };
      string name = names[_currentScale];
      this._primaryChoice = TabType.Scales;
      View.InitializeSecondaryChoice(names, name);
    }

    void SetEdgeSecondaryChoice()
    {
      string[] names = new string[_axisStyleInfoSortedByName.Count];
      string name = string.Empty;
      for (int i = 0; i < names.Length; i++)
      {
        names[i] = _axisStyleInfoSortedByName[i].NameOfAxisStyle;
        if (_axisStyleInfoSortedByName[i].Identifier == _currentAxisID)
          name = _axisStyleInfoSortedByName[i].NameOfAxisStyle;
      }
      this._primaryChoice = TabType.Styles;
      View.InitializeSecondaryChoice(names, name);
    }

    void SetPlaneSecondaryChoice()
    {
      this._primaryChoice = TabType.Planes;
      string[] names = new string[] { "Front" };
      string name = "Front";
      View.InitializeSecondaryChoice(names, name);
    }

    public void EhView_PageChanged(string firstChoice)
    {
      ApplyCurrentController(false);

      _currentPageName = firstChoice;
      SetCurrentTabController(true);
    }

    public void EhView_PageEnabledChanged(bool pageEnabled)
    {
      if (pageEnabled)
      {
        switch (_currentPageName)
        {
          case "TitleAndFormat":
            // Create an axis style for this and add it to the layer
            _doc.AxisStyles.CreateDefault(_currentAxisID);
            _enableAxisStyle[_currentAxisID] = true;
            break;
          case "MajorLabels":
            // create a major label style and add it
            _doc.AxisStyles[_currentAxisID].ShowMajorLabels = true;
            this._enableMajorLabels[_currentAxisID] = pageEnabled;
            break;
          case "MinorLabels":
            // create a major label style and add it
            _doc.AxisStyles[_currentAxisID].ShowMinorLabels = true;
            this._enableMinorLabels[_currentAxisID] = pageEnabled;
            break;
        }
        SetCurrentTabController(false);
      }
      else // page is disabled
      {
        switch (_currentPageName)
        {
          case "TitleAndFormat":
            _enableAxisStyle[_currentAxisID] = pageEnabled;
            break;
          case "MajorLabels":
            this._enableMajorLabels[_currentAxisID] = pageEnabled;
            break;
          case "MinorLabels":
            this._enableMinorLabels[_currentAxisID] = pageEnabled;
            break;
        }
        SetCurrentTabController(false);
      }

    }

    public void EhView_SecondChoiceChanged(int index, string item)
    {
        if (!ApplyCurrentController(false))
          return;

      if (_primaryChoice == TabType.Scales)
      {
        _currentScale = index;
      }
      else if (_primaryChoice == TabType.Styles)
      {
        _currentAxisID = _axisStyleInfoSortedByName[index].Identifier;
      }
      else if (_primaryChoice == TabType.Planes)
      {
        _currentPlaneID = _planeIdentifier[index];
      }

      SetCurrentTabController(false);
    }

    void EhView_TabValidating(object sender, CancelEventArgs e)
    {
      if(!ApplyCurrentController(true))
        e.Cancel = true;
    }

    bool ApplyCurrentController(bool force)
      {
      if (_currentController == null)
        return true;

      if (!force && object.ReferenceEquals(_currentController, _lastControllerApplied))
        return true;

      if (!_currentController.Apply())
        return false;
      _lastControllerApplied = _currentController;

      if(object.ReferenceEquals(_currentController,_coordinateController))
      {
        _doc.CoordinateSystem = (G2DCoordinateSystem)_coordinateController.ModelObject;
        SetCoordinateSystemDependentObjects();
      }
      else if (_currentPageName == "TitleAndFormat")
      {
        // if we have enabled/disabled some items, we must update the enable states of the minor/major controllers
        AxisStyle axstyle = (AxisStyle)_currentController.ModelObject;
        if (_currentAxisID == axstyle.StyleID)
        {
          _enableMajorLabels[axstyle.StyleID] = axstyle.ShowMajorLabels;
          _enableMinorLabels[axstyle.StyleID] = axstyle.ShowMinorLabels;
        }
        else // than we have applied a position offset, and the old StyleID is no longer valid
        {
          _doc.AxisStyles.Remove(_currentAxisID);
          _doc.AxisStyles.Add(axstyle);
          _currentAxisID = axstyle.StyleID; // take this for axstyle
          SetCoordinateSystemDependentObjects();
          SetEdgeSecondaryChoice(); // update left side list of choices
          SetCurrentTabController(true); // we must simulate a page change in order to update the title-format tab page
        }
      }
      else if (_currentPageName == "MajorLabels")
      {
        if (!_enableMajorLabels[_currentAxisID] && _doc.AxisStyles.Contains(_currentAxisID))
        {
          _doc.AxisStyles[_currentAxisID].ShowMajorLabels = false;
        }
        if (_TitleFormatController.ContainsKey(_currentAxisID))
          _TitleFormatController.Remove(_currentAxisID);

      }
      else if (_currentPageName == "MinorLabels")
      {
        if (!_enableMinorLabels[_currentAxisID] && _doc.AxisStyles.Contains(_currentAxisID))
          _doc.AxisStyles[_currentAxisID].ShowMinorLabels = false;

        if (_TitleFormatController.ContainsKey(_currentAxisID))
          _TitleFormatController.Remove(_currentAxisID);
      }
      else if (_currentPageName == "GridStyle")
      {
        GridPlane gp = (GridPlane)_currentController.ModelObject;
        this._doc.GridPlanes[_currentPlaneID] = gp.IsUsed ? gp : null;
      }

      return true;
    }

    #region IMVCController Members
   

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        ILayerView oldvalue = _view;
        _view = value as ILayerView;

        if (!object.ReferenceEquals(_view, oldvalue))
        {
          if (oldvalue != null)
          {
            oldvalue.TabValidating -= EhView_TabValidating;
            oldvalue.Controller = null;
          }
          if (_view != null)
          {
            _view.TabValidating += EhView_TabValidating;
            _view.Controller = this;
            SetViewElements();
          }
        }
      }
    }

    public object ModelObject
    {
      get { return this._originalDoc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      ApplyCurrentController(false);

      // Remove axis styles that are not enabled
      List<CSLineID> toremove = new List<CSLineID>();
      foreach (CSLineID id in _doc.AxisStyleIDs)
      {
        if (_enableAxisStyle[id] == false)
          toremove.Add(id);
      }
      foreach (CSLineID id in toremove)
        _doc.AxisStyles.Remove(id);


      _doc.GridPlanes.RemoveUnused(); // Remove unused grid planes

      _originalDoc.CopyFrom(_doc); // _doc remains suspended
    
      return true;
    }

    #endregion

    #region Dialog

    public static bool ShowDialog(XYPlotLayer layer)
    {
      return ShowDialog( layer, "Scale", new CSLineID(0, 0));
    }
    public static bool ShowDialog( XYPlotLayer layer, string currentPage)
    {
      return ShowDialog(layer, currentPage, new CSLineID(0, 0));
    }
    public static bool ShowDialog(XYPlotLayer layer, string currentPage, CSLineID currentEdge)
    {
      LayerController ctrl = new LayerController(layer, currentPage, currentEdge);
      LayerControl view = new LayerControl();
      ctrl.View = view;

      return Current.Gui.ShowDialog(ctrl, layer.Name, true);
    }


    #endregion

    #region Edit Handlers

    public static void RegisterEditHandlers()
    {
      // register here editor methods

      XYPlotLayer.AxisScaleEditorMethod = new DoubleClickHandler(EhAxisScaleEdit);
      XYPlotLayer.AxisStyleEditorMethod = new DoubleClickHandler(EhAxisStyleEdit);
      XYPlotLayer.AxisLabelMajorStyleEditorMethod = new DoubleClickHandler(EhAxisLabelMajorStyleEdit);
      XYPlotLayer.AxisLabelMinorStyleEditorMethod = new DoubleClickHandler(EhAxisLabelMinorStyleEdit);
      XYPlotLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);

    }

    public static bool EhLayerPositionEdit(IHitTestObject hit)
    {
      XYPlotLayer layer = hit.HittedObject as XYPlotLayer;
      if (layer == null)
        return false;

      ShowDialog( layer, "Position");

      return false;
    }

    public static bool EhAxisScaleEdit(IHitTestObject hit)
    {
      AxisLineStyle style = hit.HittedObject as AxisLineStyle;
      if (style == null || hit.ParentLayer == null)
        return false;


      ShowDialog( hit.ParentLayer, "Scale", style.AxisStyleID);

      return false;
    }

    public static bool EhAxisStyleEdit(IHitTestObject hit)
    {
      AxisLineStyle style = hit.HittedObject as AxisLineStyle;
      if (style == null || hit.ParentLayer == null)
        return false;

      ShowDialog(hit.ParentLayer, "TitleAndFormat", style.AxisStyleID);

      return false;
    }

    public static bool EhAxisLabelMajorStyleEdit(IHitTestObject hit)
    {
      AxisLabelStyle style = hit.HittedObject as AxisLabelStyle;
      if (style == null || hit.ParentLayer == null)
        return false;

      ShowDialog( hit.ParentLayer, "MajorLabels", style.AxisStyleID);

      return false;
    }
    public static bool EhAxisLabelMinorStyleEdit(IHitTestObject hit)
    {
      AxisLabelStyle style = hit.HittedObject as AxisLabelStyle;
      if (style == null || hit.ParentLayer == null)
        return false;

      ShowDialog(hit.ParentLayer, "MinorLabels", style.AxisStyleID);

      return false;
    }

    #endregion


  }
}
