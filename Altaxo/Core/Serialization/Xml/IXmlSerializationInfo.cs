#nullable enable

using System;
using System.Collections.Generic;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Abstraction for writing values to an XML stream during serialization.
  /// Provides APIs to write primitives, arrays, elements, complex objects, and raw XML, including base-type handling.
  /// </summary>
  public interface IXmlSerializationInfo
  {
    /// <summary>Sets a custom property on the serializer.</summary>
    /// <param name="propertyname">Property name.</param>
    /// <param name="propertyvalue">Property value.</param>
    void SetProperty(string propertyname, string? propertyvalue);

    /// <summary>Gets a previously set custom property.</summary>
    /// <param name="propertyname">Property name.</param>
    /// <returns>Property value or null.</returns>
    string? GetProperty(string propertyname);

    /// <summary>
    /// Clears the property dictionary. Useful if the serialization info should be used to serialize multiple values.
    /// If you clear the properties before the serialization of each value, the serialization behaves as if each value is
    /// serialized independent of each other.
    /// </summary>
    void ClearProperties();

    /// <summary>Adds an Int32 attribute to the current element.</summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="val">Attribute value.</param>
    void AddAttributeValue(string name, int val);

    /// <summary>Adds a string attribute to the current element.</summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="val">Attribute value.</param>
    void AddAttributeValue(string name, string val);

    /// <summary>Adds a boolean value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, bool val);

    /// <summary>Adds a nullable boolean value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, bool? val);

    /// <summary>Adds a char value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, char val);

    /// <summary>Adds an Int32 value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, int val);

    /// <summary>Adds a nullable Int32 value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, int? val);

    /// <summary>Adds an Int64 value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, long val);

    /// <summary>Adds a Single value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, float val);

    /// <summary>Adds a Double value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, double val);

    /// <summary>Adds a nullable Double value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, double? val);

    /// <summary>Adds a string value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, string? val);

    /// <summary>Adds a DateTime value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, DateTime val);

    /// <summary>Adds a TimeSpan value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, TimeSpan val);

    /// <summary>Adds a MemoryStream value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Value.</param>
    void AddValue(string name, System.IO.MemoryStream val);

    /// <summary>Adds an enum value element.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Enum value.</param>
    void AddEnum(string name, System.Enum val); // special name since otherwise _all_ enums would be serialized by that

    /// <summary>Adds a nullable enum value element.</summary>
    /// <typeparam name="T">Enum type.</typeparam>
    /// <param name="name">Element name.</param>
    /// <param name="val">Enum value.</param>
    void AddNullableEnum<T>(string name, T? val) where T : struct;

    /// <summary>Sets the text content of the current node directly.</summary>
    /// <param name="nodeContent">Inner text.</param>
    void SetNodeContent(string nodeContent); // sets Node content directly

    /// <summary>Begins an array scope.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="count">Number of elements to write.</param>
    void CreateArray(string name, int count);

    /// <summary>Commits the current array scope.</summary>
    void CommitArray();

    /// <summary>Writes an array of Int32 values.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Values.</param>
    /// <param name="count">Number of elements.</param>
    void AddArray(string name, IReadOnlyList<int> val, int count);

    /// <summary>Writes an array of Single values.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Values.</param>
    /// <param name="count">Number of elements.</param>
    void AddArray(string name, float[] val, int count);

    /// <summary>Writes an array of Double values.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Values.</param>
    /// <param name="count">Number of elements.</param>
    void AddArray(string name, IReadOnlyList<double> val, int count);

    /// <summary>Writes an array of DateTime values.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Values.</param>
    /// <param name="count">Number of elements.</param>
    void AddArray(string name, DateTime[] val, int count);

    /// <summary>Writes an array of string values.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Values.</param>
    /// <param name="count">Number of elements.</param>
    void AddArray(string name, IReadOnlyList<string?> val, int count);

    /// <summary>
    /// Adds an array of nullable boolean values. The array <paramref name="val"/> contains the boolean values, whereas
    /// the array <paramref name="cond"/> indicates if the boolean value is set (true) or not set (false).
    /// </summary>
    /// <param name="name">The name of the entry.</param>
    /// <param name="val">The value.</param>
    /// <param name="cond">The cond.</param>
    /// <param name="count">The count.</param>
    void AddArray(string name, System.Collections.BitArray val, System.Collections.BitArray cond, int count);

    /// <summary>Writes an array of object values.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Values.</param>
    /// <param name="count">Number of elements.</param>
    void AddArray(string name, IReadOnlyList<object> val, int count);

    /// <summary>Writes an array of nullable object values.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="val">Values.</param>
    /// <param name="count">Number of elements.</param>
    void AddArrayOfNullableElements(string name, IReadOnlyList<object?> val, int count);

    /// <summary>Creates a child element to write values into.</summary>
    /// <param name="name">Element name.</param>
    void CreateElement(string name);

    /// <summary>Commits the current element.</summary>
    void CommitElement();

    /// <summary>Adds a complex value by serializing the object.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="o">Object to serialize.</param>
    void AddValue(string name, object o);

    /// <summary>Adds a complex value or null.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="o">Object to serialize or null.</param>
    void AddValueOrNull(string name, object? o);

    /// <summary>Checks if an object is serializable by this serializer.</summary>
    /// <param name="o">Object to check.</param>
    /// <returns><c>true</c> if serializable.</returns>
    bool IsSerializable(object? o);

    /// <summary>Writes the embedded base portion of a derived object.</summary>
    /// <param name="o">Object instance.</param>
    /// <param name="basetype">Base type.</param>
    void AddBaseValueEmbedded(object o, System.Type basetype);

    /// <summary>Writes the standalone base portion of a derived object.</summary>
    /// <param name="name">Element name.</param>
    /// <param name="o">Object instance.</param>
    /// <param name="basetype">Base type.</param>
    void AddBaseValueStandalone(string name, object o, System.Type basetype);

    /// <summary>Gets or sets the default array encoding strategy.</summary>
    /// <value>The default array encoding to use when writing arrays.</value>
    XmlArrayEncoding DefaultArrayEncoding { get; set; }

    /// <summary>
    /// Writes a raw Xml string. This can be used for instance if the string to write was lazy loaded from another Xml document.
    /// </summary>
    /// <param name="rawXmlString">The raw XML string.</param>
    void WriteRaw(string rawXmlString);
  }
}
