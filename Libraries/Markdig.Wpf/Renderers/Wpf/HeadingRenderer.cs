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
    public class HeadingRenderer : WpfObjectRenderer<HeadingBlock>
    {
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] HeadingBlock obj)
        {
            var paragraph = new Paragraph() { Tag = obj };
            switch (obj.Level)
            {
                case 1: renderer.Styles.ApplyHeading1Style(paragraph); break;
                case 2: renderer.Styles.ApplyHeading2Style(paragraph); break;
                case 3: renderer.Styles.ApplyHeading3Style(paragraph); break;
                case 4: renderer.Styles.ApplyHeading4Style(paragraph); break;
                case 5: renderer.Styles.ApplyHeading5Style(paragraph); break;
                case 6: renderer.Styles.ApplyHeading6Style(paragraph); break;
            }

            renderer.Push(paragraph);
            renderer.WriteLeafInline(obj);
            renderer.Pop();
        }
    }
}
