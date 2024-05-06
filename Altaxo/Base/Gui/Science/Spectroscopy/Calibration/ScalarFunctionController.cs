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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Altaxo.Calc;
using Altaxo.Calc.FitFunctions;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Science.Spectroscopy.Calibration
{
  public interface IScalarFunctionView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IScalarFunctionView))]
  public class ScalarFunctionController : MVCANControllerEditImmutableDocBase<IScalarFunctionDD, IScalarFunctionView>
  {
    public enum TypeOfFunction { Peak, Polynomial, ExpressionString };

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<TypeOfFunction> _typeOfTheFunction;

    public ItemsController<TypeOfFunction> TypeOfTheFunction
    {
      get => _typeOfTheFunction;
      set
      {
        if (!(_typeOfTheFunction == value))
        {
          _typeOfTheFunction = value;
          OnPropertyChanged(nameof(TypeOfTheFunction));

        }
      }
    }

    public bool IsPeakFunction
    {
      get
      {
        return _typeOfTheFunction.SelectedValue == TypeOfFunction.Peak;
      }
      set
      {
        if (!IsPeakFunction && value == true)
        {
          TypeOfTheFunction.SelectedValue = TypeOfFunction.Peak;
        }
      }
    }
    public bool IsPolynomial
    {
      get
      {
        return _typeOfTheFunction.SelectedValue == TypeOfFunction.Polynomial;
      }
      set
      {
        if (!IsPolynomial && value == true)
        {
          TypeOfTheFunction.SelectedValue = TypeOfFunction.Polynomial;
        }
      }
    }

    public bool IsExpressionString
    {
      get
      {
        return _typeOfTheFunction.SelectedValue == TypeOfFunction.ExpressionString;
      }
      set
      {
        if (!IsExpressionString && value == true)
        {
          TypeOfTheFunction.SelectedValue = TypeOfFunction.ExpressionString;
        }
      }
    }

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
          if (IsPolynomial)
          {
            EhCurveShapeChanged(typeof(Altaxo.Calc.FitFunctions.General.Polynomial));
          }
          else if (IsPeakFunction && AvailableShapes?.SelectedValue is Type shapeType)
          {
            EhCurveShapeChanged(shapeType);
          }
        }
      }
    }


    private string _expressionString;

    public string ExpressionString
    {
      get => _expressionString;
      set
      {
        if (!(_expressionString == value))
        {
          _expressionString = value;
          OnPropertyChanged(nameof(ExpressionString));
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

    private string _fitFunctionDescription = string.Empty;

    public string FitFunctionDescription
    {
      get => _fitFunctionDescription;
      set
      {
        if (!(_fitFunctionDescription == value))
        {
          _fitFunctionDescription = value;
          OnPropertyChanged(nameof(FitFunctionDescription));
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


    #endregion Bindings

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        TypeOfFunction type;
        int numberOfTerms = 0;
        int orderOfBaselinePolynomial = -1;

        if (_doc is FitFunctionDDWrapper ffw)
        {
          if (ffw.FitFunction is IFitFunctionPeak ffp)
          {
            type = TypeOfFunction.Peak;
            numberOfTerms = ffp.NumberOfTerms;
            orderOfBaselinePolynomial = ffp.OrderOfBaselinePolynomial;
          }
          else if (_doc is Altaxo.Calc.FitFunctions.General.Polynomial po)
          {
            type = TypeOfFunction.Polynomial;
            orderOfBaselinePolynomial = po.PolynomialOrder_PositiveExponents;
          }
          else
          {
            throw new NotImplementedException();
          }
        }
        else if (_doc is ScalarFunctionDDExpression sfe)
        {
          type = TypeOfFunction.ExpressionString;
          ExpressionString = sfe.Expression;
        }
        else
        {
          throw new NotImplementedException();
        }

        // Parameters
        ParametersOfCurve.Clear();
        if (_doc is FitFunctionDDWrapper ddw)
        {
          for (int i = 0; i < ddw.FitFunction.NumberOfParameters; i++)
          {
            ParametersOfCurve.Add(new ParameterItem { Name = ddw.FitFunction.ParameterName(i), Value = ddw.Parameters[i] });
          }
        }

        TypeOfTheFunction = new ItemsController<TypeOfFunction>(new SelectableListNodeList(type), EhTypeOfFunctionChanged);

        NumberOfTerms = numberOfTerms;
        OrderOfBaselinePolynomial = orderOfBaselinePolynomial;

        var fitFuncTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.FitFunctions.Peaks.IFitFunctionPeak));
        AvailableShapes = new ItemsController<Type>(
          new Collections.SelectableListNodeList(fitFuncTypes.Select(ff => new Collections.SelectableListNode(ff.Name, ff, false))), EhCurveShapeChanged);

        if (_doc is FitFunctionDDWrapper ffw1 && ffw1.FitFunction is IFitFunctionPeak ffp1)
          AvailableShapes.SelectedValue = ffp1.GetType();


      }
    }

    private void EhTypeOfFunctionChanged(TypeOfFunction function)
    {
      OnPropertyChanged(nameof(IsPeakFunction));
      OnPropertyChanged(nameof(IsPolynomial));
      OnPropertyChanged(nameof(IsExpressionString));
      if (function == TypeOfFunction.Polynomial)
      {
        if (OrderOfBaselinePolynomial == -1)
        {
          OrderOfBaselinePolynomial = 0;
        }
        EhCurveShapeChanged(typeof(Altaxo.Calc.FitFunctions.General.Polynomial));
      }
      else if (function == TypeOfFunction.Peak && AvailableShapes.SelectedValue is not null)
      {
        EhCurveShapeChanged(AvailableShapes.SelectedValue);
      }
    }

    private void EhCurveShapeChanged(Type newType)
    {
      var parameterDict = new Dictionary<string, double>();
      parameterDict.AddRange(ParametersOfCurve.Select(p => new KeyValuePair<string, double>(p.Name, p.Value)));

      var newFunction = (IFitFunction?)Activator.CreateInstance(newType) ?? throw new InvalidOperationException($"Can not create instance of class {newType}");
      if (newFunction is IFitFunctionPeak ffp1)
        newFunction = ffp1.WithNumberOfTerms(NumberOfTerms).WithOrderOfBaselinePolynomial(OrderOfBaselinePolynomial);
      else if (newFunction is Altaxo.Calc.FitFunctions.General.Polynomial po)
        newFunction = po.WithPolynomialOrder_PositiveExponents(OrderOfBaselinePolynomial);
      ParametersOfCurve.Clear();
      for (int i = 0; i < newFunction.NumberOfParameters; ++i)
      {
        var name = newFunction.ParameterName(i);
        double value = 0;
        if (!parameterDict.TryGetValue(name, out value))
          value = 0;
        ParametersOfCurve.Add(new ParameterItem { Name = name, Value = value });
      }

      object[] attribs = newType.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
      FitFunctionDescription = (attribs.Length == 0) ? string.Empty : StringParser.Parse(((System.ComponentModel.DescriptionAttribute)attribs[0]).Description);
    }

    public override bool Apply(bool disposeController)
    {
      if (IsExpressionString)
      {
        try
        {
          _doc = new ScalarFunctionDDExpression(ExpressionString);
        }
        catch (Exception ex)
        {
          Current.Gui.ErrorMessageBox($"Unable to successful parse the expression. Message:\r\n\r\n{ex.Message}");
          return ApplyEnd(false, disposeController);
        }
      }
      else if (IsPolynomial)
      {
        var f = new Altaxo.Calc.FitFunctions.General.Polynomial(OrderOfBaselinePolynomial, 0);
        _doc = new FitFunctionDDWrapper(f, ParametersOfCurve.Select(x => x.Value).ToArray());
      }
      else if (IsPeakFunction)
      {
        if (AvailableShapes.SelectedValue is null)
        {
          Current.Gui.ErrorMessageBox("Please select one of the peak functions!");
          return ApplyEnd(false, disposeController);
        }

        var curveInstance = (IFitFunction)Activator.CreateInstance(AvailableShapes.SelectedValue);
        if (curveInstance is IFitFunctionPeak ffp)
        {
          curveInstance = ffp.WithNumberOfTerms(NumberOfTerms).WithOrderOfBaselinePolynomial(OrderOfBaselinePolynomial);
          _doc = new FitFunctionDDWrapper(curveInstance, ParametersOfCurve.Select(x => x.Value).ToArray());
        }
      }


      return ApplyEnd(true, disposeController);
    }
  }
}
