// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers.Inlines;
using Markdig.Renderers;

namespace Markdig.Extensions.CustomContainers
{
    /// <summary>
    /// Extension to allow custom containers.
    /// </summary>
    /// <seealso cref="IMarkdownExtension" />
    public class CustomContainerExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<CustomContainerParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new CustomContainerParser());
            }

            // Plug the inline parser for CustomContainerInline
            var inlineParser = pipeline.InlineParsers.Find<EmphasisInlineParser>();
            if (inlineParser != null && !inlineParser.HasEmphasisChar(':'))
            {
                inlineParser.EmphasisDescriptors.Add(new EmphasisDescriptor(':', 2, 2, true));
                inlineParser.TryCreateEmphasisInlineList.Add((emphasisChar, delimiterCount) =>
                {
                    if (delimiterCount == 2 && emphasisChar == ':')
                    {
                        return new CustomContainerInline();
                    }
                    return null;
                });
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlCustomContainerRenderer>())
                {
                    // Must be inserted before CodeBlockRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlCustomContainerRenderer());
                }
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlCustomContainerInlineRenderer>())
                {
                    // Must be inserted before EmphasisRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlCustomContainerInlineRenderer());
                }
            }

        }
    }
}