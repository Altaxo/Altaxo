// project created on 2/6/2003 at 11:10 AM
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Drawing;

namespace ICSharpCode.SharpDevelop 
{
	public class ExceptionBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox exceptionTextBox;
		private System.Windows.Forms.CheckBox copyErrorCheckBox;
		private System.Windows.Forms.CheckBox includeSysInfoCheckBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button continueButton;
		private System.Windows.Forms.Button reportButton;
		private System.Windows.Forms.PictureBox pictureBox;
		Exception exceptionThrown;
		
		public ExceptionBox(Exception e)
		{
			this.exceptionThrown = e;
			InitializeComponent();
			exceptionTextBox.Text = e.ToString();
			
			ResourceManager resources = new ResourceManager("BitmapResources", Assembly.GetEntryAssembly());
			this.pictureBox.Image = (Bitmap)resources.GetObject("ErrorReport");
		}
		
		string getClipboardString()
		{
			string str = "";
			if (includeSysInfoCheckBox.Checked) {
				str  = ".NET Version         : " + Environment.Version.ToString() + Environment.NewLine;
				str += "OS Version           : " + Environment.OSVersion.ToString() + Environment.NewLine;
				str += "Boot Mode            : " + SystemInformation.BootMode + Environment.NewLine;
				str += "Working Set Memory   : " + (Environment.WorkingSet / 1024) + "kb" + Environment.NewLine + Environment.NewLine;
				Version v = Assembly.GetEntryAssembly().GetName().Version;
				str += "SharpDevelop Version : " + v.Major + "." + v.Minor + "." + v.Revision + "." + v.Build + Environment.NewLine;
			}
			
			str += "Exception thrown: " + Environment.NewLine;
			str += exceptionThrown.ToString();
			return str;
		}
		
		void CopyInfoToClipboard()
		{
			if (copyErrorCheckBox.Checked) {
				Clipboard.SetDataObject(new DataObject(System.Windows.Forms.DataFormats.Text, getClipboardString()), true);
			}
		}
		
		// This method is used in the forms designer.
		// Change this method on you own risk
		void buttonClick(object sender, System.EventArgs e)
		{
			CopyInfoToClipboard();
			// open IE via process.start to our bug reporting forum
			Process.Start("http://www.icsharpcode.net/OpenSource/SD/Forum/forum.asp?FORUM_ID=5");
		}
		
		void continueButtonClick(object sender, System.EventArgs e)
		{
//			CopyInfoToClipboard();
			DialogResult = System.Windows.Forms.DialogResult.Ignore;
		}
		
		void InitializeComponent() {
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.reportButton = new System.Windows.Forms.Button();
			this.continueButton = new System.Windows.Forms.Button();
			this.label = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.includeSysInfoCheckBox = new System.Windows.Forms.CheckBox();
			this.copyErrorCheckBox = new System.Windows.Forms.CheckBox();
			this.exceptionTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(224, 464);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// reportButton
			// 
			this.reportButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.reportButton.Location = new System.Drawing.Point(232, 424);
			this.reportButton.Name = "reportButton";
			this.reportButton.Size = new System.Drawing.Size(216, 23);
			this.reportButton.TabIndex = 4;
			this.reportButton.Text = "Report Error to SharpDevelop Team";
			this.reportButton.Click += new System.EventHandler(this.buttonClick);
			// 
			// continueButton
			// 
			this.continueButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.continueButton.Location = new System.Drawing.Point(600, 424);
			this.continueButton.Name = "continueButton";
			this.continueButton.TabIndex = 5;
			this.continueButton.Text = "Continue";
			this.continueButton.Click += new System.EventHandler(this.continueButtonClick);
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(232, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(448, 48);
			this.label.TabIndex = 6;
			this.label.Text = "An unhandled exception has occurred in SharpDevelop. This is unexpected and we\'d " +
"ask you to help us improve SharpDevelop by reporting this error to the SharpDeve" +
"lop team. ";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(232, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(448, 80);
			this.label2.TabIndex = 8;
			this.label2.Text = @"How to report errors efficiently: We have set up a Web-based forum to report and track errors that are reported by users of SharpDevelop. To minimize necessary questions by the team members, in addition to providing the error message that is copied to the clipboard for easier pasting in the error report, we ask that you provide us with an as detailed as possible step-by-step procedure to reproduce this bug. ";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(232, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(448, 23);
			this.label3.TabIndex = 9;
			this.label3.Text = "Thank you for helping make SharpDevelop a better program for everyone!";
			// 
			// includeSysInfoCheckBox
			// 
			this.includeSysInfoCheckBox.Checked = true;
			this.includeSysInfoCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.includeSysInfoCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.includeSysInfoCheckBox.Location = new System.Drawing.Point(232, 392);
			this.includeSysInfoCheckBox.Name = "includeSysInfoCheckBox";
			this.includeSysInfoCheckBox.Size = new System.Drawing.Size(440, 24);
			this.includeSysInfoCheckBox.TabIndex = 3;
			this.includeSysInfoCheckBox.Text = "Include system information (version of Windows, .NET framework)";
			// 
			// copyErrorCheckBox
			// 
			this.copyErrorCheckBox.Checked = true;
			this.copyErrorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.copyErrorCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.copyErrorCheckBox.Location = new System.Drawing.Point(232, 368);
			this.copyErrorCheckBox.Name = "copyErrorCheckBox";
			this.copyErrorCheckBox.Size = new System.Drawing.Size(440, 24);
			this.copyErrorCheckBox.TabIndex = 2;
			this.copyErrorCheckBox.Text = "Copy error message to clipboard";
			// 
			// exceptionTextBox
			// 
			this.exceptionTextBox.Location = new System.Drawing.Point(232, 176);
			this.exceptionTextBox.Multiline = true;
			this.exceptionTextBox.Name = "exceptionTextBox";
			this.exceptionTextBox.ReadOnly = true;
			this.exceptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.exceptionTextBox.Size = new System.Drawing.Size(448, 184);
			this.exceptionTextBox.TabIndex = 1;
			this.exceptionTextBox.Text = "textBoxExceptionText";
			// 
			// ExceptionBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(688, 453);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label);
			this.Controls.Add(this.continueButton);
			this.Controls.Add(this.reportButton);
			this.Controls.Add(this.includeSysInfoCheckBox);
			this.Controls.Add(this.copyErrorCheckBox);
			this.Controls.Add(this.exceptionTextBox);
			this.Controls.Add(this.pictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExceptionBox";
			this.Text = "Unhandled exception has occured";
			this.ResumeLayout(false);
		}
	}
}
