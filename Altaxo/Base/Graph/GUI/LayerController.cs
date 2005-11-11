#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

    System.Windows.Forms.Control CurrentContent { get; set; }
    void SetCurrentContentWithEnable(System.Windows.Forms.Control control, bool enable, string title);
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
    protected ILayerView m_View;

    protected XYPlotLayer m_Layer;

    private string   m_CurrentPage;
    private EdgeType m_CurrentEdge;

    private bool[] _enableMajorLabels = new bool[4];
    private bool[] _enableMinorLabels = new bool[4];

    Main.GUI.IMVCController m_CurrentController;

    enum ElementType { Unique, HorzVert, Edge };

    protected ILayerPositionController m_LayerPositionController;
    protected ILineScatterLayerContentsController m_LayerContentsController;
    protected IAxisScaleController[] m_AxisScaleController;
    protected ITitleFormatLayerController[] m_TitleFormatLayerController;
    protected Altaxo.Gui.Graph.IXYAxisLabelStyleController[] m_LabelStyleController;
    protected Altaxo.Gui.Graph.IXYAxisLabelStyleController[] m_MinorLabelStyleController;

    
    public int CurrHorzVertIdx
    {
      get 
      {
        return (m_CurrentEdge==EdgeType.Left || m_CurrentEdge==EdgeType.Right) ? 0 : 1;
      }
    }
    public int CurrEdgeIdx
    {
      get 
      {
        return (int)m_CurrentEdge;
      }
    }

  
    public LayerController(XYPlotLayer layer)
      : this(layer,"Scale",EdgeType.Bottom)
    {
    }

    public LayerController(XYPlotLayer layer, string currentPage, EdgeType currentEdge)
    {
      m_Layer = layer;

      m_LayerContentsController = new LineScatterLayerContentsController(m_Layer);

      m_LayerPositionController = new LayerPositionController(m_Layer);

      m_AxisScaleController = new AxisScaleController[2]{
                                                          new AxisScaleController(m_Layer,AxisScaleController.AxisDirection.Vertical),
                                                          new AxisScaleController(m_Layer,AxisScaleController.AxisDirection.Horizontal)
                                                        };

      m_TitleFormatLayerController = new TitleFormatLayerController[4]{
                                                                        new TitleFormatLayerController(m_Layer,EdgeType.Left),
                                                                        new TitleFormatLayerController(m_Layer,EdgeType.Bottom),
                                                                        new TitleFormatLayerController(m_Layer,EdgeType.Right),
                                                                        new TitleFormatLayerController(m_Layer,EdgeType.Top)
                                                                      };

      m_LabelStyleController = new Altaxo.Gui.Graph.XYAxisLabelStyleController[4]{
                                                                        new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.LeftLabelStyle),
                                                                        new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.BottomLabelStyle),
                                                                        new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.RightLabelStyle),
                                                                        new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.TopLabelStyle)
                                                                      };

      m_MinorLabelStyleController = new Altaxo.Gui.Graph.XYAxisLabelStyleController[4]{
                                                                                   new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.AxisStyles[EdgeType.Left].MinorLabelStyle),
                                                                                   new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.AxisStyles[EdgeType.Bottom].MinorLabelStyle),
                                                                                   new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.AxisStyles[EdgeType.Right].MinorLabelStyle),
                                                                                   new Altaxo.Gui.Graph.XYAxisLabelStyleController((XYAxisLabelStyle)m_Layer.AxisStyles[EdgeType.Top].MinorLabelStyle),
      };


      for(int i=0;i<4;i++)
        this._enableMajorLabels[i] = layer.AxisStyles[(EdgeType)i].ShowMajorLabels;

      for(int i=0;i<4;i++)
        this._enableMinorLabels[i] = layer.AxisStyles[(EdgeType)i].ShowMinorLabels;


      m_CurrentPage = currentPage;
      m_CurrentEdge = currentEdge;

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

      ShowDialog(Current.MainWindow,layer, "Position", EdgeType.Bottom);

      return false;
    }

    public static bool EhAxisScaleEdit(IHitTestObject hit)
    {
      XYAxisStyle style = hit.HittedObject as XYAxisStyle;
      if(style==null || hit.ParentLayer==null)
        return false;

      EdgeType edge = EdgeType.Bottom;
      if(hit.ParentLayer.LeftAxisStyle==style)
        edge = EdgeType.Left;
      else if(hit.ParentLayer.BottomAxisStyle==style)
        edge = EdgeType.Bottom;
      else if(hit.ParentLayer.RightAxisStyle==style)
        edge = EdgeType.Right;
      else if(hit.ParentLayer.TopAxisStyle==style)
        edge = EdgeType.Top;

      ShowDialog(Current.MainWindow, hit.ParentLayer, "Scale", edge);

      return false;
    }

    public static bool EhAxisStyleEdit(IHitTestObject hit)
    {
      XYAxisStyle style = hit.HittedObject as XYAxisStyle;
      if(style==null || hit.ParentLayer==null)
        return false;

      EdgeType edge = EdgeType.Bottom;
      if(hit.ParentLayer.LeftAxisStyle==style)
        edge = EdgeType.Left;
      else if(hit.ParentLayer.BottomAxisStyle==style)
        edge = EdgeType.Bottom;
      else if(hit.ParentLayer.RightAxisStyle==style)
        edge = EdgeType.Right;
      else if(hit.ParentLayer.TopAxisStyle==style)
        edge = EdgeType.Top;

      ShowDialog(Current.MainWindow, hit.ParentLayer, "TitleAndFormat",edge);

      return false;
    }

    public static bool EhAxisLabelStyleEdit(IHitTestObject hit)
    {
      XYAxisLabelStyle style = hit.HittedObject as XYAxisLabelStyle;
      if(style==null || hit.ParentLayer==null)
        return false;

      EdgeType edge = EdgeType.Bottom;
      if(hit.ParentLayer.LeftLabelStyle==style)
        edge = EdgeType.Left;
      else if(hit.ParentLayer.BottomLabelStyle==style)
        edge = EdgeType.Bottom;
      else if(hit.ParentLayer.RightLabelStyle==style)
        edge = EdgeType.Right;
      else if(hit.ParentLayer.TopLabelStyle==style)
        edge = EdgeType.Top;

      ShowDialog(Current.MainWindow, hit.ParentLayer, "MajorLabels",edge);

      return false;
    }

    public ILayerView View
    {
      get { return m_View; }
      set 
      {
        if(null!=m_View)
        {
          m_View.Controller = null;
        }

        m_View = value;
        
        if(null!=m_View)
        {
          m_View.Controller = this;
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

      // Set the controller of the current visible Tab
      SetCurrentTabController(true);
    }



    void SetCurrentTabController(bool pageChanged)
    {
      if(null!=m_CurrentController) 
        m_CurrentController.ViewObject=null; // detach current view

      switch(m_CurrentPage)
      {
        case "Scale":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetHorzVertSecondaryChoice();
            View.CurrentContent = new AxisScaleControl();
          }

          m_CurrentController = m_AxisScaleController[CurrHorzVertIdx];
          
          
          break;
        case "TitleAndFormat":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetEdgeSecondaryChoice();
            View.CurrentContent = new TitleFormatLayerControl();
          }
          m_CurrentController = m_TitleFormatLayerController[CurrEdgeIdx];
          break;
        case "Contents":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetLayerSecondaryChoice();
            View.CurrentContent = new LineScatterLayerContentsControl();
          }

          m_CurrentController = m_LayerContentsController;
          break;
        case "Position":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetLayerSecondaryChoice();
            View.CurrentContent = new LayerPositionControl();
          }

          m_CurrentController = m_LayerPositionController;
          break;
        case "MajorLabels":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetEdgeSecondaryChoice();
            View.SetCurrentContentWithEnable( new Altaxo.Gui.Graph.XYAxisLabelStyleControl(), this._enableMajorLabels[CurrEdgeIdx], "Show major labels");
          }
          m_CurrentController = m_LabelStyleController[CurrEdgeIdx];
          View.IsPageEnabled = this._enableMajorLabels[CurrEdgeIdx];
          break;
        case "MinorLabels":
          if(pageChanged)
          {
            View.SelectTab(m_CurrentPage);
            SetEdgeSecondaryChoice();
            View.SetCurrentContentWithEnable( new Altaxo.Gui.Graph.XYAxisLabelStyleControl(), this._enableMinorLabels[CurrEdgeIdx], "Show minor labels");
          }
          m_CurrentController = m_MinorLabelStyleController[CurrEdgeIdx];
          View.IsPageEnabled = this._enableMinorLabels[CurrEdgeIdx];
          break;
      }

      if(null!=m_CurrentController)
        m_CurrentController.ViewObject = View.CurrentContent; 
    }


    void SetLayerSecondaryChoice()
    {
      string[] names = new string[1]{"XYPlotLayer"};
      string name = names[0];
      View.InitializeSecondaryChoice(names,name);
    }

    void SetHorzVertSecondaryChoice()
    {
      string[] names = new string[2]{"Vertical","Horizontal"};
      string name = names[CurrHorzVertIdx];
      View.InitializeSecondaryChoice(names,name);
    }

    void SetEdgeSecondaryChoice()
    {
      string[] names = new string[4]{"Left","Bottom","Right","Top"};
      string name = names[CurrEdgeIdx];
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
        this._enableMajorLabels[this.CurrEdgeIdx] = pageEnabled;
      if(m_CurrentPage=="MinorLabels")
        this._enableMinorLabels[this.CurrEdgeIdx] = pageEnabled;
    }

    public void EhView_SecondChoiceChanged(int index, string item)
    {
      switch(item)
      {
        case "Left":
          this.m_CurrentEdge = EdgeType.Left;
          break;
        case "Bottom":
          this.m_CurrentEdge = EdgeType.Bottom;
          break;
        case "Right":
          this.m_CurrentEdge = EdgeType.Right;
          break;
        case "Top":
          this.m_CurrentEdge = EdgeType.Top;
          break;
        case "Horizontal":
          if(this.m_CurrentEdge!=EdgeType.Bottom && this.m_CurrentEdge!=EdgeType.Top)
            this.m_CurrentEdge=EdgeType.Bottom;
          break;
        case "Vertical":
          if(this.m_CurrentEdge!=EdgeType.Left && this.m_CurrentEdge!=EdgeType.Right)
            this.m_CurrentEdge=EdgeType.Left;
          break;
      }
      SetCurrentTabController(false);
    }


    public static bool ShowDialog(System.Windows.Forms.Form parentWindow, XYPlotLayer layer)
    {
      return ShowDialog(parentWindow,layer,"Scale",EdgeType.Bottom);
    }

    public static bool ShowDialog(System.Windows.Forms.Form parentWindow, XYPlotLayer layer, string currentPage, EdgeType currentEdge)
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

      if(!this.m_LayerContentsController.Apply())
        return false;

      if(!this.m_LayerPositionController.Apply())
        return false;

      // do the apply for all controllers that are allocated so far
      for(i=0;i<2;i++)
      {
        if(!m_AxisScaleController[i].Apply())
        {
          return false;
        }
      }


      for(i=0;i<4;i++)
      {
        if(!m_TitleFormatLayerController[i].Apply())
        {
          return false;
        }
      }

      for(i=0;i<4;i++)
      {
        if(!m_LabelStyleController[i].Apply())
        {
          return false;
        }
         this.m_Layer.AxisStyles[(EdgeType)i].ShowMajorLabels = this._enableMajorLabels[i];
      }

      for(i=0;i<4;i++)
      {
        if(!m_MinorLabelStyleController[i].Apply())
        {
          return false;
        }
        this.m_Layer.AxisStyles[(EdgeType)i].ShowMinorLabels = this._enableMinorLabels[i];
      }

      return true;
    }

    #endregion
  }
}
