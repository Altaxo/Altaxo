using System;
using System.Xml;

namespace Altaxo.Serialization.Xml
{

	/// <summary>
	/// This interface has to be implemented by all XmlSerializationSurrogates.
	/// </summary>
	public interface IXmlSerializationSurrogate
	{
		/// <summary>
		/// Serialize the object <code>o</code> into xml.
		/// </summary>
		/// <param name="o">The object to serialize.</param>
		/// <param name="info">The serialization info used to serialize the object.</param>
		void Serialize(object o, IXmlSerializationInfo info);

		/// <summary>
		/// Deserialize the object from xml.
		/// </summary>
		/// <param name="info">The serialization info used to deserialize the stream.</param>
		/// <returns>The object which is deserialized.</returns>
		object Deserialize(IXmlSerializationInfo info, object parentobject);
		
	}
}
