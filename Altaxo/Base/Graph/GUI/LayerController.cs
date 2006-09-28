#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Graph.GUI
{
  #region Interfaces
  public interface ILayerController: Main.GUI.IApplyController
  {
    void EhView_PageChanged(string firstChoice);
    void EhView_SecondChoiceChanged(int index, string item);
    void EhView_PageEnabledChanged( bool pageEnabled);

  }

  public interface ILayerView
  {

    ILayerController Controller { get; set; }

    System.Windows.Forms.Form Form  { get; }

    void AddTab(string name, string text);

    object CurrentContent { get; set; }
    void SetCurrentContentWithEnable(object guielement, bool enable, string title);
    bool IsPageEnabled { get; set; }

    void SelectTab(string name);
    void InitializeSecondaryChoice(string[] names, string name);

  }
  #endregion

  /// <summary>
  /// Summary description for LayerController.
  /// </summary>
  public class LayerController : ILayerController
  {
    protected ILayerView _view;

    protected XYPlotLayer _layer;

    private string   m_CurrentPage;

    enum TabType { Unique, Scales, Styles };
    TabType _primaryChoice; // which tab type is currently choosen
    private int _currentScale; // which scale is choosen 0==X-AxisScale, 1==Y-AxisScale
    private CS2DLineID _currentAxisID; // which style is currently choosen

  

    Main.GUI.IMVCController m_CurrentController;

  
    protected ILayerPositionController m_LayerPositionController;
    protected ILineScatterLayerContentsController m_LayerContentsController;
    protected IAxisScaleController[] m_AxisScaleController;
    protected ITitleFormatLayerController[] m_TitleFormatLayerController;
    protected Altaxo.Gui.Graph.IXYAxisLabelStyleController[] m_LabelStyleController;
    protected Altaxo.Gui.Graph.IXYAxisLabelStyleController[] m_MinorLabelStyleController;
    protected Altaxo.Main.GUI.IMVCAController[] _GridStyleController;

    Dictionary<CS2DLineID, A2DAxisStyleInformation> _axisStyleIds;
    List<A2DAxisStyleInformation> _axisStyleInfoSortedByName;
    
    Dictionary<CS2DLineID, ITitleFormatLayerController> _TitleFormatController;
    Dictionary<CS2DLineID, Altaxo.Gui.Graph.IXYAxisLabelStyleController> _MajorLabelController;
    Dictionary<CS2DLineID, Altaxo.Gui.Graph.IXYAxisLabelStyleController> _MinorLabelController;

    Dictionary<CS2DLineID, bool> _enableMajorLabels;
    Dictionary<CS2DLineID, bool> _enableMinorLabels;
  
  
    public LayerController(XYPlotLayer layer)
      : this(layer,"Scale",1,null)
    {
    }
    public LayerController(XYPlotLayer layer, string currentPage, CS2DLineID id)
      : this(layer,currentPage,id.ParallelAxisNumber,id)
    {
    }


    LayerController(XYPlotLayer layer, string currentPage, int axisScaleIdx, CS2DLineID id)
    {
      _layer = layer;

      // collect the AxisStyleIdentifier from the actual layer and also all possible AxisStyleIdentifier
      _axisStyleIds = new Dictionary<CS2DLineID, A2DAxisStyleInformation>();
      _axisStyleInfoSortedByName = new List<A2DAxisStyleInformation>();
      foreach (CS2DLineID ids in _layer.CoordinateSystem.GetJoinedAxisStyleIdentifier(_layer.ScaleStyles.AxisStyleIDs, new CS2DLineID[] { id }))
      {
        A2DAxisStyleInformation info = _layer.CoordinateSystem.GetAxisStyleInformation(ids);
        _axisStyleIds.Add(info.Identifier, info);
        _axisStyleInfoSortedByName.Add(info);
      }


      _currentScale = axisScaleIdx;
      _currentAxisID = id;


      m_AxisScaleController = new AxisScaleController[2];
      _GridStyleController = new Altaxo.Main.GUI.IMVCAController[2];
      _TitleFormatController = new Dictionary<CS2DLineID, ITitleFormatLayerController>();
      _MajorLabelController = new Dictionary<CS2DLineID, Altaxo.Gui.Graph.IXYAxisLabelStyleController>();
      _MinorLabelController = new Dictionary<CS2DLineID, Altaxo.Gui.Graph.IXYAxisLabelStyleController>();

      _enableMajorLabels = new Dictionary<CS2DLineID, bool>();
      _enableMinorLabels = new Dictionary<CS2DLineID, bool>();
      foreach(CS2DLineID ident in _axisStyleIds.Keys)
      {
        AxisStyle prop = layer.ScaleStyles.AxisStyle(ident);
        if(prop==null)
        {
          _enableMajorLabels.Add(ident, false);
          _enableMinorLabels.Add(ident, false);
        }
        else
        {
          _enableMajorLabels.Add(ident, prop.ShowMajorLabels);
          _enableMinorLabels.Add(ident, prop.ShowMinorLabels);
        }
      }

      m_CurrentPage = currentPage;

      if(null!=View)
        SetViewElements();
    }

    public static void RegisterEditHandlers()
    {
      // register here editor methods

      XYPlotLayer.AxisScaleEditorMethod = new DoubleClickHandler(EhAxisScaleEdit);
      XYPlotLayer.AxisStyleEditorMethod = new DoubleClickHandler(EhAxisStyleEdit);
      XYPlotLayer.AxisLabelStyleEditorMethod = new DoubleClickHandler(EhAxisLabelStyleEdit);
      XYPlotLayer.LayerPositionEditorMethod = new DoubleClickHandler(EhLayerPositionEdit);

    }

    public static bool EhLayerPositionEdit(IHitTestObject hit)
    {
      XYPlotLayer layer = hit.HittedObject as XYPlotLayer;
      if(layer==null)
        return false;

      ShowDialog(Current.MainWindow, layer, "Position");

      return false;
    }

    public static bool EhAxisScaleEdit(IHitTestObject hit)
    {
      AxisLineStyle style = hit.HittedObject as AxisLineStyle;
      if(style==null || hit.ParentLayer==null)
        return false;
   

      ShowDialog(Current.MainWindow, hit.ParentLayer, "Scale", style.AxisStyleID);

      return false;
    }

    public static bool EhAxisStyleEdit(IHitTestObject hit)
    {
      AxisLineStyle style = hit.HittedObject as AxisLineStyle;
      if(style==null || hit.ParentLayer==null)
        return false;

      ShowDialog(Current.MainWindow, hit.ParentLayer, "TitleAndFormat",style.AxisStyleID);

      return false;
    }

    public static bool EhAxisLabelStyleEdit(IHitTestObject hit)
    {
      AxisLabelStyle style = hit.HittedObject as AxisLabelStyle;
      if(style==null || hit.ParentLayer==null)
        return false;

      ShowDialog(Current.MainWindow, hit.ParentLayer, "MajorLabels",style.AxisStyleID);

      return false;
    }

    public ILayerView View
    {
      get { return _view; }
      set 
      {
        if(null!=_view)
        {
          _view.Controller = null;
        }

        _view = value;
        
        if(null!=_view)
        {
          _view.Controller = this;
          SetViewElements();
        }
      }
    }

    void SetViewElements()
    {
      if(null==View)
        return;

      // add all necessary Tabs
      View.AddTab("Scale","Scale");
      View.AddTab("TitleAndFormat","Title&&Format");
      View.AddTab("Contents","Contents");
      View.AddTab("Position","Position");
      View.AddTab("MajorLabels","Major labels");
      View.AddTab("MinorLabels","Minor labels");
      View.AddTab("GridStyle","Grid style");

      // Set the controller of the current visible Tab
      SetCurrentTabController(true);
    }



    void SetCurrentTabController(bool pageChanged)
    {
      switch(m_CurrentPage)
      {
        case "Contents":
          if (pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetLayerSecondaryChoice();
          }
          if (null == m_LayerContentsController)
          {
            m_LayerContentsController = new LineScatterLayerContentsController(_layer);
            m_LayerContentsController.View = new LineScatterLayerContentsControl();
          }
          m_CurrentController = m_LayerContentsController;
          View.CurrentContent = m_CurrentController.ViewObject;
          break;
        case "Position":
          if (pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetLayerSecondaryChoice();
          }
          if (null == m_LayerPositionController)
          {
            m_LayerPositionController = new LayerPositionController(_layer);
            m_LayerPositionController.View = new LayerPositionControl();
          }
          m_CurrentController = m_LayerPositionController;
          View.CurrentContent = m_LayerPositionController.ViewObject;
          break;


        case "Scale":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetHorzVertSecondaryChoice();
          }
          if (m_AxisScaleController[_currentScale] == null)
          {
            m_AxisScaleController[_currentScale] = new AxisScaleController(_layer, _currentScale);
            m_AxisScaleController[_currentScale].ViewObject = new AxisScaleControl();
          }
          m_CurrentController = m_AxisScaleController[_currentScale];
          View.CurrentContent = m_CurrentController.ViewObject;
          break;
        case "GridStyle":
          if (pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetHorzVertSecondaryChoice();
          }

          if (_layer.ScaleStyles.ContainsAxisStyle(_currentAxisID))
          {
            if (null != _GridStyleController[_currentScale])
            {
              m_CurrentController = _GridStyleController[_currentScale];
              View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, true, "Show grid");
            }
            else
            {
              EhView_PageEnabledChanged(true);
            }
          }
          else
          {
            View.SetCurrentContentWithEnable(null, false, "Show grid");
          }
          break;

        case "TitleAndFormat":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetEdgeSecondaryChoice();
          }

          if (_layer.ScaleStyles.ContainsAxisStyle(_currentAxisID))
          {
            if (_TitleFormatController.ContainsKey(_currentAxisID))
            {
              m_CurrentController = _TitleFormatController[_currentAxisID];
              View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, true, "Show axis");
            }
            else
            {
              EhView_PageEnabledChanged(true);
            }
          }
          else
          {
            View.SetCurrentContentWithEnable(null, false, "Show axis");
          }
          break;
       
        case "MajorLabels":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetEdgeSecondaryChoice();
          }
          if (_layer.ScaleStyles.ContainsAxisStyle(_currentAxisID))
          {
            if (_MajorLabelController.ContainsKey(_currentAxisID))
            {
              m_CurrentController = _MajorLabelController[_currentAxisID];
              View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, _enableMajorLabels[_currentAxisID], "Show major labels");
            }
            else if(_enableMajorLabels[_currentAxisID])
            {
              EhView_PageEnabledChanged(_enableMajorLabels[_currentAxisID]);
            }
            else
            {
              View.SetCurrentContentWithEnable(null, false, "Show minor labels");
            }
          }
          else
          {
            View.SetCurrentContentWithEnable(null, false, "Show major labels");
          }
          break;
        case "MinorLabels":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetEdgeSecondaryChoice();
          }
          if (_layer.ScaleStyles.ContainsAxisStyle(_currentAxisID))
          {
            if (_MinorLabelController.ContainsKey(_currentAxisID))
            {
              m_CurrentController = _MinorLabelController[_currentAxisID];
              View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, _enableMinorLabels[_currentAxisID], "Show minor labels");
            }
            else if(_enableMinorLabels[_currentAxisID])
            {
              EhView_PageEnabledChanged(_enableMinorLabels[_currentAxisID]);
            }
            else
            {
              View.SetCurrentContentWithEnable(null, false, "Show minor labels");
            }
          }
          else
          {
            View.SetCurrentContentWithEnable(null, false, "Show minor labels");
          }
          break;
      }
    }


    void SetLayerSecondaryChoice()
    {
      string[] names = new string[1]{"Common"};
      string name = names[0];
      this._primaryChoice = TabType.Unique;
      View.InitializeSecondaryChoice(names,name);
    }

    void SetHorzVertSecondaryChoice()
    {
      string[] names = new string[2]{"Y-Scale","X-Scale"};
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
      View.InitializeSecondaryChoice(names,name);
    }
  
    public void EhView_PageChanged(string firstChoice)
    {
      m_CurrentPage = firstChoice;
      SetCurrentTabController(true);
    }

    public void EhView_PageEnabledChanged( bool pageEnabled)
    {
      if(m_CurrentPage=="MajorLabels")
        this._enableMajorLabels[_currentAxisID] = pageEnabled;
      if(m_CurrentPage=="MinorLabels")
        this._enableMinorLabels[_currentAxisID] = pageEnabled;

      if (pageEnabled)
      {
        if (m_CurrentPage == "TitleAndFormat")
        {
          if (!_TitleFormatController.ContainsKey(_currentAxisID))
          {

            TitleFormatLayerController newCtrl = new TitleFormatLayerController(_layer.ScaleStyles.AxisStyleEnsured(_currentAxisID));
            newCtrl.View = new TitleFormatLayerControl();
            _TitleFormatController.Add(_currentAxisID, newCtrl);
            m_CurrentController = newCtrl;
            View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, pageEnabled, "Show title and format");
          }
        }
        else if (m_CurrentPage == "MajorLabels")
        {
          if (!_MajorLabelController.ContainsKey(_currentAxisID))
          {
            Altaxo.Gui.Graph.XYAxisLabelStyleController newCtrl = new Altaxo.Gui.Graph.XYAxisLabelStyleController((AxisLabelStyle)_layer.ScaleStyles.AxisStyleEnsured(_currentAxisID).MajorLabelStyle);
            newCtrl.View = new Altaxo.Gui.Graph.XYAxisLabelStyleControl();
            _MajorLabelController.Add(_currentAxisID, newCtrl);
            m_CurrentController = newCtrl;
            View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, pageEnabled, "Show title and format");
          }
        }
        else if (m_CurrentPage == "MinorLabels")
        {
          if (!_MinorLabelController.ContainsKey(_currentAxisID))
          {
            Altaxo.Gui.Graph.XYAxisLabelStyleController newCtrl = new Altaxo.Gui.Graph.XYAxisLabelStyleController((AxisLabelStyle)_layer.ScaleStyles.AxisStyleEnsured(_currentAxisID).MinorLabelStyle);
            newCtrl.View = new Altaxo.Gui.Graph.XYAxisLabelStyleControl();
            m_CurrentController = newCtrl;
            View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, pageEnabled, "Show title and format");
            _MinorLabelController.Add(_currentAxisID, newCtrl);
          }
        }
        else if (m_CurrentPage == "GridStyle")
        {
          if (null==_GridStyleController[_currentScale])
          {
            if (_layer.ScaleStyles.ScaleStyle(_currentScale).GridStyle == null)
              _layer.ScaleStyles.ScaleStyle(_currentScale).GridStyle = new GridStyle();

            Altaxo.Gui.Graph.XYGridStyleController newCtrl = new Altaxo.Gui.Graph.XYGridStyleController(_layer.ScaleStyles.ScaleStyle(_currentScale).GridStyle);
            newCtrl.ViewObject = new Altaxo.Gui.Graph.XYGridStyleControl();
            _GridStyleController[_currentScale] = newCtrl;

            m_CurrentController = newCtrl;
            View.SetCurrentContentWithEnable(m_CurrentController.ViewObject, pageEnabled, "Show grid");
          }
        }
      }
      else // if !PageEnabled
      {

      }

    }

    public void EhView_SecondChoiceChanged(int index, string item)
    {
      if (_primaryChoice == TabType.Scales)
      {
        _currentScale = index;
      }
      else if (_primaryChoice == TabType.Styles)
      {
        _currentAxisID = _axisStyleInfoSortedByName[index].Identifier;
      }

      SetCurrentTabController(false);
    }


    public static bool ShowDialog(System.Windows.Forms.Form parentWindow, XYPlotLayer layer)
    {
      return ShowDialog(parentWindow,layer,"Scale", new CS2DLineID(0,0) );
    }
    public static bool ShowDialog(System.Windows.Forms.Form parentWindow, XYPlotLayer layer, string currentPage)
    {
      return ShowDialog(parentWindow, layer, currentPage, new CS2DLineID(0,0));
    }

    public static bool ShowDialog(System.Windows.Forms.Form parentWindow, XYPlotLayer layer, string currentPage, CS2DLineID currentEdge)
    {
     
      LayerController ctrl = new LayerController(layer,currentPage,currentEdge);
      LayerControl view = new LayerControl();
      ctrl.View = view;

      Main.GUI.DialogShellController dsc = new Main.GUI.DialogShellController(
        new Main.GUI.DialogShellView(view), ctrl);

      return dsc.ShowDialog(parentWindow);
    }


    #region IApplyController Members

    public bool Apply()
    {
      int i;

      if(null!=this.m_LayerContentsController && !this.m_LayerContentsController.Apply())
        return false;

      if(null!=m_LayerPositionController && !this.m_LayerPositionController.Apply())
        return false;

      // do the apply for all controllers that are allocated so far
      for(i=0;i<2;i++)
      {
        if(null != m_AxisScaleController[i] && !m_AxisScaleController[i].Apply())
        {
          return false;
        }
      }


      foreach (CS2DLineID id in _TitleFormatController.Keys)
      {
        if(!_TitleFormatController[id].Apply())
        {
          return false;
        }
      }

      foreach (CS2DLineID id in _axisStyleIds.Keys)
      {
        if (this._enableMajorLabels[id])
        {
          if (_MajorLabelController.ContainsKey(id) && !_MajorLabelController[id].Apply())
          {
            return false;
          }
        }
        else
        {
          if(_layer.ScaleStyles.ContainsAxisStyle(id))
            _layer.ScaleStyles.AxisStyle(id).ShowMajorLabels = false;
        }
      }

      foreach (CS2DLineID id in _axisStyleIds.Keys)
      {
        if (this._enableMinorLabels[id])
        {
          if (_MinorLabelController.ContainsKey(id)  && !_MinorLabelController[id].Apply())
          {
            return false;
          }
        }
        else
        {
          if (_layer.ScaleStyles.ContainsAxisStyle(id))
            this._layer.ScaleStyles.AxisStyle(id).ShowMinorLabels = false;
        }
      }

      for(i=0;i<2;i++)
      {
        if (_GridStyleController[i] != null)
        {
          if (_GridStyleController[i].Apply())
            this._layer.ScaleStyles.ScaleStyle(i).GridStyle = (GridStyle)_GridStyleController[i].ModelObject;
          else
            return false;
        }
      }

      return true;
    }

    #endregion
  }
}
