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

		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is not linked.
		/// </summary>
		public static void CreateNewLayerNormalBottomXLeftY(this GraphDocument doc)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.RootLayer.DefaultChildLayerPosition, doc.RootLayer.DefaultChildLayerSize);
			newlayer.CreateDefaultAxes();
			doc.RootLayer.Layers.Add(newlayer);
		}

		/// <summary>
		/// Creates a new layer with top x axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedTopX(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			HostLayer linkedLayer;
			var isValidIndex = doc.RootLayer.IsValidIndex(linklayernumber, out linkedLayer);

			XYPlotLayer newlayer = new XYPlotLayer(doc.RootLayer.DefaultChildLayerPosition, doc.RootLayer.DefaultChildLayerSize);
			if (isValidIndex)
				doc.RootLayer.InsertLast(linklayernumber, newlayer);
			else
				doc.RootLayer.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer

			// link the new layer to the last old layer
			newlayer.LinkedLayer = linkedLayer as XYPlotLayer;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
		}

		/// <summary>
		/// Creates a new layer with right y axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedRightY(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.RootLayer.DefaultChildLayerPosition, doc.RootLayer.DefaultChildLayerSize);
			HostLayer linkedLayer;
			var isValidIndex = doc.RootLayer.IsValidIndex(linklayernumber, out linkedLayer);
			if (isValidIndex)
				doc.RootLayer.InsertLast(linklayernumber, newlayer);
			else
				doc.RootLayer.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer

			// link the new layer to the last old layer
			newlayer.LinkedLayer = linkedLayer as XYPlotLayer;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);

			// set enabling of axis
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));
		}

		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedTopXRightY(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.RootLayer.DefaultChildLayerPosition, doc.RootLayer.DefaultChildLayerSize);

			HostLayer linkedLayer;
			var isValidIndex = doc.RootLayer.IsValidIndex(linklayernumber, out linkedLayer);
			if (isValidIndex)
				doc.RootLayer.InsertLast(linklayernumber, newlayer);
			else
				doc.RootLayer.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer

			// link the new layer to the last old layer
			newlayer.LinkedLayer = linkedLayer as XYPlotLayer;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);

			// set enabling of axis
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));
		}

		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis. The x axis is linked straight to the x axis of the linked layer.
		/// </summary>
		public static void CreateNewLayerLinkedTopXRightY_XAxisStraight(this GraphDocument doc, IEnumerable<int> linklayernumber)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.RootLayer.DefaultChildLayerPosition, doc.RootLayer.DefaultChildLayerSize);
			HostLayer linkedLayer;
			var isValidIndex = doc.RootLayer.IsValidIndex(linklayernumber, out linkedLayer);
			if (isValidIndex)
				doc.RootLayer.InsertLast(linklayernumber, newlayer);
			else
				doc.RootLayer.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer

			// link the new layer to the last old layer
			var linkedLayerAsXYPlotLayer = linkedLayer as XYPlotLayer;
			newlayer.LinkedLayer = linkedLayer as XYPlotLayer;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);

			if (null != linkedLayerAsXYPlotLayer)
			{
				// create a linked x axis of the same type than in the linked layer
				var scaleLinkedTo = newlayer.LinkedLayer.Scales.X.Scale;
				var xScale = new Scales.LinkedScale((Scales.Scale)scaleLinkedTo.Clone(), scaleLinkedTo, 0, linkedLayerAsXYPlotLayer.LayerNumber);
				newlayer.Scales.SetScaleWithTicks(0, xScale, (Scales.Ticks.TickSpacing)newlayer.LinkedLayer.Scales.X.TickSpacing.Clone());
			}

			// set enabling of axis
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));
		}

		#endregion XYPlotLayer Creation
	}
}