// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 4191 $</version>
// </file>

using System;
using System.Windows.Input;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Custom commands for CodeEditor.
	/// </summary>
	public static class CustomCommands
	{
		public static readonly RoutedCommand CtrlSpaceCompletion = new RoutedCommand(
			"CtrlSpaceCompletion", typeof(CodeEditor),
			new InputGestureCollection {
				new KeyGesture(Key.Space, ModifierKeys.Control)
			});
	}
}
