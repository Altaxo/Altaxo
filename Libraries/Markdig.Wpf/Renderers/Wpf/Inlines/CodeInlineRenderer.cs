// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Markdig.Annotations;
using Markdig.Syntax.Inlines;
using Markdig.Wpf;

namespace Markdig.Renderers.Wpf.Inlines
{
    public class CodeInlineRenderer : WpfObjectRenderer<CodeInline>
    {
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] CodeInline obj)
        {
            var span = new Span() { Tag = obj };
            renderer.Styles.ApplyCodeStyle(span);

            var run = new Run("\x202F"); // Narrow fixed space
            run.FontStretch = FontStretches.UltraCondensed;
            span.Inlines.Add(run);

            run = new Run(obj.Content.Replace(" ", "\xA0")) { Tag = obj }; // Replace Space with fixed space
            span.Inlines.Add(run);

            run = new Run("\x202F"); // Narrow fixed space
            run.FontStretch = FontStretches.UltraCondensed; // has an effect only for non fixed space fonts
            span.Inlines.Add(run);

            renderer.WriteInline(span);
        }
    }
}
