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

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for GraphObjectCollection.
	/// </summary>
	[Serializable]
	public class GraphObjectCollection : System.Collections.CollectionBase
	{

		public GraphObjectCollection()
		: base()
		{
		}


		public GraphObjectCollection(GraphObjectCollection coll)
			: base()
		{
			this.AddRange(coll);
		}

		public GraphObjectCollection(GraphObject[] g)
			: base()
		{
			this.AddRange(g);
		}

		public void DrawObjects(Graphics g, float Scale, object container)
		{
			int len = this.InnerList.Count;
			for(int i=0;i<len;i++)
			{
				((GraphObject)this.InnerList[i]).Paint(g, container);
			}
		}

		public GraphObject FindObjectAtPoint(PointF pt)
		{
			if(null!=this.InnerList)
			{
				int len = this.InnerList.Count;
				foreach(GraphObject g in this.InnerList)
				{
					if(null!=g.HitTest(pt))
						return g;
				}
			}
				return null;
			
		}


		 public GraphObject this[int index]
		 {
			 get
			 {
				 return (GraphObject)List[index];
			 }
			 set
			 {
				 List[index] = value;
			 }
		 }

		public int Add(GraphObject go)
		{
			go.Container = this;
			return List.Add(go);
		}

		public void AddRange(GraphObject[] gos)
		{
			int len = gos.Length;
			for(int i=0;i<len;i++)
				this.Add(gos[i]);
		}

		public void AddRange(GraphObjectCollection goc)
		{
			int len = goc.Count;
			for(int i=0;i<len;i++)
				this.Add(goc[i]);
		}

		public bool Contains(GraphObject go)
		{
			return List.Contains(go);
		}

		public void CopyTo(GraphObject[] array, int index)
		{
			List.CopyTo(array, index);
		}

		public int IndexOf(GraphObject go)
		{
			return List.IndexOf(go);
		}
		public void Insert(int index, GraphObject go)
		{
			List.Insert(index, go);
		}

    // See also 'System.Collections.IEnumerator'
		public new  GraphObjectEnumerator GetEnumerator()
		{
			return new GraphObjectEnumerator(this);
		}

		public void Remove(GraphObject go)
		{
			List.Remove(go);
		}
	} // end class GraphObjectCollection


	public class GraphObjectEnumerator :  IEnumerator
	{
		private IEnumerator baseEnumerator;

		private IEnumerable temp;


		public GraphObjectEnumerator(GraphObjectCollection mappings)
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
