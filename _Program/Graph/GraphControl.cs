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

		protected LayerList graphLayers = new LayerList();
		protected int m_ActualLayer = 0;

		/// <summary> 
		/// Required designer variable
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GraphControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

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
			// 
			// AltaxoGraphControl
			// 
			this.Name = "AltaxoGraphControl";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.AltaxoGraphControl_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AltaxoGraphControl_MouseDown);

		}
		#endregion


		public LayerList Layer
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

		public class LayerList : System.Collections.ArrayList
		{
			public new  Layer this[int i]
			{
				get { return (Layer)base[i]; }
				set { base[i]=value; }
			}
		}

	} // end of class
} // end of namespace
