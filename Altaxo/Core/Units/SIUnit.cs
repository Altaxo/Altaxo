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

namespace Altaxo.Units
{
	/// <summary>
	/// Represents a SI unit.
	/// </summary>
	public abstract class SIUnit : IUnit,  IEquatable<SIUnit>, IEquatable<IUnit>
	{
		sbyte _metre;
		sbyte _kilogram;
		sbyte _second;
		sbyte _ampere;
		sbyte _kelvin;
		sbyte _mole;
		sbyte _candela;

		/// <summary>
		/// Cache for unit names. Here the units, as specified by their individual powers of basic units, act as keys, whereas the commonly used name of this units are the values.
		/// </summary>
		static Dictionary<SIUnit, string> _specialNames = new Dictionary<SIUnit, string>();

		/// <summary>
		/// Constructor of the SI unit.
		/// </summary>
		/// <param name="metre">Power of 'Metre' units that the constructed unit will contain.</param>
		/// <param name="kilogram">Power of 'Kilogram' units that the constructed unit will contain.</param>
		/// <param name="second">Power of 'Second' units that the constructed unit will contain.</param>
		/// <param name="ampere">Power of 'Ampere' units that the constructed unit will contain.</param>
		/// <param name="kelvin">Power of 'Kelvin' units that the constructed unit will contain.</param>
		/// <param name="mole">Power of 'Mole' units that the constructed unit will contain.</param>
		/// <param name="candela">Power of 'Candela' units that the constructed unit will contain.</param>
		public SIUnit(sbyte metre, sbyte kilogram, sbyte second, sbyte ampere, sbyte kelvin, sbyte mole, sbyte candela)
		{
			_metre = metre;
			_kilogram = kilogram;
			_second = second;
			_ampere = ampere;
			_kelvin = kelvin;
			_mole = mole;
			_candela = candela;
		}

		/// <summary>
		/// Multiplies this unit by another unit <paramref name="b"/>.
		/// </summary>
		/// <param name="b">Other unit.</param>
		void Multiply(SIUnit b)
		{
			this._metre += b._metre;
			this._kilogram += b._kilogram;
			this._second += b._second;
			this._ampere += b._ampere;
			this._kelvin += b._kelvin;
			this._mole += b._mole;
			this._candela += b._candela;
		}

		/// <summary>Divides this unit by another unit <paramref name="b"/>.</summary>
		/// <param name="b">Other unit.</param>
		void DivideBy(SIUnit b)
		{
			this._metre -= b._metre;
			this._kilogram -= b._kilogram;
			this._second -= b._second;
			this._ampere -= b._ampere;
			this._kelvin -= b._kelvin;
			this._mole -= b._mole;
			this._candela -= b._candela;
		}

		/// <summary>
		/// Takes the inverse of this unit.
		/// </summary>
		void Invert()
		{
			this._metre = (sbyte)-this._metre;
			this._kilogram = (sbyte)-this._kilogram;
			this._second = (sbyte)-this._second;
			this._ampere = (sbyte)-this._ampere;
			this._kelvin = (sbyte)-this._kelvin;
			this._mole = (sbyte)-this._mole;
			this._candela = (sbyte)-this._candela;
		}

		/// <summary>Compares this unit with another unit <paramref name="b"/> and returns <c>true</c> when both are equal.</summary>
		/// <param name="b">The other unit.</param>
		/// <returns><c>True</c> when both units are equal.</returns>
		public bool Equals(SIUnit b)
		{
			return null == b ? false :
			this._metre == b._metre &&
			this._kilogram == b._kilogram &&
			this._second == b._second &&
			this._ampere == b._ampere &&
			this._kelvin == b._kelvin &&
			this._mole == b._mole &&
			this._candela == b._candela;
		}

		/// <summary>Compares this unit with another unit <paramref name="obj"/> and returns <c>true</c> when both are equal.</summary>
		/// <param name="obj">The other unit.</param>
		/// <returns><c>True</c> when both units are equal.</returns>
		public bool Equals(IUnit obj)
		{
			SIUnit b = obj as SIUnit;
			return null == b ? false : Equals(b);
		}

		/// <summary>Determines whether the specified <see cref="System.Object"/> is equal to this instance.</summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			SIUnit b = obj as SIUnit;
			return null == b ? false : Equals(b);
		}


		/// <summary>Returns a hash code for this instance.</summary>
		/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
		public override int GetHashCode()
		{
			return
				_metre << 24 +
				_kilogram << 20 +
				_second << 16 +
				_ampere << 12 +
				_kelvin << 8 +
				_mole << 4 +
				_candela;
		}


		/// <summary>Full name of the unit.</summary>
		public abstract string Name
		{
			get; 
		}

		/// <summary>Usual shortcut of the unit.</summary>
		public abstract string ShortCut
		{
			get; 
		}

		/// <summary>Converts <paramref name="x"/> to the corresponding SI unit.</summary>
		/// <param name="x">Value to convert.</param>
		/// <returns>The corresponding value of <paramref name="x"/> in SI units. Since this instance represents a SI unit, the value <paramref name="x"/> is returned unchanged.</returns>
		public double ToSIUnit(double x)
		{
			return x;
		}

		/// <summary>Converts <paramref name="x"/> (in SI units) to the corresponding value in this unit.</summary>
		/// <param name="x">Value in SI units.</param>
		/// <returns>The corresponding value in this unit. Since this instance represents a SI unit, the value <paramref name="x"/> is returned unchanged.</returns>
		public double FromSIUnit(double x)
		{
			return x;
		}

		/// <summary>Returns a list of possible prefixes for this unit (like µ, m, k, M, G..).</summary>
		public abstract ISIPrefixList Prefixes
		{
			get;
		}

		/// <summary>Returns the corresponding SI unit. Since this instance already represents a SI unit, the returned value is this instance itself.</summary>
		SIUnit IUnit.SIUnit
		{
			get { return this; }
		}
	}
}
