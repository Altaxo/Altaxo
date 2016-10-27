using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
	/// <summary>
	/// Determines how the plot color influences the colors of a scatter symbol.
	/// </summary>
	[Flags]
	public enum PlotColorInfluence
	{
		/// <summary>
		/// The plot color has no influence on any of the colors in the scatter symbol.
		/// </summary>
		None = 0,

		/// <summary>
		/// The fill color is controlled by the plot color.
		/// </summary>
		FillColor = 1,

		/// <summary>
		/// The frame color is controlled by the plot color.
		/// </summary>
		FrameColor = 2,

		/// <summary>
		/// The inset color is controlled by the plot color.
		/// </summary>
		InsetColor = 4,
	}
}