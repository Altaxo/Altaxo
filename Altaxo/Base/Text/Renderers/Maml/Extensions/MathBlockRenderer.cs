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

using Altaxo;
using Altaxo.Graph;
using Altaxo.Main.Services;
using Altaxo.Text.Renderers;
using Altaxo.Text.Renderers.Maml;
using Markdig.Extensions.Mathematics;
using System.Collections.Generic;
using System.IO;

namespace Altaxo.Text.Renderers.Maml.Extensions
{
	public class MathBlockRenderer : MamlObjectRenderer<MathBlock>
	{
		protected override void Write(MamlRenderer renderer, MathBlock obj)
		{
			string formulaText = string.Empty; // obj.Content.Text.Substring(obj.Content.Start, obj.Content.Length);

			for (int i = 0; i < obj.Lines.Count; ++i)
			{
				var l = obj.Lines.Lines[i];
				formulaText += l.Slice.Text.Substring(l.Slice.Start, l.Slice.Length);
			}

			if (string.IsNullOrEmpty(formulaText))
				return;

			var formulaService = Current.GetRequiredService<ILaTeXFormulaImageStreamProvider>();

			var (stream, placement, width, height) = formulaService.Parse(formulaText, renderer.BodyTextFontFamily, renderer.BodyTextFontSize, 192, renderer.IsIntendedForHelp1File);

			if (null == stream)
				return;

			stream.Seek(0, SeekOrigin.Begin);
			var streamHash = MemoryStreamImageProxy.ComputeStreamHash(stream);
			stream.Seek(0, SeekOrigin.Begin);

			try
			{
				renderer.StorePngImageFile(stream, streamHash);
				stream.Close();
			}
			finally
			{
				stream.Dispose();
			}

			// now render to Maml file

			string localUrl = "../media/" + streamHash + ".png";

			var attributes = new Dictionary<string, string>();
			attributes.Add("src", localUrl);
			attributes.Add("width", System.Xml.XmlConvert.ToString(width));
			attributes.Add("height", System.Xml.XmlConvert.ToString(height));

			renderer.Push(MamlElements.markup);

			renderer.Push(MamlElements.img, attributes);

			renderer.PopTo(MamlElements.img);

			renderer.PopTo(MamlElements.markup);
		}
	}
}
