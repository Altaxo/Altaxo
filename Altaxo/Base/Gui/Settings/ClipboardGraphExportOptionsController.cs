using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Settings
{
	using Altaxo.Graph.Gdi;
	using Altaxo.Gui.Graph;

	[ExpectedTypeOfView(typeof(IGraphExportOptionsView))]
	[UserControllerForObject(typeof(GraphExportOptions))]
	public class ClipboardGraphExportOptionsController : GraphExportOptionsController
	{
		public ClipboardGraphExportOptionsController()
		{
			var doc = GraphDocumentClipboardActions.CopyPageOptions.Clone();
			InitializeDocument(doc);
		}

		public override bool Apply()
		{
			var result = base.Apply();

			if (result)
				GraphDocumentClipboardActions.CopyPageOptions = _originalDoc;

			return result;
		}
	}
}