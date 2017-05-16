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
	public interface IPLS2CalibrationModel : IMultivariateCalibrationModel
	{
		IROMatrix<double> XWeights
		{
			get;
		}

		IROMatrix<double> XLoads
		{
			get;
		}

		IROMatrix<double> YLoads
		{
			get;
		}

		IROMatrix<double> CrossProduct
		{
			get;
		}
	}

	public class PLS2CalibrationModel : MultivariateCalibrationModel, IPLS2CalibrationModel
	{
		private IROMatrix<double> _xWeights;
		private IROMatrix<double> _xLoads;
		private IROMatrix<double> _yLoads;
		private IROMatrix<double> _crossProduct;

		public IROMatrix<double> XWeights
		{
			get { return _xWeights; }
			set { _xWeights = value; }
		}

		public IROMatrix<double> XLoads
		{
			get { return _xLoads; }
			set { _xLoads = value; }
		}

		public IROMatrix<double> YLoads
		{
			get { return _yLoads; }
			set { _yLoads = value; }
		}

		public IROMatrix<double> CrossProduct
		{
			get { return _crossProduct; }
			set { _crossProduct = value; }
		}
	}
}