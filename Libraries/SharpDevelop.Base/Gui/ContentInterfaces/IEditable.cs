// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface IEditable
	{
		IClipboardHandler ClipboardHandler {
			get;
		}
		
		string Text {
			get;
			set;
		}
		
		void Undo();
		void Redo();
		
	}
}
