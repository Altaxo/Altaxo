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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui.Text.Viewing
{
	public class ImageProvider : Markdig.Renderers.WpfImageProviderBase
	{
		private const string graphPretext = "graph:";

		public override Inline GetInlineItem(string url)
		{
			if (url.StartsWith(graphPretext))
			{
				string graphName = url.Substring(graphPretext.Length);

				if (Current.Project.GraphDocumentCollection.Contains(graphName))
				{
					var graph = Current.Project.GraphDocumentCollection[graphName];

					var options = new Altaxo.Graph.Gdi.GraphExportOptions()
					{
						SourceDpiResolution = 96,
						DestinationDpiResolution = 96,
					};

					using (var stream = new System.IO.MemoryStream())
					{
						Altaxo.Graph.Gdi.GraphDocumentExportActions.RenderToStream(graph, stream, options);
						stream.Seek(0, System.IO.SeekOrigin.Begin);

						var imageSource = BitmapFrame.Create(stream,
																			BitmapCreateOptions.None,
																			BitmapCacheOption.OnLoad);

						return new InlineUIContainer(new Image() { Source = imageSource });
					}
				}

				return new Run("ERROR-Image not found");
			}
			else
			{
				return base.GetInlineItem(url);
			}
		}
	}
}
