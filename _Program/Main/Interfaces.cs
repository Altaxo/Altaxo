using System;

namespace Altaxo
{
	/// <summary>Designates a object which supports the changed event.</summary>
	interface IChangedEventSource
	{
		/// <summary>Fired when something in the object has changed.</summary>
		event System.EventHandler Changed;
	}

	interface IChildChangedEventSink
	{
		void OnChildChanged(object child, EventArgs e);
	}

	/// <summary>ChangedEventArgs can be used by originators of a Changed event to preserve the originator of the Changed event, even if 
	/// the event is chained through a couple of parent objects.</summary>
	public class ChangedEventArgs : EventArgs
	{
		/// <summary>Stores the original object that caused the Changed event.</summary>
		public object Originator;
		/// <summary>Can be used to store additional information about that Changed event.</summary>
		public object Tag;

		/// <summary>
		/// Creates the ChangedEventArgs.
		/// </summary>
		/// <param name="originator">The originator of the Changed event.</param>
		/// <param name="tag">Additional information about the event, may be null.</param>
		public ChangedEventArgs(object originator, object tag)
		{
			Originator = originator;
			Tag = tag;
		}
	}


}
