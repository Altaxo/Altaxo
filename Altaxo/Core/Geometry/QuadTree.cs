#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
namespace Altaxo.Geometry
{
  /// <summary>
  /// Provides fast and efficient storage and retrieval of items with a rectangular area in a 2D coordinate system.
  /// </summary>
  /// <typeparam name="TItem">Type of item to store in the quad tree. The item must contain enough information in order to calculate a bounding rectangle of the item.</typeparam>
  public class QuadTree<TItem> 
  {
    // How many objects can exist in a QuadTree before it sub divides itself
    private readonly int _maximumNumberOfItemsPerNode = 2;

    /// <summary>The items of this quad at this level</summary>
    private List<TItem>? _items = null;

    /// <summary>Bounding rectangle of the quad tree at this level</summary>
    private RectangleD2D _boundaries;

    /// <summary>Function that evaluates the bounding rectangle of an item. Input is the item, output is the bounding rectangle of the item.</summary>
    private Func<TItem, RectangleD2D> _evaluateItemBounds;

    /// <summary>Structure to stores the child quads (top-left, top-right, bottom-left and bottom-right).</summary>
    private struct ChildTrees
    {
      public QuadTree<TItem> TL; // top-left quad
      public QuadTree<TItem> TR; // top-right quad
      public QuadTree<TItem> BL; // bottom-left quad
      public QuadTree<TItem> BR; // bottom-right quad
    }

    /// <summary>The child quads (top-left, top-right, bottom-left and bottom-right).</summary>
    private ChildTrees? _children;


    /// <summary>
    /// Creates a quad tree for the specified rectangular boundaries.
    /// </summary>
    /// <param name="rect">The rectangular boundaries of this quad. See remarks for further instructions.</param>
    /// <param name="EvaluateItemBounds">A function that evaluates the item's bounding rectangle. The area of the returned rectangle may be small, but should not be empty.</param>
    /// <remarks>If you create a quad tree, choose the boundaries so that all items that you intend to add to this quad tree
    /// will intersect with the boundary rectangle. Items that will not intersect will be not added!!</remarks>
    public QuadTree(RectangleD2D rect, Func<TItem, RectangleD2D> EvaluateItemBounds)
      : this(rect, EvaluateItemBounds, 2)
    {
    }

    /// <summary>
    /// Creates a quad tree for the specified rectangular boundaries.  See remarks for further instructions how to choose the boundaries.
    /// </summary>
    /// <param name="x">The top-left position of the area rectangle.</param>
    /// <param name="y">The top-right position of the area reactangle.</param>
    /// <param name="width">The width of the area rectangle.</param>
    /// <param name="height">The height of the area rectangle.</param>
    /// <param name="EvaluateItemBounds">A function that evaluates the item's bounding rectangle. The area of the returned rectangle may be small, but should not be empty.</param>
    /// <remarks>If you create a quad tree, choose the boundaries so that all items that you intend to add to this quad tree
    /// will intersect with the boundary rectangle. Items that will not intersect will be not added!!</remarks>
    public QuadTree(int x, int y, int width, int height, Func<TItem, RectangleD2D> EvaluateItemBounds)
      : this(new RectangleD2D(x, y, width, height), EvaluateItemBounds, 2)
    {
    }

    /// <summary>
    /// Creates a quad tree for the specified rectangular boundaries.
    /// </summary>
    /// <param name="rect">The rectangular boundaries of this quad. See remarks for further instructions.</param>
    /// <param name="EvaluateItemBounds">A function that evaluates the rectangular boundaries for each item.  The area of the returned rectangle may be small, but should not be empty.</param>
    /// <param name="maximumNumberOfItemsPerNode">The maximum number of item that on node should store.</param>
    /// <remarks>If you create a quad tree, choose the boundaries so that all items that you intend to add to this quad tree
    /// will intersect with the boundary rectangle. Items that will not intersect will be not added!!</remarks>
    public QuadTree(RectangleD2D rect, Func<TItem, RectangleD2D> EvaluateItemBounds, int maximumNumberOfItemsPerNode)
    {
      _boundaries = rect;
      _evaluateItemBounds = EvaluateItemBounds ?? throw new ArgumentNullException(nameof(EvaluateItemBounds));

      if (maximumNumberOfItemsPerNode < 2)
        throw new ArgumentOutOfRangeException(nameof(maximumNumberOfItemsPerNode), "Maximum number of items per node must be >=2");
      _maximumNumberOfItemsPerNode = maximumNumberOfItemsPerNode;
    }

    /// <summary>
    /// The rectangular area that is covered by this node of the quad tree.
    /// </summary>
    public RectangleD2D Boundaries { get { return _boundaries; } }

    /// <summary>
    /// The top left child of this node of the quad tree (or <c>null</c> if it not exists).
    /// </summary>
    public QuadTree<TItem>? TopLeftChild { get { return _children?.TL; } }

    /// <summary>
    /// The top right child of this node of the quad tree (or <c>null</c> if it not exists).
    /// </summary>
    public QuadTree<TItem>? TopRightChild { get { return _children?.TR; } }

    /// <summary>
    /// The bottom left child of this node of the quad tree (or <c>null</c> if it not exists).
    /// </summary>
    public QuadTree<TItem>? BottomLeftChild { get { return _children?.BL; } }

    /// <summary>
    /// The bottom right child of this node of the quad tree (or <c>null</c> if it not exists).
    /// </summary>
    public QuadTree<TItem>? BottomRightChild { get { return _children?.BR; } }

    /// <summary>
    /// The items contained in this node of the quad tree (i.e. without (!) the items in the child nodes).
    /// </summary>
    public List<TItem>? ItemsAtThisLevel { get { return _items; } }

    /// <summary>
    /// Total number of items in this quad tree, including the items in its children.
    /// </summary>
    public int Count
    {
      get
      {
        var count = _items?.Count ?? 0; // number of items at this level

        if (_children is { } children)
        { // number of items in the child nodes
          count += children.TL.Count;
          count += children.TR.Count;
          count += children.BL.Count;
          count += children.BR.Count;
        }
        return count;
      }
    }

    /// <summary>
    /// Removes all items from this node of the quad tree and its children.
    /// </summary>
    public void Clear()
    {
      // clear the children
      if (_children is { } children)
      {
        children.TL.Clear();
        children.TR.Clear();
        children.BL.Clear();
        children.BR.Clear();
        _children = null;
      }

      // clear the items at this level
      if (_items is not null)
      {
        _items.Clear();
        _items = null;
      }
    }

    /// <summary>
    /// Removes an item from the quad tree.
    /// If the item is removed successfully, and afterwards the quad has no items in its child quads anymore,
    /// the child quads will be removed as well.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(TItem item)
    {
      bool removed;
      // if the quad at this level contains the object, then remove it
      if (!(removed = _items?.Remove(item) ?? false) && _children is { } children)
      {
        // otherwise, try to remove the item from one of its children
        removed = children.TL.Remove(item) ||
                  children.TR.Remove(item) ||
                  children.BL.Remove(item) ||
                  children.BR.Remove(item);

        // if the remove operation was successful,
        // test if the child quads contain items anymore.
        // If not, remove the child quads
        if (removed &&
            children.TL.Count == 0 &&
            children.TR.Count == 0 &&
            children.BL.Count == 0 &&
            children.BR.Count == 0)
        {
          _children = null;
        }
      }
      return removed;
    }


    /// <summary>
    /// Adds an item to the quad tree.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <remarks>The item is added only if the quad boundaries intersect with the item's boundary.
    /// Make sure that the root quad boundaries are big enough to accomodate all items intended to be added to the quad tree!.</remarks>
    public void Add(TItem item)
    {
      if (!_boundaries.IntersectsWith(_evaluateItemBounds(item)))
      {
        return; // item bondaries do not intersect with the quad boundaries
      }

      if (_items is null || (_children is null && _items.Count < _maximumNumberOfItemsPerNode))
      {
        // if the number limit is not reached, add the item to this level
        (_items ??= new List<TItem>()).Add(item);
      }
      else
      {
        if (_children is null)
        {
          // Number limit reached, thus subdivide into child quads
          SubDivide();
        }

        // find out to which child quad the item should be added
        // Note: if the item's rectangle is too big, the item may be added to this level
        // instead of to one of the child quads
        var destTree = GetDestinationTree(item);
        if (object.ReferenceEquals(destTree, this))
        {
          (_items ??= new List<TItem>()).Add(item);
        }
        else
        {
          destTree.Add(item);
        }
      }
    }

    /// <summary>
    /// Adds items to the quad tree.
    /// </summary>
    /// <param name="items">The items to add.</param>
    /// <remarks>The items are added only if the quad boundaries intersect with the item's boundary.
    /// Make sure that the root quad boundaries are big enough to accomodate all items intended to be added to the quad tree!.</remarks>
    public void AddRange(IEnumerable<TItem> items)
    {
      if (items is not null)
      {
        foreach (var item in items)
          Add(item);
      }
    }

    /// <summary>
    /// Get the items in this tree that intersect with the specified rectangle.
    /// </summary>
    /// <param name="rect">The rectangular area in which to find items.</param>
    /// <returns>A list of items whose rectangles intersect with the rectangle given in the parameter.</returns>
    public List<TItem> GetItems(RectangleD2D rect)
    {
      var results = new List<TItem>();
      GetItems(rect, results);
      return results;
    }

    /// <summary>
    /// Get the items in this tree that intersect with the specified rectangle.
    /// </summary>
    /// <param name="rect">The rectangular area in which to find items.</param>
    /// <param name="results">A list, which is filled with items whose rectangles intersect with the rectangle given in the parameter.</param>
    public void GetItems(RectangleD2D rect, List<TItem> results)
    {
      if (results is null)
        throw new ArgumentNullException(nameof(results));

      if (rect.Contains(_boundaries))
      {
        GetAllItems(results); // if the searching rectangle completely contains this quad rectangle, just get every object in this quad and all of its children
      }
      else if (rect.IntersectsWith(_boundaries)) // add objects that intersect with the searching rectangle
      {
        // first, look into the items at this level
        if (_items is { } items)
        {
          for (int i = 0; i < items.Count; i++)
          {
            if (rect.IntersectsWith(_evaluateItemBounds(items[i])))
            {
              results.Add(items[i]);
            }
          }
        }

        // then, look into the items in the child quads
        if (_children is { } children)
        {
          children.TL.GetItems(rect, results);
          children.TR.GetItems(rect, results);
          children.BL.GetItems(rect, results);
          children.BR.GetItems(rect, results);
        }
      }
    }


    /// <summary>
    /// Get all items in this quad tree, including all items in the children.
    /// </summary>
    /// <returns>A list of all items in this quad tree.</returns>
    public List<TItem> GetAllItems()
    {
      var results = new List<TItem>();
      GetAllItems(results);
      return results;
    }

    /// <summary>
    /// Get all items in this quad tree, including all items in the children.
    /// </summary>
    /// <param name="results">A list, that is filled with all items in this quad tree.</param>
    public void GetAllItems(List<TItem> results)
    {
      // If this Quad has objects, add them
      if (_items is { } items)
      {
        results.AddRange(items);
      }

      // If we have children, get their objects too
      if (_children is { } children)
      {
        children.TL.GetAllItems(results);
        children.TR.GetAllItems(results);
        children.BL.GetAllItems(results);
        children.BR.GetAllItems(results);
      }

    }

    /// <summary>
    /// Subdivide this quad tree and move its children into the appropriate child quad tree if possible.
    /// The items are only moved if the child quad tree covers the entire area of the item.
    /// </summary>
    private void SubDivide()
    {
      if (_items is null)
        throw new InvalidProgramException();

      var halfsize = new PointD2D(_boundaries.Width / 2, _boundaries.Height / 2);
      var center = new PointD2D(_boundaries.X + halfsize.X, _boundaries.Y + halfsize.Y);

      _children = new ChildTrees
      {
        TL = new QuadTree<TItem>(new RectangleD2D(_boundaries.Left, _boundaries.Top, halfsize.X, halfsize.Y), _evaluateItemBounds, _maximumNumberOfItemsPerNode),
        TR = new QuadTree<TItem>(new RectangleD2D(center.X, _boundaries.Top, halfsize.X, halfsize.Y), _evaluateItemBounds, _maximumNumberOfItemsPerNode),
        BL = new QuadTree<TItem>(new RectangleD2D(_boundaries.Left, center.Y, halfsize.X, halfsize.Y), _evaluateItemBounds, _maximumNumberOfItemsPerNode),
        BR = new QuadTree<TItem>(new RectangleD2D(center.X, center.Y, halfsize.X, halfsize.Y), _evaluateItemBounds, _maximumNumberOfItemsPerNode)
      };

      // if the items are completely contained in the child quad, move them to the child tree
      // otherwise, leave them in the parent quad
      for (int i = _items.Count - 1; i >= 0; --i)
      {
        // get the destination tree with that quad rectangle that fully contains the item's rectangle
        var destTree = GetDestinationTree(_items[i]);

        if (!object.ReferenceEquals(destTree, this))
        {
          // add the item to the child quad, remove the item from the parent quad
          destTree.Add(_items[i]);
          _items.RemoveAt(i);
        }
      }
    }

    /// <summary>
    /// Get the child quad that fully contains the item's rectangle.
    /// If the item's rectange is not fully contained in the child quads area, the parent quad is returned.
    /// </summary>
    /// <param name="item">The item for which to get the child quad.</param>
    /// <returns>the child quad that fully contains the item's rectangle. If the item's rectange is not fully contained in the child quads area, the parent quad is returned instead.</returns>
    private QuadTree<TItem> GetDestinationTree(TItem item)
    {
      var destTree = this; // by default, return the parent quad
      var itemRect = _evaluateItemBounds(item);

      if (_children is { } children)
      {
        if (children.TL.Boundaries.Contains(itemRect))
        {
          destTree = children.TL;
        }
        else if (children.TR.Boundaries.Contains(itemRect))
        {
          destTree = children.TR;
        }
        else if (children.BL.Boundaries.Contains(itemRect))
        {
          destTree = children.BL;
        }
        else if (children.BR.Boundaries.Contains(itemRect))
        {
          destTree = children.BR;
        }
      }

      return destTree;
    }

    #region Debugging only

    /// <summary>
    /// Searches for an item in the quad tree.
    /// </summary>
    /// <param name="item">The item to search for.</param>
    /// <param name="level">The level of this node.</param>
    /// <returns>If the search was successful, a string which designates the location of the item in the quad tree; otherwise the return value is <c>null</c>.</returns>
    internal string? SearchFor(TItem item, string level = "Root")
    {
      if(_items is { } items)
      {
        // look into the items at this level
        foreach(var oitem in items)
        {
          if (object.Equals(oitem, item))
          {
            return level;
          }
        }
      }

      string? res;
      if(_children is { } children)
      {
        // look into the items in the child quads
        res = children.TL.SearchFor(item, level+"_TL");
        if (res is not null)
          return res;
        res = children.TR.SearchFor(item, level+"_TR");
        if (res is not null)
          return res;
        res = children.BL.SearchFor(item, level+"_BL");
        if (res is not null)
          return res;
        res = children.BR.SearchFor(item, level+"_BR");
        if (res is not null)
          return res;
      }

      return null;
    }

    #endregion
  }
}

