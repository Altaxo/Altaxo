// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax;

namespace Markdig.Renderers.Html;

/// <summary>
/// A HTML renderer for a <see cref="ParagraphBlock"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{ParagraphBlock}" />
public class ParagraphRenderer : HtmlObjectRenderer<ParagraphBlock>
{
    protected override void Write(HtmlRenderer renderer, ParagraphBlock obj)
    {
        if (!renderer.ImplicitParagraph && renderer.EnableHtmlForBlock)
        {
            if (!renderer.IsFirstInContainer)
            {
                renderer.EnsureLine();
            }

            renderer.Write("<p");
            renderer.WriteAttributes(obj);
            renderer.WriteRaw('>');
        }
        renderer.WriteLeafInline(obj);
        if (!renderer.ImplicitParagraph)
        {
            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</p>");
            }

            renderer.EnsureLine();
        }
    }
}