using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{

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


  public struct TransformedValue
  {
    #region Transformations (static)
    
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


    public static double FromTo(double srcValue, TransformedValueRepresentation srcUnit, TransformedValueRepresentation destUnit)
    {
      if (srcUnit == destUnit)
        return srcValue;
      else
        return BaseValueToTransformedValue(TransformedValueToBaseValue(srcValue, srcUnit), destUnit);
    }

    #endregion
  }
}
