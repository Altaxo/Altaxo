// Copyright (c) 2018 Dr. Dirk Lellinger. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Documents;

namespace Markdig.Wpf
{
    /// <summary>
    /// This style class assigns styles not directly, but by reference. This means that any change in
    /// <see cref="Application.Current.Resources"/> immediately reflects in the appearance of the flow document's items.
    /// Use <see cref="Instance"/> to get an instance of this class.
    /// </summary>
    public class DynamicStyles : Styles, IStyles
    {
        public static DynamicStyles Instance { get; protected set; } = new DynamicStyles();

        protected DynamicStyles()
        {
        }

        public void ApplyCodeBlockStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, CodeBlockStyleKey);
        }

        public void ApplyCodeStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, CodeStyleKey);
        }

        public void ApplyDocumentStyle(FrameworkContentElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, DocumentStyleKey);
        }

        public void ApplyHeading1Style(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, Heading1StyleKey);
        }

        public void ApplyHeading2Style(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, Heading2StyleKey);
        }

        public void ApplyHeading3Style(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, Heading3StyleKey);
        }

        public void ApplyHeading4Style(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, Heading4StyleKey);
        }

        public void ApplyHeading5Style(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, Heading5StyleKey);
        }

        public void ApplyHeading6Style(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, Heading6StyleKey);
        }

        public void ApplyHyperlinkStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, HyperlinkStyleKey);
        }

        public void ApplyImageStyle(FrameworkElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, ImageStyleKey);
        }

        public void ApplyInsertedStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, InsertedStyleKey);
        }

        public void ApplyListStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, ListStyleKey);
        }

        public void ApplyMarkedStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, MarkedStyleKey);
        }

        public void ApplyParagraphStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, ParagraphStyleKey);
        }

        public void ApplyQuoteBlockStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, QuoteBlockStyleKey);
        }

        public void ApplyStrikeThroughStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, StrikeThroughStyleKey);
        }

        public void ApplySubscriptStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, SubscriptStyleKey);
        }

        public void ApplySuperscriptStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, SuperscriptStyleKey);
        }

        public void ApplyTableCellStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, TableCellStyleKey);
        }

        public void ApplyTableHeaderStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, TableHeaderStyleKey);
        }

        public void ApplyTableStyle(TextElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, TableStyleKey);
        }

        public void ApplyTaskListStyle(FrameworkElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, TaskListStyleKey);
        }

        public void ApplyThematicBreakStyle(FrameworkElement element)
        {
            element.SetResourceReference(FrameworkContentElement.StyleProperty, ThematicBreakStyleKey);
        }
    }
}
