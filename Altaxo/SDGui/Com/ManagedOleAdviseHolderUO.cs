using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	/// <summary>
	/// Managed OleAdviseHolder (<b>U</b>sing <b>O</b>le) that wraps an IOleAdviseHolder.
	/// </summary>
	public class ManagedOleAdviseHolderUO
	{
		private IOleAdviseHolder _oleAdviseHolder;

		private InvokeableThread _invokeableThread;

		private int _invokeCount;
		private string _previousActionName;

		public ManagedOleAdviseHolderUO()
		{
			_invokeableThread = new InvokeableThread("OleAdviseThread", ((System.Windows.Window)Current.Workbench.ViewObject).Dispatcher);

			this.Invoke("Creation of IOleAdviseHolder", () =>
				{
					int res = Ole32Func.CreateOleAdviseHolder(out _oleAdviseHolder);
					System.Diagnostics.Debug.Assert(res == ComReturnValue.S_OK);
				});
		}

		/// <summary>Makes invocation of Invoke more concise with parameter-
		/// less methods.</summary>
		private void Invoke(string actionName, Action invoker)
		{
			if (0 == _invokeCount) // no reentrancy
			{
				_previousActionName = actionName;
				++_invokeCount;
#if COMLOGGING
				Debug.ReportInfo("OleAdviseThread {0}", actionName);
#endif
				_invokeableThread.Invoke(invoker);
				--_invokeCount;
			}
			else // Invoke was called before and is has not finished up to now
			{
#if COMLOGGING
				Debug.ReportWarning("ManagedOleAdviseHolder.Invoke: because a previous action ({0}) is still running, the action ({1}) is invoked asynchronously now.", _previousActionName, actionName);
#endif
				_invokeableThread.InvokeAsync(invoker);
			}
		}

		public void Advise(IAdviseSink pAdvise, out int pdwConnection)
		{
			int conn = -1;
			Invoke("Advise", () =>
			{
#if COMLOGGING
				Debug.ReportInfo("ManagedOleAdviseHolder.Advise (before calling Advise)", conn);
#endif
				_oleAdviseHolder.Advise(pAdvise, out conn);
#if COMLOGGING
				Debug.ReportInfo("ManagedOleAdviseHolder.Advise has given out cookie={0}", conn);
#endif
			}
			);

			System.Diagnostics.Debug.Assert(-1 != conn);
			pdwConnection = conn;
		}

		public void Unadvise(int dwConnection)
		{
			Invoke("Unadvise", () =>
			{
#if COMLOGGING
				Debug.ReportInfo("ManagedOleAdviseHolder.Unadvise, cookie={0}", dwConnection);
#endif
				_oleAdviseHolder.Unadvise(dwConnection);
			}
			);
		}

		public IEnumSTATDATA EnumAdvise()
		{
			IEnumSTATDATA returnValue = null;
			Invoke("EnumAdvise", () =>
				{
#if COMLOGGING
					Debug.ReportInfo("ManagedOleAdviseHolder.EnumAdvise");
#endif
					returnValue = _oleAdviseHolder.EnumAdvise();
				}
				);
			return returnValue;
		}

		public void SendOnRename(IMoniker pmk)
		{
			Invoke("SendOnRename()", () =>
				{
#if COMLOGGING
					Debug.ReportInfo("ManagedOleAdviseHolder.SendOnRename");
#endif
					_oleAdviseHolder.SendOnRename(pmk);
				}
				);
		}

		public void SendOnSave()
		{
			Invoke("SendOnSave", () =>
			{
#if COMLOGGING
				Debug.ReportInfo("ManagedOleAdviseHolder.SendOnSave");
#endif
				_oleAdviseHolder.SendOnSave();
			}
			);
		}

		public void SendOnClose()
		{
			Invoke("SendOnClose", () =>
				{
#if COMLOGGING
					Debug.ReportInfo("ManagedOleAdviseHolder.SendOnClose");
#endif
					_oleAdviseHolder.SendOnClose();
				}
				);
		}
	}
}