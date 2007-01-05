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
using Altaxo.Calc.Regression;
using Altaxo.Gui;

namespace Altaxo.Worksheet.GUI
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

    void InitializeNumberOfPoints(int val,  int max);
    void InitializeDerivativeOrder(int val, int max);
    void InitializePolynomialOrder(int val, int max);

    int GetNumberOfPoints();
    int GetDerivativeOrder();
    int GetPolynomialOrder();
  }

  #endregion

  /// <summary>
  /// Summary description for SavitzkyGolayParameterController.
  /// </summary>
  [UserControllerForObject(typeof(SavitzkyGolayParameters),100)]
  [ExpectedTypeOfView(typeof(ISavitzkyGolayParameterView))]
  public class SavitzkyGolayParameterController : IMVCAController, ISavitzkyGolayParameterViewEventSink
  {
    SavitzkyGolayParameters _doc;
    ISavitzkyGolayParameterView _view;

    int _numberOfPoints;
    int _polynomialOrder;
    int _derivativeOrder;

    public SavitzkyGolayParameterController(SavitzkyGolayParameters parameters)
    {
      _doc = parameters;
      _numberOfPoints = parameters.NumberOfPoints;
      _polynomialOrder = parameters.PolynomialOrder;
      _derivativeOrder = parameters.DerivativeOrder;
    }
    #region IApplyController Members

    void Initialize()
    {
      if(_view!=null)
      {
        _view.InitializeNumberOfPoints(_numberOfPoints,int.MaxValue);
        _view.InitializePolynomialOrder(_polynomialOrder,_numberOfPoints);
        _view.InitializeDerivativeOrder(_derivativeOrder,_polynomialOrder);
        
      }
    }

    public bool Apply()
    {
      if(_view!=null)
      {
        _doc.NumberOfPoints = _numberOfPoints;
        _doc.DerivativeOrder = _derivativeOrder;
        _doc.PolynomialOrder = _polynomialOrder;
        return true;
      }

      return true;
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
        if(_view!=null)
          _view.Controller = null;

        _view = value as ISavitzkyGolayParameterView;
        
        Initialize();

        if(_view!=null)
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

    public void EhValidatingNumberOfPoints(int val)
    {
      _polynomialOrder = Math.Min(_polynomialOrder,val);
      _view.InitializePolynomialOrder(_polynomialOrder,val);
      EhValidatingPolynomialOrder(_polynomialOrder);
    }

    public void EhValidatingPolynomialOrder(int val)
    {
      _derivativeOrder = Math.Min(_derivativeOrder,val);
      _view.InitializeDerivativeOrder(_derivativeOrder,val);
      EhValidatingDerivativeOrder(_derivativeOrder);
    }
    public void EhValidatingDerivativeOrder(int val)
    {
      _derivativeOrder = val;
    }
  }
}
