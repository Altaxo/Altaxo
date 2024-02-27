#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Gui.Analysis.NonLinearFitting;
using Altaxo.Gui.Common;


namespace Altaxo.Gui.Calc.Interpolation
{
  public interface INonlinearFitAsInterpolationView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(INonlinearFitAsInterpolationView))]
  [UserControllerForObject(typeof(NonlinearFitAsInterpolation))]
  public class NonlinearFitAsInterpolationController : MVCANControllerEditImmutableDocBase<NonlinearFitAsInterpolation, INonlinearFitAsInterpolationView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(ParameterController, () => ParameterController = null!);
    }

    public NonlinearFitAsInterpolationController()
    {
      CmdConfigureShape = new RelayCommand(EhConfigureShape);
      CmdCopyValues = new RelayCommand(EhCopyValues);
      CmdPasteValues = new RelayCommand(EhPasteValues);
    }


    #region Bindings

    public ICommand CmdConfigureShape { get; }
    public ICommand CmdPasteValues { get; }
    public ICommand CmdCopyValues { get; }

    private void EhConfigureShape()
    {
      object fitFunc = _selectedShapeInstance;
      if (Current.Gui.ShowDialog(ref fitFunc, "Edit fit function"))
      {
        EhCurveShapeInstanceChanged((IFitFunction)fitFunc);
      }
    }

    private IFitFunction _selectedShapeInstance;

    private ItemsController<Type> _availableShapes;

    public ItemsController<Type> AvailableShapes
    {
      get => _availableShapes;
      set
      {
        if (!(_availableShapes == value))
        {
          _availableShapes = value;
          OnPropertyChanged(nameof(AvailableShapes));
        }
      }
    }

    private IMVCANController _parameterController;

    public IMVCANController ParameterController
    {
      get => _parameterController;
      set
      {
        if (!(_parameterController == value))
        {
          _parameterController?.Dispose();
          _parameterController = value;
          OnPropertyChanged(nameof(ParameterController));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _selectedShapeInstance = _doc.CurveShape;
        var fitFuncTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IFitFunction));

        // Parameters
        var paraSet = new ParameterSet(_doc.Parameters);
        ParameterController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { paraSet }, typeof(IMVCANController));

        AvailableShapes = new ItemsController<Type>(
          new Collections.SelectableListNodeList(fitFuncTypes.Select(ff => new Collections.SelectableListNode(ff.Name, ff, false))), EhCurveShapeChanged);
        AvailableShapes.SelectedValue = _selectedShapeInstance.GetType();
      }
    }

    private void EhCurveShapeChanged(Type newType)
    {
      var newFunction = (IFitFunction)Activator.CreateInstance(newType);
      EhCurveShapeInstanceChanged(newFunction);
    }

    private void EhCurveShapeInstanceChanged(IFitFunction newFunction)
    {
      if (newFunction is null)
        return;

      ParameterController.Apply(false);
      var parameterSet = (ParameterSet)ParameterController.ModelObject;

      var parameterDict = new Dictionary<string, ParameterSetElement>();
      parameterDict.AddRange(parameterSet.Select(p => new KeyValuePair<string, ParameterSetElement>(p.Name, p)));

      var newParameterSet = new List<ParameterSetElement>();
      for (int i = 0; i < newFunction.NumberOfParameters; ++i)
      {
        var name = newFunction.ParameterName(i);

        if (!parameterDict.TryGetValue(name, out var value))
          value = new ParameterSetElement(name);
        newParameterSet.Add(value);
      }
      ParameterController.InitializeDocument(new ParameterSet(newParameterSet));
      _selectedShapeInstance = newFunction;
    }

    private void EhPasteValues()
    {
      if (ParameterController is ParameterSetController psc)
        psc.EhPasteParameterValues();
    }

    private void EhCopyValues()
    {
      if (ParameterController is ParameterSetController psc)
        psc.EhCopyParameterValues();
    }

    public override bool Apply(bool disposeController)
    {
      if (!ParameterController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }
      var parameterSet = (ParameterSet)ParameterController.ModelObject;

      _doc = new NonlinearFitAsInterpolation(
        _selectedShapeInstance,
        parameterSet
        );

      return ApplyEnd(true, disposeController);
    }
  }
}
