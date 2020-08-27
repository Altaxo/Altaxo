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
using Altaxo.Calc.Regression;

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

  public interface ISavitzkyGolayParameterViewEventSink
  {
    void EhValidatingNumberOfPoints(int val);

    void EhValidatingPolynomialOrder(int val);

    void EhValidatingDerivativeOrder(int val);
  }

  public interface ISavitzkyGolayParameterView
  {
    ISavitzkyGolayParameterViewEventSink Controller { set; }

    void InitializeNumberOfPoints(int val, int max);

    void InitializeDerivativeOrder(int val, int max);

    void InitializePolynomialOrder(int val, int max);

    int GetNumberOfPoints();

    int GetDerivativeOrder();

    int GetPolynomialOrder();
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for SavitzkyGolayParameterController.
  /// </summary>
  [UserControllerForObject(typeof(SavitzkyGolayParameters), 100)]
  [ExpectedTypeOfView(typeof(ISavitzkyGolayParameterView))]
  public class SavitzkyGolayParameterController : IMVCAController, ISavitzkyGolayParameterViewEventSink
  {
    private SavitzkyGolayParameters _doc;
    private ISavitzkyGolayParameterView _view;

    private int _numberOfPoints;
    private int _polynomialOrder;
    private int _derivativeOrder;

    public SavitzkyGolayParameterController(SavitzkyGolayParameters parameters)
    {
      _doc = parameters;
      _numberOfPoints = parameters.NumberOfPoints;
      _polynomialOrder = parameters.PolynomialOrder;
      _derivativeOrder = parameters.DerivativeOrder;
    }

    #region IApplyController Members

    private void Initialize()
    {
      if (_view != null)
      {
        _view.InitializeNumberOfPoints(_numberOfPoints, int.MaxValue);
        _view.InitializePolynomialOrder(_polynomialOrder, _numberOfPoints);
        _view.InitializeDerivativeOrder(_derivativeOrder, _polynomialOrder);
      }
    }

    public bool Apply(bool disposeController)
    {
      if (_view != null)
      {
        _doc.NumberOfPoints = _numberOfPoints;
        _doc.DerivativeOrder = _derivativeOrder;
        _doc.PolynomialOrder = _polynomialOrder;
        return true;
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
        return _view;
      }
      set
      {
        if (_view != null)
          _view.Controller = null;

        _view = value as ISavitzkyGolayParameterView;

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

    public void EhValidatingNumberOfPoints(int val)
    {
      _numberOfPoints = val;
      _polynomialOrder = Math.Min(_polynomialOrder, val);
      _view.InitializePolynomialOrder(_polynomialOrder, val);
      EhValidatingPolynomialOrder(_polynomialOrder);
    }

    public void EhValidatingPolynomialOrder(int val)
    {
      _polynomialOrder = val;
      _derivativeOrder = Math.Min(_derivativeOrder, val);
      _view.InitializeDerivativeOrder(_derivativeOrder, val);
      EhValidatingDerivativeOrder(_derivativeOrder);
    }

    public void EhValidatingDerivativeOrder(int val)
    {
      _derivativeOrder = val;
    }
  }
}
