using System;

namespace Altaxo.Serialization.Xml
{

	/// <summary>
	/// Used to point to the target type for which this class provides a serialization surrogate.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=false)]
	public class XmlSerializationCodeForAttribute : Attribute
	{
		protected	int m_Version;
		protected System.Type m_SerializationType;
		
		/// <summary>
		/// Constructor. The class this attribute is applied provides a serialization surrogate for the type <code>serializationtype</code>, version <code>version.</code>.
		/// </summary>
		/// <param name="serializationtype">The type this class provides a surrogate for.</param>
		/// <param name="version">The version of the class for which this surrogate is intended.</param>
		public XmlSerializationCodeForAttribute(Type serializationtype, int version )
		{
			m_Version = version;
			m_SerializationType = serializationtype; 
		}
	
		/// <summary>
		/// returns the version of the class, for which the surrogate is intended
		/// </summary>
		public int Version
		{
			get { return m_Version; }
		}
		/// <summary>
		///Returns the target type for which the class this attribute is applied for is the serialization surrogate.
		/// </summary>
		public System.Type SerializationType
		{
			get { return m_SerializationType; }
		}
	} // end class SerializationCodeForAttribute


}
