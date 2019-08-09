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

#endregion Copyright

using System;

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
    double FitCurveXmin { get; }

    /// <summary>Returns the user choice, wether or not the formula should be shown in the graph.</summary>
    bool ShowFormulaOnGraph { get; }
  }

  /// <summary>
  /// Interface for accessing the polynomial fit view.
  /// </summary>
  public interface IFitPolynomialDialogControl
  {
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
    private int _Order;
    private double _FitCurveXmin;
    private double _FitCurveXmax;
    private bool _ShowFormulaOnGraph;
    private IFitPolynomialDialogControl _View;

    /// <summary>Returns the fitting order.</summary>
    public int Order { get { return _Order; } }

    /// <summary>Returns the maximum x of the fitting curve.</summary>
    public double FitCurveXmax { get { return _FitCurveXmax; } }

    /// <summary>Returns the minimum x for the fitting curve.</summary>
    public double FitCurveXmin { get { return _FitCurveXmin; } }

    /// <summary>Returns the user choice, wether or not the formula should be shown in the graph.</summary>
    public bool ShowFormulaOnGraph { get { return _ShowFormulaOnGraph; } }

    public FitPolynomialDialogController(int order, double xmin, double xmax, bool bShowFormulaOnGraph)
    {
      _Order = order;
      _FitCurveXmin = xmin;
      _FitCurveXmax = xmax;
      _ShowFormulaOnGraph = bShowFormulaOnGraph;

      SetElements(true);
    }

    private void SetElements(bool bInit)
    {
      if (bInit)
      {
      }

      if (null != View)
      {
        View.Order = _Order;
        View.FitCurveXmin = _FitCurveXmin;
        View.FitCurveXmax = _FitCurveXmax;
        View.ShowFormulaOnGraph = _ShowFormulaOnGraph;
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
        _View = value;

        if (null != _View)
        {
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    #endregion ILinkAxisController Members

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      if (null != View)
      {
        _Order = View.Order;
        _FitCurveXmin = View.FitCurveXmin;
        _FitCurveXmax = View.FitCurveXmax;
        _ShowFormulaOnGraph = View.ShowFormulaOnGraph;
      }

      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members

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

    public void Dispose()
    {
    }

    #endregion IMVCController Members
  }
}
