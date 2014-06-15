using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Interface that must be implemented by all data sources that can provide data for an Altaxo data table.
	/// </summary>
	public interface IAltaxoTableDataSource : Main.ICopyFrom
	{
		/// <summary>
		/// Fills (or refills) the data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable"/>.
		/// </summary>
		/// <param name="destinationTable">The destination table.</param>
		void FillData(Altaxo.Data.DataTable destinationTable);

		/// <summary>
		/// Gets a value indicating whether the data that are cached in the Altaxo table should be saved within the Altaxo project.
		/// </summary>
		/// <value>
		/// If <c>True</c>, the data of the table attached to this data source are not stored in the Altaxo project file.
		/// </value>
		bool DoNotSaveCachedTableData { get; }

		/// <summary>
		/// Gets a value indicating whether the table script is executed after importing data from this data source.
		/// </summary>
		/// <value>
		/// <c>true</c> if [execute table script after import]; otherwise, <c>false</c>.
		/// </value>
		bool ExecuteTableScriptAfterImport { get; }

		/// <summary>
		/// Gets the cause of a reread of the data source.
		/// </summary>
		/// <value>
		/// The cause of a reread of the data source.
		/// </value>
		ImportTriggerSource ImportTriggerSource { get; }

		/// <summary>
		/// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
		/// </summary>
		event Action<IAltaxoTableDataSource> DataSourceChanged;
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