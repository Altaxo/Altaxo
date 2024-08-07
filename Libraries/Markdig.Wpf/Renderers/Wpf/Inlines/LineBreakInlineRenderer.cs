// Copyright (c) Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using Markdig.Syntax.Inlines;
using System;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf.Inlines
{
    /// <summary>
    /// A WPF renderer for a <see cref="LineBreakInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Wpf.WpfObjectRenderer{Markdig.Syntax.Inlines.LineBreakInline}" />
    public class LineBreakInlineRenderer : WpfObjectRenderer<LineBreakInline>
    {
        /// <inheritdoc/>
        protected override void Write(WpfRenderer renderer, LineBreakInline obj)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

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
