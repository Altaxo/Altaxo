using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization
{
	/// <summary>
	/// Altaxo's interface to a data object, independent on any Gui....
	/// </summary>
	public interface IDataObject
	{
		/// <summary>
		/// Retrieves a data object in a specified format, optionally converting the data to the specified format.
		/// </summary>
		/// <param name="format">A string that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <param name="autoConvert">true to attempt to automatically convert the data to the specified format; false for no data format conversion. If this parameter is false, the method returns data in the specified format if available, or null if the data is not available in the specified format.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		object GetData(string format, bool autoConvert);

		/// <summary>
		/// Retrieves a data object in a specified format; the data format is specified by a <see cref="T:System.Type" /> object.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> object that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		object GetData(Type format);

		/// <summary>
		/// Retrieves a data object in a specified format; the data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		object GetData(string format);

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format. A Boolean flag indicates whether to check if the data can be converted to the specified format, if it is not available in that format.
		/// </summary>
		/// <param name="format">A string that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="autoConvert">false to only check for the specified format; true to also check whether or not data stored in this data object can be converted to the specified format.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		bool GetDataPresent(string format, bool autoConvert);

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format. The data format is specified by a <see cref="T:System.Type" /> object.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		bool GetDataPresent(Type format);

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format; the data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		bool GetDataPresent(string format);

		/// <summary>
		/// Returns a list of all formats that the data in this data object is stored in. A Boolean flag indicates whether or not to also include formats that the data can be automatically converted to.
		/// </summary>
		/// <param name="autoConvert">true to retrieve all formats that data stored in this data object is stored in, or can be converted to; false to retrieve only formats that data stored in this data object is stored in (excluding formats that the data is not stored in, but can be automatically converted to).</param>
		/// <returns>
		/// An array of strings, with each string specifying the name of a format supported by this data object.
		/// </returns>
		string[] GetFormats(bool autoConvert);

		/// <summary>
		/// Returns a list of all formats that the data in this data object is stored in, or can be converted to.
		/// </summary>
		/// <returns>
		/// An array of strings, with each string specifying the name of a format supported by this data object.
		/// </returns>
		string[] GetFormats();

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. This overload includes a Boolean flag to indicate whether the data may be converted to another format on retrieval.
		/// </summary>
		/// <param name="format">A string that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		/// <param name="autoConvert">true to allow the data to be converted to another format on retrieval; false to prohibit the data from being converted to another format on retrieval.</param>
		void SetData(string format, object data, bool autoConvert);

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. The data format is specified by a <see cref="T:System.Type" /> class.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		void SetData(Type format, object data);

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. The data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		void SetData(string format, object data);

		/// <summary>
		/// Stores the specified data in this data object, automatically converting the data format from the source object type.
		/// </summary>
		/// <param name="data">The data to store in this data object.</param>
		void SetData(object data);
	}
}