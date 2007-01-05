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
using System.Windows.Forms;
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
    // Get / sets the controller of this view
    IXYPlotLineStyleViewEventSink Controller { get; set; }
    
    
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
  }

  /// <summary>
  /// This is the controller interface of the XYPlotLineStyleView
  /// </summary>
  public interface IXYPlotLineStyleViewEventSink
  {
    

  }

  public interface IXYPlotLineStyleController : IMVCANController
  {
    /// <summary>
    /// If activated, this causes the view to disable all gui elements if neither a line style nor a fill style is choosen.
    /// </summary>
    /// <param name="bActivate"></param>
    void SetEnableDisableMain(bool bActivate);    
  }


  #endregion

  /// <summary>
  /// Summary description for XYPlotLineStyleController.
  /// </summary>
  [UserControllerForObject(typeof(LinePlotStyle))]
  [ExpectedTypeOfView(typeof(IXYPlotLineStyleView))]
  public class XYPlotLineStyleController : IXYPlotLineStyleViewEventSink, IXYPlotLineStyleController
  {
    IXYPlotLineStyleView _view;
    LinePlotStyle _doc;
    LinePlotStyle _tempDoc;
    IColorTypeThicknessPenController _penController;

    UseDocument _useDocumentCopy;
    public XYPlotLineStyleController()
    {
    }
    public XYPlotLineStyleController(LinePlotStyle doc)
    {
      if(doc==null)
        throw new ArgumentNullException("doc is null");

      if (!InitializeDocument(doc))
        throw new ApplicationException("Programming error");
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is LinePlotStyle))
        return false;

      bool isFirstTime = (null == _doc);
      _doc = (LinePlotStyle)args[0];
      _tempDoc = _useDocumentCopy == UseDocument.Directly ? _doc : (LinePlotStyle)_doc.Clone();
      Initialize(isFirstTime);
      return true;
    }

    public UseDocument UseDocumentCopy { set { _useDocumentCopy = value; } } // not used here

   
    public object ViewObject
    {
      get { return _view; }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IXYPlotLineStyleView;
        
        Initialize(false);

        if(_view!=null)
          _view.Controller = this;
      }
    }
    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    public static string [] GetPlotColorNames()
    {
      string[] arr = new string[1+PlotColors.Colors.Count];

      arr[0] = "Custom";

      int i=1;
      foreach(PlotColor c in PlotColors.Colors)
      {
        arr[i++] = c.Name;
      }

      return arr;
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

    void Initialize(bool firstTime)
    {
      _penController = new ColorTypeThicknessPenController(_tempDoc.PenHolder);
      if(_view!=null)
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
          id = ((CSPlaneID)_view.LineFillDirection.Item);

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
