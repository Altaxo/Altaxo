// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Workbench;

namespace Altaxo.Gui.Workbench.Commands
{
    public class SelectNextWindow : AbstractMenuCommand
    {
        public override void Run()
        {
            var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();

            if (workbench.ActiveViewContent == null ||
                         workbench.ViewContentCollection.Count == 0)
            {
                return;
            }
            int index = workbench.ViewContentCollection.IndexOf(workbench.ActiveViewContent);
            workbench.ViewContentCollection[(index + 1) % workbench.ViewContentCollection.Count].IsSelected = true;
        }
    }

    public class SelectPrevWindow : AbstractMenuCommand
    {
        public override void Run()
        {
            var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
            if (workbench.ActiveViewContent == null ||
                    workbench.ViewContentCollection.Count == 0)
            {
                return;
            }
            int index = workbench.ViewContentCollection.IndexOf(workbench.ActiveViewContent);
            workbench.ViewContentCollection[(index + workbench.ViewContentCollection.Count - 1) % workbench.ViewContentCollection.Count].IsSelected = true;
        }
    }

    public class CloseAllWindows : AbstractMenuCommand
    {
        public override void Run()
        {
            var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
            workbench.CloseAllViews();
        }
    }

    public class CloseFileTab : AbstractMenuCommand
    {
        public override void Run()
        {
            var window = Owner as IWorkbenchWindow;
            if (window != null)
            {
                window.CloseWindow(false);
            }
        }
    }

    public class CloseAllButThisFileTab : AbstractMenuCommand
    {
        public override void Run()
        {
            var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();

            var thisWindow = Owner as IViewContent;
            for (int i = workbench.ViewContentCollection.Count - 1; i >= 0; --i)
            {
                var window = workbench.ViewContentCollection[i];
                if (window != thisWindow)
                {
                    if (!window.CloseCommand.CanExecute(false))
                        break;
                }
            }
        }
    }
}