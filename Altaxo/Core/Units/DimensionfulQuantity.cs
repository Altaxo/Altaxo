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

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Represents a quantity, consisting of a numeric value, the corresponding unit and, optionally, a SI prefix for the unit.
  /// Please note that two <see cref="DimensionfulQuantity"/> instances are considered equal only if (i) the units are equal, (ii) the prefixes are equal, and (iii) the values are equal.
  /// If you want to compare the SI values, please compare the <see cref="AsValueInSIUnits"/> values.
  /// </summary>
  public struct DimensionfulQuantity : IComparable, IComparable<DimensionfulQuantity>, IEquatable<DimensionfulQuantity>, IFormattable
  {
    private double _value;
    private SIPrefix? _prefix;
    private IUnit? _unit;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionfulQuantity), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionfulQuantity)obj;

        info.AddValue("Value", s.Value);
        info.AddValue("Prefix", s.Prefix);
        info.AddValueOrNull("Unit", s.IsEmpty ? null : s.Unit);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var value = info.GetDouble("Value");
        var prefix = info.GetValue<SIPrefix>("Prefix", parent);
        var unit = info.GetValueOrNull<IUnit>("Unit", parent);

        if (unit is null)
          return DimensionfulQuantity.Empty;
        else
          return new DimensionfulQuantity(value, prefix, unit);
      }
    }
    #endregion

    /// <summary>Creates a dimensionless quantity with the provided value.</summary>
    /// <param name="value">Value.</param>
    public DimensionfulQuantity(double value)
        : this(value, SIPrefix.None, Altaxo.Units.Dimensionless.Unity.Instance)
    {
    }

    /// <summary>Creates a quantity with the provided value in the given unit.</summary>
    /// <param name="value">The value of the created quantity.</param>
    /// <param name="unit">The unit of the created quantity.</param>
    public DimensionfulQuantity(double value, IUnit unit)
        : this(value, SIPrefix.None, unit)
    {
    }

    /// <summary>Creates a quantity with the provided value in the given prefixed unit.</summary>
    /// <param name="value">The value of the created quantity.</param>
    /// <param name="prefix">The prefix of the unit.</param>
    /// <param name="unit">The unit of the created quantity.</param>
    public DimensionfulQuantity(double value, SIPrefix prefix, IUnit unit)
    {
      if (unit is null)
        throw new ArgumentNullException(nameof(unit));
      prefix ??= SIPrefix.None;

      if (unit is IPrefixedUnit punit)
      {
        _unit = punit.Unit;
        (var newPrefix, var remainingFactor) = SIPrefix.FromMultiplication(prefix, punit.Prefix);
        _prefix = newPrefix;
        _value = value * remainingFactor;
      }
      else
      {
        _value = value;
        _prefix = prefix ?? SIPrefix.None;
        _unit = unit;
      }

      if (_unit is IPrefixedUnit)
        throw new ArgumentException("Nested IPrefixedUnit is not supported");
      if (_unit is IBiasedUnit && _prefix != SIPrefix.None)
        throw new ArgumentException("Biased units must not have a prefix!");
    }

    /// <summary>Creates a quantity with the provided value in the given prefixed unit.</summary>
    /// <param name="value">The value of the created quantity.</param>
    /// <param name="prefixedUnit">The prefixed unit of the created quanity.</param>
    public DimensionfulQuantity(double value, IPrefixedUnit prefixedUnit)
    {
      if (prefixedUnit is null)
        throw new ArgumentNullException(nameof(prefixedUnit));

      _value = value;
      _prefix = prefixedUnit.Prefix;
      _unit = prefixedUnit.Unit;
    }

    /// <summary>Determines whether this instance is equal to another quanity in all three components (value, prefix and unit). This is <b>not</b> a comparison for the physical equality of the quantities.</summary>
    /// <param name="a">Quantity to compare.</param>
    /// <returns>Returns <c>true</c> if <paramref name="a"/> is equal in all three components(value, prefix, unit) to this quantity; otherwise, <c>false</c>.</returns>
    public bool IsEqualInValuePrefixUnit(DimensionfulQuantity a)
    {
      return _value == a._value && Prefix == a.Prefix && _unit == a._unit;
    }

    /// <summary>
    /// Creates an instance with a new value, and with the same prefix and unit as this quantity.
    /// </summary>
    /// <param name="value">New numeric value.</param>
    /// <returns>A new quantity with the provided value and the same prefix and unit as this quantity.</returns>
    public DimensionfulQuantity WithNewValue(double value)
    {
      if (_unit is null)
        throw new InvalidOperationException($"{nameof(DimensionfulQuantity)} is uninitialized!");

      return new DimensionfulQuantity(value, Prefix, _unit);
    }

    /// <summary>Gets a value indicating whether this instance is empty. It is empty if no unit has been associated so far with this instance.</summary>
    /// <value>Is <see langword="true"/> if this instance is empty; otherwise, <see langword="false"/>.</value>
    public bool IsEmpty
    {
      get
      {
        return _unit is null;
      }
    }

    /// <summary>Gets an empty, i.e. uninitialized, quantity.</summary>
    public static DimensionfulQuantity Empty
    {
      get
      {
        return new DimensionfulQuantity() { _value = double.NaN };
      }
    }

    /// <summary>Gets the unit of this quantity.</summary>
    public IUnit Unit
    {
      get
      {
        return _unit ?? throw new InvalidOperationException($"{nameof(DimensionfulQuantity)} is uninitialized!");
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

    /// <summary>
    /// Gets the prefixed unit of this quantity.
    /// </summary>
    /// <value>
    /// The prefixed unit of this quantity.
    /// </value>
    public PrefixedUnit PrefixedUnit => new PrefixedUnit(Prefix, Unit);

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
        if (double.IsNaN(_value))
          return _value;

        if (_unit is null)
          throw new InvalidOperationException("This instance is empty");

        double result = _value;
        if (_prefix is not null)
          result = _prefix.ToSIUnit(result);
        if (_unit is not null)
          result = _unit.ToSIUnit(result);
        return result;
      }
    }

    /// <summary>Converts this quantity to its numerical value in the given unit (without prefix).</summary>
    /// <param name="unit">The unit in which to get the numerical value of this quantity.</param>
    /// <returns>Numerical value of this quantity in the provided unit (without prefix).</returns>
    public double AsValueIn(IUnit unit)
    {
      if (double.IsNaN(_value))
        return _value;
      if (unit is null)
        throw new ArgumentNullException(nameof(unit));
      if (_unit is null)
        throw new InvalidOperationException("This instance is empty, i.e. the unit of this quantity is set to null.");
      if (unit.SIUnit != _unit.SIUnit)
        throw new ArgumentException(string.Format("Provided unit ({0}) is incompatible with this unit ({1})", unit.SIUnit, _unit));

      return unit.FromSIUnit(AsValueInSIUnits);
    }

    /// <summary>Converts this quantity to its numerical value in the given unit, with the given prefix.</summary>
    /// <param name="prefix">The prefix of the unit in which to get the numerical value of this quantity.</param>
    /// <param name="unit">The unit in which to get the numerical value of this quantity.</param>
    /// <returns>Numerical value of this quantity in the provided unit with the provided prefix.</returns>
    public double AsValueIn(SIPrefix prefix, IUnit unit)
    {
      if (double.IsNaN(_value))
        return _value;
      if (unit is null)
        throw new ArgumentNullException(nameof(unit));
      if (prefix is null)
        throw new ArgumentNullException(nameof(prefix));
      if (_unit is null)
        throw new InvalidOperationException("This instance is empty");
      if (unit.SIUnit != _unit.SIUnit)
        throw new ArgumentException($"Provided unit ({unit.SIUnit}) is incompatible with this unit ({_unit})");

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
        return new DimensionfulQuantity(AsValueInSIUnits, _unit is { } unit ? unit.SIUnit : throw new InvalidOperationException("Unit is empty"));
      }
    }

    /// <summary>Converts this quantity to another quantity in the provided unit (without prefix).</summary>
    /// <param name="unit">The unit to convert the quantity to.</param>
    /// <returns>New instance of a quantity in the provided unit (without prefix).</returns>
    public DimensionfulQuantity AsQuantityIn(IUnit unit)
    {
      return new DimensionfulQuantity(AsValueIn(unit), SIPrefix.None, unit);
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
      if (_unit is null || other._unit is null || _unit.SIUnit != other._unit.SIUnit)
        throw new ArgumentException($"Incompatible units in comparison of a quantity in {_unit?.Name} with a quantity in {other._unit?.Name}");

      double thisval = AsValueIn(_unit.SIUnit);
      double otherval = other.AsValueIn(_unit.SIUnit);
      return thisval.CompareTo(otherval);
    }

    public int CompareTo(object? value)
    {
      if (value is null)
        return 1;
      if (value is DimensionfulQuantity other)
        return CompareTo(other);
      else
        throw new ArgumentException($"Parameter must be of type {typeof(DimensionfulQuantity)}, but acutally is {value?.GetType()}");
    }

    public readonly bool Equals(DimensionfulQuantity other)
    {
      return this._value.Equals(other._value) &&
             EqualityComparer<SIPrefix>.Default.Equals(_prefix, other._prefix) &&
             EqualityComparer<IUnit>.Default.Equals(_unit, other._unit);

    }

    public override readonly bool Equals(object? obj)
    {
      return obj is DimensionfulQuantity other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      return _value.GetHashCode() + 17 * (_prefix ?? SIPrefix.None).GetHashCode() + (_unit is null ? 0 : 31 * _unit.GetHashCode());
    }

    public static bool operator ==(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (!(a._unit == b._unit))
        return false;
      if (!(a._prefix == b._prefix))
        return false;
      if (!(a._value == b._value))
        return false;

      return true;
    }

    public static bool operator !=(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      return !(a == b);
    }

    public static bool operator <(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      return !double.IsNaN(a._value) && a.CompareTo(b) < 0;
    }
    public static bool operator >(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      return !double.IsNaN(b._value) && a.CompareTo(b) > 0;
    }

    public static bool operator <=(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      return !double.IsNaN(a._value) && a.CompareTo(b) <= 0;
    }

    public static bool operator >=(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      return !double.IsNaN(b._value) && a.CompareTo(b) >= 0;
    }


    /// <summary>
    /// Gets the quantity as treated as unbiased difference value. If the unit of this quantity is not biased, the
    /// return value is exactly this quantity. But if the unit is biased, the return value is the difference of this quantity and the same quantity with zero value.
    /// Example: 20 °C will transform to 20 K, because 20°C - 0 °C = 20 K.
    /// </summary>
    /// <value>
    /// As treated as unbiased difference.
    /// </value>
    public DimensionfulQuantity TreatedAsUnbiasedDifference
    {
      get
      {
        if (HasBiasedUnit)
          return new DimensionfulQuantity(((IBiasedUnit)Unit).ToSIUnitIfTreatedAsDifference(Value), SIPrefix.None, Unit.SIUnit);
        else
          return this;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has a biased unit (i.e. a unit implementing <see cref="IBiasedUnit"/>.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has biased unit; otherwise, <c>false</c>.
    /// </value>
    public bool HasBiasedUnit
    {
      get
      {
        return _unit is IBiasedUnit;
      }
    }

    public static DimensionfulQuantity operator +(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (a.IsEmpty || b.IsEmpty)
        return DimensionfulQuantity.Empty;

      if (a.Unit.SIUnit != b.Unit.SIUnit)
        throw new ArithmeticException("Try to add two incompatible units");

      int bias = (a.HasBiasedUnit ? 1 : 0) + (b.HasBiasedUnit ? 2 : 0);

      switch (bias)
      {
        case 0:
          return new DimensionfulQuantity(a.Value + b.AsValueIn(a.Prefix, a.Unit), a.Prefix, a.Unit);

        case 1: // a is biased
          return new DimensionfulQuantity(((IBiasedUnit)a.Unit).AddBiasedValueOfThisUnitAndSIValue(a.Value, b.AsValueInSIUnits), a.Prefix, a.Unit);

        case 2: // b is biased
          return new DimensionfulQuantity(((IBiasedUnit)b.Unit).AddBiasedValueOfThisUnitAndSIValue(b.Value, a.AsValueInSIUnits), b.Prefix, b.Unit);

        default:
          throw new ArithmeticException("Can not add two biased units");
      }
    }

    /// <summary>
    /// Implements the subtraction operator. Here, the intended operation is ambiguous if we subtract a biased unit and an unbiased unit. The result
    /// can be treated either as biased or unbiased. For example:
    /// 20°C minus 20 Kelvin. The result is either 293.15 K - 20 K = 273.15 K or 20°C - 20 K = 0 °C (biased). Although the same, this
    /// results in difficulties if subsequently rates are calculated from this result.
    /// Because of that, the decision was made to treat the result as unbiased value.
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <exception cref="ArithmeticException">
    /// Try to subtract two incompatible units
    /// or
    /// Can not execute subtraction of unbiased unit minus biased unit
    /// </exception>
    /// <exception cref="NotImplementedException"></exception>
    public static DimensionfulQuantity operator -(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (a.IsEmpty || b.IsEmpty)
        return DimensionfulQuantity.Empty;

      if (a.Unit.SIUnit != b.Unit.SIUnit)
        throw new ArithmeticException("Try to subtract two incompatible units");

      int bias = (a.HasBiasedUnit ? 1 : 0) + (b.HasBiasedUnit ? 2 : 0);

      switch (bias)
      {
        case 0:
          return new DimensionfulQuantity(a.Value - b.AsValueIn(a.Prefix, a.Unit), a.Prefix, a.Unit);

        case 1: // a is biased, b is not biased. We treat the result as unbiased, see comment of this function above
          return new DimensionfulQuantity(a.AsValueInSIUnits - b.AsValueInSIUnits, SIPrefix.None, a.Unit.SIUnit);

        case 2: // b is biased
          throw new ArithmeticException("Can not execute subtraction of unbiased unit minus biased unit");
        case 3: // both units are biased, thus the result is the unbiased SI unit
          return new DimensionfulQuantity(a.AsValueInSIUnits - b.AsValueInSIUnits, SIPrefix.None, a.Unit.SIUnit);

        default:
          throw new NotImplementedException();
      }
    }

    public static DimensionfulQuantity operator *(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (a.IsEmpty || b.IsEmpty)
        return DimensionfulQuantity.Empty;

      return new DimensionfulQuantity(
          a.AsValueInSIUnits * b.AsValueInSIUnits,
          SIPrefix.None,
          a.Unit.SIUnit * b.Unit.SIUnit);
    }

    public static DimensionfulQuantity operator /(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (a.IsEmpty || b.IsEmpty)
        return DimensionfulQuantity.Empty;

      a = a.TreatedAsUnbiasedDifference;
      b = b.TreatedAsUnbiasedDifference;

      return new DimensionfulQuantity(
          a.AsValueInSIUnits / b.AsValueInSIUnits,
          SIPrefix.None,
          a.Unit.SIUnit / b.Unit.SIUnit);
    }

    public static DimensionfulQuantity operator *(DimensionfulQuantity a, double b)
    {
      if (a.IsEmpty)
        return DimensionfulQuantity.Empty;

      a = a.TreatedAsUnbiasedDifference;

      return new DimensionfulQuantity(
          a.Value * b,
         a.Prefix,
          a.Unit);
    }

    public static DimensionfulQuantity operator /(DimensionfulQuantity a, double b)
    {
      if (a.IsEmpty)
        return DimensionfulQuantity.Empty;

      a = a.TreatedAsUnbiasedDifference;

      return new DimensionfulQuantity(
          a.Value / b,
         a.Prefix,
          a.Unit);
    }

    public static DimensionfulQuantity operator *(double a, DimensionfulQuantity b)
    {
      if (b.IsEmpty)
        return DimensionfulQuantity.Empty;


      b = b.TreatedAsUnbiasedDifference;

      return new DimensionfulQuantity(
          b.Value * a,
          b.Prefix,
          b.Unit);
    }

    public static DimensionfulQuantity operator /(double a, DimensionfulQuantity b)
    {
      if (b.IsEmpty)
        return DimensionfulQuantity.Empty;

      b = b.TreatedAsUnbiasedDifference;

      (var newPrefix, var remainingFactor) = SIPrefix.FromDivision(SIPrefix.None, b.Prefix);

      return new DimensionfulQuantity(
          remainingFactor * a / b.Value,
          newPrefix,
          Dimensionless.Unity.Instance / b.Unit.SIUnit);
    }

    public static DimensionfulQuantity operator -(DimensionfulQuantity a)
    {
      if (a.IsEmpty)
        return DimensionfulQuantity.Empty;

      if (a.Unit is IBiasedUnit)
        throw new ArithmeticException("Can not invert a biased unit");

      return new DimensionfulQuantity(-a.Value, a.Prefix, a.Unit);
    }

    public static DimensionfulQuantity Abs(DimensionfulQuantity a)
    {
      if (a.IsEmpty)
        return DimensionfulQuantity.Empty;

      if (a.Unit is IBiasedUnit && a.Value < 0)
        throw new ArithmeticException("Can not invert a biased unit");

      return new DimensionfulQuantity(Math.Abs(a.Value), a.Prefix, a.Unit);
    }

    private static double RoundUpOrDown(double x, double y)
    {
      if (y == 0)
        return x;

      double s = x / y;
      s = Math.Round(s, MidpointRounding.AwayFromZero);
      return s * y;
    }

    public static DimensionfulQuantity RoundUpOrDown(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (a.IsEmpty || b.IsEmpty)
        return DimensionfulQuantity.Empty;

      if (b.Unit is IBiasedUnit)
        throw new ArithmeticException("Can not round if denominator unit is biased");

      var c = RoundUpOrDown(a.Value, b.AsValueIn(a.Prefix, a.Unit));
      return new DimensionfulQuantity(c, a.Prefix, a.Unit);
    }

    public static DimensionfulQuantity Max(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (a.IsEmpty || b.IsEmpty)
        return DimensionfulQuantity.Empty;

      var c = b.AsValueIn(a.Prefix, a.Unit);
      return new DimensionfulQuantity(Math.Max(a.Value, c), a.Prefix, a.Unit);
    }

    public static DimensionfulQuantity Min(DimensionfulQuantity a, DimensionfulQuantity b)
    {
      if (a.IsEmpty || b.IsEmpty)
        return DimensionfulQuantity.Empty;

      var c = b.AsValueIn(a.Prefix, a.Unit);
      return new DimensionfulQuantity(Math.Min(a.Value, c), a.Prefix, a.Unit);
    }

    public override string ToString()
    {
      if (IsEmpty)
        return double.NaN.ToString();

      return string.Format("{0} {1}{2}", _value, Prefix.ShortCut, Unit.ShortCut);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
      return IsEmpty
        ? double.NaN.ToString(formatProvider)
        : string.Format(formatProvider, "{0} {1}{2}", _value.ToString(format, formatProvider), Prefix.ShortCut, Unit.ShortCut);
    }


  }
}
