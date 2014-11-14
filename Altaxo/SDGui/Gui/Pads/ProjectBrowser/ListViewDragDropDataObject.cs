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

namespace Altaxo.Gui.Pads.ProjectBrowser
{
	public class ListViewDragDropDataObject : Altaxo.Serialization.IDataObject
	{
		public const string Format_ApplicationInstanceGuid = "Altaxo.Current.ApplicationInstanceGuid";
		public const string Format_ProjectFolder = "Altaxo.Gui.Pads.ProjectBrowser.FolderName";
		public const string Format_ItemList = "Altaxo.Gui.Pads.ProjectBrowser.ItemList";
		public const string Format_ItemReferenceList = "Altaxo.Gui.Pads.ProjectBrowser.ItemReferenceList";

		private List<string> _availableFormats;

		/// <summary>
		/// Gets or sets a value indicating whether the item list was rendered.
		/// </summary>
		/// <value>
		/// <c>true</c> if item list was rendered; otherwise, <c>false</c>.
		/// </value>
		public bool ItemListWasRendered { get; protected set; }

		public object GetData(string format, bool autoConvert)
		{
			object result = null;
			switch (format)
			{
				case Format_ProjectFolder:
					result = FolderName;
					break;

				case Format_ApplicationInstanceGuid:
					result = Current.ApplicationInstanceGuid.ToString();
					break;

				case Format_ItemList:
					{
						var items = new Altaxo.Main.Commands.ProjectItemCommands.ProjectItemClipboardList(ItemList, FolderName);
						var stb = Altaxo.Serialization.ClipboardSerialization.SerializeToStringBuilder(items);
						result = stb.ToString();
						ItemListWasRendered = true;
					}
					break;

				case Format_ItemReferenceList:
					{
						var itemReferenceList = new List<object>(ItemList.Select(x => new Altaxo.Main.DocNodeProxy(x)));
						var items = new Altaxo.Main.Commands.ProjectItemCommands.ProjectItemClipboardList(itemReferenceList, FolderName);
						var stb = Altaxo.Serialization.ClipboardSerialization.SerializeToStringBuilder(items);
						result = stb.ToString();
					}
					break;

				default:
					result = null;
					break;
			}

			return result;
		}

		public object GetData(string format)
		{
			return GetData(format, true);
		}

		public bool GetDataPresent(string format, bool autoConvert)
		{
			if (null == _availableFormats)
				SetFormats();

			return _availableFormats.Contains(format);
		}

		public bool GetDataPresent(string format)
		{
			return GetDataPresent(format, true);
		}

		public string[] GetFormats(bool autoConvert)
		{
			if (null == _availableFormats)
				SetFormats();

			return _availableFormats.ToArray();
		}

		public string[] GetFormats()
		{
			return GetFormats(true);
		}

		public void SetFormats()
		{
			_availableFormats = new List<string>();

			if (null != FolderName)
				_availableFormats.Add(Format_ProjectFolder);

			_availableFormats.Add(Format_ApplicationInstanceGuid);

			_availableFormats.Add(Format_ItemList);
			_availableFormats.Add(Format_ItemReferenceList);
		}

		public string FolderName { get; set; }

		public List<object> ItemList { get; set; }

		#region Not implemented interface functions

		public object GetData(Type format)
		{
			throw new NotImplementedException();
		}

		public bool GetDataPresent(Type format)
		{
			throw new NotImplementedException();
		}

		public void SetData(string format, object data, bool autoConvert)
		{
			throw new NotImplementedException();
		}

		public void SetData(Type format, object data)
		{
			throw new NotImplementedException();
		}

		public void SetData(string format, object data)
		{
			throw new NotImplementedException();
		}

		public void SetData(object data)
		{
			throw new NotImplementedException();
		}

		#endregion Not implemented interface functions
	}
}