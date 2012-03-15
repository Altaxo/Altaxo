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
	/// Represents a quantity, consisting of a numeric value, the corresponding unit and, optionally, a SI prefix for the unit.
	/// </summary>
	public struct DimensionfulQuantity : IComparable<DimensionfulQuantity>
	{
		double _value;
		SIPrefix _prefix;
		IUnit _unit;

		/// <summary>Creates a dimensionless quantity with the provided value.</summary>
		/// <param name="value">Value.</param>
		public DimensionfulQuantity(double value)
			: this(value, null, Altaxo.Units.Dimensionless.Unity.Instance)
		{
		}

		/// <summary>Creates a quantity with the provided value in the given unit.</summary>
		/// <param name="value">The value of the created quantity.</param>
		/// <param name="unit">The unit of the created quantity.</param>
		public DimensionfulQuantity(double value, IUnit unit)
			: this(value, null, unit)
		{
		}

		/// <summary>Creates a quantity with the provided value in the given prefixed unit.</summary>
		/// <param name="value">The value of the created quantity.</param>
		/// <param name="prefix">The prefix of the unit.</param>
		/// <param name="unit">The unit of the created quantity.</param>
		public DimensionfulQuantity(double value, SIPrefix prefix, IUnit unit)
		{
			if (null == unit)
				throw new ArgumentNullException("unit");

			_value = value;
			_prefix = prefix;
			_unit = unit;
		}

		/// <summary>Creates a quantity with the provided value in the given prefixed unit.</summary>
		/// <param name="value">The value of the created quantity.</param>
		/// <param name="prefixedUnit">The prefixed unit of the created quanity.</param>
		public DimensionfulQuantity(double value, IPrefixedUnit prefixedUnit)
		{
			_value = value;
			_prefix = prefixedUnit.Prefix;
			_unit = prefixedUnit.Unit;
		}

		/// <summary>Determines whether this instance is equal to another quanity in all three components (value, prefix and unit). This is <b>not</b> a comparison for the physical equality of the quantities.</summary>
		/// <param name="a">Quantity to compare.</param>
		/// <returns>Returns <c>true</c> if <paramref name="a"/> is equal in all three components(value, prefix, unit) to this quantity; otherwise, <c>false</c>.</returns>
		public bool IsEqualInValuePrefixUnit(DimensionfulQuantity a)
		{
			return this._value == a._value && this.Prefix == a.Prefix && this._unit == a._unit; 
		}

		/// <summary>
		/// Creates an instance with a new value, and with the same prefix and unit as this quantity.
		/// </summary>
		/// <param name="value">New numeric value.</param>
		/// <returns>A new quantity with the provided value and the same prefix and unit as this quantity.</returns>
		public DimensionfulQuantity WithNewValue(double value)
		{
			return new DimensionfulQuantity(value, _prefix, _unit);
		}

		/// <summary>Gets a value indicating whether this instance is empty. It is empty if no unit has been associated so far with this instance.</summary>
		/// <value>Is <see langword="true"/> if this instance is empty; otherwise, <see langword="false"/>.</value>
		public bool IsEmpty
		{
			get
			{
				return _unit == null;
			}
		}

		/// <summary>Gets an empty, i.e. uninitialized, quantity.</summary>
		public static DimensionfulQuantity Empty
		{
			get
			{
				return new DimensionfulQuantity();
			}
		}

		/// <summary>Gets the unit of this quantity.</summary>
		public IUnit Unit
		{
			get
			{
				return _unit;
			}
		}

		/// <summary>Gets the SI prefix of this quantity.</summary>
		public SIPrefix Prefix
		{
			get
			{
				return _prefix ?? SIPrefix.None;
			}
		}

		/// <summary>Gets the numeric value of this quantity in the context of prefix and unit.</summary>
		public double Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>Converts this quantity to its numerical value in SI units (without prefix).</summary>
		public double AsValueInSIUnits
		{
			get
			{
				if (null == _unit)
					throw new InvalidOperationException("This instance is empty");

				double result = _value;
				if (null != _prefix)
					result = _prefix.ToSIUnit(result);
				if (null != _unit)
					result = _unit.ToSIUnit(result);
				return result;
			}
		}

		/// <summary>Converts this quantity to its numerical value in the given unit (without prefix).</summary>
		/// <param name="unit">The unit in which to get the numerical value of this quantity.</param>
		/// <returns>Numerical value of this quantity in the provided unit (without prefix).</returns>
		public double AsValueIn(IUnit unit)
		{
			if (null == unit)
				throw new ArgumentNullException("unit");
			if (null == _unit)
				throw new InvalidOperationException("This instance is empty");
			if (unit.SIUnit != this._unit.SIUnit)
				throw new ArgumentException(string.Format("Provided unit ({0}) is incompatible with this unit ({1})", unit.SIUnit, this._unit));

			return unit.FromSIUnit(AsValueInSIUnits);
		}

		/// <summary>Converts this quantity to its numerical value in the given unit, with the given prefix.</summary>
		/// <param name="prefix">The prefix of the unit in which to get the numerical value of this quantity.</param>
		/// <param name="unit">The unit in which to get the numerical value of this quantity.</param>
		/// <returns>Numerical value of this quantity in the provided unit with the provided prefix.</returns>
		public double AsValueIn(SIPrefix prefix, IUnit unit)
		{
			if (null == unit)
				throw new ArgumentNullException("unit");
			if (null == prefix)
				throw new ArgumentNullException("prefix");
			if (null == _unit)
				throw new InvalidOperationException("This instance is empty");
			if (unit.SIUnit != this._unit.SIUnit)
				throw new ArgumentException(string.Format("Provided unit ({0}) is incompatible with this unit ({1})", unit.SIUnit, this._unit));

			return prefix.FromSIUnit(unit.FromSIUnit(AsValueInSIUnits));
		}

		/// <summary>Converts this quantity to its numerical value in the given unit, with the given prefix.</summary>
		/// <param name="prefixedUnit">The prefixed unit in which to get the numerical value of this quantity.</param>
		/// <returns>Numerical value of this quantity in the provided unit with the provided prefix.</returns>
		public double AsValueIn(IPrefixedUnit prefixedUnit)
		{
			return AsValueIn(prefixedUnit.Prefix, prefixedUnit.Unit);
		}


		/// <summary>Gets this quantity in SI units (without prefix).</summary>
		public DimensionfulQuantity AsQuantityInSIUnits
		{
			get
			{
				return new DimensionfulQuantity(AsValueInSIUnits, _unit == null ? null : _unit.SIUnit);
			}
		}

		/// <summary>Converts this quantity to another quantity in the provided unit (without prefix).</summary>
		/// <param name="unit">The unit to convert the quantity to.</param>
		/// <returns>New instance of a quantity in the provided unit (without prefix).</returns>
		public DimensionfulQuantity AsQuantityIn(IUnit unit)
		{
			return new DimensionfulQuantity(AsValueIn(unit), null, unit);
		}

		/// <summary>Converts this quantity to another quantity in the provided unit, with the provided prefix.</summary>
		/// <param name="prefix">The prefix of the unit to convert the quantity to.</param>
		/// <param name="unit">The unit to convert the quantity to.</param>
		/// <returns>New instance of a quantity in the provided unit with the provided prefix.</returns>
		public DimensionfulQuantity AsQuantityIn(SIPrefix prefix, IUnit unit)
		{
			return new DimensionfulQuantity(AsValueIn(prefix, unit), prefix, unit);
		}

		/// <summary>Converts this quantity to another quantity in the provided prefixed unit.</summary>
		/// <param name="prefixedUnit">The prefixed unit to convert the quantity to.</param>
		/// <returns>New instance of a quantity in the provided prefixed unit.</returns>
		public DimensionfulQuantity AsQuantityIn(IPrefixedUnit prefixedUnit)
		{
			return AsQuantityIn(prefixedUnit.Prefix, prefixedUnit.Unit);
		}



		/// <summary>Compares this quanitity to another quantity.</summary>
		/// <param name="other">The other quantity to compare with.</param>
		/// <returns>The value is 1, if this quantity is greater than the other quantity; 0 if both quantities are equal, and -1 if this quantity is less than the other quantity.</returns>
		public int CompareTo(DimensionfulQuantity other)
		{
			if(null == this._unit || null== other._unit || this._unit.SIUnit != other._unit.SIUnit)
				throw new ArgumentException(string.Format("Incompatible units in comparison of a quantity in {0} with a quantity in {1}",this._unit.Name, other._unit.Name));

			double thisval = this.AsValueIn(_unit.SIUnit);
			double otherval = other.AsValueIn(_unit.SIUnit);
			return thisval.CompareTo(otherval);
		}
	}
}
