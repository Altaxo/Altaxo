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
using System.Drawing;

namespace WeifenLuo.WinFormsUI
{
	public class DockList : ReadOnlyCollectionBase
	{
		private IDockListContainer m_container;
		private DisplayingDockList m_displayingList;

		internal DockList(IDockListContainer container)
		{
			m_container = container;
			m_displayingList = new DisplayingDockList(this);
		}

		public IDockListContainer Container
		{
			get	{	return m_container;	}
		}
		
		internal DisplayingDockList DisplayingList
		{
			get	{	return m_displayingList;	}
		}

		public DockState DockState
		{
			get	{	return Container.DockState;	}
		}

		public bool IsFloat
		{
			get	{	return DockState == DockState.Float;	}
		}

		public bool Contains(DockPane pane)
		{
			return InnerList.Contains(pane);
		}

		public int IndexOf(DockPane pane)
		{
			return InnerList.IndexOf(pane);
		}

		public DockPane this[int index]
		{
			get	{	return InnerList[index] as DockPane;	}
		}

		internal void Add(DockPane pane)
		{
			InnerList.Add(pane);
		}

		internal void Remove(DockPane pane)
		{
			if (!Contains(pane))
				return;

			NestedDockingStatus statusPane = pane.GetNestedDockingStatus(IsFloat);
			DockPane lastNestedPane = null;
			for (int i=Count - 1; i> IndexOf(pane); i--)
			{
				if (this[i].GetNestedDockingStatus(IsFloat).PrevPane == pane)
				{
					lastNestedPane = this[i];
					break;
				}
			}

			if (lastNestedPane != null)
			{
				int indexLastNestedPane = IndexOf(lastNestedPane);
				InnerList.Remove(lastNestedPane);
				InnerList[IndexOf(pane)] = lastNestedPane;
				NestedDockingStatus lastNestedDock = lastNestedPane.GetNestedDockingStatus(IsFloat);
				lastNestedDock.SetStatus(this, statusPane.PrevPane, statusPane.Alignment, statusPane.Proportion);
				for (int i=indexLastNestedPane - 1; i>IndexOf(lastNestedPane); i--)
				{
					NestedDockingStatus status = this[i].GetNestedDockingStatus(IsFloat);
					if (status.PrevPane == pane)
						status.SetStatus(this, lastNestedPane, status.Alignment, status.Proportion);
				}
			}
			else
				InnerList.Remove(pane);

			statusPane.SetStatus(null, null, DockAlignment.Left, 0.5);
			statusPane.SetDisplayingStatus(false, null, DockAlignment.Left, 0.5);
			statusPane.SetDisplayingBounds(Rectangle.Empty, Rectangle.Empty, Rectangle.Empty);

			if (InnerList.Count == 0 && this.Container is FloatWindow)
				((FloatWindow)Container).Dispose();
		}
	}
}
