#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Serialization;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Graph.GUI
{
  /// <summary>
  /// Summary description for GraphView.
  /// </summary>
  public class GraphView : System.Windows.Forms.UserControl, 
		Altaxo.Gui.Graph.Viewing.IGraphView
  {
    private System.Windows.Forms.ImageList _graphToolsImages;
    private System.Windows.Forms.ImageList _layerButtonImages;
    private System.Windows.Forms.ToolBar _layerToolbar;
    private GraphPanel _graphPanel;
    // private System.Windows.Forms.TextBox m_TextBox;
    private System.ComponentModel.IContainer components;
    private Altaxo.Graph.GUI.WinFormsGraphController _winFormsController;
		private Altaxo.Gui.Graph.Viewing.GraphController _controller;
		
    [Browsable(false)]
    private MainMenu _mainMenu;

    [Browsable(false)]
    private ToolBar _graphToolsToolBar=null;
		private VScrollBar _verticalScrollBar;
		private HScrollBar _horizontalScrollBar;

   

    [Browsable(false)]
    private int        _cachedCurrentLayer = -1;


    public GraphView()
    {
      this.SetStyle(ControlStyles.Selectable,true);
      this.UpdateStyles();

      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

			_winFormsController = new WinFormsGraphController(this);

			
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphView));
			this._graphToolsImages = new System.Windows.Forms.ImageList(this.components);
			this._layerButtonImages = new System.Windows.Forms.ImageList(this.components);
			this._layerToolbar = new System.Windows.Forms.ToolBar();
			this._verticalScrollBar = new System.Windows.Forms.VScrollBar();
			this._horizontalScrollBar = new System.Windows.Forms.HScrollBar();
			this._graphPanel = new Altaxo.Graph.GUI.GraphPanel();
			this.SuspendLayout();
			// 
			// m_GraphToolsImages
			// 
			this._graphToolsImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_GraphToolsImages.ImageStream")));
			this._graphToolsImages.TransparentColor = System.Drawing.Color.Transparent;
			this._graphToolsImages.Images.SetKeyName(0, "");
			this._graphToolsImages.Images.SetKeyName(1, "");
			// 
			// m_LayerButtonImages
			// 
			this._layerButtonImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._layerButtonImages.ImageSize = new System.Drawing.Size(1, 1);
			this._layerButtonImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// m_LayerToolbar
			// 
			this._layerToolbar.ButtonSize = new System.Drawing.Size(22, 22);
			this._layerToolbar.Dock = System.Windows.Forms.DockStyle.Left;
			this._layerToolbar.DropDownArrows = true;
			this._layerToolbar.ImageList = this._layerButtonImages;
			this._layerToolbar.Location = new System.Drawing.Point(0, 0);
			this._layerToolbar.Name = "m_LayerToolbar";
			this._layerToolbar.ShowToolTips = true;
			this._layerToolbar.Size = new System.Drawing.Size(22, 311);
			this._layerToolbar.TabIndex = 0;
			this._layerToolbar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.EhLayerToolbar_ButtonClick);
			this._layerToolbar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EhLayerToolbar_MouseDown);
			// 
			// _verticalScrollBar
			// 
			this._verticalScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this._verticalScrollBar.Location = new System.Drawing.Point(317, 0);
			this._verticalScrollBar.Maximum = 1000000;
			this._verticalScrollBar.Name = "_verticalScrollBar";
			this._verticalScrollBar.Size = new System.Drawing.Size(17, 311);
			this._verticalScrollBar.TabIndex = 1;
			this._verticalScrollBar.Visible = false;
			this._verticalScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.EhVerticalScrollBar_Scroll);
			// 
			// _horizontalScrollBar
			// 
			this._horizontalScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this._horizontalScrollBar.Location = new System.Drawing.Point(22, 294);
			this._horizontalScrollBar.Maximum = 1000000;
			this._horizontalScrollBar.Name = "_horizontalScrollBar";
			this._horizontalScrollBar.Size = new System.Drawing.Size(295, 17);
			this._horizontalScrollBar.TabIndex = 2;
			this._horizontalScrollBar.Visible = false;
			this._horizontalScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.EhHorizontalScrollBar_Scroll);
			// 
			// m_GraphPanel
			// 
			this._graphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._graphPanel.Location = new System.Drawing.Point(22, 0);
			this._graphPanel.Name = "m_GraphPanel";
			this._graphPanel.Size = new System.Drawing.Size(312, 311);
			this._graphPanel.TabIndex = 3;
			this._graphPanel.DoubleClick += new System.EventHandler(this.EhGraphPanel_DoubleClick);
			this._graphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.EhGraphPanel_Paint);
			this._graphPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EhGraphPanel_MouseMove);
			this._graphPanel.Click += new System.EventHandler(this.EhGraphPanel_Click);
			this._graphPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EhGraphPanel_MouseDown);
			this._graphPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EhGraphPanel_MouseUp);
			this._graphPanel.SizeChanged += new System.EventHandler(this.EhGraphPanel_SizeChanged);
			// 
			// GraphView
			// 
			this.Controls.Add(this._horizontalScrollBar);
			this.Controls.Add(this._verticalScrollBar);
			this.Controls.Add(this._graphPanel);
			this.Controls.Add(this._layerToolbar);
			this.Name = "GraphView";
			this.Size = new System.Drawing.Size(334, 311);
			this.ResumeLayout(false);
			this.PerformLayout();

    }
    #endregion

  
    
    public Altaxo.Graph.GUI.WinFormsGraphController WinFormsController
    {
      get { return _winFormsController; }
      set { _winFormsController = value; }
    }

		public Altaxo.Gui.Graph.Viewing.GraphController GC
		{
			get
			{
				return _controller;
			}
		}

    /// <summary>
    /// Sets the title of this view.
    /// </summary>
    public string GraphViewTitle
    {
      set { this.Text = value; }
    }


    private void EhLayerToolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
    {
      if(null!=_controller)
      {
        int pushedLayerNumber = System.Convert.ToInt32(e.Button.Text);
    
        _controller.EhView_CurrentLayerChoosen(pushedLayerNumber, false);
      }
    }

    private void EhLayerToolbar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
      {
        if(e.Button == MouseButtons.Right)
        {
          Point pt = new Point(e.X,e.Y);
          for(int i=0;i<_layerToolbar.Buttons.Count;i++)
          {
            if(_layerToolbar.Buttons[i].Rectangle.Contains(pt))
            {
              _controller.EhView_ShowDataContextMenu(i, this, pt);
              return;
            }
          }
        }
      }
    }

    private void EhGraphPanel_Click(object sender, System.EventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_GraphPanelMouseClick(e);
    }

    private void EhGraphPanel_DoubleClick(object sender, System.EventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_GraphPanelMouseDoubleClick(e);
    }

    private void EhGraphPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_GraphPanelPaint(e);
    }

    private void EhGraphPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_GraphPanelMouseDown(e);
    }

    private void EhGraphPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_GraphPanelMouseMove(e);
    }

    private void EhGraphPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if(null!=_winFormsController)
        _winFormsController.EhView_GraphPanelMouseUp(e);
    }

    private void EhGraphPanel_SizeChanged(object sender, System.EventArgs e)
    {
      if(null!=_controller)
        _controller.EhView_GraphPanelSizeChanged();
    }

  

    private void EhGraphToolsToolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
    {
      if(null!=_winFormsController)
        throw new NotImplementedException("This is not implemented any longer");
    }


    public void OnViewSelection()
    {

    }

    

    public void OnViewDeselection()
    {
      if(null!=_graphToolsToolBar)
        _graphToolsToolBar.Parent=null;
    }

  

    #region IGraphView Members
 
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MainMenu GraphMenu
    {
      set
      {
        _mainMenu = value;
      }
    }

    /// <summary>
    /// This creates a graphics context for the graph.
    /// </summary>
    /// <returns>The graphics context.</returns>
    public Graphics CreateGraphGraphics()
    {
      return this._graphPanel.CreateGraphics();
    }


    public void InvalidateGraph()
    {
      this._graphPanel.Invalidate();
    }



		/// <summary>
		/// Get / sets the AutoScroll size property 
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowGraphScrollBars
		{
			set
			{
				this._horizontalScrollBar.Visible = value;
				this._verticalScrollBar.Visible = value;
			}
		}

    /// <summary>
    /// Get /sets the scroll position of the graph
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PointF GraphScrollPosition 
    { 
      get
      {
				return new PointF(_horizontalScrollBar.Value/(float)_horizontalScrollBar.Maximum, _verticalScrollBar.Value/(float)_verticalScrollBar.Maximum);
      }
      set
      {
				var controller = _winFormsController;
				_winFormsController = null; // suppress scrollbar events

				this._horizontalScrollBar.Value = (int)(value.X*_horizontalScrollBar.Maximum);
				this._verticalScrollBar.Value = (int)(value.Y * _verticalScrollBar.Maximum);

				_winFormsController = controller;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Size GraphSize
    {
      get
      {
        return this._graphPanel.Size;
      }
    }
   
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentLayer
    {
      set
      {
        _cachedCurrentLayer = value;
        for(int i=0;i<_layerToolbar.Buttons.Count;i++)
          _layerToolbar.Buttons[i].Pushed = (i==_cachedCurrentLayer);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int NumberOfLayers
    {
      set
      {
        int nNumButtons = _layerToolbar.Buttons.Count;

        if(value > nNumButtons)
        {
					for (int i = nNumButtons; i < value; i++)
					{
						var newbutton = new ToolBarButton(i.ToString());
						_layerToolbar.Buttons.Add(newbutton);
					}
        }
        else if(nNumButtons > value)
        {
          for(int i=nNumButtons-1;i>=value;i--)
            _layerToolbar.Buttons.RemoveAt(i);
        }

        // now press the currently active layer button
        for(int i=0;i<_layerToolbar.Buttons.Count;i++)
          _layerToolbar.Buttons[i].Pushed = (i==_cachedCurrentLayer);
      }
    }

    #endregion

    /// <summary>
    /// This function is to solve the problem, that after selection of the graph window by
    /// clicking in the graphdoc, the View did not receive KeyPressed messages. 
    /// The cause was that by clicking the graphdoc, the control did not receive focus, because
    /// it was at this moment invisible. By making it explicit visible it now can receive the focus.
    /// </summary>
    public void TakeFocus()
    {
      this.Parent.Show();
      this.Show();
      this.Focus();
     
    }

   
    /// <summary>
    /// Sets the Cursor of the graph panel.
    /// </summary>
    /// <param name="cursor">The cursor showing in the graph panel.</param>
    public void SetPanelCursor(Cursor cursor)
    {
      this._graphPanel.Cursor = cursor;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if(null!=this.WinFormsController)
      {
        if(true==WinFormsController.EhView_ProcessCmdKey(ref msg, keyData))
          return true;
      }
      //      System.Diagnostics.Trace.WriteLine("GraphView CmdKey pressed");
      return base.ProcessCmdKey (ref msg, keyData);
    }

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
		}

		private void EhVerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			if (ScrollEventType.ThumbTrack == e.Type)
				return;

			if (null != _controller)
				_controller.EhView_Scroll();
		}

		private void EhHorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			if (ScrollEventType.ThumbTrack == e.Type)
				return;

			if (null != _controller)
				_controller.EhView_Scroll();
		}


		public void SetGraphToolFromInternal(Altaxo.Gui.Graph.Viewing.GraphToolType value)
		{
			
				_winFormsController.GraphTool = value;
				if (null != _controller)
					_controller.EhView_CurrentGraphToolChanged();
		}

		public void SetActiveLayerFromInternal(int layerNumber)
		{
			_controller.EhView_CurrentLayerChoosen(layerNumber, false);
		}


		#region IGraphView Members

		public Altaxo.Graph.Gdi.GraphDocument Doc
		{
			get
			{
				return _controller != null ? _controller.Doc : null;
			}
		}

		Altaxo.Gui.Graph.Viewing.IGraphViewEventSink Altaxo.Gui.Graph.Viewing.IGraphView.Controller
		{
			set
			{
				_controller = value as Altaxo.Gui.Graph.Viewing.GraphController;
			}
		}


		/// <summary>
		/// Triggers a full redrawing of the graph. If the image is cached, the cache is invalidated.
		/// </summary>
		void Altaxo.Gui.Graph.Viewing.IGraphView.RefreshGraph()
		{
			_winFormsController.RefreshGraph();
		}

	
		public SizeF ViewportSizeInInch
		{
			get
			{
				float dpix, dpiy;
				using (Graphics grfx = _graphPanel.CreateGraphics())
				{
					dpix = grfx.DpiX;
					dpiy = grfx.DpiY;
				}

				return new SizeF(_graphPanel.Width/dpix, _graphPanel.Height/dpiy);
			}
		}

		public Altaxo.Graph.Gdi.XYPlotLayer ActiveLayer
		{
			get
			{
				return _controller.ActiveLayer;
			}
		}

	

		public IList<IHitTestObject> SelectedObjects
		{
			get
			{
				return _winFormsController.SelectedObjects;
			}
		}

	


		Altaxo.Gui.Graph.Viewing.GraphToolType Altaxo.Gui.Graph.Viewing.IGraphView.GraphTool
		{
			get
			{
				return _winFormsController.GraphTool;
			}
			set
			{
				_winFormsController.GraphTool = value;
			}
		}

		#endregion




	}
}
  

