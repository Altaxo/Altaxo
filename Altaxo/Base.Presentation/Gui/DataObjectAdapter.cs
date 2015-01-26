#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Straightforward translation from an <see cref="T:Altaxo.Serialization.IDataObject"/> instance to a <see cref="T:System.Windows.IDataObject"/> instance.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(true)]
	public class DataObjectAdapterAltaxoToWpf : System.Windows.IDataObject
	{
		private Altaxo.Serialization.Clipboard.IDataObject o;

		/// <summary>
		/// Returns a Wpf data object from an Altaxo data object. If the Altaxo data object is just an adapter from a Wpf data object,
		/// this Wpf data object is returned. If this is not the case, an <see cref="DataObjectAdapterAltaxoToWpf"/> is created, wrapping the provided data object,
		/// and this adapter is returned.
		/// </summary>
		/// <param name="altaxoDataObject">The Altaxo data object.</param>
		/// <returns>A Wpf data object.</returns>
		public static System.Windows.IDataObject FromAltaxoDataObject(Altaxo.Serialization.Clipboard.IDataObject altaxoDataObject)
		{
			var adapter = altaxoDataObject as DataObjectAdapterWpfToAltaxo;
			if (null != adapter) // is it an adapter from Altaxo to Wpf
				return adapter.DataObjectWpf; // if yes, then we use the underlying Altaxo data object directly, without wrapping it in another adapter
			else
				return new DataObjectAdapterAltaxoToWpf(altaxoDataObject); // else we have to wrap it in an adapter
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataObjectAdapterAltaxoToWpf"/> class, using an Altaxo data object.
		/// </summary>
		/// <param name="dao">The Altaxo data object.</param>
		/// <exception cref="System.ArgumentNullException">dao</exception>
		public DataObjectAdapterAltaxoToWpf(Altaxo.Serialization.Clipboard.IDataObject dao)
		{
			if (null == dao)
				throw new ArgumentNullException("dao");

			o = dao;
		}

		/// <summary>
		/// Gets the underlying Altaxo data object.
		/// </summary>
		/// <value>
		/// The Altaxo data object.
		/// </value>
		public Altaxo.Serialization.Clipboard.IDataObject DataObjectAltaxo { get { return o; } }

		/// <summary>
		/// Retrieves a data object in a specified format, optionally converting the data to the specified format.
		/// </summary>
		/// <param name="format">A string that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <param name="autoConvert">true to attempt to automatically convert the data to the specified format; false for no data format conversion. If this parameter is false, the method returns data in the specified format if available, or null if the data is not available in the specified format.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		public object GetData(string format, bool autoConvert)
		{
			return o.GetData(format, autoConvert);
		}

		/// <summary>
		/// Retrieves a data object in a specified format; the data format is specified by a <see cref="T:System.Type" /> object.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> object that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		public object GetData(Type format)
		{
			return o.GetData(format);
		}

		/// <summary>
		/// Retrieves a data object in a specified format; the data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		public object GetData(string format)
		{
			return o.GetData(format);
		}

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format. A Boolean flag indicates whether to check if the data can be converted to the specified format, if it is not available in that format.
		/// </summary>
		/// <param name="format">A string that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="autoConvert">false to only check for the specified format; true to also check whether or not data stored in this data object can be converted to the specified format.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(string format, bool autoConvert)
		{
			return o.GetDataPresent(format, autoConvert);
		}

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format. The data format is specified by a <see cref="T:System.Type" /> object.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(Type format)
		{
			return o.GetDataPresent(format);
		}

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format; the data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(string format)
		{
			return o.GetDataPresent(format);
		}

		/// <summary>
		/// Returns a list of all formats that the data in this data object is stored in. A Boolean flag indicates whether or not to also include formats that the data can be automatically converted to.
		/// </summary>
		/// <param name="autoConvert">true to retrieve all formats that data stored in this data object is stored in, or can be converted to; false to retrieve only formats that data stored in this data object is stored in (excluding formats that the data is not stored in, but can be automatically converted to).</param>
		/// <returns>
		/// An array of strings, with each string specifying the name of a format supported by this data object.
		/// </returns>
		public string[] GetFormats(bool autoConvert)
		{
			return o.GetFormats(autoConvert);
		}

		/// <summary>
		/// Returns a list of all formats that the data in this data object is stored in, or can be converted to.
		/// </summary>
		/// <returns>
		/// An array of strings, with each string specifying the name of a format supported by this data object.
		/// </returns>
		public string[] GetFormats()
		{
			return o.GetFormats();
		}

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. This overload includes a Boolean flag to indicate whether the data may be converted to another format on retrieval.
		/// </summary>
		/// <param name="format">A string that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		/// <param name="autoConvert">true to allow the data to be converted to another format on retrieval; false to prohibit the data from being converted to another format on retrieval.</param>
		public void SetData(string format, object data, bool autoConvert)
		{
			o.SetData(format, data, autoConvert);
		}

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. The data format is specified by a <see cref="T:System.Type" /> class.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		public void SetData(Type format, object data)
		{
			o.SetData(format, data);
		}

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. The data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		public void SetData(string format, object data)
		{
			o.SetData(format, data);
		}

		/// <summary>
		/// Stores the specified data in this data object, automatically converting the data format from the source object type.
		/// </summary>
		/// <param name="data">The data to store in this data object.</param>
		public void SetData(object data)
		{
			o.SetData(data);
		}
	}

	/// <summary>
	/// Straightforward translation from a <see cref="T:System.Windows.IDataObject"/> to an <see cref="T:Altaxo.Serialization.IDataObject"/> instance.
	/// </summary>
	public class DataObjectAdapterWpfToAltaxo : Altaxo.Serialization.Clipboard.IDataObject
	{
		/// <summary>
		/// Returns an Altaxo data object from a Windows (Wpf) data object. If the Windows data object is just an adapter from an Altaxo data object,
		/// this Altaxo data object is returned. If this is not the case, an <see cref="DataObjectAdapterWpfToAltaxo"/> is created, wrapping the provided data object,
		/// and this adapter is returned.
		/// </summary>
		/// <param name="wpfDo">The WPF data object.</param>
		/// <returns>An Altaxo data object.</returns>
		public static Altaxo.Serialization.Clipboard.IDataObject FromWpfDataObject(System.Windows.IDataObject wpfDo)
		{
			var adapter = wpfDo as DataObjectAdapterAltaxoToWpf;
			if (null != adapter) // is it an adapter from Altaxo to Wpf
				return adapter.DataObjectAltaxo; // if yes, then we use the underlying Altaxo data object directly, without wrapping it in another adapter
			else
				return new DataObjectAdapterWpfToAltaxo(wpfDo); // else we have to wrap it in an adapter
		}

		private System.Windows.IDataObject o;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataObjectAdapterWpfToAltaxo"/> class, using an System.Windows data object.
		/// </summary>
		/// <param name="dao">The System.Windows data object to wrap.</param>
		/// <exception cref="System.ArgumentNullException">dao</exception>
		public DataObjectAdapterWpfToAltaxo(System.Windows.IDataObject dao)
		{
			if (null == dao)
				throw new ArgumentNullException("dao");

			o = dao;
		}

		/// <summary>
		/// Gets the underlying Wpf data object.
		/// </summary>
		/// <value>
		/// The Wpf data object.
		/// </value>
		public System.Windows.IDataObject DataObjectWpf { get { return o; } }

		/// <summary>
		/// Retrieves a data object in a specified format, optionally converting the data to the specified format.
		/// </summary>
		/// <param name="format">A string that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <param name="autoConvert">true to attempt to automatically convert the data to the specified format; false for no data format conversion. If this parameter is false, the method returns data in the specified format if available, or null if the data is not available in the specified format.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		public object GetData(string format, bool autoConvert)
		{
			return o.GetData(format, autoConvert);
		}

		/// <summary>
		/// Retrieves a data object in a specified format; the data format is specified by a <see cref="T:System.Type" /> object.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> object that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		public object GetData(Type format)
		{
			return o.GetData(format);
		}

		/// <summary>
		/// Retrieves a data object in a specified format; the data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to retrieve the data as. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// A data object with the data in the specified format, or null if the data is not available in the specified format.
		/// </returns>
		public object GetData(string format)
		{
			return o.GetData(format);
		}

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format. A Boolean flag indicates whether to check if the data can be converted to the specified format, if it is not available in that format.
		/// </summary>
		/// <param name="format">A string that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="autoConvert">false to only check for the specified format; true to also check whether or not data stored in this data object can be converted to the specified format.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(string format, bool autoConvert)
		{
			return o.GetDataPresent(format, autoConvert);
		}

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format. The data format is specified by a <see cref="T:System.Type" /> object.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(Type format)
		{
			return o.GetDataPresent(format);
		}

		/// <summary>
		/// Checks to see whether the data is available in, or can be converted to, a specified format; the data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to check for. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <returns>
		/// true if the data is in, or can be converted to, the specified format; otherwise, false.
		/// </returns>
		public bool GetDataPresent(string format)
		{
			return o.GetDataPresent(format);
		}

		/// <summary>
		/// Returns a list of all formats that the data in this data object is stored in. A Boolean flag indicates whether or not to also include formats that the data can be automatically converted to.
		/// </summary>
		/// <param name="autoConvert">true to retrieve all formats that data stored in this data object is stored in, or can be converted to; false to retrieve only formats that data stored in this data object is stored in (excluding formats that the data is not stored in, but can be automatically converted to).</param>
		/// <returns>
		/// An array of strings, with each string specifying the name of a format supported by this data object.
		/// </returns>
		public string[] GetFormats(bool autoConvert)
		{
			return o.GetFormats(autoConvert);
		}

		/// <summary>
		/// Returns a list of all formats that the data in this data object is stored in, or can be converted to.
		/// </summary>
		/// <returns>
		/// An array of strings, with each string specifying the name of a format supported by this data object.
		/// </returns>
		public string[] GetFormats()
		{
			return o.GetFormats();
		}

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. This overload includes a Boolean flag to indicate whether the data may be converted to another format on retrieval.
		/// </summary>
		/// <param name="format">A string that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		/// <param name="autoConvert">true to allow the data to be converted to another format on retrieval; false to prohibit the data from being converted to another format on retrieval.</param>
		public void SetData(string format, object data, bool autoConvert)
		{
			o.SetData(format, data, autoConvert);
		}

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. The data format is specified by a <see cref="T:System.Type" /> class.
		/// </summary>
		/// <param name="format">A <see cref="T:System.Type" /> that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of predefined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		public void SetData(Type format, object data)
		{
			o.SetData(format, data);
		}

		/// <summary>
		/// Stores the specified data in this data object, along with one or more specified data formats. The data format is specified by a string.
		/// </summary>
		/// <param name="format">A string that specifies what format to store the data in. See the <see cref="T:System.Windows.DataFormats" /> class for a set of pre-defined data formats.</param>
		/// <param name="data">The data to store in this data object.</param>
		public void SetData(string format, object data)
		{
			o.SetData(format, data);
		}

		/// <summary>
		/// Stores the specified data in this data object, automatically converting the data format from the source object type.
		/// </summary>
		/// <param name="data">The data to store in this data object.</param>
		public void SetData(object data)
		{
			o.SetData(data);
		}
	}
}