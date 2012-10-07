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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Serialization;
using System.Drawing;
using Altaxo.Graph.Gdi;

using Altaxo.Main;
using Altaxo.Data;
using Altaxo.Units;
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
		Font SelectedFont { get; set; }

    /// <summary>
    /// Initializes/gets the content of the Color combo box.
    /// </summary>
		NamedColor SelectedColor { get; set; }

    /// <summary>
    /// Initializes/gets the background.
    /// </summary>
    IBackgroundStyle Background { get; set; }
  

    

    /// <summary>
    /// Initializes the horizontal aligment combo box.
    /// </summary>
    /// <param name="list">The possible choices.</param>
    void Init_HorizontalAlignment(SelectableListNodeList list);

		ListNode SelectedHorizontalAlignment { get; }

    /// <summary>
    /// Initializes the vertical alignement combo box.
    /// </summary>
    /// <param name="list">The possible choices.</param>
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
    void Init_AttachedAxis(SelectableListNodeList names);

		ListNode AttachedAxis { get; }

    /// <summary>
    /// Initializes the content of the Rotation edit box.
    /// </summary>
    double SelectedRotation{get; set;}


    /// <summary>
    /// Initializes the content of the XOffset edit box.
    /// </summary>
    void Init_XOffset(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value);

		DimensionfulQuantity XOffset { get; }


    /// <summary>
    /// Initializes the content of the YOffset edit box.
    /// </summary>
		void Init_YOffset(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value);


		DimensionfulQuantity YOffset { get; }

    /// <summary>
    /// Initializes the content of the Independent color checkbox
    /// </summary>
		bool IndependentColor { get; set; }


		/// <summary>
		/// Indicates, whether only colors of plot color sets should be shown.
		/// </summary>
		/// <param name="showPlotColorsOnly">True if only colors of plot color sets should be shown.</param>
		void SetShowPlotColorsOnly(bool showPlotColorsOnly);

		#region events

		/// <summary>
		/// Occurs when the user choice for IndependentColor has changed.
		/// </summary>
		event Action IndependentColorChanged;

		#endregion
  }

  #endregion

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
  [UserControllerForObject(typeof(LabelPlotStyle))]
  [ExpectedTypeOfView(typeof(IXYPlotLabelStyleView))]
	public class XYPlotLabelStyleController : MVCANControllerBase<LabelPlotStyle, IXYPlotLabelStyleView>
  {
		/// <summary>Tracks the presence of a color group style in the parent collection.</summary>
		ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    /// <summary>The font of the label.</summary>
    protected Font _font;

    /// <summary>
    /// True if the color is independent of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    /// <summary>The color for the label.</summary>
    protected NamedColor  _color;
   
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

		ChangeableRelativePercentUnit _percentFontSizeUnit = new ChangeableRelativePercentUnit("%Em font size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));

    public XYPlotLabelStyleController()
    {
    }

    protected override void Initialize(bool initData)
    {
      if(initData)
      {
				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

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
				_percentFontSizeUnit = new ChangeableRelativePercentUnit("%Em size", "%", new DimensionfulQuantity(_font.Size, Units.Length.Point.Instance));
			}

      if(null!=_view)
      {
				_view.SetShowPlotColorsOnly(_colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor));
				_view.SelectedFont = _font;
				_view.IndependentColor = _independentColor;
				_view.SelectedColor = _color;
				_view.Init_HorizontalAlignment(new SelectableListNodeList(_horizontalAlignment));
				_view.Init_VerticalAlignment(new SelectableListNodeList(_verticalAlignment));
				_view.AttachToAxis = _attachToEdge;
        SetAttachmentDirection();
				_view.SelectedRotation = _originalDoc.Rotation;

				_percentFontSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_font.Size, Units.Length.Point.Instance);

				var xEnv = new QuantityWithUnitGuiEnvironment( GuiLengthUnits.Collection, _percentFontSizeUnit);
				_view.Init_XOffset(xEnv, new DimensionfulQuantity(_xOffset * 100, _percentFontSizeUnit));
				_view.Init_YOffset(xEnv, new DimensionfulQuantity(_yOffset * 100, _percentFontSizeUnit));
				_view.Background = _backgroundStyle;

        InitializeLabelColumnText();
      }
    }


    public void SetAttachmentDirection()
    {
      IPlotArea layer = DocumentPath.GetRootNodeImplementing(_originalDoc, typeof(IPlotArea)) as IPlotArea;

			var names = new SelectableListNodeList();

      if (layer != null)
      {
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _originalDoc.AttachedAxis }))
        {
          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          names.Add(new SelectableListNode(info.Name, id, id==_originalDoc.AttachedAxis));
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

		void EhIndependentColorChanged()
		{
			if (null != _view)
			{
				_doc.IndependentColor = _view.IndependentColor;
				_view.SetShowPlotColorsOnly(_colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor));
			}
		}

    #region IXYPlotLabelStyleController Members

  

	

    public void EhView_FontChanged(Font newValue)
    {
      _font = newValue;
    }

    public void EhView_ColorChanged(NamedColor color)
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
      _attachedEdge = ((CSPlaneID)newValue.Tag);
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
			_percentFontSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_view.SelectedFont.Size, Units.Length.Point.Instance);
		}
  
    #endregion

    #region IApplyController Members

    public override bool Apply()
    {
      _doc.BackgroundStyle = _view.Background;
			_doc.Font = _view.SelectedFont;
			_doc.IndependentColor = _view.IndependentColor;
			_doc.Color = _view.SelectedColor;
			_doc.HorizontalAlignment = (StringAlignment)(_view.SelectedHorizontalAlignment).Tag;
			_doc.VerticalAlignment = (StringAlignment)(_view.SelectedVerticalAlignment).Tag;

			var xOffs = _view.XOffset;
			if (xOffs.Unit is IRelativeUnit)
				_doc.XOffset = ((IRelativeUnit)xOffs.Unit).GetRelativeValueFromValue(xOffs.Value);
			else
				_doc.XOffset = xOffs.AsValueIn(Units.Length.Point.Instance) / _font.Size;

			var yOffs = _view.YOffset;
			if (yOffs.Unit is IRelativeUnit)
				_doc.YOffset = ((IRelativeUnit)yOffs.Unit).GetRelativeValueFromValue(yOffs.Value);
			else
				_doc.YOffset = yOffs.AsValueIn(Units.Length.Point.Instance) / _font.Size;

			if (_view.AttachToAxis && null != _view.AttachedAxis)
				_doc.AttachedAxis = (CSPlaneID)_view.AttachedAxis.Tag;
			else
				_doc.AttachedAxis = null;

      _doc.Rotation = _view.SelectedRotation;
      
      _doc.LabelColumn  = _labelColumn;

			if (_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);

      return true;
    }

    #endregion

    #region IMVCController Members

		protected override void AttachView()
		{
			base.AttachView();
			_view.LabelColumnSelected += EhView_SelectLabelColumn;
			_view.FontSizeChanged += EhView_FontSizeChanged;
			_view.IndependentColorChanged += EhIndependentColorChanged;
		}

		protected override void DetachView()
		{
			_view.LabelColumnSelected -= EhView_SelectLabelColumn;
			_view.FontSizeChanged -= EhView_FontSizeChanged;
			_view.IndependentColorChanged -= EhIndependentColorChanged;
			base.DetachView();
		}


  



  
    #endregion
  }
}
