using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
	/// <summary>
	/// Designates the usage of a color in plot styles.
	/// </summary>
	public enum ColorLinkage
	{
		/// <summary>
		/// Fully dependent color. Must be a member of a plot color set. Act both as color provider and color receiver.
		/// </summary>
		Dependent = 0,

		/// <summary>
		/// Fully independent color. Is neither a provider of the color nor a receiver.
		/// </summary>
		Independent=1,

	

		/// <summary>
		/// Dependent color. Can not be a provider. When receiving the color from other providers, the alpha value of the original color is preserved.
		/// This means that therefrom resulting color is probably not a plot color, and has no parent color set (this is the reason that the color can not act as provider).
		/// </summary>
		PreserveAlpha


		

	}
}
