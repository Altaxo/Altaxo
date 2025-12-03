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
  /// <summary>
  /// Provides an abstraction for reading values during XML deserialization.
  /// Implementations expose helpers to read primitives, arrays, complex values, and to manage element scope and callbacks.
  /// </summary>
  public interface IXmlDeserializationInfo
  {
    /// <summary>Returns the name of the current xml element.</summary>
    string CurrentElementName { get; }

    /// <summary>Reads a boolean value from the current element content.</summary>
    bool GetBoolean();

    /// <summary>Reads a named boolean child element.</summary>
    /// <param name="name">Element name.</param>
    bool GetBoolean(string name);

    /// <summary>Reads a named nullable boolean child element.</summary>
    /// <param name="name">Element name.</param>
    bool? GetNullableBoolean(string name);

    /// <summary>Reads a named char child element.</summary>
    /// <param name="name">Element name.</param>
    char GetChar(string name);

    /// <summary>Reads an Int32 from the current element content.</summary>
    int GetInt32();

    /// <summary>Reads a named Int32 child element.</summary>
    /// <param name="name">Element name.</param>
    int GetInt32(string name);

    /// <summary>Reads a named Int64 child element.</summary>
    /// <param name="name">Element name.</param>
    long GetInt64(string name);

    /// <summary>Reads a named nullable Int32 child element.</summary>
    /// <param name="name">Element name.</param>
    int? GetNullableInt32(string name);

    /// <summary>Reads a Single from the current element content.</summary>
    float GetSingle();

    /// <summary>Reads a named Single child element.</summary>
    /// <param name="name">Element name.</param>
    float GetSingle(string name);

    /// <summary>Reads a Double from the current element content.</summary>
    double GetDouble();

    /// <summary>Reads a named Double child element.</summary>
    /// <param name="name">Element name.</param>
    double GetDouble(string name);

    /// <summary>Reads a named nullable Double child element.</summary>
    /// <param name="name">Element name.</param>
    double? GetNullableDouble(string name);

    /// <summary>Reads a string from the current element content.</summary>
    string GetString();

    /// <summary>Reads a named string child element.</summary>
    /// <param name="name">Element name.</param>
    string GetString(string name);

    /// <summary>Reads a named DateTime child element.</summary>
    /// <param name="name">Element name.</param>
    DateTime GetDateTime(string name);

    /// <summary>Reads a named TimeSpan child element.</summary>
    /// <param name="name">Element name.</param>
    TimeSpan GetTimeSpan(string name);

    /// <summary>Reads a named MemoryStream child element.</summary>
    /// <param name="name">Element name.</param>
    System.IO.MemoryStream? GetMemoryStream(string name);

    /// <summary>Reads a named enum value of the given <paramref name="type"/>.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="type">Enum type.</param>
    object GetEnum(string name, System.Type type); // see remarks on serialization

    /// <summary>Reads a named enum value of type <typeparamref name="T"/>.</summary>
    /// <param name="name">Element name.</param>
    T GetEnum<T>(string name) where T : Enum;

    /// <summary>Reads a named nullable enum value of type <typeparamref name="T"/>.</summary>
    /// <param name="name">Element name.</param>
    T? GetNullableEnum<T>(string name) where T : struct;

    /// <summary>Gets the inner text content of the current node.</summary>
    string GetNodeContent(); // gets the inner text of the node directly

    /// <summary>Reads an Int32 attribute of the current element.</summary>
    /// <param name="name">Attribute name.</param>
    int GetInt32Attribute(string name);

    /// <summary>Reads a string attribute or returns null if missing.</summary>
    /// <param name="name">Attribute name.</param>
    string? GetStringAttributeOrNull(string name);

    /// <summary>Reads a string attribute of the current element.</summary>
    /// <param name="name">Attribute name.</param>
    string GetStringAttribute(string name);

    /// <summary>Opens the current array and returns its element count.</summary>
    int OpenArray(); // get Number of Array elements

    /// <summary>Opens a named array element and returns its element count.</summary>
    /// <param name="name">Element name.</param>
    int OpenArray(string name);

    /// <summary>Closes the current array scope.</summary>
    /// <param name="count">Expected number of elements processed.</param>
    void CloseArray(int count);

    /// <summary>Deserializes an array of Single values from the current node.</summary>
    /// <param name="val">Destination array allocated by the implementation.</param>
    void GetArray(out float[] val);

    /// <summary>
    /// Deserializes an array of double values. The array is allocated automatically.
    /// </summary>
    /// <param name="name">Name of the array.</param>
    /// <param name="val">The resulting deserialized array.</param>
    void GetArray(string name, out double[] val);

    /// <summary>Deserializes an array of Int32 values.</summary>
    /// <param name="name">Name of the array.</param>
    /// <param name="val">The resulting deserialized array.</param>
    void GetArray(string name, out int[] val);

    /// <summary>
    /// Deserializes an array of double value. The xml node must be opened before with <see cref="OpenArray()" />
    /// </summary>
    /// <param name="val">The array, must be at least of length <c>count</c>.</param>
    /// <param name="count">The number of elements to deserialize. If this is less than the number of elements in the xml stream, the other elements are safely ignored.</param>
    void GetArray(double[] val, int count);

    /// <summary>Deserializes an array of DateTime values.</summary>
    /// <param name="val">Destination array.</param>
    /// <param name="count">Number of elements to read.</param>
    void GetArray(DateTime[] val, int count);

    /// <summary>Reads an array of strings from a named element.</summary>
    /// <param name="name">Element name.</param>
    /// <returns>Array of strings.</returns>
    string[] GetArrayOfStrings(string name);

    /// <summary>Deserializes an array of nullable strings.</summary>
    /// <param name="val">Destination array.</param>
    /// <param name="count">Number of elements to read.</param>
    void GetArray(string?[] val, int count);

    /// <summary>
    /// Gets an array of nullable booleans.
    /// </summary>
    /// <param name="values">The boolean values to store (true or false).</param>
    /// <param name="conditions">The conditions to store (if true, the value is set, if false, the value is null).</param>
    /// <param name="count">The element count.</param>
    void GetArray(System.Collections.BitArray values, System.Collections.BitArray conditions, int count);

    /// <summary>Reads an array of values of type <typeparamref name="T"/>.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="parent">Parent object for context.</param>
    /// <returns>Array of <typeparamref name="T"/>.</returns>
    T[] GetArrayOfValues<T>(string name, object parent);

    /// <summary>Opens the current element.</summary>
    void OpenElement();

    /// <summary>Closes the current element.</summary>
    void CloseElement();

    /// <summary>Retrieves the name of the current node</summary>
    /// <returns>The name of the current node.</returns>
    string GetNodeName();

    /// <summary>Reads a value of unknown type.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="parent">Parent object for context.</param>
    /// <returns>Deserialized value.</returns>
    object GetValue(string name, object? parent);

    /// <summary>Reads a value of unknown type or returns null.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="parent">Parent object for context.</param>
    object? GetValueOrNull(string name, object? parent);

    /// <summary>Reads a value of type <typeparamref name="T"/>.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="parentObject">Parent object for context.</param>
    /// <returns>Deserialized value.</returns>
    T GetValue<T>(string name, object? parentObject) where T : notnull;

    /// <summary>Reads a value of type <typeparamref name="T"/> or returns null.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="parentObject">Parent object for context.</param>
    T? GetValueOrNull<T>(string name, object? parentObject) where T : class;

    /// <summary>Reads a nullable struct value.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="parentObject">Parent object for context.</param>
    T? GetNullableStruct<T>(string name, object? parentObject) where T : struct;

    /// <summary>Reads a value or returns the outer XML when not deserializable.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="parent">Parent object for context.</param>
    /// <param name="returnValueIsOuterXml">True if the return value is the outer XML.</param>
    /// <returns>Value or outer XML.</returns>
    object? GetValueOrOuterXml(string name, object parent, out bool returnValueIsOuterXml);

    /// <summary>Reads an embedded base value for a derived instance.</summary>
    /// <param name="instance">Instance to populate.</param>
    /// <param name="basetype">Base type.</param>
    /// <param name="parent">Parent object for context.</param>
    void GetBaseValueEmbedded(object instance, System.Type basetype, object? parent);

    /// <summary>Deserializes the embedded base type.</summary>
    /// <param name="instance">The instance of the object to deserialize.</param>
    /// <param name="fullyQualifiedBaseTypeName">Fully qualified base type name. It is the short name of the assembly, comma, the full type name, comma, and the version. The string must not contain whitespaces. Example: 'AltaxoBase,SampleFileRenamer.Main.DocumentPath,0'.</param>
    /// <param name="parent">The parent object of the current object to deserialize.</param>
    [Obsolete("For backward compatibility only (if base type has changed). Use 'void GetBaseValueEmbedded(object instance, System.Type basetype, object? parent)' instead!")]
    object? GetBaseValueEmbeddedOrNull(object instance, string fullyQualifiedBaseTypeName, object? parent);

    /// <summary>Reads a standalone base value for a derived instance.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="instance">Instance to populate.</param>
    /// <param name="basetype">Base type.</param>
    /// <param name="parent">Parent object for context.</param>
    void GetBaseValueStandalone(string name, object instance, System.Type basetype, object? parent);

    /// <summary>Gets the outer XML of a named element.</summary>
    /// <param name="name">Element name.</param>
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
