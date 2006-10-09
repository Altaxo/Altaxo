#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui;
using Altaxo.Gui.Common;



namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface ILineScatterLayerContentsController : IApplyController, IMVCController
  {
    ILineScatterLayerContentsView View { get; set; }

    void EhView_DataAvailableBeforeExpand(NGTreeNode node);

    void EhView_ContentsDoubleClick(NGTreeNode selNode);

    void EhView_PutData(NGTreeNode[] selNodes);
    void EhView_PullDataClick(NGTreeNode[] selNodes);

    void EhView_ListSelUpClick(NGTreeNode[] selNodes);
    void EhView_SelDownClick(NGTreeNode[] selNodes);

    void EhView_GroupClick(NGTreeNode[] selNodes);
    void EhView_UngroupClick(NGTreeNode[] selNodes);
    void EhView_EditRangeClick(NGTreeNode[] selNodes);
    void EhView_PlotAssociationsClick(NGTreeNode[] selNodes);

  }

  public interface ILineScatterLayerContentsView : IMVCView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    ILineScatterLayerContentsController Controller { get; set; }

    /// <summary>
    /// Gets the hosting parent form of this view.
    /// </summary>
    System.Windows.Forms.Form Form  { get; }

    /// <summary>
    /// Initializes the treeview of available data with content.
    /// </summary>
    /// <param name="nodes"></param>
    void DataAvailable_Initialize(NGTreeNodeCollection nodes);

    /// <summary>
    /// Clears all selection from the DataAvailable tree view.
    /// </summary>
    void DataAvailable_ClearSelection();

    /// <summary>
    /// Initializes the content list box by setting the number of items.
    /// </summary>
    /// <param name="itemcount">Number of items.</param>
    void Contents_SetItems(NGTreeNodeCollection items);

    void Contents_RemoveItems(NGTreeNode[] items);


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
    protected XYPlotLayer m_Layer;

    NGTreeNode m_RootNode = new NGTreeNode();
    NGTreeNodeCollection m_ItemArray;
    
    bool m_bDirty=false;

    public LineScatterLayerContentsController(XYPlotLayer layer)
    {
      m_Layer = layer;
      m_ItemArray = m_RootNode.Nodes;
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
        int nTables = Current.Project.DataTableCollection.Count;
        NGTreeNode no = new NGTreeNode();
        int i=0;
        foreach(Data.DataTable dt in Current.Project.DataTableCollection)
        {
          no.Nodes.Add( new NGTreeNode(dt.Name,new NGTreeNode[1]{new NGTreeNode()}));
        }

        View.DataAvailable_Initialize(no.Nodes);
      }

      // now fill the list box with all plot associations currently inside
      if(bInit)
      {
        m_ItemArray.Clear();
        AddToNGTreeNode(m_RootNode, m_Layer.PlotItems);
      }

      if(null!=View)
        View.Contents_SetItems(m_ItemArray);


      // if initializing set dirty to false
      if(bInit)
        m_bDirty = false;
    }


    private void AddToNGTreeNode(NGTreeNode node, PlotItemCollection picoll)
    {
      foreach (IGPlotItem pa in picoll)
      {
        if (pa is PlotItemCollection) // if this is a plot item collection
        {
          // add only one item to the list box, namely a PLCon group item with
          // all the members of that group
          NGTreeNode grpNode = new NGTreeNode();
          grpNode.Text = "PlotGroup";
          grpNode.Tag = pa;
          node.Nodes.Add(grpNode);
          // add all the items in the group also to the list of added items 
          AddToNGTreeNode(grpNode, (PlotItemCollection)pa);
        }
        else // else if the item is not in a plot group
        {
          NGTreeNode toAdd = new NGTreeNode();
          toAdd.Text = pa.GetName(2);
          toAdd.Tag = pa;
          node.Nodes.Add(toAdd);
        }
      }
    }

    private void AddToPlotItemCollection(PlotItemCollection picoll, NGTreeNode rootnode)
    {
      picoll.Clear();
      foreach (NGTreeNode node in rootnode.Nodes)
      {
        IGPlotItem item = (IGPlotItem)node.Tag;
        if (item is PlotItemCollection) // if this is a plot item collection
          AddToPlotItemCollection((PlotItemCollection)item, node);

        picoll.Add(item);

      }
    }


    private IGPlotItem NewPlotItemFromPLCon(string tablename, string columnname)
    {
      // create a new plotassociation from the column
      // first, get the y column from table and name
      Data.DataTable tab = Current.Project.DataTableCollection[tablename];
      if(null!=tab)
      {
        Data.DataColumn ycol = tab[columnname];
        if(null!=ycol)
        {
          Data.DataColumn xcol = tab.DataColumns.FindXColumnOf(ycol);
          if(null==xcol)
            return  new XYColumnPlotItem(new XYColumnPlotData(new Altaxo.Data.IndexerColumn(),ycol),new G2DPlotStyleCollection(LineScatterPlotStyleKind.Scatter));
          else
            return  new XYColumnPlotItem(new XYColumnPlotData(xcol,ycol),new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter));
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


    public void EhView_DataAvailableBeforeExpand(NGTreeNode node)
    {
      Data.DataTable dt = Current.Project.DataTableCollection[node.Text];
      if(null!=dt)
      {
        node.Nodes.Clear();
        NGTreeNode[] toadd = new NGTreeNode[dt.DataColumns.ColumnCount];
        for(int i=0;i<toadd.Length;i++)
        {
          toadd[i] = new NGTreeNode(dt[i].Name);
        }
        node.Nodes.AddRange(toadd);
      }
      
    }



    public void EhView_PutData(NGTreeNode[] selNodes)
    {

      // first, put the selected node into the list, even if it is not checked

      foreach (NGTreeNode sn  in selNodes)
      {
        if(null!=sn.Parent)
        {
          NGTreeNode newNode = new NGTreeNode();
          newNode.Text = sn.Text;
          newNode.Tag = this.NewPlotItemFromPLCon(sn.Parent.Text,sn.Text);
           m_ItemArray.Add(newNode);
        }
      }

      View.Contents_SetItems(m_ItemArray);
      View.DataAvailable_ClearSelection();
      SetDirty();
    }

    public void EhView_PullDataClick(NGTreeNode[] selNodes)
    {
      View.Contents_RemoveItems(selNodes);

      foreach(NGTreeNode node in selNodes)
        node.Remove();
     
      SetDirty();
    }





    public void EhView_ContentsDoubleClick(NGTreeNode selNode)
    {
      object tag = selNode.Tag;
      IGPlotItem pa = null;
      if (tag is IGPlotItem)
      {
        pa = tag as IGPlotItem;
      }
     
      if (null != pa)
      {
        DialogFactory.ShowPlotStyleAndDataDialog(View.Form, pa, pa.ParentCollection.GroupStyles);
      }
    }
    


    public void EhView_ListSelUpClick(NGTreeNode[] selNodes)
    {
      // move the selected items upwards in the list
      ContentsListBox_MoveUpDown(-1,selNodes);
      SetDirty();
    }

    public void EhView_SelDownClick(NGTreeNode[] selNodes)
    {
      // move the selected items downwards in the list
      ContentsListBox_MoveUpDown(1,selNodes);
      SetDirty();
    }

    public void ContentsListBox_MoveUpDown(int iDelta, NGTreeNode[] selNodes)
    {
      if (NGTreeNode.HaveSameParent(selNodes))
      {
        NGTreeNode.MoveUpDown(iDelta, selNodes);
        View.Contents_SetItems(this.m_ItemArray);
        SetDirty();
      }
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
          NGTreeNode helpSeg;
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
        if(selidxs[selidxs.Length-1]==m_ItemArray.Count-1)    // if last item is selected, we can't move downwards
        {
          return;
        }

        for(i=selidxs.Length-1;i>=0;i--)
        {
          NGTreeNode helpSeg;
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


    /// <summary>
    /// Group the selected nodes.
    /// </summary>
    /// <param name="selNodes"></param>
    public void EhView_GroupClick(NGTreeNode[] selNodes)
    {
      
      // retrieve the selected items
      if(selNodes.Length<2)
        return; // we cannot group anything if no or only one item is selected

      // look, if one of the selected items is a plot group
      // if found, use this group and add the remaining items to this
      int   foundindex=-1;
      for(int i=0;i<selNodes.Length;i++)
      {
        if(selNodes[i].Tag is PlotItemCollection)
        {
          foundindex = i;
          break;
        }
       }

      // if a group was found use this to add the remaining items
       if (foundindex >= 0)
       {
         for (int i = 0; i < selNodes.Length; i++)
           if (i != foundindex)
           {
             selNodes[i].Remove();
             selNodes[foundindex].Nodes.Add(selNodes[i]);
           }
       }
       // else use a new PLCon to add the items to
       else
       {
         
         NGTreeNode newNode = new NGTreeNode();
         newNode.Tag = new PlotItemCollection();
         newNode.Text = "PlotGroup";


         // now add the remaining selected items to the found group
         for (int i = 0; i < selNodes.Length; i++)
         {
           NGTreeNode node = selNodes[i];
           if (node.Nodes.Count > 0) // if it is a group, add the members of the group to avoid more than one recursion
           {
             while(node.Nodes.Count>0)
             {
               NGTreeNode addnode = node.Nodes[0];
               addnode.Remove();
               newNode.Nodes.Add(addnode);
             }
           }
           else // item to add is not a group
           {
             node.Remove();
             newNode.Nodes.Add(node);
           }
         } // end for
         m_RootNode.Nodes.Add(newNode);
       }
      // now all items are in the new group

      // so update the list box:
       View.Contents_SetItems(this.m_ItemArray);

      SetDirty();
    }

    public void EhView_UngroupClick(NGTreeNode[] selNodes)
    {
      // retrieve the selected items
      if(selNodes.Length<1)
        return; // we cannot ungroup anything if nothing selected

      selNodes = NGTreeNode.FilterIndependentNodes(selNodes);

      for(int i=selNodes.Length-1;i>=0;i--)
      {
        if (selNodes[i].Nodes.Count==0 && selNodes[i].Parent!=null && selNodes[i].Parent.Parent!=null)
        {
          NGTreeNode parent = selNodes[i].Parent;
          NGTreeNode grandParent = parent.Parent;
          selNodes[i].Remove();
          grandParent.Nodes.Add(selNodes[i]);

          if (parent.Nodes.Count == 0)
            parent.Remove();
        }
        else if (selNodes[i].Nodes.Count > 0 && selNodes[i].Parent != null)
        {
          NGTreeNode parent = selNodes[i].Parent;
          while(selNodes[i].Nodes.Count>0)
          {
            NGTreeNode no = selNodes[i].Nodes[0];
            no.Remove();
            parent.Nodes.Add(no);
          }
          selNodes[i].Remove();
        }
      } // end for
      View.Contents_SetItems(m_ItemArray);
      SetDirty();

    }


    public void EhView_EditRangeClick(NGTreeNode[] selNodes)
    {
      if (selNodes.Length == 1)
      {
        if (selNodes[0].Tag is IGPlotItem)
        {
          IGPlotItem pi = (IGPlotItem)selNodes[0].Tag;
          DialogFactory.ShowPlotStyleAndDataDialog(Current.MainWindow, pi, pi.ParentCollection.GroupStyles);
        }
      }
      
    }

    public void EhView_PlotAssociationsClick(NGTreeNode[] selNodes)
    {
      EhView_EditRangeClick(selNodes);
    }


    #endregion

    #region IApplyController Members
    
    public bool Apply()
    {

      if(!this.m_bDirty)
        return true; // not dirty - so no need to apply something

      m_Layer.PlotItems.Clear(); // first, clear all Plot items
      AddToPlotItemCollection(m_Layer.PlotItems, m_RootNode);
      
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

    public object ModelObject
    {
      get { return this.m_Layer; }
    }

    #endregion

   
  }
}
