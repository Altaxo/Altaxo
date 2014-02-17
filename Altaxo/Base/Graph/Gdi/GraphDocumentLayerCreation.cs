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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	public static class GraphDocumentLayerCreation
	{
		#region XYPlotLayer Creation

		private static XYPlotLayer CreateNewLayerAtSamePosition(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			HostLayer oldLayer;
			var isValidIndex = doc.RootLayer.IsValidIndex(linklayernumber, out oldLayer);
			if (!isValidIndex)
				throw new ArgumentOutOfRangeException("index was not valid");

			IItemLocation location;
			if (oldLayer.Location is ItemLocationByGrid)
			{
				location = (IItemLocation)oldLayer.Location.Clone();
			}
			else if (oldLayer.Location is ItemLocationDirect)
			{
				// 1. check if it is possible to create a grid in the parent layer of the old layer
				if (null != oldLayer.ParentLayer && oldLayer.ParentLayer.CanCreateGridForLocation((ItemLocationDirect)oldLayer.Location))
				{
					ItemLocationByGrid gridCell = oldLayer.ParentLayer.CreateGridForLocation((ItemLocationDirect)oldLayer.Location);
					oldLayer.Location = gridCell.Clone();
					location = gridCell.Clone();
				}
				else // if we can not create a grid, then we must set the location to that of the the oldLayer, but of course then the two layer locations are not linked
				{
					location = (IItemLocation)oldLayer.Location.Clone();
				}
			}
			else
			{
				throw new NotImplementedException("Location type not implemented");
			}

			var newLayer = new XYPlotLayer(oldLayer.ParentLayer, location);
			doc.RootLayer.InsertLast(linklayernumber, newLayer);

			return newLayer;
		}

		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is not linked.
		/// </summary>
		public static void CreateNewLayerNormalBottomXLeftY(this GraphDocument doc)
		{
			var context = doc.GetPropertyHierarchy();
			var location = new ItemLocationDirect();
			location.PositionX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.X);
			location.PositionY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativePosition.Y);
			location.SizeX = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.X);
			location.SizeY = RADouble.NewRel(HostLayer.DefaultChildLayerRelativeSize.Y);

			XYPlotLayer newlayer = new XYPlotLayer(doc.RootLayer, location);
			doc.RootLayer.Layers.Add(newlayer);
			newlayer.CreateDefaultAxes(context);
		}

		/// <summary>
		/// Creates a new layer with top x axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedTopX(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			var context = doc.GetPropertyHierarchy();
			var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1), context);
		}

		/// <summary>
		/// Creates a new layer with right y axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedRightY(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			var context = doc.GetPropertyHierarchy();
			var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1), context);
		}

		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedTopXRightY(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			var context = doc.GetPropertyHierarchy();
			var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1), context);
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1), context);
		}

		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis. The x axis is linked straight to the x axis of the linked layer.
		/// </summary>
		public static void CreateNewLayerLinkedTopXRightY_XAxisStraight(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			var context = doc.GetPropertyHierarchy();
			var newlayer = CreateNewLayerAtSamePosition(doc, linklayernumber);

			HostLayer oldLayer;
			var isValidIndex = doc.RootLayer.IsValidIndex(linklayernumber, out oldLayer);
			var linkedLayerAsXYPlotLayer = oldLayer as XYPlotLayer;

			if (null != linkedLayerAsXYPlotLayer)
			{
				// create a linked x axis of the same type than in the linked layer
				var scaleLinkedTo = linkedLayerAsXYPlotLayer.Scales.X.Scale;
				var xScale = new Scales.LinkedScale((Scales.Scale)scaleLinkedTo.Clone(), scaleLinkedTo, 0, linkedLayerAsXYPlotLayer.LayerNumber);
				newlayer.Scales.SetScaleWithTicks(0, xScale, (Scales.Ticks.TickSpacing)linkedLayerAsXYPlotLayer.Scales.X.TickSpacing.Clone());
			}

			// set enabling of axis
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1), context);
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1), context);
		}

		#endregion XYPlotLayer Creation
	}
}