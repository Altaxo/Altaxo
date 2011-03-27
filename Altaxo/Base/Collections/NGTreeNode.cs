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
using System.Collections.ObjectModel;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Represents a collection of <see cref="NGTreeNode" />.
  /// </summary>
	public interface NGTreeNodeCollection : IList<NGTreeNode>, System.Collections.Specialized.INotifyCollectionChanged
  {
    /// <summary>
    /// Adds a bunch of nodes.
    /// </summary>
    /// <param name="nodes">Array of nodes to add to this collection.</param>
    void AddRange(NGTreeNode[] nodes);

		/// <summary>
		/// Swap the nodes of indices i and j.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		void Swap(int i, int j);
  }

  // Represents a non GUI tree node that can be used for interfacing/communication with Gui components.
  public class NGTreeNode : System.ComponentModel.INotifyPropertyChanged
  {
		static NGTreeNode _dummyNode = new NGTreeNode();

		object _tag;
		object _guiTag;
		string _text;
		bool _isExpanded;
		bool _isSelected;

		/// <summary>
		/// Collection of child nodes.
		/// </summary>
		NGTreeNodeCollection _nodes;

		/// <summary>
		/// Parent node.
		/// </summary>
		NGTreeNode _parent;

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


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

		public NGTreeNode(bool lazyLoadChildren)
		{
			if (lazyLoadChildren)
			{
				_nodes = new MyColl3(this);
				_nodes.Add(_dummyNode);
			}
		}

		public bool AlwaysFalse
		{
			get
			{
				return false;
			}
			set
			{
				OnPropertyChanged("AlwaysFalse");
			}
		}


		protected virtual void OnPropertyChanged(string name)
		{
			if(null!=PropertyChanged)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
		}

    /// <summary>
    /// Can be used by the application to get a connection to document items.
    /// Do not use this for Gui items, the <c>GuiTag</c> can be used for this purpose.
    /// </summary>
		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				var oldValue = _tag;
				_tag = value;
				if (!object.ReferenceEquals(value, oldValue))
					OnPropertyChanged("Tag");
			}
		}

    /// <summary>
    /// Can be used by some GUI to get a connection to GUI elements.
    /// </summary>
		public object GuiTag
		{
			get
			{
				return _guiTag;
			}
			set
			{
				var oldValue = _guiTag;
				_guiTag = value;
				if (!object.ReferenceEquals(value, oldValue))
					OnPropertyChanged("GuiTag");
			}
		}


    /// <summary>
    /// Text a Gui node can display.
    /// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				var oldVal = _text;
				_text = value;
				if (oldVal != value)
					OnPropertyChanged("Text");
			}
		}

  
    /// <summary>
    /// Parent tree node.
    /// </summary>
    public NGTreeNode Parent { get { return _parent; } }

  

		/// <summary>
		/// Gets/sets whether the TreeViewItem 
		/// associated with this object is expanded.
		/// </summary>
		public virtual bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (value != _isExpanded)
				{
					_isExpanded = value;
					this.OnPropertyChanged("IsExpanded");
				}

				// Expand all the way up to the root.
				if (_isExpanded && _parent != null)
					_parent.IsExpanded = true;

				// Lazy load the child items, if necessary.
				if (this.HasDummyChild)
				{
					this._nodes.Remove(_dummyNode);
					this.LoadChildren();
				}
			}
		}

		/// <summary>
		/// Gets/sets whether the TreeViewItem 
		/// associated with this object is selected.
		/// </summary>
		public virtual bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (value != _isSelected)
				{
					_isSelected = value;
					this.OnPropertyChanged("IsSelected");
				}
			}
		}

		public virtual void ClearSelectionRecursively()
		{
			IsSelected = false;
			foreach (var n in Nodes)
				n.ClearSelectionRecursively();
		}


		/// <summary>
		/// Returns true if this object's Children have not yet been populated.
		/// </summary>
		public bool HasDummyChild
		{
			get { return null!= this._nodes && this._nodes.Count == 1 && this._nodes[0] ==_dummyNode; }
		}

		/// <summary>
		/// Invoked when the child items need to be loaded on demand.
		/// Subclasses can override this to populate the Children collection.
		/// </summary>
		protected virtual void LoadChildren()
		{
		}


		/// <summary>
		/// Returns an image index, or null if no image is set. The implementation here returns null, but can be overriden.
		/// </summary>
		public virtual int? ImageIndex { get { return null; } }

		/// <summary>
		/// Returns an image index (for the selected node), or null if no image is set. The implementation here returns null, but can be overriden.
		/// </summary>
		public virtual int? SelectedImageIndex { get { return null; } }


    /// <summary>
    /// Tests for childs without creating a child collection.
    /// </summary>
    public bool HasChilds
    {
      get
      {
        return _nodes != null && _nodes.Count > 0;
      }
    }

    /// <summary>
    /// Collection of the child nodes of this node.
    /// </summary>
    public NGTreeNodeCollection Nodes
    {
      get
      {
        if (null == _nodes)
          _nodes = new MyColl3(this);

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
    /// Returns only the nodes with the lowest hierarchy level.
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

		private class MyColl2 : System.Collections.ObjectModel.ObservableCollection<NGTreeNode>
		{
			NGTreeNode _parent;

			  public MyColl2(NGTreeNode parent)
     {
       this._parent = parent;
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

					var node_i = this[i];
					var node_j = this[j];
					base.SetItem(i, node_j);
					base.SetItem(j, node_i);

					OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Replace,node_i,node_j));
					OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Replace,node_j,node_i));
				}

			protected override void InsertItem(int index, NGTreeNode item)
			{
				if (item._parent != null && !object.ReferenceEquals(item._parent,_parent))
					throw new ApplicationException("Parent of the node is not null. Please remove the node before adding it");
				item._parent = _parent;
				base.InsertItem(index, item);
			}

			protected override void SetItem(int index, NGTreeNode item)
			{
				if (item._parent != null && !object.ReferenceEquals(item._parent,_parent))
					throw new ApplicationException("Parent of the node is not null. Please remove the node before adding it");
				item._parent = _parent;
				base.SetItem(index, item);
			}

			protected override void RemoveItem(int index)
			{
				this[index]._parent = null;
				base.RemoveItem(index);
			}
		}

		private class MyColl3 : System.Collections.ObjectModel.ObservableCollection<NGTreeNode>, NGTreeNodeCollection
		{
			readonly NGTreeNode _parent;

			public MyColl3(NGTreeNode parent)
			{
				_parent = parent;
			}

			public void AddRange(NGTreeNode[] nodes)
			{
				foreach (var n in nodes)
					base.Add(n);
			}

			public void Swap(int i, int j)
			{
				if (i < 0 || i >= Count)
					throw new ArgumentOutOfRangeException("i");
				if (j < 0 || j >= Count)
					throw new ArgumentOutOfRangeException("j");
				
				NGTreeNode node_i = base[i];
				base[i] = base[j];
				base[j] = node_i;
			}



			protected override void InsertItem(int index, NGTreeNode item)
			{
				item._parent = _parent;
				base.InsertItem(index, item);
			}

			protected override void SetItem(int index, NGTreeNode item)
			{
				item._parent = _parent;
				base.SetItem(index, item);
			}

			protected override void RemoveItem(int index)
			{
				base[index]._parent = null;
				base.RemoveItem(index);
			}

		}

    #endregion

	}

 
}
