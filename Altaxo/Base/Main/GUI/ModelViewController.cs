using System;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// The interface that a controller of the MVC (Model-View-Controller) model must implement.
	/// </summary>
	public interface IMVCController
	{
		/// <summary>
		/// Returns the view that shows the model.
		/// </summary>
		object ViewObject { get; set; }

		/// <summary>
		/// Returns the model (document) that this controller controls
		/// </summary>
		object ModelObject { get; }
	}

	/// <summary>
	/// The interface that a view of the MVC (Model-View-Controller) model must implement.
	/// </summary>
	public interface IMVCView
	{
		/// <summary>
		/// Returns the controller object that controls this view.
		/// </summary>
		object ControllerObject { get; set; }
	}
}
