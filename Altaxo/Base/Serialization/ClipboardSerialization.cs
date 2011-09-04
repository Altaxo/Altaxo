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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization
{
	public static class ClipboardSerialization
	{
		public static void PutObjectToClipboard(string clipBoardFormat, object toSerialize)
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			var stb = new System.Text.StringBuilder();
			var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
			info.BeginWriting(stb);

			info.AddValue("Object", toSerialize);

			info.EndWriting();
			dao.SetData(clipBoardFormat, stb.ToString());
			Current.Gui.SetClipboardDataObject(dao, true);
		}


		public static object GetObjectFromClipboard(string clipBoardFormat)
		{
			var dao = Current.Gui.OpenClipboardDataObject();
			string s = (string)dao.GetData(clipBoardFormat);
			if (!string.IsNullOrEmpty(s))
			{
				var info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
				info.BeginReading(s);
				object o = info.GetValue("Object", null);
				info.EndReading();

				return o;
			}
			else
			{
				return null;
			}
		}
	}
}
