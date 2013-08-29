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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Collections;

namespace Altaxo.Graph.Gdi.Shapes
{
	/// <summary>
	/// Summary description for GraphicsObjectCollection.
	/// </summary>
	[Serializable]
	public class GraphicCollection
		:
		IList<IGraphicBase>,
		Main.IChangedEventSource,
		Main.IChildChangedEventSink,
		Main.IDocumentNode
	{

		[field: NonSerialized]
		public event System.EventHandler Changed;

		[NonSerialized]
		object _parent;

		PartitionableList<IGraphicBase> _items = new PartitionableList<IGraphicBase>();



		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphicsObjectCollection", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicCollection), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

				GraphicCollection s = null != o ? (GraphicCollection)o : new GraphicCollection();

				int count = info.OpenArray();
				for (int i = 0; i < count; i++)
				{
					GraphicBase go = (GraphicBase)info.GetValue(s);
					s.Add(go);
				}
				info.CloseArray(count);

				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// restore the event chain
			for (int i = 0; i < Count; i++)
				this[i].Changed += new EventHandler(this.EhChildChanged);
		}
		#endregion



		public GraphicCollection()
		{
		}


		/// <summary>
		/// Copy constructor. Clones (!) all the graph items from the other collection
		/// </summary>
		/// <param name="from">The collection to clone the items from.</param>
		public GraphicCollection(GraphicCollection from)
		{
			for (int i = 0; i < from.Count; i++)
				this.Add((GraphicBase)from[i].Clone());
		}

		public GraphicCollection(GraphicBase[] g)
			: base()
		{
			this.AddRange(g);
		}

			/// <summary>
		/// Creates a partial view. A partial view is a view of the list of items of the original collection, that fulfill a certain condition.
		/// </summary>
		/// <param name="selectionCriterium">The selection condition. If this function returns <c>true</c> on an item of the original collection, that item is member of the partial view.</param>
		/// <returns>List of items of the original collection, that fullfils the selection condition. The returned list is automatically updated, when the original collection changed or when
		/// the user changed the partial view itself.</returns>
		public IObservableList<IGraphicBase> CreatePartialView(Func<IGraphicBase, bool> selectionCriterium)
		{
			return _items.CreatePartialView(selectionCriterium);
		}


		public IObservableList<M> CreatePartialViewOfType<M>(Action<M> actionBeforeInsertion) where M : IGraphicBase
		{
			return _items.CreatePartialViewOfType<M>(actionBeforeInsertion);
		}


		public void Paint(Graphics g, double Scale, object container)
		{
			int len = this._items.Count;
			for (int i = 0; i < len; i++)
			{
				this._items[i].Paint(g, container);
			}
		}

		public IHitTestObject HitTest(HitTestPointData layerHitTestData)
		{
			 // hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
				for(int i=this._items.Count-1;i>=0;--i)
				{
					var hit = _items[i].HitTest(layerHitTestData);
					if (null != hit)
					{
						if (null == hit.Remove && (hit.HittedObject is GraphicBase))
							hit.Remove = new DoubleClickHandler(EhGraphicsObject_Remove);
						return hit;
					}
				}

				return null;
			}

		static bool EhGraphicsObject_Remove(IHitTestObject o)
		{
			GraphicBase go = (GraphicBase)o.HittedObject;
			o.ParentLayer.GraphObjects.Remove(go);
			return true;
		}
	
		public GraphicBase FindObjectAtPoint(HitTestPointData htd)
		{
			if (null != this._items)
			{
				int len = this._items.Count;
				foreach (GraphicBase g in this._items)
				{
					if (null != g.HitTest(htd))
						return g;
				}
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
			foreach (GraphicBase o in this._items)
			{
				GraphicBase.ScalePosition(o, xscale, yscale);
			}
		}

		public IGraphicBase this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				value.ParentObject = this;
				_items[index] = value;
				OnChanged();
			}
		}

		public void Add(IGraphicBase go, bool fireChangedEvent)
		{
			go.ParentObject = this;
			_items.Add(go);

			if (fireChangedEvent)
				OnChanged();
		}

		public void AddRange(IGraphicBase[] gos)
		{
			int len = gos.Length;
			for (int i = 0; i < len; i++)
				this.Add(gos[i], false);

			OnChanged();
		}

		public void AddRange(GraphicCollection goc)
		{
			int len = goc.Count;
			for (int i = 0; i < len; i++)
				this.Add(goc[i], false);

			OnChanged();
		}

		public bool Contains(IGraphicBase go)
		{
			return _items.Contains(go);
		}

		public void CopyTo(IGraphicBase[] array, int index)
		{
			_items.CopyTo(array, index);
		}

		public int IndexOf(IGraphicBase go)
		{
			return _items.IndexOf(go);
		}
		public void Insert(int index, IGraphicBase go)
		{
			go.ParentObject = this;
			_items.Insert(index, go);
			OnChanged();
		}

		/// <summary>
		/// Moves the selected objects forwards or backwards in the list.
		/// </summary>
		/// <param name="selectedObjects">List of objects that should be moved. These objects must be part of this GraphicCollection.</param>
		/// <param name="steps">Number of steps to move. Positive values move the objects to higher indices, thus to the top of the drawing.</param>
		public void Move(ISet<IGraphicBase> selectedObjects, int steps)
		{
			Altaxo.Collections.ListMoveOperations.MoveSelectedItems(this, i => selectedObjects.Contains(this[i]), steps);
		}

		#region IChangedEventSource Members


		public virtual void EhChildChanged(object child, EventArgs e)
		{
			OnChanged();
		}

		protected virtual void OnChanged()
		{
			if (this._parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, new Main.ChangedEventArgs(this, null));

			if (null != Changed)
				Changed(this, new Main.ChangedEventArgs(this, null));
		}
		#endregion

		#region IDocumentNode Members

		public object ParentObject
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public string Name
		{
			get { return "GraphicCollection"; }
		}

		#endregion

		#region IList<GraphicBase> Members


		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
			OnChanged();
		}

		#endregion

		#region ICollection<GraphicBase> Members

		public void Add(IGraphicBase item)
		{
			Add(item, true);
		}

		public void Clear()
		{
			_items.Clear();
			OnChanged();
		}

		public int Count
		{
			get { return _items.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(IGraphicBase item)
		{
			bool result = _items.Remove(item);
			if (result)
				OnChanged();
			return result;
		}

		#endregion

		#region IEnumerable<GraphicBase> Members

		public IEnumerator<IGraphicBase> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion
	} // end class GraphicsObjectCollection
}
