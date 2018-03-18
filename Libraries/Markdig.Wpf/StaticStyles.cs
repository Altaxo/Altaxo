// Copyright (c) 2018 Dr. Dirk Lellinger. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Documents;

namespace Markdig.Wpf
{
    /// <summary>
    /// This class applies styles statically to the items of a flow document. That means that the appearance of the flow document's items
    /// remain the same, even if the global resources (see <see cref="Application.Current.Resources"/>) change.
    /// </summary>
    /// <seealso cref="Markdig.Wpf.IStyles" />
    public class StaticStyles : Styles, IStyles
    {
        private ResourceDictionary _resources;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticStyles"/> class.
        /// </summary>
        /// <param name="resources">The resource dictionary containing the styles for the flow document's items.</param>
        /// <exception cref="ArgumentNullException">resources</exception>
        public StaticStyles(ResourceDictionary resources)
        {
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public void ApplyCodeBlockStyle(TextElement element)
        {
            element.Style = (Style)_resources[CodeBlockStyleKey];
        }

        public void ApplyCodeStyle(TextElement element)
        {
            element.Style = (Style)_resources[CodeStyleKey];
        }

        public void ApplyDocumentStyle(TextElement element)
        {
            element.Style = (Style)_resources[DocumentStyleKey];
        }

        public void ApplyHeading1Style(TextElement element)
        {
            element.Style = (Style)_resources[Heading1StyleKey];
        }

        public void ApplyHeading2Style(TextElement element)
        {
            element.Style = (Style)_resources[Heading2StyleKey];
        }

        public void ApplyHeading3Style(TextElement element)
        {
            element.Style = (Style)_resources[Heading3StyleKey];
        }

        public void ApplyHeading4Style(TextElement element)
        {
            element.Style = (Style)_resources[Heading4StyleKey];
        }

        public void ApplyHeading5Style(TextElement element)
        {
            element.Style = (Style)_resources[Heading5StyleKey];
        }

        public void ApplyHeading6Style(TextElement element)
        {
            element.Style = (Style)_resources[Heading6StyleKey];
        }

        public void ApplyHyperlinkStyle(TextElement element)
        {
            element.Style = (Style)_resources[HyperlinkStyleKey];
        }

        public void ApplyImageStyle(FrameworkElement element)
        {
            element.Style = (Style)_resources[ImageStyleKey];
        }

        public void ApplyInsertedStyle(TextElement element)
        {
            element.Style = (Style)_resources[InsertedStyleKey];
        }

        public void ApplyListStyle(TextElement element)
        {
            element.Style = (Style)_resources[ListStyleKey];
        }

        public void ApplyMarkedStyle(TextElement element)
        {
            element.Style = (Style)_resources[MarkedStyleKey];
        }

        public void ApplyParagraphStyle(TextElement element)
        {
            element.Style = (Style)_resources[ParagraphStyleKey];
        }

        public void ApplyQuoteBlockStyle(TextElement element)
        {
            element.Style = (Style)_resources[QuoteBlockStyleKey];
        }

        public void ApplyStrikeThroughStyle(TextElement element)
        {
            element.Style = (Style)_resources[StrikeThroughStyleKey];
        }

        public void ApplySubscriptStyle(TextElement element)
        {
            element.Style = (Style)_resources[SubscriptStyleKey];
        }

        public void ApplySuperscriptStyle(TextElement element)
        {
            element.Style = (Style)_resources[SuperscriptStyleKey];
        }

        public void ApplyTableCellStyle(TextElement element)
        {
            element.Style = (Style)_resources[TableCellStyleKey];
        }

        public void ApplyTableHeaderStyle(TextElement element)
        {
            element.Style = (Style)_resources[TableHeaderStyleKey];
        }

        public void ApplyTableStyle(TextElement element)
        {
            element.Style = (Style)_resources[TableStyleKey];
        }

        public void ApplyTaskListStyle(FrameworkElement element)
        {
            element.Style = (Style)_resources[TaskListStyleKey];
        }

        public void ApplyThematicBreakStyle(FrameworkElement element)
        {
            element.Style = (Style)_resources[ThematicBreakStyleKey];
        }
    }
}
