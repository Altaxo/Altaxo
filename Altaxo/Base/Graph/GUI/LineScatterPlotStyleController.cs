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

namespace Altaxo.Graph.GUI
{
  #region Interfaces
  /// <summary>
  /// This view interface is for showing the options of the XYLineScatterPlotStyle
  /// </summary>
  public interface ILineScatterPlotStyleView
  {
    // Get / sets the controller of this view
    ILineScatterPlotStyleController Controller { get; set; }
  
    
    /// <summary>
    /// Returns the form this view is shown in.
    /// </summary>
    System.Windows.Forms.Form Form { get; }
    
    /// <summary>
    /// Initialized the plot style combobox.
    /// </summary>
    /// <param name="arr">Array of possible selections.</param>
    /// <param name="sel">Current selection.</param>
    void InitializePlotType(string[] arr, string sel);
    
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


    /// <summary>
    /// Initializes the plot group conditions.
    /// </summary>
    /// <param name="bMemberOfPlotGroup">True if this PlotItem is member of a plot group.</param>
    /// <param name="bIndependent">True if all plots independent from each other.</param>
    /// <param name="bColor">True if the color is changed.</param>
    /// <param name="bLineType">True if the line type is changed.</param>
    /// <param name="bSymbol">True if the symbol shape is changed.</param>
    void InitializePlotGroupConditions(bool bMemberOfPlotGroup, bool bIndependent, bool bColor, bool bLineType, bool bSymbol);


    #region Getter

    bool LineSymbolGap { get; }
    string SymbolColor { get; }
    string LineConnect { get; }
    string LineType    { get; }
    string LineWidth   { get; }
    bool   LineFillArea { get; }
    string LineFillDirection { get; }
    string LineFillColor {get; }

    string SymbolShape {get; }
    string SymbolStyle {get; }
    string SymbolSize  {get; }

    bool DropLineLeft  {get; }
    bool DropLineBottom {get; }
    bool DropLineRight {get; }
    bool DropLineTop   {get; }

    bool PlotGroupIncremental { get; }
    bool PlotGroupColor { get; }
    bool PlotGroupLineType { get; }
    bool PlotGroupSymbol { get; }





    #endregion // Getter
  }

  /// <summary>
  /// This is the controller interface of the LineScatterPlotStyleView
  /// </summary>
  public interface ILineScatterPlotStyleController
  {
    void EhView_PlotGroupIndependent_Changed(bool bPlotGroupIsIndependent);
  }

  #endregion

  /// <summary>
  /// Summary description for LineScatterPlotStyleController.
  /// </summary>
  public class LineScatterPlotStyleController : ILineScatterPlotStyleController, Main.GUI.IApplyController
  {
    protected AbstractXYPlotStyle m_MasterItemPlotStyle;
    protected AbstractXYPlotStyle m_PlotItemPlotStyle;
    protected AbstractXYPlotStyle m_PlotStyle;
    protected PlotGroup m_PlotGroup;
    protected PlotGroupStyle m_PlotGroupStyle;
    ILineScatterPlotStyleView m_View;

    public LineScatterPlotStyleController(AbstractXYPlotStyle ps, PlotGroup plotGroup)
    {
      // if this plotstyle belongs to a plot group of this layer,
      // use the master plot style instead of the plotstyle itself
      m_PlotGroup=plotGroup;
      m_PlotItemPlotStyle = ps;
      m_MasterItemPlotStyle = null!=plotGroup ? (AbstractXYPlotStyle)plotGroup.MasterItem.Style : null;
  
      if(null!=m_PlotGroup)
      {
        m_PlotStyle = (AbstractXYPlotStyle)m_PlotGroup.MasterItem.Style;
        m_PlotGroupStyle = m_PlotGroup.Style;
      }
      else // not member of a plotgroup
      {
        m_PlotStyle = ps;
        m_PlotGroupStyle = 0;
      }
    }


    public static bool ShowLineScatterPlotStyleAndDataDialog(System.Windows.Forms.Form parentWindow, PlotItem pa, PlotGroup plotGroup)
    {
      LineScatterPlotStyleController  ctrl = new LineScatterPlotStyleController((AbstractXYPlotStyle)pa.Style,plotGroup);
      LineScatterPlotStyleControl view = new LineScatterPlotStyleControl();
      ctrl.View = view;

      Main.GUI.DialogShellController dsc = new Main.GUI.DialogShellController(
        new Main.GUI.DialogShellView(view), ctrl);

      return dsc.ShowDialog(parentWindow);
    }

    public ILineScatterPlotStyleView View
    {
      get { return m_View; }
      set
      {
        m_View = value;
        m_View.Controller = this;

        SetPlotGroupElements();
        SetPlotStyleElements();
      }
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


    void SetPlotGroupElements()
    {
      SetPlotGroupConditions(this.m_PlotGroup);
    }

    void SetPlotStyleElements()
    {
      // now we have to set all dialog elements to the right values
      SetPlotType(this.m_PlotStyle);
      SetPlotStyleColor(this.m_PlotStyle);
      SetLineSymbolGapCondition(this.m_PlotStyle);


      // Scatter properties
      SetSymbolShape(m_PlotStyle);
      SetSymbolStyle(m_PlotStyle);
      SetSymbolSize(m_PlotStyle);
      SetDropLineConditions(m_PlotStyle);


      // Line properties
      SetLineConnect(m_PlotStyle);
      SetLineStyle(m_PlotStyle);
      SetLineWidth(m_PlotStyle);
      SetFillCondition(m_PlotStyle);
      SetFillDirection(m_PlotStyle);
      SetFillColor(m_PlotStyle);
    }


    public void SetSymbolSize(AbstractXYPlotStyle ps)
    {
      string[] SymbolSizes = 
      { "0","1","3","5","8","12","15","18","24","30"};

      float symbolsize = 1;
      if(null!=ps && null!=ps.XYPlotScatterStyle && null!=ps.XYPlotScatterStyle)
        symbolsize = ps.XYPlotScatterStyle.SymbolSize;

      string name = symbolsize.ToString();


      View.InitializeSymbolSize(SymbolSizes, name);
    }

  
    public void SetSymbolStyle(AbstractXYPlotStyle ps)
    {
      string [] names = System.Enum.GetNames(typeof(XYPlotScatterStyles.Style));

      XYPlotScatterStyles.Style sh = XYPlotScatterStyles.Style.Solid;
      if(null!=ps && null!=ps.XYPlotScatterStyle)
        sh = ps.XYPlotScatterStyle.Style;

      string name = sh.ToString();
    
      View.InitializeSymbolStyle(names,name);
    }

  
    public void SetSymbolShape(AbstractXYPlotStyle ps)
    {
      string [] names = System.Enum.GetNames(typeof(XYPlotScatterStyles.Shape));

      XYPlotScatterStyles.Shape sh = XYPlotScatterStyles.Shape.NoSymbol;
      if(null!=ps && null!=ps.XYPlotScatterStyle)
        sh = ps.XYPlotScatterStyle.Shape;

      string name = sh.ToString();
      
      View.InitializeSymbolShape(names,name);
    }

    public void SetLineSymbolGapCondition(AbstractXYPlotStyle ps)
    {
      bool bGap = ps.LineSymbolGap; // default
      View.InitializeLineSymbolGapCondition( bGap );
    }

    public void SetDropLineConditions(AbstractXYPlotStyle ps)
    {
      bool bLeft = 0!=(ps.XYPlotScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Left);
      bool bRight = 0!=(ps.XYPlotScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Right);
      bool bTop = 0!=(ps.XYPlotScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Top);
      bool bBottom = 0!=(ps.XYPlotScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Bottom);
    
      View.InitializeDropLineConditions(bLeft,bBottom,bRight,bTop);
    }

    public void SetLineConnect(AbstractXYPlotStyle ps)
    {

      string [] names = System.Enum.GetNames(typeof(XYPlotLineStyles.ConnectionStyle));
    
      XYPlotLineStyles.ConnectionStyle cn = XYPlotLineStyles.ConnectionStyle.NoLine; // default

      if(ps!=null && ps.XYPlotLineStyle!=null)
        cn = ps.XYPlotLineStyle.Connection;

      string name = cn.ToString();

      View.InitializeLineConnect(names,name);
    }

    public void SetLineStyle(AbstractXYPlotStyle ps)
    {
      string [] names = System.Enum.GetNames(typeof(DashStyle));

      DashStyle ds = DashStyle.Solid; // default
      if(ps!=null && ps.XYPlotLineStyle!=null && ps.XYPlotLineStyle.PenHolder!=null)
        ds = ps.XYPlotLineStyle.PenHolder.DashStyle;

      string name = ds.ToString();

      View.InitializeLineStyle(names,name);
    }


  
    public void SetLineWidth(AbstractXYPlotStyle ps)
    {
      float[] LineWidths = 
      { 0.2f,0.5f,1,1.5f,2,3,4,5 };
      string[] names = new string[LineWidths.Length];
      for(int i=0;i<names.Length;i++)
        names[i] = LineWidths[i].ToString();

      float linewidth = 1; // default value
      if(null!=ps && null!=ps.XYPlotLineStyle && null!=ps.XYPlotLineStyle.PenHolder)
        linewidth = ps.XYPlotLineStyle.PenHolder.Width;

      string name = linewidth.ToString();

      View.InitializeLineWidth(names,name);
    }

    public void SetFillCondition(AbstractXYPlotStyle ps)
    {
      bool bFill = false; // default
      if(null!=ps && null!=ps.XYPlotLineStyle)
        bFill = ps.XYPlotLineStyle.FillArea;

      View.InitializeFillCondition( bFill );

    }

    public void SetFillDirection(AbstractXYPlotStyle ps)
    {
      string [] names = System.Enum.GetNames(typeof(XYPlotLineStyles.FillDirection));
      

      XYPlotLineStyles.FillDirection dir = XYPlotLineStyles.FillDirection.Bottom; // default
      if(null!=ps && null!=ps.XYPlotLineStyle)
        dir = ps.XYPlotLineStyle.FillDirection;

      string name = dir.ToString();
      View.InitializeFillDirection(names,name);
    }

    public void SetFillColor(AbstractXYPlotStyle ps)
    {
      string name = "Custom"; // default

      if(null!=ps && null!=ps.XYPlotLineStyle && null!=ps.XYPlotLineStyle.FillBrush)
      {
        name = "Custom";
        if(ps.XYPlotLineStyle.FillBrush.BrushType==BrushType.SolidBrush) 
        {
          name = AbstractXYPlotStyle.GetPlotColorName(ps.XYPlotLineStyle.FillBrush.Color);
          if(null==name) name = "Custom";
        }
      }
      View.InitializeFillColor(GetPlotColorNames(), name);
    }



    public void SetPlotType(AbstractXYPlotStyle ps)
    {

      string[] arr = { "Nothing", "Line", "Symbol", "Line_Symbol" };
      
      string sel = "Nothing";
      if(null!=ps.XYPlotLineStyle && null!=ps.XYPlotScatterStyle)
        sel = "Line_Symbol";
      else if(null!=ps.XYPlotLineStyle && null==ps.XYPlotScatterStyle)
        sel = "Line";
      else if(null==ps.XYPlotLineStyle && null!=ps.XYPlotScatterStyle)
        sel = "Symbol";
      else
        sel = "Nothing";

      View.InitializePlotType(arr,sel);
    }


    public void SetPlotStyleColor(AbstractXYPlotStyle ps)
    {
      string name = "Custom"; // default

      if(null!=ps && null!=ps.XYPlotLineStyle && null!=ps.XYPlotLineStyle.PenHolder)
      {
        name = "Custom";
        if(ps.XYPlotLineStyle.PenHolder.PenType == PenType.SolidColor)
        {
          name = AbstractXYPlotStyle.GetPlotColorName(ps.XYPlotLineStyle.PenHolder.Color);
          if(null==name) 
            name = "Custom";
        }
      }
      else if(null!=ps && null!=ps.XYPlotScatterStyle && null!=ps.XYPlotScatterStyle.Pen)
      {
        name = "Custom";
        if(ps.XYPlotScatterStyle.Pen.PenType == PenType.SolidColor)
        {
          name = AbstractXYPlotStyle.GetPlotColorName(ps.XYPlotScatterStyle.Pen.Color);
          if(null==name) name = "Custom";
        }
      }
      
      View.InitializePlotStyleColor(GetPlotColorNames(),name);
    }


    private void SetPlotGroupConditions(PlotGroup grp)
    {
      PlotGroupStyle Style = null!=grp ? grp.Style : 0;
  
      View.InitializePlotGroupConditions(
        null!=grp,
        null!=grp && grp.IsIndependent,
        (0!=(Style&PlotGroupStyle.Color)),
        (0!=(Style & PlotGroupStyle.Line)),
        (0!=(Style & PlotGroupStyle.Symbol))
        );

    }
    #region IApplyController Members

    public bool Apply()
    {

      // don't trust user input, so all into a try statement
      try
      {
        // Plot group options first, since they determine what AbstractXYPlotStyle needs to be changed
        m_PlotGroupStyle = 0;
        if(View.PlotGroupIncremental)
        {
          if(View.PlotGroupColor)    m_PlotGroupStyle |= PlotGroupStyle.Color;
          if(View.PlotGroupLineType) m_PlotGroupStyle |= PlotGroupStyle.Line;
          if(View.PlotGroupSymbol)   m_PlotGroupStyle |= PlotGroupStyle.Symbol;
  
          m_PlotStyle = m_MasterItemPlotStyle!=null ? m_MasterItemPlotStyle : m_PlotStyle;
        }
        else // independent
        {
          m_PlotStyle = m_PlotItemPlotStyle;
        }



        // Symbol Gap
        m_PlotStyle.LineSymbolGap = View.LineSymbolGap;

        // Symbol Color
        string str = View.SymbolColor;
        if(str!="Custom")
        {
          m_PlotStyle.Color = Color.FromName(str);
        }


        // Line Connect
        m_PlotStyle.XYPlotLineStyle.Connection = (XYPlotLineStyles.ConnectionStyle)Enum.Parse(typeof(XYPlotLineStyles.ConnectionStyle),View.LineConnect);
        // Line Type
        m_PlotStyle.XYPlotLineStyle.PenHolder.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),View.LineType);
        // Line Width
        float width = System.Convert.ToSingle(View.LineWidth);
        m_PlotStyle.XYPlotLineStyle.PenHolder.Width = width;


        // Fill Area
        m_PlotStyle.XYPlotLineStyle.FillArea = View.LineFillArea;
        // Line fill direction
        m_PlotStyle.XYPlotLineStyle.FillDirection = (XYPlotLineStyles.FillDirection)Enum.Parse(typeof(XYPlotLineStyles.FillDirection),View.LineFillDirection);
        // Line fill color
        str = View.LineFillColor;
        if(str!="Custom")
          m_PlotStyle.XYPlotLineStyle.FillBrush = new BrushHolder(Color.FromName(str));


        // Symbol Shape
        str = View.SymbolShape;
        m_PlotStyle.XYPlotScatterStyle.Shape = (XYPlotScatterStyles.Shape)Enum.Parse(typeof(XYPlotScatterStyles.Shape),str);

        // Symbol Style
        str = View.SymbolStyle;
        m_PlotStyle.XYPlotScatterStyle.Style = (XYPlotScatterStyles.Style)Enum.Parse(typeof(XYPlotScatterStyles.Style),str);

        // Symbol Size
        str = View.SymbolSize;
        m_PlotStyle.SymbolSize = System.Convert.ToSingle(str);

        // Drop line left
        if(View.DropLineLeft) 
          m_PlotStyle.XYPlotScatterStyle.DropLine |= XYPlotScatterStyles.DropLine.Left;
        else
          m_PlotStyle.XYPlotScatterStyle.DropLine &= (XYPlotScatterStyles.DropLine.All^XYPlotScatterStyles.DropLine.Left);


        // Drop line bottom
        if(View.DropLineBottom) 
          m_PlotStyle.XYPlotScatterStyle.DropLine |= XYPlotScatterStyles.DropLine.Bottom;
        else
          m_PlotStyle.XYPlotScatterStyle.DropLine &= (XYPlotScatterStyles.DropLine.All^XYPlotScatterStyles.DropLine.Bottom);

        // Drop line right
        if(View.DropLineRight) 
          m_PlotStyle.XYPlotScatterStyle.DropLine |= XYPlotScatterStyles.DropLine.Right;
        else
          m_PlotStyle.XYPlotScatterStyle.DropLine &= (XYPlotScatterStyles.DropLine.All^XYPlotScatterStyles.DropLine.Right);

        // Drop line top
        if(View.DropLineTop) 
          m_PlotStyle.XYPlotScatterStyle.DropLine |= XYPlotScatterStyles.DropLine.Top;
        else
          m_PlotStyle.XYPlotScatterStyle.DropLine &= (XYPlotScatterStyles.DropLine.All^XYPlotScatterStyles.DropLine.Top);


        if(null!=m_PlotGroup)
        {
          m_PlotGroup.Style = m_PlotGroupStyle;
          if(!m_PlotGroup.IsIndependent)
            m_PlotGroup.UpdateMembers();
        }

      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(this.View.Form,ex.ToString(),"A problem occured");
        return false;
      }

      return true;
    }

    #endregion

    #region ILineScatterPlotStyleController Members
    
    public void EhView_PlotGroupIndependent_Changed(bool bPlotGroupIsIndependent)
    {
      if(bPlotGroupIsIndependent)
      {
        m_PlotStyle = this.m_PlotItemPlotStyle;
        this.SetPlotStyleElements();
      }
      else // Plot Group is dependent
      {
        m_PlotStyle = this.m_MasterItemPlotStyle;
        this.SetPlotStyleElements();
      }

    }

    #endregion
  } // end of class LineScatterPlotStyleController
} // end of namespace
