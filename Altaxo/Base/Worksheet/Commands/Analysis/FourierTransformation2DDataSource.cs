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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Data source for a table that was created by a two-dimensional Fourier transformation.
  /// </summary>
  public class FourierTransformation2DDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private RealFourierTransformation2DOptions _transformationOptions;
    private DataTableMatrixProxy _inputData;
    private IDataSourceImportOptions _importOptions;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-07-08 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourierTransformation2DDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is FourierTransformation2DDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new FourierTransformation2DDataSource(info, 0);
        return s;
      }

      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FourierTransformation2DDataSource)obj;

        info.AddValue("InputData", s._inputData);
        info.AddValue("TransformationOptions", s._transformationOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_inputData), nameof(_transformationOptions))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _inputData = (DataTableMatrixProxy)info.GetValue("InputData", this);
      if (_inputData is not null)
        _inputData.ParentObject = this;

      _transformationOptions = (RealFourierTransformation2DOptions)info.GetValue("TransformationOptions", this);
      _importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", this);

      InputData = _inputData;
    }

    #endregion Version 0

    protected FourierTransformation2DDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="FourierTransformation2DDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the original source of data (used then for the Fourier transformation).</param>
    /// <param name="transformationOptions">The Fourier transformation options.</param>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="System.ArgumentNullException">
    /// inputData
    /// or
    /// transformationOptions
    /// or
    /// importOptions
    /// </exception>
    public FourierTransformation2DDataSource(DataTableMatrixProxy inputData, RealFourierTransformation2DOptions transformationOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (transformationOptions is null)
        throw new ArgumentNullException(nameof(transformationOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      using (var token = SuspendGetToken())
      {
        FourierTransformation2DOptions = transformationOptions;
        ImportOptions = importOptions;
        InputData = inputData;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FourierTransformation2DDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public FourierTransformation2DDataSource(FourierTransformation2DDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_transformationOptions), nameof(_inputData))]
    protected void CopyFrom(FourierTransformation2DDataSource from)
    {
      ChildCopyToMember(ref _importOptions, from._importOptions);
      ChildCopyToMember(ref _transformationOptions, from._transformationOptions);
      ChildCopyToMember(ref _inputData, from._inputData);
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

      if (obj is FourierTransformation2DDataSource from)
      {
        using (var token = SuspendGetToken())
        {
          CopyFrom(from);

          return true;
        }
      }
      return false;
    }



    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_inputData is not null)
        yield return new Main.DocumentNodeAndName(_inputData, () => _inputData = null!, "Data");

      if (_transformationOptions is not null)
        yield return new Main.DocumentNodeAndName(_transformationOptions, () => _transformationOptions = null!, "TransformationOptions");

      if (_importOptions is not null)
        yield return new Main.DocumentNodeAndName(_importOptions, () => _importOptions = null!, "ImportOptions");
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return new FourierTransformation2DDataSource(this);
    }

    /// <summary>
    /// Fills (or refills) the data table with the 2D-Fourier transformation of the original data.. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    public void FillData(DataTable destinationTable)
    {
      try
      {
        FourierCommands.ExecuteFouriertransformation2D(_inputData, _transformationOptions, destinationTable);
      }
      catch (Exception ex)
      {
        destinationTable.Notes.WriteLine("Error during execution of data source ({0}): {1}", GetType().Name, ex.Message);
      }
    }

    /// <summary>
    /// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
    /// </summary>
    public event Action<Data.IAltaxoTableDataSource>? DataSourceChanged
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
    public DataTableMatrixProxy InputData
    {
      get
      {
        return _inputData;
      }
      [MemberNotNull(nameof(_inputData))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(InputData));

        if (ChildSetMember(ref _inputData, value))
        {
          EhInputDataChanged(this, EventArgs.Empty);
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
        if (value is null)
          throw new ArgumentNullException(nameof(ImportOptions));

        if (ChildSetMember(ref _importOptions, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets or sets the options for the 2D Fourier transformation.
    /// </summary>
    /// <value>
    /// The 2D Fourier transformation options.
    /// </value>
    /// <exception cref="System.ArgumentNullException">FourierTransformation2DOptions</exception>
    public RealFourierTransformation2DOptions FourierTransformation2DOptions
    {
      get
      {
        return _transformationOptions;
      }
      [MemberNotNull(nameof(_transformationOptions))]
      set
      {
        if (value is null)
          throw new ArgumentNullException("FourierTransformation2DOptions");

        if (ChildSetMember(ref _transformationOptions, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Called when the input data have changed. Depending on the <see cref="ImportOptions"/>, the input data may be reprocessed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void EhInputDataChanged(object sender, EventArgs e)
    {
      if (_importOptions.ImportTriggerSource == ImportTriggerSource.DataSourceChanged)
      {
        EhChildChanged(sender, TableDataSourceChangedEventArgs.Empty);
      }
    }

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
      if (_inputData is not null)
        _inputData.VisitDocumentReferences(ReportProxies);
    }
  }
}
