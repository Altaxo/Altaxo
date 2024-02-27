﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds the properties of one fit parameter, as <see cref="Name"/>, the value of the parameter <see cref="Parameter"/>, the <see cref="Variance"/>
  /// and whether this parameter is fixed or can be varied.
  /// </summary>
  /// <seealso cref="System.ICloneable" />
  public record ParameterSetElement : Main.IImmutable
  {
    private string _name;

    /// <summary>
    /// Gets or sets the parameter's name.
    /// </summary>
    /// <value>
    /// The parameter's name.
    /// </value>
    public string Name
    {
      get => _name;

      [MemberNotNull(nameof(_name))]
      init
      {
        _name = value ?? throw new ArgumentNullException(nameof(Name));
      }
    }

    /// <summary>
    /// Gets or sets the parameter's value.
    /// </summary>
    /// <value>
    /// The parameter's value.
    /// </value>
    public double Parameter { get; init; }

    /// <summary>
    /// Gets or sets the variance of the parameter.
    /// </summary>
    /// <value>
    /// The variance of the parameter.
    /// </value>
    public double Variance { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ParameterSetElement"/> can vary during the fitting calculation.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the parameter can vary; otherwise, <c>false</c>.
    /// </value>
    public bool Vary { get; init; }

    /// <summary>
    /// Gets or sets the lower bound for the parameter.
    /// </summary>
    public double? LowerBound { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the lower bound value is exclusive.
    /// </summary>
    public bool IsLowerBoundExclusive { get; init; }

    /// <summary>
    /// Gets or sets the lower bound for the parameter.
    /// </summary>
    public double? UpperBound { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the lower bound value is exclusive.
    /// </summary>
    public bool IsUpperBoundExclusive { get; init; }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.ParameterSetElement", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException();
        /*
        var s = (ParameterSetElement)obj;

        info.AddValue("Name", s.Name);
        info.AddValue("Value", s.Parameter);
        info.AddValue("Variance", s.Variance);
        info.AddValue("Vary", s.Vary);
        */
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var name = info.GetString("Name");
        var parameter = info.GetDouble("Value");
        var variance = info.GetDouble("Variance");
        var vary = info.GetBoolean("Vary");
        return new ParameterSetElement(name, parameter, variance, vary);
      }
    }

    /// <summary>
    /// 2023-06-08 V1: Extended by LowerBound, IsLowerBoundExclusive, UpperBound, IsUpperBoundExclusive
    /// 2024-02-27 V2: Moved to AltaxoCore, now immutable
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.ParameterSetElement", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSetElement), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ParameterSetElement)obj;

        info.AddValue("Name", s.Name);
        info.AddValue("Value", s.Parameter);
        info.AddValue("Variance", s.Variance);
        info.AddValue("Vary", s.Vary);
        info.AddValue("LowerBound", s.LowerBound);
        info.AddValue("IsLowerBoundExclusive", s.IsLowerBoundExclusive);
        info.AddValue("UpperBound", s.UpperBound);
        info.AddValue("IsUpperBoundExclusive", s.IsUpperBoundExclusive);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var name = info.GetString("Name");
        var parameter = info.GetDouble("Value");
        var variance = info.GetDouble("Variance");
        var vary = info.GetBoolean("Vary");
        var lowerBound = info.GetNullableDouble("LowerBound");
        var isLowerBoundExclusive = info.GetBoolean("IsLowerBoundExclusive");
        var upperBound = info.GetNullableDouble("UpperBound");
        var isUpperBoundExclusive = info.GetBoolean("IsUpperBoundExclusive");
        return new ParameterSetElement(name, parameter, variance, vary, lowerBound, isLowerBoundExclusive, upperBound, isUpperBoundExclusive);
      }
    }

    #endregion Serialization

    public ParameterSetElement(string name)
    {
      Name = name;
      Vary = true;
    }

    public ParameterSetElement(string name, double value)
    {
      Name = name;
      Parameter = value;
      Vary = true;
    }

    public ParameterSetElement(string name, double value, double variance, bool vary)
    {
      Name = name;
      Parameter = value;
      Variance = variance;
      Vary = vary;
    }

    public ParameterSetElement(string name, double value, double variance, bool vary, double? lowerBound, bool isLowerBoundExclusive, double? upperBound, bool isUpperBoundExclusive)
    {
      Name = name;
      Parameter = value;
      Variance = variance;
      Vary = vary;
      LowerBound = lowerBound;
      UpperBound = upperBound;
      IsLowerBoundExclusive = isLowerBoundExclusive;
      IsUpperBoundExclusive = isUpperBoundExclusive;
    }

    /// <summary>
    /// Gets the effective (inclusive) lower bound. The <see cref="LowerBound"/> value is slightly increased,
    /// if <see cref="IsLowerBoundExclusive"/> is true in order to get the inclusive lower bound.
    /// </summary>
    public double? LowerBoundInclusive
    {
      get
      {
        double? lb;

        lb = LowerBound;
        if (lb.HasValue && IsLowerBoundExclusive)
        {
          if (lb.Value == 0)
            lb = double.Epsilon;
          else
            lb = lb.Value + Math.Abs(lb.Value) * DoubleConstants.DBL_EPSILON;
        }

        return lb;
      }
    }

    /// <summary>
    /// Gets the effective (inclusive) upper bound. The <see cref="UpperBound"/> value is slightly decreased,
    /// if <see cref="IsUpperBoundExclusive"/> is true in order to get the inclusive upper bound.
    /// </summary>
    public double? UpperBoundInclusive
    {
      get
      {
        double? ub;

        ub = UpperBound;
        if (ub.HasValue && IsUpperBoundExclusive)
        {
          if (ub.Value == 0)
            ub = -double.Epsilon;
          else
            ub = ub.Value - Math.Abs(ub.Value) * DoubleConstants.DBL_EPSILON;
        }

        return ub;
      }
    }

    /// <summary>
    /// Gets the arrays of parameter values, isFixed, lowerBounds and upperBounds for nonlinear fitting.
    /// The arrays have the same length as the provided list of elements has.
    /// </summary>
    /// <param name="elements">The parameter set elements.</param>
    /// <returns>Tuple of the arrays parameter values, isFixed, lowerBounds (inclusive) and upperBounds (inclusive) for nonlinear fitting.
    /// If lowerBounds or upperBounds not contain any value, the corresponding returned array is null.
    /// </returns>
    public static (double[] parameterValues, bool[] isFixed, double?[]? lowerBounds, double?[]? upperBounds) GetFitArrays(IReadOnlyList<ParameterSetElement> elements)
    {
      var paraValues = new double[elements.Count];
      var isFixed = new bool[elements.Count];
      var lowerBounds = new double?[elements.Count];
      var upperBounds = new double?[elements.Count];

      bool hasAnyLowerBound = false;
      bool hasAnyUpperBound = false;

      for (int i = 0; i < elements.Count; ++i)
      {
        paraValues[i] = elements[i].Parameter;
        isFixed[i] = !elements[i].Vary;
        lowerBounds[i] = elements[i].LowerBoundInclusive;
        upperBounds[i] = elements[i].UpperBoundInclusive;
        hasAnyLowerBound |= elements[i].LowerBoundInclusive.HasValue;
        hasAnyUpperBound |= elements[i].UpperBoundInclusive.HasValue;
      }

      return (paraValues, isFixed, hasAnyLowerBound ? lowerBounds : null, hasAnyUpperBound ? upperBounds : null);
    }

    /// <summary>
    /// Collects a list of only the varying parameters (!) and their corresponding boundaries.
    /// The returned lists have the same length or less length than the provided elements.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A tuple of three lists, one with the parameters, one with the lower bounds (inclusive, can be null), and one with the upper bounds (inclusive, can be null).</returns>
    public static (List<double> varyingParameterValues, List<double?>? lowerBounds, List<double?>? upperBounds) CollectVaryingParametersAndBoundaries(IReadOnlyList<ParameterSetElement> parameters)
    {
      var varyingParameters = new List<double>();
      var lowerBounds = new List<double?>();
      var upperBounds = new List<double?>();

      bool hasAnyLowerBound = false;
      bool hasAnyUpperBound = false;

      for (int i = 0; i < parameters.Count; i++)
      {
        var p = parameters[i];
        if (p.Vary)
        {
          varyingParameters.Add(p.Parameter);
          lowerBounds.Add(p.LowerBoundInclusive);
          upperBounds.Add(p.UpperBoundInclusive);
          hasAnyLowerBound |= p.LowerBoundInclusive.HasValue;
          hasAnyUpperBound |= p.UpperBoundInclusive.HasValue;
        }
      }
      return (varyingParameters, hasAnyLowerBound ? lowerBounds : null, hasAnyUpperBound ? upperBounds : null);
    }

    /// <summary>
    /// Tests the value of the parameter and the lower and upper boundaries for inconsistencies, and corrects them.
    /// </summary>
    /// <param name="stb">The <see cref="StringBuilder"/> that is used to collect error messages.</param>
    /// <param name="isFatal">If the returned value is true, then the error is fatal, i.e. it can not be corrected. This is the case if
    /// both <see cref="LowerBound"/> and <see cref="UpperBound"/> are set, and <see cref="LowerBoundInclusive"/> is greater than <see cref="UpperBoundInclusive"/>.</param>
    /// <returns>The corrected instance of the <see cref="ParameterSetElement"/>.</returns>
    public ParameterSetElement TestAndCorrectParameterAndBoundaries(StringBuilder stb, ref bool isFatal)
    {
      var p = this;
      var lb = p.LowerBoundInclusive;
      var ub = p.UpperBoundInclusive;
      if (lb.HasValue && ub.HasValue)
      {
        if (!(lb.Value <= ub.Value))
        {
          stb.AppendLine($"Parameter {p.Name}: lower bound is greater than upper bound, CORRECTION REQUIRED!");
          isFatal = true;
        }
        else if (lb.Value == ub.Value && p.Vary)
        {
          p = p with
          {
            Parameter = lb.Value,
            Vary = false,
          };
          stb.AppendLine($"Parameter {p.Name}: was set to fixed since lower bound is equal to upper bound!");
        }
      }
      if (ub.HasValue && !(p.Parameter <= ub.Value))
      {
        p = p with { Parameter = ub.Value };
        stb.AppendLine($"Parameter {p.Name}: was set to upper bound since it was greater than upper bound!");
      }
      if (lb.HasValue && !(p.Parameter >= lb.Value))
      {
        p = p with { Parameter = lb.Value };
        stb.AppendLine($"Parameter {p.Name}: was set to lower bound since it was less than lower bound!");
      }
      return p;
    }
  }
}
