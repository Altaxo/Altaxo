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
	public class DockWindowCollection : ReadOnlyCollectionBase
	{
		internal DockWindowCollection(DockPanel dockPanel)
		{
			InnerList.Add(new DockWindow(dockPanel, DockState.Document));
			InnerList.Add(new DockWindow(dockPanel, DockState.DockLeft));
			InnerList.Add(new DockWindow(dockPanel, DockState.DockRight));
			InnerList.Add(new DockWindow(dockPanel, DockState.DockTop));
			InnerList.Add(new DockWindow(dockPanel, DockState.DockBottom));
		}

		public DockWindow this [DockState dockState]
		{
			get
			{
				if (dockState == DockState.Document)
					return InnerList[0] as DockWindow;
				else if (dockState == DockState.DockLeft)
					return InnerList[1] as DockWindow;
				else if (dockState == DockState.DockRight)
					return InnerList[2] as DockWindow;
				else if (dockState == DockState.DockTop)
					return InnerList[3] as DockWindow;
				else if (dockState == DockState.DockBottom)
					return InnerList[4] as DockWindow;

				throw (new ArgumentOutOfRangeException());
			}
		}
	}
}
