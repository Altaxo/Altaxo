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

using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IPlottingRangeViewEventSink
  {
    /// <summary>
    /// Called if one of the value has changed.
    /// </summary>
    /// <param name="fromValue">The old value.</param>
    /// <param name="toValue">The new selected item of the combo box.</param>
    void EhView_Changed(int fromValue, int toValue);
  }

  public interface IPlottingRangeView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IPlottingRangeViewEventSink Controller { get; set; }

    /// <summary>
    /// Initializes the view.
    /// </summary>
    /// <param name="from">First value of plot range.</param>
    /// <param name="to">Last value of plot range.</param>
    /// <param name="isInfinity">True if the plot range is infinite large.</param>
    void Initialize(int from, int to, bool isInfinity);
  }

  

  #endregion

  
  /// <summary>
  /// Summary description.
  /// </summary>
  [UserControllerForObject(typeof(ContiguousNonNegativeIntegerRange))]
  [ExpectedTypeOfView(typeof(IPlottingRangeView))]
  public class PlottingRangeController : IPlottingRangeViewEventSink, IMVCAController
  {
    IPlottingRangeView _view;
    ContiguousNonNegativeIntegerRange _doc;
    ContiguousNonNegativeIntegerRange _tempDoc;

    public PlottingRangeController(ContiguousNonNegativeIntegerRange doc)
    {
      _doc = doc;
      _tempDoc = doc;
    }

    public void Initialize()
    {
      if (_view != null)
      {
        _view.Initialize(_tempDoc.Start, _tempDoc.Last, _tempDoc.IsInfinite);
      }
    }

    #region IMVCController Members

    public object ViewObject
    {
      get { return _view; }
      set
      {
        if (_view != null)
          _view.Controller = null;

        _view = value as IPlottingRangeView;

        Initialize();

        if (_view != null)
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
      _doc =  _tempDoc;
      return true;
    }

    #endregion

    #region IPlotRangeViewEventSink Members

    public void EhView_Changed(int from, int to)
    {
      try
      {
        if (to != int.MaxValue)
          _tempDoc = ContiguousNonNegativeIntegerRange.NewFromStartAndLast(from, to);
        else
          _tempDoc = ContiguousNonNegativeIntegerRange.NewFromStartToInfinity(from);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
      }
    }

    #endregion
  }
}
