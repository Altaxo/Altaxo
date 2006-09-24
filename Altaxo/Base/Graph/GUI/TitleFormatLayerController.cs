#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Graph.Gdi.Axis;

namespace Altaxo.Graph.GUI
{
  #region Interfaces
  public interface ITitleFormatLayerController : Main.GUI.IApplyController, Main.GUI.IMVCController
  {
    ITitleFormatLayerView View { get; set; }
    void EhView_ShowAxisChanged(bool bShow);
    void EhView_TitleChanged(string text);
    void EhView_ColorChanged(string text);
    void EhView_ThicknessChanged(string text);
    void EhView_MajorTickLengthChanged(string text);
    void EhView_MajorTicksChanged(int sel);
    void EhView_MinorTicksChanged(int sel);
    void EhView_AxisPositionChanged(int sel);
    void EhView_AxisPositionValueChanged( string text);

  }

  public interface ITitleFormatLayerView : Main.GUI.IMVCView
  {

    ITitleFormatLayerController Controller { get; set; }

    System.Windows.Forms.Form Form  { get; }

    void InitializeShowAxis(bool bShow);

    void InitializeTitle(string org);

    void InitializeColor(string[] arr, string sel);

    void InitializeThickness(string[] arr, string sel);

    void InitializeMajorTickLength(string[] arr, string sel);

    void InitializeMajorTicks(string[] arr, int sel);

    void InitializeMinorTicks(string[] arr, int sel);

    void InitializeAxisPosition(string[] arr, int sel);

    void InitializeAxisPositionValue(string end);

    void InitializeAxisPositionValueEnabled(bool bEnab);


  }
  #endregion


  /// <summary>
  /// Summary description for TitleFormatLayerController.
  /// </summary>
  public class TitleFormatLayerController : ITitleFormatLayerController
  {
    protected ITitleFormatLayerView m_View;
    protected G2DAxisStyle _doc;

    protected bool    m_ShowAxis;
    protected bool    m_Original_ShowAxis;

    protected string  m_Title;
    protected string  m_Original_Title;

    protected string  m_Color;
    protected string  m_Original_Color;

    protected string  m_Thickness;
    protected string  m_Original_Thickness;

    protected string  m_MajorTickLength;
    protected string  m_Original_MajorTickLength;

    protected int     m_MajorTicks;
    protected int     m_Original_MajorTicks;

    protected int     m_MinorTicks;
    protected int     m_Original_MinorTicks;

    protected int     m_AxisPosition;
    protected int     m_Original_AxisPosition;

    protected string  m_AxisPositionValue;
    protected string  m_Original_AxisPositionValue;

    protected bool    m_AxisPositionValueEnabled = true;

    public TitleFormatLayerController(G2DAxisStyle doc)
    {
      _doc = doc;
      this.SetElements(true);
    }

    public void SetElements(bool bInit)
    {
      int i;
      System.Collections.ArrayList arr = new System.Collections.ArrayList();



      G2DAxisLineStyle axstyle=_doc.AxisLineStyle;
      string title = _doc.TitleText;
      bool bAxisEnabled=_doc.ShowAxisLine;

      // Show Axis
      if(bInit)
        m_ShowAxis = m_Original_ShowAxis = bAxisEnabled ;
      if(null!=View)
        View.InitializeShowAxis(m_ShowAxis);

      // fill axis title box
      if(bInit)
        m_Title = m_Original_Title = (null!=title ? title : "");
      if(null!=View)
        View.InitializeTitle(m_Title);

      // Color
      if(bInit)
      {
        this.m_Original_Color = PlotColors.Colors.GetPlotColorName(axstyle.Color);
        if(null==this.m_Original_Color)
          this.m_Original_Color = "Custom";
        m_Color = m_Original_Color;
      }
      if(null!=View)
      {
        arr.Clear();
        arr.Add("Custom");
        foreach(PlotColor c in PlotColors.Colors)
        {
          arr.Add(c.Name);
        }
        object sarr = arr.ToArray(typeof(string));
        View.InitializeColor((string[])sarr,this.m_Color);
      }


      // Thickness
      if(bInit)
        this.m_Thickness = m_Original_Thickness = axstyle.Thickness.ToString();
      if(null!=View)
      {
        // fill axis thickness combo box
        double[] thicknesses = new double[]{0.0,0.2,0.5,1.0,1.5,2.0,3.0,4.0,5.0};
        string[] s_thicknesses = new string[thicknesses.Length];
        for(i=0;i<s_thicknesses.Length;i++)
          s_thicknesses[i] = thicknesses[i].ToString();
        View.InitializeThickness(s_thicknesses,this.m_Thickness);
      }

      // Major tick length
      if(bInit)
        this.m_MajorTickLength = this.m_Original_MajorTickLength = axstyle.MajorTickLength.ToString();
      if(null!=View)
      {
        double[] ticklengths = new double[]{3,4,5,6,8,10,12,15,18,24,32};
        string[] s_ticklengths = new string[ticklengths.Length];
        for(i=0;i<s_ticklengths.Length;i++)
          s_ticklengths[i] = ticklengths[i].ToString();
        View.InitializeMajorTickLength(s_ticklengths,this.m_MajorTickLength);
      }


      // Major ticks
      if(bInit)
        this.m_MajorTicks = this.m_Original_MajorTicks = (axstyle.LeftSideMajorTicks?1:0) + (axstyle.RightSideMajorTicks?2:0); 
      if(null!=View)
      {
      if(_doc.CachedAxisInformation!=null)
        View.InitializeMajorTicks(new string[]{"None",_doc.CachedAxisInformation.NameOfLeftSide,_doc.CachedAxisInformation.NameOfRightSide,
          _doc.CachedAxisInformation.NameOfLeftSide+"&"+_doc.CachedAxisInformation.NameOfRightSide}, this.m_MajorTicks);
      else
       View.InitializeMajorTicks(new string[]{"None","LeftSide","RightSide","LeftSide&RightSide"},this.m_MajorTicks);
      }
      // Minor ticks
      if(bInit)
        this.m_MinorTicks = this.m_Original_MinorTicks = (axstyle.LeftSideMinorTicks?1:0) + (axstyle.RightSideMinorTicks?2:0); 
      if(null!=View)
      {
      if(_doc.CachedAxisInformation!=null)
        View.InitializeMinorTicks(new string[]{"None",_doc.CachedAxisInformation.NameOfLeftSide,_doc.CachedAxisInformation.NameOfRightSide,
          _doc.CachedAxisInformation.NameOfLeftSide+"&"+_doc.CachedAxisInformation.NameOfRightSide},this.m_MinorTicks);
      else
        View.InitializeMinorTicks(new string[]{"None","In","Out","In&Out"},this.m_MinorTicks);
      }


      // Position and PositionValue
      if(bInit)
      {
        if(axstyle.Position.Value==0)
        {
          this.m_Original_AxisPosition=0;
          this.m_Original_AxisPositionValue = "";
          this.m_AxisPositionValueEnabled = false;
        }
        else if(axstyle.Position.IsRelative)
        {
          this.m_Original_AxisPosition=1;
          this.m_Original_AxisPositionValue= (100.0*axstyle.Position.Value).ToString();
          this.m_AxisPositionValueEnabled = true;
        }
        else
        {
          this.m_Original_AxisPosition=2;
          this.m_Original_AxisPositionValue = axstyle.Position.Value.ToString();
          this.m_AxisPositionValueEnabled = true;
        }

        m_AxisPosition = m_Original_AxisPosition;
        m_AxisPositionValue = m_Original_AxisPositionValue;
      }

      if(null!=View)
      {
        View.InitializeAxisPosition(new string[] {
                                                   "Default",
                                                   "% from default"
                                                 }, this.m_AxisPosition);

        View.InitializeAxisPositionValue(this.m_AxisPositionValue);

        View.InitializeAxisPositionValueEnabled(this.m_AxisPositionValueEnabled);
      }
    }


    #region ITitleFormatLayerController Members

    public ITitleFormatLayerView View 
    { 
      get 
      {
        return m_View;
      }

      set
      {
        if(null!=m_View)
          m_View.Controller = null;
        
        m_View = value;

        if(null!=m_View)
        {
          m_View.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    public object ViewObject
    {
      get { return View; }
      set { View = value as ITitleFormatLayerView; }
    }
    public object ModelObject
    {
      get { return this._doc; }
    }

    public void EhView_ShowAxisChanged(bool bShow)
    {
      m_ShowAxis = bShow;
    }

    public void EhView_TitleChanged(string text)
    {
      m_Title = text;
    }

    public void EhView_ColorChanged(string text)
    {
      m_Color = text;
    }

    public void EhView_ThicknessChanged(string text)
    {
      m_Thickness = text;
    }

    public void EhView_MajorTickLengthChanged(string text)
    {
      m_MajorTickLength = text;
    }

    public void EhView_MajorTicksChanged(int sel)
    {
      m_MajorTicks = sel;
    }

    public void EhView_MinorTicksChanged(int sel)
    {
      m_MinorTicks = sel;
    }

    public void EhView_AxisPositionChanged(int sel)
    {
      m_AxisPosition = sel;
      if(null!=View)
        View.InitializeAxisPositionValueEnabled(m_AxisPosition!=0);
    }

    public void EhView_AxisPositionValueChanged(string text)
    {
      m_AxisPositionValue = text;
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      G2DAxisLineStyle axstyle=_doc.AxisLineStyle;

      try
      {

        // read axis title
        if(null!=m_Title && m_Title.Length==0)
          m_Title=null;

        // Axis enable and title
        _doc.ShowAxisLine = m_ShowAxis;
        _doc.TitleText = m_Title;


        // axis color
        if(m_Color!="Custom" && m_Color!=m_Original_Color)
          axstyle.Color = Color.FromName(m_Color);


        // read axis thickness
        if(m_Thickness != m_Original_Thickness)
          axstyle.Thickness = System.Convert.ToSingle(m_Thickness);

        // read major thick length
        if(m_MajorTickLength != m_Original_MajorTickLength)
          axstyle.MajorTickLength = System.Convert.ToSingle(m_MajorTickLength);

        // read major ticks
        if(m_MajorTicks != m_Original_MajorTicks)
        {
          axstyle.LeftSideMajorTicks = 0!=(1&m_MajorTicks);
          axstyle.RightSideMajorTicks = 0!=(2&m_MajorTicks);
        }
        // read minor ticks
        if(m_MinorTicks != m_Original_MinorTicks)
        {
          axstyle.LeftSideMinorTicks = 0!=(1&m_MinorTicks);
          axstyle.RightSideMinorTicks = 0!=(2&m_MinorTicks);
        }


        // read axis position
        if(m_AxisPosition!=m_Original_AxisPosition || m_AxisPositionValue != m_Original_AxisPositionValue)
        {
          double posval;
          switch(m_AxisPosition)
          {
            case 0:
              axstyle.Position = new Calc.RelativeOrAbsoluteValue(0,false);
              break;
            case 1:
              posval = System.Convert.ToDouble(m_AxisPositionValue);
              axstyle.Position = new Calc.RelativeOrAbsoluteValue(posval/100,true);
              break;
            case 2:
              posval = System.Convert.ToDouble(this.m_AxisPositionValue);
              axstyle.Position = new Calc.RelativeOrAbsoluteValue(posval,false);
              break;
            
          }
        }
      

        SetElements(true); // refill the Controller with the newly set values to ensure synchronization

      }
      catch(Exception)
      {
        return false; // failed
      }


      return true; // all ok
    }

    #endregion
  }
}
