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
using System.Xml;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Summary description for XmlStreamSerializationInfo.
  /// </summary>
  public class XmlStreamSerializationInfo : IXmlSerializationInfo
  {
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

    public XmlStreamSerializationInfo()
    {
      _bufferSize = 16384;
      _buffer = new byte[_bufferSize];
      _surrogateSelector = new XmlSurrogateSelector();
      _surrogateSelector.TraceLoadedAssembliesForSurrogates();
      _writer = _nullWriter;
    }

    public void BeginWriting(System.IO.Stream stream)
    {
      _writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
      _isWriterCreatedHere = true;
      _writer.WriteStartDocument();
    }

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

    public void SetProperty(string propertyname, string? propertyvalue)
    {
      if (_properties.ContainsKey(propertyname))
        _properties[propertyname] = propertyvalue;
      else
        _properties.Add(propertyname, propertyvalue);
    }

    public string? GetProperty(string propertyname)
    {
      return _properties[propertyname];
    }

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

    public XmlArrayEncoding DefaultArrayEncoding
    {
      get { return m_DefaultArrayEncoding; }
      set { m_DefaultArrayEncoding = value; }
    }

    public void AddValue(string name, bool val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, bool? val)
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, XmlConvert.ToString(val.Value));
    }

    public void AddValue(string name, char val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, int val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, int? val)
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, XmlConvert.ToString((int)val));
    }

    public void AddValue(string name, long val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, string? val)
    {
      _writer.WriteElementString(name, val);
    }


    public void AddAttributeValue(string name, int val)
    {
      _writer.WriteAttributeString(name, XmlConvert.ToString(val));
    }

    public void AddAttributeValue(string name, string val)
    {
      _writer.WriteAttributeString(name, val);
    }

    public void AddValue(string name, float val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, double val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, double? val)
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, XmlConvert.ToString((double)val));
    }

    public void AddValue(string name, DateTime val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val, XmlDateTimeSerializationMode.RoundtripKind));
    }

    public void AddValue(string name, TimeSpan val)
    {
      _writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, System.IO.MemoryStream stream)
    {
      _writer.WriteStartElement(name);
      if (stream is null)
        _writer.WriteAttributeString("Length", XmlConvert.ToString(0));
      else
      {
        byte[] buffer = stream.ToArray();
        _writer.WriteAttributeString("Length", XmlConvert.ToString(buffer.Length));
        _writer.WriteBase64(buffer, 0, buffer.Length);
      }
      _writer.WriteEndElement();
    }

    public void AddEnum(string name, System.Enum val)
    {
      _writer.WriteElementString(name, val.ToString());
    }

    public void AddNullableEnum<T>(string name, T? val) where T : struct
    {
      if (val is null)
        _writer.WriteElementString(name, string.Empty);
      else
        _writer.WriteElementString(name, val.Value.ToString());
    }

    public void SetNodeContent(string nodeContent)
    {
      _writer.WriteString(nodeContent);
    }

    public void CreateArray(string name, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException("count has to be >=0");

      _writer.WriteStartElement(name);
      _writer.WriteAttributeString("Count", XmlConvert.ToString(count));
    }

    public void CommitArray()
    {
      _writer.WriteEndElement(); // Node "name"
    }

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

    public void AddArray(string name, double[] val, int count)
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
          AddArrayOfPrimitiveType(name, val, count, _size_of_double, m_DefaultArrayEncoding);
        }
      } // count>0
      CommitArray();
    }

    public void AddArray(string name, int[] val, int count)
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
          AddArrayOfPrimitiveType(name, val, count, _size_of_int, m_DefaultArrayEncoding);
        }
      } // count>0
      CommitArray();
    }

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

    public void AddArray(string name, string?[] val, int count)
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

    public void AddArray(string name, object[] val, int count)
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

    public void AddArrayOfNullableElements(string name, object?[] val, int count)
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

    public void CreateElement(string name)
    {
      _writer.WriteStartElement(name);
    }

    public void CommitElement()
    {
      _writer.WriteEndElement();
    }

    public bool IsSerializable(object? o)
    {
      return o is null || _surrogateSelector.GetSurrogate(o.GetType()) is not null;
    }

    public bool IsSerializableType(System.Type type)
    {
      return _surrogateSelector.GetSurrogate(type) is not null;
    }

    public void AddValue(string name, object o)
    {
#if !NONULLSTRICTCHECK
      AddValueOrNull(name, o ?? throw new ArgumentNullException(nameof(o)));
#else
      AddValueOrNull(name, o);
#endif
    }


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
