// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui 
{
	/// <summary>
	/// The IPadContent interface is the basic interface to all "tool" windows
	/// in SharpDevelop.
	/// </summary>
	public interface IPadContent : IDisposable
	{
		/// <summary>
		/// Returns the title of the pad.
		/// </summary>
		string Title {
			get;
		}
		
		/// <summary>
		/// Returns the icon bitmap resource name of the pad. May be null, if the pad has no
		/// icon defined.
		/// </summary>
		string Icon {
			get;
		}
#if !LINUX
		/// <summary>
		/// Returns the Windows.Control for this pad.
		/// </summary>
		Control Control {
			get;
		}
#endif
		
		/// <summary>
		/// Re-initializes all components of the pad. Don't call unless
		/// you know what you do.
		/// </summary>
		void RedrawContent();
		
		/// <summary>
		/// Is called when the title of this pad has changed.
		/// </summary>
		event EventHandler TitleChanged;
		
		/// <summary>
		/// Is called when the icon of this pad has changed.
		/// </summary>
		event EventHandler IconChanged;
		
		/// <summary>
		/// Tries to make the pad visible to the user.
		/// </summary>
		void BringToFront();
	}
}
