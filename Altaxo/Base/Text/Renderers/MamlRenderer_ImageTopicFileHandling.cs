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

using Altaxo.Collections;
using Altaxo.Text.Renderers.Maml;
using Markdig.Renderers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Altaxo.Text.Renderers
{
	public partial class MamlRenderer : TextRendererBase<MamlRenderer>
	{
		public string ImageTopicFileGuid { get; } = "ACAC6A80-7CE0-4CB9-B36C-B2FB6ACAB027"; // Guid of image topic file is fixed

		/// <summary>
		/// Writes a file which contains all referenced images in native resolution
		/// (without using width and height attributes).
		/// Including this file helps to ensure that all referenced images will be included into the help file.
		/// </summary>
		/// <returns>The guid of this .aml file.</returns>
		public (string fullFileName, string guid) WriteImageTopicFile()
		{
			var fileName = AmlBaseFileName + "_Images.aml";
			var tw = new System.IO.StreamWriter(fileName, false, Encoding.UTF8, 1024);
			this.Writer = tw;

			Push(MamlElements.topic, new[] { new KeyValuePair<string, string>("id", ImageTopicFileGuid), new KeyValuePair<string, string>("revisionNumber", "1") });
			Push(MamlElements.developerConceptualDocument, new[] { new KeyValuePair<string, string>("xmlns", "http://ddue.schemas.microsoft.com/authoring/2003/5"), new KeyValuePair<string, string>("xmlns:xlink", "http://www.w3.org/1999/xlink") });
			Push(MamlElements.introduction);
			Write("This page contains all images used in this help file in native resolution. The ordering of the images is arbitrary.");
			PopTo(MamlElements.introduction);
			Push(MamlElements.section);
			Push(MamlElements.title);
			Write("Appendix: All images in native resolution");
			EnsureLine();
			PopTo(MamlElements.title);
			Push(MamlElements.content);
			Push(MamlElements.sections);

			// all links to all images here
			foreach (var entry in OldToNewImageUris)
			{
				var localUrl = System.IO.Path.GetFileNameWithoutExtension(entry.Value);

				Push(MamlElements.section, new[] { new KeyValuePair<string, string>("address", localUrl) });

				Push(MamlElements.content);

				Push(MamlElements.para);

				Push(MamlElements.mediaLinkInline);

				Push(MamlElements.image, new[] { new KeyValuePair<string, string>("xlink:href", localUrl) });

				PopTo(MamlElements.section);
			}

			// the same again for the formulas
			foreach (var entry in _imageFileNames)
			{
				var localUrl = System.IO.Path.GetFileNameWithoutExtension(entry);

				Push(MamlElements.section, new[] { new KeyValuePair<string, string>("address", localUrl) });

				Push(MamlElements.content);

				Push(MamlElements.para);

				Push(MamlElements.mediaLinkInline);

				Push(MamlElements.image, new[] { new KeyValuePair<string, string>("xlink:href", localUrl) });

				PopTo(MamlElements.section);
			}

			PopAll();

			this.Writer.Close();
			this.Writer.Dispose();
			this.Writer = StreamWriter.Null;

			return (fileName, ImageTopicFileGuid);
		}
	}
}
