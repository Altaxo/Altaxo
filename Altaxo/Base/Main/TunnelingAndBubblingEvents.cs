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
	public class BubblingEventArgs : EventArgs
	{
	}

	public interface ISupportsTunnelingAndBubblingEvents
	{
		/// <summary>
		/// Informs about an event that bubbles from a child node up to the root node.
		/// The handler should send this event up to the parent node.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void EhBubblingEvent(object sender, BubblingEventArgs e);

		/// <summary>
		/// Informs about an event that tunnels from a root node down to all child nodes. The handler
		/// should enumerate over all child nodes and send the event down to them.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void EhTunnelingEvent(object sender, TunnelingEventArgs e);

		// event Action<object, BubblingEventArgs> BubblingEvent;
		// event Action<object, TunnelingEventArgs> TunnelingEvent;
	}

	public class TunnelingEventArgs
	{
	}

	public class PreviewDisposeEventArgs : TunnelingEventArgs
	{
		public static readonly PreviewDisposeEventArgs Empty = new PreviewDisposeEventArgs();

		private PreviewDisposeEventArgs()
		{
		}
	}

	public class DisposeEventArgs : TunnelingEventArgs
	{
		public static readonly DisposeEventArgs Empty = new DisposeEventArgs();

		private DisposeEventArgs()
		{
		}
	}

	public class DocumentPathChangedEventArgs : TunnelingEventArgs
	{
		public static readonly DocumentPathChangedEventArgs Empty = new DocumentPathChangedEventArgs();

		private DocumentPathChangedEventArgs()
		{
		}
	}
}