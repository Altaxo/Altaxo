#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Contains options how to split a table that contains an independent variable with cycling values into
	/// another table, where this independent variable is unique and sorted.
	/// </summary>
	public class DecomposeByColumnContentOptions
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		ICloneable
	{
		#region Enums

		public enum OutputFormat
		{
			GroupOneColumn,
			GroupAllColumns,
		}

		public enum OutputSorting
		{
			None,
			Ascending,
			Descending
		}

		#endregion Enums

		#region Members

		/// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
		protected OutputFormat _destinationOutput;

		/// <summary>If set, the destination columns will be sorted according to the first averaged column (if there is any).</summary>
		protected OutputSorting _destinationColumnSorting;

		#endregion Members

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2016-05-10 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DecomposeByColumnContentOptions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DecomposeByColumnContentOptions)obj;

				info.AddEnum("DestinationOutput", s._destinationOutput);
				info.AddEnum("DestinationColumnSorting", s._destinationColumnSorting);
			}

			protected virtual DecomposeByColumnContentOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = o as DecomposeByColumnContentOptions ?? new DecomposeByColumnContentOptions();

				s._destinationOutput = (OutputFormat)info.GetEnum("DestinationOutput", typeof(OutputFormat));
				s._destinationColumnSorting = (OutputSorting)info.GetEnum("DestinationColumnSorting", typeof(OutputSorting));

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

		#region Construction

		public DecomposeByColumnContentOptions()
		{
		}

		public DecomposeByColumnContentOptions(DecomposeByColumnContentOptions from)
		{
			CopyFrom(from);
		}

		public object Clone()
		{
			return new DecomposeByColumnContentOptions(this);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as DecomposeByColumnContentOptions;
			if (null != from)
			{
				this._destinationOutput = from._destinationOutput;
				this._destinationColumnSorting = from._destinationColumnSorting;

				EhSelfChanged();

				return true;
			}
			return false;
		}

		#endregion Construction

		#region Properties

		/// <summary>Designates the order of the newly created columns of the dependent variables.</summary>
		public OutputFormat DestinationOutput { get { return _destinationOutput; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationOutput, value); } }

		/// <summary>If set, the destination columns will be either not sorted or sorted.</summary>
		public OutputSorting DestinationColumnSorting { get { return _destinationColumnSorting; } set { SetMemberEnumAndRaiseSelfChanged(ref _destinationColumnSorting, value); } }

		#endregion Properties
	}

	/// <summary>
	/// Holds both the data (see <see cref="DataTableMultipleColumnProxy"/>) and the options (see <see cref="DecomposeByColumnContentOptions"/>) to perform
	/// the decomposition of a table containing a column with a cycling variable.
	/// </summary>
	public class DecomposeByColumnContentDataAndOptions : ICloneable
	{
		/// <summary>
		/// Holds the data nessessary for decomposing of a table containing a column with a cycling variable.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public DataTableMultipleColumnProxy Data { get; private set; }

		/// <summary>
		/// Holds the options nessessary for decomposing of a table containing a column with a cycling variable.
		/// </summary>
		/// <value>
		/// The options.
		/// </value>
		public DecomposeByColumnContentOptions Options { get; private set; }

		/// <summary>Identifies the column with the cycling variable in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
		public const string ColumnWithCyclingVariableIdentifier = "ColumnWithContentToSplit";

		/// <summary>Identifies all columns which participate in the <see cref="DataTableMultipleColumnProxy"/> instance.</summary>
		public const string ColumnsParticipatingIdentifier = "ColumnsParticipating";

		public DecomposeByColumnContentDataAndOptions(DataTableMultipleColumnProxy data, DecomposeByColumnContentOptions options)
		{
			Data = data;
			Options = options;
		}

		/// <summary>
		/// Tests if the data in <paramref name="data"/> can be used for the DecomposeByColumnContent action.
		/// </summary>
		/// <param name="data">The data to test.</param>
		/// <param name="throwIfNonCoherent">If true, an exception is thrown if any problems are detected. If false, it is tried to rectify the problem by making some assumtions.</param>
		public static void EnsureCoherence(DataTableMultipleColumnProxy data, bool throwIfNonCoherent)
		{
			if (null == data.DataTable) // this is mandatory, thus an exception is always thrown
			{
				throw new ArgumentNullException("SourceTable is null, it must be set before");
			}

			data.EnsureExistenceOfIdentifier(ColumnsParticipatingIdentifier);
			data.EnsureExistenceOfIdentifier(ColumnWithCyclingVariableIdentifier, 1);

			if (data.GetDataColumns(ColumnsParticipatingIdentifier).Count == 0)
			{
				if (throwIfNonCoherent)
					throw new ArgumentException(!data.ContainsIdentifier(ColumnsParticipatingIdentifier) ? "ColumnsToProcess is not set" : "ColumnsToProcess is empty");
			}

			if (data.GetDataColumnOrNull(ColumnWithCyclingVariableIdentifier) == null)
			{
				if (throwIfNonCoherent)
					throw new ArgumentException("Column with cycling variable was not included in columnsToProcess");
				else
				{
					var col = data.GetDataColumns(ColumnsParticipatingIdentifier).FirstOrDefault();
					if (null != col)
						data.SetDataColumn(ColumnWithCyclingVariableIdentifier, col);
				}
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			return new DecomposeByColumnContentDataAndOptions((DataTableMultipleColumnProxy)this.Data.Clone(), (DecomposeByColumnContentOptions)this.Options.Clone());
		}
	}

	public static class DecomposeByColumnContentActions
	{
		public static void ShowDecomposeByColumnContentDialog(this DataTable srcTable, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns)
		{
			DataTableMultipleColumnProxy proxy = null;
			DecomposeByColumnContentOptions options = null;

			try
			{
				proxy = new DataTableMultipleColumnProxy(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier, srcTable, selectedDataRows, selectedDataColumns);
				proxy.EnsureExistenceOfIdentifier(DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier, 1);

				options = new DecomposeByColumnContentOptions();
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(string.Format("{0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error in preparation of 'Decompose by column content'");
				return;
			}

			var dataAndOptions = new DecomposeByColumnContentDataAndOptions(proxy, options);

			// in order to show the column names etc in the dialog, it is neccessary to set the source
			if (true == Current.Gui.ShowDialog(ref dataAndOptions, "Choose options", false))
			{
				var destTable = new DataTable();
				proxy = dataAndOptions.Data;
				options = dataAndOptions.Options;

				string error = null;
				try
				{
					error = DecomposeByColumnContent(dataAndOptions.Data, dataAndOptions.Options, destTable);
				}
				catch (Exception ex)
				{
					error = ex.ToString();
				}
				if (null != error)
					Current.Gui.ErrorMessageBox(error);

				destTable.Name = srcTable.Name + "_Decomposed";

				// Create a DataSource
				var dataSource = new DecomposeByColumnContentDataSource(proxy, options, new Altaxo.Data.DataSourceImportOptions());
				destTable.DataSource = dataSource;

				Current.Project.DataTableCollection.Add(destTable);
				Current.ProjectService.ShowDocumentView(destTable);
			}
		}

		/// <summary>
		/// Decompose the source columns according to the provided options. The source table and the settings are provided in the <paramref name="options"/> variable.
		/// The provided destination table is cleared from all data and property values before.
		/// </summary>
		/// <param name="inputData">The data containing the source table, the participating columns and the column with the cycling variable.</param>
		/// <param name="options">The settings for decomposing.</param>
		/// <param name="destTable">The destination table. Any data will be removed before filling with the new data.</param>
		/// <returns>Null if the method finishes successfully, or an error information.</returns>
		public static string DecomposeByColumnContent(DataTableMultipleColumnProxy inputData, DecomposeByColumnContentOptions options, DataTable destTable)
		{
			var srcTable = inputData.DataTable;

			try
			{
				DecomposeByColumnContentDataAndOptions.EnsureCoherence(inputData, true);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

			destTable.DataColumns.RemoveColumnsAll();
			destTable.PropCols.RemoveColumnsAll();

			DataColumn srcCycCol = inputData.GetDataColumnOrNull(DecomposeByColumnContentDataAndOptions.ColumnWithCyclingVariableIdentifier);
			var decomposedValues = Decompose(srcCycCol);
			// the decomposedValues are not sorted yes

			if (options.DestinationColumnSorting == DecomposeByColumnContentOptions.OutputSorting.Ascending)
			{
				decomposedValues.Sort();
			}
			else if (options.DestinationColumnSorting == DecomposeByColumnContentOptions.OutputSorting.Descending)
			{
				decomposedValues.Sort();
				decomposedValues.Reverse();
			}
			// get the other columns to process

			var srcColumnsToProcess = new List<DataColumn>(inputData.GetDataColumns(DecomposeByColumnContentDataAndOptions.ColumnsParticipatingIdentifier));
			// subtract the column containing the decompose values
			srcColumnsToProcess.Remove(srcCycCol);

			// the only property column that is now usefull is that with the repeated values
			var destPropCol = destTable.PropCols.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCycCol), srcCycCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCycCol), srcTable.DataColumns.GetColumnGroup(srcCycCol));

			if (options.DestinationOutput == DecomposeByColumnContentOptions.OutputFormat.GroupOneColumn)
			{
				// columns originating from the same column but with different property are grouped together, but they will get different group numbers
				foreach (var srcCol in srcColumnsToProcess)
				{
					int nCreatedCol = -1;
					int nCreatedProp = 0;
					foreach (var prop in decomposedValues)
					{
						++nCreatedCol;
						++nCreatedProp;
						var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCreatedCol.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), nCreatedProp);
						var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
						for (int i = 0, j = 0; i < srcCycCol.Count; ++i)
						{
							if (prop == srcCycCol[i])
							{
								destCol[j] = srcCol[i];
								++j;
							}
						}
						// fill also property column
						destPropCol[nDestCol] = prop;
					}
				}
			}
			else if (options.DestinationOutput == DecomposeByColumnContentOptions.OutputFormat.GroupAllColumns)
			{
				// all columns with the same property are grouped together, and those columns will share the same group number
				int nCreatedCol = -1; // running number of processed range for column creation (Naming)
				int nCreatedProp = -1;
				foreach (var prop in decomposedValues)
				{
					++nCreatedProp;
					++nCreatedCol;

					foreach (var srcCol in srcColumnsToProcess)
					{
						var destCol = destTable.DataColumns.EnsureExistence(srcTable.DataColumns.GetColumnName(srcCol) + "." + nCreatedCol.ToString(), srcCol.GetType(), srcTable.DataColumns.GetColumnKind(srcCol), nCreatedProp);
						var nDestCol = destTable.DataColumns.GetColumnNumber(destCol);
						for (int i = 0, j = 0; i < srcCycCol.Count; ++i)
						{
							if (prop == srcCycCol[i])
							{
								destCol[j] = srcCol[i];
								++j;
							}
						}
						// fill also property column
						destPropCol[nDestCol] = prop;
					}
				}
			}
			else
			{
				throw new NotImplementedException("The option for destination output is unknown: " + options.DestinationOutput.ToString());
			}

			return null;
		}

		/// <summary>
		/// Decomposes a column into repeat units by analysing the values of the column with increasing index.
		/// If a column value is repeated, the current range is finalized and a new range is started. At the end,
		/// a list of index ranges is returned. Inside each range the column values are guaranteed to be unique.
		/// </summary>
		/// <param name="col">Column to decompose.</param>
		/// <returns>List of integer ranges. Inside a single range the column values are ensured to be unique.</returns>
		public static List<AltaxoVariant> Decompose(DataColumn col)
		{
			var result = new List<AltaxoVariant>();
			var alreadyIn = new HashSet<AltaxoVariant>();
			for (int i = 0; i < col.Count; i++)
			{
				var item = col[i];
				if (!alreadyIn.Contains(item))
				{
					result.Add(item);
					alreadyIn.Add(item);
				}
			}
			return result;
		}
	}
}