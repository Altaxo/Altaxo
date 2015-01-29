#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
	/// Interface for a data object to put data on the clipboard.
	/// </summary>
	public interface IClipboardSetDataObject
	{
		void SetImage(System.Drawing.Image image);

		void SetFileDropList(System.Collections.Specialized.StringCollection filePaths);

		void SetData(string format, object data);

		void SetData(Type format, object data);

		void SetCommaSeparatedValues(string text);
	}

	/// <summary>
	/// Interface for a data object to get data from the clipboard.
	/// </summary>
	public interface IClipboardGetDataObject
	{
		string[] GetFormats();

		bool GetDataPresent(string format);

		bool GetDataPresent(System.Type type);

		object GetData(string format);

		object GetData(System.Type type);

		bool ContainsFileDropList();

		System.Collections.Specialized.StringCollection GetFileDropList();

		bool ContainsImage();

		System.Drawing.Image GetImage();
	}
}