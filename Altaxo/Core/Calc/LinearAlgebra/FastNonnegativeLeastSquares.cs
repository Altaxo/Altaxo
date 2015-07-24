using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
	public static class FastNonnegativeLeastSquares
	{
		/// <summary>
		/// Exection of the fast nonnegative least squares algorithm. The algorithm finds an vector x with all elements xi&gt;=0 which minimizes |X*x-y|.
		/// </summary>
		/// <param name="XtX">X transposed multiplied by X, thus a square matrix.</param>
		/// <param name="Xty">X transposed multiplied by Y, thus a matrix with one column and same number of rows as X.</param>
		/// <param name="tolerance">Can be null. Used to determine if a solution element is less than or equal to zero.</param>
		/// <param name="x">Output: solution vector (matrix with one column and number of rows according to dimension of X.</param>
		/// <param name="w">Output: Lagrange vector. Elements which take place in the fit are set to 0. Elements fixed to zero contain a negative number.</param>
		public static void Execute(IROMatrix XtX, IROMatrix Xty, double? tolerance, out IMatrix x, out IMatrix w)
		{
			if (null == XtX)
				throw new ArgumentNullException(nameof(XtX));
			if (null == Xty)
				throw new ArgumentNullException(nameof(Xty));

			if (XtX.Rows != XtX.Columns)
				throw new ArgumentException("Matrix should be a square matrix", nameof(XtX));
			if (Xty.Columns != 1)
				throw new ArgumentException(nameof(Xty) + " should be a column vector (number of columns should be equal to 1)", nameof(Xty));
			if (Xty.Rows != XtX.Columns)
				throw new ArgumentException("Number of rows in " + nameof(Xty) + " should match number of columns in " + nameof(XtX), nameof(Xty));

			var matrixGenerator = new Func<int, int, DoubleMatrix>((rows, cols) => new DoubleMatrix(rows, cols));

			// if nargin < 3
			//   tol = 10 * eps * norm(XtX, 1) * length(XtX);
			// end
			double tol = tolerance.HasValue ? tolerance.Value : 10 * DoubleConstants.DBL_EPSILON * MatrixMath.Norm(XtX, MatrixNorm.M1Norm) * Math.Max(XtX.Rows, XtX.Columns);

			//	[m, n] = size(XtX);
			int n = XtX.Columns;

			// P = zeros(1, n);
			// Z = 1:n;
			var P = new bool[n]; // POSITIVE SET: all indices which are currently not fixed (Negative set is simple this, but inverted)

			// x = P';
			x = matrixGenerator(n, 1);

			// w = Xty-XtX*x;
			w = new DoubleMatrix(Xty);
			var helper = matrixGenerator(n, 1);
			MatrixMath.Multiply(XtX, x, helper);
			MatrixMath.Subtract(w, helper, w);

			// set up iteration criterion
			int iter = 0;
			int itmax = 30 * n;

			// outer loop to put variables into set to hold positive coefficients
			// while any(Z) & any(w(ZZ) > tol)
			while (P.Any(ele => false == ele) && w.Any((r, c, ele) => false == P[r] && ele > tol))
			{
				// [wt, t] = max(w(ZZ));
				// t = ZZ(t);
				int t = -1; // INDEX
				double wt = double.NegativeInfinity;
				for (int i = 0; i < n; ++i)
				{
					if (!P[i])
					{
						if (w[i, 0] > wt)
						{
							wt = w[i, 0];
							t = i;
						}
					}
				}

				// P(1, t) = t;
				// Z(t) = 0;
				P[t] = true;

				// z(PP')=(Xty(PP)'/XtX(PP,PP)');
				var temp1 = Xty.SubMatrix(P, 0, matrixGenerator); // Xty(PP)'
																													//temp1.Transpose();
				var temp2 = XtX.SubMatrix(P, P, matrixGenerator);
				//temp2.Transpose();
				var solver = new DoubleLUDecomp(temp2);
				var temp3 = solver.Solve(temp1);
				var z = new DoubleMatrix(n, 1);
				for (int i = 0, ii = 0; i < n; ++i)
					z[i, 0] = P[i] ? temp3[ii++, 0] : 0;

				// inner loop to remove elements from the positive set which no longer belong

				while (z.Any((r, c, ele) => true == P[r] && ele <= tol) && iter < itmax)
				{
					++iter;
					// QQ = find((z <= tol) & P');
					//alpha = min(x(QQ)./ (x(QQ) - z(QQ)));
					double alpha = double.PositiveInfinity;
					for (int i = 0; i < n; ++i)
					{
						if ((z[i, 0] <= tol && true == P[i]))
						{
							alpha = Math.Min(alpha, x[i, 0] / (x[i, 0] - z[i, 0]));
						}
					}
					// x = x + alpha * (z - x);
					for (int i = 0; i < n; ++i)
						x[i, 0] += alpha * (z[i, 0] - x[i, 0]);

					// ij = find(abs(x) < tol & P' ~= 0);
					// Z(ij) = ij';
					// P(ij) = zeros(1, length(ij));

					for (int i = 0; i < n; ++i)
					{
						if (Math.Abs(x[i, 0]) < tol && P[i] == true)
						{
							P[i] = false;
						}
					}

					//PP = find(P);
					//ZZ = find(Z);
					//nzz = size(ZZ);
					//z(PP) = (Xty(PP)'/XtX(PP,PP)');

					var subXtyt = Xty.SubMatrix(P, 0, matrixGenerator);

					var subXtXt = XtX.SubMatrix(P, P, matrixGenerator);

					solver = new DoubleLUDecomp(subXtXt);
					var subResult = solver.Solve(subXtyt);

					for (int i = 0, ii = 0; i < n; ++i)
						z[i, 0] = P[i] ? subResult[ii++, 0] : 0;
				} // end inner loop

				MatrixMath.Copy(z, x);
				MatrixMath.Copy(Xty, w);
				var temp4 = matrixGenerator(Xty.Rows, Xty.Columns);
				MatrixMath.Multiply(XtX, x, temp4);
				MatrixMath.Subtract(w, temp4, w);
			}
		}
	}
}