using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Class for simple non linear fitting that can be used inside of scripts.
  /// </summary>
  public class SimpleNonlinearFit
  {
    #region Inner classes

    class DummyFitFunc : IFitFunction
    {
      
      FitEvaluationFunction _func;
      double[] _defaultParameter;

      public DummyFitFunc(FitEvaluationFunction func, double[] defaultParameter)
      {
        _func = func;
        _defaultParameter = (double[])defaultParameter.Clone();
      }

      #region IFitFunction Members

      public int NumberOfIndependentVariables
      {
        get { return 1; }
      }

      public int NumberOfDependentVariables
      {
        get { return 1; }
      }

      public int NumberOfParameters
      {
        get { return _defaultParameter.Length; }
      }

      public string IndependentVariableName(int i)
      {
        return "x";
      }

      public string DependentVariableName(int i)
      {
        return "y";
      }

      public string ParameterName(int i)
      {
        return "P" + i.ToString();
      }

      public double DefaultParameterValue(int i)
      {
        return _defaultParameter[i];
      }

      public IVarianceScaling DefaultVarianceScaling(int i)
      {
        return new ConstantVarianceScaling();
      }

      public void Evaluate(double[] independent, double[] parameters, double[] FV)
      {
        _func(independent, parameters, FV);
      }

      #endregion
    }

    #endregion

    NonlinearFitDocument _fitDoc;
    FitElement _fitEle;
    LevMarAdapter _fitAdapter;

    /// <summary>
    /// Creates an instance of this class.
    /// </summary>
    /// <param name="fitFunc">Fitting function.</param>
    /// <param name="parameter">Array of default parameters (length must match the expected number of parameter of fitFunc).</param>
    /// <param name="xCol">Data column of independent values.</param>
    /// <param name="yCol">Data column of dependent values.</param>
    /// <param name="start">First point to be used for fitting.</param>
    /// <param name="count">Number of points to be used for fitting.</param>
    public SimpleNonlinearFit(FitEvaluationFunction fitFunc, double[] parameter, Altaxo.Data.INumericColumn xCol, Altaxo.Data.INumericColumn yCol, int start, int count)
    {
      _fitDoc = new NonlinearFitDocument();
      _fitEle = new FitElement(xCol, yCol, start, count);
      _fitEle.FitFunction = new DummyFitFunc(fitFunc, parameter);
      _fitDoc.FitEnsemble.Add(_fitEle);
      _fitDoc.SetDefaultParametersForFitElement(0);
    }

    /// <summary>
    /// Executes the fit. Afterwards, you can get the fit parameters by <see cref="GetParameter"/>, or the resulting Chi².
    /// </summary>
    public void Fit()
    {
      _fitAdapter = new LevMarAdapter(_fitDoc.FitEnsemble, _fitDoc.CurrentParameters);
      _fitAdapter.Fit();
      _fitAdapter.CopyParametersBackTo(_fitDoc.CurrentParameters);
    }

    /// <summary>
    /// Sets the values of the parameter back to their default value.
    /// </summary>
    public void ResetToDefaultParameters()
    {
      _fitDoc.SetDefaultParametersForFitElement(0);
    }

    /// <summary>
    /// Sets the parameter with index i to a certain value, that is used as starting value for fitting.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <param name="val">Value of the parameter.</param>
    public void SetParameter(int i, double val)
    {
      _fitDoc.CurrentParameters[i].Parameter = val;
    }

    /// <summary>
   /// Gets the parameter with index i.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>Value of the parameter i.</returns>
    public double GetParameter(int i)
    {
      return _fitDoc.CurrentParameters[i].Parameter;
    }

    /// <summary>
    /// Sets the parameter with index i to either fixed (parameter can't vary during fit) or unfixed.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <param name="paraFixed">If true, the parameter is fixed during fitting.</param>
    public void SetParameterIsFixed(int i, bool paraFixed)
    {
      _fitDoc.CurrentParameters[i].Vary = !paraFixed;
    }

    /// <summary>
    /// Gets the fixed/unfixed state of the parameter with index i.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>If true, the parameter is fixed during fitting.</returns>
    public bool GetParameterIsFixed(int i)
    {
      return !_fitDoc.CurrentParameters[i].Vary;
    }

    /// <summary>
    /// After a fit, returns the resulting Chi Square.
    /// </summary>
    public double ResultingChiSquare
    {
      get
      {
        return _fitAdapter.ResultingChiSquare;
      }
    }

    /// <summary>
    /// With these function is is possible to change the data column used for the independent variable.
    /// </summary>
    /// <param name="xCol">Data column representing the independent variable.</param>
    public void SetIndependentVariable(Altaxo.Data.INumericColumn xCol)
    {
      _fitEle.SetIndependentVariable(0, xCol);
    }

    /// <summary>
    /// With these function is is possible to change the data column used for the dependent variable.
    /// </summary>
    /// <param name="yCol">Data column representing the dependent variable.</param>
    public void SetDependentVariable(Altaxo.Data.INumericColumn yCol)
    {
      _fitEle.SetDependentVariable(0, yCol);
    }

  }
}
