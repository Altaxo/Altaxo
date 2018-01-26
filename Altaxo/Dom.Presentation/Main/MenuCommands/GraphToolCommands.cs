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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Gdi;
using Altaxo.Gui.Graph.Gdi.Viewing;
using Altaxo.Gui.Scripting;
using Altaxo.Gui.Workbench;
using Altaxo.Main;
using Altaxo.Main.Services;
using Altaxo.Scripting;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Input;

namespace Altaxo.Graph.Commands
{
	/// <summary>
	/// This class is intented to be used for commands into the graph tools toolbar. Commands derived
	/// from it will update the toolbar whenever its state changed.
	/// </summary>
	public abstract class AbstractGraphToolsCommand : AbstractCheckableGraphControllerCommand
	{
		private GraphController myCurrentGraphController;
		private GraphToolType _graphToolType;

		protected AbstractGraphToolsCommand(GraphToolType toolType)
		{
			_graphToolType = toolType;
			if (null != Current.Workbench)
			{
				Current.Workbench.PropertyChanged += this.EhWorkbenchContentChanged;
				this.EhWorkbenchContentChanged(this, new PropertyChangedEventArgs(nameof(Current.Workbench.ActiveViewContent)));
			}
		}

		protected void EhWorkbenchContentChanged(object o, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(Current.Workbench.ActiveViewContent))
				return;

			if (!object.ReferenceEquals(Controller, myCurrentGraphController))
			{
				if (null != myCurrentGraphController)
				{
					lock (this)
					{
						this.myCurrentGraphController.CurrentGraphToolChanged -= new EventHandler(this.EhGraphToolChanged);
						this.myCurrentGraphController = null;
					}
				}
				if (Controller != null)
				{
					lock (this)
					{
						this.myCurrentGraphController = this.Controller;
						this.myCurrentGraphController.CurrentGraphToolChanged += new EventHandler(this.EhGraphToolChanged);
					}
				}
				OnPropertyChanged("IsChecked");
			}
		}

		protected void EhGraphToolChanged(object o, EventArgs e)
		{
			OnPropertyChanged(nameof(IsChecked));
		}

		public override bool IsChecked
		{
			get
			{
				return null == Controller ? false : _graphToolType == Controller.CurrentGraphTool;
			}
			set
			{
				if (value == true && Controller != null)
				{
					Controller.CurrentGraphTool = _graphToolType;
				}
				OnPropertyChanged("IsChecked");
			}
		}
	}

	/// <summary>
	/// Test class for a selected item
	/// </summary>
	public class SelectPointerTool : AbstractGraphToolsCommand
	{
		public SelectPointerTool()
			: base(GraphToolType.ObjectPointer)
		{
		}
	}

	/// <summary>
	/// Test class for a selected item
	/// </summary>
	public class SelectTextTool : AbstractGraphToolsCommand
	{
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
		public ReadPlotItemDataTool()
			: base(GraphToolType.ReadPlotItemData)
		{
		}
	}

	/// <summary>Edits the grid of the current layer, or if it has no childs, the grid of the parent layer.</summary>
	public class EditGridTool : AbstractGraphToolsCommand
	{
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
		public ReadXYCoordinatesTool()
			: base(GraphToolType.ReadXYCoordinates)
		{
		}
	}

	/// <summary>
	/// Drawing a simple line with two points.
	/// </summary>
	public class SingleLineDrawingTool : AbstractGraphToolsCommand
	{
		public SingleLineDrawingTool()
			: base(GraphToolType.SingleLineDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing a simple line with two points.
	/// </summary>
	public class ArrowLineDrawingTool : AbstractGraphToolsCommand
	{
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
		public RectangleDrawingTool()
			: base(GraphToolType.RectangleDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing a rectangle on the graph.
	/// </summary>
	public class CurlyBraceDrawingTool : AbstractGraphToolsCommand
	{
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
		public EllipseDrawingTool()
			: base(GraphToolType.EllipseDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing an ellipse on the graph.
	/// </summary>
	public class RegularPolygonDrawingTool : AbstractGraphToolsCommand
	{
		public RegularPolygonDrawingTool()
			: base(GraphToolType.RegularPolygonDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing of an open cardinal spline on a graph.
	/// </summary>
	public class OpenCardinalSplineDrawingTool : AbstractGraphToolsCommand
	{
		public OpenCardinalSplineDrawingTool()
			: base(GraphToolType.OpenCardinalSplineDrawing)
		{
		}
	}

	/// <summary>
	/// Drawing of an closed cardinal spline on a graph.
	/// </summary>
	public class ClosedCardinalSplineDrawingTool : AbstractGraphToolsCommand
	{
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
		public ZoomAxesTool()
			: base(GraphToolType.ZoomAxes)
		{
		}
	}
}