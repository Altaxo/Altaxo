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

namespace Altaxo.Calc.Regression
{
	/// <summary>
	/// Class for doing a quick and dirty regression of order 1 only returning intercept and slope.
	/// Can not handle too big or too input values.
	/// </summary>
	public class QuickLinearRegression
	{
		private double _n;
		private double _sx;
		private double _sxx;
		private double _sy;
		private double _syy;
		private double _syx;

		/// <summary>
		/// Adds a data point to the regression.
		/// </summary>
		/// <param name="x">The x value of the data point.</param>
		/// <param name="y">The y value of the data point.</param>
		public void Add(double x, double y)
		{
			_n += 1;
			_sx += x;
			_sxx += x * x;
			_sy += y;
			_syy += y * y;
			_syx += y * x;
		}

		/// <summary>
		/// Returns the number of entries added.
		/// </summary>
		public double N
		{
			get
			{
				return _n;
			}
		}

		/// <summary>
		/// Gets the intercept value of the linear regression. Returns NaN if not enough data points entered.
		/// </summary>
		/// <returns>The intercept value or NaN if not enough data points are entered.</returns>
		public double GetA0()
		{
			return (_sy * _sxx - _syx * _sx) / GetDeterminant();
		}

		/// <summary>
		/// Gets the slope value of the linear regression. Returns NaN if not enough data points entered.
		/// </summary>
		/// <returns>The slope value or NaN if not enough data points are entered.</returns>
		public double GetA1()
		{
			return (_n * _syx - _sx * _sy) / GetDeterminant();
		}

		/// <summary>
		/// Gets the intercept of the linear regression with the X-axis. Returns NaN if not enough data points entered.
		/// </summary>
		/// <returns>The intercept value with the X-Axis, i.e. the point where the regression value is zero. Returns NaN if not enough data points are entered.</returns>
		public double GetX0()
		{
			return -(_sy * _sxx - _syx * _sx) / (_n * _syx - _sx * _sy);
		}

		/// <summary>
		/// Returns the determinant of regression. If zero, not enough data points have been entered.
		/// </summary>
		/// <returns>The determinant of the regression.</returns>
		public double GetDeterminant()
		{
			return _n * _sxx - _sx * _sx;
		}

		public double MeanX
		{
			get
			{
				return _sx / _n;
			}
		}

		public double MeanY
		{
			get
			{
				return _sy / _n;
			}
		}

		public double PearsonCorrelationCoefficient
		{
			get
			{
				return (_n * _syx - _sx * _sy) / Math.Sqrt((_n * _sxx - _sx * _sx) * (_n * _syy - _sy * _sy));
			}
		}
	}
}