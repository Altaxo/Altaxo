// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using Markdig.Syntax.Inlines;
using System.Windows.Documents;
using Markdig.Annotations;

namespace Markdig.Renderers.Wpf.Inlines
{
    /// <summary>
    /// A WPF renderer for a <see cref="LineBreakInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Wpf.WpfObjectRenderer{Markdig.Syntax.Inlines.LineBreakInline}" />
    public class LineBreakInlineRenderer : WpfObjectRenderer<LineBreakInline>
    {
        /// <inheritdoc/>
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] LineBreakInline obj)
        {
            if (obj.IsHard)
            {
                renderer.WriteInline(new LineBreak() { Tag = obj });
            }
            else
            {
                // neither hard nor backslash -> but we have to add a space at least
                renderer.WriteInline(new Run(" ") { Tag = obj });
            }
        }
    }
}
