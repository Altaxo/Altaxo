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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;
using Altaxo.Main.GUI;

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
    
    
    void InitializeIndependentColor(bool val);
 
    /// <summary>
    /// Initializes the plot style color combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializePlotStyleColor(string[] arr , string sel);

   

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
    /// Initializes the Line Style combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializeLineStyle(string[] arr , string sel);

    /// <summary>
    /// Initializes the Line Width combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializeLineWidth(string[] arr , string sel);

    /// <summary>
    /// Initializes the fill check box.
    /// </summary>
    /// <param name="bFill">True if the plot should be filled.</param>
    void InitializeFillCondition(bool bFill);

    /// <summary>
    /// Initializes the fill direction combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializeFillDirection(string[] arr , string sel);

    /// <summary>
    /// Initializes the fill color combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializeFillColor(string[] arr , string sel);
  

    #region Getter

    bool LineSymbolGap { get; }
    bool IndependentColor { get; }
   
    string SymbolColor { get; }
    string LineConnect { get; }
    string LineType    { get; }
    string LineWidth   { get; }
    bool   LineFillArea { get; }
    string LineFillDirection { get; }
    string LineFillColor {get; }

    

    #endregion // Getter
  }

  /// <summary>
  /// This is the controller interface of the XYPlotLineStyleView
  /// </summary>
  public interface IXYPlotLineStyleViewEventSink
  {
    
  }



  #endregion

  /// <summary>
  /// Summary description for XYPlotLineStyleController.
  /// </summary>
  	[UserControllerForObject(typeof(XYPlotLineStyle))]
  public class XYPlotLineStyleController : IXYPlotLineStyleViewEventSink, Main.GUI.IMVCAController
  {
    IXYPlotLineStyleView _view;
    XYPlotLineStyle _doc;
    XYPlotLineStyle _tempDoc;

    public XYPlotLineStyleController(XYPlotLineStyle doc)
    {
      _doc = doc;
      _tempDoc = (XYPlotLineStyle)_doc.Clone();
    }


   
    public object ViewObject
    {
      get { return _view; }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IXYPlotLineStyleView;
        
        Initialize();

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


  

    void Initialize()
    {
      if(_view!=null)
      {
        _view.InitializeIndependentColor(_tempDoc.IndependentColor);
       

        // now we have to set all dialog elements to the right values
        SetPlotStyleColor();
        SetLineSymbolGapCondition();

        // Line properties
        SetLineConnect();
        SetLineStyle();
        SetLineWidth();
        SetFillCondition();
        SetFillDirection();
        SetFillColor();
      }
    }


    public void SetLineSymbolGapCondition()
    {
      _view.InitializeLineSymbolGapCondition( _tempDoc.LineSymbolGap );
    }


    public void SetLineConnect()
    {

      string [] names = System.Enum.GetNames(typeof(Altaxo.Graph.XYPlotLineStyles.ConnectionStyle));
    
      _view.InitializeLineConnect(names,_tempDoc.Connection.ToString());
    }

    public void SetLineStyle()
    {
      string [] names = System.Enum.GetNames(typeof(DashStyle));

      _view.InitializeLineStyle(names,_tempDoc.PenHolder.DashStyle.ToString());
    }


  
    public void SetLineWidth()
    {
      float[] LineWidths = 
      { 0.2f,0.5f,1,1.5f,2,3,4,5 };
      string[] names = new string[LineWidths.Length];
      for(int i=0;i<names.Length;i++)
        names[i] = LineWidths[i].ToString();

      _view.InitializeLineWidth(names,_tempDoc.PenHolder.Width.ToString());
    }

    public void SetFillCondition()
    {
      _view.InitializeFillCondition( _tempDoc.FillArea );
    }

    public void SetFillDirection()
    {
      string [] names = System.Enum.GetNames(typeof(Altaxo.Graph.XYPlotLineStyles.FillDirection));
      _view.InitializeFillDirection(names,_tempDoc.FillDirection.ToString());
    }

    public void SetFillColor()
    {
      string name = "Custom"; // default

      if(null!=_tempDoc.FillBrush)
      {
        name = "Custom";
        if(_tempDoc.FillBrush.BrushType==BrushType.SolidBrush) 
        {
          name = PlotColors.Colors.GetPlotColorName(_tempDoc.FillBrush.Color);
          if(null==name)
            name = "Custom";
        }
      }
      _view.InitializeFillColor(GetPlotColorNames(), name);
    }


    public void SetPlotStyleColor()
    {
      string name = "Custom"; // default

      if(null!=_tempDoc.PenHolder)
      {
        name = "Custom";
        if(_tempDoc.PenHolder.PenType == PenType.SolidColor)
        {
          name = PlotColors.Colors.GetPlotColorName(_tempDoc.PenHolder.Color);
          if(null==name) 
            name = "Custom";
        }
      }
      
      
      _view.InitializePlotStyleColor(GetPlotColorNames(),name);
    }


    #region IApplyController Members

    public bool Apply()
    {

      // don't trust user input, so all into a try statement
      try
      {

        // Symbol Gap
        _doc.LineSymbolGap = _view.LineSymbolGap;

        // Symbol Color
        string str = _view.SymbolColor;
        if(str!="Custom")
        {
          _doc.Color = Color.FromName(str);
        }

        _doc.IndependentColor = _view.IndependentColor;
       
        // Line Connect
        _doc.Connection = (Altaxo.Graph.XYPlotLineStyles.ConnectionStyle)Enum.Parse(typeof(Altaxo.Graph.XYPlotLineStyles.ConnectionStyle),_view.LineConnect);
        // Line Type
        _doc.PenHolder.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),_view.LineType);
        // Line Width
        float width = System.Convert.ToSingle(_view.LineWidth);
        _doc.PenHolder.Width = width;


        // Fill Area
        _doc.FillArea = _view.LineFillArea;
        // Line fill direction
        _doc.FillDirection = (Altaxo.Graph.XYPlotLineStyles.FillDirection)Enum.Parse(typeof(Altaxo.Graph.XYPlotLineStyles.FillDirection),_view.LineFillDirection);
        // Line fill color
        str = _view.LineFillColor;
        if(str!="Custom")
         _doc.FillBrush = new BrushHolder(Color.FromName(str));

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
