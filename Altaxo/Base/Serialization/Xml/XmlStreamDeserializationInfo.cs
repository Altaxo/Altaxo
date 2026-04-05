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
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Deserializes objects from an XML stream.
  /// </summary>
  public class XmlStreamDeserializationInfo : IXmlDeserializationInfo, IDisposable
  {
    private static readonly XmlTextReader _nullXmlReader = new XmlTextReader(System.IO.Stream.Null);

    private XmlTextReader _xmlReader;

    private XmlSurrogateSelector _surrogateSelector;
    private System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder();
    private Dictionary<string, object?> _propertyDictionary = new Dictionary<string, object?>();

    private byte[] _buffer;
    private int _bufferSize;

    private const int _size_of_int = 4;
    private const int _size_of_float = 4;
    private const int _size_of_double = 8;
    private const int _size_of_DateTime = 8;

    /// <summary>
    /// This event is called if the deserialization process of all objects is finished and
    /// the deserialized objects are sorted into the document. Then the application should
    /// call AllFinished, which fires this event. The purpose of this event is to
    /// resolve the references in the deserialized objects. This resolving process can be successfully
    /// done only if the objects are put in the right places in the document, so that
    /// the document paths can be resolved to the right objects.
    /// </summary>
    public event XmlDeserializationCallbackEventHandler? DeserializationFinished;

    /// <summary>
    /// Occurs after (!) the deserialization process has completely finished, and the dirty flag of the document was cleared. This callback is intended to activate
    /// the data sources of the document, which should be suspended during the deserialization process.
    /// </summary>
    public event Action? AfterDeserializationHasCompletelyFinished;

    /// <summary>
    /// Occurs when a new instance of this class is created. Argument is the created instance.
    /// This event is hold weak, thus you can safely add your handler without running in memory leaks.
    /// </summary>
    private static WeakDelegate<Action<XmlStreamDeserializationInfo>> _instanceCreated = new WeakDelegate<Action<XmlStreamDeserializationInfo>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlStreamDeserializationInfo"/> class.
    /// </summary>
    public XmlStreamDeserializationInfo()
    {
      _bufferSize = 16384;
      _buffer = new byte[_bufferSize];
      _surrogateSelector = new XmlSurrogateSelector();
      _surrogateSelector.TraceLoadedAssembliesForSurrogates();
      _xmlReader = _nullXmlReader;

      // Announce that an instance of the deserialization info was created.
      _instanceCreated.Target?.Invoke(this);
    }

    /// <summary>
    /// Begins reading from the specified stream.
    /// </summary>
    /// <param name="stream">The XML stream to read from.</param>
    public void BeginReading(System.IO.Stream stream)
    {
      _xmlReader = new XmlTextReader(stream);
      if (_propertyDictionary.ContainsKey(Altaxo.Serialization.Xml.XmlStreamSerializationInfo.UseXmlIndentation))
      {
        _xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
      }
      _xmlReader.MoveToContent();
    }

    /// <summary>
    /// Begins reading from the specified XML text.
    /// </summary>
    /// <param name="s">The XML text to read.</param>
    public void BeginReading(string s)
    {
      _xmlReader = new XmlTextReader(new System.IO.StringReader(s));
      _xmlReader.MoveToContent();
    }

    /// <summary>
    /// Begins reading from the specified XML reader.
    /// </summary>
    /// <param name="xmlReader">The XML reader to use.</param>
    public void BeginReading(XmlReader xmlReader)
    {
      if (xmlReader is XmlTextReader)
        _xmlReader = (XmlTextReader)xmlReader;
      else
        _xmlReader = (XmlTextReader)XmlTextReader.Create(xmlReader, new XmlReaderSettings());
    }

    /// <summary>
    /// Ends reading and detaches the current XML reader.
    /// </summary>
    public void EndReading()
    {
      // m_Reader.Close(); Do not close the reader, since the underlying stream is closed too then..., this will not work if reading zip files
      _xmlReader = _nullXmlReader;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      _xmlReader = _nullXmlReader;
    }

    /// <summary>
    /// Occurs when a new instance of this class is created. Argument is the created instance.
    /// This event is hold weak, thus you can safely add your handler without running in memory leaks.
    /// </summary>
    public static event Action<XmlStreamDeserializationInfo> InstanceCreated
    {
      add
      {
        _instanceCreated.Combine(value);
      }
      remove
      {
        _instanceCreated.Remove(value);
      }
    }

    /// <summary>
    /// Gets the property dictionary. This is a dictionary where some string/value pairs could be stored, and used during or after deserialization
    /// </summary>
    /// <value>
    /// The property dictionary.
    /// </value>
    public IDictionary<string, object?> PropertyDictionary { get { return _propertyDictionary; } }

    /// <inheritdoc />
    [return: MaybeNull]
    public T GetPropertyOrDefault<T>(string propertyKey)
    {
      if (_propertyDictionary.TryGetValue(propertyKey, out var result))
      {
        return (T)result!;
      }
      else
      {
#nullable disable
        return default;
#nullable enable
      }
    }



    /// <summary>
    /// Announces that deserialization of the document hierarchy has reached the current stage.
    /// </summary>
    /// <param name="documentRoot">The root of the deserialized document.</param>
    /// <param name="isFinalCall">If set to <c>true</c>, this is the final announcement for the document.</param>
    public void AnnounceDeserializationEnd(Main.IDocumentNode documentRoot, bool isFinalCall)
    {
      DeserializationFinished?.Invoke(this, documentRoot, isFinalCall);
    }

    /// <summary>
    /// Announces that deserialization has completely finished.
    /// </summary>
    public void AnnounceDeserializationHasCompletelyFinished()
    {
      AfterDeserializationHasCompletelyFinished?.Invoke();
    }

    #region IXmlSerializationInfo Members

    /// <summary>Returns the name of the current XML element.</summary>
    public string CurrentElementName
    {
      get { return _xmlReader.LocalName; }
    }

    /// <inheritdoc />
    public bool GetBoolean()
    {
      return XmlConvert.ToBoolean(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public bool GetBoolean(string name)
    {
      return XmlConvert.ToBoolean(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public bool? GetNullableBoolean(string name)
    {
      string s = _xmlReader.ReadElementString();
      if (string.IsNullOrEmpty(s))
        return null;
      else
        return XmlConvert.ToBoolean(s);
    }

    /// <inheritdoc />
    public char GetChar(string name)
    {
      return XmlConvert.ToChar(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public int GetInt32()
    {
      return XmlConvert.ToInt32(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public int? GetNullableInt32(string name)
    {
      string s = _xmlReader.ReadElementString();
      if (string.IsNullOrEmpty(s))
        return null;
      else
        return XmlConvert.ToInt32(s);
    }

    /// <inheritdoc />
    public int GetInt32(string name)
    {
      return GetInt32();
    }

    /// <inheritdoc />
    public long GetInt64(string name)
    {
      return XmlConvert.ToInt64(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public float GetSingle()
    {
      return XmlConvert.ToSingle(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public float GetSingle(string name)
    {
      return XmlConvert.ToSingle(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public double GetDouble()
    {
      return XmlConvert.ToDouble(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public double? GetNullableDouble(string name)
    {
      string s = _xmlReader.ReadElementString();
      if (string.IsNullOrEmpty(s))
        return null;
      else
        return XmlConvert.ToDouble(s);
    }

    /// <inheritdoc />
    public DateTime GetDateTime(string name)
    {
      return _xmlReader.ReadElementContentAsDateTime();
    }

    /// <inheritdoc />
    public TimeSpan GetTimeSpan(string name)
    {
      return XmlConvert.ToTimeSpan(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public double GetDouble(string name)
    {
      return XmlConvert.ToDouble(_xmlReader.ReadElementString());
    }

    /// <inheritdoc />
    public string GetString()
    {
      return _xmlReader.ReadElementString();
    }

    /// <inheritdoc />
    public string GetString(string name)
    {
      return GetString();
    }

    /// <inheritdoc />
    public System.IO.MemoryStream? GetMemoryStream(string name)
    {
      int length = XmlConvert.ToInt32(_xmlReader["Length"]!);
      if (length == 0)
      {
        _xmlReader.ReadStartElement();
        _xmlReader.ReadEndElement();
        return null;
      }
      else
      {
        byte[] buffer = new byte[length];

        int readed = _xmlReader.ReadElementContentAsBase64(buffer, 0, length);
        if (readed != length)
          throw new System.FormatException(string.Format("Length of the stream was smaller than denoted in the length attribute in the node header; Expected length: {0}; actual stream length: {1}", length, readed));
        if (0 != _xmlReader.ReadElementContentAsBase64(buffer, 0, length)) // this second call should return 0; it is only called to advance the text reader to the next node
          throw new System.FormatException("Length of the stream was greater than denoted in the length attribute in the node header");

        return new System.IO.MemoryStream(buffer);
      }
    }

    /// <inheritdoc />
    public object GetEnum(string name, System.Type type)
    {
      string val = _xmlReader.ReadElementString(name);
      return System.Enum.Parse(type, val);
    }

    /// <inheritdoc />
    public T GetEnum<T>(string name) where T : Enum
    {
      string val = _xmlReader.ReadElementString(name);
      return (T)System.Enum.Parse(typeof(T), val);
    }

    /// <inheritdoc />
    public T? GetNullableEnum<T>(string name) where T : struct
    {
      string val = _xmlReader.ReadElementString(name);
      if (string.IsNullOrEmpty(val))
        return null;
      else
        return (T)System.Enum.Parse(typeof(T), val);
    }

    /// <inheritdoc />
    public string GetNodeContent()
    {
      return _xmlReader.ReadString();
    }

    /// <inheritdoc />
    public int GetInt32Attribute(string name)
    {
      return XmlConvert.ToInt32(_xmlReader[name]??string.Empty);
    }

    /// <inheritdoc />
    public string? GetStringAttributeOrNull(string name)
    {
      return _xmlReader[name];
    }

    /// <inheritdoc />
    public string GetStringAttribute(string name)
    {
      return _xmlReader[name] ?? throw new InvalidOperationException($"String attribute \"{name}\" is missing");
    }

    /// <summary>
    /// Reads an array of primitive values encoded as Base64 into the provided destination array.
    /// </summary>
    /// <param name="val">The destination array.</param>
    /// <param name="count">The number of elements to read.</param>
    /// <param name="sizeOfElement">The size of each array element in bytes.</param>
    public void GetArrayOfPrimitiveTypeBase64(System.Array val, int count, int sizeOfElement)
    {
      int bytesreaded;
      int pos = 0;
      int remainingbytes = count * _size_of_double;
      while (remainingbytes > 0 && (0 != (bytesreaded = _xmlReader.ReadBase64(_buffer, 0, Math.Min(_bufferSize, remainingbytes)))))
      {
        if (!(0 == bytesreaded % _size_of_double))
          throw new InvalidProgramException();
        System.Buffer.BlockCopy(_buffer, 0, val, pos, bytesreaded);
        pos += bytesreaded;
        remainingbytes -= bytesreaded;
      }
      _xmlReader.Read(); // read the rest of the element
    }

    /// <summary>
    /// Reads an array of primitive values encoded as BinHex into the provided destination array.
    /// </summary>
    /// <param name="val">The destination array.</param>
    /// <param name="count">The number of elements to read.</param>
    /// <param name="sizeOfElement">The size of each array element in bytes.</param>
    public void GetArrayOfPrimitiveTypeBinHex(System.Array val, int count, int sizeOfElement)
    {
      int bytesreaded;
      int pos = 0;
      int remainingbytes = count * _size_of_double;
      while (remainingbytes > 0 && (0 != (bytesreaded = _xmlReader.ReadBinHex(_buffer, 0, Math.Min(_bufferSize, remainingbytes)))))
      {
        if (!(0 == bytesreaded % _size_of_double))
          throw new InvalidProgramException();
        System.Buffer.BlockCopy(_buffer, 0, val, pos, bytesreaded);
        pos += bytesreaded;
        remainingbytes -= bytesreaded;
      }
      _xmlReader.Read(); // read the rest of the element
    }

    /// <inheritdoc />
    public int OpenArray()
    {
      int count = XmlConvert.ToInt32(_xmlReader["Count"]!);

      if (count > 0)
        _xmlReader.ReadStartElement();

      return count;
    }

    /// <inheritdoc />
    public int OpenArray(string name)
    {
      int count = XmlConvert.ToInt32(_xmlReader["Count"]!);

      if (count > 0)
        _xmlReader.ReadStartElement();

      return count;
    }

    /// <inheritdoc />
    public void CloseArray(int count)
    {
      if (count > 0)
        _xmlReader.ReadEndElement();
      else
        _xmlReader.Read();
    }

    /// <inheritdoc />
    public void GetArray(out float[] val)
    {
      int count = GetInt32Attribute("Count");
      //int count = this.OpenArray();
      val = new float[count];
      // Attribute must be readed before ReadStartElement
      if (count > 0)
      {
        _xmlReader.ReadStartElement(); // read the first inner element

        switch (_xmlReader.Name)
        {
          default:
            for (int i = 0; i < count; i++)
              val[i] = _xmlReader.ReadElementContentAsFloat();
            break;

          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val, count, _size_of_float);
            break;

          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val, count, _size_of_float);
            break;
        } // end of switch
        _xmlReader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        _xmlReader.Read();
      }
    }

    /// <inheritdoc />
    public void GetArray(string name, out double[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new double[count];
      GetArray(val, count);
    }

    /// <inheritdoc />
    public void GetArray(double[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if (count > 0)
      {
        _xmlReader.ReadStartElement(); // read the first inner element

        switch (_xmlReader.Name)
        {
          default:
            for (int i = 0; i < count; i++)
              val[i] = _xmlReader.ReadElementContentAsDouble();
            break;

          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val, count, _size_of_double);
            break;

          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val, count, _size_of_double);
            break;
        } // end of switch
        _xmlReader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        _xmlReader.Read();
      }
    }

    /// <inheritdoc />
    public void GetArray(string name, out int[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new int[count];
      GetArray(val, count);
    }

    /// <summary>
    /// Deserializes an array of integer values from the current XML element.
    /// </summary>
    /// <param name="val">The destination array, which must be able to store at least <paramref name="count" /> values.</param>
    /// <param name="count">The number of elements to read.</param>
    public void GetArray(int[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if (count > 0)
      {
        _xmlReader.ReadStartElement(); // read the first inner element

        switch (_xmlReader.Name)
        {
          default:
            for (int i = 0; i < count; i++)
              val[i] = _xmlReader.ReadElementContentAsInt();
            break;

          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val, count, _size_of_int);
            break;

          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val, count, _size_of_int);
            break;
        } // end of switch
        _xmlReader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        _xmlReader.Read();
      }
    }

    /// <inheritdoc />
    public void GetArray(DateTime[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if (count > 0)
      {
        _xmlReader.ReadStartElement(); // read the first inner element

        switch (_xmlReader.Name)
        {
          default:
            for (int i = 0; i < count; i++)
              val[i] = _xmlReader.ReadElementContentAsDateTime();
            break;

          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val, count, _size_of_DateTime);
            break;

          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val, count, _size_of_DateTime);
            break;
        } // end of switch
        _xmlReader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        _xmlReader.Read();
      }
    }

    /// <inheritdoc />
    public string[] GetArrayOfStrings(string name)
    {
      int count = GetInt32Attribute("Count");
      var val = new string[count];
      GetArray(val, count);
      return val;
    }

    /// <inheritdoc />
    public void GetArray(string?[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if (count > 0)
      {
        _xmlReader.ReadStartElement(); // read the first inner element

        for (int i = 0; i < count; i++)
          val[i] = _xmlReader.ReadElementString();
        _xmlReader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        _xmlReader.Read();
      }
    }

    /// <inheritdoc />
    public void GetArray(System.Collections.BitArray values, System.Collections.BitArray conditions, int count)
    {
      // Attribute must be readed before ReadStartElement
      if (count > 0)
      {
        _xmlReader.ReadStartElement(); // read the first inner element

        for (int i = 0; i < count; i++)
        {
          var s = _xmlReader.ReadElementString();
          if (string.IsNullOrEmpty(s))
          {
            conditions[i] = false;
            values[i] = false;
          }
          else
          {
            conditions[i] = true;
            values[i] = System.Xml.XmlConvert.ToBoolean(s);
          }
        }
        _xmlReader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        _xmlReader.Read();
      }
    }


    /// <inheritdoc />
    public T[] GetArrayOfValues<T>(string name, object? parent)
    {
      var count = OpenArray(name);
      var result = new T[count];
      for (int i = 0; i < count; ++i)
        result[i] = (T)GetValue("e", parent);
      CloseArray(count);
      return result;
    }


    /// <inheritdoc />
    public void OpenElement()
    {
      _xmlReader.ReadStartElement();
    }

    /// <inheritdoc />
    public void CloseElement()
    {
      _xmlReader.ReadEndElement();
    }

    /// <inheritdoc />
    public string GetNodeName()
    {
      return _xmlReader.LocalName;
    }

    /// <inheritdoc />
    public object GetValue(string name, object? parentobject)
    {
#if !NONULLSTRICTCHECK
      return GetValueOrNull(parentobject) ?? throw new DeserializationNullException(name, parentobject);
#else
      return GetValueOrNull(parentobject)!;
#endif
    }

    /// <inheritdoc />
    public object? GetValueOrNull(string name, object? parentobject)
    {
      return GetValueOrNull(parentobject);
    }

    /// <inheritdoc />
    public T GetValue<T>(string name, object? parentObject) where T : notnull
    {

#if !NONULLSTRICTCHECK
      var o = GetValueOrNull(parentObject) ?? throw new DeserializationNullException(name, parentObject);

      return (o is T result) ?
          result :
          throw new DeserializationException($"Name: {name}, Parent: {parentObject}, Type unexpected: Current: {o?.GetType()} but expected: {typeof(T)}");
#else
      var o = GetValueOrNull(parentObject);
#nullable disable
      return (T)o;
#nullable enable
#endif
    }

    /// <inheritdoc />
    public T? GetValueOrNull<T>(string name, object? parentObject) where T : class
    {
      var o = GetValueOrNull(parentObject);

      return (o is null) ?
          null :
          (o is T result) ?
              result :
              throw new DeserializationException($"Name: {name}, Parent: {parentObject}, Type unexpected: Current: {o?.GetType()} but expected: {typeof(T)}");
    }

    /// <inheritdoc />
    public T? GetNullableStruct<T>(string name, object? parentObject) where T : struct
    {
      var o = GetValueOrNull(parentObject);

      return o switch
      {
        null => (T?)null,
        T t => t,
        _ => throw new DeserializationException($"Name: {name}, Parent: {parentObject}, Type unexpected: Current: {o?.GetType()} but expected: {typeof(T)}")
      };
    }


    /// <summary>
    /// Reads a value from the current element using its <c>Type</c> attribute and returns <see langword="null" /> for undefined values.
    /// </summary>
    /// <param name="parentobject">The parent object used as context for deserialization.</param>
    /// <returns>The deserialized value, or <see langword="null" /> if the current element represents an undefined value.</returns>
    public object? GetValueOrNull(object? parentobject)
    {
      var type = _xmlReader.GetAttribute("Type");

      if (type is not null)
      {
        if ("UndefinedValue" == type)
        {
          _xmlReader.Read();
          return null;
        }

        // Get the surrogate for this type
        if (_surrogateSelector.GetSurrogate(type) is { } surr)
        {
          bool bNotEmpty = !_xmlReader.IsEmptyElement;
          // System.Diagnostics.Trace.WriteLine(string.Format("Xml val {0}, type {1}, empty:{2}",m_Reader.Name,type,bNotEmpty));

          if (bNotEmpty)
            _xmlReader.ReadStartElement();  // note: this must now be done by  in the deserialization code

          var retvalue = surr.Deserialize(null, this, parentobject);

          if (bNotEmpty)
            _xmlReader.ReadEndElement();
          else
            _xmlReader.Read();

          return retvalue;
        }
        else
        {
          throw new ApplicationException(string.Format("Unable to find XmlSurrogate for type {0}!", type));
        }
      }
      else
      {
        throw new ApplicationException(string.Format("Unable to deserialize element at line {0}, position {1}, Type attribute missing!", _xmlReader.LineNumber, _xmlReader.LinePosition));
      }
    }

    /// <summary>
    /// Intended for lazy loading. Tries to get the deserialized value. If the type is unknown (this can happen if (especially for addin DLLs) a DLL
    /// is not loaded yet), the outer XML node is returned as string value, so it can be deserialized later.
    /// </summary>
    /// <param name="name">Name of the node.</param>
    /// <param name="parentobject">The parent object.</param>
    /// <param name="returnValueIsOuterXml">If set to <c>true</c>, the return value is a string containing the outer XML node.</param>
    /// <returns>The deserialized value (if <paramref name="returnValueIsOuterXml"/> is false), or the outer XML node (if <paramref name="returnValueIsOuterXml"/> is true).</returns>
    /// <exception cref="ApplicationException"></exception>
    /// <inheritdoc />
    public object? GetValueOrOuterXml(string name, object parentobject, out bool returnValueIsOuterXml)
    {
      returnValueIsOuterXml = false;
      var type = _xmlReader.GetAttribute("Type");

      if (type is not null)
      {
        if ("UndefinedValue" == type)
        {
          _xmlReader.Read();
          return null;
        }

        // Get the surrogate for this type
        if (_surrogateSelector.GetSurrogate(type) is { } surr)
        {
          bool bNotEmpty = !_xmlReader.IsEmptyElement;
          // System.Diagnostics.Trace.WriteLine(string.Format("Xml val {0}, type {1}, empty:{2}",m_Reader.Name,type,bNotEmpty));

          if (bNotEmpty)
            _xmlReader.ReadStartElement();  // note: this must now be done by  in the deserialization code

          var retvalue = surr.Deserialize(null, this, parentobject);

          if (bNotEmpty)
            _xmlReader.ReadEndElement();
          else
            _xmlReader.Read();

          return retvalue;
        }
        else
        {
          returnValueIsOuterXml = true;
          return _xmlReader.ReadOuterXml();
        }
      }
      else
      {
        throw new ApplicationException(string.Format("Unable to deserialize element at line {0}, position {1}, Type attribute missing!", _xmlReader.LineNumber, _xmlReader.LinePosition));
      }
    }



    /// <inheritdoc />
    public void GetBaseValueEmbedded(object instance, System.Type basetype, object? parent)
    {
      if ("BaseType" == CurrentElementName)
      {
        string basetypestring = _xmlReader.ReadElementString();
        if (_surrogateSelector.GetSurrogate(basetypestring) is { } ss)
          ss.Deserialize(instance, this, parent);
        else
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));

      }
      else
      {
        if (_surrogateSelector.GetSurrogate(basetype) is { } ss)
          ss.Deserialize(instance, this, parent);
        else
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
      }
    }


    /// <summary>Deserializes the embedded base type.</summary>
    /// <param name="instance">The instance of the object to deserialize.</param>
    /// <param name="fullyQualifiedBaseTypeName">Fully qualified base type name. It is the short name of the assembly, comma, the full type name, comma, and the version. The string must not contain whitespaces. Example: 'AltaxoBase,SampleFileRenamer.Main.DocumentPath,0'.</param>
    /// <param name="parent">The parent object of the current object to deserialize.</param>
    /// <inheritdoc />
    public object? GetBaseValueEmbeddedOrNull(object instance, string fullyQualifiedBaseTypeName, object? parent)
    {
      object? obj;
      if ("BaseType" == CurrentElementName)
      {
        string basetypestring = _xmlReader.ReadElementString();
        if (_surrogateSelector.GetSurrogate(basetypestring) is { } ss)
          obj = ss.Deserialize(instance, this, parent);
        else
          throw new DeserializationException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", fullyQualifiedBaseTypeName));
      }
      else
      {
        if (_surrogateSelector.GetSurrogate(fullyQualifiedBaseTypeName) is { } ss)
          obj = ss.Deserialize(instance, this, parent);
        else
          throw new DeserializationException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", fullyQualifiedBaseTypeName));
      }


      return obj;
    }

    /// <inheritdoc />
    public void GetBaseValueStandalone(string name, object instance, System.Type basetype, object? parent)
    {
      if (_surrogateSelector.GetSurrogate(basetype) is { } ss)
      {
        _xmlReader.ReadStartElement(); // note: this must now be done by  in the deserialization code
        ss.Deserialize(instance, this, parent);
        _xmlReader.ReadEndElement();
      }
      else
        throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));

    }

    /// <inheritdoc />
    public string GetElementAsOuterXml(string name)
    {
      return _xmlReader.ReadOuterXml();
    }

    #endregion IXmlSerializationInfo Members
  }
}
