using System;
using System.Windows.Forms;
using System.Drawing;


namespace Altaxo.Graph
{
	#region Interfaces
	public interface ILineScatterLayerContentsController : Gui.IApplyController, Main.IMVCController
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

		/// <summary>
		/// Get/sets the controller of this view.
		/// </summary>
		ILineScatterLayerContentsController Controller { get; set; }

		/// <summary>
		/// Gets the hosting parent form of this view.
		/// </summary>
		System.Windows.Forms.Form Form	{	get; }

		/// <summary>
		/// Initializes the treeview of available data with content.
		/// </summary>
		/// <param name="nodes"></param>
		void DataAvailable_Initialize(TreeNode[] nodes);

		/// <summary>
		/// Clears all selection from the DataAvailable tree view.
		/// </summary>
		void DataAvailable_ClearSelection();

		/// <summary>
		/// Initializes the content list box by setting the number of items.
		/// </summary>
		/// <param name="itemcount">Number of items.</param>
		void Contents_SetItemCount(int itemcount);
		/// <summary>
		/// Select/deselect the item number idx in the content list box.
		/// </summary>
		/// <param name="idx">Index of the item to select/deselect.</param>
		/// <param name="bSelected">True if the item should be selected, false if it should be deselected.</param>
		void Contents_SetSelected(int idx, bool bSelected);

		/// <summary>
		/// Invalidates the items idx1 and idx2 and has to force the MeasureItem call for these two items.
		/// </summary>
		/// <param name="idx1">Index of the first item to invalidate.</param>
		/// <param name="idx2">Index of the second item to invalidate.</param>
		void Contents_InvalidateItems(int idx1, int idx2);

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
			SetElements(true);
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

				View.DataAvailable_Initialize(nodes);
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


		private PlotItem NewPlotItemFromPLCon(PLCon item)
		{
			if(!item.IsSingleNewItem)
				return null;

			// create a new plotassociation from the column
			// first, get the y column from table and name
			Data.DataTable tab = App.Current.Doc.DataSet[item.table];
			if(null!=tab)
			{
				Data.DataColumn ycol = tab[item.column];
				if(null!=ycol)
				{
					Data.DataColumn xcol = tab.FindXColumnOfGroup(ycol.Group);
					if(null==xcol)
						return  new Graph.XYDataPlot(new PlotAssociation(new Altaxo.Data.IndexerColumn(),ycol),new LineScatterPlotStyle());
					else
						return  new Graph.XYDataPlot(new PlotAssociation(xcol,ycol),new LineScatterPlotStyle());
					// now enter the plotassociation back into the layer's plot association list
				}
			}
			return null;
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
				TreeNode[] toadd = new TreeNode[dt.ColumnCount];
				for(int i=0;i<toadd.Length;i++)
				{
					toadd[i] = new TreeNode(dt[i].ColumnName);
				}
				node.Nodes.AddRange(toadd);
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
			View.DataAvailable_ClearSelection();
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
			PlotItem pa = ((PLCon)this.m_ItemArray[selectedIndex]).PlotItem;
			if(null!=pa)
			{
				PlotGroup plotGroup = m_Layer.PlotItems.GetPlotGroupOf(pa);
				//LineScatterPlotStyleController.ShowPlotStyleDialog(View.Form,pa,plotGroup);
				Gui.DialogFactory.ShowLineScatterPlotStyleAndDataDialog(View.Form,pa,plotGroup);
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

					View.Contents_InvalidateItems(iSeg-1,iSeg);
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

					View.Contents_InvalidateItems(iSeg+1,iSeg);
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

			if(!this.m_bDirty)
				return true; // not dirty - so no need to apply something

			m_Layer.PlotItems.Clear();

			// now we must get all items out of the listbox and look
			// for which items are new or changed
			for(int i=0;i<m_ItemArray.Count;i++)
			{
				PLCon item = (PLCon)m_ItemArray[i];
				PlotItem plotitem=null;
				
				if(item.IsSingleNewItem)
				{
					plotitem = this.NewPlotItemFromPLCon(item);
					if(null!=plotitem)
					{
						m_Layer.PlotItems.Add(plotitem);
					}
				}
				else if(item.IsSingleKnownItem)
				{
					plotitem = item.PlotItem;
					m_Layer.PlotItems.Add(plotitem);
				}
				else if(item.IsUnchangedOldGroup)
				{
					// if the group was not changed, add all group members to the
					// plotassociation collection and add the group to the group list
					for(int j=0;j<item.m_Group.Count;j++)
					{
						PLCon member = (PLCon)item.m_Group[j];
						m_Layer.PlotItems.Add(member.PlotItem);
					} // end for
					m_Layer.PlotItems.Add(item.m_OriginalGroup); // add the unchanged group back to the layer
				} // if item.IsUnchangedOldGroup
				else if(item.IsChangedOldGroup) // group exists before, but was changed
				{
					item.m_OriginalGroup.Clear();
					for(int j=0;j<item.m_Group.Count;j++)
					{
						PLCon member = (PLCon)item.m_Group[j];
						if(member.IsSingleKnownItem)
						{
							m_Layer.PlotItems.Add(member.PlotItem);
							item.m_OriginalGroup.Add(member.PlotItem);
						}
						else // than it is a single new item
						{
							plotitem = this.NewPlotItemFromPLCon(member);
							if(null!=plotitem)
							{
								m_Layer.PlotItems.Add(member.PlotItem);
								item.m_OriginalGroup.Add(plotitem);
							}
						}
					} // end for
					m_Layer.PlotItems.Add(item.m_OriginalGroup); // add the plot group back to the layer
				} // else if item.IsChangedOldGroup
				else if(item.IsNewGroup) // if it is a new group
				{
					// 1st) create a new PlotGroup
					PlotGroup newplotgrp = new PlotGroup(PlotGroupStyle.All);
					// if the group was not changed, add all group members to the
					// plotassociation collection and add the group to the group list
					for(int j=0;j<item.m_Group.Count;j++)
					{
						PLCon member = (PLCon)item.m_Group[j];
						if(member.IsSingleKnownItem)
						{
							m_Layer.PlotItems.Add(member.PlotItem);
							newplotgrp.Add(member.PlotItem);
						}
						else // than it is a single new item
						{
							plotitem = this.NewPlotItemFromPLCon(member);
							if(null!=plotitem)
							{
								m_Layer.PlotItems.Add(plotitem);
								newplotgrp.Add(plotitem);
							}
						}
					} // for all items in that new group
					m_Layer.PlotItems.Add(newplotgrp); // add the new plot group to the layer
				} // if it was a new group
			} // end for all items in the list box
			
			SetElements(true); // Reload the applied contents to make sure it is synchronized
			return true; // all ok
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
			public PlotItem PlotItem;   // or the plot association itself in case of existing PlotAssociations
			
			// m_Group holds (in PLCon items) information about the group members
			public PLCon.Collection m_Group; 
			//  m_OriginalGroup is set to the original plot group in case it exists before
			public PlotGroup m_OriginalGroup=null;

			public PLCon(string table, string column)
			{
				this.table=table;
				this.column=column;
				this.PlotItem=null;
				this.m_Group=null;
			}
			public PLCon(PlotItem pa)
			{
				this.table=null;
				this.column=null;
				this.PlotItem=pa;
				this.m_Group=null;
			}

			public PLCon(PLCon[] array)
			{
				this.table=null;
				this.column=null;
				this.PlotItem=null;
				this.m_OriginalGroup = null;
				this.m_Group = new PLCon.Collection();
				this.m_Group.AddRange(array);
			}

			public PLCon(PlotGroup grp)
			{
				this.table=null;
				this.column=null;
				this.PlotItem=null;

				this.m_OriginalGroup = grp;
				this.m_Group = new PLCon.Collection();
				for(int i=0;i<grp.Count;i++)
					m_Group.Add(new PLCon(grp[i]));
			}


			public bool IsValid
			{
				get { return null!=PlotItem || (null!=table && null!=column); }
			}

			public bool IsSingleNewItem
			{
				get { return null==PlotItem && null==m_Group; }
			}
			public bool IsSingleKnownItem
			{
				get { return null!=PlotItem && null==m_Group; }
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
						if(!object.ReferenceEquals(m_OriginalGroup[i],m_Group[i].PlotItem))
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
				if(null!=PlotItem)
					return PlotItem.GetName(0);
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
