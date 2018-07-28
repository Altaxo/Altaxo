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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Information of what happens to superfluous layers during the arrange layers action.
  /// </summary>
  public enum SuperfluousLayersAction
  {
    /// <summary>Leave the layers untouched.</summary>
    Untouched,

    /// <summary>Remove the superfluous layers.</summary>
    Remove,

    /// <summary>The superfluous layers take the same position like the first layer.</summary>
    OverlayFirstLayer,

    /// <summary>The superfluous layers take the same position like the last regularly arranged layer.</summary>
    OverlayLastLayer
  }

  /// <summary>
  /// Holds the information how to arrange layers in a graph document.
  /// Is used by the <see cref="Altaxo.Gui.Graph.Gdi.ArrangeLayersController" /> controller.
  /// </summary>
  public class ArrangeLayersDocument : Main.ICopyFrom
  {
    public int NumberOfRows;
    public int NumberOfColumns;
    public double RowSpacing;
    public double ColumnSpacing;
    public double LeftMargin;
    public double TopMargin;
    public double RightMargin;
    public double BottomMargin;
    public SuperfluousLayersAction SuperfluousLayersAction;

    public bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as ArrangeLayersDocument;
      if (null != from)
      {
        this.NumberOfColumns = from.NumberOfColumns;
        this.NumberOfRows = from.NumberOfRows;
        this.RowSpacing = from.RowSpacing;
        this.ColumnSpacing = from.ColumnSpacing;
        this.LeftMargin = from.LeftMargin;
        this.TopMargin = from.TopMargin;
        this.RightMargin = from.RightMargin;
        this.BottomMargin = from.BottomMargin;
        this.SuperfluousLayersAction = from.SuperfluousLayersAction;
        return true;
      }
      return false;
    }

    public ArrangeLayersDocument()
    {
      NumberOfRows = 2;
      NumberOfColumns = 1;
      RowSpacing = 5 / 100.0;
      ColumnSpacing = 5 / 100.0;
      LeftMargin = 15 / 100.0;
      TopMargin = 10 / 100.0;
      RightMargin = 10 / 100.0;
      BottomMargin = 15 / 100.0;
      SuperfluousLayersAction = SuperfluousLayersAction.Untouched;
    }

    public ArrangeLayersDocument(ArrangeLayersDocument from)
    {
      CopyFrom(from);
    }

    object ICloneable.Clone()
    {
      return new ArrangeLayersDocument(this);
    }

    public ArrangeLayersDocument Clone()
    {
      return new ArrangeLayersDocument(this);
    }
  }

  /// <summary>
  /// Contains extension methods for the arrangement of layers.
  /// </summary>
  public static class GraphDocumentLayerArrangement
  {
    /// <summary>
    /// Shows the layer arrangement dialog and then arranges the layers according to the user input.
    /// </summary>
    /// <param name="graph">Graph that contains the layers to arrange.</param>
    /// <param name="activeLayer">Layer that is currently active.</param>
    public static void ShowLayerArrangementDialog(this GraphDocument graph, HostLayer activeLayer)
    {
      ArrangeLayersDocument arrangement = new ArrangeLayersDocument();
      object doco = arrangement;

      if (Current.Gui.ShowDialog(ref doco, "Arrange layers"))
      {
        arrangement = (ArrangeLayersDocument)doco;
        try
        {
          ArrangeLayers(activeLayer, arrangement);
        }
        catch (Exception ex)
        {
          Current.Gui.ErrorMessageBox(ex.Message);
        }
      }
    }

    public static void ArrangeGrid(this ArrangeLayersDocument arrangement, GridPartitioning grid)
    {
      grid.XPartitioning.Clear();
      grid.YPartitioning.Clear();

      double columnSize = Math.Max(0, 1 - arrangement.LeftMargin - arrangement.RightMargin - (arrangement.NumberOfColumns - 1) * arrangement.ColumnSpacing);
      double rowSize = Math.Max(0, 1 - arrangement.TopMargin - arrangement.BottomMargin - (arrangement.NumberOfRows - 1) * arrangement.RowSpacing);

      if (arrangement.NumberOfColumns > 0)
      {
        grid.XPartitioning.Add(RADouble.NewRel(arrangement.LeftMargin));
        for (int i = arrangement.NumberOfColumns - 1; i >= 0; --i)
        {
          grid.XPartitioning.Add(RADouble.NewRel(columnSize / arrangement.NumberOfColumns));
          if (i != 0)
            grid.XPartitioning.Add(RADouble.NewRel(arrangement.ColumnSpacing));
        }
        grid.XPartitioning.Add(RADouble.NewRel(arrangement.RightMargin));
      }
      else
      {
        grid.XPartitioning.Add(RADouble.NewRel(1));
      }

      if (arrangement.NumberOfRows > 0)
      {
        grid.YPartitioning.Add(RADouble.NewRel(arrangement.TopMargin));
        for (int i = arrangement.NumberOfRows - 1; i >= 0; --i)
        {
          grid.YPartitioning.Add(RADouble.NewRel(rowSize / arrangement.NumberOfRows));
          if (i != 0)
            grid.YPartitioning.Add(RADouble.NewRel(arrangement.RowSpacing));
        }
        grid.YPartitioning.Add(RADouble.NewRel(arrangement.BottomMargin));
      }
      else
      {
        grid.YPartitioning.Add(RADouble.NewRel(1));
      }
    }

    /// <summary>
    /// Arranges the layers according to the provided options.
    /// </summary>
    /// <param name="activeLayer">Layer, whose siblings are about to be arranged. (Exception: If the root layer is the active layer, then the childs of the root layer will be arranged.</param>
    /// <param name="arrangement">The layer arrangement options (contain the information how to arrange the layers).</param>
    public static void ArrangeLayers(this HostLayer activeLayer, ArrangeLayersDocument arrangement)
    {
      var context = activeLayer.GetPropertyContext();
      var parentLayer = activeLayer.ParentLayer ?? activeLayer;

      int numPresentLayers = parentLayer.Layers.Count;
      int numDestLayers = arrangement.NumberOfColumns * arrangement.NumberOfRows;

      int additionalLayers = Math.Max(0, numDestLayers - numPresentLayers);

      if (null == parentLayer.Grid)
        parentLayer.CreateDefaultGrid();

      ArrangeGrid(arrangement, parentLayer.Grid);

      int nLayer = -1;
      for (int i = 0; i < arrangement.NumberOfRows; ++i)
      {
        for (int j = 0; j < arrangement.NumberOfColumns; ++j)
        {
          nLayer++;

          if (nLayer >= numPresentLayers)
          {
            var graph = Altaxo.Graph.Gdi.GraphTemplates.TemplateWithXYPlotLayerWithG2DCartesicCoordinateSystem.CreateGraph(context, Guid.NewGuid().ToString(), "", false);

            if (graph != null && graph.RootLayer.Layers.Count > 0)
            {
              var newLayer = (HostLayer)graph.RootLayer.Layers[0].Clone();
              parentLayer.Layers.Add(newLayer);
            }
            else
            {
              var newLayer = new XYPlotLayer(parentLayer);
              newLayer.CreateDefaultAxes(context);
              parentLayer.Layers.Add(newLayer);
            }
          }

          var oldSize = parentLayer.Layers[nLayer].Size;
          parentLayer.Layers[nLayer].Location = new ItemLocationByGrid { GridColumn = 2 * j + 1, GridRow = 2 * i + 1, GridColumnSpan = 1, GridRowSpan = 1 };
          var newSize = parentLayer.Layers[nLayer].Size;
        }
      }

      // act now on superfluous layers
      if (numPresentLayers > numDestLayers)
      {
        switch (arrangement.SuperfluousLayersAction)
        {
          case SuperfluousLayersAction.Remove:
            for (int i = numPresentLayers - 1; i >= numDestLayers; i--)
              parentLayer.Layers.RemoveAt(i);
            break;

          case SuperfluousLayersAction.OverlayFirstLayer:
          case SuperfluousLayersAction.OverlayLastLayer:

            int template = arrangement.SuperfluousLayersAction == SuperfluousLayersAction.OverlayFirstLayer ? 0 : numDestLayers - 1;
            var templateLayer = parentLayer.Layers[template];

            for (int i = numDestLayers; i < numPresentLayers; i++)
            {
              var oldSize = parentLayer.Layers[i].Size;
              parentLayer.Layers[i].Location = (IItemLocation)templateLayer.Location.Clone();
              var newSize = parentLayer.Layers[i].Size;
            }

            break;
        }
      }
    }
  }
}
