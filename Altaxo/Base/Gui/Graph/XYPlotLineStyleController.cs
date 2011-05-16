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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;

using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotLineStyle
  /// </summary>
  public interface IXYPlotLineStyleView
  {
    /// <summary>
    /// If activated, this causes the view to disable all gui elements if neither a line style nor a fill style is choosen.
    /// </summary>
    /// <param name="bActivate"></param>
    void SetEnableDisableMain(bool bActivate);    


    void InitializeIndependentColor(bool val);
 
    void InitializePen(IColorTypeThicknessPenController controller);

    /// <summary>
    /// Initializes the LineSymbolGap check box.
    /// </summary>
    /// <param name="bGap">True if a gap between symbols and line should be shown.</param>
    void InitializeLineSymbolGapCondition(bool bGap);


    /// <summary>
    /// Initializes the Line connection combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections.</param>
    /// <param name="sel">Current selection.</param>
    void InitializeLineConnect(string[] arr , string sel);

   

    /// <summary>
    /// Initializes the fill check box.
    /// </summary>
    /// <param name="bFill">True if the plot should be filled.</param>
    void InitializeFillCondition(bool bFill);

    /// <summary>
    /// Initializes the fill direction combobox.
    /// </summary>
    /// <param name="list">List of possible selections.</param>
    /// <param name="sel">Current selection index.</param>
    void InitializeFillDirection(List<ListNode> list, int sel);

    /// <summary>
    /// Initializes the fill color combobox.
    /// </summary>
    /// <param name="sel">Current selection.</param>
    void InitializeFillColor(BrushX sel);
  

    #region Getter

    bool LineSymbolGap { get; }
    bool IndependentColor { get; }
   
 
    string LineConnect { get; }
    bool ConnectCircular { get; set; }
  
    bool   LineFillArea { get; }
    ListNode LineFillDirection { get; }
    BrushX LineFillColor {get; }
    bool IndependentFillColor { get; set; }

    

    #endregion // Getter

		#region events

		event Action IndependentColorChanged;

		#endregion
	}


  #endregion

  /// <summary>
  /// Summary description for XYPlotLineStyleController.
  /// </summary>
  [UserControllerForObject(typeof(LinePlotStyle))]
  [ExpectedTypeOfView(typeof(IXYPlotLineStyleView))]
	public class XYPlotLineStyleController : IMVCANController
  {
    IXYPlotLineStyleView _view;
    LinePlotStyle _doc;
    LinePlotStyle _tempDoc;
    ColorTypeThicknessPenController _penController;

		/// <summary>Contains the color group style of the parent plot item collection (if present).</summary>
		Altaxo.Graph.Plot.Groups.ColorGroupStyle _colorGroupStyle;

    UseDocument _useDocumentCopy;
    public XYPlotLineStyleController()
    {
    }
  

    public bool InitializeDocument(params object[] args)
    {
      if (args==null || args.Length == 0 || !(args[0] is LinePlotStyle))
        return false;

			var tempView = this.ViewObject; // deactivate the view to avoid cascading updates
			this.ViewObject = null;

      _doc = (LinePlotStyle)args[0];
      _tempDoc = _useDocumentCopy == UseDocument.Directly ? _doc : (LinePlotStyle)_doc.Clone();
      Initialize(true);

			this.ViewObject = tempView;

      return true;
    }

    public UseDocument UseDocumentCopy { set { _useDocumentCopy = value; } } // not used here

   
    public object ViewObject
    {
      get { return _view; }
      set
      {
				if (_view != null)
				{
					_view.IndependentColorChanged -= EhIndependentColorChanged;
				}

        _view = value as IXYPlotLineStyleView;

				if (_view != null)
				{
					Initialize(false);
					_view.IndependentColorChanged += EhIndependentColorChanged;
				}
      }
    }

	

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

 


    bool _ActivateEnableDisableMain = false;
    /// <summary>
    /// If activated, this causes the view to disable all gui elements if neither a line style nor a fill style is choosen.
    /// </summary>
    /// <param name="bActivate"></param>
    public void SetEnableDisableMain(bool bActivate)
    {
      _ActivateEnableDisableMain = bActivate;
      if(null!=_view)
        _view.SetEnableDisableMain(bActivate);
    }

    void Initialize(bool initData)
		{
			if(initData)
			{
				// try to get the color group style that is responsible for coloring this item
				_colorGroupStyle = GetColorGroupStyle();

				var penController = new ColorTypeThicknessPenController(_tempDoc.PenHolder);
				_penController = penController;

				if (null != _colorGroupStyle && !_tempDoc.IndependentColor)
					penController.SetSelectableColors( _colorGroupStyle.ColorSet, true);
			}

			if (_view != null)
			{
				_view.InitializeIndependentColor(_tempDoc.IndependentColor);


				// now we have to set all dialog elements to the right values
				_view.InitializePen(_penController);
				SetLineSymbolGapCondition();

				// Line properties
				SetLineConnect();
				SetFillCondition();
				SetFillDirection();
				SetFillColor();
				_view.IndependentFillColor = _tempDoc.IndependentFillColor;
				_view.ConnectCircular = _tempDoc.ConnectCircular;
				_view.SetEnableDisableMain(_ActivateEnableDisableMain);
			}
		}

		void EhIndependentColorChanged()
		{
			_tempDoc.IndependentColor = _view.IndependentColor;
			if (!_tempDoc.IndependentColor && null != _colorGroupStyle)
			{
				_penController.SetSelectableColors(_colorGroupStyle.ColorSet, true);
			}
			else
			{
				_penController.SetSelectableColors(NamedColor.Collection, false);
			}
		}

		public Altaxo.Graph.Plot.Groups.ColorGroupStyle GetColorGroupStyle()
		{
			var plotItemCollection = Altaxo.Main.DocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.Plot.PlotItemCollection>(_doc);
			if (null == plotItemCollection)
				return null;

			if (plotItemCollection.GroupStyles.ContainsType(typeof(Altaxo.Graph.Plot.Groups.ColorGroupStyle)))
				return (Altaxo.Graph.Plot.Groups.ColorGroupStyle)plotItemCollection.GroupStyles.GetPlotGroupStyle(typeof(Altaxo.Graph.Plot.Groups.ColorGroupStyle));
			else
				return null;
		}


    public void SetLineSymbolGapCondition()
    {
      _view.InitializeLineSymbolGapCondition( _tempDoc.LineSymbolGap );
    }


    public void SetLineConnect()
    {

      string [] names = System.Enum.GetNames(typeof(Altaxo.Graph.Gdi.Plot.Styles.XYPlotLineStyles.ConnectionStyle));
    
      _view.InitializeLineConnect(names,_tempDoc.Connection.ToString());
    }

    public void SetFillCondition()
    {
      _view.InitializeFillCondition( _tempDoc.FillArea );
    }

    public void SetFillDirection()
    {
      IPlotArea layer = Main.DocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;

      List<ListNode> names = new List<ListNode>();

      int idx = -1;
      if (layer != null)
      {
        int count = -1;
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _doc.FillDirection }))
        {
          count++;
          if (id == _doc.FillDirection)
            idx = count;

          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          names.Add(new ListNode(info.Name, id));
        }
      }
      _view.InitializeFillDirection(names,Math.Max(idx,0)); // _tempDoc.FillDirection.ToString());
    }

    public void SetFillColor()
    {
      _view.InitializeFillColor(_tempDoc.FillBrush);
    }


 


    #region IApplyController Members

    public bool Apply()
    {

      // don't trust user input, so all into a try statement
      try
      {

        // Symbol Gap
        _doc.LineSymbolGap = _view.LineSymbolGap;

        // Pen
        _doc.IndependentColor = _view.IndependentColor;
        _penController.Apply();
        _doc.PenHolder.CopyFrom( _tempDoc.PenHolder );

       
        // Line Connect
        _doc.Connection = (Altaxo.Graph.Gdi.Plot.Styles.XYPlotLineStyles.ConnectionStyle)Enum.Parse(typeof(Altaxo.Graph.Gdi.Plot.Styles.XYPlotLineStyles.ConnectionStyle), _view.LineConnect);
        _doc.ConnectCircular = _view.ConnectCircular;

        // Fill Area
        _doc.FillArea = _view.LineFillArea;
        // Line fill direction
        CSPlaneID id = null;
        if (_doc.FillArea && null != _view.LineFillDirection)
          id = ((CSPlaneID)_view.LineFillDirection.Tag);

        _doc.FillDirection = id;
        // Line fill color
        _doc.FillBrush = _view.LineFillColor;
        _doc.IndependentFillColor = _view.IndependentFillColor;

      }
      catch(Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occured. " + ex.Message);
        return false;
      }

      return true;
    }

    #endregion

 
  } // end of class XYPlotLineStyleController
} // end of namespace
