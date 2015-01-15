#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>Helper class to suspend and resume change events (or other events). This class keeps a counter variable (suspend counter), which is incremented when a call to <see cref="SuspendGetToken"/> has been made, and is
	/// decremented when the suspend token returned by this call is disposed. Although you can use this class as it is - to keep track of the number of suspends - it is designed to be used in derived classes.
	/// See the remarks on details of the functions you should override.
	/// </summary>
	/// <remarks>
	/// There are four functions which can be overridden. In this class, these functions do nothing.
	/// <list type="List">
	/// <item><see cref="OnSuspended"/> is called when the suspend counter has been incremented from 0 to 1, i.e. the instance is suspended now.</item>
	/// <item><see cref="OnAboutToBeResumed"/> is called when the suspend counter is still 1, i.e. the instance is still suspended, but the suspend counter will afterwards be decremented to 0.</item>
	/// <item><see cref="OnResume"/> is called when the suspend counter has been decremented to 0, i.e. the instance is resumed now, and should fire events again. It should also fire a change event if something has changed during the suspended state.</item>
	/// <item><see cref="OnResumeSilently"/> is called when the suspend counter has been decremented to 0 as result of a call to <see cref="ISuspendToken.ResumeSilently()"/></item>
	/// </list>
	/// </remarks>
	public class SuspendableObject : ISuspendableByToken
	{
		#region Inner class SuspendToken

		private class SuspendToken : ISuspendToken
		{
			private SuspendableObject _parent;

			internal SuspendToken(SuspendableObject parent)
			{
				var suspendLevel = System.Threading.Interlocked.Increment(ref parent._suspendLevel);
				_parent = parent;

				if (1 == suspendLevel)
				{
					try
					{
						_parent.OnSuspended();
					}
					catch (Exception)
					{
						System.Threading.Interlocked.Decrement(ref parent._suspendLevel);
						_parent = null;
						throw;
					}
				}
			}

			~SuspendToken()
			{
				Dispose();
			}

			/// <summary>
			/// Disarms this SuppressToken so that it can not raise the resume event anymore.
			/// </summary>
			public void ResumeSilently()
			{
				var parent = System.Threading.Interlocked.Exchange<SuspendableObject>(ref _parent, null);
				if (parent != null)
				{
					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

					if (0 == newLevel)
					{
						try
						{
							parent.EhResumeSilently();
						}
						finally
						{
						}
					}
					else if (newLevel < 0)
					{
						throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
					}
				}
			}

			public void Resume()
			{
				Dispose();
			}

			public void Resume(EventFiring eventFiring)
			{
				switch (eventFiring)
				{
					case EventFiring.Enabled:
						Resume();
						break;

					case EventFiring.Suppressed:
						ResumeSilently();
						break;

					default:
						throw new NotImplementedException(string.Format("Unknown option: {0}", eventFiring));
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				var parent = System.Threading.Interlocked.Exchange<SuspendableObject>(ref _parent, null);
				if (parent != null)
				{
					Exception exceptionInAboutToBeResumed = null;
					if (1 == parent._suspendLevel)
					{
						try
						{
							parent.EhAboutToBeResumed();
						}
						catch (Exception ex)
						{
							exceptionInAboutToBeResumed = ex;
						}
					}

					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

					if (0 == newLevel)
					{
						try
						{
							parent.EhResume();
						}
						finally
						{
						}
					}
					else if (newLevel < 0)
					{
						throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
					}

					if (null != exceptionInAboutToBeResumed)
						throw exceptionInAboutToBeResumed;
				}
			}

			#endregion IDisposable Members
		}

		#endregion Inner class SuspendToken

		/// <summary>How many times was the Suspend function called (without corresponding Resume)</summary>
		private int _suspendLevel;

		/// <summary>
		/// Increase the SuspendLevel by one, and return a token that, if disposed, will resume the object.
		/// </summary>
		/// <returns>A token, which must be handed to the resume function to decrease the suspend level. Alternatively,
		/// the object can be used in an using statement. In this case, the call to the Resume function is not neccessary.</returns>
		public ISuspendToken SuspendGetToken()
		{
			return new SuspendToken(this);
		}

		/// <summary>
		/// Decrease the suspend level by disposing the suppress token. The token will fire the Resume event
		/// if the suppress level falls to zero.
		/// </summary>
		/// <param name="token"></param>
		/// <returns>The event count accumulated during the suspend phase.</returns>
		public int Resume(ref ISuspendToken token)
		{
			int result = 0;
			if (token != null)
			{
				token.Dispose(); // the OnResume function is called from the SuppressToken
				token = null;
			}
			return result;
		}

		/// <summary>
		/// Decrease the suspend level by disposing the suppress token. The token will fire the Resume event
		/// if the suppress level falls to zero. You can suppress the resume event by setting argument 'suppressResumeEvent' to true.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="firingOfResumeEvent">Designates whether or not to fire the resume event.</param>
		/// <returns>The event count accumulated during the suspend phase.</returns>
		public int Resume(ref ISuspendToken token, EventFiring firingOfResumeEvent)
		{
			int result = 0;
			if (token != null)
			{
				if (firingOfResumeEvent == EventFiring.Suppressed)
				{
					token.ResumeSilently();
				}

				token.Dispose(); // the OnResume function is called from the SuppressToken
				token = null;
			}
			return result;
		}

		/// <summary>
		/// For the moment of execution, the suspend status is interrupted, and one time the OnResume function would be called.
		/// </summary>
		public void ResumeShortly()
		{
			if (_suspendLevel != 0)
				EhResume();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is suspended.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
		/// </value>
		public bool IsSuspended { get { return _suspendLevel != 0; } }

		public bool IsNotSuspended { get { return _suspendLevel == 0; } }

		private void EhAboutToBeResumed()
		{
			OnAboutToBeResumed();
		}

		private void EhResume()
		{
			OnResume();
		}

		private void EhResumeSilently()
		{
			OnResumeSilently();
		}

		/// <summary>
		/// Called when the suspend level has just gone from 0 to 1, i.e. the object was suspended.
		/// </summary>
		protected virtual void OnSuspended()
		{
		}

		/// <summary>
		/// Is called when the suspend level is still 1 (one), but is about to fall to zero, i.e. shortly before the call to <see cref="OnResume"/>. This function is not called before <see cref="OnResumeSilently"/>!
		/// </summary>
		protected virtual void OnAboutToBeResumed()
		{
		}

		/// <summary>
		/// Is called when the suspend level falls down from 1 to zero  by a call to <see cref="ISuspendToken.Resume()"/> or a call to <see cref="M:ISuspendToken.Dispose"/>.
		/// </summary>
		protected virtual void OnResume()
		{
		}

		/// <summary>
		/// Is called when the suspend level falls down from 1 to zero by a call to <see cref="ISuspendToken.ResumeSilently"/>.
		/// The implementation should delete any accumulated events, should also disarm the suspendTokens of the childs of this object, and should not fire any Changed events nor set the change state of the object to dirty.
		/// </summary>
		protected virtual void OnResumeSilently()
		{
		}
	}
}