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
	[SerializationSurrogate(0,typeof(XYCurvePlotItem.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class XYCurvePlotItem : PlotItem, System.Runtime.Serialization.IDeserializationCallback
	{
		protected XYCurvePlotData m_PlotData;
		protected PlotStyle       m_PlotStyle;

		#region Serialization
		/// <summary>Used to serialize theXYDataPlot Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes XYDataPlot Version 0.
			/// </summary>
			/// <param name="obj">The XYDataPlot to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				XYCurvePlotItem s = (XYCurvePlotItem)obj;
				info.AddValue("Data",s.m_PlotData);  
				info.AddValue("Style",s.m_PlotStyle);  
			}
			/// <summary>
			/// Deserializes the XYDataPlot Version 0.
			/// </summary>
			/// <param name="obj">The empty XYDataPlot object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized XYDataPlot.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				XYCurvePlotItem s = (XYCurvePlotItem)obj;

				s.m_PlotData = (XYCurvePlotData)info.GetValue("Data",typeof(PlotAssociation));
				s.m_PlotStyle = (PlotStyle)info.GetValue("Style",typeof(PlotStyle));
		
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYCurvePlotItem),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYCurvePlotItem s = (XYCurvePlotItem)obj;
				info.AddValue("Data",s.m_PlotData);  
				info.AddValue("Style",s.m_PlotStyle); 
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				XYCurvePlotData pa  = (XYCurvePlotData)info.GetValue("Data",typeof(PlotAssociation));
				PlotStyle ps = (PlotStyle)info.GetValue("Style",typeof(PlotStyle));
		
				if(null==o)
				{
					return new XYCurvePlotItem(pa,ps);
				}
				else
				{
					XYCurvePlotItem s = (XYCurvePlotItem)o;
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

			if(null!=m_PlotStyle && m_PlotStyle is IChangedEventSource)
			{
				((IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
			}
		}
		#endregion



		public XYCurvePlotItem(XYCurvePlotData pa, PlotStyle ps)
		{
			this.Data = pa;
			this.Style = ps;
		}

		public XYCurvePlotItem(XYCurvePlotItem from)
		{
			this.Data = from.Data;   // also wires the event
			this.Style = from.Style; // also wires the event
		}

		public override object Clone()
		{
			return new XYCurvePlotItem(this);
		}


		public override object Data
		{
			get { return m_PlotData; }
			set
			{
				if(null==value)
					throw new System.ArgumentNullException();
				else if(!(value is XYCurvePlotData))
					throw new System.ArgumentException("The provided data object is not of the type " + m_PlotData.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
				else
				{
					if(!object.ReferenceEquals(m_PlotData,value))
					{
						if(null!=m_PlotData)
						{
							m_PlotData.Changed -= new EventHandler(OnDataChangedEventHandler);
						}

						m_PlotData = (XYCurvePlotData)value;
					
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
				else if(!(value is PlotStyle))
					throw new System.ArgumentException("The provided data object is not of the type " + m_PlotData.GetType().ToString() + ", but of type " + value.GetType().ToString() + "!");
				else
				{
					if(!object.ReferenceEquals(m_PlotStyle,value))
					{
						// delete event wiring to old PlotStyle
						if(null!=m_PlotStyle && m_PlotStyle is IChangedEventSource)
						{
							((IChangedEventSource)m_PlotStyle).Changed -= new EventHandler(OnStyleChangedEventHandler);
						}
					
						m_PlotStyle = (PlotStyle)value;

						// create event wire to new Plotstyle
						if(null!=m_PlotStyle && m_PlotStyle is IChangedEventSource)
						{
							((IChangedEventSource)m_PlotStyle).Changed += new EventHandler(OnStyleChangedEventHandler);
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

		public override void Paint(Graphics g, Graph.Layer layer)
		{
			if(null!=this.m_PlotStyle)
			{
				m_PlotStyle.Paint(g,layer,m_PlotData);
			}
		}

	}
}
