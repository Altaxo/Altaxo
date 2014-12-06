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
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Interface that must be implemented by all data sources that can provide data for an Altaxo data table.
	/// </summary>
	public interface IAltaxoTableDataSource : Main.ICopyFrom, IDisposable, Main.ISuspendableByToken, Main.IDocumentNode
	{
		/// <summary>
		/// Fills (or refills) the data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable"/>.
		/// </summary>
		/// <param name="destinationTable">The destination table.</param>
		void FillData(Altaxo.Data.DataTable destinationTable);

		IDataSourceImportOptions ImportOptions { get; set; }

		/// <summary>
		/// Called after deserization of a data source instance, when it is already associated with a data table.
		/// </summary>
		void OnAfterDeserialization();

		void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies);
	}

	/// <summary>
	/// Designates the cause of a re-read of the data source.
	/// </summary>
	public enum ImportTriggerSource
	{
		/// <summary>
		/// The user triggers a reread of the data source.
		/// </summary>
		Manual,

		/// <summary>
		/// The data source is reread if the associated table is used for the first time.
		/// </summary>
		FirstUse,

		/// <summary>
		/// The data source is reread every time the data source has changed. This includes the first use of the associated table.
		/// </summary>
		DataSourceChanged
	}

	/// <summary>
	/// Designates the event that the table's data source has changed.
	/// </summary>
	public class TableDataSourceChangedEventArgs : EventArgs
	{
		public new static readonly TableDataSourceChangedEventArgs Empty = new TableDataSourceChangedEventArgs();
	}
}