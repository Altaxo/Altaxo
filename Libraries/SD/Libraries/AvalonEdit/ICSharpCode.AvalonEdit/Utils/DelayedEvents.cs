// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3635 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Maintains a list of delayed events to raise.
	/// </summary>
	sealed class DelayedEvents
	{
		struct EventCall
		{
			EventHandler handler;
			object sender;
			EventArgs e;
			
			public EventCall(EventHandler handler, object sender, EventArgs e)
			{
				this.handler = handler;
				this.sender = sender;
				this.e = e;
			}
			
			public void Call()
			{
				handler(sender, e);
			}
		}
		
		Queue<EventCall> eventCalls = new Queue<EventCall>();
		
		public void DelayedRaise(EventHandler handler, object sender, EventArgs e)
		{
			if (handler != null) {
				eventCalls.Enqueue(new EventCall(handler, sender, e));
			}
		}
		
		public void RaiseEvents()
		{
			while (eventCalls.Count > 0)
				eventCalls.Dequeue().Call();
		}
	}
}
