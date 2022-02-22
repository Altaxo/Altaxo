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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Units;
using Altaxo.Collections;
using AUL = Altaxo.Units.Length;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface ICardinalSplinePointsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(ICardinalSplinePointsView))]
  public class CardinalSplinePointsController : MVCANControllerEditImmutableDocBase<(List<PointD2D> CurvePoints, double Tension), ICardinalSplinePointsView>
  {
    GraphicBase _documentNode;

    public CardinalSplinePointsController( List<PointD2D> curvePoints, double tension, GraphicBase documentNode)
    {
      _documentNode = documentNode;
      _doc = (curvePoints, tension);

      Tension = new DimensionfulQuantity(tension, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(TensionEnvironment.DefaultUnit);
      CurvePoints=curvePoints;

      CmdCopyCurvePoints = new RelayCommand(EhCurvePointsCopyTriggered);
      CmdCopyCurvePoints = new RelayCommand(EhCurvePointsPasteTriggered);
      CmdCopyCurvePointsAsPhysical = new RelayCommand(EhCurvePointsCopyPhysicalTriggered);
      CmdPasteCurvePointsAsPhysical = new RelayCommand(EhCurvePointsPastePhysicalTriggered);
      CmdCopyCurvePointsAsLogical = new RelayCommand(EhCurvePointsCopyLogicalTriggered);
      CmdPasteCurvePointsAsLogical = new RelayCommand(EhCurvePointsPasteLogicalTriggered);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    public ICommand CmdCopyCurvePoints { get; }
    public ICommand CmdPasteCurvePoints { get; }
    public ICommand CmdCopyCurvePointsAsPhysical { get; }
    public ICommand CmdPasteCurvePointsAsPhysical { get; }
    public ICommand CmdCopyCurvePointsAsLogical { get; }
    public ICommand CmdPasteCurvePointsAsLogical { get; }

    public QuantityWithUnitGuiEnvironment TensionEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _tension;

    public DimensionfulQuantity Tension
    {
      get => _tension;
      set
      {
        if (!(_tension == value))
        {
          _tension = value;
          OnPropertyChanged(nameof(Tension));
        }
      }
    }

    private ObservableCollection<PointD2DClass> _curvePointsEditable=new ObservableCollection<PointD2DClass>();

    public ObservableCollection<PointD2DClass> CurvePointsEditable
    {
      get => _curvePointsEditable;
      set
      {
        if (!(_curvePointsEditable == value))
        {
          _curvePointsEditable = value;
          OnPropertyChanged(nameof(CurvePointsEditable));
        }
      }
    }


    #endregion

    private List<PointD2D> CurvePoints
    {
      get
      {
        var pts = new List<PointD2D>();
        foreach (var p in _curvePointsEditable)
          pts.Add(new PointD2D(p.X, p.Y));
        return pts;
      }
      set
      {
        _curvePointsEditable.Clear();
        foreach (var p in value)
          _curvePointsEditable.Add(new PointD2DClass(p));
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = (CurvePoints, Tension.AsValueInSIUnits);
      return ApplyEnd(true, disposeController);
    }

    private void EhCurvePointsPasteTriggered()
    {
      Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
      if (table is null)
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

      if (!(xcol is not null && ycol is not null))
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

      CurvePoints = list;
    }

    private void EhCurvePointsPastePhysicalTriggered()
    {
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_documentNode);

      if (layer is null)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of physical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _documentNode.TransformationFromHereToParent(layer);

      Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
      if (table is null)
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

      if (!(xcol is not null && ycol is not null))
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

      CurvePoints = list;
    }

    private void EhCurvePointsPasteLogicalTriggered()
    {
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_documentNode);

      if (layer is null)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of physical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _documentNode.TransformationFromHereToParent(layer);

      Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
      if (table is null)
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

      if (!(xcol is not null && ycol is not null))
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

      CurvePoints = list;
    }

    private void EhCurvePointsCopyTriggered()
    {
      var points = CurvePoints;

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
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_documentNode);

      if (layer is null)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of physical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _documentNode.TransformationFromHereToParent(layer);

      var points = CurvePoints;

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
      var layer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.XYPlotLayer>(_documentNode);

      if (layer is null)
      {
        Current.Gui.ErrorMessageBox("Could not find a parent X-Y layer. Thus, the calculation of logical coordinates is not possible!");
        return;
      }

      var cachedTransformation = _documentNode.TransformationFromHereToParent(layer);
      var points = CurvePoints;

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

    #region Editable Curve Points

    public class PointD2DClass : System.ComponentModel.IEditableObject
    {
      public PointD2DClass()
      {
      }

      public PointD2DClass(PointD2D p)
      {
        X = p.X;
        Y = p.Y;
      }

      public double X { get; set; }

      public double Y { get; set; }

      public DimensionfulQuantity XQuantity
      {
        get
        {
          return new DimensionfulQuantity(X, Altaxo.Units.Length.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit);
        }
        set
        {
          X = value.AsValueIn(Altaxo.Units.Length.Point.Instance);
        }
      }

      public DimensionfulQuantity YQuantity
      {
        get
        {
          return new DimensionfulQuantity(Y, Altaxo.Units.Length.Point.Instance).AsQuantityIn(PositionEnvironment.Instance.DefaultUnit);
        }
        set
        {
          Y = value.AsValueIn(Altaxo.Units.Length.Point.Instance);
        }
      }

      public void BeginEdit()
      {
      }

      public void CancelEdit()
      {
      }

      public void EndEdit()
      {
      }
    }


    #endregion
  }
}
