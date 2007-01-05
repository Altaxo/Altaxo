#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Xml;

namespace Altaxo.Serialization.Xml
{
  /// <summary>
  /// Summary description for XmlStreamSerializationInfo.
  /// </summary>
  public class XmlStreamSerializationInfo : IXmlSerializationInfo
  {
    XmlTextWriter m_Writer;

    XmlSurrogateSelector m_SurrogateSelector;
    System.Text.StringBuilder m_StringBuilder = new System.Text.StringBuilder();

    byte[] m_Buffer;
    int    m_BufferSize;

    XmlArrayEncoding m_DefaultArrayEncoding = XmlArrayEncoding.Xml;

    private System.Collections.Specialized.StringDictionary m_Properties = new System.Collections.Specialized.StringDictionary();

    private const int _size_of_int=4;
    private const int _size_of_float=4;
    private const int _size_of_double=8;
    private const int _size_of_DateTime=8;

    

    public XmlStreamSerializationInfo()
    {
      m_BufferSize=16384;
      m_Buffer = new byte[m_BufferSize];
      m_SurrogateSelector = new XmlSurrogateSelector();
      m_SurrogateSelector.TraceLoadedAssembliesForSurrogates();
    }

    public void BeginWriting(System.IO.Stream stream)
    {
      m_Writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
      m_Writer.WriteStartDocument();
    }

    public void EndWriting()
    {
      m_Writer.WriteEndDocument();
      m_Writer.Flush();       
      m_Writer=null;
    }

    public void SetProperty(string propertyname, string propertyvalue)
    {
      if(m_Properties.ContainsKey(propertyname))
        m_Properties[propertyname] = propertyvalue;
      else
        m_Properties.Add(propertyname,propertyvalue);
    }

    public string GetProperty(string propertyname)
    {
      return m_Properties[propertyname];
    }

    #region IXmlSerializationInfo Members

    public XmlArrayEncoding DefaultArrayEncoding
    {
      get { return m_DefaultArrayEncoding;}
      set { m_DefaultArrayEncoding = value; }
    }

    public void AddValue(string name, bool val)
    {
      m_Writer.WriteElementString(name,XmlConvert.ToString(val));
    }
    public void AddValue(string name, int val)
    {
      m_Writer.WriteElementString(name,XmlConvert.ToString(val));
    }
    public void AddValue(string name, string val)
    {
      m_Writer.WriteElementString(name,val);
    }
    public void AddAttributeValue(string name, int val)
    {
      m_Writer.WriteAttributeString(name,XmlConvert.ToString(val));
    }

    public void AddValue(string name, float val)
    {
      m_Writer.WriteElementString(name, XmlConvert.ToString(val));
    }
    public void AddValue(string name, double val)
    {
      m_Writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, DateTime val)
    {
      m_Writer.WriteElementString(name, XmlConvert.ToString(val, XmlDateTimeSerializationMode.Utc));
    }

    public void AddValue(string name, TimeSpan val)
    {
      m_Writer.WriteElementString(name, XmlConvert.ToString(val));
    }

    public void AddValue(string name, System.IO.MemoryStream stream)
    {
      m_Writer.WriteStartElement(name);
      if (stream == null)
        m_Writer.WriteAttributeString("Length", XmlConvert.ToString(0));
      else
      {
        byte[] buffer = stream.ToArray();
        m_Writer.WriteAttributeString("Length", XmlConvert.ToString(buffer.Length));
        m_Writer.WriteBase64(buffer, 0, buffer.Length);
      }
      m_Writer.WriteEndElement();
    }
    
    public void AddEnum(string name, System.Enum val)
    {
      m_Writer.WriteElementString(name, val.ToString());
    }
    
    public void SetNodeContent(string nodeContent)
    {
      m_Writer.WriteString(nodeContent);
    }

    public void CreateArray(string name, int count)
    {
      m_Writer.WriteStartElement(name);
      m_Writer.WriteAttributeString("Count", XmlConvert.ToString(count));
    }

    public void CommitArray()
    {
      m_Writer.WriteEndElement(); // Node "name"
    }


    public void AddArray(string name, float[] val, int count)
    {
      this.CreateArray(name,count);

      if(count>0)
      {
        if(m_DefaultArrayEncoding==XmlArrayEncoding.Xml)
        {
          m_Writer.WriteAttributeString("Encoding","Xml");
          m_Writer.WriteStartElement("e");
          m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0]));
          for(int i=1;i<count;i++)
          {
            m_Writer.WriteRaw("</e><e>");
            m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i]));
          }
          m_Writer.WriteEndElement(); // node "e"
        
        }
        else
        {
          AddArrayOfPrimitiveType(name,val,count,_size_of_float,m_DefaultArrayEncoding);
        }
      } // count>0
      this.CommitArray();
    }

    public void AddArray(string name, double[] val, int count)
    {
      this.CreateArray(name,count);

      if(count>0)
      {
        if(m_DefaultArrayEncoding==XmlArrayEncoding.Xml)
        {
          m_Writer.WriteAttributeString("Encoding","Xml");
          m_Writer.WriteStartElement("e");
          m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0]));
          for(int i=1;i<count;i++)
          {
            m_Writer.WriteRaw("</e><e>");
            m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i]));
          }
          m_Writer.WriteEndElement(); // node "e"
        
        }
        else
        {
          AddArrayOfPrimitiveType(name,val,count,_size_of_double,m_DefaultArrayEncoding);
        }
      } // count>0
      this.CommitArray();
    }

    public void AddArray(string name, int[] val, int count)
    {
      this.CreateArray(name,count);

      if(count>0)
      {
        if(m_DefaultArrayEncoding==XmlArrayEncoding.Xml)
        {
          m_Writer.WriteAttributeString("Encoding","Xml");
          m_Writer.WriteStartElement("e");
          m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0]));
          for(int i=1;i<count;i++)
          {
            m_Writer.WriteRaw("</e><e>");
            m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i]));
          }
          m_Writer.WriteEndElement(); // node "e"
        
        }
        else
        {
          AddArrayOfPrimitiveType(name,val,count,_size_of_int,m_DefaultArrayEncoding);
        }
      } // count>0
      this.CommitArray();
    }


    public void AddArray(string name, DateTime[] val, int count)
    {
      this.CreateArray(name,count);

      if(count>0)
      {
        if(m_DefaultArrayEncoding==XmlArrayEncoding.Xml)
        {
          m_Writer.WriteAttributeString("Encoding","Xml");
          m_Writer.WriteStartElement("e");
          m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[0],System.Xml.XmlDateTimeSerializationMode.Utc));
          for(int i=1;i<count;i++)
          {
            m_Writer.WriteRaw("</e><e>");
            m_Writer.WriteRaw(System.Xml.XmlConvert.ToString(val[i],System.Xml.XmlDateTimeSerializationMode.Utc));
          }
          m_Writer.WriteEndElement(); // node "e"
        
        }
        else
        {
          AddArrayOfPrimitiveType(name,val,count,_size_of_DateTime,m_DefaultArrayEncoding);
        }
      } // count>0
      this.CommitArray();
    }

    public void AddArray(string name, string[] val, int count)
    {
      this.CreateArray(name,count);

      if(count>0)
      {
        for(int i=0;i<count;i++)
        {
          m_Writer.WriteElementString("e",val[i]);
        }
      
      } // count>0
      this.CommitArray();
    }

    public void AddArrayOfPrimitiveType(string name, System.Array val, int count, int sizeofelement, XmlArrayEncoding encoding)
    {
      switch(encoding)
      {
        case XmlArrayEncoding.Base64:
        {
          m_Writer.WriteAttributeString("Encoding","Base64");
          m_Writer.WriteStartElement("Base64");
          int remainingBytes = count * sizeofelement;
          for(int pos=0;pos<remainingBytes;)
          {
            int bytesToWrite = Math.Min(m_BufferSize,remainingBytes-pos);
            System.Buffer.BlockCopy(val,pos,m_Buffer,0,bytesToWrite);
            m_Writer.WriteBase64(m_Buffer,0,bytesToWrite);
            pos+=bytesToWrite;
          }
          m_Writer.WriteEndElement();
        }
          break;
        case XmlArrayEncoding.BinHex:
        {
          m_Writer.WriteAttributeString("Encoding","BinHex");
          m_Writer.WriteStartElement("BinHex");
          int remainingBytes = count * sizeofelement;
          for(int pos=0;pos<remainingBytes;)
          {
            int bytesToWrite = Math.Min(m_BufferSize,remainingBytes-pos);
            System.Buffer.BlockCopy(val,pos,m_Buffer,0,bytesToWrite);
            m_Writer.WriteBinHex(m_Buffer,0,bytesToWrite);
            pos+=bytesToWrite;
          }
          m_Writer.WriteEndElement();
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
      this.CreateArray(name,count);

      if(count>0)
      {
        for(int i=0;i<count;i++)
        {
          this.AddValue("e",val[i]);
        }
      
      } // count>0
      this.CommitArray();
    }


    public void CreateElement(string name)
    {
      m_Writer.WriteStartElement(name);
    }

    public void CommitElement()
    {
      m_Writer.WriteEndElement();
    }

    public bool IsSerializable(object o)
    {
      return null!=m_SurrogateSelector.GetSurrogate(o.GetType());
    }

    public void AddValue(string name, object o)
    {
      if(null!=o)
      {
        IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(o.GetType());
        if(null==ss)
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized",o.GetType()));
        else
        {
          m_Writer.WriteStartElement(name);
          m_Writer.WriteAttributeString("Type",m_SurrogateSelector.GetFullyQualifiedTypeName(o.GetType()));
          ss.Serialize(o,this);
          m_Writer.WriteEndElement();
        } 
      }
      else // o is null, we add only an empty element
      {
        m_Writer.WriteStartElement(name);
        m_Writer.WriteAttributeString("Type","UndefinedValue");
        m_Writer.WriteEndElement();
      }
    }

    public void AddBaseValueEmbedded(object o, System.Type basetype)
    {
      IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(basetype);
      if(null==ss)
        throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized",basetype));
      else
      {
        m_Writer.WriteElementString("BaseType", m_SurrogateSelector.GetFullyQualifiedTypeName(basetype));
        ss.Serialize(o,this);
      }   
    }
    public void AddBaseValueStandalone(string name, object o, System.Type basetype)
    {
      IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(basetype);
      if(null==ss)
        throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized",basetype));
      else
      {
        m_Writer.WriteStartElement(name);
        m_Writer.WriteAttributeString("Type",m_SurrogateSelector.GetFullyQualifiedTypeName(basetype));
        ss.Serialize(o,this);
        m_Writer.WriteEndElement();
      }   
    }

    #endregion

  
  }
}
