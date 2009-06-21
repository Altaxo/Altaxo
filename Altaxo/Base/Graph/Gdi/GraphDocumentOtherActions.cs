using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	public static class GraphDocumentOtherActions
	{
		public static void ShowRenameDialog(this GraphDocument doc)
		{
			var tvctrl = new Altaxo.Gui.Common.TextValueInputController(doc.Name, "Enter a name for the graph:");
			tvctrl.Validator = new GraphRenameValidator(doc);

			if (Current.Gui.ShowDialog(tvctrl, "Rename graph", false))
				doc.Name = tvctrl.InputText.Trim();
		}

		private class GraphRenameValidator : Altaxo.Gui.Common.TextValueInputController.NonEmptyStringValidator
		{
			GraphDocument _doc;

			public GraphRenameValidator(GraphDocument graphdoc)
				: base("The graph's name must not be empty! Please enter a valid name.")
			{
				_doc = graphdoc;
			}

			public override string Validate(string graphname)
			{
				string err = base.Validate(graphname);
				if (null != err)
					return err;

				if (_doc.Name == graphname)
					return null;
				else if (GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_doc) == null)
					return null; // if there is no parent data set we can enter anything
				else if (GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_doc).Contains(graphname))
					return "This graph name already exists, please choose another name!";
				else
					return null;
			}
		}


		/// <summary>
		/// This command will rescale all axes in all layers
		/// </summary>
		public static void RescaleAxes(this GraphDocument doc)
		{
			for (int i = 0; i < doc.Layers.Count; i++)
			{
				doc.Layers[i].RescaleXAxis();
				doc.Layers[i].RescaleYAxis();
			}
		}


		#region Layer manipulation

		public static bool IsValidLayerNumber(this GraphDocument doc, int layerNumber)
		{
			return layerNumber >= 0 && layerNumber < doc.Layers.Count;
		}


		public static void ShowLayerDialog(this GraphDocument doc, int layerNumber)
		{
			if (IsValidLayerNumber(doc,layerNumber))
				Altaxo.Gui.Graph.LayerController.ShowDialog(doc.Layers[layerNumber]);
		}


		/// <summary>
		/// Deletes the active layer.
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="withGui">If true, a message box will ask the user for approval.</param>
		public static void DeleteLayer(this GraphDocument doc, int layerNumber, bool withGui)
		{
			if (!IsValidLayerNumber(doc,layerNumber))
				return;

			if (withGui && false == Current.Gui.YesNoMessageBox("This will delete the active layer. Are you sure?", "Attention", false))
				return;

			doc.Layers.RemoveAt(layerNumber);
		}

		/// <summary>
		/// Deletes the active layer
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="destposition"></param>
		public static void MoveLayerToPosition(this GraphDocument doc, int sourcePosition, int destposition)
		{
			if (!IsValidLayerNumber(doc, sourcePosition))
				return;

			XYPlotLayer layer = doc.Layers[sourcePosition];

			doc.Layers.RemoveAt(sourcePosition);

			if (destposition > sourcePosition)
				destposition--;
			doc.Layers.Insert(destposition, layer);
		}


		/// <summary>
		/// Deletes the active layer
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="destposition"></param>
		public static void ShowMoveLayerToPositionDialog(this GraphDocument doc, int layerNumber)
		{
			var ivictrl = new Altaxo.Gui.Common.IntegerValueInputController(0, "Please enter the new position (>=0):");
			ivictrl.Validator = new Altaxo.Gui.Common.IntegerValueInputController.ZeroOrPositiveIntegerValidator();
			int newposition;
			if (!Current.Gui.ShowDialog(ivictrl, "New position", false))
				return;

			newposition = ivictrl.EnteredContents;
			MoveLayerToPosition(doc, layerNumber, newposition);
		}



		#endregion

	}
}
