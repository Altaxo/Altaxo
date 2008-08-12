using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop;

namespace Altaxo.Gui.Scripting
{
	public class SDScriptOpenedFile : OpenedFile
	{
		List<ICSharpCode.SharpDevelop.Gui.IViewContent> _views = new List<ICSharpCode.SharpDevelop.Gui.IViewContent>();
		
		public SDScriptOpenedFile(string scriptName)
		{
			this.FileName = scriptName;
		}

		public override void RegisterView(ICSharpCode.SharpDevelop.Gui.IViewContent view)
		{
			_views.Add(view);
		}

		public override void UnregisterView(ICSharpCode.SharpDevelop.Gui.IViewContent view)
		{
			_views.Remove(view);
		}

		public override IList<ICSharpCode.SharpDevelop.Gui.IViewContent> RegisteredViewContents
		{
			get { return _views; }
		}

		public override void SaveToDisk()
		{
			// do nothing here
		}
	}
}
