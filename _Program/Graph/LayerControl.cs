using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for LayerControl.
	/// </summary>
	public class LayerControl : System.Windows.Forms.UserControl, ILayerView
	{
		private System.Windows.Forms.ListBox m_lbEdges;
		private System.Windows.Forms.TabControl m_TabCtrl;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LayerControl()
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
			this.m_lbEdges = new System.Windows.Forms.ListBox();
			this.m_TabCtrl = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// m_lbEdges
			// 
			this.m_lbEdges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.m_lbEdges.Location = new System.Drawing.Point(8, 8);
			this.m_lbEdges.Name = "m_lbEdges";
			this.m_lbEdges.Size = new System.Drawing.Size(64, 316);
			this.m_lbEdges.TabIndex = 2;
			// 
			// m_TabCtrl
			// 
			this.m_TabCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_TabCtrl.Location = new System.Drawing.Point(80, 8);
			this.m_TabCtrl.Name = "m_TabCtrl";
			this.m_TabCtrl.SelectedIndex = 0;
			this.m_TabCtrl.Size = new System.Drawing.Size(432, 320);
			this.m_TabCtrl.TabIndex = 3;
			// 
			// LayerControl
			// 
			this.Controls.Add(this.m_TabCtrl);
			this.Controls.Add(this.m_lbEdges);
			this.Name = "LayerControl";
			this.Size = new System.Drawing.Size(520, 336);
			this.ResumeLayout(false);

		}
		#endregion

		#region ILayerView Members

		public ILayerController Controller
		{
			get
			{
				// TODO:  Add LayerControl.Controller getter implementation
				return null;
			}
			set
			{
				// TODO:  Add LayerControl.Controller setter implementation
			}
		}

		public Form Form
		{
			get
			{
				return this.ParentForm;
			}
		}

		public void AddTab(Control window, string name)
		{
			System.Windows.Forms.TabPage tc = new System.Windows.Forms.TabPage(name);
			tc.Controls.Add(window);
			this.m_TabCtrl.Controls.Add( tc );
		}

		#endregion
	}
}
