// Copyright (c) 2018 Dr. Dirk Lellinger. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Documents;

namespace Markdig.Wpf
{
    public interface IStyles
    {
        void ApplyCodeStyle(TextElement element);

        void ApplyCodeBlockStyle(TextElement element);

        void ApplyDocumentStyle(FrameworkContentElement element);

        void ApplyHeading1Style(TextElement element);

        void ApplyHeading2Style(TextElement element);

        void ApplyHeading3Style(TextElement element);

        void ApplyHeading4Style(TextElement element);

        void ApplyHeading5Style(TextElement element);

        void ApplyHeading6Style(TextElement element);

        void ApplyHyperlinkStyle(TextElement element);

        void ApplyImageStyle(FrameworkElement element);

        void ApplyInsertedStyle(TextElement element);

        void ApplyListStyle(TextElement element);

        void ApplyMarkedStyle(TextElement element);

        void ApplyParagraphStyle(TextElement element);

        void ApplyQuoteBlockStyle(TextElement element);

        void ApplyStrikeThroughStyle(TextElement element);

        void ApplySubscriptStyle(TextElement element);

        void ApplySuperscriptStyle(TextElement element);

        void ApplyTableStyle(TextElement element);

        void ApplyTableCellStyle(TextElement element);

        void ApplyTableHeaderStyle(TextElement element);

        void ApplyTaskListStyle(FrameworkElement element);

        void ApplyThematicBreakStyle(FrameworkElement element);
    }
}
