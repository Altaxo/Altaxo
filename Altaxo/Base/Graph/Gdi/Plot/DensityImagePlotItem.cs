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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Plot
{
  using System.Diagnostics.CodeAnalysis;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;
  using Groups;
  using Styles;

  /// <summary>
  /// Association of data and style specialized for x-y-plots of column data.
  /// </summary>
  public class DensityImagePlotItem
    :
    PlotItem,
    IXBoundsHolder,
    IYBoundsHolder
  {
    protected XYZMeshedColumnPlotData _plotData;
    protected DensityImagePlotStyle _plotStyle;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.DensityImagePlotItem", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotItem), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DensityImagePlotItem)obj;
        info.AddValue("Data", s._plotData);
        info.AddValue("Style", s._plotStyle);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var pa = (XYZMeshedColumnPlotData)info.GetValue("Data", null);
        var ps = (DensityImagePlotStyle)info.GetValue("Style", null);

        if (o is null)
        {
          return new DensityImagePlotItem(pa, ps);
        }
        else
        {
          var s = (DensityImagePlotItem)o;
          s.ChildSetMember(ref s._plotData, pa);
          s.Style = ps;
          return s;
        }
      }
    }

    #endregion Serialization

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _plotData)
        yield return new Main.DocumentNodeAndName(_plotData, () => _plotData = null!, "Data");
      if (null != _plotStyle)
        yield return new Main.DocumentNodeAndName(_plotStyle, () => _plotStyle = null!, "Style");
    }

    public DensityImagePlotItem(XYZMeshedColumnPlotData pa, DensityImagePlotStyle ps) 
    {
      ChildSetMember(ref _plotStyle, ps);
      ChildSetMember(ref _plotData, pa);
    }

    public DensityImagePlotItem(DensityImagePlotItem from)
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_plotData), nameof(_plotStyle))]
    protected void CopyFrom(DensityImagePlotItem from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      ChildCopyToMember(ref _plotData, from._plotData);
      ChildCopyToMember(ref _plotStyle, from._plotStyle);
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      if (obj is DensityImagePlotItem from)
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

    public override object Clone()
    {
      return new DensityImagePlotItem(this);
    }

    public object Data
    {
      get { return _plotData; }
    }

    public override Main.IDocumentLeafNode StyleObject
    {
      get { return _plotStyle; }
      set { Style = (DensityImagePlotStyle)value; }
    }

    public override Main.IDocumentLeafNode DataObject
    {
      get { return _plotData; }
    }

    public DensityImagePlotStyle Style
    {
      get { return _plotStyle; }
      [MemberNotNull(nameof(_plotStyle))]
      set
      {
        if (null == value)
          throw new System.ArgumentNullException();
        if (ChildSetMember(ref _plotStyle, value))
        {
          EhSelfChanged(PlotItemStyleChangedEventArgs.Empty);
        }
      }
    }

    public override string GetName(int level)
    {
      return _plotData.ToString();
    }

    public override string GetName(string style)
    {
      return GetName(0);
    }

    public override string ToString()
    {
      return GetName(int.MaxValue);
    }

    public override void Paint(Graphics g, IPaintContext context, IPlotArea layer, IGPlotItem? previousPlotItem, IGPlotItem? nextPlotItem)
    {
      if (null != _plotStyle)
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
      if (null != _plotData)
      {
        _plotData.CalculateCachedData(layer.XAxis.DataBoundsObject, layer.YAxis.DataBoundsObject);

        if (null != _plotStyle)
          _plotStyle.PrepareScales(layer, _plotData);
      }
    }

    protected override void OnChanged(EventArgs e)
    {
      if (e is PlotItemDataChangedEventArgs)
      {
        // first inform our AbstractXYPlotStyle of the change, so it can invalidate its cached data
        if (null != _plotStyle)
          _plotStyle.EhDataChanged(this);
      }

      base.OnChanged(e);
    }

    #region IXBoundsHolder Members

    public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
    {
      _plotData.SetXBoundsFromTemplate(val);
    }

    public void MergeXBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeXBoundsInto(pb);
    }

    #endregion IXBoundsHolder Members

    #region IYBoundsHolder Members

    public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
    {
      _plotData.SetYBoundsFromTemplate(val);
    }

    public void MergeYBoundsInto(IPhysicalBoundaries pb)
    {
      _plotData.MergeYBoundsInto(pb);
    }

    #endregion IYBoundsHolder Members

    public override void CollectStyles(PlotGroupStyleCollection styles)
    {
    }

    public override void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, IPlotArea layer)
    {
    }

    public override void ApplyGroupStyles(PlotGroupStyleCollection externalGroups)
    {
    }

    /// <summary>
    /// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
    /// </summary>
    /// <param name="template">The template item to copy the plot styles from.</param>
    /// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
    public override void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
    {
      if (!(template is DensityImagePlotItem))
        return;
      var from = (DensityImagePlotItem)template;
      _plotStyle.CopyFrom(from._plotStyle);
    }

    /// <summary>
    /// Gets a pixelwise image of the data. Horizontal or vertical axes are not taken into accout.
    /// The horizontal dimension of the image is associated with the columns of the data table. The
    /// vertical dimension of the image is associated with the rows of the data table.
    /// </summary>
    /// <returns>Bitmap with the plot image.</returns>
    public Bitmap GetPixelwiseImage()
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
    public void GetPixelwiseImage([AllowNull] ref Bitmap image)
    {
      _plotData.DataTableMatrix.GetWrappers(
        x => x, // transformation function for row header values
        Altaxo.Calc.RMath.IsFinite,       // selection functiton for row header values
        x => x, // transformation function for column header values
        Altaxo.Calc.RMath.IsFinite,       // selection functiton for column header values
        out var matrix,
        out var rowVec,
        out var colVec
        );

      _plotStyle.GetPixelwiseImage(matrix, ref image);
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
