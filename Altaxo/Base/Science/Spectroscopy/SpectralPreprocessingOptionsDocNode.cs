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
using System.Collections.Immutable;
using Altaxo.Data;
using Altaxo.Main;
using Altaxo.Science.Spectroscopy.Calibration;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Wrapper around <see cref="SpectralPreprocessingOptions"/>
  /// that keeps track of nodes that have references to tables
  /// (currently only XCalibrationByDataSource).
  /// </summary>
  public class SpectralPreprocessingOptionsDocNode : Main.SuspendableDocumentNodeWithEventArgs, ICloneable
  {
   protected SpectralPreprocessingOptions _spectralPreprocessingOptions;

    protected DataTableProxy? _calibrationTableProxy;


    #region Serialization

    /// <summary>
    /// 2022-08-06 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptionsDocNode), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingOptionsDocNode)obj;
        info.AddValue("SpectralPreprocessingOptions", s.GetSpectralPreprocessingOptions());
        info.AddValueOrNull("CalibrationTableProxy", s._calibrationTableProxy);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var options = info.GetValue<SpectralPreprocessingOptions>("SpectralPreprocessingOptions", null);
        var calibrationTableProxy = info.GetValueOrNull<DataTableProxy>("CalibrationTableProxy", parent);

        return new SpectralPreprocessingOptionsDocNode(options, calibrationTableProxy);
      }
    }

    #endregion

    protected SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptions options, DataTableProxy? calibrationProxy)
    {
      _spectralPreprocessingOptions = options;
      ChildSetMember(ref _calibrationTableProxy, calibrationProxy);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SpectralPreprocessingOptionsDocNode"/> class.
    /// </summary>
    /// <param name="options">The spectral preprocessing options to wrap.</param>
    public SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptions options)
    {
      _spectralPreprocessingOptions = options;

      if (options.Calibration is IReferencingTable xds)
      {
        if (Current.Project.DataTableCollection.TryGetValue(xds.TableName, out var table))
        {
          ChildSetMember(ref _calibrationTableProxy, new DataTableProxy(table));
        }
      }
    }

    public SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptionsDocNode from)
    {
      this._spectralPreprocessingOptions = from._spectralPreprocessingOptions;
      ChildSetMember(ref _calibrationTableProxy, (DataTableProxy?)from._calibrationTableProxy?.Clone());
    }

    public object Clone()
    {
      return new SpectralPreprocessingOptionsDocNode(this);
    }

    /// <inheritdoc/>
    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_calibrationTableProxy is not null)
        yield return new DocumentNodeAndName(_calibrationTableProxy, nameof(_calibrationTableProxy));
    }



    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public SpectralPreprocessingOptions GetSpectralPreprocessingOptions()
    {
        if (_calibrationTableProxy is not null && _calibrationTableProxy.Document is { } table)
        {
          if (_spectralPreprocessingOptions.Calibration is IXCalibrationTable xct)
          {
            // Update the calibration data
            if (table.DataSource is IXCalibrationDataSource xcds && xcds.IsContainingValidXAxisCalibration(table))
            {
              var xCalibrationData = xcds.GetXAxisCalibration(table);
              _spectralPreprocessingOptions = _spectralPreprocessingOptions with
              {
                Calibration = (ICalibration)(xct.WithCalibrationTable(xCalibrationData.ToImmutableArray()))
              };
            }
          }

          // Change the name of the table in the Calibration processor, if it is different
          if (_spectralPreprocessingOptions.Calibration is IReferencingTable xds && xds.TableName != table.Name)
          {
            _spectralPreprocessingOptions = _spectralPreprocessingOptions with
            {
              Calibration = (ICalibration)(xds.WithTableName(table.Name))
            };
          }
        }
        return _spectralPreprocessingOptions;
      
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="T:Altaxo.Main.DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
    {
      using (var suspendToken = SuspendGetToken()) 
      {
        Report(_calibrationTableProxy, this, "CalibrationTableProxy");
        suspendToken.Resume();
      }
    }
  }
}
