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

#nullable enable
using System;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Class for simple non linear fitting that can be used inside of scripts.
  /// </summary>
  public class SimpleNonlinearFit
  {
    #region Inner classes

    private class DummyFitFunc : IFitFunction
    {
      private FitEvaluationFunction _func;
      private double[] _defaultParameter;

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

      /// <summary>
      /// Not used here since this fit function never changed.
      /// </summary>
      public event EventHandler? Changed;

      protected virtual void OnChanged()
      {
        Changed?.Invoke(this, EventArgs.Empty);
      }

      #endregion IFitFunction Members
    }

    #endregion Inner classes

    private NonlinearFitDocument _fitDoc;
    private FitElement _fitEle;
    private LevMarAdapter? _fitAdapter;

    /*
    /// <summary>
    /// Creates an instance of this class.
    /// </summary>
    /// <param name="fitFunc">Fitting function.</param>
    /// <param name="parameter">Array of default parameters (length must match the expected number of parameter of fitFunc).</param>
    /// <param name="start">First point to be used for fitting.</param>
    /// <param name="count">Number of points to be used for fitting.</param>
    public SimpleNonlinearFit(FitEvaluationFunction fitFunc, double[] parameter, int start, int count)
      : this(fitFunc, parameter, null, 0, null, null, start, count)
    {
    }
    */
    /// <summary>
    /// Creates an instance of this class. This constructor needs either
    /// <paramref name="dataTable"/>, <paramref name="xCol"/> and <paramref name="yCol"/> to be valid, or all to be null.
    /// If all null, consider to use the other provided constructor.
    /// </summary>
    /// <param name="fitFunc">Fitting function.</param>
    /// <param name="parameter">Array of default parameters (length must match the expected number of parameter of fitFunc).</param>
    /// <param name="dataTable">The data table from which the provided <paramref name="xCol"/> and <paramref name="yCol"/> originate.</param>
    /// <param name="groupNumber">The group number of the columns <paramref name="xCol"/> and <paramref name="yCol"/>.</param>
    /// <param name="xCol">Data column of independent values.</param>
    /// <param name="yCol">Data column of dependent values.</param>
    /// <param name="start">First point to be used for fitting.</param>
    /// <param name="count">Number of points to be used for fitting.</param>
    public SimpleNonlinearFit(FitEvaluationFunction fitFunc, double[] parameter, Altaxo.Data.DataTable dataTable, int groupNumber, Altaxo.Data.INumericColumn xCol, Altaxo.Data.INumericColumn yCol, int start, int count)
    {
      _fitDoc = new NonlinearFitDocument();

      var fitFunction = new DummyFitFunc(fitFunc, parameter);
      if (dataTable is null && xCol is null && yCol is null)
        _fitEle = new FitElement(fitFunction, Altaxo.Data.Selections.RangeOfRowIndices.FromStartAndCount(start, count));
      else if (dataTable is not null && xCol is not null && yCol is not null)
        _fitEle = new FitElement(fitFunction, dataTable, groupNumber, Altaxo.Data.Selections.RangeOfRowIndices.FromStartAndCount(start, count), xCol, yCol);
      else
        throw new ArgumentException($"Either all three arguments {nameof(dataTable)}, {nameof(xCol)}, {nameof(yCol)} must be null or not null!");

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
    /// Gets the variance of parameter with index i.
    /// </summary>
    /// <param name="i">Index of the parameter.</param>
    /// <returns>Variance of the parameter i.</returns>
    public double GetParameterVariance(int i)
    {
      return _fitDoc.CurrentParameters[i].Variance;
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
        if (_fitAdapter is null)
          throw new InvalidOperationException($"Result not available yet. Please call {nameof(Fit)} before trying to access the result.");
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
