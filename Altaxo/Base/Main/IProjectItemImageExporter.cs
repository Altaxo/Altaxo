using Altaxo.Drawing;
using Altaxo.Graph.Graph3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Interface to a class that is used to export project items as images.
	/// </summary>
	public interface IProjectItemImageExporter
	{
		/// <summary>
		/// Saves the project item as image to the provided stream.
		/// </summary>
		/// <param name="item">The item to export, for instance an item of type <see cref="Altaxo.Graph.Gdi.GraphDocument"/> or <see cref="Altaxo.Graph.Graph3D.GraphDocument"/>.</param>
		/// <param name="options">The export options.</param>
		/// <param name="toStream">The stream to save the image to.</param>
		void ExportAsImageToStream(Main.IProjectItem item, Altaxo.Graph.Gdi.GraphExportOptions options, System.IO.Stream toStream);
	}
}