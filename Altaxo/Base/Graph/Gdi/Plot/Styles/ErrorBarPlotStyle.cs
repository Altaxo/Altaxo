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

using Altaxo.Data;
using Altaxo.Graph.Plot.Data;
using Altaxo.Graph.Plot.Groups;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  public class ErrorBarPlotStyle : IG2DPlotStyle
  {
    private NumericColumnProxy _positiveErrorColumn;
    private NumericColumnProxy _negativeErrorColumn;

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    /// <summary>Pen used to draw the error bar.</summary>
    PenX _strokePen;

    /// <summary>
    /// True when to plot horizontal error bars.
    /// </summary>
    bool _isHorizontalStyle;

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    bool _independentSymbolSize;

    /// <summary>Controls the length of the end bar.</summary>
    float _symbolSize;

    /// <summary>
    /// True when the line is not drawn in the circel of diameter SymbolSize around the symbol center.
    /// </summary>
    bool _symbolGap;

    /// <summary>
    /// If true, the bars are capped by an end bar.
    /// </summary>
    bool _showEndBars = true;


    /// <summary>
    /// When true, bar graph position group styles are not applied, i.e. the item remains where it is.
    /// </summary>
    bool _doNotShiftHorizontalPosition;

    /// <summary>
    /// Skip frequency.
    /// </summary>
    protected int _skipFreq;

    /// <summary>
    /// When we deal with bar charts, this is the logical shift between real point
    /// and the independent value where the bar is really drawn to.
    /// </summary>
    double _cachedLogicalShiftOfIndependent;

    [NonSerialized]
    object _parent;

    [field: NonSerialized]
    public event EventHandler Changed;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ErrorBarPlotStyle), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ErrorBarPlotStyle s = (ErrorBarPlotStyle)obj;

        info.AddValue("PositiveError", s._positiveErrorColumn);
        info.AddValue("NegativeError", s._negativeErrorColumn);

        info.AddValue("IndependentColor", s._isHorizontalStyle);
        info.AddValue("Pen", s._strokePen);

        info.AddValue("Axis", s._isHorizontalStyle ? 0 : 1);
        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SymbolSize", s._symbolSize);
        info.AddValue("SymbolGap", s._symbolGap);
        info.AddValue("SkipFreq", s._skipFreq);

        info.AddValue("ShowEndBars", s._showEndBars);
        info.AddValue("NotShiftHorzPos", s._doNotShiftHorizontalPosition);
      }

      protected virtual ErrorBarPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ErrorBarPlotStyle s = null != o ? (ErrorBarPlotStyle)o : new ErrorBarPlotStyle();

        s._positiveErrorColumn = (Altaxo.Data.NumericColumnProxy)info.GetValue("PositiveError");
        s._negativeErrorColumn = (Altaxo.Data.NumericColumnProxy)info.GetValue("NegativeError");

        s._independentColor = info.GetBoolean("IndependentColor");
        s.Pen = (PenX)info.GetValue("Pen", s);

        s._isHorizontalStyle = (0==info.GetInt32("Axis"));
        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetInt32("SymbolSize");
        s._symbolGap = info.GetBoolean("SymbolGap");
        s._skipFreq = info.GetInt32("SkipFreq");
        s._showEndBars = info.GetBoolean("ShowEndBars");
        s._doNotShiftHorizontalPosition = info.GetBoolean("NotShiftHorzPos");

        return s;
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ErrorBarPlotStyle s = SDeserialize(o, info, parent);

        return s;
      }

    }

    

    #endregion


    public ErrorBarPlotStyle()
    {
      this._strokePen = new PenX(Color.Black);
    }
    public ErrorBarPlotStyle(ErrorBarPlotStyle from)
    {
      CopyFrom(from);
    }

    void CopyFrom(ErrorBarPlotStyle from)
    {
      this._independentSymbolSize = from._independentSymbolSize;
      this._symbolSize = from._symbolSize;
      this._symbolGap = from._symbolGap;
      this._independentColor = from._independentColor;
      this._showEndBars = from._showEndBars;
      this._isHorizontalStyle = from._isHorizontalStyle;
      this._doNotShiftHorizontalPosition = from._doNotShiftHorizontalPosition;
      this._strokePen = (PenX)from._strokePen.Clone();
      this._positiveErrorColumn = (NumericColumnProxy)from._positiveErrorColumn.Clone();
      this._negativeErrorColumn = (NumericColumnProxy)from._negativeErrorColumn.Clone();
      this._cachedLogicalShiftOfIndependent = from._cachedLogicalShiftOfIndependent;
    }

    #region Properties

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    public bool IndependentSymbolSize
    {
      get { return _independentSymbolSize; }
      set { _independentSymbolSize = value; }
    }

    /// <summary>Controls the length of the end bar.</summary>
    public float SymbolSize
    {
      get { return _symbolSize; }
      set { _symbolSize = value; }
    }

    /// <summary>Controls the length of the end bar.</summary>
    public int SkipFrequency
    {
      get { return _skipFreq; }
      set { _skipFreq = value; }
    }

    /// <summary>
    /// True when the line is not drawn in the circel of diameter SymbolSize around the symbol center.
    /// </summary>
    public bool SymbolGap
    {
      get { return _symbolGap; }
      set { _symbolGap = value; }
    }

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    public bool IndependentColor
    {
      get { return _independentColor; }
      set { _independentColor = value; }
    }

    /// <summary>
    /// If true, the bars are capped by an end bar.
    /// </summary>
    public bool ShowEndBars
    {
      get
      {
        return _showEndBars; 
      }
      set
      {
        _showEndBars = value;
      }
    }

    /// <summary>
    /// True when we don't want to shift the horizontal position, for instance due to the bar graph plot group.
    /// </summary>
    public bool DoNotShiftIndependentVariable
    {
      get
      {
        return _doNotShiftHorizontalPosition;
      }
      set
      {
        _doNotShiftHorizontalPosition = value;
      }
    }

    /// <summary>
    /// True when no vertical, but horizontal error bars are shown.
    /// </summary>
    public bool IsHorizontalStyle
    {
      get
      {
        return _isHorizontalStyle;
      }
      set
      {
        _isHorizontalStyle = value;
      }
    }

    /// <summary>Pen used to draw the error bar.</summary>
    public PenX Pen
    {
      get { return _strokePen; }
      set { _strokePen = value; }
    }

    /// <summary>
    /// Data that define the error in the positive direction.
    /// </summary>
    public INumericColumn PositiveErrorColumn
    {
      get { return _positiveErrorColumn == null ? null : _positiveErrorColumn.Document; }
      set
      {
        _positiveErrorColumn = new NumericColumnProxy(value);
      }
    }

    /// <summary>
    /// Data that define the error in the negative direction.
    /// </summary>
    public INumericColumn NegativeErrorColumn
    {
      get { return _negativeErrorColumn == null ? null : _negativeErrorColumn.Document; }
      set
      {
        _negativeErrorColumn = new NumericColumnProxy(value);
      }
    }


    #endregion

    #region IG2DPlotStyle Members

    public void CollectExternalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);

      SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group only)
     
    }

    public void PrepareGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
    {
      if (!_independentColor)
        Graph.Plot.Groups.ColorGroupStyle.PrepareStyle(externalGroups,localGroups, delegate() { return PlotColors.Colors.GetPlotColor(this._strokePen.Color); });

      // SkipFrequency should be the same for all sub plot styles, so there is no "private" property
      SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate() { return SkipFrequency; });


      // note: symbol size and barposition are only applied, but not prepared
      // this item can not be used as provider of a symbol size
    }

    public void ApplyGroupStyles(Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection externalGroups, Altaxo.Graph.Gdi.Plot.Groups.PlotGroupStyleCollection localGroups)
    {
      // color
      if (!_independentColor)
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(PlotColor c) { this._strokePen.Color = c; });

      // SkipFrequency should be the same for all sub plot styles, so there is no "private" property
      SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(int c) { this.SkipFrequency = c; });

      // symbol size
      if (!_independentSymbolSize)
      {
        if (!SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate(float size) { this._symbolSize = size; }))
        {
          this._symbolSize = 0;
        }
      }

      // bar position
      BarWidthPositionGroupStyle bwp = PlotGroupStyle.GetStyleToApply<BarWidthPositionGroupStyle>(externalGroups, localGroups);
      if (null != bwp && !_doNotShiftHorizontalPosition)
      {
        double innerGapW, outerGapW, width, lpos;
        bwp.Apply(out innerGapW, out outerGapW, out width, out lpos);
        _cachedLogicalShiftOfIndependent = lpos + width / 2;
      }
      else
      {
        _cachedLogicalShiftOfIndependent = 0;
      }

    
    }



    public void Paint(System.Drawing.Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
    {
      if (_isHorizontalStyle)
        PaintXErrorBars(g, layer, pdata);
      else
        PaintYErrorBars(g, layer, pdata);
    }

    protected void PaintYErrorBars(System.Drawing.Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
    {

      // Plot error bars for the dependent variable (y)
      PlotRangeList rangeList = pdata.RangeList;
      PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
      INumericColumn posErrCol = _positiveErrorColumn.Document;
      INumericColumn negErrCol = _negativeErrorColumn.Document;

      if (posErrCol == null && negErrCol == null)
        return; // nothing to do if both error columns are null

      System.Drawing.Drawing2D.GraphicsPath errorBarPath = new System.Drawing.Drawing2D.GraphicsPath();

      Region oldClippingRegion = g.Clip;
      Region newClip = (Region)oldClippingRegion.Clone();

      foreach (PlotRange r in rangeList)
      {
        int lower = r.LowerBound;
        int upper = r.UpperBound;
        int offset = r.OffsetToOriginal;
        for (int j = lower; j < upper; j++)
        {
          AltaxoVariant y = pdata.GetYPhysical(j + offset);
          Logical3D lm = layer.GetLogical3D(pdata, j + offset);
          lm.RX += _cachedLogicalShiftOfIndependent;
          if (lm.IsNaN)
            continue;

          Logical3D lh = lm;
          Logical3D ll = lm;
          bool lhvalid=false;
          bool llvalid=false;
          if (posErrCol != null)
          {
            lh.RY = layer.YAxis.PhysicalVariantToNormal(y + Math.Abs(posErrCol[j + offset]));
            lhvalid = !lh.IsNaN;
          }
          if (negErrCol != null)
          {
            ll.RY = layer.YAxis.PhysicalVariantToNormal(y - Math.Abs(negErrCol[j + offset]));
            llvalid = !ll.IsNaN;
          }
          if (!(lhvalid || llvalid))
            continue; // nothing to do for this point if both pos and neg logical point are invalid.

          // now paint the error bar
          if (_symbolGap) // if symbol gap, then clip the painting, exclude a rectangle of size symbolSize x symbolSize
          {
            double xlm, ylm;
            layer.CoordinateSystem.LogicalToLayerCoordinates(lm,out xlm, out ylm);
            newClip.Union(oldClippingRegion);
            newClip.Exclude(new RectangleF((float)(xlm - _symbolSize / 2), (float)(ylm - _symbolSize / 2), _symbolSize, _symbolSize));
            g.Clip = newClip;
          }

          if (lhvalid && llvalid)
          {
            errorBarPath.Reset();
            layer.CoordinateSystem.GetIsoline(errorBarPath, ll, lm);
            layer.CoordinateSystem.GetIsoline(errorBarPath, lm, lh);
            g.DrawPath(_strokePen, errorBarPath);
          }
          else if (llvalid)
          {
            layer.CoordinateSystem.DrawIsoline(g, _strokePen, ll, lm);
          }
          else if (lhvalid)
          {
            layer.CoordinateSystem.DrawIsoline(g, _strokePen, lm, lh);
          }


          // now the end bars
          if (_showEndBars)
          {
            if (lhvalid)
            {
              PointF outDir;
              layer.CoordinateSystem.GetNormalizedDirection(lm, lh, 1, new Logical3D(1, 0), out outDir);
              outDir.X *= _symbolSize / 2;
              outDir.Y *= _symbolSize / 2;
              double xlay, ylay;
              layer.CoordinateSystem.LogicalToLayerCoordinates(lh, out xlay, out ylay);
              // Draw a line from x,y to 
              g.DrawLine(_strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
            }

            if (llvalid)
            {
              PointF outDir;
              layer.CoordinateSystem.GetNormalizedDirection(lm, ll, 1, new Logical3D(1, 0), out outDir);
              outDir.X *= _symbolSize / 2;
              outDir.Y *= _symbolSize / 2;
              double xlay, ylay;
              layer.CoordinateSystem.LogicalToLayerCoordinates(ll, out xlay, out ylay);
              // Draw a line from x,y to 
              g.DrawLine(_strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
            }

          }
        }
      }

      g.Clip = oldClippingRegion;
    }


    protected void PaintXErrorBars(System.Drawing.Graphics g, IPlotArea layer, Altaxo.Graph.Gdi.Plot.Data.Processed2DPlotData pdata)
    {

      // Plot error bars for the independent variable (x)
      PlotRangeList rangeList = pdata.RangeList;
      PointF[] ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
      INumericColumn posErrCol = _positiveErrorColumn.Document;
      INumericColumn negErrCol = _negativeErrorColumn.Document;

      if (posErrCol == null && negErrCol == null)
        return; // nothing to do if both error columns are null

      System.Drawing.Drawing2D.GraphicsPath errorBarPath = new System.Drawing.Drawing2D.GraphicsPath();

      Region oldClippingRegion = g.Clip;
      Region newClip = (Region)oldClippingRegion.Clone();

      foreach (PlotRange r in rangeList)
      {
        int lower = r.LowerBound;
        int upper = r.UpperBound;
        int offset = r.OffsetToOriginal;
        for (int j = lower; j < upper; j++)
        {
          AltaxoVariant x = pdata.GetXPhysical(j + offset);
          Logical3D lm = layer.GetLogical3D(pdata, j + offset);
          lm.RX += _cachedLogicalShiftOfIndependent;
          if (lm.IsNaN)
            continue;

          Logical3D lh = lm;
          Logical3D ll = lm;
          bool lhvalid = false;
          bool llvalid = false;
          if (posErrCol != null)
          {
            lh.RX = layer.XAxis.PhysicalVariantToNormal(x + Math.Abs(posErrCol[j + offset]));
            lhvalid = !lh.IsNaN;
          }
          if (negErrCol != null)
          {
            ll.RX = layer.XAxis.PhysicalVariantToNormal(x - Math.Abs(negErrCol[j + offset]));
            llvalid = !ll.IsNaN;
          }
          if (!(lhvalid || llvalid))
            continue; // nothing to do for this point if both pos and neg logical point are invalid.

          // now paint the error bar
          if (_symbolGap) // if symbol gap, then clip the painting, exclude a rectangle of size symbolSize x symbolSize
          {
            double xlm, ylm;
            layer.CoordinateSystem.LogicalToLayerCoordinates(lm, out xlm, out ylm);
            newClip.Union(oldClippingRegion);
            newClip.Exclude(new RectangleF((float)(xlm - _symbolSize / 2), (float)(ylm - _symbolSize / 2), _symbolSize, _symbolSize));
            g.Clip = newClip;
          }

          if (lhvalid && llvalid)
          {
            errorBarPath.Reset();
            layer.CoordinateSystem.GetIsoline(errorBarPath, ll, lm);
            layer.CoordinateSystem.GetIsoline(errorBarPath, lm, lh);
            g.DrawPath(_strokePen, errorBarPath);
          }
          else if (llvalid)
          {
            layer.CoordinateSystem.DrawIsoline(g, _strokePen, ll, lm);
          }
          else if (lhvalid)
          {
            layer.CoordinateSystem.DrawIsoline(g, _strokePen, lm, lh);
          }


          // now the end bars
          if (_showEndBars)
          {
            if (lhvalid)
            {
              PointF outDir;
              layer.CoordinateSystem.GetNormalizedDirection(lm, lh, 1, new Logical3D(0, 1), out outDir);
              outDir.X *= _symbolSize / 2;
              outDir.Y *= _symbolSize / 2;
              double xlay, ylay;
              layer.CoordinateSystem.LogicalToLayerCoordinates(lh, out xlay, out ylay);
              // Draw a line from x,y to 
              g.DrawLine(_strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
            }

            if (llvalid)
            {
              PointF outDir;
              layer.CoordinateSystem.GetNormalizedDirection(lm, ll, 1, new Logical3D(0, 1), out outDir);
              outDir.X *= _symbolSize / 2;
              outDir.Y *= _symbolSize / 2;
              double xlay, ylay;
              layer.CoordinateSystem.LogicalToLayerCoordinates(ll, out xlay, out ylay);
              // Draw a line from x,y to 
              g.DrawLine(_strokePen, (float)(xlay - outDir.X), (float)(ylay - outDir.Y), (float)(xlay + outDir.X), (float)(ylay + outDir.Y));
            }

          }
        }
      }

      g.Clip = oldClippingRegion;
    }



    public System.Drawing.RectangleF PaintSymbol(System.Drawing.Graphics g, System.Drawing.RectangleF bounds)
    {
      // Error bars are not painted in the symbol
      return bounds;
    }

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      return new ErrorBarPlotStyle(this);
    }

    #endregion

    #region IChangedEventSource Members

    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (Changed != null)
        Changed(this, EventArgs.Empty);
    }

    #endregion

    #region IDocumentNode Members

    object Altaxo.Main.IDocumentNode.ParentObject
    {
      get { return _parent; }
     // set { _parent = value; }
    }

    public string Name
    {
      get { return "ErrorPlotStyle"; }
    }

    #endregion
  }
}
