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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IAxisLinkController : IMVCAController
  {
    /// <summary>
    /// Get/sets the view this controller controls.
    /// </summary>
    IAxisLinkView View { get; set; }

    /// <summary>
    /// Called if the type of the link is changed.
    /// </summary>
    /// <param name="linktype">The linktype. Valid arguments are "None", "Straight" and "Custom".</param>
    void EhView_LinkTypeChanged(ScaleLinkType linktype);

    /// <summary>
    /// Called when the contents of OrgA is changed.
    /// </summary>
    /// <param name="orgA">Contents of OrgA.</param>
    /// <param name="bCancel">Normally false, this can be set to true if OrgA is not a valid entry.</param>
    void EhView_OrgAValidating(string orgA, ref bool bCancel);
    /// <summary>
    /// Called when the contents of OrgB is changed.
    /// </summary>
    /// <param name="orgB">Contents of OrgB.</param>
    /// <param name="bCancel">Normally false, this can be set to true if OrgB is not a valid entry.</param>
    void EhView_OrgBValidating(string orgB, ref bool bCancel);
    /// <summary>
    /// Called when the contents of EndA is changed.
    /// </summary>
    /// <param name="endA">Contents of EndA.</param>
    /// <param name="bCancel">Normally false, this can be set to true if EndA is not a valid entry.</param>
    void EhView_EndAValidating(string endA, ref bool bCancel);
    /// <summary>
    /// Called when the contents of EndB is changed.
    /// </summary>
    /// <param name="endB">Contents of EndB.</param>
    /// <param name="bCancel">Normally false, this can be set to true if EndB is not a valid entry.</param>
    void EhView_EndBValidating(string endB, ref bool bCancel);


  }

  public interface IAxisLinkView : IMVCView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IAxisLinkController Controller { get; set; }

    /// <summary>
    /// Gets the hosting parent form of this view.
    /// </summary>
    System.Windows.Forms.Form Form  { get; }

    /// <summary>
    /// Initializes the type of the link.
    /// </summary>
    /// <param name="linktype"></param>
    void LinkType_Initialize(ScaleLinkType linktype);

    /// <summary>
    /// Initializes the content of the OrgA edit box.
    /// </summary>
    void OrgA_Initialize(string text);

    /// <summary>
    /// Initializes the content of the OrgB edit box.
    /// </summary>
    void OrgB_Initialize(string text);

    /// <summary>
    /// Initializes the content of the EndA edit box.
    /// </summary>
    void EndA_Initialize(string text);

    /// <summary>
    /// Initializes the content of the EndB edit box.
    /// </summary>
    void EndB_Initialize(string text);


    /// <summary>
    /// Enables / Disables the edit boxes for the org and end values
    /// </summary>
    /// <param name="bEnable">True if the boxes are enabled for editing.</param>
    void Enable_OrgAndEnd_Boxes(bool bEnable);
  
  }
  #endregion

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
  public class AxisLinkController : IAxisLinkController
  {
    IAxisLinkView m_View;
    XYPlotLayer m_Layer;
    bool  m_bXAxis;

    ScaleLinkType m_LinkType;
    double m_OrgA;
    double m_OrgB;
    double m_EndA;
    double m_EndB;


    public AxisLinkController(XYPlotLayer layer, bool bXAxis)
    {
      m_Layer = layer;
      m_bXAxis = bXAxis;
      SetElements(true);
    }


    void SetElements(bool bInit)
    {
      if(bInit)
      {
        if(m_bXAxis)
        {
          m_LinkType = m_Layer.LinkedScales.X.AxisLinkType;
          m_OrgA      = m_Layer.LinkedScales.X.LinkOrgA;
          m_OrgB = m_Layer.LinkedScales.X.LinkOrgB;
          m_EndA = m_Layer.LinkedScales.X.LinkEndA;
          m_EndB = m_Layer.LinkedScales.X.LinkEndB;
        }
        else
        {
          m_LinkType  = m_Layer.LinkedScales.Y.AxisLinkType;
          m_OrgA = m_Layer.LinkedScales.Y.LinkOrgA;
          m_OrgB = m_Layer.LinkedScales.Y.LinkOrgB;
          m_EndA = m_Layer.LinkedScales.Y.LinkEndA;
          m_EndB = m_Layer.LinkedScales.Y.LinkEndB;
        }
      }

      if(null!=View)
      {
        View.LinkType_Initialize(m_LinkType);
        View.OrgA_Initialize(m_OrgA.ToString());
        View.OrgB_Initialize(m_OrgB.ToString());
        View.EndA_Initialize(m_EndA.ToString());
        View.EndB_Initialize(m_EndB.ToString());
      }
    }
    #region ILinkAxisController Members

    public IAxisLinkView View
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

    public void EhView_LinkTypeChanged(ScaleLinkType linktype)
    {
      m_LinkType = linktype;

      if(null!=View)
        View.Enable_OrgAndEnd_Boxes(linktype == ScaleLinkType.Custom);
    }

    public void EhView_OrgAValidating(string orgA, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(orgA, out m_OrgA);
    }

    public void EhView_OrgBValidating(string orgB, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(orgB, out m_OrgB);
    }

    public void EhView_EndAValidating(string endA, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(endA, out m_EndA);
    }

    public void EhView_EndBValidating(string endB, ref bool bCancel)
    {
      bCancel = !NumberConversion.IsDouble(endB, out m_EndB);
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if(this.m_bXAxis)
      {
        m_Layer.LinkedScales.X.SetLinkParameter(m_LinkType, m_OrgA,m_OrgB,m_EndA,m_EndB);
      }
      else
      {
        m_Layer.LinkedScales.Y.SetLinkParameter(m_LinkType, m_OrgA,m_OrgB,m_EndA,m_EndB);
      }
      return true;
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
        View = value as IAxisLinkView;
      }
    }

    public object ModelObject
    {
      get { return this.m_Layer; }
    }

    #endregion
  }
}
