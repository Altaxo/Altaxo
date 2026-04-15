#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Drawing;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;

namespace Altaxo.Graph.Graph3D.Plot
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Calc;
  using Graph;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;
  using GraphicsContext;
  using Groups;
  using Styles;

  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  public class DataMeshPlotItem
    :
    PlotItem,
    IXBoundsHolder,
    IYBoundsHolder,
    IZBoundsHolder
  {
    /// <summary>
    /// Stores the plot data.
    /// </summary>
    protected XYZMeshedColumnPlotData _plotData;
    /// <summary>
    /// Stores the plot style.
    /// </summary>
    protected DataMeshPlotStyle _plotStyle;

    #region Serialization

    /// <summary>
    /// 2015-11-14 initial version.
    /// </summary>
    /// <summary>
    /// Serializes <see cref="DataMeshPlotItem"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataMeshPlotItem), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataMeshPlotItem)o;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyle);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pa = (XYZMeshedColumnPlotData)info.GetValue("Data", null);
        var ps = (DataMeshPlotStyle)info.GetValue("Style", null);

        if (o is null)
        {
          return new DataMeshPlotItem(pa, ps);
        }
        else
        {
          var s = (DataMeshPlotItem)o;
          s.Data = pa;
          s.Style = ps;
          return s;
        }
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_plotData is not null)
        yield return new Main.DocumentNodeAndName(_plotData, () => _plotData = null!, "Data");
      if (_plotStyle is not null)
        yield return new Main.DocumentNodeAndName(_plotStyle, () => _plotStyle = null!, "Style");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataMeshPlotItem"/> class.
    /// </summary>
    /// <param name="pa">The plot data.</param>
    /// <param name="ps">The plot style.</param>
    public DataMeshPlotItem(XYZMeshedColumnPlotData pa, DataMeshPlotStyle ps)
    {
      ChildSetMember(ref _plotStyle, ps);
      ChildSetMember(ref _plotData, pa);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataMeshPlotItem"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public DataMeshPlotItem(DataMeshPlotItem from)
    {
      CopyFrom(from, false);
    }

    /// <summary>
    /// Copies values from another <see cref="DataMeshPlotItem"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    /// <param name="withBaseMembers">If set to <c>true</c>, base-class members are copied as well.</param>
    [MemberNotNull(nameof(_plotData), nameof(_plotStyle))]
    protected void CopyFrom(DataMeshPlotItem from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      ChildCopyToMember(ref _plotData, from._plotData);
      ChildCopyToMember(ref _plotStyle, from._plotStyle);
    }

    /// <inheritdoc/>
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is DataMeshPlotItem from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    /// <inheritdoc/>
    public override object Clone()
    {
      return new DataMeshPlotItem(this);
    }

    /// <summary>
    /// Gets or sets the plot data object.
    /// </summary>
    public object Data
    {
      get { return _plotData; }
      set
      {
        if (value is null)
          throw new System.ArgumentNullException();
        else if (!(value is XYZMeshedColumnPlotData))
          throw new System.ArgumentException("The provided data object is not of the type " + _plotData.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
        else
        {
          if (ChildSetMember(ref _plotData, (XYZMeshedColumnPlotData)value))
          {
            EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
          }
        }
      }
    }

    /// <inheritdoc/>
    public override Main.IDocumentLeafNode StyleObject
    {
      get { return _plotStyle; }
      set { Style = (DataMeshPlotStyle)value; }
    }

    /// <inheritdoc/>
    public override Main.IDocumentLeafNode DataObject
    {
      get { return _plotData; }
    }

    /// <summary>
    /// Gets or sets the plot style.
    /// </summary>
    public DataMeshPlotStyle Style
    {
      get { return _plotStyle; }
      set
      {
        if (value is null)
          throw new System.ArgumentNullException();
        if (ChildSetMember(ref _plotStyle, value))
        {
          EhSelfChanged(PlotItemStyleChangedEventArgs.Empty);
        }
      }
    }

    /// <inheritdoc/>
    public override string GetName(int level)
    {
      return _plotData.ToString();
    }

    /// <summary>
    /// Returns the string representation of this plot item.
    /// </summary>
    /// <returns>The plot-item name.</returns>
    public override string ToString()
    {
      return GetName(int.MaxValue);
    }

    /// <inheritdoc/>
    public override void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext context, IPlotArea layer, IGPlotItem? previousPlotItem, IGPlotItem? nextPlotItem)
    {
      if (_plotStyle is not null)
      {
        _plotStyle.Paint(g, layer, _plotData);
      }
    }

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    public override void PrepareScales(IPlotArea layer)
    {
      if (_plotData is not null)
      {
        _plotData.CalculateCachedData(layer.XAxis.DataBoundsObject, layer.YAxis.DataBoundsObject, layer.ZAxis.DataBoundsObject);

        // in case our plot style uses its own scale for coloring the mesh, we do prepare the scale used by the style
        _plotStyle?.PrepareScales(layer, _plotData);
      }
    }

    /// <summary>
    /// Handles change notifications raised by child objects.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnChanged(EventArgs e)
    {
      if (e is PlotItemDataChangedEventArgs)
      {
        // first inform our AbstractXYPlotStyle of the change, so it can invalidate its cached data
        if (_plotStyle is not null)
          _plotStyle.EhDataChanged(this);
      }

      base.OnChanged(e);
    }

    #region IXBoundsHolder Members

    /// <summary>
    /// Initializes the x bounds from a template boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      _plotData.SetXBoundsFromTemplate(val);
    }

    /// <inheritdoc/>
    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeXBoundsInto(pb);
    }

    #endregion IXBoundsHolder Members

    #region IYBoundsHolder Members

    /// <summary>
    /// Initializes the y bounds from a template boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      _plotData.SetYBoundsFromTemplate(val);
    }

    /// <summary>
    /// Merges y bounds from this item into the specified boundary accumulator.
    /// </summary>
    /// <param name="pb">The boundary accumulator to update.</param>
    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeYBoundsInto(pb);
    }

    #endregion IYBoundsHolder Members

    #region IZBoundHolder

    /// <summary>
    /// Initializes z bounds from a template boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    public void SetZBoundsFromTemplate(IPhysicalBoundaries val)
    {
      _plotData.SetVBoundsFromTemplate(val);
    }

    /// <summary>
    /// Merges z bounds from this item into the specified boundary accumulator.
    /// </summary>
    /// <param name="pb">The boundary accumulator to update.</param>
    public void MergeZBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeVBoundsInto(pb);
    }

    #endregion IZBoundHolder

    /// <summary>
    /// Collects group styles required by this plot item.
    /// </summary>
    /// <param name="styles">The collection to receive the required styles.</param>
    public override void CollectStyles(PlotGroupStyleCollection styles)
    {
    }

    /// <summary>
    /// Prepares group styles for this plot item.
    /// </summary>
    /// <param name="styles">The external group styles.</param>
    /// <param name="layer">The plot layer.</param>
    public override void PrepareGroupStyles(PlotGroupStyleCollection styles, IPlotArea layer)
    {
    }

    /// <summary>
    /// Applies prepared group styles to this plot item.
    /// </summary>
    /// <param name="styles">The external group styles.</param>
    public override void ApplyGroupStyles(PlotGroupStyleCollection styles)
    {
    }

    /// <summary>
    /// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
    /// </summary>
    /// <param name="template">The template item to copy the plot styles from.</param>
    /// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
    public override void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
    {
      if (!(template is DataMeshPlotItem))
        return;
      var from = (DataMeshPlotItem)template;
      _plotStyle.CopyFrom(from._plotStyle);
    }

    /// <summary>
    /// Gets a pixelwise image of the data. Horizontal or vertical axes are not taken into accout.
    /// The horizontal dimension of the image is associated with the columns of the data table. The
    /// vertical dimension of the image is associated with the rows of the data table.
    /// </summary>
    /// <returns>Bitmap with the plot image.</returns>
    public Bitmap? GetPixelwiseImage()
    {
      Bitmap? result = null;
      GetPixelwiseImage(ref result);
      return result;
    }

    /// <summary>
    /// Gets a pixelwise image of the data. Horizontal or vertical axes are not taken into accout.
    /// The horizontal dimension of the image is associated with the columns of the data table. The
    /// vertical dimension of the image is associated with the rows of the data table.
    /// </summary>
    /// <param name="image">Bitmap to fill with the plot image. If null, a new image is created.</param>
    /// <exception cref="ArgumentException">An exception will be thrown if the provided image is smaller than the required dimensions.</exception>
    public void GetPixelwiseImage(ref Bitmap? image)
    {
      _plotData.DataTableMatrix.GetWrappers(
        x => x, // transformation function for row header values
        Precision.IsFinite,       // selection functiton for row header values
        x => x, // transformation function for column header values
        Precision.IsFinite,       // selection functiton for column header values
        out var matrix,
        out var rowVec,
        out var colVec
        );

      //_plotStyle.GetPixelwiseImage(matrix, ref image);
    }

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
    public override void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      _plotData.VisitDocumentReferences(Report);
    }
  }
}
