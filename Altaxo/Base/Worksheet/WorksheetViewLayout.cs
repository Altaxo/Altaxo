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

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Worksheet
{
	/// <summary>
	/// Stores information how a worksheet is shown in the view. Currently, this instance stores a link to the <see cref="WorksheetLayout"/> that defines the colors and the widths of the table columns.
	/// Later, it is planned to additionally store here the positions of horizontal and vertical scroll values in order to be able to restore those settings.
	/// </summary>
	public class WorksheetViewLayout : IProjectItemViewModel
	{
		private WorksheetLayout _worksheetLayout;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Worksheet.GUI.WorksheetController", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Worksheet.GUI.SDWorksheetController", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.SharpDevelop.SDWorksheetViewContent", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.SharpDevelop.SDWorksheetViewContent", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Gui.Worksheet.Viewing.WorksheetController", 1)] // until 2012-02-01 buid 743
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetViewLayout), 0)] // since 2012-02-01 buid 744
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			private AbsoluteDocumentPath _PathToLayout;
			private WorksheetViewLayout _TableController;

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (WorksheetViewLayout)obj;
				info.AddValue("Layout", AbsoluteDocumentPath.GetAbsolutePath(s.WorksheetLayout));
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (WorksheetViewLayout)o : new WorksheetViewLayout();

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
				surr._TableController = s;
				if (info.CurrentElementName == "Controller")
				{
					info.OpenElement();
					surr._PathToLayout = (AbsoluteDocumentPath)info.GetValue("Layout", s);
					info.CloseElement();
				}
				else if (info.CurrentElementName == "BaseType")
				{
					info.GetString("BaseType");
					surr._PathToLayout = (AbsoluteDocumentPath)info.GetValue("Layout", s);
				}
				else
				{
					surr._PathToLayout = (AbsoluteDocumentPath)info.GetValue("Layout", s);
				}

				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

				return s;
			}

			private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
			{
				if (null != _PathToLayout)
				{
					var o = AbsoluteDocumentPath.GetObject(_PathToLayout, documentRoot);
					if (o is Altaxo.Worksheet.WorksheetLayout)
					{
						_TableController._worksheetLayout = (Altaxo.Worksheet.WorksheetLayout)o;
						_PathToLayout = null;
					}
				}

				if (null == _PathToLayout)
				{
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
				}
			}
		}

		#endregion Serialization

		public WorksheetViewLayout(WorksheetLayout worksheetLayout)
		{
			_worksheetLayout = worksheetLayout;
		}

		private WorksheetViewLayout()
		{
		}

		public WorksheetLayout WorksheetLayout
		{
			get
			{
				return _worksheetLayout;
			}
		}

		IProjectItem IProjectItemViewModel.ProjectItem
		{
			get
			{
				return _worksheetLayout?.DataTable;
			}
		}
	}
}