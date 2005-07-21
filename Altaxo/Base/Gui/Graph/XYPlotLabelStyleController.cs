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
using Altaxo.Graph;
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IXYPlotLabelStyleViewEventSink
  {
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

  public interface IXYPlotLabelStyleView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IXYPlotLabelStyleViewEventSink Controller { get; set; }

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

  public interface IXYPlotLabelStyleController : Main.GUI.IMVCAController
  {
  }

  #endregion

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
  [UserControllerForObject(typeof(XYPlotLabelStyle))]
  public class XYPlotLabelStyleController : IXYPlotLabelStyleViewEventSink, IXYPlotLabelStyleController
  {
    IXYPlotLabelStyleView _view;
    XYPlotLabelStyle _doc;

    /// <summary>The font of the label.</summary>
    protected string _fontFamily;

    /// <summary>
    /// True if the color is independent of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    /// <summary>The color for the label.</summary>
    protected Color  _color;

    /// <summary>The color for the label.</summary>
    protected Color  _backgroundColor;
  
    /// <summary>The size of the font.</summary>
    protected float _fontSize;

    protected System.Drawing.StringAlignment _horizontalAlignment;

    protected System.Drawing.StringAlignment _verticalAlignment;


    /// <summary>If true, the label is attached to one of the four edges of the layer.</summary>
    protected bool _attachToEdge;

    /// <summary>The axis where the label is attached to (if it is attached).</summary>
    protected EdgeType _attachedEdge;

    /// <summary>If true, the label is painted on a white background.</summary>
    protected bool _whiteOut;

    /// <summary>The x offset in EM units.</summary>
    protected double _xOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double _yOffset;

    /// <summary>The rotation of the label.</summary>
    protected double _rotation;

    public XYPlotLabelStyleController(XYPlotLabelStyle plotStyle)
    {
      _doc = plotStyle;
      Initialize(true);
    }


    void Initialize(bool bInit)
    {
      if(bInit)
      {
        _fontFamily  = _doc.Font.FontFamily.Name;
        _independentColor = _doc.IndependentColor;
        _color = _doc.Color;
        _backgroundColor = _doc.BackgroundColor;
        _fontSize = _doc.FontSize;
        _horizontalAlignment = _doc.HorizontalAlignment;
        _verticalAlignment = _doc.VerticalAlignment;
        _attachToEdge = _doc.AttachToAxis;
        _attachedEdge = _doc.AttachedAxis;
        _whiteOut     = _doc.WhiteOut;
        _rotation     = _doc.Rotation;
        _xOffset      = _doc.XOffset;
        _yOffset      = _doc.YOffset;
      }

      if(null!=View)
      {
        View.Font_Initialize(_fontFamily);
        View.IndependentColor_Initialize(_independentColor);
        View.Color_Initialize(_color);
        View.BackgroundColor_Initialize(_backgroundColor);
        View.FontSize_Initialize(new string[]{"6","8","10","12","16","24","32","48","72"},Serialization.NumberConversion.ToString(_fontSize));
        View.HorizontalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),_horizontalAlignment));
        View.VerticalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),_verticalAlignment));
        View.AttachToAxis_Initialize(_attachToEdge);
        View.AttachedAxis_Initialize(System.Enum.GetNames(typeof(EdgeType)),System.Enum.GetName(typeof(EdgeType),_attachedEdge));
        View.WhiteOut_Initialize(_whiteOut);
        View.Rotation_Initialize(Serialization.NumberConversion.ToString(_rotation));
        View.XOffset_Initialize(Serialization.NumberConversion.ToString(_xOffset*100));
        View.YOffset_Initialize(Serialization.NumberConversion.ToString(_yOffset*100));
      }
    }
    #region IXYPlotLabelStyleController Members

    public IXYPlotLabelStyleView View
    {
      get
      {
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value;
        
        Initialize(false);

        if(_view!=null)
          _view.Controller = this;
        
      }
    }

    public void EhView_FontChanged(string newValue)
    {
      _fontFamily = newValue;
    }

    public void EhView_ColorChanged(System.Drawing.Color color)
    {
      _color = color;
    }

    public void EhView_BackgroundColorChanged(System.Drawing.Color color)
    {
      _backgroundColor = color;
    }

    public void EhView_FontSizeChanged(string newValue)
    {
      double numValue;
      if(NumberConversion.IsDouble(newValue, out numValue))
        _fontSize = (float)numValue;
    }

    public void EhView_HorizontalAlignmentChanged(string newValue)
    {
      _horizontalAlignment = (StringAlignment)System.Enum.Parse(typeof(StringAlignment),newValue);
    }

    public void EhView_VerticalAlignmentChanged(string newValue)
    {
      _verticalAlignment = (StringAlignment)System.Enum.Parse(typeof(StringAlignment),newValue);
    }

    public void EhView_AttachToAxisChanged(bool newValue)
    {
      _attachToEdge = newValue;
    }

    public void EhView_AttachedAxisChanged(string newValue)
    {
      _attachedEdge = (EdgeType)System.Enum.Parse(typeof(EdgeType),newValue);
    }

    public void EhView_WhiteOutChanged(bool newValue)
    {
      _whiteOut = newValue;
    }

    public void EhView_IndependentColorChanged(bool newValue)
    {
      _independentColor = newValue;
    }

    public void EhView_RotationValidating(string newValue, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(newValue, out _rotation))
        bCancel = true;
    }

    public void EhView_XOffsetValidating(string newValue, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(newValue, out _xOffset))
        bCancel = true;
      else
        _xOffset/=100;
    }

    public void EhView_YOffsetValidating(string newValue, ref bool bCancel)
    {
      if(!NumberConversion.IsDouble(newValue, out _yOffset))
        bCancel = true;
      else
        _yOffset/=100;
    }

  
    #endregion

    #region IApplyController Members

    public bool Apply()
    {
    
      _doc.Font = new Font(_fontFamily,_fontSize,GraphicsUnit.World);
      _doc.IndependentColor = _independentColor;
      _doc.Color = _color;
      _doc.BackgroundColor = _backgroundColor;
      _doc.HorizontalAlignment = _horizontalAlignment;
      _doc.VerticalAlignment   = _verticalAlignment;
      _doc.AttachToAxis = _attachToEdge;
      _doc.AttachedAxis = _attachedEdge;
      _doc.WhiteOut     = _whiteOut;
      _doc.Rotation     = _rotation;
      _doc.XOffset      = _xOffset;
      _doc.YOffset      = _yOffset;

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
      get { return this._doc; }
    }

    #endregion
  }
}
