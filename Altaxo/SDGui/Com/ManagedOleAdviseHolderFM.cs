using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	/// <summary>
	/// Class that mimics an IOleAdviseHolder in a managed class. This implementation is fully managed (..FM), i.e. it does not use any OLE/COM helper class.
	/// </summary>
	public class ManagedOleAdviseHolderFM
	{
		private IList<IAdviseSink> _advises = new List<IAdviseSink>();

		public void Advise(IAdviseSink pAdvise, out int pdwConnection)
		{
			pdwConnection = _advises.Count + 1; // this is the cookie: Attention: cookies with a value of 0 will not be accepted, so we increment count by 1 to have always a cookie > 0
			_advises.Add(pAdvise);
#if COMLOGGING
			Debug.ReportInfo("{0}.Advise giving out cookie={1}", this.GetType().Name, pdwConnection);
#endif
		}

		public void Unadvise(int dwConnection)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.Unadvise cookie={1}", this.GetType().Name, dwConnection);
#endif
			int idx = dwConnection - 1; // we have to reverse incrementation, see Advise function
			System.Diagnostics.Debug.Assert(_advises[idx] != null);
			_advises[idx] = null;
		}

		public IEnumSTATDATA EnumAdvise()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.EnumAdvise", this.GetType().Name);
#endif
			return new EnumSTATDATA(_advises);
		}

		public void SendOnRename(IMoniker pmk)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.SendOnRename calling (for all sinks) IAdviseSink.OnRename(Moniker)", this.GetType().Name);
#endif
			foreach (IAdviseSink sink in new List<IAdviseSink>(_advises.Where(x => null != x))) // new List is neccessary because the original list might be modified during the operation
				sink.OnRename(pmk);
		}

		public void SendOnSave()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.SendOnSave calling (for all sinks) IAdviseSink.OnSave()", this.GetType().Name);
#endif
			// note we don't have an IOleAdviseHolder for this, we use a list of sinks
			foreach (var sink in _advises.Where(x => null != x))
				sink.OnSave();
		}

		public void SendOnClose()
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.SendOnClose calling (for all sinks) IAdviseSink.OnClose()", this.GetType().Name);
#endif
			foreach (var sink in new List<IAdviseSink>(_advises.Where(x => null != x))) // new List is neccessary because the original list might be modified during the operation
				sink.OnClose();
		}

		#region EnumStatData

		/// <summary>
		/// Helps enumerate the formats available in our DataObject class.
		/// </summary>
		[ComVisible(true)]
		private class EnumSTATDATA : ManagedEnumBase<STATDATA>, IEnumSTATDATA
		{
			public EnumSTATDATA(IList<IAdviseSink> list)
				: base(list.Where((x) => null != x).Select((x, i) => new STATDATA { advSink = x, connection = i + 1 }))
			{
			}

			public EnumSTATDATA(EnumSTATDATA from)
				: base(from)
			{
			}

			public void Clone(out IEnumSTATDATA newEnum)
			{
				newEnum = new EnumSTATDATA(this);
			}
		}

		#endregion EnumStatData
	}
}