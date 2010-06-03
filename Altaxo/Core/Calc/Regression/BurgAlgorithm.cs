using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
	/// <summary>
	/// Implements Burg's algorithm with real numbers.
	/// </summary>
	public class BurgAlgorithm
	{
		/// <summary>Forward prediction errors.</summary>
		double[] _f;

		/// <summary>Backward prediction errors.</summary> 
		double[] _b;

		/// <summary>Prediction coefficients. Note that for technical reasons _Ak[0] is always 1 and the calculated coefficients start with _Ak[1].</summary>
		double[] _Ak;

		/// <summary>Wrapper for the coefficients that can be returned by <see cref="Coefficients"/>.</summary>
		IVector _AkWrapper;

		/// <summary>Number of coefficients that were calculated.</summary>
		int _numberOfCoefficients;

		/// <summary>Mean square error calculated during the last run.</summary>
		double _meanSquareError;

		/// <summary>
		/// Returns the number of coefficients that were used for the last run of the algorithm.
		/// </summary>
		public int NumberOfCoefficients
		{
			get
			{
				return _numberOfCoefficients;
			}
		}

		/// <summary>
		/// Returns the coefficients that were calculated during the last run of the algorithm.
		/// </summary>
		public IROVector Coefficients
		{
			get
			{
				return _AkWrapper;
			}
		}

		/// <summary>Mean square error calculated during the last run of the algorithm.</summary>
		public double MeanSquareError
		{
			get
			{
				return _meanSquareError;
			}
		}


		/// <summary>
		/// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="numberOfCoefficients">Number of coefficients of the model.</param>
		public void Execute(IROVector x, int numberOfCoefficients)
		{
			EnsureAllocation(x.Length, numberOfCoefficients);
			_meanSquareError = Execution(x, _AkWrapper, null, null, this);
		}


		/// <summary>
		/// Uses th signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
		public void Execute(IROVector x, IVector coefficients)
		{
			_meanSquareError = Execution(x, coefficients, null, null, this);
		}


		/// <summary>
		/// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
		/// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
		public void Execute(IROVector x, IVector coefficients, IVector errors)
		{
			_meanSquareError = Execution(x, coefficients, errors, null, this);
		}

		/// <summary>
		/// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
		/// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
		/// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
		public void Execute(IROVector x, IVector coefficients, IVector errors, IVector reflectionCoefficients)
		{
			_meanSquareError = Execution(x, coefficients, errors, reflectionCoefficients, this);
		}


		/// <summary>
		/// Predict values towards the end of the vector. The predicted values are then used to predict more values. See remarks for details.
		/// </summary>
		/// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (firstPoint-NumberOfCoefficents) to (firstPoint-1). The predicted values are then stored in this vector.</param>
		/// <param name="firstPoint">Index of the first point to predict.</param>
		/// <remarks>
		/// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points before the <c>firstPoint</c> to predict the value at <c>firstPoint</c>.
		/// Then the window is shifted by one towards the end of the vecctor, hence including the predicted value, and the point at <c>firstPoint+1</c> is predicted. The procedure is repeated until all points to the end of the vector are predicted.
		/// </remarks>
		public void PredictRecursivelyForward(IVector x, int firstPoint)
		{
			PredictRecursivelyForward(x, firstPoint, x.Length - firstPoint);
		}

		/// <summary>
		/// Predict values towards the end of the vector. The predicted values are then used to predict more values. See remarks for details.
		/// </summary>
		/// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (firstPoint-NumberOfCoefficents) to (firstPoint-1). The predicted values are then stored in this vector.</param>
		/// <param name="firstPoint">Index of the first point to predict.</param>
		/// <param name="count">Number of points to predict.</param>
		/// <remarks>
		/// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points before the <c>firstPoint</c> to predict the value at <c>firstPoint</c>.
		/// Then the window is shifted by one towards the end of the vecctor, hence including the predicted value, and the point at <c>firstPoint+1</c> is predicted. The procedure is repeated until <c>count</c> points are predicted.
		/// </remarks>
		public void PredictRecursivelyForward(IVector x, int firstPoint, int count)
		{
			int last = firstPoint + count;
			for (int i = firstPoint; i < last; i++)
			{
				double sum = 0;
				for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
				{
					sum -= _Ak[k] * x[i - k];
				}
				x[i] = sum;
			}
		}

		/// <summary>
		/// This algorithm determines the mean forward prediction error using the model stored in this instance. See remarks for details.
		/// </summary>
		/// <param name="x">Signal for which to determine the mean forward prediction error.</param>
		/// <returns>Mean backward prediction error.</returns>
		/// <remarks>
		/// 1. The prediction is done non recursively, i.e. part of the signal (the signal window) is used to predict the signal value immediately after the window, and this predicted signal value is
		/// then compared with the original signal value stored in x to build the sum of errors. But the predicted signal value is <b>not</b> used to make further predictions.
		/// Instead, the signal window is moved by one point to the right and another prediction is made, with the original signal in x. This is repeated until the last point
		/// is predicted. The return value is the square root of the sum of squared differences between predicted signal values and original values, divided by the number of predicted values.
		/// The number of predicted values is the length of the signal x minus the number of coefficents of the model.
		/// </remarks>
		public double GetMeanPredictionErrorNonrecursivelyForward(IROVector x)
		{
			int first = _numberOfCoefficients;
			int last = x.Length;
			double sumsqr = 0;
			for (int i = first; i < last; i++)
			{
				double sum = 0;
				for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
				{
					sum -= _Ak[k] * x[i - k];
				}
				sumsqr += Square(x[i] - sum);
			}
			return Math.Sqrt(sumsqr / (last - first));
		}


		/// <summary>
		/// Predict values towards the start of the vector. The predicted values are then used to predict more values. See remarks for details.
		/// </summary>
		/// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (lastPoint+1) to (lastPoint+NumberOfCoefficents). The predicted values are then stored in the first part of this vector from indices (0) to (lastPoint).</param>
		/// <param name="lastPoint">Index of the last point to predict.</param>
		/// <remarks>
		/// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points after the <c>lastPoint</c> to predict the value at <c>lastPoint</c>.
		/// Then the window is shifted by one towards the start of the vecctor, hence including the predicted value, and the point at <c>lastPoint-1</c> is predicted. The procedure is repeated until the value at index 0 is predicted.
		/// </remarks>
		public void PredictRecursivelyBackward(IVector x, int lastPoint)
		{
			PredictRecursivelyBackward(x, lastPoint, lastPoint + 1);
		}

		/// <summary>
		/// Predict values towards the start of the vector. The predicted values are then used to predict more values. See remarks for details.
		/// </summary>
		/// <param name="x">Signal which holds at least <see cref="NumberOfCoefficients"/> valid points (the signal window to start the prediction with) from index (lastPoint+1) to (lastPoint+NumberOfCoefficents). The predicted values are then stored in the first part of this vector from indices (lastPoint-count+1) to (lastPoint).</param>
		/// <param name="lastPoint">Index of the last point to predict.</param>
		/// <param name="count">Number of points to predict.</param>
		/// <remarks>
		/// The algorithm uses a signal window of <c>NumberOfCoefficients</c> signal points after the <c>lastPoint</c> to predict the value at <c>lastPoint</c>.
		/// Then the window is shifted by one towards the start of the vecctor, hence including the predicted value, and the point at <c>lastPoint-1</c> is predicted. The procedure is repeated until <c>count</c> points are predicted.
		/// </remarks>
		public void PredictRecursivelyBackward(IVector x, int lastPoint, int count)
		{
			int first = lastPoint - count;
			for (int i = lastPoint; i > first; i--)
			{
				double sum = 0;
				for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
				{
					sum -= _Ak[k] * x[i + k];
				}
				x[i] = sum;
			}
		}

		/// <summary>
		/// This algorithm determines the mean backward prediction error using the model stored in this instance. See remarks for details.
		/// </summary>
		/// <param name="x">Signal for which to determine the mean backward prediction error.</param>
		/// <returns>Mean backward prediction error.</returns>
		/// <remarks>
		/// 1. The prediction is done non recursively, i.e. part of the signal (the signal window) is used to predict the signal value before, and this predicted signal value is
		/// then compared with the original signal value stored in x to build the sum of errors. But the predicted signal value is <b>not</b> used to make further predictions.
		/// Instead, the signal window is moved by one point to the left and another prediction is made, with the original signal in x. This is repeated until the first point (index 0)
		/// is predicted. The return value is the square root of the sum of squared differences between predicted signal values and original values, divided by the number of predicted values.
		/// The number of predicted values is the length of the signal x minus the number of coefficents of the model.
		/// </remarks>
		public double GetMeanPredictionErrorNonrecursivelyBackward(IROVector x)
		{
			int last = x.Length - _numberOfCoefficients;
			double sumsqr = 0;
			for (int i = last - 1; i >= 0; i--)
			{
				double sum = 0;
				for (int k = 1; k <= _numberOfCoefficients; k++) // note that Ak[0] is always 1 for technical reasons, thus we start here with index 1
				{
					sum -= _Ak[k] * x[i + k];
				}
				sumsqr += Square(x[i] - sum);
			}
			return Math.Sqrt(sumsqr / (last));
		}



		/// <summary>
		/// Ensures that temporary arrays are allocated in order to execute the Burg algorithm.
		/// </summary>
		/// <param name="xLength">Length of the vector to build the model.</param>
		/// <param name="coeffLength">Number of parameters of the model.</param>
		private void EnsureAllocation(int xLength, int coeffLength)
		{
			_numberOfCoefficients = coeffLength;

			if (null == _Ak || _Ak.Length < coeffLength + 1)
			{
				_Ak = new double[coeffLength + 1];
				_AkWrapper = VectorMath.ToVector(_Ak, 1, _numberOfCoefficients);
			}

			if (_numberOfCoefficients != _AkWrapper.Length)
				_AkWrapper = VectorMath.ToVector(_Ak, 1, _numberOfCoefficients);

			if (null == _b || _b.Length < xLength)
				_b = new double[xLength];

			if (null == _f || _f.Length < xLength)
				_f = new double[xLength];
		}


		/// <summary>
		/// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
		/// <returns>The mean square error of backward and forward prediction.</returns>
		public static double Execution(IROVector x, IVector coefficients)
		{
			return Execution(x, coefficients, null, null, null);
		}

		/// <summary>
		/// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
		/// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
		/// <returns>The mean square error of backward and forward prediction.</returns>
		public static double Execution(IROVector x, IVector coefficients, IVector errors)
		{
			return Execution(x, coefficients, errors, null, null);
		}

		/// <summary>
		/// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
		/// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
		/// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
		/// <returns>The mean square error of backward and forward prediction.</returns>
		public static double Execution(IROVector x, IVector coefficients, IVector errors, IVector reflectionCoefficients)
		{
			return Execution(x, coefficients, errors, reflectionCoefficients, null);
		}

		/// <summary>
		/// Uses the signal in vector x to build a model with <c>numberOfCoefficients</c> parameter.
		/// </summary>
		/// <param name="x">Signal for building the model.</param>
		/// <param name="coefficients">Vector to be filled with the coefficients of the model.</param>
		/// <param name="errors">Vector to be filled with the sum of forward and backward prediction error for every stage of the model.</param>
		/// <param name="reflectionCoefficients">Vector to be filled with the reflection coefficients.</param>
		/// <param name="tempStorage">Instance of this class used to hold the temporary arrays.</param>
		/// <returns>The mean square error of backward and forward prediction.</returns>
		private static double Execution(IROVector x, IVector coefficients, IVector errors, IVector reflectionCoefficients, BurgAlgorithm tempStorage)
		{
			int N = x.Length - 1;
			int m = coefficients.Length;

			double[] Ak; // Prediction coefficients, Ak[0] is always 1
			double[] b; // backward prediction errors
			double[] f; // forward prediction errors

			if (null != tempStorage)
			{
				tempStorage.EnsureAllocation(x.Length, coefficients.Length);
				Ak = tempStorage._Ak;
				b = tempStorage._b;
				f = tempStorage._f;
				for (int i = 1; i <= coefficients.Length; i++)
					Ak[i] = 0;
			}
			else
			{
				Ak = new double[coefficients.Length + 1];
				b = new double[x.Length];
				f = new double[x.Length];
			}

			Ak[0] = 1;

			// Initialize forward and backward prediction errors with x
			for (int i = 0; i <= N; i++)
				f[i] = b[i] = x[i];

			double Dk = 0;

			for (int i = 0; i <= N; i++)
				Dk += 2 * f[i] * f[i];

			Dk -= f[0] * f[0] + b[N] * b[N];

			// Burg recursion
			int k;
			double sumE = 0; // error sum
			for (k = 0; (k < m) && (Dk > 0); k++)
			{
				// Compute mu
				double mu = 0;
				for (int n = 0; n < N - k; n++)
					mu += f[n + k + 1] * b[n];

				mu *= -2 / Dk;

				// Update Ak
				for (int n = 0; n <= (k + 1) / 2; n++)
				{
					double t1 = Ak[n] + mu * Ak[k + 1 - n];
					double t2 = Ak[k + 1 - n] + mu * Ak[n];
					Ak[n] = t1;
					Ak[k + 1 - n] = t2;
				}
				if (null != reflectionCoefficients)
					reflectionCoefficients[k] = Ak[k + 1];

				// update forward and backward predition error with simultaneous total error calculation
				sumE = 0;
				for (int n = 0; n < N - k; n++)
				{
					double t1 = f[n + k + 1] + mu * b[n];
					double t2 = b[n] + mu * f[n + k + 1];
					f[n + k + 1] = t1;
					b[n] = t2;
					sumE += t1 * t1 + t2 * t2;
				}
				if (null != errors)
					errors[k] = sumE / (2 * (N - k));
				// Update Dk
				// Note that it is possible to update Dk without total error calculation because sumE = Dk*(1-mu.GetModulusSquared())
				// but this will render the algorithm numerically unstable especially for higher orders and low noise
				Dk = sumE - (f[k + 1] * f[k + 1] + b[N - k - 1] * b[N - k - 1]);
			}

			// Assign coefficients
			for (int i = 0; i < m; i++)
				coefficients[i] = Ak[i + 1];

			// Assign the rest of reflection coefficients and errors with zero
			// if not all stages could be calculated because Dk was zero or because of rounding effects smaller than zero
			for (int i = k + 1; i < m; i++)
			{
				if (null != reflectionCoefficients)
					reflectionCoefficients[i] = 0;
				if (null != errors)
					errors[i] = 0;
			}

			return sumE / (2 * (N - k));
		}

		/// <summary>Square of x.</summary>
		/// <param name="x">x</param>
		/// <returns>Square of x.</returns>
		static double Square(double x)
		{
			return x * x;
		}
	}
}
