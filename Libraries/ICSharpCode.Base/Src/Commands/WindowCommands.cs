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
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SelectNextWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SD.Workbench.ActiveWorkbenchWindow == null ||
						 SD.Workbench.WorkbenchWindowCollection.Count == 0)
			{
				return;
			}
			int index = SD.Workbench.WorkbenchWindowCollection.IndexOf(SD.Workbench.ActiveWorkbenchWindow);
			SD.Workbench.WorkbenchWindowCollection[(index + 1) % SD.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}

	public class SelectPrevWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SD.Workbench.ActiveWorkbenchWindow == null ||
					SD.Workbench.WorkbenchWindowCollection.Count == 0)
			{
				return;
			}
			int index = SD.Workbench.WorkbenchWindowCollection.IndexOf(SD.Workbench.ActiveWorkbenchWindow);
			SD.Workbench.WorkbenchWindowCollection[(index + SD.Workbench.WorkbenchWindowCollection.Count - 1) % SD.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}

	public class CloseAllWindows : AbstractMenuCommand
	{
		public override void Run()
		{
			SD.Workbench.CloseAllViews();
		}
	}
}