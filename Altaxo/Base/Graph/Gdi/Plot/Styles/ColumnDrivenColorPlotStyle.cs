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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Data;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Main;
  using Geometry;
  using Graph.Plot.Groups;
  using Graph.Scales;
  using Graph3D.GraphicsContext;
  using Plot.Data;
  using Plot.Groups;

  /// <summary>
  /// This style provides a variable symbol size dependent on the data of a user choosen column. The data of that column at the index of the data point determine the symbol size.
  /// This plot style is non-visual, i.e. it has no visual equivalent,
  /// since it is only intended to provide the symbol size to other plot styles.
  /// </summary>
  public class ColumnDrivenColorPlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG2DPlotStyle, Graph3D.Plot.Styles.IG3DPlotStyle
  {
    #region Members

    /// <summary>
    /// Data which are converted to scatter size.
    /// </summary>
    private IReadableColumnProxy _dataColumnProxy;

    /// <summary>True if the data in the data column changed, but the scale was not updated up to now.</summary>
    [NonSerialized]
    private bool _doesScaleNeedsDataUpdate;

    /// <summary>
    /// Converts the numerical values of the data colum into logical values.
    /// </summary>
    private NumericalScale _scale;

    /// <summary>
    /// Converts the logical value (from the scale) to a color value.
    /// </summary>
    private IColorProvider _colorProvider;

    /// <summary>If true, the color is applied as a fill color for symbols, bar graphs etc.</summary>
    private bool _appliesToFill = true;

    /// <summary>If true, the color is applied as a stroke color for framing symbols, bar graphs etc.</summary>
    private bool _appliesToStroke;

    /// <summary>If true, the color is used to color the background, for instance of labels.</summary>
    private bool _appliesToBackground;

    #endregion Members

    #region Serialization

    /// <summary>
    /// 2015-09-29 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnDrivenColorPlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ColumnDrivenColorPlotStyle)obj;

        info.AddValue("DataColumn", s._dataColumnProxy);
        info.AddValue("Scale", s._scale);

        info.AddValue("ColorProvider", s._colorProvider);

        info.AddValue("AppliesToFill", s._appliesToFill);
        info.AddValue("AppliesToStroke", s._appliesToStroke);
        info.AddValue("AppliesToBackground", s._appliesToBackground);
      }

      protected virtual ColumnDrivenColorPlotStyle SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ColumnDrivenColorPlotStyle?)o ?? new ColumnDrivenColorPlotStyle();

        s._dataColumnProxy = (IReadableColumnProxy)info.GetValue("DataColumn", s);
        if (s._dataColumnProxy is not null)
          s._dataColumnProxy.ParentObject = s;

        s._scale = (NumericalScale)info.GetValue("Scale", s);
        if (s._scale is not null)
          s._scale.ParentObject = s;

        s._colorProvider = (IColorProvider)info.GetValue("ColorProvider", s);

        s._appliesToFill = info.GetBoolean("AppliesToFill");
        s._appliesToStroke = info.GetBoolean("AppliesToStroke");
        s._appliesToBackground = info.GetBoolean("AppliesToBackground");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return SDeserialize(o, info, parent);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates a new instance with default values.
    /// </summary>
    public ColumnDrivenColorPlotStyle()
    {
      InternalSetScale(new LinearScale());
      InternalSetDataColumnProxy(NumericColumnProxyBase.FromColumn(new Altaxo.Data.EquallySpacedColumn(0, 0.25)));
      _colorProvider = new ColorProvider.VisibleLightSpectrum();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Other instance to copy the data from.</param>
    /// <param name="copyWithDataReferences">If true, data references are copyied from the template style to this style. If false, the data references of this style are left as they are.</param>
    public ColumnDrivenColorPlotStyle(ColumnDrivenColorPlotStyle from, bool copyWithDataReferences)
    {
      InternalSetDataColumnProxy(NumericColumnProxyBase.FromColumn(new Altaxo.Data.EquallySpacedColumn(0, 0.25)));
      CopyFrom(from, copyWithDataReferences);
    }

    [MemberNotNull(nameof(_scale), nameof(_colorProvider))]
    protected void CopyFrom(ColumnDrivenColorPlotStyle from, bool copyWithDataReferences)
    {
      _appliesToFill = from._appliesToFill;
      _appliesToStroke = from._appliesToStroke;
      _appliesToBackground = from._appliesToBackground;
      InternalSetScale((NumericalScale)from._scale.Clone());
      _colorProvider = from._colorProvider;

      if (copyWithDataReferences)
      {
        InternalSetDataColumnProxy((IReadableColumnProxy)from._dataColumnProxy.Clone());
      }
    }

    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is ColumnDrivenColorPlotStyle from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, copyWithDataReferences);
          suspendToken.ResumeSilently();
        }
        EhSelfChanged(EventArgs.Empty);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Copies the member variables from another instance.
    /// </summary>
    /// <param name="obj">Another instance to copy the data from.</param>
    /// <returns>True if data was copied, otherwise false.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      return CopyFrom(obj, true);
    }

    /// <inheritdoc/>
    public object Clone(bool copyWithDataReferences)
    {
      return new ColumnDrivenColorPlotStyle(this, copyWithDataReferences);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new ColumnDrivenColorPlotStyle(this, true);
    }

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_dataColumnProxy is not null)
        yield return new DocumentNodeAndName(_dataColumnProxy, "Data");
      if (_scale is not null)
        yield return new DocumentNodeAndName(_scale, "Scale");
    }

    #region DataColumnProxy handling

    /// <summary>
    /// Sets the data column proxy and creates the necessary event links.
    /// </summary>
    /// <param name="proxy"></param>
    [MemberNotNull(nameof(_dataColumnProxy))]
    protected void InternalSetDataColumnProxy(IReadableColumnProxy proxy)
    {
      ChildSetMember(ref _dataColumnProxy, proxy);
    }

    #region Changed event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(_dataColumnProxy, sender))
      {
        if (e is Main.InstanceChangedEventArgs) // Data column object has changed
        {
          _doesScaleNeedsDataUpdate = true; // Instance that the proxy holds has changed
        }
        else
        {
          _doesScaleNeedsDataUpdate = true; // data in data column have changed
        }
      }
      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed event handling

    /// <summary>
    /// Updates the scale if the data of the data column have changed.
    /// </summary>
    private void InternalUpdateScaleWithNewData()
    {
      // in order to set the bounds of the scale, the data column must
      // - be set (not null)
      // - have a defined count.

      if (_dataColumnProxy.Document() is { } dataColumn && dataColumn.Count.HasValue)
      {
        int len = dataColumn.Count.Value;

        var bounds = _scale.DataBounds;

        using (var suspendToken = bounds.SuspendGetToken())
        {
          for (int i = 0; i < len; i++)
            bounds.Add(dataColumn, i);

          suspendToken.Resume();
        }
        _doesScaleNeedsDataUpdate = false;
      }
    }

    /// <summary>
    /// Gets/sets the data column that provides the data that is used to calculate the symbol size.
    /// </summary>
    public Altaxo.Data.IReadableColumn? DataColumn
    {
      get
      {
        return _dataColumnProxy.Document();
      }
      set
      {
        if (ChildSetMember(ref _dataColumnProxy, ReadableColumnProxyBase.FromColumn(value)))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets the name of the data column, if it is a data column. Otherwise, null is returned.
    /// </summary>
    /// <value>
    /// The name of the label column if it is a data column. Otherwise, null.
    /// </value>
    public string? DataColumnName
    {
      get
      {
        return _dataColumnProxy?.DocumentPath()?.LastPartOrDefault;
      }
    }

    #endregion DataColumnProxy handling

    #region Scale handling

    /// <summary>
    /// Sets the scale and create the necessary event links.
    /// </summary>
    /// <param name="scale"></param>
    [MemberNotNull(nameof(_scale))]
    protected void InternalSetScale(NumericalScale scale)
    {
      if (ChildSetMember(ref _scale, scale))
      {
        _doesScaleNeedsDataUpdate = true;
      }
    }

    #endregion Scale handling

    #region Properties

    /// <summary>
    /// Gets/sets the scale.
    /// </summary>
    public NumericalScale Scale
    {
      get
      {
        return _scale;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException("Scale");

        InternalSetScale(value);
        EhSelfChanged(EventArgs.Empty);
      }
    }

    public IColorProvider ColorProvider
    {
      get { return _colorProvider; }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(value, _colorProvider))
        {
          _colorProvider = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    #endregion Properties

    /// <summary>
    /// Gets the color for the index idx.
    /// </summary>
    /// <param name="idx">Index into the row of the data column.</param>
    /// <returns>The calculated color for the provided index.</returns>
    private Color GetColor(int idx)
    {
      var dataColumn = DataColumn;
      if (dataColumn is not null)
      {
        return _colorProvider.GetColor(_scale.PhysicalToNormal(dataColumn[idx]));
      }
      else
      {
        return _colorProvider.GetColor(double.NaN);
      }
    }

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      // this is only for internal use inside one plot item
    }

    public void CollectExternalGroupStyles(Graph3D.Plot.Groups.PlotGroupStyleCollection externalGroups)
    {
      // this is only for internal use inside one plot item
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      VariableColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void CollectLocalGroupStyles(Graph3D.Plot.Groups.PlotGroupStyleCollection externalGroups, Graph3D.Plot.Groups.PlotGroupStyleCollection localGroups)
    {
      VariableColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed2DPlotData pdata)
    {
      VariableColorGroupStyle.PrepareStyle(externalGroups, localGroups, GetColor);
    }

    public void PrepareGroupStyles(Graph3D.Plot.Groups.PlotGroupStyleCollection externalGroups, Graph3D.Plot.Groups.PlotGroupStyleCollection localGroups, Graph3D.IPlotArea layer, Graph3D.Plot.Data.Processed3DPlotData pdata)
    {
      VariableColorGroupStyle.PrepareStyle(externalGroups, localGroups, GetColor);
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      // there is nothing to apply here, because it is only a provider
    }

    public void ApplyGroupStyles(Graph3D.Plot.Groups.PlotGroupStyleCollection externalGroups, Graph3D.Plot.Groups.PlotGroupStyleCollection localGroups)
    {
      // there is nothing to apply here, because it is only a provider
    }

    public void Paint(Graphics g, IPlotArea layer, Processed2DPlotData pdata, Processed2DPlotData? prevItemData, Processed2DPlotData? nextItemData)
    {
      // this is not a visible style, thus doing nothing
    }

    public void Paint(IGraphicsContext3D g, Graph3D.IPlotArea layer, Graph3D.Plot.Data.Processed3DPlotData pdata, Graph3D.Plot.Data.Processed3DPlotData? prevItemData, Graph3D.Plot.Data.Processed3DPlotData? nextItemData)
    {
      // this is not a visible style, thus doing nothing
    }

    public RectangleF PaintSymbol(Graphics g, RectangleF bounds)
    {
      // this is not a visible style, thus doing nothing
      return RectangleF.Empty;
    }

    public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
    {
      // this is not a visible style, thus doing nothing
      return RectangleD3D.Empty;
    }

    /// <summary>
    /// Prepares the scale of this plot style.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(IPlotArea layer)
    {
      var dataColumn = DataColumn;
      if (dataColumn is not null)
      {
        if (_doesScaleNeedsDataUpdate)
          InternalUpdateScaleWithNewData();
      }
    }

    /// <summary>
    /// Prepares the scale of this plot style.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(Graph3D.IPlotArea layer)
    {
      var dataColumn = DataColumn;
      if (dataColumn is not null)
      {
        if (_doesScaleNeedsDataUpdate)
          InternalUpdateScaleWithNewData();
      }
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      Report(_dataColumnProxy, this, "DataColumn");
    }

    /// <inheritdoc/>
    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn? Column, // the column as it was at the time of this call
      string? ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn?> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield return (nameof(DataColumn), DataColumn, _dataColumnProxy?.DocumentPath()?.LastPartOrDefault, (col) => DataColumn = col as IReadableColumn);
    }
  }
}
