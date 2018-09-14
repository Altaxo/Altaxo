#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2013 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 3 of the License, or
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
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Extends the IList interface by a function that allows to swap two items by index.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ISwappableList<T> : IList<T>
  {
  }

  /// <summary>
  /// Defines a simple tree node, where the child nodes are enumerable. There is no reference to the node's parent.
  /// </summary>
  /// <typeparam name="T">Type of the node.</typeparam>
  public interface ITreeNode<T> where T : ITreeNode<T>
  {
    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    /// <value>
    /// The child nodes.
    /// </value>
    IEnumerable<T> ChildNodes { get; }
  }

  /// <summary>
  /// Defines a tree node, where the childs can be accessed by index. There is no reference to the node's parent.
  /// </summary>
  /// <typeparam name="T">Type of the node.</typeparam>
  public interface ITreeListNode<T> : ITreeNode<T> where T : ITreeListNode<T>
  {
    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    /// <value>
    /// The child nodes.
    /// </value>
    new IList<T> ChildNodes { get; }
  }

  /// <summary>
  /// Defines a tree node, where the childs can be accessed by index. There is no reference to the node's parent.
  /// </summary>
  /// <typeparam name="T">Type of the node.</typeparam>
  public interface ITreeSwappableListNode<T> : ITreeListNode<T> where T : ITreeSwappableListNode<T>
  {
    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    /// <value>
    /// The child nodes.
    /// </value>
    new ISwappableList<T> ChildNodes { get; }
  }

  /// <summary>
  /// Defines an interface of a node, with has a parent node.
  /// </summary>
  /// <typeparam name="T">Type of the node.</typeparam>
  public interface INodeWithParentNode<T>
  {
    /// <summary>
    /// Gets the parent node of this node.
    /// </summary>
    /// <value>
    /// The parent node.
    /// </value>
    T ParentNode { get; }
  }

  /// <summary>
  /// Defines a simple tree node with parent.
  /// </summary>
  /// <typeparam name="T">Type of the node.</typeparam>
  public interface ITreeNodeWithParent<T> : ITreeNode<T>, INodeWithParentNode<T> where T : ITreeNodeWithParent<T>
  {
  }

  /// <summary>
  /// Defines a tree node with parent. The child nodes can be accessed by index.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ITreeListNodeWithParent<T> : ITreeListNode<T>, INodeWithParentNode<T> where T : ITreeListNodeWithParent<T>
  {
  }

  /// <summary>
  /// Implements algorithms common for all trees.
  /// </summary>
  public static class TreeNodeExtensions
  {
    public static void FromHereToLeavesDo<T>(this T node, Action<T> action) where T : ITreeNode<T>
    {
      action(node);
      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes)
        {
          FromHereToLeavesDo<T>(childNode, action);
        }
      }
    }

    public static void FromLeavesToHereDo<T>(this T node, Action<T> action) where T : ITreeNode<T>
    {
      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes)
        {
          FromLeavesToHereDo<T>(childNode, action);
        }
      }
      action(node);
    }

    public static T AnyBetweenHereAndLeaves<T>(this T node, Func<T, bool> condition) where T : ITreeNode<T>
    {
      if (condition(node))
        return node;

      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes)
        {
          var result = AnyBetweenHereAndLeaves(childNode, condition);
          if (null != result)
            return result;
        }
      }
      return default(T);
    }

    public static T AnyBetweenLeavesAndHere<T>(this T node, Func<T, bool> condition) where T : ITreeNode<T>
    {
      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes)
        {
          var result = AnyBetweenLeavesAndHere(childNode, condition);
          if (null != result)
            return result;
        }
      }

      if (condition(node))
        return node;

      return default(T);
    }

    /// <summary>
    /// Enumerates through all tree nodes from the provided node <paramref name="node"/> up to the leaves of the tree. If <paramref name="includeThisNode"/> is <c>true</c>, the provided node <paramref name="node"/> is included in the enumeration.
    /// The direction of the enumeration of the child nodes depend on the return value of a function <paramref name="directionSelector"/>, which is applied to the parent node.
    ///
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <param name="includeThisNode">If set to <c>true</c> the node <paramref name="node"/> is included in the enumeration, otherwise, it is not part of the enumeration.</param>
    /// <param name="directionSelector">Function to determine the enumeration order of the child nodes of a parent node. The function is called with the parent node as argument.
    /// If the direction selector always returns <see cref="IndexDirection.Ascending"/>, the result is equal to that of <see cref="TakeFromHereToFirstLeaves{T}(T, bool)"/>.</param>
    /// <returns>All tree nodes from <paramref name="node"/> up to the leaves of the tree.</returns>
    public static IEnumerable<T> TakeFromHereToLeaves<T>(this T node, bool includeThisNode, Func<T, IndexDirection> directionSelector) where T : ITreeNode<T>
    {
      if (includeThisNode)
        yield return node;

      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        var direction = directionSelector(node);
        switch (direction)
        {
          case IndexDirection.Ascending:
            foreach (var childNode in childNodes)
            {
              foreach (var subchild in TakeFromHereToLeaves(childNode, true, directionSelector))
                yield return subchild;
            }
            break;

          case IndexDirection.Descending:
            foreach (var childNode in childNodes.Reverse())
            {
              foreach (var subchild in TakeFromHereToLeaves(childNode, true, directionSelector))
                yield return subchild;
            }
            break;

          default:
            throw new NotImplementedException(direction.ToString());
        }
      }
    }

    /// <summary>
    /// Enumerates through all tree nodes from the provided node <paramref name="node"/> up to the leaves of the tree. If <paramref name="includeThisNode"/> is <c>true</c>, the provided node <paramref name="node"/> is included in the enumeration.
    /// The direction of the enumeration of the child nodes depend on the return value of a function <paramref name="directionSelector"/>, which is applied to the parent node.
    /// In addition to the node itself, the enumeration also delivers the index of the node in the parent's collection.
    ///
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <param name="nodeIndex">The index of the node provided in the argument <paramref name="node"/>.</param>
    /// <param name="includeThisNode">If set to <c>true</c> the node <paramref name="node"/> is included in the enumeration, otherwise, it is not part of the enumeration.</param>
    /// <param name="directionSelector">Function to determine the enumeration order of the child nodes of a parent node. The function is called with the parent node as argument.
    /// If the direction selector always returns <see cref="IndexDirection.Ascending"/>, the result is equal to that of <see cref="TakeFromHereToFirstLeaves{T}(T, bool)"/>.</param>
    /// <returns>All tree nodes from <paramref name="node"/> up to the leaves of the tree.</returns>
    public static IEnumerable<Tuple<T, int>> TakeFromHereToLeavesWithIndex<T>(this T node, int nodeIndex, bool includeThisNode, Func<T, IndexDirection> directionSelector) where T : ITreeNode<T>
    {
      if (includeThisNode)
        yield return new Tuple<T, int>(node, nodeIndex);

      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        var direction = directionSelector(node);
        switch (direction)
        {
          case IndexDirection.Ascending:
            nodeIndex = -1;
            foreach (var childNode in childNodes)
            {
              ++nodeIndex;
              foreach (var subchild in TakeFromHereToLeavesWithIndex(childNode, nodeIndex, true, directionSelector))
                yield return subchild;
            }
            break;

          case IndexDirection.Descending:
            nodeIndex = childNodes.Count();
            foreach (var childNode in childNodes.Reverse())
            {
              --nodeIndex;
              foreach (var subchild in TakeFromHereToLeavesWithIndex(childNode, nodeIndex, true, directionSelector))
                yield return subchild;
            }
            break;

          default:
            throw new NotImplementedException(direction.ToString());
        }
      }
    }

    /// <summary>
    /// Enumerates through all tree nodes from (and including) the provided node <paramref name="node"/> up to the leaves of the tree.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <returns>All tree nodes from <paramref name="node"/> up to the leaves of the tree.</returns>
    public static IEnumerable<T> TakeFromHereToFirstLeaves<T>(this T node) where T : ITreeNode<T>
    {
      return TakeFromHereToFirstLeaves(node, true);
    }

    /// <summary>
    /// Enumerates through all tree nodes from the provided node <paramref name="node"/> up to the leaves of the tree. If <paramref name="includeThisNode"/> is <c>true</c>, the provided node <paramref name="node"/> is included in the enumeration.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <param name="includeThisNode">If set to <c>true</c> the node <paramref name="node"/> is included in the enumeration, otherwise, it is not part of the enumeration.</param>
    /// <returns>All tree nodes from <paramref name="node"/> up to the leaves of the tree.</returns>
    public static IEnumerable<T> TakeFromHereToFirstLeaves<T>(this T node, bool includeThisNode) where T : ITreeNode<T>
    {
      if (includeThisNode)
        yield return node;

      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes)
        {
          foreach (var subchild in TakeFromHereToFirstLeaves(childNode, true))
            yield return subchild;
        }
      }
    }

    /// <summary>
    /// Enumerates through all tree nodes from (and including) the provided node <paramref name="node"/> to the leaves of the tree. The downmost leaves will be enumerated first.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <returns>All tree nodes from <paramref name="node"/> up to the leaves of the tree. The downmost leaves will be enumerated first.</returns>
    public static IEnumerable<T> TakeFromHereToLastLeaves<T>(this T node) where T : ITreeNode<T>
    {
      return TakeFromHereToLastLeaves(node, true);
    }

    /// <summary>
    /// Enumerates through all tree nodes from the provided node <paramref name="node"/> to the leaves of the tree. The downmost leaves will be enumerated first. If <paramref name="includeThisNode"/> is <c>true</c>, the provided node <paramref name="node"/> is included in the enumeration.
    /// Attention: Since the order of the nodes must be reversed, this enumeration is only efficient for <see cref="ITreeListNode{T}"/> types.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <param name="includeThisNode">If set to <c>true</c> the node <paramref name="node"/> is included in the enumeration, otherwise, it is not part of the enumeration.</param>
    /// <returns>All tree nodes from <paramref name="node"/> to the leaves of the tree. The downmost leaves will be enumerated first.</returns>
    public static IEnumerable<T> TakeFromHereToLastLeaves<T>(this T node, bool includeThisNode) where T : ITreeNode<T>
    {
      if (includeThisNode)
        yield return node;

      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes.Reverse())
        {
          foreach (var subchild in TakeFromHereToLastLeaves(childNode, true))
            yield return subchild;
        }
      }
    }

    /// <summary>
    /// Enumerates through all tree nodes from the upmost leaf of the tree down to the provided node <paramref name="node"/>. The provided node <paramref name="node"/> is included in the enumeration.
    /// Attention: Since the order of the nodes must be reversed, this enumeration is only efficient for <see cref="ITreeListNode{T}"/> types.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <returns>All tree nodes from the upmost leaf of the tree down to the provided node <paramref name="node"/>.</returns>
    public static IEnumerable<T> TakeFromFirstLeavesToHere<T>(this T node) where T : ITreeNode<T>
    {
      return TakeFromFirstLeavesToHere(node, true);
    }

    /// <summary>
    /// Enumerates through all tree nodes from the upmost leaf of the tree down to the provided node <paramref name="node"/>. If <paramref name="includeThisNode"/> is <c>true</c>, the provided node <paramref name="node"/> is included in the enumeration.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <param name="includeThisNode">If set to <c>true</c> the node <paramref name="node"/> is included in the enumeration, otherwise, it is not part of the enumeration.</param>
    /// <returns>All tree nodes from the upmost leaf of the tree down to the provided node <paramref name="node"/>.</returns>
    public static IEnumerable<T> TakeFromFirstLeavesToHere<T>(this T node, bool includeThisNode) where T : ITreeNode<T>
    {
      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes)
        {
          foreach (var subchild in TakeFromFirstLeavesToHere(childNode, true))
            yield return subchild;
        }
      }

      if (includeThisNode)
        yield return node;
    }

    /// <summary>
    /// Enumerates through all tree nodes from the downmost leaf of the tree down to the provided node <paramref name="node"/>. The provided node <paramref name="node"/> is included in the enumeration.
    /// Attention: Since the order of the nodes must be reversed, this enumeration is only efficient for <see cref="ITreeListNode{T}"/> types.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <returns>All tree nodes from the downmost leaf of the tree down to the provided node <paramref name="node"/>.</returns>
    public static IEnumerable<T> TakeFromLeavesToHere<T>(this T node) where T : ITreeNode<T>
    {
      return TakeFromLastLeavesToHere(node, true);
    }

    /// <summary>
    /// Enumerates through all tree nodes from the downmost leaf of the tree down to the provided node <paramref name="node"/>.
    /// If <paramref name="includeThisNode"/> is <c>true</c>, the provided node <paramref name="node"/> is included in the enumeration.
    /// Attention: Since the order of the nodes must be reversed, this enumeration is only efficient for <see cref="ITreeListNode{T}"/> types.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <param name="includeThisNode">If set to <c>true</c> the node <paramref name="node"/> is included in the enumeration, otherwise, it is not part of the enumeration.</param>
    /// <returns>All tree nodes from the downmost leaf of the tree down to the provided node <paramref name="node"/>.</returns>
    public static IEnumerable<T> TakeFromLastLeavesToHere<T>(this T node, bool includeThisNode) where T : ITreeNode<T>
    {
      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes.Reverse())
        {
          foreach (var subchild in TakeFromLastLeavesToHere(childNode, true))
            yield return subchild;
        }
      }

      if (includeThisNode)
        yield return node;
    }

    /// <summary>
    /// Enumerates through all tree nodes from the downmost leaf of the tree down to the provided node <paramref name="node"/>.
    /// Local data is provided for each enumerated node. The local data is calculated from the root node up to the enumerated node.
    /// If <paramref name="includeThisNode"/> is <c>true</c>, the provided node <paramref name="node"/> is included in the enumeration.
    /// Attention: Since the order of the nodes must be reversed, this enumeration is only efficient for <see cref="ITreeListNode{T}"/> types.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <typeparam name="D">Type of some data that is associated with a node (the node's local data).</typeparam>
    /// <param name="node">The node from which to start visiting the tree.</param>
    /// <param name="nodesLocalData">Local data belonging to the provided <paramref name="node"/>.</param>
    /// <param name="includeThisNode">If set to <c>true</c> the node <paramref name="node"/> is included in action execution, otherwise, it is not part of the action execution.</param>
    /// <param name="transformLocalDataFromParentToChild">When traversing the tree from the root node up to the leaves, the provided local data can be transformed so that the data always reflect the state of the nodes.
    /// First argument is the child node, second argument is the local data from the parent node.
    /// The return value should be the local data for the child node given in the first argument.</param>
    /// <returns>All tree nodes from the downmost leaf of the tree down to the provided node <paramref name="node"/>.</returns>
    public static IEnumerable<Tuple<T, D>> TakeFromLastLeavesToHere<T, D>(this T node, D nodesLocalData, bool includeThisNode, Func<T, D, D> transformLocalDataFromParentToChild) where T : ITreeNode<T>
    {
      var childNodes = node.ChildNodes;
      if (null != childNodes)
      {
        foreach (var childNode in childNodes.Reverse())
        {
          var childNodesLocalData = transformLocalDataFromParentToChild(childNode, nodesLocalData);
          foreach (var subchildTuple in TakeFromLastLeavesToHere(childNode, childNodesLocalData, true, transformLocalDataFromParentToChild))
            yield return subchildTuple;
        }
      }

      if (includeThisNode)
        yield return new Tuple<T, D>(node, nodesLocalData);
    }

    /// <summary>
    /// Projects a tree (source tree) to a new tree (destination tree).
    /// </summary>
    /// <typeparam name="S">Type of the source tree node.</typeparam>
    /// <typeparam name="D">Type of the destination tree node.</typeparam>
    /// <param name="sourceRoot">The source root tree node.</param>
    /// <param name="createDestinationNodeFromSourceNode">Function used to create a destination node from a source node.</param>
    /// <param name="addChildToDestinationNode">Procedure to add a child node to a destination node (first argument is the parent node, 2nd argument is the child node).</param>
    /// <returns>The root node of the newly created destination tree that reflects the structure of the source tree.</returns>
    public static D ProjectTreeToNewTree<S, D>(this S sourceRoot, Func<S, D> createDestinationNodeFromSourceNode, Action<D, D> addChildToDestinationNode)
      where D : ITreeNode<D>
      where S : ITreeNode<S>
    {
      var destRoot = createDestinationNodeFromSourceNode(sourceRoot);

      var sourceChildNodes = sourceRoot.ChildNodes;
      if (null != sourceChildNodes)
      {
        foreach (var sourceChild in sourceChildNodes)
        {
          var destChild = ProjectTreeToNewTree(sourceChild, createDestinationNodeFromSourceNode, addChildToDestinationNode);
          addChildToDestinationNode(destRoot, destChild);
        }
      }
      return destRoot;
    }

    /// <summary>
    /// Projects a tree (source tree) to a new tree (destination tree). The creation function for the new tree nodes gets information about the node indices.
    /// </summary>
    /// <typeparam name="S">Type of the source tree node.</typeparam>
    /// <typeparam name="D">Type of the destination tree node.</typeparam>
    /// <param name="sourceRoot">The source root tree node.</param>
    /// <param name="indices">List of indices that describes the destination root node. If this parameter is null, an internal list will be created, and the destination root node will get the index 0.</param>
    /// <param name="createDestinationNodeFromSourceNode">Function used to create a destination node from a source node. First parameter is the source node, 2nd parameter is a list of indices that describe the destination node.</param>
    /// <param name="addChildToDestinationNode">Procedure to add a child node to a destination node (first argument is the parent node, 2nd argument is the child node).</param>
    /// <returns>The root node of the newly created destination tree that reflects the structure of the source tree.</returns>
    public static D ProjectTreeToNewTree<S, D>(this S sourceRoot, IList<int> indices, Func<S, IList<int>, D> createDestinationNodeFromSourceNode, Action<D, D> addChildToDestinationNode)
      where D : ITreeNode<D>
      where S : ITreeNode<S>
    {
      if (null == indices)
        indices = new List<int>();
      var destRoot = createDestinationNodeFromSourceNode(sourceRoot, indices);

      var sourceChildNodes = sourceRoot.ChildNodes;
      if (null != sourceChildNodes)
      {
        indices.Add(0);
        foreach (var sourceChild in sourceChildNodes)
        {
          var destChild = ProjectTreeToNewTree(sourceChild, indices, createDestinationNodeFromSourceNode, addChildToDestinationNode);
          addChildToDestinationNode(destRoot, destChild);
          ++indices[indices.Count - 1];
        }
        indices.RemoveAt(indices.Count - 1);
      }

      return destRoot;
    }

    /// <summary>
    /// Ensures that a list of indices that point to a node in a tree is valid.
    /// </summary>
    /// <typeparam name="T">Type of the tree node.</typeparam>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="index">The index list. On return, it is ensured that this index list designates a valid index of a node inside the tree.</param>
    /// <returns><c>True</c> if the index list was changed to ensure validity, <c>False</c> if it was not neccessary to change the index list.</returns>
    public static bool EnsureValidityOfNodeIndex<T>(this T rootNode, IList<int> index) where T : ITreeListNode<T>
    {
      if (null == rootNode)
        throw new ArgumentNullException("rootNode");
      if (null == index)
        throw new ArgumentNullException("index");

      if (index.Count > 0)
        return EnsureValidityOfNodeIndex(rootNode.ChildNodes, index, 0);

      return false;
    }

    /// <summary>
    /// Ensures that a list of indices that point to a node in a tree is valid. Here, only the childs of the root node are tested.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nodes">The nodes collection of a node of the tree.</param>
    /// <param name="index">The index list. On return, it is ensured that this index list designates a valid index of a node inside the tree.</param>
    /// <param name="level">The level. Must be always greater than 0.</param>
    /// <returns><c>True</c> if the index list was changed to ensure validity, <c>False</c> if it was not neccessary to change the index list.</returns>
    private static bool EnsureValidityOfNodeIndex<T>(IList<T> nodes, IList<int> index, int level) where T : ITreeListNode<T>
    {
      if (null == nodes || 0 == nodes.Count)
      {
        for (int i = index.Count - 1; i >= level; --i)
          index.RemoveAt(i);
        return true;
      }

      if (index[level] < 0)
      {
        index[level] = 0;
        for (int i = index.Count - 1; i > level; --i)
          index.RemoveAt(i);
        return true;
      }
      else if (index[level] >= nodes.Count)
      {
        index[level] = nodes.Count - 1;
        for (int i = index.Count - 1; i > level; --i)
          index.RemoveAt(i);
        return true;
      }

      if (index.Count > (level + 1))
      {
        return EnsureValidityOfNodeIndex(nodes[index[level]].ChildNodes, index, level + 1);
      }

      return false;
    }

    /// <summary>
    /// Gets a node inside a tree by using an index array.
    /// </summary>
    /// <typeparam name="T">Type of node.</typeparam>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="index">The index array. The member at index 0 must always be 0, since this indicates the provided root node. Examples: {0} designates the root node; {0, 1} designates the 2nd child of the root node; {0,1,0} designates the first child of the second child of the root node.</param>
    /// <returns>The node that is designated by the provided index.</returns>
    /// <exception cref="System.ArgumentNullException">rootNode</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Index list is null or empty; or index[0] is not 0; or index is otherwise invalid.
    /// </exception>
    public static T ElementAt<T>(this T rootNode, IEnumerable<int> index) where T : ITreeListNode<T>
    {
      if (null == rootNode)
        throw new ArgumentNullException("rootNode");

      if (null == index)
        throw new ArgumentOutOfRangeException("index is null ");

      T result;
      using (var it = index.GetEnumerator())
      {
        if (it.MoveNext())
          result = ElementAtInternal(rootNode.ChildNodes, it, 1);
        else
          result = rootNode; // List is empty => return the root node.
      }
      return result;
    }

    private static T ElementAtInternal<T>(IList<T> nodes, IEnumerator<int> it, int level) where T : ITreeListNode<T>
    {
      // check this in a public function before making a call to this: rootNode != null, index.Count>0, level>0

      if (null == nodes || 0 == nodes.Count)
        throw new ArgumentOutOfRangeException(string.Format("node at level {0} does not has children as expected", level - 1));

      var idx = it.Current;
      if (idx < 0)
        throw new ArgumentOutOfRangeException(string.Format("index at level {0} is < 0", level));
      else if (idx >= nodes.Count)
        throw new ArgumentOutOfRangeException(string.Format("index at level {0} is greater than number of child nodes", level));

      if (it.MoveNext())
        return ElementAtInternal(nodes[idx].ChildNodes, it, level + 1);
      else
        return nodes[idx];
    }

    /// <summary>
    /// Determines whether the given <paramref name="index"/> is valid or not.
    /// </summary>
    /// <typeparam name="T">Type of nodes.</typeparam>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="index">The index that points to a node inside the tree.</param>
    /// <returns><c>true</c> if the given index is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidIndex<T>(this T rootNode, IEnumerable<int> index) where T : ITreeListNode<T>
    {
      return IsValidIndex(rootNode, index, out var nodeAtIndex);
    }

    /// <summary>
    /// Determines whether the given <paramref name="index"/> is valid or not.
    /// </summary>
    /// <typeparam name="T">Type of nodes.</typeparam>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="index">The index that points to a node inside the tree.</param>
    /// <param name="nodeAtIndex">If the return value was true, this parameter contains the node at the given index.</param>
    /// <returns><c>true</c> if the given index is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidIndex<T>(this T rootNode, IEnumerable<int> index, out T nodeAtIndex) where T : ITreeListNode<T>
    {
      if (null == rootNode)
        throw new ArgumentNullException("rootNode");

      if (null == index)
      {
        nodeAtIndex = default(T);
        return false;
      }

      bool result;
      using (var it = index.GetEnumerator())
      {
        if (it.MoveNext())
          result = IsValidIndex(rootNode.ChildNodes, it, 1, out nodeAtIndex);
        else
        {
          nodeAtIndex = rootNode;
          result = true; // List is empty => return true, since this is the root node.
        }
      }
      return result;
    }

    private static bool IsValidIndex<T>(IList<T> nodes, IEnumerator<int> it, int level, out T nodeAtIndex) where T : ITreeListNode<T>
    {
      nodeAtIndex = default(T);

      if (null == nodes || 0 == nodes.Count)
        return false;

      var idx = it.Current;
      if (idx < 0)
        return false; // throw new ArgumentOutOfRangeException(string.Format("index at level {0} is < 0", level));
      else if (idx >= nodes.Count)
        return false; //  throw new ArgumentOutOfRangeException(string.Format("index at level {0} is greater than number of child nodes", level));

      if (it.MoveNext())
      {
        return IsValidIndex(nodes[idx].ChildNodes, it, level + 1, out nodeAtIndex);
      }
      else
      {
        nodeAtIndex = nodes[idx];
        return true;
      }
    }

    /// <summary>
    /// Inserts the specified node at a certain index in the tree.
    /// </summary>
    /// <typeparam name="T">Type of node.</typeparam>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="index">The index inside the tree where the node should be inserted.</param>
    /// <param name="nodeToInsert">The node to insert.</param>
    public static void Insert<T>(this T rootNode, IEnumerable<int> index, T nodeToInsert) where T : ITreeListNode<T>
    {
      if (null == rootNode)
        throw new ArgumentNullException("rootNode");
      if (null == index)
        throw new ArgumentNullException("index");
      if (null == nodeToInsert)
        throw new ArgumentNullException("nodeToInsert");

      var parent = ElementAt(rootNode, index.TakeAllButLast());
      parent.ChildNodes.Insert(index.Last(), nodeToInsert);
    }

    /// <summary>
    /// Inserts the specified node after a certain index in the tree.
    /// </summary>
    /// <typeparam name="T">Type of node.</typeparam>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="index">The index inside the tree after which the node should be inserted.</param>
    /// <param name="nodeToInsert">The node to insert.</param>
    public static void InsertAfter<T>(this T rootNode, IEnumerable<int> index, T nodeToInsert) where T : ITreeListNode<T>
    {
      if (null == rootNode)
        throw new ArgumentNullException("rootNode");
      if (null == index)
        throw new ArgumentNullException("index");
      if (null == nodeToInsert)
        throw new ArgumentNullException("nodeToInsert");

      var parent = ElementAt(rootNode, index.TakeAllButLast());
      var idx = index.Last();
      parent.ChildNodes.Insert(idx + 1, nodeToInsert);
    }

    /// <summary>
    /// Inserts the specified node after all other siblings of the node at a certain index in the tree.
    /// </summary>
    /// <typeparam name="T">Type of node.</typeparam>
    /// <param name="rootNode">The root node of the tree.</param>
    /// <param name="index">The index inside the tree that points to a node. The <paramref name="nodeToInsert"/> is inserted at the end of the same collection that this node belongs to.</param>
    /// <param name="nodeToInsert">The node to insert.</param>
    public static void InsertLast<T>(this T rootNode, IEnumerable<int> index, T nodeToInsert) where T : ITreeListNode<T>
    {
      if (null == rootNode)
        throw new ArgumentNullException("rootNode");
      if (null == index)
        throw new ArgumentNullException("index");
      if (null == nodeToInsert)
        throw new ArgumentNullException("nodeToInsert");

      var parent = ElementAt(rootNode, index.TakeAllButLast());
      parent.ChildNodes.Add(nodeToInsert);
    }

    /// <summary>
    /// Gets the index of a given node inside a tree.
    /// </summary>
    /// <typeparam name="T">Type of the node.</typeparam>
    /// <param name="node">The node for which the index is determined.</param>
    /// <returns>Index of the node inside the tree.</returns>
    public static IList<int> IndexOf<T>(this T node) where T : ITreeListNodeWithParent<T>
    {
      if (null == node)
        throw new ArgumentException("node");
      var result = new List<int>();
      IndexOfInternal(node, result);
      return result;
    }

    /// <summary>
    /// Gets the index of a given node inside a tree.
    /// </summary>
    /// <typeparam name="T">Type of the node.</typeparam>
    /// <param name="node">The node for which the index is determined.</param>
    /// <param name="existingList">List that can be used to hold the indices. If this parameter is null, a new List will be created.</param>
    /// <returns>Index of the node inside the tree. If <paramref name="existingList"/> was not null, the  <paramref name="existingList"/> is returned. Otherwise, a new list is returned.</returns>
    public static IList<int> IndexOf<T>(this T node, IList<int> existingList) where T : ITreeListNodeWithParent<T>
    {
      if (existingList == null)
        existingList = new List<int>();
      IndexOfInternal(node, existingList);
      return existingList;
    }

    private static void IndexOfInternal<T>(this T node, IList<int> list) where T : ITreeListNodeWithParent<T>
    {
      if (node.ParentNode == null) // node is the root node of the tree
      {
        list.Clear();
      }
      else
      {
        IndexOfInternal(node.ParentNode, list);
        var childColl = node.ParentNode.ChildNodes;
        int idx = null == childColl ? -1 : childColl.IndexOf(node);
        if (idx < 0)
          throw new InvalidOperationException(string.Format("Tree node {0} is not contained in the Nodes collection of its parent", node));
        list.Add(idx);
      }
    }

    /// <summary>
    /// Fixes the and test the parent-child relationship in a tree.
    /// </summary>
    /// <typeparam name="T">Type of node of the tree.</typeparam>
    /// <param name="node">The node where the test starts (normally the root node of the tree).</param>
    /// <param name="Set1stArgParentNodeTo2ndArg">Action to set the Parent node property of a node given as the 1st argument to a node given as 2nd argument.</param>
    /// <returns>True if something changed (i.e. the parent-child relationship was broken), false otherwise.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// node is null
    /// or
    /// Set1stArgParentNodeTo2ndArg is null.
    /// </exception>
    public static bool FixAndTestParentChildRelations<T>(this T node, Action<T, T> Set1stArgParentNodeTo2ndArg) where T : ITreeListNodeWithParent<T>
    {
      if (null == node)
        throw new ArgumentNullException("node");
      if (null == Set1stArgParentNodeTo2ndArg)
        throw new ArgumentNullException("Set1stArgParentNodeTo2ndArg");
      return FixAndTestParentChildRelationsInternal(node, Set1stArgParentNodeTo2ndArg);
    }

    private static bool FixAndTestParentChildRelationsInternal<T>(this T node, Action<T, T> Set1stArgParentNodeTo2ndArg) where T : ITreeListNodeWithParent<T>
    {
      bool changed = false;
      if (null != node.ChildNodes)
      {
        foreach (var child in node.ChildNodes)
        {
          if (!object.ReferenceEquals(node, child.ParentNode))
          {
            Set1stArgParentNodeTo2ndArg(child, node);
            changed = true;
          }

          changed |= FixAndTestParentChildRelations(child, Set1stArgParentNodeTo2ndArg);
        }
      }
      return changed;
    }

    /// <summary>
    /// Enumerates through all tree nodes from (and including) the provided node <paramref name="node"/> down to the root node.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node to start the enumeration with.</param>
    /// <returns>All tree nodes from <paramref name="node"/> down to the root of the tree.</returns>
    public static IEnumerable<T> TakeFromHereToRoot<T>(this T node) where T : INodeWithParentNode<T>
    {
      if (null == node)
        throw new ArgumentNullException("node");

      yield return node;

      node = node.ParentNode;
      while (null != node)
      {
        yield return node;
        node = node.ParentNode;
      }
    }

    /// <summary>
    /// Enumerates through all tree nodes from the root node of the tree to (and including) the provided node <paramref name="node"/>.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node inside a tree that the enumeration ends with.</param>
    /// <returns>All tree nodes from the root node of the tree up to <paramref name="node"/>.</returns>
    /// <exception cref="System.ArgumentNullException">Node is null.</exception>
    public static IEnumerable<T> TakeFromRootToHere<T>(this T node) where T : INodeWithParentNode<T>
    {
      if (null == node)
        throw new ArgumentNullException("node");

      var list = new List<T>
      {
        node
      };
      node = node.ParentNode;
      while (null != node)
      {
        list.Add(node);
        node = node.ParentNode;
      }
      for (int i = list.Count - 1; i >= 0; --i)
        yield return list[i];
    }

    /// <summary>
    /// Gets the root node of a tree to which the given node <paramref name="node"/> belongs.
    /// </summary>
    /// <param name="node">The node to start with.</param>
    /// <returns>The root node of the tree to which the given node <paramref name="node"/> belongs.</returns>
    public static T RootNode<T>(this T node) where T : INodeWithParentNode<T>
    {
      if (null == node)
        throw new ArgumentNullException("node");

      var parent = node.ParentNode;
      while (null != parent)
      {
        node = parent;
        parent = node.ParentNode;
      }

      return node;
    }

    /// <summary>
    /// Determines the level of the specified node. The root node (= node that has no parent) will return a level of 0, the child nodes of the root node a level of 1 and so on.
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    /// <param name="node">The node for which the level is returned.</param>
    /// <returns>The node's level.</returns>
    /// <exception cref="System.ArgumentNullException">node is null.</exception>
    public static int Level<T>(this T node) where T : INodeWithParentNode<T>
    {
      if (null == node)
        throw new ArgumentNullException("node");

      var level = 0;
      var parent = node.ParentNode;
      while (null != parent)
      {
        ++level;
        node = parent;
        parent = node.ParentNode;
      }

      return level;
    }

    /// <summary>
    /// Determines whether all nodes in the provided enumeration have the same level (see <see cref="Level"/> for an explanation of level).
    /// </summary>
    /// <typeparam name="T">Type of tree node.</typeparam>
    /// <param name="selNodes">Enumeration of nodes</param>
    /// <returns><c>True</c> if all nodes have the same level; otherwise <c>false</c>. <c>False</c> is also returned if the provided enumeration was empty.</returns>
    public static bool AreAllNodesFromSameLevel<T>(this IEnumerable<T> selNodes) where T : INodeWithParentNode<T>
    {
      int? resultingLevel = null;
      foreach (var node in selNodes)
      {
        int nodeLevel = node.Level();

        if (null == resultingLevel)
          resultingLevel = nodeLevel;
        else if (resultingLevel.Value != nodeLevel)
          return false;
      }
      return null == resultingLevel ? false : true;
    }

    /// <summary>
    /// Returns the firsts ancestor of this node that has the type M.
    /// </summary>
    /// <typeparam name="M">The type to search for.</typeparam>
    /// <typeparam name="T">Type of the node.</typeparam>
    /// <param name="node">The node. The first node being considered is the parent node of this node.</param>
    /// <returns></returns>
    public static M FirstAncestorImplementing<M, T>(this T node)
        where T : INodeWithParentNode<T>
        where M : class
    {
      if (node.ParentNode != null)
      {
        foreach (var n in TakeFromHereToRoot(node.ParentNode))
        {
          var nodeAsM = n as M;
          if (null != nodeAsM)
            return nodeAsM;
        }
      }
      return null;
    }

    /// <summary>
    /// Determines whether a couple of nodes share the same parent node.
    /// </summary>
    /// <typeparam name="T">Type of node.</typeparam>
    /// <param name="nodes">The nodes.</param>
    /// <returns>True if all nodes in the enumeration share the same parent. An exception is thrown if the enumeration is empty or contains empty elements.</returns>
    public static bool HaveSameParent<T>(IEnumerable<T> nodes) where T : INodeWithParentNode<T>
    {
      T firstNode;
      try
      {
        firstNode = nodes.First();
      }
      catch (Exception ex)
      {
        throw new Exception("The enumeration was probably empty. Check the inner exception for details", ex);
      }

      var parent = firstNode.ParentNode;
      foreach (var node in nodes)
      {
        if (0 != Comparer<T>.Default.Compare(parent, node.ParentNode))
          return false;
      }

      return true;
    }

    #region Move TreeNodes up/down

    /// <summary>
    /// Frees this node, i.e. removes the node from it's parent collection.
    /// </summary>
    public static bool Remove<T>(this T node) where T : ITreeListNodeWithParent<T>
    {
      var parent = node.ParentNode;
      if (parent != null)
        return parent.ChildNodes.Remove(node);
      else
        return false;
    }

    /// <summary>
    /// Returns only the nodes with the highest hierarchy level among all the provided nodes (i.e. the nodes most close to the leaf nodes of the true).
    /// First, the <paramref name="nodes"/> collection is iterated through to determine the highest node level. Then only those nodes with the hightest node level are returned.
    /// </summary>
    /// <param name="nodes">Nodes to filter.</param>
    /// <returns>Only those nodes wich have the same highest level number among all the provided nodes.</returns>
    public static HashSet<T> NodesOfSameHighestLevel<T>(IEnumerable<T> nodes) where T : ITreeListNodeWithParent<T>
    {
      int level = int.MaxValue;
      foreach (var node in nodes)
        level = Math.Min(node.Level(), level);

      var hashSet = new HashSet<T>();
      foreach (var node in nodes)
        if (level == node.Level())
          hashSet.Add(node);

      return hashSet;
    }

    private static bool MoveNodesOneIndexDownwards<T>(IEnumerable<T> nodesToMove) where T : ITreeListNodeWithParent<T>
    {
      var hashOfSelectedNodes = (nodesToMove is HashSet<T>) ? (HashSet<T>)nodesToMove : new HashSet<T>(nodesToMove);
      if (0 == hashOfSelectedNodes.Count)
        return false; // nothing to move

      T parent = hashOfSelectedNodes.First().ParentNode;
      var childs = parent.ChildNodes;

      // we iterate through the list of child nodes

      int childsCount = childs.Count;
      for (int i = 0; i < childsCount; ++i)
      {
        if (hashOfSelectedNodes.Contains(childs[i]))
        {
          if (i != 0)
          {
            // swap index i and i-1
            var h = childs[i];
            childs[i] = childs[i - 1];
            childs[i - 1] = h;
          }
          else
          {
            return false; // if the first item is selected, we can't move downwards
          }
        }
      }

      return true;
    }

    private static bool MoveNodesOneIndexUpwards<T>(IEnumerable<T> nodesToMove) where T : ITreeListNodeWithParent<T>
    {
      var hashOfSelectedNodes = (nodesToMove is HashSet<T>) ? (HashSet<T>)nodesToMove : new HashSet<T>(nodesToMove);
      if (0 == hashOfSelectedNodes.Count)
        return false; // nothing to move

      T parent = hashOfSelectedNodes.First().ParentNode;
      var childs = parent.ChildNodes;

      int childsCountM1 = childs.Count - 1;
      for (int i = childsCountM1; i >= 0; --i)
      {
        if (hashOfSelectedNodes.Contains(childs[i]))
        {
          if (i != childsCountM1)
          {
            // swap index i and i+1
            var h = childs[i];
            childs[i] = childs[i + 1];
            childs[i + 1] = h;
          }
          else
          {
            return false; // if the last item is selected, we can't move down
          }
        }
      }
      return true;
    }

    /// <summary>
    /// This procedure will move nodes some indices up or down. All nodes to move should have the same parent.
    /// </summary>
    /// <param name="indexDelta">Number of movement steps. Value less than zero will move up the nodes in the tree, values greater null will move down the nodes in the tree.</param>
    /// <param name="nodesToMove">Nodes to move.</param>
    /// <returns>The number of indices the nodes were moved (either a positive number if the nodes where moved to higher indices, or negative if the nodes were moved to lower indices).</returns>
    /// <remarks>The following assumptions must be fullfilled:
    /// <para>The nodes have to have the same parent, otherwise an exception is thrown.</para>
    /// </remarks>
    public static int MoveNodesUpDown<T>(int indexDelta, IEnumerable<T> nodesToMove) where T : ITreeListNodeWithParent<T>
    {
      if (indexDelta == 0)
        return 0; // nothing moved
      if (null == nodesToMove)
        throw new ArgumentNullException("nodesToMove");

      var hashOfSelectedNodes = (nodesToMove is HashSet<T>) ? (HashSet<T>)nodesToMove : new HashSet<T>(nodesToMove);
      if (0 == hashOfSelectedNodes.Count)
        return 0; // nothing to move

      if (!HaveSameParent(hashOfSelectedNodes))
        throw new ArgumentException("The provided nodes do not have the same parent. This presumtion is neccessary for moving operations");

      int numberOfMoveSteps = 0;
      if (indexDelta < 0)
      {
        for (int i = 0; i < (-indexDelta); i++)
        {
          if (MoveNodesOneIndexDownwards(hashOfSelectedNodes))
            --numberOfMoveSteps;
          else
            break;
        }
      }
      else
      {
        for (int i = 0; i < indexDelta; i++)
        {
          if (MoveNodesOneIndexUpwards(hashOfSelectedNodes))
            ++numberOfMoveSteps;
          else
            break;
        }
      }

      return numberOfMoveSteps;
    }

    #endregion Move TreeNodes up/down

    #region Comparison of trees

    private static IEnumerable<T> GetEmptyEnumerable<T>()
    {
      yield break;
    }

    public static bool IsStructuralEquivalentTo<T, M>(this T tree1, M tree2, Func<T, M, bool> AreNodesEquivalent)
      where T : ITreeNode<T>
      where M : ITreeNode<M>
    {
      if (null == tree1)
        throw new ArgumentNullException("tree1");
      if (null == tree2)
        throw new ArgumentNullException("tree2");

      if (!AreNodesEquivalent(tree1, tree2))
        return false; // this nodes not equivalent

      if (null == tree1.ChildNodes && null == tree2.ChildNodes)
        return true; // both child node collections are null

      var enum1 = null != tree1.ChildNodes ? tree1.ChildNodes.GetEnumerator() : GetEmptyEnumerable<T>().GetEnumerator();
      var enum2 = null != tree2.ChildNodes ? tree2.ChildNodes.GetEnumerator() : GetEmptyEnumerable<M>().GetEnumerator();

      for (; ; )
      {
        var moved1 = enum1.MoveNext();
        var moved2 = enum2.MoveNext();

        if (!moved1 && !moved2)
          return true; // End of both enumerations
        if (moved1 ^ moved2)
          return false; // End of one enumeration

        if (!IsStructuralEquivalentTo(enum1.Current, enum2.Current, AreNodesEquivalent))
          return false; // substructures not equivalent
      }
    }

    #endregion Comparison of trees
  }
}
