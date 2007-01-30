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

using Altaxo.Graph.Scales;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IAxisScaleController : IMVCAController
  {
    void EhView_AxisTypeChanged(string text);
  }

  public interface IAxisScaleView : IMVCView
  {

    IAxisScaleController Controller { get; set; }

    void InitializeAxisType(string[] arr, string sel);

    void SetBoundaryView(object guiobject);
    void SetScaleView(object guiobject);

  }
  #endregion

  /// <summary>
  /// Summary description for AxisScaleController.
  /// </summary>
  public class AxisScaleController : IAxisScaleController
  {
    public enum AxisDirection { Horizontal=0, Vertical=1 }
    protected IAxisScaleView m_View;
    protected XYPlotLayer m_Layer;
    protected int m_axisNumber;
    
    // Cached values
    protected Scale m_Axis;

    protected Scale _tempAxis;

    protected string  m_AxisOrg;
    protected string  m_Original_AxisOrg;

    protected string  m_AxisEnd;
    protected string  m_Original_AxisEnd;

    protected string  m_AxisType;
    protected string  m_Original_AxisType;

    protected string  m_AxisRescale;
    protected string  m_Original_AxisRescale;

    protected IMVCAController m_ScaleController;
    protected IMVCAController m_BoundaryController;


    public AxisScaleController(XYPlotLayer layer, int axisnumber)
    {
      m_Layer = layer;
      m_axisNumber = axisnumber;
      m_Axis = m_Layer.LinkedScales.Scale(axisnumber);
      _tempAxis = (Scale)m_Axis.Clone();


      SetElements();
    }

    public AxisScaleController(XYPlotLayer layer, Scale ax)
    {
      m_Layer = layer;
      m_axisNumber = layer.LinkedScales.IndexOf(ax);
      if (m_axisNumber < 0)
        throw new ArgumentException("Provided axis is not member of the layer");

      m_Axis = ax;
      _tempAxis = (Scale)m_Axis.Clone();


      SetElements();
    }

    public IAxisScaleView View
    {
      get { return m_View; }
      set
      {
        if(null!=m_View)
          m_View.Controller = null;

        m_View = value;

        if(null!=m_View)
        {
          m_View.Controller = this;
          SetViewElements();
        }
      }
    }

    public object ViewObject
    {
      get { return View; }
      set { View = value as IAxisScaleView; }
    }

    public object ModelObject
    {
      get { return this.m_Axis; }
    }

    public void SetElements()
    {
      SetAxisType(true);
      SetScaleController(true);
      SetBoundaryController(true);
    }

    public void SetViewElements()
    {
      SetAxisType(false);
      SetScaleController(false);
      SetBoundaryController(false);
    }



    public void SetAxisType(bool bInit)
    {
      string[] names = new string[Scale.AvailableAxes.Keys.Count];

      int i=0;
      string curraxisname=null;
      foreach(string axs in Scale.AvailableAxes.Keys)
      {
        names[i++] = axs;
        if(_tempAxis.GetType()==Scale.AvailableAxes[axs])
          curraxisname = axs;
      }

      if(bInit)
        m_AxisType = m_Original_AxisType = curraxisname;


      if(null!=View)
        View.InitializeAxisType(names,m_AxisType);
    }

    public void SetScaleController(bool bInit)
    {
      if (bInit)
      {
        object scaleObject = _tempAxis;
        m_ScaleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { scaleObject }, typeof(IMVCAController));
      }
      if (null != View)
      {
        View.SetScaleView(null==m_ScaleController ? null : m_ScaleController.ViewObject);
      }
    }

    public void SetBoundaryController(bool bInit)
    {
      if(bInit)
      {
        object rescalingObject = _tempAxis.RescalingObject;
        if (rescalingObject != null)
          m_BoundaryController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { rescalingObject, _tempAxis }, typeof(IMVCAController));
        else
          m_BoundaryController = null;
      }
      if(null!=View)
      {
        View.SetBoundaryView(null!=m_BoundaryController ? m_BoundaryController.ViewObject : null);
      }
    }

    public bool Apply()
    {
      // note: the order is essential here
      // first set the axis in the layer, _then_ apply the RescaleConditions

      m_Layer.LinkedScales.SetScale(m_axisNumber, _tempAxis);

      if (null != m_ScaleController)
      {
        if (false == m_ScaleController.Apply())
          return false;
      }

      if(null!=m_BoundaryController)
      {
        if(false==m_BoundaryController.Apply())
          return false;
      }


    
    

      SetElements();
      
    
      
      return true; // all ok
    }
   
    public void EhView_AxisTypeChanged(string text)
    {
      m_AxisType = text;
      try
      {

        System.Type axistype = (System.Type)Scale.AvailableAxes[m_AxisType];
        if(null!=axistype)
        {
          if(axistype!=_tempAxis.GetType())
          {
            // replace the current axis by a new axis of the type axistype
            Scale _oldAxis = _tempAxis;
            _tempAxis = (Scale)System.Activator.CreateInstance(axistype);

            // Try to set the same org and end as the axis before
            // this will fail for instance if we switch from linear to logarithmic with negative bounds
            try
            {
              _tempAxis.ProcessDataBounds(_oldAxis.OrgAsVariant,true,_oldAxis.EndAsVariant,true);
            }
            catch(Exception)
            {
            }

            SetScaleController(true);
            // now we have also to replace the controller and the control for the axis boundaries
            SetBoundaryController(true);
          }
        }
      }
      catch(Exception )
      {
      }
    }

  }

}
