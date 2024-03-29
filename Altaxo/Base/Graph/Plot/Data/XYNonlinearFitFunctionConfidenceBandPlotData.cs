﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Graph.Plot.Data
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Calc.LinearAlgebra;
  using Altaxo.Calc.Regression.Nonlinear;
  using Altaxo.Data;

  /// <summary>
  /// Summary description for XYFunctionPlotData.
  /// </summary>
  [Serializable]
  public class XYNonlinearFitFunctionConfidenceBandPlotData : XYFunctionPlotDataBase
  {
    /// <summary>
    /// A Guid string that is identical for all fit function elements with the same fit document.
    /// </summary>
    protected string _fitDocumentIdentifier;

    /// <summary>The nonlinear fit this function belongs to.</summary>
    private NonlinearFitDocument _fitDocument;

    /// <summary>Index of the fit element this function belongs to.</summary>
    private int _fitElementIndex;

    /// <summary>
    /// Index of the the independent variable of the fit element that is shown in this plot item.
    /// </summary>
    private int _independentVariableIndex;

    private IVariantToVariantTransformation? _independentVariableTransformation;

    /// <summary>
    /// Index of the the dependent variable of the fit element that is shown in this plot item.
    /// </summary>
    private int _dependentVariableIndex;

    private IVariantToVariantTransformation? _dependentVariableTransformation;

    /// <summary>
    /// The number of fit points. Used to calculate the quantile of the student's distribution.
    /// </summary>
    private int _numberOfFitPoints;

    private double _sigmaSquare;

    /// <summary>
    /// The covariance matrix of the fit parameters.
    /// </summary>
    private double[,] _covarianceMatrix;

    /// <summary>
    /// If false, this function represents the upper confidence interval, if true, the lower confidence interval.
    /// </summary>
    public bool IsLowerBand { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is prediction band. If it is a prediction band, then the value of sigmaSquare
    /// is added to the confidence band.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is prediction band; otherwise, <c>false</c>.
    /// </value>
    public bool IsPredictionBand { get; protected set; }

    private double _confidenceLevel = 0.95;

    // Cached values - don't serialize
    private double[] _cachedParameters;

    private double[] _cachedParametersForJacobianEvaluation;
    private double[] _cachedJacobian;
    private double[] _independentVariable = new double[1];
    private double[] _functionValues;
    private IFitFunction? _cachedFitFunction;
    private double _cachedQuantileOfStudentsDistribution;

    /// <summary>
    /// The indices of the parameters of this fitelement, which are varying.
    /// </summary>
    private int[] _cachedIndicesOfVaryingParametersOfThisFitElement;

    #region Serialization

    /// <summary>
    /// Initial version, 2017-11-09.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYNonlinearFitFunctionConfidenceBandPlotData), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (XYNonlinearFitFunctionConfidenceBandPlotData)obj;

        info.AddValue("FitDocumentIdentifier", s._fitDocumentIdentifier);
        info.AddValue("FitDocument", s._fitDocument);
        info.AddValue("FitElementIndex", s._fitElementIndex);
        info.AddValue("IndependentVariableIndex", s._dependentVariableIndex);
        info.AddValueOrNull("IndependentVariableTransformation", s._independentVariableTransformation);
        info.AddValue("DependentVariableIndex", s._dependentVariableIndex);
        info.AddValueOrNull("DependentVariableTransformation", s._dependentVariableTransformation);
        info.AddValue("NumberOfFitPoints", s._numberOfFitPoints);
        info.AddValue("SigmaSquare", s._sigmaSquare);

        info.AddValue("IsLowerBand", s.IsLowerBand);
        info.AddValue("IsPredictionBand", s.IsPredictionBand);
        info.AddValue("ConfidenceLevel", s._confidenceLevel);

        {
          info.CreateArray("Covariances", s._cachedIndicesOfVaryingParametersOfThisFitElement.Length * s._cachedIndicesOfVaryingParametersOfThisFitElement.Length);
          foreach (var i in s._cachedIndicesOfVaryingParametersOfThisFitElement)
            foreach (var j in s._cachedIndicesOfVaryingParametersOfThisFitElement)
              info.AddValue("e", s._covarianceMatrix[i, j]);
          info.CommitArray();
        }
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (XYNonlinearFitFunctionConfidenceBandPlotData?)o ?? new XYNonlinearFitFunctionConfidenceBandPlotData(info);

        s._fitDocumentIdentifier = info.GetString("FitDocumentIdentifier");
        s.ChildSetMember(ref s._fitDocument, (NonlinearFitDocument)info.GetValue("FitDocument", s));
        s._fitElementIndex = info.GetInt32("FitElementIndex");
        s._independentVariableIndex = info.GetInt32("IndependentVariableIndex");
        s._independentVariableTransformation = info.GetValueOrNull<IVariantToVariantTransformation>("IndependentVariableTransformation", null);
        s._dependentVariableIndex = info.GetInt32("DependentVariableIndex");
        s._dependentVariableTransformation = info.GetValueOrNull<IVariantToVariantTransformation>("DependentVariableTransformation", null);
        s._numberOfFitPoints = info.GetInt32("NumberOfFitPoints");
        s._sigmaSquare = info.GetDouble("SigmaSquare");
        s.IsLowerBand = info.GetBoolean("IsLowerBand");
        s.IsPredictionBand = info.GetBoolean("IsPredictionBand");
        s._confidenceLevel = info.GetDouble("ConfidenceLevel");

        s.CreateCachedMembers();

        { // Deserialize covariances
          int count = info.OpenArray("Covariances");
          if (!(count == s._cachedIndicesOfVaryingParametersOfThisFitElement.Length * s._cachedIndicesOfVaryingParametersOfThisFitElement.Length))
            throw new InvalidOperationException("Number of elements in covariance array does not match the number of varying parameters");

          foreach (var i in s._cachedIndicesOfVaryingParametersOfThisFitElement)
            foreach (var j in s._cachedIndicesOfVaryingParametersOfThisFitElement)
              s._covarianceMatrix[i, j] = info.GetDouble("e");
          info.CloseArray(count);
        }

        return s;
      }
    }

    #endregion Serialization

    #region Construction and Copying

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected XYNonlinearFitFunctionConfidenceBandPlotData(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.



    [MemberNotNull(nameof(_cachedParameters), nameof(_cachedParametersForJacobianEvaluation), nameof(_cachedJacobian), nameof(_functionValues), nameof(_covarianceMatrix), nameof(_cachedIndicesOfVaryingParametersOfThisFitElement))]
    private (List<string> allVaryingParameterNames, int[] indicesOfThisFitElementsVaryingParametersInAllVaryingParameters) CreateCachedMembers()
    {
      var fitElement = _fitDocument.FitEnsemble[_fitElementIndex];

      _cachedFitFunction = fitElement.FitFunction;
      _cachedParameters = _fitDocument.GetParametersForFitElement(_fitElementIndex);
      _cachedParametersForJacobianEvaluation = _fitDocument.GetParametersForFitElement(_fitElementIndex);
      _cachedJacobian = new double[_cachedParameters.Length];
      _functionValues = new double[fitElement.NumberOfDependentVariables];

      // CovarianceMatrix: we have to pick exactly the varying parameters of this fitelement!

      // next line retrieves all varying parameters of all fitelements, this corresponds to the rows of the provided covariance matrix
      var allVaryingParameterNames = new List<string>(_fitDocument.CurrentParameters.Where(x => x.Vary).Select(x => x.Name));

      // Indices of the varying parameters of this fit element (with respect to the parameter set of this fitelement)
      _cachedIndicesOfVaryingParametersOfThisFitElement = Enumerable.Range(0, fitElement.NumberOfParameters).Where(i => allVaryingParameterNames.IndexOf(fitElement.ParameterName(i)) >= 0).ToArray();

      // Indices in allVaryingParameters of the parameters in this fitlement, which are varying
      var indicesOfThisFitElementsVaryingParametersInAllVaryingParameters = Enumerable.Range(0, _cachedIndicesOfVaryingParametersOfThisFitElement.Length).Select(i => allVaryingParameterNames.IndexOf(fitElement.ParameterName(_cachedIndicesOfVaryingParametersOfThisFitElement[i]))).ToArray();
      // now we are able to pick the values out of the covariance matrix

      if (!(_numberOfFitPoints > _cachedIndicesOfVaryingParametersOfThisFitElement.Length))
        _cachedQuantileOfStudentsDistribution = double.NaN; // 0 degrees of freedom
      else
        // for a given confidence interval, e.g. 0.95, we need to make 0.975 out of it
        _cachedQuantileOfStudentsDistribution = Altaxo.Calc.Probability.StudentsTDistribution.Quantile(1 - (0.5 * (1 - _confidenceLevel)), _numberOfFitPoints - _cachedIndicesOfVaryingParametersOfThisFitElement.Length);

      // for convenience, we let this covariance matrix to be of dimension NumberOfParameters x NumberOfParameters, but we
      // don't set the elements corresponding to the non-varying parameters, thus they remain zero
      _covarianceMatrix = new double[fitElement.NumberOfParameters, fitElement.NumberOfParameters];

      return (allVaryingParameterNames, indicesOfThisFitElementsVaryingParametersInAllVaryingParameters);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XYNonlinearFitFunctionConfidenceBandPlotData"/> class.
    /// </summary>
    /// <param name="isPredictionBand">If true, the prediction band is displayed instead of the confidence band. The prediction band is a little wider, because the sigma of the data points give an additional contribution.</param>
    /// <param name="isLowerBand">True if this data present the lower confidence (or prediction) band; true if the data represent the upper confidence (or prediction) band.</param>
    /// <param name="confidenceLevel">A number greater than 0 and less than 1 representing the confidence level. Usual values are e.g. 0.95, 0.99, 0.999.</param>
    /// <param name="fitDocumentIdentifier">The fit document identifier.</param>
    /// <param name="fitDocument">The fit document. The document will be cloned before stored in this instance.</param>
    /// <param name="fitElementIndex">Index of the fit element.</param>
    /// <param name="dependentVariableIndex">Index of the dependent variable of the fit element.</param>
    /// <param name="dependentVariableTransformation">Transformation, which is applied to the result of the fit function to be then shown in the plot. Can be null.</param>
    /// <param name="independentVariableIndex">Index of the independent variable of the fit element.</param>
    /// <param name="independentVariableTransformation">Transformation, which is applied to the x value before it is applied to the fit function. Can be null.</param>
    /// <param name="numberOfFittedPoints">Number of points that were used for fitting. Needed to calculate the Student's distribution quantile.</param>
    /// <param name="sigmaSquare">Mean square difference between data points and fitting curve = sumChiSquare/(n-r).</param>
    /// <param name="covarianceMatrixTimesSigmaSquare">A matrix, representing sigma²(A*At)^-1, which are the covariances of the parameter.</param>
    public XYNonlinearFitFunctionConfidenceBandPlotData(
        bool isPredictionBand,
        bool isLowerBand,
        double confidenceLevel,
        string fitDocumentIdentifier,
        NonlinearFitDocument fitDocument,
        int fitElementIndex,
        int dependentVariableIndex,
        IVariantToVariantTransformation dependentVariableTransformation,
        int independentVariableIndex,
        IVariantToVariantTransformation independentVariableTransformation,
        int numberOfFittedPoints,
        double sigmaSquare,
        Matrix<double> covarianceMatrixTimesSigmaSquare)
    {
      if (fitDocumentIdentifier is null)
        throw new ArgumentNullException(nameof(fitDocumentIdentifier));
      if (fitDocument is null)
        throw new ArgumentNullException(nameof(fitDocument));
      if (!(confidenceLevel > 0 && confidenceLevel < 1))
        throw new ArgumentOutOfRangeException("Confidence level must be > 0 and < 1", nameof(confidenceLevel));
      if (!(sigmaSquare >= 0))
        throw new ArgumentOutOfRangeException("SigmaSquare must be >=0", nameof(sigmaSquare));

      IsLowerBand = isLowerBand;
      _confidenceLevel = confidenceLevel;
      _sigmaSquare = sigmaSquare;
      ChildCloneToMember(ref _fitDocument, fitDocument); // clone here, because we want to have a local copy which can not change.
      _fitDocumentIdentifier = fitDocumentIdentifier;
      _fitElementIndex = fitElementIndex;
      _dependentVariableIndex = dependentVariableIndex;
      _dependentVariableTransformation = dependentVariableTransformation;
      _independentVariableTransformation = independentVariableTransformation;
      _numberOfFitPoints = numberOfFittedPoints;

      var (allVaryingParameterNames, indicesOfThisFitElementsVaryingParametersInAllVaryingParameters) = CreateCachedMembers();

      // the covariance matrix should have the dimensions of allVaryingParameters.Count x allVaryingParameters.Count
      if (!(covarianceMatrixTimesSigmaSquare.RowCount == allVaryingParameterNames.Count && covarianceMatrixTimesSigmaSquare.ColumnCount == allVaryingParameterNames.Count))
        throw new InvalidProgramException("Covariance matrix dimension does not match with number of varying parameters");

      for (int i = 0; i < _cachedIndicesOfVaryingParametersOfThisFitElement.Length; ++i)
      {
        int iRowOriginalCovMat = indicesOfThisFitElementsVaryingParametersInAllVaryingParameters[i];
        int iRowThisCovMat = _cachedIndicesOfVaryingParametersOfThisFitElement[i];

        for (int j = 0; j < _cachedIndicesOfVaryingParametersOfThisFitElement.Length; ++j)
        {
          int jColOriginalCovMat = indicesOfThisFitElementsVaryingParametersInAllVaryingParameters[j];
          int jColThisCovMat = _cachedIndicesOfVaryingParametersOfThisFitElement[j];

          _covarianceMatrix[iRowThisCovMat, jColThisCovMat] = covarianceMatrixTimesSigmaSquare[iRowOriginalCovMat, jColOriginalCovMat];
        }
      }
    }

    public override object Clone()
    {
      return new XYNonlinearFitFunctionConfidenceBandPlotData(this);
    }

    public XYNonlinearFitFunctionConfidenceBandPlotData(XYNonlinearFitFunctionConfidenceBandPlotData from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_fitDocumentIdentifier), nameof(_fitDocument), nameof(_cachedParameters), nameof(_cachedParametersForJacobianEvaluation), nameof(_cachedJacobian), nameof(_functionValues), nameof(_covarianceMatrix), nameof(_cachedIndicesOfVaryingParametersOfThisFitElement))]
    protected void CopyFrom(XYNonlinearFitFunctionConfidenceBandPlotData from)
    {
      _fitDocumentIdentifier = from._fitDocumentIdentifier;
      ChildCopyToMember(ref _fitDocument, from._fitDocument);
      _fitElementIndex = from._fitElementIndex;
      _independentVariableIndex = from._independentVariableIndex;
      _independentVariableTransformation = from._independentVariableTransformation;
      _dependentVariableIndex = from._dependentVariableIndex;
      _dependentVariableTransformation = from._dependentVariableTransformation;
      _numberOfFitPoints = from._numberOfFitPoints;
      _sigmaSquare = from._sigmaSquare;

      IsLowerBand = from.IsLowerBand;
      IsPredictionBand = from.IsPredictionBand;
      _confidenceLevel = from._confidenceLevel;
      CreateCachedMembers();
      // covariance matrix must be cloned after CreateCachedMembers, because it's allocated there
      _covarianceMatrix = (double[,])from._covarianceMatrix.Clone();
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (!base.CopyFrom(obj))
        return false;

      if (obj is XYNonlinearFitFunctionConfidenceBandPlotData from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    #endregion Construction and Copying

    public override string ToString()
    {
      return string.Format("NLFit {0} {1} band {2}%, FitElement: {3}, DependentVariable: {4}", IsLowerBand ? "lower" : "upper", IsPredictionBand ? "prediction" : "confidence", 100 * _confidenceLevel, _fitElementIndex, _dependentVariableIndex);
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_fitDocument is not null)
        yield return new Main.DocumentNodeAndName(_fitDocument, () => _fitDocument = null!, "FitDocument");
    }

    /// <summary>
    /// Gets a copy of the fit document.
    /// </summary>
    /// <value>
    /// The copy of the fit document.
    /// </value>
    public NonlinearFitDocument FitDocumentCopy
    {
      get
      {
        return (NonlinearFitDocument)_fitDocument.Clone();
      }
    }

    /// <summary>
    /// A Guid string that is identical for all fit function elements with the same fit document.
    /// </summary>
    public string FitDocumentIdentifier
    {
      get
      {
        return _fitDocumentIdentifier;
      }
    }

    /// <summary>Index of the fit element of the <see cref="FitDocumentCopy"/> this function belongs to.</summary>
    public int FitElementIndex { get { return _fitElementIndex; } }

    /// <summary>
    /// Index of the the dependent variable of the fit element that is shown in this plot item.
    /// </summary>
    public int DependentVariableIndex
    {
      get { return _dependentVariableIndex; }
    }

    /// <summary>
    /// Gets the dependent variable column.
    /// </summary>
    /// <value>
    /// The dependent variable column.
    /// </value>
    public IReadableColumn? DependentVariableColumn
    {
      get
      {
        var fitEle = _fitDocument.FitEnsemble[FitElementIndex];
        return fitEle.DependentVariables(DependentVariableIndex);
      }
    }

    public double ConfidenceLevel
    {
      get
      {
        return _confidenceLevel;
      }
      set
      {
        if (!(value > 0 && value < 1))
          throw new ArgumentOutOfRangeException("Value must be > 0 and < 1", nameof(value));

        if (!(_confidenceLevel == value))
        {
          _confidenceLevel = value;
          CreateCachedMembers();
        }
      }
    }

    #region Function Evaluation

    /// <summary>
    /// Evaluates the fit function value at value x. Attention! This is out <b>our</b> function value
    /// (because we do not display the fit function itself, but the confidence band).
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="parameters">The fit parameters.</param>
    /// <returns></returns>
    private double EvaluateFitFunctionValue(double x, double[] parameters)
    {
      if (_cachedFitFunction is { } fitFunction)
      {
        _independentVariable[0] = x;
        fitFunction.Evaluate(_independentVariable, parameters, _functionValues);
        return _functionValues[_dependentVariableIndex];
      }
      else
      {
        return double.NaN;
      }
    }

    private const double DBL_EPSILON = 2.2204460492503131e-016;
    private const double SQRT_DBL_EPSILON = 1.490116119384765631426592E-8;

    private double EvaluateFunctionValueAndJacobian(double x, double[] jacobian)
    {
      double y = EvaluateFitFunctionValue(x, _cachedParameters);
      if (double.IsNaN(y) || double.IsInfinity(y))
        return y; // jacobian needs not to be evaluated if y is not defined or infinity

      double eps = SQRT_DBL_EPSILON;

      foreach (var iParameter in _cachedIndicesOfVaryingParametersOfThisFitElement)
      {
        var parameter = _cachedParameters[iParameter];
        var h = eps * Math.Abs(parameter);
        if (h == 0.0)
          h = eps;
        _cachedParametersForJacobianEvaluation[iParameter] = parameter + h;
        try
        {
          var ydev = EvaluateFitFunctionValue(x, _cachedParametersForJacobianEvaluation);
          if (!double.IsNaN(ydev))
          {
            jacobian[iParameter] = (ydev - y) / h;
          }
          else // if right derivative fails, try left derivative
          {
            _cachedParametersForJacobianEvaluation[iParameter] = parameter - h;
            ydev = EvaluateFitFunctionValue(x, _cachedParametersForJacobianEvaluation);
            jacobian[iParameter] = (y - ydev) / h;
          }
        }
        catch (Exception)
        {
          jacobian[iParameter] = double.NaN;
        }
        finally
        {
          _cachedParametersForJacobianEvaluation[iParameter] = _cachedParameters[iParameter];
        }
      }

      return y;
    }

    /// <summary>
    /// Evaluates the function value at the specified independent variable value x.
    /// </summary>
    /// <param name="x">The value x of the independent variable.</param>
    /// <returns></returns>
    public override double Evaluate(double x)
    {
      if (_independentVariableTransformation is not null)
        x = _independentVariableTransformation.Transform(x);

      var y = EvaluateFunctionValueAndJacobian(x, _cachedJacobian);

      if (double.IsNaN(y))
        return y;
      if (double.IsNaN(_cachedQuantileOfStudentsDistribution))
        return double.NaN;

      // calculate derivation

      // calculate jacobian*CovarianceMatrixTimesSigma²*jacobian
      double jacCovJac = 0;

      foreach (var iRow in _cachedIndicesOfVaryingParametersOfThisFitElement)
      {
        double sum = 0;
        foreach (var jCol in _cachedIndicesOfVaryingParametersOfThisFitElement)
          sum += _covarianceMatrix[iRow, jCol] * _cachedJacobian[jCol];
        jacCovJac += _cachedJacobian[iRow] * sum;
      }

      // if this is a predition band, we must add sigmaSquare
      if (IsPredictionBand)
      {
        jacCovJac += _sigmaSquare;
      }

      var h = _cachedQuantileOfStudentsDistribution * Math.Sqrt(jacCovJac);
      var result = IsLowerBand ? y - h : y + h;

      if (_dependentVariableTransformation is not null)
        return _dependentVariableTransformation.Transform(result);
      else
        return result;
    }

    #endregion Function Evaluation
  }
}
