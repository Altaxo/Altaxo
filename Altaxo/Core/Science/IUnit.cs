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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	/// <summary>
	/// Represents an arbitrary unit (SI or any other unit).
	/// </summary>
	public interface IUnit
	{
		/// <summary>Full name of the unit.</summary>
		string Name { get; }

		/// <summary>Usual shortcut of the unit.</summary>
		string ShortCut { get; }

		/// <summary>
		/// Converts <paramref name="x"/> to the corresponding SI unit.
		/// </summary>
		/// <param name="x">Value to convert.</param>
		/// <returns>The corresponding value of <paramref name="x"/> in SI units.</returns>
		double ToSIUnit(double x);

		/// <summary>
		/// Converts <paramref name="x"/> (in SI units) to the corresponding value in this unit.
		/// </summary>
		/// <param name="x">Value in SI units.</param>
		/// <returns>The corresponding value in this unit.</returns>
		double FromSIUnit(double x);

		/// <summary>
		/// Returns a list of possible prefixes for this unit (like µ, m, k, M, G..).
		/// </summary>
		ISIPrefixList Prefixes { get; }

		/// <summary>
		/// Returns the corresponding SI unit.
		/// </summary>
		SIUnit SIUnit { get; }
	}

	/// <summary>
	/// Represents no unit at all.
	/// </summary>
	public class Dimensionless : SIUnit
	{
		static readonly Dimensionless _instance = new Dimensionless();

		/// <summary>Gets the (single) instance of this dimensionless unit.</summary>
		public static Dimensionless Instance { get { return _instance; } }

		/// <summary>Prevents a default instance of the <see cref="Dimensionless"/> class from being created.</summary>
		private Dimensionless() : base(0, 0, 0, 0, 0, 0, 0) { }

		/// <summary>Full name of the unit.</summary>
		public override string Name
		{
			get { return ""; }
		}

		/// <summary>Usual shortcut of the unit.</summary>
		public override string ShortCut
		{
			get { return ""; }
		}

		/// <summary>Returns a list of possible prefixes for this unit (like µ, m, k, M, G..).</summary>
		public override ISIPrefixList Prefixes
		{
			get { return SIPrefix.ListWithNonePrefixOnly; }
		}
	}






}
