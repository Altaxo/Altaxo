using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI
{
	/// <summary>
	/// Summary description for GraphView.
	/// </summary>
	[SerializationSurrogate(0,typeof(GraphView.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version.")]
	public class GraphView : System.Windows.Forms.UserControl, IGraphView
	{
		private System.Windows.Forms.ImageList m_GraphToolsImages;
		private System.Windows.Forms.ImageList m_LayerButtonImages;
		private System.Windows.Forms.ToolBar m_LayerToolbar;
		private GraphPanel m_GraphPanel;
		private System.ComponentModel.IContainer components;
		private IGraphController m_Ctrl;

		[Browsable(false)]
		private	MainMenu m_Menu;

		[Browsable(false)]
		private ToolBar m_GraphToolsToolBar=null;

		[Browsable(false)]
		private GraphTools m_CachedCurrentGraphTool = GraphTools.ObjectPointer;

		[Browsable(false)]
		private int        m_CachedCurrentLayer = -1;

		#region Serialization
		public class SerializationSurrogate0 : IDeserializationSubstitute, System.Runtime.Serialization.ISerializationSurrogate, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
		{
			protected Point		m_Location;
			protected Size		m_Size;
			protected object	m_Controller=null;

			// we need a empty constructor
			public SerializationSurrogate0() {}

			// not used for deserialization, since the ISerializable constructor is used for that
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector){return obj;}
			// not used for serialization, instead the ISerializationSurrogate is used for that
			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)	{}

			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				info.SetType(this.GetType());
				GraphView s = (GraphView)obj;
				info.AddValue("Location",s.Location);
				info.AddValue("Size",s.Size);
				info.AddValue("Controller",s.m_Ctrl);
			}

			public SerializationSurrogate0(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
			{
				m_Location = (Point)info.GetValue("Location",typeof(Point));
				m_Size     = (Size)info.GetValue("Size",typeof(Size));
				m_Controller = info.GetValue("Controller",typeof(object));
			}

			public void OnDeserialization(object o)
			{
			}

			public object GetRealObject(object parent)
			{
				// We create the view firstly without controller to have the creation finished
				// before the controler is set
				// otherwise we will have callbacks to not initialized variables
				GraphView frm = new GraphView();
				frm.Location = m_Location;
				frm.Size = m_Size;
			
				((IGraphController)m_Controller).View = frm;

				if(m_Controller is System.Runtime.Serialization.IDeserializationCallback)
				{
					DeserializationFinisher finisher = new DeserializationFinisher(frm);
					((System.Runtime.Serialization.IDeserializationCallback)m_Controller).OnDeserialization(finisher);
				}
				return frm;
			}
		}
		#endregion


		public GraphView()
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GraphView));
			this.m_GraphToolsImages = new System.Windows.Forms.ImageList(this.components);
			this.m_LayerButtonImages = new System.Windows.Forms.ImageList(this.components);
			this.m_LayerToolbar = new System.Windows.Forms.ToolBar();
			this.m_GraphPanel = new GraphPanel();
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
			this.m_LayerToolbar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EhLayerToolbar_MouseDown);
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
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.m_GraphPanel);
			this.Controls.Add(this.m_LayerToolbar);
			this.Name = "GraphView";
			this.Text = "GraphView";
			this.ResumeLayout(false);

		}
		#endregion

	
		
		public IGraphController Controller
		{
			get { return m_Ctrl; }
			set { m_Ctrl = value; }
		}
		
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged (e);

			if(m_Menu != null)
				this.GraphMenu = m_Menu;

			
		}


		private void EhLayerToolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(null!=m_Ctrl)
			{
				int pushedLayerNumber = System.Convert.ToInt32(e.Button.Text);
		
				m_Ctrl.EhView_CurrentLayerChoosen(pushedLayerNumber, false);
			}
		}

		private void EhLayerToolbar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(null!=m_Ctrl)
			{
				if(e.Button == MouseButtons.Right)
				{
					Point pt = new Point(e.X,e.Y);
					for(int i=0;i<m_LayerToolbar.Buttons.Count;i++)
					{
						if(m_LayerToolbar.Buttons[i].Rectangle.Contains(pt))
						{
							m_Ctrl.EhView_ShowDataContextMenu(i,this,pt);
							return;
						}
					}
				}
			}
		}

		private void EhGraphPanel_Click(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_GraphPanelMouseClick(e);
		}

		private void EhGraphPanel_DoubleClick(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_GraphPanelMouseDoubleClick(e);
		}

		private void EhGraphPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_GraphPanelPaint(e);
		}

		private void EhGraphPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_GraphPanelMouseDown(e);
		}

		private void EhGraphPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_GraphPanelMouseMove(e);
		}

		private void EhGraphPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_GraphPanelMouseUp(e);
		}

		private void EhGraphPanel_SizeChanged(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_GraphPanelSizeChanged(e);
		}

	

		private void EhGraphToolsToolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_CurrentGraphToolChoosen((GraphTools)e.Button.Tag);
		}


		public void OnViewSelection()
		{
#if FormerGuiState
			if(Current.Workbench is Altaxo.AltaxoWorkbench)
			{
				if(null==m_GraphToolsToolBar)
					m_GraphToolsToolBar = CreateGraphToolsToolbar();

				// restore the parent - so the toolbar is shown
				// TODO replace this by a service
				if(Current.MainWindow as Form != null)
					m_GraphToolsToolBar.Parent = (Form)Current.MainWindow;
			}
#endif
		}

		

		public void OnViewDeselection()
		{
			if(null!=m_GraphToolsToolBar)
				m_GraphToolsToolBar.Parent=null;
		}

	

		#region IGraphView Members
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control Window
		{
			get
			{
				return this;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Form Form
		{
			get
			{
				return this.ParentForm;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MainMenu GraphMenu
		{
			set
			{
				m_Menu = value;
#if FormerGuiState
				if(this.ParentForm is Altaxo.Main.GUI.WorkbenchForm && null!=m_Menu)
				{
					if(null!=this.ParentForm.Menu)
						this.ParentForm.Menu.MergeMenu( m_Menu ); // do not clone the menu
					else
						this.ParentForm.Menu = m_Menu; // do not clone the menu
				}
#endif
			}
		}

		/// <summary>
		/// This creates a graphics context for the graph.
		/// </summary>
		/// <returns>The graphics context.</returns>
		public Graphics CreateGraphGraphics()
		{
			return this.m_GraphPanel.CreateGraphics();
		}


		public void InvalidateGraph()
		{
			this.m_GraphPanel.Invalidate();
		}



		/// <summary>
		/// Get / sets the AutoScroll size property 
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size GraphScrollSize 
		{
			get
			{
				return this.m_GraphPanel.AutoScrollMinSize;
			}
			set
			{
				this.m_GraphPanel.AutoScrollMinSize = value;
			}
		}

		/// <summary>
		/// Get /sets the scroll position of the graph
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point GraphScrollPosition 
		{ 
			get
			{
				return this.m_GraphPanel.AutoScrollPosition;
			}
			set
			{
				this.m_GraphPanel.AutoScrollPosition = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size GraphSize
		{
			get
			{
				return this.m_GraphPanel.Size;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GraphTools CurrentGraphTool
		{
			set
			{
				m_CachedCurrentGraphTool = value;
				
				if(null!=this.m_GraphToolsToolBar)
				{
					for(int i=0;i<m_GraphToolsToolBar.Buttons.Count;i++)
						m_GraphToolsToolBar.Buttons[i].Pushed = (i==(int)value);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentLayer
		{
			set
			{
				m_CachedCurrentLayer = value;
				for(int i=0;i<m_LayerToolbar.Buttons.Count;i++)
					m_LayerToolbar.Buttons[i].Pushed = (i==m_CachedCurrentLayer);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int NumberOfLayers
		{
			set
			{
				int nNumButtons = m_LayerToolbar.Buttons.Count;

				if(value > nNumButtons)
				{
					for(int i=nNumButtons;i<value;i++)
						m_LayerToolbar.Buttons.Add(new ToolBarButton(i.ToString()));
				}
				else if(nNumButtons > value)
				{
					for(int i=nNumButtons-1;i>=value;i--)
						m_LayerToolbar.Buttons.RemoveAt(i);
				}

				// now press the currently active layer button
				for(int i=0;i<m_LayerToolbar.Buttons.Count;i++)
					m_LayerToolbar.Buttons[i].Pushed = (i==m_CachedCurrentLayer);
			}
		}

		#endregion


		public ToolBar CreateGraphToolsToolbar()
		{
			ToolBar tb = new ToolBar();
			tb.ImageList = this.m_GraphToolsImages;
			tb.ButtonSize = new System.Drawing.Size(16, 24);
			tb.AutoSize = true;
			tb.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.EhGraphToolsToolbar_ButtonClick);

			for(int i=0;i<2;i++)
			{
				ToolBarButton tbb = new ToolBarButton();
				tbb.ImageIndex=i;
				tbb.Tag = (GraphTools)i; 
				tb.Buttons.Add(tbb);
			}
			tb.Dock = DockStyle.Bottom;
	
				
			foreach(ToolBarButton bt in tb.Buttons)
				bt.Pushed = (((GraphTools)bt.Tag) == m_CachedCurrentGraphTool);
		
			return tb;
		}
	}
}
	

