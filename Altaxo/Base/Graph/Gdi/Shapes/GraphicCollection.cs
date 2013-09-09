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

		/// <summary>
		/// Copy constructor. Clones (!) all the graph items from the other collection
		/// </summary>
		/// <param name="from">The collection to clone the items from.</param>
		public GraphicCollection(GraphicCollection from)
		{
			for (int i = 0; i < from.Count; i++)
				this.Add((IGraphicBase)from[i].Clone());
		}

		public GraphicCollection(IGraphicBase[] g)
			: base()
		{
			this.AddRange(g);
		}

		public IHitTestObject HitTest(HitTestPointData layerHitTestData)
		{
			// hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
			for (int i = this.Count - 1; i >= 0; --i)
			{
				var hit = this[i].HitTest(layerHitTestData);
				if (null != hit)
				{
					if (null == hit.Remove && (hit.HittedObject is IGraphicBase))
						hit.Remove = new DoubleClickHandler(EhGraphicsObject_Remove);
					return hit;
				}
			}

			return null;
		}

		private static bool EhGraphicsObject_Remove(IHitTestObject o)
		{
			var go = (IGraphicBase)o.HittedObject;
			o.ParentLayer.GraphObjects.Remove(go);
			return true;
		}

		public IGraphicBase FindObjectAtPoint(HitTestPointData htd)
		{
			int len = this.Count;
			foreach (IGraphicBase g in this)
			{
				if (null != g.HitTest(htd))
					return g;
			}
			return null;
		}

		/// <summary>
		/// Scales the position of all items according to xscale and yscale.
		/// </summary>
		/// <param name="xscale"></param>
		/// <param name="yscale"></param>
		public void ScalePosition(double xscale, double yscale)
		{
			foreach (IGraphicBase o in this)
			{
				GraphicBase.ScalePosition(o, xscale, yscale);
			}
		}

		/// <summary>
		/// Moves the selected objects forwards or backwards in the list.
		/// </summary>
		/// <param name="selectedObjects">List of objects that should be moved. These objects must be part of this GraphicCollection.</param>
		/// <param name="steps">Number of steps to move. Positive values move the objects to higher indices, thus to the top of the drawing.</param>
		public void Move(ISet<IGraphicBase> selectedObjects, int steps)
		{
			using (var token = this.GetEventDisableToken())
			{
				Altaxo.Collections.ListExtensions.MoveSelectedItems(this, i => selectedObjects.Contains(this[i]), steps);
			}
		}
	} // end class GraphicsObjectCollection
}