using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Summary description for SingleValueDialog.
	/// </summary>
	public class SingleValueDialog : System.Windows.Forms.Form, ISingleValueFormView
	{
		protected ISingleValueFormController m_Ctrl;
		private System.Windows.Forms.Label m_Label1;
		private System.Windows.Forms.TextBox m_edEdit;
		private System.Windows.Forms.Button m_btOK;
		private System.Windows.Forms.Button m_btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SingleValueDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public SingleValueDialog(string title, string message)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Text = title;
			this.m_Label1.Text = message;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_Label1 = new System.Windows.Forms.Label();
			this.m_edEdit = new System.Windows.Forms.TextBox();
			this.m_btOK = new System.Windows.Forms.Button();
			this.m_btCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_Label1
			// 
			this.m_Label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_Label1.Location = new System.Drawing.Point(8, 8);
			this.m_Label1.Name = "m_Label1";
			this.m_Label1.Size = new System.Drawing.Size(344, 16);
			this.m_Label1.TabIndex = 0;
			this.m_Label1.Text = "Please enter :";
			this.m_Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// m_edEdit
			// 
			this.m_edEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_edEdit.Location = new System.Drawing.Point(8, 32);
			this.m_edEdit.Name = "m_edEdit";
			this.m_edEdit.Size = new System.Drawing.Size(352, 20);
			this.m_edEdit.TabIndex = 1;
			this.m_edEdit.Text = "";
			// 
			// m_btOK
			// 
			this.m_btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_btOK.Location = new System.Drawing.Point(168, 80);
			this.m_btOK.Name = "m_btOK";
			this.m_btOK.TabIndex = 2;
			this.m_btOK.Text = "OK";
			// 
			// m_btCancel
			// 
			this.m_btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_btCancel.Location = new System.Drawing.Point(264, 80);
			this.m_btCancel.Name = "m_btCancel";
			this.m_btCancel.TabIndex = 3;
			this.m_btCancel.Text = "Cancel";
			// 
			// SingleValueDialog
			// 
			this.AcceptButton = this.m_btOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.m_btCancel;
			this.ClientSize = new System.Drawing.Size(368, 106);
			this.Controls.Add(this.m_btCancel);
			this.Controls.Add(this.m_btOK);
			this.Controls.Add(this.m_edEdit);
			this.Controls.Add(this.m_Label1);
			this.MinimumSize = new System.Drawing.Size(376, 136);
			this.Name = "SingleValueDialog";
			this.Text = "Please enter:";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.EhClosing);
			this.ResumeLayout(false);

		}
		#endregion

		private void EhClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=m_Ctrl && this.DialogResult==DialogResult.OK)
			{
				m_Ctrl.EhView_EditBoxValidating(e);
				if(e.Cancel==true)
					this.m_edEdit.Focus();
			}
		}


		#region ISingleValueFormView members
		
		/// <summary>
		/// Returns either the view itself if the view is a form, or the form where this view is contained into, if it is a control or so.
		/// </summary>
		public System.Windows.Forms.Form Form 
		{
			get { return this; }
		}

	
		public ISingleValueFormController Controller
		{
			get { return this.m_Ctrl; }
			set { this.m_Ctrl = value; }
		}

		public string EditBoxContents 
		{
			get { return this.m_edEdit.Text; }
			set { this.m_edEdit.Text = value; }
		}
	
		#endregion


		
	}
}
