// *****************************************************************************
// 
//  (c) Crownwood Consulting Limited 2002-2003
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Crownwood Consulting 
//	Limited, Crownwood, Bracknell, Berkshire, England and are supplied subject 
//  to licence terms.
// 
//  Magic Version 1.7.4.0 	www.dotnetmagic.com
// *****************************************************************************

using System;
using System.Collections;

namespace Crownwood.Magic.Collections
{
    // Declare the event signatures
    public delegate void CollectionClear();
    public delegate void CollectionChange(int index, object value);

	public class CollectionWithEvents : CollectionBase
	{
		// Instance fields
		private int _suspendCount; 

		// Collection change events
		public event CollectionClear Clearing;
		public event CollectionClear Cleared;
		public event CollectionChange Inserting;
		public event CollectionChange Inserted;
		public event CollectionChange Removing;
		public event CollectionChange Removed;

		public CollectionWithEvents()
		{
			// Default to not suspended
			_suspendCount = 0;
		}

		// Do not generate change events until resumed
		public void SuspendEvents()
		{
			_suspendCount++;
		}

		// Safe to resume change events.
		public void ResumeEvents()
		{
			--_suspendCount;
		}

		// Are change events currently suspended?
		public bool IsSuspended
		{
			get { return (_suspendCount > 0); }
		}

		// Overrides for generating events
		protected override void OnClear()
		{
			if (!IsSuspended)
			{
				// Any attached event handlers?
				if (Clearing != null)
					Clearing();
			}
		}	

		protected override void OnClearComplete()
		{
			if (!IsSuspended)
			{
				// Any attached event handlers?
				if (Cleared != null)
					Cleared();
			}
		}	

		protected override void OnInsert(int index, object value)
		{
			if (!IsSuspended)
			{
				// Any attached event handlers?
				if (Inserting != null)
					Inserting(index, value);
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			if (!IsSuspended)
			{
				// Any attached event handlers?
				if (Inserted != null)
					Inserted(index, value);
			}
		}

		protected override void OnRemove(int index, object value)
		{
			if (!IsSuspended)
			{
				// Any attached event handlers?
				if (Removing != null)
					Removing(index, value);
			}
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			if (!IsSuspended)
			{
				// Any attached event handlers?
				if (Removed != null)
					Removed(index, value);
			}
		}

		protected int IndexOf(object value)
		{
			// Find the 0 based index of the requested entry
			return base.List.IndexOf(value);
		}
	}
}
	