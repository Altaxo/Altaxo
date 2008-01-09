#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui;
using Altaxo.Gui.Common;



namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface ILineScatterLayerContentsController : IMVCANController
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

    void EhView_CopyClipboard(NGTreeNode[] selNodes);
    void EhView_PasteClipboard();

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
    /// Initializes the content list box by setting the items.
    /// </summary>
    /// <param name="items">Collection of items.</param>
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
  /// Controls the content of a <see cref="PlotItemCollection" />
  /// </summary>
  [UserControllerForObject(typeof(PlotItemCollection))]
  [ExpectedTypeOfView(typeof(ILineScatterLayerContentsView))]
  public class LineScatterLayerContentsController : ILineScatterLayerContentsController
  {
    protected ILineScatterLayerContentsView _view;
    protected PlotItemCollection _doc;
    protected PlotItemCollection _originalDoc;

    NGTreeNode _plotItemsRootNode = new NGTreeNode();
    NGTreeNodeCollection _plotItemsTree;
    
    bool m_bDirty=false;
    UseDocument _useDocument;
    public UseDocument UseDocumentCopy { set { _useDocument = value; } }
    public LineScatterLayerContentsController()
    {
    }

    /// <summary>
    /// Initialize the controller with the document. If successfull, the function has to return true.
    /// </summary>
    /// <param name="args">The arguments neccessary to create the controller. Normally, the first argument is the document, the second can be the parent of the document and so on.</param>
    /// <returns>True if successfull, else false.</returns>
    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0)
        return false;
      _doc = _originalDoc = args[0] as PlotItemCollection;
      if (null == _originalDoc)
        return false;

      if (_useDocument == UseDocument.Copy)
        _doc = _originalDoc.Clone();

      _plotItemsTree = _plotItemsRootNode.Nodes;
      SetElements(true);
      return true;
    }

    public void SetDirty()
    {
      m_bDirty=true;
    }

    public void SetElements(bool bInit)
    {
      if (_doc == null)
        throw new ApplicationException("Doc was not set before!");

      // Available Items
      if(null!=View)
      {
        int nTables = Current.Project.DataTableCollection.Count;
        NGTreeNode no = new NGTreeNode();
        foreach(Data.DataTable dt in Current.Project.DataTableCollection)
        {
          NGTreeNode newnode = new NGTreeNode(dt.Name, new NGTreeNode[1] { new NGTreeNode() });
          newnode.Tag = dt;
          no.Nodes.Add( newnode );
        }

        View.DataAvailable_Initialize(no.Nodes);
      }

      // now fill the tree view  with all plot associations currently inside
      if(bInit)
      {
        _plotItemsTree.Clear();
        AddToNGTreeNode(_plotItemsRootNode, _doc);
      }

      if(null!=View)
        View.Contents_SetItems(_plotItemsTree);


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

    private void TransferTreeToDoc(NGTreeNode rootnode, PlotItemCollection picoll)
    {
      picoll.Clear();
      foreach (NGTreeNode node in rootnode.Nodes)
      {
        IGPlotItem item = (IGPlotItem)node.Tag;
        if (item is PlotItemCollection) // if this is a plot item collection
          TransferTreeToDoc(node, (PlotItemCollection)item);

        picoll.Add(item);
      }
    }


    static XYColumnPlotItem FindFirstXYColumnPlotItem(PlotItemCollection coll)
    {
     
      // search in our document for the first plot item that is XYColumnPlotItem,
      // we need this as template style
      foreach (IGPlotItem pi in coll)
      {
        if (pi is PlotItemCollection)
        {
          XYColumnPlotItem result = FindFirstXYColumnPlotItem(pi as PlotItemCollection);
          if (result != null)
            return result;
        }
        else if (pi is XYColumnPlotItem)
        {
          return pi as XYColumnPlotItem;
        }
      }
      return null;
    }

    private IGPlotItem CreatePlotItem(string tablename, string columnname)
    {
      if (string.IsNullOrEmpty(tablename) || string.IsNullOrEmpty(columnname))
        return null;

     
     

      // create a new plotassociation from the column
      // first, get the y column from table and name
      Data.DataTable tab = Current.Project.DataTableCollection[tablename];
      if(null!=tab)
      {
        Data.DataColumn ycol = tab[columnname];
        if(null!=ycol)
        {
          Data.DataColumn xcol = tab.DataColumns.FindXColumnOf(ycol);

          // search in our document for the first plot item that is an XYColumnPlotItem,
          // we need this as template style
          XYColumnPlotItem templatePlotItem = FindFirstXYColumnPlotItem(_doc);
          G2DPlotStyleCollection templatePlotStyle;
          if (null != templatePlotItem)
          {
            templatePlotStyle = templatePlotItem.Style.Clone();
          }
          else // there is no item that can be used as template
          {
            int numRows = Math.Min(ycol.Count, xcol.Count);
            if (numRows < 100)
            {
              templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter);
            }
            else
            {
              templatePlotStyle = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line);
            }
          }


          XYColumnPlotItem result;
          if(null==xcol)
            result = new XYColumnPlotItem(new XYColumnPlotData(new Altaxo.Data.IndexerColumn(),ycol),templatePlotStyle);
          else
            result = new XYColumnPlotItem(new XYColumnPlotData(xcol,ycol),templatePlotStyle);


          return result;
        }
      }
      return null;
    }






    #region ILineScatterLayerContentsController Members

    public ILineScatterLayerContentsView View
    { 
      get 
      {
        return _view;
      }

      set
      {
        if(null!=_view)
          _view.Controller = null;
        
        _view = value;

        if(null!=_view)
        {
          _view.Controller = this;
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


    /// <summary>
    /// Puts the selected data columns into the plot content.
    /// </summary>
    /// <param name="selNodes"></param>
    public void EhView_PutData(NGTreeNode[] selNodes)
    {

      // first, put the selected node into the list, even if it is not checked
      foreach (NGTreeNode sn  in selNodes)
      {
        if(null!=sn.Parent && (sn.Parent.Tag is Altaxo.Data.DataTable))
        {
          IGPlotItem newItem = this.CreatePlotItem(sn.Parent.Text,sn.Text);
          if (null == newItem)
            continue;

          _doc.Add(newItem);
          NGTreeNode newNode = new NGTreeNode();
          newNode.Text = newItem.GetName(2);
          newNode.Tag = newItem;
          _plotItemsTree.Add(newNode);
        }
      }

      View.Contents_SetItems(_plotItemsTree);
      View.DataAvailable_ClearSelection();
      SetDirty();
    }

    public void EhView_PullDataClick(NGTreeNode[] selNodes)
    {
      View.Contents_RemoveItems(selNodes);

      foreach (NGTreeNode node in selNodes)
        node.Remove();

      TransferTreeToDoc(_plotItemsRootNode, _doc);
      View.Contents_SetItems(_plotItemsTree);
      SetDirty();
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
        TransferTreeToDoc(_plotItemsRootNode, _doc);
        View.Contents_SetItems(this._plotItemsTree);
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

          helpSeg = _plotItemsTree[iSeg-1];
          _plotItemsTree[iSeg-1] = _plotItemsTree[iSeg];
          _plotItemsTree[iSeg] = helpSeg;

          View.Contents_InvalidateItems(iSeg-1,iSeg);
          View.Contents_SetSelected(iSeg-1,true); // select upper item,
          View.Contents_SetSelected(iSeg,false); // deselect lower item
        }
      } // end if iDelta==-1
      else if(iDelta==1) // move one position down
      {
        if(selidxs[selidxs.Length-1]==_plotItemsTree.Count-1)    // if last item is selected, we can't move downwards
        {
          return;
        }

        for(i=selidxs.Length-1;i>=0;i--)
        {
          NGTreeNode helpSeg;
          int iSeg=selidxs[i];

          helpSeg = _plotItemsTree[iSeg+1];
          _plotItemsTree[iSeg+1]=_plotItemsTree[iSeg];
          _plotItemsTree[iSeg]=helpSeg;

          View.Contents_InvalidateItems(iSeg+1,iSeg);
          View.Contents_SetSelected(iSeg+1,true);
          View.Contents_SetSelected(iSeg,false);
        }
      } // end if iDelta==1

      TransferTreeToDoc(_plotItemsRootNode, _doc);
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
       else // if we found no group to add to, we have to create a new group
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
         _plotItemsRootNode.Nodes.Add(newNode);
       }
      // now all items are in the new group

       TransferTreeToDoc(_plotItemsRootNode, _doc);
      // so update the list box:
       View.Contents_SetItems(this._plotItemsTree);

      SetDirty();
    }

    public void EhView_UngroupClick(NGTreeNode[] selNodes)
    {
      // retrieve the selected items
      if(selNodes.Length<1)
        return; // we cannot ungroup anything if nothing selected

      selNodes = NGTreeNode.FilterIndependentNodes(selNodes);

      for(int i=0;i<selNodes.Length;i++)
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

      TransferTreeToDoc(_plotItemsRootNode, _doc);
      View.Contents_SetItems(_plotItemsTree);
      SetDirty();

    }



    public void EhView_ContentsDoubleClick(NGTreeNode selNode)
    {
      IGPlotItem pi = selNode.Tag as IGPlotItem;
      if (null != pi)
      {
        if (pi is PlotItemCollection)
        {
          // show not the dialog for PlotItemCollection, but only those for the group styles into that collection
          Current.Gui.ShowDialog(new object[] { ((PlotItemCollection)pi).GroupStyles }, pi.Name);
        }
        else
        {
          Current.Gui.ShowDialog(new object[] { pi }, pi.GetName(2),true);
        }
      }
    }
    
    public void EhView_EditRangeClick(NGTreeNode[] selNodes)
    {
      if (selNodes.Length == 1 && selNodes[0].Tag is IGPlotItem)
        EhView_ContentsDoubleClick(selNodes[0]);
    }

    public void EhView_PlotAssociationsClick(NGTreeNode[] selNodes)
    {
      if (selNodes.Length == 1)
        EhView_ContentsDoubleClick(selNodes[0]);
    }

    public void EhView_CopyClipboard(NGTreeNode[] selNodes)
    {
      System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();

      ArrayList objectList = new ArrayList();

      foreach (NGTreeNode o in selNodes)
      {

          objectList.Add(o.Tag);
      }

      dao.SetData("Altaxo.Graph.PlotItemList", objectList);

      // Test code to test if the object list can be serialized
#if false
      {
      System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binform = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        binform.Serialize(stream, objectList);

        stream.Flush();
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        object obj = binform.Deserialize(stream);
        stream.Close();
        stream.Dispose();
      }
#endif

      // now copy the data object to the clipboard
      System.Windows.Forms.Clipboard.SetDataObject(dao, true);
    }
    public void EhView_PasteClipboard()
    {
      System.Windows.Forms.DataObject dao = System.Windows.Forms.Clipboard.GetDataObject() as System.Windows.Forms.DataObject;

      string[] formats = dao.GetFormats();
      if (dao.GetDataPresent("Altaxo.Graph.PlotItemList"))
      {
        object obj = dao.GetData("Altaxo.Graph.PlotItemList");

        // if at this point obj is a memory stream, you probably have forgotten the deserialization constructor of the class you expect to deserialize here
        if (obj is ArrayList)
        {
          ArrayList list = (ArrayList)obj;
          foreach (object item in list)
          {
            if (item is IGPlotItem)
            {
              IGPlotItem newItem = item as IGPlotItem;
              _doc.Add(newItem);

              NGTreeNode newNode = new NGTreeNode();
              newNode.Text = newItem.GetName(2);
              newNode.Tag = newItem;
              _plotItemsTree.Add(newNode);
            }
          }

          View.Contents_SetItems(_plotItemsTree);
          SetDirty();

        }

      }
    }

    #endregion

    #region IApplyController Members
    
    public bool Apply()
    {

      if(!this.m_bDirty)
        return true; // not dirty - so no need to apply something

      _originalDoc.Clear(); // first, clear all Plot items
      TransferTreeToDoc(_plotItemsRootNode, _originalDoc);

      if (_useDocument== UseDocument.Copy)
        _doc = _originalDoc.Clone();
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
      get { return this._originalDoc; }
    }

    #endregion

   
  }
}
