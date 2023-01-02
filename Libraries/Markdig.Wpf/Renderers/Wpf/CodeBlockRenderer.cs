// Copyright (c) Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using Markdig.Helpers;
using Markdig.Syntax;
using System;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf
{
    public class CodeBlockRenderer : WpfObjectRenderer<CodeBlock>
    {
        /// <summary>
        /// Create a private tag in order to be able to tag every single line of a code block
        /// </summary>
        /// <seealso cref="Markdig.Syntax.MarkdownObject" />
        private class CodeBlockLine : Markdig.Syntax.Inlines.Inline
        {
            public CodeBlockLine(StringSlice l)
            {
                this.Span = new SourceSpan(l.Start, l.End);
            }
        }

        protected override void Write(WpfRenderer renderer, CodeBlock obj)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            var paragraph = new Paragraph() { Tag = obj };
            renderer.Styles.ApplyCodeBlockStyle(paragraph);
            renderer.Push(paragraph);

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
                        renderer.WriteInline(new Run(slices[i].Slice.ToString()));
                        renderer.WriteInline(new LineBreak());
                    }
                }
            }

            renderer.Pop();
        }
    }
}
