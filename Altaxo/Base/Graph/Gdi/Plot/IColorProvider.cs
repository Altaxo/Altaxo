using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot
{
	/// <summary>
	/// Interface to calculate a color out of a relative value that is normally
	/// between 0 and 1. Special colors should be used for values between 0, above 1, and for NaN.
	/// </summary>
	public interface IColorProvider : Main.IChangedEventSource, ICloneable, Main.ICopyFrom
	{
		/// <summary>
		/// Calculates a color from the provided relative value.
		/// </summary>
		/// <param name="relVal">Value used for color calculation. Normally between 0 and 1.</param>
		/// <returns>A color associated with the relative value.</returns>
		System.Drawing.Color GetColor(double relVal);
	}
}
