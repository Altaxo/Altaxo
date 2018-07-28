#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// This postprocessor processes fenced code blocks. If it finds one, the lines inside the block
  /// are resolved to <see cref="LiteralInline"/>s and <see cref="LineBreakInline"/>s, which have valid spans.
  /// The purpose of this is that later during Wpf rendering, the <see cref="LiteralInline"/>s will be converted
  /// to Runs which are tagged then with the appropriate <see cref="LiteralInline"/>, so that the text location can
  /// be precisely resolved.
  /// </summary>
  public static class FencedCodeBlockLineTaggingPostProcessor
  {
    /// <summary>
    /// Executes the post-processor.
    /// </summary>
    /// <param name="document">The markdown document.</param>
    public static void PostProcess(MarkdownDocument document)
    {
      PostProcessContainerBlock(document);
    }

    /// <summary>
    /// Executes the post-processor recursively for all container blocks.
    /// </summary>
    /// <param name="containerBlock">The container block.</param>
    private static void PostProcessContainerBlock(ContainerBlock containerBlock)
    {
      foreach (var block in containerBlock) // Note: as long as we post-process fenced code blocks, we need to parse only the first level blocks
      {
        if (block is FencedCodeBlock fcb)
        {
          fcb.Inline = new ContainerInline(); // in order to add custom tags to each line, we have to add a container to this CodeBlock

          // original code: renderer.WriteLeafRawLines(obj); // Expand this call directly here in order to be able to include tags
          var lines = fcb.Lines;
          if (lines.Lines != null)
          {
            var slices = lines.Lines;
            for (var i = 0; i < lines.Count; i++)
            {
              var slice = slices[i].Slice;
              fcb.Inline.AppendChild(new LiteralInline(slice) { Span = new SourceSpan(slice.Start, slice.End) }); // insert this in the newly created container inline
              fcb.Inline.AppendChild(new LineBreakInline() { IsHard = true, Span = new SourceSpan(slice.End + 1, slice.End) });
            }
          }
        }
        else if (block is ContainerBlock childContainerBlock)
        {
          PostProcessContainerBlock(childContainerBlock);
        }
      }
    }

    /// <summary>
    /// Registers this post processor in the pipeline
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <returns>The modified pipeline</returns>
    public static MarkdownPipelineBuilder UseFencedCodeBlockLineTaggingPostProcessor(this MarkdownPipelineBuilder pipeline)
    {
      if (pipeline == null)
        throw new ArgumentNullException(nameof(pipeline));
      pipeline.DocumentProcessed += PostProcess;
      return pipeline;
    }
  }
}
