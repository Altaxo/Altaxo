using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Data;

namespace Altaxo.Graph
{
	/// <summary>
	/// Association of data and style specialized for x-y-plots of column data.
	/// </summary>
	[SerializationSurrogate(0,typeof(XYFunctionPlotItem.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class XYFunctionPlotItem : PlotItem, System.Runtime.Serialization.IDeserializationCallback
	{
		protected XYFunctionPlotData m_PlotData;
		protected AbstractXYPlotStyle       m_PlotStyle;

		#region Serialization
		/// <summary>Used to serialize theXYDataPlot Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes XYColumnPlotItem Version 0.
			/// </summary>
			/// <param name="obj">The XYColumnPlotItem to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
				info.AddValue("Data",s.m_PlotData);  
				info.AddValue("Style",s.m_PlotStyle);  
			}
			/// <summary>
			/// Deserializes the XYColumnPlotItem Version 0.
			/// </summary>
			/// <param name="obj">The empty XYColumnPlotItem object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized XYColumnPlotItem.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				XYFunctionPlotItem s = (XYFunctionPlotItem)obj;

				s.m_PlotData = (XYFunctionPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
				s.m_PlotStyle = (AbstractXYPlotStyle)info.GetValue("Style",typeof(AbstractXYPlotStyle));
		
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYFunctionPlotItem),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYFunctionPlotItem s = (XYFunctionPlotItem)obj;
				info.AddValue("Data",s.m_PlotData);  
				info.AddValue("Style",s.m_PlotStyle); 
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				XYFunctionPlotData pa  = (XYFunctionPlotData)info.GetValue("Data",typeof(XYColumnPlotData));
				AbstractXYPlotStyle ps = (AbstractXYPlotStyle)info.GetValue("Style",typeof(AbstractXYPlotStyle));
		
				if(null==o)
				{
					return new XYFunctionPlotItem(pa,ps);
				}
				else
				{
					XYFunctionPlotItem s = (XYFunctionPlotItem)o;
					s.Data = pa;
					s.Style = ps;
					return s;
				}
			
			}
		}

		/// <summary>
		/// Finale measures after deserialization of the linear axis.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// Restore the event chain

			if(null!=m_PlotData)
			{
				m_PlotData.Changed += new EventHandler(OnDataChangedEventHandler);
			}

			if(null!=m_PlotStyle && m_PlotStyle is Main.IChangedEventSource)
			{
				((Main.IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
			}
		}
		#endregion



		public XYFunctionPlotItem(XYFunctionPlotData pa, AbstractXYPlotStyle ps)
		{
			this.Data = pa;
			this.Style = ps;
		}

		public XYFunctionPlotItem(XYFunctionPlotItem from)
		{
			this.Data = from.Data;   // also wires the event
			this.Style = from.Style; // also wires the event
		}

		public override object Clone()
		{
			return new XYFunctionPlotItem(this);
		}


		public override object Data
		{
			get { return m_PlotData; }
			set
			{
				if(null==value)
					throw new System.ArgumentNullException();
				else if(!(value is XYFunctionPlotData))
					throw new System.ArgumentException("The provided data object is not of the type " + m_PlotData.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
				else
				{
					if(!object.ReferenceEquals(m_PlotData,value))
					{
						if(null!=m_PlotData)
						{
							m_PlotData.Changed -= new EventHandler(OnDataChangedEventHandler);
						}

						m_PlotData = (XYFunctionPlotData)value;
					
						if(null!=m_PlotData)
						{
							m_PlotData.Changed += new EventHandler(OnDataChangedEventHandler);
						}

						OnDataChanged();
					}
				}
			}
		}
		public override object Style
		{
			get { return m_PlotStyle; }
			set
			{
				if(null==value)
					throw new System.ArgumentNullException();
				else if(!(value is AbstractXYPlotStyle))
					throw new System.ArgumentException("The provided data object is not of the type " + m_PlotData.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
				else
				{
					if(!object.ReferenceEquals(m_PlotStyle,value))
					{
						// delete event wiring to old AbstractXYPlotStyle
						if(null!=m_PlotStyle && m_PlotStyle is Main.IChangedEventSource)
						{
							((Main.IChangedEventSource)m_PlotStyle).Changed -= new EventHandler(OnStyleChangedEventHandler);
						}
					
						m_PlotStyle = (AbstractXYPlotStyle)value;

						// create event wire to new Plotstyle
						if(null!=m_PlotStyle && m_PlotStyle is Main.IChangedEventSource)
						{
							((Main.IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
						}

						// indicate the style has changed
						OnStyleChanged();
					}
				}
			}
		}


		public override string GetName(int level)
		{
			return m_PlotData.ToString();
		}

		public override string ToString()
		{
			return GetName(int.MaxValue);
		}

		public override void Paint(Graphics g, Graph.XYPlotLayer layer)
		{
			if(null!=this.m_PlotStyle)
			{
				m_PlotStyle.Paint(g,layer,m_PlotData);
			}
		}

	}
}
