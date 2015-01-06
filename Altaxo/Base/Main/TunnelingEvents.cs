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
	/// <summary>
	/// Interface that indicates that its owner can announce tunneling events by firing the <see cref="TunneledEvent"/>.
	/// </summary>
	public interface ITunnelingEventSource
	{
		/// <summary>
		/// The event that is fired when the object is disposed. First argument is the sender, second argument is the original source, and third argument is the event arg.
		/// </summary>
		event Action<object, object, Main.TunnelingEventArgs> TunneledEvent;
	}

	public class TunnelingEventArgs : EventArgs
	{
	}

	public class DisposeEventArgs : TunnelingEventArgs
	{
		public static new readonly DisposeEventArgs Empty = new DisposeEventArgs();

		private DisposeEventArgs()
		{
		}
	}

	public class DocumentPathChangedEventArgs : TunnelingEventArgs
	{
		public static new readonly DocumentPathChangedEventArgs Empty = new DocumentPathChangedEventArgs();

		private DocumentPathChangedEventArgs()
		{
		}
	}
}