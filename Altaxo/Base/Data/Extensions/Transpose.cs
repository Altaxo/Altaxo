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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Data
{
	/// <summary>
	/// Options for transposing a worksheet.
	/// </summary>
	public class DataTableTransposeOptions : ICloneable
	{
		/// <summary>
		/// Gets or sets the number of data columns to transpose.
		/// </summary>
		/// <value>
		/// The number of data columns to transpose.
		/// </value>
		public int DataColumnsMoveToPropertyColumns { get; set; }

		/// <summary>
		/// Gets or sets the number of property columns to transpose.
		/// </summary>
		/// <value>
		/// The number of property columns to transpose.
		/// </value>
		public int PropertyColumnsMoveToDataColumns { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the existing column names of the source table should be stored in the first data column of the transposed table.
		/// </summary>
		/// <value>
		/// <c>true</c> if the existing column names of the source table should be stored in the first data column of the transposed table; otherwise, <c>false</c>.
		/// </value>
		public bool StoreDataColumnNamesInFirstDataColumn { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the first data column of the source table should be used to set the column names in the transposed table.
		/// </summary>
		/// <value>
		/// <c>true</c> if the first data column of the source table should be used to set the column names in the transposed table; otherwise, <c>false</c>.
		/// </value>
		public bool UseFirstDataColumnForColumnNaming { get; set; }

		private string _columnNamingPreString = "Row";

		public string ColumnNamingPreString
		{
			get { return _columnNamingPreString; }
			set { _columnNamingPreString = value ?? string.Empty; }
		}

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2015-08-26 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableTransposeOptions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DataTableTransposeOptions)obj;

				info.AddValue("NumberOfDataColumnsMovingToPropertyColumns", s.DataColumnsMoveToPropertyColumns);
				info.AddValue("NumberOfPropertyColumnsMovingToDataColumns", s.PropertyColumnsMoveToDataColumns);
				info.AddValue("StoreDataColumnNamesInFirstDataColumn", s.StoreDataColumnNamesInFirstDataColumn);
				info.AddValue("UseFirstDataColumnForColumnNaming", s.UseFirstDataColumnForColumnNaming);
				info.AddValue("ColumnNamingPreString", s.ColumnNamingPreString);
			}

			protected virtual DataTableTransposeOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new DataTableTransposeOptions() : (DataTableTransposeOptions)o);
				s.DataColumnsMoveToPropertyColumns = info.GetInt32("NumberOfDataColumnsMovingToPropertyColumns");
				s.PropertyColumnsMoveToDataColumns = info.GetInt32("NumberOfPropertyColumnsMovingToDataColumns");
				s.StoreDataColumnNamesInFirstDataColumn = info.GetBoolean("StoreDataColumnNamesInFirstDataColumn");
				s.UseFirstDataColumnForColumnNaming = info.GetBoolean("UseFirstDataColumnForColumnNaming");
				s.ColumnNamingPreString = info.GetString("ColumnNamingPreString");
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

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public class DataTableTransposeDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
	{
		private DataTableTransposeOptions _processOptions;
		private DataTableProxy _processData;
		private IDataSourceImportOptions _importOptions;

		public Action<IAltaxoTableDataSource> _dataSourceChanged;

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2015-08-26 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableTransposeDataSource), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DataTableTransposeDataSource)obj;

				info.AddValue("ProcessData", s._processData);
				info.AddValue("ProcessOptions", s._processOptions);
				info.AddValue("ImportOptions", s._importOptions);
			}

			protected virtual DataTableTransposeDataSource SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new DataTableTransposeDataSource() : (DataTableTransposeDataSource)o);

				s._processData = (DataTableProxy)info.GetValue("ProcessData", s);
				s._processOptions = (DataTableTransposeOptions)info.GetValue("ProcessOptions", s);
				s._importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", s);

				s.InputData = s._processData;

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

		protected DataTableTransposeDataSource()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTableTransposeDataSource"/> class.
		/// </summary>
		/// <param name="inputData">The input data designates the original source of data (used then for the processing).</param>
		/// <param name="dataSourceOptions">The Fourier transformation options.</param>
		/// <param name="importOptions">The data source import options.</param>
		/// <exception cref="System.ArgumentNullException">
		/// inputData
		/// or
		/// transformationOptions
		/// or
		/// importOptions
		/// </exception>
		public DataTableTransposeDataSource(DataTableProxy inputData, DataTableTransposeOptions dataSourceOptions, IDataSourceImportOptions importOptions)
		{
			if (null == inputData)
				throw new ArgumentNullException(nameof(inputData));
			if (null == dataSourceOptions)
				throw new ArgumentNullException(nameof(dataSourceOptions));
			if (null == importOptions)
				throw new ArgumentNullException(nameof(importOptions));

			using (var token = SuspendGetToken())
			{
				this.TransposeOptions = dataSourceOptions;
				this.ImportOptions = importOptions;
				this.InputData = inputData;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExpandCyclingVariableColumnDataSource"/> class.
		/// </summary>
		/// <param name="from">Another instance to copy from.</param>
		public DataTableTransposeDataSource(DataTableTransposeDataSource from)
		{
			CopyFrom(from);
		}

		/// <summary>
		/// Copies from another instance.
		/// </summary>
		/// <param name="obj">The object to copy from.</param>
		/// <returns><c>True</c> if anything could be copied from the object, otherwise <c>false</c>.</returns>
		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as DataTableTransposeDataSource;
			if (null != from)
			{
				using (var token = SuspendGetToken())
				{
					DataTableTransposeOptions dataSourceOptions = null;
					DataTableProxy inputData = null;
					IDataSourceImportOptions importOptions = null;

					CopyHelper.Copy(ref importOptions, from._importOptions);
					CopyHelper.Copy(ref dataSourceOptions, from._processOptions);
					CopyHelper.Copy(ref inputData, from._processData);

					this.TransposeOptions = dataSourceOptions;
					this.ImportOptions = importOptions;
					this.InputData = inputData;

					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public object Clone()
		{
			return new DataTableTransposeDataSource(this);
		}

		#region IAltaxoTableDataSource

		/// <summary>
		/// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
		/// </summary>
		/// <param name="destinationTable">The destination table.</param>
		public void FillData(DataTable destinationTable)
		{
			try
			{
				DataTable srcTable = _processData.Document;
				if (srcTable == null)
					throw new InvalidOperationException(string.Format("Source table was not found: {0}", _processData));

				Transposing.Transpose(srcTable, _processOptions, destinationTable);
			}
			catch (Exception ex)
			{
				destinationTable.Notes.WriteLine("Error during execution of data source ({0}): {1}", this.GetType().Name, ex.Message);
			}
		}

		/// <summary>
		/// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
		/// </summary>
		public event Action<Data.IAltaxoTableDataSource> DataSourceChanged
		{
			add
			{
				bool isFirst = null == _dataSourceChanged;
				_dataSourceChanged += value;
				if (isFirst)
				{
					//EhInputDataChanged(this, EventArgs.Empty);
				}
			}
			remove
			{
				_dataSourceChanged -= value;
				bool isLast = null == _dataSourceChanged;
				if (isLast)
				{
				}
			}
		}

		/// <summary>
		/// Gets or sets the input data.
		/// </summary>
		/// <value>
		/// The input data. This data is the input for the 2D-Fourier transformation.
		/// </value>
		public DataTableProxy InputData
		{
			get
			{
				return _processData;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (ChildSetMember(ref _processData, value))
				{
					EhChildChanged(_processData, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the data source import options.
		/// </summary>
		/// <value>
		/// The import options.
		/// </value>
		/// <exception cref="System.ArgumentNullException">ImportOptions</exception>
		public Data.IDataSourceImportOptions ImportOptions
		{
			get
			{
				return _importOptions;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				var oldValue = _importOptions;

				_importOptions = value;
			}
		}

		/// <summary>
		/// Gets or sets the options for the transpose operation.
		/// </summary>
		/// <value>
		/// The transpose options.
		/// </value>
		/// <exception cref="System.ArgumentNullException">FourierTransformation2DOptions</exception>
		public DataTableTransposeOptions TransposeOptions
		{
			get
			{
				return _processOptions;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				var oldValue = _processOptions;

				_processOptions = value;
			}
		}

		#region Change event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(_processData, sender)) // incoming call from data proxy
			{
				if (_importOptions.ImportTriggerSource == ImportTriggerSource.DataSourceChanged)
				{
					e = TableDataSourceChangedEventArgs.Empty;
				}
				else
				{
					return true; // if option is not DataSourceChanged, absorb this event
				}
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		#endregion Change event handling

		#region Document Node functions

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _processData)
				yield return new Main.DocumentNodeAndName(_processData, "ProcessData");
		}

		#endregion Document Node functions

		/// <summary>
		/// Called after deserization of a data source instance, when it is already associated with a data table.
		/// </summary>
		public void OnAfterDeserialization()
		{
		}

		/// <summary>
		/// Visits all document references.
		/// </summary>
		/// <param name="ReportProxies">The report proxies.</param>
		public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
		{
			if (_processData != null)
				ReportProxies(_processData, this, "ProcessData");
		}

		#endregion IAltaxoTableDataSource
	}

	/// <summary>
	/// Contains methods to transpose a worksheet.
	/// </summary>
	public static class Transposing
	{
		/// <summary>
		/// Tests if the transpose of a table is possible.
		/// </summary>
		/// <param name="table">Table to test.</param>
		/// <param name="numConvertedDataColumns">Number of data columns (beginning from index 0) that will be converted to property columns.</param>
		/// <param name="indexOfProblematicColumn">On return, if transpose is not possible, will give the index of the first column which differs in type from the first transposed data column.</param>
		/// <returns>True when the transpose is possible without problems, false otherwise.</returns>
		public static bool TransposeIsPossible(this DataTable table, int numConvertedDataColumns, out int indexOfProblematicColumn)
		{
			if (numConvertedDataColumns < 0)
				throw new ArgumentOutOfRangeException("numConvertedDataColumns is less than zero");

			indexOfProblematicColumn = 0;
			if (numConvertedDataColumns >= table.DataColumnCount)
				return true; // when all columns convert to property columns, that will be no problem

			System.Type masterColumnType = table[numConvertedDataColumns].GetType();

			for (int i = numConvertedDataColumns + 1; i < table.DataColumnCount; i++)
			{
				if (table[i].GetType() != masterColumnType)
				{
					indexOfProblematicColumn = i;
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Tests whether or not all columns in this collection have the same type.
		/// </summary>
		/// <param name="col">The column collection containing the columns to test.</param>
		/// <param name="selectedColumnIndices">The column indices of the columns to test.</param>
		/// <param name="firstdifferentcolumnindex">Out: returns the first column that has a different type from the first column</param>.
		/// <returns>True if all selected columns are of the same type. True is also returned if the number of selected columns is zero.</returns>
		public static bool AreAllColumnsOfTheSameType(DataColumnCollection col, IContiguousIntegerRange selectedColumnIndices, out int firstdifferentcolumnindex)
		{
			firstdifferentcolumnindex = 0;

			if (0 == col.ColumnCount || 0 == selectedColumnIndices.Count)
				return true;

			System.Type firstType = col[selectedColumnIndices[0]].GetType();

			int len = selectedColumnIndices.Count;
			for (int i = 0; i < len; i++)
			{
				if (col[selectedColumnIndices[i]].GetType() != firstType)
				{
					firstdifferentcolumnindex = selectedColumnIndices[i];
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Transpose transpose the table, i.e. exchange columns and rows
		/// this can only work if all columns in the table are of the same type
		/// </summary>
		/// <param name="srcTable">Table to transpose.</param>
		/// <param name="options">Options that control the transpose process.</param>
		/// <param name="destTable">Table in which the transposed table should be stored.</param>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException">The data columns to transpose are not of the same type. The first column that has a deviating type is column number  + firstDifferentColumnIndex.ToString()</exception>
		public static void Transpose(this DataTable srcTable, DataTableTransposeOptions options, DataTable destTable)
		{
			if (null == srcTable)
				throw new ArgumentNullException(nameof(srcTable));
			if (null == destTable)
				throw new ArgumentNullException(nameof(destTable));
			if (object.ReferenceEquals(srcTable, destTable))
				throw new ArgumentException(nameof(srcTable) + " and " + nameof(destTable) + " are identical. This inline transpose operation is not supported.");

			int numberOfDataColumnsChangeToPropertyColumns = Math.Min(options.DataColumnsMoveToPropertyColumns, srcTable.DataColumnCount);

			int numberOfPropertyColumnsChangeToDataColumns = Math.Min(options.PropertyColumnsMoveToDataColumns, srcTable.PropertyColumnCount);

			// number of data columns in the destination table that originates either from converted property columns or from the label column which contains the column names
			int numberOfPriorDestDataColumns = numberOfPropertyColumnsChangeToDataColumns + (options.StoreDataColumnNamesInFirstDataColumn ? 1 : 0);

			var dataColumnsToTransposeIndices = ContiguousIntegerRange.FromStartAndEndExclusive(numberOfDataColumnsChangeToPropertyColumns, srcTable.DataColumnCount);

			int firstDifferentColumnIndex;
			if (!AreAllColumnsOfTheSameType(srcTable.DataColumns, dataColumnsToTransposeIndices, out firstDifferentColumnIndex))
			{
				throw new InvalidOperationException("The data columns to transpose are not of the same type. The first column that has a deviating type is column number " + firstDifferentColumnIndex.ToString());
			}

			using (var suspendToken = destTable.SuspendGetToken())
			{
				destTable.DataColumns.ClearData();
				destTable.PropCols.ClearData();

				// 0th, store the data column names in the first column
				if (options.StoreDataColumnNamesInFirstDataColumn)
				{
					var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(0, "DataColumnNames", typeof(TextColumn), ColumnKind.Label, 0);
					for (int j = numberOfDataColumnsChangeToPropertyColumns, k = 0; j < srcTable.DataColumnCount; ++j, ++k)
						destCol[k] = srcTable.DataColumns.GetColumnName(j);
				}

				int numberOfExtraPriorDestColumns = (options.StoreDataColumnNamesInFirstDataColumn ? 1 : 0);

				// 1st, copy the property columns to data columns
				for (int i = 0; i < numberOfPropertyColumnsChangeToDataColumns; ++i)
				{
					var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(i + numberOfExtraPriorDestColumns, srcTable.PropertyColumns.GetColumnName(i), srcTable.PropertyColumns[i].GetType(), srcTable.PropertyColumns.GetColumnKind(i), srcTable.PropertyColumns.GetColumnGroup(i));
					var srcCol = srcTable.PropertyColumns[i];
					for (int j = numberOfDataColumnsChangeToPropertyColumns, k = 0; j < srcCol.Count; ++j, ++k)
						destCol[k] = srcCol[j];
				}

				// 2rd, transpose the data columns
				int srcRows = 0;
				foreach (int i in dataColumnsToTransposeIndices)
					srcRows = Math.Max(srcRows, srcTable.DataColumns[i].Count);

				// create as many columns in destTable as srcRows and fill them with data
				Type columnType = dataColumnsToTransposeIndices.Count > 0 ? srcTable.DataColumns[dataColumnsToTransposeIndices[0]].GetType() : null;
				for (int i = 0; i < srcRows; ++i)
				{
					string destColName = string.Format("{0}{1}", options.ColumnNamingPreString, i);
					if (options.UseFirstDataColumnForColumnNaming)
					{
						destColName = string.Format("{0}{1}", options.ColumnNamingPreString, srcTable.DataColumns[0][i]);
					}

					var destCol = destTable.DataColumns.EnsureExistenceAtPositionStrictly(numberOfPriorDestDataColumns + i, destColName, false, columnType, ColumnKind.V, 0);
					int k = 0;
					foreach (int j in dataColumnsToTransposeIndices)
						destCol[k++] = srcTable.DataColumns[j][i];
				}

				// 3rd, copy the first data columns to property columns
				for (int i = 0; i < numberOfDataColumnsChangeToPropertyColumns; ++i)
				{
					var destCol = destTable.PropertyColumns.EnsureExistenceAtPositionStrictly(i, srcTable.DataColumns.GetColumnName(i), srcTable.DataColumns[i].GetType(), srcTable.DataColumns.GetColumnKind(i), srcTable.DataColumns.GetColumnGroup(i));
					var srcCol = srcTable.DataColumns[i];
					for (int j = numberOfPriorDestDataColumns, k = 0; k < srcCol.Count; ++j, ++k)
						destCol[j] = srcCol[k];
				}

				// 4th, fill the rest of the property columns with the rest of the data columns
				for (int i = 0; i < numberOfDataColumnsChangeToPropertyColumns; ++i)
				{
					for (int j = 0; j < numberOfPropertyColumnsChangeToDataColumns; ++j)
					{
						try
						{
							destTable.PropertyColumns[i][j + numberOfExtraPriorDestColumns] = srcTable.PropertyColumns[j][i];
						}
						catch { }
					}
				}

				// and 5th, copy the remaining property columns to property columns
				for (int i = numberOfPropertyColumnsChangeToDataColumns, j = numberOfDataColumnsChangeToPropertyColumns; i < srcTable.PropertyColumns.ColumnCount; ++i, ++j)
				{
					var destCol = destTable.PropertyColumns.EnsureExistenceAtPositionStrictly(j, srcTable.PropertyColumns.GetColumnName(i), false, srcTable.PropertyColumns[i].GetType(), srcTable.PropertyColumns.GetColumnKind(i), srcTable.DataColumns.GetColumnGroup(i));
					destCol.Data = srcTable.PropertyColumns[i];
				}

				suspendToken.Resume();
			}
		}
	}
}