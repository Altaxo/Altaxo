using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Interface that must be implemented by all data sources that can provide data for an Altaxo data table.
	/// </summary>
	public interface IAltaxoTableDataSource : Main.ICopyFrom, IDisposable
	{
		/// <summary>
		/// Fills (or refills) the data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable"/>.
		/// </summary>
		/// <param name="destinationTable">The destination table.</param>
		void FillData(Altaxo.Data.DataTable destinationTable);

		IDataSourceImportOptions ImportOptions { get; set; }

		/// <summary>
		/// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
		/// </summary>
		event Action<IAltaxoTableDataSource> DataSourceChanged;

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
}