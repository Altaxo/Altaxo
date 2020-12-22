#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Summary description for IXmlSerializationInfo.
  /// </summary>
  public interface IXmlSerializationInfo
  {
    void SetProperty(string propertyname, string? propertyvalue);

    string? GetProperty(string propertyname);

    /// <summary>
    /// Clears the property dictionary. Useful if the serialization info should be used to serialize multiple values.
    /// If you clear the properties before the serialization of each value, the serialization behaves as if each value is
    /// serialized independent of each other.
    /// </summary>
    void ClearProperties();

    void AddAttributeValue(string name, int val);

    void AddAttributeValue(string name, string val);

    void AddValue(string name, bool val);

    void AddValue(string name, bool? val);

    void AddValue(string name, char val);

    void AddValue(string name, int val);

    void AddValue(string name, int? val);

    void AddValue(string name, long val);

    void AddValue(string name, float val);

    void AddValue(string name, double val);

    void AddValue(string name, double? val);

    void AddValue(string name, string? val);

    void AddValue(string name, DateTime val);

    void AddValue(string name, TimeSpan val);

    void AddValue(string name, System.IO.MemoryStream val);

    void AddEnum(string name, System.Enum val); // special name since otherwise _all_ enums would be serialized by that

    void AddNullableEnum<T>(string name, T? val) where T : struct;

    void SetNodeContent(string nodeContent); // sets Node content directly

    void CreateArray(string name, int count);

    void CommitArray();

    void AddArray(string name, int[] val, int count);

    void AddArray(string name, float[] val, int count);

    void AddArray(string name, double[] val, int count);

    void AddArray(string name, DateTime[] val, int count);

    void AddArray(string name, string?[] val, int count);

    /// <summary>
    /// Adds an array of nullable boolean values. The array <paramref name="val"/> contains the boolean values, whereas
    /// the array <paramref name="cond"/> indicates if the boolean value is set (true) or not set (false).
    /// </summary>
    /// <param name="name">The name of the entry.</param>
    /// <param name="val">The value.</param>
    /// <param name="cond">The cond.</param>
    /// <param name="count">The count.</param>
    void AddArray(string name, System.Collections.BitArray val, System.Collections.BitArray cond, int count);

    void AddArray(string name, object[] val, int count);
    void AddArrayOfNullableElements(string name, object?[] val, int count);

    void CreateElement(string name);

    void CommitElement();

    void AddValue(string name, object o);
    void AddValueOrNull(string name, object? o);

    bool IsSerializable(object? o);

    void AddBaseValueEmbedded(object o, System.Type basetype);

    void AddBaseValueStandalone(string name, object o, System.Type basetype);

    XmlArrayEncoding DefaultArrayEncoding { get; set; }

    /// <summary>
    /// Writes a raw Xml string. This can be used for instance if the string to write was lazy loaded from another Xml document.
    /// </summary>
    /// <param name="rawXmlString">The raw XML string.</param>
    void WriteRaw(string rawXmlString);
  }
}
