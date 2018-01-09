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

using System.Collections.Generic;
using System;
using System.Threading;
using Altaxo.Main.Services;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Minimum requirements to a status bar.
	/// </summary>
	[GlobalService("StatusBar")]
	public interface IStatusBarService : IDisposable
	{
		/// <summary>
		/// Sets text that is displayed in the right corner of the status bar. This is intended for volative text, like clock time.
		/// Do not use it for static text messages.
		/// </summary>
		/// <param name="text">The text.</param>
		void SetRightCornerText(string text);

		/// <summary>
		/// Sets the message shown in the left-most pane in the status bar.
		/// </summary>
		/// <param name="message">The message text.</param>
		/// <param name="highlighted">Whether to highlight the text</param>
		/// <param name="icon">Icon to show next to the text. If this parameter is a string, it will be interpreted as a resource string.</param>
		void SetMessage(string message, bool highlighted = false, object icon = null);

		/// <summary>
		/// Creates a new <see cref="IProgressMonitor"/> that can be used to report
		/// progress to the status bar.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token to use for
		/// <see cref="IProgressReporter.CancellationToken"/></param>
		/// <returns>The new IProgressMonitor instance. This return value must be disposed
		/// once the background task has completed.</returns>
		IProgressReporter CreateProgressReporter(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Shows progress for the specified ProgressCollector in the status bar.
		/// </summary>
		void AddProgress(ProgressCollector progress);
	}
}