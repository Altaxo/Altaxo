// Copyright (c) 2016-2017 Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license.
// See the LICENSE.md file in the project root for more information.

using Markdig.Annotations;
using Markdig.Syntax;

namespace Markdig.Renderers.Xaml
{
    /// <summary>
    /// An XAML renderer for a <see cref="HeadingBlock"/>.
    /// </summary>
    /// <seealso cref="Xaml.XamlObjectRenderer{T}" />
    public class HeadingRenderer : XamlObjectRenderer<HeadingBlock>
    {
        protected override void Write([NotNull] XamlRenderer renderer, [NotNull] HeadingBlock obj)
        {
            renderer.Write("<Paragraph");
            if (obj.Level > 0 && obj.Level <= 6)
            {
                // Apply style depending on heading level
                renderer.Write($" Style=\"{{StaticResource {{x:Static markdig:Styles.Heading{obj.Level}StyleKey}}}}\"");
            }
            renderer.WriteLine(">");
            renderer.WriteLeafInline(obj);
            renderer.EnsureLine();
            renderer.WriteLine("</Paragraph>");
        }
    }
}
