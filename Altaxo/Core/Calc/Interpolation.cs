// Original MatPack-1.7.3\Source\mpcurvebase.h
//                               mpcurvebase.cc
//                               mpfcspline.h
//                               mpfcspline.cc
//                               mpaspline.h
//                               mpaspline.cc
//                               mpbspline.h
//                               mpbspline.cc
//                               mpcspline.h
//                               mpcspline.cc
//                               mprspline.h
//                               mprspline.cc
//                               mpespline.h
//                               mpespline.cc
//                               mppolyinterpol.h
//                               mppolyinterpol.cc
//                               mpratinterpol.h
//                               mpratinterpol.cc
//                               mpgcvspline.h
//                               mpgcvspline.cc







using System;

namespace Altaxo.Calc.Interpolation
{
	public interface IVector
	{
		double this[int i]
		{
			get; set;
		}

		int Lo();
		int Hi();
		bool Empty();
		int LowerBound { get; }
		int UpperBound { get; }
		
		int Elements(); // change this later to length property

	}

	public class IntVector
	{
		protected int[] x;
		protected int lo = 0;
		protected int hi = -1;


		public int this[int i]
		{
			get { return x[i]; }
			set { x[i] = value; }
		}

		public void SetAllElementsTo(int val)
		{
			for(int i=lo; i<=hi; i++)
				x[i] = val;
		}

		public void Remove()
		{
			x=null;
			lo=0;
			hi=-1;
		}

		public void Resize(int lo, int hi)
		{
			if(x==null || hi>=x.Length)
			{
				x = new int[hi+1];
			}
			this.lo = lo;
			this.hi = hi;
		}
	}

	public class DoubleVector : IVector
	{
		protected double[] x;
		protected int lo = 0;
		protected int hi = -1;

		public DoubleVector()
		{
		}

		public DoubleVector(int lowerBound, int upperBound)
		{
			this.Resize(lowerBound,upperBound);
		}

		public double this[int i]
		{
			get { return x[i]; }
			set { x[i] = value; }
		}

		public int Lo() { return lo; }
		public int Hi() { return hi; }
		public bool Empty() { return hi<lo; }
		public int LowerBound { get { return lo; }}
		public int UpperBound { get { return hi;} }

		public void Remove()
		{
			x=null;
			lo=0;
			hi=-1;
		}
		public void Resize(int lo, int hi)
		{
			if(x==null || hi>=x.Length)
			{
				x = new double[hi+1];
			}
			this.lo = lo;
			this.hi = hi;
		}
		public int Elements() // change this later to length property
		{
			return hi-lo+1;
		}

		public void CopyFrom(IVector a)
		{
			Resize(a.Lo(),a.Hi());
			for(int i=a.Lo();i<a.Hi();i++)
				x[i] = a[i];
		}
	}

	public interface IScene
	{
	}


	

	#region CurveBase
	//----------------------------------------------------------------------------//
	//
	// class CurveBase is currently inherited by:
	//
	//   class FritschCarlsonCubicSpline
	//   class AkimaCubicSpline
	//   class BezierCubicSpline
	//   class CardinalCubicSpline
	//   class RationalCubicSpline
	//   class ExponentialSpline
	//   class PolynomialInterpolation
	//   class RationalInterpolation
	//
	//----------------------------------------------------------------------------//

	public abstract class CurveBase
	{

		/// <summary>
		/// Represents the smallest number where 1+DBL_EPSILON is not equal to 1.
		/// </summary>
		public const double DBL_EPSILON = 2.2204460492503131e-016;

		public enum BoundaryConditions 
		{ 
			Natural = 0, 		// natural boundaries, zero 2nd deriv.
			FiniteDifferences, 	// finite differences for 1st derivatives
			Supply1stDerivative,	// user supplied f'(x_lo), f'(x_hi)
			Supply2ndDerivative,	// user supplied f''(x_lo), f''(x_hi)
			Periodic  		// periodic boundaries (NOT YET IMPLEMENTED)
		};

		public enum Parametrization 
		{	// curve parametrization methods
			No = 0,			// don't parametrize (default)
			Norm2,			// use sqrt(dx^2+dy^2)
			SqrNorm2,			// use (dx^2+dy^2)
			Norm1 			// use |dx| + |dy|
		};


		protected IVector x;
		protected IVector y;
  
		#region Helper functions
		protected static double sqr(double x)
		{
			return x*x;
		}

		/// <summary>
		/// Return True if vectors have the same index range, False otherwise.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		protected static bool MatchingIndexRange (IVector a, IVector b)
		{
			return (a.Lo() == b.Lo() && a.Hi() == b.Hi());
		}
		#endregion

		#region FindInterval


		//----------------------------------------------------------------------------//
		//
		// int MpCurveBase::FindIntervall (double u, const Vector &x) const
		//
		// Find index of largest element in the increasingly ordered vector x, 
		// which is smaller than u. If u is smaller than the smallest value in 
		// the vector then the lowest index minus one is returned. 
		// A fast binary search is performed.
		// Note, that the vector must be strictly increasing.
		//
		//----------------------------------------------------------------------------//

		public static int FindIntervall (double u, IVector x) 
		{
			int i, j;
			int lo =  x.LowerBound;
			int hi = x.UpperBound;
			if (u < x[lo]) 
			{
				i = lo - 1;	// attention: return index below smallest index
			} 
			else if (u >= x[hi]) 
			{
				i = hi;	// attention: return highest index
			} 
			else 
			{
				i = lo;
				j = hi;
				do 
				{
					int k = (i + j) / 2;
					if (u < x[k])  j = k;
					if (u >= x[k]) i = k;
				} while (j > i + 1);
			}
			return i;
		}


		#endregion
		

		




		//----------------------------------------------------------------------------//
		//
		// double MpCurveBase::CubicSplineHorner (double u, 
		//                                        const Vector &x, const Vector &y, 
		//			                  const Vector &y1, const Vector &y2, 
		//			                  const Vector &y3) const
		//
		// Return the interpolation value P(u) for a piecewise cubic curve determined
		// by the abscissa vector x, the ordinate vector y, the 1st derivative
		// vector y1, the 2nd derivative vector y2, and the 3rd derivative vector y3,
		// using the Horner scheme. 
		// All vectors must have conformant dimenions.
		// The abscissa x(i) values must be strictly increasing.
		// In the special case of empty data vectors (x,y) a value of 0.0 is returned.
		//
		// This subroutine evaluates the function
		//
		//    P(u) = y(i) + dx * (y1(i) + dx * (y2(i) + dx * y3(i)))
		//
		// where  x(i) <= u < x(i+1) and dx = u - x(i), using Horner's rule
		//
		//    lo <= i <= hi is the index range of the vectors.
		//    if  u <  x(lo) then  i = lo  is used.
		//    if  u >= x(hi) then  i = hi  is used.
		//
		// Input arguments:
		// ----------------
		//
		//    double u       = the abscissa at which the interpolation is to be evaluated
		//
		//    Vector &x
		//    Vector &y      = the vectors (lo,hi) of data abscissas and ordinates
		//
		//    Vector &y1
		//    Vector &y2
		//    Vector &y3     = vectors (lo,hi) of coefficients
		//                     y1(i) contains the 1st derivative y'(x(i))
		//                     y2(i) contains the 2nd derivative y''(x(i))
		//
		//  Notes:
		//  ------
		//
		//    A fast binary search is performed to determine the proper interval.
		//
		//---------------------------------------------------
		public double CubicSplineHorner (double u,
			IVector x,
			IVector y, 
			IVector y1,
			IVector y2, 
			IVector y3)
		{
			// special case that there are no data. Return 0.0.
			if (x.Empty()) return 0.0;

			int i = FindIntervall(u,x);  
			if (i  < x.Lo()) i = x.Lo();  // extrapolate to the left
			if (i == x.Hi()) i--; 	// extrapolate to the right
			double dx = u - x[i];
			return (y[i] + dx * (y1[i] + dx * (y2[i] + dx * y3[i])));
		}


		//----------------------------------------------------------------------------//
		//
		// void MpCurveBase::CubicSplineCoefficients (const Vector &x, 
		//                                            const Vector &y, 
		//				              const Vector &y1, 
		//				              Vector &y2, Vector &y3) const
		//
		// Calculate the spline coefficients y2(i) and y3(i) for a natural cubic
		// spline, given the abscissa x(i), the ordinate y(i), and the 1st 
		// derivative y(i).
		//
		// The spline interpolation can then be evaluated using Horner's rule
		//
		//      P(u) = y(i) + dx * (y1(i) + dx * (y2(i) + dx * y3(i)))
		//
		// where  x(i) <= u < x(i+1) and dx = u - x(i).
		//
		//----------------------------------------------------------------------------//

		public void CubicSplineCoefficients (IVector x, 
			IVector y, 
			IVector y1, 
			IVector y2,
			IVector y3)
		{
			int lo = x.Lo(), 
				hi = x.Hi();

			for (int i = lo; i < hi; i++) 
			{
				double h  = x[i+1] - x[i],
					mi = (y[i+1] - y[i]) / h;
				y2[i] = (3 * mi - 2 * y1[i] - y1[i+1]) / h;
				y3[i] = (y1[i] + y1[i+1] - 2 * mi) / (h * h);
			}

			y2[hi] = y3[hi] = 0.0;
		}

		
		public abstract int Interpolate (IVector x, IVector y);
		public abstract double GetX (double x);
		public abstract double GetY (double x);


	


		//----------------------------------------------------------------------------//
		// void MpCurveBase::Parametrize (const Vector& x, const Vector& y, 
		// 	  			  Vector& t, int parametrization) const
		//
		// Curve length parametrization. Returns the accumulated "distances"
		// between the points (x(i),y(i)) and (x(i+1),y(i+1)) in t(i+1) 
		// for i = lo ... hi. t(lo) = 0.0 always. 
		//
		// The way of parametrization is controlled by the parameter parametrization.
		// Parametrizes curve length using: 
		//
		//    |dx| + |dy|       if  parametrization = Norm1
		//    sqrt(dx^2+dy^2)   if  parametrization = Norm2
		//    (dx^2+dy^2)       if  parametrization = SqrNorm2
		//
		// Parametrization using Norm2 usually gives the best results.
		//
		//----------------------------------------------------------------------------//

		public virtual void Parametrize (IVector x, IVector y, IVector t, Parametrization parametrization)
		{
			int lo = x.Lo(),
				hi = x.Hi(),
				i; 

			switch (parametrization)
			{
				case Parametrization.Norm1:  
					for (i = lo+1, t[lo] = 0.0; i <= hi; i++)
						t[i] = t[i-1] + Math.Abs(x[i]-x[i-1]) + Math.Abs(y[i]-y[i-1]);
					break;

				case Parametrization.Norm2:  
					for (i = lo+1, t[lo] = 0.0; i <= hi; i++)
						t[i] = t[i-1] + BasicFunctions.hypot( x[i]-x[i-1],y[i]-y[i-1] );
					break;

				case Parametrization.SqrNorm2:  
					for (i = lo+1, t[lo] = 0.0; i <= hi; i++)
						t[i] = t[i-1] + sqr(x[i]-x[i-1]) + sqr(y[i]-y[i-1]);
					break;

				default: 
					throw new System.ArgumentException("illegal value for parametrization method");
					
			}


		

		}

		#region GetResolution and DrawCurve
#if IncludeScene


		//----------------------------------------------------------------------------//
		//
		// int MpCurveBase::GetResolution (const Scene& scene, 
		//			  	   double x1, double y1, 
		//				   double x2, double y2) const
		//
		// Calculate the number of intermediate points neccessary to get a 
		// smooth appearance when drawing a line segment between (x1,y1) and (x2,y2).
		//
		//----------------------------------------------------------------------------//

		int GetResolution (IScene scene, 
			double x1, double y1, double x2, double y2)
	{
  int r = scene.curve.resolution + 2;
  Pixel2D p1( scene.Map(x1,y1)), p2(scene.Map(x2,y2) );
  return 1 + int( (r + abs(p2.px-p1.px) + abs(p2.py-p1.py)) / (2 * r) );
}

		//----------------------------------------------------------------------------//
		//
		// void MpCurveBase::DrawCurve (Scene &scene, double xlo, double xhi)
		//
		// Draws an interpolation curve between the abscissa values xlo and xhi.
		// It calls the virtual methods MpCurveBase::GetX() and GetY() to obtain the
		// interpolation values. Note, that before method DrawCurve() can be called
		// the method Interpolate() must have been called. Otherwise, not interpolation
		// is available.
		//
		//  Input arguments:
		//  ----------------
		//   
		//    Scene  &scene  = the scene the drawing is directed to
		//
		//    double xlo
		//    double xhi     = the drawing range of the curve is [xlo,xhi]
		//
		//----------------------------------------------------------------------------//

		public void DrawCurve (IScene scene, double x_lo, double x_hi)
		{
			if (! scene.IsOpen()) 
				Matpack.Error("MpCurveBase::DrawCurve: scene is not open for drawing");

			// nothing to draw if zero or one element
			if (x->Elements() < 2) return;

			// Find index of the element in the abscissa vector x, that is smaller
			// than the lower (upper) value xlo (xhi) of the drawing range. If xlo is
			// smaller than the lowest abscissa value the lowest index minus one is
			// returned.
			int i_lo = FindIntervall(xlo,*x),
				i_hi = FindIntervall(xhi,*x);

			// Interpolation values for the boundaries of the drawing range [xlo,xhi]
			double ylo = GetY(xlo),
				yhi = GetY(xhi);

			int k;
			double x0,t,delta;

			scene.MoveTo(xlo,ylo);
			k = GetResolution(scene,xlo,ylo, (*x)(i_lo+1),(*y)(i_lo+1));
			delta = ((*x)(i_lo+1) - xlo) / k;
			for (int j = 0; j < k; j++) 
			{
				t = xlo + j * delta;
				scene.LineTo( GetX(t), GetY(t) );
			}

			for (int i = i_lo+1; i < i_hi; i++) 
			{
				x0 = (*x)(i);
				k = GetResolution(scene,x0,(*y)(i),(*x)(i+1),(*y)(i+1));
				delta = ((*x)(i+1)-x0) / k;
				for (int j = 0; j < k; j++) 
				{
					t = x0 + j * delta;
					scene.LineTo( GetX(t), GetY(t) );
				}
			}

			x0 = (*x)(i_hi);
			k = GetResolution(scene,x0,(*y)(i_hi),xhi,yhi);
			delta = (xhi - x0) / k;
			for (int j = 0; j < k; j++) 
			{
				t = x0 + j * delta;
				scene.LineTo( GetX(t), GetY(t) );
			}  

			// don't forget last point
			scene.LineTo(xhi,yhi);

			// flush graphics buffer 
			scene.Flush();  
		}


#endif
		#endregion


	}

	#endregion

	#region LinearInterpolation

	/// <summary>
	/// Contains static methods for linear interpolation of data.
	/// </summary>
	public class LinearInterpolation
	{
	
		public static int GetNextIndexOfValidPair(IVector xcol, IVector ycol, int sourceLength, int currentIndex)
		{
			for(int sourceIndex=currentIndex;sourceIndex<sourceLength;sourceIndex++)
			{
				if(!double.IsNaN(xcol[sourceIndex]) && !double.IsNaN(ycol[sourceIndex]))
					return sourceIndex;
			}

			return -1;
		}

		public static double Interpolate(double x, double x0, double x1, double y0, double y1)
		{
			double r = (x-x0)/(x1-x0);
			return (1-r)*y0 + r*y1;
		}

		public static string Interpolate(
			IVector xcol,
			IVector ycol,
			int sourceLength,
			double xstart, double xincrement, int numberOfValues,
			double yOutsideOfBounds,
			out double[] resultCol)
		{
			resultCol = new double[numberOfValues];
			
			int currentIndex = GetNextIndexOfValidPair(xcol,ycol,sourceLength, 0);
			if(currentIndex<0)
				return "The two columns don't contain a valid pair of values";

			int nextIndex = GetNextIndexOfValidPair(xcol,ycol,sourceLength,currentIndex+1);
			if(nextIndex<0)
				return "The two columns contain only one valid pair of values, but at least two valid pairs are neccessary!";
	

			double x_current = xcol[currentIndex];
			double x_next = xcol[nextIndex];

			int resultIndex=0;
			
			// handles values before interpolation range
			for(resultIndex=0;resultIndex<numberOfValues;resultIndex++)
			{
				double x_result = xstart + resultIndex* xincrement;
				if(x_result>=x_current)
					break;

				resultCol[resultIndex] = yOutsideOfBounds;
			}

			// handle values in the interpolation range
			for(;resultIndex<numberOfValues;resultIndex++)
			{
				double x_result = xstart + resultIndex* xincrement;

			tryinterpolation:
				if(x_result>=x_current && x_result<=x_next)
				{
					resultCol[resultIndex] = Interpolate(x_result, x_current, x_next, ycol[currentIndex],ycol[nextIndex]);
				}
				else
				{
					currentIndex = nextIndex;
					x_current    = x_next;
					nextIndex = GetNextIndexOfValidPair(xcol,ycol,sourceLength, currentIndex+1);
					if(nextIndex<0)
						break;

					x_next = xcol[nextIndex];
					goto tryinterpolation;
				}
			}

			// handle values behind the interplation range
			for(;resultIndex<numberOfValues;resultIndex++)
			{
				resultCol[resultIndex] = yOutsideOfBounds;
			}

			return null;
		}
	}

	#endregion

	#region FritschCarlsonCubicSpline

	class FritschCarlsonCubicSpline : CurveBase
	{
	
		protected DoubleVector y1 = new DoubleVector();
		protected DoubleVector y2 = new DoubleVector();
		protected DoubleVector y3 = new DoubleVector();


		//----------------------------------------------------------------------------//
		//
		// int MpFritschCarlsonCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Calculate the Fritsch-Carlson monotone cubic spline interpolation for the 
		// given abscissa vector x and ordinate vector y. 
		// All vectors must have conformant dimenions.
		// The abscissa vector must be strictly increasing.
		//
		// The Fritsch-Carlson interpolation produces a neat monotone
		// piecewise cubic curve, which is especially suited for the
		// presentation of scientific data. 
		// This is the state of the art to create curves that preserve
		// monotonicity, although it is not so well known as Akima's
		// interpolation. The commonly used Akima interpolation doesn't 
		// produce so pleasant results.
		//
		// Reference:
		//    F.N.Fritsch,R.E.Carlson: Monotone Piecewise Cubic
		//    Interpolation, SIAM J. Numer. Anal. Vol 17, No. 2, 
		//    April 1980
		//
		// Copyright (C) 1991-1998 by Berndt M. Gammel
		// 
		//----------------------------------------------------------------------------//

		public override int Interpolate(IVector x, IVector y)
		{
			// check input parameters

			if ( ! MatchingIndexRange(x,y) )
				throw new ArgumentException("index range mismatch of vectors");
   
			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Empty()) 
			{
				y1.Remove();
				y2.Remove();
				y3.Remove();
				return 0; // ok
			}

			int lo = x.Lo(),
				hi = x.Hi();
  
			// Resize the auxilliary vectors. Note, that there is no reallocation if the
			// vector already has the appropriate dimension.
			y1.Resize(lo,hi);
			y2.Resize(lo,hi);
			y3.Resize(lo,hi);

			if (x.Elements() == 1) 
			{
    
				// default derivative is 0.0
				y1[lo] = y2[lo] = y3[lo] = 0.0;

			} 
			else if (x.Elements() == 2) 
			{
    
				// set derivatives for a line
				y1[lo] = y1[hi] = (y[hi]-y[lo]) / (x[hi]-x[lo]);
				y2[lo] = y2[hi] = 
					y3[lo] = y3[hi] = 0.0;

			} 
			else 
			{ // three or more points
    
				// initial guess derivative vector 
				y1[lo] = deriv1(x,y,lo+1,-1);
				y1[hi] = deriv1(x,y,hi-1,1);
				for (int i = lo+1; i < hi; i++)
					y1[i] = deriv2(x,y,i);

				if (x.Elements() > 3) 
				{

					// adjust derivatives at boundaries
					if (y1[lo] * y1[lo+1] < 0) y1[lo] = 0;
					if (y1[hi] * y1[hi-1] < 0) y1[hi] = 0;
      
					// adjustment of cubic interpolant 
					fritsch(x,y,y1);
				}

				// calculate remaining spline coefficients y2(i) and y3(i)
				CubicSplineCoefficients(x,y,y1,y2,y3);
    
			}

			return 0; // ok
		}

		public override double GetX (double u)
		{
			return u;
		}
		
		public override double GetY (double u) 
		{
			return CubicSplineHorner(u,x,y,y1,y2,y3);
		}

		#region deriv1

		//-----------------------------------------------------------------------------//
		// 
		// Initial derivatives at boundaries of data set using
		// quadratic Newton interpolation
		// 
		//-----------------------------------------------------------------------------//

		static double deriv1 (IVector x, IVector y, int i, int sgn)
		{
			double di,dis,di2,his;
			int i1,i2;

			i1  = i+1;
			i2  = i-1;
			his = x[i1]-x[i2];
			dis = (y[i1]-y[i2]) / his;
			di  = (y[i1]-y[i]) / (x[i1]-x[i]);
			di2 = (di-dis) / (x[i]-x[i2]);
			return dis + sgn * di2 * his;
		}
		#endregion

		
		//-----------------------------------------------------------------------------//
		// 
		// Initial derivatives within data set using
		// quadratic Newton interpolation
		// 
		//-----------------------------------------------------------------------------//


		static double deriv2 (IVector x, IVector y, int i)
		{
			double di0,di1,di2,hi0;
			int i1,i2;

			i1  = i+1;
			i2  = i-1;
			hi0 = x[i]-x[i2];
			di0 = (y[i]-y[i2]) / hi0;
			di1 = (y[i1]-y[i]) / (x[i1]-x[i]);
			di2 = (di1-di0) / (x[i1]-x[i2]);
			return di0 + di2 * hi0;
		}


		//-----------------------------------------------------------------------------//
		// 
		// Fritsch-Carlson iteration to adjust the monotone
		// cubic interpolant. The iteration converges with cubic order.
		// 
		//-----------------------------------------------------------------------------//

		static void fritsch (IVector x, IVector y, IVector d)
		{
			int i,i1;
			bool stop;
			double d1,r2,t;

			const int max_loop = 20; // should never happen! Note, that currently it
			// can happen when the curve is not strictly 
			// monotone. In future this case should be handled
			// more gracefully without wasting CPU time.
			int loop = 0;

			do 
			{
				stop = true;
				for (i = x.Lo(); i < x.Hi(); i++) 
				{
					i1 = i + 1;
					d1 = (y[i1]-y[i]) / (x[i1]-x[i]);
					if (d1 == 0.0)
						d[i] = d[i1] = 0.0;
					else 
					{
						t = d[i]/d1;
						r2 = t*t;
						t = d[i1]/d1;
						r2 += t*t;
						if (r2 > 9.0) 
						{
							t = 3.0 / Math.Sqrt(r2);
							d[i]  *= t;
							d[i1] *= t;
							stop = false;
						}
					}
				}
			} while (!stop && ++loop < max_loop);
		}


	}

	#endregion // MpFrischCarlsonCubicSpline

	#region AkimaCubicSpline
	class AkimaCubicSpline : CurveBase
	{
		protected	DoubleVector y1 = new DoubleVector();
		protected	DoubleVector  y2 = new DoubleVector();
		protected	DoubleVector y3 = new DoubleVector();
		

		private double m(int i)
		{
			return ((y[i+1]-y[i]) / (x[i+1]-x[i]));
		}

		//----------------------------------------------------------------------------//
		//
		// int MpAkimaCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Calculate the Akima cubic spline interpolation for the given abscissa
		// vector x and ordinate vector y. 
		// All vectors must have conformant dimenions.
		// The abscissa vector must be strictly increasing.
		//----------------------------------------------------------------------------//

		public override int Interpolate (IVector x, IVector y)
		{
			// check input parameters

			if ( ! MatchingIndexRange(x,y) )
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Empty()) 
			{
				y1.Remove();
				y2.Remove();
				y3.Remove();
				return 0; // ok
			}
     
			int lo = x.Lo(), lo1 = lo+1, lo2 = lo+2, 
				hi = x.Hi(), hi1 = hi-1, hi2 = hi-2, hi3 = hi-3;

			// Resize the auxilliary vectors. Note, that there is no reallocation if the
			// vectors already have the appropriate dimensions.
			y1.Resize(lo,hi);
			y2.Resize(lo,hi);
			y3.Resize(lo,hi);

			if (x.Elements() == 1) 
			{
    
				// default derivatives are 0.0
				y1[lo] = y2[lo] = y3[lo] = 0.0;

			} 
			else if (x.Elements() == 2) 
			{
    
				// set derivatives for a line
				y1[lo] = y1[hi] = (y[hi]-y[lo]) / (x[hi]-x[lo]);
				y2[lo] = y2[hi] = 
					y3[lo] = y3[hi] = 0.0;

			} 
			else 
			{ // three or more elements - do Akima interpolation

				double num, den,
					m_m1, m_m2, m_p1, m_p2,
					x_m1, x_m2, x_p1, x_p2,
					y_m1, y_m2, y_p1, y_p2;

				// short form to save some typing
				// #define m(i) ((y(i+1)-y(i)) / (x(i+1)-x(i)))

				// interpolate the missing points
				x_m1 = x[lo] + x[lo1] - x[lo2]; 
				y_m1 = (x[lo]-x_m1) * (m(lo1) - 2 * m(lo)) + y[lo];
				m_m1 = (y[lo]-y_m1)/(x[lo]-x_m1);
    
				x_m2 = 2 * x[lo] - x[lo2];
				y_m2 = (x_m1-x_m2) * (m(lo) - 2 * m_m1) + y_m1;
				m_m2 = (y_m1-y_m2)/(x_m1-x_m2);
    
				x_p1 = x[hi] + x[hi1] - x[hi2];
				y_p1 = (2 * m(hi1) - m(hi2)) * (x_p1 - x[hi]) + y[hi];
				m_p1 = (y_p1-y[hi])/(x_p1-x[hi]);
    
				x_p2 = 2 * x[hi] - x[hi2];
				y_p2 = (2 * m_p1 - m(hi1)) * (x_p2 - x_p1) + y_p1;
				m_p2 = (y_p2-y_p1)/(x_p2-x_p1);
    
				// i = 0
				num = Math.Abs(m(lo1) - m(lo)) * m_m1 + Math.Abs(m_m1 - m_m2) * m(lo);
				den = Math.Abs(m(lo1) - m(lo)) + Math.Abs(m_m1 - m_m2);  	
				y1[lo] = (den != 0.0) ? num / den : 0.0;
    
				// i = 1 
				if (x.Elements() > 3) 
				{

					num = Math.Abs(m(lo2) - m(lo1)) * m(lo) + Math.Abs(m(lo) - m_m1) * m(lo1);
					den = Math.Abs(m(lo2) - m(lo1)) + Math.Abs(m(lo) - m_m1);
					y1[lo1] = (den != 0.0) ? num / den :  0.0;
      
					for (int i = lo2; i < hi1; i++) 
					{
						double mip1 = m(i+1),
							mi   = m(i),
							mim1 = m(i-1),
							mim2 = m(i-2);
						num = Math.Abs(mip1 - mi) * mim1 + Math.Abs(mim1 - mim2) * mi;
						den = Math.Abs(mip1 - mi) + Math.Abs(mim1 - mim2);
						y1[i] = (den != 0.0) ? num / den : 0.0;
					}
      
					// i = n - 2
					num = Math.Abs(m_p1 - m(hi1)) * m(hi2) + Math.Abs(m(hi2) - m(hi3)) * m(hi1);
					den = Math.Abs(m_p1 - m(hi1)) + Math.Abs(m(hi2) - m(hi3));
					y1[hi1] = (den != 0.0) ? num / den : 0.0;

				} 
				else 
				{ // exactly three elements
					num = m(lo);
					den = (m(lo1) - num) / (x[lo2] - x[lo]);
					y1[lo1] = num + den * (x[lo1] - x[lo]);
				}

				// i = n - 1
				num = Math.Abs(m_p2 - m_p1) * m(hi1) + Math.Abs(m(hi1) - m(hi2)) * m_p1;
				den = Math.Abs(m_p2 - m_p1) + Math.Abs(m(hi1) - m(hi2));
				y1[hi] = (den != 0.0) ? num / den : 0.0;
    
				// calculate remaining spline coefficients y2(i) and y3(i)
				CubicSplineCoefficients(x,y,y1,y2,y3);
			}

			return 0; // ok
		}
		public override double GetX (double u)
		{
			return u;
		}
		public override double GetY (double u)
		{
			return CubicSplineHorner(u,x,y,y1,y2,y3);
		}
	}

	#endregion MpAkimaCubicSpline

	#region BezierCubicSpline

	public class BezierCubicSpline : CurveBase
	{

		//----------------------------------------------------------------------------//
		//
		// int MpBezierCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Calculate the Bezier cubic spline interpolation for the 
		// given abscissa vector x and ordinate vector y. 
		// All vectors must have conformant dimensions.
		//
		//
		//                              1   / -1.0  3.0 -3.0  1.0 \   / P1 \  
		//  BSpline(t) = (t^3 t^2 t 1) --- |   3.0 -6.0  3.0  0.0  | |  P2  | = T M G
		//                              6  |  -3.0  0.0  3.0  0.0  | |  P3  |
		//                                  \  1.0  4.0  1.0  0.0 /   \ P4 /  
		//
		//  T is the polynomial basis vector
		//  M is the basis matrix of the Bezier spline
		//  G is the geometry vector of the control points
		//
		//--------------------------------------------------------------------------
		public override int Interpolate (IVector x, IVector y)
		{
			// verify index range
			if ( ! MatchingIndexRange(x,y) ) 
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			return 0; // ok
		}

		public override double GetX (double t)
		{
			int lo = x.Lo(), 
				hi = x.Hi(), 
				i  = FindIntervall(t,x);  

			if (i < lo || i >= hi || hi-lo == 1) 
			{
				// linear extrapolation and interpolation for 2 points
				return t;

			} 
			else if (i == lo) 
			{
				i = lo;
				double u  = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(u2/6.0-1.0),
					c2 = u*(1.0-u2/3.0),
					c3 = u*u2/6.0;
				return c1*x[i] + c2*x[i+1] + c3*x[i+2];

			} 
			else if (i == hi-1) 
			{
				i = hi-1;
				double u  = (x[i+1] - t) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(u2/6.0-1.0),
					c2 = u*(1.0-u2/3.0),
					c3 = u*u2/6.0;
				return c1*x[i+1] + c2*x[i] + c3*x[i-1];

			} 
			else 
			{
				double u  = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					u3 = 1.0-u,
					u4 = u3*u3,
					c1 = u3*u4/6.0,
					c2 = u2*(u/2.0-1.0)+4.0/6.0,
					c3 = u*(1.0+u*u3)/2.0+1.0/6.0,
					c4 = u*u2/6.0;
				return c1*x[i-1] + c2*x[i] + c3*x[i+1] + c4*x[i+2];
			}
		}
		public override double GetY (double t)
		{
			int lo = x.Lo(), 
				hi = x.Hi(), 
				i  = FindIntervall(t,x);  

			if (i < lo || hi-lo == 1) 
			{
				// linear extrapolation and interpolation for 2 points
				return  y[lo] + (t-x[lo]) * (y[lo+1]-y[lo])/(x[lo+1]-x[lo]);

			} 
			else if (i == lo) 
			{
				i = lo;
				double u = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(u2/6.0-1.0),
					c2 = u*(1.0-u2/3.0),
					c3 = u*u2/6.0;
				return c1*y[i] + c2*y[i+1] + c3*y[i+2];

			} 
			else if (i >= hi) 
			{
				// linear extrapolation
				return  y[hi] + (t-x[hi]) * (y[hi]-y[hi-1])/(x[hi]-x[hi-1]);

			} 
			else if (i == hi-1) 
			{
				i = hi-1;
				double u  = (x[i+1] - t) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(u2/6.0-1.0),
					c2 = u*(1.0-u2/3.0),
					c3 = u*u2/6.0;
				return c1*y[i+1] + c2*y[i] + c3*y[i-1];

			} 
			else 
			{
				double u  = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					u3 = 1.0-u,
					u4 = u3*u3,
					c1 = u3*u4/6.0,
					c2 = u2*(u/2.0-1.0)+4.0/6.0,
					c3 = u*(1.0+u*u3)/2.0+1.0/6.0,
					c4 = u*u2/6.0;
				return c1*y[i-1] + c2*y[i] + c3*y[i+1] + c4*y[i+2];
			}
		}


		#region Scene drawing
#if IncludeScene

		//----------------------------------------------------------------------------//
//
//  compute a cubic Bezier polynomial for the points
//  (x[0],y[0])...(x[3],y[3]) and plot it from (x[1],y[1]) to (x[2],y[2])
//
//----------------------------------------------------------------------------//

		protected void Draw4 (Scene& scene, const double *x, const double *y, int &first)
{
  double u,u2,u3,u4,c1,c2,c3,c4,rx,ry;
  int n = GetResolution(scene,x[1],y[1],x[2],y[2]);

  for (int i = 0; i <= n; i++) {
    u  = (double) i / n;
    u2 = u*u;
    u3 = 1.0-u;
    u4 = u3*u3;

    c1 = u3*u4/6.0;
    c2 = u2*(u/2.0-1.0)+4.0/6.0;
    c3 = u*(1.0+u*u3)/2.0+1.0/6.0;
    c4 = u*u2/6.0;
    
    rx = c1*x[0]+c2*x[1]+c3*x[2]+c4*x[3];
    ry = c1*y[0]+c2*y[1]+c3*y[2]+c4*y[3];
    if (first) {
      scene.MoveTo(rx,ry);
      first = false;
    } else
      scene.LineTo(rx,ry);
  }
}

//----------------------------------------------------------------------------//
//
// void MpBezierCubicSpline::DrawClosedCurve (Scene &scene)
//
// Special method to draw a closed curve
//
//----------------------------------------------------------------------------//

void DrawClosedCurve (Scene &scene)
{
  if ( ! scene.IsOpen() ) 
    Matpack.Error("MpBezierCubicSpline::DrawClosedCurve: "
		  "scene is not open for drawing");

  // get index range
  int lo = x->Lo(),
      hi = x->Hi();

  // nothing to draw - one point
  if ( lo >= hi ) return;

  // special case - two points
  if ( hi == lo+1 ) {
    scene.Line((*x)(lo),(*y)(lo),(*x)(hi),(*y)(hi));
    return;
  }

  // three or more points 

  int first = true;

  double xx[6],yy[6];
  
  for (int i = 0; i <= 2; i++) {
    xx[i] = (*x)[hi-2+i];
    yy[i] = (*y)[hi-2+i];
  }
  
  for (int i = 3; i <= 5; i++) {
    xx[i] = (*x)[lo+i-3];
    yy[i] = (*y)[lo+i-3];
  }
  
  for (int i = lo; i <= hi-3; i++)
    Draw4(scene,&(*x)(i),&(*y)(i),first);
  
  for (int i = 0; i <= 2; i++)
    Draw4(scene,&xx[i],&yy[i],first);
  
  // flush graphics buffer
  scene.Flush();  
}
		
#endif
		#endregion
	};

	#endregion

	#region CardinalCubicSpline

	
	public class CardinalCubicSpline : CurveBase
	{
			
		//----------------------------------------------------------------------------//
		//
		// int MpBezierCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Calculate the Cardinal cubic spline interpolation for the 
		// given abscissa vector x and ordinate vector y. 
		// All vectors must have conformant dimensions.
		//
		//                              / -0.5  1.5 -1.5  0.5 \   / P1 \  
		//  CSpline(t) = (t^3 t^2 t 1) |   1.0 -2.5  2.0 -0.5  | |  P2  | = T M G  
		//                             |  -0.5  0.0  0.5  0.0  | |  P3  |  
		//                              \  0.0  1.0  0.0  0.0 /   \ P4 /  
		//
		//  T is the polynomial basis vector
		//  M is the basis matrix of the Cardinal spline
		//  G is the geometry vector of the control points
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate (IVector x, IVector y)
		{
			// verify index range
			if ( ! MatchingIndexRange(x,y) ) 
				throw new System.ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			return 0; // ok
		}

		//----------------------------------------------------------------------------//

		public override double GetX (double t)
		{
			int lo = x.Lo(), 
				hi = x.Hi(), 
				i  = FindIntervall(t,x);  

			if (i < lo || i >= hi || hi-lo == 1) 
			{
				// linear extrapolation and interpolation for 2 points
				return t;

			} 
			else if (i == lo) 
			{
				i = lo;
				double u  = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(-1.0+u*(u-1.0)/2.0),
					c2 = u*(1.0+u*(1.0-u)),
					c3 = u2*(u-1.0)/2.0;
				return c1*x[i] + c2*x[i+1] + c3*x[i+2];

			} 
			else if (i == hi-1) 
			{
				i = hi-1;
				double u  = (x[i+1] - t) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(-1.0+u*(u-1.0)/2.0),
					c2 = u*(1.0+u*(1.0-u)),
					c3 = u2*(u-1.0)/2.0;
				return c1*x[i+1] + c2*x[i] + c3*x[i-1];

			} 
			else 
			{
				double u  = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					u3 = 1.0-u,
					u4 = u3*u3,
					c1 = -u4*u/2.0,
					c2 = 1.0+u2*(3.0*u-5.0)/2.0,
					c3 = u*(1.0+u*(4.0-3.0*u))/2.0,
					c4 = -u2*u3/2.0;
				return c1*x[i-1] + c2*x[i] + c3*x[i+1] + c4*x[i+2];
			}
		}

		public override double GetY (double t)
		{
			int lo = x.Lo(), 
				hi = x.Hi(), 
				i  = FindIntervall(t,x);  

			if (i < lo || hi-lo == 1) 
			{
				// linear extrapolation and interpolation for 2 points
				return  y[lo] + (t-x[lo]) * (y[lo+1]-y[lo])/(x[lo+1]-x[lo]);

			} 
			else if (i == lo) 
			{
				i = lo;
				double u = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(-1.0+u*(u-1.0)/2.0),
					c2 = u*(1.0+u*(1.0-u)),
					c3 = u2*(u-1.0)/2.0;
				return c1*y[i] + c2*y[i+1] + c3*y[i+2];

			} 
			else if (i >= hi) 
			{
				// linear extrapolation
				return  y[hi] + (t-x[hi]) * (y[hi]-y[hi-1])/(x[hi]-x[hi-1]);

			} 
			else if (i == hi-1) 
			{
				i = hi-1;
				double u  = (x[i+1] - t) / (x[i+1] - x[i]),
					u2 = u*u,
					c1 = 1.0+u*(-1.0+u*(u-1.0)/2.0),
					c2 = u*(1.0+u*(1.0-u)),
					c3 = u2*(u-1.0)/2.0;
				return c1*y[i+1] + c2*y[i] + c3*y[i-1];

			} 
			else 
			{
				double u  = (t - x[i]) / (x[i+1] - x[i]),
					u2 = u*u,
					u3 = 1.0-u,
					u4 = u3*u3,
					c1 = -u4*u/2.0,
					c2 = 1.0+u2*(3.0*u-5.0)/2.0,
					c3 = u*(1.0+u*(4.0-3.0*u))/2.0,
					c4 = -u2*u3/2.0;
				return c1*y[i-1] + c2*y[i] + c3*y[i+1] + c4*y[i+2];
			}
		}
	

	
		#region Scene Drawing
#if IncludeScene
	//----------------------------------------------------------------------------//
//
//  compute a cubic Cardinal polynomial for the points
//  (x[0],y[0])...(x[3],y[3]) and plot it from (x[1],y[1]) to (x[2],y[2])
//
//----------------------------------------------------------------------------//

void MpCardinalCubicSpline::Draw4 (Scene& scene, 
				   const double *x, const double *y, int &first)
{
  double u,u2,u3,u4,c1,c2,c3,c4,rx,ry;
  int n = GetResolution(scene,x[1],y[1],x[2],y[2]);

  for (int i = 0; i <= n; i++) {
    u  = (double) i / n;
    u2 = u*u;
    u3 = 1.0-u;
    u4 = u3*u3;

    c1 = -u4*u/2.0;
    c2 = 1.0+u2*(3.0*u-5.0)/2.0;
    c3 = u*(1.0+u*(4.0-3.0*u))/2.0;
    c4 = -u2*u3/2.0;
    
    rx = c1*x[0]+c2*x[1]+c3*x[2]+c4*x[3];
    ry = c1*y[0]+c2*y[1]+c3*y[2]+c4*y[3];
    if (first) {
      scene.MoveTo(rx,ry);
      first = false;
    } else
      scene.LineTo(rx,ry);
  }
}

//----------------------------------------------------------------------------//

void MpCardinalCubicSpline::DrawClosedCurve (Scene &scene)
{
  if ( ! scene.IsOpen() ) 
    Matpack.Error("MpCardinalCubicSpline::DrawClosedCurve: "
		  "scene is not open for drawing");

  // get index range
  int lo = x->Lo(),
      hi = x->Hi();

  // nothing to draw - one point
  if ( lo >= hi ) return;

  // special case - two points
  if ( hi == lo+1 ) {
    scene.Line((*x)(lo),(*y)(lo),(*x)(hi),(*y)(hi));
    return;
  }

  // three or more points 

  int first = true;

  double xx[6],yy[6];
  
  for (int i = 0; i <= 2; i++) {
    xx[i] = (*x)[hi-2+i];
    yy[i] = (*y)[hi-2+i];
  }
  
  for (int i = 3; i <= 5; i++) {
    xx[i] = (*x)[lo+i-3];
    yy[i] = (*y)[lo+i-3];
  }
  
  for (int i = lo; i <= hi-3; i++)
    Draw4(scene,&(*x)(i),&(*y)(i),first);
  
  for (int i = 0; i <= 2; i++)
    Draw4(scene,&xx[i],&yy[i],first);

  // flush graphics buffer
  scene.Flush();  
}
#endif
		#endregion
	}

	#endregion

	#region RationalCubicSpline

	class RationalCubicSpline : CurveBase
	{
		protected	BoundaryConditions boundary;	
		protected	double p,r1,r2;
		protected	DoubleVector dx = new DoubleVector();
		protected	DoubleVector dy = new DoubleVector();
		protected	DoubleVector a = new DoubleVector();
		protected	DoubleVector b = new DoubleVector();
		protected	DoubleVector c = new DoubleVector();
		protected	DoubleVector d = new DoubleVector();



		
		//-----------------------------------------------------------------------------//
		//
		// static double deriv1 (const Vector &x, const Vector &y, int i, int sgn)
		//
		// Initial derivatives at boundaries of data set using
		// quadratic Newton interpolation
		// 
		//-----------------------------------------------------------------------------//

		static double deriv1 (IVector x, IVector y, int i, int sgn)
		{
			if (x.Elements() <= 1) 
				return 0.0;

			else if (x.Elements() == 2)
				return (y[y.Hi()]-y[y.Lo()]) / (x[x.Hi()]-x[x.Lo()]);
  
			else 
			{
				int    i1  = i+1,
					i2  = i-1;
				double his = x[i1]-x[i2],
					dis = (y[i1]-y[i2]) / his,
					di  = (y[i1]-y[i]) / (x[i1]-x[i]),
					di2 = (di-dis) / (x[i]-x[i2]);
				return dis + sgn * di2 * his;
			}
		}


		
		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SetSmoothing (double smoothing) 
		//
		// Set the value of the smoothing paramenter. A value of p = 0 
		// for the smoothing parameter results in a standard cubic spline. 
		// A value of p with -1 < p < 0 results in "unsmoothing" that means 
		// overshooting oscillations. A value of p with p > 0 gives increasing
		// smoothness. p to infinity results in a linear interpolation. A value
		// smaller or equal to -1.0 leads to an error.
		//
		//----------------------------------------------------------------------------//

		public double Smoothing
		{ 
			get
			{
				return p;
			}
			set
			{
				if (value > -1.0)
					p = value; 
				else
					throw new ArgumentException("smoothing parameter must be greater than -1.0");
			}
		}


		
		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SetBoundaryConditions (int boundary, 
		//						      double b1 = 0.0, 
		// 					              double b2 = 0.0)
		//
		// The following values are possible:
		//
		//      Natural 
		//          natural boundaries, that means the 2nd derivatives are zero 
		//          at both boundaries. This is the default value.
		//
		//      FiniteDifferences
		//          use  finite difference approximation for 1st derivatives.
		//
		//      Supply1stDerivative
		//          user supplied values for 1st derivatives are given in b1 and b2
		//          i.e. f'(x_lo) in b1
		//               f'(x_hi) in b2
		//
		//      Supply2ndDerivative 
		//          user supplied values for 2nd derivatives are given in b1 and b2
		//          i.e. f''(x_lo) in b1
		//               f''(x_hi) in b2
		//
		//      Periodic 
		//          periodic boundary conditions for periodic curves or functions.
		//          NOT YET IMPLEMENTED IN THIS VERSION.
		//
		// If the parameters b1,b2 are omitted the default value is 0.0.
		// 
		//----------------------------------------------------------------------------//

		public void SetBoundaryConditions (BoundaryConditions bnd, 
			double b1, double b2)
		{
			boundary = bnd;
			r1 = b1;
			r2 = b2;
		}
	
		public BoundaryConditions GetBoundaryConditions (out double b1, out double b2) 
		{
			b1 = r1; 
			b2 = r2;
			return boundary;
		}
		
		public BoundaryConditions GetBoundaryConditions()
		{
			return boundary;
		}
	
	

		
		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::Differences (const Vector& x, Vector& dx)
		//
		// Calculate difference vector dx(i) from vector x(i) and 
		// assure that x(i) is strictly monotone increasing or decreasing.
		// Can be called with both arguments the same vector in order to 
		// do it inplace!
		//
		//----------------------------------------------------------------------------//

		public static void Differences (IVector x, IVector dx)
		{
			int sgn;
			double t;

			// get dimensions
			int lo = x.Lo(),
				hi = x.Hi(); 

			if (hi > lo) 
			{
				if ( ( sgn = Math.Sign( x[lo+1]-x[lo] ) ) == 0) 
					throw new ArgumentException("abscissa is not strictly monotone");
    
				for (int i = lo; i < hi; i++) 
				{
					if ( Math.Sign( t = x[i+1]-x[i] ) != sgn ) 
						throw new System.ArgumentException("abscissa is not strictly monotone");
					dx[i] = t;
				}
			}
  
			dx[hi] = 0;
		}



		
		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::InverseDifferences (const Vector& x, Vector& dx)
		//
		// Calculate inverse difference vector dx(i) from vector x(i) and 
		// assure that x(i) is strictly monotone increasing or decreasing.
		// Can be called with both arguments the same vector in order to 
		// do it inplace!
		//
		//----------------------------------------------------------------------------//

		public static void InverseDifferences (IVector x, IVector dx)
		{
			int lo,hi,sgn;
			double t;

			// get dimensions
			lo = x.Lo(); 
			hi = x.Hi(); 

			if (hi > lo) 
			{
				if ( ( sgn = Math.Sign( x[lo+1]-x[lo] ) ) == 0) 
					throw new ArgumentException("abscissa is not strictly monotone");
    
				for (int i = lo; i < hi; i++) 
				{
					if ( Math.Sign( t = x[i+1]-x[i] ) != sgn ) 
						throw new ArgumentException("abscissa is not strictly monotone");
					dx[i] = 1.0 / t;
				}
			}

			dx[hi] = 0;
		}



		
		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineA (double p, 
		//                        const Vector& dx, Vector& z)
		//
		// input parameters:   p   smoothing parameter
		//                     dx  inverse abscissa difference vector
		//
		// output parameters:  z  coefficient vector SplineB1 and SplineB2
		// 			   
		//----------------------------------------------------------------------------//

		protected void SplineA (double p, IVector dx, IVector z)
		{
			double h1,h2,p2;

			// get dimensions
			int lo = dx.Lo(),
				hi = dx.Hi();

			// calculate vector z
			z[lo] = 0.0;
			h1 = dx[lo];
			p2 = 2.0 + p;
			for (int j = lo, k = lo+1; k < hi; j = k++) 
			{
				h2 = dx[k];
				z[k] = 1.0 / (p2*(h1+h2)-h1*h1*z[j]);
				h1 = h2;
			}
		}

		

		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineB1 (double p, 
		//				         const Vector& dx, const Vector& y, 
		//				         Vector& y1, Vector& f, const Vector& z)
		//
		//  input paramaters:    p  smoothing parameter
		//                      dx  inverse abscissa difference vector
		//                       y  ordinata vector
		//                       z  the coefficients computed by SplineA
		//                       f  working vector
		//                      y1  1st derivative vector with elements y1(lo) and y1(hi)
		//                          supplied by the user.
		//
		//  output parameters:  y1  1st derivative vector with elements
		//                          y1(i), i = lo+1...hi-1 calculated newly
		//
		//----------------------------------------------------------------------------//

		protected void SplineB1 (double p, 
			IVector dx, IVector y, 
			IVector y1, IVector f, IVector z)
		{
			int j = 0, k;
			double h,h1=0.0,h2,r1=0.0,r2;

			// get dimensions
			int lo  = dx.Lo(),
				hi  = dx.Hi(),
				lo1 = lo+1,
				hi1 = hi-1;

			// calculate the derivative vector y1
			f[lo] = 0.0;
			double p3 = 3.0 + p;
			for (k = lo; k < hi; k++) 
			{
				h2 = dx[k];
				r2 = p3*h2*h2*(y[k+1]-y[k]);
				if (k != lo) 
				{
					h = r1 + r2;
					if (k == lo1) h -= h1*y1[lo];
					if (k == hi1) h -= h2*y1[hi];
					f[k] = z[k] * (h-h1*f[j]);
				}
				j = k;
				h1 = h2;
				r1 = r2;
			}
			if (hi-lo < 1) return;
			y1[hi1] = f[hi1];
			int n2 = hi-2;
			for (j = lo+1; j <= n2; j++) 
			{
				k = hi-j;
				y1[k] = f[k]-z[k]*dx[k]*y1[k+1];
			}
		}
	

		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineC1 (double p,
		//				      const Vector& x, const Vector& dx, 
		//				      const Vector& y, const Vector& y1,
		//				      Vector &a, Vector &b, Vector &c, Vector &d)
		//
		// Calculates the spline coefficients a(i), b(i), c(i), d(i) for a spline
		// with given 1st derivative. It uses the coefficients calculated by 
		// SplineA and SplineB1.
		//
		//----------------------------------------------------------------------------//

		protected void SplineC1 (double p,
			IVector x, IVector dx, 
			IVector y, IVector y1,
			IVector a, IVector b, IVector c, IVector d)
		{
			// get dimensions
			int lo = x.Lo(),
				hi = x.Hi(); 
   
			// auxilliaries
			double p2 = 2.0 + p,
				p3 = 3.0 + p,
				p4 = 1.0/(p2*p2-1.0); 
    
			// calculate spline coefficients
			for (int i = lo; i < hi; i++) 
			{
				double dy = y[i+1] - y[i];
				c[i] = (p3 * dy - p2 * y1[i] / dx[i] - y1[i+1] / dx[i] ) * p4;
				d[i] = (-p3 * dy + y1[i] / dx[i] + p2 * y1[i+1] / dx[i] ) * p4;
				a[i] = y[i]-c[i];
				b[i] = y[i+1]-d[i];
			}
			a[hi] = b[hi] = c[hi] = d[hi] = 0.0;
		}


		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineB2 (double p, 
		//				         const Vector& dx, const Vector& y, 
		//				         Vector& y2, Vector& f, const Vector& z)
		//
		//  input paramaters:    p  smoothing parameter
		//                      dx  abscissa difference vector
		//                       y  ordinata vector
		//                       z  the coefficients computed by SplineA
		//                       f  working vector
		//                      y2  2nd derivative vector with elements y2(lo) and y2(hi)
		//                          supplied by the user.
		//
		//  output parameters:  y2  2nd derivative vector with elements
		//                          y2(i), i = lo+1...hi-1 calculated newly
		//
		//----------------------------------------------------------------------------//

		protected void SplineB2 (double p, 
			IVector dx, IVector y, 
			IVector y2, IVector f, IVector z)
		{
			int j = 0 ,k;
			double h,h1 = 0.0,h2,r1=0.0,r2;

			// get dimensions
			int lo  = dx.Lo(),
				hi  = dx.Hi(),
				lo1 = lo+1,
				hi1 = hi-1;

			// calculate the derivative vector y2
			f[lo] = 0.0;
			double pp = 2.0*p*(3.0+p)+6.0;
			for (k = lo; k < hi; k++) 
			{
				h2 = dx[k];
				r2 = pp*(y[k+1]-y[k])/h2;
				if (k != lo) 
				{
					h = r2 - r1;
					if (k == lo1) h -= h1*y2[lo]; 
					if (k == hi1) h -= h2*y2[hi];
					f[k] = z[k] * (h-h1*f[j]);
				}
				j = k;
				h1 = h2;
				r1 = r2;
			}
			if (hi-lo < 1) return;
			y2[hi1] = f[hi1];
			int n2 = hi-2;
			for (j = lo+1; j <= n2; j++) 
			{
				k = hi-j;
				y2[k] = f[k]-z[k]*dx[k]*y2[k+1];
			}
		}


		//----------------------------------------------------------------------------//
		//
		// void MpRationalCubicSpline::SplineC2 (double p, 
		//				         const Vector& x, const Vector& dx, 
		//				         const Vector& y, const Vector& y2,
		//				         Vector &a, Vector &b, Vector &c, Vector &d)
		//
		// Calculates the spline coefficients a(i), b(i), c(i), d(i) for a spline
		// with given 2nd derivative. It uses the coefficients calculated by 
		// SplineA and SplineB2.
		//
		//----------------------------------------------------------------------------//

		void SplineC2 (double p, 
			IVector x, IVector dx, 
			IVector y, IVector y2,
			IVector a, IVector b, IVector c, IVector d)
		{
			// get dimensions
			int lo = x.Lo(),
				hi = x.Hi(); 
   
			// auxilliaries
			double pp = 0.5 / (p*(3.0+p)+3.0);

			// calculate spline coefficients
			for (int i = lo; i < hi; i++) 
			{    
				double h = pp * sqr( dx[i] );
				c[i] = h * y2[i];
				d[i] = h * y2[i+1];
				a[i] = y[i]-c[i];
				b[i] = y[i+1]-d[i];
			}
			a[hi] = b[hi] = c[hi] = d[hi] = 0.0;
		}


		public RationalCubicSpline()
		{
			boundary=BoundaryConditions.Natural;
			p=0.0;
		}
	
		
		//----------------------------------------------------------------------------//
		//
		// int MpRationalCubicSpline::Interpolate (const Vector &x, const Vector &y)
		//
		// Rational Cubic Spline Interpolation:
		// ------------------------------------
		//
		// This kind of generalized splines give much more pleasent results
		// than cubic splines when interpolating, e.g., experimental data.
		// A control parameter p can be used to tune the interpolation smoothly 
		// between cubic splines and a linear interpolation. 
		// But this doesn't mean smoothing of the data - the rational spline curve 
		// will still go through all data points.
		//
		// The basis functions for rational cubic splines are
		//
		//   g1 = u
		//   g2 = t                     with   t = (x - x(i)) / (x(i+1) - x(i))
		//   g3 = u^3 / (p*t + 1)              u = 1 - t
		//   g4 = t^3 / (p*u + 1)
		//
		// A rational spline with coefficients a(i),b(i),c(i),d(i) is determined by
		//
		//          f(i)(x) = a(i)*g1 + b(i)*g2 + c(i)*g3 + d(i)*g4
		//
		//
		// Choosing the smoothing parameter p:
		// -----------------------------------
		//
		// Use the method 
		//
		//      void MpRationalCubicSpline::SetSmoothing (double smoothing) 
		//
		// to set the value of the smoothing paramenter. A value of p = 0 
		// for the smoothing parameter results in a standard cubic spline. 
		// A value of p with -1 < p < 0 results in "unsmoothing" that means 
		// overshooting oscillations. A value of p with p > 0 gives increasing
		// smoothness. p to infinity results in a linear interpolation. A value
		// smaller or equal to -1.0 leads to an error.
		//
		//
		// Choosing the boundary conditions:
		// ---------------------------------
		//
		// Use the method 
		//
		//      void MpRationalCubicSpline::SetBoundaryConditions (int boundary, 
		//						           double b1, double b2)
		//
		// to set the boundary conditions. The following values are possible:
		//
		//      Natural 
		//          natural boundaries, that means the 2nd derivatives are zero 
		//          at both boundaries. This is the default value.
		//
		//      FiniteDifferences
		//          use  finite difference approximation for 1st derivatives.
		//
		//      Supply1stDerivative
		//          user supplied values for 1st derivatives are given in b1 and b2
		//          i.e. f'(x_lo) in b1
		//               f'(x_hi) in b2
		//
		//      Supply2ndDerivative 
		//          user supplied values for 2nd derivatives are given in b1 and b2
		//          i.e. f''(x_lo) in b1
		//               f''(x_hi) in b2
		//
		//      Periodic 
		//          periodic boundary conditions for periodic curves or functions.
		//          NOT YET IMPLEMENTED IN THIS VERSION.
		//
		// 
		// If the parameters b1,b2 are omitted the default value is 0.0.
		//
		//
		// Input parameters: 
		// -----------------
		//
		//      Vector x(lo,hi)  The abscissa vector  
		//      Vector y(lo,hi)  The ordinata vector
		//                       If the spline is not parametric then the
		//                       abscissa must be strictly monotone increasing
		//                       or decreasing!
		//
		//
		// References:
		// -----------
		//   Dr.rer.nat. Helmuth Spaeth, 
		//   Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen,
		//   3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
		//
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate (IVector x, IVector y)
		{
			// check input parameters

			if ( ! MatchingIndexRange(x,y) )
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Empty()) 
			{
				dx.Remove();
				dy.Remove();
				a.Remove();
				b.Remove();
				c.Remove();
				d.Remove();
				return 0; // ok
			}

			int lo = x.Lo(),
				hi = x.Hi();

			dx.Resize(lo,hi);  // abscissa difference vector
			dy.Resize(lo,hi);  // vector of derivatives
			a.Resize(lo,hi);   // spline coefficients
			b.Resize(lo,hi);   // spline coefficients
			c.Resize(lo,hi);   // spline coefficients
			d.Resize(lo,hi);   // spline coefficients

			if (boundary == BoundaryConditions.FiniteDifferences || boundary == BoundaryConditions.Supply1stDerivative) 
			{
      
				if (boundary == BoundaryConditions.FiniteDifferences) 
				{
	
					// finite differences (quadratic Newton interpolation)
					dy[lo] = deriv1(x,y,lo+1,-1);
					dy[hi] = deriv1(x,y,hi-1,1);
	
				} 
				else 
				{  // the 1st derivatives are supplied by the user
	
					// user supplied data
					dy[lo] = r1;
					dy[hi] = r2;
				}
      
				// start the calculation
				InverseDifferences(x,dx);     	// inverse difference vector
				SplineA(p,dx,a);			// a used as working vector
				SplineB1(p,dx,y,dy,b,a);		// a and b used as working vector
				SplineC1(p,x,dx,y,dy,a,b,c,d);	// spline coeff. returned in a,b,c,d  
      
			} 
			else if (boundary == BoundaryConditions.Natural || boundary == BoundaryConditions.Supply2ndDerivative) 
			{
      
				if (boundary == BoundaryConditions.Natural) 
				{ 
	
					// 2nd derivatives are zero
					dy[lo] = dy[hi] = 0.0;

				} 
				else 
				{  // the 2nd derivatives are supplied by the user

					// user supplied data
					dy[lo] = r1;
					dy[hi] = r2;
				}

				// start the calculation
				Differences(x,dx);            	// difference vector
				SplineA(p,dx,a);			// a used as working vector
				SplineB2(p,dx,y,dy,b,a);		// a and b used as working vector
				SplineC2(p,x,dx,y,dy,a,b,c,d);	// spline coeff. returned in a,b,c,d

			} 
			else if (boundary == BoundaryConditions.Periodic) 
			{
				throw new NotImplementedException("PERIODIC BOUNDARIES NOT YET IMPLEMENTED");	    
			}

			return 0; // ok
		}  


		public override double GetX (double u) 
		{
			return u;
		}


		public override double GetY (double u)
		{
			// special case that there are no data. Return 0.0.
			if (x.Empty()) return 0.0;

			int i = FindIntervall(u,x);  

			if (i < x.Lo()) 
			{ 		// extrapolation

				i++;
				double dx = u - x[i],
					h = x[i+1] - x[i],
					y0 = a[i]+c[i],
					y1 = (b[i]-a[i]-(3+p)*c[i]) / h,
					y2 = 2 * (p*p+3*p+3)*c[i] / (h*h);
				return y0 + dx * (y1 + dx * y2);
    
			} 
			else if (i == x.Hi()) 
			{	// extrapolation

				i--;
				double dx = u - x[i+1],
					h = x[i+1] - x[i],
					y0 = b[i]+d[i],
					y1 = (b[i]-a[i]+(3+p)*d[i]) / h,
					y2 = 2 * (p*p+3*p+3)*d[i] / (h*h);
				return y0 + dx * (y1 + dx * y2);

			} 
			else 
			{ 			// interpolation

				double t = (u - x[i]) / (x[i+1] - x[i]),
					v = 1.0 - t; 
				return a[i]*v + b[i]*t + c[i] * v*v*v/(p*t+1) + d[i] * t*t*t/(p*v+1);

			}
		}
		


	}

	#endregion

	#region ExponentialSpline


	//----------------------------------------------------------------------------//
	// Exponential Splines
	//
	// References:
	// -----------
	// (1) D.G. Schweikert, "An Interpolation Curve using a Spline in Tension"
	//     J. Math. Physics, 45, pp 312-317 (1966).
	// (2) Dr.rer.nat. Helmuth Spaeth, 
	//     "Spline-Algorithmen zur Konstruktion glatter Kurven und Flaechen",
	//     3. Auflage, R. Oldenburg Verlag, Muenchen, Wien, 1983.
	// (3) A. K. Cline, Commun. of the ACM, 17, 4, pp 218-223 (Apr 1974).
	// (4) This algorithm is also implemented in the Unix spline tool by
	//     James R. Van Zandt (jrv@mitre-bedford), 1985.
	//----------------------------------------------------------------------------//
	class ExponentialSpline : CurveBase
	{
		protected	BoundaryConditions boundary;	
		protected double sigma,r1,r2;
		protected DoubleVector y1 = new DoubleVector();
		protected DoubleVector tmp = new DoubleVector();

		public ExponentialSpline()
		{
			boundary = BoundaryConditions.FiniteDifferences;
			sigma= 1.0;
		}

		
		//----------------------------------------------------------------------------//
		//
		// int MpExponentialSpline::Interpolate (const Vector &x, const Vector &y)
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate (IVector x, IVector y)
		{
			// check input parameters

			if ( ! MatchingIndexRange(x,y) )
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Empty()) 
			{
				y1.Remove();
				tmp.Remove();
				return 0; // ok
			}

			int lo = x.Lo(),
				hi = x.Hi(),
				n = x.Elements();

			y1.Resize(lo,hi);    // spline coefficients

			if (n == 1) 
			{
				y1[lo] = 0.0;
				return 0; // ok
			}

			tmp.Resize(lo,hi);   // temporary
  
			double slp1 = 0.0, slpn = 0.0;
			double dels, delx1, delx2, delx12, deln, delnm1, delnn, c1, c2, c3,
				diag1, diag2, diagin, dx1, dx2=0.0, exps, sigmap, sinhs, sinhin, 
				slpp1, slppn=0.0, spdiag;

			delx1 = x[lo + 1] - x[lo];
			dx1   = (y[lo + 1] - y[lo]) / delx1;

			slpp1 = dx1;	// to get y1(lo) = 0 in unspecified cases
			if (boundary == BoundaryConditions.Supply1stDerivative)
				slpp1 = slp1;
			else if (n != 2) 
			{
				delx2 = x[lo + 2] - x[lo + 1];
				delx12 = x[lo + 2] - x[lo];
				c1 = -(delx12 + delx1) / delx12 / delx1;
				c2 = delx12 / delx1 / delx2;
				c3 = -delx1 / delx12 / delx2;
				slpp1 = c1 * y[lo] + c2 * y[lo + 1] + c3 * y[lo + 2];
			} 
			else 
			{
				y1[lo] = y1[lo + 1] = 0.0;
			}

			if (boundary == BoundaryConditions.Supply1stDerivative)
				slppn = slpn;
			else if (n != 2) 
			{
				deln = x[hi] - x[hi - 1];
				delnm1 = x[hi - 1] - x[hi - 2];
				delnn = x[hi] - x[hi - 2];
				c1 = (delnn + deln) / delnn / deln;
				c2 = -delnn / deln / delnm1;
				c3 = deln / delnn / delnm1;
				slppn = c3 * y[hi - 2] + c2 * y[hi - 1] + c1 * y[hi];
			} 
			else 
			{
				y1[lo] = y1[lo + 1] = 0.0;
			}

			// denormalize tension factor
			sigmap = Math.Abs(sigma) * (n - 1) / (x[hi] - x[lo]);
  
			// set up right hand side and tridiagonal system for y1 and perform forward
			// elimination                
			dels = sigmap * delx1;
			exps = Math.Exp(dels);
			sinhs = 0.5 * (exps - 1.0 / exps);
			sinhin = 1.0 / (delx1 * sinhs);
			diag1 = sinhin * (dels * 0.5 * (exps + 1.0 / exps) - sinhs);
			diagin = 1.0 / diag1;
			y1[lo] = diagin * (dx1 - slpp1);
			spdiag = sinhin * (sinhs - dels);
			tmp[lo] = diagin * spdiag;
			if (n != 2) 
			{
				for (int i = lo+1; i <= hi - 1; i++) 
				{
					delx2 = x[i + 1] - x[i];
					dx2 = (y[i + 1] - y[i]) / delx2;
					dels = sigmap * delx2;
					exps = Math.Exp(dels);
					sinhs = 0.5 * (exps - 1.0 / exps);
					sinhin = 1.0 / (delx2 * sinhs);
					diag2 = sinhin * (dels * (0.5 * (exps + 1.0 / exps)) - sinhs);
					diagin = 1.0 / (diag1 + diag2 - spdiag * tmp[i - 1]);
					y1[i] = diagin * (dx2 - dx1 - spdiag * y1[i - 1]);
					spdiag = sinhin * (sinhs - dels);
					tmp[i] = diagin * spdiag;
					dx1 = dx2;
					diag1 = diag2;
				}
			}
  
			diagin = 1.0 / (diag1 - spdiag * tmp[hi - 1]);

			// the expression below does not make sense if n == 2
			if (n != 2) 
				y1[hi] = diagin * (slppn - dx2 - spdiag * y1[hi - 1]);

			// perform back substitution
			for (int i = hi - 1; i >= lo; i--)
				y1[i] -= tmp[i] * y1[i + 1];

			return 0; // ok
		}
 
		public override  double GetX (double u) 
		{
			return u;
		}
		public override double GetY (double u)
		{
			int lo = x.Lo(), 
				hi = x.Hi(), 
				n  = x.Elements(),
				i  = lo+1; 

			// special cases
			if (n == 1) return y[lo];

			// search for x(i-1) <= u < x(i), lo < i <= hi
			while (i < hi && u >= x[i]) i++;
			while (i > lo+1 && x[i - 1] > u) i--;

			double sigmap = Math.Abs(sigma) * (n - 1) / (x[hi] - x[lo]),
				del1 = u - x[i - 1],
				del2 = x[i] - u,
				dels = x[i] - x[i - 1],
				exps1 = Math.Exp(sigmap * del1),
				sinhd1 = 0.5 * (exps1 - 1.0 / exps1),
				exps = Math.Exp(sigmap * del2),
				sinhd2 = 0.5 * (exps - 1.0 / exps),
				exps2 = exps1 * exps,
				sinhs = 0.5 * (exps2 - 1.0 / exps2);

			return (y1[i] * sinhd1 + y1[i - 1] * sinhd2) / sinhs +
				((y[i] - y1[i]) * del1 + (y[i - 1] - y1[i - 1]) * del2) / dels;
		}
	
		public double Smoothing
		{
			get
			{
				return sigma; 
			}
			set
			{ 
				if (value > 0.0)
					sigma = value; 
				else
					throw new ArgumentException("smoothing parameter must be greater than 0.0");
			}
		}

		public BoundaryConditions BoundaryCondition
		{
			get { return boundary; }
			set {	SetBoundaryConditions(value,0,0); }
		}

		public void SetBoundaryConditions (BoundaryConditions bnd, double b1, double b2)
		{
			// check boundary conditions argument
			if (bnd == BoundaryConditions.Supply1stDerivative || bnd == BoundaryConditions.FiniteDifferences) 
			{
				boundary = bnd;
				r1 = b1;
				r2 = b2;
			} 
			else 
				throw new ArgumentException("Only Supply1stDerivative or FiniteDifferences boundary conditions");
		}

		public BoundaryConditions GetBoundaryConditions(out double b1, out double b2) 
		{
			b1 = r1;
			b2 = r2;
			return boundary;
		}
	}


	#endregion

	#region PolynomialInterpolation

	
	class PolynomialInterpolation : CurveBase
	{
		protected	DoubleVector C = new DoubleVector();
		protected DoubleVector D = new DoubleVector();

		
		//----------------------------------------------------------------------------//
		//
		// int MpPolynomialInterpolation::Interpolate (const Vector &x, const Vector &y)
		//
		//
		//----------------------------------------------------------------------------//
		
		public override int Interpolate (IVector x, IVector y)

		{
			// check input parameters

			if ( ! MatchingIndexRange(x,y) )
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;  

			// Empty data vectors - free auxilliary storage
			if (x.Empty()) 
			{
				C.Remove();
				D.Remove();
			}

			return 0; // ok
		}
		public override double GetX (double u)
		{ 
			return u;
		}

		
		//----------------------------------------------------------------------------//
		//
		// double MpPolynomialInterpolation::GetY (double u)
		//
		// Polynomial interpolation using the Aitken-Neville tableaux.
		// The returned y is the value that corresponds to the value of 
		// the poynomial y = P(x) of degree n = hi-lo 
		// that interpolates the data points (x(i),y(i)), lo <= i <= hi. 
		// In the special case of empty data vectors (x,y) a value of 0.0 is returned.
		//
		//----------------------------------------------------------------------------//

		public override  double GetY (double u)
		{
			// special case that there are no data. Return 0.0.
			if (x.Empty()) return 0.0;

			int lo = x.Lo(), 
				hi = x.Hi();

			// allocate (resize) auxilliary vectors - the resize method has the property
			// that no (de-)allocation is done, if the size of the vector is not changed.
			// Thus there is no overhead if GetY() is called many times with the
			// same vectors, for instance, if a whole curve is drawn.
			C.Resize(lo,hi); 
			D.Resize(lo,hi); 

			C.CopyFrom(y);  			// initialize // TODO original was C = D = *y; check Vector if this is a copy operation 
			D.CopyFrom(y);

			int pos = lo;  		// find position of closest abscissa value
			double delta = Math.Abs( u - x[lo] ), delta2;
			for (int i = lo; i <= hi; i++)
				if ( (delta2 = Math.Abs(u - x[i])) < delta ) 
				{
					delta = delta2;
					pos = i;
				}
 
			double dy, yy = C[pos--]; 	// initial approximation

			for (int m = lo; m < hi; m++) 
			{
				for (int i = lo; i <= hi-m; i++) 
				{
					double h1 = x[i] - u,
						h2 = x[i+m] - u,
						CD = C[i+1] - D[i],
						denom;
					if ( (denom = h1-h2) == 0.0 ) 
						throw new ArgumentException("two abscissa values are identical");
					denom = CD/denom;
					C[i] = denom * h1;
					D[i] = denom * h2;
				}
				yy += (dy = (2*pos-lo+1 < (hi-m) ? C[pos+1] : D[pos--] ));
			}

			return yy;			// return value
		}
	}

	#endregion

	#region RationalInterpolation

	public class RationalInterpolation : CurveBase
	{
		protected	DoubleVector xr,yr;
		protected	IntVector m;
		protected	int num;
		protected	double epsilon;
	
		public RationalInterpolation ()
		{
			num = 0;
			epsilon = DBL_EPSILON;
		}



		//----------------------------------------------------------------------------//
		//
		// int MpRationalInterpolation::Interpolate (const Vector &x, const Vector &y)
		//
		// Description:
		// ------------
		//
		//	Calculates a rational interpolation
		//	polynomial with numerator degree N and denominator 
		//	degree D which passes through the n given points
		//	(x[i],y[i]). In the unique solution the denominator
		//	degree D is determined by the relation
		//	
		//	      D = n - 1 - N
		//	
		//	for the given values of n and N.
		//
		//	The required precision "double epsilon" should be set before calling
		//	this function.  Use function SetPrecision (double eps) for this purpose.
		//
		// Arguments:
		// ----------
		//
		//    Vector& x
		//    Vector& y
		//
		//	The x- and y- values (x[i],y[i]) which are to be
		//	interpolated. The vectors must have the same index
		//	range i = lo,...,hi. This means n = hi-lo+1 values.
		//
		// Return values:
		// --------------
		//
		//   0  everything is ok
		//
		//   1  Interpolation function doesn't exist. You should  try a numerator 
		//      degree N > (n - 1) / 2
		//
		//   2  Number of points still to interpolate and degree of numerator 
		//      polynomial N < 0. You should try to change the numerator degree.
		//
		//   3  Degree of denominator polynomial < 0. You should try to change 
		//      the numerator degree.
		//
		//   4  Not all points have been used in the interpolation. You should 
		//      try to change the numerator degree.
		//
		//
		// Reference:
		// ----------
		//
		//   H. Werner, A reliable and Numerically Stable Program for Rational
		//   Interpolation of Lagrange Data, Computing, Vol 31, 269 (1983).
		//
		//   H. Werner, R. Schaback, Praktische Mathematik II, Springer,
		//   Berlin, Heidelberg, New York, 1972, 2. Aufl. 1979.
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate (IVector x, IVector y)
		{
			// check input parameters

			if ( ! MatchingIndexRange(x,y) )
				throw new ArgumentException("index range mismatch of vectors");

			// link original data vectors into base class
			base.x = x;
			base.y = y;  

			// Empty data vectors - free auxilliary storage
			if (x.Empty()) 
			{
				xr.Remove();
				yr.Remove();
				m.Remove();
				return 0;
			}

			int lo = x.Lo(),
				hi = x.Hi();

			xr.Resize(lo,hi);
			yr.Resize(lo,hi);
			m.Resize(lo,hi);

			int i, j, j1, denom, nend;
			double xj, yj, x2, y2;

			int n  = x.Elements()-1;

			if (n < 1) 
				throw new ArgumentException(string.Format("less than two points where given ({0})",n+1));

			if (num <= 0) 
				throw new ArgumentException("numerator degree must be positive");

			if (num > n) 
				throw new ArgumentException(string.Format("degree of numerator polynomial ({0}) was greater or equal to the number of data points ({1})", 
					num, n+1));

			for (i = lo; i < hi; i++)
				for (j = i+1; j <= hi; j++)
					if (x[i] == x[j]) 
						throw new ArgumentException(string.Format("two equal x values at ({0}) and ({1})", 
							i, j));
    
			// precision limit
			epsilon = Math.Max(epsilon, 128.0 * DBL_EPSILON);

			// allocate auxilliary storage
			DoubleVector z = new DoubleVector(lo,hi);
    
			// copy original values
			xr.CopyFrom( x );
			yr.CopyFrom( y );

			// initialize M to 1
			m.SetAllElementsTo( 1 );
    
			nend = hi;
			denom = n - num; // degree of denominator polynomial

			if (num < denom) 
			{
				for (i = lo; i <= hi; i++)
					if (yr[i] != 0.0) yr[i] = 1.0 / yr[i];
					else return 1; // interpolation function doesn't exist
				m[hi] = 0;
				j = num;
				num = denom;
				denom = j;
			}

			while (nend > lo) 
			{

				for (i = 1; i <= num-denom; i++) 
				{
					xj = xr[nend];  
					yj = yr[nend];
					for (j = lo; j < nend; j++)
						yr[j] = (yr[j] - yj) / (xr[j] - xj);
					--nend;
				}

				if (nend < lo && denom < 0) return 2;

				if (nend > lo) 
				{

					ymin(nend, out xj, out yj, xr, yr);

					j1 = lo;
					for (j = lo; j < nend; j++) 
					{
						y2 = yr[j] - yj;
						x2 = xr[j] - xj;
						if (Math.Abs(y2) <= Math.Abs(x2) * epsilon)
							z[j1++] = xr[j];
						else 
						{
							yr[j-j1+lo] = x2 / y2;
							xr[j-j1+lo] = xr[j];
						}
					}
					for (j = lo; j < j1; j++) 
					{
						xr[nend-1] = z[j];
						yr[nend-1] = 0.0;
						for (i = lo; i < nend; i++)
							yr[i] *= xr[i] - xr[nend];
						--nend;
					}
					if (nend > lo) 
					{
						m[--nend] = 0;
						num = denom;
						denom = nend - num;
					}
					if (denom < 0 && nend < lo)
						return 3; // degree of denominator polynomial < 0
				}
			}

			y2 = Math.Abs(yr[hi]);
			for (i = lo; i < hi; i++) 
				y2 += Math.Abs(yr[i]);
			for (i = lo; i <= hi; i++) 
			{
				x2 = this.GetY( x[i] );
				if (Math.Abs(x2-y[i]) > n * epsilon * y2)
					return 4; // not all points have been used
			}

			return 0; // ok
		}
		public override double GetX (double u)
		{
			return u;
		}

		readonly static double RootMax   = Math.Sqrt(double.MaxValue);
		public override double GetY (double u)
		{
			const double SquareEps = DBL_EPSILON * DBL_EPSILON;

			int lo = xr.Lo(), 
				hi = yr.Hi();

			double val = yr[lo];
			for (int i = lo+1; i <= hi; i++) 
			{
				if ( m[i-1] !=0 )
					val = yr[i] + (u-xr[i]) * val;
				else if (Math.Abs(val) > SquareEps)
					val = yr[i] + (u-xr[i]) / val;
				else
					return RootMax;
			}

			if ( m[hi] !=0 )
				return val;
			else if (Math.Abs(val) > SquareEps)
				return 1.0/val;
			else
				return RootMax;
		}

		public double Precision 
		{
			set { epsilon = value; }
			get { return epsilon; }
		}

		public int NumeratorDegree
		{
			set { num = value; }
			get { return num; }
		}
	


		//----------------------------------------------------------------------------//
		//
		// static void ymin (int nend, double& xj, double& yj, Vector& x, Vector& y)
		//
		// local auxilliary function
		//
		//----------------------------------------------------------------------------//

		static void ymin (int nend, out double xj, out double yj, IVector x, IVector y)
		{
			int j;
			yj = y[nend];
			j = nend;
			for (int k = x.Lo(); k < nend; k++)
				if (Math.Abs(y[k]) < Math.Abs(yj)) 
				{
					j = k;
					yj = y[j];
				}
			xj = x[j];
			x[j] = x[nend]; x[nend] = xj;
			y[j] = y[nend]; y[nend] = yj;
		}
	}

	#endregion

	#region CrossValidatedCubicSpline
#if false
	public class CrossValidatedCubicSpline : MpCurveBase
																			{

		//----------------------------------------------------------------------------//
		// static globals
		//----------------------------------------------------------------------------//

		const double zero = 0.0;
		const double  one = 1.0;
		const double  two = 2.0;


		//----------------------------------------------------------------------------//
		// error flags
		//----------------------------------------------------------------------------//

		enum ErrorFlag
		{
			no_error             = 0,
			too_few_datapoints   = 1,
			abscissa_not_ordered = 2,
			stddev_non_positive  = 3
		};

		protected	double var;
		protected IVector dy;
		protected DoubleVector y0, y1, y2, y3, se, wk;

		public	MpCrossValidatedCubicSpline ()
		{
			var = -1.0;
			dy = null;
		}


		// TODO : this routine has to be rebased to zero and the pointer arithmetic
		// must be removed

		static void spfit (int n, 
			double[] x, // const double *x,
			out double avh, // double *avh, 
			double[] dy, // const double *dy,
			double rho, 
			out double p, // double *p,
			out double q, // double *q,
			out double fun, // double *fun,
			out double var, // double *var,
			double[] stat, // double *stat,
			double[] a, // double *a,
			double[] c1, // double *c1, 
			double[] c2, // double *c2,
			double[] c3, // double *c3,
			double[] r, // double *r,
			double[] t, // double *t, 
			double[] u, // double *u,
			double[] v // double *v
			)
			//
			// Fits a cubic smoothing spline to data with relative
			// weighting dy for a given value of the smoothing parameter
			// rho using an algorithm based on that of C.H.Reinsch (1967),
			// Numer. Math. 10, 177-183.
			// The trace of the influence matrix is calculated using an
			// algorithm developed by M.F.hutchinson and F.R.de Hoog (Numer.
			// Math., in press), enabling the generalized cross validation
			// and related statistics to be calculated in order n operations.
			// The arrays a, c, r and t are assumed to have been initialized
			// by the subroutine spint.  Overflow and underflow problems are
			// avoided by using p=rho/(1 + rho) and q=1/(1 + rho) instead of
			// rho and by scaling the differences x[i+1] - x[i] by avh.
			// the values in df are assumed to have been scaled so that the
			// sum of their squared values is n.  The value in var, when it is
			// non-negative, is assumed to have been scaled to compensate for
			// the scaling of the values in df.
			// The value returned in fun is an estimate of the true mean square
			// when var is non-negative, and is the generalized cross validation
			// when var is negative.
			//
		{
			int i, r_dim1, r_offset, t_dim1, t_offset;
			double e, f, g, h, rho1, d1;

			// Parameter adjustments
			t_dim1 = n + 2;
			t_offset = t_dim1;
			t -= t_offset;
			r_dim1 = n + 2;
			r_offset = r_dim1;
			r -= r_offset;
			--stat;

			// use p and q instead of rho to prevent overflow or underflow
			rho1 = one + rho;
			p = rho / rho1;
			q = one / rho1;
			if (rho1 == one) p = zero;
			if (rho1 == rho) q = zero;

			// rational cholesky decomposition of p*c + q*t
			f = g = h = zero;

			for (i = 0; i <= 1; ++i)
				r[i] = zero; // r[i + r_dim1] = zero;

			for (i = 2; i < n; ++i) 
			{
				r[i-2+r_dim1*2] = g * r[i-2]; // DL
				r[i-1+r_dim1] = f * r[i-1]; // DL
				r[i] = one / (p * c1[i]+q*t[i+t_dim1]-f*r[i-1+(r_dim1<<1)]
					-g*r[i-2+r_dim1*3]);
				f = p * c2[i] + q * t[i+(t_dim1<<1)] - h*r[i-1+(r_dim1<<1)];
				g = h;
				h = p * c3[i];
			}

			// solve for u
			u[0] = u[1] = zero;
			for (i = 2; i < n; ++i)
				u[i] = a[i]-r[i-1+(r_dim1<<1)]*u[i-1]-r[i-2+r_dim1*3]*u[i-2];
			u[n] = u[n+1] = zero;
			for (i = n-1; i >= 2; --i)
				u[i] = r[i+r_dim1]*u[i]-r[i+(r_dim1<<1)]*u[i+1]-r[i+r_dim1*3]*u[i+2];

			// calculate residual vector v
			e = h = zero;
			for (i = 1; i < n; ++i) 
			{
				g = h;
				h = (u[i+1] - u[i]) / ((x[i+1] - x[i]) / avh);
				v[i] = dy[i] * (h - g);
				e += v[i] * v[i];
			}
			v[n] = dy[n] * (-h);
			e += v[n] * v[n];

			// calculate upper three bands of inverse matrix
			r[n+r_dim1] =
				r[n+(r_dim1<<1)] =
				r[n+1+r_dim1] = zero;
			for (i = n - 1; i >= 2; --i) 
			{
				g = r[i+(r_dim1<<1)];
				h = r[i+r_dim1*3];
				r[i+(r_dim1<<1)] = -g * r[i+1+r_dim1] - h*r[i+1+(r_dim1<<1)];
				r[i+r_dim1*3] = -g * r[i+1+(r_dim1<<1)] - h*r[i+2+r_dim1];
				r[i+r_dim1] = r[i+r_dim1] - g*r[i+(r_dim1<<1)] - h*r[i+r_dim1*3];
			}

			// calculate trace
			f = g = h = zero;
			for (i = 2; i < n; ++i) 
			{
				f += r[i+r_dim1] * c1[i];
				g += r[i+(r_dim1<<1)] * c2[i];
				h += r[i+r_dim1*3] * c3[i];
			}
			f += two * (g + h);

			// calculate statistics
			stat[1] = p;
			stat[2] = f * p;
			stat[3] = n * e / (f * f);
			stat[4] = e * p * p / n;
			stat[6] = e * p / f;

			if (var >= zero) 
			{
				d1 = stat[4] - two * var * stat[2] / n + var;
				stat[5] = Math.Max(d1,zero);
				fun = stat[5];
			} 
			else 
			{
				stat[5] = stat[6] - stat[4];
				fun = stat[3];
			}
		}


		
		//----------------------------------------------------------------------------//
		//
		// int MpCrossValidatedSpline (const Vector &X, 
		//                             const Vector &F, 
		//                             Vector &DF,
		//			       Vector &Y, Vector &C1, Vector &C2, Vector &C3,
		//			       double& var, Vector &SE, Vector &WK)
		//
		//  Arguments:
		//
		//              X   Vector of length n containing the abscissae of the
		//		    n data points (x[i],f[i]).
		//		    x must be ordered so that x[i] < x[i+1].
		//
		//		F   Vector of length n containing the ordinates
		//		    of the n data points (x[i],f[i]).
		//
		//             DF   Vector df[i] is the relative standard
		//		    deviation of the error associated with data point i.
		//                  Each df[i] must be positive. The values in df are
		//		    scaled by the subroutine so that their mean square
		//		    value is 1, and unscaled again on normal exit.
		//                  The mean square value of the df[i] is returned in
		//		    wk[6] on normal exit.
		//                  If the absolute standard deviations are known,
		//                  these should be provided in df and the error
		//                  variance parameter var (see below) should then be
		//                  set to 1.
		//                  If the relative standard deviations are unknown,
		//                  set each df[i]=1.
		//
		//     Y,C1,C2,C3   Spline coefficient arrays of length n. (output)
		//		    The value of the spline approximation at t is
		//
		//                    s(t) = ((c3[i]*d+c2[i])*d+c1[i])*d+y[i]
		//
		//                  where x[i] <= t < x[i+1] and d = t-x[i].
		//
		//		    That means
		//			 y[i]  contains the function value y(x[i])
		//			 c1[i] contains the 1st derivative y'(x[i])
		//			 c2[i] contains the 2nd derivative y''(x[i])
		//		    of the smoothing spline.
		//
		//            var   Error variance. (input/output)
		//                  If var is negative (i.e. unknown) then
		//                  the smoothing parameter is determined
		//                  by minimizing the generalized cross validation
		//                  and an estimate of the error variance is returned in var.
		//                  If var is non-negative (i.e. known) then the
		//                  smoothing parameter is determined to minimize
		//                  an estimate, which depends on var, of the true
		//                  mean square error, and var is unchanged.
		//                  In particular, if var is zero, then an
		//                  interpolating natural cubic spline is calculated.
		//                  var should be set to 1 if absolute standard
		//                  deviations have been provided in df (see above).
		//
		//            SE    Vector se of length n returning Bayesian standard
		//                  error estimates of the fitted spline values in y.
		//                  If a NullVector is passed to the subroutine
		//		    then no standard error estimates are computed.
		//
		//            WK    Work vector of length 7*(n+2)+1, arbitrary offset. 
		//                  On normal exit the first 7 values of wk are assigned 
		//                  as follows:
		//
		//                  ( here we arbitrarily start numbering from 0)
		//
		//                  wk[0] = smoothing parameter = rho/(rho+1)
		//	      	            If w[1]=0 (rho=0) an interpolating natural
		//			    cubic spline has been calculated.
		//                          If wk[1]=1 (rho=infinite) a least squares
		//                           regression line has been calculated.
		//                  wk[1] = estimate of the number of degrees of
		//                          freedom of the residual sum of squares
		//                          which reduces to the usual value of n-2
		//		            when a least squares regression line
		//		            is calculated.
		//                  wk[2] = generalized cross validation
		//                  wk[3] = mean square residual
		//                  wk[4] = estimate of the true mean square error
		//                          at the data points
		//                  wk[5] = estimate of the error variance
		//                          wk[6] coincides with the output value of
		//			    var if var is negative on input. It is
		//		            calculated with the unscaled values of the
		//			    df[i] to facilitate comparisons with a
		//			    priori variance estimates.
		//                  wk[6] = mean square value of the df[i]
		//
		//                  wk[2],wk[3],wk[4] are calculated with the df[i]
		//                  scaled to have mean square value 1. The unscaled
		//		    values of wk[2],wk[3],wk[4] may be calculated by
		//		    dividing by wk[6].
		//
		//  Return value:
		//		    = 0  if no errors occured.
		//                  = 1  if number of data points n is less than 3.
		//                  = 2  if input abscissae are not ordered x[i] < x[i+1].
		//                  = 3  if standard deviation df[i] not positive for some i.
		//
		// Description:
		//
		// Calculates a natural cubic spline curve which smoothes a given set
		// of data points, using statistical considerations to determine the amount
		// of smoothing required, as described in reference 2. If the error variance
		// is known, it should be supplied to the routine in 'var'. The degree of
		// smoothing is then determined by minimizing an unbiased estimate of the
		// true mean square error.  On the other hand, if the error variance is
		// not known, 'var' should be set to -1.0. The routine then determines the
		// degree of smoothing by minimizing the generalized cross validation.
		// This is asymptotically the same as minimizing the true mean square error
		// (see reference 1).  In this case, an estimate of the error variance is
		// returned in 'var' which may be compared with any a priori approximate
		// estimates. In either case, an estimate of the true mean square error
		// is returned in 'wk[4]'.  This estimate, however, depends on the error
		// variance estimate, and should only be accepted if the error variance
		// estimate is reckoned to be correct.
		// Bayesian estimates of the standard error of each smoothed data value are
		// returned in the array 'se' (if a non null vector is given for the 
		// paramenter 'se' - use (double*)0 if you don't want estimates). 
		// These also depend on the error variance estimate and should only 
		// be accepted if the error variance estimate is reckoned to be correct. 
		// See reference 4.
		//
		// The number of arithmetic operations and the amount of storage required by
		// the routine are both proportional to 'n', so that very large data sets may
		// be analysed. The data points do not have to be equally spaced or uniformly
		// weighted. The residual and the spline coefficients are calculated in the
		// manner described in reference 3, while the trace and various statistics,
		// including the generalized cross validation, are calculated in the manner
		// described in reference 2.
		//
		// When 'var' is known, any value of 'n' greater than 2 is acceptable. It is
		// advisable, however, for 'n' to be greater than about 20 when 'var'
		// is unknown. If the degree of smoothing done by this function when 'var' is
		// unknown is not satisfactory, the user should try specifying the degree
		// of smoothing by setting 'var' to a reasonable value.
		//
		// Notes:
		//
		// Algorithm 642, "cubgcv", collected algorithms from ACM.
		// Algorithm appeared in ACM-Trans. Math. Software, Vol.12, No. 2,
		// Jun., 1986, p. 150.
		//
		// Originally written by M.F.Hutchinson, CSIRO Division of Mathematics
		// and Statistics, P.O. Box 1965, Canberra, Act 2601, Australia.
		// Latest revision 15 august 1985.
		//
		// Fortran source code transfered to C++ by B.M.Gammel, Physik Department,
		// TU Muenchen, 8046 Garching, Germany. Revision of september 1992.
		//
		// References:
		//
		// 1.  Craven, Peter and Wahba, Grace, "Smoothing noisy data with spline
		//     functions", Numer. Math. 31, 377-403 (1979).
		// 2.  Hutchinson, M.F. and de Hoog, F.R., "Smoothing noisy data with spline
		//     functions", Numer. Math. 47, 99-106 (1985).
		// 3.  Reinsch, C.H., "Smoothing by spline functions", Numer. Math. 10,
		//     177-183 (1967).
		// 4.  Wahba, Grace, "Bayesian 'confidence intervals' for the cross-validated
		//     smoothing spline", J.R.Statist. Soc. B 45, 133-150 (1983).
		//
		//----------------------------------------------------------------------------//

		public override int Interpolate (IVector x, IVector y)
		{
			// check input parameters

			if ( ! MatchingIndexRange(x,y) )
				throw new ArgumentException("index range mismatch of vectors");
 
			// link original data vectors into base class
			base.x = x;
			base.y = y;

			// Empty data vectors - free auxilliary storage
			if (x.Empty()) 
			{
				y0.Remove();
				y1.Remove();
				y2.Remove();
				y3.Remove();
				se.Remove();
				wk.Remove();
				return 0;
			}

			int lo = x.Lo(),
				hi = x.Hi(),
				n  = x.Elements();

			// Resize the auxilliary vectors. Note, that there is no reallocation if the
			// vector already has the appropriate dimension.
			y0.Resize(lo,hi);
			y1.Resize(lo,hi);
			y2.Resize(lo,hi);
			y3.Resize(lo,hi);
			// se.Resize(lo,hi); // currently zero
			wk.Resize(0,7*(n+2));

			// set derivatives for a single point
			if (x.Elements() == 1) 
			{
				y0[lo] = y[lo];
				y1[lo] = y2[lo] = y3[lo] = 0.0;
				return 0;
			}

			// set derivatives for a line
			if (x.Elements() == 2) 
			{
				y0[lo] = y[lo]; 
				y0[hi] = y[hi];
				y1[lo] = y1[hi] = (y[hi]-y[lo]) / (x[hi]-x[lo]);
				y2[lo] = y2[hi] = 
					y3[lo] = y3[hi] = 0.0;
				return 0;
			} 
    
			// three or more points
			const double ratio = 2.0;
			double tau   = (Math.Sqrt(5.0)+1.0) / 2.0;

			int error_flag, i, wk_dim1, wk_offset;

			double avdf, avar,  gf1, gf2, gf3, gf4,
				avh, err, p, q, delta, r1, r2, r3, r4;
			double [] stat = new double[6];

			// adjust pointers to vectors so that indexing starts from 1
			const double *xx  = x.Store() - 1;
			const double *f   = y.Store() - 1;

			double  *yy = y0.Store() - 1; // coefficients calculated
			double	*c1 = y1.Store() - 1;
			double	*c2 = y2.Store() - 1;
			double	*c3 = y3.Store() - 1;
			double  *df = dy->Store() - 1;

			// index starts from 0
			double *ww = wk.Store();

			// set ss to (double*)0 if a NullVector is given
			double *ss = 0;
			if (! se.Empty()) ss = se.Store() - 1;

			// Parameter adjustments
			wk_dim1 = n + 2;
			wk_offset = wk_dim1;
			ww -= wk_offset;

			spint(n,xx,&avh,f,df,&avdf,yy,c1,c2,c3,
				&ww[wk_offset], &ww[wk_dim1 * 4], error_flag);

			if (error_flag) return error_flag;

			avar = var;
			if (var > zero) avar = var * avdf * avdf;

			// check for zero variance, i.e. compute a natural cubic spline
			if (var == zero) 
			{
				r1 = zero;
				goto natural_spline;
			}

			// find local minimum of gcv or the expected mean square error
			r1 = one;
			r2 = ratio * r1;
			spfit(n, xx, &avh, df, r2, &p, &q, &gf2, &avar, stat, yy, c1, c2, c3,
				&ww[wk_offset], &ww[wk_dim1 * 4],
				&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);

			for (;;) 
			{
				spfit(n, xx, &avh, df, r1, &p, &q, &gf1, &avar, stat, yy, c1, c2, c3,
					&ww[wk_offset], &ww[wk_dim1 * 4],
					&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);
				if (gf1 > gf2) break;
				// exit if p is zero
				if (p <= zero) goto spline_coefficients;
				r2 = r1;
				gf2 = gf1;
				r1 /= ratio;
			}

			r3 = ratio * r2;

			for (;;) 
			{
				spfit(n, xx, &avh, df, r3, &p, &q, &gf3, &avar, stat, yy, c1, c2, c3,
					&ww[wk_offset], &ww[wk_dim1 * 4],
					&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);

				if (gf3 > gf2) break;
				// exit if q is zero
				if (q <= zero) goto spline_coefficients;
				r2 = r3;
				gf2 = gf3;
				r3 = ratio * r3;
			}

			r2 = r3;
			gf2 = gf3;
			delta = (r2 - r1) / tau;
			r4 = r1 + delta;
			r3 = r2 - delta;
			spfit(n, xx, &avh, df, r3, &p, &q, &gf3, &avar, stat, yy, c1, c2, c3,
				&ww[wk_offset], &ww[wk_dim1 * 4],
				&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);

			spfit(n, xx, &avh, df, r4, &p, &q, &gf4, &avar, stat, yy, c1, c2, c3,
				&ww[wk_offset], &ww[wk_dim1 * 4],
				&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);

			do 
			{  // golden section search for local minimum

				if (gf3 > gf4) 
				{
					r1 = r3;
					gf1 = gf3;
					r3 = r4;
					gf3 = gf4;
					delta /= tau;
					r4 = r1 + delta;
					spfit(n, xx, &avh, df, r4, &p, &q, &gf4,
						&avar, stat, yy, c1, c2, c3,
						&ww[wk_offset], &ww[wk_dim1 * 4],
						&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);
				} 
				else 
				{
					r2 = r4;
					gf2 = gf4;
					r4 = r3;
					gf4 = gf3;
					delta /= tau;
					r3 = r2 - delta;
					spfit(n, xx, &avh, df, r3, &p, &q, &gf3,
						&avar, stat, yy, c1, c2, c3,
						&ww[wk_offset], &ww[wk_dim1 * 4],
						&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);
				}

				err = (r2-r1) / (r1+r2);

			} while (err * err + one > one && err > 1e-6);

			r1 = (r1+r2) * 0.5;

			natural_spline:

				spfit(n, xx, &avh, df, r1, &p, &q, &gf1, &avar, stat, yy, c1, c2, c3,
					&ww[wk_offset], &ww[wk_dim1 * 4],
					&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);

			spline_coefficients:

				spcof(n, xx, avh, f, df, p, q, yy, c1, c2, c3,
					&ww[wk_dim1 * 6], &ww[wk_dim1 * 7]);

			// optionally calculate standard error estimates
			if (var < zero) 
			{
				avar = stat[5];
				var = avar / (avdf * avdf);
			}

			if (ss != 0)
				sperr(n, xx, &avh, df, &ww[wk_offset], &p, &avar, ss);

			// unscale df
			for (i = 1; i <= n; ++i) df[i] *= avdf;

			// put statistics in wk
			for (i = 0; i <= 5; ++i)
				ww[i + wk_dim1] = stat[i];
			ww[wk_dim1 + 5] = stat[5] / (avdf * avdf);
			ww[wk_dim1 + 6] = avdf * avdf;

			return error_flag;
		}

		public override double GetX (double u) { return u; }
		public override double GetY (double u) { return CubicSplineHorner(u,x,y0,y1,y2,y3); }


		void SetErrorVariance (IVector dyy, double errvar)
			{
			dy.CopyFrom(dyy);
			var = errvar;
			}
	
		double GetErrorVariance()
		{
			return var;
		}
	
		//----------------------------------------------------------------------------//
		//
		// void MpCrossValidatedCubicSpline::GetFitResults (double &smpar, double &ndf, 
		//						    double &gcv, double &msqred,
		//						    double &msqerr, double &var, 
		//						    double &msqdf)
		//
		// Returns results of the fit from the working vector
		//
		// double &smpar:   wk[0] = smoothing parameter = rho/(rho+1)
		//	      	            If w[1]=0 (rho=0) an interpolating natural
		//			    cubic spline has been calculated.
		//                          If wk[1]=1 (rho=infinite) a least squares
		//                           regression line has been calculated.
		//
		// double &ndf:     wk[1] = estimate of the number of degrees of
		//                          freedom of the residual sum of squares
		//                          which reduces to the usual value of n-2
		//		            when a least squares regression line
		//		            is calculated.
		//
		// double &gcv:     wk[2] = generalized cross validation
		//
		// double &msqred:  wk[3] = mean square residual
		//
		// double &msqerr:  wk[4] = estimate of the true mean square error
		//                          at the data points
		//
		// double &var:     wk[5] = estimate of the error variance
		//                          wk[6] coincides with the output value of
		//			    var if var is negative on input. It is
		//		            calculated with the unscaled values of the
		//			    df[i] to facilitate comparisons with a
		//			    priori variance estimates.
		//
		// double &msqdf:   wk[6] = mean square value of the df[i]
		//
		//                  wk[2],wk[3],wk[4] are calculated with the df[i]
		//                  scaled to have mean square value 1. The unscaled
		//		    values of wk[2],wk[3],wk[4] may be calculated by
		//		    dividing by wk[6].
		//
		//----------------------------------------------------------------------------//

		void   GetFitResults (out double smpar, out double ndf, out double gcv, out double msqred,
	out double msqerr, out double var, out double msqdf)
		{
			if (! wk.Empty() ) 
			{
				smpar   = wk[0]; 
				ndf     = wk[1]; 
				gcv     = wk[2]; 
				msqred  = wk[3];
				msqerr  = wk[4]; 
				var     = wk[5]; 
				msqdf   = wk[6];
			}
		}

		//----------------------------------------------------------------------------//

}

#endif
	#endregion

	#region Shepard2d

	
	#endregion

}
