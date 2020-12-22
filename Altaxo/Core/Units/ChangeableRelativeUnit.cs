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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ChangeableRelativeUnit), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ChangeableRelativeUnit)obj;

        info.AddValue("Name", s._name);
        info.AddValue("ShortCut", s._shortCut);
        info.AddValue("Divider", s._divider);
        info.AddValue("Reference", s._referenceQuantity);
      }

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

    /// <summary>Full name of the unit.</summary>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>Usual shortcut of the unit.</summary>
    public string ShortCut
    {
      get { return _shortCut; }
    }

    /// <summary>The corresponding quantity that this unit encapsulates.</summary>
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

    /// <summary>Converts <paramref name="x"/> to the corresponding SI unit.</summary>
    /// <param name="x">Value to convert.</param>
    /// <returns>The corresponding value of <paramref name="x"/> in SI units.</returns>
    public double ToSIUnit(double x)
    {
      return (x / _divider) * _referenceQuantity.AsValueInSIUnits;
    }

    /// <summary>Converts <paramref name="x"/> (in SI units) to the corresponding value in this unit.</summary>
    /// <param name="x">Value in SI units.</param>
    /// <returns>The corresponding value in this unit.</returns>
    public double FromSIUnit(double x)
    {
      return _divider * x / _referenceQuantity.AsValueInSIUnits;
    }

    /// <summary>
    /// Calculated the dimensionless prefactor to multiply the <see cref="ReferenceQuantity"/> with.
    /// Example: Given that the relative unit is 'percent of page with', a value of <paramref name="x"/>=5 is converted to 0.05. The result can then be used
    /// to calculate the absolute quantity by multiplying the result of 0.05 with the 'page with'.
    /// </summary>
    /// <param name="x">Numerical value to convert.</param>
    /// <returns>The prefactor to multiply the <see cref="ReferenceQuantity"/> with in order to get the absolute quantity.</returns>
    public double GetRelativeValueFromValue(double x)
    {
      return x / _divider;
    }

    /// <summary>Returns a list of possible prefixes for this unit (like µ, m, k, M, G..).</summary>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <summary>Returns the corresponding SI unit.</summary>
    public SIUnit SIUnit
    {
      get { return _referenceQuantity.Unit.SIUnit; }
    }
  }
}
