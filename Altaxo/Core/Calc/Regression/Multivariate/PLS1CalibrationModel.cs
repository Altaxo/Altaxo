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

using Altaxo.Calc.LinearAlgebra;
using System;

namespace Altaxo.Calc.Regression.Multivariate
{
	public class PLS1CalibrationModel : MultivariateCalibrationModel
	{
		private IROMatrix<double>[] _xWeights;
		private IROMatrix<double>[] _xLoads;
		private IROMatrix<double>[] _yLoads;
		private IROMatrix<double>[] _crossProduct;

		public override int NumberOfY
		{
			get { return _numberOfY; }
			set
			{
				_numberOfY = value;
				Allocate(value);
			}
		}

		protected void Allocate(int numberOfY)
		{
			_xWeights = new IROMatrix<double>[numberOfY];
			_xLoads = new IROMatrix<double>[numberOfY];
			_yLoads = new IROMatrix<double>[numberOfY];
			_crossProduct = new IROMatrix<double>[numberOfY];
		}

		public IROMatrix<double>[] XWeights
		{
			get { return _xWeights; }
		}

		public IROMatrix<double>[] XLoads
		{
			get { return _xLoads; }
		}

		public IROMatrix<double>[] YLoads
		{
			get { return _yLoads; }
		}

		public IROMatrix<double>[] CrossProduct
		{
			get { return _crossProduct; }
		}
	}
}