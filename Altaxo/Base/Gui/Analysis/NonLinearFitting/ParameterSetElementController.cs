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

using Altaxo.Calc.Regression.Nonlinear;
using System;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public interface IParameterSetElementView
  {
    void Initialize(string name, string value, bool vary, string variance);

    IParameterSetElementViewEventSink Controller { get; set; }
  }

  public interface IParameterSetElementViewEventSink
  {
    void EhView_ParameterValidating(string value, System.ComponentModel.CancelEventArgs e);

    void EhView_VarianceValidating(string value, System.ComponentModel.CancelEventArgs e);

    void EhView_VarySelectionChanged(bool value);
  }

  public interface IParameterSetElementController : IMVCAController, IParameterSetElementViewEventSink, Altaxo.Gui.IRefreshable
  {
  }

  /// <summary>
  /// Summary description for ParameterSetElementControl.
  /// </summary>
  [UserControllerForObject(typeof(ParameterSetElement), 100)]
  [ExpectedTypeOfView(typeof(IParameterSetElementView))]
  public class ParameterSetElementController : IParameterSetElementController
  {
    private ParameterSetElement _doc;
    private ParameterSetElement _tempdoc;
    private IParameterSetElementView _view;

    public ParameterSetElementController(ParameterSetElement doc)
    {
      _doc = doc;
      _tempdoc = new ParameterSetElement(doc);
    }

    protected void Initialize()
    {
      if (_view != null)
      {
        _view.Initialize(_tempdoc.Name,
          Altaxo.Serialization.GUIConversion.ToString(_tempdoc.Parameter),
          _tempdoc.Vary,
          Altaxo.Serialization.GUIConversion.ToString(_tempdoc.Variance)
          );
      }
    }

    /// <summary>
    /// Called when the doc has changed outside the controller. All changes that have been
    /// made manually are discarded, and the values of the changed document are shown on the view.
    /// </summary>
    public void Refresh()
    {
      // the doc has
      _tempdoc = new ParameterSetElement(_doc);
      Initialize();
    }

    public void EhView_ParameterValidating(string value, System.ComponentModel.CancelEventArgs e)
    {
      if (Altaxo.Serialization.GUIConversion.IsDouble(value))
      {
        double t;
        Altaxo.Serialization.GUIConversion.IsDouble(value, out t);
        _tempdoc.Parameter = t;
      }
      else
      {
        e.Cancel = true;
      }
    }

    public void EhView_VarianceValidating(string value, System.ComponentModel.CancelEventArgs e)
    {
      if (Altaxo.Serialization.GUIConversion.IsDouble(value))
      {
        double t;
        Altaxo.Serialization.GUIConversion.IsDouble(value, out t);
        _tempdoc.Variance = t;
      }
      else
      {
        e.Cancel = true;
      }
    }

    public void EhView_VarySelectionChanged(bool value)
    {
      _tempdoc.Vary = value;
    }

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view != null)
          _view.Controller = null;

        _view = value as IParameterSetElementView;

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

    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    public bool Apply(bool disposeController)
    {
      _doc.CopyFrom(_tempdoc);
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
  }
}
