using System;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// Summary description for IXmlSerializationInfo.
	/// </summary>
	public interface IXmlSerializationInfo
	{
		void SetProperty(string propertyname, string propertyvalue);
		string GetProperty(string propertyname);

		void AddAttributeValue(string name, int val);
		
		void AddValue(string name, bool val);

		void AddValue(string name, int val);

		void AddValue(string name, float val);

		void AddValue(string name, double val);

		void AddValue(string name, string val);

		// void AddValue(string name, System.Enum val); not used since _all_ enums then get serialized by this function (also those who have a serialization surrogate)
	
		void CreateArray(string name, int count);

		void CommitArray();

		void AddArray(string name, double[] val, int count);
		void AddArray(string name, DateTime[] val, int count);
		void AddArray(string name, string[] val, int count);
	
		void CreateElement(string name);
		void CommitElement();

		void AddValue(string name, object o);

		void AddBaseValueEmbedded(object o, System.Type basetype);
		void AddBaseValueStandalone(string name, object o, System.Type basetype);


		XmlArrayEncoding DefaultArrayEncoding		{	get; set;		}
	}
}
