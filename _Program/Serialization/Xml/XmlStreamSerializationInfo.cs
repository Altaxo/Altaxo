using System;
using System.Xml;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// Summary description for XmlStreamSerializationInfo.
	/// </summary>
	public class XmlStreamSerializationInfo : IXmlSerializationInfo
	{
		bool m_bWriting;

		XmlTextWriter m_Writer;
		XmlTextReader m_Reader;

		XmlSurrogateSelector m_SurrogateSelector;
		System.Text.StringBuilder m_StringBuilder = new System.Text.StringBuilder();

		byte[] m_Buffer;
		int    m_BufferSize;

		XmlArrayEncoding m_DefaultArrayEncoding = XmlArrayEncoding.Xml;

		private const int _size_of_double=8;
		private const int _size_of_DateTime=8;

		private object m_DeserializationInstance;

		public XmlStreamSerializationInfo(System.IO.Stream stream, bool bWriting)
		{
			m_bWriting = bWriting;
			if(bWriting)
			{
				m_Writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);
				m_Writer.WriteStartDocument();
			}
			else
			{
				m_Reader = new XmlTextReader(stream);
				m_Reader.MoveToContent();
			}

			m_BufferSize=16384;
			m_Buffer = new byte[m_BufferSize];
			m_SurrogateSelector = new XmlSurrogateSelector();
			m_SurrogateSelector.TraceLoadedAssembliesForSurrogates();
		}

		public void Finish()
		{
			if(m_bWriting)
			{
				m_Writer.WriteEndDocument();
				m_Writer.Flush();
				m_Writer.Close();
				m_Writer=null;
			}
			else
			{
				m_Reader.Close();
				m_Reader=null;
			}
		}

		#region IXmlSerializationInfo Members



		public XmlArrayEncoding DefaultArrayEncoding
		{
			get { return m_DefaultArrayEncoding;}
			set {	m_DefaultArrayEncoding = value;	}
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

		public int GetInt32Attribute(string name)
		{
			return XmlConvert.ToInt32(m_Reader[name]);
		}

		public void AddValue(string name, float val)
		{
			m_Writer.WriteElementString(name, XmlConvert.ToString(val));
		}
		public void AddValue(string name, double val)
		{
			m_Writer.WriteElementString(name, XmlConvert.ToString(val));
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

		public void AddArray(string name, DateTime[] val, int count)
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
							val[i] = XmlConvert.ToDouble(m_Reader.ReadElementString());
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
							val[i] = XmlConvert.ToDateTime(m_Reader.ReadElementString());
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

		public void CreateElement(string name)
		{
			m_Writer.WriteStartElement(name);
		}

		public void CommitElement()
		{
			m_Writer.WriteEndElement();
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


		public object GetValue(string name, object parentobject)
		{
			return GetValue(parentobject);
		}
		public object GetValue(object parentobject)
		{
			string type = m_Reader.GetAttribute("Type");
			
			if(null!=type)
			{
				
				// Get the surrogate for this type
				IXmlSerializationSurrogate surr = m_SurrogateSelector.GetSurrogate(type);
				if(null==surr)
					throw new ApplicationException(string.Format("Unable to find XmlSurrogate for type {0}!",type));
				else
				{
					m_Reader.ReadStartElement();
					object retvalue =  surr.Deserialize(null,this,parentobject);
					m_Reader.ReadEndElement();
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
			IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(basetype);
			if(null==ss)
				throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized",basetype));
			else
			{
				ss.Deserialize(instance,this,parent);
			}		
		}
		public void GetBaseValueStandalone(string name, object instance, System.Type basetype, object parent)
		{
			IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(basetype);
			if(null==ss)
				throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized",basetype));
			else
			{
				m_Reader.ReadStartElement();
				ss.Deserialize(instance,this,parent);
				m_Reader.ReadEndElement();
			}		
		}


		public object DeserializationInstance
		{
			get { return m_DeserializationInstance; }
		}

		#endregion

	
	}
}
