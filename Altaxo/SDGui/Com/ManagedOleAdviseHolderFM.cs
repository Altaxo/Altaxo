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
			ComDebug.ReportInfo("{0}.Advise giving out cookie={1}", this.GetType().Name, pdwConnection);
		}

		public void Unadvise(int dwConnection)
		{
			ComDebug.ReportInfo("{0}.Unadvise cookie={1}", this.GetType().Name, dwConnection);
			int idx = dwConnection - 1; // we have to reverse incrementation, see Advise function
			if (!(_advises[idx] != null))
				throw new InvalidOperationException(nameof(_advises) + "[idx] should be != null");

			_advises[idx] = null;
		}

		public IEnumSTATDATA EnumAdvise()
		{
			ComDebug.ReportInfo("{0}.EnumAdvise", this.GetType().Name);
			return new EnumSTATDATA(_advises);
		}

		public void SendOnRename(IMoniker pmk)
		{
			ComDebug.ReportInfo("{0}.SendOnRename calling (for all sinks) IAdviseSink.OnRename(Moniker)", this.GetType().Name);
			foreach (IAdviseSink sink in new List<IAdviseSink>(_advises.Where(x => null != x))) // new List is neccessary because the original list might be modified during the operation
				sink.OnRename(pmk);
		}

		public void SendOnSave()
		{
			ComDebug.ReportInfo("{0}.SendOnSave calling (for all sinks) IAdviseSink.OnSave()", this.GetType().Name);
			// note we don't have an IOleAdviseHolder for this, we use a list of sinks
			foreach (var sink in _advises.Where(x => null != x))
				sink.OnSave();
		}

		public void SendOnClose()
		{
			ComDebug.ReportInfo("{0}.SendOnClose calling (for all sinks) IAdviseSink.OnClose()", this.GetType().Name);
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