// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3908 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// This class is used to prevent stack overflows by representing a 'busy' flag
	/// that prevents reentrance when another call is running.
	/// However, using a simple 'bool busy' is not thread-safe, so we use a
	/// thread-static BusyManager.
	/// </summary>
	sealed class BusyManager
	{
		public struct BusyLock : IDisposable
		{
			public static readonly BusyLock Failed = new BusyLock(null);
			
			readonly BusyManager manager;
			
			public BusyLock(BusyManager manager)
			{
				this.manager = manager;
			}
			
			public bool Success {
				get { return manager != null; }
			}
			
			public void Dispose()
			{
				if (manager != null) {
					manager.activeObjects.RemoveAt(manager.activeObjects.Count - 1);
				}
			}
		}
		
		readonly List<object> activeObjects = new List<object>();
		
		public BusyLock Enter(object obj)
		{
			for (int i = 0; i < activeObjects.Count; i++) {
				if (activeObjects[i] == obj)
					return BusyLock.Failed;
			}
			activeObjects.Add(obj);
			return new BusyLock(this);
		}
	}
}
