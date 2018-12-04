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

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Syntax;

namespace Altaxo.Text.Renderers.OpenXML
{
  /// <summary>
  /// Maml renderer for a <see cref="CodeBlock"/>.
  /// </summary>
  /// <seealso cref="OpenXMLObjectRenderer{T}" />
  public class CodeBlockRenderer : OpenXMLObjectRenderer<CodeBlock>
  {
    protected override void Write(OpenXMLRenderer renderer, CodeBlock obj)
    {
      renderer.Paragraph = renderer.Body.AppendChild(new Paragraph());
      renderer.Run = null;
      renderer.ApplyStyleToParagraph("CodeBlock", "CodeBlock", renderer.Paragraph);

      if (obj.Inline != null)
      {
        // there was a post-processor which has already processed the lines in this code block
        renderer.WriteChildren(obj.Inline);
      }
      else // there was no post-processor - we have to do the writing of the code lines
      {
        // original code: renderer.WriteLeafRawLines(obj); // Expand this call directly here in order to be able to include tags
        var lines = obj.Lines;
        if (lines.Lines != null)
        {
          var slices = lines.Lines;
          for (var i = 0; i < lines.Count; i++)
          {
            renderer.Paragraph.AppendChild(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text() { Text = slices[i].Slice.ToString(), Space = SpaceProcessingModeValues.Preserve }));
            renderer.Paragraph.AppendChild(new Run(new Break()));
          }
        }
      }
    }
  }
}
