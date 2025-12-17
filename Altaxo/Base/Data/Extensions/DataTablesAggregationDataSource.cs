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
  public class DataTablesAggregationDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    public struct AggregationKey : IEquatable<AggregationKey>, IComparable<AggregationKey>
    {
      public AltaxoVariant[] KeyValues;

      public AggregationKey(AltaxoVariant[] keyValues)
      {
        KeyValues = keyValues;
      }

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

      public readonly bool Equals(AggregationKey other)
      {
        if (KeyValues.Length != other.KeyValues.Length) return false;
        for (int i = 0; i < KeyValues.Length; i++)
        {
          if (!KeyValues[i].Equals(other.KeyValues[i])) return false;
        }
        return true;
      }

      public override readonly int GetHashCode()
      {
        int hash = 17;
        foreach (var val in KeyValues)
        {
          hash = hash * 31 + val.GetHashCode();
        }
        return hash;
      }
    }

    private DataTablesAggregationOptions _processOptions;
    private DataTablesAggregationProcessData _processData;
    private IDataSourceImportOptions _importOptions;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2015-08-26 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTablesAggregationDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataTablesAggregationDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }



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
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));

    }

    #endregion Version 0

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
    /// Initializes a new instance of the <see cref="ExpandCyclingVariableColumnDataSource"/> class.
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
        IDataSourceImportOptions? importOptions = null;

        CopyHelper.Copy(ref importOptions, from._importOptions);
        dataSourceOptions = from._processOptions; // immutable
        CopyHelper.Copy(ref inputData, from._processData);

        ProcessOptions = dataSourceOptions;
        ImportOptions = importOptions;
        ProcessData = inputData;
      }
    }
    /// <summary>
    /// Copies from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>True</c> if anything could be copied from the object, otherwise <c>false</c>.</returns>
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

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return new DataTablesAggregationDataSource(this);
    }

    #region IAltaxoTableDataSource

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
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      destinationTable.DataColumns.RemoveColumnsAll();
      destinationTable.PropCols.RemoveColumnsAll();

      var tablesParticipating = ProcessData.UpdateTableProxiesAndGetSourceTables(destinationTable);

      var allAggregationResults = new Dictionary<string, Dictionary<AggregationKey, List<AltaxoVariant>>>();

      foreach (var columnName in ProcessOptions.AggregatedColumnNames)
      {
        var aggregationResults = new Dictionary<AggregationKey, List<AltaxoVariant>>();
        foreach (var table in tablesParticipating)
        {
          if (table.DataColumns.Contains(columnName))
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


            for (int rowIndex = 0; rowIndex < column.Count; rowIndex++)
            {
              if (column.IsElementEmpty(rowIndex))
                continue;

              var keyValues = new AltaxoVariant[ProcessOptions.ClusteredPropertiesNames.Count];
              for (int i = 0; i < ProcessOptions.ClusteredPropertiesNames.Count; i++)
              {
                var clusteredPropertyName = ProcessOptions.ClusteredPropertiesNames[i];
                if (propDictColumns.TryGetValue(clusteredPropertyName, out var clusteredColumn))
                {
                  keyValues[i] = clusteredColumn[rowIndex];
                }
                else
                {
                  keyValues[i] = propDictConstants[clusteredPropertyName];
                }
              }
              var aggregationKey = new AggregationKey(keyValues);
              if (!aggregationResults.ContainsKey(aggregationKey))
              {
                aggregationResults[aggregationKey] = new List<AltaxoVariant>();
              }
              aggregationResults[aggregationKey].Add(column[rowIndex]);
            }
          }
        } // for all tables
        allAggregationResults[columnName] = aggregationResults;
      }

      // now we have all aggregation results collected, we can create the output table
      // first create the clustered columns
      for (int i = 0; i < ProcessOptions.ClusteredPropertiesNames.Count; i++)
      {
        destinationTable.DataColumns.EnsureExistence(ProcessOptions.ClusteredPropertiesNames[i], GetKindOfClusteredColumn(allAggregationResults, i), ColumnKind.X, 0);
      }
      var listOfKeys = allAggregationResults.Values.SelectMany(dict => dict.Keys).Distinct().ToList();
      listOfKeys.Sort();

      int idxRow = 0;
      foreach (var aggregationKey in listOfKeys)
      {
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

            if (aggregationKind == KindOfAggregation.Count)
            {
              c[idxRow] = aggregation.Count;
            }
            else if (aggregationKind == KindOfAggregation.Mean)
            {
              c[idxRow] = aggregation.Select(v => v.ToDouble()).Average();
            }
            else if (aggregationKind == KindOfAggregation.Sum)
            {
              c[idxRow] = aggregation.Select(v => v.ToDouble()).Sum();
            }
            else if (aggregationKind == KindOfAggregation.Minimum)
            {
              c[idxRow] = aggregation.Select(v => v.ToDouble()).Min();
            }
            else if (aggregationKind == KindOfAggregation.Maximum)
            {
              c[idxRow] = aggregation.Select(v => v.ToDouble()).Max();
            }
            else if (aggregationKind == KindOfAggregation.StdDev)
            {
              c[idxRow] = aggregation.Select(v => v.ToDouble()).StandardDeviation();
            }
            else if (aggregationKind == KindOfAggregation.SStdDev)
            {
              c[idxRow] = aggregation.Select(v => v.ToDouble()).PopulationStandardDeviation();
            }
            else if (aggregationKind == KindOfAggregation.Median)
            {
              c[idxRow] = aggregation.Select(v => v.ToDouble()).PopulationStandardDeviation();
            }

          }
          ++idxRow;
        }
      }
    }


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
    /// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
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
    /// The input data. This data is the input for the 2D-Fourier transformation.
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
    /// <exception cref="System.ArgumentNullException">ImportOptions</exception>
    public override Data.IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      [MemberNotNull(nameof(_importOptions))]
      set
      {
        if (ChildSetMember(ref _importOptions, value ?? throw new ArgumentNullException(nameof(value))))
        {
          EhChildChanged(_importOptions, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the options for the transpose operation.
    /// </summary>
    /// <value>
    /// The transpose options.
    /// </value>
    /// <exception cref="System.ArgumentNullException">FourierTransformation2DOptions</exception>
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

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => _processOptions;
      set => ProcessOptions = (DataTablesAggregationOptions)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (DataTablesAggregationProcessData)value;
    }

    #region Change event handling

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

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processData is not null)
        yield return new Main.DocumentNodeAndName(_processData, "ProcessData");
      // if (null != _processOptions)
      //   yield return new Main.DocumentNodeAndName(_processOptions, "ProcessOptions");
      if (_importOptions is not null)
        yield return new Main.DocumentNodeAndName(_importOptions, "ImportOptions");
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
      if (_processData is not null)
        _processData.VisitDocumentReferences(ReportProxies);
    }

    #endregion IAltaxoTableDataSource
  }
}
