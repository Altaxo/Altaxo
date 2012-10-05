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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.ComponentModel;

using Altaxo.Main;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi;
using Altaxo.Graph;
using Altaxo.Collections;



namespace Altaxo.Gui.Graph
{
  #region Interfaces
  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotScatterStyle
  /// </summary>
  public interface IXYPlotScatterStyleView
  {
   
  
    /// <summary>
    /// If activated, this causes the view to disable all gui elements if neither a line style nor a fill style is choosen.
    /// </summary>
    /// <param name="bActivate"></param>
    void SetEnableDisableMain(bool bActivate);    
    
    /// <summary>
    /// Initializes the plot style color combobox.
    /// </summary>
    /// <param name="sel">Current selection.</param>
    void InitializePlotStyleColor(NamedColor sel);

    /// <summary>
    /// Indicates, whether only colors of plot color sets should be shown.
    /// </summary>
    /// <param name="showPlotColorsOnly">True if only colors of plot color sets should be shown.</param>
    void SetShowPlotColorsOnly(bool showPlotColorsOnly);

    /// <summary>
    /// Initializes the symbol size combobox.
    /// </summary>
    /// <param name="value">Currently selected symbol size.</param>
    void InitializeSymbolSize(double value);

    /// <summary>
    /// Initializes the independent symbol size check box.
    /// </summary>
    /// <param name="val">True when independent symbol size is choosen.</param>
    void InitializeIndependentSymbolSize(bool val);

    /// <summary>
    /// Initializes the symbol style combobox.
    /// </summary>
    /// <param name="list">Possible selections</param>
    void InitializeSymbolStyle(SelectableListNodeList list);

    /// <summary>
    /// Initializes the symbol shape combobox.
    /// </summary>
    /// <param name="list">Possible selections</param>
		void InitializeSymbolShape(SelectableListNodeList list);


    /// <summary>
    /// Intitalizes the drop line checkboxes.
    /// </summary>
    /// <param name="names">List of names plus the information if they are selected or not.</param>
    void InitializeDropLineConditions(SelectableListNodeList names);

    
    void InitializeIndependentColor(bool val);
   
    void InitializeSkipPoints(int val);
   

    #region Getter

    bool IndependentColor { get; }
    
    NamedColor SymbolColor { get; }
    SelectableListNode SymbolShape {get; }
    bool   IndependentSymbolSize { get; }
    SelectableListNode SymbolStyle {get; }
    double SymbolSize  {get; }

    SelectableListNodeList DropLines { get; }
    int SkipPoints { get; }

		string RelativePenWidth { get; set; }

    #endregion // Getter

    #region events

    event Action IndependentColorChanged;

    #endregion
  }

 

 

  #endregion

  /// <summary>
  /// Summary description for XYPlotScatterStyleController.
  /// </summary>
  [UserControllerForObject(typeof(ScatterPlotStyle))]
  [ExpectedTypeOfView(typeof(IXYPlotScatterStyleView))]
	public class XYPlotScatterStyleController : IMVCANController
  {
    ScatterPlotStyle _originalDoc;
    ScatterPlotStyle _doc;
    IXYPlotScatterStyleView _view;
    UseDocument _useDocumentCopy;

    /// <summary>Contains the color group style of the parent plot item collection (if present).</summary>
    Altaxo.Graph.Plot.Groups.ColorGroupStyle _colorGroupStyle;
    bool _showPlotColorsOnly = false;
    
    public XYPlotScatterStyleController()
    { 
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args==null || args.Length == 0 || !(args[0] is ScatterPlotStyle))
        return false;

			var tempView = this.ViewObject;
			this.ViewObject = null; // temporarily deactivate view to avoid cascading updates

      _originalDoc = (ScatterPlotStyle)args[0];
      _doc = _useDocumentCopy == UseDocument.Directly ? _originalDoc : (ScatterPlotStyle)_originalDoc.Clone();
      Initialize(true);

			this.ViewObject = tempView;
      return true;
    }

    public UseDocument UseDocumentCopy { set { _useDocumentCopy = value; } } // not used here


  


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
      if (initData)
      {
        // try to get the color group style that is responsible for coloring this item
        _colorGroupStyle = GetColorGroupStyle();
        _showPlotColorsOnly = (!_doc.IndependentColor && null != _colorGroupStyle);
      }
      if(_view!=null)
      {
        // now we have to set all dialog elements to the right values
        _view.SetShowPlotColorsOnly(_showPlotColorsOnly);
        _view.InitializeIndependentColor(_doc.IndependentColor);
        _view.InitializeIndependentSymbolSize(_doc.IndependentSymbolSize);
       
        SetPlotStyleColor();

        // Scatter properties
        SetSymbolShape();
        SetSymbolStyle();
        SetSymbolSize();
        SetDropLineConditions();
        _view.SetEnableDisableMain(_ActivateEnableDisableMain);
        _view.InitializeSkipPoints(_doc.SkipFrequency);
				_view.RelativePenWidth = Altaxo.Serialization.GUIConversion.ToString(Math.Round(100*_doc.RelativePenWidth, 3));
      }
    }

    public Altaxo.Graph.Plot.Groups.ColorGroupStyle GetColorGroupStyle()
    {
      var plotItemCollection = Altaxo.Main.DocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.Plot.PlotItemCollection>(_originalDoc);
      if (null == plotItemCollection)
        return null;

      if (plotItemCollection.GroupStyles.ContainsType(typeof(Altaxo.Graph.Plot.Groups.ColorGroupStyle)))
        return (Altaxo.Graph.Plot.Groups.ColorGroupStyle)plotItemCollection.GroupStyles.GetPlotGroupStyle(typeof(Altaxo.Graph.Plot.Groups.ColorGroupStyle));
      else
        return null;
    }


    void EhIndependentColorChanged()
    {
      _doc.IndependentColor = _view.IndependentColor;
      _showPlotColorsOnly = (!_doc.IndependentColor && null != _colorGroupStyle);
      if (null != _view)
        _view.SetShowPlotColorsOnly(_showPlotColorsOnly);
    }

    public void SetSymbolSize()
    {
      string[] SymbolSizes = 
      { "0","1","3","5","8","12","15","18","24","30"};

      _view.InitializeSymbolSize(_doc.SymbolSize);
    }

  
    public void SetSymbolStyle()
    {
      _view.InitializeSymbolStyle(new SelectableListNodeList(_doc.Style));
    }

  
    public void SetSymbolShape()
    {
      _view.InitializeSymbolShape(new SelectableListNodeList(_doc.Shape));
    }
   

    


    public void SetDropLineConditions()
    {
      XYPlotLayer layer = DocumentPath.GetRootNodeImplementing(_originalDoc, typeof(XYPlotLayer)) as XYPlotLayer;

      SelectableListNodeList names = new SelectableListNodeList();

      foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyles.AxisStyleIDs, _doc.DropLine))
      {

        bool sel = _doc.DropLine.Contains(id);
        CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
        names.Add(new SelectableListNode(info.Name, id, sel));
      }

      _view.InitializeDropLineConditions(names); 
    }




    public void SetPlotStyleColor()
    {
      _view.InitializePlotStyleColor(_doc.Pen.Color);
    }


    #region IMVCController Members
    public object ViewObject
    {
      get { return _view; }
      set
      {
				if (_view != null)
				{
          _view.IndependentColorChanged -= EhIndependentColorChanged;
				}

        _view = value as IXYPlotScatterStyleView;

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
        return _originalDoc;
      }
    }
    #endregion
  
    #region IApplyController Members

    public bool Apply()
    {

      // don't trust user input, so all into a try statement
      try
      {
      

        // Symbol Color
        _originalDoc.Color = _view.SymbolColor;

        _originalDoc.IndependentColor = _view.IndependentColor;
      
        _originalDoc.IndependentSymbolSize = _view.IndependentSymbolSize;

        // Symbol Shape
				_originalDoc.Shape = (Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Shape)_view.SymbolShape.Tag;

        // Symbol Style
				_originalDoc.Style = (Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Style)_view.SymbolStyle.Tag;

        // Symbol Size
				_originalDoc.SymbolSize = _view.SymbolSize;

        // Drop line left
        _originalDoc.DropLine.Clear();
        foreach (SelectableListNode node in _view.DropLines)
					if(node.IsSelected)
						_originalDoc.DropLine.Add((CSPlaneID)node.Tag);

        // Skip points

        _originalDoc.SkipFrequency = _view.SkipPoints;

				double relPenWidth;
				if (!Altaxo.Serialization.GUIConversion.IsDouble(_view.RelativePenWidth, out relPenWidth))
				{
					Current.Gui.ErrorMessageBox("Relative pen width is not a number");
					return false;
				}
				else
				{
					_originalDoc.RelativePenWidth = (float)relPenWidth/100;
				}
       

      }
      catch(Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occured: " + ex.Message);
        return false;
      }

      return true;
    }

    #endregion

   
  } // end of class XYPlotScatterStyleController
} // end of namespace
