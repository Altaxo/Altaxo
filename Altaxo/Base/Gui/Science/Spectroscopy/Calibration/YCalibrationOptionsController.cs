#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.Calibration;

namespace Altaxo.Gui.Science.Spectroscopy.Calibration
{
  public interface IYCalibrationOptionsView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IYCalibrationOptionsView))]
  [UserControllerForObject(typeof(YCalibrationOptions))]
  public class YCalibrationOptionsController : MVCANControllerEditImmutableDocBase<YCalibrationOptions, IYCalibrationOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(PreprocessingController, () => PreprocessingController = null);
    }

    #region Bindings



    private int _numberOfTerms;

    public int NumberOfTerms
    {
      get => _numberOfTerms;
      set
      {
        if (!(_numberOfTerms == value))
        {
          _numberOfTerms = value;
          OnPropertyChanged(nameof(NumberOfTerms));
          if (AvailableShapes?.SelectedValue is Type shapeType)
          {
            EhCurveShapeChanged(shapeType);
          }
        }
      }
    }

    private int _orderOfBaselinePolynomial;

    public int OrderOfBaselinePolynomial
    {
      get => _orderOfBaselinePolynomial;
      set
      {
        if (!(_orderOfBaselinePolynomial == value))
        {
          _orderOfBaselinePolynomial = value;
          OnPropertyChanged(nameof(OrderOfBaselinePolynomial));
          if (AvailableShapes?.SelectedValue is Type shapeType)
          {
            EhCurveShapeChanged(shapeType);
          }
        }
      }
    }

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

    public class ParameterItem : INotifyPropertyChanged
    {
      private string _name;

      public string Name
      {
        get => _name;
        set
        {
          if (!(_name == value))
          {
            _name = value;
            OnPropertyChanged(nameof(Name));
          }
        }
      }



      private double _value;

      public event PropertyChangedEventHandler? PropertyChanged;
      protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      public double Value
      {
        get => _value;
        set
        {
          if (!(_value == value))
          {
            _value = value;
            OnPropertyChanged(nameof(Value));
          }
        }
      }

    }

    public ObservableCollection<ParameterItem> ParametersOfCurve { get; } = new ObservableCollection<ParameterItem>();

    private SpectralPreprocessingController _preprocessingController;

    public SpectralPreprocessingController PreprocessingController
    {
      get => _preprocessingController;
      set
      {
        if (!(_preprocessingController == value))
        {
          _preprocessingController?.Dispose();
          _preprocessingController = value;
          OnPropertyChanged(nameof(PreprocessingController));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var preprocessingController = new SpectralPreprocessingController();
        preprocessingController.InitializeDocument(_doc.Preprocessing);
        Current.Gui.FindAndAttachControlTo(preprocessingController);
        PreprocessingController = preprocessingController;

        NumberOfTerms = _doc.CurveShape is IFitFunctionPeak ffp ? ffp.NumberOfTerms : 1;
        OrderOfBaselinePolynomial = _doc.CurveShape is IFitFunctionPeak ffp1 ? ffp1.OrderOfBaselinePolynomial : -1;
        var fitFuncTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.FitFunctions.Peaks.IFitFunctionPeak));

        // Parameters
        ParametersOfCurve.AddRange(_doc.CurveParameters.Select(p => new ParameterItem { Name = p.Name, Value = p.Value }));

        AvailableShapes = new ItemsController<Type>(
          new Collections.SelectableListNodeList(fitFuncTypes.Select(ff => new Collections.SelectableListNode(ff.Name, ff, false))), EhCurveShapeChanged);
        AvailableShapes.SelectedValue = _doc.CurveShape.GetType();
      }
    }

    private void EhCurveShapeChanged(Type newType)
    {
      var parameterDict = new Dictionary<string, double>();
      parameterDict.AddRange(ParametersOfCurve.Select(p => new KeyValuePair<string, double>(p.Name, p.Value)));

      var newFunction = (Altaxo.Calc.FitFunctions.Peaks.IFitFunctionPeak)Activator.CreateInstance(newType);
      newFunction = newFunction.WithNumberOfTerms(NumberOfTerms).WithOrderOfBaselinePolynomial(OrderOfBaselinePolynomial);
      ParametersOfCurve.Clear();
      for (int i = 0; i < newFunction.NumberOfParameters; ++i)
      {
        var name = newFunction.ParameterName(i);
        double value = 0;
        if (!parameterDict.TryGetValue(name, out value))
          value = 0;
        ParametersOfCurve.Add(new ParameterItem { Name = name, Value = value });
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!PreprocessingController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }

      var curveInstance = (IFitFunction)Activator.CreateInstance(AvailableShapes.SelectedValue);
      if (curveInstance is IFitFunctionPeak ffp)
      {
        curveInstance = ffp.WithNumberOfTerms(NumberOfTerms).WithOrderOfBaselinePolynomial(OrderOfBaselinePolynomial);
      }

      _doc = new YCalibrationOptions
      {
        Preprocessing = (SpectralPreprocessingOptionsBase)PreprocessingController.ModelObject,
        CurveShape = curveInstance,
        CurveParameters = ParametersOfCurve.Select(p => (p.Name, p.Value)).ToImmutableArray(),
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
