using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>Helper class to suspend and resume change events (or other events). In contrast to the simpler class <see cref="TemporaryDisabler"/>,
	/// this class keeps also track of events that happen in the suspend period. Per default, the action on resume is
	/// fired only if some events have happened during the suspend period.</summary>
	public class SuspendableLeafObject : Main.ISuspendableByToken
	{
		#region Inner class SuppressToken

		private class SuspendToken : ISuspendToken
		{
			private SuspendableLeafObject _parent;

			public SuspendToken(SuspendableLeafObject parent)
			{
				System.Threading.Interlocked.Increment(ref parent._suppressLevel);
				_parent = parent;
			}

			~SuspendToken()
			{
				Dispose();
			}

			/// <summary>
			/// Disarms this SuppressToken so that it can not raise the resume event anymore.
			/// </summary>
			public void Disarm()
			{
				var parent = System.Threading.Interlocked.Exchange<SuspendableLeafObject>(ref _parent, null);
				if (parent != null)
				{
					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suppressLevel);
				}
			}

			public void Resume()
			{
				Dispose();
			}

			#region IDisposable Members

			public void Dispose()
			{
				var parent = System.Threading.Interlocked.Exchange<SuspendableLeafObject>(ref _parent, null);
				if (parent != null)
				{
					Exception exceptionInAboutToBeResumed = null;
					if (1 == parent._suppressLevel)
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

					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suppressLevel);

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

		#endregion Inner class SuppressToken

		/// <summary>How many times was the Suspend function called (without corresponding Resume)</summary>
		private int _suppressLevel;

		/// <summary>
		/// Constructor. You have to provide a callback function, that is been called when the event handling resumes.
		/// </summary>
		/// <param name="resumeEventHandler">The callback function called when the events resume. See remarks when the callback function is called.</param>
		/// <remarks>The callback function is called only (i) if the event resumes (exactly: the _suppressLevel changes from 1 to 0),
		/// and (ii) in that moment the _eventCount is &gt;0.
		/// To get the _eventCount&gt;0, someone must call either GetEnabledWithCounting or GetDisabledWithCounting
		/// during the suspend period.</remarks>
		public SuspendableLeafObject()
		{
		}

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
					token.Disarm();
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
			if (_suppressLevel != 0)
				EhResume();
		}

		public bool IsSuspended { get { return _suppressLevel != 0; } }

		public bool IsNotSuspended { get { return _suppressLevel == 0; } }

		private void EhResume()
		{
			OnResume();
		}

		private void EhAboutToBeResumed()
		{
			OnAboutToBeResumed();
		}

		/// <summary>
		/// Is called when the suppress level falls down from 1 to zero and the event count is != 0.
		/// Per default, the resume event handler is called that you provided in the constructor.
		/// </summary>
		protected virtual void OnResume()
		{
		}

		/// <summary>
		/// Is called when the suppress level is still 1 (one), but is about to fall to zero.
		/// </summary>
		protected virtual void OnAboutToBeResumed()
		{
		}
	}
}