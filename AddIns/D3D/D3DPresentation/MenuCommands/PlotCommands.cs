using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Worksheet.Commands
{
	public class Plot3D : AbstractWorksheetControllerCommand
	{
		public override void Run(Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl)
		{
			// Problem: here maybe the PresentationCore is not referenced in this assembly, because it was up to now not needed
			// Result: the reflection subsystem will skip this assembly when searching for user controls, because it thinks that we have no dependency
			// on PresentationCore and hence no user controls can exist in this assembly
			// Solution: make sure that the presentation core is referenced, by referencing an arbitrary type in it
			System.Windows.TextAlignment alignment = new System.Windows.TextAlignment();
			string t = alignment.ToString();

			var graph = Graph3D.GraphDocument3DBuilder.CreateNewStandardGraphWithXYZPlotLayer(null);

			var graphController = (Gui.Graph3D.Viewing.Graph3DControllerWpf)Current.Gui.GetControllerAndControl(new object[] { graph }, typeof(Gui.IMVCANController), Gui.UseDocument.Directly);

			if (null == graphController.ViewObject)
				Current.Gui.FindAndAttachControlTo(graphController);

			var viewContent = new Altaxo.Gui.SharpDevelop.SDGraph3DViewContent(graphController);

			if (null != Current.Workbench)
				Current.Workbench.ShowView(viewContent);
		}
	}
}