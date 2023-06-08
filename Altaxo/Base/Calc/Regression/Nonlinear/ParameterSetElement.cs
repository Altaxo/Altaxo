#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds the properties of one fit parameter, as <see cref="Name"/>, the value of the parameter <see cref="Parameter"/>, the <see cref="Variance"/>
  /// and whether this parameter is fixed or can be varied.
  /// </summary>
  /// <seealso cref="System.ICloneable" />
  public class ParameterSetElement : ICloneable
  {
    /// <summary>
    /// Gets or sets the parameter's name.
    /// </summary>
    /// <value>
    /// The parameter's name.
    /// </value>
    public string Name { get => _name; [MemberNotNull(nameof(_name))] set => _name = value; }
    private string _name;

    /// <summary>
    /// Gets or sets the parameter's value.
    /// </summary>
    /// <value>
    /// The parameter's value.
    /// </value>
    public double Parameter { get; set; }

    /// <summary>
    /// Gets or sets the variance of the parameter.
    /// </summary>
    /// <value>
    /// The variance of the parameter.
    /// </value>
    public double Variance { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ParameterSetElement"/> can vary during the fitting calculation.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the parameter can vary; otherwise, <c>false</c>.
    /// </value>
    public bool Vary { get; set; }

    /// <summary>
    /// Gets or sets the lower bound for the parameter.
    /// </summary>
    public double? LowerBound { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the lower bound value is exclusive.
    /// </summary>
    public bool IsLowerBoundExclusive { get; set; }

    /// <summary>
    /// Gets or sets the lower bound for the parameter.
    /// </summary>
    public double? UpperBound { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the lower bound value is exclusive.
    /// </summary>
    public bool IsUpperBoundExclusive { get; set; }

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
    /// 2023-06-08 Extend by LowerBound, IsLowerBoundExclusive, UpperBound, IsUpperBoundExclusive
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSetElement), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    protected ParameterSetElement(Altaxo.Serialization.Xml.IXmlDeserializationInfo _)
    {
      Name = string.Empty;
    }

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

    public ParameterSetElement(ParameterSetElement from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_name))]
    public void CopyFrom(ParameterSetElement from)
    {
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
      if (ReferenceEquals(this, from))
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.


      Name = from.Name;
      Parameter = from.Parameter;
      Variance = from.Variance;
      Vary = from.Vary;
      LowerBound = from.LowerBound;
      UpperBound = from.UpperBound;
      IsLowerBoundExclusive = from.IsLowerBoundExclusive;
      IsUpperBoundExclusive = from.IsUpperBoundExclusive;
    }

    #region ICloneable Members

    public object Clone()
    {
      return new ParameterSetElement(this);
    }

    #endregion ICloneable Members
  }
}
