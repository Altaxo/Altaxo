using System;

namespace Altaxo.Serialization.Xml
{
	/// <summary>
	/// Defines the encoding used to store Arrays of primitive types
	/// </summary>
	public enum XmlArrayEncoding
	{
		/// <summary>
		/// Use a xml element for every array element.
		/// </summary>
		Xml,

		/// <summary>
		/// Store the array data in binary form using Base64 encoding.
		/// </summary>
		Base64,

		/// <summary>
		/// Store th array data in binary form using BinHex encoding.
		/// </summary>
		BinHex
	}
}
