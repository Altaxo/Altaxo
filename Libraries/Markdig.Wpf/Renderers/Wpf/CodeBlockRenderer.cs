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
    public class CodeBlockRenderer : WpfObjectRenderer<CodeBlock>
    {
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] CodeBlock obj)
        {
            var paragraph = new Paragraph() { Tag = obj };
            renderer.Styles.ApplyCodeBlockStyle(paragraph);
            renderer.Push(paragraph);
            renderer.WriteLeafRawLines(obj);
            renderer.Pop();
        }
    }
}
