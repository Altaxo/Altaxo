using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for LinkAxisControl.
	/// </summary>
	public class AxisLinkControl : System.Windows.Forms.UserControl, IAxisLinkView
	{
		private IAxisLinkController m_Ctrl;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisEndB;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisEndA;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisOrgB;
		private System.Windows.Forms.TextBox m_Layer_edLinkXAxisOrgA;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkXAxisCustom;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkXAxisStraight;
		private System.Windows.Forms.RadioButton m_Layer_rbLinkXAxisNone;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AxisLinkControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label26 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.m_Layer_edLinkXAxisEndB = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkXAxisEndA = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkXAxisOrgB = new System.Windows.Forms.TextBox();
			this.m_Layer_edLinkXAxisOrgA = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.m_Layer_rbLinkXAxisCustom = new System.Windows.Forms.RadioButton();
			this.m_Layer_rbLinkXAxisStraight = new System.Windows.Forms.RadioButton();
			this.m_Layer_rbLinkXAxisNone = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(96, 56);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(16, 8);
			this.label26.TabIndex = 21;
			this.label26.Text = "b";
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(48, 56);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(16, 8);
			this.label25.TabIndex = 20;
			this.label25.Text = "a";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(8, 96);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(24, 16);
			this.label24.TabIndex = 19;
			this.label24.Text = "End";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_Layer_edLinkXAxisEndB
			// 
			this.m_Layer_edLinkXAxisEndB.Location = new System.Drawing.Point(88, 96);
			this.m_Layer_edLinkXAxisEndB.Name = "m_Layer_edLinkXAxisEndB";
			this.m_Layer_edLinkXAxisEndB.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisEndB.TabIndex = 18;
			this.m_Layer_edLinkXAxisEndB.Text = "";
			this.m_Layer_edLinkXAxisEndB.Validating += new System.ComponentModel.CancelEventHandler(this.EhEndB_Validating);
			// 
			// m_Layer_edLinkXAxisEndA
			// 
			this.m_Layer_edLinkXAxisEndA.Location = new System.Drawing.Point(40, 96);
			this.m_Layer_edLinkXAxisEndA.Name = "m_Layer_edLinkXAxisEndA";
			this.m_Layer_edLinkXAxisEndA.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisEndA.TabIndex = 17;
			this.m_Layer_edLinkXAxisEndA.Text = "";
			this.m_Layer_edLinkXAxisEndA.Validating += new System.ComponentModel.CancelEventHandler(this.EhEndA_Validating);
			// 
			// m_Layer_edLinkXAxisOrgB
			// 
			this.m_Layer_edLinkXAxisOrgB.Location = new System.Drawing.Point(88, 72);
			this.m_Layer_edLinkXAxisOrgB.Name = "m_Layer_edLinkXAxisOrgB";
			this.m_Layer_edLinkXAxisOrgB.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisOrgB.TabIndex = 16;
			this.m_Layer_edLinkXAxisOrgB.Text = "";
			this.m_Layer_edLinkXAxisOrgB.Validating += new System.ComponentModel.CancelEventHandler(this.EhOrgB_Validating);
			// 
			// m_Layer_edLinkXAxisOrgA
			// 
			this.m_Layer_edLinkXAxisOrgA.Location = new System.Drawing.Point(40, 72);
			this.m_Layer_edLinkXAxisOrgA.Name = "m_Layer_edLinkXAxisOrgA";
			this.m_Layer_edLinkXAxisOrgA.Size = new System.Drawing.Size(40, 20);
			this.m_Layer_edLinkXAxisOrgA.TabIndex = 15;
			this.m_Layer_edLinkXAxisOrgA.Text = "";
			this.m_Layer_edLinkXAxisOrgA.Validating += new System.ComponentModel.CancelEventHandler(this.EhOrgA_Validating);
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(8, 72);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(24, 16);
			this.label23.TabIndex = 14;
			this.label23.Text = "Org";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// m_Layer_rbLinkXAxisCustom
			// 
			this.m_Layer_rbLinkXAxisCustom.Location = new System.Drawing.Point(16, 32);
			this.m_Layer_rbLinkXAxisCustom.Name = "m_Layer_rbLinkXAxisCustom";
			this.m_Layer_rbLinkXAxisCustom.Size = new System.Drawing.Size(104, 16);
			this.m_Layer_rbLinkXAxisCustom.TabIndex = 13;
			this.m_Layer_rbLinkXAxisCustom.Text = "Custom (a+bx)";
			this.m_Layer_rbLinkXAxisCustom.CheckedChanged += new System.EventHandler(this.EhLinkCustom_CheckedChanged);
			// 
			// m_Layer_rbLinkXAxisStraight
			// 
			this.m_Layer_rbLinkXAxisStraight.Location = new System.Drawing.Point(16, 16);
			this.m_Layer_rbLinkXAxisStraight.Name = "m_Layer_rbLinkXAxisStraight";
			this.m_Layer_rbLinkXAxisStraight.Size = new System.Drawing.Size(104, 16);
			this.m_Layer_rbLinkXAxisStraight.TabIndex = 12;
			this.m_Layer_rbLinkXAxisStraight.Text = "Straight (1:1)";
			this.m_Layer_rbLinkXAxisStraight.CheckedChanged += new System.EventHandler(this.EhLinkStraight_CheckedChanged);
			// 
			// m_Layer_rbLinkXAxisNone
			// 
			this.m_Layer_rbLinkXAxisNone.Location = new System.Drawing.Point(16, 0);
			this.m_Layer_rbLinkXAxisNone.Name = "m_Layer_rbLinkXAxisNone";
			this.m_Layer_rbLinkXAxisNone.Size = new System.Drawing.Size(104, 16);
			this.m_Layer_rbLinkXAxisNone.TabIndex = 11;
			this.m_Layer_rbLinkXAxisNone.Text = "None";
			this.m_Layer_rbLinkXAxisNone.CheckedChanged += new System.EventHandler(this.EhLinkNone_CheckedChanged);
			// 
			// LinkAxisControl
			// 
			this.Controls.Add(this.label26);
			this.Controls.Add(this.label25);
			this.Controls.Add(this.label24);
			this.Controls.Add(this.m_Layer_edLinkXAxisEndB);
			this.Controls.Add(this.m_Layer_edLinkXAxisEndA);
			this.Controls.Add(this.m_Layer_edLinkXAxisOrgB);
			this.Controls.Add(this.m_Layer_edLinkXAxisOrgA);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.m_Layer_rbLinkXAxisCustom);
			this.Controls.Add(this.m_Layer_rbLinkXAxisStraight);
			this.Controls.Add(this.m_Layer_rbLinkXAxisNone);
			this.Name = "LinkAxisControl";
			this.Size = new System.Drawing.Size(128, 120);
			this.ResumeLayout(false);

		}
		#endregion

		private void EhLinkNone_CheckedChanged(object sender, System.EventArgs e)
		{
			if(null!=Controller && ((RadioButton)sender).Checked==true)
				Controller.EhView_LinkTypeChanged(Layer.AxisLinkType.None);
		}

		private void EhLinkStraight_CheckedChanged(object sender, System.EventArgs e)
		{
			if(null!=Controller && ((RadioButton)sender).Checked==true)
				Controller.EhView_LinkTypeChanged(Layer.AxisLinkType.Straight);
		}

		private void EhLinkCustom_CheckedChanged(object sender, System.EventArgs e)
		{
			if(null!=Controller && ((RadioButton)sender).Checked==true)
				Controller.EhView_LinkTypeChanged(Layer.AxisLinkType.Custom);
		}

		private void EhOrgA_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_OrgAValidating(((TextBox)sender).Text,ref bCancel);
				e.Cancel = bCancel;
			}
		}

		private void EhOrgB_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_OrgBValidating(((TextBox)sender).Text,ref bCancel);
				e.Cancel = bCancel;
			}
		
		}

		private void EhEndA_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_EndAValidating(((TextBox)sender).Text,ref bCancel);
				e.Cancel = bCancel;
			}
		}

		private void EhEndB_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=Controller)
			{
				bool bCancel = e.Cancel;
				Controller.EhView_EndBValidating(((TextBox)sender).Text,ref bCancel);
				e.Cancel = bCancel;
			}
		
		}
		#region ILinkAxisView Members

		public IAxisLinkController Controller
		{
			get
			{
				return m_Ctrl;
			}
			set
			{
				m_Ctrl = value;
			}
		}

		public Form Form
		{
			get
			{
				return this.ParentForm;
			}
		}

		void EnableCustom(bool bEnab)
		{
			this.m_Layer_edLinkXAxisOrgA.Enabled = bEnab;
			this.m_Layer_edLinkXAxisOrgB.Enabled = bEnab;
			this.m_Layer_edLinkXAxisEndA.Enabled = bEnab;
			this.m_Layer_edLinkXAxisEndB.Enabled = bEnab;
		}

		public void LinkType_Initialize(Layer.AxisLinkType linktype)
		{
			switch(linktype)
			{
				case Layer.AxisLinkType.None:
					this.m_Layer_rbLinkXAxisNone.Checked = true;
					EnableCustom(false);
					break;
				case Layer.AxisLinkType.Straight:
					this.m_Layer_rbLinkXAxisStraight.Checked = true;
					EnableCustom(false);
					break;
				case Layer.AxisLinkType.Custom:
					this.m_Layer_rbLinkXAxisCustom.Checked = true;
					EnableCustom(true);
					break;
			}
			
		}

		public void OrgA_Initialize(string text)
		{
			this.m_Layer_edLinkXAxisOrgA.Text = text;
		}

		public void OrgB_Initialize(string text)
		{
			this.m_Layer_edLinkXAxisOrgB.Text = text;
		}

		public void EndA_Initialize(string text)
		{
			this.m_Layer_edLinkXAxisEndA.Text = text;
		}

		public void EndB_Initialize(string text)
		{
			this.m_Layer_edLinkXAxisEndB.Text = text;
		}

		#endregion

		#region IMVCView Members

		public object ControllerObject
		{
			get
			{
				return Controller;
			}
			set
			{
				Controller = value as IAxisLinkController;
			}
		}

		#endregion
	}
}
