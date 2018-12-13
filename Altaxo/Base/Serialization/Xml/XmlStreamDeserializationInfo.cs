#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Xml;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Deserializes object from a XML stream.
  /// </summary>
  public class XmlStreamDeserializationInfo : IXmlDeserializationInfo, IDisposable
  {
    private XmlTextReader _xmlReader;

    private XmlSurrogateSelector _surrogateSelector;
    private System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder();
    private Dictionary<string, object> _propertyDictionary = new Dictionary<string, object>();

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
    public event XmlDeserializationCallbackEventHandler DeserializationFinished;

    /// <summary>
    /// Occurs after (!) the deserialization process has completely finished, and the dirty flag of the document was cleared. This callback is intended to activate
    /// the data sources of the document, which should be suspended during the deserialization process.
    /// </summary>
    public event Action AfterDeserializationHasCompletelyFinished;

    /// <summary>
    /// Occurs when a new instance of this class is created. Argument is the created instance.
    /// This event is hold weak, thus you can safely add your handler without running in memory leaks.
    /// </summary>
    private static WeakDelegate<Action<XmlStreamDeserializationInfo>> _instanceCreated = new WeakDelegate<Action<XmlStreamDeserializationInfo>>();

    public XmlStreamDeserializationInfo()
    {
      _bufferSize = 16384;
      _buffer = new byte[_bufferSize];
      _surrogateSelector = new XmlSurrogateSelector();
      _surrogateSelector.TraceLoadedAssembliesForSurrogates();

      // Announce that an instance of the deserialization info was created.
      _instanceCreated.Target?.Invoke(this);
    }

    public void BeginReading(System.IO.Stream stream)
    {
      _xmlReader = new XmlTextReader(stream);
      _xmlReader.MoveToContent();
    }

    public void BeginReading(string s)
    {
      _xmlReader = new XmlTextReader(new System.IO.StringReader(s));
      _xmlReader.MoveToContent();
    }

    public void BeginReading(XmlReader xmlReader)
    {
      if (xmlReader is XmlTextReader)
        _xmlReader = (XmlTextReader)xmlReader;
      else
        _xmlReader = (XmlTextReader)XmlTextReader.Create(xmlReader, new XmlReaderSettings());
    }

    public void EndReading()
    {
      // m_Reader.Close(); Do not close the reader, since the underlying stream is closed too then..., this will not work if reading zip files
      _xmlReader = null;
    }

    public void Dispose()
    {
      _xmlReader = null;
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
    public IDictionary<string, object> PropertyDictionary { get { return _propertyDictionary; } }

    /// <inheritdoc />
    public T GetPropertyOrDefault<T>(string propertyKey)
    {
      if (_propertyDictionary.TryGetValue(propertyKey, out var result))
      {
        return (T)result;
      }
      else
      {
        return default(T);
      }
    }

    public void AnnounceDeserializationEnd(Main.IDocumentNode documentRoot, bool isFinalCall)
    {
      DeserializationFinished?.Invoke(this, documentRoot, isFinalCall);
    }

    public void AnnounceDeserializationHasCompletelyFinished()
    {
      AfterDeserializationHasCompletelyFinished?.Invoke();
    }

    #region IXmlSerializationInfo Members

    /// <summary>Returns the name of the current xml element.</summary>
    public string CurrentElementName
    {
      get { return _xmlReader.LocalName; }
    }

    public bool GetBoolean()
    {
      return XmlConvert.ToBoolean(_xmlReader.ReadElementString());
    }

    public bool GetBoolean(string name)
    {
      return XmlConvert.ToBoolean(_xmlReader.ReadElementString());
    }

    public bool? GetNullableBoolean(string name)
    {
      string s = _xmlReader.ReadElementString();
      if (string.IsNullOrEmpty(s))
        return null;
      else
        return XmlConvert.ToBoolean(s);
    }

    public char GetChar(string name)
    {
      return XmlConvert.ToChar(_xmlReader.ReadElementString());
    }

    public int GetInt32()
    {
      return XmlConvert.ToInt32(_xmlReader.ReadElementString());
    }

    public int? GetNullableInt32(string name)
    {
      string s = _xmlReader.ReadElementString();
      if (string.IsNullOrEmpty(s))
        return null;
      else
        return XmlConvert.ToInt32(s);
    }

    public int GetInt32(string name)
    {
      return GetInt32();
    }

    public long GetInt64(string name)
    {
      return XmlConvert.ToInt64(_xmlReader.ReadElementString());
    }

    public float GetSingle()
    {
      return XmlConvert.ToSingle(_xmlReader.ReadElementString());
    }

    public float GetSingle(string name)
    {
      return XmlConvert.ToSingle(_xmlReader.ReadElementString());
    }

    public double GetDouble()
    {
      return XmlConvert.ToDouble(_xmlReader.ReadElementString());
    }

    public double? GetNullableDouble(string name)
    {
      string s = _xmlReader.ReadElementString();
      if (string.IsNullOrEmpty(s))
        return null;
      else
        return XmlConvert.ToDouble(s);
    }

    public DateTime GetDateTime(string name)
    {
      return _xmlReader.ReadElementContentAsDateTime();
    }

    public TimeSpan GetTimeSpan(string name)
    {
      return XmlConvert.ToTimeSpan(_xmlReader.ReadElementString());
    }

    public double GetDouble(string name)
    {
      return XmlConvert.ToDouble(_xmlReader.ReadElementString());
    }

    public string GetString()
    {
      return _xmlReader.ReadElementString();
    }

    public string GetString(string name)
    {
      return GetString();
    }

    public System.IO.MemoryStream GetMemoryStream(string name)
    {
      int length = XmlConvert.ToInt32(_xmlReader["Length"]);
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

    public object GetEnum(string name, System.Type type)
    {
      string val = _xmlReader.ReadElementString(name);
      return System.Enum.Parse(type, val);
    }

    public T? GetNullableEnum<T>(string name) where T : struct
    {
      string val = _xmlReader.ReadElementString(name);
      if (string.IsNullOrEmpty(val))
        return default(System.Nullable<T>);
      else
        return (T)System.Enum.Parse(typeof(T), val);
    }

    public string GetNodeContent()
    {
      return _xmlReader.ReadString();
    }

    public int GetInt32Attribute(string name)
    {
      return XmlConvert.ToInt32(_xmlReader[name]);
    }

    public string GetStringAttribute(string name)
    {
      return _xmlReader[name];
    }

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

    public int OpenArray()
    {
      int count = XmlConvert.ToInt32(_xmlReader["Count"]);

      if (count > 0)
        _xmlReader.ReadStartElement();

      return count;
    }

    public int OpenArray(string name)
    {
      int count = XmlConvert.ToInt32(_xmlReader["Count"]);

      if (count > 0)
        _xmlReader.ReadStartElement();

      return count;
    }

    public void CloseArray(int count)
    {
      if (count > 0)
        _xmlReader.ReadEndElement();
      else
        _xmlReader.Read();
    }

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

    public void GetArray(string name, out double[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new double[count];
      GetArray(val, count);
    }

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

    public void GetArray(string name, out int[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new int[count];
      GetArray(val, count);
    }

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

    public void GetArray(string name, out string[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new string[count];
      GetArray(val, count);
    }

    public void GetArray(string[] val, int count)
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

    public void OpenElement()
    {
      _xmlReader.ReadStartElement();
    }

    public void CloseElement()
    {
      _xmlReader.ReadEndElement();
    }

    public string GetNodeName()
    {
      return _xmlReader.LocalName;
    }

    public object GetValue(string name, object parentobject)
    {
      return GetValue(parentobject);
    }

    public object GetValue(object parentobject)
    {
      string type = _xmlReader.GetAttribute("Type");

      if (null != type)
      {
        if ("UndefinedValue" == type)
        {
          _xmlReader.Read();
          return null;
        }

        // Get the surrogate for this type
        IXmlSerializationSurrogate surr = _surrogateSelector.GetSurrogate(type);
        if (null == surr)
        {
          throw new ApplicationException(string.Format("Unable to find XmlSurrogate for type {0}!", type));
        }
        else
        {
          bool bNotEmpty = !_xmlReader.IsEmptyElement;
          // System.Diagnostics.Trace.WriteLine(string.Format("Xml val {0}, type {1}, empty:{2}",m_Reader.Name,type,bNotEmpty));

          if (bNotEmpty)
            _xmlReader.ReadStartElement();  // note: this must now be done by  in the deserialization code

          object retvalue = surr.Deserialize(null, this, parentobject);

          if (bNotEmpty)
            _xmlReader.ReadEndElement();
          else
            _xmlReader.Read();

          return retvalue;
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
    public object GetValueOrOuterXml(string name, object parentobject, out bool returnValueIsOuterXml)
    {
      returnValueIsOuterXml = false;
      string type = _xmlReader.GetAttribute("Type");

      if (null != type)
      {
        if ("UndefinedValue" == type)
        {
          _xmlReader.Read();
          return null;
        }

        // Get the surrogate for this type
        IXmlSerializationSurrogate surr = _surrogateSelector.GetSurrogate(type);
        if (null == surr)
        {
          returnValueIsOuterXml = true;
          return _xmlReader.ReadOuterXml();
        }
        else
        {
          bool bNotEmpty = !_xmlReader.IsEmptyElement;
          // System.Diagnostics.Trace.WriteLine(string.Format("Xml val {0}, type {1}, empty:{2}",m_Reader.Name,type,bNotEmpty));

          if (bNotEmpty)
            _xmlReader.ReadStartElement();  // note: this must now be done by  in the deserialization code

          object retvalue = surr.Deserialize(null, this, parentobject);

          if (bNotEmpty)
            _xmlReader.ReadEndElement();
          else
            _xmlReader.Read();

          return retvalue;
        }
      }
      else
      {
        throw new ApplicationException(string.Format("Unable to deserialize element at line {0}, position {1}, Type attribute missing!", _xmlReader.LineNumber, _xmlReader.LinePosition));
      }
    }



    public void GetBaseValueEmbedded(object instance, System.Type basetype, object parent)
    {
      if ("BaseType" == CurrentElementName)
      {
        string basetypestring = _xmlReader.ReadElementString();
        IXmlSerializationSurrogate ss = _surrogateSelector.GetSurrogate(basetypestring);
        if (null == ss)
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
        ss.Deserialize(instance, this, parent);
      }
      else
      {
        IXmlSerializationSurrogate ss = _surrogateSelector.GetSurrogate(basetype);
        if (null == ss)
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
        ss.Deserialize(instance, this, parent);
      }
    }

    /// <summary>Deserializes the embedded base type.</summary>
    /// <param name="instance">The instance of the object to deserialize.</param>
    /// <param name="fullyQualifiedBaseTypeName">Fully qualified base type name. It is the short name of the assembly, comma, the full type name, comma, and the version. The string must not contain whitespaces. Example: 'AltaxoBase,Altaxo.Main.DocumentPath,0'.</param>
    /// <param name="parent">The parent object of the current object to deserialize.</param>
    public object GetBaseValueEmbedded(object instance, string fullyQualifiedBaseTypeName, object parent)
    {
      object obj;
      if ("BaseType" == CurrentElementName)
      {
        string basetypestring = _xmlReader.ReadElementString();
        IXmlSerializationSurrogate ss = _surrogateSelector.GetSurrogate(basetypestring);
        if (null == ss)
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", fullyQualifiedBaseTypeName));
        obj = ss.Deserialize(instance, this, parent);
      }
      else
      {
        IXmlSerializationSurrogate ss = _surrogateSelector.GetSurrogate(fullyQualifiedBaseTypeName);
        if (null == ss)
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", fullyQualifiedBaseTypeName));
        obj = ss.Deserialize(instance, this, parent);
      }
      return obj;
    }

    public void GetBaseValueStandalone(string name, object instance, System.Type basetype, object parent)
    {
      IXmlSerializationSurrogate ss = _surrogateSelector.GetSurrogate(basetype);
      if (null == ss)
        throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
      else
      {
        _xmlReader.ReadStartElement(); // note: this must now be done by  in the deserialization code
        ss.Deserialize(instance, this, parent);
        _xmlReader.ReadEndElement();
      }
    }

    public string GetElementAsOuterXml(string name)
    {
      return _xmlReader.ReadOuterXml();
    }

    #endregion IXmlSerializationInfo Members
  }
}
