// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SharpMessageBox : Form
	{
		Button[] buttons;
		int      retvalue = -1;
		string   header;
		string   text;
		string[] buttontexts;
		
		public SharpMessageBox(string header, string text, params string[] buttontexts)
		{
			this.header = header;
			this.text   = text;
			this.buttontexts = buttontexts;
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			buttons = new Button[buttontexts.Length];
			for (int i = 0; i < buttontexts.Length; ++i) {
				buttons[i] = new Button();
			}
			Label label1  = new Label();
			
			this.SuspendLayout();
			Text = header;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MinimizeBox = false;
			MaximizeBox = false;
			
			Owner         = WorkbenchSingleton.Workbench as Form;
			StartPosition = FormStartPosition.CenterParent;
			
			const int dialogWidth = 320;
			const int buttonWidth = 96;
			
			Icon = null;
			ShowInTaskbar = false;
			
			int v = (dialogWidth - buttontexts.Length * (buttonWidth + 4)) / 2;
			for (int i = 0; i < buttontexts.Length; ++i) {
				buttons[i].Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
				buttons[i].Location = new Point(v + i * (buttonWidth + 4) + dialogWidth / buttonWidth, 50);
				buttons[i].Size = new Size(buttonWidth, 24);
				buttons[i].TabIndex = i;
				buttons[i].Text = buttontexts[i];
				
				buttons[i].Click += new EventHandler(ButtonClick);
			}
			label1.Location = new Point(8, 8);
			label1.Text     = text;
			label1.Size     = new Size(dialogWidth - 8, 50);
			
			
			AcceptButton = buttons[buttons.Length - 1];
			CancelButton = buttons[buttons.Length - 1];
			buttons[buttons.Length - 1].Select();
			
			Controls.AddRange(buttons);
			Controls.Add(label1);
			
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new Size(dialogWidth, 80);
			this.ResumeLayout(false);
		}
		
		void ButtonClick(object sender, EventArgs e)
		{
			for (int i = 0; i < buttons.Length; ++i) {
				if (sender == buttons[i]) {
					retvalue = i;
					break;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		public int ShowMessageBox()
		{
			ShowDialog();
			return retvalue;
		}
	}
}
