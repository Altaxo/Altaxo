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

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// The IPadContent interface is interface of the viewmodel of all "tool" windows.</summary>
	public interface IPadContent : IWorkbenchContent, IDisposable
	{
		/// <summary>
		/// Returns the category (this is used for defining where the menu item to
		/// this pad goes)
		/// </summary>
		string Category { get; set; }

		/// <summary>
		/// Returns the icon bitmap resource name of the pad. May be an empty string
		/// if the pad has no icon defined.
		/// </summary>
		string IconSource { get; }

		/// <summary>
		/// Returns the menu shortcut for the view menu item.
		/// </summary>
		string Shortcut { get; set; }

		/// <summary>
		/// Gets a string which uniquely identifies the content.
		/// </summary>
		string ContentId { get; }

		/// <summary>
		/// Gets the default position of this pad.
		/// </summary>
		/// <value>
		/// The default pad position.
		/// </value>
		DefaultPadPositions DefaultPosition { get; }

		/// <summary>
		/// Gets or sets the pad descriptor that has created this pad content.
		/// </summary>
		/// <value>
		/// The pad descriptor.
		/// </value>
		PadDescriptor PadDescriptor { get; set; }
	}
}