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
using Altaxo.Serialization;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;
using Altaxo.Collections;
namespace Altaxo.Gui.Graph
{

  #region Interfaces
  public interface ILayerPositionViewEventSink
  {
    void EhView_LinkedLayerChanged(SelectableListNode txt);
    void EhView_LeftTypeChanged(SelectableListNode txt);
    void EhView_TopTypeChanged(SelectableListNode txt);
    void EhView_WidthTypeChanged(SelectableListNode txt);
    void EhView_HeightTypeChanged(SelectableListNode txt);

    void EhView_LeftChanged(string txt, ref bool bCancel);
    void EhView_TopChanged(string txt, ref bool bCancel);
    void EhView_WidthChanged(string txt, ref bool bCancel);
    void EhView_HeightChanged(string txt, ref bool bCancel);
    void EhView_RotationChanged(float newValue);
    void EhView_ScaleChanged(string txt, ref bool bCancel);
    void EhView_ClipDataToFrameChanged(bool value);
  }

  public interface ILayerPositionView 
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    ILayerPositionViewEventSink Controller { get; set; }

    void InitializeLeft(string txt);
    void InitializeTop(string txt);
    void InitializeHeight(string txt);
    void InitializeWidth(string txt);
    void InitializeRotation(float val);
    void InitializeScale(string txt);
    void InitializeClipDataToFrame(bool value);

    void InitializeLeftType(SelectableListNodeList names);
    void InitializeTopType(SelectableListNodeList names);
    void InitializeHeightType(SelectableListNodeList names);
    void InitializeWidthType(SelectableListNodeList names);
    void InitializeLinkedLayer(SelectableListNodeList names);

    IAxisLinkView GetXAxisLink();
    IAxisLinkView GetYAxisLink();

  }
  #endregion

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
  public class LayerPositionController : ILayerPositionViewEventSink,  IMVCAController
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
        m_ClipDataToFrame = m_Layer.ClipDataToFrame == LayerDataClipping.StrictToCS;

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

        InitializeWidthValue();
        InitializeHeightValue();

        InitializeLeftValue();
        InitializeTopValue();

        View.InitializeRotation((float)m_Rotation);
        View.InitializeScale(Serialization.GUIConversion.GetPercentMeasureText(m_Scale));
        View.InitializeClipDataToFrame(m_ClipDataToFrame);

        InitializePositionTypes();
        InitializeSizeTypes();
        InitializeLinkedAxisChoices();

        // initialize the axis link properties
        m_XAxisLink.View = View.GetXAxisLink();
        m_YAxisLink.View = View.GetYAxisLink();

      }

    }

    void InitializeLinkedAxisChoices()
    {
      SelectableListNodeList list = new SelectableListNodeList();
      list.Add(new SelectableListNode("None",null,m_LinkedLayer==null));
      if (null != m_Layer.ParentLayerList)
      {
        for (int i = 0; i < m_Layer.ParentLayerList.Count; i++)
        {
          if (!m_Layer.IsLayerDependentOnMe(m_Layer.ParentLayerList[i]))
            list.Add(new SelectableListNode("XYPlotLayer " + i.ToString(),m_Layer.ParentLayerList[i],m_Layer.ParentLayerList[i]==m_LinkedLayer));
        }
      }
      View.InitializeLinkedLayer(list);
    }

    /// <summary>
    /// Get the list of position types in dependence of whether or not we have a linked layer.
    /// </summary>
    /// <returns>List of possible position types.</returns>
    SelectableListNodeList GetPositionTypeList()
    {
      SelectableListNodeList list = new SelectableListNodeList();
      XYPlotLayerPositionType toadd = XYPlotLayerPositionType.AbsoluteValue;
      list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd),  toadd, false));
      toadd = XYPlotLayerPositionType.RelativeToGraphDocument;
      list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd),  toadd, false));
      if (null != m_LinkedLayer)
      {
        toadd = XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear;
        list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd),  toadd, false));
        toadd = XYPlotLayerPositionType.RelativeThisNearToLinkedLayerFar;
        list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd),  toadd, false));
        toadd = XYPlotLayerPositionType.RelativeThisFarToLinkedLayerNear;
        list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd),  toadd, false));
        toadd = XYPlotLayerPositionType.RelativeThisFarToLinkedLayerFar;
        list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd),  toadd, false));
      }
      return list;
    }

    /// <summary>
    /// Get the list of size types in dependence of whether or not we have a linked layer.
    /// </summary>
    /// <returns>List of possible size types.</returns>
    SelectableListNodeList GetSizeTypeList()
    {
      SelectableListNodeList list = new SelectableListNodeList();
      XYPlotLayerSizeType toadd = XYPlotLayerSizeType.AbsoluteValue;
      list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd), toadd, false));
      toadd = XYPlotLayerSizeType.RelativeToGraphDocument;
      list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd), toadd, false));
      if (null != m_LinkedLayer)
      {
        toadd = XYPlotLayerSizeType.RelativeToLinkedLayer;
        list.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(toadd), toadd, false));
      }
      return list;
    }


    void InitializePositionTypes()
    {
      SelectableListNodeList leftlist = GetPositionTypeList();
      foreach (SelectableListNode node in leftlist)
        if ((XYPlotLayerPositionType)node.Item == m_LeftType)
          node.Selected = true;

      View.InitializeLeftType(leftlist);

      SelectableListNodeList toplist = GetPositionTypeList();
      foreach (SelectableListNode node in toplist)
        if ((XYPlotLayerPositionType)node.Item == m_TopType)
          node.Selected = true;

      View.InitializeTopType(toplist);
    }
    void InitializeLeftValue()
    {
      string text = (m_LeftType== XYPlotLayerPositionType.AbsoluteValue)?
        Serialization.GUIConversion.GetLengthMeasureText(m_Left):
        Serialization.GUIConversion.GetPercentMeasureText(m_Left);

      View.InitializeLeft(text);
    }
    void InitializeTopValue()
    {
      string text = (m_TopType == XYPlotLayerPositionType.AbsoluteValue) ?
        Serialization.GUIConversion.GetLengthMeasureText(m_Top) :
        Serialization.GUIConversion.GetPercentMeasureText(m_Top);

      View.InitializeTop(text);
    }

    void InitializeSizeTypes()
    {
      SelectableListNodeList list = GetSizeTypeList();
      foreach (SelectableListNode node in list)
        if ((XYPlotLayerSizeType)node.Item == m_WidthType)
          node.Selected = true;

      View.InitializeWidthType(list);

      list = GetSizeTypeList();
      foreach (SelectableListNode node in list)
        if ((XYPlotLayerSizeType)node.Item == m_HeightType)
          node.Selected = true;

      View.InitializeHeightType(list);
    }

    void InitializeWidthValue()
    {
      string text = (m_WidthType == XYPlotLayerSizeType.AbsoluteValue) ?
        Serialization.GUIConversion.GetLengthMeasureText(m_Width) :
        Serialization.GUIConversion.GetPercentMeasureText(m_Width);

      View.InitializeWidth(text);
    }
    void InitializeHeightValue()
    {
      string text = (m_HeightType == XYPlotLayerSizeType.AbsoluteValue) ?
        Serialization.GUIConversion.GetLengthMeasureText(m_Height) :
        Serialization.GUIConversion.GetPercentMeasureText(m_Height);

      View.InitializeHeight(text);
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
        m_Layer.ClipDataToFrame = m_ClipDataToFrame?LayerDataClipping.StrictToCS:LayerDataClipping.None;

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

    public void EhView_LinkedLayerChanged(SelectableListNode node)
    {
      XYPlotLayer oldLinkedLayer = m_LinkedLayer;
      m_LinkedLayer = (XYPlotLayer)node.Item;

      // we have to check if there is a need to update the type comboboxes
      if (oldLinkedLayer != null && m_LinkedLayer != null)
      {
      }
      else if (oldLinkedLayer == null && m_LinkedLayer != null)
      {
        InitializePositionTypes();
        InitializeSizeTypes();
      }
      else if (oldLinkedLayer != null && m_LinkedLayer == null)
      {
        if (m_LeftType != XYPlotLayerPositionType.AbsoluteValue && m_LeftType != XYPlotLayerPositionType.RelativeToGraphDocument)
          ChangeLeftType(XYPlotLayerPositionType.RelativeToGraphDocument);
        if (m_TopType != XYPlotLayerPositionType.AbsoluteValue && m_TopType != XYPlotLayerPositionType.RelativeToGraphDocument)
          ChangeTopType(XYPlotLayerPositionType.RelativeToGraphDocument);

        if (m_WidthType != XYPlotLayerSizeType.AbsoluteValue && m_WidthType != XYPlotLayerSizeType.RelativeToGraphDocument)
          ChangeWidthType(XYPlotLayerSizeType.RelativeToGraphDocument);
        if (m_HeightType != XYPlotLayerSizeType.AbsoluteValue && m_HeightType != XYPlotLayerSizeType.RelativeToGraphDocument)
          ChangeHeightType(XYPlotLayerSizeType.RelativeToGraphDocument);

        InitializeSizeTypes();
        InitializePositionTypes();
      }
    }


    void ChangeLeftType(XYPlotLayerPositionType newType)
    {
      XYPlotLayerPositionType oldType = m_LeftType;
      if (newType == oldType)
        return;
      double oldPosition = m_Layer.XPositionToPointUnits(m_Left,oldType);
      m_Left  = m_Layer.XPositionToUserUnits(oldPosition,newType);
      m_LeftType = newType;

      InitializeLeftValue();
    }
    void ChangeTopType(XYPlotLayerPositionType newType)
    {
      XYPlotLayerPositionType oldType = m_TopType;
      if (newType == oldType)
        return;
      double oldPosition = m_Layer.YPositionToPointUnits(m_Top, oldType);
      m_Top = m_Layer.YPositionToUserUnits(oldPosition, newType);
      m_TopType = newType;

      InitializeTopValue();
    }
    void ChangeWidthType(XYPlotLayerSizeType newType)
    {
      XYPlotLayerSizeType oldType = m_WidthType;
      if (newType == oldType)
        return;
      double oldSize = m_Layer.WidthToPointUnits(m_Width, oldType);
      m_Width = m_Layer.WidthToUserUnits(oldSize, newType);
      m_WidthType = newType;
      InitializeWidthValue();

    }
    void ChangeHeightType(XYPlotLayerSizeType newType)
    {
      XYPlotLayerSizeType oldType = m_HeightType;
      if (newType == oldType)
        return;
      double oldSize = m_Layer.HeightToPointUnits(m_Height, oldType);
      m_Height = m_Layer.HeightToUserUnits(oldSize, newType);
      m_HeightType = newType;
      InitializeHeightValue();

    }

    public void EhView_LeftTypeChanged(SelectableListNode node)
    {
      ChangeLeftType((XYPlotLayerPositionType)node.Item);
    }

    public void EhView_TopTypeChanged(SelectableListNode node)
    {
      ChangeTopType((XYPlotLayerPositionType)node.Item);
    }

    public void EhView_WidthTypeChanged(SelectableListNode node)
    {
      ChangeWidthType((XYPlotLayerSizeType)node.Item);
    }

    public void EhView_HeightTypeChanged(SelectableListNode node)
    {
      ChangeHeightType((XYPlotLayerSizeType)node.Item);
    }

    public void EhView_LeftChanged(string txt, ref bool bCancel)
    {
      if (m_LeftType == XYPlotLayerPositionType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref m_Left);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref m_Left);
    }

    void ILayerPositionViewEventSink.EhView_TopChanged(string txt, ref bool bCancel)
    {
      if (m_TopType == XYPlotLayerPositionType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref m_Top);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref m_Top);
    }

    public void EhView_WidthChanged(string txt, ref bool bCancel)
    {
      if (m_WidthType == XYPlotLayerSizeType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref m_Width);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref m_Width);
    }

    public void EhView_HeightChanged(string txt, ref bool bCancel)
    {
      if (m_HeightType == XYPlotLayerSizeType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref m_Height);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref m_Height);
    }

    public void EhView_RotationChanged(float newVal)
    {
      m_Rotation = newVal;
    }

    public void EhView_ScaleChanged(string txt, ref bool bCancel)
    {
      bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref m_Scale);
    }

    public void EhView_ClipDataToFrameChanged(bool value)
    {
      m_ClipDataToFrame = value;
    }

    #endregion
  }
}
