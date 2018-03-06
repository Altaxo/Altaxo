// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Documents;
using Markdig.Annotations;
using Markdig.Syntax;
using Markdig.Wpf;

namespace Markdig.Renderers.Wpf
{
    public class ThematicBreakRenderer : WpfObjectRenderer<ThematicBreakBlock>
    {
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] ThematicBreakBlock obj)
        {
            var line = new System.Windows.Shapes.Line { X2 = 1 };
            renderer.Styles.ApplyThematicBreakStyle(line);

            var paragraph = new Paragraph
            {
                Tag = obj,
                Inlines = { new InlineUIContainer(line) }
            };

            renderer.WriteBlock(paragraph);
        }
    }
}
