// Copyright (c) Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using Markdig.Syntax;
using System;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf
{
    public class ThematicBreakRenderer : WpfObjectRenderer<ThematicBreakBlock>
    {
        protected override void Write(WpfRenderer renderer, ThematicBreakBlock obj)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

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
