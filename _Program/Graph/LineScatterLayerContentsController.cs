using System;
using System.Windows.Forms;
using System.Drawing;


namespace Altaxo.Graph
{
	#region Interfaces
	public interface ILineScatterLayerContentsController : Main.IApplyController, Main.IMVCController
	{
		ILineScatterLayerContentsView View { get; set; }

		void EhView_DataAvailableBeforeExpand(TreeNode node);

		void EhView_ContentsMeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e);
		void EhView_ContentsDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e);
		void EhView_ContentsDoubleClick(int selidx);

		void EhView_PutData(System.Collections.Hashtable hashtable);
		void EhView_PullDataClick(int[] selidxs);

		void EhView_ListSelUpClick(int[] selidxs);
		void EhView_SelDownClick(int[] selidxs);

		void EhView_GroupClick(int[] selidx);
		void EhView_UngroupClick(int[] selidxs);


	}

	public interface ILineScatterLayerContentsView : Main.IMVCView
	{

		ILineScatterLayerContentsController Controller { get; set; }

		System.Windows.Forms.Form Form	{	get; }

		void InitializeDataAvailable(TreeNode[] nodes);

		void Contents_SetItemCount(int itemcount);
		void Contents_SetSelected(int idx, bool bSelected);
		void Contents_Redraw();

	}
	#endregion
	/// <summary>
	/// Summary description for LineScatterLayerContentsController.
	/// </summary>
	public class LineScatterLayerContentsController : ILineScatterLayerContentsController
	{
		protected ILineScatterLayerContentsView m_View;
		protected Layer m_Layer;

		System.Collections.ArrayList m_ItemArray = new System.Collections.ArrayList();
		bool m_bDirty=false;

		public LineScatterLayerContentsController(Layer layer)
		{
			m_Layer = layer;
		}

		public void SetDirty()
		{
			m_bDirty=true;
		}

		public void SetElements(bool bInit)
		{
			// Available Items
			if(null!=View)
			{
				int nTables = App.Current.Doc.DataSet.Count;
				TreeNode[] nodes = new TreeNode[nTables];
				int i=0;
				foreach(Data.DataTable dt in App.Current.Doc.DataSet)
				{
					nodes[i++] = new TreeNode(dt.TableName,new TreeNode[1]{new TreeNode()});
				}

				View.InitializeDataAvailable(nodes);
			}

			// now fill the list box with all plot associations currently inside
			
			if(bInit)
			{
				m_ItemArray = new System.Collections.ArrayList();
				System.Collections.Hashtable addedItems = new System.Collections.Hashtable();
				for(int i=0;i<m_Layer.PlotItems.Count;i++)
				{
					PlotItem pa = m_Layer.PlotItems[i];
				
					if(!addedItems.ContainsKey(pa)) // if not already added to the list box
					{
						PlotGroup grp = m_Layer.PlotItems.GetPlotGroupOf(pa); // get the plot group of the item
				
						if(null!=grp) // if the item is member of a group
						{
							// add only one item to the list box, namely a PLCon group item with
							// all the members of that group
							PLCon plitem = new PLCon(grp); 
							m_ItemArray.Add(plitem);
							// add all the items in the group also to the list of added items 
							for(int j=0;j<grp.Count;j++)
							{
								addedItems.Add(grp[j],null);
							}
						}
						else // else if the item is not in a plot group
						{
							m_ItemArray.Add(new PLCon(pa));
							addedItems.Add(pa,null);
						}
					}				
				}
			}

			if(null!=View)
				View.Contents_SetItemCount(m_ItemArray.Count);


			// if initializing set dirty to false
			if(bInit)
				m_bDirty = false;
		}

		#region ILineScatterLayerContentsController Members

		public ILineScatterLayerContentsView View
		{ 
			get 
			{
				return m_View;
			}

			set
			{
				if(null!=m_View)
					m_View.Controller = null;
				
				m_View = value;

				if(null!=m_View)
				{
					m_View.Controller = this;
					SetElements(false); // set only the view elements, dont't initialize the variables
				}
			}
		}


		public void EhView_DataAvailableBeforeExpand(TreeNode node)
		{
			Data.DataTable dt = App.Current.Doc.DataSet[node.Text];
			if(null!=dt)
			{
				node.Nodes.Clear();
				for(int i=0;i<dt.ColumnCount;i++)
				{
					node.Nodes.Add(new TreeNode(dt[i].ColumnName));
				}
			}
			
		}



		public void EhView_PutData(System.Collections.Hashtable hashtable)
		{

			// first, put the selected node into the list, even if it is not checked

			foreach(MWCommon.MWTreeNodeWrapper tw in hashtable.Values)
			{
				TreeNode sn= tw.Node;
				if(null!=sn && null!=sn.Parent)
				{
					m_ItemArray.Add(new PLCon(sn.Parent.Text,sn.Text));
				}
			}

			View.Contents_SetItemCount(m_ItemArray.Count);
			SetDirty();
		}

		public void EhView_PullDataClick(int[] selidxs)
		{
			// for each selected item in the list, 
			// remove it from the list
			for(int i=selidxs.Length-1;i>=0;i--)
				m_ItemArray.RemoveAt(selidxs[i]);

			View.Contents_SetItemCount(m_ItemArray.Count);
			SetDirty();
		}


		
		public void EhView_ContentsMeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
		{
			PLCon item = (PLCon)m_ItemArray[e.Index];
			if(item.IsGroup)
				e.ItemHeight *= item.m_Group.Count;
		}


		public void EhView_ContentsDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			int height = e.Bounds.Height;
			e.DrawBackground();
			PLCon item = (PLCon)this.m_ItemArray[e.Index];
			using(Brush brush = new SolidBrush(e.ForeColor))
			{
				if(item.IsGroup) // item is a group
				{
					height /= item.m_Group.Count;
					for(int i=0;i<item.m_Group.Count;i++)
					{
						string str = string.Format("g{0} {1}",e.Index,item.m_Group[i].ToString());
						e.Graphics.DrawString(str,
							e.Font,brush,0,e.Bounds.Top+i*height);
					}
				}
				else // item is not a group
				{
					e.Graphics.DrawString(item.ToString(),
						e.Font,brush,0,e.Bounds.Top);
				}
			}
		
		}

		public void EhView_ContentsDoubleClick(int selectedIndex)
		{
			PlotItem pa = ((PLCon)this.m_ItemArray[selectedIndex]).plotassociation;
			if(null!=pa)
			{
				PlotGroup plotGroup = m_Layer.PlotItems.GetPlotGroupOf(pa);
				LineScatterPlotStyleController.ShowPlotStyleDialog(View.Form,pa,plotGroup);
			}
		}


		public void EhView_ListSelUpClick(int[] selidxs)
		{
			// move the selected items upwards in the list
			ContentsListBox_MoveUpDown(-1,selidxs);
			SetDirty();
		}

		public void EhView_SelDownClick(int[] selidxs)
		{
			// move the selected items downwards in the list
			ContentsListBox_MoveUpDown(1,selidxs);
			SetDirty();
		}

		public void ContentsListBox_MoveUpDown(int iDelta, int[] selidxs)
		{
			int i;

			if(iDelta!=1 && iDelta!=-1)
				return;

			if(iDelta==-1 ) // move one position upwards
			{
				if(selidxs[0]==0) // if the first item is selected, we can't move upwards
				{
					return;
				}

			for(i=0;i<selidxs.Length;i++)
				{
					object helpSeg;
					int iSeg=selidxs[i];

					helpSeg = m_ItemArray[iSeg-1];
					m_ItemArray[iSeg-1] = m_ItemArray[iSeg];
					m_ItemArray[iSeg] = helpSeg;

					View.Contents_SetSelected(iSeg-1,true); // select upper item,
					View.Contents_SetSelected(iSeg,false); // deselect lower item
				}
			} // end if iDelta==-1
			else if(iDelta==1) // move one position down
			{
				if(selidxs[selidxs.Length-1]==m_ItemArray.Count-1)		// if last item is selected, we can't move downwards
				{
					return;
				}

				for(i=selidxs.Length-1;i>=0;i--)
				{
					object helpSeg;
					int iSeg=selidxs[i];

					helpSeg = m_ItemArray[iSeg+1];
					m_ItemArray[iSeg+1]=m_ItemArray[iSeg];
					m_ItemArray[iSeg]=helpSeg;

					View.Contents_SetSelected(iSeg+1,true);
					View.Contents_SetSelected(iSeg,false);
				}
			} // end if iDelta==1
		}
	

		public void EhView_GroupClick(int[] selidxs)
		{
			// retrieve the selected items
			if(selidxs.Length<2)
				return; // we cannot group anything if no or only one item is selected

			// look, if one of the selected items is a plot group
			// if found, use this group and add the remaining items to this
			PLCon foundgroup=null;
			int   foundindex=-1;
			int i;
			for(i=0;i<selidxs.Length;i++)
			{
				foundgroup = (PLCon)m_ItemArray[selidxs[i]];
				if(foundgroup.IsGroup)
				{
					foundindex = selidxs[i];
					break;
				}
				foundgroup=null; // set to null to indicate not a group item
			}


			// if a group was found use this to add the remaining items
			// else use a new PLCon to add the items to
			PLCon addgroup = null!=foundgroup ? foundgroup : new PLCon(new PLCon[0]); 
			// now add the remaining selected items to the found group
			for(i=0;i<selidxs.Length;i++)
			{
				if(selidxs[i]==foundindex) continue; // don't add the found group to itself
				PLCon item = (PLCon)m_ItemArray[selidxs[i]];
				if(item.IsGroup) // if it is a group, add the members of the group to avoid more than one recursion
				{
					for(int j=0;j<item.m_Group.Count;j++)
						addgroup.m_Group.Add(item.m_Group[i]);
				}
				else // item to add is not a group
				{
					addgroup.m_Group.Add(item);
				}
			} // end for

			// now all items are in the new group

			// so update the list box:
			// delete all items except of the found group

			if(null!=foundgroup)
			{
				for(i=selidxs.Length-1;i>=0;i--) // step from end of list because items shift away if removing some items
				{
					if(selidxs[i]==foundindex)
					{
						m_ItemArray[selidxs[i]]=addgroup; // this is only a trick to force measuring the item again
						continue; // don't add the found group to itself
					}
					m_ItemArray.RemoveAt(selidxs[i]);
				}
			}
			else // if no previous group was found, replace first selected item by the group
			{
				m_ItemArray[selidxs[0]]= addgroup;
				// remove the remaining items
				for(i=selidxs.Length-1;i>=1;i--)
				{
					m_ItemArray.RemoveAt(selidxs[i]);
				}
			}
			View.Contents_SetItemCount(m_ItemArray.Count);
			SetDirty();
		}

		public void EhView_UngroupClick(int[] selidxs)
		{
			// retrieve the selected items
			if(selidxs.Length<1)
				return; // we cannot ungroup anything if nothing selected

			for(int i=selidxs.Length-1;i>=0;i--)
			{
				PLCon item = (PLCon)m_ItemArray[selidxs[i]];
			
				if(item.IsGroup)
				{
					// insert all items contained in that group in the next position
					for(int j=item.m_Group.Count-1;j>=1;j--)
					{
						m_ItemArray.Insert(selidxs[i]+1,item.m_Group[j]);
					}
					// and replace the group item by the first item of that group
					m_ItemArray[selidxs[i]] = item.m_Group[0];
				}
			} // end for
			View.Contents_SetItemCount(m_ItemArray.Count);
			SetDirty();
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			// TODO:  Add LineScatterLayerContentsController.Apply implementation
			return false;
		}

		#endregion

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return View;
			}
			set
			{
				View = value as ILineScatterLayerContentsView;
			}
		}

		#endregion

		#region Item class
		public class PLCon
		{
			public string table, column; // holds either name of table and column if freshly added
			public PlotItem plotassociation;   // or the plot association itself in case of existing PlotAssociations
			
			// m_Group holds (in PLCon items) information about the group members
			public PLCon.Collection m_Group; 
			//  m_OriginalGroup is set to the original plot group in case it exists before
			public PlotGroup m_OriginalGroup=null;

			public PLCon(string table, string column)
			{
				this.table=table;
				this.column=column;
				this.plotassociation=null;
				this.m_Group=null;
			}
			public PLCon(PlotItem pa)
			{
				this.table=null;
				this.column=null;
				this.plotassociation=pa;
				this.m_Group=null;
			}

			public PLCon(PLCon[] array)
			{
				this.table=null;
				this.column=null;
				this.plotassociation=null;
				this.m_OriginalGroup = null;
				this.m_Group = new PLCon.Collection();
				this.m_Group.AddRange(array);
			}

			public PLCon(PlotGroup grp)
			{
				this.table=null;
				this.column=null;
				this.plotassociation=null;

				this.m_OriginalGroup = grp;
				this.m_Group = new PLCon.Collection();
				for(int i=0;i<grp.Count;i++)
					m_Group.Add(new PLCon(grp[i]));
			}


			public bool IsValid
			{
				get { return null!=plotassociation || (null!=table && null!=column); }
			}

			public bool IsSingleNewItem
			{
				get { return null==plotassociation && null==m_Group; }
			}
			public bool IsSingleKnownItem
			{
				get { return null!=plotassociation && null==m_Group; }
			}
			public bool IsGroup
			{
				get { return null!=m_Group && m_Group.Count>0; }
			}
			public bool IsUnchangedOldGroup
			{
				get 
				{
					if(!IsGroup)
						return false;
				
					if(null==m_OriginalGroup)
						return false; // a original group must exist

					// and the counts of the original group and the m_Group Collection have to match
					if(m_OriginalGroup.Count!=m_Group.Count)
						return false;

					// and all items in that original group have to match the items in
					// the m_Group Collection
					for(int i=0;i<m_OriginalGroup.Count;i++)
					{
						if(!object.ReferenceEquals(m_OriginalGroup[i],m_Group[i].plotassociation))
							return false;
					}
					
					return true; // if all conditions fullfilled, it is unchanged
				}
			}

			public bool IsChangedOldGroup
			{
				get
				{
					return IsGroup && (null!=m_OriginalGroup) && (!IsUnchangedOldGroup);
				}
			}
			public bool IsNewGroup
			{
				get { return IsGroup && (null==m_OriginalGroup); }
			}

			public override string ToString()
			{
				if(null!=plotassociation)
					return plotassociation.GetName(0);
				else if(table!=null && column!=null)
					return table+"\\"+this.column;
				else
					return "<no more available>";
					
			}

			public class Collection : Altaxo.Data.CollectionBase
			{
				public void Add(PLCon item)
				{
					base.InnerList.Add(item);
				}

				public void AddRange(PLCon[] array)
				{
					base.InnerList.AddRange(array);
				}

				public PLCon this[int i]
				{
					get { return (PLCon)base.InnerList[i]; }
				}

			}
		} // end class PLCon

		#endregion
	}
}
