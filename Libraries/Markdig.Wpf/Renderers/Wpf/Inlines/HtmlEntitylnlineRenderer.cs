// Copyright (c) 2018 Dr. Dirk Lellinger. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using Markdig.Annotations;
using Markdig.Syntax.Inlines;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf.Inlines
{
    /// <summary>
    /// A WPF renderer for a <see cref="HtmlEntityInline"/>.
    /// </summary>
    /// <seealso cref="Markdig.Renderers.Wpf.WpfObjectRenderer{Markdig.Syntax.Inlines.HtmlEntityInline}" />
    public class HtmlEntityInlineRenderer : WpfObjectRenderer<HtmlEntityInline>
    {
        /// <inheritdoc/>
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] HtmlEntityInline obj)
        {
            if (obj.Transcoded.IsEmpty)
                return;

            renderer.WriteInline(new Run(obj.Transcoded.ToString()) { Tag = obj });
        }
    }
}
