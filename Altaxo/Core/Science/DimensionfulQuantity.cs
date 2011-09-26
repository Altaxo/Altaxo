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
	/// Represents a quantity, consisting of a numeric value, the corresponding unit and, optionally, a SI prefix for the unit.
	/// </summary>
	public struct DimensionfulQuantity : IComparable<DimensionfulQuantity>
	{
		double _value;
		SIPrefix _prefix;
		IUnit _unit;

		public DimensionfulQuantity(double value)
			: this(value, null, null)
		{
		}

		public DimensionfulQuantity(double value, IUnit unit)
			: this(value, null, unit)
		{
		}

		public DimensionfulQuantity(double value, SIPrefix prefix, IUnit unit)
		{
			_value = value;
			_prefix = prefix;
			_unit = unit;
		}

		public bool IsEqualInValuePrefixUnit(DimensionfulQuantity a)
		{
			return this.Value == a.Value && this.Prefix == a.Prefix && this.Unit == a.Unit; 
		}

		/// <summary>
		/// Creates a quantity with a new provided value, and with the same prefix and unit as this quantity.
		/// </summary>
		/// <param name="value">New numeric value.</param>
		/// <returns>A new quantity with the provided value and the same prefix and unit as this quantity.</returns>
		public DimensionfulQuantity NewValue(double value)
		{
			return new DimensionfulQuantity(value, _prefix, _unit);
		}

		public IUnit Unit
		{
			get
			{
				return _unit ?? Dimensionless.Instance;
			}
		}

		public SIPrefix Prefix
		{
			get
			{
				return _prefix ?? SIPrefix.None;
			}
		}

		public double Value
		{
			get
			{
				return _value;
			}
		}

		public double InSIUnits
		{
			get
			{
				double result = _value;
				if (null != _prefix)
					result = _prefix.ToSIUnit(result);
				if (null != _unit)
					result = _unit.ToSIUnit(result);
				return result;
			}
		}

		public double AsValueIn(IUnit unit)
		{
			if (null == unit)
				throw new ArgumentNullException("unit");
			if (unit.SIUnit != this.Unit.SIUnit)
				throw new ArgumentException(string.Format("Provided unit ({0}) is incompatible with this unit ({1})", unit.SIUnit, this.Unit));

			return unit.FromSIUnit(InSIUnits);
		}

		public DimensionfulQuantity AsQuantityIn(IUnit unit)
		{
			return new DimensionfulQuantity(AsValueIn(unit), null, unit);
		}

		public double AsValueIn(SIPrefix prefix, IUnit unit)
		{
			if (null == unit)
				throw new ArgumentNullException("unit");
			if (null == prefix)
				throw new ArgumentNullException("prefix");
			if (unit.SIUnit != this.Unit.SIUnit)
				throw new ArgumentException(string.Format("Provided unit ({0}) is incompatible with this unit ({1})", unit.SIUnit, this.Unit));

			return prefix.FromSIUnit(unit.FromSIUnit(InSIUnits));
		}

		public DimensionfulQuantity AsQuantityIn(IPrefixedUnit prefixedUnit)
		{
			return AsQuantityIn(prefixedUnit.Prefix, prefixedUnit.Unit);
		}

		public DimensionfulQuantity AsQuantityIn(SIPrefix prefix, IUnit unit)
		{
			return new DimensionfulQuantity(AsValueIn(prefix, unit), prefix, unit);
		}

		public DimensionfulQuantity InSIUnitsAsQuantity
		{
			get
			{
				return new DimensionfulQuantity(InSIUnits, _unit == null ? null : _unit.SIUnit);
			}
		}


		public int CompareTo(DimensionfulQuantity other)
		{
			if(this._unit.SIUnit != other._unit.SIUnit)
				throw new ArgumentException(string.Format("Incompatible units in comparison of a quantity in {0} with a quantity in {1}",this._unit.Name, other._unit.Name));

			double thisval = this.AsValueIn(_unit.SIUnit);
			double otherval = other.AsValueIn(_unit.SIUnit);
			return thisval.CompareTo(otherval);
		}
	}
}
