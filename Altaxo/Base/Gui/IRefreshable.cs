using System;

namespace Altaxo.Gui
{
	/// <summary>
	///  Provides the method to refresh a controller and the corresponding view.
	/// </summary>
	public interface IRefreshable
	{
    /// <summary>
    /// Refresh a controller and the corresponding view. Use this if the document has
    /// changed outside of the controller, but the controller has no way to determine when the document has changed.
    /// </summary>
    void Refresh();
	}
}
