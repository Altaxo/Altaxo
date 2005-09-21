using System;

namespace Altaxo.Main
{
	/// <summary>
	/// Extends the IDisposable interface in a way that an event is fired if the object is disposed.
	/// </summary>
	public interface IEventIndicatedDisposable : System.IDisposable
	{
    /// <summary>
    /// The event that is fired when the object is disposed.
    /// </summary>
		event EventHandler Disposed;
	}
}
