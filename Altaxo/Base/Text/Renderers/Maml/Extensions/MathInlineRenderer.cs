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

using System.Collections.Generic;
using System.IO;
using Altaxo;
using Altaxo.Graph;
using Altaxo.Main.Services;
using Altaxo.Text.Renderers;
using Altaxo.Text.Renderers.Maml;
using Markdig.Extensions.Mathematics;

namespace Altaxo.Text.Renderers.Maml.Extensions
{
  public class MathInlineRenderer : MamlObjectRenderer<MathInline>
  {
    protected override void Write(MamlRenderer renderer, MathInline obj)
    {
      var formulaText = obj.Content.Text.Substring(obj.Content.Start, obj.Content.Length);

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

      var attributes = new Dictionary<string, string>
      {
        { "src", localUrl },
        { "align", placement },

        { "width", System.Xml.XmlConvert.ToString(width) },
        { "height", System.Xml.XmlConvert.ToString(height) }
      };

      renderer.Push(MamlElements.markup);

      renderer.Push(MamlElements.a, new[] { new KeyValuePair<string, string>("href", renderer.ImageTopicFileGuid + ".htm#" + streamHash) });

      renderer.Push(MamlElements.img, attributes);

      renderer.PopTo(MamlElements.markup);
    }
  }
}
