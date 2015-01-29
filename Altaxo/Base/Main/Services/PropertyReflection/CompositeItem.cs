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
using System.Collections.ObjectModel;
using System.Text;

namespace Altaxo.Main.Services.PropertyReflection
{
	/// <summary>
	///
	/// </summary>
	/// <remarks>
	/// <para>This class originated from the 'WPG Property Grid' project (<see href="http://wpg.codeplex.com"/>), licensed under Ms-PL.</para>
	/// </remarks>
	public abstract class CompositeItem : Item
	{
		#region Fields

		private readonly ObservableCollection<Item> _items = new ObservableCollection<Item>();

		#endregion Fields

		#region Properties

		public ObservableCollection<Item> Items
		{
			get { return _items; }
		}

		#endregion Properties

		#region IDisposable Members

		protected override void Dispose(bool disposing)
		{
			if (Disposed)
			{
				return;
			}
			if (disposing)
			{
				foreach (Item item in Items)
				{
					item.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#endregion IDisposable Members
	}
}