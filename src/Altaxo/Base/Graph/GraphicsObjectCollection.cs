/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for GraphicsObjectCollection.
	/// </summary>
	[SerializationSurrogate(0,typeof(GraphicsObjectCollection.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class GraphicsObjectCollection : Altaxo.Data.CollectionBase, Main.IChangedEventSource, Main.IChildChangedEventSink
	{

		#region "Serialization"

		/// <summary>Used to serialize the GraphicsObjectCollection Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{

			/// <summary>
			/// Serializes GraphicsObjectCollection Version 0.
			/// </summary>
			/// <param name="obj">The GraphicsObjectCollection to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				GraphicsObjectCollection s = (GraphicsObjectCollection)obj;
				info.AddValue("Data",s.myList);
			}

			/// <summary>
			/// Deserializes the GraphicsObjectCollection Version 0.
			/// </summary>
			/// <param name="obj">The empty GraphicsObjectCollection object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized GraphicsObjectCollection.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				GraphicsObjectCollection s = (GraphicsObjectCollection)obj;

				s.myList =	(System.Collections.ArrayList)info.GetValue("Data",typeof(System.Collections.ArrayList));
				
				return s;
			}
		}


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphicsObjectCollection),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphicsObjectCollection s = (GraphicsObjectCollection)obj;
				
				info.CreateArray("GraphObjects",s.myList.Count);
				for(int i=0;i<s.myList.Count;i++)
					info.AddValue("GraphicsObject",s.myList[i]);
				info.CommitArray();
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				GraphicsObjectCollection s = null!=o ? (GraphicsObjectCollection)o : new GraphicsObjectCollection();

				int count = info.OpenArray();
				for(int i=0;i<count;i++)
				{
					GraphicsObject go = (GraphicsObject)info.GetValue(s);
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
			for(int i=0;i<Count;i++)
				this[i].Changed += new EventHandler(this.EhChildChanged);
		}
		#endregion



		public GraphicsObjectCollection()
		{
		}


		/// <summary>
		/// Copy constructor. Clones (!) all the graph items from the other collection
		/// </summary>
		/// <param name="from">The collection to clone the items from.</param>
		public GraphicsObjectCollection(GraphicsObjectCollection from)
		{
			for(int i=0;i<from.Count;i++)
				this.Add((GraphicsObject)from[i].Clone());
		}

		public GraphicsObjectCollection(GraphicsObject[] g)
			: base()
		{
			this.AddRange(g);
		}

		public void DrawObjects(Graphics g, float Scale, object container)
		{
			int len = this.InnerList.Count;
			for(int i=0;i<len;i++)
			{
				((GraphicsObject)this.InnerList[i]).Paint(g, container);
			}
		}

		public GraphicsObject FindObjectAtPoint(PointF pt)
		{
			if(null!=this.InnerList)
			{
				int len = this.InnerList.Count;
				foreach(GraphicsObject g in this.InnerList)
				{
					if(null!=g.HitTest(pt))
						return g;
				}
			}
				return null;
			
		}


		 public GraphicsObject this[int index]
		 {
			 get
			 {
				 return (GraphicsObject)List[index];
			 }
			 set
			 {
				 value.Container = this;
				 List[index] = value;
			 }
		 }

		public int Add(GraphicsObject go)
		{
			go.Container = this;
			return List.Add(go);
		}

		public void AddRange(GraphicsObject[] gos)
		{
			int len = gos.Length;
			for(int i=0;i<len;i++)
				this.Add(gos[i]);
		}

		public void AddRange(GraphicsObjectCollection goc)
		{
			int len = goc.Count;
			for(int i=0;i<len;i++)
				this.Add(goc[i]);
		}

		public bool Contains(GraphicsObject go)
		{
			return List.Contains(go);
		}

		public void CopyTo(GraphicsObject[] array, int index)
		{
			List.CopyTo(array, index);
		}

		public int IndexOf(GraphicsObject go)
		{
			return List.IndexOf(go);
		}
		public void Insert(int index, GraphicsObject go)
		{
			List.Insert(index, go);
		}

    // See also 'System.Collections.IEnumerator'
		public new  GraphObjectEnumerator GetEnumerator()
		{
			return new GraphObjectEnumerator(this);
		}

		public void Remove(GraphicsObject go)
		{
			List.Remove(go);
		}
		#region IChangedEventSource Members

		public event System.EventHandler Changed;

		public void EhChildChanged(object child, EventArgs e)
		{
			OnChildChanged(child,e);
		}

		public virtual void OnChildChanged(object child, EventArgs e)
		{
			if(null!=Changed)
				Changed(this, e);
	
		}

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this, new Main.ChangedEventArgs(this,null));
		}
		#endregion
	} // end class GraphicsObjectCollection


	public class GraphObjectEnumerator :  IEnumerator
	{
		private IEnumerator baseEnumerator;

		private IEnumerable temp;


		public GraphObjectEnumerator(GraphicsObjectCollection mappings)
			: base()
		{
			this.temp = (IEnumerable)mappings;
			this.baseEnumerator = temp.GetEnumerator();
		}

		public object Current
		{
			get
			{
				return baseEnumerator.Current;
			}
		}

		public bool MoveNext()
		{
			return baseEnumerator.MoveNext();
		}


		public void Reset()
		{
			baseEnumerator.Reset();
		}

	}


}
