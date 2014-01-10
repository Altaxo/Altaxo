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
			Debug.ReportInfo("ManagedOleAdviseHolder2.Advise giving out cookie={0}", pdwConnection);
#endif
		}

		public void Unadvise(int dwConnection)
		{
#if COMLOGGING
			Debug.ReportInfo("ManagedOleAdviseHolder2.Unadvise cookie={0}", dwConnection);
#endif
			int idx = dwConnection - 1; // we have to reverse incrementation, see Advise function
			System.Diagnostics.Debug.Assert(_advises[idx] != null);
			_advises[idx] = null;
		}

		public IEnumSTATDATA EnumAdvise()
		{
#if COMLOGGING
			Debug.ReportInfo("ManagedOleAdviseHolder2.EnumAdvise");
#endif
			return new EnumSTATDATA1(_advises);
		}

		public void SendOnRename(IMoniker pmk)
		{
#if COMLOGGING
			Debug.ReportInfo("ManagedOleAdviseHolder2.SendOnRename calling (for all sinks) IAdviseSink.OnRename(Moniker)");
#endif
			foreach (IAdviseSink sink in new List<IAdviseSink>(_advises.Where(x => null != x))) // new List is neccessary because the original list might be modified during the operation
				sink.OnRename(pmk);
		}

		public void SendOnSave()
		{
#if COMLOGGING
			Debug.ReportInfo("ManagedOleAdviseHolder2.SendOnRename calling (for all sinks) IAdviseSink.OnSave()");
#endif
			// note we don't have an IOleAdviseHolder for this, we use a list of sinks
			foreach (var sink in _advises.Where(x => null != x))
				sink.OnSave();
		}

		public void SendOnClose()
		{
#if COMLOGGING
			Debug.ReportInfo("ManagedOleAdviseHolder2.SendOnClose calling (for all sinks) IAdviseSink.OnClose()");
#endif
			foreach (var sink in new List<IAdviseSink>(_advises.Where(x => null != x))) // new List is neccessary because the original list might be modified during the operation
				sink.OnClose();
		}

		#region EnumStatData

		/// <summary>
		/// Helps enumerate the formats available in our DataObject class.
		/// </summary>
		[ComVisible(true)]
		private class EnumSTATDATA1 : ManagedEnumBase<STATDATA>, IEnumSTATDATA
		{
			public EnumSTATDATA1(IList<IAdviseSink> list)
				: base(list.Where((x) => null != x).Select((x, i) => new STATDATA { advSink = x, connection = i + 1 }))
			{
			}

			public EnumSTATDATA1(EnumSTATDATA1 from)
				: base(from)
			{
			}

			public void Clone(out IEnumSTATDATA newEnum)
			{
				newEnum = new EnumSTATDATA1(this);
			}
		}

		private class EnumSTATDATA : IEnumSTATDATA
		{
			// Keep an array of the formats for enumeration
			private IList<IAdviseSink> _advises;

			// The index of the next item
			private int currentIndex = 0;

			/// <summary>
			/// Creates an instance from a list of key value pairs.
			/// </summary>
			/// <param name="storage">List of FORMATETC/STGMEDIUM key value pairs</param>
			internal EnumSTATDATA(IList<IAdviseSink> advises)
			{
				this._advises = advises;
			}

			#region IEnumFORMATETC Members

			/// <summary>
			/// Creates a clone of this enumerator.
			/// </summary>
			/// <param name="newEnum">When this function returns, contains a new instance of IEnumFORMATETC.</param>
			public void Clone(out IEnumSTATDATA newEnum)
			{
				var ret = new EnumSTATDATA(_advises);
				ret.currentIndex = currentIndex;
				newEnum = ret;
			}

			/// <summary>
			/// Retrieves the next elements from the enumeration.
			/// </summary>
			/// <param name="celt">The number of elements to retrieve.</param>
			/// <param name="rgelt">An array to receive the formats requested.</param>
			/// <param name="pceltFetched">An array to receive the number of element fetched.</param>
			/// <returns>If the fetched number of formats is the same as the requested number, S_OK is returned.
			/// There are several reasons S_FALSE may be returned: (1) The requested number of elements is less than
			/// or equal to zero. (2) The rgelt parameter equals null. (3) There are no more elements to enumerate.
			/// (4) The requested number of elements is greater than one and pceltFetched equals null or does not
			/// have at least one element in it. (5) The number of fetched elements is less than the number of
			/// requested elements.</returns>
			public int Next(int celt, STATDATA[] rgelt, int[] pceltFetched)
			{
				// Start with zero fetched, in case we return early
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = 0;

				// This will count down as we fetch elements
				int cReturn = celt;

				// Short circuit if they didn't request any elements, or didn't
				// provide room in the return array, or there are not more elements
				// to enumerate.
				if (celt <= 0 || rgelt == null || currentIndex >= _advises.Count)
					return 1; // S_FALSE

				// If the number of requested elements is not one, then we must
				// be able to tell the caller how many elements were fetched.
				if ((pceltFetched == null || pceltFetched.Length < 1) && celt != 1)
					return 1; // S_FALSE

				// If the number of elements in the return array is too small, we
				// throw. This is not a likely scenario, hence the exception.
				if (rgelt.Length < celt)
					throw new ArgumentException("The number of elements in the return array is less than the number of elements requested");

				// Fetch the elements.
				for (int i = 0; currentIndex < _advises.Count && cReturn > 0; currentIndex++)
				{
					if (_advises[currentIndex] != null)
					{
						rgelt[i].advSink = _advises[currentIndex];
						rgelt[i].connection = currentIndex + 1;
						i++;
						cReturn--;
					}
				}

				// Return the number of elements fetched
				if (pceltFetched != null && pceltFetched.Length > 0)
					pceltFetched[0] = celt - cReturn;

				// cReturn has the number of elements requested but not fetched.
				// It will be greater than zero, if multiple elements were requested
				// but we hit the end of the enumeration.
				return (cReturn == 0) ? 0 : 1; // S_OK : S_FALSE
			}

			/// <summary>
			/// Resets the state of enumeration.
			/// </summary>
			/// <returns>S_OK</returns>
			public int Reset()
			{
				currentIndex = 0;
				return 0; // S_OK
			}

			/// <summary>
			/// Skips the number of elements requested.
			/// </summary>
			/// <param name="celt">The number of elements to skip.</param>
			/// <returns>If there are not enough remaining elements to skip, returns S_FALSE. Otherwise, S_OK is returned.</returns>
			public int Skip(int celt)
			{
				if (currentIndex + celt > _advises.Count)
					return 1; // S_FALSE

				currentIndex += celt;
				return 0; // S_OK
			}

			#endregion IEnumFORMATETC Members
		}

		#endregion EnumStatData
	}
}