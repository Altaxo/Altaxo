#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.ComponentModel;
using Altaxo.Gui.Graph.Gdi.Viewing;

namespace Altaxo.Graph.Commands
{
  /// <summary>
  /// This class is intended to be used for commands in the graph tools toolbar. Commands derived
  /// from it will update the toolbar whenever its state changed.
  /// </summary>
  public abstract class AbstractGraphToolsCommand : AbstractCheckableGraphControllerCommand
  {
    private GraphController myCurrentGraphController;
    private GraphToolType _graphToolType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractGraphToolsCommand"/> class.
    /// </summary>
    /// <param name="toolType">The graph tool type represented by the command.</param>
    protected AbstractGraphToolsCommand(GraphToolType toolType)
    {
      _graphToolType = toolType;
      if (Current.Workbench is not null)
      {
        Current.Workbench.PropertyChanged += EhWorkbenchContentChanged;
        EhWorkbenchContentChanged(this, new PropertyChangedEventArgs(nameof(Current.Workbench.ActiveViewContent)));
      }
    }

    /// <summary>
    /// Handles changes of the active workbench content.
    /// </summary>
    /// <param name="o">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected void EhWorkbenchContentChanged(object o, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != nameof(Current.Workbench.ActiveViewContent))
        return;

      if (!object.ReferenceEquals(Controller, myCurrentGraphController))
      {
        if (myCurrentGraphController is not null)
        {
          lock (this)
          {
            myCurrentGraphController.CurrentGraphToolChanged -= new EventHandler(EhGraphToolChanged);
            myCurrentGraphController = null;
          }
        }
        if (Controller is not null)
        {
          lock (this)
          {
            myCurrentGraphController = Controller;
            myCurrentGraphController.CurrentGraphToolChanged += new EventHandler(EhGraphToolChanged);
          }
        }
        OnPropertyChanged("IsChecked");
      }
    }

    /// <summary>
    /// Handles changes of the currently selected graph tool.
    /// </summary>
    /// <param name="o">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected void EhGraphToolChanged(object o, EventArgs e)
    {
      OnPropertyChanged(nameof(IsChecked));
    }

    /// <inheritdoc/>
    public override bool IsChecked
    {
      get
      {
        return Controller is null ? false : _graphToolType == Controller.CurrentGraphTool;
      }
      set
      {
        if (value == true && Controller is not null)
        {
          Controller.CurrentGraphTool = _graphToolType;
        }
        OnPropertyChanged("IsChecked");
      }
    }
  }

  /// <summary>
  /// Selects the object pointer tool.
  /// </summary>
  public class SelectPointerTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectPointerTool"/> class.
    /// </summary>
    public SelectPointerTool()
      : base(GraphToolType.ObjectPointer)
    {
    }
  }

  /// <summary>
  /// Selects the text drawing tool.
  /// </summary>
  public class SelectTextTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTextTool"/> class.
    /// </summary>
    public SelectTextTool()
      : base(GraphToolType.TextDrawing)
    {
    }
  }

  /// <summary>
  /// Tool for reading the x-y scatter values of a data point.
  /// </summary>
  public class ReadPlotItemDataTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadPlotItemDataTool"/> class.
    /// </summary>
    public ReadPlotItemDataTool()
      : base(GraphToolType.ReadPlotItemData)
    {
    }
  }

  /// <summary>Edits the grid of the current layer, or if it has no childs, the grid of the parent layer.</summary>
  public class EditGridTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EditGridTool"/> class.
    /// </summary>
    public EditGridTool()
      : base(GraphToolType.EditGrid)
    {
    }
  }

  /// <summary>
  /// Tool for reading the x-y coordinate values of a layer.
  /// </summary>
  public class ReadXYCoordinatesTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadXYCoordinatesTool"/> class.
    /// </summary>
    public ReadXYCoordinatesTool()
      : base(GraphToolType.ReadXYCoordinates)
    {
    }
  }

  /// <summary>
  /// Tool for evaluating two points on a curve.
  /// </summary>
  public class TwoPointsOnCurveTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TwoPointsOnCurveTool"/> class.
    /// </summary>
    public TwoPointsOnCurveTool()
      : base(GraphToolType.TwoPointsOnCurve)
    {
    }
  }

  /// <summary>
  /// Tool for evaluating four points on a curve.
  /// </summary>
  public class FourPointsOnCurveTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointsOnCurveTool"/> class.
    /// </summary>
    public FourPointsOnCurveTool()
      : base(GraphToolType.FourPointsOnCurve)
    {
    }
  }

  /// <summary>
  /// Tool for evaluating a step by four selected points on a curve.
  /// </summary>
  public class FourPointStepEvaluationTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointStepEvaluationTool"/> class.
    /// </summary>
    public FourPointStepEvaluationTool()
      : base(GraphToolType.FourPointStepEvaluation)
    {
    }
  }

  /// <summary>
  /// Tool for evaluating a peak by four selected points on a curve.
  /// </summary>
  public class FourPointPeakEvaluationTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointPeakEvaluationTool"/> class.
    /// </summary>
    public FourPointPeakEvaluationTool()
      : base(GraphToolType.FourPointPeakEvaluation)
    {
    }
  }

  /// <summary>
  /// Drawing a simple line with two points.
  /// </summary>
  public class SingleLineDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleLineDrawingTool"/> class.
    /// </summary>
    public SingleLineDrawingTool()
      : base(GraphToolType.SingleLineDrawing)
    {
    }
  }

  /// <summary>
  /// Draws an arrow line with two points.
  /// </summary>
  public class ArrowLineDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ArrowLineDrawingTool"/> class.
    /// </summary>
    public ArrowLineDrawingTool()
      : base(GraphToolType.ArrowLineDrawing)
    {
    }
  }

  /// <summary>
  /// Drawing a rectangle on the graph.
  /// </summary>
  public class RectangleDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RectangleDrawingTool"/> class.
    /// </summary>
    public RectangleDrawingTool()
      : base(GraphToolType.RectangleDrawing)
    {
    }
  }

  /// <summary>
  /// Draws a curly brace on the graph.
  /// </summary>
  public class CurlyBraceDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CurlyBraceDrawingTool"/> class.
    /// </summary>
    public CurlyBraceDrawingTool()
      : base(GraphToolType.CurlyBraceDrawing)
    {
    }
  }

  /// <summary>
  /// Drawing an ellipse on the graph.
  /// </summary>
  public class EllipseDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EllipseDrawingTool"/> class.
    /// </summary>
    public EllipseDrawingTool()
      : base(GraphToolType.EllipseDrawing)
    {
    }
  }

  /// <summary>
  /// Draws a regular polygon on the graph.
  /// </summary>
  public class RegularPolygonDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RegularPolygonDrawingTool"/> class.
    /// </summary>
    public RegularPolygonDrawingTool()
      : base(GraphToolType.RegularPolygonDrawing)
    {
    }
  }

  /// <summary>
  /// Draws an open cardinal spline on the graph.
  /// </summary>
  public class OpenCardinalSplineDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCardinalSplineDrawingTool"/> class.
    /// </summary>
    public OpenCardinalSplineDrawingTool()
      : base(GraphToolType.OpenCardinalSplineDrawing)
    {
    }
  }

  /// <summary>
  /// Draws a closed cardinal spline on the graph.
  /// </summary>
  public class ClosedCardinalSplineDrawingTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedCardinalSplineDrawingTool"/> class.
    /// </summary>
    public ClosedCardinalSplineDrawingTool()
      : base(GraphToolType.ClosedCardinalSplineDrawing)
    {
    }
  }

  /// <summary>
  /// Magnifies the axes according to the selected area.
  /// </summary>
  public class ZoomAxesTool : AbstractGraphToolsCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ZoomAxesTool"/> class.
    /// </summary>
    public ZoomAxesTool()
      : base(GraphToolType.ZoomAxes)
    {
    }
  }
}
