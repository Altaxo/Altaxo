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
using System.Collections.Generic;

namespace Altaxo.Serialization.Xml
{
  public interface IXmlDeserializationInfo
  {
    /// <summary>Returns the name of the current xml element.</summary>
    string CurrentElementName { get; }

    bool GetBoolean();

    bool GetBoolean(string name);

    bool? GetNullableBoolean(string name);

    char GetChar(string name);

    int GetInt32();

    int GetInt32(string name);

    long GetInt64(string name);

    int? GetNullableInt32(string name);

    float GetSingle();

    float GetSingle(string name);

    double GetDouble();

    double GetDouble(string name);

    double? GetNullableDouble(string name);

    string GetString();

    string GetString(string name);

    DateTime GetDateTime(string name);

    TimeSpan GetTimeSpan(string name);

    System.IO.MemoryStream? GetMemoryStream(string name);

    object GetEnum(string name, System.Type type); // see remarks on serialization

    T GetEnum<T>(string name) where T : Enum;
    T? GetNullableEnum<T>(string name) where T : struct;

    string GetNodeContent(); // gets the inner text of the node directly

    int GetInt32Attribute(string name);

    string? GetStringAttributeOrNull(string name);

    string GetStringAttribute(string name);

    int OpenArray(); // get Number of Array elements

    int OpenArray(string name);

    void CloseArray(int count);

    void GetArray(out float[] val);

    /// <summary>
    /// Deserializes an array of double values. The array is allocated automatically.
    /// </summary>
    /// <param name="name">Name of the array.</param>
    /// <param name="val">The resulting deserialized array.</param>
    void GetArray(string name, out double[] val);

    void GetArray(string name, out int[] val);

    /// <summary>
    /// Deserializes an array of double value. The xml node must be opened before with <see cref="OpenArray()" />
    /// </summary>
    /// <param name="val">The array, must be at least of length <c>count</c>.</param>
    /// <param name="count">The number of elements to deserialize. If this is less than the number of elements in the xml stream, the other elements are safely ignored.</param>
    void GetArray(double[] val, int count);

    void GetArray(DateTime[] val, int count);

    void GetArray(string name, out string[] val);

    void GetArray(string?[] val, int count);

    /// <summary>
    /// Gets an array of nullable booleans.
    /// </summary>
    /// <param name="values">The boolean values to store (true or false).</param>
    /// <param name="conditions">The conditions to store (if true, the value is set, if false, the value is null).</param>
    /// <param name="count">The element count.</param>
    void GetArray(System.Collections.BitArray values, System.Collections.BitArray conditions, int count);


    void OpenElement();

    void CloseElement();

    /// <summary>Retrieves the name of the current node</summary>
    /// <returns>The name of the current node.</returns>
    string GetNodeName();

    object GetValue(string name, object? parent);

    object? GetValueOrNull(string name, object? parent);

    T GetValue<T>(string name, object? parentObject) where T : notnull;

    T? GetValueOrNull<T>(string name, object? parentObject) where T : class;

    T? GetNullableStruct<T>(string name, object? parentObject) where T : struct;

    object? GetValueOrOuterXml(string name, object parent, out bool returnValueIsOuterXml);

    void GetBaseValueEmbedded(object instance, System.Type basetype, object? parent);

    /// <summary>Deserializes the embedded base type.</summary>
    /// <param name="instance">The instance of the object to deserialize.</param>
    /// <param name="fullyQualifiedBaseTypeName">Fully qualified base type name. It is the short name of the assembly, comma, the full type name, comma, and the version. The string must not contain whitespaces. Example: 'AltaxoBase,SampleFileRenamer.Main.DocumentPath,0'.</param>
    /// <param name="parent">The parent object of the current object to deserialize.</param>
    [Obsolete("For backward compatibility only (if base type has changed). Use 'void GetBaseValueEmbedded(object instance, System.Type basetype, object? parent)' instead!")]
    object? GetBaseValueEmbeddedOrNull(object instance, string fullyQualifiedBaseTypeName, object? parent);

    void GetBaseValueStandalone(string name, object instance, System.Type basetype, object? parent);

    string GetElementAsOuterXml(string name);

    /// <summary>
    /// Gets the property dictionary. This is a dictionary where some string/value pairs could be stored, and used during or after deserialization
    /// </summary>
    /// <value>
    /// The property dictionary.
    /// </value>
    IDictionary<string, object?> PropertyDictionary { get; }

    /// <summary>
    /// Gets a property value from the property dictionary identified by the provided key string. If the property does not exist in the dictionary, the default value is returned.
    /// </summary>
    /// <typeparam name="T">Type of the property</typeparam>
    /// <param name="propertyKey">The property key.</param>
    /// <returns>If the property exists, the property value is returned; otherwise, the default value of the expected property value type is returned.</returns>
    T GetPropertyOrDefault<T>(string propertyKey);

    /// <summary>
    /// This event is called if the deserialization process of all objects is finished and
    /// the deserialized objects are sorted into the document. Then the application should
    /// call AllFinished, which fires this event. The purpose of this event is to
    /// resolve the references in the deserialized objects. This resolving process can be successfully
    /// done only if the objects are put in the right places in the document, so that
    /// the document paths can be resolved to the right objects.
    /// </summary>
    event XmlDeserializationCallbackEventHandler? DeserializationFinished;

    /// <summary>
    /// Occurs after (!) the deserialization process has completely finished, and the dirty flag of the document was cleared. This callback is intended to activate
    /// the data sources of the document, which should be suspended during the deserialization process.
    /// </summary>
    event Action? AfterDeserializationHasCompletelyFinished;
  }
}
