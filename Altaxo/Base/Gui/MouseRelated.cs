using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Specifies constants that define which mouse button was pressed. 
	/// </summary>
	[Flags]
	public enum AltaxoMouseButtons
	{
		/// <summary>No mouse button was pressed.</summary>
		None,
		/// <summary>The left mouse button was pressed.</summary>
		Left, //   
		/// <summary>The middle mouse button was pressed. </summary>
		Middle,
		/// <summary>The right mouse button was pressed.</summary>
		Right,
		/// <summary>The first XButton was pressed.</summary>
		XButton1,
		/// <summary>The second XButton was pressed. </summary>
		XButton2,
	}

	/// <summary>Provides data for the MouseUp, MouseDown, and MouseMove events.</summary>
	public class AltaxoMouseEventArgs
	{
		/// <summary>Gets which mouse button was pressed.</summary>
		public AltaxoMouseButtons Button { get; set; }
		/// <summary>Gets the number of times the mouse button was pressed and released. </summary>
		int Clicks { get; set; }
		/// <summary>Gets a signed count of the number of detents the mouse wheel has rotated. A detent is one notch of the mouse wheel. </summary>
		int Delta { get; set; }
		/// <summary>Gets the x-coordinate of the mouse during the generating mouse event.</summary>
		double X { get; set; }
		/// <summary>Gets the y-coordinate of the mouse during the generating mouse event. </summary>
		double Y { get; set; }
	}

}
