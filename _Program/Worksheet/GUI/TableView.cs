using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Serialization;

namespace Altaxo.Worksheet
{
	/// <summary>
	/// TableView is our class for visualizing data tables.
	/// </summary>
	[SerializationSurrogate(0,typeof(TableView.SerializationSurrogate0))]
	[SerializationVersion(0,"Initial version.")]
	public class TableView : System.Windows.Forms.UserControl, ITableView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.VScrollBar m_VertScrollBar;
		private System.Windows.Forms.HScrollBar m_HorzScrollBar;
		private Altaxo.Worksheet.TablePanel m_GridPanel;

		/// <summary>
		/// The controller that controls this view
		/// </summary>
		private ITableController m_Ctrl;


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
				TableView s = (TableView)obj;
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
				TableView frm = new TableView((System.Windows.Forms.Form)parent,null);
				frm.Location = m_Location;
				frm.Size = m_Size;
				frm.Dock = DockStyle.Fill;

				((ITableController)m_Controller).View = frm;

				if(m_Controller is System.Runtime.Serialization.IDeserializationCallback)
				{
					DeserializationFinisher finisher = new DeserializationFinisher(frm);
					((System.Runtime.Serialization.IDeserializationCallback)m_Controller).OnDeserialization(finisher);
				}
				return frm;
			}
		}
		#endregion

		
		public TableView(System.Windows.Forms.Form parent, ITableController ctrl)
		{
			//if(null!=parent)
				//this.MdiParent = parent;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// register event so to be informed when activated
			if(parent is IMdiActivationEventSource)
			{
				((IMdiActivationEventSource)parent).MdiChildDeactivateBefore += new EventHandler(this.EhMdiChildDeactivate);
				((IMdiActivationEventSource)parent).MdiChildActivateAfter += new EventHandler(this.EhMdiChildActivate);
			}
			else if(parent!=null)
			{
				parent.MdiChildActivate += new EventHandler(this.EhMdiChildActivate);
				parent.MdiChildActivate += new EventHandler(this.EhMdiChildDeactivate);
			}

			// Monitor closed and closing events to intervent if neccessary
			parent.Closing += new CancelEventHandler(this.EhClosing);
			parent.Closed += new EventHandler(this.EhClosed);

			// the setting of the graph controller should be left out until the end, since it calls back some functions of
			// the view so that the view should be initialized before
			m_Ctrl = ctrl;
		
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
			this.m_VertScrollBar = new System.Windows.Forms.VScrollBar();
			this.m_HorzScrollBar = new System.Windows.Forms.HScrollBar();
			this.m_GridPanel = new Altaxo.Worksheet.TablePanel();
			this.SuspendLayout();
			// 
			// m_VertScrollBar
			// 
			this.m_VertScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.m_VertScrollBar.Location = new System.Drawing.Point(276, 0);
			this.m_VertScrollBar.Name = "m_VertScrollBar";
			this.m_VertScrollBar.Size = new System.Drawing.Size(16, 266);
			this.m_VertScrollBar.TabIndex = 2;
			this.m_VertScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.EhVertScrollBar_Scroll);
			// 
			// m_HorzScrollBar
			// 
			this.m_HorzScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_HorzScrollBar.Location = new System.Drawing.Point(0, 250);
			this.m_HorzScrollBar.Name = "m_HorzScrollBar";
			this.m_HorzScrollBar.Size = new System.Drawing.Size(276, 16);
			this.m_HorzScrollBar.TabIndex = 3;
			this.m_HorzScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.EhHorzScrollBar_Scroll);
			// 
			// m_GridPanel
			// 
			this.m_GridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_GridPanel.Location = new System.Drawing.Point(0, 0);
			this.m_GridPanel.Name = "m_GridPanel";
			this.m_GridPanel.Size = new System.Drawing.Size(276, 250);
			this.m_GridPanel.TabIndex = 4;
			this.m_GridPanel.Click += new System.EventHandler(this.EhTableArea_Click);
			this.m_GridPanel.SizeChanged += new System.EventHandler(this.EhTableArea_SizeChanged);
			this.m_GridPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseUp);
			this.m_GridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.EhTableArea_Paint);
			this.m_GridPanel.DoubleClick += new System.EventHandler(this.EhTableArea_DoubleClick);
			this.m_GridPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseMove);
			this.m_GridPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EhTableArea_MouseDown);
			// 
			// TableView
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.m_GridPanel);
			this.Controls.Add(this.m_HorzScrollBar);
			this.Controls.Add(this.m_VertScrollBar);
			this.Name = "TableView";
			this.Text = "TableView";
			this.ResumeLayout(false);

		}
		#endregion

		#region Event handlers


		private void EhVertScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_VertScrollBarScroll(e);
		}

		private void EhHorzScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_HorzScrollBarScroll(e);
		}


		private void EhTableArea_Click(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_TableAreaMouseClick(e);
		}

		private void EhTableArea_DoubleClick(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_TableAreaMouseDoubleClick(e);
		}

		private void EhTableArea_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_TableAreaPaint(e);
		}

		private void EhTableArea_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_TableAreaMouseDown(e);
		}

		private void EhTableArea_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_TableAreaMouseMove(e);
		}

		private void EhTableArea_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_TableAreaMouseUp(e);
		}

		private void EhTableArea_SizeChanged(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_TableAreaSizeChanged(e);
		}

		
		protected void EhClosed(object sender, System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_Closed(e);
		}

		protected void EhClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_Closing(e);
		}

	
		protected void EhMdiChildActivate(object sender, EventArgs e)
		{
			/*
			if(((System.Windows.Forms.Form)sender).ActiveMdiChild==this)
			{
				
				// if no toolbar present already, create a toolbar
				if(null==m_GraphToolsToolBar)
					m_GraphToolsToolBar = CreateGraphToolsToolbar();

				// restore the parent - so the toolbar is shown
				m_GraphToolsToolBar.Parent = (System.Windows.Forms.Form)(App.Current);
			
				}
				*/
		}

		protected void EhMdiChildDeactivate(object sender, EventArgs e)
		{
			/*
			if(((System.Windows.Forms.Form)sender).ActiveMdiChild!=this)
			{
				
				if(null!=m_GraphToolsToolBar)
					m_GraphToolsToolBar.Parent=null;
			
				}
		*/
		}
		#endregion

		#region ITableView Members

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control TableViewWindow
		{
			get
			{
				return this;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Form TableViewForm
		{
			get
			{
				return this.ParentForm;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ITableController TableController
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MainMenu TableViewMenu
		{
			set
			{
				if(null!=this.ParentForm)
				{
					if(null!=this.ParentForm.Menu)
						this.ParentForm.Menu.MergeMenu( value ); // do not clone the menu
					else
						this.ParentForm.Menu = value; // do not clone the menu
				}
			}
		}

		public int TableViewHorzScrollMaximum
		{
			get
			{
				return m_HorzScrollBar.Maximum;
			}
			set
			{
				// A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
				// the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
				m_HorzScrollBar.Maximum = value + m_HorzScrollBar.LargeChange-1;
				m_HorzScrollBar.Refresh();
			}
		}

		/// <summary>
		/// Sets the title of this view.
		/// </summary>
		public string TableViewTitle
		{
			set { this.Text = value; }
		}

		public int TableViewVertScrollMaximum
		{
			get
			{
				return m_VertScrollBar.Maximum;
			}
			set
			{
				// A issue in Windows: if I set the maximum to 100 and LargeChange is 10 (default),
				// the ScrollBar scrolls only till 91. To fix this, I added LargeScale-1 to the intended maximum.
				m_VertScrollBar.Maximum = value + m_VertScrollBar.LargeChange-1;
				m_VertScrollBar.Refresh();
			}
		}

		public int TableViewHorzScrollValue
		{
			get
			{
				return m_HorzScrollBar.Value;
			}
			set
			{
				m_HorzScrollBar.Value = value;
			}
		}

		public int TableViewVertScrollValue
		{
			get
			{
				return m_VertScrollBar.Value;
			}
			set
			{
				m_VertScrollBar.Value = value;
			}
		}

		public Graphics TableAreaCreateGraphics()
		{
			return m_GridPanel.CreateGraphics();
		}

		public void TableAreaInvalidate()
		{
			m_GridPanel.Invalidate();
		}


		public Size TableAreaSize
		{
			get
			{
				return m_GridPanel.Size;
			}
		}

		public bool TableAreaCapture
		{
			get { return this.m_GridPanel.Capture; }
			set { this.m_GridPanel.Capture = value; }
		}

		public System.Windows.Forms.Cursor TableAreaCursor
		{
			get { return this.m_GridPanel.Cursor; }
			set { this.m_GridPanel.Cursor = value; }
		}

		#endregion
	}
}
