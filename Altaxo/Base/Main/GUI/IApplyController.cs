using System;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// This interface can be used by all controllers where the user input needs to
	/// be applied to the document being controlled.
	/// </summary>
	public interface IApplyController
	{
		/// <summary>
		/// Called when the user input has to be applied to the document being controlled. Returns true if Apply is successfull.
		/// </summary>
		/// <returns>True if the apply was successfull, otherwise false.</returns>
		/// <remarks>This function is called in two cases: Either the user pressed OK or the user pressed Apply.</remarks>
		bool Apply();
	}
}
