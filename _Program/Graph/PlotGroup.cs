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
		PlotGroupStyle m_Style;
		System.Collections.ArrayList m_PlotAssociations;
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
				info.AddValue("Group",s.m_PlotAssociations);  
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
				s.m_PlotAssociations = (System.Collections.ArrayList)info.GetValue("Group",typeof(System.Collections.ArrayList));
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
		}
		#endregion



		public PlotGroup(PlotAssociation assoc, PlotGroupStyle style)
		{
			m_Style = style;
			m_PlotAssociations = new System.Collections.ArrayList();
			m_PlotAssociations.Add(assoc);
		}

		public PlotGroup(PlotGroupStyle style)
		{
			m_Style = style;
			m_PlotAssociations = new System.Collections.ArrayList();
		}

		public int Count
		{
			get { return null!=m_PlotAssociations ? m_PlotAssociations.Count : 0; }
		}

		public PlotAssociation this[int i]
		{
			get { return (PlotAssociation)m_PlotAssociations[i]; }
		}

		public void Add(PlotAssociation assoc)
		{
			if(null!=assoc)
			{
				int cnt = m_PlotAssociations.Count;
				if(cnt>0)
				{
					assoc.PlotStyle.SetToNextStyle(((PlotAssociation)m_PlotAssociations[cnt-1]).PlotStyle,m_Style);
				}
				m_PlotAssociations.Add(assoc);
			}
		}

		public bool Contains(PlotAssociation assoc)
		{
			return this.m_PlotAssociations.Contains(assoc);
		}

		public void Clear()
		{
			m_PlotAssociations.Clear();
		}

		public PlotAssociation MasterItem
		{
			get { return m_PlotAssociations.Count>0 ? (PlotAssociation)m_PlotAssociations[0] : null; }
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
					UpdateMembers();
			}
		}

		public void UpdateMembers()
		{
			// update the styles beginning from the master item
			if(!IsIndependent && Count>0)
			{
				for(int i=1;i<Count;i++)
					this[i].PlotStyle.SetToNextStyle(this[i-1].PlotStyle,this.m_Style);
			}
		}

		[Serializable]
		public class Collection
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
			}

			public void Clear()
			{
				m_List.Clear();
			}

			public PlotGroup GetPlotGroupOf(PlotAssociation assoc)
			{
				// search for the (first) plot group, to which assoc belongs,
				// and return this group

				for(int i=0;i<m_List.Count;i++)
					if(((PlotGroup)m_List[i]).Contains(assoc)	)
						return ((PlotGroup)m_List[i]);

				return null; // assoc belongs not to any plot group
			}
		} // end of class Collection

	}
	}
