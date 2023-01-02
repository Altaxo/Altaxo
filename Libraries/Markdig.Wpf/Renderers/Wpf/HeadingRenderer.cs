// Copyright (c) Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using Markdig.Syntax;
using System;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf
{
    public class HeadingRenderer : WpfObjectRenderer<HeadingBlock>
    {
        protected override void Write(WpfRenderer renderer, HeadingBlock obj)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

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
