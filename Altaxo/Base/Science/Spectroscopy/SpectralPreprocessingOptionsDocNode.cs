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
using System.Linq;
using Altaxo.Data;
using Altaxo.Main;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Serialization.Xml;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Wrapper around <see cref="SpectralPreprocessingOptions"/>
  /// that keeps track of nodes that have references to tables
  /// (currently only XCalibrationByDataSource).
  /// </summary>
  public class SpectralPreprocessingOptionsDocNode : Main.SuspendableDocumentNodeWithEventArgs, ICloneable
  {
    protected SpectralPreprocessingOptionsBase _spectralPreprocessingOptions;

    /// <summary>
    /// Dictionary that contains the spectral preprocessor as value, and a proxy (DataTableProxy, .. other proxies) as value
    /// </summary>
    protected Dictionary<ISingleSpectrumPreprocessor, IDocumentLeafNode> _proxyCache;


    #region Serialization

    /// <summary>
    /// 2022-08-06 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptionsDocNode), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
        var s = (SpectralPreprocessingOptionsDocNode)obj;
        info.AddValue("SpectralPreprocessingOptions", s.GetSpectralPreprocessingOptions());
        info.AddValueOrNull("CalibrationTableProxy", s._calibrationTableProxy);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var options = info.GetValue<SpectralPreprocessingOptions>("SpectralPreprocessingOptions", null);
        var proxyList = DeserializeProxiesVersion0(info, parent, options);
        return new SpectralPreprocessingOptionsDocNode(options, proxyList);
      }

      public static List<(int number, IDocumentLeafNode proxy)> DeserializeProxiesVersion0(IXmlDeserializationInfo info, object? parent, SpectralPreprocessingOptions options)
      {
        var calibrationTableProxy = info.GetValueOrNull<DataTableProxy>("CalibrationTableProxy", parent);
        var proxyList = new List<(int number, IDocumentLeafNode proxy)>(1);
        if (calibrationTableProxy is not null)
        {
          int processorNumber = -1;
          foreach (var processor in options.GetProcessorElements())
          {
            ++processorNumber;
            if (processor is IXCalibration)
            {
              proxyList.Add((processorNumber, calibrationTableProxy));
            }
          }
        }

        return proxyList;
      }
    }

    /// <summary>
    /// 2023-03-29 Extensions to arbitrary processors
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptionsDocNode), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingOptionsDocNode)obj;
        var processingOptions = s.GetSpectralPreprocessingOptions();
        info.AddValue("SpectralPreprocessingOptions", processingOptions);

        SerializeProxiesVersion1(info, s, processingOptions);
      }

      public static void SerializeProxiesVersion1(IXmlSerializationInfo info, SpectralPreprocessingOptionsDocNode s, SpectralPreprocessingOptionsBase processingOptions)
      {
        var proxyList = new List<(int number, object proxy)>(10);
        int processorNumber = -1;
        foreach (var processor in processingOptions.GetProcessorElements())
        {
          ++processorNumber;
          if (s._proxyCache.TryGetValue(processor, out var proxy) && proxy is not null)
          {
            proxyList.Add((processorNumber, proxy));
          }
        }

        info.CreateArray("Proxies", proxyList.Count);
        {
          foreach (var ele in proxyList)
          {
            info.CreateElement("e");
            {
              info.AddValue("Number", ele.number);
              info.AddValue("Proxy", ele.proxy);
            }
            info.CommitElement();
          }
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var options = info.GetValue<SpectralPreprocessingOptions>("SpectralPreprocessingOptions", null);
        var proxyList = DeserializeProxiesVersion1(info);

        return new SpectralPreprocessingOptionsDocNode(options, proxyList);
      }

      public static List<(int number, IDocumentLeafNode proxy)> DeserializeProxiesVersion1(IXmlDeserializationInfo info)
      {
        var count = info.OpenArray("Proxies");
        var proxyList = new List<(int number, IDocumentLeafNode proxy)>(count);
        {
          for (int i = 0; i < count; ++i)
          {
            info.OpenElement(); // e
            {
              var n = info.GetInt32("Number");
              var proxy = info.GetValue<IDocumentLeafNode>("Proxy", null);
              proxyList.Add((n, proxy));
            }
            info.CloseElement();
          }
        }
        info.CloseArray(count);
        return proxyList;
      }
    }

    #endregion

    protected SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptions options, List<(int number, IDocumentLeafNode proxy)> proxyList)
    {
      _spectralPreprocessingOptions = options;
      _proxyCache = new Dictionary<ISingleSpectrumPreprocessor, IDocumentLeafNode>();

      int listIndex = 0;
      int processorNumber = -1;
      foreach (var ele in options.GetProcessorElements())
      {
        ++processorNumber;

        if (listIndex < proxyList.Count && proxyList[listIndex].number == processorNumber)
        {
          var proxy = proxyList[listIndex].proxy;
          proxy.ParentObject = this;
          _proxyCache.Add(ele, proxy);
          ++listIndex;
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectralPreprocessingOptionsDocNode"/> class.
    /// </summary>
    /// <param name="options">The spectral preprocessing options to wrap.</param>
    public SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptionsBase options)
    {
      _spectralPreprocessingOptions = options;
      _proxyCache = new Dictionary<ISingleSpectrumPreprocessor, IDocumentLeafNode>();

      foreach (var element in options.GetProcessorElements())
      {
        if (element is IReferencingTable xds)
        {
          if (Current.Project.DataTableCollection.TryGetValue(xds.TableName, out var table))
          {
            var proxy = new DataTableProxy(table) { ParentObject = this };
            _proxyCache.Add(element, proxy);
          }
        }
        if (element is IReferencingXYColumns rxy && rxy.XYDataOrigin.HasValue)
        {
          var info = rxy.XYDataOrigin.Value;
          Current.Project.DataTableCollection.TryGetValue(info.TableName, out var table);
          var xcol = table?.DataColumns?.TryGetColumn(info.XColumnName);
          var ycol = table?.DataColumns?.TryGetColumn(info.YColumnName);

          if (table is not null && xcol is not null && ycol is not null)
          {
            var proxy = new DataTableXYColumnProxy(table, xcol, ycol) { ParentObject = this };
            _proxyCache.Add(element, proxy);
          }
        }
      }
    }

    public SpectralPreprocessingOptionsDocNode(SpectralPreprocessingOptionsDocNode from)
    {
      this._spectralPreprocessingOptions = from._spectralPreprocessingOptions;
      _proxyCache = new Dictionary<ISingleSpectrumPreprocessor, IDocumentLeafNode>();
      foreach (var ele in from._proxyCache)
      {
        var proxy = (IDocumentLeafNode?)(ele.Value as ICloneable)?.Clone();
        if (proxy is not null)
        {
          proxy.ParentObject = this;
          _proxyCache.Add(ele.Key, proxy);
        }
      }
    }

    public object Clone()
    {
      return new SpectralPreprocessingOptionsDocNode(this);
    }

    /// <inheritdoc/>
    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      int proxyNumber = -1;
      foreach (var element in _proxyCache)
      {
        if (element.Value is not null)
        {
          ++proxyNumber;
          yield return new DocumentNodeAndName(element.Value, $"Proxy{proxyNumber}");
        }
      }
    }



    /// <summary>
    /// Gets the wrapped spectral preprocessing options. When neccessary, the calibration is updated to reflect the content of the linked calibration table.
    /// </summary>
    /// <returns>The wrapped spectral preprocessing options</returns>
    public SpectralPreprocessingOptionsBase GetSpectralPreprocessingOptions()
    {
      var elements = _spectralPreprocessingOptions.GetProcessorElements().ToArray();

      bool hasChanged = false;
      for (int i = 0; i < elements.Length; ++i)
      {
        var oldElement = elements[i];
        var newElement = oldElement;
        _proxyCache.TryGetValue(oldElement, out var proxy);

        switch (oldElement)
        {
          case XCalibrationByDataSource xcal:
            newElement = UpdateXCalibrationByDataSourceElement(xcal, _proxyCache[oldElement]);
            ReplaceProxyKey(oldElement, newElement, proxy);
            break;
          case YCalibrationByDataSource ycal:
            newElement = UpdateYCalibrationByDataSourceElement(ycal, _proxyCache[oldElement]);
            ReplaceProxyKey(oldElement, newElement, proxy);
            break;
          case IReferencingXYColumns refXYCol:
            newElement = UpdateElementIReferencingXYColumns(refXYCol, _proxyCache[oldElement]);
            ReplaceProxyKey(oldElement, newElement, proxy);
            break;
          case IReferencingTable rt:
            throw new NotImplementedException($"Unhandled element referencing a table: {oldElement.GetType}");
        }
        if (!object.ReferenceEquals(oldElement, newElement))
        {
          elements[i] = newElement;
          hasChanged = true;
        }
      }

      if (hasChanged)
      {
        _spectralPreprocessingOptions = (SpectralPreprocessingOptionsBase)Activator.CreateInstance(_spectralPreprocessingOptions.GetType(), (IEnumerable<ISingleSpectrumPreprocessor>)elements);
      }
      return _spectralPreprocessingOptions;
    }

    /// <summary>
    /// Replaces the key in the <see cref="_proxyCache"/> dictionary, i.e. the spectral preprocessor element, whereas
    /// the value (the proxy) remains the same.
    /// </summary>
    /// <param name="oldKey">The old key to the dictionary.</param>
    /// <param name="newKey">The new key to the dictionary.</param>
    /// <param name="proxy">The value (the proxy) for the dictionary entry.</param>
    private void ReplaceProxyKey(ISingleSpectrumPreprocessor oldKey, ISingleSpectrumPreprocessor newKey, IDocumentLeafNode proxy)
    {
      _proxyCache.Remove(oldKey);
      _proxyCache[newKey] = proxy;
    }

    public static XCalibrationByDataSource UpdateXCalibrationByDataSourceElement(XCalibrationByDataSource element, IDocumentLeafNode proxy)
    {
      if (proxy is not DataTableProxy calibrationTableProxy)
        throw new ArgumentException($"Expected: DataTableProxy for calibration, but it is: {proxy.GetType()}");

      if (calibrationTableProxy is not null && calibrationTableProxy.Document is { } table)
      {
        // Update the data in this processor element
        if (element is IXCalibrationTable xct)
        {
          // Update the calibration data
          if (table.DataSource is IXCalibrationDataSource xcds && xcds.IsContainingValidXAxisCalibration(table))
          {
            var xCalibrationData = xcds.GetXAxisCalibration(table);
            element = (XCalibrationByDataSource)(xct.WithCalibrationTable(xCalibrationData.ToImmutableArray()));
          }
        }
        // Update the data origin info of this processor element
        if (element is IReferencingTable rt && rt.TableName != table.Name)
        {
          element = (XCalibrationByDataSource)rt.WithTableName(table.Name);
        }
      }
      return element;
    }

    private YCalibrationByDataSource UpdateYCalibrationByDataSourceElement(YCalibrationByDataSource element, IDocumentLeafNode proxy)
    {
      if (proxy is not DataTableProxy calibrationTableProxy)
        throw new ArgumentException($"Expected: DataTableProxy for calibration, but it is: {proxy.GetType()}");

      if (calibrationTableProxy is not null && calibrationTableProxy.Document is { } table)
      {
        // Update the calibration data
        if (element is IYCalibrationTable xct)
        {
          // Update the calibration data
          if (table.DataSource is IYCalibrationDataSource xcds && xcds.IsContainingValidYAxisCalibration(table))
          {
            var yCalibrationData = xcds.GetYAxisCalibration(table);
            element = (YCalibrationByDataSource)(xct.WithCalibrationTable(yCalibrationData.ToImmutableArray()));
          }
        }
        // Update the data origin info of this processor element
        if (element is IReferencingTable rt && rt.TableName != table.Name)
        {
          element = (YCalibrationByDataSource)rt.WithTableName(table.Name);
        }
      }
      return element;
    }

    /// <summary>
    /// Updates a <see cref="ISingleSpectrumPreprocessor"/> element that references x-y-columns, for instance <see cref="DarkSubtraction.SpectrumSubtraction"/>,
    /// with the information in the proxy that references the x-y columns.
    /// Both the x-y values in the processor element are updates, as well as the information about the names of the x and y columns.
    /// </summary>
    /// <param name="element">The <see cref="ISingleSpectrumPreprocessor"/> node that references an x-y curve.</param>
    /// <param name="proxyNode">The proxy node that stores the data of the x and y columns.</param>
    /// <returns>A new instance with data updated. In case that fails, the old instance is returned.</returns>
    /// <exception cref="Markdig.Helpers.ThrowHelper.ArgumentException(System.String)">Expected: {typeof(DataTableXYColumnProxy)}, but it is: {proxyNode.GetType()}</exception>
    public static ISingleSpectrumPreprocessor UpdateElementIReferencingXYColumns(IReferencingXYColumns element, IDocumentLeafNode proxyNode)
    {
      if (proxyNode is null)
        return (ISingleSpectrumPreprocessor)element;

      if (proxyNode is not DataTableXYColumnProxy proxy)
        throw new ArgumentException($"Expected: {typeof(DataTableXYColumnProxy)}, but it is: {proxyNode.GetType()}");

      var table = proxy.DataTable;
      var xcol = (DataColumn?)proxy.XColumn;
      var ycol = (DataColumn?)proxy.YColumn;

      if (table is not null && xcol is not null && ycol is not null)
      {
        // Update the data of the spectral preprocessor
        int len = Math.Min(xcol.Count, ycol.Count);
        if (!IsDataEqual(xcol, ycol, element.XYCurve)) // Chances are high that we do not need to update, so test first for new data
        {
          var arr = new (double x, double y)[len];
          for (int i = 0; i < len; ++i)
          {
            arr[i] = (xcol[i], ycol[i]);
          }
          element = element.WithXYCurve(arr.ToImmutableArray());
        }

        // Update the data origin info of the spectra preprocessor
        var tname = table.Name;
        var group = table.DataColumns.GetColumnGroup(ycol);
        var xname = table.DataColumns.GetColumnName(xcol);
        var yname = table.DataColumns.GetColumnName(ycol);
        var info = element.XYDataOrigin;
        if (!info.HasValue ||
             info.Value.TableName != tname ||
             info.Value.GroupNumber != group ||
             info.Value.XColumnName != xname ||
             info.Value.YColumnName != yname)
        {
          element = element.WithXYDataOrigin((tname, group, xname, yname));
        }
      }
      return (ISingleSpectrumPreprocessor)element;
    }

    private static bool IsDataEqual(DataColumn xcol, DataColumn ycol, ImmutableArray<(double x, double y)> arr)
    {
      var len2 = Math.Min(xcol.Count, ycol.Count);
      if (len2 != arr.Length)
        return false;
      for (int i = 0; i < len2; ++i)
      {
        var e = arr[i];
        if (xcol[i] != e.x || ycol[i] != e.y)
          return false;
      }
      return true;
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
        foreach (var childNode in GetDocumentNodeChildrenWithName())
        {
          if (childNode.DocumentNode is IProxy proxy)
            Report(proxy, this, childNode.Name);
          else if (childNode.DocumentNode is IHasDocumentReferences hasReferences)
            hasReferences.VisitDocumentReferences(Report);
          else
            throw new NotImplementedException($"Don't know that to do with type {childNode.DocumentNode}");
        }
        suspendToken.Resume();
      }
    }
  }
}
