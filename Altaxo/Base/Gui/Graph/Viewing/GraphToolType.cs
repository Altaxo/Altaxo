using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Viewing
{
	public enum GraphToolType
	{
		None,
		ObjectPointer, 
		ArrowLineDrawing,
		SingleLineDrawing,
		RectangleDrawing,
		CurlyBraceDrawing,
		EllipseDrawing,
		TextDrawing,
		ReadPlotItemData,
		ReadXYCoordinates,
		ZoomAxes,
		RegularPolygonDrawing,
		OpenCardinalSplineDrawing,
		ClosedCardinalSplineDrawing
	}
}
