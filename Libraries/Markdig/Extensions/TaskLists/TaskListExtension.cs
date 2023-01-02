// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Normalize;

namespace Markdig.Extensions.TaskLists
{
    /// <summary>
    /// Extension to enable TaskList.
    /// </summary>
    public class TaskListExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<TaskListInlineParser>())
            {
                // Insert the parser after the code span parser
                pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new TaskListInlineParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlTaskListRenderer>();
            }

            if (renderer is NormalizeRenderer normalizeRenderer)
            {
                normalizeRenderer.ObjectRenderers.AddIfNotAlready<NormalizeTaskListRenderer>();
            }
        }
    }
}