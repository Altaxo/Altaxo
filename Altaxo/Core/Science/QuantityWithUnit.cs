using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	/// <summary>
	/// Represents a quantity, consisting of a numeric value, the corresponding unit and, optionally, a SI prefix for the unit.
	/// </summary>
	public struct QuantityWithUnit
	{
		double _value;
		SIPrefix _prefix;
		IUnit _unit;

		public QuantityWithUnit(double value)
			: this(value, null, null)
		{
		}

		public QuantityWithUnit(double value, IUnit unit)
			: this(value, null, unit)
		{
		}

		public QuantityWithUnit(double value, SIPrefix prefix, IUnit unit)
		{
			_value = value;
			_prefix = prefix;
			_unit = unit;
		}

		/// <summary>
		/// Creates a quantity with a new provided value, and with the same prefix and unit as this quantity.
		/// </summary>
		/// <param name="value">New numeric value.</param>
		/// <returns>A new quantity with the provided value and the same prefix and unit as this quantity.</returns>
		public QuantityWithUnit NewValue(double value)
		{
			return new QuantityWithUnit(value, _prefix, _unit);
		}

		public IUnit Unit
		{
			get
			{
				return _unit ?? UnitLess.Instance;
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

		public QuantityWithUnit AsQuantityIn(IUnit unit)
		{
			return new QuantityWithUnit(AsValueIn(unit), null, unit);
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

		public QuantityWithUnit AsQuantityIn(SIPrefix prefix, IUnit unit)
		{
			return new QuantityWithUnit(AsValueIn(prefix, unit), prefix, unit);
		}

		public QuantityWithUnit InSIUnitsAsQuantity
		{
			get
			{
				return new QuantityWithUnit(InSIUnits, _unit == null ? null : _unit.SIUnit);
			}
		}

	}
}
