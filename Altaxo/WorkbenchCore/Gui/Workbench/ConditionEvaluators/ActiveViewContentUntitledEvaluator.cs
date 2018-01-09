﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using Altaxo.AddInItems;

namespace Altaxo.Gui.Workbench
{
    /// <summary>
    /// Tests if the active view content is untitled.
    /// </summary>
    /// <attributes name="activewindowuntitled">Boolean value to test against.</attributes>
    /// <example title="Test if the active view content is untitled">
    /// &lt;Condition name = "ActiveViewContentUntitled" activewindowuntitled="True"&gt;
    /// - or -
    /// &lt;Condition name = "ActiveViewContentUntitled"&gt;
    /// </example>
    /// <example title="Test if the active view content has a title">
    /// &lt;Condition name = "ActiveViewContentUntitled" activewindowuntitled="False"&gt;
    /// </example>
    public class ActiveViewContentUntitledConditionEvaluator : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            var workbench = Altaxo.Current.GetService<Workbench.IWorkbenchEx>();
            if (workbench == null)
            {
                return false;
            }

            var viewContent = workbench.ActiveViewContent as IFileViewContent;
            if (viewContent == null || viewContent.PrimaryFile == null)
            {
                return false;
            }

            if (!condition.Properties.Contains("activewindowuntitled"))
                return viewContent.PrimaryFile.IsUntitled;
            bool activewindowuntitled = Boolean.Parse(condition.Properties["activewindowuntitled"]);
            return viewContent.PrimaryFile.IsUntitled == activewindowuntitled;
        }
    }
}