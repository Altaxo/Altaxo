#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;



namespace Altaxo.Graph.Gdi.Plot.Groups
{
  using Plot.Data;

  public class WaterfallTransform : ICoordinateTransformingGroupStyle
  {
    /// <summary>User defined scale. The multiplication with _xinc results in the xinc that is used for the waterfall.</summary>
    double _scaleXInc=1;
    /// <summary>User defined scale. The multiplication with _yinc results in the yinc that is used for the waterfall.</summary>
    double _scaleYInc = 1;
    /// <summary>If true, the actual plot item is clipped by the previous plot items.</summary>
    bool _useClipping;

    double _xinc=0;
    double _yinc=0;

    public WaterfallTransform()
    {
    }

    public WaterfallTransform(WaterfallTransform from)
    {
      this._scaleXInc = from._scaleXInc;
      this._scaleYInc = from._scaleYInc;

      this._xinc = from._xinc;
      this._yinc = from._yinc;
    }

    #region ICoordinateTransformingGroupStyle Members



    public void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      if(!(pb is NumericalBoundaries))
      {
        CoordinateTransformingStyleBase.MergeXBoundsInto(pb,coll);
        return;
      }

      NumericalBoundaries xbounds = (NumericalBoundaries)pb.Clone();
      xbounds.Reset();

      int nItems = 0;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          IXBoundsHolder xbpi = (IXBoundsHolder)pi;
          xbpi.MergeXBoundsInto(xbounds);
        }
        if(pi is G2DPlotItem)
          nItems++;

      }

     
      if (nItems == 0)
        _xinc = 0;
      else
        _xinc = (xbounds.UpperBound - xbounds.LowerBound) / nItems;

      int idx = 0;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          IXBoundsHolder xbpi = (IXBoundsHolder)pi;
          xbounds.Reset();
          xbpi.MergeXBoundsInto(xbounds);
          xbounds.Shift(_xinc * idx);
          pb.Add(xbounds);
        }
        if (pi is G2DPlotItem)
          idx++;
      }

    }


    public void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      if (!(pb is NumericalBoundaries))
      {
        CoordinateTransformingStyleBase.MergeYBoundsInto(pb, coll);
        return;
      }

      NumericalBoundaries ybounds = (NumericalBoundaries)pb.Clone();
      ybounds.Reset();

      int nItems = 0;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IYBoundsHolder)
        {
          IYBoundsHolder ybpi = (IYBoundsHolder)pi;
          ybpi.MergeYBoundsInto(ybounds);
        }
        if (pi is G2DPlotItem)
          nItems++;

      }


      if (nItems == 0)
        _yinc = 0;
      else
        _yinc = (ybounds.UpperBound - ybounds.LowerBound) / nItems;

      int idx = 0;
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IYBoundsHolder)
        {
          IYBoundsHolder ybpi = (IYBoundsHolder)pi;
          ybounds.Reset();
          ybpi.MergeYBoundsInto(ybounds);
          ybounds.Shift(_yinc * idx);
          pb.Add(ybounds);
        }
        if (pi is G2DPlotItem)
          idx++;
      }

    }


    public void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
    {
      System.Drawing.Region[] clippingColl = new System.Drawing.Region[coll.Count];
      Processed2DPlotData[] plotDataColl = new Processed2DPlotData[coll.Count];
      double[] xincColl = new double[coll.Count];
      double[] yincColl = new double[coll.Count];

      int idx = -1;
      Processed2DPlotData previousPlotData = null;
      for (int i = 0; i < coll.Count; i++)
      {
        if (coll[i] is G2DPlotItem)
        {
          idx++;
          double currxinc = xincColl[i] = idx * _xinc * _scaleXInc;
          double curryinc = yincColl[i] = idx * _yinc * _scaleYInc;

          G2DPlotItem gpi = coll[i] as G2DPlotItem;
          Processed2DPlotData plotdata = plotDataColl[i] = gpi.GetRangesAndPoints(layer);
          plotdata.PreviousItemData = previousPlotData;
          previousPlotData = plotdata;

          int j = -1;
          foreach (int rowIndex in plotdata.RangeList.OriginalRowIndices())
          {
            j++;

            AltaxoVariant xx = plotdata.GetXPhysical(rowIndex) + currxinc;
            AltaxoVariant yy = plotdata.GetYPhysical(rowIndex) + curryinc;

            Logical3D rel = new Logical3D(layer.XAxis.PhysicalVariantToNormal(xx), layer.YAxis.PhysicalVariantToNormal(yy));
            double xabs, yabs;
            layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
            plotdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);
          }

          if (_useClipping)
          {
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

          }
        }
      }
      for (int i = coll.Count - 1; i >= 0; i--)
      {
        if (null == plotDataColl[i])
        {
          coll[i].Paint(g, layer);
        }
        else
        {
          TransformedLayerWrapper layerwrapper = new TransformedLayerWrapper(layer, xincColl[i], yincColl[i]);
          ((G2DPlotItem)coll[i]).Paint(g, layerwrapper, plotDataColl[i]);
        }
      }
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      return new WaterfallTransform(this);
    }

    #endregion

    #region ICoordinateTransformingGroupStyle Members


   
    #endregion


    #region Inner Classes - TransformedLayerWrapper

    class TransformedLayerWrapper : IPlotArea
    {
      IPlotArea _layer;
      TransformedScale _xScale;
      TransformedScale _yScale;

      public TransformedLayerWrapper(IPlotArea layer, double xinc, double yinc)
      {
        _layer = layer;
        _xScale = new TransformedScale(layer.XAxis, xinc);
        _yScale = new TransformedScale(layer.YAxis, yinc);
      }

      #region IPlotArea Members

      public bool Is3D
      {
        get { return _layer.Is3D; }
      }

      public Altaxo.Graph.Scales.Scale XAxis
      {
        get { return _xScale; }
      }

      public Altaxo.Graph.Scales.Scale YAxis
      {
        get { return _yScale; }
      }

      public G2DCoordinateSystem CoordinateSystem
      {
        get { return _layer.CoordinateSystem; }
      }

      public SizeF Size
      {
        get { return _layer.Size; }
      }

      public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
      {
        Logical3D shifted = new Logical3D(
          _xScale.PhysicalVariantToNormal(acc.GetXPhysical(idx)),
          _yScale.PhysicalVariantToNormal(acc.GetYPhysical(idx)));
        return shifted;
      }

      public IEnumerable<CSLineID> AxisStyleIDs
      {
        get { return _layer.AxisStyleIDs; }
      }


      public void UpdateCSPlaneID(CSPlaneID id)
      {
        if (id.UsePhysicalValue)
        {
          int scaleidx = id.PerpendicularAxisNumber;
          switch (scaleidx)
          {
            case 0:
              id.LogicalValue = _xScale.PhysicalVariantToNormal(id.PhysicalValue);
              break;
            case 1:
              id.LogicalValue = _yScale.PhysicalVariantToNormal(id.PhysicalValue);
              break;
            default:
              _layer.UpdateCSPlaneID(id);
              break;
          }
        }
      }

    

    

      #endregion

      #region Helper Scale Wrapper
      class TransformedScale : Altaxo.Graph.Scales.Scale
      {
        Altaxo.Graph.Scales.Scale _originalScale;
        double _offset;

        public TransformedScale(Altaxo.Graph.Scales.Scale scale, double offset)
        {
          _originalScale = scale;
          _offset = offset;
        }


        public override object Clone()
        {
          throw new Exception("The method or operation is not implemented.");
        }

        public override double PhysicalVariantToNormal(AltaxoVariant x)
        {
          return _originalScale.PhysicalVariantToNormal(x + _offset);
        }

        public override AltaxoVariant NormalToPhysicalVariant(double x)
        {
          AltaxoVariant result = _originalScale.NormalToPhysicalVariant(x);
          return result - _offset;
        }

        public override AltaxoVariant[] GetMajorTicksAsVariant()
        {
          throw new Exception("The method or operation is not implemented.");
        }

        public override double[] GetMajorTicksNormal()
        {
          throw new Exception("The method or operation is not implemented.");
        }

        public override AltaxoVariant[] GetMinorTicksAsVariant()
        {
          throw new Exception("The method or operation is not implemented.");
        }

        public override double[] GetMinorTicksNormal()
        {
          throw new Exception("The method or operation is not implemented.");
        }

        public override object RescalingObject
        {
          get { throw new Exception("The method or operation is not implemented."); }
        }

        public override IPhysicalBoundaries DataBoundsObject
        {
          get { throw new Exception("The method or operation is not implemented."); }
        }

        public override AltaxoVariant OrgAsVariant
        {
          get
          {
            throw new Exception("The method or operation is not implemented.");
          }
          set
          {
            throw new Exception("The method or operation is not implemented.");
          }
        }

        public override AltaxoVariant EndAsVariant
        {
          get
          {
            throw new Exception("The method or operation is not implemented.");
          }
          set
          {
            throw new Exception("The method or operation is not implemented.");
          }
        }

        public override void ProcessDataBounds()
        {
          throw new Exception("The method or operation is not implemented.");
        }

        public override void ProcessDataBounds(AltaxoVariant org, bool orgfixed, AltaxoVariant end, bool endfixed)
        {
          throw new Exception("The method or operation is not implemented.");
        }
      }
      #endregion
    }


    #endregion
  }
}
