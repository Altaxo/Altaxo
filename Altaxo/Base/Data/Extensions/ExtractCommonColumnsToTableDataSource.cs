#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  public class ExtractCommonColumnsToTableDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private ExtractCommonColumnsToTableOptions _processOptions;
    private ExtractCommonColumnsToTableData _processData;
    private IDataSourceImportOptions _importOptions;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-11-02 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExtractCommonColumnsToTableDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExtractCommonColumnsToTableDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is ExtractCommonColumnsToTableDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new ExtractCommonColumnsToTableDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (ExtractCommonColumnsToTableData)info.GetValue("ProcessData", this));
      _processOptions = info.GetValue<ExtractCommonColumnsToTableOptions>("ProcessOptions", this);
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
    }

    #endregion Version 0

    protected ExtractCommonColumnsToTableDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="ExtractCommonColumnsToTableDataSource"/> class.
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
    public ExtractCommonColumnsToTableDataSource(ExtractCommonColumnsToTableData inputData, ExtractCommonColumnsToTableOptions dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException("inputData");
      if (dataSourceOptions is null)
        throw new ArgumentNullException("dataSourceOptions");
      if (importOptions is null)
        throw new ArgumentNullException("importOptions");


      _processOptions = dataSourceOptions;
      ChildSetMember(ref _importOptions, importOptions);
      ChildSetMember(ref _processData, inputData);

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtractCommonColumnsToTableDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public ExtractCommonColumnsToTableDataSource(ExtractCommonColumnsToTableDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    void CopyFrom(ExtractCommonColumnsToTableDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        ExtractCommonColumnsToTableData? inputData = null;
        IDataSourceImportOptions? importOptions = null;

        CopyHelper.Copy(ref importOptions, from._importOptions);
        _processOptions = from._processOptions;
        CopyHelper.Copy(ref inputData, from._processData);

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

      if (obj is ExtractCommonColumnsToTableDataSource from)
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
      return new ExtractCommonColumnsToTableDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      Execute(destinationTable, _processData, _processOptions);
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
    /// Gets all the names of the columns common to all tables in a unordered fashion.
    /// </summary>
    /// <returns>The names of all the columns common to all tables in a unordered fashion</returns>
    public static HashSet<string> GetCommonColumnNamesUnordered(IReadOnlyList<DataTable> tables)
    {
      if (tables.Count == 0)
        return new HashSet<string>();

      // now determine which columns are common to all selected tables.
      var commonColumnNames = new HashSet<string>(tables[0].DataColumns.GetColumnNames());
      for (int i = 1; i < tables.Count; i++)
        commonColumnNames.IntersectWith(tables[i].DataColumns.GetColumnNames());
      return commonColumnNames;
    }

    /// <summary>
    /// Gets all the names of the columns common to all tables in the order as the columns appear in the first table.
    /// </summary>
    /// <returns>The names of all the columns common to all tables in the order as the columns appear in the first table.</returns>
    public static List<string> GetCommonColumnNamesOrderedByAppearanceInFirstTable(IReadOnlyList<DataTable> tables)
    {
      if (tables.Count == 0)
        return new List<string>();

      var commonColumnNames = GetCommonColumnNamesUnordered(tables);
      var result = new List<string>();
      foreach (var name in tables[0].DataColumns.GetColumnNames())
      {
        // Note: we will add the column names in the order like in the first table
        if (commonColumnNames.Contains(name))
          result.Add(name);
      }
      return result;
    }

    /// <summary>
    /// Executes the 'Plot common column' command.
    /// </summary>
    public static void Execute(DataTable destinationTable, ExtractCommonColumnsToTableData data, ExtractCommonColumnsToTableOptions options)
    {
      bool useCommonX = true;

      /* ------------ Create destination table --------------------------------------
      var commonFolderName = Main.ProjectFolder.GetCommonFolderOfNames(_tables.Select(table => table.Name));
      DataTable destinationTable = new DataTable();
      var tableName = commonFolderName + "WExtractedData";
      tableName = Current.Project.DataTableCollection.FindNewItemName(tableName);
      destinationTable.Name = tableName;
      Current.Project.DataTableCollection.Add(destinationTable);
      */



      // we grab everything (source x-columns, y-columns) beforehand,
      // because if that fails, we do not want to clear the destination table!

      var tables = data.Tables.Select(proxy => proxy.Document ?? throw new InvalidOperationException($"Table '{proxy?.DocumentPath()}' is not found!"));
      var cachedData = new List<(DataTable Table, DataColumn XColumn, DataColumn[] YColumns)>();


      foreach (var table in tables)
      {
        var xCol = table.DataColumns.TryGetColumn(data.XColumnName) ?? throw new InvalidOperationException($"Table '{table.Name}' does not contain the x-column '{data.XColumnName}'");
        var yCol = new DataColumn[data.YColumnNames.Length];

        for (int i = 0; i < data.YColumnNames.Length; ++i)
          yCol[i] = table.DataColumns.TryGetColumn(data.YColumnNames[i]) ?? throw new InvalidOperationException($"Table '{table.Name}' does not contain the column '{data.YColumnNames[i]}'");

        cachedData.Add((table, xCol, yCol));
      }

      // so, now that everything is grabbed, we can safely clear the data in the destination table
      using var token = destinationTable.SuspendGetToken();
      destinationTable.DataColumns.RemoveColumnsAll();
      destinationTable.PropertyColumns.RemoveColumnsAll();




      // Dictionary that has the x-value as key, and its row index as value
      var xToIndex = new Dictionary<AltaxoVariant, int>();
      var xDstColumnName = !string.IsNullOrEmpty(options.UserDefinedNameForXColumn) ? options.UserDefinedNameForXColumn : data.XColumnName;
      var xDstCol = destinationTable.DataColumns.EnsureExistence(xDstColumnName, cachedData[0].XColumn.GetType(), ColumnKind.X, 0);


      if (options.UseResampling) // Use resampling
      {
        double rangeStart = double.NegativeInfinity, rangeEnd = double.PositiveInfinity;
        foreach (var t in cachedData)
        {
          double min = double.PositiveInfinity, max = double.NegativeInfinity;
          var len = t.XColumn.Count;
          for (int i = 0; i < len; ++i)
          {
            double x = t.XColumn[i];
            min = Math.Min(min, x);
            max = Math.Max(max, x);
          }

          rangeStart = Math.Max(rangeStart, min);
          rangeEnd = Math.Min(rangeEnd, max);
        }

        double start = options.UserSpecifiedInterpolationStart ?? rangeStart;
        double end = options.UserSpecifiedInterpolationEnd ?? rangeEnd;

        if (!(start < end))
        {
          throw new InvalidOperationException($"Interpolation start {start} is greater than end {end}");
        }

        var step = Math.Abs(options.InterpolationInterval);

        var approximateNumberOfPoints = (end - start) / step;

        if (approximateNumberOfPoints > int.MaxValue / sizeof(double))
        {
          throw new InvalidOperationException($"Interpolation start {start}, end {end}, and step ({step}) will result in too many points (approx. {Math.Round(approximateNumberOfPoints)})");
        }

        int numberOfPoints = (int)Math.Ceiling(approximateNumberOfPoints);
        for (int i = 0; i < numberOfPoints; ++i)
        {
          var x = start + i * step;
          xToIndex[x] = i;
          xDstCol[i] = x;
        }
      }
      else // no resampling
      {
        var setOfXValues = new List<HashSet<AltaxoVariant>>();
        foreach (var t in cachedData)
        {
          var hash = new HashSet<AltaxoVariant>();
          var len = t.XColumn.Count;
          for (int i = 0; i < len; ++i)
            hash.Add(t.XColumn[i]);
          setOfXValues.Add(hash);
        }

        if (setOfXValues.Count == 0)
          return;

        var commonX = new HashSet<AltaxoVariant>(setOfXValues[0]);


        if (useCommonX)
        {
          for (int i = 1; i < setOfXValues.Count; i++)
            commonX.IntersectWith(setOfXValues[i]);
        }
        else // union
        {
          for (int i = 1; i < setOfXValues.Count; i++)
            commonX.UnionWith(setOfXValues[i]);
        }

        if (commonX.Count == 0)
          return;

        var sortedX = commonX.ToList();
        sortedX.Sort();
        for (int i = 0; i < sortedX.Count; i++)
        {
          xToIndex[sortedX[i]] = i;
          xDstCol[i] = sortedX[i];
        }
      }



      var pcolOriginalTableName = options.CreatePropertyColumnWithSourceTableName ?
        destinationTable.PropertyColumns.EnsureExistence("OriginalTableName", typeof(TextColumn), ColumnKind.V, 0) :
        null;

      if (options.PlaceMultipleYColumnsAdjacentInDestinationTable)
      {
        int idxTable = -1;
        foreach (var t in cachedData)
        {
          idxTable++;
          var xSrcCol = t.XColumn;
          var len = xSrcCol.Count;
          for (int iy = 0; iy < t.YColumns.Length; ++iy)
          {
            var yDstColumnName = (iy < options.UserDefinedNamesForYColumns.Length && !string.IsNullOrEmpty(options.UserDefinedNamesForYColumns[iy])) ?
                                 options.UserDefinedNamesForYColumns[iy] :
                                 data.YColumnNames[iy];

            var yDstCol = destinationTable.DataColumns.EnsureExistence($"{yDstColumnName}.{idxTable}", t.YColumns[iy].GetType(), ColumnKind.V, 0);

            if (options.UseResampling)
            {
              var xArr = ((DoubleColumn)xSrcCol).ToArray();
              var yArr = ((DoubleColumn)t.YColumns[iy]).ToArray();
              Array.Sort(xArr, yArr);
              var interpolatingFunction = options.Interpolation.Interpolate(xArr, yArr);
              for (int i = 0; i < xDstCol.Count; ++i)
              {
                yDstCol[i] = interpolatingFunction.GetYOfX(xDstCol[i]);
              }
            }
            else
            {
              for (int srcIdx = 0; srcIdx < len; ++srcIdx)
              {
                if (xToIndex.TryGetValue(xSrcCol[srcIdx], out int dstIdx))
                {
                  yDstCol[dstIdx] = t.YColumns[iy][srcIdx];
                }
              }
            }

            { // deal with the property columns
              if (pcolOriginalTableName is not null)
              {
                pcolOriginalTableName[destinationTable.DataColumns.GetColumnNumber(yDstCol)] = t.Table.Name;
              }
              if (options.CopyColumnProperties)
              {
                var idxDstCol = destinationTable.DataColumns.GetColumnNumber(yDstCol);
                var idxSrcCol = t.Table.DataColumns.GetColumnNumber(t.YColumns[iy]);

                for (int ipc = 0; ipc < t.Table.PropertyColumns.ColumnCount; ++ipc)
                {
                  var srcPCol = t.Table.PropertyColumns[ipc];
                  if (!srcPCol.IsElementEmpty(idxSrcCol))
                  {
                    var dstpCol = destinationTable.PropertyColumns.EnsureExistence(t.Table.PropertyColumns.GetColumnName(srcPCol), srcPCol.GetType(), t.Table.PropertyColumns.GetColumnKind(srcPCol), t.Table.PropertyColumns.GetColumnGroup(srcPCol));
                    dstpCol[idxDstCol] = srcPCol[idxSrcCol];
                  }
                }
              }
            } // end deal with property columns

          } // end inner loop
        } // end outer loop
      }
      else // don't place the y-columns adjacent
      {
        for (int iy = 0; iy < data.YColumnNames.Length; ++iy)
        {
          int idxTable = -1;
          foreach (var t in cachedData)
          {
            idxTable++;
            var xSrcCol = t.XColumn;
            var len = xSrcCol.Count;

            var yDstColumnName = (iy < options.UserDefinedNamesForYColumns.Length && !string.IsNullOrEmpty(options.UserDefinedNamesForYColumns[iy])) ?
                     options.UserDefinedNamesForYColumns[iy] :
                     data.YColumnNames[iy];

            var yDstCol = destinationTable.DataColumns.EnsureExistence($"{yDstColumnName}.{idxTable}", t.YColumns[iy].GetType(), ColumnKind.V, 0);

            if (options.UseResampling)
            {
              var xArr = ((DoubleColumn)xSrcCol).ToArray();
              var yArr = ((DoubleColumn)t.YColumns[iy]).ToArray();
              Array.Sort(xArr, yArr);
              var interpolatingFunction = options.Interpolation.Interpolate(xArr, yArr);
              for (int i = 0; i < xDstCol.Count; ++i)
              {
                yDstCol[i] = interpolatingFunction.GetYOfX(xDstCol[i]);
              }
            }
            else
            {
              for (int srcIdx = 0; srcIdx < len; ++srcIdx)
              {
                if (xToIndex.TryGetValue(xSrcCol[srcIdx], out int dstIdx))
                {
                  yDstCol[dstIdx] = t.YColumns[iy][srcIdx];
                }
              }
            }

            { // deal with the property columns

              if (pcolOriginalTableName is not null)
              {
                pcolOriginalTableName[destinationTable.DataColumns.GetColumnNumber(yDstCol)] = t.Table.Name;
              }
              if (options.CopyColumnProperties)
              {
                var idxDstCol = destinationTable.DataColumns.GetColumnNumber(yDstCol);
                var idxSrcCol = t.Table.DataColumns.GetColumnNumber(t.YColumns[iy]);

                for (int ipc = 0; ipc < t.Table.PropertyColumns.ColumnCount; ++ipc)
                {
                  var srcPCol = t.Table.PropertyColumns[ipc];
                  if (!srcPCol.IsElementEmpty(idxSrcCol))
                  {
                    var dstpCol = destinationTable.PropertyColumns.EnsureExistence(t.Table.PropertyColumns.GetColumnName(srcPCol), srcPCol.GetType(), t.Table.PropertyColumns.GetColumnKind(srcPCol), t.Table.PropertyColumns.GetColumnGroup(srcPCol));
                    dstpCol[idxDstCol] = srcPCol[idxSrcCol];
                  }
                }
              }

            } // end deal with property columns
          } // end inner loop
        } // end outer loop
      } // end else
    }

    /// <summary>
    /// Gets or sets the input data.
    /// </summary>
    /// <value>
    /// The input data. This data is the input for the 2D-Fourier transformation.
    /// </value>
    public ExtractCommonColumnsToTableData ProcessData
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
    /// Gets or sets the data source options.
    /// </summary>
    /// <value>
    /// The data source options.
    /// </value>
    /// <exception cref="System.ArgumentNullException">FourierTransformation2DOptions</exception>
    public ExtractCommonColumnsToTableOptions ProcessOptions
    {
      get
      {
        return _processOptions;
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (!object.Equals(_processOptions, value))
        {
          _processOptions = value;
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => _processOptions;
      set => ProcessOptions = (ExtractCommonColumnsToTableOptions)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (ExtractCommonColumnsToTableData)value;
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
      {
        _processData.VisitDocumentReferences(ReportProxies);
      }
    }

    #endregion IAltaxoTableDataSource
  }
}
