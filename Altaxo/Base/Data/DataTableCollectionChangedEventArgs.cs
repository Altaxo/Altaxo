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

namespace Altaxo.Data
{
	/// <summary>
	/// Holds information about what has changed in the collection of tables.
	/// </summary>
	public class DataTableCollectionChangedEventArgs : System.EventArgs
	{
		/// <summary>
		/// If true, one or more tables where added.
		/// </summary>
		public bool TableAdded;

		/// <summary>
		/// If true, one or more table where removed.
		/// </summary>
		public bool TableRemoved;

		/// <summary>
		/// If true, one or more tables where renamed.
		/// </summary>
		public bool TableRenamed;

		/// <summary>
		/// Empty constructor.
		/// </summary>
		public DataTableCollectionChangedEventArgs()
		{
		}

		/// <summary>
		/// Returns an empty instance.
		/// </summary>
		public static new DataTableCollectionChangedEventArgs Empty
		{
			get { return new DataTableCollectionChangedEventArgs(); }
		}

		/// <summary>
		/// Returns an instance with TableAdded set to true;.
		/// </summary>
		public static DataTableCollectionChangedEventArgs IfTableAdded
		{
			get
			{
				DataTableCollectionChangedEventArgs e = new DataTableCollectionChangedEventArgs();
				e.TableAdded = true;
				return e;
			}
		}

		/// <summary>
		/// Returns an instance with TableRemoved set to true.
		/// </summary>
		public static DataTableCollectionChangedEventArgs IfTableRemoved
		{
			get
			{
				DataTableCollectionChangedEventArgs e = new DataTableCollectionChangedEventArgs();
				e.TableRemoved = true;
				return e;
			}
		}

		/// <summary>
		/// Returns an  instance with TableRenamed set to true.
		/// </summary>
		public static DataTableCollectionChangedEventArgs IfTableRenamed
		{
			get
			{
				DataTableCollectionChangedEventArgs e = new DataTableCollectionChangedEventArgs();
				e.TableRenamed = true;
				return e;
			}
		}

		/// <summary>
		/// Merges information from another instance in this ChangedEventArg.
		/// </summary>
		/// <param name="from"></param>
		public void Merge(DataTableCollectionChangedEventArgs from)
		{
			this.TableAdded |= from.TableAdded;
			this.TableRemoved |= from.TableRemoved;
			this.TableRenamed |= from.TableRenamed;
		}

		/// <summary>
		/// Returns true when the collection has changed (addition, removal or renaming of tables).
		/// </summary>
		public bool CollectionChanged
		{
			get { return TableAdded | TableRemoved | TableRenamed; }
		}
	}
}