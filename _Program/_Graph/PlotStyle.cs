using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for PlotStyle.
	/// </summary>
	public abstract class PlotStyle : ICloneable
	{
	public static Color[] PlotColors = 
			{
				Color.Black,
				Color.Red,
				Color.Green,
				Color.Blue,
				Color.Magenta,
				Color.Yellow,
				Color.Coral
			};


		public static string GetPlotColorName(Color c)
		{
			for(int i=0;i<PlotColors.Length;i++)
			{
				if(c.ToArgb()==PlotColors[i].ToArgb())
				{
					string name = PlotColors[i].ToString();
					return name.Substring(7,name.Length-8);
				}
			}
			return null;
		}


		public static Color GetNextPlotColor(Color c)
		{
			for(int i=0;i<PlotColors.Length;i++)
			{
				// !!!todo!!! more advanced: find the color with the closest match to a plotcolor
				// and use the next color then

				// currently implemented: find the color, if found use the next color
				// if not found, use the first plot color
				if(c.ToArgb()==PlotColors[i].ToArgb())
					return (i+1)<PlotColors.Length ? PlotColors[i+1] : PlotColors[0];
			}

			// default if the color was not found
			return PlotColors[0];
		}

		public abstract object Clone();
		public abstract void Paint(Graphics g, Graph.Layer gl, object plotObject); // plots the curve with the choosen style
		public abstract void PaintSymbol(Graphics g); // draws a symbol that represents the style at position (0,0)

		// public abstract PlotAssociation PlotAssociation	{	get; set; } 


		public abstract void SetToNextStyle(PlotStyle ps, PlotGroupStyle style);

		public abstract System.Drawing.Color Color { get; set; }
		
		public abstract LineStyle			LineStyle { get; set; }
		
		public abstract ScatterStyle		ScatterStyle { get; set; }

		public virtual float SymbolSize 
		{
			get {	return 0; }
			set { }
		}
		public virtual bool LineSymbolGap 
		{
			get { return false; }
			set { }
		}
	} // end of interface PlotStyle


	public class LineScatterPlotStyle : PlotStyle
	{

		protected LineStyle			m_LineStyle;
		protected ScatterStyle	m_ScatterStyle;
		// protected PlotAssociation m_PlotAssociation; // the plot association this plotstyle is for
		protected bool  m_LineSymbolGap;



		public LineScatterPlotStyle(LineScatterPlotStyle from)
		{
			
			this.m_LineStyle				=	null==from.m_LineStyle?null:(LineStyle)from.m_LineStyle.Clone();
			this.m_ScatterStyle			= null==from.m_ScatterStyle?null:(ScatterStyle)from.m_ScatterStyle.Clone();
			// this.m_PlotAssociation	= null; // do not clone the plotassociation!
			this.m_LineSymbolGap    = from.m_LineSymbolGap;
		}

		public LineScatterPlotStyle()
		{
			this.m_LineStyle = new LineStyle();
			this.m_ScatterStyle = new ScatterStyle();
			// this.m_PlotAssociation = null;
			this.m_LineSymbolGap = true;
		}

		public LineScatterPlotStyle(PlotAssociation pa)
		{
			this.m_LineStyle = new LineStyle();
			this.m_ScatterStyle = new ScatterStyle();
			// this.m_PlotAssociation = pa;
			this.m_LineSymbolGap = true;
		}


		public override object Clone()
		{
			return new LineScatterPlotStyle(this);
		}


		public override void SetToNextStyle(PlotStyle pstemplate, PlotGroupStyle style)
		{
			if(0!= (style & PlotGroupStyle.Color))
				this.Color =GetNextPlotColor(pstemplate.Color);
			if(0!= (style & PlotGroupStyle.Line))
				this.LineStyle.SetToNextLineStyle(pstemplate.LineStyle);
			if(0!= (style & PlotGroupStyle.Symbol))
				this.ScatterStyle.SetToNextStyle(pstemplate.ScatterStyle);
		}


		public override System.Drawing.Color Color
		{
			get
			{
				return this.m_LineStyle.PenHolder.Color;
			}
			set
			{
				this.m_LineStyle.PenHolder.Color = value;
				this.m_ScatterStyle.Color = value;
			}
		}
		/*
		public override PlotAssociation PlotAssociation
		{
			get	{	return m_PlotAssociation;	}
			set	
			{	
				PlotAssociation oldPlotAssociation = m_PlotAssociation;
				m_PlotAssociation = value;

				// do not change order of setting, since infinite loops may happen
				if(null!=oldPlotAssociation && !object.ReferenceEquals(oldPlotAssociation,value))
					oldPlotAssociation.PlotStyle = null; // good by my old plot association

				if(null!=m_PlotAssociation && !object.ReferenceEquals(m_PlotAssociation.PlotStyle,this))
					m_PlotAssociation.PlotStyle = this;
			}
		}
*/
		public override LineStyle LineStyle
		{
			get { return m_LineStyle; }
			set 
			{
				if(null==value)
					throw new ArgumentNullException("LineStyle","LineStyle may not be set to null in PlotStyle");

				m_LineStyle = (LineStyle)value.Clone();
			}
		}

		public override ScatterStyle ScatterStyle
		{
			get { return m_ScatterStyle; }
			set 
			{
				if(null==value)
					throw new ArgumentNullException("ScatterStyle","ScatterStyle may not be set to null in PlotStyle");

				m_ScatterStyle = (ScatterStyle)value.Clone();
			}
		}


		public override float SymbolSize 
		{
			get 
			{
				return m_ScatterStyle.SymbolSize;
			}
			set 
			{
				m_ScatterStyle.SymbolSize = value;
			}
		}

		
		public override bool LineSymbolGap 
		{
			get { return m_LineSymbolGap; }
			set { m_LineSymbolGap = value; }
		}



		public override void Paint(Graphics g, Graph.Layer layer, object plotObject)
		{

			if(!(plotObject is PlotAssociation))
				return; // we cannot plot any other than a PlotAssociation now

			PlotAssociation myPlotAssociation = (PlotAssociation)plotObject;

			double layerWidth = layer.Size.Width;
			double layerHeight = layer.Size.Height;


			Altaxo.Data.DoubleColumn xColumn = myPlotAssociation.XColumn as Altaxo.Data.DoubleColumn;
			Altaxo.Data.DoubleColumn yColumn = myPlotAssociation.YColumn as Altaxo.Data.DoubleColumn;

			if(null==xColumn || null==yColumn)
				return; // this plotstyle is only for x and y double columns

			int len = System.Math.Min(xColumn.Count,yColumn.Count);


			

			if(myPlotAssociation.PlottablePoints>0)
			{

				// allocate an array PointF to hold the line points
				PointF[] ptArray = new PointF[len];

				// Fill the array with values
				// only the points where x and y are not NaNs are plotted!

				int i,j;

				bool bInPlotSpace = true;
				int  rangeStart=0;
				PlotRangeList rangeList = new PlotRangeList();

				for(i=0,j=0;i<len;i++)
				{
					if(Double.IsNaN(xColumn[i]) || Double.IsNaN(yColumn[i]))
					{
						if(!bInPlotSpace)
						{
							bInPlotSpace=true;
							rangeList.Add(new PlotRange(rangeStart,j));
						}
						continue;
					}
					

					double x_rel = layer.XAxis.PhysicalToNormal(xColumn[i]);
					double y_rel = layer.YAxis.PhysicalToNormal(yColumn[i]);
					
					// after the conversion to relative coordinates it is possible
					// that with the choosen axis the point is undefined 
					// (for instance negative values on a logarithmic axis)
					// in this case the returned value is NaN
					if(!Double.IsNaN(x_rel) && !Double.IsNaN(y_rel))
					{
						if(bInPlotSpace)
						{
							bInPlotSpace=false;
							rangeStart = j;
						}

						ptArray[j].X = (float)(layerWidth * x_rel);
						ptArray[j].Y = (float)(layerHeight * (1-y_rel));
						j++;
					}
					else
					{
						if(!bInPlotSpace)
						{
							bInPlotSpace=true;
							rangeList.Add(new PlotRange(rangeStart,j));
						}
					}
				} // end for
				if(!bInPlotSpace)
				{
					bInPlotSpace=true;
					rangeList.Add(new PlotRange(rangeStart,j)); // add the last range
				}


				// paint the line style
				if(null!=m_LineStyle)
				{
					m_LineStyle.Paint(g,ptArray,rangeList,layer.Size, m_LineSymbolGap?SymbolSize:0);
				}

				// paint the drop style
				if(null!=m_ScatterStyle && m_ScatterStyle.DropLine!=ScatterStyles.DropLine.NoDrop)
				{
					PenHolder ph = m_ScatterStyle.Pen;
					ph.Cached=true;
					Pen pen = ph.Pen; // do not dispose this pen, since it is cached
					float xe=layer.Size.Width;
					float ye=layer.Size.Height;
					if( (0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Top)) && (0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Bottom)))
					{
						for(j=0;j<ptArray.Length;j++)
						{
							float x = ptArray[j].X;
							g.DrawLine(pen,x,0,x,ye);
						}
					}
					else if( 0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Top))
					{
						for(j=0;j<ptArray.Length;j++)
						{
							float x = ptArray[j].X;
							float y = ptArray[j].Y;
							g.DrawLine(pen,x,0,x,y);
						}
					}
					else if( 0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Bottom))
					{
						for(j=0;j<ptArray.Length;j++)
						{
							float x = ptArray[j].X;
							float y = ptArray[j].Y;
							g.DrawLine(pen,x,y,x,ye);
						}
					}

					if( (0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Left)) && (0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Right)))
					{
						for(j=0;j<ptArray.Length;j++)
						{
							float y = ptArray[j].Y;
							g.DrawLine(pen,0,y,xe,y);
						}
					}
					else if( 0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Right))
					{
						for(j=0;j<ptArray.Length;j++)
						{
							float x = ptArray[j].X;
							float y = ptArray[j].Y;
							g.DrawLine(pen,x,y,xe,y);
						}
					}
					else if( 0!=(m_ScatterStyle.DropLine&ScatterStyles.DropLine.Left))
					{
						for(j=0;j<ptArray.Length;j++)
						{
							float x = ptArray[j].X;
							float y = ptArray[j].Y;
							g.DrawLine(pen,0,y,x,y);
						}
					}
				} // end paint the drop style


				// paint the scatter style
				if(null!=m_ScatterStyle && m_ScatterStyle.Shape!=ScatterStyles.Shape.NoSymbol)
				{
				// save the graphics stat since we have to translate the origin
				System.Drawing.Drawing2D.GraphicsState gs = g.Save();


					float xpos=0, ypos=0;
					float xdiff,ydiff;
					for(j=0;j<ptArray.Length;j++)
					{
						xdiff = ptArray[j].X - xpos;
						ydiff = ptArray[j].Y - ypos;
						xpos = ptArray[j].X;
						ypos = ptArray[j].Y;
						g.TranslateTransform(xdiff,ydiff);
						m_ScatterStyle.Paint(g);
					} // end for

					g.Restore(gs); // Restore the graphics state

				}

			}
		}


		public override void PaintSymbol(Graphics g)
		{
		}
	
	} // end of class LineScatterPlotStyle
}
