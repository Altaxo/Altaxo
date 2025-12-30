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

namespace Altaxo.Science
{
  /// <summary>
  /// Defines a transformation from a double value to another double value.
  /// Implementations should also provide a human-readable string representation and a back-transformation.
  /// </summary>
  public interface IDoubleToDoubleValueTransformation
  {
    /// <summary>
    /// Transforms the supplied value.
    /// </summary>
    /// <param name="value">The input value to transform.</param>
    /// <returns>The transformed value.</returns>
    double Transform(double value);

    /// <summary>
    /// Gets a string that describes the transformation.
    /// </summary>
    string StringRepresentation { get; }

    /// <summary>
    /// Gets the transformation that reverses this transformation.
    /// </summary>
    IDoubleToDoubleValueTransformation BackTransformation { get; }
  }

  /// <summary>
  /// Transformation that returns the multiplicative inverse (1/x).
  /// </summary>
  public class InverseTransformation : IDoubleToDoubleValueTransformation
  {
    /// <inheritdoc/>
    public double Transform(double value)
    {
      return 1 / value;
    }

    /// <inheritdoc/>
    public string StringRepresentation
    {
      get { return "1/x"; }
    }

    /// <inheritdoc/>
    public IDoubleToDoubleValueTransformation BackTransformation
    {
      get { return this; }
    }
  }

  /// <summary>
  /// Transformation that negates the value (-x).
  /// </summary>
  public class NegateTransformation : IDoubleToDoubleValueTransformation
  {
    /// <inheritdoc/>
    public double Transform(double value)
    {
      return -value;
    }

    /// <inheritdoc/>
    public string StringRepresentation
    {
      get { return "-x"; }
    }

    /// <inheritdoc/>
    public IDoubleToDoubleValueTransformation BackTransformation
    {
      get { return this; }
    }
  }

  /// <summary>
  /// Transformation that adds a fixed offset to the input value (x + offset).
  /// </summary>
  public class OffsetTransformation : IDoubleToDoubleValueTransformation
  {
    private double _offsetValue;

    /// <inheritdoc/>
    public double Transform(double value)
    {
      return value + _offsetValue;
    }

    /// <inheritdoc/>
    public string StringRepresentation
    {
      get { return string.Format("(x+{0})", _offsetValue); }
    }

    /// <inheritdoc/>
    public IDoubleToDoubleValueTransformation BackTransformation
    {
      get { return new OffsetTransformation() { _offsetValue = -_offsetValue }; }
    }
  }

  /// <summary>
  /// Transformation that scales the input value by a factor (scale * x).
  /// </summary>
  public class ScaleTransformation : IDoubleToDoubleValueTransformation
  {
    private double _scaleValue;

    /// <inheritdoc/>
    public double Transform(double value)
    {
      return value * _scaleValue;
    }

    /// <inheritdoc/>
    public string StringRepresentation
    {
      get { return string.Format("({0}*x)", _scaleValue); }
    }

    /// <inheritdoc/>
    public IDoubleToDoubleValueTransformation BackTransformation
    {
      get { return new ScaleTransformation() { _scaleValue = 1 / _scaleValue }; }
    }
  }

  /// <summary>
  /// Transformation that applies the natural logarithm (ln(x)).
  /// </summary>
  public class NaturalLogarithmTransformation : IDoubleToDoubleValueTransformation
  {
    /// <inheritdoc/>
    public double Transform(double value)
    {
      return Math.Log(value);
    }

    /// <inheritdoc/>
    public string StringRepresentation
    {
      get { return "ln(x)"; }
    }

    /// <inheritdoc/>
    public IDoubleToDoubleValueTransformation BackTransformation
    {
      get { return new NaturalExponentialTransform(); }
    }
  }

  /// <summary>
  /// Transformation that applies the natural exponential function (exp(x)).
  /// </summary>
  public class NaturalExponentialTransform : IDoubleToDoubleValueTransformation
  {
    /// <inheritdoc/>
    public double Transform(double value)
    {
      return Math.Exp(value);
    }

    /// <inheritdoc/>
    public string StringRepresentation
    {
      get { return "exp(x)"; }
    }

    /// <inheritdoc/>
    public IDoubleToDoubleValueTransformation BackTransformation
    {
      get { return new NaturalLogarithmTransformation(); }
    }
  }

  /// <summary>
  /// Transformation that applies a sequence of transformations in order.
  /// </summary>
  public class CombinedTransform : IDoubleToDoubleValueTransformation
  {
    private List<IDoubleToDoubleValueTransformation> _transformations = new List<IDoubleToDoubleValueTransformation>();

    /// <inheritdoc/>
    public double Transform(double value)
    {
      foreach (var item in _transformations)
        value = item.Transform(value);
      return value;
    }

    /// <inheritdoc/>
    public string StringRepresentation
    {
      get { return "CombinedTransform"; }
    }

    /// <inheritdoc/>
    public IDoubleToDoubleValueTransformation BackTransformation
    {
      get
      {
        var t = new CombinedTransform();
        for (int i = _transformations.Count - 1; i >= 0; i--)
          t._transformations.Add(_transformations[i].BackTransformation);
        return t;
      }
    }
  }

  /// <summary>
  /// Specifies the various representations a value can have when transformed.
  /// These representations are used by the helpers in the `TransformedValue` struct
  /// to convert between a base value and its transformed form.
  /// </summary>
  public enum TransformedValueRepresentation
  {
    /// <summary>Value is used directly (no transformation).</summary>
    Original = 0,

    /// <summary>Value is used in form of its inverse.</summary>
    Inverse,

    /// <summary>Value is used in form of its negative.</summary>
    Negative,

    /// <summary>Value is used in the form of its decadic logarithm.</summary>
    DecadicLogarithm,

    /// <summary>Value is used in the form of its negative decadic logarithm.</summary>
    NegativeDecadicLogarithm,

    /// <summary>Value is used in the form of its natural logarithm.</summary>
    NaturalLogarithm,

    /// <summary>Value is used in the form of its negative natural logarithm.</summary>
    NegativeNaturalLogarithm
  }

  /// <summary>
  /// Static helpers for converting between transformed and base values.
  /// </summary>
  public struct TransformedValue
  {
    #region Transformations (static)

    /// <summary>
    /// Converts a transformed value back to the original base value.
    /// </summary>
    /// <param name="srcValue">The transformed value.</param>
    /// <param name="srcUnit">The representation of the transformed value.</param>
    /// <returns>The base (original) value.</returns>
    public static double TransformedValueToBaseValue(double srcValue, TransformedValueRepresentation srcUnit)
    {
      switch (srcUnit)
      {
        case TransformedValueRepresentation.Original:
          return srcValue;

        case TransformedValueRepresentation.Inverse:
          return 1 / srcValue;

        case TransformedValueRepresentation.Negative:
          return -srcValue;

        case TransformedValueRepresentation.DecadicLogarithm:
          return Math.Pow(10, srcValue);

        case TransformedValueRepresentation.NegativeDecadicLogarithm:
          return Math.Pow(10, -srcValue);

        case TransformedValueRepresentation.NaturalLogarithm:
          return Math.Exp(srcValue);

        case TransformedValueRepresentation.NegativeNaturalLogarithm:
          return Math.Exp(-srcValue);

        default:
          throw new ArgumentOutOfRangeException("ValueTransformationType unknown: " + srcUnit.ToString());
      }
    }

    /// <summary>
    /// Converts a base value to the requested transformed representation.
    /// </summary>
    /// <param name="baseValue">The original base value.</param>
    /// <param name="destTransform">The desired transformed representation.</param>
    /// <returns>The value expressed in the transformed representation.</returns>
    public static double BaseValueToTransformedValue(double baseValue, TransformedValueRepresentation destTransform)
    {
      switch (destTransform)
      {
        case TransformedValueRepresentation.Original:
          return baseValue;

        case TransformedValueRepresentation.Inverse:
          return 1 / baseValue;

        case TransformedValueRepresentation.Negative:
          return -baseValue;

        case TransformedValueRepresentation.DecadicLogarithm:
          return Math.Log10(baseValue);

        case TransformedValueRepresentation.NegativeDecadicLogarithm:
          return -Math.Log10(baseValue);

        case TransformedValueRepresentation.NaturalLogarithm:
          return Math.Log(baseValue);

        case TransformedValueRepresentation.NegativeNaturalLogarithm:
          return -Math.Log(baseValue);

        default:
          throw new ArgumentOutOfRangeException("ValueTransformationType unknown: " + destTransform.ToString());
      }
    }

    /// <summary>
    /// Returns a formula string for the specified transformation applied to the given variable name.
    /// </summary>
    /// <param name="nameOfVariable">The variable name to use in the formula.</param>
    /// <param name="transform">The transformation representation.</param>
    /// <returns>A string representing the formula for the transformation.</returns>
    public static string GetFormula(string nameOfVariable, TransformedValueRepresentation transform)
    {
      switch (transform)
      {
        case TransformedValueRepresentation.Original:
          return nameOfVariable;

        case TransformedValueRepresentation.Inverse:
          return "1/" + nameOfVariable;

        case TransformedValueRepresentation.Negative:
          return "-" + nameOfVariable;

        case TransformedValueRepresentation.DecadicLogarithm:
          return "lg(" + nameOfVariable + ")";

        case TransformedValueRepresentation.NegativeDecadicLogarithm:
          return "-lg(" + nameOfVariable + ")";

        case TransformedValueRepresentation.NaturalLogarithm:
          return "ln(" + nameOfVariable + ")";

        case TransformedValueRepresentation.NegativeNaturalLogarithm:
          return "-ln(" + nameOfVariable + ")";

        default:
          throw new ArgumentOutOfRangeException("ValueTransformationType unknown: " + transform.ToString());
      }
    }

    /// <summary>
    /// Converts a value from one transformed representation to another.
    /// </summary>
    /// <param name="srcValue">The source value in the source representation.</param>
    /// <param name="srcUnit">The source representation.</param>
    /// <param name="destUnit">The destination representation.</param>
    /// <returns>The value expressed in the destination representation.</returns>
    public static double FromTo(double srcValue, TransformedValueRepresentation srcUnit, TransformedValueRepresentation destUnit)
    {
      if (srcUnit == destUnit)
        return srcValue;
      else
        return BaseValueToTransformedValue(TransformedValueToBaseValue(srcValue, srcUnit), destUnit);
    }

    #endregion Transformations (static)
  }
}
