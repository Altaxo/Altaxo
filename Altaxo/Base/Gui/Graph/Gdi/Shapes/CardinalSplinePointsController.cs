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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface ICardinalSplinePointsView
  {
    double Tension { get; set; }

    List<PointD2D> CurvePoints { get; set; }

    event Action CurvePointsCopyTriggered;

    event Action CurvePointsCopyAsPhysicalTriggered;

    event Action CurvePointsCopyAsLogicalTriggered;

    event Action CurvePointsPasteTriggered;

    event Action CurvePointsPastePhysicalTriggered;

    event Action CurvePointsPasteLogicalTriggered;
  }

  public class CardinalSplinePointsController
  {
    private ICardinalSplinePointsView _view;
    private GraphicBase _doc;

    public CardinalSplinePointsController(ICardinalSplinePointsView view, List<PointD2D> curvePoints, double tension, GraphicBase documentNode)
    {
      _view = view;
      _doc = documentNode;

      _view.CurvePointsCopyTriggered += new Action(EhCurvePointsCopyTriggered);
      _view.CurvePointsCopyAsPhysicalTriggered += new Action(EhCurvePointsCopyPhysicalTriggered);
      _view.CurvePointsCopyAsLogicalTriggered += new Action(EhCurvePointsCopyLogicalTriggered);
      _view.CurvePointsPasteTriggered += new Action(EhCurvePointsPasteTriggered);
      _view.CurvePointsPastePhysicalTriggered += new Action(EhCurvePointsPastePhysicalTriggered);
      _view.CurvePointsPasteLogicalTriggered += new Action(EhCurvePointsPasteLogicalTriggered);

      _view.Tension = tension;
      _view.CurvePoints = curvePoints;
    }

    public bool Apply(out List<PointD2D> curvePoints, out double tension)
    {
      curvePoints = _view.CurvePoints;
      tension = _view.Tension;
      return true;
    }

    private void EhCurvePointsPasteTriggered()
    {
      Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
      if (null == table)
        return;
      Altaxo.Data.DoubleColumn xcol = null;
      Altaxo.Data.DoubleColumn ycol = null;
      // Find the first column that contains numeric values
      int i;
      for (i = 0; i < table.DataColumnCount; i++)
      {
        if (table[i] is Altaxo.Data.DoubleColumn)
        {
          xcol = table[i] as Altaxo.Data.DoubleColumn;
          break;
        }
      }
      for (i = i + 1; i < table.DataColumnCount; i++)
      {
        if (table[i] is Altaxo.Data.DoubleColumn)
        {
          ycol = table[i] as Altaxo.Data.DoubleColumn;
          break;
        }
      }

      if (!(xcol != null && ycol != null))
        return;

      int len = Math.Min(xcol.Count, ycol.Count);
      var list = new List<PointD2D>();
      for (i = 0; i < len; i++)
      {
        list.Add(new PointD2D(
          new DimensionfulQuantity(xcol[i], PositionEnvironment.Instance.DefaultUnit).AsValueIn(AUL.Point.Instance),
          new DimensionfulQuantity(ycol[i], PositionEnvironment.Instance.DefaultUnit).AsValueIn(AUL.Point.Instance)
          ));
      }

      _view.CurvePoints = list;
    }

    private void EhCurvePointsPastePhysicalTriggered()
    {
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_doc);

      if (null == layer)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of physical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _doc.TransformationFromHereToParent(layer);

      Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
      if (null == table)
        return;
      Altaxo.Data.DoubleColumn xcol = null;
      Altaxo.Data.DoubleColumn ycol = null;
      // Find the first column that contains numeric values
      int i;
      for (i = 0; i < table.DataColumnCount; i++)
      {
        if (table[i] is Altaxo.Data.DoubleColumn)
        {
          xcol = table[i] as Altaxo.Data.DoubleColumn;
          break;
        }
      }
      for (i = i + 1; i < table.DataColumnCount; i++)
      {
        if (table[i] is Altaxo.Data.DoubleColumn)
        {
          ycol = table[i] as Altaxo.Data.DoubleColumn;
          break;
        }
      }

      if (!(xcol != null && ycol != null))
        return;

      int len = Math.Min(xcol.Count, ycol.Count);
      var list = new List<PointD2D>();
      for (i = 0; i < len; i++)
      {
        // calculate position
        var lx = layer.XAxis.PhysicalVariantToNormal(xcol[i]);
        var ly = layer.YAxis.PhysicalVariantToNormal(ycol[i]);

        if (layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(lx, ly), out var xpos, out var ypos))
        {
          var pt = cachedTransformation.InverseTransformPoint(new PointD2D(xpos, ypos));
          list.Add(new PointD2D(
            new DimensionfulQuantity(pt.X, PositionEnvironment.Instance.DefaultUnit).AsValueIn(AUL.Point.Instance),
            new DimensionfulQuantity(pt.Y, PositionEnvironment.Instance.DefaultUnit).AsValueIn(AUL.Point.Instance)
            ));
        }
      }

      _view.CurvePoints = list;
    }

    private void EhCurvePointsPasteLogicalTriggered()
    {
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_doc);

      if (null == layer)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of physical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _doc.TransformationFromHereToParent(layer);

      Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
      if (null == table)
        return;
      Altaxo.Data.DoubleColumn xcol = null;
      Altaxo.Data.DoubleColumn ycol = null;
      // Find the first column that contains numeric values
      int i;
      for (i = 0; i < table.DataColumnCount; i++)
      {
        if (table[i] is Altaxo.Data.DoubleColumn)
        {
          xcol = table[i] as Altaxo.Data.DoubleColumn;
          break;
        }
      }
      for (i = i + 1; i < table.DataColumnCount; i++)
      {
        if (table[i] is Altaxo.Data.DoubleColumn)
        {
          ycol = table[i] as Altaxo.Data.DoubleColumn;
          break;
        }
      }

      if (!(xcol != null && ycol != null))
        return;

      int len = Math.Min(xcol.Count, ycol.Count);
      var list = new List<PointD2D>();
      for (i = 0; i < len; i++)
      {
        // calculate position
        var lx = xcol[i];
        var ly = ycol[i];

        if (layer.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(lx, ly), out var xpos, out var ypos))
        {
          var pt = cachedTransformation.InverseTransformPoint(new PointD2D(xpos, ypos));
          list.Add(new PointD2D(
            new DimensionfulQuantity(pt.X, PositionEnvironment.Instance.DefaultUnit).AsValueIn(AUL.Point.Instance),
            new DimensionfulQuantity(pt.Y, PositionEnvironment.Instance.DefaultUnit).AsValueIn(AUL.Point.Instance)
            ));
        }
      }

      _view.CurvePoints = list;
    }

    private void EhCurvePointsCopyTriggered()
    {
      var points = _view.CurvePoints;

      var dao = Current.Gui.GetNewClipboardDataObject();
      var xcol = new Altaxo.Data.DoubleColumn();
      var ycol = new Altaxo.Data.DoubleColumn();
      for (int i = 0; i < points.Count; i++)
      {
        xcol[i] = new DimensionfulQuantity(points[i].X, AUL.Point.Instance).AsValueIn(PositionEnvironment.Instance.DefaultUnit);
        ycol[i] = new DimensionfulQuantity(points[i].Y, AUL.Point.Instance).AsValueIn(PositionEnvironment.Instance.DefaultUnit);
      }

      PutXYColumnsToClipboard(xcol, ycol);
    }

    private void EhCurvePointsCopyPhysicalTriggered()
    {
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_doc);

      if (null == layer)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of physical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _doc.TransformationFromHereToParent(layer);

      var points = _view.CurvePoints;

      var xcol = new Altaxo.Data.DoubleColumn();
      var ycol = new Altaxo.Data.DoubleColumn();
      for (int i = 0; i < points.Count; i++)
      {
        var pt = cachedTransformation.TransformPoint(points[i]);
        if (layer.CoordinateSystem.LayerToLogicalCoordinates(pt.X, pt.Y, out var r))
        {
          var x = layer.XAxis.NormalToPhysicalVariant(r.RX);
          var y = layer.YAxis.NormalToPhysicalVariant(r.RY);
          xcol[i] = x;
          ycol[i] = y;
        }
        else
        {
          xcol[i] = ycol[i] = double.NaN;
        }
      }

      PutXYColumnsToClipboard(xcol, ycol);
    }

    private void EhCurvePointsCopyLogicalTriggered()
    {
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_doc);

      if (null == layer)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of logical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _doc.TransformationFromHereToParent(layer);
      var points = _view.CurvePoints;

      var xcol = new Altaxo.Data.DoubleColumn();
      var ycol = new Altaxo.Data.DoubleColumn();
      for (int i = 0; i < points.Count; i++)
      {
        var pt = cachedTransformation.TransformPoint(points[i]);
        if (layer.CoordinateSystem.LayerToLogicalCoordinates(pt.X, pt.Y, out var r))
        {
          var x = r.RX;
          var y = r.RY;
          xcol[i] = x;
          ycol[i] = y;
        }
        else
        {
          xcol[i] = ycol[i] = double.NaN;
        }
      }

      PutXYColumnsToClipboard(xcol, ycol);
    }

    private static void PutXYColumnsToClipboard(Altaxo.Data.DoubleColumn xcol, Altaxo.Data.DoubleColumn ycol)
    {
      var tb = new Altaxo.Data.DataTable();
      tb.DataColumns.Add(xcol, "XPosition", Altaxo.Data.ColumnKind.V, 0);
      tb.DataColumns.Add(ycol, "YPosition", Altaxo.Data.ColumnKind.V, 0);

      var dao = Current.Gui.GetNewClipboardDataObject();
      Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
        tb, new Altaxo.Collections.AscendingIntegerCollection(),
        new Altaxo.Collections.AscendingIntegerCollection(),
        new Altaxo.Collections.AscendingIntegerCollection(),
        dao);
      Current.Gui.SetClipboardDataObject(dao, true);
    }
  }
}
