using System;
using Altaxo.Data;

namespace Altaxo.Calc
{
	/// <summary>
	/// Contains static methods for linear interpolation of data.
	/// </summary>
	public class LinearInterpolation
	{
	
		public static int GetNextIndexOfValidPair(INumericColumn xcol, INumericColumn ycol, int sourceLength, int currentIndex)
		{
			for(int sourceIndex=currentIndex;sourceIndex<sourceLength;sourceIndex++)
			{
				if(!double.IsNaN(xcol.GetDoubleAt(sourceIndex)) && !double.IsNaN(ycol.GetDoubleAt(sourceIndex)))
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
			INumericColumn xcol,
			INumericColumn ycol,
			int sourceLength,
			double xstart, double xincrement, int numberOfValues,
			double yOutsideOfBounds,
			out DoubleColumn resultCol)
		{

			resultCol = new DoubleColumn();
			
			int currentIndex = GetNextIndexOfValidPair(xcol,ycol,sourceLength, 0);
			if(currentIndex<0)
				return "The two columns don't contain a valid pair of values";

			int nextIndex = GetNextIndexOfValidPair(xcol,ycol,sourceLength,currentIndex+1);
			if(nextIndex<0)
				return "The two columns contain only one valid pair of values, but at least two valid pairs are neccessary!";
	

			double x_current = xcol.GetDoubleAt(currentIndex);
			double x_next = xcol.GetDoubleAt(nextIndex);

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
					resultCol[resultIndex] = Interpolate(x_result, x_current, x_next, ycol.GetDoubleAt(currentIndex),ycol.GetDoubleAt(nextIndex));
				}
				else
				{
					currentIndex = nextIndex;
					x_current    = x_next;
					nextIndex = GetNextIndexOfValidPair(xcol,ycol,sourceLength, currentIndex+1);
					if(nextIndex<0)
						break;

					x_next = xcol.GetDoubleAt(nextIndex);
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
}
