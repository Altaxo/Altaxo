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
	[SerializationSurrogate(0,typeof(XYLineScatterPlotStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class XYLineScatterPlotStyle : AbstractXYPlotStyle, System.Runtime.Serialization.IDeserializationCallback, IChangedEventSource, IChildChangedEventSink
	{

		protected XYPlotLineStyle			m_LineStyle;
		protected XYPlotScatterStyle	m_ScatterStyle;
		protected bool					m_LineSymbolGap;



		#region Serialization
		/// <summary>Used to serialize the XYLineScatterPlotStyle Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes XYLineScatterPlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The XYLineScatterPlotStyle to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
				info.AddValue("XYPlotLineStyle",s.m_LineStyle);  
				info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
				info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
			}
			/// <summary>
			/// Deserializes the XYLineScatterPlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The empty axis object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized XYLineScatterPlotStyle.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;

				// do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
				s.m_LineStyle = (XYPlotLineStyle)info.GetValue("XYPlotLineStyle",typeof(XYPlotLineStyle));
				// do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
				s.m_ScatterStyle = (XYPlotScatterStyle)info.GetValue("XYPlotScatterStyle",typeof(XYPlotScatterStyle));
				s.m_LineSymbolGap = info.GetBoolean("LineSymbolGap");
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYLineScatterPlotStyle),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYLineScatterPlotStyle s = (XYLineScatterPlotStyle)obj;
				info.AddValue("XYPlotLineStyle",s.m_LineStyle);  
				info.AddValue("XYPlotScatterStyle",s.m_ScatterStyle);
				info.AddValue("LineSymbolGap",s.m_LineSymbolGap);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				XYLineScatterPlotStyle s = null!=o ? (XYLineScatterPlotStyle)o : new XYLineScatterPlotStyle();
				// do not use settings lie s.XYPlotLineStyle= here, since the XYPlotLineStyle is cloned, but maybe not fully deserialized here!!!
				s.XYPlotLineStyle = (XYPlotLineStyle)info.GetValue("XYPlotLineStyle",typeof(XYPlotLineStyle));
				// do not use settings lie s.XYPlotScatterStyle= here, since the XYPlotScatterStyle is cloned, but maybe not fully deserialized here!!!
				s.XYPlotScatterStyle = (XYPlotScatterStyle)info.GetValue("XYPlotScatterStyle",typeof(XYPlotScatterStyle));
				s.LineSymbolGap = info.GetBoolean("LineSymbolGap");

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



		public XYLineScatterPlotStyle(XYLineScatterPlotStyle from)
		{
			
			this.XYPlotLineStyle				=	from.m_LineStyle;
			this.XYPlotScatterStyle			= from.m_ScatterStyle;
			// this.m_PlotAssociation	= null; // do not clone the plotassociation!
			this.LineSymbolGap    = from.m_LineSymbolGap;
		}

		public XYLineScatterPlotStyle()
		{
			this.XYPlotLineStyle = new XYPlotLineStyle();
			this.XYPlotScatterStyle = new XYPlotScatterStyle();
			// this.m_PlotAssociation = null;
			this.LineSymbolGap = true;
		}

		public XYLineScatterPlotStyle(XYColumnPlotData pa)
		{
			this.XYPlotLineStyle = new XYPlotLineStyle();
			this.XYPlotScatterStyle = new XYPlotScatterStyle();
			// this.m_PlotAssociation = pa;
			this.LineSymbolGap = true;
		}




		public override object Clone()
		{
			return new XYLineScatterPlotStyle(this);
		}


		public override void SetToNextStyle(AbstractXYPlotStyle pstemplate, PlotGroupStyle style)
		{
			if(0!= (style & PlotGroupStyle.Color))
				this.Color =GetNextPlotColor(pstemplate.Color);
			if(0!= (style & PlotGroupStyle.Line))
				this.XYPlotLineStyle.SetToNextLineStyle(pstemplate.XYPlotLineStyle);
			if(0!= (style & PlotGroupStyle.Symbol))
				this.XYPlotScatterStyle.SetToNextStyle(pstemplate.XYPlotScatterStyle);
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

		public override XYPlotLineStyle XYPlotLineStyle
		{
			get { return m_LineStyle; }
			set 
			{
				if(null==value)
					throw new ArgumentNullException("XYPlotLineStyle","XYPlotLineStyle may not be set to null in AbstractXYPlotStyle");

				if(null!=m_LineStyle)
					m_LineStyle.Changed -= new EventHandler(OnLineStyleChanged);
	
				m_LineStyle = (XYPlotLineStyle)value.Clone();

				if(null!=m_LineStyle)
					m_LineStyle.Changed += new EventHandler(OnLineStyleChanged);
				
				OnChanged(); // Fire changed event
			}
		}

		public override XYPlotScatterStyle XYPlotScatterStyle
		{
			get { return m_ScatterStyle; }
			set 
			{
				if(null==value)
					throw new ArgumentNullException("XYPlotScatterStyle","XYPlotScatterStyle may not be set to null in AbstractXYPlotStyle");

				if(null!=m_ScatterStyle)
					m_ScatterStyle.Changed -= new EventHandler(OnScatterStyleChanged);

				m_ScatterStyle = (XYPlotScatterStyle)value.Clone();

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

		public override void Paint(Graphics g, Graph.XYPlotLayer layer, object plotObject)
		{
			if(plotObject is XYColumnPlotData)
				Paint(g,layer,(XYColumnPlotData)plotObject);
			else if(plotObject is Altaxo.Calc.IScalarFunctionDD)
				Paint(g,layer,(Altaxo.Calc.IScalarFunctionDD)plotObject);
		}

		public void Paint(Graphics g, Graph.XYPlotLayer layer, XYColumnPlotData myPlotAssociation)
		{
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


				// now plot the point array
				Paint(g,layer,rangeList,ptArray);
			}
		}


		public void Paint(Graphics g, Graph.XYPlotLayer layer, Altaxo.Calc.IScalarFunctionDD plotFunction)
		{
			const int functionPoints=1000;
			double layerWidth = layer.Size.Width;
			double layerHeight = layer.Size.Height;

			// allocate an array PointF to hold the line points
			PointF[] ptArray = new PointF[functionPoints];

			double xorg = layer.XAxis.Org;
			double xend = layer.XAxis.End;
			// Fill the array with values
			// only the points where x and y are not NaNs are plotted!

			int i,j;

			bool bInPlotSpace = true;
			int  rangeStart=0;
			PlotRangeList rangeList = new PlotRangeList();
		
			for(i=0,j=0;i<functionPoints;i++)
			{
				double x = layer.XAxis.NormalToPhysical(((double)i)/(functionPoints-1));
				double y = plotFunction.Function(x);
        
				if(Double.IsNaN(x) || Double.IsNaN(y))
				{
					if(!bInPlotSpace)
					{
						bInPlotSpace=true;
						rangeList.Add(new PlotRange(rangeStart,j));
					}
					continue;
				}
					

				double x_rel = layer.XAxis.PhysicalToNormal(x);
				double y_rel = layer.YAxis.PhysicalToNormal(y);
					
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



			// ------------------ end of creation of plot array -----------------------------------------------
		
			// now plot the point array
			Paint(g,layer,rangeList,ptArray);
		}

		public void Paint(Graphics g, Graph.XYPlotLayer layer, PlotRangeList rangeList, PointF[] ptArray)
		{

			// --------------------- now do the job of plotting ------------------------------------------------


			// paint the line style
			if(null!=m_LineStyle)
			{
				m_LineStyle.Paint(g,ptArray,rangeList,layer.Size, (m_LineSymbolGap && m_ScatterStyle.Shape!=XYPlotScatterStyles.Shape.NoSymbol)?SymbolSize:0);
			}

			// paint the drop style
			if(null!=m_ScatterStyle && m_ScatterStyle.DropLine!=XYPlotScatterStyles.DropLine.NoDrop)
			{
				PenHolder ph = m_ScatterStyle.Pen;
				ph.Cached=true;
				Pen pen = ph.Pen; // do not dispose this pen, since it is cached
				float xe=layer.Size.Width;
				float ye=layer.Size.Height;
				if( (0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Top)) && (0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Bottom)))
				{
					for(int j=0;j<ptArray.Length;j++)
					{
						float x = ptArray[j].X;
						g.DrawLine(pen,x,0,x,ye);
					}
				}
				else if( 0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Top))
				{
					for(int j=0;j<ptArray.Length;j++)
					{
						float x = ptArray[j].X;
						float y = ptArray[j].Y;
						g.DrawLine(pen,x,0,x,y);
					}
				}
				else if( 0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Bottom))
				{
					for(int j=0;j<ptArray.Length;j++)
					{
						float x = ptArray[j].X;
						float y = ptArray[j].Y;
						g.DrawLine(pen,x,y,x,ye);
					}
				}

				if( (0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Left)) && (0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Right)))
				{
					for(int j=0;j<ptArray.Length;j++)
					{
						float y = ptArray[j].Y;
						g.DrawLine(pen,0,y,xe,y);
					}
				}
				else if( 0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Right))
				{
					for(int j=0;j<ptArray.Length;j++)
					{
						float x = ptArray[j].X;
						float y = ptArray[j].Y;
						g.DrawLine(pen,x,y,xe,y);
					}
				}
				else if( 0!=(m_ScatterStyle.DropLine&XYPlotScatterStyles.DropLine.Left))
				{
					for(int j=0;j<ptArray.Length;j++)
					{
						float x = ptArray[j].X;
						float y = ptArray[j].Y;
						g.DrawLine(pen,0,y,x,y);
					}
				}
			} // end paint the drop style


			// paint the scatter style
			if(null!=m_ScatterStyle && m_ScatterStyle.Shape!=XYPlotScatterStyles.Shape.NoSymbol)
			{
				// save the graphics stat since we have to translate the origin
				System.Drawing.Drawing2D.GraphicsState gs = g.Save();


				float xpos=0, ypos=0;
				float xdiff,ydiff;
				for(int j=0;j<ptArray.Length;j++)
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
			if(null!=this.XYPlotLineStyle && this.XYPlotLineStyle.Connection != XYPlotLineStyles.ConnectionStyle.NoLine)
			{
				if(LineSymbolGap==true)
				{
					// plot a line with the length of symbolsize from 
					this.XYPlotLineStyle.PaintLine(g,new PointF(-linelen,0),new PointF(-symsize,0));
					this.XYPlotLineStyle.PaintLine(g, new PointF(symsize,0),new PointF(linelen,0));
				}
				else // no gap
				{
					this.XYPlotLineStyle.PaintLine(g,new PointF(-linelen,0),new PointF(linelen,0));
				}
			}
			// now Paint the symbol
			if(null!=this.XYPlotScatterStyle && this.XYPlotScatterStyle.Shape != XYPlotScatterStyles.Shape.NoSymbol)
				this.XYPlotScatterStyle.Paint(g);

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
	} // end of class XYLineScatterPlotStyle
}
