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
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IDensityImagePlotStyleViewEventSink
  {
    /// <summary>
    /// Called if the type of the scaling is changed.
    /// </summary>
    /// <param name="scalingstyle">The scaling type.</param>
    void EhView_ScalingStyleChanged(string scalingstyle);

    /// <summary>
    /// Called when the contents of RangeFrom is changed.
    /// </summary>
    /// <param name="rangeFrom">Contents of RangeFrom.</param>
    /// <param name="bCancel">Normally false, this can be set to true if RangeFrom is not a valid entry.</param>
    void EhView_RangeFromValidating(string rangeFrom, ref bool bCancel);
    /// <summary>
    /// Called when the contents of RangeTo is changed.
    /// </summary>
    /// <param name="rangeTo">Contents of RangeTo.</param>
    /// <param name="bCancel">Normally false, this can be set to true if RangeTo is not a valid entry.</param>
    void EhView_RangeToValidating(string rangeTo, ref bool bCancel);

    void EhView_ColorBelowChanged(System.Drawing.Color color);

    void EhView_ColorAboveChanged(System.Drawing.Color color);

    void EhView_ColorInvalidChanged(System.Drawing.Color color);

    void EhView_ClipToLayerChanged(bool bClip);

  }

  public interface IDensityImagePlotStyleView 
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IDensityImagePlotStyleViewEventSink Controller { get; set; }

    /// <summary>
    /// Initializes the type of the link.
    /// </summary>
    /// <param name="names">The possible names for this choice.</param>
    /// <param name="name">The actual name of the choice.</param>
    void ScalingStyle_Initialize(string[] names, string name);

    /// <summary>
    /// Initializes the content of the RangeFrom edit box.
    /// </summary>
    void RangeFrom_Initialize(string text);

    /// <summary>
    /// Initializes the content of the RangeTo edit box.
    /// </summary>
    void RangeTo_Initialize(string text);

    /// <summary>
    /// Initializes the content of the ColorBelow combo box.
    /// </summary>
    void ColorBelow_Initialize(System.Drawing.Color color);

    /// <summary>
    /// Initializes the content of the ColorAbove combo box.
    /// </summary>
    void ColorAbove_Initialize(System.Drawing.Color color);

    /// <summary>
    /// Initializes the content of the ColorInvalid combo box.
    /// </summary>
    void ColorInvalid_Initialize(System.Drawing.Color color);

    /// <summary>
    /// Initializes the content of the ClipToLayer checkbox
    /// </summary>
    /// <param name="bClip">True if the image is clipped at the layer boundaries.</param>
    void ClipToLayer_Initialize(bool bClip);
  
  }
  #endregion

  /// <summary>
  /// Controller for the density image plot style
  /// </summary>
  [UserControllerForObject(typeof(DensityImagePlotStyle))]
  [ExpectedTypeOfView(typeof(IDensityImagePlotStyleView))]
  public class DensityImagePlotStyleController : IMVCANController, IDensityImagePlotStyleViewEventSink
  {
    IDensityImagePlotStyleView m_View;
    DensityImagePlotStyle m_PlotStyle;

    UseDocument _useDocumentCopy;

    DensityImagePlotStyle.ScalingStyle m_ScalingStyle;
    double m_RangeFrom;
    double m_RangeTo;
    System.Drawing.Color m_ColorBelow;
    System.Drawing.Color m_ColorAbove;
    System.Drawing.Color m_ColorInvalid;
    bool m_ClipToLayer;


    public DensityImagePlotStyleController(DensityImagePlotStyle plotStyle)
    {
      m_PlotStyle = plotStyle;
      SetElements(true);
    }

    public DensityImagePlotStyleController()
    {
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is DensityImagePlotStyle))
        return false;
      m_PlotStyle = (DensityImagePlotStyle)args[0];
      //_doc = _originalDoc; // _useDocumentCopy == UseDocument.Directly ? _originalDoc : (DensityImagePlotStyle)_originalDoc.Clone();
      SetElements(true); // initialize always because we have to update the temporary variables
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { _useDocumentCopy = value; }
    }


    void SetElements(bool bInit)
    {
      if(bInit)
      {
        m_ScalingStyle  = m_PlotStyle.Scaling;
        m_RangeFrom     = m_PlotStyle.RangeFrom;
        m_RangeTo     = m_PlotStyle.RangeTo;
        m_ColorBelow  = m_PlotStyle.ColorBelow;
        m_ColorAbove  = m_PlotStyle.ColorAbove;
        m_ColorInvalid  = m_PlotStyle.ColorInvalid;
        m_ClipToLayer = m_PlotStyle.ClipToLayer;
      
      }

      if(null!=View)
      {
        View.ScalingStyle_Initialize(System.Enum.GetNames(typeof(DensityImagePlotStyle.ScalingStyle)),System.Enum.GetName(typeof(DensityImagePlotStyle.ScalingStyle),m_ScalingStyle));
        View.RangeFrom_Initialize(double.IsNaN(m_RangeFrom) ? "" : m_RangeFrom.ToString());
        View.RangeTo_Initialize(double.IsNaN(m_RangeTo) ? "" : m_RangeTo.ToString());
        View.ColorBelow_Initialize(m_ColorBelow);
        View.ColorAbove_Initialize(m_ColorAbove);
        View.ColorInvalid_Initialize(m_ColorInvalid);
        View.ClipToLayer_Initialize(m_ClipToLayer);
      }
    }
    #region IDensityImagePlotStyleController Members

    public IDensityImagePlotStyleView View
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

    public void EhView_ScalingStyleChanged(string scalingStyle)
    {
      m_ScalingStyle = (DensityImagePlotStyle.ScalingStyle)System.Enum.Parse(typeof(DensityImagePlotStyle.ScalingStyle),scalingStyle);
    }

    public void EhView_RangeFromValidating(string rangeFrom, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(rangeFrom, out m_RangeFrom))
        m_RangeFrom = double.NaN;
    }

    public void EhView_RangeToValidating(string rangeTo, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(rangeTo, out m_RangeTo))
        m_RangeTo = double.NaN;
    }

    public void EhView_ColorBelowChanged(System.Drawing.Color color)
    {
      m_ColorBelow = color;
    }

    public void EhView_ColorAboveChanged(System.Drawing.Color color)
    {
      m_ColorAbove = color;
    }

    public void EhView_ColorInvalidChanged(System.Drawing.Color color)
    {
      m_ColorInvalid = color;
    }
  
    public void EhView_ClipToLayerChanged(bool bClip)
    {
      m_ClipToLayer = bClip;
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
    
      m_PlotStyle.Scaling = m_ScalingStyle;
      m_PlotStyle.RangeFrom    = m_RangeFrom;
      m_PlotStyle.RangeTo      = m_RangeTo;
      m_PlotStyle.ColorBelow   = m_ColorBelow;
      m_PlotStyle.ColorAbove   = m_ColorAbove;
      m_PlotStyle.ColorInvalid = m_ColorInvalid;
      m_PlotStyle.ClipToLayer  = m_ClipToLayer;

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
        View = value as IDensityImagePlotStyleView;
      }
    }

    public object ModelObject
    {
      get { return this.m_PlotStyle; }
    }

    #endregion
  }
}
