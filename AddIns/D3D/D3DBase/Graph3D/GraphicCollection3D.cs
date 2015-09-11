#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Graph3D
{
	/// <summary>
	/// Summary description for GraphicsObjectCollection.
	/// </summary>
	[Serializable]
	public class GraphicCollection3D
		:
		PartitionableList<IGraphicBase3D>
	{
		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicCollection3D), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphicCollection3D s = (GraphicCollection3D)obj;

				info.CreateArray("GraphObjects", s.Count);
				for (int i = 0; i < s.Count; i++)
					info.AddValue("GraphicsObject", s[i]);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				IList<IGraphicBase3D> s = null != o ? (IList<IGraphicBase3D>)o : new List<IGraphicBase3D>();

				int count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					IGraphicBase3D go = (IGraphicBase3D)info.GetValue("e", s);
					s.Add(go);
				}
				info.CloseArray(count);

				return s;
			}
		}

		#endregion "Serialization"

		public GraphicCollection3D(Action<IGraphicBase3D> insertAction)
			: base(insertAction)
		{
		}
	} // end class GraphicsObjectCollection
}