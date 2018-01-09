#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	using Altaxo.Main.Services;
	using UnmanagedApi.Ole32;

	/// <summary>
	/// Managed OleAdviseHolder (<b>U</b>sing <b>O</b>le) that wraps an IOleAdviseHolder.
	/// </summary>
	public class ManagedOleAdviseHolderUO
	{
		private IOleAdviseHolder _oleAdviseHolder;

		private IInvokeableThread _invokeableThread;

		private int _invokeCount;
		private string _previousActionName;

		public ManagedOleAdviseHolderUO()
		{
			_invokeableThread = new InvokeableThread("OleAdviseThread", Current.Dispatcher);

			this.Invoke("Creation of IOleAdviseHolder", () =>
				{
					int res = Ole32Func.CreateOleAdviseHolder(out _oleAdviseHolder);
					if (!(res == ComReturnValue.S_OK))
						throw new InvalidOperationException("The COM operation was not successful");
				});
		}

		/// <summary>Invokes on the invokable thread.</summary>
		/// <param name="actionName">For debugging purposes.</param>
		/// <param name="action">The action to invoke.</param>
		private void Invoke(string actionName, Action action)
		{
			if (0 == _invokeCount) // no reentrancy
			{
				_previousActionName = actionName;
				++_invokeCount;
				ComDebug.ReportInfo("OleAdviseThread {0}", actionName);
				_invokeableThread.Invoke(action);
				--_invokeCount;
			}
			else // Invoke was called before and is has not finished up to now
			{
				ComDebug.ReportWarning("ManagedOleAdviseHolder.Invoke: because a previous action ({0}) is still running, the action ({1}) is invoked asynchronously now.", _previousActionName, actionName);
				_invokeableThread.InvokeAsync(action);
			}
		}

		public void Advise(IAdviseSink pAdvise, out int pdwConnection)
		{
			int conn = -1;
			Invoke("Advise", () =>
			{
				ComDebug.ReportInfo("ManagedOleAdviseHolder.Advise (before calling Advise)", conn);
				_oleAdviseHolder.Advise(pAdvise, out conn);
				ComDebug.ReportInfo("ManagedOleAdviseHolder.Advise has given out cookie={0}", conn);
			}
			);

			if (!(-1 != conn))
				throw new InvalidOperationException(nameof(conn) + " should be != -1");

			pdwConnection = conn;
		}

		public void Unadvise(int dwConnection)
		{
			Invoke("Unadvise", () =>
			{
				ComDebug.ReportInfo("ManagedOleAdviseHolder.Unadvise, cookie={0}", dwConnection);
				_oleAdviseHolder.Unadvise(dwConnection);
			}
			);
		}

		public IEnumSTATDATA EnumAdvise()
		{
			IEnumSTATDATA returnValue = null;
			Invoke("EnumAdvise", () =>
				{
					ComDebug.ReportInfo("ManagedOleAdviseHolder.EnumAdvise");
					returnValue = _oleAdviseHolder.EnumAdvise();
				}
				);
			return returnValue;
		}

		public void SendOnRename(IMoniker pmk)
		{
			Invoke("SendOnRename()", () =>
				{
					ComDebug.ReportInfo("ManagedOleAdviseHolder.SendOnRename");
					_oleAdviseHolder.SendOnRename(pmk);
				}
				);
		}

		public void SendOnSave()
		{
			Invoke("SendOnSave", () =>
			{
				ComDebug.ReportInfo("ManagedOleAdviseHolder.SendOnSave");
				_oleAdviseHolder.SendOnSave();
			}
			);
		}

		public void SendOnClose()
		{
			Invoke("SendOnClose", () =>
				{
					ComDebug.ReportInfo("ManagedOleAdviseHolder.SendOnClose");
					_oleAdviseHolder.SendOnClose();
				}
				);
		}
	}
}