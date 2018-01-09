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

using System.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Represents a thread running a message loop.
	/// </summary>
	[GlobalService("MainThread", FallbackImplementation = typeof(FakeMessageLoop))]
	public interface IDispatcherMessageLoopWpf : IDispatcherMessageLoop
	{
		/// <summary>
		/// Gets the dispatcher for this message loop.
		/// </summary>
		Dispatcher Dispatcher { get; }

		#region InvokeIfRequired (Actions)

		/// <inheritdoc see="Invoke(Action)"/>
		void InvokeIfRequired(Action callback, DispatcherPriority priority);

		/// <inheritdoc see="Invoke(Action)"/>
		void InvokeIfRequired(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

		#endregion InvokeIfRequired (Actions)

		#region InvokeIfRequired (Functions)

		/// <inheritdoc see="Invoke{T}(Func{T})"/>
		T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority);

		/// <inheritdoc see="Invoke{T}(Func{T})"/>
		T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken);

		#endregion InvokeIfRequired (Functions)

		#region InvokeAndForget (Actions)

		/// <inheritdoc see="InvokeAsyncAndForget(Action)"/>
		void InvokeAndForget(Action callback, DispatcherPriority priority);

		#endregion InvokeAndForget (Actions)

		#region InvokeAsync

		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task InvokeAsync(Action callback, DispatcherPriority priority);

		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);

		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority);

		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken);

		#endregion InvokeAsync
	}
}