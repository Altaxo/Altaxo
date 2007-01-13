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
using Altaxo.Gui;

namespace Altaxo.Gui.Graph
{
  /// <summary>
  /// Interface for controlling the polynomial fit view.
  /// </summary>
  public interface IFitPolynomialDialogController : IMVCAController
  {
    /// <summary>Returns the fitting order.</summary>
    int Order { get; }
    /// <summary>Returns the maximum x of the fitting curve.</summary>
    double FitCurveXmax { get; }
    /// <summary>Returns the minimum x for the fitting curve.</summary>
    double FitCurveXmin { get ; }
    /// <summary>Returns the user choice, wether or not the formula should be shown in the graph.</summary>
    bool ShowFormulaOnGraph { get ; }

  }

  /// <summary>
  /// Interface for accessing the polynomial fit view.
  /// </summary>
  public interface IFitPolynomialDialogControl
  {
    IFitPolynomialDialogController Controller { get; set; }

    int Order { get; set; }
    double FitCurveXmin { get; set; }
    double FitCurveXmax { get; set; }
    bool ShowFormulaOnGraph { get; set; }
  }


  /// <summary>
  /// Controls the polynomial fit view.
  /// </summary>
  [ExpectedTypeOfView(typeof(IFitPolynomialDialogControl))]
  public class FitPolynomialDialogController : IFitPolynomialDialogController
  {
    int _Order;
    double _FitCurveXmin;
    double _FitCurveXmax;
    bool _ShowFormulaOnGraph;
    IFitPolynomialDialogControl _View;

    /// <summary>Returns the fitting order.</summary>
    public int Order { get { return _Order; }}
    /// <summary>Returns the maximum x of the fitting curve.</summary>
    public double FitCurveXmax { get { return _FitCurveXmax; }}
    /// <summary>Returns the minimum x for the fitting curve.</summary>
    public double FitCurveXmin { get { return _FitCurveXmin; }}
    /// <summary>Returns the user choice, wether or not the formula should be shown in the graph.</summary>
    public bool ShowFormulaOnGraph { get { return _ShowFormulaOnGraph; }}


    public FitPolynomialDialogController(int order, double xmin, double xmax, bool bShowFormulaOnGraph)
    {
      _Order = order;
      _FitCurveXmin = xmin;
      _FitCurveXmax = xmax;
      _ShowFormulaOnGraph = bShowFormulaOnGraph;

      SetElements(true);
    }



    void SetElements(bool bInit)
    {
      if(bInit)
      {
        
      }

      if(null!=View)
      {
        View.Order = this._Order;
        View.FitCurveXmin = this._FitCurveXmin;
        View.FitCurveXmax = this._FitCurveXmax;
        View.ShowFormulaOnGraph = this._ShowFormulaOnGraph;
      }
    }
    #region ILinkAxisController Members

    public IFitPolynomialDialogControl View
    {
      get
      {
        return _View;
      }
      set
      {
        if(null!=_View)
          _View.Controller = null;
        
        _View = value;

        if(null!=_View)
        {
          _View.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if(null!=View)
      {
        this._Order = View.Order;
        this._FitCurveXmin = View.FitCurveXmin;
        this._FitCurveXmax = View.FitCurveXmax;
        this._ShowFormulaOnGraph = View.ShowFormulaOnGraph;
      }
    
      return true;
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return View;
      }
      set
      {
        View = value as IFitPolynomialDialogControl;
      }
    }

    public object ModelObject
    {
      get { return this; }
    }

    #endregion
  }
}
