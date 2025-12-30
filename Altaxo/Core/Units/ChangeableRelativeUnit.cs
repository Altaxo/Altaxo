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

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// This unit refers to a reference quantity. Since the reference quantity can be changed, instances of this class are <b>not</b> immutable.
  /// Example: 'percent of page width': here the page width can change depending on the user defined settings.
  /// </summary>
  public class ChangeableRelativeUnit : IRelativeUnit
  {
    private string _name;
    private string _shortCut;
    protected double _divider;
    private DimensionfulQuantity _referenceQuantity;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="ChangeableRelativeUnit"/> (version 0).
    /// Handles custom serialization and deserialization of the outer type.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ChangeableRelativeUnit), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <summary>
      /// Serializes the specified <see cref="ChangeableRelativeUnit"/> instance into the provided
      /// <see cref="Altaxo.Serialization.Xml.IXmlSerializationInfo"/>.
      /// </summary>
      /// <param name="obj">The object to serialize (expected to be a <see cref="ChangeableRelativeUnit"/>).</param>
      /// <param name="info">The serialization info where values should be written.</param>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ChangeableRelativeUnit)obj;

        info.AddValue("Name", s._name);
        info.AddValue("ShortCut", s._shortCut);
        info.AddValue("Divider", s._divider);
        info.AddValue("Reference", s._referenceQuantity);
      }

      /// <summary>
      /// Deserializes an instance of <see cref="ChangeableRelativeUnit"/> from the provided
      /// <see cref="Altaxo.Serialization.Xml.IXmlDeserializationInfo"/> and returns the reconstructed object.
      /// </summary>
      /// <param name="o">An optional existing object instance (ignored).</param>
      /// <param name="info">The deserialization info to read values from.</param>
      /// <param name="parent">The parent object in the object graph (may be <c>null</c>).</param>
      /// <returns>A new <see cref="ChangeableRelativeUnit"/> instance created from the serialized data.</returns>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var value = info.GetString("Name");
        var shortCut = info.GetString("ShortCut");
        var divider = info.GetDouble("Divider");
        var reference = info.GetValue<DimensionfulQuantity>("Reference", null);
        return new ChangeableRelativeUnit(value, shortCut, divider, reference);
      }
    }
    #endregion

    /// <summary>Initializes a new instance of the <see cref="ChangeableRelativeUnit"/> class.</summary>
    /// <param name="name">The full name of the unit (e.g. 'percent of page with').</param>
    /// <param name="shortcut">The shortcut of the unit (e.g. %PW).</param>
    /// <param name="divider">Used to calculate the relative value from the numeric value of the quantity. In the above example (percent of page width), the divider is 100.</param>
    /// <param name="referenceQuantity">The reference quantity.</param>
    public ChangeableRelativeUnit(string name, string shortcut, double divider, DimensionfulQuantity referenceQuantity)
    {
      _name = name ?? throw new ArgumentNullException(nameof(name));
      _shortCut = shortcut ?? throw new ArgumentNullException(nameof(shortcut));
      _divider = divider;
      _referenceQuantity = referenceQuantity;
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return _name; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return _shortCut; }
    }

    /// <inheritdoc/>
    public DimensionfulQuantity ReferenceQuantity
    {
      get
      {
        return _referenceQuantity;
      }
      set
      {
        _referenceQuantity = value;
      }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return (x / _divider) * _referenceQuantity.AsValueInSIUnits;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return _divider * x / _referenceQuantity.AsValueInSIUnits;
    }

    /// <inheritdoc/>
    public double GetRelativeValueFromValue(double x)
    {
      return x / _divider;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return _referenceQuantity.Unit.SIUnit; }
    }
  }
}
