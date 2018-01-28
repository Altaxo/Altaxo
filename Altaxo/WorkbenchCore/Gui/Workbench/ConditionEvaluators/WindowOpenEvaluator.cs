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

using System;

using Altaxo.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Tests if a window of a specified type or implementing an interface is open.
	/// The window does not need to be the active window.
	/// </summary>
	/// <attribute name="openwindow">
	/// The fully qualified name of the type the window should be or the
	/// interface name it should implement.
	/// "*" to test if any window is open.
	/// </attribute>
	/// <example title="Test if a text editor is opened">
	/// &lt;Condition name="WindowOpen" openwindow="Altaxo.Gui.ITextEditor"&gt;
	/// </example>
	/// <example title="Test if any window is open">
	/// &lt;Condition name="WindowOpen" openwindow="*"&gt;
	/// </example>
	public class WindowOpenConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			var workbench = Altaxo.Current.GetService<Workbench.IWorkbenchEx>();
			if (!(caller is IViewContent activeViewContent)) // active view content is coming from the data context of the menu
				activeViewContent = workbench.ActiveViewContent; // else active view content is retrieved from the workbench

			string openWindow = condition.Properties["openwindow"];

			Type openWindowType = condition.AddIn.FindType(openWindow);
			if (openWindowType == null)
			{
				Current.Log.WarnFormatted("WindowOpenCondition: cannot find Type {0}", openWindow);
				return false;
			}

			// ask the active view content, if it has a sub-content of the given window type
			if (null != activeViewContent?.GetService(openWindowType))
				return true;

			if (openWindow == "*")
			{
				return activeViewContent != null;
			}

			foreach (IViewContent view in workbench.ViewContentCollection)
			{
				Type currentType = view.GetType();
				if (currentType.ToString() == openWindow)
				{
					return true;
				}
				foreach (Type i in currentType.GetInterfaces())
				{
					if (i.ToString() == openWindow)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}