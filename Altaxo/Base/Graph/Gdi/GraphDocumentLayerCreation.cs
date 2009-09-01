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
			XYPlotLayer newlayer = new XYPlotLayer(doc.DefaultLayerPosition, doc.DefaultLayerSize);
			newlayer.CreateDefaultAxes();
			doc.Layers.Add(newlayer);
		}

		/// <summary>
		/// Creates a new layer with top x axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedTopX(this GraphDocument doc, int linklayernumber)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.DefaultLayerPosition, doc.DefaultLayerSize);
			doc.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
			// link the new layer to the last old layer
			newlayer.LinkedLayer = (linklayernumber >= 0 && linklayernumber < doc.Layers.Count) ? doc.Layers[linklayernumber] : null;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
		}

		/// <summary>
		/// Creates a new layer with right y axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedRightY(this GraphDocument doc, int linklayernumber)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.DefaultLayerPosition, doc.DefaultLayerSize);
			doc.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
			// link the new layer to the last old layer
			newlayer.LinkedLayer = (linklayernumber >= 0 && linklayernumber < doc.Layers.Count) ? doc.Layers[linklayernumber] : null;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);

			// set enabling of axis
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));
		}

		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis.
		/// </summary>
		public static void CreateNewLayerLinkedTopXRightY(this GraphDocument doc, int linklayernumber)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.DefaultLayerPosition, doc.DefaultLayerSize);
			doc.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
			// link the new layer to the last old layer
			newlayer.LinkedLayer = (linklayernumber >= 0 && linklayernumber < doc.Layers.Count) ? doc.Layers[linklayernumber] : null;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);

			// set enabling of axis
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));

		}


		/// <summary>
		/// Creates a new layer with bottom x axis and left y axis, which is linked to the same position with top x axis and right y axis. The x axis is linked straight to the x axis of the linked layer.
		/// </summary>
		public static void CreateNewLayerLinkedTopXRightY_XAxisStraight(this GraphDocument doc, int linklayernumber)
		{
			XYPlotLayer newlayer = new XYPlotLayer(doc.DefaultLayerPosition, doc.DefaultLayerSize);
			doc.Layers.Add(newlayer); // it is neccessary to add the new layer this early since we must set some properties relative to the linked layer
			// link the new layer to the last old layer
			var layerLinkedTo = (linklayernumber >= 0 && linklayernumber < doc.Layers.Count) ? doc.Layers[linklayernumber] : null;
			newlayer.LinkedLayer = layerLinkedTo;
			newlayer.SetPosition(0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear, 0, XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear);
			newlayer.SetSize(1, XYPlotLayerSizeType.RelativeToLinkedLayer, 1, XYPlotLayerSizeType.RelativeToLinkedLayer);

			if (null != layerLinkedTo)
			{
				// create a linked x axis of the same type than in the linked layer
				var scaleLinkedTo = layerLinkedTo.Scales.X.Scale;
				var xScale = new Scales.LinkedScale((Scales.Scale)scaleLinkedTo.Clone(), scaleLinkedTo, 0);
				newlayer.Scales.SetScaleWithTicks(0, xScale, (Scales.Ticks.TickSpacing)layerLinkedTo.Scales.X.TickSpacing.Clone());
			}

			// set enabling of axis
			newlayer.AxisStyles.CreateDefault(new CSLineID(0, 1));
			newlayer.AxisStyles.CreateDefault(new CSLineID(1, 1));


		}



		#endregion

	}
}
