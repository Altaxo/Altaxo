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
	/// Summary description for AltaxoGraphControl.
	/// </summary>
	public class GraphControl : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// This enumeration declares the current choosen tool for the GraphControl
		/// The numeric values have to match the icon positions in the corresponding toolbar
		/// </summary>
		public enum GraphTools { ObjectPointer=0, Text=1 }


		private const double MinimumGridSize = 20;
		private const float UnitPerInch = 72;

		// following default unit is point (1/72 inch)
		private RectangleF m_PageBounds = new RectangleF(0, 0, 842, 595);
		private RectangleF m_PrintableBounds = new RectangleF(14, 14, 814 , 567 );
		private float m_HorizRes  = 300;
		private float m_VertRes = 300;
		private Color m_NonPrintingAreaColor = Color.Gray;
		private int m_MarginLineWidth = 1;
		private Color m_MarginColor = Color.Green;
		private float m_Zoom  = 0.4f;
		private bool  m_AutoZoom = true; // if true, the sheet is zoomed as big as possible to fit into window
		private BrushHolder m_PageGroundBrush = new BrushHolder(Color.LightGray);
		private BrushHolder m_PrintableAreaBrush = new BrushHolder(Color.Snow);

		protected Layer.LayerCollection graphLayers = new Layer.LayerCollection();
		protected int m_ActualLayer = 0;
		protected GraphTools m_CurrentGraphTool = GraphTools.ObjectPointer;
		private GraphPanel m_GraphPanel;
		protected PointF m_LastMouseDownPoint; // MouseCoordinaates of the last mouse down


		/// <summary> 
		/// Required designer variable
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			
				m_PageBounds = doc.DefaultPageSettings.Bounds;
				// since Bounds are in 100th inch, we have to adjust them to points (72th inch)
				m_PageBounds.X *= UnitPerInch/100;
				m_PageBounds.Y *= UnitPerInch/100;
				m_PageBounds.Width *= UnitPerInch/100;
				m_PageBounds.Height *= UnitPerInch/100;


				System.Drawing.Printing.Margins ma = doc.DefaultPageSettings.Margins;
				m_PrintableBounds.X			= ma.Left * UnitPerInch/100;
				m_PrintableBounds.Y			= ma.Top * UnitPerInch/100;
				m_PrintableBounds.Width	= m_PageBounds.Width - ((ma.Left+ma.Right)*UnitPerInch/100);
				m_PrintableBounds.Height = m_PageBounds.Height - ((ma.Top+ma.Bottom)*UnitPerInch/100);
			}

			graphLayers.Add(new Altaxo.Graph.Layer(PrintableSize));
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
			this.m_GraphPanel = new Altaxo.Graph.GraphPanel();
			this.SuspendLayout();
			// 
			// m_GraphPanel
			// 
			this.m_GraphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_GraphPanel.Name = "m_GraphPanel";
			this.m_GraphPanel.Size = new System.Drawing.Size(150, 150);
			this.m_GraphPanel.TabIndex = 0;
			this.m_GraphPanel.Click += new System.EventHandler(this.OnGraphPanel_Click);
			this.m_GraphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.AltaxoGraphControl_Paint);
			this.m_GraphPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnGraphPanel_MouseDown);
			// 
			// GraphControl
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.m_GraphPanel});
			this.Name = "GraphControl";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AltaxoGraphControl_MouseDown);
			this.ResumeLayout(false);

		}
		#endregion



		public GraphTools CurrentGraphTool
		{
			get { return m_CurrentGraphTool; }
			set 
			{
				m_CurrentGraphTool = value;
			}
		}

		public bool IamOnTheActiveMdiChild
		{
			get { return this.ParentForm==this.ParentForm.ParentForm.ActiveMdiChild; } 
		}


		public void OnActivationOfParentForm()
		{
			Console.WriteLine("GraphControl activated");
		}

		public void OnDeactivationOfParentForm()
		{
			Console.WriteLine("GraphControl deactivated");
		}



		public Layer.LayerCollection Layer
		{
			get { return this.graphLayers; }
		}

		public int ActualLayer
		{
			get { return m_ActualLayer; }
			set
			{
				if(value<0 || value>=graphLayers.Count)
					throw new ArgumentOutOfRangeException("ActualLayer",value,"Must between 0 and Layer.Count-1");

				m_ActualLayer = value;
			}
		}


		public RectangleF PageBounds
		{
			get
			{
				return m_PageBounds;
			}
			set
			{
				m_PageBounds = value;
				this.Invalidate();
			}
		}
		public virtual RectangleF PrintableBounds
		{
			get
			{
				return m_PrintableBounds;
			}
			set
			{
				m_PrintableBounds = value;
			}
		}

		public virtual SizeF PrintableSize
		{
			get { return new SizeF(m_PrintableBounds.Width,m_PrintableBounds.Height); }
		}

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
				
				// layers not deal with page margins, thats why translate coordinates
				// so the printable area starts with (0,0)
				g.TranslateTransform(this.m_PrintableBounds.X,this.m_PrintableBounds.Y);

				int len = graphLayers.Count;
				for(int i=0;i<len;i++)
				{
					((Altaxo.Graph.Layer)graphLayers[i]).Paint(g);
				}
			}
			catch(System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(this,ex.ToString());
			}

		}

		private void AltaxoGraphControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			PointF mousePT = PixelToPageCoordinates(new PointF(e.X,e.Y));
			GraphicsPath gp;

			foreach(Altaxo.Graph.Layer gl in graphLayers)
			{
				gp = gl.HitTest(mousePT);

				if(gp!=null)
				{
					Graphics g = this.CreateGraphics();
					g.PageUnit = GraphicsUnit.Point;
					g.PageScale = this.m_Zoom;
					float pointsh = UnitPerInch*this.AutoScrollPosition.X/(this.m_HorizRes*this.m_Zoom);
					float pointsv = UnitPerInch*this.AutoScrollPosition.Y/(this.m_VertRes*this.m_Zoom);
					g.TranslateTransform(pointsh,pointsv);

					g.DrawPath(Pens.Blue,gp);
				}

			}
		}


		
		
		private void OnGraphPanel_Click(object sender, System.EventArgs e)
		{
		
			if(this.m_CurrentGraphTool==GraphTools.Text)
			{
				// get the page coordinates (in Point (1/72") units)
				PointF pageCoord = PixelToPageCoordinates(m_LastMouseDownPoint);
				// with knowledge of the current active layer, calculate the layer coordinates from them
				PointF layerCoord = Layer[ActualLayer].ToLayerCoordinates(pageCoord);

				ExtendedTextGraphObject tgo = new ExtendedTextGraphObject();
				tgo.Position = layerCoord;

				TextControlDialog dlg = new TextControlDialog(Layer[ActualLayer],tgo);
				if(DialogResult.OK==dlg.ShowDialog(this))
				{
					// add the resulting textgraphobject to the layer
					if(!dlg.TextGraphObject.Empty)
						Layer[ActualLayer].GraphObjects.Add(dlg.TextGraphObject);
				}
			}
		}

		private void OnGraphPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		m_LastMouseDownPoint = new PointF(e.X,e.Y);
		}

		/*
		public class LayerList : System.Collections.ArrayList
		{
			public new  Layer this[int i]
			{
				get { return (Layer)base[i]; }
				set { base[i]=value; }
			}
		}
		*/


	} // end of class
} // end of namespace
