using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Delegate for the <see cref="IGuiTimerService"/>. Note that the argument is the Utc time. If you need local time, you have to convert the
	/// argument to local time.
	/// </summary>
	/// <param name="utcNow">The UTC current time.</param>
	public delegate void GuiTimerServiceHandler(DateTime utcNow);

	/// <summary>
	/// Interface for a timer service that calls back using the Gui thread.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public interface IGuiTimerService : IDisposable
	{
		/// <summary>
		/// A Gui timer tick that occurs every 10 ms.
		/// </summary>
		event GuiTimerServiceHandler TickEvery10ms;

		/// <summary>
		/// A Gui timer tick that occurs every 100 ms.
		/// </summary>
		event GuiTimerServiceHandler TickEvery100ms;

		/// <summary>
		/// A Gui timer tick that occurs every 1000 ms.
		/// </summary>
		event GuiTimerServiceHandler TickEvery1000ms;
	}
}