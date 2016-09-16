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

namespace Altaxo.Serialization.Clipboard
{
	/// <summary>
	/// Supports clipboard operations. The operations are based on Altaxo's XML serialization / deserialization capabilities.
	/// </summary>
	public static class ClipboardSerialization
	{
		/// <summary>Puts an object into the provided dataObject.</summary>
		/// <param name="clipBoardFormat">The clip board format string (is used later on to identify the data on the clipboard).</param>
		/// <param name="toSerialize">Data to put on the clipboard.</param>
		/// <param name="dataObject">The data object to put the data into.</param>
		public static void PutObjectToDataObject(string clipBoardFormat, object toSerialize, Altaxo.Gui.IClipboardSetDataObject dataObject)
		{
			if (null == dataObject)
				throw new ArgumentNullException("dataObject");

			var stb = SerializeToStringBuilder(toSerialize);
			dataObject.SetData(clipBoardFormat, stb.ToString());
		}

		/// <summary>Puts an object to the clipboard.</summary>
		/// <param name="clipBoardFormat">The clip board format string (is used later on to identify the data on the clipboard).</param>
		/// <param name="toSerialize">Data to put on the clipboard.</param>
		public static void PutObjectToClipboard(string clipBoardFormat, object toSerialize)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			PutObjectToDataObject(clipBoardFormat, toSerialize, dao);
			Current.Gui.SetClipboardDataObject(dao, true);
		}

		/// <summary>
		/// Serializes the provided object <paramref name="toSerialize"/> into a new <see cref="T:System.Text.StringBuilder"/>.
		/// </summary>
		/// <param name="toSerialize">The object to serialize.</param>
		/// <returns>A <see cref="T:System.Text.StringBuilder"/> which contains the serialized object.</returns>
		/// <remarks>The object is serialized into a root node named 'Object'.</remarks>
		public static StringBuilder SerializeToStringBuilder(object toSerialize)
		{
			var stb = new System.Text.StringBuilder();
			var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
			info.BeginWriting(stb);

			info.AddValue("Object", toSerialize);

			info.EndWriting();
			return stb;
		}

		/// <summary>Determines whether data in the given clipboard format is available.</summary>
		/// <param name="clipBoardFormat">The clip board format.</param>
		/// <returns><c>True</c> if data in the given clipboard format are available; otherwise, <c>false</c>.</returns>
		public static bool IsClipboardFormatAvailable(string clipBoardFormat)
		{
			var dao = Current.Gui.OpenClipboardDataObject();
			return dao.GetDataPresent(clipBoardFormat);
		}

		/// <summary>Gets an object from the clipboard.</summary>
		/// <param name="clipBoardFormat">The clip board format string.</param>
		/// <returns>The deserialized object that was on the clipboard, or <c>null</c> otherwise.</returns>
		public static object GetObjectFromClipboard(string clipBoardFormat)
		{
			return GetObjectFromClipboard<object>(clipBoardFormat);
		}

		/// <summary>Gets an object of a certain type from the clipboard.</summary>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		/// <param name="clipBoardFormat">The clip board format string.</param>
		/// <returns>The deserialized object. If deserialization was not possible, or the deserialized data was not of the expected type, the default object for type T is returned (default(T)).</returns>
		public static T GetObjectFromClipboard<T>(string clipBoardFormat)
		{
			var dao = Current.Gui.OpenClipboardDataObject();
			string s = (string)dao.GetData(clipBoardFormat);
			if (!string.IsNullOrEmpty(s))
			{
				return DeserializeObjectFromString<T>(s);
			}
			else
			{
				return default(T);
			}
		}

		/// <summary>
		/// Deserializes an object from a string containing Altaxo's XML format.
		/// </summary>
		/// <typeparam name="T">Type of the object that is expected to be deserialized.</typeparam>
		/// <param name="s">The string containing Altaxo's XML format. Is is expected that the root node is named 'Object'.</param>
		/// <returns>The deserialized object, or default(T) if either the object was null or had a type that was not the expected type.</returns>
		public static T DeserializeObjectFromString<T>(string s)
		{
			object readObject = null;
			using (var info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo())
			{
				info.BeginReading(s);
				readObject = info.GetValue("Object", null);
				info.EndReading();
			}

			if ((null != readObject) && (readObject is T))
			{
				return (T)readObject;
			}
			else
			{
				return default(T);
			}
		}
	}
}