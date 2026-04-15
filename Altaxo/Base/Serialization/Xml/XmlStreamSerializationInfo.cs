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
using System.Linq;
using System.Text;
using System.Xml;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Provides XML serialization support for writing values and objects to an XML stream.
  /// </summary>
  public class XmlStreamSerializationInfo : IXmlSerializationInfo
  {
    /// <summary>
    /// Property key that enables indented XML output.
    /// </summary>
    public const string UseXmlIndentation = "UseXmlIndentation";
    private static readonly XmlWriter _nullWriter = new XmlTextWriter(System.IO.Stream.Null, System.Text.Encoding.UTF8);

    private XmlWriter _writer;

    /// <summary>Designates, whether the XmlWriter is created in this instance or not. If it is created here, it will be closed when calling <see cref="EndWriting"/>. Otherwise, it will only be detached.</summary>
    private bool _isWriterCreatedHere;

    private XmlSurrogateSelector _surrogateSelector;
    private System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder();

    private byte[] _buffer;
    private int _bufferSize;

    private XmlArrayEncoding m_DefaultArrayEncoding = XmlArrayEncoding.Xml;

    private System.Collections.Specialized.StringDictionary _properties = new System.Collections.Specialized.StringDictionary();

    private const int _size_of_int = 4;
    private const int _size_of_float = 4;
    private const int _size_of_double = 8;
    private const int _size_of_DateTime = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlStreamSerializationInfo"/> class.
    /// </summary>
    public XmlStreamSerializationInfo()
    {
      _bufferSize = 16384;
      _buffer = new byte[_bufferSize];
      _surrogateSelector = new XmlSurrogateSelector();
      _surrogateSelector.TraceLoadedAssembliesForSurrogates();
      _writer = _nullWriter;
    }

    /// <summary>
    /// Begins writing XML to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public void BeginWriting(System.IO.Stream stream)
    {
      if (string.IsNullOrEmpty(_properties[Altaxo.Serialization.Xml.XmlStreamSerializationInfo.UseXmlIndentation]))
      {
        _writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
      }
      else
      {
        var settings = new XmlWriterSettings
        {
          Encoding = Encoding.UTF8,
          NewLineOnAttributes = false,
          Indent = true
        };
        _writer = XmlWriter.Create(stream, settings);
      }

      _isWriterCreatedHere = true;
      _writer.WriteStartDocument();
    }

    /// <summary>
    /// Begins writing XML to the specified string builder.
    /// </summary>
    /// <param name="stb">The string builder that receives the generated XML.</param>
    public void BeginWriting(System.Text.StringBuilder stb)
    {
      _writer = XmlWriter.Create(stb);
      _isWriterCreatedHere = true;
      _writer.WriteStartDocument();
    }

    /// <summary>
    /// Begins the writing using an already created xmlWriter. WriteStartDocument is not called here, because we assume that the open XmlWriter has already done that.
    /// </summary>
    /// <param name="xmlWriter">The XML writer.</param>
    public void BeginWriting(XmlWriter xmlWriter)
    {
      _writer = xmlWriter;
      _isWriterCreatedHere = false;
    }

    /// <summary>
    /// Ends XML writing and flushes or detaches the underlying writer.
    /// </summary>
    public void EndWriting()
    {
      if (_isWriterCreatedHere)
      {
        _writer?.WriteEndDocument();
        _writer?.Flush();
        _writer = _nullWriter;
      }
      else
      {
        _writer = _nullWriter;
      }
    }

    /// <summary>
    /// Sets a serializer property.
    /// </summary>
    /// <param name="propertyname">The property name.</param>
    /// <param name="propertyvalue">The property value.</param>
    public void SetProperty(string propertyname, string? propertyvalue)
    {
      if (_properties.ContainsKey(propertyname))
        _properties[propertyname] = propertyvalue;
      else
        _properties.Add(propertyname, propertyvalue);
    }

    /// <summary>
    /// Gets a serializer property.
    /// </summary>
    /// <param name="propertyname">The property name.</param>
    /// <returns>The property value, or <c>null</c> if the property is not set.</returns>
    public string? GetProperty(string propertyname)
    {
      return _properties[propertyname];
    }

    /// <summary>
    /// Saves the current property value and then sets a new value.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="propertyValue">The new property value.</param>
    /// <returns>The previous property value, or <c>null</c> if the property was not set.</returns>
    public string? SaveAndSetProperty(string propertyName, string? propertyValue)
    {
      var result = _properties[propertyName];
      SetProperty(propertyName, propertyValue);
      return result;
    }

    /// <summary>
    /// Clears the property dictionary. Useful if the serialization info should be used to serialize multiple values.
    /// If you clear the properties before the serialization of each value, the serialization behaves as if each value is
    /// serialized independent of each other.
    /// </summary>
    public void ClearProperties()
    {
      _properties.Clear();
    }

    #region IXmlSerializationInfo Members

    /// <inheritdoc/>
    public XmlArrayEncoding DefaultArrayEncoding
    {
      get { return m_DefaultArrayEncoding; }
      set { m_DefaultArrayEncoding = value; }
    }

    /// <inheritdoc/>
    public void AddValue(string name, bool val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, bool? val)
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, XmlConvert.ToString(val.Value));
    }

    /// <inheritdoc/>
    public void AddValue(string name, char val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, int val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, int? val)
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, XmlConvert.ToString((int)val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, long val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, string? val)
    {
      _writer.WriteElementString(name, val);
    }


    /// <inheritdoc/>
    public void AddAttributeValue(string name, int val)
    {
      _writer.WriteAttributeString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddAttributeValue(string name, string val)
    {
      _writer.WriteAttributeString(name, val);
    }

    /// <inheritdoc/>
    public void AddValue(string name, float val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, double val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, double? val)
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, XmlConvert.ToString((double)val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, DateTime val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val, XmlDateTimeSerializationMode.RoundtripKind));
    }

    /// <inheritdoc/>
    public void AddValue(string name, TimeSpan val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    /// <inheritdoc/>
    public void AddValue(string name, System.IO.MemoryStream val)
    {
      _writer.WriteStartElement(name);
      if (val is null)
        _writer.WriteAttributeString("Length", XmlConvert.ToString(0));
      else
      {
        byte[] buffer = val.ToArray();
        _writer.WriteAttributeString("Length", XmlConvert.ToString(buffer.Length));
        _writer.WriteBase64(buffer, 0, buffer.Length);
      }
      _writer.WriteEndElement();
    }

    /// <inheritdoc/>
    public void AddEnum(string name, System.Enum val)
    {
      _writer.WriteElementString(name, val.ToString());
    }

    /// <inheritdoc/>
    public void AddNullableEnum<T>(string name, T? val) where T : struct
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, val.Value.ToString());
    }

    /// <inheritdoc/>
    public void SetNodeContent(string nodeContent)
    {
      _writer.WriteString(nodeContent);
    }

    /// <inheritdoc/>
    public void CreateArray(string name, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count has to be >=0");

      _writer.WriteStartElement(name);
      _writer.WriteAttributeString("Count", XmlConvert.ToString(count));
    }

    /// <inheritdoc/>
    public void CommitArray()
    {
      _writer.WriteEndElement(); // Node "name"
    }

    /// <inheritdoc/>
    public void AddArray(string name, float[] val, int count)
    {
      CreateArray(name, count);

      if (count > 0)
      {
        if (m_DefaultArrayEncoding == XmlArrayEncoding.Xml)
        {
          _writer.WriteAttributeString("Encoding", "Xml");
          _writer.WriteStartElement("e");
          _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0]));
          for (int i = 1; i < count; i++)
          {
            _writer.WriteRaw("</e><e>");
            _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i]));
          }
          _writer.WriteEndElement(); // node "e"
        }
        else
        {
          AddArrayOfPrimitiveType(name, val, count, _size_of_float, m_DefaultArrayEncoding);
        }
      } // count>0
      CommitArray();
    }

    /// <inheritdoc/>
    public void AddArray(string name, IReadOnlyList<double> val, int count)
    {
      CreateArray(name, count);

      if (count > 0)
      {
        if (m_DefaultArrayEncoding == XmlArrayEncoding.Xml)
        {
          _writer.WriteAttributeString("Encoding", "Xml");
          _writer.WriteStartElement("e");
          _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0]));
          for (int i = 1; i < count; i++)
          {
            _writer.WriteRaw("</e><e>");
            _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i]));
          }
          _writer.WriteEndElement(); // node "e"
        }
        else
        {
          AddArrayOfPrimitiveType(name, val is Array valarr ? valarr : val.ToArray(), count, _size_of_double, m_DefaultArrayEncoding);
        }
      } // count>0
      CommitArray();
    }

    /// <inheritdoc/>
    public void AddArray(string name, IReadOnlyList<int> val, int count)
    {
      CreateArray(name, count);

      if (count > 0)
      {
        if (m_DefaultArrayEncoding == XmlArrayEncoding.Xml)
        {
          _writer.WriteAttributeString("Encoding", "Xml");
          _writer.WriteStartElement("e");
          _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0]));
          for (int i = 1; i < count; i++)
          {
            _writer.WriteRaw("</e><e>");
            _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i]));
          }
          _writer.WriteEndElement(); // node "e"
        }
        else if (val is int[] arr)
        {
          AddArrayOfPrimitiveType(name, arr, count, _size_of_int, m_DefaultArrayEncoding);
        }
        else
        {
          throw new NotImplementedException("Serialization of primitive type not yet supported from a read-only list.");
        }
      } // count>0
      CommitArray();
    }

    /// <inheritdoc/>
    public void AddArray(string name, DateTime[] val, int count)
    {
      CreateArray(name, count);

      if (count > 0)
      {
        if (m_DefaultArrayEncoding == XmlArrayEncoding.Xml)
        {
          _writer.WriteAttributeString("Encoding", "Xml");
          _writer.WriteStartElement("e");
          _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0], System.Xml.XmlDateTimeSerializationMode.RoundtripKind));
          for (int i = 1; i < count; i++)
          {
            _writer.WriteRaw("</e><e>");
            _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i], System.Xml.XmlDateTimeSerializationMode.RoundtripKind));
          }
          _writer.WriteEndElement(); // node "e"
        }
        else
        {
          AddArrayOfPrimitiveType(name, val, count, _size_of_DateTime, m_DefaultArrayEncoding);
        }
      } // count>0
      CommitArray();
    }

    /// <inheritdoc/>
    public void AddArray(string name, IReadOnlyList<string?> val, int count)
    {
      CreateArray(name, count);

      if (count > 0)
      {
        for (int i = 0; i < count; i++)
        {
          _writer.WriteElementString("e", val[i]);
        }
      } // count>0
      CommitArray();
    }

    /// <inheritdoc/>
    public void AddArray(string name, System.Collections.BitArray val, System.Collections.BitArray cond, int count)
    {
      CreateArray(name, count);


      if (count > 0)
      {
        if (m_DefaultArrayEncoding == XmlArrayEncoding.Xml)
        {
          _writer.WriteAttributeString("Encoding", "Xml");
          _writer.WriteStartElement("e");
          _writer.WriteRaw(cond[0] ? System.Xml.XmlConvert.ToString(val[0]) : string.Empty);
          for (int i = 1; i < count; i++)
          {
            _writer.WriteRaw("</e><e>");
            if (cond[i])
            {
              _writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i]));
            }
          }
          _writer.WriteEndElement(); // node "e"
        }
        else
        {
          throw new NotImplementedException();
        }
      } // count>0
      CommitArray();
    }

    /// <summary>
    /// Adds an array of primitive values using the specified XML encoding.
    /// </summary>
    /// <param name="name">The element name.</param>
    /// <param name="val">The array containing the values.</param>
    /// <param name="count">The number of elements to write.</param>
    /// <param name="sizeofelement">The size of each element in bytes.</param>
    /// <param name="encoding">The XML array encoding to use.</param>
    public void AddArrayOfPrimitiveType(string name, System.Array val, int count, int sizeofelement, XmlArrayEncoding encoding)
    {
      switch (encoding)
      {
        case XmlArrayEncoding.Base64:
          {
            _writer.WriteAttributeString("Encoding", "Base64");
            _writer.WriteStartElement("Base64");
            int remainingBytes = count * sizeofelement;
            for (int pos = 0; pos < remainingBytes;)
            {
              int bytesToWrite = Math.Min(_bufferSize, remainingBytes - pos);
              System.Buffer.BlockCopy(val, pos, _buffer, 0, bytesToWrite);
              _writer.WriteBase64(_buffer, 0, bytesToWrite);
              pos += bytesToWrite;
            }
            _writer.WriteEndElement();
          }
          break;

        case XmlArrayEncoding.BinHex:
          {
            _writer.WriteAttributeString("Encoding", "BinHex");
            _writer.WriteStartElement("BinHex");
            int remainingBytes = count * sizeofelement;
            for (int pos = 0; pos < remainingBytes;)
            {
              int bytesToWrite = Math.Min(_bufferSize, remainingBytes - pos);
              System.Buffer.BlockCopy(val, pos, _buffer, 0, bytesToWrite);
              _writer.WriteBinHex(_buffer, 0, bytesToWrite);
              pos += bytesToWrite;
            }
            _writer.WriteEndElement();
          }
          break;

        case XmlArrayEncoding.Xml:
          throw new ArgumentException("This function must not be called with encoding=" + encoding.ToString());
        default:
          throw new ApplicationException("Unknown encoding value: " + encoding.ToString());
      } // end switch
    }


    /// <inheritdoc/>
    public void AddArray(string name, IReadOnlyList<object> val, int count)
    {
      CreateArray(name, count);

      if (count > 0)
      {
        for (int i = 0; i < count; i++)
        {
          AddValue("e", val[i]);
        }
      } // count>0
      CommitArray();
    }

    /// <inheritdoc/>
    public void AddArrayOfNullableElements(string name, IReadOnlyList<object?> val, int count)
    {
      CreateArray(name, count);

      if (count > 0)
      {
        for (int i = 0; i < count; i++)
        {
          AddValueOrNull("e", val[i]);
        }
      } // count>0
      CommitArray();
    }

    /// <inheritdoc/>
    public void CreateElement(string name)
    {
      _writer.WriteStartElement(name);
    }

    /// <inheritdoc/>
    public void CommitElement()
    {
      _writer.WriteEndElement();
    }

    /// <inheritdoc/>
    public bool IsSerializable(object? o)
    {
      return o is null || _surrogateSelector.GetSurrogate(o.GetType()) is not null;
    }

    /// <summary>
    /// Determines whether the specified type can be serialized by the current surrogate selector.
    /// </summary>
    /// <param name="type">The type to test.</param>
    /// <returns><c>true</c> if the type is serializable; otherwise, <c>false</c>.</returns>
    public bool IsSerializableType(System.Type type)
    {
      return _surrogateSelector.GetSurrogate(type) is not null;
    }

    /// <inheritdoc/>
    public void AddValue(string name, object o)
    {
#if !NONULLSTRICTCHECK
      AddValueOrNull(name, o ?? throw new ArgumentNullException(nameof(o)));
#else
      AddValueOrNull(name, o);
#endif
    }


    /// <inheritdoc/>
    public void AddValueOrNull(string name, object? o)
    {
      if (o is not null)
      {
        if (_surrogateSelector.GetSurrogate(o.GetType()) is { } ss)
        {
          _writer.WriteStartElement(name);
          _writer.WriteAttributeString("Type", _surrogateSelector.GetFullyQualifiedTypeName(o.GetType()));
          ss.Serialize(o, this);
          _writer.WriteEndElement();
        }
        else
        {
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", o.GetType()));
        }
      }
      else // o is null, we add only an empty element
      {
        _writer.WriteStartElement(name);
        _writer.WriteAttributeString("Type", "UndefinedValue");
        _writer.WriteEndElement();
      }
    }

    /// <inheritdoc/>
    public void AddBaseValueEmbedded(object o, System.Type basetype)
    {
      if (_surrogateSelector.GetSurrogate(basetype) is { } ss)
      {
        _writer.WriteElementString("BaseType", _surrogateSelector.GetFullyQualifiedTypeName(basetype)); // included since 2006-06-20 (Rev. 471)
        ss.Serialize(o, this);
      }
      else
      {
        throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
      }
    }

    /// <inheritdoc/>
    public void AddBaseValueStandalone(string name, object o, System.Type basetype)
    {
      if (_surrogateSelector.GetSurrogate(basetype) is { } ss)
      {
        _writer.WriteStartElement(name);
        _writer.WriteAttributeString("Type", _surrogateSelector.GetFullyQualifiedTypeName(basetype));
        ss.Serialize(o, this);
        _writer.WriteEndElement();
      }
      else
      {
        throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
      }
    }

    /// <summary>
    /// Writes a raw Xml string. This can be used for instance if the string to write was lazy loaded from another Xml document.
    /// </summary>
    /// <param name="rawXmlString">The raw XML string.</param>
    public void WriteRaw(string rawXmlString)
    {
      _writer.WriteRaw(rawXmlString);
    }

    #endregion IXmlSerializationInfo Members
  }
}
