// Copyright (c) Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using Markdig.Syntax.Inlines;
using System;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf.Inlines
{
    /// <summary>
    /// A WPF renderer for an <see cref="EmphasisInline"/>.
    /// </summary>
    /// <seealso cref="EmphasisInline" />
    public class EmphasisInlineRenderer : WpfObjectRenderer<EmphasisInline>
    {
        protected override void Write(WpfRenderer renderer, EmphasisInline obj)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            Span? span = null;

            switch (obj.DelimiterChar)
            {
                case '*':
                case '_':
                    span = obj.DelimiterCount == 2 ? (Span)new Bold() : new Italic();
                    break;

                case '~':
                    span = new Span();
                    if (obj.DelimiterCount == 2)
                    {
                        renderer.Styles.ApplyStrikeThroughStyle(span);
                    }
                    else
                    {
                        renderer.Styles.ApplySubscriptStyle(span);
                        if (span.FontSize < 1)
                        {
                            span.FontSize = renderer.CurrentFontSize() * span.FontSize;
                        }
                    }
                    break;

                case '^':
                    span = new Span();
                    renderer.Styles.ApplySuperscriptStyle(span);
                    if (span.FontSize < 1)
                    {
                        span.FontSize = renderer.CurrentFontSize() * span.FontSize;
                    }

                    break;

                case '+':
                    span = new Span();
                    renderer.Styles.ApplyInsertedStyle(span);
                    break;

                case '=':
                    span = new Span();
                    renderer.Styles.ApplyMarkedStyle(span);
                    break;
            }

            if (span is not null)
            {
                span.Tag = obj;
                renderer.Push(span);
                renderer.WriteChildren(obj);
                renderer.Pop();
            }
            else
            {
                renderer.WriteChildren(obj);
            }
        }
    }
}
