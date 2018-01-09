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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Units
{
	/// <summary>
	/// Represents an SI (Système international) unit.
	/// </summary>
	public class SIUnit : IUnit, IEquatable<SIUnit>, IEquatable<IUnit>
	{
		private sbyte _metre;
		private sbyte _kilogram;
		private sbyte _second;
		private sbyte _ampere;
		private sbyte _kelvin;
		private sbyte _mole;
		private sbyte _candela;

		/// <summary>
		/// Cache for unit names. Here the units, as specified by their individual powers of basic units, act as keys, whereas the commonly used name of this units are the values.
		/// </summary>
		private static Dictionary<SIUnit, string> _specialNames = new Dictionary<SIUnit, string>();

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

		public static SIUnit operator *(SIUnit x, SIUnit y)
		{
			checked
			{
				var metre = (sbyte)(x._metre + y._metre);
				var kilogram = (sbyte)(x._kilogram + y._kilogram);
				var second = (sbyte)(x._second + y._second);
				var ampere = (sbyte)(x._ampere + y._ampere);
				var kelvin = (sbyte)(x._kelvin + y._kelvin);
				var mole = (sbyte)(x._mole + y._mole);
				var candela = (sbyte)(x._candela + y._candela);
				return new SIUnit(metre, kilogram, second, ampere, kelvin, mole, candela);
			}
		}

		public static SIUnit operator /(SIUnit x, SIUnit y)
		{
			checked
			{
				var metre = (sbyte)(x._metre - y._metre);
				var kilogram = (sbyte)(x._kilogram - y._kilogram);
				var second = (sbyte)(x._second - y._second);
				var ampere = (sbyte)(x._ampere - y._ampere);
				var kelvin = (sbyte)(x._kelvin - y._kelvin);
				var mole = (sbyte)(x._mole - y._mole);
				var candela = (sbyte)(x._candela - y._candela);
				return new SIUnit(metre, kilogram, second, ampere, kelvin, mole, candela);
			}
		}

		/// <summary>Compares this unit with another unit <paramref name="b"/> and returns <c>true</c> when both are equal. Two SI units are considered equal if the exponents
		/// are equal, independently of the unit name. This means e.g. that J (Joule), Nm, and Ws are considered equal. If you want to compare the name too, use <see cref="Equals(IUnit)"/></summary>.
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

		public static bool operator ==(SIUnit a, SIUnit b)
		{
			return a?.Equals(b) ?? false;
		}

		public static bool operator ==(SIUnit a, IUnit b)
		{
			return a?.Equals(b) ?? false;
		}

		public static bool operator !=(SIUnit a, SIUnit b)
		{
			return !(a == b);
		}

		public static bool operator !=(SIUnit a, IUnit b)
		{
			return !(a == b);
		}

		/// <summary>Compares this unit with another unit <paramref name="obj"/> and returns <c>true</c> if both are equal.
		/// To be equal, the other unit has to be (i) a SI unit, and (ii) the same name. Thus, J (Joule) and Nm (Newtonmeter) are not considered equal.</summary>
		/// <param name="obj">The other unit.</param>
		/// <returns><c>True</c> when both units are equal.</returns>
		public bool Equals(IUnit obj)
		{
			if (!(obj is SIUnit other))
				return false;

			return this.GetType() == other.GetType();
		}

		/// <summary>Determines whether the specified <see cref="System.Object"/> is equal to this instance.</summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is IUnit other))
				return false;

			return Equals(other);
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
		public virtual string Name
		{
			get
			{
				var stb = new StringBuilder();
				var invCult = System.Globalization.CultureInfo.InvariantCulture;

				if (_metre != 0)
				{
					if (stb.Length != 0)
						stb.Append(" ");
					stb.Append("m");
					if (_metre != 1)
					{
						stb.Append("^");
						stb.Append(_metre.ToString(invCult));
					}
				}

				if (_kilogram != 0)
				{
					if (stb.Length != 0)
						stb.Append(" ");
					stb.Append("kg");
					if (_kilogram != 1)
					{
						stb.Append("^");
						stb.Append(_kilogram.ToString(invCult));
					}
				}

				if (_second != 0)
				{
					if (stb.Length != 0)
						stb.Append(" ");
					stb.Append("s");
					if (_second != 1)
					{
						stb.Append("^");
						stb.Append(_second.ToString(invCult));
					}
				}

				if (_ampere != 0)
				{
					if (stb.Length != 0)
						stb.Append(" ");
					stb.Append("A");
					if (_ampere != 1)
					{
						stb.Append("^");
						stb.Append(_ampere.ToString(invCult));
					}
				}

				if (_kelvin != 0)
				{
					if (stb.Length != 0)
						stb.Append(" ");
					stb.Append("K");
					if (_kelvin != 1)
					{
						stb.Append("^");
						stb.Append(_kelvin.ToString(invCult));
					}
				}

				if (_mole != 0)
				{
					if (stb.Length != 0)
						stb.Append(" ");
					stb.Append("mol");
					if (_mole != 1)
					{
						stb.Append("^");
						stb.Append(_mole.ToString(invCult));
					}
				}

				if (_candela != 0)
				{
					if (stb.Length != 0)
						stb.Append(" ");
					stb.Append("cd");
					if (_candela != 1)
					{
						stb.Append("^");
						stb.Append(_candela.ToString(invCult));
					}
				}
				return stb.ToString();
			}
		}

		/// <summary>Usual shortcut of the unit.</summary>
		public virtual string ShortCut
		{
			get { return Name; }
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
		public virtual ISIPrefixList Prefixes
		{
			get
			{
				return SIPrefix.ListWithNonePrefixOnly;
			}
		}

		/// <summary>Returns the corresponding SI unit. Since this instance already represents a SI unit, the returned value is this instance itself.</summary>
		SIUnit IUnit.SIUnit
		{
			get { return this; }
		}
	}
}