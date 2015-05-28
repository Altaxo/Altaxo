using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Determines the degree of interaction with the user during operations.
	/// </summary>
	public enum UserInteractionLevel
	{
		/// <summary>
		/// No interaction with the user. Warnings will be ignored, and errors usually throw an exception.
		/// </summary>
		None = 0,

		/// <summary>
		/// Interaction with the user only if errors occured. Warnings will be ignored.
		/// </summary>
		InteractOnErrors = 1,

		/// <summary>
		/// Interaction with the user only if errors or warnings occured.
		/// </summary>
		InteractOnWarningsAndErrors = 2,

		/// <summary>
		/// Always interaction with the user. This means that in any case for instance a dialog will be presented to the user.
		/// </summary>
		InteractAlways = 3 
	}
}
