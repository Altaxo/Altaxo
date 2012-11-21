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
    void EhView_RotationChanged(double newValue);
    void EhView_ScaleChanged(double newValue);
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
  }
  #endregion

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
	[ExpectedTypeOfView(typeof(ILayerPositionView))]
  public class LayerPositionController : ILayerPositionViewEventSink,  IMVCAController
  {
    // the view
    ILayerPositionView _view;

    // the document
    XYPlotLayer _layer;

    // Shadow variables
    double _left, _top, _width, _height, _rotation, _scale;
    XYPlotLayerPositionType _leftPositionType, _topPositionType;
    XYPlotLayerSizeType      _heightType, _widthType;
    bool _clipDataToFrame;
    XYPlotLayer _linkedLayer;

    public LayerPositionController(XYPlotLayer layer)
    {
      _layer = layer;
      SetElements(true);
    }


    public void SetElements(bool bInit)
    {


      if(bInit)
      {
        _height    = _layer.UserHeight;
        _width     = _layer.UserWidth;
        _left      = _layer.UserXPosition;
        _top       = _layer.UserYPosition;
        _rotation  = _layer.Rotation;
        _scale     = _layer.Scale;
        _clipDataToFrame = _layer.ClipDataToFrame == LayerDataClipping.StrictToCS;

        _leftPositionType = _layer.UserXPositionType;
        _topPositionType  = _layer.UserYPositionType;
        _heightType = _layer.UserHeightType;
        _widthType = _layer.UserWidthType;
        _linkedLayer = _layer.LinkedLayer;
      }

      if(View!=null)
      {

        InitializeWidthValue();
        InitializeHeightValue();

        InitializeLeftValue();
        InitializeTopValue();

        View.InitializeRotation((float)_rotation);
        View.InitializeScale(GUIConversion.GetPercentMeasureText(_scale));
        View.InitializeClipDataToFrame(_clipDataToFrame);

        InitializePositionTypes();
        InitializeSizeTypes();
        InitializeLinkedAxisChoices();
      }

    }

    void InitializeLinkedAxisChoices()
    {
      SelectableListNodeList list = new SelectableListNodeList();
      list.Add(new SelectableListNode("None",null,_linkedLayer==null));
      if (null != _layer.ParentLayerList)
      {
        for (int i = 0; i < _layer.ParentLayerList.Count; i++)
        {
          if (!_layer.IsLayerDependentOnMe(_layer.ParentLayerList[i]))
            list.Add(new SelectableListNode("XYPlotLayer " + i.ToString(),_layer.ParentLayerList[i],_layer.ParentLayerList[i]==_linkedLayer));
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
      if (null != _linkedLayer)
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
      if (null != _linkedLayer)
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
        if ((XYPlotLayerPositionType)node.Tag == _leftPositionType)
          node.IsSelected = true;

      View.InitializeLeftType(leftlist);

      SelectableListNodeList toplist = GetPositionTypeList();
      foreach (SelectableListNode node in toplist)
        if ((XYPlotLayerPositionType)node.Tag == _topPositionType)
          node.IsSelected = true;

      View.InitializeTopType(toplist);
    }
    void InitializeLeftValue()
    {
      string text = (_leftPositionType== XYPlotLayerPositionType.AbsoluteValue)?
        GUIConversion.GetLengthMeasureText(_left):
        GUIConversion.GetPercentMeasureText(_left);

      View.InitializeLeft(text);
    }
    void InitializeTopValue()
    {
      string text = (_topPositionType == XYPlotLayerPositionType.AbsoluteValue) ?
        GUIConversion.GetLengthMeasureText(_top) :
        GUIConversion.GetPercentMeasureText(_top);

      View.InitializeTop(text);
    }

    void InitializeSizeTypes()
    {
      SelectableListNodeList list = GetSizeTypeList();
      foreach (SelectableListNode node in list)
        if ((XYPlotLayerSizeType)node.Tag == _widthType)
          node.IsSelected = true;

      View.InitializeWidthType(list);

      list = GetSizeTypeList();
      foreach (SelectableListNode node in list)
        if ((XYPlotLayerSizeType)node.Tag == _heightType)
          node.IsSelected = true;

      View.InitializeHeightType(list);
    }

    void InitializeWidthValue()
    {
      string text = (_widthType == XYPlotLayerSizeType.AbsoluteValue) ?
        GUIConversion.GetLengthMeasureText(_width) :
        GUIConversion.GetPercentMeasureText(_width);

      View.InitializeWidth(text);
    }
    void InitializeHeightValue()
    {
      string text = (_heightType == XYPlotLayerSizeType.AbsoluteValue) ?
        GUIConversion.GetLengthMeasureText(_height) :
        GUIConversion.GetPercentMeasureText(_height);

      View.InitializeHeight(text);
    }

    #region IApplyController Members

    public bool Apply()
    {
      bool bSuccess = true;

      try
      {
        _layer.LinkedLayer = _linkedLayer;

      
        // now update the layer
        _layer.Rotation = _rotation;
        _layer.Scale    = _scale;
        _layer.ClipDataToFrame = _clipDataToFrame?LayerDataClipping.StrictToCS:LayerDataClipping.None;

        _layer.SetSize(_width,_widthType,_height,_heightType);
        _layer.SetPosition(_left,_leftPositionType,_top,_topPositionType);
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
      get { return this._layer; }
    }

    #endregion

    #region ILayerPositionController Members

    public ILayerPositionView View
    {
      get
      {
        return _view;
      }
      set
      {
        if(null!=_view)
          _view.Controller = null;
        
        _view = value;

        if(null!=_view)
        {
          _view.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    public void EhView_LinkedLayerChanged(SelectableListNode node)
    {
      XYPlotLayer oldLinkedLayer = _linkedLayer;
      _linkedLayer = (XYPlotLayer)node.Tag;

      // we have to check if there is a need to update the type comboboxes
      if (oldLinkedLayer != null && _linkedLayer != null)
      {
      }
      else if (oldLinkedLayer == null && _linkedLayer != null)
      {
        InitializePositionTypes();
        InitializeSizeTypes();
      }
      else if (oldLinkedLayer != null && _linkedLayer == null)
      {
        if (_leftPositionType != XYPlotLayerPositionType.AbsoluteValue && _leftPositionType != XYPlotLayerPositionType.RelativeToGraphDocument)
          ChangeLeftType(XYPlotLayerPositionType.RelativeToGraphDocument);
        if (_topPositionType != XYPlotLayerPositionType.AbsoluteValue && _topPositionType != XYPlotLayerPositionType.RelativeToGraphDocument)
          ChangeTopType(XYPlotLayerPositionType.RelativeToGraphDocument);

        if (_widthType != XYPlotLayerSizeType.AbsoluteValue && _widthType != XYPlotLayerSizeType.RelativeToGraphDocument)
          ChangeWidthType(XYPlotLayerSizeType.RelativeToGraphDocument);
        if (_heightType != XYPlotLayerSizeType.AbsoluteValue && _heightType != XYPlotLayerSizeType.RelativeToGraphDocument)
          ChangeHeightType(XYPlotLayerSizeType.RelativeToGraphDocument);

        InitializeSizeTypes();
        InitializePositionTypes();
      }
    }


    void ChangeLeftType(XYPlotLayerPositionType newType)
    {
      XYPlotLayerPositionType oldType = _leftPositionType;
      if (newType == oldType)
        return;
      double oldPosition = _layer.XPositionToPointUnits(_left,oldType);
      _left  = _layer.XPositionToUserUnits(oldPosition,newType);
      _leftPositionType = newType;

      InitializeLeftValue();
    }
    void ChangeTopType(XYPlotLayerPositionType newType)
    {
      XYPlotLayerPositionType oldType = _topPositionType;
      if (newType == oldType)
        return;
      double oldPosition = _layer.YPositionToPointUnits(_top, oldType);
      _top = _layer.YPositionToUserUnits(oldPosition, newType);
      _topPositionType = newType;

      InitializeTopValue();
    }
    void ChangeWidthType(XYPlotLayerSizeType newType)
    {
      XYPlotLayerSizeType oldType = _widthType;
      if (newType == oldType)
        return;
      double oldSize = _layer.WidthToPointUnits(_width, oldType);
      _width = _layer.WidthToUserUnits(oldSize, newType);
      _widthType = newType;
      InitializeWidthValue();

    }
    void ChangeHeightType(XYPlotLayerSizeType newType)
    {
      XYPlotLayerSizeType oldType = _heightType;
      if (newType == oldType)
        return;
      double oldSize = _layer.HeightToPointUnits(_height, oldType);
      _height = _layer.HeightToUserUnits(oldSize, newType);
      _heightType = newType;
      InitializeHeightValue();

    }

    public void EhView_LeftTypeChanged(SelectableListNode node)
    {
      ChangeLeftType((XYPlotLayerPositionType)node.Tag);
    }

    public void EhView_TopTypeChanged(SelectableListNode node)
    {
      ChangeTopType((XYPlotLayerPositionType)node.Tag);
    }

    public void EhView_WidthTypeChanged(SelectableListNode node)
    {
      ChangeWidthType((XYPlotLayerSizeType)node.Tag);
    }

    public void EhView_HeightTypeChanged(SelectableListNode node)
    {
      ChangeHeightType((XYPlotLayerSizeType)node.Tag);
    }

    public void EhView_LeftChanged(string txt, ref bool bCancel)
    {
      if (_leftPositionType == XYPlotLayerPositionType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref _left);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref _left);
    }

    void ILayerPositionViewEventSink.EhView_TopChanged(string txt, ref bool bCancel)
    {
      if (_topPositionType == XYPlotLayerPositionType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref _top);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref _top);
    }

    public void EhView_WidthChanged(string txt, ref bool bCancel)
    {
      if (_widthType == XYPlotLayerSizeType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref _width);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref _width);
    }

    public void EhView_HeightChanged(string txt, ref bool bCancel)
    {
      if (_heightType == XYPlotLayerSizeType.AbsoluteValue)
        bCancel = !GUIConversion.GetLengthMeasureValue(txt, ref _height);
      else
        bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref _height);
    }

    public void EhView_RotationChanged(double newVal)
    {
      _rotation = newVal;
    }

    public void EhView_ScaleChanged(double newValue)
    {

			_scale = newValue;
     // bCancel = !GUIConversion.GetPercentMeasureValue(txt, ref m_Scale);
    }

    public void EhView_ClipDataToFrameChanged(bool value)
    {
      _clipDataToFrame = value;
    }

    #endregion
  }
}
