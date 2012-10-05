#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Helper class to temporarily disable something, e.g. some events. By calling <see cref="Disable"/> one gets a disposable token, that,
	/// when disposed, enables again, which fires then the action that is given as parameter to the constructor. It is possible to make nested calls to <see cref="Disable"/>. In this case all tokens
	/// must be disposed before the <see cref="IsEnabled"/> is again <c>true</c> and the re-enabling action is fired.
	/// </summary>
	public class TemporaryDisabler
	{
		#region Inner class SuppressToken
		private class SuppressToken : IDisposable
		{
			TemporaryDisabler _parent;

			public SuppressToken(TemporaryDisabler parent)
			{
				_parent = parent;
				System.Threading.Interlocked.Increment(ref _parent._disablingLevel);
			}

			/// <summary>
			/// Disarms this SuppressToken so that it can not raise the suspend event anymore.
			/// </summary>
			public void Disarm()
			{
				if (_parent != null)
				{
					var parent = _parent;
					_parent = null;
					int newLevel = System.Threading.Interlocked.Decrement(ref _parent._disablingLevel);
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (_parent != null)
				{
					var parent = _parent;
					_parent = null;
					int newLevel = System.Threading.Interlocked.Decrement(ref parent._disablingLevel);
					if (0 == newLevel)
					{
						parent.OnReenabling();
					}
					else if (newLevel < 0)
					{
						throw new ApplicationException("Fatal programming error - suppress level has fallen down to negative values");
					}
				}
			}

			#endregion
		}
		#endregion

		/// <summary>How many times was the <see cref="Disable"/> function called (without disposing the tokens got in these calls)</summary>
		private int _disablingLevel;

		/// <summary>Action that is taken when the suppress levels falls down to zero and the event count is equal to or greater than one (i.e. during the suspend phase, at least an event had occured).</summary>
		Action _reenablingEventHandler;


		   /// <summary>
    /// Constructor. You have to provide a callback function, that is been called when the event handling resumes.
    /// </summary>
    /// <param name="reenablingEventHandler">The callback function called when the events resume. See remarks when the callback function is called.</param>
    /// <remarks>The callback function is called only (i) if the event resumes (exactly: the _suppressLevel changes from 1 to 0),
    /// and (ii) in that moment the _eventCount is &gt;0.
    /// To get the _eventCount&gt;0, someone must call either GetEnabledWithCounting or GetDisabledWithCounting
    /// during the suspend period.</remarks>
    public TemporaryDisabler(Action reenablingEventHandler)
    {
      _reenablingEventHandler = reenablingEventHandler;
    }

		/// <summary>
		/// Increase the SuspendLevel.
		/// </summary>
		/// <returns>An object, which must be disposed in order to re-enabling again.
		/// The most convenient way is to use a using statement with this function call
		/// </returns>
		public IDisposable Disable()
		{
			return new SuppressToken(this);
		}

		/// <summary>
		/// Returns true when the disabling level is equal to zero (initial state).
		/// Otherwise false.
		/// </summary>
		public bool IsEnabled { get { return _disablingLevel == 0; } }
		
		/// <summary>
		/// Returns true when the disabling level is greater than zero (after calling the <see cref="Disable"/> function).
		/// Returns false if the disabling level is zero.
		/// </summary>
		public bool IsDisabled { get { return _disablingLevel != 0; } }


		/// <summary>
		/// Just fires the reenabling action that was given in the constructor, 
		/// without changing the disabling level.
		/// </summary>
		public void ReenableShortly()
		{
			OnReenabling();
		}

		/// <summary>
		/// Is called when the suppress level falls down from 1 to zero and the event count is != 0.
		/// Per default, the resume event handler is called that you provided in the constructor.
		/// </summary>
		protected virtual void OnReenabling()
		{
				if (_reenablingEventHandler != null)
					_reenablingEventHandler();
		}
	}
}
