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
  /// Deserializes object from a XML stream.
  /// </summary>
  public class XmlStreamDeserializationInfo : IXmlDeserializationInfo
  {
    XmlTextReader m_Reader;

    XmlSurrogateSelector m_SurrogateSelector;
    System.Text.StringBuilder m_StringBuilder = new System.Text.StringBuilder();

    byte[] m_Buffer;
    int    m_BufferSize;

    private const int _size_of_int=4;
    private const int _size_of_float=4;
    private const int _size_of_double=8;
    private const int _size_of_DateTime=8;

    

    public XmlStreamDeserializationInfo()
    {
      m_BufferSize=16384;
      m_Buffer = new byte[m_BufferSize];
      m_SurrogateSelector = new XmlSurrogateSelector();
      m_SurrogateSelector.TraceLoadedAssembliesForSurrogates();
    }
    
    public void BeginReading(System.IO.Stream stream)
    {
      m_Reader = new XmlTextReader(stream);
      m_Reader.MoveToContent();
    }

    public void EndReading()
    {
      // m_Reader.Close(); Do not close the reader, since the underlying stream is closed too then..., this will not work if reading zip files
      m_Reader=null;
    }

  
    public void AnnounceDeserializationEnd(object documentRoot)
    {
      if(null!= DeserializationFinished)
        DeserializationFinished(this,documentRoot);
    }

    #region IXmlSerializationInfo Members

    public event XmlDeserializationCallbackEventHandler DeserializationFinished;

    /// <summary>Returns the name of the current xml element.</summary>
    public string CurrentElementName
    { 
      get { return m_Reader.LocalName; }
    }

    public bool GetBoolean()
    {
      return XmlConvert.ToBoolean(m_Reader.ReadElementString());
    }
    public bool GetBoolean(string name)
    {
      return XmlConvert.ToBoolean(m_Reader.ReadElementString());
    }
    public int GetInt32()
    {
      return XmlConvert.ToInt32(m_Reader.ReadElementString());
    }
    public int GetInt32(string name)
    {
      return GetInt32();
    }

    public float GetSingle()
    {
      return XmlConvert.ToSingle(m_Reader.ReadElementString());
    }
    public float GetSingle(string name)
    {
      return XmlConvert.ToSingle(m_Reader.ReadElementString());
    }

    public double GetDouble()
    {

      return XmlConvert.ToDouble(m_Reader.ReadElementString());
    }

    public DateTime GetDateTime(string name)
    {
      return m_Reader.ReadElementContentAsDateTime();
    }

    public TimeSpan GetTimeSpan(string name)
    {
      return XmlConvert.ToTimeSpan(m_Reader.ReadElementString());
    }

    public double GetDouble(string name)
    {
      return XmlConvert.ToDouble(m_Reader.ReadElementString());
    }

    public string GetString()
    {
      return m_Reader.ReadElementString();
    }
    public string GetString(string name)
    {
      return GetString();
    }

    public System.IO.MemoryStream GetMemoryStream(string name)
    {
      int length = XmlConvert.ToInt32(m_Reader["Length"]);
      if (length == 0)
      {
        m_Reader.ReadStartElement();
        m_Reader.ReadEndElement();
        return null;
      }
      else
      {
        byte[] buffer = new byte[length];
       
        int readed = m_Reader.ReadElementContentAsBase64(buffer, 0, length);
        if (readed!=length)
          throw new System.FormatException(string.Format("Length of the stream was smaller than denoted in the length attribute in the node header; Expected length: {0}; actual stream length: {1}",length,readed));
        if (0 != m_Reader.ReadElementContentAsBase64(buffer, 0, length)) // this second call should return 0; it is only called to advance the text reader to the next node
          throw new System.FormatException("Length of the stream was greater than denoted in the length attribute in the node header");
       
        return new System.IO.MemoryStream(buffer);
      }
    }
    
    public object GetEnum(string name, System.Type type)
    {
      string val = m_Reader.ReadElementString(name);
      return System.Enum.Parse(type,val);
    }
    
    public string GetNodeContent()
    {
      return m_Reader.ReadString();
    }

    public int GetInt32Attribute(string name)
    {
      return XmlConvert.ToInt32(m_Reader[name]);
    }







    public void GetArrayOfPrimitiveTypeBase64(System.Array val, int count, int sizeOfElement)
    {
      int bytesreaded;
      int pos=0;
      int remainingbytes = count*_size_of_double;
      while(remainingbytes>0 && (0!=(bytesreaded = m_Reader.ReadBase64(m_Buffer, 0, Math.Min(m_BufferSize,remainingbytes)))))
      {

        System.Diagnostics.Debug.Assert(0==bytesreaded%_size_of_double);
        System.Buffer.BlockCopy(m_Buffer,0,val,pos,bytesreaded);
        pos+=bytesreaded;
        remainingbytes-=bytesreaded;
      }
      m_Reader.Read(); // read the rest of the element
    }
    public void GetArrayOfPrimitiveTypeBinHex(System.Array val, int count, int sizeOfElement)
    {
      int bytesreaded;
      int pos=0;
      int remainingbytes = count*_size_of_double;
      while(remainingbytes>0 && (0!=(bytesreaded = m_Reader.ReadBinHex(m_Buffer, 0, Math.Min(m_BufferSize,remainingbytes)))))
      {

        System.Diagnostics.Debug.Assert(0==bytesreaded%_size_of_double);
        System.Buffer.BlockCopy(m_Buffer,0,val,pos,bytesreaded);
        pos+=bytesreaded;
        remainingbytes-=bytesreaded;
      }
      m_Reader.Read(); // read the rest of the element
    }

    public int OpenArray()
    {
      int count = XmlConvert.ToInt32(m_Reader["Count"]);

      if (count > 0)
        m_Reader.ReadStartElement();

      return count;
    }

    public int OpenArray(string name)
    {
      int count = XmlConvert.ToInt32(m_Reader["Count"]);

      if(count>0)
        m_Reader.ReadStartElement();

      return count;
    }
  

    public void CloseArray(int count)
    {
      if(count>0)
        m_Reader.ReadEndElement();
      else
        m_Reader.Read();
    }

    public void GetArray(out float[] val)
    {
      int count = this.OpenArray();
      val = new float[count];
      // Attribute must be readed before ReadStartElement
      if(count>0)
      {
        m_Reader.ReadStartElement(); // read the first inner element

        switch(m_Reader.Name)
        {
          default:
            for(int i=0;i<count;i++)
              val[i] = XmlConvert.ToSingle(m_Reader.ReadElementString());
            break;
          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val,count,_size_of_float);
            break;
          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val,count,_size_of_float);
            break;
        } // end of switch
        m_Reader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        m_Reader.Read();
      }
    }


    public void GetArray(string name, out double[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new double[count];
      GetArray(val,count);
    }

    public void GetArray(double[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if(count>0)
      {
        m_Reader.ReadStartElement(); // read the first inner element

        switch(m_Reader.Name)
        {
          default:
            for(int i=0;i<count;i++)
              val[i] = m_Reader.ReadElementContentAsDouble();
            break;
          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val,count,_size_of_double);
            break;
          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val,count,_size_of_double);
            break;
        } // end of switch
        m_Reader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        m_Reader.Read();
      }
    }


    public void GetArray(string name, out int[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new int[count];
      GetArray(val,count);
    }

    public void GetArray(int[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if(count>0)
      {
        m_Reader.ReadStartElement(); // read the first inner element

        switch(m_Reader.Name)
        {
          default:
            for(int i=0;i<count;i++)
              val[i] = m_Reader.ReadElementContentAsInt();
            break;
          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val,count,_size_of_int);
            break;
          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val,count,_size_of_int);
            break;
        } // end of switch
        m_Reader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        m_Reader.Read();
      }
    }

    public void GetArray(DateTime[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if(count>0)
      {
        m_Reader.ReadStartElement(); // read the first inner element

        switch(m_Reader.Name)
        {
          default:
            for(int i=0;i<count;i++)
              val[i] = m_Reader.ReadElementContentAsDateTime();
            break;
          case "Base64":
            GetArrayOfPrimitiveTypeBase64(val,count,_size_of_DateTime);
            break;
          case "BinHex":
            GetArrayOfPrimitiveTypeBinHex(val,count,_size_of_DateTime);
            break;
        } // end of switch
        m_Reader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        m_Reader.Read();
      }
    }

    
    public void GetArray(string name, out string[] val)
    {
      int count = GetInt32Attribute("Count");
      val = new string[count];
      GetArray(val,count);
    }

    public void GetArray(string[] val, int count)
    {
      // Attribute must be readed before ReadStartElement
      if(count>0)
      {
        m_Reader.ReadStartElement(); // read the first inner element

        for(int i=0;i<count;i++)
          val[i] = m_Reader.ReadElementString();
        m_Reader.ReadEndElement(); // read the outer XmlElement, i.e. "DoubleArray"
      } // if count>0
      else
      {
        m_Reader.Read();
      }
    }


    public void OpenElement()
    {
      m_Reader.ReadStartElement();
    }

    public void CloseElement()
    {
      m_Reader.ReadEndElement();
    }
    
    public string GetNodeName()
    {
      return m_Reader.LocalName;
    }

    public object GetValue(string name, object parentobject)
    {
      return GetValue(parentobject);
    }

    public object GetValue(object parentobject)
    {
      string type = m_Reader.GetAttribute("Type");
     
      if(null!=type)
      {
        if("UndefinedValue"==type)
        {
          m_Reader.Read();
          return null;
        }

        // Get the surrogate for this type
        IXmlSerializationSurrogate surr = m_SurrogateSelector.GetSurrogate(type);
        if(null==surr)
          throw new ApplicationException(string.Format("Unable to find XmlSurrogate for type {0}!",type));
        else
        {
          bool bNotEmpty = !m_Reader.IsEmptyElement;
          // System.Diagnostics.Trace.WriteLine(string.Format("Xml val {0}, type {1}, empty:{2}",m_Reader.Name,type,bNotEmpty));

          
          if(bNotEmpty)
            m_Reader.ReadStartElement();  // note: this must now be done by  in the deserialization code
          
          object retvalue =  surr.Deserialize(null,this,parentobject);
          
          if(bNotEmpty)
            m_Reader.ReadEndElement();
          else
            m_Reader.Read();

          
          return retvalue;
        }
      }
      else
      {
        throw new ApplicationException(string.Format("Unable to deserialize element at line {0}, position {1}, Type attribute missing!",m_Reader.LineNumber,m_Reader.LinePosition));
      }
    }

    public void GetBaseValueEmbedded(object instance, System.Type basetype, object parent )
    {
      if ("BaseType" == CurrentElementName)
      {
        string basetypestring = m_Reader.ReadElementString();
        IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(basetypestring);
        if (null == ss)
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
        ss.Deserialize(instance, this, parent);
      }
      else
      {
        IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(basetype);
        if (null == ss)
          throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized", basetype));
        ss.Deserialize(instance, this, parent);
      }
    }
    public void GetBaseValueStandalone(string name, object instance, System.Type basetype, object parent)
    {
      IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(basetype);
      if(null==ss)
        throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized",basetype));
      else
      {
        m_Reader.ReadStartElement(); // note: this must now be done by  in the deserialization code
        ss.Deserialize(instance,this,parent);
        m_Reader.ReadEndElement();
      }   
    }


    #endregion

  
  }
}
