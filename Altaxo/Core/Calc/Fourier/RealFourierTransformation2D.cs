#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Fourier
{
  /// <summary>
  /// Class to assist in executing a 2D Fourier transform on data originating from a matrix of real values.
  /// </summary>
  public class RealFourierTransformation2D
  {
    protected IMatrix<double> _realMatrix;

    protected IMatrix<double>? _imagMatrix;

    protected Action<IMatrix<double>>? _pretreatment;

    protected double? _columnSpacing;

    protected double? _rowSpacing;

    // Working members

    #region Working members

    /// <summary><c>True</c> if the FFT was executed, and the arrays contain already the transformed values.</summary>
    private bool _arraysContainTransformation;

    #endregion Working members

    /// <summary>
    /// Initializes a new instance of the <see cref="RealFourierTransformation2D"/> class.
    /// </summary>
    /// <param name="sourceMatrix">The source matrix. This member is mandatory. Before you call <see cref="Execute"/>, you can set the other properties of this class as you like.</param>
    /// <param name="allowOverwriting">If <c>true</c>, the provided matrix is allowed to be modified by this instance. If <c>false</c>, a local copy of the provided matrix will be created.</param>
    /// <exception cref="System.ArgumentNullException">SourceMatrix must not be null</exception>
    public RealFourierTransformation2D(IMatrix<double> sourceMatrix, bool allowOverwriting)
    {
      if (sourceMatrix is null)
        throw new ArgumentNullException("SourceMatrix must not be null");

      if (sourceMatrix.RowCount < 2 || sourceMatrix.ColumnCount < 2)
        throw new ArgumentException("SourceMatrix must at least have the dimensions 2x2");

      if (allowOverwriting && (sourceMatrix is IMatrixInArray1DRowMajorRepresentation<double>))
      {
        _realMatrix = sourceMatrix;
      }
      else
      {
        _realMatrix = new DoubleMatrixInArray1DRowMajorRepresentation(sourceMatrix.RowCount, sourceMatrix.ColumnCount);
        MatrixMath.Copy(sourceMatrix, _realMatrix);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RealFourierTransformation2D"/> class.
    /// </summary>
    /// <param name="sourceMatrix">The source matrix. This member is mandatory. Before you call <see cref="Execute"/>, you can set the other properties of this class as you like.</param>
    /// <exception cref="System.ArgumentNullException">SourceMatrix must not be null</exception>
    public RealFourierTransformation2D(IROMatrix<double> sourceMatrix)
    {
      if (sourceMatrix is null)
        throw new ArgumentNullException("SourceMatrix must not be null");

      if (sourceMatrix.RowCount < 2 || sourceMatrix.ColumnCount < 2)
        throw new ArgumentException("SourceMatrix must at least have the dimensions 2x2");

      _realMatrix = new DoubleMatrixInArray1DRowMajorRepresentation(sourceMatrix.RowCount, sourceMatrix.ColumnCount);
      MatrixMath.Copy(sourceMatrix, _realMatrix);
    }

    #region Input Properties

    /// <summary>
    /// Registering point for data pretreatment actions. All actions that are registered here will be executed immediately before the Fourier transformation.
    /// The provided matrix is the matrix of the values to transform.
    /// </summary>
    public event Action<IMatrix<double>> DataPretreatment
    {
      add
      {
        _pretreatment += value;
      }
      remove
      {
        _pretreatment -= value;
      }
    }

    /// <summary>
    /// Custom spacing of the original input values in row direction. After the Fourier transformation, this value is used to calculate the row frequencies.
    /// </summary>
    /// <value>
    /// The row spacing.
    /// </value>
    public double RowSpacing
    {
      set
      {
        _rowSpacing = value;
      }
    }

    /// <summary>
    /// Custom spacing of the original input values in column direction. After the Fourier transformation, this value is used to calculate the column frequencies.
    /// </summary>
    /// <value>
    /// The column spacing.
    /// </value>
    public double ColumnSpacing
    {
      set
      {
        _columnSpacing = value;
      }
    }

    #endregion Input Properties

    #region Output properties

    public int NumberOfRows
    {
      get
      {
        return _realMatrix.RowCount;
      }
    }

    public int NumberOfColumns
    {
      get
      {
        return _realMatrix.ColumnCount;
      }
    }

    #endregion Output properties

    /// <summary>
    /// Executes the Fourier transformation. First, the dimensions of the matrix are checked. Then the original data is pretreated by calling all Action that are registered in <see cref="DataPretreatment"/>.
    /// Finally, the Fourier transformation is executed.
    /// </summary>
    public void Execute()
    {
      Pretreatment();
      ExecuteFourierTransformation();
    }

    /// <summary>
    /// Executes all data pretreatment actions registered in <see cref="DataPretreatment"/>.
    /// </summary>
    protected virtual void Pretreatment()
    {
      var numColumns = NumberOfColumns;
      var numRows = NumberOfRows;
      _pretreatment?.Invoke(_realMatrix); // and call the pretreatment function(s)
    }

    /// <summary>
    /// Executes the fourier transformation itself (without data pretreatment).
    /// </summary>
    /// <exception cref="System.InvalidOperationException">The Fourier transformation was already executed.</exception>
    protected virtual void ExecuteFourierTransformation()
    {
      if (_arraysContainTransformation)
        throw new InvalidOperationException("The Fourier transformation was already executed.");

      var numColumns = NumberOfColumns;
      var numRows = NumberOfRows;

      var rePart = ((IMatrixInArray1DRowMajorRepresentation<double>)_realMatrix).GetArray1DRowMajor();

      _imagMatrix = new DoubleMatrixInArray1DRowMajorRepresentation(numRows, numColumns);
      var imPart = ((IMatrixInArray1DRowMajorRepresentation<double>)_imagMatrix).GetArray1DRowMajor();

      // fourier transform either with Pfa (faster) or with the Chirp-z-transform
      if (Pfa235FFT.CanFactorized(numRows) && Pfa235FFT.CanFactorized(numColumns))
      {
        var fft = new Pfa235FFT(numRows, numColumns);
        fft.FFT(rePart, imPart, FourierDirection.Forward);
      }
      else
      {
        var matrixRe = new DoubleMatrixInArray1DRowMajorRepresentation(rePart, numRows, numColumns);
        ChirpFFT.FourierTransformation2D(matrixRe, _imagMatrix, FourierDirection.Forward);
      }

      _arraysContainTransformation = true;
    }

    #region Result functions

    /// <summary>
    /// Gets a fresh matrix to capture a result.
    /// </summary>
    /// <returns>A fresh matrix.</returns>
    private IMatrix<double> GetMatrix()
    {
      var numColumns = NumberOfColumns;
      var numRows = NumberOfRows;
      var d = new double[numRows * numColumns];
      var m = MatrixMath.ToMatrixFromColumnMajorLinearArray(d, numRows);
      return m;
    }

    /// <summary>
    /// Gets an evaluation function depending on the requested type of output.
    /// </summary>
    /// <param name="kind">Requested type of output.</param>
    /// <returns>A function that takes the real part and the imaginary part of one Fourier transformation point and returns the requested output value.</returns>
    /// <exception cref="System.NotImplementedException">Triggered if the requested output type is not supported.</exception>
    private Func<double, double, double> GetEvalFunction(RealFourierTransformationOutputKind kind)
    {
      int numRows = NumberOfRows;
      int numCols = NumberOfColumns; // make this two variables local because they get included in the functions;
      double fac = 1.0 / (((double)numCols) * numRows);

      switch (kind)
      {
        case RealFourierTransformationOutputKind.RealPart:
          return (re, im) => re * fac;

        case RealFourierTransformationOutputKind.ImaginaryPart:
          return (re, im) => im * fac;

        case RealFourierTransformationOutputKind.Amplitude:
          return (re, im) => Math.Sqrt(re * re + im * im) * fac;

        case RealFourierTransformationOutputKind.Phase:
          return (re, im) => Math.Atan2(im, re);

        case RealFourierTransformationOutputKind.Power:
          return (re, im) => (re * re + im * im) * fac * fac;

        default:
          throw new NotImplementedException(string.Format("The transformation kind {0} is not implemented!", kind));
      }
    }

    /// <summary>
    /// Gets a result of the Fourier transformation.
    /// </summary>
    /// <param name="rowFraction">Number (0..1) that designates the fraction of output row frequencies that should be included in the result.</param>
    /// <param name="columnFraction">Number (0..1) that designates the fraction of output column frequencies that should be included in the result.</param>
    /// <param name="kind">Requested type of output.</param>
    /// <param name="matrix">The matrix that accomodates the result.</param>
    /// <param name="rowFrequencies">Vector that accomodates the row frequencies.</param>
    /// <param name="columnFrequencies">Vector that accomodates the column frequencies.</param>
    /// <exception cref="System.InvalidOperationException">Before getting any result, you must execute the Fourier transformation first (by calling Execute).</exception>
    public void GetResult(double rowFraction, double columnFraction, RealFourierTransformationOutputKind kind, out IMatrix<double> matrix, out IROVector<double> rowFrequencies, out IROVector<double> columnFrequencies)
    {
      GetResult(rowFraction, columnFraction, GetEvalFunction(kind), out matrix, out rowFrequencies, out columnFrequencies);
    }

    /// <summary>
    /// Gets a result of the Fourier transformation.
    /// </summary>
    /// <param name="rowFraction">Number (0..1) that designates the fraction of output row frequencies that should be included in the result.</param>
    /// <param name="columnFraction">Number (0..1) that designates the fraction of output column frequencies that should be included in the result.</param>
    /// <param name="resultantEval">A function that takes the real part and the imaginary part of one Fourier transformation point and returns the requested output value.</param>
    /// <param name="matrix">The matrix that accomodates the result.</param>
    /// <param name="rowFrequencies">Vector that accomodates the row frequencies.</param>
    /// <param name="columnFrequencies">Vector that accomodates the column frequencies.</param>
    /// <exception cref="System.InvalidOperationException">Before getting any result, you must execute the Fourier transformation first (by calling Execute).</exception>
    public void GetResult(double rowFraction, double columnFraction, Func<double, double, double> resultantEval, out IMatrix<double> matrix, out IROVector<double> rowFrequencies, out IROVector<double> columnFrequencies)
    {
      if (!_arraysContainTransformation)
        throw new InvalidOperationException("Before getting any result, you must execute the Fourier transformation first (by calling Execute).");

      int resultRows = Math.Min(NumberOfRows, Math.Max(1, (int)(rowFraction * NumberOfRows)));
      int resultColumns = Math.Min(NumberOfColumns, Math.Max(1, (int)(columnFraction * NumberOfColumns)));

      var d = new double[resultRows * resultColumns];
      matrix = MatrixMath.ToMatrixFromColumnMajorLinearArray(d, resultRows);

      GetResult(matrix, resultantEval);
      rowFrequencies = GetRowFrequencies(resultRows);
      columnFrequencies = GetColumnFrequencies(resultColumns);
    }

    /// <summary>
    /// Gets a result of the Fourier transformation. Here, the row and column frequency of zero is located in the center of the matrix.
    /// </summary>
    /// <param name="rowFraction">Number (0..1) that designates the fraction of output row frequencies that should be included in the result.</param>
    /// <param name="columnFraction">Number (0..1) that designates the fraction of output column frequencies that should be included in the result.</param>
    /// <param name="kind">Requested type of output.</param>
    /// <param name="matrix">The matrix that accomodates the result.</param>
    /// <param name="rowFrequencies">Vector that accomodates the row frequencies.</param>
    /// <param name="columnFrequencies">Vector that accomodates the column frequencies.</param>
    /// <exception cref="System.InvalidOperationException">Before getting any result, you must execute the Fourier transformation first (by calling Execute).</exception>
    public void GetResultCentered(double rowFraction, double columnFraction, RealFourierTransformationOutputKind kind, out IMatrix<double> matrix, out IROVector<double> rowFrequencies, out IROVector<double> columnFrequencies)
    {
      GetResultCentered(rowFraction, columnFraction, GetEvalFunction(kind), out matrix, out rowFrequencies, out columnFrequencies);
    }

    /// <summary>
    /// Gets a result of the Fourier transformation.  Here, the row and column frequency of zero is located in the center of the matrix.
    /// </summary>
    /// <param name="rowFraction">Number (0..1) that designates the fraction of output row frequencies that should be included in the result.</param>
    /// <param name="columnFraction">Number (0..1) that designates the fraction of output column frequencies that should be included in the result.</param>
    /// <param name="resultantEval">A function that takes the real part and the imaginary part of one Fourier transformation point and returns the requested output value.</param>
    /// <param name="matrix">The matrix that accomodates the result.</param>
    /// <param name="rowFrequencies">Vector that accomodates the row frequencies.</param>
    /// <param name="columnFrequencies">Vector that accomodates the column frequencies.</param>
    /// <exception cref="System.InvalidOperationException">Before getting any result, you must execute the Fourier transformation first (by calling Execute).</exception>
    public void GetResultCentered(double rowFraction, double columnFraction, Func<double, double, double> resultantEval, out IMatrix<double> matrix, out IROVector<double> rowFrequencies, out IROVector<double> columnFrequencies)
    {
      if (!_arraysContainTransformation)
        throw new InvalidOperationException("Before getting any result, you must execute the Fourier transformation first (by calling Execute).");

      int resultRows = Math.Min(NumberOfRows, Math.Max(1, (int)(rowFraction * NumberOfRows)));
      int resultColumns = Math.Min(NumberOfColumns, Math.Max(1, (int)(columnFraction * NumberOfColumns)));

      var d = new double[resultRows * resultColumns];
      matrix = MatrixMath.ToMatrixFromColumnMajorLinearArray(d, resultRows);

      GetResultCentered(matrix, resultantEval);

      rowFrequencies = GetRowFrequenciesCentered(resultRows);
      columnFrequencies = GetColumnFrequenciesCentered(resultColumns);
    }

    /// <summary>
    /// Gets the result of the Fourier transformation.
    /// </summary>
    /// <param name="matrix">The matrix to accomodate the result. Can have the dimensions <see cref="NumberOfRows"/>x<see cref="NumberOfColumns"/> or less, but not more.</param>
    /// <param name="resultantEval">A function that takes the real part and the imaginary part of one Fourier transformation point and returns the requested output value.</param>
    /// <exception cref="System.InvalidOperationException">Before getting any result, you must execute the Fourier transformation first (by calling Execute).</exception>
    /// <exception cref="InvalidDimensionMatrixException">If the provided matrix <paramref name="matrix"/> has more rows or more columns than the result can provide.</exception>
    public void GetResult(IMatrix<double> matrix, Func<double, double, double> resultantEval)
    {
      if (!_arraysContainTransformation || _imagMatrix is null)
        throw new InvalidOperationException("Before getting any result, you must execute the Fourier transformation first (by calling Execute).");

      var numColumns = NumberOfColumns;
      var numRows = NumberOfRows;

      int resultNumRows = matrix.RowCount;
      int resultNumColumns = matrix.ColumnCount;

      if (resultNumRows > numRows)
        throw new InvalidDimensionMatrixException(string.Format("The provided matrix has more rows ({0}), than the result can provide ({1}).", matrix.RowCount, numRows));
      if (resultNumColumns > numColumns)
        throw new InvalidDimensionMatrixException(string.Format("The provided matrix has more columns ({0}), than the result can provide ({1}).", matrix.ColumnCount, numColumns));

      var rePart = ((IMatrixInArray1DRowMajorRepresentation<double>)_realMatrix).GetArray1DRowMajor();
      var imPart = ((IMatrixInArray1DRowMajorRepresentation<double>)_imagMatrix).GetArray1DRowMajor();

      for (int j = 0; j < resultNumRows; j++)
      {
        var rowOffs = j * numColumns;
        for (int i = 0; i < resultNumColumns; i++)
        {
          int idx = i + rowOffs;
          matrix[j, i] = resultantEval(rePart[idx], imPart[idx]);
        }
      }
    }

    /// <summary>
    /// Gets the result of the Fourier transformation.  Here, the value associated with the row and column frequency of zero is located in the center of the matrix.
    /// </summary>
    /// <param name="matrix">The matrix to accomodate the result. Can have the dimensions <see cref="NumberOfRows"/>x<see cref="NumberOfColumns"/> or less, but not more.</param>
    /// <param name="resultantEval">A function that takes the real part and the imaginary part of one Fourier transformation point and returns the requested output value.</param>
    /// <exception cref="System.InvalidOperationException">Before getting any result, you must execute the Fourier transformation first (by calling Execute).</exception>
    /// <exception cref="InvalidDimensionMatrixException">If the provided matrix <paramref name="matrix"/> has more rows or more columns than the result can provide.</exception>
    protected void GetResultCentered(IMatrix<double> matrix, Func<double, double, double> resultantEval)
    {
      if (!_arraysContainTransformation || _imagMatrix is null)
        throw new InvalidOperationException("Before getting any result, you must execute the Fourier transformation first (by calling Execute).");

      var numColumns = NumberOfColumns;
      var numRows = NumberOfRows;

      int resultNumRows = matrix.RowCount;
      int resultNumColumns = matrix.ColumnCount;

      if (resultNumRows > numRows)
        throw new InvalidDimensionMatrixException(string.Format("The provided matrix has more rows ({0}), than the result can provide ({1}).", matrix.RowCount, numRows));
      if (resultNumColumns > numColumns)
        throw new InvalidDimensionMatrixException(string.Format("The provided matrix has more columns ({0}), than the result can provide ({1}).", matrix.ColumnCount, numColumns));

      int colsNegative = (numColumns - 1) / 2; // number of negative frequency points
      int colsPositive = numColumns - colsNegative; // number of positive (or null) frequency points
      int rowsNegative = (numRows - 1) / 2;
      int rowsPositive = numRows - rowsNegative;

      var rePart = ((IMatrixInArray1DRowMajorRepresentation<double>)_realMatrix).GetArray1DRowMajor();
      var imPart = ((IMatrixInArray1DRowMajorRepresentation<double>)_imagMatrix).GetArray1DRowMajor();

      int columnOffset = (numColumns - resultNumColumns) / 2;
      int rowOffset = (numRows - resultNumRows) / 2;

      for (int i = 0; i < resultNumColumns; ++i)
      {
        var ii = i + columnOffset;
        int sc = ii < colsNegative ? ii + colsPositive : ii - colsNegative; // source column index centered
        for (int j = 0; j < resultNumRows; ++j)
        {
          int jj = j + rowOffset;
          int sr = jj < rowsNegative ? jj + rowsPositive : jj - rowsNegative; // source row index centered
          int idx = sc + sr * numColumns;
          matrix[j, i] = resultantEval(rePart[idx], imPart[idx]);
        }
      }
    }

    /// <summary>
    /// Gets the resultant matrix of Fourier amplitudes.
    /// </summary>
    /// <returns>Resultant matrix of Fourier amplitudes.</returns>
    public IMatrix<double> GetFourierAmplitude()
    {
      var m = GetMatrix();
      GetResult(m, (re, im) => Math.Sqrt(re * re + im * im));
      return m;
    }

    /// <summary>
    /// Gets the resultant matrix of Fourier amplitudes.  Here, the value associated with a row and column frequency of zero is located in the center of the matrix.
    /// </summary>
    /// <returns>Resultant matrix of Fourier amplitudes.</returns>
    public IMatrix<double> GetFourierAmplitudeCentered()
    {
      var m = GetMatrix();
      GetResultCentered(m, (re, im) => Math.Sqrt(re * re + im * im));
      return m;
    }

    /// <summary>
    /// Gets the row frequencies.
    /// </summary>
    /// <param name="numberOfResultRowFrequencies">The number of resulting rows. Use the value <see cref="NumberOfRows"/> to get all frequencies, or a value less than that to get only a fraction of frequencies.</param>
    /// <returns>Vector that accomodates the row frequencies.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">NumberOfResultRows has to be less than or equal to the existing number of rows.</exception>
    public IROVector<double> GetRowFrequencies(int numberOfResultRowFrequencies)
    {
      if (numberOfResultRowFrequencies > NumberOfRows)
        throw new ArgumentOutOfRangeException(string.Format("numberOfResultRows has to be less than or equal to the existing number of rows"));

      var endFreq = 0.5 / (_rowSpacing.HasValue ? _rowSpacing.Value : 1);
      return VectorMath.CreateEquidistantSequenceByStartEndLength(0, endFreq, numberOfResultRowFrequencies);
    }

    /// <summary>
    /// Gets the row frequencies. Here, the row frequency of zero is located in the center of the matrix.
    /// </summary>
    /// <param name="numberOfResultRowFrequencies">The number of resulting rows. Use the value <see cref="NumberOfRows"/> to get all frequencies, or a value less than that to get only a fraction of frequencies.</param>
    /// <returns>Vector that accomodates the row frequencies.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">NumberOfResultRows has to be less than or equal to the existing number of rows.</exception>
    public IROVector<double> GetRowFrequenciesCentered(int numberOfResultRowFrequencies)
    {
      if (numberOfResultRowFrequencies > NumberOfRows)
        throw new ArgumentOutOfRangeException(string.Format("numberOfResultRows has to be less than or equal to the existing number of rows"));

      int rowsNegative = (NumberOfRows - 1) / 2;
      double stepFrequency = 0.5 / (NumberOfRows * (_rowSpacing.HasValue ? _rowSpacing.Value : 1));
      int rowOffset = (NumberOfRows - numberOfResultRowFrequencies) / 2;

      return VectorMath.CreateEquidistantSequencyByStartAtOffsetStepLength(0, rowsNegative - rowOffset, stepFrequency, numberOfResultRowFrequencies);
    }

    /// <summary>
    /// Gets the column frequencies.
    /// </summary>
    /// <param name="numberOfResultColumnFrequencies">The number of resulting column frequencies. Use the value <see cref="NumberOfColumns"/> to get all frequencies, or a value less than that to get only a part of the frequencies.</param>
    /// <returns>Vector that accomodates the column frequencies.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">NumberOfResultRows has to be less than or equal to the existing number of rows.</exception>
    public IROVector<double> GetColumnFrequencies(int numberOfResultColumnFrequencies)
    {
      if (numberOfResultColumnFrequencies > NumberOfColumns)
        throw new ArgumentOutOfRangeException(string.Format("numberOfResultRows has to be less than or equal to the existing number of rows"));

      var endFreq = 0.5 / (_columnSpacing.HasValue ? _columnSpacing.Value : 1);
      return VectorMath.CreateEquidistantSequenceByStartEndLength(0, endFreq, numberOfResultColumnFrequencies);
    }

    /// <summary>
    /// Gets the column frequencies. Here, the column frequency of zero is located in the center of the matrix.
    /// </summary>
    /// <param name="numberOfResultColumnFrequencies">The number of resulting column frequencies. Use the value <see cref="NumberOfColumns"/> to get all frequencies, or a value less than that to get only a part of the frequencies.</param>
    /// <returns>Vector that accomodates the column frequencies.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">NumberOfResultRows has to be less than or equal to the existing number of rows.</exception>
    public IROVector<double> GetColumnFrequenciesCentered(int numberOfResultColumnFrequencies)
    {
      if (numberOfResultColumnFrequencies > NumberOfColumns)
        throw new ArgumentOutOfRangeException(string.Format("numberOfResultRows has to be less than or equal to the existing number of rows"));
      int colsNegative = (NumberOfColumns - 1) / 2;
      double stepFrequency = 0.5 / (NumberOfColumns * (_columnSpacing.HasValue ? _columnSpacing.Value : 1));
      int columnOffset = (NumberOfColumns - numberOfResultColumnFrequencies) / 2;
      return VectorMath.CreateEquidistantSequencyByStartAtOffsetStepLength(0, colsNegative - columnOffset, stepFrequency, numberOfResultColumnFrequencies);
    }

    #endregion Result functions

    #region Static public helper functions

    /// <summary>
    /// Removes the zero-th order of a matrix by calculating the mean of the (valid) matrix elements, and then subtracting the mean from each matrix element.
    /// Here, only matrix elements that have a finite value are included in the calculation of the mean value.
    /// </summary>
    /// <param name="m">The matrix to change.</param>
    public static void RemoveZeroOrderFromMatrixIgnoringInvalidElements(IMatrix<double> m)
    {
      int rows = m.RowCount;
      int cols = m.ColumnCount;

      double sum = 0;
      int n = 0;
      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          double x = m[r, c];
          if (RMath.IsFinite(x))
          {
            sum += x;
            ++n;
          }
        }
      }

      double mean = sum / n;

      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          m[r, c] -= mean;
        }
      }
    }

    /// <summary>
    /// Removes the first order of a matrix by calculating the parameters a, b, c of the regression of the matrix in the form: z = a + b*x + c*y (x and y are the rows and columns of the matrix, z the matrix elements ),
    /// and then subtracting the regression function from each matrix element, thus effectively removing the first order.
    /// Here, only matrix elements that have a finite value are included in the calculation of the regression.
    /// </summary>
    /// <param name="m">The matrix to change.</param>
    public static void RemoveFirstOrderFromMatrixIgnoringInvalidElements(IMatrix<double> m)
    {
      int rows = m.RowCount;
      int cols = m.ColumnCount;

      int rowsBy2 = rows / 2;
      int colsBy2 = cols / 2;

      var independent = new List<double>[2];
      for (int i = 0; i < independent.Length; ++i)
        independent[i] = new List<double>();

      var dependent = new List<double>();
      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          var z = m[r, c];
          if (RMath.IsFinite(z))
          {
            long x = c - colsBy2;
            long y = r - rowsBy2;

            independent[0].Add(x);
            independent[1].Add(y);
            dependent.Add(z);
          }
        }
      }

      var regress = new Altaxo.Calc.Regression.LinearFitBySvd(new MyPriv2ndOrderIndependentMatrix(independent), dependent.ToArray(), null, dependent.Count, 1 + independent.Length, 1e-6);
      var coef = regress.Parameter;

      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          var z = m[r, c];
          if (RMath.IsFinite(z))
          {
            long x = c - colsBy2;
            long y = r - rowsBy2;

            double offs = coef[0] + coef[1] * x + coef[2] * y;
            m[r, c] -= offs;
          }
        }
      }
    }

    /// <summary>
    /// Removes the second order of a matrix by calculating the parameters a, b, .. e, f of the regression of the matrix in the form: z = a + b*x + c*y + d*x*x + e*x*y + f*y*y (x and y are the rows and columns of the matrix, z the matrix elements ),
    /// and then subtracting the regression function from each matrix element, thus effectively removing the second order.
    /// Here, only matrix elements that have a finite value are included in the calculation of the regression.
    /// </summary>
    /// <param name="m">The matrix to change.</param>
    public static void RemoveSecondOrderFromMatrixIgnoringInvalidElements(IMatrix<double> m)
    {
      int rows = m.RowCount;
      int cols = m.ColumnCount;

      int rowsBy2 = rows / 2;
      int colsBy2 = cols / 2;

      var independent = new List<double>[5];
      for (int i = 0; i < independent.Length; ++i)
        independent[i] = new List<double>();

      var dependent = new List<double>();
      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          var z = m[r, c];
          if (RMath.IsFinite(z))
          {
            long x = c - colsBy2;
            long y = r - rowsBy2;

            independent[0].Add(x);
            independent[1].Add(y);
            independent[2].Add(x * x);
            independent[3].Add(x * y);
            independent[4].Add(y * y);
            dependent.Add(z);
          }
        }
      }

      var regress = new Altaxo.Calc.Regression.LinearFitBySvd(new MyPriv2ndOrderIndependentMatrix(independent), dependent.ToArray(), null, dependent.Count, 1 + independent.Length, 1e-6);
      var coef = regress.Parameter;

      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          var z = m[r, c];
          if (RMath.IsFinite(z))
          {
            long x = c - colsBy2;
            long y = r - rowsBy2;

            double offs = coef[0] + coef[1] * x + coef[2] * y + coef[3] * x * x + coef[4] * x * y + coef[5] * y * y;
            m[r, c] -= offs;
          }
        }
      }
    }

    /// <summary>
    /// Removes the third order of a matrix by calculating the parameters a, b, .. i, j of the regression of the matrix in the form: z = a + b*x + c*y + d*x*x + e*x*y + f*y*y + g*x*x*x + h*x*x*y + i*x*y*y + j*y*y*y (x and y are the rows and columns of the matrix, z the matrix elements ),
    /// and then subtracting the regression function from each matrix element, thus effectively removing the third order.
    /// Here, only matrix elements that have a finite value are included in the calculation of the regression.
    /// </summary>
    /// <param name="m">The matrix to change.</param>
    public static void RemoveThirdOrderFromMatrixIgnoringInvalidElements(IMatrix<double> m)
    {
      int rows = m.RowCount;
      int cols = m.ColumnCount;

      int rowsBy2 = rows / 2;
      int colsBy2 = cols / 2;

      var independent = new List<double>[9];
      for (int i = 0; i < independent.Length; ++i)
        independent[i] = new List<double>();

      var dependent = new List<double>();
      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          var z = m[r, c];
          if (RMath.IsFinite(z))
          {
            long x = c - colsBy2;
            long y = r - rowsBy2;

            independent[0].Add(x);
            independent[1].Add(y);
            independent[2].Add(x * x);
            independent[3].Add(x * y);
            independent[4].Add(y * y);
            independent[5].Add(x * x * x);
            independent[6].Add(x * x * y);
            independent[7].Add(x * y * y);
            independent[8].Add(y * y * y);
            dependent.Add(z);
          }
        }
      }

      var regress = new Altaxo.Calc.Regression.LinearFitBySvd(new MyPriv2ndOrderIndependentMatrix(independent), dependent.ToArray(), null, dependent.Count, 1 + independent.Length, 1e-6);
      var coef = regress.Parameter;

      for (int r = 0; r < rows; ++r)
      {
        for (int c = 0; c < cols; ++c)
        {
          var z = m[r, c];
          if (RMath.IsFinite(z))
          {
            long x = c - colsBy2;
            long y = r - rowsBy2;

            double offs = coef[0] + coef[1] * x + coef[2] * y + coef[3] * x * x + coef[4] * x * y + coef[5] * y * y + coef[6] * x * x * x + coef[7] * x * x * y + coef[8] * x * y * y + coef[9] * y * y * y;
            m[r, c] -= offs;
          }
        }
      }
    }

    private class MyPriv2ndOrderIndependentMatrix : IROMatrix<double>
    {
      private List<double>[] _list;
      private int _numberOfRows;
      private int _numberOfColumns;

      internal MyPriv2ndOrderIndependentMatrix(List<double>[] lists)
      {
        _list = lists;
        _numberOfColumns = lists.Length + 1;
        _numberOfRows = lists[0].Count;
      }

      public double this[int row, int col]
      {
        get
        {
          if (0 == col)
            return 1; // Intercept
          else
            return _list[col - 1][row];
        }
      }

      public int RowCount
      {
        get { return _numberOfRows; }
      }

      public int ColumnCount
      {
        get { return _numberOfColumns; }
      }
    }

    #endregion Static public helper functions
  }
}
