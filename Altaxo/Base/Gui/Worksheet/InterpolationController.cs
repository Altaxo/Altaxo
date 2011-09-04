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
#endregion

using System;
using System.Collections.Generic;
using Altaxo.Calc.Regression;
using Altaxo.Gui;
using Altaxo.Collections;
using Altaxo.Calc;

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces
  
  public interface IInterpolationParameterView
  {
    void InitializeClassList(SelectableListNodeList list);
    void InitializeNumberOfPoints(string val);
    void InitializeXOrg(string val);
    void InitializeXEnd(string val);
    void SetDetailControl(object detailControl);

		event Action<ValidationEventArgs<string>> ValidatingFrom;
		event Action<ValidationEventArgs<string>> ValidatingTo;
		event Action<ValidationEventArgs<string>> ValidatingNumberOfPoints;
		event Action ChangedInterpolationMethod;
  }


  public class InterpolationParameters
  {
    public Altaxo.Calc.Interpolation.IInterpolationFunction InterpolationInstance;
    public double XOrg;
    public double XEnd;
    public int NumberOfPoints;
  }

  #endregion

  /// <summary>
  /// Summary description for InterpolationParameterController.
  /// </summary>
  [UserControllerForObject(typeof(InterpolationParameters),100)]
  [ExpectedTypeOfView(typeof(IInterpolationParameterView))]
  public class InterpolationParameterController : IMVCAController
  {
    InterpolationParameters _doc;
    IInterpolationParameterView _view;

    int? _numberOfPoints;
    double? _xOrg;
    double? _xEnd;

    Altaxo.Calc.Interpolation.IInterpolationFunction _interpolationInstance;
    IMVCAController _interpolationDetailController;
		SelectableListNodeList _classListA = new SelectableListNodeList();


    public InterpolationParameterController(InterpolationParameters parameters)
    {
      _doc = parameters;
      _numberOfPoints = parameters.NumberOfPoints;
      _xOrg = parameters.XOrg;
      _xEnd = parameters.XEnd;
      _interpolationInstance = parameters.InterpolationInstance;
      

    }

    void SetInterpolationDetailController(IMVCAController ctrl)
    {
      IMVCAController oldController = this._interpolationDetailController;
      this._interpolationDetailController = ctrl;

      if(_view!=null)
      {
        _view.SetDetailControl(ctrl==null ? null : ctrl.ViewObject);
      }

    }
    #region IApplyController Members

    void Initialize()
    {
      if(_view!=null)
      {
        _view.InitializeNumberOfPoints(Altaxo.Serialization.GUIConversion.ToString(_numberOfPoints));
				_view.InitializeXOrg(Altaxo.Serialization.GUIConversion.ToString(_xOrg));
        _view.InitializeXEnd(Altaxo.Serialization.GUIConversion.ToString(_xEnd));

        RetrieveClassList();

				if(null==_classListA.FirstSelectedNode)
					_classListA[0].IsSelected = true;
        
				_view.InitializeClassList(_classListA);
				this.EhInterpolationClassChanged(); // to make sure the right InterpolationInstance is set
      }
    }


    void RetrieveClassList()
    {
      System.Type[] rawTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.Interpolation.IInterpolationFunction));

			var list = new List<System.Type>();
      foreach(System.Type type in rawTypes)
      {
        if(type.IsClass && type.IsPublic && !type.IsAbstract && null!=type.GetConstructor(new System.Type[]{}))
          list.Add(type);
      }

			foreach(var clsType in list)
				_classListA.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(clsType),clsType,_interpolationInstance!=null && clsType==_interpolationInstance.GetType()));
		}

    public bool Apply()
    {
      if(null!=_interpolationDetailController && false==_interpolationDetailController.Apply())
      {
        return false;
      }

			if (null==_interpolationInstance || null == _numberOfPoints || null == _xOrg || null == _xEnd)
				return false;

        _doc.NumberOfPoints = (int)_numberOfPoints;
        _doc.XOrg = (double)_xOrg;
        _doc.XEnd = (double)_xEnd;
        _doc.InterpolationInstance = _interpolationInstance;


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
				if (_view != null)
				{
					_view.ChangedInterpolationMethod -= EhInterpolationClassChanged;
					_view.ValidatingFrom -= EhValidatingXOrg;
					_view.ValidatingTo -= EhValidatingXEnd;
					_view.ValidatingNumberOfPoints -= EhValidatingNumberOfPoints;
				}

        _view = value as IInterpolationParameterView;
        

				if (_view != null)
				{
					_view.ChangedInterpolationMethod += EhInterpolationClassChanged;
					_view.ValidatingFrom += EhValidatingXOrg;
					_view.ValidatingTo += EhValidatingXEnd;
					_view.ValidatingNumberOfPoints += EhValidatingNumberOfPoints;

					Initialize();
				}
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

    public void EhInterpolationClassChanged()
    {
			var sel = _classListA.FirstSelectedNode;
      this._interpolationInstance = (Altaxo.Calc.Interpolation.IInterpolationFunction)System.Activator.CreateInstance((System.Type)sel.Tag);
      SetInterpolationDetailController((IMVCAController)Current.Gui.GetControllerAndControl(new object[]{this._interpolationInstance},typeof(IMVCAController)));
    }

    public void EhValidatingNumberOfPoints(ValidationEventArgs<string> e)
    {
			_numberOfPoints = null;
      int var;
			if(!Altaxo.Serialization.GUIConversion.IsInteger(e.ValueToValidate, out var))
			{
				e.AddError("Value has to be an integer!");
				return;
			}

			if (var < 2)
			{
				e.AddError("Value has to be >=2");
				return;
			}

			_numberOfPoints = var;
    }

    public void EhValidatingXOrg(ValidationEventArgs<string> e)
    {
			_xOrg = null;
      double var;
      if(!Altaxo.Serialization.GUIConversion.IsDouble(e.ValueToValidate, out var))
			{
				e.AddError("Value has to be a number");
				return;
			}

			if (!var.IsFinite())
			{
				e.AddError("Value has to be a finite number");
				return;
			}

			_xOrg = var;
    }

		public void EhValidatingXEnd(ValidationEventArgs<string> e)
		{
			_xEnd = null;
			double var;
			if (!Altaxo.Serialization.GUIConversion.IsDouble(e.ValueToValidate, out var))
			{
				e.AddError("Value has to be a number");
				return;
			}

			if (!var.IsFinite())
			{
				e.AddError("Value has to be a finite number");
				return;
			}

			_xEnd = var;
		}

    #endregion
  }
}
