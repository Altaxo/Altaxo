using System;

namespace Altaxo.Graph
{
	[Flags]
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
	public class PlotGroup
	{
		PlotGroupStyle m_Style;
		System.Collections.ArrayList m_PlotAssociations;
		private PlotGroup.Collection m_Parent;

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
