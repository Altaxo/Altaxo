// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.ErrorDialogs
{
	public class LoadingError : System.Windows.Forms.Form 
	{
		private System.ComponentModel.Container components;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label3;
		
		private System.Windows.Forms.Label label1;
		
		public LoadingError(Exception e)
		{
			InitializeComponent();
			textBox1.Text = e.ToString();
			TopMost        = true;
			StartPosition  = FormStartPosition.CenterScreen;
			FormBorderStyle    = FormBorderStyle.FixedDialog;
			MaximizeBox  = MinimizeBox = ControlBox  = false;
			Icon = null;
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			
			button1.Location = new System.Drawing.Point(152, 184);
			button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			button1.Size = new System.Drawing.Size(75, 23);
			button1.TabIndex = 4;
			button1.Text = "OK";
			
			label1.Location = new System.Drawing.Point(8, 8);
			label1.Text = "An error occured while loading Sharp Develop, the execution " + 
				"can\'t be continued, please reinstall";
			label1.Size = new System.Drawing.Size(344, 32);
			label1.TabIndex = 0;
			
			textBox1.Location = new System.Drawing.Point(8, 64);
			textBox1.ReadOnly = true;
			textBox1.Multiline = true;
			textBox1.TabIndex = 3;
			textBox1.Size = new System.Drawing.Size(344, 112);
			
			label3.Location = new System.Drawing.Point(8, 48);
			label3.Text = "Description:";
			label3.Size = new System.Drawing.Size(72, 16);
			label3.TabIndex = 2;
			this.Text = "Loading Error";
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(360, 213);
			
			this.Controls.Add(button1);
			this.Controls.Add(textBox1);
			this.Controls.Add(label3);
			this.Controls.Add(label1);
		}
	}
}
