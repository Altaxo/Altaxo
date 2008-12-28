using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Options to copy styles or items from one graph document to the other.
	/// </summary>
	[Flags]
	public enum GraphCopyOptions
	{
		/// <summary>The notes will be copied.</summary>
		CloneNotes=0x01,

		/// <summary>The graph document properties will be copied.</summary>
		CloneProperties=0x02,

		/// <summary>The layers will be copied including every child.</summary>
		CloneLayers = 0x04,

		/// <summary>The layers will be copied from the other layers.</summary>
		CopyFromLayers = 0x08,

		/// <summary>Plotitems inclusive all styles will be cloned.</summary>
		ClonePlotItems = 0x100,

		/// <summary>Only the plot styles will be copied</summary>
		CopyPlotStyles = 0x200,

		All = -1
	}
}
