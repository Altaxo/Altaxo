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
  /// Encapsulates an existing unit, but limits the set of possible prefixes.
  /// </summary>
  public class UnitWithLimitedPrefixes : IUnit
  {
    private SIPrefixList _prefixes;
    private IUnit _unit;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="UnitWithLimitedPrefixes"/> (version 0).
    /// Handles custom serialization and deserialization of the outer type.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UnitWithLimitedPrefixes), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <summary>
      /// Serializes the specified <see cref="UnitWithLimitedPrefixes"/> instance into the provided
      /// <see cref="Altaxo.Serialization.Xml.IXmlSerializationInfo"/>.
      /// </summary>
      /// <param name="obj">The object to serialize (expected to be a <see cref="UnitWithLimitedPrefixes"/>).</param>
      /// <param name="info">The serialization info where values should be written.</param>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (UnitWithLimitedPrefixes)obj;

        info.AddValue("Unit", s.Unit);

        info.CreateArray("PrefixList", s.Prefixes.Count);
        foreach (var prefix in s.Prefixes)
          info.AddValue("e", prefix);
        info.CommitArray();
      }

      /// <summary>
      /// Deserializes an instance of <see cref="UnitWithLimitedPrefixes"/> from the provided
      /// <see cref="Altaxo.Serialization.Xml.IXmlDeserializationInfo"/> and returns the reconstructed object.
      /// </summary>
      /// <param name="o">An optional existing object instance (ignored).</param>
      /// <param name="info">The deserialization info to read values from.</param>
      /// <param name="parent">The parent object in the object graph (may be <c>null</c>).</param>
      /// <returns>A new <see cref="UnitWithLimitedPrefixes"/> instance created from the serialized data.</returns>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var unit = (IUnit)info.GetValue("Unit", parent);

        int count = info.OpenArray("PrefixList");

        var list = new SIPrefix[count];
        for (int i = 0; i < count; ++i)
          list[i] = (SIPrefix)info.GetValue("e", parent);
        info.CloseArray(count);

        return new UnitWithLimitedPrefixes(unit, list);
      }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitWithLimitedPrefixes"/> class that wraps an existing unit
    /// and restricts its allowed SI prefixes to the provided set.
    /// </summary>
    /// <param name="unit">The underlying unit to wrap (must not be <c>null</c>).</param>
    /// <param name="allowedPrefixes">The set of allowed prefixes for the resulting unit.</param>
    public UnitWithLimitedPrefixes(IUnit unit, IEnumerable<SIPrefix> allowedPrefixes)
    {
      if (allowedPrefixes is null)
        throw new ArgumentNullException(nameof(allowedPrefixes));
      _unit = unit ?? throw new ArgumentNullException(nameof(unit));

      var l = new HashSet<SIPrefix>(_unit.Prefixes);
      l.IntersectWith(allowedPrefixes);
      _prefixes = new SIPrefixList(l);
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return _unit.Name; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return _unit.ShortCut; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return _unit.ToSIUnit(x);
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return _unit.FromSIUnit(x);
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return _prefixes; }
    }

    /// <summary>
    /// Gets the underlying unit that is wrapped by this instance.
    /// </summary>
    /// <value>The underlying <see cref="IUnit"/>.</value>
    public IUnit Unit
    {
      get { return _unit; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return _unit.SIUnit; }
    }
  }
}
