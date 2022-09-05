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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Data;
using Altaxo.Worksheet.Commands.Analysis;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Data source of a table that contains predicted data. The data were predicted from a multivariate model (a table containing a <see cref="DimensionReductionAndRegressionDataSource"/>),
  /// and a table containing the spectra used for prediction.
  /// </summary>
  public class DimensionReductionAndRegressionPredictionDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private DimensionReductionAndRegressionPredictionProcessData _processData;
    private IDataSourceImportOptions _importOptions;
    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-07-19 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionAndRegressionPredictionDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionAndRegressionPredictionDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ImportOptions", s._importOptions);
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is DimensionReductionAndRegressionPredictionDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new DimensionReductionAndRegressionPredictionDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processData))]
    void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
     ChildSetMember(ref _processData, (DimensionReductionAndRegressionPredictionProcessData)info.GetValue("ProcessData", this));
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));

      ProcessData = _processData;
    }



    #endregion Version 0

    protected DimensionReductionAndRegressionPredictionDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="DimensionReductionAndRegressionPredictionDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the location of the model table, and the data to predict.</param>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="System.ArgumentNullException">
    /// inputData
    /// or
    /// transformationOptions
    /// or
    /// importOptions
    /// </exception>
    public DimensionReductionAndRegressionPredictionDataSource(DimensionReductionAndRegressionPredictionProcessData inputData, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      using (var token = SuspendGetToken())
      {
        ImportOptions = importOptions;
        ProcessData = inputData;
      }
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public DimensionReductionAndRegressionPredictionDataSource(DimensionReductionAndRegressionPredictionDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processData))]
    void CopyFrom(DimensionReductionAndRegressionPredictionDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        DimensionReductionAndRegressionPredictionProcessData? inputData = null;
        IDataSourceImportOptions? importOptions = null;

        CopyHelper.Copy(ref importOptions, from._importOptions);
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

      if (obj is DimensionReductionAndRegressionPredictionDataSource from)
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
      return new DimensionReductionAndRegressionPredictionDataSource(this);
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
        var modelTable = _processData.TableWithModel.Document;
        if (modelTable is null)
          throw new InvalidOperationException("ProcessData link to model table is broken.");
        if (modelTable.DataSource is not DimensionReductionAndRegressionDataSource dataSource)
          throw new InvalidOperationException($"DataSource of table {modelTable.Name} is {modelTable.DataSource}, but type {typeof(DimensionReductionAndRegressionDataSource)} is expected.");

        var (preferredNumberOfFactors, _) = ChemometricCommands.GetPreferredNumberOfFactorsAndNumberOfConcentrations(modelTable);
        if (preferredNumberOfFactors < 0)
          return;

        var analysis = ChemometricCommands.GetAnalysis(modelTable);

        if (analysis is not null)
        {
          var table = _processData.DataToPredict.DataTable;
          var xColumn = _processData.DataToPredict.RowHeaderColumn;

          analysis.PredictValues(
            _processData.DataToPredict,
            preferredNumberOfFactors,
            modelTable,
            destinationTable);
        }
      }
      catch (Exception ex)
      {
        destinationTable.Notes.WriteLine("Error during execution of data source ({0}): {1}", GetType().Name, ex.Message);
      }
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
    public DimensionReductionAndRegressionPredictionProcessData ProcessData
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
    public Data.IDataSourceImportOptions ImportOptions
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
    /// Gets or sets the data source options. Here, null is returned.
    /// </summary>
    /// <value>
    /// The data source options.
    /// </value>
    public DimensionReductionAndRegressionOptions ProcessOptions
    {
      get
      {
        return null;
      }
      set
      {
      }
    }



    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => null;
      set { }
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (DimensionReductionAndRegressionPredictionProcessData)value;
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
