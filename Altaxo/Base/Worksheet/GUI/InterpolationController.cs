#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Main.GUI;

namespace Altaxo.Worksheet.GUI
{
  #region Interfaces
  
  public interface IInterpolationParameterViewEventSink
  {
    void EhValidatingClassName(int val);
    void EhValidatingNumberOfPoints(string val, System.ComponentModel.CancelEventArgs e);
    void EhValidatingXOrg(string val, System.ComponentModel.CancelEventArgs e);
    void EhValidatingXEnd(string val, System.ComponentModel.CancelEventArgs e);

  }
  public interface IInterpolationParameterView
  {
    IInterpolationParameterViewEventSink Controller { set; }

    void InitializeClassList(string[] classes, int preselection);
    void InitializeNumberOfPoints(int val);
    void InitializeXOrg(double val);
    void InitializeXEnd(double val);
  }


  public class InterpolationParameters
  {
    public Altaxo.Calc.Interpolation.CurveBase InterpolationInstance;
    public double XOrg;
    public double XEnd;
    public int NumberOfPoints;
  }

  #endregion

  /// <summary>
  /// Summary description for InterpolationParameterController.
  /// </summary>
  [UserControllerForObject(typeof(InterpolationParameters),100)]
  public class InterpolationParameterController : Main.GUI.IMVCAController, IInterpolationParameterViewEventSink
  {
    InterpolationParameters _doc;
    IInterpolationParameterView _view;

    int _numberOfPoints;
    double _xOrg;
    double _xEnd;
    Altaxo.Calc.Interpolation.CurveBase _interpolationInstance;
    System.Type[] _classList;
    string[] _classListStrings;

    public InterpolationParameterController(InterpolationParameters parameters)
    {
      _doc = parameters;
      _numberOfPoints = parameters.NumberOfPoints;
      _xOrg = parameters.XOrg;
      _xEnd = parameters.XEnd;
      _interpolationInstance = parameters.InterpolationInstance;

    }
    #region IApplyController Members

    void Initialize()
    {
      if(_view!=null)
      {
        _view.InitializeNumberOfPoints(_numberOfPoints);
        _view.InitializeXOrg(_xOrg);
        _view.InitializeXEnd(_xEnd);

        RetrieveClassList();
        _view.InitializeClassList(_classListStrings,0);
        EhValidatingClassName(0); // to make sure the right InterpolationInstance is set
      }
    }


    void RetrieveClassList()
    {
      System.Type[] rawTypes = Altaxo.Main.Services.ReflectionService.GetSubclassesOf(typeof(Altaxo.Calc.Interpolation.IInterpolationFunction));

      System.Collections.ArrayList list = new System.Collections.ArrayList();
      foreach(System.Type type in rawTypes)
      {
        if(type.IsClass && type.IsPublic && !type.IsAbstract && null!=type.GetConstructor(new System.Type[]{}))
          list.Add(type);
      }

      _classList = (System.Type[])list.ToArray(typeof(System.Type));
      _classListStrings = new string[_classList.Length];
      for(int i=0;i<_classList.Length;i++)
        _classListStrings[i] = _classList[i].ToString();



    }

    public bool Apply()
    {
      if(_view!=null)
      {
        _doc.NumberOfPoints = _numberOfPoints;
        _doc.XOrg = _xOrg;
        _doc.XEnd = _xEnd;
        _doc.InterpolationInstance = this._interpolationInstance;

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

        _view = value as IInterpolationParameterView;
        
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

    #region IInterpolationParameterViewEventSink Members

    public void EhValidatingClassName(int val)
    {
      this._interpolationInstance = (Altaxo.Calc.Interpolation.CurveBase)System.Activator.CreateInstance(_classList[val]);
    }

    public void EhValidatingNumberOfPoints(string val, System.ComponentModel.CancelEventArgs e)
    {
      int var;
      if(Altaxo.Serialization.GUIConversion.IsInteger(val, out var))
   
        _numberOfPoints = var;
      else
        e.Cancel = true;
    }

    public void EhValidatingXOrg(string val, System.ComponentModel.CancelEventArgs e)
    {
      double var;
      if(Altaxo.Serialization.GUIConversion.IsDouble(val, out var))
   
        _xOrg = var;
      else
        e.Cancel = true;
    }

    public void EhValidatingXEnd(string val, System.ComponentModel.CancelEventArgs e)
    {
      double var;
      if(Altaxo.Serialization.GUIConversion.IsDouble(val, out var))
   
        _xEnd = var;
      else
        e.Cancel = true;
    }

    #endregion
  }
}
