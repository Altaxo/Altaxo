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
	/// <summary>
	/// Base class for a suspendable document node which has no children, i.e. is a leaf node of the document tree.
	/// It implements most of the code neccessary to handle own change events and to accumulate data, if this object is suspended.
	/// </summary>
	public abstract class SuspendableDocumentLeafNode : SuspendableDocumentNodeBase
	{
		/// <summary>How many times was the Suspend function called (without corresponding Resume)</summary>
		private int _suspendLevel;

		#region Suspend state questions

		/// <summary>
		/// Suspend will increase the SuspendLevel.
		/// </summary>
		/// <returns>An object, which must be handed to the resume function to decrease the suspend level. Alternatively,
		/// the object can be used in an using statement. In this case, the call to the Resume function is not neccessary.</returns>
		public override ISuspendToken SuspendGetToken()
		{
			return new SuspendToken(this);
		}

		/// <summary>
		/// Resumes the object  completely for the time the returned token is referenced and not disposed.
		/// The return value is a token that had 'absorbed' the suspend count of the object, resulting in the suspend count
		/// of the object dropped to 0 (zero). When the returned token is finally disposed, the suspend count of the object is increased again by the 'absorbed' suspend count.
		/// </summary>
		/// <returns>A new token. As long as this token is not disposed, and not other process calls SuspendGetToken, the object is fre (not suspended). The object is suspended again when
		/// the returned token is disposed.</returns>
		public override IDisposable ResumeCompleteTemporarilyGetToken()
		{
			var result = new TemporaryResumeToken(this);
			result.ResumeTemporarily();
			return result;
		}

		/// <summary>
		/// Resumes the object completely for only a short time. Thus, if object was suspended before, it will be suspended again when the function returns.
		/// </summary>
		public override void ResumeCompleteTemporarily()
		{
			var result = new TemporaryResumeToken(this);
			result.ResumeTemporarily();
			result.Dispose();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is suspended.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is suspended; otherwise, <c>false</c>.
		/// </value>
		public override bool IsSuspended { get { return _suspendLevel != 0; } }

		#endregion Suspend state questions

		#region Call back functions by the suspendToken

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
		/// Is called when the suspend level falls down from 1 to zero  by a call to <see cref="M:Altaxo.Main.ISuspendToken.Resume"/> or a call to <see cref="M:Altaxo.Main.ISuspendToken.Dispose()"/>.
		/// </summary>
		protected virtual void OnResume()
		{
			EventArgs singleArg;
			if (AccumulatedEventData_HasZeroOrOneEventArg(out singleArg) && null != singleArg) // we have a single event arg accumulated
			{
				if (null == singleArg) // no events during suspended state
				{
					// nothing to do
				}
				else // one (1) event during suspend state
				{
					AccumulatedEventData_Clear();

					var parent = _parent as Main.IChildChangedEventSink;
					if (null != parent)
					{
						parent.EhChildChanged(this, singleArg);
					}
					if (!IsSuspended)
					{
						OnChanged(singleArg); // Fire the changed event
					}
				}
			}
			else // there is more than one event arg accumulated
			{
				var accumulatedEvents = AccumulatedEventData.ToArray();
				AccumulatedEventData_Clear();

				var parent = _parent as Main.IChildChangedEventSink;
				if (null != parent)
				{
					foreach (var eventArg in accumulatedEvents)
						parent.EhChildChanged(this, eventArg);
				}
				if (!IsSuspended)
				{
					foreach (var eventArg in accumulatedEvents)
						OnChanged(eventArg); // Fire the changed event
				}
			}
		}

		/// <summary>
		/// Is called when the suspend level falls down from 1 to zero by a call to <see cref="ISuspendToken.ResumeSilently"/>.
		/// The implementation should delete any accumulated events, should also disarm the suspendTokens of the childs of this object, and should not fire any Changed events nor set the change state of the object to dirty.
		/// </summary>
		protected virtual void OnResumeSilently()
		{
			AccumulatedEventData_Clear();
		}

		#endregion Call back functions by the suspendToken

		#region Change event handling

		/// <summary>
		/// Handles the cases when a child changes, but a reaction is neccessary only if the table is not suspended currently.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		/// <returns>True if the event has not changed the state of the table (i.e. it requires no further action).</returns>
		protected virtual bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Called if some member of this instance itself has changed.
		/// </summary>
		protected void EhSelfChanged(EventArgs e)
		{
			if (!IsSuspended)
			{
				// Notify parent
				if (null != _parent && !_parent.IsDisposeInProgress)
				{
					_parent.EhChildChanged(this, e); // parent may change our suspend state
				}

				if (!IsSuspended)
				{
					OnChanged(e); // Fire change event
					return;
				}
			}

			// at this point we are suspended for sure, or resume is still in progress
			AccumulateChangeData(this, e);  // child is unable to accumulate change data, we have to to it by ourself
		}

		#endregion Change event handling

		#region Inner class SuspendToken

		private class SuspendToken : ISuspendToken
		{
			private SuspendableDocumentLeafNode _parent;

			internal SuspendToken(SuspendableDocumentLeafNode parent)
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
				var parent = System.Threading.Interlocked.Exchange<SuspendableDocumentLeafNode>(ref _parent, null);
				if (parent != null)
				{
					int newLevel = System.Threading.Interlocked.Decrement(ref parent._suspendLevel);

					if (0 == newLevel)
					{
						try
						{
							parent.OnResumeSilently();
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
				var parent = System.Threading.Interlocked.Exchange<SuspendableDocumentLeafNode>(ref _parent, null);
				if (parent != null)
				{
					Exception exceptionInAboutToBeResumed = null;
					if (1 == parent._suspendLevel)
					{
						try
						{
							parent.OnAboutToBeResumed();
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
							parent.OnResume();
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

			#region Resume temporarily

			public IDisposable ResumeCompleteTemporarilyGetToken()
			{
				var parent = _parent;
				if (null == parent)
					throw new ObjectDisposedException("This token is already disposed");

				var result = new TemporaryResumeToken(parent);
				result.ResumeTemporarily();
				return result;
			}

			public void ResumeCompleteTemporarily()
			{
				var parent = _parent;
				if (null == parent)
					throw new ObjectDisposedException("This token is already disposed");

				var result = new TemporaryResumeToken(parent);
				result.ResumeTemporarily();
				result.Dispose();
			}

			#endregion Resume temporarily
		}

		/// <summary>
		/// Class that 'absorbs' the suspend count of the parent object, so that the events of the parent are temporarily resumed. The absorbed suspend count is stored in the token, and when the token
		/// is disposed, the suspend count of the parent is increased by that number again.
		/// </summary>
		private class TemporaryResumeToken : IDisposable
		{
			private SuspendableDocumentLeafNode _parent;
			private int _numberOfSuspendLevelsAbsorbed;

			internal TemporaryResumeToken(SuspendableDocumentLeafNode parent)
			{
				_parent = parent;
			}

			~TemporaryResumeToken()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			internal void ResumeTemporarily()
			{
				Exception ex1 = null;
				Exception ex2 = null;
				Exception ex3 = null;

				// Try to bring the suspend level to 0
				int suspendLevel = _parent._suspendLevel;
				while (suspendLevel != 0)
				{
					if (suspendLevel > 0)
					{
						if (suspendLevel == 1)
						{
							try
							{
								_parent.OnAboutToBeResumed();
							}
							catch (Exception ex)
							{
								ex1 = ex;
							}
						}

						suspendLevel = System.Threading.Interlocked.Decrement(ref _parent._suspendLevel);
						++_numberOfSuspendLevelsAbsorbed;

						if (suspendLevel == 0)
						{
							try
							{
								_parent.OnResume();
							}
							catch (Exception ex)
							{
								ex2 = ex;
							}
						}
					}
					else if (suspendLevel < 0)
					{
						suspendLevel = System.Threading.Interlocked.Increment(ref _parent._suspendLevel);
						--_numberOfSuspendLevelsAbsorbed;

						if (suspendLevel == 1)
						{
							try
							{
								_parent.OnSuspended();
							}
							catch (Exception ex)
							{
								ex3 = ex;
							}
						}
					}
				}

				if (null != ex1)
					throw ex1;
				if (null != ex2)
					throw ex2;
				if (null != ex3)
					throw ex3;
			}

			public void Dispose(bool isDisposing)
			{
				Exception exception = null;
				while (_numberOfSuspendLevelsAbsorbed > 0)
				{
					int suspendLevel = System.Threading.Interlocked.Increment(ref _parent._suspendLevel);
					--_numberOfSuspendLevelsAbsorbed;

					if (suspendLevel == 1)
					{
						if (1 == suspendLevel)
						{
							try
							{
								_parent.OnSuspended();
							}
							catch (Exception ex)
							{
								exception = ex;
							}
						}
					}
				}

				_parent = null;

				// Suspend level is now restored
				if (exception != null)
					throw exception;
			}
		}

		#endregion Inner class SuspendToken
	}
}