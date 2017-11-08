#region Copyright

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

using System;
using System.Drawing;

namespace Altaxo.Graph.Plot.Data
{
    using Altaxo.Calc.Regression.Nonlinear;
    using Altaxo.Data;
    using Altaxo.Graph.Gdi;
    using Gdi.Plot.Data;

    /// <summary>
    /// Summary description for XYFunctionPlotData.
    /// </summary>
    [Serializable]
    public class XYNonlinearFitFunctionConfidenceIntervalPlotData :
        Main.SuspendableDocumentNodeWithSingleAccumulatedData<PlotItemDataChangedEventArgs>,
        Main.ICopyFrom,
        IXYFunctionPlotData
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
        /// Index of the the dependent variable of the fit element that is shown in this plot item.
        /// </summary>
        private int _dependentVariableIndex;

        /// <summary>
        /// If false, this function represents the upper confidence interval, if true, the lower confidence interval.
        /// </summary>
        private bool _isLowerConfidenceInterval;

        /// <summary>
        /// The covariance matrix of the fit parameters.
        /// </summary>
        private Altaxo.Calc.LinearAlgebra.DoubleMatrix _covarianceMatrix;

        // Cached values - don't serialize
        private double[] _cachedParameters;

        private double[] _cachedParametersForJacobianEvaluation;
        private double[] _cachedJacobian;
        private double[] _independentVariable = new double[1];
        private double[] _functionValues;
        private IFitFunction _cachedFitFunction;

        #region Serialization

        [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYNonlinearFitFunctionConfidenceIntervalPlotData), 0)]
        private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
        {
            public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
            {
                var s = (XYNonlinearFitFunctionConfidenceIntervalPlotData)obj;

                info.AddValue("FitDocumentIdentifier", s._fitDocumentIdentifier);
                info.AddValue("FitDocument", s._fitDocument);
                info.AddValue("FitElementIndex", s._fitElementIndex);
                info.AddValue("DependentVariableIndex", s._dependentVariableIndex);
            }

            public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
            {
                var s = null != o ? (XYNonlinearFitFunctionConfidenceIntervalPlotData)o : new XYNonlinearFitFunctionConfidenceIntervalPlotData();

                s._fitDocumentIdentifier = info.GetString("FitDocumentIdentifier");
                s.ChildSetMember(ref s._fitDocument, (NonlinearFitDocument)info.GetValue("FitDocument", s));
                s._fitElementIndex = info.GetInt32("FitElementIndex");
                s._dependentVariableIndex = info.GetInt32("DependentVariableIndex");

                return s;
            }
        }

        #endregion Serialization

        /// <summary>
        /// Only for deserialization purposes.
        /// </summary>
        protected XYNonlinearFitFunctionConfidenceIntervalPlotData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XYNonlinearFitFunctionConfidenceIntervalPlotData"/> class.
        /// </summary>
        /// <param name="fitDocumentIdentifier">The fit document identifier.</param>
        /// <param name="fitDocument">The fit document. The document will be cloned before stored in this instance.</param>
        /// <param name="fitElementIndex">Index of the fit element.</param>
        /// <param name="dependentVariableIndex">Index of the dependent variable of the fit element.</param>
        /// <param name="dependentVariableTransformation">Transformation, which is applied to the result of the fit function to be then shown in the plot. Can be null.</param>
        /// <param name="independentVariableIndex">Index of the independent variable of the fit element.</param>
        /// <param name="independentVariableTransformation">Transformation, which is applied to the x value before it is applied to the fit function. Can be null.</param>
        public XYNonlinearFitFunctionConfidenceIntervalPlotData(string fitDocumentIdentifier, NonlinearFitDocument fitDocument, int fitElementIndex, int dependentVariableIndex, IVariantToVariantTransformation dependentVariableTransformation, int independentVariableIndex, IVariantToVariantTransformation independentVariableTransformation)
        {
            if (null == fitDocumentIdentifier)
                throw new ArgumentNullException(nameof(fitDocumentIdentifier));
            if (null == fitDocument)
                throw new ArgumentNullException(nameof(fitDocument));

            ChildCloneToMember(ref _fitDocument, fitDocument); // clone here, because we want to have a local copy which can not change.
            _fitDocumentIdentifier = fitDocumentIdentifier;
            _fitElementIndex = fitElementIndex;
            _dependentVariableIndex = dependentVariableIndex;

            _cachedFitFunction = _fitDocument.FitEnsemble[fitElementIndex].FitFunction;
            _cachedParameters = _fitDocument.GetParametersForFitElement(_fitElementIndex);
            _cachedParametersForJacobianEvaluation = _fitDocument.GetParametersForFitElement(_fitElementIndex);
            _cachedJacobian = new double[_cachedParameters.Length];
            _functionValues = new double[_cachedFitFunction.NumberOfDependentVariables];
            //            Function = new FitFunctionToScalarFunctionDDWrapper(_fitDocument.FitEnsemble[fitElementIndex].FitFunction, dependentVariableIndex, dependentVariableTransformation, independentVariableIndex, independentVariableTransformation, _fitDocument.GetParametersForFitElement(fitElementIndex));
        }

        public XYNonlinearFitFunctionConfidenceIntervalPlotData(XYNonlinearFitFunctionConfidenceIntervalPlotData from)

        {
            CopyFrom(from);
        }

        public object Clone()
        {
            return new XYNonlinearFitFunctionConfidenceIntervalPlotData(this);
        }

        public bool CopyFrom(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (obj is XYNonlinearFitFunctionConfidenceIntervalPlotData from)
            {
                this._fitDocumentIdentifier = from._fitDocumentIdentifier;
                ChildCopyToMember(ref this._fitDocument, from._fitDocument);
                this._fitElementIndex = from._fitElementIndex;
                this._dependentVariableIndex = from._dependentVariableIndex;
                return true;
            }
            return false;
        }

        protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
        {
            if (null != _fitDocument)
                yield return new Main.DocumentNodeAndName(_fitDocument, () => _fitDocument = null, "FitDocument");
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
                return (NonlinearFitDocument)_fitDocument?.Clone();
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
        public IReadableColumn DependentVariableColumn
        {
            get
            {
                var fitEle = _fitDocument.FitEnsemble[FitElementIndex];
                return fitEle.DependentVariables(DependentVariableIndex);
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
            _independentVariable[0] = x;
            _cachedFitFunction.Evaluate(_independentVariable, parameters, _functionValues);
            return _functionValues[_dependentVariableIndex];
        }

        private const double DBL_EPSILON = 2.2204460492503131e-016;
        private const double SQRT_DBL_EPSILON = 1.490116119384765631426592E-8;

        private double EvaluateFunctionValueAndJacobian(double x, double[] jacobian)
        {
            double y = EvaluateFitFunctionValue(x, _cachedParameters);
            double eps = SQRT_DBL_EPSILON;

            for (int iParameter = 0; iParameter < _cachedParameters.Length; ++iParameter)
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
        public double Evaluate(double x)
        {
            var y = EvaluateFunctionValueAndJacobian(x, _cachedJacobian);

            // calculate derivation

            // calculate jacobian*CovarianceMatrix*jacobian
            double jacCovJac = 0;
            for (int iRow = 0; iRow < _cachedParameters.Length; ++iRow)
            {
                double sum = 0;
                for (int jCol = 0; jCol < _cachedParameters.Length; ++jCol)
                    sum += _covarianceMatrix[iRow, jCol] * _cachedJacobian[jCol];
                jacCovJac += _cachedJacobian[iRow] * sum;
            }

            return _isLowerConfidenceInterval ? y - jacCovJac : y + jacCovJac;
        }

        #endregion Function Evaluation

        #region GetRangesAndPoints

        private class MyPlotData
        {
            public double[] _xPhysical;
            public double[] _yPhysical;

            public Altaxo.Data.AltaxoVariant GetXPhysical(int originalRowIndex)
            {
                return _xPhysical[originalRowIndex];
            }

            public Altaxo.Data.AltaxoVariant GetYPhysical(int originalRowIndex)
            {
                return _yPhysical[originalRowIndex];
            }
        }

        /// <summary>
        /// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
        /// the function must have knowledge how to calculate the points out of the data. This will be done
        /// by a function provided by the calling function.
        /// </summary>
        /// <param name="layer">The plot layer.</param>
        /// <returns>An array of plot points in layer coordinates.</returns>
        public Processed2DPlotData GetRangesAndPoints(
            Gdi.IPlotArea layer)
        {
            const int functionPoints = 1000;
            const double MaxRelativeValue = 1E6;

            // allocate an array PointF to hold the line points
            PointF[] ptArray = new PointF[functionPoints];
            Processed2DPlotData result = new Processed2DPlotData();
            MyPlotData pdata = new MyPlotData();
            result.PlotPointsInAbsoluteLayerCoordinates = ptArray;
            double[] xPhysArray = new double[functionPoints];
            double[] yPhysArray = new double[functionPoints];
            pdata._xPhysical = xPhysArray;
            pdata._yPhysical = yPhysArray;
            result.XPhysicalAccessor = new IndexedPhysicalValueAccessor(pdata.GetXPhysical);
            result.YPhysicalAccessor = new IndexedPhysicalValueAccessor(pdata.GetYPhysical);

            // double xorg = layer.XAxis.Org;
            // double xend = layer.XAxis.End;
            // Fill the array with values
            // only the points where x and y are not NaNs are plotted!

            int i, j;

            bool bInPlotSpace = true;
            int rangeStart = 0;
            PlotRangeList rangeList = new PlotRangeList();
            result.RangeList = rangeList;
            Gdi.G2DCoordinateSystem coordsys = layer.CoordinateSystem;

            var xaxis = layer.XAxis;
            var yaxis = layer.YAxis;
            if (xaxis == null || yaxis == null)
                return null;

            for (i = 0, j = 0; i < functionPoints; i++)
            {
                double x_rel = ((double)i) / (functionPoints - 1);
                var x_variant = xaxis.NormalToPhysicalVariant(x_rel);
                double x = x_variant.ToDouble();
                double y = Evaluate(x);

                if (Double.IsNaN(x) || Double.IsNaN(y))
                {
                    if (!bInPlotSpace)
                    {
                        bInPlotSpace = true;
                        rangeList.Add(new PlotRange(rangeStart, j));
                    }
                    continue;
                }

                // double x_rel = layer.XAxis.PhysicalToNormal(x);
                double y_rel = yaxis.PhysicalVariantToNormal(y);

                // chop relative values to an range of about -+ 10^6
                if (y_rel > MaxRelativeValue)
                    y_rel = MaxRelativeValue;
                if (y_rel < -MaxRelativeValue)
                    y_rel = -MaxRelativeValue;

                // after the conversion to relative coordinates it is possible
                // that with the choosen axis the point is undefined
                // (for instance negative values on a logarithmic axis)
                // in this case the returned value is NaN
                double xcoord, ycoord;
                if (coordsys.LogicalToLayerCoordinates(new Logical3D(x_rel, y_rel), out xcoord, out ycoord))
                {
                    if (bInPlotSpace)
                    {
                        bInPlotSpace = false;
                        rangeStart = j;
                    }
                    xPhysArray[j] = x;
                    yPhysArray[j] = y;
                    ptArray[j].X = (float)xcoord;
                    ptArray[j].Y = (float)ycoord;
                    j++;
                }
                else
                {
                    if (!bInPlotSpace)
                    {
                        bInPlotSpace = true;
                        rangeList.Add(new PlotRange(rangeStart, j));
                    }
                }
            } // end for
            if (!bInPlotSpace)
            {
                bInPlotSpace = true;
                rangeList.Add(new PlotRange(rangeStart, j)); // add the last range
            }
            return result;
        }
    }

    #endregion GetRangesAndPoints
}