using System;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// This function is used to call back Deserialization surrogates after finishing deserialization
	/// </summary>
	public delegate void XmlDeserializationCallbackEventHandler(IXmlDeserializationInfo info, object documentRoot);

}
