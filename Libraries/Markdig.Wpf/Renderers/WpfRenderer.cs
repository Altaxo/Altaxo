// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using Markdig.Annotations;
using Markdig.Helpers;
using Markdig.Renderers.Wpf;
using Markdig.Renderers.Wpf.Extensions;
using Markdig.Renderers.Wpf.Inlines;
using Markdig.Syntax;
using Markdig.Wpf;
using Block = System.Windows.Documents.Block;

namespace Markdig.Renderers
{
    /// <summary>
    /// WPF renderer for a Markdown <see cref="MarkdownDocument"/> object.
    /// </summary>
    /// <seealso cref="RendererBase" />
    public class WpfRenderer : RendererBase
    {
        private readonly Stack<IAddChild> stack = new Stack<IAddChild>();
        private char[] buffer;
        public IStyles Styles { get; private set; }

        private IWpfImageProvider _imageProvider = WpfImageProviderBase.Instance;

        /// <summary>
        /// Gets or sets the image provider, i.e. a hook to provide images to the renderer.
        /// </summary>
        /// <value>
        /// The image provider. If set to null, the default image provider will be used.
        /// </value>
        public IWpfImageProvider ImageProvider
        {
            get
            {
                return _imageProvider;
            }
            set
            {
                _imageProvider = value ?? WpfImageProviderBase.Instance;
            }
        }

        public WpfRenderer([NotNull] IAddChild document)
            : this(document, null)
        {
        }

        public WpfRenderer([NotNull] IAddChild document, IStyles styles)
        {
            buffer = new char[1024];
            Styles = styles ?? Markdig.Wpf.DynamicStyles.Instance;
            Document = document;
            if (document is TextElement teDocument)
                Styles.ApplyDocumentStyle(teDocument);
            stack.Push(document);

            // Extension renderers that must be registered before the default renders
            ObjectRenderers.Add(new MathBlockRenderer()); // since MathBlock derives from CodeBlock, it must be registered before CodeBlockRenderer

            // Default block renderers
            ObjectRenderers.Add(new CodeBlockRenderer());
            ObjectRenderers.Add(new ListRenderer());
            ObjectRenderers.Add(new HeadingRenderer());
            ObjectRenderers.Add(new ParagraphRenderer());
            ObjectRenderers.Add(new QuoteBlockRenderer());
            ObjectRenderers.Add(new ThematicBreakRenderer());

            // Default inline renderers
            ObjectRenderers.Add(new AutolinkInlineRenderer());
            ObjectRenderers.Add(new CodeInlineRenderer());
            ObjectRenderers.Add(new DelimiterInlineRenderer());
            ObjectRenderers.Add(new EmphasisInlineRenderer());
            ObjectRenderers.Add(new LineBreakInlineRenderer());
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new LiteralInlineRenderer());

            // Extension renderers
            ObjectRenderers.Add(new TableRenderer());
            ObjectRenderers.Add(new TaskListRenderer());
            ObjectRenderers.Add(new MathInlineRenderer());
        }

        public IAddChild Document { get; }

        /// <inheritdoc/>
        public override object Render([NotNull] MarkdownObject markdownObject)
        {
            Write(markdownObject);
            return Document;
        }

        public object Render(IList<MarkdownObject> markdownObjects)
        {
            foreach (var markdownObject in markdownObjects)
                Write(markdownObject);
            return Document;
        }

        /// <summary>
        /// Writes the inlines of a leaf inline.
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        /// <returns>This instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLeafInline([NotNull] LeafBlock leafBlock)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
            var inline = (Syntax.Inlines.Inline)leafBlock.Inline;
            while (inline != null)
            {
                Write(inline);
                inline = inline.NextSibling;
            }
        }

        /// <summary>
        /// Writes the lines of a <see cref="LeafBlock"/>
        /// </summary>
        /// <param name="leafBlock">The leaf block.</param>
        public void WriteLeafRawLines([NotNull] LeafBlock leafBlock)
        {
            if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
            if (leafBlock.Lines.Lines != null)
            {
                var lines = leafBlock.Lines;
                var slices = lines.Lines;
                for (var i = 0; i < lines.Count; i++)
                {
                    WriteText(ref slices[i].Slice);
                    WriteInline(new LineBreak());
                }
            }
        }

        internal void Push([NotNull] IAddChild o)
        {
            var pred = stack.Peek();
            stack.Push(o);
            pred.AddChild(o);
        }

        internal void Pop()
        {
            var popped = stack.Pop();
        }

        internal double CurrentFontSize()
        {
            if (stack.Peek() is TextElement te)
                return te.FontSize;
            else if (stack.Peek() is FlowDocument flowDoc)
                return flowDoc.FontSize;
            else
                return 0;
        }

        internal void WriteBlock([NotNull] Block block)
        {
            stack.Peek().AddChild(block);
        }

        internal void WriteInline([NotNull] Inline inline)
        {
            AddInline(stack.Peek(), inline);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteText(ref StringSlice slice)
        {
            if (slice.Start > slice.End)
                return;

            WriteText(slice.Text, slice.Start, slice.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteText([CanBeNull] string text)
        {
            WriteInline(new Run(text));
        }

        internal void WriteText([CanBeNull] string text, int offset, int length)
        {
            if (text == null)
                return;

            if (offset == 0 && text.Length == length)
            {
                WriteText(text);
            }
            else
            {
                if (length > buffer.Length)
                {
                    buffer = text.ToCharArray();
                    WriteText(new string(buffer, offset, length));
                }
                else
                {
                    text.CopyTo(offset, buffer, 0, length);
                    WriteText(new string(buffer, 0, length));
                }
            }
        }

        private static void AddInline([NotNull] IAddChild parent, [NotNull] Inline inline)
        {
            if (!EndsWithSpace(parent) && !StartsWithSpace(inline))
            {
                //parent.AddText(" ");
            }

            parent.AddChild(inline);
        }

        private static bool StartsWithSpace([NotNull] Inline inline)
        {
            while (true)
            {
                if (inline is Run run)
                {
                    return run.Text.Length == 0 || run.Text.First().IsWhitespace();
                }
                if (inline is Span span)
                {
                    inline = span.Inlines.FirstInline;
                    continue;
                }

                return true;
            }
        }

        private static bool EndsWithSpace([NotNull] IAddChild element)
        {
            while (true)
            {
                var inlines = (element as Span)?.Inlines ?? (element as Paragraph)?.Inlines;

                if (inlines?.LastInline is Run run)
                {
                    return run.Text.Length == 0 || run.Text.Last().IsWhitespace();
                }
                if (inlines?.LastInline is Span span)
                {
                    element = span;
                    continue;
                }

                return true;
            }
        }
    }
}
