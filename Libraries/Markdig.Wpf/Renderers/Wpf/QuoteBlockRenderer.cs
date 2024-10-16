// Copyright (c) Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using Markdig.Syntax;
using System;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf
{
    public class QuoteBlockRenderer : WpfObjectRenderer<QuoteBlock>
    {
        /// <inheritdoc/>
        protected override void Write(WpfRenderer renderer, QuoteBlock obj)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var section = new Section() { Tag = obj };
            renderer.Styles.ApplyQuoteBlockStyle(section);

            renderer.Push(section);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }
}
