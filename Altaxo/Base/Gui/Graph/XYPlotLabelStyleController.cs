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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Serialization;
using System.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.GUI;

using Altaxo.Data;

using Altaxo.Graph;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IXYPlotLabelStyleViewEventSink
  {
    /// <summary>
    /// Called if the font family is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_FontChanged(Font newValue);

    /// <summary>
    /// Called if the color is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_ColorChanged(Color newValue);

   
    

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
    void EhView_AttachedAxisChanged(ListNode newValue);

  

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
    /// Is called when the user wants to select a new label column.
    /// </summary>
    void EhView_SelectLabelColumn();
  }

  public interface IXYPlotLabelStyleView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IXYPlotLabelStyleViewEventSink Controller { get; set; }


    /// <summary>
    /// Initializes the name of the label column.
    /// </summary>
    /// <param name="labelColumnAsText">Label column's name.</param>
    void LabelColumn_Initialize(string labelColumnAsText);
    
    /// <summary>
    /// Initializes the font family combo box.
    /// </summary>
    /// <param name="font">The actual font of the choice.</param>
    void Font_Initialize(Font font);

    /// <summary>
    /// Initializes the content of the Color combo box.
    /// </summary>
    void Color_Initialize(System.Drawing.Color color);

    /// <summary>
    /// Initializes the background.
    /// </summary>
    IBackgroundStyle Background { get; set; }
  

    

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
    /// <param name="sel">The actual choice.</param>
    void AttachedAxis_Initialize(List<ListNode> names, int sel);


    /// <summary>
    /// Initializes the content of the Rotation edit box.
    /// </summary>
    float Rotation{get; set;}


    /// <summary>
    /// Initializes the content of the XOffset edit box.
    /// </summary>
    void XOffset_Initialize(string text);

    /// <summary>
    /// Initializes the content of the YOffset edit box.
    /// </summary>
    void YOffset_Initialize(string text);

   

    /// <summary>
    /// Initializes the content of the Independent color checkbox
    /// </summary>
    /// <param name="bIndependent">True if the label has a white background.</param>
    void IndependentColor_Initialize(bool bIndependent);
  }

  public interface IXYPlotLabelStyleController : IMVCANController
  {
  }

  #endregion

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
  [UserControllerForObject(typeof(LabelPlotStyle))]
  [ExpectedTypeOfView(typeof(IXYPlotLabelStyleView))]
  public class XYPlotLabelStyleController : IXYPlotLabelStyleViewEventSink, IXYPlotLabelStyleController
  {
    IXYPlotLabelStyleView _view;
    LabelPlotStyle _doc;

    /// <summary>The font of the label.</summary>
    protected Font _font;

    /// <summary>
    /// True if the color is independent of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    /// <summary>The color for the label.</summary>
    protected Color  _color;
  
   
    protected System.Drawing.StringAlignment _horizontalAlignment;

    protected System.Drawing.StringAlignment _verticalAlignment;


    /// <summary>If true, the label is attached to one of the four edges of the layer.</summary>
    protected bool _attachToEdge;

    /// <summary>The axis where the label is attached to (if it is attached).</summary>
    protected CSPlaneID _attachedEdge;

   

    /// <summary>The x offset in EM units.</summary>
    protected double _xOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double _yOffset;

   
    protected IReadableColumn _labelColumn;

    protected IBackgroundStyle _backgroundStyle;

    UseDocument _useDocumentCopy;

    public XYPlotLabelStyleController()
    {
    }
    public XYPlotLabelStyleController(LabelPlotStyle doc)
    {
      if (doc == null)
        throw new ArgumentNullException("doc is null");

      if (!InitializeDocument(doc))
        throw new ApplicationException("Programming error");
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is LabelPlotStyle))
        return false;

      bool isFirstTime = (null == _doc);
      _doc = (LabelPlotStyle)args[0];
     // _tempDoc = _useDocumentCopy == UseDocument.Directly ? _doc : (LabelPlotStyle)_doc.Clone();
      Initialize(true); // initialize always because we have to update the temporary variables
      return true;
    }

    public UseDocument UseDocumentCopy { set { _useDocumentCopy = value; } } // not used here


    void Initialize(bool bInit)
    {
      if(bInit)
      {
        _font = _doc.Font;
        _independentColor = _doc.IndependentColor;
        _color = _doc.Color;
        
        _horizontalAlignment = _doc.HorizontalAlignment;
        _verticalAlignment = _doc.VerticalAlignment;
        _attachToEdge = _doc.AttachedAxis!=null;
        _attachedEdge = _doc.AttachedAxis;
        _xOffset      = _doc.XOffset;
        _yOffset      = _doc.YOffset;
        _labelColumn = _doc.LabelColumn;
        _backgroundStyle = _doc.BackgroundStyle;
      }

      if(null!=View)
      {
        View.Font_Initialize(_font);
        View.IndependentColor_Initialize(_independentColor);
        View.Color_Initialize(_color);
        View.HorizontalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),_horizontalAlignment));
        View.VerticalAlignment_Initialize(System.Enum.GetNames(typeof(System.Drawing.StringAlignment)),System.Enum.GetName(typeof(System.Drawing.StringAlignment),_verticalAlignment));
        View.AttachToAxis_Initialize(_attachToEdge);
        SetAttachmentDirection();
        View.Rotation = (float)_doc.Rotation;
        View.XOffset_Initialize(Serialization.NumberConversion.ToString(_xOffset*100));
        View.YOffset_Initialize(Serialization.NumberConversion.ToString(_yOffset*100));
        View.Background = _backgroundStyle;

        InitializeLabelColumnText();
      }
    }


    public void SetAttachmentDirection()
    {
      IPlotArea layer = Main.DocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;

      List<ListNode> names = new List<ListNode>();

      int idx = -1;
      if (layer != null)
      {
        int count = -1;
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _doc.AttachedAxis }))
        {
          count++;
          if (id == _doc.AttachedAxis)
            idx = count;

          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          names.Add(new ListNode(info.Name, id));
        }
      }

      _view.AttachedAxis_Initialize(names, Math.Max(idx, 0)); 
    }


    void InitializeLabelColumnText()
    {
      if(View!=null)
      {
        string name = _labelColumn==null ? string.Empty : _labelColumn.FullName;
        View.LabelColumn_Initialize(name);
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

    public void EhView_FontChanged(Font newValue)
    {
      _font = newValue;
    }

    public void EhView_ColorChanged(System.Drawing.Color color)
    {
      this._color = color;
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

    public void EhView_AttachedAxisChanged(ListNode newValue)
    {
      _attachedEdge = ((CSPlaneID)newValue.Item);
    }

    public void EhView_IndependentColorChanged(bool newValue)
    {
      _independentColor = newValue;
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

    public void EhView_SelectLabelColumn()
    {
      SingleColumnChoice choice = new SingleColumnChoice();
      choice.SelectedColumn = _doc.LabelColumn as DataColumn;
      object choiceAsObject = choice;
      if(Current.Gui.ShowDialog(ref choiceAsObject,"Select label column"))
      {
        choice = (SingleColumnChoice)choiceAsObject;

       
        _labelColumn = choice.SelectedColumn;
        InitializeLabelColumnText();
        
      }
    }
  
    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc.BackgroundStyle = _view.Background;
      _doc.Font = new Font(_font.FontFamily,_font.Size,_font.Style,GraphicsUnit.World);
      _doc.IndependentColor = _independentColor;
      _doc.Color = _color;
      _doc.HorizontalAlignment = _horizontalAlignment;
      _doc.VerticalAlignment   = _verticalAlignment;

      if (_attachToEdge)
        _doc.AttachedAxis = _attachedEdge;
      else
        _doc.AttachedAxis = null;

      _doc.Rotation = _view.Rotation;
      _doc.XOffset      = _xOffset;
      _doc.YOffset      = _yOffset;
      _doc.LabelColumn  = _labelColumn;

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
