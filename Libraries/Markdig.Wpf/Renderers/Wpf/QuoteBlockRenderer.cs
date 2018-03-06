// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System.Windows;
using Markdig.Syntax;
using System.Windows.Documents;
using Markdig.Annotations;
using Markdig.Wpf;

namespace Markdig.Renderers.Wpf
{
    public class QuoteBlockRenderer : WpfObjectRenderer<QuoteBlock>
    {
        /// <inheritdoc/>
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] QuoteBlock obj)
        {
            var section = new Section() { Tag = obj };
            renderer.Styles.ApplyQuoteBlockStyle(section);

            renderer.Push(section);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }
}
