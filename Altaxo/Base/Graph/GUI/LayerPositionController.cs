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
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI
{

  #region Interfaces
  public interface ILayerPositionController : Main.GUI.IApplyController, Main.GUI.IMVCController
  {
    /// <summary>
    /// Get/sets the view this controller controls.
    /// </summary>
    ILayerPositionView View { get; set; }

    void EhView_LinkedLayerChanged(string txt);
    void EhView_CommonTypeChanged(string txt);
    void EhView_LeftTypeChanged(string txt);
    void EhView_TopTypeChanged(string txt);
    void EhView_WidthTypeChanged(string txt);
    void EhView_HeightTypeChanged(string txt);

    void EhView_LeftChanged(string txt, ref bool bCancel);
    void EhView_TopChanged(string txt, ref bool bCancel);
    void EhView_WidthChanged(string txt, ref bool bCancel);
    void EhView_HeightChanged(string txt, ref bool bCancel);
    void EhView_RotationChanged(string txt, ref bool bCancel);
    void EhView_ScaleChanged(string txt, ref bool bCancel);
    void EhView_ClipDataToFrameChanged(bool value);
  }

  public interface ILayerPositionView : Main.GUI.IMVCView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    ILayerPositionController Controller { get; set; }

    /// <summary>
    /// Gets the hosting parent form of this view.
    /// </summary>
    System.Windows.Forms.Form Form  { get; }

    void InitializeLeft(string txt);
    void InitializeTop(string txt);
    void InitializeHeight(string txt);
    void InitializeWidth(string txt);
    void InitializeRotation(string txt);
    void InitializeScale(string txt);
    void InitializeClipDataToFrame(bool value);

    void InitializeLeftType(string[] names, string txt);
    void InitializeTopType(string[] names, string txt);
    void InitializeHeightType(string[] names, string txt);
    void InitializeWidthType(string[] names, string txt);
    void InitializeLinkedLayer(string[] names, string name);

    IAxisLinkView GetXAxisLink();
    IAxisLinkView GetYAxisLink();

  }
  #endregion

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
  public class LayerPositionController : ILayerPositionController
  {
    // the view
    ILayerPositionView m_View;

    // the document
    XYPlotLayer m_Layer;

    // Shadow variables
    double m_Left, m_Top, m_Width, m_Height, m_Rotation, m_Scale;
    XYPlotLayerPositionType m_LeftType, m_TopType;
    XYPlotLayerSizeType      m_HeightType, m_WidthType;
    bool m_ClipDataToFrame;
    XYPlotLayer m_LinkedLayer;
    IAxisLinkController m_XAxisLink, m_YAxisLink;

    public LayerPositionController(XYPlotLayer layer)
    {
      m_Layer = layer;
      SetElements(true);
    }


    public void SetElements(bool bInit)
    {


      if(bInit)
      {
        m_Height    = m_Layer.UserHeight;
        m_Width     = m_Layer.UserWidth;
        m_Left      = m_Layer.UserXPosition;
        m_Top       = m_Layer.UserYPosition;
        m_Rotation  = m_Layer.Rotation;
        m_Scale     = m_Layer.Scale;
        m_ClipDataToFrame = m_Layer.ClipDataToFrame;

        m_LeftType = m_Layer.UserXPositionType;
        m_TopType  = m_Layer.UserYPositionType;
        m_HeightType = m_Layer.UserHeightType;
        m_WidthType = m_Layer.UserWidthType;
        m_LinkedLayer = m_Layer.LinkedLayer;


        m_XAxisLink = new AxisLinkController(m_Layer,true);
        m_YAxisLink = new AxisLinkController(m_Layer,false);
      }

      if(View!=null)
      {
      
        View.InitializeHeight(Serialization.NumberConversion.ToString(m_Height));
        View.InitializeWidth(Serialization.NumberConversion.ToString(m_Width));

        View.InitializeLeft(Serialization.NumberConversion.ToString(m_Left));
        View.InitializeTop(Serialization.NumberConversion.ToString(m_Top));

        View.InitializeRotation(Serialization.NumberConversion.ToString(m_Rotation));
        View.InitializeScale(Serialization.NumberConversion.ToString(m_Scale));
        View.InitializeClipDataToFrame(m_ClipDataToFrame);


        // Fill the comboboxes of the x and y position with possible values
        string [] names = Enum.GetNames(typeof(XYPlotLayerPositionType));
        
        string nameLeft = Enum.GetName(typeof(XYPlotLayerPositionType),m_LeftType);
        string nameTop = Enum.GetName(typeof(XYPlotLayerPositionType),m_TopType);

        View.InitializeLeftType(names,nameLeft);
        View.InitializeTopType(names,nameTop);

        // Fill the comboboxes of the width  and height with possible values
        names = Enum.GetNames(typeof(XYPlotLayerSizeType));
        string nameWidth  = Enum.GetName(typeof(XYPlotLayerSizeType),m_WidthType);
        string nameHeigth = Enum.GetName(typeof(XYPlotLayerSizeType),m_HeightType);

        View.InitializeWidthType(names,nameWidth);
        View.InitializeHeightType(names,nameHeigth);

        // Fill the combobox of linked layer with possible values
        System.Collections.ArrayList arr = new System.Collections.ArrayList();
        arr.Add("None");
        if(null!=m_Layer.ParentLayerList)
        {
          for(int i=0;i<m_Layer.ParentLayerList.Count;i++)
          {
            if(!m_Layer.IsLayerDependentOnMe(m_Layer.ParentLayerList[i]))
              arr.Add("XYPlotLayer " + i.ToString());
          }
        }

        // now if we have a linked layer, set the selected item to the right value
        string nameLL= null==m_LinkedLayer ? "None" : "XYPlotLayer " + m_LinkedLayer.Number;

        View.InitializeLinkedLayer((string[])arr.ToArray(typeof(string)),nameLL);

        // initialize the axis link properties
        m_XAxisLink.View = View.GetXAxisLink();
        m_YAxisLink.View = View.GetYAxisLink();

      }

    }

  

    #region IApplyController Members

    public bool Apply()
    {
      bool bSuccess = true;

      try
      {
        m_Layer.LinkedLayer = m_LinkedLayer;

      
        // now update the layer
        m_Layer.Rotation = (float)m_Rotation;
        m_Layer.Scale    = (float)m_Scale;
        m_Layer.ClipDataToFrame = m_ClipDataToFrame;

        m_Layer.SetSize(m_Width,m_WidthType,m_Height,m_HeightType);
        m_Layer.SetPosition(m_Left,m_LeftType,m_Top,m_TopType);

        if(!this.m_XAxisLink.Apply())
          bSuccess = false;
        if(!this.m_YAxisLink.Apply())
          bSuccess = false;
      }
      catch(Exception)
      {
        return false; // indicate that something failed
      }
      return bSuccess;
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return View;
      }
      set
      {
        View = value as ILayerPositionView;
      }
    }

    public object ModelObject
    {
      get { return this.m_Layer; }
    }

    #endregion

    #region ILayerPositionController Members

    public ILayerPositionView View
    {
      get
      {
        return m_View;
      }
      set
      {
        if(null!=m_View)
          m_View.Controller = null;
        
        m_View = value;

        if(null!=m_View)
        {
          m_View.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    public void EhView_LinkedLayerChanged(string txt)
    {
      int linkedlayernumber = -1;
      string label = "XYPlotLayer ";
      if(txt.StartsWith(label))
        linkedlayernumber= System.Convert.ToInt32(txt.Substring(label.Length));

      m_LinkedLayer = linkedlayernumber<0 ? null : m_Layer.ParentLayerList[linkedlayernumber];
    }

    public void EhView_CommonTypeChanged(string txt)
    {
      // TODO:  Add LayerPositionController.EhView_CommonTypeChanged implementation
    }

    public void EhView_LeftTypeChanged(string txt)
    {
      this.m_LeftType = (XYPlotLayerPositionType)Enum.Parse(typeof(XYPlotLayerPositionType),txt);
    }

    public void EhView_TopTypeChanged(string txt)
    {
      this.m_TopType = (XYPlotLayerPositionType)Enum.Parse(typeof(XYPlotLayerPositionType),txt);
    }

    public void EhView_WidthTypeChanged(string txt)
    {
      this.m_WidthType = (XYPlotLayerSizeType)Enum.Parse(typeof(XYPlotLayerSizeType),txt);
    }

    public void EhView_HeightTypeChanged(string txt)
    {
      this.m_HeightType = (XYPlotLayerSizeType)Enum.Parse(typeof(XYPlotLayerSizeType),txt);
    }

    public void EhView_LeftChanged(string txt, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(txt, out m_Left);
    }

    void ILayerPositionController.EhView_TopChanged(string txt, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(txt, out m_Top);
    }

    public void EhView_WidthChanged(string txt, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(txt, out m_Width);
    }

    public void EhView_HeightChanged(string txt, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(txt, out m_Height);
    }

    public void EhView_RotationChanged(string txt, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(txt, out m_Rotation);
    }

    public void EhView_ScaleChanged(string txt, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(txt, out m_Scale);
    }

    public void EhView_ClipDataToFrameChanged(bool value)
    {
      m_ClipDataToFrame = value;
    }

    #endregion
  }
}
