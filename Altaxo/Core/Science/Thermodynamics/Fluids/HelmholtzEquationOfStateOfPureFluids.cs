#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	public abstract class HelmholtzEquationOfStateOfPureFluids : HelmholtzEquationOfState
	{
		#region Constants

		/// <summary>Gets the triple point temperature in K.</summary>
		public abstract double TriplePointTemperature { get; }

		/// <summary>Gets the triple point pressure in Pa.</summary>
		public abstract double TriplePointPressure { get; }

		/// <summary>Gets the saturated liquid density at the triple point in kg/m³.</summary>
		public abstract double TriplePointSaturatedLiquidDensity { get; }

		/// <summary>Gets the saturated vapor density at the triple point in kg/m³.</summary>
		public abstract double TriplePointSaturatedVaporDensity { get; }

		/// <summary>Gets the temperature at the critical point in Kelvin.</summary>
		public abstract double CriticalPointTemperature { get; }

		/// <summary>Gets the pressure at the critical point in Pa.</summary>
		public abstract double CriticalPointPressure { get; }

		/// <summary>Gets the density at the critical point in kg/m³.</summary>
		public abstract double CriticalPointDensity { get; }

		/// <summary>Gets the molecular weight in kg/mol.</summary>
		public abstract double MolecularWeight { get; } // kg/mol

		#endregion Constants
	}
}
