/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;


namespace Altaxo.Graph
{
	/// <summary>
	/// Hosts a graph and handles the painting of the graph and the actions to modify the graph
	/// </summary>
	public class GraphControl : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// This enumeration declares the current choosen tool for the GraphControl
		/// The numeric values have to match the icon positions in the corresponding toolbar
		/// </summary>
		public enum GraphTools 
		{
			/// <summary>The tool to click to the objects, dragging them, and open the object dialogs by doubleclicking on them</summary>
			ObjectPointer=0,
			/// <summary>The tool to write text, i.e. to create ExtendedTextGraphObjects</summary>
			Text=1
		}


		private const double MinimumGridSize = 20;
		private const float UnitPerInch = 72;

		// following default unit is point (1/72 inch)
		
		/// <summary>
		/// overall size of the page (usually the size of the sheet of paper that is selected as printing document)
		/// </summary>
		private RectangleF m_PageBounds = new RectangleF(0, 0, 842, 595);

		
		/// <summary>
		/// the printable area of the document, i.e. the page size minus the margins at each side
		/// </summary>
		private RectangleF m_PrintableBounds = new RectangleF(14, 14, 814 , 567 );

		/// <summary>
		/// Brush to fill the page ground. Since the printable area is filled with another brush, in effect
		/// this brush fills only the non printable margins of the page. 
		/// </summary>
		private BrushHolder m_PageGroundBrush = new BrushHolder(Color.LightGray);

		/// <summary>
		/// Brush to fill the printable area of the graph.
		/// </summary>
		private BrushHolder m_PrintableAreaBrush = new BrushHolder(Color.Snow);


		protected GraphDocument m_Graph=new GraphDocument();

		private float m_HorizRes  = 300;
		private float m_VertRes = 300;
		private Color m_NonPrintingAreaColor = Color.Gray;
		private int m_MarginLineWidth = 1;
		private Color m_MarginColor = Color.Green;
		private float m_Zoom  = 0.4f;
		private bool  m_AutoZoom = true; // if true, the sheet is zoomed as big as possible to fit into window
		protected int m_ActualLayer = 0;
		protected GraphTools m_CurrentGraphTool = GraphTools.ObjectPointer;
		private GraphPanel m_GraphPanel;
//		protected PointF m_LastMouseDownPoint;
		private System.Windows.Forms.ImageList m_GraphToolsImages;
		private System.ComponentModel.IContainer components;

		private ToolBar m_GraphToolsToolBar=null;
		private MouseStateHandler m_MouseState= new ObjectPointerMouseHandler();


		/// <summary>
		/// The hashtable of the selected objects. The key is the selected object itself,
		/// the data is a int object, which stores the layer number the object belongs to.
		/// </summary>
		protected System.Collections.Hashtable m_SelectedObjects = new System.Collections.Hashtable();

		/// <summary>
		/// This holds a frozen image of the graph during the moving time
		/// </summary>
		protected Bitmap m_FrozenGraph=null;


		public GraphControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.ResizeRedraw=true;

			// Adjust the zoom level just so, that area fits into control
			Graphics grfx = this.CreateGraphics();
			this.m_HorizRes = grfx.DpiX;
			this.m_VertRes = grfx.DpiY;
			grfx.Dispose();

			if(null!=App.CurrentApplication) // if we are at design time, this is null and we use the default values above
			{
				System.Drawing.Printing.PrintDocument doc = App.CurrentApplication.PrintDocument;
			
				RectangleF pageBounds = doc.DefaultPageSettings.Bounds;
				// since Bounds are in 100th inch, we have to adjust them to points (72th inch)
				pageBounds.X *= UnitPerInch/100;
				pageBounds.Y *= UnitPerInch/100;
				pageBounds.Width *= UnitPerInch/100;
				pageBounds.Height *= UnitPerInch/100;

				RectangleF printableBounds = new RectangleF();
				System.Drawing.Printing.Margins ma = doc.DefaultPageSettings.Margins;
				printableBounds.X			= ma.Left * UnitPerInch/100;
				printableBounds.Y			= ma.Top * UnitPerInch/100;
				printableBounds.Width	= m_PageBounds.Width - ((ma.Left+ma.Right)*UnitPerInch/100);
				printableBounds.Height = m_PageBounds.Height - ((ma.Top+ma.Bottom)*UnitPerInch/100);
			
				m_Graph.PageBounds = pageBounds;
				m_Graph.PrintableBounds = printableBounds;
			}

			m_Graph.Layers.Add(new Altaxo.Graph.Layer(PrintableSize));
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GraphControl));
			this.m_GraphPanel = new Altaxo.Graph.GraphPanel();
			this.m_GraphToolsImages = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// m_GraphPanel
			// 
			this.m_GraphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_GraphPanel.Name = "m_GraphPanel";
			this.m_GraphPanel.Size = new System.Drawing.Size(150, 150);
			this.m_GraphPanel.TabIndex = 0;
			this.m_GraphPanel.Click += new System.EventHandler(this.OnGraphPanel_Click);
			this.m_GraphPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnGraphPanel_MouseUp);
			this.m_GraphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.AltaxoGraphControl_Paint);
			this.m_GraphPanel.DoubleClick += new System.EventHandler(this.OnGraphPanel_DoubleClick);
			this.m_GraphPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnGraphPanel_MouseMove);
			this.m_GraphPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnGraphPanel_MouseDown);
			// 
			// m_GraphToolsImages
			// 
			this.m_GraphToolsImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.m_GraphToolsImages.ImageSize = new System.Drawing.Size(16, 16);
			this.m_GraphToolsImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_GraphToolsImages.ImageStream")));
			this.m_GraphToolsImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// GraphControl
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.m_GraphPanel});
			this.Name = "GraphControl";
			this.ResumeLayout(false);

		}
		#endregion



		public GraphTools CurrentGraphTool
		{
			get { return m_CurrentGraphTool; }
			set 
			{
				m_CurrentGraphTool = value;
				
				if(null!=this.m_GraphToolsToolBar)
				{
					for(int i=0;i<m_GraphToolsToolBar.Buttons.Count;i++)
						m_GraphToolsToolBar.Buttons[i].Pushed = (i==(int)value);
				}

				// select the appropriate mouse handler
				switch(m_CurrentGraphTool)
				{
					case GraphTools.ObjectPointer:
						if(!(m_MouseState is ObjectPointerMouseHandler))
							m_MouseState = new ObjectPointerMouseHandler();
						break;
					case GraphTools.Text:
						if(!(m_MouseState is TextToolMouseHandler))
							m_MouseState = new TextToolMouseHandler();
					break;
				}

			}
		}


		public void OnActivationOfParentForm()
		{
			Console.WriteLine("GraphControl activated");
	
			if(null==m_GraphToolsToolBar)
			{
				// 
				// m_GraphToolsToolBar
				// 
				this.m_GraphToolsToolBar = new ToolBar();
				this.m_GraphToolsToolBar.ImageList = this.m_GraphToolsImages;
				this.m_GraphToolsToolBar.ButtonSize = new System.Drawing.Size(16, 24);
				this.m_GraphToolsToolBar.AutoSize = true;
				//this.m_GraphToolsToolBar.DropDownArrows = true;
				//this.m_GraphToolsToolBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
				//this.m_GraphToolsToolBar.Name = "m_GraphToolsToolBar";
				//this.m_GraphToolsToolBar.ShowToolTips = true;
				//this.m_GraphToolsToolBar.Size = new System.Drawing.Size(16, 24);
				//this.m_GraphToolsToolBar.TabIndex = 1;
				//this.m_GraphToolsToolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
				this.m_GraphToolsToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.m_GraphToolsToolbar_ButtonClick);

				for(int i=0;i<2;i++)
				{
					ToolBarButton tbb = new ToolBarButton();
					tbb.ImageIndex=i;
					//this.m_PushedLayerBotton = tbb;
					//tbb.Style = ToolBarButtonStyle.ToggleButton;
					tbb.Tag = (GraphControl.GraphTools)i; 
					m_GraphToolsToolBar.Buttons.Add(tbb);
				}
				this.m_GraphToolsToolBar.Dock = DockStyle.Bottom;
			}
				
			foreach(ToolBarButton bt in this.m_GraphToolsToolBar.Buttons)
				bt.Pushed = (((GraphTools)bt.Tag)==CurrentGraphTool);

			this.m_GraphToolsToolBar.Parent = (System.Windows.Forms.Form)(App.CurrentApplication);
		}


		private void m_GraphToolsToolbar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// because the buttons should behave like radio buttons, we
			// have to unpush all buttons except the one pushed
			foreach(ToolBarButton bt in this.m_GraphToolsToolBar.Buttons)
				bt.Pushed = (bt==e.Button);

			this.CurrentGraphTool = (GraphTools)e.Button.Tag; 
		}

		public void OnDeactivationOfParentForm()
		{
			Console.WriteLine("GraphControl deactivated");
		
					
			if(null!=m_GraphToolsToolBar)
				m_GraphToolsToolBar.Parent=null;

		}


		public Layer.LayerCollection Layers
		{
			get { return m_Graph.Layers; }
		}

		public int ActualLayer
		{
			get { return m_ActualLayer; }
			set
			{
				if(value<0 || value>=Layers.Count)
					throw new ArgumentOutOfRangeException("ActualLayer",value,"Must between 0 and Layer.Count-1");

				m_ActualLayer = value;
			}
		}


		public RectangleF PageBounds
		{
			get
			{
				return m_Graph.PageBounds;
			}
			set
			{
				m_Graph.PageBounds = value;
				this.InvalidateGraph();
			}
		}

		public virtual RectangleF PrintableBounds
		{
			get
			{
				return m_Graph.PrintableBounds;
			}
			set
			{
				m_Graph.PrintableBounds = value;
				this.InvalidateGraph();
			}
		}

		/// <summary>
		/// The size of the printable area in points (1/72 inch).
		/// </summary>
		public virtual SizeF PrintableSize
		{
			get { return m_Graph.PrintableSize; }
		}

		/// <summary>
		/// Zoom value of the graph view.
		/// </summary>
		public float Zoom
		{
			get
			{
				return m_Zoom;
			}
			set
			{
				if( value > 0.05 )
					m_Zoom = value;
				else
					m_Zoom = 0.05f;
			} 
		}

		/// <summary>
		/// Enables / disable the autozoom feature.
		/// </summary>
		/// <remarks>If autozoom is enables, the zoom factor is calculated depending on the size of the
		/// graph view so that the graph fits best possible inside the view.</remarks>
		public bool AutoZoom
		{
			get
			{
				return this.m_AutoZoom;
			}
			set
			{
				this.m_AutoZoom = value;
				if(this.m_AutoZoom)
				{
					this.m_Zoom = CalculateAutoZoom();
					this.AutoScrollMinSize = new Size(0,0);
					Invalidate();
				}
			}
		}

		
		public float HorizFactorPageToPixel()
		{
			return this.m_HorizRes*this.m_Zoom/UnitPerInch;
		}
		public float VertFactorPageToPixel()
		{
			return this.m_HorizRes*this.m_Zoom/UnitPerInch;
		}

		public PointF PageCoordinatesToPixel(PointF pagec)
		{
			return new PointF(pagec.X*HorizFactorPageToPixel(),pagec.Y*VertFactorPageToPixel());
		}

		public PointF PixelToPageCoordinates(PointF pixelc)
		{
			return new PointF(pixelc.X/HorizFactorPageToPixel(),pixelc.Y/VertFactorPageToPixel());
		}

		/// <summary>
		/// Converts x,y differences in pixels to the corresponding
		/// differences in page coordinates
		/// </summary>
		/// <param name="pixeldiff">X,Y differences in pixel units</param>
		/// <returns>X,Y differences in page coordinates</returns>
		public PointF PixelToPageDifferences(PointF pixeldiff)
		{
			return new PointF(pixeldiff.X/HorizFactorPageToPixel(),pixeldiff.Y/VertFactorPageToPixel());
		}

		/// <summary>
		/// converts from pixel to printable area coordinates
		/// </summary>
		/// <param name="pixelc">pixel coordinates as returned by MouseEvents</param>
		/// <returns>coordinates of the printable area in 1/72 inch</returns>
		public PointF PixelToPrintableAreaCoordinates(PointF pixelc)
		{
			PointF r = PixelToPageCoordinates(pixelc);
			r.X -= this.PrintableBounds.X;
			r.Y -= this.PrintableBounds.Y;
			return r;
		}
		protected virtual void DrawMargins(Graphics g)
		{
			//Rectangle margins = ZoomRectangle(ConvertToPixels(this.m_PrintableBounds));
			RectangleF margins = this.m_PrintableBounds;
			Pen marginPen = new Pen(m_MarginColor);
			marginPen.DashStyle = DashStyle.Dash;
			marginPen.Width = m_MarginLineWidth;
			g.DrawRectangle(marginPen, margins.X,margins.Y,margins.Width,margins.Height);
				
			float infx = -margins.Width/20;
			float infy = -margins.Height/20;
			for(int i=0;i<8;i++)
			{
				margins.Inflate(infx,infy);
				g.DrawRectangle(marginPen, margins.X,margins.Y,margins.Width,margins.Height);
			}	


			System.Console.WriteLine("margins {0},{1}",margins.Width,margins.Height);
		}

		protected virtual float CalculateAutoZoom()
		{
			float zoomh = (UnitPerInch*this.ClientSize.Width/this.m_HorizRes)/this.m_PageBounds.Width;
			float zoomv = (UnitPerInch*this.ClientSize.Height/this.m_VertRes)/this.m_PageBounds.Height;
			return System.Math.Min(zoomh,zoomv);

		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if(m_AutoZoom)
			{
				this.m_Zoom = CalculateAutoZoom();
		
				// System.Console.WriteLine("h={0}, v={1} {3} {4} {5}",zoomh,zoomv,UnitPerInch,this.ClientSize.Width,this.m_HorizRes, this.m_PageBounds.Width);
				// System.Console.WriteLine("SizeX = {0}, zoom = {1}, dpix={2},in={3}",this.ClientSize.Width,this.m_Zoom,this.m_HorizRes,this.ClientSize.Width/(this.m_HorizRes*this.m_Zoom));
			
				this.AutoScrollMinSize= new Size(0,0);
			}
			else
			{
				double pixelh = System.Math.Ceiling(this.m_PageBounds.Width*this.m_HorizRes*this.m_Zoom/(UnitPerInch));
				double pixelv = System.Math.Ceiling(this.m_PageBounds.Height*this.m_VertRes*this.m_Zoom/(UnitPerInch));
				this.AutoScrollMinSize = new Size((int)pixelh,(int)pixelv);
			}
			base.OnSizeChanged(e);

			Invalidate();
		}


		public void OnPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ppea)
		{
			Graphics g = ppea.Graphics;
			DoPaint(g,true);
		}


		private void AltaxoGraphControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			DoPaint(g,false);
		}

		private void DoPaint(Graphics g, bool bForPrinting)
		{
			try
			{
				// g.SmoothingMode = SmoothingMode.AntiAlias;
				// get the dpi settings of the graphics context,
				// for example; 96dpi on screen, 600dpi for the printer
				// used to adjust grid and margin sizing.
				this.m_HorizRes = g.DpiX;
				this.m_VertRes = g.DpiY;

				g.PageUnit = GraphicsUnit.Point;
				
				if(bForPrinting)
					g.PageScale=1;
				else
					g.PageScale = this.m_Zoom;

				float pointsh = UnitPerInch*this.AutoScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
				float pointsv = UnitPerInch*this.AutoScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
				g.TranslateTransform(pointsh,pointsv);

				if(!bForPrinting)
				{
					g.Clear(this.m_NonPrintingAreaColor);
					// Fill the page with its own color
					g.FillRectangle(m_PageGroundBrush,this.m_PageBounds);
					g.FillRectangle(m_PrintableAreaBrush,this.m_PrintableBounds);
					// DrawMargins(g);
				}

				System.Console.WriteLine("Paint with zoom {0}",this.m_Zoom);
				// handle the possibility that the viewport is scrolled,
				// adjust my origin coordintates to compensate
				Point pt = this.AutoScrollPosition;
				

				// Paint the graph now
				g.TranslateTransform(PrintableBounds.X,PrintableBounds.Y); // translate the painting to the printable area
				m_Graph.DoPaint(g,bForPrinting);

				// finally, mark the selected objects
				if(!bForPrinting && m_SelectedObjects.Count>0)
				{
					foreach(GraphObject graphObject in m_SelectedObjects.Keys)
					{
						int nLayer = (int)m_SelectedObjects[graphObject];
						g.DrawPath(Pens.Blue,Layers[nLayer].LayerToGraphCoordinates(graphObject.GetSelectionPath()));
					}
				}
			}
			catch(System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this,ex.ToString());
			}

		}


		/// <summary>
		/// Translates the graphics properties to graph (printable area), so that the beginning of printable area now is (0,0) and the units are in Points (1/72 inch)
		/// </summary>
		/// <param name="g">Graphics to translate</param>
		void TranslateGraphicsToGraphUnits(Graphics g)
		{
			g.PageUnit = GraphicsUnit.Point;
			g.PageScale = this.m_Zoom;

			// the graphics path was returned in printable area ("graph") coordinates
			// thats why we have to shift our coordinate system to printable area coordinates also
			float pointsh = UnitPerInch*this.AutoScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
			float pointsv = UnitPerInch*this.AutoScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
			pointsh += this.m_PrintableBounds.X;
			pointsv += this.m_PrintableBounds.Y; 

			// shift the coordinates to page coordinates
			g.TranslateTransform(pointsh,pointsv);
		}

	

		/// <summary>
		/// Originates a repainting of the graph
		/// </summary>
		public void InvalidateGraph()
		{
			this.m_GraphPanel.Invalidate();
		}


		/// <summary>
		/// Clears the selection list and repaints the graph if neccessary
		/// </summary>
		public void ClearSelections()
		{
			bool bRepaint = (m_SelectedObjects.Count>0); // is a repaint neccessary
			m_SelectedObjects.Clear();
			
			
			this.m_MouseState = new ObjectPointerMouseHandler();

			if(bRepaint)
				InvalidateGraph(); 
		}

		/// <summary>
		/// Determines whether or not the pixel position in <paramref name="pixelPos"/> is on a already selected object
		/// </summary>
		/// <param name="pixelPos">The pixel position to test (on the graph panel)</param>
		/// <param name="foundObject">Returns the object the position <paramref name="pixelPos"/> is pointing to, else null</param>
		/// <returns>True when the pixel position is upon a selected object, else false</returns>
		public bool IsPixelPositionOnAlreadySelectedObject(PointF pixelPos, out GraphObject foundObject)
		{
			PointF graphXY = this.PixelToPrintableAreaCoordinates(pixelPos); // Graph area coordinates

			// have we clicked on one of the already selected objects
			foreach(GraphObject graphObject in m_SelectedObjects.Keys)
			{
				int nLayer = (int)m_SelectedObjects[graphObject];
				if(null!=graphObject.HitTest(Layers[nLayer].GraphToLayerCoordinates(graphXY)))
				{
					foundObject = graphObject;
					return true;
				}
			}
			foundObject = null;
			return false;
		}



		/// <summary>
		/// Looks for a graph object at pixel position <paramref name="pixelPos"/> and returns true if one is found.
		/// </summary>
		/// <param name="pixelPos">The pixel coordinates (graph panel coordinates)</param>
		/// <param name="foundObject">Found object if there is one found, else null</param>
		/// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
		/// <returns>True if a object was found at the pixel coordinates <paramref name="pixelPos"/>, else false.</returns>
		public bool FindGraphObjectAtPixelPosition(PointF pixelPos, out GraphObject foundObject, out int foundInLayerNumber)
		{
			// search for a object first
			PointF mousePT = PixelToPrintableAreaCoordinates(pixelPos);
			GraphicsPath gp;

			for(int nLayer=0;nLayer<Layers.Count;nLayer++)
			{
				Altaxo.Graph.Layer layer = Layers[nLayer];
				foundObject = layer.HitTest(mousePT, out gp);
				if(null!=foundObject)
				{
					foundInLayerNumber = nLayer;
					return true;
				}
			}
			foundObject=null;
			foundInLayerNumber=0;
			return false;
		}

		/// <summary>
		/// Draws immediately a selection rectangle around the object <paramref name="graphObject"/>.
		/// </summary>
		/// <param name="graphObject">The graph object for which a selection rectangle has to be drawn.</param>
		/// <param name="nLayer">The layer number the <paramref name="graphObject"/> belongs to.</param>
		public void DrawSelectionRectangleImmediately(GraphObject graphObject, int nLayer)
		{
			using(Graphics g = m_GraphPanel.CreateGraphics())
			{
				// now translate the graphics to graph units and paint all selection path
				this.TranslateGraphicsToGraphUnits(g);
				g.DrawPath(Pens.Blue,Layers[nLayer].LayerToGraphCoordinates(graphObject.GetSelectionPath())); // draw the selection path
			}		
		}





		private void OnGraphPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = m_MouseState.OnMouseUp(this,e);
		}
		private void OnGraphPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = m_MouseState.OnMouseDown(this,e);
		}
		private void OnGraphPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_MouseState = this.m_MouseState.OnMouseMove(this,e);
		}
		private void OnGraphPanel_Click(object sender, System.EventArgs e)
		{
			m_MouseState = m_MouseState.OnClick(this,e);
		}
		private void OnGraphPanel_DoubleClick(object sender, System.EventArgs e)
		{
			m_MouseState = m_MouseState.OnDoubleClick(this,e);
		}


		#region Mouse Handler Classes

		#region abstract mouse state handler
		/// <summary>
		/// The abstract base class of all MouseStateHandlers.
		/// </summary>
		public abstract class MouseStateHandler
		{
			protected PointF m_LastMouseUp;
			protected PointF m_LastMouseDown;

			public virtual MouseStateHandler OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				return this;
			}
			public virtual MouseStateHandler OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				m_LastMouseUp = new Point(e.X,e.Y);
				return this;
			}
			public virtual MouseStateHandler OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				m_LastMouseDown = new Point(e.X,e.Y);
				return this;
			}
			public virtual MouseStateHandler OnClick(object sender, System.EventArgs e)
			{
				return this;
			}
			public virtual MouseStateHandler OnDoubleClick(object sender, System.EventArgs e)
			{
					return this;
			}
		}
		#endregion // abstract mouse state handler

		#region Object Pointer Mouse Handler
		/// <summary>
		/// Handles the mouse events when the ObjectPointer tools is selected.
		/// </summary>
		public class ObjectPointerMouseHandler : MouseStateHandler
		{
			/// <summary>
			/// If true, the selected objects where moved when a MouseMove event is fired
			/// </summary>
			protected bool m_bMoveObjectsOnMouseMove=false;
			/// <summary>Stores the mouse position of the last point to where the selected objects where moved</summary>
			protected PointF m_MoveObjectsLastMovePoint;
			/// <summary>If objects where really moved during the moving mode, this value become true</summary>
			protected bool m_bObjectsWereMoved=false;


			public override MouseStateHandler OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseMove(sender,e);

				GraphControl grac = sender as GraphControl;

				System.Console.WriteLine("MouseMove {0},{1}",e.X,e.Y);

				PointF mouseDiff = new PointF(e.X - m_MoveObjectsLastMovePoint.X, e.Y - m_MoveObjectsLastMovePoint.Y);
				if(m_bMoveObjectsOnMouseMove && 0!= grac.m_SelectedObjects.Count && (mouseDiff.X!=0 || mouseDiff.Y!=0))
				{
					// move all the selected objects to the new position
					// first update the position of the selected objects to reflect the new position
					m_MoveObjectsLastMovePoint.X = e.X;
					m_MoveObjectsLastMovePoint.Y = e.Y;

					// indicate the objects has moved now
					m_bObjectsWereMoved = true;


					// this difference, which is in mouse coordinates, must first be 
					// converted to Graph coordinates (1/72"), and then transformed for
					// each object to the layer coordinate differences of the layer
		
					PointF graphDiff = grac.PixelToPageDifferences(mouseDiff); // calulate the moving distance in page units = graph units

					foreach(GraphObject graphObject in grac.m_SelectedObjects.Keys)
					{
						// get the layer number the graphObject belongs to
						int nLayer = (int)grac.m_SelectedObjects[graphObject];
						PointF layerDiff = grac.Layers[nLayer].GraphToLayerDifferences(graphDiff); // calculate the moving distance in layer units
						graphObject.X += layerDiff.X;
						graphObject.Y += layerDiff.Y;
						// Console.WriteLine("Moving mdiff={0}, gdiff={1}, ldiff={2}", mouseDiff,graphDiff,layerDiff);
					}
					// now paint the objects on the new position
					if(null!=grac.m_FrozenGraph)
					{
						Graphics g = grac.m_GraphPanel.CreateGraphics();
						// first paint the frozen graph, and upon that, paint all selection rectangles
						g.DrawImageUnscaled(grac.m_FrozenGraph,0,0,grac.m_GraphPanel.Width,grac.m_GraphPanel.Height);

						// now translate the graphics to graph units and paint all selection path
						grac.TranslateGraphicsToGraphUnits(g);
						foreach(GraphObject graphObject in grac.m_SelectedObjects.Keys)
						{
							int nLayer = (int)grac.m_SelectedObjects[graphObject]; 						// get the layer number the graphObject belongs to
							g.DrawPath(Pens.Blue,grac.Layers[nLayer].LayerToGraphCoordinates(graphObject.GetSelectionPath())); // draw the selection path
						}
					}
					else  // if the graph was not frozen before - what reasons ever
					{
						grac.m_GraphPanel.Invalidate(); // rise a normal paint event
					}

				}
				return this;
			}

			/// <summary>
			/// Handles the MouseDown event when the object pointer tool is selected
			/// </summary>
			/// <param name="sender">The sender of the event</param>
			/// <param name="e">The mouse event args</param>
			/// <remarks>
			/// The strategy to handle the mousedown event is as following:
			/// 
			/// Have we clicked on already selected objects?
			///		if yes (we have clicked on already selected objects) and the shift or control key was pressed -> deselect the object and repaint
			///		if yes (we have clicked on already selected objects) and none shift nor control key was pressed-> activate the object moving  mode
			///		if no (we have not clicked on already selected objects) and shift or control key was pressed -> search for the object and add it to the selected objects, then aktivate moving mode
			///		if no (we have not clicked on already selected objects) and no shift or control key pressed -> if a object was found add it to the selected objects and activate moving mode
			///		                                                                                               if no object was found clear the selection list, deactivate moving mode
			/// </remarks>
			public override MouseStateHandler OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseDown(sender, e);

				GraphControl grac = sender as GraphControl;

				if(e.Button != MouseButtons.Left)
					return this; // then there is nothing to do here

				// first, if we have a mousedown without shift key and the
				// position has changed with respect to the last mousedown
				// we have to deselect all objects
				PointF mouseXY = new PointF(e.X,e.Y);
				bool bControlKey=(Keys.Control==(Control.ModifierKeys & Keys.Control)); // Control pressed
				bool bShiftKey=(Keys.Shift==(Control.ModifierKeys & Keys.Shift));
				PointF graphXY = grac.PixelToPrintableAreaCoordinates(mouseXY); // Graph area coordinates

	
				// have we clicked on one of the already selected objects
				GraphObject clickedSelectedObject=null;
				bool bClickedOnAlreadySelectedObjects=grac.IsPixelPositionOnAlreadySelectedObject(mouseXY, out clickedSelectedObject);

				if(bClickedOnAlreadySelectedObjects)
				{
					if(bShiftKey || bControlKey) // if shift or control is pressed, remove the selection
					{
						grac.m_SelectedObjects.Remove(clickedSelectedObject);
						grac.InvalidateGraph(); // repaint the graph
					}
					else // not shift or control pressed -> so activate the object moving mode
					{
						StartMovingObjects(grac,mouseXY);
					}
				} // end if bClickedOnAlreadySelectedObjects
				else // not clicked on a already selected object
				{
					// search for a object first
					GraphObject clickedObject;
					int clickedLayerNumber=0;
					grac.FindGraphObjectAtPixelPosition(mouseXY, out clickedObject, out clickedLayerNumber);

					if(bShiftKey || bControlKey) // if shift or control are pressed, we add the object to the selection list and start moving mode
					{
						if(null!=clickedObject)
						{
							grac.m_SelectedObjects.Add(clickedObject,clickedLayerNumber);
							grac.DrawSelectionRectangleImmediately(clickedObject,clickedLayerNumber);
							StartMovingObjects(grac,mouseXY);
						}
					}
					else // no shift or control key pressed
					{
						if(null!=clickedObject)
						{
							ClearSelections(grac);
							grac.m_SelectedObjects.Add(clickedObject,clickedLayerNumber);
							grac.DrawSelectionRectangleImmediately(clickedObject,clickedLayerNumber);
							StartMovingObjects(grac,mouseXY);
						}
						else // if clicked to nothing 
						{
							ClearSelections(grac); // clear the selection list
						}
					} // end else no shift or control

				} // end else (not cklicked on already selected object)
			return this;
			} // end of function

			public override MouseStateHandler OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
			{
				GraphControl grac = sender as GraphControl;
				System.Console.WriteLine("MouseUp {0},{1}",e.X,e.Y);
				EndMovingObjects(grac);
				return this;
			}

			public override MouseStateHandler OnDoubleClick(object sender, System.EventArgs e)
			{
				base.OnDoubleClick(sender,e);
				GraphControl grac = sender as GraphControl;

				System.Console.WriteLine("DoubleClick!");

				// if there is exactly one object selected, try to open the corresponding configuration dialog
				if(grac.m_SelectedObjects.Count==1)
				{
					IEnumerator graphEnum = grac.m_SelectedObjects.Keys.GetEnumerator(); // get the enumerator
					graphEnum.MoveNext(); // set the enumerator to the first item
					GraphObject graphObject = (GraphObject)graphEnum.Current;
					int nLayer = (int)grac.m_SelectedObjects[graphObject];
					if(graphObject is Graph.ExtendedTextGraphObject)
					{
						TextControlDialog dlg = new TextControlDialog(grac.Layers[nLayer],(ExtendedTextGraphObject)graphObject);
						if(DialogResult.OK==dlg.ShowDialog(grac))
						{
							if(!dlg.TextGraphObject.Empty)
							{
								((ExtendedTextGraphObject)graphObject).CopyFrom(dlg.TextGraphObject);
							}
							else // item is empty, so must be deleted in the layer and in the selectedObjects
							{
								grac.m_SelectedObjects.Remove(graphObject);
								grac.Layers[nLayer].GraphObjects.Remove(graphObject);
							}

							grac.InvalidateGraph(); // repaint the graph
						}
					}
				}
				return this;
			}


			public override MouseStateHandler OnClick(object sender, System.EventArgs e)
			{

				System.Console.WriteLine("Click");

				return this;
			}


			/// <summary>
			/// Actions neccessary to start the dragging of graph objects
			/// </summary>
			/// <param name="currentMousePosition">the current mouse position in pixel</param>
			protected void StartMovingObjects(GraphControl grac, PointF currentMousePosition)
			{
				if(!m_bMoveObjectsOnMouseMove)
				{
					m_bMoveObjectsOnMouseMove=true;
					m_bObjectsWereMoved=false; // up to now no objects were really moved
					m_MoveObjectsLastMovePoint = currentMousePosition;

					// create a frozen bitmap of the graph
					Graphics g = grac.m_GraphPanel.CreateGraphics(); // do not translate the graphics here!
					grac.m_FrozenGraph = new Bitmap(grac.m_GraphPanel.Width,grac.m_GraphPanel.Height,g);
					Graphics gbmp = Graphics.FromImage(grac.m_FrozenGraph);
					grac.DoPaint(gbmp,false);
					gbmp.Dispose();
				}
			}

			/// <summary>
			/// Actions neccessary to end the dragging of graph objects
			/// </summary>
			protected void EndMovingObjects(GraphControl grac)
			{
				bool bRepaint = m_bObjectsWereMoved; // repaint the graph when objects were really moved

				m_bMoveObjectsOnMouseMove = false;
				m_bObjectsWereMoved=false;
				m_MoveObjectsLastMovePoint = new Point(0,0); // this is not neccessary, but only for "order"
				if(null!=grac.m_FrozenGraph) 
				{
					grac.m_FrozenGraph.Dispose(); grac.m_FrozenGraph=null;
				}
			
				if(bRepaint)
					grac.InvalidateGraph(); // redraw the contents

			}

			/// <summary>
			/// Clears the selection list and repaints the graph if neccessary
			/// </summary>
			public void ClearSelections(GraphControl grac)
			{
				bool bRepaint = (grac.m_SelectedObjects.Count>0); // is a repaint neccessary
				grac.m_SelectedObjects.Clear();
				EndMovingObjects(grac);

				if(bRepaint)
					grac.InvalidateGraph(); 
			}


		} // end of class

		#endregion // object pointer mouse handler

		#region Text tool mouse handler

		/// <summary>
		/// This class handles the mouse events in case the text tool is selected.
		/// </summary>
	public class TextToolMouseHandler : MouseStateHandler
	{
		/// <summary>
		/// Handles the click event by opening the text tool dialog.
		/// </summary>
		/// <param name="sender">The graph control.</param>
		/// <param name="e">EventArgs.</param>
		/// <returns>The mouse state handler for handling the next mouse events.</returns>
		public override MouseStateHandler OnClick(object sender, System.EventArgs e)
		{
			base.OnClick(sender,e);

			GraphControl grac = sender as GraphControl;

			// get the page coordinates (in Point (1/72") units)
			PointF printAreaCoord = grac.PixelToPrintableAreaCoordinates(m_LastMouseDown);
			// with knowledge of the current active layer, calculate the layer coordinates from them
			PointF layerCoord = grac.Layers[grac.ActualLayer].GraphToLayerCoordinates(printAreaCoord);

			ExtendedTextGraphObject tgo = new ExtendedTextGraphObject();
			tgo.Position = layerCoord;

			// deselect the text tool
			grac.CurrentGraphTool = GraphTools.ObjectPointer;

			TextControlDialog dlg = new TextControlDialog(grac.Layers[grac.ActualLayer],tgo);
			if(DialogResult.OK==dlg.ShowDialog(grac))
			{
				// add the resulting textgraphobject to the layer
				if(!dlg.TextGraphObject.Empty)
				{
					grac.Layers[grac.ActualLayer].GraphObjects.Add(dlg.TextGraphObject);
					grac.m_GraphPanel.Invalidate();
				}
			}
			return new ObjectPointerMouseHandler();
		}
	}

		#endregion // Text Tool Mouse Handler

		#endregion // Mouse Handlers


	} // end of class Graph
} // end of namespace
