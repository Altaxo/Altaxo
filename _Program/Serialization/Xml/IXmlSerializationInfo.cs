using System;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// Summary description for IXmlSerializationInfo.
	/// </summary>
	public interface IXmlSerializationInfo
	{


	
		void AddAttributeValue(string name, int val);
		
		bool GetBoolean();
		bool GetBoolean(string name);
		void AddValue(string name, bool val);

		int GetInt32();
		int GetInt32(string name);
		void AddValue(string name, int val);

		float GetSingle();
		float GetSingle(string name);
		void AddValue(string name, float val);

		double GetDouble();
		double GetDouble(string name);
		void AddValue(string name, double val);

		string GetString();
		string GetString(string name);
		void AddValue(string name, string val);
		

		int GetInt32Attribute(string name);

	
	
		void CreateArray(string name, int count);
		void CommitArray();
		int OpenArray(); // get Number of Array elements
		void CloseArray(int count);

		void AddArray(string name, double[] val, int count);
		void AddArray(string name, DateTime[] val, int count);
		void AddArray(string name, string[] val, int count);

		void GetArray(double[] val, int count);
		void GetArray(DateTime[] val, int count);
		void GetArray(string[] val, int count);


		void OpenElement();
		void CloseElement();
		
		void CreateElement(string name);
		void CommitElement();

		void AddValue(string name, object o);

		void AddBaseValueEmbedded(object o, System.Type basetype);
		void AddBaseValueStandalone(string name, object o, System.Type basetype);
		object GetValue(object parent);
		object GetValue(string name, object parent);

		void GetBaseValueEmbedded(object instance, System.Type basetype, object parent);
		void GetBaseValueStandalone(string name, object instance, System.Type basetype, object parent);

		XmlArrayEncoding DefaultArrayEncoding		{	get; set;		}

		/// <summary>
		/// Returns the object instance that is currently in deserialization process
		/// </summary>
		object DeserializationInstance { get; }
	}
}
