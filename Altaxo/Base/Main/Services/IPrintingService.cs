using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// This interface provides access to printers and forms.
	/// </summary>
	public interface IPrintingService
	{
		/// <summary>
		/// Returns the current print document for this instance of the application.
		/// This contains settings that store the current printer, paper size, orientation and so on.
		/// </summary>
		System.Drawing.Printing.PrintDocument PrintDocument { get; }

		System.Drawing.Printing.Margins PrintingMargins { get; }

		System.Drawing.RectangleF PrintingBounds { get; }

		/// <summary>
		/// Update the default bounds and margins after the printer settings changed.
		/// This function must be called manually after a page setup dialog.
		/// </summary>
		void UpdateBoundsAndMargins();
	}
}