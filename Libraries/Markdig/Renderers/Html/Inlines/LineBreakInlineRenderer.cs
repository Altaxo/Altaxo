// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace Markdig.Renderers.Html.Inlines;

/// <summary>
/// A HTML renderer for a <see cref="LineBreakInline"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{LineBreakInline}" />
public class LineBreakInlineRenderer : HtmlObjectRenderer<LineBreakInline>
{
    /// <summary>
    /// Gets or sets a value indicating whether to render this softline break as a HTML hardline break tag (&lt;br /&gt;)
    /// </summary>
    public bool RenderAsHardlineBreak { get; set; }

    protected override void Write(HtmlRenderer renderer, LineBreakInline obj)
    {
        if (renderer.IsLastInContainer) return;

        if (renderer.EnableHtmlForInline)
        {
            if (obj.IsHard || RenderAsHardlineBreak)
            {
                renderer.WriteLine("<br />");
            }
        }

        renderer.EnsureLine();
    }
}