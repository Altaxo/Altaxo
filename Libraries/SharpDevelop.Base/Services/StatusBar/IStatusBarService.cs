// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public interface IStatusBarService
	{
		IProgressMonitor ProgressMonitor {
			get;
		}
		
		Control Control {
			get;
		}
		bool CancelEnabled {
			get;
			set;
		}
		
		bool Visible {
			get;
			set;
		}
		
		void ShowErrorMessage(string message);
		
		void SetMessage(string message);
		void SetMessage(Image image, string message);
		void SetCaretPosition(int x, int y, int charOffset);
		void SetInsertMode(bool insertMode);
		
		void RedrawStatusbar();
	}
}
