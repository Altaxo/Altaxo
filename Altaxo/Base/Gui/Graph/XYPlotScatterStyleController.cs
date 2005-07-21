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
using Altaxo.Main.GUI;
using Altaxo.Graph;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotScatterStyle
  /// </summary>
  public interface IXYPlotScatterStyleView
  {
    // Get / sets the controller of this view
    IXYPlotScatterStyleViewEventSink Controller { get; set; }
  
    
   
    
    
    
    /// <summary>
    /// Initializes the plot style color combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializePlotStyleColor(string[] arr , string sel);

    /// <summary>
    /// Initializes the symbol size combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializeSymbolSize(string[] arr , string sel);

    /// <summary>
    /// Initializes the symbol style combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializeSymbolStyle(string[] arr , string sel);

    /// <summary>
    /// Initializes the symbol shape combobox.
    /// </summary>
    /// <param name="arr">String array of possible selections</param>
    /// <param name="sel">Current selection.</param>
    void InitializeSymbolShape(string[] arr , string sel);


    /// <summary>
    /// Intitalizes the drop line checkboxes.
    /// </summary>
    /// <param name="bLeft">True when a line should be drawn to the left layer edge.</param>
    /// <param name="bBottom">True when a line should be drawn to the bottom layer edge.</param>
    /// <param name="bRight">True when a line should be drawn to the right layer edge.</param>
    /// <param name="bTop">True when a line should be drawn to the top layer edge.</param>
    void InitializeDropLineConditions(bool bLeft, bool bBottom, bool bRight, bool bTop);

    

   

    #region Getter

    string SymbolColor { get; }
    string SymbolShape {get; }
    string SymbolStyle {get; }
    string SymbolSize  {get; }

    bool DropLineLeft  {get; }
    bool DropLineBottom {get; }
    bool DropLineRight {get; }
    bool DropLineTop   {get; }


    #endregion // Getter
  }

  /// <summary>
  /// This is the controller interface of the XYPlotScatterStyleView
  /// </summary>
  public interface IXYPlotScatterStyleViewEventSink
  {
  }

  #endregion

  /// <summary>
  /// Summary description for XYPlotScatterStyleController.
  /// </summary>
  [UserControllerForObject(typeof(XYPlotScatterStyle))]
  public class XYPlotScatterStyleController : IXYPlotScatterStyleViewEventSink, Main.GUI.IMVCAController
  {
    XYPlotScatterStyle _doc;
    XYPlotScatterStyle _tempDoc;
    IXYPlotScatterStyleView _view;


    public XYPlotScatterStyleController(XYPlotScatterStyle doc)
    {

      _doc = doc;
      _tempDoc = (XYPlotScatterStyle)_doc.Clone();
    }

    public static string [] GetPlotColorNames()
    {
      string[] arr = new string[1+AbstractXYPlotStyle.PlotColors.Length];

      arr[0] = "Custom";

      int i=1;
      foreach(Color c in AbstractXYPlotStyle.PlotColors)
      {
        string name = c.ToString();
        arr[i++] = name.Substring(7,name.Length-8);
      }

      return arr;
    }


  

    void Initialize()
    {
      if(_view!=null)
      {
        // now we have to set all dialog elements to the right values
     
        SetPlotStyleColor();
      


        // Scatter properties
        SetSymbolShape();
        SetSymbolStyle();
        SetSymbolSize();
        SetDropLineConditions();
      }
    }


    public void SetSymbolSize()
    {
      string[] SymbolSizes = 
      { "0","1","3","5","8","12","15","18","24","30"};

       _view.InitializeSymbolSize(SymbolSizes, _tempDoc.SymbolSize.ToString());
    }

  
    public void SetSymbolStyle()
    {
      string [] names = System.Enum.GetNames(typeof(Altaxo.Graph.XYPlotScatterStyles.Style));
      _view.InitializeSymbolStyle(names,_tempDoc.Style.ToString());
    }

  
    public void SetSymbolShape()
    {
      string [] names = System.Enum.GetNames(typeof(Altaxo.Graph.XYPlotScatterStyles.Shape));

      Altaxo.Graph.XYPlotScatterStyles.Shape sh = Altaxo.Graph.XYPlotScatterStyles.Shape.NoSymbol;
      sh = _tempDoc.Shape;
      string name = sh.ToString();
      _view.InitializeSymbolShape(names,name);

    
    }
   

    public void SetDropLineConditions()
    {
      bool bLeft = 0!=(_tempDoc.DropLine&Altaxo.Graph.XYPlotScatterStyles.DropLine.Left);
      bool bRight = 0!=(_tempDoc.DropLine&Altaxo.Graph.XYPlotScatterStyles.DropLine.Right);
      bool bTop = 0!=(_tempDoc.DropLine&Altaxo.Graph.XYPlotScatterStyles.DropLine.Top);
      bool bBottom = 0!=(_tempDoc.DropLine&Altaxo.Graph.XYPlotScatterStyles.DropLine.Bottom);
    
      _view.InitializeDropLineConditions(bLeft,bBottom,bRight,bTop);
    }

   



    public void SetPlotStyleColor()
    {
      string name = "Custom"; // default

      if(null!=_tempDoc.Pen)
      {
        name = "Custom";
        if(_tempDoc.Pen.PenType == PenType.SolidColor)
        {
          name = AbstractXYPlotStyle.GetPlotColorName(_tempDoc.Pen.Color);
          if(null==name) 
            name = "Custom";
        }
      }
      
      _view.InitializePlotStyleColor(GetPlotColorNames(),name);
    }


    #region IMVCController Members
    public object ViewObject
    {
      get { return _view; }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IXYPlotScatterStyleView;
        
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
    #endregion
  
    #region IApplyController Members

    public bool Apply()
    {

      // don't trust user input, so all into a try statement
      try
      {
      

        // Symbol Color
        string str = _view.SymbolColor;
        if(str!="Custom")
        {
          _doc.Color = Color.FromName(str);
        }

        // Symbol Shape
        str = _view.SymbolShape;
        _doc.Shape = (Altaxo.Graph.XYPlotScatterStyles.Shape)Enum.Parse(typeof(Altaxo.Graph.XYPlotScatterStyles.Shape),str);

        // Symbol Style
        str = _view.SymbolStyle;
        _doc.Style = (Altaxo.Graph.XYPlotScatterStyles.Style)Enum.Parse(typeof(Altaxo.Graph.XYPlotScatterStyles.Style),str);

        // Symbol Size
        str = _view.SymbolSize;
        _doc.SymbolSize = System.Convert.ToSingle(str);

        // Drop line left
        if(_view.DropLineLeft) 
          _doc.DropLine |= Altaxo.Graph.XYPlotScatterStyles.DropLine.Left;
        else
          _doc.DropLine &= (Altaxo.Graph.XYPlotScatterStyles.DropLine.All^Altaxo.Graph.XYPlotScatterStyles.DropLine.Left);


        // Drop line bottom
        if(_view.DropLineBottom) 
          _doc.DropLine |= Altaxo.Graph.XYPlotScatterStyles.DropLine.Bottom;
        else
          _doc.DropLine &= (Altaxo.Graph.XYPlotScatterStyles.DropLine.All^Altaxo.Graph.XYPlotScatterStyles.DropLine.Bottom);

        // Drop line right
        if(_view.DropLineRight) 
          _doc.DropLine |= Altaxo.Graph.XYPlotScatterStyles.DropLine.Right;
        else
          _doc.DropLine &= (Altaxo.Graph.XYPlotScatterStyles.DropLine.All^Altaxo.Graph.XYPlotScatterStyles.DropLine.Right);

        // Drop line top
        if(_view.DropLineTop) 
          _doc.DropLine |= Altaxo.Graph.XYPlotScatterStyles.DropLine.Top;
        else
          _doc.DropLine &= (Altaxo.Graph.XYPlotScatterStyles.DropLine.All^Altaxo.Graph.XYPlotScatterStyles.DropLine.Top);


       

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
