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

using Altaxo.Collections;
using Altaxo.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
	/// <summary>
	/// Summary description for GraphicsObjectCollection.
	/// </summary>
	[Serializable]
	public class GraphicCollection
		:
		PartitionableList<IGraphicBase>
	{
		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphicsObjectCollection", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicCollection), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphicCollection s = (GraphicCollection)obj;

				info.CreateArray("GraphObjects", s.Count);
				for (int i = 0; i < s.Count; i++)
					info.AddValue("GraphicsObject", s[i]);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				IList<IGraphicBase> s = null != o ? (IList<IGraphicBase>)o : new List<IGraphicBase>();

				int count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					IGraphicBase go = (IGraphicBase)info.GetValue(s);
					s.Add(go);
				}
				info.CloseArray(count);

				return s;
			}
		}

		#endregion "Serialization"

		public GraphicCollection(Action<IGraphicBase> insertAction)
			: base(insertAction)
		{
		}
	} // end class GraphicsObjectCollection
}