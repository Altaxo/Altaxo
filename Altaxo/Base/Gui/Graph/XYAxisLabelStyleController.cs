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
using Altaxo.Graph.GUI;
using Altaxo.Main.GUI;
using Altaxo.Data;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IXYAxisLabelStyleViewEventSink
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
    /// Called if the background style changed.
    /// </summary>
    /// <param name="newValue">The new index of the style.</param>
    void EhView_BackgroundStyleChanged(int newValue);

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


    /// <summary>
    /// Called if the label style was changed by the user.
    /// </summary>
    /// <param name="newValue">The new selected item (index) of the combo box.</param>
    void EhView_LabelStyleChanged(int newValue);


    /// <summary>
    /// Called if the automatic alignment was changed by the user.
    /// </summary>
    /// <param name="newValue">The new selected value of AutomaticAlignment.</param>
    void EhView_AutomaticAlignmentChanged(bool newValue);
  }

  public interface IXYAxisLabelStyleView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IXYAxisLabelStyleViewEventSink Controller { get; set; }
    
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
    /// Initializes the enable state of the background color combo box.
    /// </summary>
    void BackgroundColorEnable_Initialize(bool enable);

    /// <summary>
    /// Initializes the background styles.
    /// </summary>
    /// <param name="names"></param>
    /// <param name="selection"></param>
    void BackgroundStyle_Initialize(string[] names, int selection);

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
    /// Sets the automatic alignment check box.
    /// </summary>
    /// <param name="value"></param>
    void AutomaticAlignment_Initialize(bool value);
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
    /// Initializes the label style combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="name">The actual name of the choice.</param>
    void LabelStyle_Initialize(string[] names, string name);

  }

  public interface IXYAxisLabelStyleController : Main.GUI.IMVCAController
  {
  }

  #endregion

  /// <summary>
  /// Summary description.
  /// </summary>
  [UserControllerForObject(typeof(XYAxisLabelStyle))]
  public class XYAxisLabelStyleController : IXYAxisLabelStyleViewEventSink, IXYAxisLabelStyleController
  {
    IXYAxisLabelStyleView _view;
    XYAxisLabelStyle _doc;

    /// <summary>The font of the label.</summary>
    protected string _fontFamily;

    /// <summary>The color for the label.</summary>
    protected Color  _color;
  
    /// <summary>The size of the font.</summary>
    protected float _fontSize;

    protected System.Drawing.StringAlignment _horizontalAlignment;

    protected System.Drawing.StringAlignment _verticalAlignment;

    protected bool _automaticAlignment;

    
    /// <summary>The x offset in EM units.</summary>
    protected double _xOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double _yOffset;

    /// <summary>The rotation of the label.</summary>
    protected double _rotation;

    protected System.Type[] _labelTypes;

    protected int           _currentLabelStyle;

    protected Altaxo.Graph.LabelFormatting.ILabelFormatting _currentLabelStyleInstance;

    protected Altaxo.Graph.BackgroundStyles.IBackgroundStyle _currentBackgroundStyleInstance;
    
    protected System.Type[] _backgroundStyles;

    public XYAxisLabelStyleController(XYAxisLabelStyle style)
    {
      _doc = style;
      Initialize(true);
    }


    void Initialize(bool bInit)
    {
      if(bInit)
      {
        _fontFamily  = _doc.Font.FontFamily.Name;
        _color = _doc.Color;
        _backgroundStyles = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Graph.BackgroundStyles.IBackgroundStyle));
        _currentBackgroundStyleInstance = _doc.BackgroundStyle;

        _fontSize = _doc.FontSize;
        _horizontalAlignment = _doc.HorizontalAlignment;
        _verticalAlignment = _doc.VerticalAlignment;
        _automaticAlignment = _doc.AutomaticAlignment;
        _rotation     = _doc.Rotation;
        _xOffset      = _doc.XOffset;
        _yOffset      = _doc.YOffset;
        _currentLabelStyleInstance = _doc.LabelFormat;
      }

      if(null!=View)
      {
        View.Font_Initialize(_fontFamily);
      
        View.Color_Initialize(_color);
        View.FontSize_Initialize(new string[]{"6","8","10","12","16","24","32","48","72"},Serialization.NumberConversion.ToString(_fontSize));
        View.HorizontalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),_horizontalAlignment));
        View.VerticalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),_verticalAlignment));
        View.AutomaticAlignment_Initialize(this._automaticAlignment);
        View.Rotation_Initialize(Serialization.NumberConversion.ToString(_rotation));
        View.XOffset_Initialize(Serialization.NumberConversion.ToString(_xOffset*100));
        View.YOffset_Initialize(Serialization.NumberConversion.ToString(_yOffset*100));
        InitializeBackgroundStyle();
        InitializeLabelStyle();
      }
    }

    void InitializeBackgroundStyle()
    {
      int sel = Array.IndexOf(this._backgroundStyles,this._currentBackgroundStyleInstance==null ? null : this._currentBackgroundStyleInstance.GetType());
      View.BackgroundStyle_Initialize(Current.Gui.GetUserFriendlyClassName(this._backgroundStyles,true),sel+1);

      if(this._currentBackgroundStyleInstance!=null && this._currentBackgroundStyleInstance.SupportsColor)
        View.BackgroundColor_Initialize(this._currentBackgroundStyleInstance.Color);
      else
        View.BackgroundColor_Initialize(Color.Transparent);

      View.BackgroundColorEnable_Initialize(this._currentBackgroundStyleInstance!=null && this._currentBackgroundStyleInstance.SupportsColor);

    }

    void InitializeLabelStyle()
    {
      _labelTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Graph.LabelFormatting.ILabelFormatting));

      _currentLabelStyleInstance = _doc.LabelFormat;

      string[] names = new string[_labelTypes.Length];

      for(int i=0;i<_labelTypes.Length;++i)
      {
        names[i] = _labelTypes[i].Name;
        if(_labelTypes[i]==_currentLabelStyleInstance.GetType())
          this._currentLabelStyle = i;
      }

      View.LabelStyle_Initialize(names,_currentLabelStyleInstance.GetType().Name);
    }
  
    #region IXYAxisLabelStyleController Members

    public IXYAxisLabelStyleView View
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
      if(this._currentBackgroundStyleInstance!=null && this._currentBackgroundStyleInstance.SupportsColor)
        this._currentBackgroundStyleInstance.Color = color;
    }

    public void EhView_FontSizeChanged(string newValue)
    {
      double numValue;
      if(NumberConversion.IsDouble(newValue, out numValue))
        _fontSize = (float)numValue;
    }

    public void EhView_LabelStyleChanged(int newValue)
    {
      _currentLabelStyle = newValue;
      _currentLabelStyleInstance = (Altaxo.Graph.LabelFormatting.ILabelFormatting)Activator.CreateInstance(this._labelTypes[newValue]);

    }

    /// <summary>
    /// Called if the background style changed.
    /// </summary>
    /// <param name="newValue">The new index of the style.</param>
    public void EhView_BackgroundStyleChanged(int newValue)
    {
     
      Color backgroundColor = Color.Transparent;

      if(newValue!=0)
      {
        _currentBackgroundStyleInstance = (Altaxo.Graph.BackgroundStyles.IBackgroundStyle)Activator.CreateInstance(this._backgroundStyles[newValue-1]);
        backgroundColor = _currentBackgroundStyleInstance.Color;
      }
      else // is null
      {
        _currentBackgroundStyleInstance = null;
      }

      if(_currentBackgroundStyleInstance!=null && _currentBackgroundStyleInstance.SupportsColor)
      {
        View.BackgroundColor_Initialize(backgroundColor);
        View.BackgroundColorEnable_Initialize(true);
      }
      else
      {
        View.BackgroundColorEnable_Initialize(false);
      }
    }

    public void EhView_HorizontalAlignmentChanged(string newValue)
    {
      _horizontalAlignment = (StringAlignment)System.Enum.Parse(typeof(StringAlignment),newValue);
    }

    public void EhView_VerticalAlignmentChanged(string newValue)
    {
      _verticalAlignment = (StringAlignment)System.Enum.Parse(typeof(StringAlignment),newValue);
    }

    public void EhView_AutomaticAlignmentChanged(bool newValue)
    {
      _automaticAlignment = newValue;
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
      
      _doc.Color = _color;

      _doc.BackgroundStyle = this._currentBackgroundStyleInstance;
      _doc.HorizontalAlignment = _horizontalAlignment;
      _doc.VerticalAlignment   = _verticalAlignment;
      _doc.AutomaticAlignment = _automaticAlignment;
     
      _doc.Rotation     = _rotation;
      _doc.XOffset      = _xOffset;
      _doc.YOffset      = _yOffset;
     
      _doc.LabelFormat = this._currentLabelStyleInstance;

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
        View = value as IXYAxisLabelStyleView;
      }
    }

    public object ModelObject
    {
      get { return this._doc; }
    }

    #endregion
  }
}
