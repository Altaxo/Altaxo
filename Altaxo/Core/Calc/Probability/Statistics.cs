using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Probability
{
	public static class Statistics
	{
		public static double Mean(this IROVector x)
		{
			double result = 0;
			for (int i = x.Length - 1; i >= 0; i--)
				result += x[i];

			return result / x.Length;
		}

		public static double Mean(this IROVector x, bool ignoreNaN)
		{
			double result = 0;
			int n = 0;
			if (ignoreNaN)
			{
				for (int i = x.Length - 1; i >= 0; i--)
				{
					double xx = x[i];
					if (double.IsNaN(xx))
						continue;

					result += xx;
					++n;
				}
			}
			else
			{
				for (int i = x.Length - 1; i >= 0; i--)
				{
					double xx = x[i];
					if (double.IsNaN(xx))
						throw new ArgumentException("NaN found in vector x at position " + i.ToString());
					result += xx;
					++n;
				}
			}

			return result / n;
		}


		public static double StandardDeviation(this IROVector x)
		{
			double mean = Mean(x);
			double sum = 0;
			for (int i = 0; i < x.Length; i++)
				sum += RMath.Pow2(x[i] - mean);

			return Math.Sqrt(sum/(x.Length-1));
		}

		public static double InterQuartileRange(this IROVector x)
		{
			return Math.Abs(Quantile(x, 0.75) - Quantile(x, 0.25));
		}

		/// <summary>
		/// The quantile value of x.
		/// </summary>
		/// <param name="x">Sorted array (in ascending order) of data. No check is made whether the array is sorted or contains missing data (NaNs).</param>
		/// <param name="f">The quantile [0,1].</param>
		/// <returns>The quantile value of the array of data.</returns>
		public static double Quantile(this IROVector x, double f)
		{
			return Quantile(x, f, x.Length, 1, false);
		}

		/// <summary>
		/// The quantile value of x.
		/// </summary>
		/// <param name="x">Sorted array (in ascending order) of data. No check is made whether the array is sorted or contains missing data (NaNs).</param>
		/// <param name="f">The quantile [0,1].</param>
		/// <param name="n">Number of values to test. Normally set to <c>x.Length</c>.</param>
		/// <param name="stride">Stride. Normally set to 1.</param>
		/// <param name="checkArray">If true, checks the array for missing values and whether the array is sorted. An exception is thrown if the array contains missing values or is not sorted.</param>
		/// <returns>The quantile value of the array of data.</returns>
		public static double Quantile(this IROVector x, double f, int n, int stride, bool checkArray)
		{
			if (checkArray)
			{
				double prev = double.NegativeInfinity;
				for (int i = 0; i < x.Length; i++)
				{
					double curr = x[i];
					if (!(curr >= prev))
						throw new ArgumentException("Array x is not sorted in ascending order at index " + i.ToString());
					if(double.IsNaN(curr))
						throw new ArgumentException("Array x contains missing values at index " + i.ToString());
					prev = curr;
				}
			}

			if(!f.IsInIntervalCC(0,1))
				throw new ArgumentException("f outside [0,1]");
			
			double index = f * (n - 1);
			int lhs = (int)index;
			 double delta = index - lhs;
			double result;

			if (n == 0)
				return 0.0;

		
			if (lhs == n - 1)
			{
				result = x[ lhs * stride];
			}
			else
			{
				result = (1 - delta) * x[ lhs * stride] + delta * x[ (lhs + 1) * stride];
			}

			return result;
		}

		public enum ConvolutionKernel
		{
			Gaussian,
			Epanechnikov,
			Rectangular,
			Triangular,
			Biweight,
			Cosine,
			Optcosine
		}

		public struct ProbabilityDensityResult
		{
			public IROVector X { get; set; }
			public IROVector Y { get; set; }
			public double Bandwidth { get; set; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="bw"></param>
		/// <param name="adjust"></param>
		/// <param name="kernel"></param>
		/// <param name="weights"></param>
		/// <param name="n"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="cut"></param>
		/// <param name="removeNaN"></param>
		/// <remarks>Adapted from the R-project (www.r-project.org), Version 2.72, file density.R</remarks>
		public static ProbabilityDensityResult ProbabilityDensity(
			this IROVector x,
			double bw,
			string bwSel,
			double adjust,
			ConvolutionKernel kernel,
			IROVector weights,
			double width,
			string widthSel,
			int n,
			double from,
			double to,
			double cut // default: 3
			)
		{
			double wsum;
			if (null == weights)
			{
				weights = VectorMath.GetConstantVector(1.0 / x.Length, x.Length);
				wsum = 1;
			}
			else
			{
				wsum = weights.Sum();
			}

			double totMass = 1;

			int n_user = n;
			n = Math.Max(n, 512);
			if (n > 512)
				n = BinaryMath.NextPowerOfTwo(n);


			if (bw.IsNaN() && !(width.IsNaN() && null == widthSel))
			{
				if (!width.IsNaN())
				{
					// S has width equal to the length of the support of the kernel
					// except for the gaussian where it is 4 * sd.
					// R has bw a multiple of the sd.
					double fac = 1;
					switch (kernel)
					{
						case ConvolutionKernel.Gaussian:
							fac = 4;
							break;
						case ConvolutionKernel.Rectangular:
							fac = 2 * Math.Sqrt(3);
							break;
						case ConvolutionKernel.Triangular:
							fac = 2 * Math.Sqrt(6);
							break;
						case ConvolutionKernel.Epanechnikov:
							fac = 2 * Math.Sqrt(5);
							break;
						case ConvolutionKernel.Biweight:
							fac = 2 * Math.Sqrt(7);
							break;
						case ConvolutionKernel.Cosine:
							fac = 2 / Math.Sqrt(1 / 3 - 2 / (Math.PI * Math.PI));
							break;
						case ConvolutionKernel.Optcosine:
							fac = 2 / Math.Sqrt(1 - 8 / (Math.PI * Math.PI));
							break;
						default:
							throw new ArgumentException("Unknown convolution kernel");
					}
					bw = width / fac;
				}
				else
				{
					bwSel = widthSel;
				}
			}

			if (null != bwSel)
			{
				if (x.Length < 2)
					throw new ArgumentException("need at least 2 points to select a bandwidth automatically");
				switch (bwSel.ToLowerInvariant())
				{
					case "nrd0":
						//nrd0 = bw.nrd0(x),
						break;
					case "nrd":
						//nrd = bw.nrd(x),
						break;
					case "ucv":
						//ucv = bw.ucv(x),
						break;
					case "bcv":
						//bcv = bw.bcv(x),
						break;
					case "sj":
						//sj = , "sj-ste" = bw.SJ(x, method="ste"),
						break;
					case "sj-dpi":
						//"sj-dpi" = bw.SJ(x, method="dpi"),
						break;
					default:
						throw new ArgumentException("Unknown bandwith selection rule: " + bwSel.ToString());
				}
			}


			if (!RMath.IsFinite(bw))
				throw new ArithmeticException("Bandwidth is not finite");

			bw = adjust * bw;

			if (!(bw > 0))
				throw new ArithmeticException("Bandwith is not positive");

			if (from.IsNaN())
				from = x.GetMinimum() - cut * bw;
			if (to.IsNaN())
				to = x.GetMaximum() + cut * bw;

			if (!RMath.IsFinite(from))
				throw new ArithmeticException("non-finite 'from'");
			if (!to.IsFinite())
				throw new ArithmeticException("non-finite 'to'");
			double lo = from - 4 * bw;
			double up = to + 4 * bw;

			var y = new DoubleVector(2 * n);
			MassDistribution(x, weights, lo, up, y, n);
			y.Multiply(totMass);

			var kords = new DoubleVector(2 * n);
			kords.FillWithLinearSequenceGivenByStartEnd(0, 2 * (up - lo));

			for (int i = n + 1, j = n - 1; j >= 0; i++, j--)
				kords[i] = -kords[j];

			switch (kernel)
			{
				case ConvolutionKernel.Gaussian:
					kords.Apply(new Probability.NormalDistribution(0, bw).PDF);
					break;
				case ConvolutionKernel.Rectangular:
					double a = bw * Math.Sqrt(3);
					kords.Apply(delegate(double xx) { return Math.Abs(xx) < a ? 0.5 / a : 0; });
					break;
				case ConvolutionKernel.Triangular:
					a = bw * Math.Sqrt(6);
					kords.Apply(delegate(double xx) { return Math.Abs(xx) < a ? (1 - Math.Abs(xx) / a) / a : 0; });
					break;
				case ConvolutionKernel.Epanechnikov:
					a = bw * Math.Sqrt(5);
					kords.Apply(delegate(double xx) { return Math.Abs(xx) < a ? 0.75 * (1 - RMath.Pow2(Math.Abs(xx) / a)) / a : 0; });
					break;
				case ConvolutionKernel.Biweight:
					a = bw * Math.Sqrt(7);
					kords.Apply(delegate(double xx) { return Math.Abs(xx) < a ? 15.0 / 16.0 * RMath.Pow2(1 - RMath.Pow2(Math.Abs(xx) / a)) / a : 0; });
					break;
				case ConvolutionKernel.Cosine:
					a = bw / Math.Sqrt(1.0 / 3 - 2 / RMath.Pow2(Math.PI));
					kords.Apply(delegate(double xx) { return Math.Abs(xx) < a ? (1 + Math.Cos(Math.PI * xx / a)) / (2 * a) : 0; });
					break;
				case ConvolutionKernel.Optcosine:
					a = bw / Math.Sqrt(1 - 8 / RMath.Pow2(Math.PI));
					kords.Apply(delegate(double xx) { return Math.Abs(xx) < a ? Math.PI / 4 * Math.Cos(Math.PI * xx / (2 * a)) / a : 0; });
					break;
				default:
					throw new ArgumentException("Unknown convolution kernel");
			}

			var result = new DoubleVector(2 * n);
			Fourier.FastHartleyTransform.CyclicRealConvolution(y.GetInternalData(), kords.GetInternalData(), result.GetInternalData(), 2 * n, null);
			y.Multiply(1.0 / (2 * n));
			VectorMath.Max(y, 0, y);
			var xords = VectorMath.CreateEquidistantSequenceByStartEndLength(lo, up, n);
			var xu = VectorMath.CreateEquidistantSequenceByStartEndLength(from, to, n_user);

			double[] res2 = new double[xu.Length];
			Interpolation.LinearInterpolation.Interpolate(xords, result, n, xu, xu.Length, 0, out res2);

			return new ProbabilityDensityResult() { X = xu, Y = VectorMath.ToROVector(res2), Bandwidth = bw };
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="xmass"></param>
		/// <param name="xlow"></param>
		/// <param name="xhigh"></param>
		/// <param name="y"></param>
		/// <remarks>Adapted from the R-project (www.r-project.org), Version 2.72, file massdist.c</remarks>
		public static void MassDistribution(
				this IROVector x,
				IROVector xmass, 
				double xlow, double xhigh,
				IVector y,
				int ny
			)
		{
			double fx, xdelta, xmi, xpos;   /* AB */
			int i, ix, ixmax, ixmin;
			int nx = x.Length;

			ixmin = 0;
			ixmax = ny - 2;
			/* AB: line deleted */
			xdelta = (xhigh - xlow) / (ny - 1);

			for (i = 0; i < ny; i++)
				y[i] = 0;

			for (i = 0; i < nx; i++)
			{
				if (x[i].IsFinite())
				{
					xpos = (x[i] - xlow) / xdelta;
					ix = (int)Math.Floor(xpos);
					fx = xpos - ix;
					xmi = xmass[i];   /* AB: new line  */
					if (ixmin <= ix && ix <= ixmax)
					{
						y[ix] += (1 - fx) * xmi;   /* AB */
						y[ix + 1] += fx * xmi; /* AB */
					}
					else if (ix == -1)
					{
						y[0] += fx * xmi;  /* AB */
					}
					else if (ix == ixmax + 1)
					{
						y[ix] += (1 - fx) * xmi;  /* AB */
					}
				}
			}

			/* AB: lines deleted */
		}
	}
}
