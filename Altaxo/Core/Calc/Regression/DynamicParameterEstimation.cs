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

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  public interface IDynamicParameterEstimationSolver
  {
    /// <summary>
    /// Solve the equation a*result=b in the least square sense, i.e. minimize the norm of (b-a*result).
    /// </summary>
    /// <param name="a">Matrix a.</param>
    /// <param name="b">Vector b.</param>
    /// <param name="result">Vector result, so that Norm(a*result-b) is minimized.</param>
    void Solve(JaggedArrayMatrix a, IReadOnlyList<double> b, IVector<double> result);
  }

  /// <summary>
  /// Algorithms to estimate the parameters of a dynamic difference equation.
  /// </summary>
  public class DynamicParameterEstimation
  {
    /// <summary>Number of y parameters to estimate.</summary>
    protected int _numY;

    /// <summary>Number of x parameters to estimate.</summary>
    protected int _numX;

    /// <summary>Number of background parameters to estimate.</summary>
    protected int _backgroundOrderPlus1;

    /// <summary>"Moves" the sequence x in relation to sequence y. Normally, the y[i] is considered in dependence on x[i], x[i-1].. and so on. By setting the offset,
    /// the y[i] is considered in dependence on x[i-offset], x[x-offset-1]...</summary>
    protected int _offsetX;

    /// <summary>
    /// Array to store the estimated parameters. First in the array, the x parameters are stored (indices 0.._numX-1).
    /// Then the y parameters, having indices of (_numX.._numX+_numY-1). Lastly, the background parameters are stored in the array
    /// (indices _numX+_numY ... end_of_array)
    /// </summary>
    protected double[] _parameter;

    /// <summary>Holds the input matrix.</summary>
    protected JaggedArrayMatrix _inputMatrix;

    /// <summary>Total number of parameter, i.e. _numX+_numY + _backgroundOrderPlus1</summary>
    protected int _numberOfParameter;

    /// <summary>Index of the point where the calculation can start.</summary>
    protected int _startingPoint;

    /// <summary>Array of y-values neccessary for backsubstitution. Is a copy of the input y vector, but
    /// only for the elements _startingPoint...end_of_y_vector.</summary>
    protected double[] _scaledY;

    /// <summary>Stores an instance of a solver used to solve the linear equation. The solver
    /// should keep and recycle the memory neccessary for solving the equation.</summary>
    protected IDynamicParameterEstimationSolver _solver;

    /// <summary>
    /// Constructur for the dynamic parameter estimation.
    /// </summary>
    /// <param name="numX">Number of history x samples to be taken into account for the parameter estimation (samples x[i], x[i-1]..x[i+1-numX]).</param>
    /// <param name="numY">Number of history y samples to be taken into account for the parameter estimation (samples y[i-1], y[i-2]..x[i-numY].</param>
    /// <param name="backgroundOrder">Order of the background fit, i.e. components 1, i, i² are fitted additionally to x and y. A order of 0 fits a constant
    /// background, 1 a linear depencence, and so on. Set this parameter to -1 if you don't need a background fit.</param>
    public DynamicParameterEstimation(int numX, int numY, int backgroundOrder)
      : this(numX, numY, backgroundOrder, SVDSolver)
    {
      SetHelperMembers(numX, numY, backgroundOrder);
    }

    /// <summary>
    /// Constructur for the dynamic parameter estimation.
    /// </summary>
    /// <param name="numX">Number of history x samples to be taken into account for the parameter estimation (samples x[i], x[i-1]..x[i+1-numX]).</param>
    /// <param name="numY">Number of history y samples to be taken into account for the parameter estimation (samples y[i-1], y[i-2]..x[i-numY].</param>
    /// <param name="backgroundOrder">Order of the background fit, i.e. components 1, i, i² are fitted additionally to x and y. A order of 0 fits a constant
    /// background, 1 a linear depencence, and so on. Set this parameter to -1 if you don't need a background fit.</param>
    /// <param name="solver">The solver to use with dynamic parameter estimation. Use the static getter methods <see cref="SVDSolver" /> or <see cref="LUSolver" /> to get a solver.</param>
    public DynamicParameterEstimation(int numX, int numY, int backgroundOrder, IDynamicParameterEstimationSolver solver)
    {
      if (solver == null)
        throw new ArgumentNullException("solver must not be null");

      _solver = solver;
      SetHelperMembers(numX, numY, backgroundOrder);
    }

    /// <summary>
    /// Calculates the dynamic parameter estimation in the constructor.
    /// </summary>
    /// <param name="x">Vector of x values ("input values"). This parameter can be null (in this case only y values are taken into account).</param>
    /// <param name="y">Vector of y values ("response values").</param>
    /// <param name="numX">Number of history x samples to be taken into account for the parameter estimation (samples x[i], x[i-1]..x[i+1-numX]).</param>
    /// <param name="numY">Number of history y samples to be taken into account for the parameter estimation (samples y[i-1], y[i-2]..x[i-numY].</param>
    /// <param name="backgroundOrder">Order of the background fit, i.e. components 1, i, i² are fitted additionally to x and y. A order of 0 fits a constant
    /// background, 1 a linear depencence, and so on. Set this parameter to -1 if you don't need a background fit.</param>
    public DynamicParameterEstimation(IReadOnlyList<double> x, IReadOnlyList<double> y, int numX, int numY, int backgroundOrder)
    {
      _solver = SVDSolver;
      SetHelperMembers(numX, numY, backgroundOrder);
      MakeEstimation(x, y);
    }

    /// <summary>
    /// Returns a fresh instance of a singular value decomposition solver that can be used for
    /// parameter estimation.
    /// </summary>
    public static IDynamicParameterEstimationSolver SVDSolver
    {
      get
      {
        return new DpeSVDSolver();
      }
    }

    /// <summary>
    /// Returns a fresh instance of a LU decomposition solver that can be used for
    /// parameter estimation.
    /// </summary>
    public static IDynamicParameterEstimationSolver LUSolver
    {
      get
      {
        return new DpeLUSolver();
      }
    }

    /// <summary>
    /// Returns a fresh instance of a QR decomposition solver that can be used for
    /// parameter estimation.
    /// </summary>
    public static IDynamicParameterEstimationSolver QRSolver
    {
      get
      {
        return new DpeQRSolver();
      }
    }

    /// <summary>"Moves" the sequence x in relation to sequence y. Normally, the y[i] is considered in dependence on x[i], x[i-1].. and so on. By setting the offset,
    /// the y[i] is considered in dependence on x[i-offset], x[x-offset-1]...</summary>
    public int OffsetX
    {
      get
      {
        return _offsetX;
      }
      set
      {
        _offsetX = value;
      }
    }

    /// <summary>
    /// Calculates the dynamic parameter estimation.
    /// </summary>
    /// <param name="x">Vector of x values ("input values"). This parameter can be null when numX was set to 0 (in this case only y values are taken into account).</param>
    /// <param name="y">Vector of y values ("response values").</param>
    public void MakeEstimation(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      CalculateStartingPoint();
      CalculateNumberOfData(x, y);
      _inputMatrix = FillInputMatrix(x, y, _inputMatrix);
      FillBacksubstitutionY(y);
      CalculateResultingParameter();
    }

    /// <summary>
    /// Sets all helper values such as _numX, _numY, _backgroundOrderPlus1, _numberOfParameter, _startingPoint.
    /// </summary>
    /// <param name="numX"></param>
    /// <param name="numY"></param>
    /// <param name="backgroundOrder"></param>
    protected virtual void SetHelperMembers(int numX, int numY, int backgroundOrder)
    {
      _numX = numX;
      _numY = numY;
      _backgroundOrderPlus1 = 1 + Math.Max(-1, backgroundOrder);
      _numberOfParameter = _numX + _numY + _backgroundOrderPlus1;

      // where to start the calculation (index of first y point that can be used)
      CalculateStartingPoint();
    }

    /// <summary>
    /// Calculates the starting point, i.e. the first index in the y array that can be used for
    /// the right side of the linear equation. The starting point increase when more x or y parameters
    /// are to evaluate, since more "history" samples are needed in this case.
    /// </summary>
    protected virtual void CalculateStartingPoint()
    {
      if (_numX == 0)
        _startingPoint = _numY;
      else
        _startingPoint = Math.Max(_offsetX + _numX - 1, _numY);
    }

    /// <summary>
    /// Calculates and returns the starting point, i.e. the first index in the y array that can be used for
    /// the right side of the linear equation. The starting point increase when more x or y parameters
    /// are to evaluate, since more "history" samples are needed in this case.
    /// </summary>
    public int StartingPoint
    {
      get
      {
        CalculateStartingPoint();
        return _startingPoint;
      }
    }

    /// <summary>
    /// Calculate the number of points that can be used at the right side of the linear equation (i.e. the number of rows of the equation).
    /// With the same length of x and y, the number of data reduces when more x or y parameters are to evaluate,
    /// since more samples are needed for the history and that samples can not be used on the right side of the equation.
    /// </summary>
    /// <param name="x">Vector of x data.</param>
    /// <param name="y">Vector of y data.</param>
    /// <returns></returns>
    protected virtual int CalculateNumberOfData(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      // Find the number of data to use.
      if (_numX > 0)
        return Math.Min(x.Count, y.Count) - _startingPoint;
      else
        return y.Count - _startingPoint;
    }

    /// <summary>
    /// Fills the input matrix, i.e. the left side of the linear equation.
    /// </summary>
    /// <param name="x">Vector of x data.</param>
    /// <param name="y">Vector of y data.</param>
    /// <param name="M">Matrix to fill. If the dimensions are not appropriate, a new matrix is allocated and stored in the member _inputMatrix.</param>
    /// <returns></returns>
    protected virtual JaggedArrayMatrix FillInputMatrix(IReadOnlyList<double> x, IReadOnlyList<double> y, JaggedArrayMatrix M)
    {
      int numberOfData = CalculateNumberOfData(x, y);
      if (M == null || M.RowCount != numberOfData || M.ColumnCount != _numberOfParameter)
        M = new JaggedArrayMatrix(numberOfData, _numberOfParameter);

      // Fill the matrix
      for (int i = 0; i < numberOfData; i++)
      {
        int yIdx = i + _startingPoint;

        // fill with x history samples
        for (int j = 0; j < _numX; j++)
        {
          M[i, j] = x[yIdx - j - _offsetX];
        }

        // fill with y history samples
        for (int j = 0; j < _numY; j++)
        {
          M[i, j + _numX] = y[yIdx - 1 - j];
        }

        // fill with polynomial background component
        double background = 1;
        for (int j = 0; j < _backgroundOrderPlus1; j++)
        {
          M[i, j + _numX + _numY] = background;
          background *= yIdx;
        }
      }
      return M;
    }

    /// <summary>
    /// Fills the back substitution array with data from the data of the provided y vector.
    /// </summary>
    /// <param name="y"></param>
    protected virtual void FillBacksubstitutionY(IReadOnlyList<double> y)
    {
      // Fill the y - neccessary later for backsubstitution
      int numberOfData = _inputMatrix.RowCount;
      if (_scaledY == null || _scaledY.Length != numberOfData)
        _scaledY = new double[numberOfData];
      for (int i = 0; i < numberOfData; i++)
        _scaledY[i] = y[i + _startingPoint];
    }

    /// <summary>
    /// Calculates the resulting parameter array by calling the solver.
    /// </summary>
    protected virtual void CalculateResultingParameter()
    {
      // allocate parameter array
      if (_parameter == null || _parameter.Length != _numberOfParameter)
        _parameter = new double[_numberOfParameter];

      _solver.Solve(_inputMatrix, VectorMath.ToROVector(_scaledY), VectorMath.ToVector(_parameter));
    }

    /// <summary>
    /// Calculates the mean prediction error, i.e. Sqrt(Sum((y-yprediced)²)/N).
    /// </summary>
    /// <returns>The mean prediction error.i.e. Sqrt(Sum((y-yprediced)²)/N).</returns>
    public virtual double CalculatePredictionError()
    {
      return CalculatePredictionError(null);
    }

    /// <summary>
    /// Calculates the mean prediction error, i.e. Sqrt(Sum((y-yprediced)²)).
    /// </summary>
    /// <param name="predictedOutput">The resultant predicted output. If null, a temporary vector is allocated for calculation.</param>
    /// <returns>The mean prediction error.i.e. Sqrt(Sum((y-yprediced)²)/N).</returns>
    public virtual double CalculatePredictionError(IVector<double> predictedOutput)
    {
      if (null == predictedOutput)
        predictedOutput = new DoubleVector(_inputMatrix.RowCount);

      MatrixMath.Multiply(_inputMatrix, VectorMath.ToROVector(_parameter), predictedOutput);

      double sumsquareddifferences = VectorMath.SumOfSquaredDifferences(VectorMath.ToROVector(_scaledY), predictedOutput);
      return Math.Sqrt(sumsquareddifferences / _inputMatrix.RowCount);
    }

    /// <summary>
    /// Calculates the mean prediction error, i.e. Sqrt(Sum((y-yprediced)²)).
    /// </summary>
    /// <returns>The mean prediction error.i.e. Sqrt(Sum((y-yprediced)²)/N).</returns>
    public virtual double CalculateSelfPredictionError()
    {
      return CalculateSelfPredictionError(null);
    }

    public virtual double CalculateSelfPredictionError(IVector<double> predictedOutput)
    {
      return CalculateSelfPredictionError(_inputMatrix, VectorMath.ToROVector(_scaledY), predictedOutput);
    }

    protected virtual double CalculateSelfPredictionError(IMatrix<double> inputMatrix, IReadOnlyList<double> yCompare, IVector<double> predictedOutput)
    {
      double[] inputVector = new double[_numY];
      for (int i = 0; i < inputVector.Length; i++)
        inputVector[i] = inputMatrix[0, _numX + i];

      double sumsquareddifferences = 0;
      int nRows = inputMatrix.RowCount;
      int nCols = inputMatrix.ColumnCount;
      for (int yIdx = 0; yIdx < nRows; yIdx++)
      {
        double ypred = 0;
        for (int i = 0; i < _numX; i++)
          ypred += inputMatrix[yIdx, i] * _parameter[i];
        for (int i = 0; i < _numY; i++)
          ypred += inputVector[i] * _parameter[_numX + i]; // Calculate y-predicted
        for (int i = _numX + _numY; i < nCols; i++)
          ypred += inputMatrix[yIdx, i] * _parameter[i];

        if (predictedOutput != null)
          predictedOutput[yIdx] = ypred;

        sumsquareddifferences += RMath.Pow2(ypred - yCompare[yIdx]);

        for (int i = _numY - 1; i > 0; --i)
          inputVector[i] = inputVector[i - 1]; // shift the y sequence aback, and
        inputVector[0] = ypred; //  replace the most recent y history sample by ypred
      }

      return Math.Sqrt(sumsquareddifferences / nRows);
    }

    /// <summary>
    /// With the already evalulated parameters (!), calculates the mean error for another piece of data. Please not
    /// that both vectors must have a length of at least _startingPoint+1, since the first _startingPoint samples are used for the history.
    /// </summary>
    /// <param name="x">Vector of x data.</param>
    /// <param name="y">Vector of y data.</param>
    /// <returns>The mean error between y prediction values and actual y values.</returns>
    public virtual double CalculateCrossPredictionError(IReadOnlyList<double> x, IReadOnlyList<double> y)
    {
      return CalculateCrossPredictionError(x, y, null);
    }

    /// <summary>
    /// With the already evalulated parameters (!), calculates the mean error for another piece of data. Please not
    /// that both vectors must have a length of at least _startingPoint+1, since the first _startingPoint samples are used for the history.
    /// </summary>
    /// <param name="x">Vector of x data.</param>
    /// <param name="y">Vector of y data.</param>
    /// <param name="predictedOutput">Vector to store the predicted y values. Can be null.</param>
    /// <returns>The mean error between y prediction values and actual y values.</returns>
    public virtual double CalculateCrossPredictionError(IReadOnlyList<double> x, IReadOnlyList<double> y, IVector<double> predictedOutput)
    {
      JaggedArrayMatrix m = FillInputMatrix(x, y, null);
      return CalculateSelfPredictionError(m, VectorMath.ToROVector(y, _startingPoint, y.Count - _startingPoint), predictedOutput);
    }

    /// <summary>
    /// Calculates the frequency response for a given frequency.
    /// </summary>
    /// <param name="fdt">Frequency. Must be given as f*dt, meaning the product of frequency and sample interval.</param>
    /// <returns>The complex frequency response at the given frequency.</returns>
    public virtual Complex GetFrequencyResponse(double fdt)
    {
      double w = fdt * 2 * Math.PI;
      var nom = new Complex();
      for (int i = 0; i < _numX; i++)
      {
        nom += Complex.FromModulusArgument(_parameter[i], -i * w);
      }

      var denom = new Complex();
      for (int i = 0; i < _numY; i++)
      {
        denom += Complex.FromModulusArgument(_parameter[_numX + i], -(i + 1) * w);
      }

      return nom / (1 - denom);
    }

    /// <summary>
    /// Resulting parameters of the estimation. Index 0..numX-1 are the parameters for x history. Following from numX
    /// to numX+numY-1 are the parameters for y, and at least there are the parameters for the background fit.
    /// </summary>
    public double[] Parameter
    {
      get
      {
        return _parameter;
      }
    }

    /// <summary>
    /// Gets the impulse response to a pulse at t=0, i.e. to x[0]==1, x[1]...x[n]==0. The background component is not taken into account.
    /// </summary>
    /// <param name="yValueBeforePulse">This is the y-value (not x!) before the pulse. If the <c>NumberOfY</c> is set to zero, this parameter is ignored, since no information about y for t&lt;0 is neccessary.</param>
    /// <param name="output">Used to store the output result. Can be of arbitrary size.</param>
    public virtual void GetTransferFunction(double yValueBeforePulse, IVector<double> output)
    {
      double[] y = new double[_numY];

      // Initialization
      for (int i = 0; i < _numY; i++)
        y[i] = yValueBeforePulse;

      for (int i = 0; i < output.Length; i++)
      {
        int ioffs = i - _offsetX;
        double sum = 0;
        if (ioffs >= 0 && ioffs < _numX)
          sum = _parameter[ioffs]; // this is the contribution of x

        for (int j = 0; j < _numY; j++)
          sum += _parameter[j + _numX] * y[j];

        // right-shift both y
        for (int j = _numY - 1; j > 0; j--)
          y[j] = y[j - 1];

        // and set the actual values
        if (_numY > 0)
          y[0] = sum;

        output[i] = sum;
      }
    }

    public void EstimateParameterByBurgsAlgorithm(IReadOnlyList<double> data, int m, out double xms)
    {
      int n = data.Count;
      double[] wk1 = new double[data.Count];
      double[] wk2 = new double[data.Count];
      double[] wkm = new double[data.Count];

      double p = 0;
      for (int j = 0; j < n; ++j)
        p += RMath.Pow2(data[j]);
      xms = p / n;

      wk1[0] = data[0];
      wk2[n - 2] = data[n - 1];
      for (int j = 1; j < (n - 1); ++j)
      {
        wk1[j] = data[j];
        wk2[j - 1] = data[j];
      }
      for (int k = 0; k < m; ++k)
      {
        double num = 0;
        double denom = 0;
        for (int j = 0; j < (n - k - 1); ++j)
        {
          num += wk1[j] * wk2[j];
          denom += RMath.Pow2(wk1[j]) + RMath.Pow2(wk2[j]);
        }
        _parameter[k] = 2 * num / denom;

        xms *= (1 - RMath.Pow2(_parameter[k]));

        for (int i = 0; i < k; ++i)
          _parameter[i] = wkm[i] - _parameter[k] * wkm[k - i];

        if (k == (m - 1))
          return;

        for (int i = 0; i <= k; ++i)
        {
          wkm[i] = _parameter[i];
        }
        for (int j = 0; j < (n - k - 2); ++j)
        {
          wk1[j] -= wkm[k] * wk2[j]; // subtract actual prediction
          wk2[j] = wk2[j + 1] - wkm[k] * wk1[j + 1]; // left shift with subtracted actual prediction
        }
      }
      throw new ArithmeticException("Should never be here, this is a programming error");
    }

    /// <summary>
    /// Extrapolates y-values until the end of the vector by using linear prediction.
    /// </summary>
    /// <param name="yTraining">Input vector of y values used to calculated the prediction coefficients.
    /// <param name="yPredValues">Input/output vector of y values to extrapolate.
    /// The fields beginning from 0 to <c>len-1</c> must contain valid values used for initialization of the extrapolation.
    /// At the end of the procedure, the upper end (<c>len</c> .. <c>yPredValues.Count-1</c> contain the
    /// extrapolated data.</param>
    /// </param>
    /// <param name="len">Number of valid input data points for extrapolation (not for the training data!).</param>
    /// <param name="yOrder">Number of history samples used for prediction. Must be greater or equal to 1.</param>
    public static DynamicParameterEstimation Extrapolate(IReadOnlyList<double> yTraining, IVector<double> yPredValues, int len, int yOrder)
    {
      if (yOrder < 1)
        throw new ArgumentException("yOrder must be at least 1");
      if (yOrder >= (yTraining.Count - yOrder))
        throw new ArgumentException("Not enough data points for this degree (yOrder must be less than yTraining.Length/2).");

      var est = new DynamicParameterEstimation(null, yTraining, 0, yOrder, 0);
      // now calculate the extrapolation data

      for (int i = len; i < yPredValues.Length; i++)
      {
        double sum = 0;
        for (int j = 0; j < yOrder; j++)
        {
          sum += yPredValues[i - j - 1] * est._parameter[j];
        }
        yPredValues[i] = sum;
      }
      return est;
    }

    #region Solver classes

    private class DpeSVDSolver : IDynamicParameterEstimationSolver
    {
      /// <summary>
      /// The singular value composition of our data.
      /// </summary>
      private MatrixMath.SingularValueDecomposition _decomposition;

      #region IDynamicParameterEstimationSolver Members

      public void Solve(JaggedArrayMatrix a, IReadOnlyList<double> b, IVector<double> result)
      {
        IMatrix<double> work = new JaggedArrayMatrix(a.RowCount, a.ColumnCount);
        _decomposition = MatrixMath.GetSingularValueDecomposition(a, work);
        _decomposition.Backsubstitution(b, result);
      }

      #endregion IDynamicParameterEstimationSolver Members
    }

    private class DpeLUSolver : IDynamicParameterEstimationSolver
    {
      #region IDynamicParameterEstimationSolver Members

      public void Solve(JaggedArrayMatrix a, IReadOnlyList<double> b, IVector<double> result)
      {
        var outputMatrix = new JaggedArrayMatrix(a.ColumnCount, a.ColumnCount);
        JaggedArrayMath.MultiplyFirstTransposedWithItself(a.Array, a.RowCount, a.ColumnCount, outputMatrix.Array, outputMatrix.RowCount, outputMatrix.ColumnCount);

        var decomp = new DoubleLUDecomp(outputMatrix);
        decomp.Compute();

        double[] ycov = new double[a.ColumnCount];
        MatrixMath.MultiplyFirstTransposed(a, b, VectorMath.ToVector(ycov));

        DoubleVector dresult = decomp.Solve(VectorMath.ToROVector(ycov));
        for (int i = 0; i < result.Length; i++)
          result[i] = dresult[i];
      }

      #endregion IDynamicParameterEstimationSolver Members
    }

    private class DpeQRSolver : QRDecomposition, IDynamicParameterEstimationSolver
    {
      public void Solve(JaggedArrayMatrix a, IReadOnlyList<double> b, IVector<double> result)
      {
        base.Solve(a, b, result);
      }
    }

    #endregion Solver classes
  }

  /// <summary>
  /// Dynamic parameter estimation with variable spaced x input.
  /// </summary>
  public class DynamicParameterEstimationVariableX : DynamicParameterEstimation
  {
    private int[] _xcount;
    private int[] _ycount;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="xcount">Designate which x points are processed together as one unit. The array must have the length of _numX. For every x parameter, the element in the array
    /// designates how many points of the x history are added up and threated as one input element. If you set all elements to 1, the behaviour should
    /// be the same than GetTransferFunction.</param>
    /// <param name="numY">Number of y points used for the history.</param>
    /// <param name="backgroundOrder">Order of the background fitting.</param>
    public DynamicParameterEstimationVariableX(int[] xcount, int numY, int backgroundOrder)
      : base(xcount.Length, numY, backgroundOrder)
    {
      _xcount = xcount;
      _ycount = GetUniformCountArray(numY, 1);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="xcount">Designate which x points are processed together as one unit. The array must have the length of _numX. For every x parameter, the element in the array
    /// designates how many points of the x history are added up and threated as one input element. If you set all elements to 1, the behaviour should
    /// be the same than GetTransferFunction.</param>
    /// <param name="numY">Number of y points used for the history.</param>
    /// <param name="backgroundOrder">Order of the background fitting.</param>
    /// <param name="solver">The solver to use with dynamic parameter estimation. Use the static getter methods <see cref="P:SVDSolver" /> or <see cref="P:LUSolver" /> to get a solver.</param>
    public DynamicParameterEstimationVariableX(int[] xcount, int numY, int backgroundOrder, IDynamicParameterEstimationSolver solver)
      : base(xcount.Length, numY, backgroundOrder, solver)
    {
      _xcount = xcount;
      _ycount = GetUniformCountArray(numY, 1);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="xcount">Designate which x points are processed together as one unit. The array must have the length of _numX. For every x parameter, the element in the array
    /// designates how many points of the x history are added up and threated as one input element. If you set all elements to 1, the behaviour should
    /// be the same than GetTransferFunction.</param>
    /// <param name="ycount">Designate which y points are processed together as one unit. The array must have the length of _numX. For every x parameter, the element in the array
    /// designates how many points of the x history are added up and threated as one input element. If you set all elements to 1, the behaviour should
    /// be the same than GetTransferFunction.</param>
    /// <param name="backgroundOrder">Order of the background fitting.</param>
    /// <param name="solver">The solver to use with dynamic parameter estimation. Use the static getter methods <see cref="P:SVDSolver" /> or <see cref="P:LUSolver" /> to get a solver.</param>
    public DynamicParameterEstimationVariableX(int[] xcount, int[] ycount, int backgroundOrder, IDynamicParameterEstimationSolver solver)
      : base(xcount.Length, ycount.Length, backgroundOrder, solver)
    {
      _xcount = xcount;
      _ycount = ycount;
    }

    protected override void CalculateStartingPoint()
    {
      base.CalculateStartingPoint();
      if (_numX > 0 && _xcount != null)
      {
        if (_xcount.Length != _numX)
          throw new ArgumentException("Length of xcount is not equal to _numX");

        // Find starting point
        int xsum = 0;
        for (int i = 0; i < _xcount.Length; i++)
          xsum += _xcount[i];

        _startingPoint = Math.Max(_startingPoint, xsum - 1);
      }

      if (_numY > 0 && _ycount != null)
      {
        if (_ycount.Length != _numY)
          throw new ArgumentException("Length of ycount is not equal to _numY");

        // Find starting point
        int ysum = 0;
        for (int i = 0; i < _ycount.Length; i++)
          ysum += _ycount[i];

        _startingPoint = Math.Max(_startingPoint, ysum);
      }
    }

    protected override JaggedArrayMatrix FillInputMatrix(IReadOnlyList<double> x, IReadOnlyList<double> y, JaggedArrayMatrix M)
    {
      int numberOfData = CalculateNumberOfData(x, y);
      if (M == null || M.RowCount != numberOfData || M.ColumnCount != _numberOfParameter)
        M = new JaggedArrayMatrix(numberOfData, _numberOfParameter);

      // Fill the matrix
      for (int i = 0; i < numberOfData; i++)
      {
        int yIdx = i + _startingPoint;

        // fill with x history samples
        for (int j = 0, k = 0; j < _numX; j++)
        {
          if (_xcount == null || _xcount[j] == 1)
          {
            M[i, j] = x[yIdx - k - _offsetX];
            k++;
          }
          else
          {
            double sum = 0;
            for (int l = _xcount[j]; l > 0; l--, k++)
              sum += x[yIdx - k - _offsetX];
            M[i, j] = sum;
          }
        }

        // fill with y history samples
        for (int j = 0, k = 0; j < _numY; j++)
        {
          if (_ycount == null || _ycount[j] == 1)
          {
            M[i, j + _numX] = y[yIdx - 1 - k];
            k++;
          }
          else
          {
            double sum = 0;
            for (int l = _ycount[j]; l > 0; l--, k++)
              sum += y[yIdx - 1 - k];
            M[i, j + _numX] = sum;
          }
        }

        // fill with polynomial background component
        double background = 1;
        for (int j = 0; j < _backgroundOrderPlus1; j++)
        {
          M[i, j + _numX + _numY] = background;
          background *= yIdx;
        }
      }
      return M;
    }

    /// <summary>
    /// Gets the impulse response to a pulse at t=0, i.e. to x[0]==1, x[1]...x[n]==0. The background component is not taken into account.
    /// </summary>
    /// <param name="output">Used to store the output result. Can be of arbitrary size.</param>
    /// <param name="yValueBeforePulse">This is the y-value (not x!) before the pulse. If the <c>NumberOfY</c> is set to zero, this parameter is ignored, since no information about y for t&lt;0 is neccessary.</param>
    public override void GetTransferFunction(double yValueBeforePulse, IVector<double> output)
    {
      int maxXidx = 0;
      for (int i = 0; i < _xcount.Length; ++i)
        maxXidx += _xcount[i];

      int maxYidx = 0;
      for (int i = 0; i < _ycount.Length; ++i)
        maxYidx += _ycount[i];

      double[] y = new double[maxYidx];

      // Initialization
      for (int i = 0; i < y.Length; i++)
        y[i] = yValueBeforePulse;

      int currXcountIdx = -1;
      int sumXcount = 0;
      for (int i = 0; i < output.Length; i++)
      {
        double sum = 0;
        int ioffs = i - _offsetX;
        if (ioffs >= 0 && ioffs < maxXidx)
        {
          if (ioffs >= sumXcount)
          {
            ++currXcountIdx;
            sumXcount += _xcount[currXcountIdx];
          }
          sum += _parameter[currXcountIdx]; // this is the contribution of x
        }

        for (int j = 0, sumYCount = 0; j < _numY; j++)
          for (int l = _ycount[j]; l > 0; --l, ++sumYCount)
            sum += _parameter[j + _numX] * y[sumYCount];

        // right-shift both y
        for (int j = _numY - 1; j > 0; j--)
          y[j] = y[j - 1];

        // and set the actual values
        if (_numY > 0)
          y[0] = sum;

        output[i] = sum;
      }
    }

    public override Complex GetFrequencyResponse(double fdt)
    {
      double w = fdt * 2 * Math.PI;
      var nom = new Complex();
      for (int i = 0, j = 0; i < _numX; i++)
      {
        for (int k = _xcount[i]; k > 0; --k, ++j)
          nom += Complex.FromModulusArgument(_parameter[i], -j * w);
      }

      var denom = new Complex();
      for (int i = 0, j = 0; i < _numY; i++)
      {
        for (int k = _ycount[i]; k > 0; --k, ++j)
          denom += Complex.FromModulusArgument(_parameter[_numX + i], -(j + 1) * w);
      }

      return nom / (1 - denom);
    }

    public static int[] GetUniformCountArray(int numPara, int count)
    {
      int[] result = new int[numPara];
      for (int i = 0; i < result.Length; i++)
        result[i] = count;
      return result;
    }

    public static int[] GetXCountFromTotalLength(int totallength, int start, double factor)
    {
      if (start < 0)
        throw new ArgumentException("start must not be negative");

      if (totallength < start)
        throw new ArgumentException("totallength must not be smaller than start");

      var list = new List<int>();
      double scaledfactor = factor / (1 + factor);
      int curridx = totallength;
      for (; curridx > 0;)
      {
        double delements = (scaledfactor * (curridx - start));
        delements = Math.Min(delements, curridx);
        delements = Math.Max(delements, 1);

        if (delements == 1)
        {
          list.Add(1);
          curridx--;
        }
        else if (delements == curridx)
        {
          list.Add(curridx);
          curridx = 0;
        }
        else
        {
          int current_down = curridx - (int)Math.Ceiling(delements);
          int current_up = curridx - (int)Math.Floor(delements);

          int elements_down = (int)Math.Floor((current_down - start) * factor);
          int elements_up = (int)Math.Floor((current_up - start) * factor);

          if (current_down + elements_down == curridx)
          {
            list.Add(elements_down);
            curridx -= elements_down;
          }
          else
          {
            list.Add(elements_up);
            curridx -= elements_up;
          }
        }
      }
      if (!(curridx == 0))
        throw new InvalidProgramException();
      list.Reverse();
      return list.ToArray();
    }

    public static int[] GetXCountFromNumberOfParameters(int numX, int start, double factor)
    {
      int[] result = new int[numX];
      GetXCountFromNumberOfParameters(numX, start, factor, result);
      return result;
    }

    public static int GetXCountFromNumberOfParameters(int numX, int start, double factor, int[] result)
    {
      if (start < 0)
        throw new ArgumentException("start must not be negative");

      if (numX < start)
        throw new ArgumentException("numX must not be smaller than start");

      if (factor < 0)
        throw new ArgumentException("factor must not be negative");

      if (result.Length != numX)
        throw new ArgumentException("result.Length must be equal to numX");

      int current = 0;
      for (int i = 0; i < numX; i++)
      {
        int nelements = (int)Math.Floor((current - start) * factor);
        nelements = Math.Max(nelements, 1);

        if ((((long)current) + nelements) > int.MaxValue)
          break;

        result[i] = nelements;
        current += nelements;
      }
      return current;
    }

    public static int[] GetXCountFromNumberOfParametersAndLength(int numX, int start, int totallength)
    {
      int len = totallength;
      return GetXCountFromNumberOfParametersAndLength(numX, start, ref len, out var factor);
    }

    public static int[] GetXCountFromNumberOfParametersAndLength(int numX, int start, ref int totallength, out double factor)
    {
      if (start < 0)
        throw new ArgumentException("start must not be negative");

      if (numX < start)
        throw new ArgumentException("numX must not be smaller than start");

      if (totallength < numX)
        throw new ArgumentException("totallength must not be smaller than numX");

      int[] result = new int[numX];

      // search the parameter space from factor=0 to factor=1
      double lowerfactor = 0;
      int lowerresult = numX;

      if (lowerresult == totallength)
      {
        factor = lowerfactor;
        return GetXCountFromNumberOfParameters(numX, start, lowerfactor);
      }

      double upperfactor = 0;
      int upperresult = numX;
      for (upperfactor = 1; upperresult < totallength; upperfactor += 1)
      {
        upperresult = GetXCountFromNumberOfParameters(numX, start, upperfactor, result);
      }

      if (upperresult == totallength)
      {
        factor = upperfactor;
        return result;
      }

      double middlefactor = 0;
      int middleresult = numX;
      for (; (upperfactor - lowerfactor) > (upperfactor * DoubleConstants.DBL_EPSILON);)
      {
        middlefactor = 0.5 * (upperfactor + lowerfactor);
        middleresult = GetXCountFromNumberOfParameters(numX, start, middlefactor, result);
        if (middleresult == totallength)
        {
          factor = middlefactor;
          return result;
        }
        else
        {
          if (middleresult < totallength)
          {
            lowerfactor = middlefactor;
            lowerresult = middleresult;
          }
          else
          {
            upperfactor = middlefactor;
            upperresult = middleresult;
          }
        }
      }
      if (middleresult == lowerresult)
      {
        middlefactor = upperfactor;
        middleresult = GetXCountFromNumberOfParameters(numX, start, middlefactor, result);
      }

      // now decrease
      int decreaseby = middleresult - totallength;

      do
      {
        for (int i = 0; decreaseby > 0 && i < result.Length; i++)
        {
          if (result[i] > 1)
          {
            result[i]--;
            decreaseby--;
          }
        }
      } while (decreaseby > 0);

      factor = middlefactor;
      return result;
    }

    /// <summary>
    /// Gets an array where the first history point (y[0]) is excluded from the history (this is because y[0] is already on the right side of the equation.
    /// If the first element of xcount is greater than one, the result is a cloned xcount array with the first element reduced by one.
    /// If the first element of xcount is one, than an copyied version of xcount, without the first element, is returned.
    /// </summary>
    /// <param name="xcount">The xcount array.</param>
    /// <returns>A version of count suitable for use with the y history.</returns>
    public static int[] GetYCountFromXCount(int[] xcount)
    {
      if (xcount == null)
        throw new ArgumentNullException("xcount");
      if (xcount.Length == 0)
        throw new ArgumentException("Length of xcount is 0");
      if (xcount[0] < 1)
        throw new ArgumentException("First element of xcout array must be greater than zero");

      int[] ycount;
      if (xcount[0] > 1)
      {
        ycount = (int[])xcount.Clone();
        ycount[0]--;
      }
      else
      {
        ycount = new int[xcount.Length - 1];
        Array.Copy(xcount, 1, ycount, 0, ycount.Length);
      }
      return ycount;
    }
  }

  /// <summary>
  /// Dynamic parameter estimation with comb like spaced x and y input.
  /// </summary>
  public class DynamicParameterEstimationCombXY : DynamicParameterEstimation
  {
    private int _xSpacing;
    private int _ySpacing;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="numX">Number of x parameters to evaluate.</param>
    /// <param name="xSpacing">Interval between two x history points. The value must be greater than or equal to 1.</param>
    /// <param name="numY">Number of y parameters to evaluate.</param>
    /// <param name="ySpacing">Interval between two subsequent y history points. The value must be greater than or equal to 1.</param>
    /// <param name="backgroundOrder">Order of the background fitting.</param>
    public DynamicParameterEstimationCombXY(int numX, int xSpacing, int numY, int ySpacing, int backgroundOrder)
      : base(numX, numY, backgroundOrder)
    {
      _xSpacing = xSpacing;
      _ySpacing = ySpacing;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="numX">Number of x parameters to evaluate.</param>
    /// <param name="xSpacing">Interval between two x history points. The value must be greater or equal to 1.</param>
    /// <param name="numY">Number of y parameters to evaluate.</param>
    /// <param name="ySpacing">Interval between two subsequent y history points. The value must be greater or equal to 1.</param>
    /// <param name="backgroundOrder">Order of the background fitting.</param>
    /// <param name="solver">The solver to use with dynamic parameter estimation. Use the static getter methods <see cref="P:SVDSolver" /> or <see cref="P:LUSolver" /> to get a solver.</param>
    public DynamicParameterEstimationCombXY(int numX, int xSpacing, int numY, int ySpacing, int backgroundOrder, IDynamicParameterEstimationSolver solver)
      : base(numX, numY, backgroundOrder, solver)
    {
      _xSpacing = xSpacing;
      _ySpacing = ySpacing;
    }

    protected override void CalculateStartingPoint()
    {
      base.CalculateStartingPoint();
      if (_numX > 0)
        _startingPoint = Math.Max(_startingPoint, _offsetX + (_numX - 1) * _xSpacing);

      if (_numY > 0)
        _startingPoint = Math.Max(_startingPoint, 1 + (_numY - 1) * _ySpacing);
    }

    protected override JaggedArrayMatrix FillInputMatrix(IReadOnlyList<double> x, IReadOnlyList<double> y, JaggedArrayMatrix M)
    {
      int numberOfData = CalculateNumberOfData(x, y);
      if (M == null || M.RowCount != numberOfData || M.ColumnCount != _numberOfParameter)
        M = new JaggedArrayMatrix(numberOfData, _numberOfParameter);

      // Fill the matrix
      for (int i = 0; i < numberOfData; i++)
      {
        int yIdx = i + _startingPoint;

        // fill with x history samples
        for (int j = 0; j < _numX; j++)
        {
          M[i, j] = x[yIdx - _offsetX - j * _xSpacing];
        }

        // fill with y history samples
        for (int j = 0; j < _numY; j++)
        {
          M[i, j + _numX] = y[yIdx - 1 - j * _ySpacing];
        }

        // fill with polynomial background component
        double background = 1;
        for (int j = 0; j < _backgroundOrderPlus1; j++)
        {
          M[i, j + _numX + _numY] = background;
          background *= yIdx;
        }
      }
      return M;
    }

    /// <summary>
    /// Gets the impulse response to a pulse at t=0, i.e. to x[0]==1, x[1]...x[n]==0. The background component is not taken into account.
    /// </summary>
    /// <param name="output">Used to store the output result. Can be of arbitrary size.</param>
    /// <param name="yValueBeforePulse">This is the y-value (not x!) before the pulse. If the <c>NumberOfY</c> is set to zero, this parameter is ignored, since no information about y for t&lt;0 is neccessary.</param>
    public override void GetTransferFunction(double yValueBeforePulse, IVector<double> output)
    {
      double[] y = new double[_numY * _ySpacing];

      // Initialization
      for (int i = 0; i < y.Length; i++)
        y[i] = yValueBeforePulse;

      for (int i = 0; i < output.Length; i++)
      {
        double sum = 0;

        if (0 == i % _xSpacing)
          sum += _parameter[i / _xSpacing]; // this is the contribution of x

        for (int j = 0; j < _numY; j++)
          sum += _parameter[j + _numX] * y[j * _ySpacing];

        // right-shift both y
        for (int j = y.Length - 1; j > 0; j--)
          y[j] = y[j - 1];

        // and set the actual values
        if (_numY > 0)
          y[0] = sum;

        output[i] = sum;
      }
    }

    public override Complex GetFrequencyResponse(double fdt)
    {
      double w = fdt * 2 * Math.PI;
      var nom = new Complex();
      for (int i = 0; i < _numX; i++)
      {
        nom += Complex.FromModulusArgument(_parameter[i], -i * _xSpacing * w);
      }

      var denom = new Complex();
      for (int i = 0; i < _numY; i++)
      {
        denom += Complex.FromModulusArgument(_parameter[_numX + i], -(1 + i * _ySpacing) * w);
      }

      return nom / (1 - denom);
    }
  }

  public class DynamicParameterEstimationWithChooseableBins : DynamicParameterEstimation
  {
    private int[] _xBins;
    private int[] _yBins;
    private int _maxXBin;
    private int _maxYBin;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicParameterEstimationWithChooseableBins"/> class. This will regress an linear equation
    /// <c>y_i = a_0*x_i-xb0 + a_1*x_i-xb1 + ... +  a_n*x_i-xbn + b_0*y_i-yb0 + b_1*y_i-yb1 + ... + b_m*y_i-ybm</c>.
    /// The xb0 .. xbn are called the xBins. They are the lags for the x-Values and range from 0 .. Infinity.
    /// the yb0 .. ybn are called the yBins. They are the lags of the y-Values and range from 1 .. Infinity.
    /// </summary>
    /// <param name="xBins">The x bins.</param>
    /// <param name="yBins">The y bins.</param>
    /// <param name="backgroundOrder">The background order. Use -1 if you want to fit without background. A value of 0 already designates a constant background, a value of 1 a linear background.</param>
    public DynamicParameterEstimationWithChooseableBins(int[] xBins, int[] yBins, int backgroundOrder)
      : base(xBins.Length, yBins.Length, backgroundOrder)
    {
      _xBins = (int[])xBins.Clone();
      _yBins = (int[])yBins.Clone();

      // do some parameter testing
      if (_xBins.Length >= 1)
      {
        if (_xBins[0] < 0)
          throw new ArgumentOutOfRangeException(string.Format("xBin[0] must be greater than or equal to zero, but actually is {0}", _xBins[0]));

        for (int i = 1; i < _xBins.Length; ++i)
        {
          if (_xBins[i] <= _xBins[i - 1])
            throw new ArgumentOutOfRangeException(string.Format("xBins[{0}]={1} is less than or equal to xBin[{2}]={3}", i, _xBins[i], i - 1, _xBins[i - 1]));
        }
      }

      if (_yBins.Length >= 1)
      {
        if (_yBins[0] < 1)
          throw new ArgumentOutOfRangeException(string.Format("yBin[0] must be greater than or equal to one, but actually is: {0}", _yBins[0]));

        for (int i = 1; i < _yBins.Length; ++i)
        {
          if (_yBins[i] <= _yBins[i - 1])
            throw new ArgumentOutOfRangeException(string.Format("yBins[{0}]={1} is less than or equal to yBin[{2}]={3}", i, _xBins[i], i - 1, _xBins[i - 1]));
        }
      }

      _maxXBin = _xBins.Length > 0 ? _xBins[_xBins.Length - 1] : 0;

      _maxYBin = _yBins.Length > 0 ? _yBins[_yBins.Length - 1] : 0;
    }

    protected override void CalculateStartingPoint()
    {
      base.CalculateStartingPoint();
      if (_numX > 0)
        _startingPoint = Math.Max(_startingPoint, _offsetX + _maxXBin);

      if (_numY > 0)
        _startingPoint = Math.Max(_startingPoint, _maxYBin);
    }

    protected override JaggedArrayMatrix FillInputMatrix(IReadOnlyList<double> x, IReadOnlyList<double> y, JaggedArrayMatrix M)
    {
      int numberOfData = CalculateNumberOfData(x, y);
      if (M == null || M.RowCount != numberOfData || M.ColumnCount != _numberOfParameter)
        M = new JaggedArrayMatrix(numberOfData, _numberOfParameter);

      // Fill the matrix
      for (int i = 0; i < numberOfData; ++i)
      {
        int yIdx = i + _startingPoint;

        // fill with x history samples
        for (int j = 0; j < _numX; j++)
        {
          M[i, j] = x[yIdx - _offsetX - _xBins[j]];
        }

        // fill with y history samples
        for (int j = 0; j < _numY; j++)
        {
          M[i, j + _numX] = y[yIdx - _yBins[j]];
        }

        // fill with polynomial background component
        double background = 1;
        for (int j = 0; j < _backgroundOrderPlus1; j++)
        {
          M[i, j + _numX + _numY] = background;
          background *= yIdx;
        }
      }
      return M;
    }

    /// <summary>
    /// Gets the impulse response to a pulse at t=0, i.e. to x[0]==1, x[1]...x[n]==0. The background component is not taken into account.
    /// </summary>
    /// <param name="output">Used to store the output result. Can be of arbitrary size.</param>
    /// <param name="yValueBeforePulse">This is the y-value (not x!) before the pulse. If the <c>NumberOfY</c> is set to zero, this parameter is ignored, since no information about y for t&lt;0 is neccessary.</param>
    public override void GetTransferFunction(double yValueBeforePulse, IVector<double> output)
    {
      throw new NotImplementedException("Getting the transfer function doesn't make much sense here, since the bins can be non-continuous. The purpose of this class is mainly to get the frequency response.");
    }

    public override Complex GetFrequencyResponse(double fdt)
    {
      double w = fdt * 2 * Math.PI;
      var nom = new Complex();
      for (int i = 0; i < _numX; i++)
      {
        nom += Complex.FromModulusArgument(_parameter[i], -w * _xBins[i]);
      }

      var denom = new Complex();
      for (int i = 0; i < _numY; i++)
      {
        denom += Complex.FromModulusArgument(_parameter[_numX + i], -w * _yBins[i]);
      }

      return nom / (1 - denom);
    }
  }
}
