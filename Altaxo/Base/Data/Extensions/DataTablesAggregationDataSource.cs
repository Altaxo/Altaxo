#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Altaxo.Calc.Statistics;

namespace Altaxo.Data
{
  /// <summary>
  /// Provides a data source that aggregates data from multiple tables according to specified options.
  /// </summary>
  public class DataTablesAggregationDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    /// <summary>
    /// Represents a key for grouping rows during aggregation, based on clustered property values.
    /// </summary>
    public readonly struct AggregationKey : IEquatable<AggregationKey>, IComparable<AggregationKey>
    {
      /// <summary>
      /// Gets the key values that define this aggregation key.
      /// </summary>
      public AltaxoVariant[] KeyValues { get; }

      private readonly int _hashCode;

      /// <summary>
      /// Initializes a new instance of the <see cref="AggregationKey"/> struct.
      /// </summary>
      /// <param name="keyValues">The key values that define this aggregation key.</param>
      public AggregationKey(AltaxoVariant[] keyValues)
      {
        if (keyValues is null)
          throw new ArgumentNullException(nameof(keyValues));

        KeyValues = keyValues;

        int hash = 17;
        for (int i = 0; i < keyValues.Length; ++i)
        {
          hash = hash * 31 + keyValues[i].GetHashCode();
        }
        _hashCode = hash;
      }

      /// <inheritdoc/>
      public int CompareTo(AggregationKey other)
      {
        int r = 0;
        for (int i = 0; i < KeyValues.Length; ++i)
        {
          if (0 != (r = Comparer<AltaxoVariant>.Default.Compare(KeyValues[i], other.KeyValues[i])))
            break;
        }
        return r;
      }

      /// <inheritdoc/>
      public bool Equals(AggregationKey other)
      {
        if (KeyValues.Length != other.KeyValues.Length)
          return false;

        for (int i = 0; i < KeyValues.Length; i++)
        {
          if (!KeyValues[i].Equals(other.KeyValues[i]))
            return false;
        }

        return true;
      }

      /// <inheritdoc/>
      public override bool Equals(object? obj)
      {
        return obj is AggregationKey other && Equals(other);
      }

      /// <inheritdoc/>
      public override int GetHashCode()
      {
        return _hashCode;
      }
    }

    private DataTablesAggregationOptions _processOptions;
    private DataTablesAggregationProcessData _processData;
    private IDataSourceImportOptions _importOptions;

    /// <summary>
    /// Backing field for the <see cref="DataSourceChanged"/> event.
    /// </summary>
    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2015-08-26 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTablesAggregationDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTablesAggregationDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }



      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is DataTablesAggregationDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new DataTablesAggregationDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (DataTablesAggregationProcessData)info.GetValue("ProcessData", this));
      _processOptions = (DataTablesAggregationOptions)info.GetValue("ProcessOptions", this);
      _importOptions = info.GetValue<IDataSourceImportOptions>("ImportOptions", this);

    }

    #endregion Version 0

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTablesAggregationDataSource"/> class during XML deserialization.
    /// </summary>
    /// <param name="info">The XML deserialization info.</param>
    /// <param name="version">The serialized version.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the version is not supported.</exception>
    protected DataTablesAggregationDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization




    /// <summary>
    /// Initializes a new instance of the <see cref="DataTablesAggregationDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data that designates the original source of data used for aggregation.</param>
    /// <param name="dataSourceOptions">The aggregation options.</param>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="inputData"/>, <paramref name="dataSourceOptions"/>, or <paramref name="importOptions"/> is <c>null</c>.
    /// </exception>
    public DataTablesAggregationDataSource(DataTablesAggregationProcessData inputData, DataTablesAggregationOptions dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      using (var token = SuspendGetToken())
      {
        ProcessOptions = dataSourceOptions;
        ImportOptions = importOptions;
        ProcessData = inputData;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTablesAggregationDataSource"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public DataTablesAggregationDataSource(DataTablesAggregationDataSource from)
    {
      CopyFrom(from);
    }


    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    void CopyFrom(DataTablesAggregationDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        DataTablesAggregationOptions? dataSourceOptions = null;
        DataTablesAggregationProcessData? inputData = null;

        _importOptions = from._importOptions;
        dataSourceOptions = from._processOptions; // immutable
        CopyHelper.Copy(ref inputData, from._processData);

        ProcessOptions = dataSourceOptions;
        ImportOptions = _importOptions;
        ProcessData = inputData;
      }
    }
    /// <summary>
    /// Copies from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>true</c> if anything could be copied from the object; otherwise, <c>false</c>.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is DataTablesAggregationDataSource from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new DataTablesAggregationDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <summary>
    /// Checks the process data and options and returns warnings or errors, if any.
    /// </summary>
    /// <returns>An enumerable of warning or error messages; may be empty.</returns>
    public IEnumerable<string> CheckDataAndOptions()
    {
      var tablesParticipating = ProcessData.UpdateTableProxiesAndGetSourceTables(null);

      if (ProcessOptions.AggregationKinds.Count == 0)
        yield return "There are no aggregations chosen, thus the resulting table would be empty.";

      if (ProcessOptions.AggregatedColumnNames.Count == 0)
        yield return "There are no columns to aggregate chosen, so the resulting table would be empty";

      foreach (var table in tablesParticipating)
      {
        foreach (var columnName in ProcessOptions.AggregatedColumnNames)
        {
          if (!table.DataColumns.Contains(columnName))
          {
            yield return $"col[\"{columnName}\"] is missing in table '{table.Name}'";
            continue;
          }

          foreach (var clusteredPropertyName in ProcessOptions.ClusteredPropertiesNames)
          {
            bool propertyExists = false;
            if (table.DataColumns.Contains(clusteredPropertyName))
            {
              propertyExists = true;
            }
            if (table.PropCols.Contains(clusteredPropertyName))
            {
              propertyExists = true;
            }
            var prop = IndependentAndDependentColumns.GetPropertyValueOfColumn(table.DataColumns[columnName], clusteredPropertyName, table, null);
            if (!prop.IsEmpty)
            {
              propertyExists = true;
            }

            if (!propertyExists)
            {
              yield return $"The name of the clustered property '{clusteredPropertyName}' is neither a data column name in table '{table.Name}' nor a property of the column '{columnName}' in this table.";
            }
          }
        }
      }
    }


    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter">The progress reporter.</param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      destinationTable.DataColumns.RemoveColumnsAll();
      destinationTable.PropCols.RemoveColumnsAll();

      var tablesParticipating = ProcessData.UpdateTableProxiesAndGetSourceTables(destinationTable);

      foreach (var table in tablesParticipating)
      {
        if (ProcessOptions.ExecuteTablesDataSourceBeforeAggregation && table.DataSource is { } altaxoTableDataSource)
        {
          altaxoTableDataSource.FillData(table, reporter);
        }
        if (ProcessOptions.ExecuteTablesTableScriptBeforeAggregation && table.TableScript is { } tableScript)
        {
          tableScript.ExecuteWithoutExceptionCatching(table, reporter);
        }
      }

      var allAggregationResults = new Dictionary<string, Dictionary<AggregationKey, List<AltaxoVariant>>>();

      foreach (var columnName in ProcessOptions.AggregatedColumnNames)
      {
        var aggregationResults = new Dictionary<AggregationKey, List<AltaxoVariant>>();
        foreach (var table in tablesParticipating)
        {
          if (table.DataColumns.Contains(columnName))
          {
            AggregateDataFromTableColumn(table, columnName, aggregationResults);
          } // if data table contains column
          else // if the table does not contain the column, we try to get the data from the table properties
          {
            AggregateDataFromTableProperty(table, columnName, aggregationResults);
          } // else if the table does not contain the column
        } // for all tables
        allAggregationResults.Add(columnName, aggregationResults);
      } // for each column name (or property name) to aggregate


      WriteAggregationResultsToTable(destinationTable, allAggregationResults);
    }

    /// <summary>
    /// Aggregates values from a data column across all rows and groups them by the configured clustered properties.
    /// Collected values per group are added to <paramref name="aggregationResults"/> under an <see cref="AggregationKey"/>
    /// </summary>
    /// <param name="table">The source table containing the data column.</param>
    /// <param name="columnName">The name of the column to aggregate.</param>
    /// <param name="aggregationResults">A dictionary that collects aggregation lists per aggregation key; entries will be created if missing.</param>
    public void AggregateDataFromTableColumn(DataTable table, string columnName, Dictionary<AggregationKey, List<AltaxoVariant>> aggregationResults)
    {
      var column = table.DataColumns[columnName];

      // now we have to collect the other properties to cluster by
      var propDictColumns = new Dictionary<string, Data.DataColumn>();

      foreach (var clusteredPropertyName in ProcessOptions.ClusteredPropertiesNames)
      {
        if (table.DataColumns.Contains(clusteredPropertyName))
        {
          var clusteredColumn = table.DataColumns[clusteredPropertyName];
          propDictColumns[clusteredPropertyName] = clusteredColumn;
        }
      }
      // now we have to find the other properties to cluster by (that are not provided as data columns)
      var propDictConstants = new Dictionary<string, AltaxoVariant>();
      foreach (var clusteredPropertyName in ProcessOptions.ClusteredPropertiesNames.Where(name => !propDictColumns.ContainsKey(name)))
      {
        var prop = IndependentAndDependentColumns.GetPropertyValueOfColumn(column, clusteredPropertyName, table, null);
        propDictConstants[clusteredPropertyName] = prop;
      }

      foreach (var (segmentStart, segmentEndExclusive) in ProcessData.RowSelection.GetSelectedRowIndexSegmentsFromTo(0, column.Count, table.DataColumns, column.Count))
      {
        for (int rowIndex = segmentStart; rowIndex < segmentEndExclusive; rowIndex++)
        {
          if (column.IsElementEmpty(rowIndex))
            continue;


          var keyValues = new AltaxoVariant[ProcessOptions.ClusteredPropertiesNames.Count];
          var keyValuesContainEmpty = false;
          for (int i = 0; i < ProcessOptions.ClusteredPropertiesNames.Count; i++)
          {
            var clusteredPropertyName = ProcessOptions.ClusteredPropertiesNames[i];
            AltaxoVariant keyValue;
            if (propDictColumns.TryGetValue(clusteredPropertyName, out var clusteredColumn))
            {
              if (!clusteredColumn.IsElementEmpty(rowIndex))
                keyValue = clusteredColumn[rowIndex];
              else
                keyValue = new AltaxoVariant();
            }
            else
            {
              keyValue = propDictConstants[clusteredPropertyName];
            }
            keyValuesContainEmpty |= keyValue.IsEmpty;
            keyValues[i] = keyValue;
          }
          if (!keyValuesContainEmpty)
          {
            var aggregationKey = new AggregationKey(keyValues);
            if (!aggregationResults.ContainsKey(aggregationKey))
            {
              aggregationResults[aggregationKey] = new List<AltaxoVariant>();
            }
            aggregationResults[aggregationKey].Add(column[rowIndex]);
          }
        }
      }
    }

    /// <summary>
    /// Aggregates a value from a table property and adds it to <paramref name="aggregationResults"/> grouped by the configured clustered properties.
    /// If the named property is not present on the table or required clustered properties are missing, nothing is added.
    /// </summary>
    /// <param name="table">The source table containing the property.</param>
    /// <param name="columnName">The name of the table property to aggregate.</param>
    /// <param name="aggregationResults">A dictionary that collects aggregation lists per aggregation key; entries will be created if missing.</param>
    public void AggregateDataFromTableProperty(DataTable table, string columnName, Dictionary<AggregationKey, List<AltaxoVariant>> aggregationResults)
    {
      var tablePropertyObj = table.GetTableProperty(columnName);
      if (tablePropertyObj is not null && GetKeyValuesFromTableProperties(table) is { } keyValues)
      {
        var tableProperty = new AltaxoVariant();
        if (tablePropertyObj is AltaxoVariant av)
          tableProperty = av;
        else
          tableProperty = new AltaxoVariant(tablePropertyObj);


        var aggregationKey = new AggregationKey(keyValues);
        if (!aggregationResults.ContainsKey(aggregationKey))
        {
          aggregationResults[aggregationKey] = new List<AltaxoVariant>();
        }
        aggregationResults[aggregationKey].Add(tableProperty);

      }
    }

    /// <summary>
    /// Writes the aggregated results into the destination table, creating clustered columns and aggregation result columns as needed.
    /// </summary>
    /// <param name="destinationTable">The table into which aggregation results are written.</param>
    /// <param name="allAggregationResults">A dictionary mapping column (or property) names to aggregation results grouped by <see cref="AggregationKey"/>.</param>
    public void WriteAggregationResultsToTable(DataTable destinationTable, Dictionary<string, Dictionary<AggregationKey, List<AltaxoVariant>>> allAggregationResults)
    {
      // now we have all aggregation results collected, we can create the output table
      // first create the clustered columns
      for (int i = 0; i < ProcessOptions.ClusteredPropertiesNames.Count; i++)
      {
        destinationTable.DataColumns.EnsureExistence(ProcessOptions.ClusteredPropertiesNames[i], GetKindOfClusteredColumn(allAggregationResults, i), ColumnKind.X, 0);
      }

      var listOfKeys = allAggregationResults.Values.SelectMany(dict => dict.Keys).Distinct().ToList();
      listOfKeys.Sort();

      int idxRow = -1;
      foreach (var aggregationKey in listOfKeys)
      {
        ++idxRow;
        foreach (var (columnName, aggregationResults) in allAggregationResults)
        {
          if (!aggregationResults.TryGetValue(aggregationKey, out var aggregation))
            continue;

          for (int i = 0; i < ProcessOptions.ClusteredPropertiesNames.Count; ++i)
          {
            destinationTable[ProcessOptions.ClusteredPropertiesNames[i]][idxRow] = aggregationKey.KeyValues[i];
          }


          foreach (var aggregationKind in ProcessOptions.AggregationKinds)
          {
            var c = destinationTable.DataColumns.EnsureExistence($"{columnName}_{aggregationKind}", typeof(DoubleColumn), ColumnKind.V, 0);


            switch (aggregationKind)
            {
              case KindOfAggregation.Mean:
                if (aggregation.Count == 1)
                  c[idxRow] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
                else
                  c[idxRow] = aggregation.Select(v => v.ToDouble()).Average();
                break;

              case KindOfAggregation.StdDev:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).StandardDeviation();
                break;

              case KindOfAggregation.PopulationStdDev:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).PopulationStandardDeviation();
                break;

              case KindOfAggregation.Median:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).Median();
                break;

              case KindOfAggregation.Minimum:
                if (aggregation.Count == 1)
                  c[idxRow] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
                else
                  c[idxRow] = aggregation.Select(v => v.ToDouble()).Min();
                break;

              case KindOfAggregation.Maximum:
                if (aggregation.Count == 1)
                  c[idxRow] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
                else
                  c[idxRow] = aggregation.Select(v => v.ToDouble()).Max();
                break;

              case KindOfAggregation.Count:
                c[idxRow] = aggregation.Count;
                break;

              case KindOfAggregation.Sum:
                if (aggregation.Count == 1)
                  c[idxRow] = aggregation[0]; // if there is only one item, we use the item directly. Thus even DataTime or Text could be possible for the kind of item
                else
                  c[idxRow] = aggregation.Select(v => v.ToDouble()).Sum();
                break;

              case KindOfAggregation.Variance:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).Variance();
                break;

              case KindOfAggregation.PopulationVariance:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).PopulationVariance();
                break;
              case KindOfAggregation.MinimumAbsolute:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).MinimumAbsolute();
                break;

              case KindOfAggregation.MaximumAbsolute:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).MaximumAbsolute();
                break;

              case KindOfAggregation.GeometricMean:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).GeometricMean();
                break;

              case KindOfAggregation.HarmonicMean:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).HarmonicMean();
                break;

              case KindOfAggregation.Skewness:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).Skewness();
                break;

              case KindOfAggregation.PopulationSkewness:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).PopulationSkewness();
                break;

              case KindOfAggregation.Kurtosis:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).Kurtosis();
                break;

              case KindOfAggregation.PopulationKurtosis:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).PopulationKurtosis();
                break;

              case KindOfAggregation.RootMeanSquare:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).RootMeanSquare();
                break;

              case KindOfAggregation.LowerQuartile:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).LowerQuartile();
                break;

              case KindOfAggregation.UpperQuartile:
                c[idxRow] = aggregation.Select(v => v.ToDouble()).UpperQuartile();
                break;
              default:
                throw new NotImplementedException($"The aggregation kind {aggregationKind} is not implemented!");
            }
          }
        }
      }
    }

    /// <summary>
    /// Retrieves the clustered property values from the table's properties. Returns <c>null</c> if any clustered property is missing.
    /// </summary>
    /// <param name="table">The table from which to read the clustered properties.</param>
    /// <returns>An array of <see cref="AltaxoVariant"/> representing the clustered property values, or <c>null</c> if a property is missing.</returns>
    public AltaxoVariant[]? GetKeyValuesFromTableProperties(DataTable table)
    {
      var keyValues = new AltaxoVariant[ProcessOptions.ClusteredPropertiesNames.Count];
      for (int i = 0; i < ProcessOptions.ClusteredPropertiesNames.Count; i++)
      {
        var clusteredPropertyName = ProcessOptions.ClusteredPropertiesNames[i];
        var prop = table.GetTableProperty(clusteredPropertyName);
        if (prop is null)
        {
          return null;
        }
        else if (prop is AltaxoVariant pav)
          keyValues[i] = pav;
        else
          keyValues[i] = new AltaxoVariant(prop);
      }
      return keyValues;
    }


    /// <summary>
    /// Determines the type of clustered column to create based on the aggregated key values.
    /// </summary>
    /// <param name="allAggregationResults">All aggregation results grouped by column and key.</param>
    /// <param name="idxClusterProperty">The index of the clustered property.</param>
    /// <returns>The column type for the clustered property.</returns>
    private Type GetKindOfClusteredColumn(Dictionary<string, Dictionary<AggregationKey, List<AltaxoVariant>>> allAggregationResults, int idxClusterProperty)
    {
      Type? columnType = null;
      int dataTimeCount = 0;
      int numericCount = 0;
      foreach (var aggregationResults in allAggregationResults.Values)
      {
        foreach (var aggregationKey in aggregationResults.Keys)
        {
          var keyValue = aggregationKey.KeyValues[idxClusterProperty];
          if (keyValue.IsEmpty)
            continue;
          if (keyValue.IsType(AltaxoVariant.Content.VDateTime))
            ++dataTimeCount;
          else if (keyValue.IsType(AltaxoVariant.Content.VDouble))
            ++numericCount;
        }
      }

      if (dataTimeCount > 0 && numericCount == 0)
        columnType = typeof(DateTimeColumn);
      else if (numericCount > 0)
        columnType = typeof(DoubleColumn);
      else
        columnType = typeof(TextColumn);

      return columnType ?? typeof(AltaxoVariant);
    }

    /// <summary>
    /// Occurs when the data source has changed and the import trigger source is <see cref="ImportTriggerSource.DataSourceChanged"/>. The argument is the sender of this event.
    /// </summary>
    public event Action<Data.IAltaxoTableDataSource> DataSourceChanged
    {
      add
      {
        bool isFirst = _dataSourceChanged is null;
        _dataSourceChanged += value;
        if (isFirst)
        {
          //EhInputDataChanged(this, EventArgs.Empty);
        }
      }
      remove
      {
        _dataSourceChanged -= value;
        bool isLast = _dataSourceChanged is null;
        if (isLast)
        {
        }
      }
    }

    /// <summary>
    /// Gets or sets the input data.
    /// </summary>
    /// <value>
    /// The input data used as the source for aggregation.
    /// </value>
    public DataTablesAggregationProcessData ProcessData
    {
      get
      {
        return _processData;
      }
      [MemberNotNull(nameof(_processData))]
      set
      {
        if (ChildSetMember(ref _processData, value ?? throw new ArgumentNullException(nameof(value))))
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
    /// <exception cref="ArgumentNullException">ImportOptions</exception>
    public override Data.IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      [MemberNotNull(nameof(_importOptions))]
      set
      {
        if (!object.Equals(_importOptions, value ?? throw new ArgumentNullException(nameof(value))))
        {
          _importOptions = value;
          EhChildChanged(_importOptions, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the options for the aggregation operation.
    /// </summary>
    /// <value>
    /// The aggregation options.
    /// </value>
    /// <exception cref="ArgumentNullException">Thrown when the value is <c>null</c>.</exception>
    public DataTablesAggregationOptions ProcessOptions
    {
      get
      {
        return _processOptions;
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        var oldValue = _processOptions;

        _processOptions = value;
      }
    }

    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => _processOptions;
      set => ProcessOptions = (DataTablesAggregationOptions)value;
    }

    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (DataTablesAggregationProcessData)value;
    }

    #region Change event handling

    /// <inheritdoc/>
    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
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

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processData is not null)
        yield return new Main.DocumentNodeAndName(_processData, "ProcessData");
      // if (null != _processOptions)
      //   yield return new Main.DocumentNodeAndName(_processOptions, "ProcessOptions");
    }

    #endregion Document Node functions

    /// <summary>
    /// Called after deserialization of a data source instance, when it is already associated with a data table.
    /// </summary>
    public void OnAfterDeserialization()
    {
    }

    /// <summary>
    /// Visits all document references.
    /// </summary>
    /// <param name="ReportProxies">The callback used to report proxies.</param>
    public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
      if (_processData is not null)
        _processData.VisitDocumentReferences(ReportProxies);
    }

    #endregion IAltaxoTableDataSource
  }
}
