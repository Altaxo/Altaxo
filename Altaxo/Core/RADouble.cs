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

#nullable enable
using System;

namespace Altaxo
{
  /// <summary>
  /// This structure holds a value, which is either absolute or relative to another value.
  /// </summary>
  [Serializable]
  public struct RADouble : IEquatable<RADouble>
  {
    /// <summary>
    /// True if the value <c>_value</c> is relative, false if <c>_value</c> is absolute.
    /// </summary>
    private bool _isRelative;

    /// <summary>
    /// The value to hold, either absolute or relative. If relative, the value 1 means the same value as the value it is relative to.
    /// </summary>
    private double _value;

    #region Serialization

    /// <summary>
    /// V0:
    /// V1: 2013-10-15 renaming and namespace move
    /// V2: 2023-01-14 move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.RelativeOrAbsoluteValue", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.RADouble", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RADouble), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RADouble)obj;

        info.AddValue("IsRelative", s._isRelative);
        info.AddValue("Value", s._value);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        bool rel = info.GetBoolean("IsRelative");
        double val = info.GetDouble("Value");
        return new RADouble(val, rel);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates the structure with the absolute value <paramref name="absval"/>.
    /// </summary>
    /// <param name="absval">The absolute value.</param>
    public RADouble(double absval)
    {
      _isRelative = false;
      _value = absval;
    }

    /// <summary>
    /// Constructs the structure with provided value and the information if it is an absolute or a relative value.
    /// </summary>
    /// <param name="val">The value, either absolute or relative.</param>
    /// <param name="isRelative">True if the value is relative, else false.</param>
    public RADouble(double val, bool isRelative)
    {
      _isRelative = isRelative;
      _value = val;
    }

    /// <summary>
    /// Creates a new <see cref="RADouble"/> with an absolute value.
    /// </summary>
    /// <param name="val">The absolute value.</param>
    /// <returns>A new <see cref="RADouble"/> instance with an absolute value.</returns>
    public static RADouble NewAbs(double val)
    {
      return new RADouble(val, false);
    }

    /// <summary>
    /// Creates a new <see cref="RADouble"/> with a relative value.
    /// </summary>
    /// <param name="val">The relative value.</param>
    /// <returns>A new <see cref="RADouble"/> instance with a relative value.</returns>
    public static RADouble NewRel(double val)
    {
      return new RADouble(val, true);
    }

    /// <summary>
    /// Gets a value indicating whether the value is relative.
    /// </summary>
    public bool IsRelative
    {
      get { return _isRelative; }
    }

    /// <summary>
    /// Gets a value indicating whether the value is absolute.
    /// </summary>
    public bool IsAbsolute
    {
      get { return !_isRelative; }
    }

    /// <summary>
    /// Gets the raw value. If the structure holds a relative value, this is the raw value, not the value relative to another.
    /// </summary>
    public double Value
    {
      get { return _value; }
    }

    /// <summary>
    /// Gets the value relative to the specified argument. If absolute, returns the stored value; if relative, returns the product of the stored value and <paramref name="r"/>.
    /// </summary>
    /// <param name="r">The value to which this value is relative to.</param>
    /// <returns>If absolute, the stored value; if relative, the product of the stored value with <paramref name="r"/>.</returns>
    public double GetValueRelativeTo(double r)
    {
      return _isRelative ? r * _value : _value;
    }

    /// <summary>
    /// Multiplies the <see cref="RADouble"/> value by a scale factor.
    /// </summary>
    /// <param name="r">The <see cref="RADouble"/> value.</param>
    /// <param name="scale">The scale factor.</param>
    /// <returns>The scaled <see cref="RADouble"/> value.</returns>
    public static RADouble operator *(RADouble r, double scale)
    {
      var result = r;
      r._value *= scale;
      return result;
    }

    /// <summary>
    /// Determines whether two <see cref="RADouble"/> instances are equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if both instances are equal; otherwise, false.</returns>
    public static bool operator ==(RADouble a, RADouble b)
    {
      return a._isRelative == b._isRelative && a._value == b._value;
    }

    /// <summary>
    /// Determines whether two <see cref="RADouble"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if both instances are not equal; otherwise, false.</returns>
    public static bool operator !=(RADouble a, RADouble b)
    {
      return !(a == b);
    }

    /// <inheritdoc/>
    public override bool Equals(object? o)
    {
      return o is RADouble other && _value == other._value && _isRelative == other._isRelative;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return _value.GetHashCode() + _isRelative.GetHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(RADouble other)
    {
      return _value == other._value && _isRelative == other._isRelative;
    }
  }
}
