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
		BrushX LabelBrush { get; set; }

    /// <summary>
    /// Initializes/gets the background.
    /// </summary>
    IBackgroundStyle Background { get; set; }
  

    

    /// <summary>
    /// Initializes the horizontal aligment combo box.
    /// </summary>
    /// <param name="list">The possible choices.</param>
    void Init_HorizontalAlignment(SelectableListNodeList list);

    /// <summary>
    /// Initializes the vertical alignement combo box.
    /// </summary>
    /// <param name="list">The possible choices.</param>
    void Init_VerticalAlignment(SelectableListNodeList list);

    /// <summary>
		/// Initializes the content of the AttachToAxis checkbox. True if the label is attached to one of the four axes.
    /// </summary>
		bool AttachToAxis { get; set; }


    /// <summary>
    /// Initializes the AttachedAxis combo box.
    /// </summary>
    /// <param name="names">The possible choices.</param>
    void Init_AttachedAxis(SelectableListNodeList names);

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
    bool ShowPlotColorsOnly { set; }

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

    SelectableListNodeList _horizontalAlignmentChoices;
    SelectableListNodeList _verticalAlignmentChoices;
    SelectableListNodeList _attachmentDirectionChoices;

		ChangeableRelativePercentUnit _percentFontSizeUnit = new ChangeableRelativePercentUnit("%Em font size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));

   

    public XYPlotLabelStyleController()
    {
    }

    protected override void Initialize(bool initData)
    {
      if(initData)
      {
				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);
        _horizontalAlignmentChoices = new SelectableListNodeList(_doc.HorizontalAlignment);
        _verticalAlignmentChoices = new SelectableListNodeList(_doc.VerticalAlignment);
        InitializeAttachmentDirectionChoices();
      }

      if(null!=_view)
      {
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        _view.SelectedFont = _doc.Font;
        _view.IndependentColor = _doc.IndependentColor;
        _view.LabelBrush = _doc.LabelBrush;
				_view.Init_HorizontalAlignment(_horizontalAlignmentChoices);
				_view.Init_VerticalAlignment(_verticalAlignmentChoices);
        _view.AttachToAxis = _doc.AttachedAxis != null;
        _view.Init_AttachedAxis(_attachmentDirectionChoices);
				_view.SelectedRotation = _originalDoc.Rotation;

				_percentFontSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_doc.Font.Size, Units.Length.Point.Instance);

				var xEnv = new QuantityWithUnitGuiEnvironment( GuiLengthUnits.Collection, _percentFontSizeUnit);
        _view.Init_XOffset(xEnv, new DimensionfulQuantity(_doc.XOffset * 100, _percentFontSizeUnit));
        _view.Init_YOffset(xEnv, new DimensionfulQuantity(_doc.YOffset * 100, _percentFontSizeUnit));
        _view.Background = _doc.BackgroundStyle;

        InitializeLabelColumnText();
      }
    }


    public void InitializeAttachmentDirectionChoices()
    {
      IPlotArea layer = DocumentPath.GetRootNodeImplementing(_originalDoc, typeof(IPlotArea)) as IPlotArea;

      _attachmentDirectionChoices = new SelectableListNodeList();

      if (layer != null)
      {
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _originalDoc.AttachedAxis }))
        {
          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          _attachmentDirectionChoices.Add(new SelectableListNode(info.Name, id, id == _originalDoc.AttachedAxis));
        }
      }
    }


    void InitializeLabelColumnText()
    {
			if (_view != null)
      {
        string name = _doc.LabelColumn==null ? string.Empty : _doc.LabelColumn.FullName;
				_view.Init_LabelColumn(name);
      }
    }

		void EhIndependentColorChanged()
		{
			if (null != _view)
			{
				_doc.IndependentColor = _view.IndependentColor;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
			}
		}

    #region IXYPlotLabelStyleController Members
  

    public void EhView_SelectLabelColumn()
    {
      SingleColumnChoice choice = new SingleColumnChoice();
			choice.SelectedColumn = _doc.LabelColumn as DataColumn;
      object choiceAsObject = choice;
      if(Current.Gui.ShowDialog(ref choiceAsObject,"Select label column"))
      {
        choice = (SingleColumnChoice)choiceAsObject;

       
        _doc.LabelColumn = choice.SelectedColumn;
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
			_doc.LabelBrush = _view.LabelBrush;
      _doc.HorizontalAlignment = (StringAlignment)_horizontalAlignmentChoices.FirstSelectedNode.Tag;
			_doc.VerticalAlignment = (StringAlignment)_verticalAlignmentChoices.FirstSelectedNode.Tag;

			var xOffs = _view.XOffset;
			if (xOffs.Unit is IRelativeUnit)
				_doc.XOffset = ((IRelativeUnit)xOffs.Unit).GetRelativeValueFromValue(xOffs.Value);
			else
				_doc.XOffset = xOffs.AsValueIn(Units.Length.Point.Instance) / _doc.Font.Size;

			var yOffs = _view.YOffset;
			if (yOffs.Unit is IRelativeUnit)
				_doc.YOffset = ((IRelativeUnit)yOffs.Unit).GetRelativeValueFromValue(yOffs.Value);
			else
				_doc.YOffset = yOffs.AsValueIn(Units.Length.Point.Instance) / _doc.Font.Size;

			if (_view.AttachToAxis && null != _attachmentDirectionChoices.FirstSelectedNode)
				_doc.AttachedAxis = (CSPlaneID)_attachmentDirectionChoices.FirstSelectedNode.Tag;
			else
				_doc.AttachedAxis = null;

      _doc.Rotation = _view.SelectedRotation;
      
      // _doc.LabelColumn  = _labelColumn; already set after dialog

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
