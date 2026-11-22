#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

#nullable enable
using System.Linq;
using Markdig;
using Markdig.Extensions.Figures;
using Xunit;

namespace Altaxo.Text
{
  /// <summary>
  /// Verifies that every place that builds a markdown pipeline supports the same set of extensions.
  /// </summary>
  public class MarkdownPipelineTests
  {
    private const string FigureMarkdown = "^^^\n![](graph:MyGraph){width=5cm} {#An_important_graph}\n^^^ Figure 1: This is a graph\n";

    [Fact]
    public void MarkdownUtilitiesPipelineParsesFigureBlocks()
    {
      Assert.NotNull(FindFigureIn(new MarkdownPipelineBuilder().UseSupportedExtensions().Build()));
    }

    [Fact]
    public void MarkdownImportPipelineParsesFigureBlocks()
    {
      var pipeline = MarkdownImportOptions.UseSupportedExtensions(new MarkdownPipelineBuilder()).Build();
      Assert.NotNull(FindFigureIn(pipeline));
    }

    [Fact]
    public void FigureBlocksAreNotParsedWithoutTheFigureExtension()
    {
      // The control case for the two tests above: if this test fails, the two tests above are meaningless.
      var pipeline = new MarkdownPipelineBuilder()
          .UseEmphasisExtras()
          .UseGridTables()
          .UsePipeTables()
          .UseTaskLists()
          .UseAutoLinks()
          .UseMathematics()
          .UseGenericAttributes()
          .Build();

      Assert.Null(FindFigureIn(pipeline));
    }

    [Fact]
    public void ImportFindsImagesInsideFigureBlocks()
    {
      // During import, all images are searched in the parse tree in order to embed them into the text document.
      // Since a figure block is a container, the image inside it must still be found.
      var pipeline = MarkdownImportOptions.UseSupportedExtensions(new MarkdownPipelineBuilder()).Build();
      var document = Markdig.Markdown.Parse(FigureMarkdown, pipeline);

      var image = MarkdownImportOptions.EnumerateAllMarkdownObjectsRecursively(document)
        .OfType<Markdig.Syntax.Inlines.LinkInline>()
        .Single(link => link.IsImage);

      Assert.Equal("graph:MyGraph", image.Url);
    }

    private static Figure? FindFigureIn(MarkdownPipeline pipeline)
    {
      var document = Markdig.Markdown.Parse(FigureMarkdown, pipeline);
      return MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(document).OfType<Figure>().FirstOrDefault();
    }
  }
}
