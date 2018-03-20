#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Text.GuiModels;

namespace Altaxo.Gui.Text.Viewing
{
	public interface ITextDocumentView
	{
		ITextDocumentController Controller { set; }

		/// <summary>
		/// Sets the name of the document, and the local images. This function must be called every time the document name has changed.
		/// </summary>
		/// <param name="documentName">The full name of the document.</param>
		/// <param name="localImages">The dictionary of local images.</param>
		void SetDocumentNameAndLocalImages(string documentName, IReadOnlyDictionary<string, Altaxo.Graph.MemoryStreamImageProxy> localImages);

		string SourceText { get; set; }

		event EventHandler SourceTextChanged;

		string StyleName { set; }
		bool IsViewerSelected { get; set; }
		ViewerConfiguration WindowConfiguration { get; set; }
		double FractionOfEditorWindow { get; set; }
	}

	public interface ITextDocumentController
	{
		string InsertImageInDocumentAndGetUrl(string fileName);

		string InsertImageInDocumentAndGetUrl(System.IO.MemoryStream memoryStream);

		/// <summary>
		/// Tests if the provided file name could be accepted as an image.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>True if the name could be accepted; false otherwise.</returns>
		bool CanAcceptImageFileName(string fileName);

		void EhIsViewerSelectedChanged(bool isViewerSelected);

		void EhViewerConfigurationChanged(ViewerConfiguration windowConfiguration);

		void EhFractionOfEditorWindowChanged(double fractionOfEditor);
	}
}
