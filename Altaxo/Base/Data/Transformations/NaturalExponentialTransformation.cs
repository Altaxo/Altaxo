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

namespace Altaxo.Data.Transformations
{
  /// <summary>
  /// Applies the natural exponential function <c>exp(x)</c>.
  /// </summary>
  public class NaturalExponentialTransformation : ImmutableClassWithoutMembersBase, IDoubleToDoubleTransformation
  {
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static NaturalExponentialTransformation Instance { get; private set; } = new NaturalExponentialTransformation();

    #region Serialization

    /// <summary>
    /// 2016-06-24 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NaturalExponentialTransformation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return NaturalExponentialTransformation.Instance;
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public Type InputValueType { get { return typeof(double); } }

    /// <inheritdoc/>
    public Type OutputValueType { get { return typeof(double); } }

    /// <inheritdoc/>
    public AltaxoVariant Transform(AltaxoVariant value)
    {
      return Math.Exp(value);
    }

    /// <inheritdoc/>
    public double Transform(double y)
    {
      return Math.Exp(y);
    }

    /// <inheritdoc/>
    public (double ytrans, double dydxtrans) Derivative(double y, double dydx)
    {
      var yt = Math.Exp(y);
      return (yt, dydx * yt);
    }

    /// <inheritdoc/>
    public string RepresentationAsFunction
    {
      get { return GetRepresentationAsFunction("x"); }
    }

    /// <inheritdoc/>
    public string GetRepresentationAsFunction(string arg)
    {
      return string.Format("Exp({0})", arg);
    }

    /// <inheritdoc/>
    public string RepresentationAsOperator
    {
      get { return "Exp"; }
    }

    /// <inheritdoc/>
    public IVariantToVariantTransformation BackTransformation
    {
      get { return NaturalLogarithmTransformation.Instance; }
    }
  }
}
