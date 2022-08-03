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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Science.Spectroscopy.Raman;

namespace Altaxo.Data.Extensions
{
  public class RamanCalibrationDataSource : TableDataSourceBase
  {
    private IDataSourceImportOptions _importOptions;

    NeonCalibrationOptions? _neonCalibrationOptions1;
    DataTableXYColumnProxy?  _neonCalibrationData1;
    NeonCalibrationOptions? _neonCalibrationOptions2;
    DataTableXYColumnProxy? _neonCalibrationData2;
    SiliconCalibrationOptions? _siliconCalibrationOptions;
    DataTableXYColumnProxy? _siliconCalibrationData;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-11-02 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RamanCalibrationDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RamanCalibrationDataSource)obj;

        info.AddValue("ImportOptions", s._importOptions);

        info.AddValueOrNull("NeonData1", s._neonCalibrationData1);
        info.AddValueOrNull("NeonOptions1", s._neonCalibrationOptions1);
        info.AddValueOrNull("NeonData2", s._neonCalibrationData2);
        info.AddValueOrNull("NeonOptions2", s._neonCalibrationOptions2);
        info.AddValueOrNull("SiliconData", s._siliconCalibrationData);
        info.AddValueOrNull("SiliconOptions", s._siliconCalibrationOptions);
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is RamanCalibrationDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new RamanCalibrationDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions) )]
    void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));

      ChildSetMember(ref _neonCalibrationData1, info.GetValueOrNull<DataTableXYColumnProxy>("NeonData1", this));
      _neonCalibrationOptions1 = info.GetValueOrNull<NeonCalibrationOptions>("NeonOptions1", this);
      ChildSetMember(ref _neonCalibrationData2, info.GetValueOrNull<DataTableXYColumnProxy>("NeonData2", this));
      _neonCalibrationOptions2 = info.GetValueOrNull<NeonCalibrationOptions>("NeonOptions2", this);
      ChildSetMember(ref _siliconCalibrationData, info.GetValueOrNull<DataTableXYColumnProxy>("SiliconData", this));
      _siliconCalibrationOptions = info.GetValueOrNull<SiliconCalibrationOptions>("SiliconOptions", this);
    }

    #endregion Version 0

    protected RamanCalibrationDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="RamanCalibrationDataSource"/> class.
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
    public RamanCalibrationDataSource(IDataSourceImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException("importOptions");

      using (var token = SuspendGetToken())
      {
        ImportOptions = importOptions;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RamanCalibrationDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public RamanCalibrationDataSource(RamanCalibrationDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions) )]
    void CopyFrom(RamanCalibrationDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        DataTableXYColumnProxy neon1=null, neon2=null, silicon=null;
        IDataSourceImportOptions importOptions = null;

        CopyHelper.Copy(ref neon1, from._neonCalibrationData1);
        CopyHelper.Copy(ref neon2, from._neonCalibrationData2);
        CopyHelper.Copy(ref silicon, from._siliconCalibrationData);
        CopyHelper.Copy(ref importOptions, from._importOptions);

        if (from._neonCalibrationOptions1 is not null && from._neonCalibrationData1 is not null)
        {
          SetNeonCalibration1(from._neonCalibrationOptions1, neon1);
        }
        if (from._neonCalibrationOptions2 is not null && from._neonCalibrationData2 is not null)
        {
          SetNeonCalibration2(from._neonCalibrationOptions2, neon2);
        }
        if (from._siliconCalibrationOptions is not null && from._siliconCalibrationData is not null)
        {
          SetSiliconCalibration(from._siliconCalibrationOptions, silicon);
        }

        ImportOptions = importOptions;

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

      if (obj is RamanCalibrationDataSource from)
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
      return new RamanCalibrationDataSource(this);
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
        if(_neonCalibrationData1 is { } neondata1 && _neonCalibrationOptions1 is { } neonOptions1)
        {
          SpectroscopyCommands.Raman_CalibrateWithNeonSpectrum(destinationTable, neonOptions1, neondata1.XColumn, neondata1.YColumn);
        }
        if (_neonCalibrationData2 is { } neondata2 && _neonCalibrationOptions2 is { } neonOptions2)
        {
          SpectroscopyCommands.Raman_CalibrateWithNeonSpectrum(destinationTable, neonOptions2, neondata2.XColumn, neondata2.YColumn);
        }
        if (_siliconCalibrationData is { } silicondata && _siliconCalibrationOptions is { } siliconOptions)
        {
          SpectroscopyCommands.Raman_CalibrateWithSiliconSpectrum(destinationTable, siliconOptions, silicondata.XColumn, silicondata.YColumn);
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

    public void SetNeonCalibration1(NeonCalibrationOptions options, DataTableXYColumnProxy data)
    {
      var b1 = ChildSetMember(ref _neonCalibrationData1, data);
      var b2 = !object.Equals(_neonCalibrationOptions1, options);
      _neonCalibrationOptions1 = options;

      if(b1 || b2)
      {
        EhChildChanged(_neonCalibrationData1, EventArgs.Empty);
      }
    }

    public void SetNeonCalibration2(NeonCalibrationOptions options, DataTableXYColumnProxy data)
    {
      var b1 = ChildSetMember(ref _neonCalibrationData2, data);
      var b2 = !object.Equals(_neonCalibrationOptions2, options);
      _neonCalibrationOptions2 = options;

      if (b1 || b2)
      {
        EhChildChanged(_neonCalibrationData2, EventArgs.Empty);
      }
    }


    public void SetSiliconCalibration(SiliconCalibrationOptions options, DataTableXYColumnProxy data)
    {
      var b1 = ChildSetMember(ref _siliconCalibrationData, data);
      var b2 = !object.Equals(_siliconCalibrationOptions, options);
      _siliconCalibrationOptions = options;

      if (b1 || b2)
      {
        EhChildChanged(_siliconCalibrationData, EventArgs.Empty);
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

    #region Change event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(_neonCalibrationData1, sender) ||
        object.ReferenceEquals(_neonCalibrationData2, sender) ||
        object.ReferenceEquals(_siliconCalibrationData, sender) 
        ) // incoming call from data proxy
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
      if (_neonCalibrationData1 is not null)
        yield return new Main.DocumentNodeAndName(_neonCalibrationData1, "NeonCalibrationData1");
      if (_neonCalibrationData2 is not null)
        yield return new Main.DocumentNodeAndName(_neonCalibrationData2, "NeonCalibrationData2");
      if (_siliconCalibrationData is not null)
        yield return new Main.DocumentNodeAndName(_siliconCalibrationData, "SiliconCalibrationData");
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
      if (_neonCalibrationData1 is not null)
        _neonCalibrationData1.VisitDocumentReferences(ReportProxies);
      if (_neonCalibrationData2 is not null)
        _neonCalibrationData2.VisitDocumentReferences(ReportProxies);
      if (_siliconCalibrationData is not null)
        _siliconCalibrationData.VisitDocumentReferences(ReportProxies);
    }

    #endregion IAltaxoTableDataSource
  }

}
