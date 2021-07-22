#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;

namespace Altaxo.Main
{
  /// <summary>
  /// DocumentPath holds a path to a document. This path reflects the internal organization of the class instances in Altaxo. Do not mix this
  /// concept with the concept of the folder in which a project item virtually exists  (see <see cref="ProjectFolder"/>).
  /// </summary>
  [Serializable]
  public sealed class AbsoluteDocumentPath : System.ICloneable
  {
    private static readonly string[] _emptyStringArray = new string[0];
    public static readonly AbsoluteDocumentPath DocumentPathOfRootNode = new AbsoluteDocumentPath(new string[0]);
    private string[] _pathParts = _emptyStringArray;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Main.DocumentPath", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version not supported");
        /*
                AbsoluteDocumentPath s = (AbsoluteDocumentPath)obj;

                info.AddValue("IsAbsolute", s._IsAbsolutePath);

                info.CreateArray("Path", s.Count);
                for (int i = 0; i < s.Count; i++)
                    info.AddValue("e", s[i]);
                info.CommitArray();
                 */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var isAbsolutePath = info.GetBoolean("IsAbsolute");

        if (isAbsolutePath)
        {
          int count = info.OpenArray();
          var arr = new string[count];
          for (int i = 0; i < count; i++)
            arr[i] = info.GetString();
          info.CloseArray(count);
          return new AbsoluteDocumentPath(arr);
        }
        else
        {
          int numberBackwards = 0;
          var list = new List<string>();

          int count = info.OpenArray();
          for (int i = 0; i < count; i++)
          {
            var item = info.GetString();
            if (item == "..")
              numberBackwards++;
            else
              list.Add(item);
          }
          info.CloseArray(count);

          return new RelativeDocumentPath(numberBackwards, list);
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AbsoluteDocumentPath), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AbsoluteDocumentPath)obj;

        info.CreateArray("Path", s._pathParts.Length);
        for (int i = 0; i < s._pathParts.Length; i++)
          info.AddValue("e", s._pathParts[i]);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        int count = info.OpenArray();
        var arr = new string[count];
        for (int i = 0; i < count; i++)
          arr[i] = info.GetString();
        info.CloseArray(count);
        return new AbsoluteDocumentPath(arr);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Private constructor that takes an array of path elements as it is. Make sure that the array is not given outwards, so that it is not possible to modify it unintentionally.
    /// </summary>
    /// <param name="arr">The arr.</param>
    private AbsoluteDocumentPath(string[] arr)
    {
      _pathParts = arr;
    }

    public AbsoluteDocumentPath(IEnumerable<string> path)
    {
      _pathParts = path.ToArray();
    }

    public AbsoluteDocumentPath(AbsoluteDocumentPath from)
    {
      _pathParts = (string[])from._pathParts.Clone();
    }

    public int Count { get { return _pathParts.Length; } }

    public string this[int idx] { get { return _pathParts[idx]; } }

    public override string ToString()
    {
      var stringBuilder = new System.Text.StringBuilder(128);
      stringBuilder.Append("/");

      if (Count > 0)
        stringBuilder.Append(this[0]);

      for (int i = 1; i < Count; i++)
      {
        stringBuilder.Append("/");
        stringBuilder.Append(this[i]);
      }
      return stringBuilder.ToString();
    }

    public override bool Equals(object? obj)
    {
      if (!(obj is AbsoluteDocumentPath from))
        return false;
      if (Count != from.Count)
        return false;

      for (int i = Count - 1; i >= 0; --i)
      {
        if (!(this[i] == from[i]))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      return ToString().GetHashCode();
    }

    public bool StartsWith(AbsoluteDocumentPath another)
    {
      if (Count < another.Count)
        return false;

      for (int i = 0; i < another.Count; ++i)
        if (this[i] != another[i])
          return false;

      return true;
    }

    public AbsoluteDocumentPath SubPath(int start, int count)
    {
      if (!(start >= 0))
        throw new ArgumentOutOfRangeException("start should be >= 0");
      if (!(count >= 0))
        throw new ArgumentOutOfRangeException("count should be >= 0");

      return new AbsoluteDocumentPath(_pathParts.Skip(start).Take(count));
    }

    /// <summary>
    /// Gets the parent path of the current path. The parent path contains all parts except for the last part.
    /// </summary>
    /// <value>
    /// The parent path.
    /// </value>
    /// <exception cref="InvalidOperationException">Can not return parent path of root path</exception>
    public AbsoluteDocumentPath ParentPath
    {
      get
      {
        if (this.Count == 0)
          throw new InvalidOperationException("Can not return parent path of root path");

        return SubPath(0, Count - 1);
      }
    }

    /// <summary>
    /// Gets the last part, which is often the name of the object.
    /// </summary>
    /// <value>
    /// The last part.
    /// </value>
    public string LastPart
    {
      get
      {
        if (0 == Count)
          throw new InvalidOperationException("DocumentPath is empty");

        return this[Count - 1];
      }
    }

    /// <summary>
    /// Gets the last part, which is often the name of the object.
    /// </summary>
    /// <value>
    /// The last part.
    /// </value>
    public string? LastPartOrDefault
    {
      get
      {
        if (0 == Count)
          return null;

        return this[Count - 1];
      }
    }

    public AbsoluteDocumentPath Append(AbsoluteDocumentPath other)
    {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      var arr = new string[_pathParts.Length + other._pathParts.Length];
      Array.Copy(_pathParts, arr, _pathParts.Length);
      Array.Copy(other._pathParts, 0, arr, _pathParts.Length, other._pathParts.Length);

      return new AbsoluteDocumentPath(arr);
    }

    public AbsoluteDocumentPath Append(string other)
    {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      var arr = new string[_pathParts.Length + 1];
      Array.Copy(_pathParts, arr, _pathParts.Length);
      arr[_pathParts.Length] = other;

      return new AbsoluteDocumentPath(arr);
    }

    /// <summary>Replaces the last part of the <see cref="AbsoluteDocumentPath"/>, which is often the name of the object.</summary>
    /// <param name="lastPart">The new last part of the <see cref="AbsoluteDocumentPath"/>.</param>
    public AbsoluteDocumentPath ReplaceLastPart(string lastPart)
    {
      if (Count == 0)
        throw new InvalidOperationException("DocumentPath is empty, thus replacement of last part is not possible");
      if (string.IsNullOrEmpty(lastPart))
        throw new ArgumentOutOfRangeException("lastPart is null or empty");

      var arr = (string[])_pathParts.Clone();
      arr[arr.Length - 1] = lastPart;
      return new AbsoluteDocumentPath(arr);
    }

    /// <summary>
    /// Replaces parts of the part by another part.
    /// </summary>
    /// <param name="partToReplace">Part of the path that should be replaced. This part has to match the beginning of this part. The last item of the part
    /// is allowed to be given only partially.</param>
    /// <param name="newPart">The new part to replace that piece of the path, that match the <c>partToReplace</c>.</param>
    /// <param name="newPath">The resulting new path (if the return value is <c>true</c>), or the original path, if the return value is <c>false</c>.</param>
    /// <returns>True if the part could be replaced. Returns false if the path does not fulfill the presumtions given above.</returns>
    /// <remarks>
    /// As stated above, the last item of the partToReplace can be given only partially. As an example, the path (here separated by space)
    /// <para>Tables Preexperiment1/WDaten Time</para>
    /// <para>should be replaced by </para>
    /// <para>Tables Preexperiment2\WDaten Time</para>
    /// <para>To make this replacement, the partToReplace should be given by</para>
    /// <para>Tables Preexperiment1/</para>
    /// <para>and the newPart should be given by</para>
    /// <para>Tables Preexperiment2\</para>
    /// <para>Note that Preexperiment1\ and Preexperiment2\ are only partially defined items of the path.</para>
    /// </remarks>
    public bool ReplacePathParts(AbsoluteDocumentPath partToReplace, AbsoluteDocumentPath newPart, out AbsoluteDocumentPath newPath)
    {
      newPath = this;
      // Test if the start of my path is identical to that of partToReplace
      if (Count < partToReplace.Count)
        return false;

      for (int i = 0; i < partToReplace.Count - 1; i++)
        if (0 != string.Compare(this[i], partToReplace[i]))
          return false;

      if (!this[partToReplace.Count - 1].StartsWith(partToReplace[partToReplace.Count - 1]))
        return false;

      // ok, the beginning of my path and partToReplace is identical,
      // thus we replace the beginning of my path with that of newPart

      var list = new List<string>(_pathParts);
      string s = list[partToReplace.Count - 1].Substring(partToReplace[partToReplace.Count - 1].Length);
      list[partToReplace.Count - 1] = newPart[newPart.Count - 1] + s;

      int j, k;
      for (j = partToReplace.Count - 2, k = newPart.Count - 2; j >= 0 && k >= 0; --j, --k)
        list[j] = newPart[k];

      if (k > 0)
      {
        for (; k >= 0; --k)
          list.Insert(0, newPart[k]);
      }
      else if (j > 0)
      {
        for (; j >= 0; --j)
          list.RemoveAt(0);
      }

      newPath = new AbsoluteDocumentPath(list.ToArray());
      return true;
    }

    #region static navigation methods

    /// <summary>
    /// Get the root node of the node <code>node</code>. The root node is defined as last node in the hierarchie that
    /// either has not implemented the <see cref="IDocumentLeafNode"/> interface or has no parent.
    /// </summary>
    /// <param name="node">The node from where the search begins.</param>
    /// <returns>The root node, i.e. the last node in the hierarchie that
    /// either has not implemented the <see cref="IDocumentLeafNode"/> interface or has no parent</returns>
    public static IDocumentLeafNode GetRootNode(IDocumentLeafNode node)
    {
      if (node is null)
        throw new ArgumentNullException(nameof(node));

      int maxdepth = 65536;
      var parent = node.ParentObject;
      while (parent is not null)
      {
        node = parent;
        parent = node.ParentObject;
        if (--maxdepth == 0)
          throw new InvalidProgramException("Detected unusual deep hierarchy.");
      }

      return node;
    }

    /// <summary>
    /// Get the first parent node of the node <code>node</code> that implements the given type <code>type.</code>.
    /// </summary>
    /// <param name="node">The node from where the search begins.</param>
    /// <param name="type">The type to search for.</param>
    /// <returns>The first parental node that implements the type <code>type.</code>
    /// </returns>
    public static IDocumentLeafNode? GetRootNodeImplementing(IDocumentLeafNode? node, System.Type type)
    {
      if (node is null)
        return null;

      node = node.ParentObject;
      while (node is not null && !type.IsInstanceOfType(node))
      {
        node = node.ParentObject;
      }

      return type.IsInstanceOfType(node) ? node : null;
    }

    /// <summary>
    /// Get the first parent node of the node <code>node</code> that implements the given type <code>type.</code>.
    /// </summary>
    /// <typeparam name="T">The type to search for.</typeparam>
    /// <param name="node">The node from where the search begins.</param>
    /// <returns>The first parental node that implements the type <code>T</code>.</returns>
    [return: MaybeNull]
    public static T GetRootNodeImplementing<T>(IDocumentLeafNode? node)
    {
      if (node is null)
        return default;

      node = node.ParentObject;
      while (node is not null && !(node is T))
      {
        node = node.ParentObject;
      }

      return (node is T tnode) ? tnode : default;
    }

    /// <summary>
    /// Get the absolute path of the node <code>node</code> starting from the root.
    /// </summary>
    /// <param name="node">The node for which the path is retrieved.</param>
    /// <returns>The absolute path of the node. The first element is a "/" to mark this as absolute path.</returns>
    public static AbsoluteDocumentPath GetAbsolutePath(IDocumentLeafNode node)
    {
      AbsoluteDocumentPath path = GetPath(node, int.MaxValue);
      return path;
    }

    /// <summary>
    /// Get the absolute path of the node <code>node</code> starting from the root, or null if that fails.
    /// </summary>
    /// <param name="node">The node for which the path is retrieved.</param>
    /// <returns>The absolute path of the node, or null if failed to get the path. The first element is a "/" to mark this as absolute path.</returns>
    public static AbsoluteDocumentPath? TryGetAbsolutePath(IDocumentLeafNode node)
    {
      try
      {
        AbsoluteDocumentPath path = GetPath(node, int.MaxValue);
        return path;
      }
      catch(Exception)
      {
        return null;
      }
    }
    /// <summary>
    /// Get the absolute path of the node <code>node</code> starting either from the root, or from the object in the depth
    /// <code>maxDepth</code>, whatever is reached first.
    /// </summary>
    /// <param name="node">The node for which the path is to be retrieved.</param>
    /// <param name="maxDepth">The maximal hierarchie depth (the maximal number of path elements returned).</param>
    /// <returns>The path from the root or from the node in the depth <code>maxDepth</code>, whatever is reached first. The path is <b>not</b> prepended
    /// by a "/".
    /// </returns>
    public static AbsoluteDocumentPath GetPath(IDocumentLeafNode node, int maxDepth)
    {
      if (node is null)
        throw new ArgumentNullException(nameof(node));
      if (maxDepth <= 0)
        throw new ArgumentOutOfRangeException("maxDepth should be > 0");

      var list = new List<string>();

      int depth = 0;
      var parent = node.ParentObject;
      while (parent is not null)
      {
        if (depth >= maxDepth)
          break;

        var name = parent.GetNameOfChildObject(node);

        if (name is null) // an empty string is a valid name, e.g. for a folder text document located in the root folder
          throw new InvalidOperationException(string.Format("Parent node (type:{0}) of node (type: {1}) did not return a valid name for the child node!", parent.GetType(), node.GetType()));

        list.Add(name);
        node = parent;
        parent = node.ParentObject;
        ++depth;
      }

      if (maxDepth == int.MaxValue && node is not null && !(node is IProject))
      {
        string msg = string.Format("Document {0} is not rooted. The path so far retrieved is {1}", node, list);
        throw new InvalidOperationException(msg);
      }

      return new AbsoluteDocumentPath(list.TakeFromUpperIndexExclusiveDownToLowerIndexInclusive(list.Count, 0));
    }

    public static string GetPathString(IDocumentLeafNode node, int maxDepth)
    {
      return GetPath(node, maxDepth).ToString();
    }

    /// <summary>
    /// Resolves the path by first using startnode as starting point. If that failed, try using documentRoot as starting point.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="startnode">The node object which is considered as the starting point of the path.</param>
    /// <param name="documentRoot">An alternative node which is used as starting point of the path if the first try failed.</param>
    /// <returns>The resolved object. If the resolving process failed, the return value is null.</returns>
    public static IDocumentLeafNode? GetObject(AbsoluteDocumentPath path, IDocumentLeafNode startnode, IDocumentNode documentRoot)
    {
      var retval = GetObject(path, startnode);

      if (retval is null && documentRoot is not null)
        retval = GetObject(path, documentRoot);

      return retval;
    }

    public static IDocumentLeafNode? GetObject(AbsoluteDocumentPath path, IDocumentLeafNode startnode)
    {
      if (path is null)
        throw new ArgumentNullException(nameof(path));
      if (startnode is null)
        throw new ArgumentNullException(nameof(startnode));

      IDocumentLeafNode? node = GetRootNode(startnode);

      for (int i = 0; i < path.Count; i++)
      {
        if (path[i] == "..")
        {
          if (node is null)
            return null;
          else
            node = node.ParentObject;
        }
        else
        {
          if (node is Main.IDocumentNode docnode)
            node = docnode.GetChildObjectNamed(path[i]);
          else
            return null;
        }
      } // end for
      return node;
    }

    /// <summary>
    /// Gets the node that is designated by the provided <paramref name="path"/>  or the least resolveable node.
    /// </summary>
    /// <param name="path">The document path to resolve.</param>
    /// <param name="startnode">The startnode.</param>
    /// <param name="pathWasCompletelyResolved">If set to <c>true</c> on return, the path was completely resolved. Otherwise, <c>false</c>.</param>
    /// <returns>The resolved node, or the least node on the path that could be resolved.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// path
    /// or
    /// startnode
    /// </exception>
    public static IDocumentLeafNode GetNodeOrLeastResolveableNode(AbsoluteDocumentPath path, IDocumentLeafNode startnode, out bool pathWasCompletelyResolved)
    {
      if (path is null)
        throw new ArgumentNullException(nameof(path));
      if (startnode is null)
        throw new ArgumentNullException(nameof(startnode));

      var node = startnode;

      node = GetRootNode(node);
      if (node is null)
        throw new InvalidProgramException("startnote is not rooted");

      var prevNode = node;
      pathWasCompletelyResolved = true;

      for (int i = 0; i < path.Count; i++)
      {
        prevNode = node;
        if (path[i] == "..")
        {
          node = node.ParentObject;
        }
        else
        {
          if (node is Main.IDocumentNode docnode)
            node = docnode.GetChildObjectNamed(path[i]);
          else
            node = null;
        }

        if (node is null)
        {
          pathWasCompletelyResolved = false;
          break;
        }
      } // end for

      return node ?? prevNode;
    }

    #endregion static navigation methods

    #region ICloneable Members

    object System.ICloneable.Clone()
    {
      return new AbsoluteDocumentPath(this);
    }

    public AbsoluteDocumentPath Clone()
    {
      return new AbsoluteDocumentPath(this);
    }

    #endregion ICloneable Members
  }
}
