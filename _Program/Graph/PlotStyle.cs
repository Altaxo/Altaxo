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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


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
		public abstract SizeF PaintSymbol(Graphics g, PointF atPosition, float width); // draws a symbol that represents the style at position (0,0)

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




	[SerializationSurrogate(0,typeof(LineScatterPlotStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class LineScatterPlotStyle : PlotStyle, System.Runtime.Serialization.IDeserializationCallback, IChangedEventSource, IChildChangedEventSink
	{

		protected LineStyle			m_LineStyle;
		protected ScatterStyle	m_ScatterStyle;
		protected bool					m_LineSymbolGap;



		#region Serialization
		/// <summary>Used to serialize the LineScatterPlotStyle Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes LineScatterPlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The LineScatterPlotStyle to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				LineScatterPlotStyle s = (LineScatterPlotStyle)obj;
				info.AddValue("LineStyle",s.m_LineStyle);  
				info.AddValue("ScatterStyle",s.m_ScatterStyle);
				info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
			}
			/// <summary>
			/// Deserializes the LineScatterPlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The empty axis object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized LineScatterPlotStyle.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				LineScatterPlotStyle s = (LineScatterPlotStyle)obj;

				// do not use settings lie s.LineStyle= here, since the LineStyle is cloned, but maybe not fully deserialized here!!!
				s.m_LineStyle = (LineStyle)info.GetValue("LineStyle",typeof(LineStyle));
				// do not use settings lie s.ScatterStyle= here, since the ScatterStyle is cloned, but maybe not fully deserialized here!!!
				s.m_ScatterStyle = (ScatterStyle)info.GetValue("ScatterStyle",typeof(ScatterStyle));
				s.m_LineSymbolGap = info.GetBoolean("LineSymbolGap");
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// restore the event chain here
			if(null!=m_LineStyle)
				m_LineStyle.Changed += new EventHandler(this.OnLineStyleChanged);
			if(null!=m_ScatterStyle)
				m_ScatterStyle.Changed += new EventHandler(this.OnScatterStyleChanged);
		}
		#endregion



		public LineScatterPlotStyle(LineScatterPlotStyle from)
		{
			
			this.LineStyle				=	from.m_LineStyle;
			this.ScatterStyle			= from.m_ScatterStyle;
			// this.m_PlotAssociation	= null; // do not clone the plotassociation!
			this.LineSymbolGap    = from.m_LineSymbolGap;
		}

		public LineScatterPlotStyle()
		{
			this.LineStyle = new LineStyle();
			this.ScatterStyle = new ScatterStyle();
			// this.m_PlotAssociation = null;
			this.LineSymbolGap = true;
		}

		public LineScatterPlotStyle(PlotAssociation pa)
		{
			this.LineStyle = new LineStyle();
			this.ScatterStyle = new ScatterStyle();
			// this.m_PlotAssociation = pa;
			this.LineSymbolGap = true;
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

		public override LineStyle LineStyle
		{
			get { return m_LineStyle; }
			set 
			{
				if(null==value)
					throw new ArgumentNullException("LineStyle","LineStyle may not be set to null in PlotStyle");

				if(null!=m_LineStyle)
					m_LineStyle.Changed -= new EventHandler(OnLineStyleChanged);
	
				m_LineStyle = (LineStyle)value.Clone();

				if(null!=m_LineStyle)
					m_LineStyle.Changed += new EventHandler(OnLineStyleChanged);
				
				OnChanged(); // Fire changed event
			}
		}

		public override ScatterStyle ScatterStyle
		{
			get { return m_ScatterStyle; }
			set 
			{
				if(null==value)
					throw new ArgumentNullException("ScatterStyle","ScatterStyle may not be set to null in PlotStyle");

				if(null!=m_ScatterStyle)
					m_ScatterStyle.Changed -= new EventHandler(OnScatterStyleChanged);

				m_ScatterStyle = (ScatterStyle)value.Clone();

				if(null!=m_ScatterStyle)
					m_ScatterStyle.Changed += new EventHandler(OnScatterStyleChanged);
				
				OnChanged(); // Fire Changed event
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
			set
			{ 
				m_LineSymbolGap = value; 
				OnChanged();
			}
		}



		public override void Paint(Graphics g, Graph.Layer layer, object plotObject)
		{

			if(!(plotObject is PlotAssociation))
				return; // we cannot plot any other than a PlotAssociation now

			PlotAssociation myPlotAssociation = (PlotAssociation)plotObject;

			double layerWidth = layer.Size.Width;
			double layerHeight = layer.Size.Height;


			Altaxo.Data.INumericColumn xColumn = myPlotAssociation.XColumn as Altaxo.Data.INumericColumn;
			Altaxo.Data.INumericColumn yColumn = myPlotAssociation.YColumn as Altaxo.Data.INumericColumn;

			if(null==xColumn || null==yColumn)
				return; // this plotstyle is only for x and y double columns

			if(myPlotAssociation.PlottablePoints>0)
			{

				// allocate an array PointF to hold the line points
				PointF[] ptArray = new PointF[myPlotAssociation.PlottablePoints];

				// Fill the array with values
				// only the points where x and y are not NaNs are plotted!

				int i,j;

				bool bInPlotSpace = true;
				int  rangeStart=0;
				PlotRangeList rangeList = new PlotRangeList();

				int len = myPlotAssociation.PlottablePoints;
				for(i=0,j=0;i<len;i++)
				{
					if(Double.IsNaN(xColumn.GetDoubleAt(i)) || Double.IsNaN(yColumn.GetDoubleAt(i)))
					{
						if(!bInPlotSpace)
						{
							bInPlotSpace=true;
							rangeList.Add(new PlotRange(rangeStart,j));
						}
						continue;
					}
					

					double x_rel = layer.XAxis.PhysicalToNormal(xColumn.GetDoubleAt(i));
					double y_rel = layer.YAxis.PhysicalToNormal(yColumn.GetDoubleAt(i));
					
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
					m_LineStyle.Paint(g,ptArray,rangeList,layer.Size, (m_LineSymbolGap && m_ScatterStyle.Shape!=ScatterStyles.Shape.NoSymbol)?SymbolSize:0);
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

	/// <summary>
	/// PaintSymbol paints the symbol including the line so
	/// that the centre of the symbol is at place PointF 
	/// </summary>
	/// <param name="g">Graphic context</param>
	/// <param name="pos">Position of the starting of the line</param>
 
		public override SizeF PaintSymbol(Graphics g, PointF pos, float width)
		{
			GraphicsState gs = g.Save();
			
			float symsize = this.SymbolSize;
			float linelen = width/2; // distance from start to symbol centre

			g.TranslateTransform(pos.X+linelen,pos.Y);
			if(null!=this.LineStyle && this.LineStyle.Connection != LineStyles.ConnectionStyle.NoLine)
			{
				if(LineSymbolGap==true)
				{
					// plot a line with the length of symbolsize from 
					this.LineStyle.PaintLine(g,new PointF(-linelen,0),new PointF(-symsize,0));
					this.LineStyle.PaintLine(g, new PointF(symsize,0),new PointF(linelen,0));
				}
				else // no gap
				{
					this.LineStyle.PaintLine(g,new PointF(-linelen,0),new PointF(linelen,0));
				}
			}
			// now Paint the symbol
			if(null!=this.ScatterStyle && this.ScatterStyle.Shape != ScatterStyles.Shape.NoSymbol)
				this.ScatterStyle.Paint(g);

			g.Restore(gs);
	
			return new SizeF(2*linelen,symsize);
		}
		#region IChangedEventSource Members

		public event System.EventHandler Changed;

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this,new EventArgs());
		}

		protected virtual void OnLineStyleChanged(object sender, EventArgs e)
		{
			OnChanged();
		}

		protected virtual void OnScatterStyleChanged(object sender, EventArgs e)
		{
			OnChanged();
		}


		#endregion

		#region IChildChangedEventSink Members

		public void OnChildChanged(object child, EventArgs e)
		{
			if(null!=Changed)
				Changed(this,e);
		}

		#endregion
	} // end of class LineScatterPlotStyle
}
