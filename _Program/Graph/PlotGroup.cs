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
using Altaxo.Serialization;

namespace Altaxo.Graph
{
	[Flags]
	[Serializable]
	public enum PlotGroupStyle
	{
		Color = 0x01,
		Line  = 0x02,
		Symbol = 0x04,
		All = Color | Line | Symbol
	}

	
	/// <summary>
	/// Summary description for PlotGroup.
	/// </summary>
	[SerializationSurrogate(0,typeof(PlotGroup.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class PlotGroup : System.Runtime.Serialization.IDeserializationCallback
	{
		/// <summary>
		/// Designates which dependencies the plot styles have on each other.
		/// </summary>
		PlotGroupStyle m_Style;
		System.Collections.ArrayList m_PlotItems;
		
		private PlotGroup.Collection m_Parent;



		#region Serialization
		/// <summary>Used to serialize the PlotGroup Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes PlotGroup Version 0.
			/// </summary>
			/// <param name="obj">The PlotGroup to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				PlotGroup s = (PlotGroup)obj;
				info.AddValue("Style",s.m_Style);  
				info.AddValue("Group",s.m_PlotItems);  
			}
			/// <summary>
			/// Deserializes the PlotGroup Version 0.
			/// </summary>
			/// <param name="obj">The empty axis object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized PlotGroup.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				PlotGroup s = (PlotGroup)obj;

				s.m_Style = (PlotGroupStyle)info.GetValue("Style",typeof(PlotGroupStyle));
				s.m_PlotItems = (System.Collections.ArrayList)info.GetValue("Group",typeof(System.Collections.ArrayList));
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			if(m_PlotItems.Count>0)
				((PlotItem)m_PlotItems[0]).StyleChanged += new EventHandler(this.OnMasterStyleChangedEventHandler);
		}
		#endregion



		public PlotGroup(PlotItem master, PlotGroupStyle style)
		{
			m_Style = style;
			m_PlotItems = new System.Collections.ArrayList();
			m_PlotItems.Add(master);
		}

		public PlotGroup(PlotGroupStyle style)
		{
			m_Style = style;
			m_PlotItems = new System.Collections.ArrayList();
		}

		public int Count
		{
			get { return null!=m_PlotItems ? m_PlotItems.Count : 0; }
		}

		public PlotItem this[int i]
		{
			get { return (PlotItem)m_PlotItems[i]; }
		}

		public void Add(PlotItem assoc)
		{
			if(null!=assoc)
			{
				int cnt = m_PlotItems.Count;
				if(cnt==0) // this is the first, i.e. the master item, it must be wired by a Changed event handler
				{
					assoc.StyleChanged += new EventHandler(this.OnMasterStyleChangedEventHandler);
				}
				if(cnt>0)
				{
					((PlotStyle)assoc.Style).SetToNextStyle((PlotStyle)((PlotItem)m_PlotItems[cnt-1]).Style,m_Style);
				}
				m_PlotItems.Add(assoc);

				OnChanged();
			}
		}

		public bool Contains(PlotItem assoc)
		{
			return this.m_PlotItems.Contains(assoc);
		}

		public void Clear()
		{
			m_PlotItems.Clear();

			OnChanged();
		}

		public PlotItem MasterItem
		{
			get { return m_PlotItems.Count>0 ? (PlotItem)m_PlotItems[0] : null; }
		}

		public bool IsIndependent 
		{
			get { return this.m_Style == 0; }
		}

		public PlotGroupStyle Style
		{
			get { return this.m_Style; }
			set 
			{
				bool changed = (this.m_Style==value);
				this.m_Style = value;
				
				// update the styles beginning from the master item
				if(changed)
				{
					UpdateMembers();
					OnChanged();
				}
			}
		}

		public void UpdateMembers()
		{
			// update the styles beginning from the master item
			if(!IsIndependent && Count>0)
			{
				for(int i=1;i<Count;i++)
					((PlotStyle)this[i].Style).SetToNextStyle((PlotStyle)this[i-1].Style,this.m_Style);
			}
			// no changed event here since we track only the members structure and the grouping style
		}

		protected void OnMasterStyleChangedEventHandler(object sender, EventArgs e)
		{
			UpdateMembers();
		}

		protected virtual void OnChanged()
		{
			if(null!=m_Parent)
				m_Parent.OnChildChangedEventHandler(this);
		}

		[Serializable]
			public class Collection : IChangedEventSource
		{
			protected System.Collections.ArrayList m_List;

			public Collection()
			{
				m_List = new System.Collections.ArrayList();
			}

			public void Add(PlotGroup g)
			{
				g.m_Parent=this;
				m_List.Add(g);

				OnChanged();
			}

			public void Clear()
			{
				m_List.Clear();

				OnChanged();
			}

			public PlotGroup GetPlotGroupOf(PlotItem assoc)
			{
				// search for the (first) plot group, to which assoc belongs,
				// and return this group

				for(int i=0;i<m_List.Count;i++)
					if(((PlotGroup)m_List[i]).Contains(assoc)	)
						return ((PlotGroup)m_List[i]);

				return null; // assoc belongs not to any plot group
			}


			#region IChangedEventSource Members

			public event System.EventHandler Changed;

			protected virtual void OnChanged()
			{
				if(null!=Changed)
					Changed(this,new EventArgs());
			}

			public virtual void OnChildChangedEventHandler(object sender)
			{
				OnChanged();
			}


			#endregion
		} // end of class Collection

	}
}
