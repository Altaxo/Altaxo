using System;

namespace Altaxo.Serialization.Xml
{
	public interface IXmlDeserializationInfo
	{
		bool GetBoolean();
		bool GetBoolean(string name);

		int GetInt32();
		int GetInt32(string name);

		float GetSingle();
		float GetSingle(string name);

		double GetDouble();
		double GetDouble(string name);

		string GetString();
		string GetString(string name);

		// object GetEnum(string name, System.Type type); // not used, see remarks on serialization
		

		int GetInt32Attribute(string name);
	
		int OpenArray(); // get Number of Array elements
		void CloseArray(int count);


		void GetArray(double[] val, int count);
		void GetArray(DateTime[] val, int count);
		void GetArray(string[] val, int count);


		void OpenElement();
		void CloseElement();
		

		object GetValue(object parent);
		object GetValue(string name, object parent);

		void GetBaseValueEmbedded(object instance, System.Type basetype, object parent);
		void GetBaseValueStandalone(string name, object instance, System.Type basetype, object parent);


		/// <summary>
		/// This event is called if the deserialization process of all objects is finished and
		/// the deserialized objects are sorted into the document. Then the application should
		/// call AllFinished, which fires this event. The purpose of this event is to 
		/// resolve the references in the deserialized objects. This resolving process can be successfully
		/// done only if the objects are put in the right places in the document, so that
		/// the document paths can be resolved to the right objects.
		/// </summary>
		event XmlDeserializationCallbackEventHandler DeserializationFinished;
	}
}
