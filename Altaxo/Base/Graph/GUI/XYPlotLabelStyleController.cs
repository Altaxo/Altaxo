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
using Altaxo.Serialization;
using System.Drawing;

namespace Altaxo.Graph.GUI
{
  #region Interfaces
  public interface IXYPlotLabelStyleController : Main.GUI.IApplyController, Main.GUI.IMVCController
  {
    /// <summary>
    /// Get/sets the view this controller controls.
    /// </summary>
    IXYPlotLabelStyleView View { get; set; }

    /// <summary>
    /// Called if the font family is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_FontChanged(string newValue);

    /// <summary>
    /// Called if the color is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_ColorChanged(Color newValue);

    /// <summary>
    /// Called if the background color is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_BackgroundColorChanged(Color newValue);

    /// <summary>
    /// Called if the font size is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_FontSizeChanged(string newValue);

    /// <summary>
    /// Called if the horizontal aligment is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_HorizontalAlignmentChanged(string newValue);

    /// <summary>
    /// Called if the vertical alignment is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_VerticalAlignmentChanged(string newValue);

    /// <summary>
    /// Called if the AttachToAxis box check value has changed.
    /// </summary>
    /// <param name="newValue"></param>
    void EhView_AttachToAxisChanged(bool newValue);

    /// <summary>
    /// Called if the attached axis selection is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_AttachedAxisChanged(string newValue);

    /// <summary>
    /// Called if the WhiteOut box check value has changed.
    /// </summary>
    /// <param name="newValue"></param>
    void EhView_WhiteOutChanged(bool newValue);

    /// <summary>
    /// Called if the Independent color box check value has changed.
    /// </summary>
    /// <param name="newValue"></param>
    void EhView_IndependentColorChanged(bool newValue);

    /// <summary>
    /// Called when the contents of XOffset is changed.
    /// </summary>
    /// <param name="newValue">Contents of the edit field.</param>
    /// <param name="bCancel">Normally false, this can be set to true if RangeFrom is not a valid entry.</param>
    void EhView_XOffsetValidating(string newValue, ref bool bCancel);
    
    /// <summary>
    /// Called when the contents of YOffset is changed.
    /// </summary>
    /// <param name="newValue">Contents of the edit field.</param>
    /// <param name="bCancel">Normally false, this can be set to true if RangeFrom is not a valid entry.</param>
    void EhView_YOffsetValidating(string newValue, ref bool bCancel);

    /// <summary>
    /// Called when the contents of Rotation is changed.
    /// </summary>
    /// <param name="newValue">Contents of the edit field.</param>
    /// <param name="bCancel">Normally false, this can be set to true if RangeFrom is not a valid entry.</param>
    void EhView_RotationValidating(string newValue, ref bool bCancel);
  }

  public interface IXYPlotLabelStyleView : Main.GUI.IMVCView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IXYPlotLabelStyleController Controller { get; set; }

    /// <summary>
    /// Gets the hosting parent form of this view.
    /// </summary>
    System.Windows.Forms.Form Form  { get; }

    /// <summary>
    /// Initializes the font family combo box.
    /// </summary>
    /// <param name="name">The actual name of the choice.</param>
    void Font_Initialize(string name);

    /// <summary>
    /// Initializes the content of the Color combo box.
    /// </summary>
    void Color_Initialize(System.Drawing.Color color);

    /// <summary>
    /// Initializes the content of the background color combo box.
    /// </summary>
    void BackgroundColor_Initialize(System.Drawing.Color color);

    /// <summary>
    /// Initializes the font size combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="name">The actual name of the choice.</param>
    void FontSize_Initialize(string[] names, string name);

    /// <summary>
    /// Initializes the horizontal aligment combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="name">The actual name of the choice.</param>
    void HorizontalAlignment_Initialize(string[] names, string name);
    /// <summary>
    /// Initializes the vertical alignement combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="name">The actual name of the choice.</param>
    void VerticalAlignment_Initialize(string[] names, string name);

    /// <summary>
    /// Initializes the content of the AttachToAxis checkbox
    /// </summary>
    /// <param name="bAttached">True if the label is attached to one of the four axes.</param>
    void AttachToAxis_Initialize(bool bAttached);

    /// <summary>
    /// Initializes the AttachedAxis combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="name">The actual name of the choice.</param>
    void AttachedAxis_Initialize(string[] names, string name);


    /// <summary>
    /// Initializes the content of the Rotation edit box.
    /// </summary>
    void Rotation_Initialize(string text);


    /// <summary>
    /// Initializes the content of the XOffset edit box.
    /// </summary>
    void XOffset_Initialize(string text);

    /// <summary>
    /// Initializes the content of the YOffset edit box.
    /// </summary>
    void YOffset_Initialize(string text);

    /// <summary>
    /// Initializes the content of the WhiteOut checkbox
    /// </summary>
    /// <param name="bWhiteOut">True if the label has a white background.</param>
    void WhiteOut_Initialize(bool bWhiteOut);

    /// <summary>
    /// Initializes the content of the Independent color checkbox
    /// </summary>
    /// <param name="bIndependent">True if the label has a white background.</param>
    void IndependentColor_Initialize(bool bIndependent);
  
  }
  #endregion

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
  public class XYPlotLabelStyleController : IXYPlotLabelStyleController
  {
    IXYPlotLabelStyleView m_View;
    XYPlotLabelStyle m_PlotStyle;

    /// <summary>The font of the label.</summary>
    protected string m_FontFamily;

    /// <summary>
    /// True if the color is independent of the parent plot style.
    /// </summary>
    protected bool m_IndependentColor;

    /// <summary>The color for the label.</summary>
    protected Color  m_Color;

    /// <summary>The color for the label.</summary>
    protected Color  m_BackgroundColor;
  
    /// <summary>The size of the font.</summary>
    protected float m_FontSize;

    protected System.Drawing.StringAlignment m_HorizontalAlignment;

    protected System.Drawing.StringAlignment m_VerticalAlignment;


    /// <summary>If true, the label is attached to one of the four edges of the layer.</summary>
    protected bool m_AttachToAxis;

    /// <summary>The axis where the label is attached to (if it is attached).</summary>
    protected Graph.EdgeType m_AttachedAxis;

    /// <summary>If true, the label is painted on a white background.</summary>
    protected bool m_WhiteOut;

    /// <summary>The x offset in EM units.</summary>
    protected double m_XOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double m_YOffset;

    /// <summary>The rotation of the label.</summary>
    protected double m_Rotation;

    public XYPlotLabelStyleController(XYPlotLabelStyle plotStyle)
    {
      m_PlotStyle = plotStyle;
      SetElements(true);
    }


    void SetElements(bool bInit)
    {
      if(bInit)
      {
        m_FontFamily  = m_PlotStyle.Font.FontFamily.Name;
        m_IndependentColor = m_PlotStyle.IndependentColor;
        m_Color = m_PlotStyle.Color;
        m_BackgroundColor = m_PlotStyle.BackgroundColor;
        m_FontSize = m_PlotStyle.FontSize;
        m_HorizontalAlignment = m_PlotStyle.HorizontalAlignment;
        m_VerticalAlignment = m_PlotStyle.VerticalAlignment;
        m_AttachToAxis = m_PlotStyle.AttachToAxis;
        m_AttachedAxis = m_PlotStyle.AttachedAxis;
        m_WhiteOut     = m_PlotStyle.WhiteOut;
        m_Rotation     = m_PlotStyle.Rotation;
        m_XOffset      = m_PlotStyle.XOffset;
        m_YOffset      = m_PlotStyle.YOffset;
      }

      if(null!=View)
      {
        View.Font_Initialize(m_FontFamily);
        View.IndependentColor_Initialize(m_IndependentColor);
        View.Color_Initialize(m_Color);
        View.BackgroundColor_Initialize(m_BackgroundColor);
        View.FontSize_Initialize(new string[]{"6","8","10","12","16","24","32","48","72"},Serialization.NumberConversion.ToString(m_FontSize));
        View.HorizontalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),m_HorizontalAlignment));
        View.VerticalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),m_VerticalAlignment));
        View.AttachToAxis_Initialize(m_AttachToAxis);
        View.AttachedAxis_Initialize(System.Enum.GetNames(typeof(EdgeType)),System.Enum.GetName(typeof(EdgeType),m_AttachedAxis));
        View.WhiteOut_Initialize(m_WhiteOut);
        View.Rotation_Initialize(Serialization.NumberConversion.ToString(m_Rotation));
        View.XOffset_Initialize(Serialization.NumberConversion.ToString(m_XOffset*100));
        View.YOffset_Initialize(Serialization.NumberConversion.ToString(m_XOffset*100));
      }
    }
    #region IXYPlotLabelStyleController Members

    public IXYPlotLabelStyleView View
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

    public void EhView_FontChanged(string newValue)
    {
      m_FontFamily = newValue;
    }

    public void EhView_ColorChanged(System.Drawing.Color color)
    {
      m_Color = color;
    }

    public void EhView_BackgroundColorChanged(System.Drawing.Color color)
    {
      m_BackgroundColor = color;
    }

    public void EhView_FontSizeChanged(string newValue)
    {
      double numValue;
      if(NumberConversion.IsDouble(newValue, out numValue))
        m_FontSize = (float)numValue;
    }

    public void EhView_HorizontalAlignmentChanged(string newValue)
    {
      m_HorizontalAlignment = (StringAlignment)System.Enum.Parse(typeof(StringAlignment),newValue);
    }

    public void EhView_VerticalAlignmentChanged(string newValue)
    {
      m_VerticalAlignment = (StringAlignment)System.Enum.Parse(typeof(StringAlignment),newValue);
    }

    public void EhView_AttachToAxisChanged(bool newValue)
    {
      m_AttachToAxis = newValue;
    }

    public void EhView_AttachedAxisChanged(string newValue)
    {
      m_AttachedAxis = (EdgeType)System.Enum.Parse(typeof(EdgeType),newValue);
    }

    public void EhView_WhiteOutChanged(bool newValue)
    {
      m_WhiteOut = newValue;
    }

    public void EhView_IndependentColorChanged(bool newValue)
    {
      m_IndependentColor = newValue;
    }

    public void EhView_RotationValidating(string newValue, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(newValue, out m_Rotation))
        bCancel = true;
    }

    public void EhView_XOffsetValidating(string newValue, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(newValue, out m_XOffset))
        bCancel = true;
      else
        m_XOffset/=100;
    }

    public void EhView_YOffsetValidating(string newValue, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(newValue, out m_YOffset))
        bCancel = true;
      else
        m_YOffset/=100;
    }

  
    #endregion

    #region IApplyController Members

    public bool Apply()
    {
    
      m_PlotStyle.Font = new Font(m_FontFamily,m_FontSize,GraphicsUnit.World);
      m_PlotStyle.IndependentColor = m_IndependentColor;
      m_PlotStyle.Color = m_Color;
      m_PlotStyle.BackgroundColor = m_BackgroundColor;
      m_PlotStyle.HorizontalAlignment = m_HorizontalAlignment;
      m_PlotStyle.VerticalAlignment   = m_VerticalAlignment;
      m_PlotStyle.AttachToAxis = m_AttachToAxis;
      m_PlotStyle.AttachedAxis = m_AttachedAxis;
      m_PlotStyle.WhiteOut     = m_WhiteOut;
      m_PlotStyle.Rotation     = m_Rotation;
      m_PlotStyle.XOffset      = m_XOffset;
      m_PlotStyle.YOffset      = m_YOffset;

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
        View = value as IXYPlotLabelStyleView;
      }
    }

    public object ModelObject
    {
      get { return this.m_PlotStyle; }
    }

    #endregion
  }
}
