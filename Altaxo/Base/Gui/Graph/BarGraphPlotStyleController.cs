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
using System.Text;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IBarGraphPlotStyleView
  {
    bool IndependentColor { get; set; }
    BrushX FillBrush { get; set; }
    PenX FillPen { get; set; }
    string InnerGap { get; set; }
    string OuterGap { get; set; }
    bool UsePhysicalBaseValue { get; set; }
    string BaseValue { get; set; }
    bool StartAtPreviousItem { get; set; }
    string YGap { get; set; }
  }

  #endregion
  
  [UserControllerForObject(typeof(BarGraphPlotStyle))]
  [ExpectedTypeOfView(typeof(IBarGraphPlotStyleView))]
  public class BarGraphPlotStyleController : Gui.IMVCANController
  {
    IBarGraphPlotStyleView _view;
    BarGraphPlotStyle _doc;


    void Initialize(bool docAlso)
    {
      if (_view != null)
      {
        _view.IndependentColor = _doc.IndependentColor;
        _view.FillBrush = _doc.FillBrush;
        _view.FillPen = _doc.FramePen;
        _view.InnerGap = Serialization.GUIConversion.ToString(_doc.InnerGap * 100);
        _view.OuterGap = Serialization.GUIConversion.ToString(_doc.OuterGap * 100);
        _view.UsePhysicalBaseValue = _doc.UsePhysicalBaseValue;
        _view.BaseValue = _doc.UsePhysicalBaseValue ? _doc.BaseValue.ToString() : Serialization.GUIConversion.ToString(_doc.BaseValue.ToDouble() * 100);
        _view.StartAtPreviousItem = _doc.StartAtPreviousItem;
        _view.YGap = Serialization.GUIConversion.ToString(_doc.PreviousItemYGap * 100);
      }
    }

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;
      BarGraphPlotStyle doc = args[0] as BarGraphPlotStyle;
      if (doc == null)
        return false;
      _doc = doc;
      Initialize(true);
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
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
        _view = value as IBarGraphPlotStyleView;
        Initialize(false);
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc.IndependentColor = _view.IndependentColor;
      _doc.FillBrush = _view.FillBrush;
      _doc.FramePen = _view.FillPen;
      
      double innerGap;
      if(!Serialization.GUIConversion.IsDouble(_view.InnerGap,out innerGap))
        return false;

      double outerGap;
      if(!Serialization.GUIConversion.IsDouble(_view.OuterGap,out outerGap))
        return false;

      _doc.InnerGap = innerGap/100;
      _doc.OuterGap = outerGap/100;

      _doc.UsePhysicalBaseValue = _view.UsePhysicalBaseValue;
      if (_view.UsePhysicalBaseValue)
      {
        // who can parse this string? Only the y-scale know how to parse it
      }
      else
      {
        double basevalue;
        if (!Serialization.GUIConversion.IsDouble(_view.BaseValue, out basevalue))
          return false;
        _doc.BaseValue = new Altaxo.Data.AltaxoVariant(basevalue);
      }

      _doc.StartAtPreviousItem = _view.StartAtPreviousItem;

      if(_view.StartAtPreviousItem)
      {
        double ygap;
        if(!Serialization.GUIConversion.IsDouble(_view.YGap,out ygap))
          return false;
        _doc.PreviousItemYGap = ygap/100;
      }
      return true;      
    }

    #endregion
  }
}
