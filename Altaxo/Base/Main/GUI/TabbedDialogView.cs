#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// This view is intended to be used as Dialog. It hosts an arbitrary UserControl in its client area,
	/// which provides the user interaction.
	/// The only elements it itself is shown are the 3 buttons OK, Cancel, and Apply.
	/// </summary>
	public class TabbedDialogView : System.Windows.Forms.Form, ITabbedDialogView
	{
		private ITabbedDialogController m_Controller;
		private System.Windows.Forms.Panel m_ButtonPanel;
		private System.Windows.Forms.Button m_btOK;
		private System.Windows.Forms.Button m_btCancel;
		private System.Windows.Forms.Button m_btApply;
		private System.Windows.Forms.TabControl m_TabControl;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TabbedDialogView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


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
			this.m_ButtonPanel = new System.Windows.Forms.Panel();
			this.m_btApply = new System.Windows.Forms.Button();
			this.m_btCancel = new System.Windows.Forms.Button();
			this.m_btOK = new System.Windows.Forms.Button();
			this.m_TabControl = new System.Windows.Forms.TabControl();
			this.m_ButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_ButtonPanel
			// 
			this.m_ButtonPanel.Controls.Add(this.m_btApply);
			this.m_ButtonPanel.Controls.Add(this.m_btCancel);
			this.m_ButtonPanel.Controls.Add(this.m_btOK);
			this.m_ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_ButtonPanel.Location = new System.Drawing.Point(0, 218);
			this.m_ButtonPanel.Name = "m_ButtonPanel";
			this.m_ButtonPanel.Size = new System.Drawing.Size(272, 40);
			this.m_ButtonPanel.TabIndex = 0;
			// 
			// m_btApply
			// 
			this.m_btApply.Location = new System.Drawing.Point(192, 8);
			this.m_btApply.Name = "m_btApply";
			this.m_btApply.Size = new System.Drawing.Size(72, 24);
			this.m_btApply.TabIndex = 2;
			this.m_btApply.Text = "Apply";
			this.m_btApply.Click += new System.EventHandler(this.EhButtonApply_Click);
			// 
			// m_btCancel
			// 
			this.m_btCancel.Location = new System.Drawing.Point(104, 8);
			this.m_btCancel.Name = "m_btCancel";
			this.m_btCancel.Size = new System.Drawing.Size(64, 24);
			this.m_btCancel.TabIndex = 1;
			this.m_btCancel.Text = "Cancel";
			this.m_btCancel.Click += new System.EventHandler(this.EhButtonCancel_Click);
			// 
			// m_btOK
			// 
			this.m_btOK.Location = new System.Drawing.Point(16, 8);
			this.m_btOK.Name = "m_btOK";
			this.m_btOK.Size = new System.Drawing.Size(56, 24);
			this.m_btOK.TabIndex = 0;
			this.m_btOK.Text = "OK";
			this.m_btOK.Click += new System.EventHandler(this.EhButtonOK_Click);
			// 
			// m_TabControl
			// 
			this.m_TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_TabControl.Location = new System.Drawing.Point(4, 8);
			this.m_TabControl.Name = "m_TabControl";
			this.m_TabControl.SelectedIndex = 0;
			this.m_TabControl.Size = new System.Drawing.Size(264, 200);
			this.m_TabControl.TabIndex = 1;
			// 
			// TabbedDialogView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 258);
			this.Controls.Add(this.m_TabControl);
			this.Controls.Add(this.m_ButtonPanel);
			this.Name = "TabbedDialogView";
			this.Text = "TabbedDialogView";
			this.Load += new System.EventHandler(this.EhView_Load);
			this.m_ButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

	
		private void EhButtonOK_Click(object sender, System.EventArgs e)
		{
			if(null!=m_Controller)
				m_Controller.EhOK();
		}

		private void EhButtonCancel_Click(object sender, System.EventArgs e)
		{
			if(null!=m_Controller)
				m_Controller.EhCancel();
		}

		private void EhButtonApply_Click(object sender, System.EventArgs e)
		{
			if(null!=m_Controller)
				m_Controller.EhApply();

		}

		private void EhView_Load(object sender, System.EventArgs e)
		{
			// this.ActiveControl = m_HostedControl;
		}
		#region ITabbedDialogView Members

		public Form Form
		{
			get
			{
				return this;
			}
		}

		public ITabbedDialogController Controller
		{
			get
			{
				return m_Controller;
			}
			set
			{
				m_Controller = value;
			}
		}

		public bool ApplyVisible
		{
			set { this.m_btApply.Visible = value; }
		}

		public string Title
		{
			set { this.Text = value; }
		}


		public void ClearTabs()
		{
			m_TabControl.TabPages.Clear();
		}

		public void AddTab(string title, object view)
		{

			// look if the size of the tab control is enough, otherwise make the dialog bigger
/*
			this.ResumeLayout(true);
			this.m_HostedControl.ResumeLayout(true);
			this.ClientSize = new System.Drawing.Size(m_HostedControl.Size.Width, m_HostedControl.Size.Height + this.m_ButtonPanel.Size.Height);
			this.Controls.Add(hostedControl);
			this.m_HostedControl.ResumeLayout(false);
			this.ResumeLayout(false);
*/
			System.Windows.Forms.TabPage tab = new System.Windows.Forms.TabPage(title);
			System.Windows.Forms.Control cc = (System.Windows.Forms.Control)view;
			


			tab.Controls.Add(cc);
			m_TabControl.TabPages.Add(tab);

			int diffx = Math.Max(0,cc.Width  - m_TabControl.TabPages[0].ClientSize.Width);
			int diffy = Math.Max(0,cc.Height - m_TabControl.TabPages[0].ClientSize.Height);

			if(diffx>0 || diffy>0)
			{
				this.Size = new Size(this.Size.Width+diffx,this.Size.Height+diffy);
			}
		}

		public void FocusTab(int index)
		{
			m_TabControl.TabPages[index].Focus();
		}
		#endregion
	}
}
