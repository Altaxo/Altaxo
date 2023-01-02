// Copyright (c) Nicolas Musset. All rights reserved.
// This file is licensed under the MIT license. 
// See the LICENSE.md file in the project root for more information.

using Markdig.Extensions.TaskLists;
using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Markdig.Renderers.Wpf.Extensions
{
    public class TaskListRenderer : WpfObjectRenderer<TaskList>
    {
        protected override void Write(WpfRenderer renderer, TaskList taskList)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (taskList == null)
            {
                throw new ArgumentNullException(nameof(taskList));
            }

            var checkBox = new CheckBox
            {
                IsEnabled = false,
                IsChecked = taskList.Checked
            };

            renderer.Styles.ApplyTaskListStyle(checkBox);
            renderer.WriteInline(new InlineUIContainer(checkBox) { Tag = taskList });
        }
    }
}
