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

namespace Altaxo.Graph.Gdi.Plot
{
  using Graph.Plot.Groups;
  using Groups;
  using Styles;
  using Data;
  using Graph.Plot.Data;

  [Serializable]
  public abstract class G2DPlotItem : PlotItem
  {

    protected G2DPlotStyleCollection _plotStyles;

    [NonSerialized]
    Processed2DPlotData _cachedPlotDataUsedForPainting;

    [NonSerialized]
    PlotGroupStyleCollection _localGroups;

    public override object StyleObject
    {
      get { return _plotStyles; }
      set { this.Style = (G2DPlotStyleCollection)value; }
    }


    public G2DPlotStyleCollection Style
    {
      get
      {
        return _plotStyles;
      }
      set
      {
        if (null == value)
          throw new System.ArgumentNullException();
        else
        {
          if (!object.ReferenceEquals(_plotStyles, value))
          {
            // delete event wiring to old AbstractXYPlotStyle
            if (null != _plotStyles)
            {
              ((Main.IChangedEventSource)_plotStyles).Changed -= new EventHandler(OnStyleChangedEventHandler);
              _plotStyles.ParentObject = null;
            }

            _plotStyles = (G2DPlotStyleCollection)value;

            // create event wire to new Plotstyle
            if (null != _plotStyles)
            {
              ((Main.IChangedEventSource)_plotStyles).Changed += new EventHandler(OnStyleChangedEventHandler);
              _plotStyles.ParentObject = this;
            }

            // indicate the style has changed
            OnStyleChanged();
          }
        }
      }
    }

    public abstract object DataObject { get; }

    public abstract Processed2DPlotData GetRangesAndPoints(IPlotArea layer);

    public void CopyFrom(G2DPlotItem from)
    {
      CopyFrom((PlotItem)from);
    }
    protected override void CopyFrom(PlotItem fromb)
    {
      base.CopyFrom(fromb);

      G2DPlotItem from = fromb as G2DPlotItem;
      if (from != null)
      {
        this.Style = from.Style.Clone();
      }
    }

    /// <summary>
    /// Retrieves the name of the provided object.
    /// </summary>
    /// <param name="o">The object for which the name should be found.</param>
    /// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
    public override string GetNameOfChildObject(object o)
    {
      if (o == null)
        return null;

      else
        return null;
    }

    #region IPlotItem Members

    public override void CollectStyles(PlotGroupStyleCollection styles)
    {
      // first add missing local group styles
      foreach (IG2DPlotStyle sps in _plotStyles)
        sps.CollectExternalGroupStyles(styles);
    }

    public override void PrepareStyles(PlotGroupStyleCollection externalGroups, IPlotArea layer)
    {
      Processed2DPlotData pdata = GetRangesAndPoints(layer);
      _localGroups = new PlotGroupStyleCollection();
      
      // first add missing local group styles
      foreach (IG2DPlotStyle sps in _plotStyles)
        sps.CollectLocalGroupStyles(externalGroups, _localGroups);

      // now prepare the groups
      
      foreach (IG2DPlotStyle sps in _plotStyles)
        sps.PrepareGroupStyles(externalGroups, _localGroups, layer, pdata);
    }

    public override void ApplyStyles(PlotGroupStyleCollection externalGroups)
    {
      
      foreach (IG2DPlotStyle sps in _plotStyles)
        sps.ApplyGroupStyles(externalGroups, _localGroups);
    }

    /// <summary>
    /// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
    /// </summary>
    /// <param name="template">The template item to copy the plot styles from.</param>
    /// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
    public override void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness)
    {
      if (!(template is G2DPlotItem) || object.ReferenceEquals(this,template))
        return;
      G2DPlotItem from = (G2DPlotItem)template;
      this._plotStyles.SetFromTemplate(from._plotStyles, strictness);
     }

    public override void PaintSymbol(Graphics g, RectangleF location)
    {
      _plotStyles.PaintSymbol(g, location);     
    }

    #endregion

    public override void Paint(Graphics g, IPlotArea layer)
    {
      Processed2DPlotData pdata = GetRangesAndPoints(layer);
      if(pdata!=null)
        Paint(g, layer, GetRangesAndPoints(layer));
    }


    /// <summary>
    /// Needed for coordinate transforming styles to plot the data.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="layer">The plot layer.</param>
    /// <param name="plotdata">The plot data. Since the data are transformed, you should not
    /// rely that the physical values in this item correspond to the area coordinates.</param>
    public virtual void Paint(Graphics g, IPlotArea layer, Processed2DPlotData plotdata)
    {
      if (null != this._plotStyles)
      {
        _plotStyles.Paint(g, layer, plotdata);
      }
      _cachedPlotDataUsedForPainting = plotdata;
    }


    /// <summary>
    /// Test wether the mouse hits a plot item. 
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>Null if no hit, or a <see cref="IHitTestObject" /> if there was a hit.</returns>
    public override IHitTestObject HitTest(IPlotArea layer, PointF hitpoint)
    {
      

      Processed2DPlotData pdata = _cachedPlotDataUsedForPainting;
      if (null == pdata)
        return null;

     
        PlotRangeList rangeList = pdata.RangeList;
        PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

        if (ptArray.Length < 2048)
        {
          GraphicsPath gp = new GraphicsPath();
          gp.AddLines(ptArray);
          if (gp.IsOutlineVisible(hitpoint.X, hitpoint.Y, new Pen(Color.Black, 5)))
          {
            gp.Widen(new Pen(Color.Black, 5));
            return new HitTestObject(gp, this);
          }
        }
        else // we have too much points for the graphics path, so make a hit test first
        {

          int hitindex = -1;
          for (int i = 1; i < ptArray.Length; i++)
          {
            if (Drawing2DRelated.IsPointIntoDistance(ptArray[i - 1], ptArray[i], hitpoint, 5))
            {
              hitindex = i;
              break;
            }
          }
          if (hitindex < 0)
            return null;
          GraphicsPath gp = new GraphicsPath();
          int start = Math.Max(0, hitindex - 1);
          gp.AddLine(ptArray[start], ptArray[start + 1]);
          gp.AddLine(ptArray[start + 1], ptArray[start + 2]);
          gp.Widen(new Pen(Color.Black, 5));
          return new HitTestObject(gp, this);
        }
      


      return null;
    }


    /// <summary>
    /// Returns the index of a scatter point that is nearest to the location <c>hitpoint</c>
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>The information about the point that is nearest to the location, or null if it can not be determined.</returns>
    public XYScatterPointInformation GetNearestPlotPoint(IPlotArea layer, PointF hitpoint)
    {


    

      Processed2DPlotData pdata;
      if (null != (pdata = _cachedPlotDataUsedForPainting))
      {
        PlotRangeList rangeList = pdata.RangeList;
        PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
        double mindistance = double.MaxValue;
        int minindex = -1;
        for (int i = 1; i < ptArray.Length; i++)
        {
          double distance = Drawing2DRelated.SquareDistanceLineToPoint(ptArray[i - 1], ptArray[i], hitpoint);
          if (distance < mindistance)
          {
            mindistance = distance;
            minindex = Drawing2DRelated.Distance(ptArray[i - 1], hitpoint) < Drawing2DRelated.Distance(ptArray[i], hitpoint) ? i - 1 : i;
          }
        }
        // ok, minindex is the point we are looking for
        // so we have a look in the rangeList, what row it belongs to
        int rowindex = rangeList.GetRowIndexForPlotIndex(minindex);

        return new XYScatterPointInformation(ptArray[minindex], rowindex, minindex);
      }


      return null;
    }



    /// <summary>
    /// For a given plot point of index oldplotindex, finds the index and coordinates of a plot point
    /// of index oldplotindex+increment.
    /// </summary>
    /// <param name="layer">The layer this plot belongs to.</param>
    /// <param name="oldplotindex">Old plot index.</param>
    /// <param name="increment">Increment to the plot index.</param>
    /// <returns>Information about the new plot point find at position (oldplotindex+increment). Returns null if no such point exists.</returns>
    public XYScatterPointInformation GetNextPlotPoint(IPlotArea layer, int oldplotindex, int increment)
    {

     

      Processed2DPlotData pdata;
      if (null != (pdata = _cachedPlotDataUsedForPainting))
      {
        PlotRangeList rangeList = pdata.RangeList;
        PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
        if (ptArray.Length == 0)
          return null;

        int minindex = oldplotindex + increment;
        minindex = Math.Max(minindex, 0);
        minindex = Math.Min(minindex, ptArray.Length - 1);
        // ok, minindex is the point we are looking for
        // so we have a look in the rangeList, what row it belongs to
        int rowindex = rangeList.GetRowIndexForPlotIndex(minindex);
        return new XYScatterPointInformation(ptArray[minindex], rowindex, minindex);
      }


      return null;
    }

  } // end of class PlotItem
}
