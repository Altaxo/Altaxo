using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Main
{
	/// <summary>
	/// Invokes an action when it is disposed.
	/// </summary>
	/// <remarks>
	/// This class ensures the callback is invoked at most once,
	/// even when Dispose is called on multiple threads.
	/// </remarks>
	public sealed class CallbackOnDispose : IDisposable
	{
		private Action _action;

		public CallbackOnDispose(Action action)
		{
			this._action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _action, null)?.Invoke();
		}
	}
}