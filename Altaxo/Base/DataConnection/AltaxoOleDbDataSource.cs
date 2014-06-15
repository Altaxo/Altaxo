using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.DataConnection
{
	public class AltaxoOleDbDataSource : OleDbDataQuery, Altaxo.Data.IAltaxoTableDataSource
	{
		protected Data.ImportTriggerSource _importTriggerSource;
		protected bool _doNotSaveCachedTableData;
		protected bool _executeTableScriptAfterImport;

		public AltaxoOleDbDataSource(string sql, string connection)
			: base(connection, sql)
		{
		}

		protected AltaxoOleDbDataSource()
		{
		}

		public override bool CopyFrom(object obj)
		{
			if (!base.CopyFrom(obj))
				return false;

			var from = obj as AltaxoOleDbDataSource;
			if (null != from)
			{
				_importTriggerSource = from._importTriggerSource;
				_doNotSaveCachedTableData = from._doNotSaveCachedTableData;
				_executeTableScriptAfterImport = from._executeTableScriptAfterImport;
			}
			return true;
		}

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2014-06-13 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoOleDbDataSource), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (AltaxoOleDbDataSource)obj;

				info.AddValue("Connection", s._connectionString);
				info.AddValue("Statement", s._sqlStatement);
				info.AddEnum("ImportTriggerSource", s._importTriggerSource);
				info.AddValue("ExecuteTableScriptAfterImport", s._executeTableScriptAfterImport);
				info.AddValue("DoNotSaveCachedTableData", s._doNotSaveCachedTableData);
			}

			protected virtual AltaxoOleDbDataSource SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new AltaxoOleDbDataSource() : (AltaxoOleDbDataSource)o);

				s._connectionString = info.GetString("Connection");
				s._sqlStatement = info.GetString("Statement");
				s._importTriggerSource = (Data.ImportTriggerSource)info.GetEnum("ImportTriggerSource", s._importTriggerSource.GetType());
				s._executeTableScriptAfterImport = info.GetBoolean("ExecuteTableScriptAfterImport");
				s._doNotSaveCachedTableData = info.GetBoolean("DoNotSaveCachedTableData");
				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>A clone of this instance.</returns>
		public override object Clone()
		{
			var result = new AltaxoOleDbDataSource();
			result.CopyFrom(this);
			return result;
		}

		/// <summary>
		/// Fills (or refills) the data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
		/// </summary>
		/// <param name="destinationTable">The destination table.</param>
		public void FillData(Data.DataTable destinationTable)
		{
			destinationTable.Suspend();
			try
			{
				var tableConnector = new AltaxoTableConnector(destinationTable);
				this.ReadDataFromOleDbConnection(tableConnector.ReadAction);
			}
			finally
			{
				destinationTable.Resume();
			}
		}

		/// <summary>
		/// Gets a value indicating whether the data that are cached in the Altaxo table should be saved within the Altaxo project.
		/// </summary>
		/// <value>
		/// If <c>True</c>, the data of the table attached to this data source are not stored in the Altaxo project file.
		/// </value>
		public bool DoNotSaveCachedTableData
		{
			get { return _doNotSaveCachedTableData; }
		}

		/// <summary>
		/// Gets the cause of a reread of the data source.
		/// </summary>
		/// <value>
		/// The cause of a reread of the data source.
		/// </value>
		public Data.ImportTriggerSource ImportTriggerSource
		{
			get { return _importTriggerSource; }
			set { _importTriggerSource = value; }
		}

		/// <summary>
		/// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
		/// </summary>
		public event Action<Data.IAltaxoTableDataSource> DataSourceChanged;

		/// <summary>
		/// Gets a value indicating whether the table script is executed after importing data from this data source.
		/// </summary>
		/// <value>
		/// <c>true</c> if [execute table script after import]; otherwise, <c>false</c>.
		/// </value>
		public bool ExecuteTableScriptAfterImport
		{
			get { return _executeTableScriptAfterImport; }
		}
	}
}