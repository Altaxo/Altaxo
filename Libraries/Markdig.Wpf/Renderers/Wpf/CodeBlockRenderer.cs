// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Documents;
using Markdig.Annotations;
using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Wpf;

namespace Markdig.Renderers.Wpf
{
    public class CodeBlockRenderer : WpfObjectRenderer<CodeBlock>
    {
        /// <summary>
        /// Create a private tag in order to be able to tag every single line of a code block
        /// </summary>
        /// <seealso cref="Markdig.Syntax.MarkdownObject" />
        private class CodeBlockLine : MarkdownObject
        {
            public CodeBlockLine(StringSlice l)
            {
                this.Span = new SourceSpan(l.Start, l.End);
            }
        }

        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] CodeBlock obj)
        {
            var paragraph = new Paragraph() { Tag = obj };
            renderer.Styles.ApplyCodeBlockStyle(paragraph);
            renderer.Push(paragraph);

            // original code: renderer.WriteLeafRawLines(obj); // Expand this call directly here in order to be able to include tags
            var lines = obj.Lines;
            if (lines.Lines != null)
            {
                var slices = lines.Lines;
                for (var i = 0; i < lines.Count; i++)
                {
                    renderer.WriteInline(new Run(slices[i].Slice.ToString()) { Tag = new CodeBlockLine(slices[i].Slice) });
                    renderer.WriteInline(new LineBreak());
                }
            }

            renderer.Pop();
        }
    }
}
