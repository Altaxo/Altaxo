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
    void EhView_Changed(string fromValue, string toValue);
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
  [UserControllerForObject(typeof(PositiveIntegerRange))]
  [ExpectedTypeOfView(typeof(IPlottingRangeView))]
  public class PlottingRangeController : IPlottingRangeViewEventSink, IMVCAController
  {
    IPlottingRangeView _view;
    PositiveIntegerRange _doc;
    PositiveIntegerRange _tempDoc;

    public PlottingRangeController(PositiveIntegerRange doc)
    {
      _doc = doc;
      _tempDoc = (PositiveIntegerRange)doc.Clone();
    }

    public void Initialize()
    {
      if (_view != null)
      {
        _view.Initialize(_tempDoc.First, _tempDoc.Last, _tempDoc.IsInfinite);
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
      _doc.CopyFrom( _tempDoc) ;
      return true;
    }

    #endregion

    #region IPlotRangeViewEventSink Members

    public void EhView_Changed(string fromValue, string toValue)
    {
      int from;
      int to = int.MaxValue;
      if (!int.TryParse(fromValue, out from))
        Current.Gui.ErrorMessageBox("PositiveIntegerRange 'From' is not a integer number");
      if (toValue != null && toValue.Trim().Length > 0)
      {
        if (!int.TryParse(toValue, out to))
          Current.Gui.ErrorMessageBox("PositiveIntegerRange 'To' is not a integer number");
      }

      try
      {
        if (to != int.MaxValue)
          _tempDoc = PositiveIntegerRange.NewFromFirstAndLast(from, to);
        else
          _tempDoc = PositiveIntegerRange.NewFromFirstToInfinity(from);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
      }
    }

    #endregion
  }
}
