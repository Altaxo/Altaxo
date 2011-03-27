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

using Altaxo.Data;
using Altaxo.Science;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Gui.Graph
{
  #region Interfaces

  public interface IXYPlotLabelStyleView
  {
		/// <summary>Occurs when the select label column button was pressed.</summary>
		event Action LabelColumnSelected;

		/// <summary>Occurs when the font size changed</summary>
		event Action FontSizeChanged;

    /// <summary>
    /// Initializes the name of the label column.
    /// </summary>
    /// <param name="labelColumnAsText">Label column's name.</param>
    void Init_LabelColumn(string labelColumnAsText);
    
    /// <summary>
    /// Initializes/gets the font family combo box.
    /// </summary>
    /// <param name="font">The actual font of the choice.</param>
		Font SelectedFont { get; set; }

    /// <summary>
    /// Initializes/gets the content of the Color combo box.
    /// </summary>
		System.Drawing.Color SelectedColor { get; set; }

    /// <summary>
    /// Initializes/gets the background.
    /// </summary>
    IBackgroundStyle Background { get; set; }
  

    

    /// <summary>
    /// Initializes the horizontal aligment combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="name">The actual name of the choice.</param>
    void Init_HorizontalAlignment(SelectableListNodeList list);

		ListNode SelectedHorizontalAlignment { get; }

    /// <summary>
    /// Initializes the vertical alignement combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="name">The actual name of the choice.</param>
    void Init_VerticalAlignment(SelectableListNodeList list);

		ListNode SelectedVerticalAlignment { get; }


    /// <summary>
		/// Initializes the content of the AttachToAxis checkbox. True if the label is attached to one of the four axes.
    /// </summary>
		bool AttachToAxis { get; set; }


    /// <summary>
    /// Initializes the AttachedAxis combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    /// <param name="sel">The actual choice.</param>
    void Init_AttachedAxis(SelectableListNodeList names);

		ListNode AttachedAxis { get; }

    /// <summary>
    /// Initializes the content of the Rotation edit box.
    /// </summary>
    double SelectedRotation{get; set;}


    /// <summary>
    /// Initializes the content of the XOffset edit box.
    /// </summary>
    void Init_XOffset(QuantityWithUnitGuiEnvironment environment, QuantityWithUnit value);

		QuantityWithUnit XOffset { get; }


    /// <summary>
    /// Initializes the content of the YOffset edit box.
    /// </summary>
		void Init_YOffset(QuantityWithUnitGuiEnvironment environment, QuantityWithUnit value);


		QuantityWithUnit YOffset { get; }

    /// <summary>
    /// Initializes the content of the Independent color checkbox
    /// </summary>
    /// <param name="bIndependent">True if the label has a white background.</param>
		bool IsIndependentColorSelected { get; set; }
  }

  #endregion

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
  [UserControllerForObject(typeof(LabelPlotStyle))]
  [ExpectedTypeOfView(typeof(IXYPlotLabelStyleView))]
	public class XYPlotLabelStyleController : IMVCANController
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

		ChangeableRelativePercentUnit _percentFontSizeUnit = new ChangeableRelativePercentUnit("%Em font size", new QuantityWithUnit(1, LengthUnitPoint.Instance));

    public XYPlotLabelStyleController()
    {
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is LabelPlotStyle))
        return false;

			// if a view is momentarily coupled, deactivate it to avoid a lot of cascading updates
			var tempView = _view;
			this.ViewObject = null;

      _doc = (LabelPlotStyle)args[0];
      Initialize(true); // initialize always because we have to update the temporary variables

			this.ViewObject = tempView; // reactivate the view

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
				_percentFontSizeUnit = new ChangeableRelativePercentUnit("%Em size", new QuantityWithUnit(_font.Size, LengthUnitPoint.Instance));
			}

      if(null!=_view)
      {
				_view.SelectedFont = _font;
				_view.IsIndependentColorSelected = _independentColor;
				_view.SelectedColor = _color;
				_view.Init_HorizontalAlignment(new SelectableListNodeList(_horizontalAlignment));
				_view.Init_VerticalAlignment(new SelectableListNodeList(_verticalAlignment));
				_view.AttachToAxis = _attachToEdge;
        SetAttachmentDirection();
				_view.SelectedRotation = _doc.Rotation;

				_percentFontSizeUnit.ReferenceQuantity = new QuantityWithUnit(_font.Size, LengthUnitPoint.Instance);

				var xEnv = new QuantityWithUnitGuiEnvironment( GuiLengthUnits.Collection, _percentFontSizeUnit);
				_view.Init_XOffset(xEnv, new QuantityWithUnit(_xOffset * 100, _percentFontSizeUnit));
				_view.Init_YOffset(xEnv, new QuantityWithUnit(_yOffset * 100, _percentFontSizeUnit));
				_view.Background = _backgroundStyle;

        InitializeLabelColumnText();
      }
    }


    public void SetAttachmentDirection()
    {
      IPlotArea layer = Main.DocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;

			var names = new SelectableListNodeList();

      int idx = -1;
      if (layer != null)
      {
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _doc.AttachedAxis }))
        {
          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          names.Add(new SelectableListNode(info.Name, id, id==_doc.AttachedAxis));
        }
      }

      _view.Init_AttachedAxis(names); 
    }


    void InitializeLabelColumnText()
    {
			if (_view != null)
      {
        string name = _labelColumn==null ? string.Empty : _labelColumn.FullName;
				_view.Init_LabelColumn(name);
      }
    }
    #region IXYPlotLabelStyleController Members

  

	

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
			choice.SelectedColumn = _labelColumn as DataColumn;
      object choiceAsObject = choice;
      if(Current.Gui.ShowDialog(ref choiceAsObject,"Select label column"))
      {
        choice = (SingleColumnChoice)choiceAsObject;

       
        _labelColumn = choice.SelectedColumn;
        InitializeLabelColumnText();
        
      }
    }

		public void EhView_FontSizeChanged()
		{
			_percentFontSizeUnit.ReferenceQuantity = new QuantityWithUnit(_view.SelectedFont.Size, LengthUnitPoint.Instance);
		}
  
    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc.BackgroundStyle = _view.Background;
			_doc.Font = _view.SelectedFont;
			_doc.IndependentColor = _view.IsIndependentColorSelected;
			_doc.Color = _view.SelectedColor;
			_doc.HorizontalAlignment = (StringAlignment)(_view.SelectedHorizontalAlignment).Item;
			_doc.VerticalAlignment = (StringAlignment)(_view.SelectedVerticalAlignment).Item;

			var xOffs = _view.XOffset;
			if (xOffs.Unit is IRelativeUnit)
				_doc.XOffset = ((IRelativeUnit)xOffs.Unit).GetRelativeValueFromValue(xOffs.Value);
			else
				_doc.XOffset = xOffs.AsValueIn(LengthUnitPoint.Instance) / _font.Size;

			var yOffs = _view.YOffset;
			if (yOffs.Unit is IRelativeUnit)
				_doc.YOffset = ((IRelativeUnit)yOffs.Unit).GetRelativeValueFromValue(yOffs.Value);
			else
				_doc.YOffset = yOffs.AsValueIn(LengthUnitPoint.Instance) / _font.Size;

			if (_view.AttachToAxis && null != _view.AttachedAxis)
				_doc.AttachedAxis = (CSPlaneID)_view.AttachedAxis.Item;
			else
				_doc.AttachedAxis = null;

      _doc.Rotation = _view.SelectedRotation;
      
      _doc.LabelColumn  = _labelColumn;

      return true;
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
				return _view;
      }
      set
      {
				if (_view != null)
				{
					_view.LabelColumnSelected -= EhView_SelectLabelColumn;
					_view.FontSizeChanged -= EhView_FontSizeChanged;
				}

				_view = value as IXYPlotLabelStyleView;

				if (_view != null)
				{
					Initialize(false);
					_view.LabelColumnSelected += EhView_SelectLabelColumn;
					_view.FontSizeChanged += EhView_FontSizeChanged;
				}
      }
    }



    public object ModelObject
    {
      get { return this._doc; }
    }

    #endregion
  }
}
