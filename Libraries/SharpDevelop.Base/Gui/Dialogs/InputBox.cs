// created on 08.09.2003 at 22:11
using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs {
	
	public class InputBox : BaseSharpDevelopForm
	{
		Label label;
		TextBox textBox;

		public Label Label {
			get {
				return label;
			}
		}
		public TextBox TextBox {
			get {
				return textBox;
			}
		}
		
		public InputBox() : base(System.IO.Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\dialogs\InputBox.xfrm"))
		{
			label = (Label)ControlDictionary["label"];
			textBox = (TextBox)ControlDictionary["textBox"];
		}
	}
}
