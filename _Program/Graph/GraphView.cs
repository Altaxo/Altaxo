using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for GraphView.
	/// </summary>
	public class GraphView : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ImageList m_GraphToolsImages;
		private System.Windows.Forms.ImageList m_LayerButtonImages;
		private System.Windows.Forms.ToolBar m_LayerToolbar;
		private Altaxo.Graph.GraphPanel m_GraphPanel;
		private System.ComponentModel.IContainer components;
		private IGraphController m_Ctrl;

		public GraphView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GraphView));
			this.m_GraphToolsImages = new System.Windows.Forms.ImageList(this.components);
			this.m_LayerButtonImages = new System.Windows.Forms.ImageList(this.components);
			this.m_LayerToolbar = new System.Windows.Forms.ToolBar();
			this.m_GraphPanel = new Altaxo.Graph.GraphPanel();
			this.SuspendLayout();
			// 
			// m_GraphToolsImages
			// 
			this.m_GraphToolsImages.ImageSize = new System.Drawing.Size(16, 16);
			this.m_GraphToolsImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_GraphToolsImages.ImageStream")));
			this.m_GraphToolsImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// m_LayerButtonImages
			// 
			this.m_LayerButtonImages.ImageSize = new System.Drawing.Size(1, 1);
			this.m_LayerButtonImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// m_LayerToolbar
			// 
			this.m_LayerToolbar.ButtonSize = new System.Drawing.Size(22, 22);
			this.m_LayerToolbar.Dock = System.Windows.Forms.DockStyle.Left;
			this.m_LayerToolbar.DropDownArrows = true;
			this.m_LayerToolbar.ImageList = this.m_LayerButtonImages;
			this.m_LayerToolbar.Location = new System.Drawing.Point(0, 0);
			this.m_LayerToolbar.Name = "m_LayerToolbar";
			this.m_LayerToolbar.ShowToolTips = true;
			this.m_LayerToolbar.Size = new System.Drawing.Size(22, 266);
			this.m_LayerToolbar.TabIndex = 1;
			this.m_LayerToolbar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.EhLayerToolbar_ButtonClick);
			// 
			// m_GraphPanel
			// 
			this.m_GraphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_GraphPanel.Location = new System.Drawing.Point(22, 0);
			this.m_GraphPanel.Name = "m_GraphPanel";
			this.m_GraphPanel.Size = new System.Drawing.Size(270, 266);
			this.m_GraphPanel.TabIndex = 2;
			this.m_GraphPanel.Click += new System.EventHandler(this.EhGraphPanel_Click);
			this.m_GraphPanel.SizeChanged += new System.EventHandler(this.EhGraphPanel_SizeChanged);
			this.m_GraphPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EhGraphPanel_MouseUp);
			this.m_GraphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.EhGraphPanel_Paint);
			this.m_GraphPanel.DoubleClick += new System.EventHandler(this.EhGraphPanel_DoubleClick);
			this.m_GraphPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EhGraphPanel_MouseMove);
			this.m_GraphPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EhGraphPanel_MouseDown);
			// 
			// GraphView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.m_GraphPanel);
			this.Controls.Add(this.m_LayerToolbar);
			this.Name = "GraphView";
			this.Text = "GraphView";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.EhGraphView_Closing);
			this.Closed += new System.EventHandler(this.EhGraphView_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void EhLayerToolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			int pushedLayerNumber = System.Convert.ToInt32(e.Button.Text);
		
			m_Ctrl.EhView_CurrentLayerChanged(pushedLayerNumber);
		}

		private void EhGraphPanel_Click(object sender, System.EventArgs e)
		{
		m_Ctrl.EhView_GraphPanelMouseClick(e);
		}

		private void EhGraphPanel_DoubleClick(object sender, System.EventArgs e)
		{
		m_Ctrl.EhView_GraphPanelMouseDoubleClick(e);
		}

		private void EhGraphPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		m_Ctrl.EhView_GraphPanelPaint(e);
		}

		private void EhGraphPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		m_Ctrl.EhView_GraphPanelMouseDown(e);
		}

		private void EhGraphPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		m_Ctrl.EhView_GraphPanelMouseMove(e);
		}

		private void EhGraphPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		m_Ctrl.EhView_GraphPanelMouseUp(e);
		}

		private void EhGraphPanel_SizeChanged(object sender, System.EventArgs e)
		{
		m_Ctrl.EhView_GraphPanelSizeChanged(e);
		}

		private void EhGraphView_Closed(object sender, System.EventArgs e)
		{
		m_Ctrl.EhView_Closed(e);
		}

		private void EhGraphView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		m_Ctrl.EhView_Closing(e);
		}

	}
}
