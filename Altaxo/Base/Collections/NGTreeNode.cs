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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a collection of <see cref="NGTreeNode" />.
  /// </summary>
  public interface NGTreeNodeCollection : IList<NGTreeNode>
  {
    /// <summary>
    /// Adds a bunch of nodes.
    /// </summary>
    /// <param name="nodes">Array of nodes to add to this collection.</param>
    void AddRange(NGTreeNode[] nodes);
  }

  // Represents a non GUI tree node that can be used for interfacing/communication with Gui components.
  public class NGTreeNode
  {
    /// <summary>
    /// Can be used by the application to get a connection to document items.
    /// Do not use this for Gui items, the <c>GuiTag</c> can be used for this purpose.
    /// </summary>
    public object Tag;

    /// <summary>
    /// Can be used by some GUI to get a connection to GUI elements.
    /// </summary>
    public object GuiTag;

    /// <summary>
    /// Text a Gui node can display.
    /// </summary>
    public string Text;

    /// <summary>
    /// Collection of child nodes.
    /// </summary>
    MyNGTreeNodeCollection _nodes;

    /// <summary>
    /// Parent node.
    /// </summary>
    NGTreeNode _parent;

    /// <summary>
    /// Parent tree node.
    /// </summary>
    public NGTreeNode Parent { get { return _parent; } }

    /// <summary>
    /// Empty constructor.
    /// </summary>
    public NGTreeNode()
    {
    }

    /// <summary>
    /// Constructor that initializes the text of the tree node.
    /// </summary>
    /// <param name="text">Text for the tree node.</param>
    public NGTreeNode(string text)
    {
      Text = text;
    }

    /// <summary>
    /// Constructor that initializes the text of the tree node and adds a range of elements to it.
    /// </summary>
    /// <param name="text">Text for the tree node.</param>
    /// <param name="nodes">Array of child nodes.</param>
    public NGTreeNode(string text, NGTreeNode[] nodes)
    {
      Text = text;
      Nodes.AddRange(nodes);
    }

    /// <summary>
    /// Collection of the child nodes of this node.
    /// </summary>
    public NGTreeNodeCollection Nodes
    {
      get
      {
        if (null == _nodes)
          _nodes = new MyNGTreeNodeCollection(this);

        return _nodes; 
      }
    }

    /// <summary>
    /// Frees this node, i.e. removes the node from it's parent collection (and set the parent node to <c>null</c>.
    /// </summary>
    public void Remove()
    {
      if (_parent != null)
        _parent.Nodes.Remove(this);
    }

    /// <summary>
    /// The level in the hierarchy. Nodes that have no parent return a level of 0, those with a parent return a level of 1, those with parent and
    /// grand parent a level of 2 and so on.
    /// </summary>
    public int Level
    {
      get
      {
        return _parent == null ? 0 : 1 + _parent.Level;
      }
    }

    /// <summary>
    /// Return the index in the parent's node collection or -1 if there is no parent.
    /// </summary>
    public int Index
    {
      get
      {
        return _parent == null ? -1 : _parent.Nodes.IndexOf(this);
      }
    }

    /// <summary>
    /// Returns the hierarchy of indices, i.e. the indices beginning with the root node collection and ending
    /// with the index in the nodes parent collection.
    /// </summary>
    public int[] HierarchyIndices
    {
      get
      {
        int[] result = new int[this.Level];
        NGTreeNode n = this;
        for (int i = result.Length - 1; i >= 0; i--)
        {
          result[i] = n.Index;
          n = n.Parent;
        }

        return result;
      }
    }

      /// <summary>
      /// Return the root node belonging to this node. If the node has no parent, the node itself is returned.
      /// </summary>
      public NGTreeNode RootNode
    {
      get
      {
        return null == _parent ? this : _parent.RootNode;
      }
    }

    #region Filtering

    /// <summary>
    /// If the <c>nodes</c> array contain both some nodes and their childs (or relatives up in the hierarchie), those childs are removed and only
    /// the nodes with the lowest level in the hierarchy are returned.
    /// </summary>
    /// <param name="nodes">Collection of nodes</param>
    /// <returns>Only the nodes who have no parent (or grand parent and so on) in the collection.</returns>
    public static NGTreeNode[] FilterIndependentNodes(NGTreeNode[] nodes)
    {
      System.Collections.Hashtable hash = new System.Collections.Hashtable();
      for (int i = 0; i < nodes.Length; i++)
        hash.Add(nodes[i], null);

      List<NGTreeNode> result = new List<NGTreeNode>();
      for (int i = 0; i < nodes.Length; i++)
      {
        bool isContained = false;
        for(NGTreeNode currNode = nodes[i].Parent;currNode!=null;currNode = currNode.Parent)
        {
          if (hash.ContainsKey(currNode))
          {
            isContained = true;
            break;
          }
        }
        if (!isContained)
          result.Add(nodes[i]);
      }
      return result.ToArray();
    }

    /// <summary>
    /// Returns only the nodes with the lowest hierarchie level.
    /// </summary>
    /// <param name="nodes">Array of nodes.</param>
    /// <returns>Only those nodes wich have the lowest hierarchy level.</returns>
    public static NGTreeNode[] FilterLowestLevelNodes(NGTreeNode[] nodes)
    {
      int level = int.MaxValue;
      foreach (NGTreeNode node in nodes)
        level = Math.Min(node.Level, level);

      List<NGTreeNode> list = new List<NGTreeNode>();
      foreach (NGTreeNode node in nodes)
        if (level == node.Level)
          list.Add(node);

      return list.ToArray();
    }

    /// <summary>
    /// Determines if all nodes in the array have the same parent.
    /// </summary>
    /// <param name="nodes">Array of nodes.</param>
    /// <returns>True if all nodes have the same parent. If the array is empty or contains only one element, true is returned.
    /// If all nodes have no parent (Parent==null), true is returned as well.</returns>
    public static bool HaveSameParent(NGTreeNode[] nodes)
    {
      if (nodes.Length <=1)
        return true;

      NGTreeNode parent = nodes[0].Parent;
      for (int i = 1; i < nodes.Length; i++)
        if (nodes[i].Parent != parent)
          return false;

      return true;
    }

    /// <summary>
    /// The nodes in the array are sorted by order, i.e. by there hierarchy indices.
    /// </summary>
    /// <param name="nodes">Nodes to sort. On return, contains the same nodes, but in order.</param>
    /// <remarks>
    /// Presume you have some nodes that are noted by their indices: 1, 2, 3, 1.1, 1.2, 3.1, 3.2
    /// <para>The sort by order would sort these nodes in the following order:</para>
    /// <para>1, 1.1, 1.2, 2, 3, 3.1, 3.2</para>.
    /// </remarks>
    public static void SortByOrder(NGTreeNode[] nodes)
    {
      SortedDictionary<int[], NGTreeNode> dic = new SortedDictionary<int[], NGTreeNode>(new IntArrayComparer());
      foreach (NGTreeNode node in nodes)
        dic.Add(node.HierarchyIndices, node);

      int i=0;
      foreach (KeyValuePair<int[], NGTreeNode> item in dic)
        nodes[i++] = item.Value;
    }

    private class IntArrayComparer : IComparer<int[]>
    {

      #region IComparer<int[]> Members

      public int Compare(int[] x, int[] y)
      {
        int len = Math.Min(x.Length, y.Length);
        for(int i=0;i<len;i++)
        {
          if(x[i]!=y[i])
            return x[i]<y[i] ? -1 : 1;
        }

        if(x.Length!=y.Length)
          return x.Length<y.Length ? -1 : 1;

        return 0;
      }

      #endregion
    }

    #endregion

    #region Moving


    /// <summary>
    /// This procedure will move up or move down some nodes in the tree.
    /// </summary>
    /// <param name="iDelta">Number of movement steps. Value less than zero will move up the nodes in the tree, values greater null will move down the nodes in the tree.</param>
    /// <param name="selNodes">Nodes to move.</param>
    /// <remarks>The following assumptions must be fullfilled:
    /// <para>First, the nodes are filtered: If the array contain both a parent node and child nodes of this parent node,
    /// the child nodes will be not moved.</para>
    /// <para>The remaining nodes must have the same parent, otherwise an exception is thrown.</para>
    /// </remarks>
    static public void MoveUpDown(int iDelta, NGTreeNode[] selNodes)
    {
      if (iDelta == 0 || selNodes==null || selNodes.Length==0)
        return;

      selNodes = FilterLowestLevelNodes(selNodes);
      if(!HaveSameParent(selNodes))
        throw new ArgumentException("The nodes in the array have not the same parent, which is neccessary for moving operations");

      System.Diagnostics.Debug.Assert(selNodes.Length > 0);


      NGTreeNode parent = selNodes[0].Parent;
      if(parent==null)
        throw new ArgumentException("Parent of the nodes is null");
      
     SortByOrder(selNodes);

     if (iDelta < 0)
     {
       for (int i = 0; i < (-iDelta); i++)
         MoveUp(selNodes, parent);
     }
     else
     {
       for (int i = 0; i < iDelta; i++)
         MoveDown(selNodes, parent);
     }
    }

    static void MoveUp(NGTreeNode[] selNodes, NGTreeNode parent)
    {
      if (selNodes[0].Index == 0) // if the first item is selected, we can't move upwards
        return;

      for (int i = 0; i < selNodes.Length; i++)
      {
        int idx = selNodes[i].Index;
        parent._nodes.Swap(idx, idx - 1);
      }
    }
    

    static void MoveDown(NGTreeNode[] selNodes, NGTreeNode parent)
    {
      if (selNodes[selNodes.Length - 1].Index == parent.Nodes.Count - 1)    // if last item is selected, we can't move downwards
        return;

      for (int i = selNodes.Length - 1; i >= 0; i--)
      {
        int idx = selNodes[i].Index;
        parent._nodes.Swap(idx, idx + 1);
      }
    } 
    

    #endregion

    #region MyNGTreeNodeCollection
    private class MyNGTreeNodeCollection : NGTreeNodeCollection, IList<NGTreeNode>
    {
     NGTreeNode _parent;
     List<NGTreeNode> _list;

     public MyNGTreeNodeCollection(NGTreeNode parent)
     {
       this._parent = parent;
       _list = new List<NGTreeNode>();
     }

      public void Add(NGTreeNode node)
      {
        if (node._parent != null)
          throw new ApplicationException("Parent of the node is not null. Please remove the node before adding it");
        node._parent = _parent;
        _list.Add(node);
      }

     public void AddRange(NGTreeNode[] nodes)
     {
       foreach (NGTreeNode n in nodes)
         Add(n);
     }

      public void Swap(int i, int j)
      {
        if (i < 0 || i >= Count)
          throw new ArgumentOutOfRangeException("i");
        if (j < 0 || j >= Count)
          throw new ArgumentOutOfRangeException("j");

        NGTreeNode node_i = _list[i];
        _list[i] = _list[j];
        _list[j] = node_i;
      }

      #region ICollection<NGTreeNode> Members


      public void Clear()
      {
        _list.Clear();
      }

      public bool Contains(NGTreeNode item)
      {
        return _list.Contains(item);
      }

      public void CopyTo(NGTreeNode[] array, int arrayIndex)
      {
        _list.CopyTo(array, arrayIndex);
      }

      public int Count
      {
        get { return _list.Count; }
      }

      public bool IsReadOnly
      {
        get { return false; }
      }

      public bool Remove(NGTreeNode item)
      {
      if(_list.Remove(item))
      {
        item._parent = null;
        return true;
      }
      return false;
      }

      #endregion

      #region IEnumerable<NGTreeNode> Members

      public IEnumerator<NGTreeNode> GetEnumerator()
      {
        return _list.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return _list.GetEnumerator();
      }

      #endregion

      #region IList<NGTreeNode> Members

      public int IndexOf(NGTreeNode item)
      {
        return _list.IndexOf(item);
      }

      public void Insert(int index, NGTreeNode item)
      {
        if (item._parent != null)
          throw new ApplicationException("Item must be removed from bthe parent collection before it can be inserted here");
        _list.Insert(index,item);
      }

      public void RemoveAt(int index)
      {
        NGTreeNode node = _list[index];
        node._parent = null;
        _list.RemoveAt(index);
      }

      public NGTreeNode this[int index]
      {
        get
        {
          return _list[index];
        }
        set
        {
          if (value._parent != null)
            throw new ApplicationException("Item must be removed from the parent collection before it can be inserted here");
          NGTreeNode oldNode = _list[index];
          oldNode._parent = null;
          value._parent = _parent;
          _list[index] = value;
        }
      }

      #endregion
    }
    #endregion
  }

 
}
