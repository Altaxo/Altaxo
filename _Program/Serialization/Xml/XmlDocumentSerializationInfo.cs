using System;
using System.Xml;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// Summary description for XmlSerializationInfo.
	/// </summary>
	public class XmlDocumentSerializationInfo : IXmlSerializationInfo
	{
		XmlSurrogateSelector m_SurrogateSelector;
		XmlDocument m_Doc;

		XmlElement		m_CurrentNode;
		XmlElement		m_PreviousNode;
		System.Collections.Stack m_NodeStack;
		XmlArrayEncoding m_DefaultArrayEncoding = XmlArrayEncoding.Xml;


		public XmlDocument Doc
		{
			get { return m_Doc; }
		}

		public XmlDocumentSerializationInfo() 
			:
			this(new XmlDocument())
		{
		}

		public XmlDocumentSerializationInfo(XmlDocument doc)
		{
			m_Doc = doc;
			m_SurrogateSelector = new XmlSurrogateSelector();
			m_SurrogateSelector.TraceLoadedAssembliesForSurrogates();
			m_NodeStack = new System.Collections.Stack();

			if(m_Doc.ChildNodes.Count>0)
				m_CurrentNode = (XmlElement)m_Doc.FirstChild;
		}
	
		public XmlArrayEncoding DefaultArrayEncoding
		{
			get { return m_DefaultArrayEncoding;}
			set {	m_DefaultArrayEncoding = value;	}
		}

		public void PushNodeStack()
		{
			m_NodeStack.Push(m_PreviousNode);
			m_PreviousNode = m_CurrentNode;
			
		}

		public void PopNodeStack()
		{
			m_CurrentNode = m_PreviousNode;
			m_PreviousNode = (XmlElement)m_NodeStack.Pop();
		}

		public void FinishCurrentNode()
		{
			if(null!=m_PreviousNode)
				m_PreviousNode.AppendChild(m_CurrentNode);
			else
				m_Doc.AppendChild(m_CurrentNode);

			PopNodeStack();
		}


		public void AddValue(string name, int val)
		{
			XmlElement ele = m_Doc.CreateElement(name);
			ele.InnerText = val.ToString();
			m_CurrentNode.AppendChild(ele);
		}
		public void AddValue(string name, string val)
		{
			XmlElement ele = m_Doc.CreateElement(name);
			ele.InnerText = val;
			m_CurrentNode.AppendChild(ele);
		}
		public void AddAttributeValue(string name, int val)
		{
			XmlAttribute att = m_Doc.CreateAttribute(name);
			att.InnerText = XmlConvert.ToString(val);
			m_CurrentNode.Attributes.Append(att);
		}

		public int GetInt32()
		{
			int ret = int.Parse(m_CurrentNode.InnerText);
			m_CurrentNode = (XmlElement)m_CurrentNode.NextSibling;
			return ret;
		}

		public int GetInt32(string name) 
		{
			return GetInt32();
		}

		public string GetString()
		{
			string ret = m_CurrentNode.InnerText;
			m_CurrentNode = (XmlElement)m_CurrentNode.NextSibling;
			return ret;
		}

		public string GetString(string name)
		{
			return GetString();
		}

		public int GetInt32Attribute(string name)
		{
			XmlAttribute att = m_CurrentNode.Attributes[name];
			return XmlConvert.ToInt32(att.InnerText);
		}

		public double GetDouble()
		{
			double d = double.Parse(m_CurrentNode.InnerText,System.Globalization.NumberStyles.Float,System.Globalization.NumberFormatInfo.InvariantInfo);
			m_CurrentNode = (XmlElement)m_CurrentNode.NextSibling;
			return d;
		}

		public void AddArray(string name, double[] val, int count)
		{
		}

		public void GetArray(double[] val, int count)
		{
		}

		public void CreateElement(string name)
		{
			PushNodeStack();
			m_CurrentNode =  m_Doc.CreateElement(name);
			
		}

		public void OpenInnerContent()
		{
			PushNodeStack();
			m_CurrentNode = (XmlElement)m_CurrentNode.FirstChild;
		}

		public void OpenElement()
		{
			PushNodeStack();
			m_CurrentNode = (XmlElement)m_CurrentNode.FirstChild;
		}

		public void CloseElement()
		{
			PopNodeStack();
		}

		public void CommitElement()
		{
			FinishCurrentNode();
		}

		public int NumberOfCurrentNodes()
		{
			return m_CurrentNode.ParentNode.ChildNodes.Count;
		}

		public void AddValue(string name, double val)
		{
			XmlElement ele = m_Doc.CreateElement(name);
			ele.InnerText = val.ToString("R",System.Globalization.NumberFormatInfo.InvariantInfo);
			m_CurrentNode.AppendChild(ele);
		}

		public void AddValue(string name, object o)
		{
			IXmlSerializationSurrogate ss = m_SurrogateSelector.GetSurrogate(o.GetType());
			if(null==ss)
				throw new ArgumentException(string.Format("Type {0} has no XmlSerializationSurrogate to get serialized",o.GetType()));
			else
			{
				PushNodeStack();
				m_CurrentNode  = m_Doc.CreateElement(name);
				XmlAttribute typeattr = m_Doc.CreateAttribute("Type");
				typeattr.InnerText = m_SurrogateSelector.GetFullyQualifiedTypeName(o.GetType());
				m_CurrentNode.Attributes.Append(typeattr);

				ss.Serialize(o,this);
				FinishCurrentNode();
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


		public object GetValue(string name, object parentobject)
		{
			return GetValue(parentobject);
		}
		public object GetValue(object parentobject)
		{
			if(m_CurrentNode.HasAttribute("Type"))
			{
				string type = m_CurrentNode.GetAttribute("Type");
				// Get the surrogate for this type
				IXmlSerializationSurrogate surr = m_SurrogateSelector.GetSurrogate(type);
				if(null==surr)
					throw new ApplicationException(string.Format("Unable to find XmlSurrogate for type {0}!",type));
				else
				{
					PushNodeStack();
					m_CurrentNode = (XmlElement)m_CurrentNode.FirstChild;
					object retvalue =  surr.Deserialize(null,this,parentobject);
					PopNodeStack();
					m_CurrentNode = (XmlElement)m_CurrentNode.NextSibling;
					return retvalue;
				}
			}
			else
			{
				throw new ApplicationException(string.Format("Unable to deserialize element {0}, Type attribute missing!",m_CurrentNode.Name));
			}
		}

		#region not implemented yet

		public void CreateArray(string name, int count)
		{
		}

		public void CommitArray()
		{
		}

		public int OpenArray()// get Number of Array elements
		{
			return 0;
		}
		public int OpenInnerContentAsArray()
		{
			return OpenArray();
		}

		public void CloseArray(int count)
		{
		}

		public object DeserializationInstance { get { return null; } }


		public void AddBaseValueStandalone(string name, object o, System.Type basetype)
		{
		}
	
		public void GetBaseValueEmbedded(object instance, System.Type basetype, object parent)
		{
		}
		public void GetBaseValueStandalone(string name, object instance, System.Type basetype, object parent)
		{
		}

		public void AddArray(string name, DateTime[] val, int count) {}
		public void AddArray(string name, string[] val, int count) {}

		public void GetArray(string[] val, int count) {}
		public void GetArray(DateTime[] val, int count) {}
		
		public void AddValue(string name, bool val) {}
		public bool GetBoolean() { return false; }
		public bool GetBoolean(string name) { return false; }
		public double GetDouble(string name) { return 0; }
			public float GetSingle(string name) { return 0; }
			public float GetSingle() { return 0; }
		public void AddValue(string name, float val) {}
		#endregion
	}
}
