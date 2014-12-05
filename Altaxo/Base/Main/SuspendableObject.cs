#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
	/// <summary>Helper class to suspend and resume change events (or other events). In contrast to the simpler class <see cref="TemporaryDisabler"/>,
	/// this class keeps also track of events that happen in the suspend period. Per default, the action on resume is
	/// fired only if some events have happened during the suspend period.</summary>
	public class SuspendableObject : Main.ISuspendableByToken
	{
		#region Inner class SuspendToken

		private class SuspendToken : ISuspendToken
		{
			private SuspendableObject _parent;

			internal SuspendToken(SuspendableObject parent)
			{
				System.Threading.Interlocked.Increment(ref parent._suspendLevel);
				_parent = parent;
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
				{
					var parent = System.Threading.Interlocked.Exchange<SuspendableObject>(ref _parent, null);
					if (parent != null)
					{
						int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

						if (0 == newLevel)
						{
							System.Threading.Interlocked.Increment(ref parent._resumeInProgress);
							try
							{
								parent.EhResumeSilently();
							}
							finally
							{
								System.Threading.Interlocked.Decrement(ref parent._resumeInProgress);
							}
						}
						else if (newLevel < 0)
						{
							throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
						}
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
						System.Threading.Interlocked.Increment(ref parent._resumeInProgress);
						try
						{
							parent.EhResume();
						}
						finally
						{
							System.Threading.Interlocked.Decrement(ref parent._resumeInProgress);
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

		/// <summary>Counts the number of events during the suspend phase.</summary>
		private int _eventCount;

		/// <summary>
		/// If the resume operation is currently in progress, this member is <c>true</c>. Otherwise, it is false.
		/// </summary>
		private int _resumeInProgress;

		/// <summary>
		/// Suspend will increase the SuspendLevel.
		/// </summary>
		/// <returns>An object, which must be handed to the resume function to decrease the suspend level. Alternatively,
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
				result = _eventCount;
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

				result = _eventCount;
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
		/// Gets a value indicating whether this instance is currently resuming the events.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if resume is in progress; otherwise, <c>false</c>.
		/// </value>
		public bool IsResumeInProgress { get { return _resumeInProgress > 0; } }

		public bool IsSuspendedOrResumeInProgress { get { return _resumeInProgress != 0 || _suspendLevel != 0; } }

		/// <summary>
		/// Gets a value indicating whether this instance is suspended.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
		/// </value>
		public bool IsSuspended { get { return _suspendLevel != 0; } }

		/// <summary>
		/// Returns true when the suppress level is equal to zero (initial state). Otherwise false.
		/// Attention - this function does not increment the event counter.
		/// </summary>
		public bool PeekEnabled { get { return _suspendLevel == 0; } }

		/// <summary>
		/// Returns true when the suppress level is greater than zero (initial state). Returns false if the suppress level is zero.
		/// Attention - this function does not increment the event counter.
		/// </summary>
		public bool PeekDisabled { get { return _suspendLevel != 0; } }

		/// <summary>
		/// Returns true when the suppress level is equal to zero (initial state). Otherwise false.
		/// If the suppress level not zero, this function increases the event count by one.    /// </summary>
		public bool GetEnabledWithCounting()
		{
			if (_suspendLevel != 0)
			{
				_eventCount++;
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Counts the number of events during the suspend state. Every call to this function will increment the event counter by 1 (but only in the suspended state). The event counter will be reset to zero when the object is resumed.
		/// </summary>
		public void CountEvent()
		{
			if (_suspendLevel != 0)
			{
				_eventCount++;
			}
		}

		/// <summary>
		/// Returns true when the suppress level is greater than zero (initial state). Returns false if the suppress level is zero.
		/// If the suppress level not zero, this function increases the event counter by one. At a resume, the OnResume handler is only called when the event count is greater than zero.
		/// </summary>
		public bool GetDisabledWithCounting()
		{
			if (_suspendLevel != 0)
			{
				_eventCount++;
				return true;
			}
			else
			{
				return false;
			}
		}

		private void EhAboutToBeResumed()
		{
			OnAboutToBeResumed(_eventCount);
		}

		private void EhResume()
		{
			var oldEventCount = _eventCount;
			_eventCount = 0;
			OnResume(oldEventCount);
		}

		private void EhResumeSilently()
		{
			var oldEventCount = _eventCount;
			_eventCount = 0;
			OnResumeSilently(oldEventCount);
		}

		/// <summary>
		/// Is called when the suspend level is still 1 (one), but is about to fall to zero, i.e. shortly before the call to <see cref="OnResume"/>. This function is not called before <see cref="OnResumeSilently"/>!
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="CountEvent"/> function was called during the suspended state.</param>
		protected virtual void OnAboutToBeResumed(int eventCount)
		{
		}

		/// <summary>
		/// Is called when the suspend level falls down from 1 to zero  by a call to <see cref="ISuspendToken.Resume"/> or a call to <see cref="ISuspendToken.Dispose"/>.
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="CountEvent"/> function was called during the suspended state.</param>
		protected virtual void OnResume(int eventCount)
		{
		}

		/// <summary>
		/// Is called when the suspend level falls down from 1 to zero by a call to <see cref="ISuspendToken.ResumeSilently"/>.
		/// The implementation should delete any accumulated events, should also disarm the suspendTokens of the childs of this object, and should not fire any Changed events nor set the change state of the object to dirty.
		/// </summary>
		/// <param name="eventCount">The event count. The event count is the number of times the <see cref="CountEvent"/> function was called during the suspended state.</param>
		protected virtual void OnResumeSilently(int eventCount)
		{
		}
	}
}