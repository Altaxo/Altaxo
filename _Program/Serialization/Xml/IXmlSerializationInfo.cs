using System;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// Summary description for IXmlSerializationInfo.
	/// </summary>
	public interface IXmlSerializationInfo
	{


		void AddValue(string name, int val);
		void AddAttributeValue(string name, int val);
		int GetInt32();
		int GetInt32(string name);

		string GetString();
		string GetString(string name);

		int GetInt32Attribute(string name);

		void AddValue(string name, double val);
		void AddValue(string name, string val);
		void CreateArray(string name, int count);
		void CommitArray();
		int OpenArray(); // get Number of Array elements
		void CloseArray(int count);

		void AddArray(string name, double[] val, int count);
		void GetArray(double[] val, int count);
		double GetDouble();


		void OpenElement();
		void CloseElement();
		
		void CreateElement(string name);
		void CommitElement();

		void AddValue(string name, object o);

		void AddBaseValue(object o);


		object GetValue(object parent);
		void GetBaseValue(object instance, object parent);

		XmlArrayEncoding DefaultArrayEncoding		{	get; set;		}

		/// <summary>
		/// Returns the object instance that is currently in deserialization process
		/// </summary>
		object DeserializationInstance { get; }
	}
}
