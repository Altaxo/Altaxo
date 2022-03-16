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

#nullable disable
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph
{
 
  /// <summary>
  /// Interface for accessing the polynomial fit view.
  /// </summary>
  public interface IFitPolynomialView : IDataContextAwareView
  {
  }

  public record FitPolynomialOptions
  {
    public int Order { get; init; }
    public double? FitCurveXmin { get; init; }
    public double? FitCurveXmax { get; init; }
    public bool ShowFormulaOnGraph { get; init; }
  }

  /// <summary>
  /// Controls the polynomial fit view.
  /// </summary>
  [ExpectedTypeOfView(typeof(IFitPolynomialView))]
  public class FitPolynomialDialogController : MVCANControllerEditImmutableDocBase<FitPolynomialOptions, IFitPolynomialView>
  {
    public FitPolynomialDialogController()
    {
    }

    public FitPolynomialDialogController(int order, double? xmin, double? xmax, bool bShowFormulaOnGraph)
    {
      _doc = _originalDoc = new FitPolynomialOptions { Order = order, FitCurveXmin = xmin, FitCurveXmax = xmax, ShowFormulaOnGraph = bShowFormulaOnGraph };
      Initialize(true);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _order;

    public int Order
    {
      get => _order;
      set
      {
        if (!(_order == value))
        {
          _order = value;
          OnPropertyChanged(nameof(Order));
        }
      }
    }

    private double? _fitCurveXmin;

    public double? FitCurveXmin
    {
      get => _fitCurveXmin;
      set
      {
        if (!(_fitCurveXmin == value))
        {
          _fitCurveXmin = value;
          OnPropertyChanged(nameof(FitCurveXmin));
        }
      }
    }
    private double? _fitCurveXmax;

    public double? FitCurveXmax
    {
      get => _fitCurveXmax;
      set
      {
        if (!(_fitCurveXmax == value))
        {
          _fitCurveXmax = value;
          OnPropertyChanged(nameof(FitCurveXmax));
        }
      }
    }
    private bool _showFormulaOnGraph;

    public bool ShowFormulaOnGraph
    {
      get => _showFormulaOnGraph;
      set
      {
        if (!(_showFormulaOnGraph == value))
        {
          _showFormulaOnGraph = value;
          OnPropertyChanged(nameof(ShowFormulaOnGraph));
        }
      }
    }
    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      Order = _doc.Order;
      FitCurveXmin = _doc.FitCurveXmin;
      FitCurveXmax = _doc.FitCurveXmax;
      ShowFormulaOnGraph = _doc.ShowFormulaOnGraph;
    }

    public override bool Apply(bool disposeController)
    {
      _doc = new FitPolynomialOptions { Order = Order, FitCurveXmin = FitCurveXmin, FitCurveXmax = FitCurveXmax, ShowFormulaOnGraph = ShowFormulaOnGraph };
      return ApplyEnd(true, disposeController);
    }
  }
}


