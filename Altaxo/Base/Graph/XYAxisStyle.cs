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
	/// XYAxisStyle is responsible for painting the axes on rectangular two dimensional layers.
	/// </summary>
	[SerializationSurrogate(0,typeof(XYAxisStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class XYAxisStyle
		: 
		System.Runtime.Serialization.IDeserializationCallback,
		Main.IChangedEventSource,
		ICloneable
	{
		/// <summary>Edge of the layer this axis is drawn on.</summary>
		protected Edge m_Edge = new Edge(EdgeType.Left);
		/// <summary>Pen used for painting of the axis.</summary>
		protected PenHolder m_AxisPen = new PenHolder(Color.Black,1);
		/// <summary>Pen used for painting of the major ticks.</summary>
		protected PenHolder m_MajorTickPen =  new PenHolder(Color.Black,1);
		/// <summary>Pen used for painting of the minor ticks.</summary>
		protected PenHolder m_MinorTickPen =  new PenHolder(Color.Black,1);
		/// <summary>Length of the major ticks in points (1/72 inch).</summary>
		protected float m_MajorTickLength = 12;
		/// <summary>Length of the minor ticks in points (1/72 inch).</summary>
		protected float m_MinorTickLength = 8;
		/// <summary>True if major ticks should be painted outside of the layer.</summary>
		protected bool  m_bOuterMajorTicks=true; // true if outer major ticks should be visible
		/// <summary>True if major ticks should be painted inside of the layer.</summary>
		protected bool  m_bInnerMajorTicks=true; // true if inner major ticks should be visible
		/// <summary>True if minor ticks should be painted outside of the layer.</summary>
		protected bool  m_bOuterMinorTicks=true; // true if outer minor ticks should be visible
		/// <summary>True if major ticks should be painted inside of the layer.</summary>
		protected bool  m_bInnerMinorTicks=true; // true if inner minor ticks should be visible
		/// <summary>Axis shift position, either provide as absolute values in point units, or as relative value relative to the layer size.</summary>
		protected Calc.RelativeOrAbsoluteValue m_AxisPosition; // if relative, then relative to layer size, if absolute then in points

		/// <summary>Fired if anything on this style changed</summary>
		public event System.EventHandler Changed;


	
		#region Serialization
		/// <summary>Used to serialize the axis style Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes the axis style (version 0).
			/// </summary>
			/// <param name="obj">The axis style to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				XYAxisStyle s = (XYAxisStyle)obj;
				info.AddValue("Edge",s.m_Edge);
				info.AddValue("AxisPen",s.m_AxisPen);  
				info.AddValue("MajorPen",s.m_MajorTickPen);  
				info.AddValue("MinorPen",s.m_MinorTickPen);
				info.AddValue("MajorLength",s.m_MajorTickLength);
				info.AddValue("MinorLength",s.m_MinorTickLength);
				info.AddValue("MajorOuter",s.m_bOuterMajorTicks);
				info.AddValue("MajorInner",s.m_bInnerMajorTicks);
				info.AddValue("MinorOuter",s.m_bOuterMinorTicks);
				info.AddValue("MinorInner",s.m_bInnerMinorTicks);
				info.AddValue("AxisPosition",s.m_AxisPosition);
			}
			/// <summary>
			/// Deserializes the axis style (version 0).
			/// </summary>
			/// <param name="obj">The empty axis object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized linear axis.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				XYAxisStyle s = (XYAxisStyle)obj;

				s.m_Edge         = (Edge)info.GetValue("Edge",typeof(Edge));
				s.m_AxisPen			 = (PenHolder)info.GetValue("AxisPen",typeof(PenHolder));
				s.m_MajorTickPen = (PenHolder)info.GetValue("MajorPen",typeof(PenHolder));
				s.m_MinorTickPen = (PenHolder)info.GetValue("MinorPen",typeof(PenHolder));

				s.m_MajorTickLength = (float)info.GetSingle("MajorLength");
				s.m_MinorTickLength = (float)info.GetSingle("MinorLength");
				s.m_bOuterMajorTicks = (bool)info.GetBoolean("MajorOuter");
				s.m_bInnerMajorTicks = (bool)info.GetBoolean("MajorInner");
				s.m_bOuterMinorTicks = (bool)info.GetBoolean("MinorOuter");
				s.m_bInnerMinorTicks = (bool)info.GetBoolean("MinorInner");
				s.m_AxisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition",typeof(Calc.RelativeOrAbsoluteValue));
		
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYAxisStyle),0)]
			public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYAxisStyle s = (XYAxisStyle)obj;
				info.AddValue("Edge",s.m_Edge);
				info.AddValue("AxisPen",s.m_AxisPen);  
				info.AddValue("MajorPen",s.m_MajorTickPen);  
				info.AddValue("MinorPen",s.m_MinorTickPen);
				info.AddValue("MajorLength",s.m_MajorTickLength);
				info.AddValue("MinorLength",s.m_MinorTickLength);
				info.AddValue("MajorOuter",s.m_bOuterMajorTicks);
				info.AddValue("MajorInner",s.m_bInnerMajorTicks);
				info.AddValue("MinorOuter",s.m_bOuterMinorTicks);
				info.AddValue("MinorInner",s.m_bInnerMinorTicks);
				info.AddValue("AxisPosition",s.m_AxisPosition);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				XYAxisStyle s = null!=o ? (XYAxisStyle)o : new XYAxisStyle();
				
				s.m_Edge         = (Edge)info.GetValue("Edge",s);
				s.m_AxisPen			 = (PenHolder)info.GetValue("AxisPen",s);
				s.m_MajorTickPen = (PenHolder)info.GetValue("MajorPen",s);
				s.m_MinorTickPen = (PenHolder)info.GetValue("MinorPen",s);

				s.m_MajorTickLength = (float)info.GetSingle("MajorLength");
				s.m_MinorTickLength = (float)info.GetSingle("MinorLength");
				s.m_bOuterMajorTicks = (bool)info.GetBoolean("MajorOuter");
				s.m_bInnerMajorTicks = (bool)info.GetBoolean("MajorInner");
				s.m_bOuterMinorTicks = (bool)info.GetBoolean("MinorOuter");
				s.m_bInnerMinorTicks = (bool)info.GetBoolean("MinorInner");
				s.m_AxisPosition = (Calc.RelativeOrAbsoluteValue)info.GetValue("AxisPosition",s);
		
				return s;
			}
		}

	

		/// <summary>
		/// Finale measures after deserialization of the linear axis.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			WireEventChain();
		}
		#endregion


		/// <summary>
		/// Wires the neccessary child events with event handlers in this class.
		/// </summary>
		protected virtual void WireEventChain()
		{
			m_AxisPen.Changed += new EventHandler(OnPenChangedEventHandler);
			m_MajorTickPen.Changed += new EventHandler(OnPenChangedEventHandler);
			m_MinorTickPen.Changed += new EventHandler(OnPenChangedEventHandler);
		}

		/// <summary>
		/// Creates a default axis style.
		/// </summary>
		/// <param name="st">The edge of the layer the axis is positioned to.</param>
		public XYAxisStyle(EdgeType st)
		{
			m_Edge = new Edge(st);

		WireEventChain();

		}

		public XYAxisStyle()
			: this(EdgeType.Left)
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The AxisStyle to copy from</param>
		public XYAxisStyle(XYAxisStyle from)
		{
			this.m_AxisPen = null==from.m_AxisPen ? null : (PenHolder)from.m_AxisPen.Clone();
			this.m_AxisPosition = from.m_AxisPosition;
			this.m_bInnerMajorTicks = from.m_bInnerMajorTicks;
			this.m_bInnerMinorTicks = from.m_bInnerMinorTicks;
			this.m_bOuterMajorTicks = from.m_bOuterMajorTicks;
			this.m_bOuterMinorTicks = from.m_bOuterMinorTicks;
			this.m_Edge             = from.m_Edge;
			this.m_MajorTickLength  = from.m_MajorTickLength;
			this.m_MajorTickPen     = null==from.m_MajorTickPen ? null : (PenHolder)from.m_MajorTickPen;
			this.m_MinorTickLength  = from.m_MinorTickLength;
			this.m_MinorTickPen     = (null==from.m_MinorTickPen) ? null : (PenHolder)from.m_MinorTickPen;

			// Rewire the event chain
			WireEventChain();
		}

		/// <summary>
		/// Creates a clone of this object.
		/// </summary>
		/// <returns>The cloned object.</returns>
		public  object Clone()
		{
			return new XYAxisStyle(this);
		}

		/// <summary>
		/// OuterDistance returns the used space from the middle line of the axis
		/// to the last outer object (either the outer major thicks or half
		/// of the axis thickness)
		/// </summary>
		public float OuterDistance
		{
			get
			{
				float retVal = m_AxisPen.Width/2; // half of the axis thickness
				retVal = System.Math.Max(retVal, m_bOuterMajorTicks ? m_MajorTickLength:0);
				retVal = System.Math.Max(retVal, m_bOuterMinorTicks ? m_MinorTickLength:0);
				return retVal;
			}
		}

		/// <summary>
		/// GetOffset returns the distance of the axis to the layer edge in points
		/// in most cases, the axis position is exactly onto the layer edge and offset is zero,
		/// if the axis is outside the layer, offset is a positive value, 
		/// if the axis is shifted inside the layer, offset is negative 
		/// </summary>
		public float GetOffset(SizeF layerSize)
		{
		return (float)m_AxisPosition.GetValueRelativeTo(m_Edge.GetOppositeEdgeLength(layerSize));
		}
		
		/// <summary>Get/sets the major tick length.</summary>
		/// <value>The major tick length in point units (1/72 inch).</value>
		public float MajorTickLength
		{
			get { return this.m_MajorTickLength; }
			set
			{
				if(value!=m_MajorTickLength)
				{
					m_MajorTickLength = value; 
					OnChanged(); // fire the changed event
				}
			}
		}

		/// <summary>Get/sets the minor tick length.</summary>
		/// <value>The minor tick length in point units (1/72 inch).</value>
		public float MinorTickLength
		{
			get { return this.m_MinorTickLength; }
			set 
			{
				if(value!=m_MinorTickLength)
				{
					m_MinorTickLength = value;
					OnChanged(); // fire the changed event
				}
			}
		}

		/// <summary>Get/sets if outer major ticks are drawn.</summary>
		/// <value>True if outer major ticks are drawn.</value>
		public bool OuterMajorTicks
		{
			get { return this.m_bOuterMajorTicks; }
			set 
			{
				if(value!=m_bOuterMajorTicks)
				{
					this.m_bOuterMajorTicks = value; 
					OnChanged(); // fire the changed event
				}
			}
		}

		/// <summary>Get/sets if inner major ticks are drawn.</summary>
		/// <value>True if inner major ticks are drawn.</value>
		public bool InnerMajorTicks
		{
			get { return this.m_bInnerMajorTicks; }
			set
			{
				if(value!=m_bInnerMajorTicks)
				{
					this.m_bInnerMajorTicks = value; 
					OnChanged(); // fire the changed event
				}
			}
		}

		/// <summary>Get/sets if outer minor ticks are drawn.</summary>
		/// <value>True if outer minor ticks are drawn.</value>
		public bool OuterMinorTicks
		{
			get { return this.m_bOuterMinorTicks; }
			set 
			{ 
				if(value!=m_bOuterMinorTicks)
				{
					this.m_bOuterMinorTicks = value;
					OnChanged(); // fire the changed event
				}
			}
		}

		/// <summary>Get/sets if inner minor ticks are drawn.</summary>
		/// <value>True if inner minor ticks are drawn.</value>
		public bool InnerMinorTicks
		{
			get { return this.m_bInnerMinorTicks; }
			set 
			{ 
				if(value!=m_bInnerMinorTicks)
				{
					this.m_bInnerMinorTicks = value;
					OnChanged(); // fire the changed event
				}
			}
		}

		/// <summary>
		/// Gets/sets the axis thickness.
		/// </summary>
		/// <value>Returns the thickness of the axis pen. On setting this value, it sets
		/// the thickness of the axis pen, the tickness of the major ticks pen, and the
		/// thickness of the minor ticks pen together.</value>
		public float Thickness
		{
			get { return this.m_AxisPen.Width; }
			set
			{ 
				this.m_AxisPen.Width = value;
				this.m_MajorTickPen.Width = value;
				this.m_MinorTickPen.Width = value;
				OnChanged(); // fire the changed event
			}
		}

		/// <summary>
		/// Get/sets the axis color.
		/// </summary>
		/// <value>Returns the color of the axis pen. On setting this value, it sets
		/// the color of the axis pen along with the color of the major ticks pen and the
		/// color of the minor ticks pen together.</value>
		public Color Color
		{
			get { return this.m_AxisPen.Color; }
			set
			{
				this.m_AxisPen.Color = value;
				this.m_MajorTickPen.Color = value;
				this.m_MinorTickPen.Color = value;
				OnChanged(); // fire the changed event
			}
		}

		/// <summary>
		/// Get/set the axis shift position value.
		/// </summary>
		/// <value>Zero if the axis is not shifted (normal case). Else the shift value, either as
		/// absolute value in point units (1/72 inch), or relative to the corresponding layer dimension (i.e layer width for bottom axis).</value>
		public Calc.RelativeOrAbsoluteValue Position
		{
			get { return this.m_AxisPosition; }
			set	
			{
				if(value!=m_AxisPosition)
				{
					m_AxisPosition = value;
					OnChanged(); // fire the changed event
				}
				}
		}


		/// <summary>
		/// Paint the axis in the Graphics context.
		/// </summary>
		/// <param name="g">The graphics context painting to.</param>
		/// <param name="layer">The layer the axis belongs to.</param>
		/// <param name="axis">The axis this axis style is used for.</param>
		public void Paint(Graphics g, XYPlotLayer layer, Axis axis)
		{
			SizeF layerSize = layer.Size;


			PointF orgP = m_Edge.GetOrg(layerSize);
			PointF endP = m_Edge.GetEnd(layerSize);
			PointF outVector = m_Edge.OuterVector;
			PointF offset = m_Edge.OuterVector;
			float foffset = this.GetOffset(layerSize);
			offset.X *= foffset;
			offset.Y *= foffset;
			
			orgP.X += offset.X; orgP.Y += offset.Y;
			endP.X += offset.X; endP.Y += offset.Y;

			g.DrawLine(m_AxisPen,orgP,endP);

			// now the major ticks
			double[] majorticks = axis.GetMajorTicks();
			for(int i=0;i<majorticks.Length;i++)
			{
				double r = axis.PhysicalToNormal(majorticks[i]);
				PointF tickorg = Edge.GetPointBetween(orgP,endP,r);
				PointF tickend;
				if(m_bOuterMajorTicks)
				{
					tickend	= tickorg;
					tickend.X += outVector.X * m_MajorTickLength;
					tickend.Y += outVector.Y * m_MajorTickLength;
					g.DrawLine(m_MajorTickPen,tickorg,tickend);
				}
				if(m_bInnerMajorTicks)
				{
					tickend	= tickorg;
					tickend.X -= outVector.X * m_MajorTickLength;
					tickend.Y -= outVector.Y * m_MajorTickLength;
					g.DrawLine(m_MajorTickPen,tickorg,tickend);
				}
			}
			// now the major ticks
			double[] minorticks = axis.GetMinorTicks();
			for(int i=0;i<minorticks.Length;i++)
			{
				double r = axis.PhysicalToNormal(minorticks[i]);
				PointF tickorg =  Edge.GetPointBetween(orgP,endP,r);
				PointF tickend;
				if(m_bOuterMinorTicks)
				{
					tickend	= tickorg;
					tickend.X += outVector.X * m_MinorTickLength;
					tickend.Y += outVector.Y * m_MinorTickLength;
					g.DrawLine(m_MinorTickPen,tickorg,tickend);
				}
				if(m_bInnerMinorTicks)
				{
					tickend	= tickorg;
					tickend.X -= outVector.X * m_MinorTickLength;
					tickend.Y -= outVector.Y * m_MinorTickLength;
					g.DrawLine(m_MinorTickPen,tickorg,tickend);
				}
			}
		}

		protected virtual void OnPenChangedEventHandler(object sender, EventArgs e)
		{
			OnChanged();
		}

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this,new EventArgs());
		}

	}
}
