// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.Collections;

namespace WeifenLuo.WinFormsUI
{
	public class DockContentCollection : ReadOnlyCollectionBase
	{
		internal DockContentCollection()
		{
		}

		public DockContent this[int index]
		{
			get {  return InnerList[index] as DockContent;  }
		}

		internal int Add(DockContent content)
		{
			if (Contains(content))
				return IndexOf(content);

			return InnerList.Add(content);
		}

		internal void AddAt(DockContent content, int index)
		{
			if (index < 0 || index > InnerList.Count - 1)
				return;

			if (Contains(content))
				return;

			InnerList.Insert(index, content);
		}

		internal void AddAt(DockContent content, DockContent before)
		{
			if (!Contains(before))
				return;

			if (Contains(content))
				return;

			AddAt(content, IndexOf(before));
		}

		internal void Clear()
		{
			InnerList.Clear();
		}

		public bool Contains(DockContent content)
		{
			return InnerList.Contains(content);
		}

		internal void Dispose()
		{
			for (int i=Count - 1; i>=0; i--)
				this[i].Dispose();
		}

		public int IndexOf(DockContent content)
		{
			if (!Contains(content))
				return -1;
			else
				return InnerList.IndexOf(content);
		}

		internal void Remove(DockContent content)
		{
			if (!Contains(content))
				return;

			InnerList.Remove(content);
		}

		public DockContent[] Select(DockAreas stateFilter)
		{
			int count = 0;
			foreach (DockContent c in this)
				if (DockHelper.IsDockStateValid(c.DockState, stateFilter))
					count ++;

			DockContent[] contents = new DockContent[count];

			count = 0;
			foreach (DockContent c in this)
				if (DockHelper.IsDockStateValid(c.DockState, stateFilter))
					contents[count++] = c;

			return contents;
		}
	}
}
